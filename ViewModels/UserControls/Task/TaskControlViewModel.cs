using System.Windows.Input;
using MyKpiyapProject.Services;
using System.Windows.Controls;
using MyKpiyapProject.ViewModels.Commands;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MyKpiyapProject.Views.UserControls.Task;
using MyKpiyapProject.NewModels;

namespace MyKpiyapProject.ViewModels.UserControls.Task
{
    public class TaskControlViewModel : INotifyPropertyChanged
    {
        private tbEmployee _myUser;
        private ObservableCollection<tbTask> _tasks;
        private ObservableCollection<tbTask> _allTasks;
        private TaskService _taskService;
        private UserControl _currentControl;
        private UserControl _secondControl;
        private bool _isLoading;
        private tbTask _selectedTask;

        private AdminLogService _adminLogService;
        private LoggingService _loggingService;

        private string _currentFilterPriority = "Все";
        private string _selectedPriority = "Все";
        private string _searchText;
        private string _taskCountText;


        public ObservableCollection<tbTask> Tasks
        {
            get => _tasks;
            set
            {
                _tasks = value;
                OnPropertyChanged(nameof(Tasks));
            }
        }
        public TaskService TaskService
        {
            get => _taskService;
            set
            {
                _taskService = value;
                OnPropertyChanged(nameof(Tasks));
            }
        }
        public UserControl SecondControl
        {
            get { return _secondControl; }
            set
            {
                _secondControl = value;
                OnPropertyChanged(nameof(SecondControl));
            }
        }
        public UserControl CurrentControl
        {
            get { return _currentControl; }
            set
            {
                _currentControl = value;
                OnPropertyChanged();
            }
        }
        public tbTask SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested(); 
            }
        }
        public string CurrentFilterPriority
        {
            get => _currentFilterPriority;
            set
            {
                _currentFilterPriority = value;
                OnPropertyChanged();
            }
        }
        public string SelectedPriority
        {
            get=> _selectedPriority;
            set
            {
                _selectedPriority = value;
                OnPropertyChanged();
            }
        }
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }
        public string TaskCountText
        {
            get=> _taskCountText;
            set
            {
                _taskCountText = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadAddTaskControlCommand { get; }
        public ICommand LoadEditTaskControlCommand { get; }
        public ICommand LoadDataTaskCommand { get; }
        public ICommand LoadTaskCommand { get; }
        public ICommand RemoveTaskCommand { get; }
        public ICommand FilterByPriorityCommand { get; }
        public ICommand LoadMyTaskCommand { get; }



        public TaskControlViewModel()
        {

        }

        public TaskControlViewModel(tbEmployee user, TaskService taskService, LoggingService loggingService)
        {
            _myUser = user;
            TaskService = taskService;
            Tasks = new ObservableCollection<tbTask>();

            _adminLogService = new AdminLogService();
            _loggingService = loggingService;

            LoadAddTaskControlCommand = new RelayCommand(_ => LoadAddTaskControl());
            LoadEditTaskControlCommand = new RelayCommand(_ => LoadEditTaskControl(), _ => SelectedTask != null);
            LoadTaskCommand = new RelayCommand(async _ => await LoadTask(), _ => !_isLoading);
            RemoveTaskCommand = new RelayCommand(_ => RemoveTask(), _ => SelectedTask != null);
            FilterByPriorityCommand = new RelayCommand(FilterByPriority);
            LoadDataTaskCommand = new RelayCommand(_ => LoadDataTask());
            LoadMyTaskCommand = new RelayCommand(_ => LoadMyTask());

            LoadTaskCommand.Execute(null);
        }

        public TaskControlViewModel(tbEmployee user)
        {
            _myUser = user;
            TaskService = new TaskService();
            Tasks = new ObservableCollection<tbTask>();

            _adminLogService = new AdminLogService();
            _loggingService = new LoggingService();

            LoadAddTaskControlCommand = new RelayCommand(_ => LoadAddTaskControl());
            LoadEditTaskControlCommand = new RelayCommand(_ => LoadEditTaskControl(), _ => SelectedTask != null);
            LoadTaskCommand = new RelayCommand(async _ => await LoadTask(), _ => !_isLoading);
            RemoveTaskCommand = new RelayCommand(_ => RemoveTask(), _ =>  SelectedTask != null);
            FilterByPriorityCommand = new RelayCommand(FilterByPriority);
            LoadDataTaskCommand = new RelayCommand(_ => LoadDataTask());
            LoadMyTaskCommand = new RelayCommand(_ => LoadMyTask());

            LoadTaskCommand.Execute(null);
        }

        private async System.Threading.Tasks.Task LoadMyTask()
        {
            try
            {
                var tasks = await System.Threading.Tasks.Task.Run(() => _taskService.GetAllTasks().Where(e => e.ExecutorID == _myUser.EmployeeID));
                _allTasks = new ObservableCollection<tbTask>(tasks);
                ApplyFilters();
            }
            finally
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public virtual async System.Threading.Tasks.Task LoadTask()
        {
            try
            {
                _isLoading = true;
                CommandManager.InvalidateRequerySuggested();
                var tasks = await System.Threading.Tasks.Task.Run(() => _taskService.GetAllTasks());
                _allTasks = new ObservableCollection<tbTask>(tasks);
                ApplyFilters();
                await _loggingService.LogAction(_myUser.EmployeeID, "Загрузка задач", "Операции с данными", "Успех");
            }
            catch
            {
                _isLoading = false;
                CommandManager.InvalidateRequerySuggested();
                await _loggingService.LogAction(_myUser.EmployeeID, "Загрузка задач", "Операции с данными", "Ошибка");
            }
        }

        private void LoadDataTask()
        {
            if (SelectedTask != null)
            {
                var currentTask = SelectedTask; // Сохраняем выбранную задачу
                SecondControl = null; // Сначала очищаем
                SecondControl = new ViewingTask(currentTask, RefreshData, _myUser)
                {
                    DataContext = new ViewingTaskControlViewModel(currentTask, RefreshData, _myUser)
                };

                CurrentControl = null;
            }
        }

        private void LoadAddTaskControl()
        {
            if (_myUser.PositionAndRole.PositionAndRoleName != "Админ" &&
                _myUser.PositionAndRole.PositionAndRoleName != "Менеджер" &&
                _myUser.PositionAndRole.PositionAndRoleName != "Бригадир" &&
                _myUser.PositionAndRole.PositionAndRoleName != "Инженер")
            {
                MessageBox.Show("Доступ запрещён", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            CurrentControl = new AddTaskControl(RefreshData, _myUser)
            {
                DataContext = new AddTaskControlViewModel(RefreshData, _myUser)
            };
        }
        private async void RemoveTask()
        {
            if (_myUser.PositionAndRole.PositionAndRoleName != "Админ" &&
                _myUser.PositionAndRole.PositionAndRoleName != "Менеджер" &&
                _myUser.PositionAndRole.PositionAndRoleName != "Бригадир" &&
                _myUser.PositionAndRole.PositionAndRoleName != "Инженер")
            {
                MessageBox.Show("Доступ запрещён", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                if (MessageBox.Show($"Удалить задачу {SelectedTask.Title}?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _taskService.DeleteTask(SelectedTask.TaskID);
                    RefreshData();
                    await _loggingService.LogAction(_myUser.EmployeeID, "Удвление задачи", "Операции с данными", "Успех");
                }
            }
            catch
            {
                await _loggingService.LogAction(_myUser.EmployeeID, "Удвление задачи", "Операции с данными", "Ошибка");
            }
        }
        private void LoadEditTaskControl()
        {
            if (_myUser.PositionAndRole.PositionAndRoleName != "Админ" &&
                 _myUser.PositionAndRole.PositionAndRoleName != "Менеджер" &&
                 _myUser.PositionAndRole.PositionAndRoleName != "Бригадир" &&
                 _myUser.PositionAndRole.PositionAndRoleName != "Инженер")
            {
                MessageBox.Show("Доступ запрещён", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            CurrentControl = new EditTaskControl(SelectedTask, RefreshData, _myUser)
            {
                DataContext = new EditTaskControlViewModel(SelectedTask, RefreshData, _myUser)
            };
        }
        private async void RefreshData()
        {
            await LoadTask();
            ApplyFilters();
        }
        private void ApplyFilters()
        {
            if (_allTasks == null) return;

            IEnumerable<tbTask> filtered = _allTasks;

            if (!string.IsNullOrEmpty(_selectedPriority) && _selectedPriority != "Все")
            {
                filtered = filtered.Where(e => e.Priority.PriorityName == _selectedPriority);
            }

            if (!string.IsNullOrEmpty(SearchText))
            {
                var searchTextLower = SearchText.ToLower();
                filtered = filtered.Where(e =>
                    e.Title.ToLower().Contains(searchTextLower) ||
                    e.Status.StatusName.ToLower().Contains(searchTextLower) ||
                    e.CreatorName.ToLower().Contains(searchTextLower) ||
                    e.ExecutorName.ToLower().Contains(searchTextLower) ||
                    e.NameProject.ToLower().Contains(searchTextLower) ||
                    e.Priority.PriorityName.ToLower().Contains(searchTextLower));
            }

            Tasks = new ObservableCollection<tbTask>(filtered);
            UpdateRecordCount();
        }
        private void UpdateRecordCount()
        {
            int count = Tasks.Count;
            string ending = count % 100 is >= 11 and <= 14 ? "Задач" :
                (count % 10) switch
                {
                    1 => "Задача",
                    2 or 3 or 4 => "Задачи",
                    _ => "Задач"
                };

            TaskCountText = $"{count} {ending}";
        }
        public virtual void FilterByPriority(object priority)
        {
            _currentFilterPriority = priority?.ToString() ?? "Все";
            SelectedPriority = _currentFilterPriority;
            ApplyFilters();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
