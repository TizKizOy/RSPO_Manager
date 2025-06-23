using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.Commands;
using MyKpiyapProject.ViewModels.UserControls.Task;
using MyKpiyapProject.Views.UserControls.Task;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyKpiyapProject.ViewModels.UserControls.Report;
using MyKpiyapProject.Views.UserControls.Report;
using System.Windows.Media.Animation;

namespace MyKpiyapProject.ViewModels.UserControls.Project
{

    public class ViewingProjectViewModel : INotifyPropertyChanged
    {
        private tbEmployee myUser;
        private tbProject _currentProj;
        private System.Action _refreshCallback;
        private ProjectService _projectService;
        private tbTask _selectedTask;

        private string _projectName;
        private string _projectDescription;
        private string _deadLine;
        private string _status;


        private bool _isLoading;
        private TaskService _taskService;
        private ObservableCollection<tbTask> _tasks;
        private ObservableCollection<tbTask> _allTasks;
        private UserControl _currentControl;
        private UserControl _secondControl;

        public event EventHandler<tbProject> CreateReportRequested;

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

        public ObservableCollection<tbTask> Tasks
        {
            get => _tasks;
            set
            {
                _tasks = value;
                OnPropertyChanged(nameof(Tasks));
            }
        }

        public ObservableCollection<tbTask> AllTasks
        {
            get => _allTasks;
            set
            {
                _allTasks = value;
                OnPropertyChanged(nameof(AllTasks));
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value; OnPropertyChanged();

                if (_status == "Закрыт" || _status == "Отменён")
                    ShowCreateReportDialog();
            }
        }

        public string Description
        {
            get { return _projectDescription; }
            set { _projectDescription = value; OnPropertyChanged(); }
        }
        public string ProjectName
        {
            get { return _projectName; }
            set { _projectName = value; OnPropertyChanged(); }
        }
        public string DeadLine
        {
            get { return _deadLine; }
            set { _deadLine = value; OnPropertyChanged(); }
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


        public ICommand LoadDataProjCommand { get; }
        public ICommand SaveDataProjCommand { get; }
        public ICommand LoadTaskCommand { get; }

        public ICommand LoadDataTaskCommand { get; }

        public ViewingProjectViewModel() { }
        public ViewingProjectViewModel(tbProject tbProj, System.Action action, tbEmployee employee)
        {
            myUser = employee;
            _currentProj = tbProj;
            _projectService = new ProjectService();
            _refreshCallback = action;
            _taskService = new TaskService();


            LoadDataProjCommand = new RelayCommand(_ => LoadDataProj());
            SaveDataProjCommand = new RelayCommand(_ => SaveProjData());
            LoadTaskCommand = new RelayCommand(async _ => await LoadTask(), _ => !_isLoading);
            LoadDataTaskCommand = new RelayCommand(_ => LoadDataTask());

            LoadDataProj();
            LoadTaskCommand.Execute(null);
        }

        private void ShowCreateReportDialog()
        {
            //MessageBoxResult result = MessageBox.Show(
            //    "Хотите создать отчёт для проекта?",
            //    "Создание отчёта",
            //    MessageBoxButton.YesNo,
            //    MessageBoxImage.Question
            //);

            //if (result == MessageBoxResult.Yes)
            //{
            //    // Вызываем событие, передавая текущий проект
            //    CreateReportRequested?.Invoke(this, _currentProj);
            //}
        }

        private async void RefreshData()
        {
            await LoadTask();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (_allTasks == null) return;

            Tasks = new ObservableCollection<tbTask>(_allTasks);
        }

        private async System.Threading.Tasks.Task LoadTask()
        {
            try
            {
                _isLoading = true;
                CommandManager.InvalidateRequerySuggested();

                var tasks = await System.Threading.Tasks.Task.Run(() =>
                    _taskService.GetAllTasks()
                        .Where(e => e.ProjectID == _currentProj.ProjectID)
                        .ToList());

                AllTasks = new ObservableCollection<tbTask>(tasks);
                ApplyFilters(); // Применяем фильтры после загрузки
            }
            finally
            {
                _isLoading = false;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void LoadDataTask()
        {
            if (SelectedTask != null)
            {
                var currentTask = SelectedTask;
                SecondControl = null; 
                SecondControl = new ViewingTask(currentTask, RefreshData, myUser)
                {
                    DataContext = new ViewingTaskControlViewModel(currentTask, RefreshData, myUser)
                };

                CurrentControl = null;
            }
        }

        private void LoadDataProj()
        {
            if (_currentProj == null) return;

            ProjectName = _currentProj.Title;
            Description = _currentProj.Description;
            Status = _currentProj.Status.StatusName;
            DeadLine = _currentProj.ClosingDate.ToString("dd.MM.yyyy") ?? "Не указан";
        }

        private void SaveProjData()
        {
            if (myUser.PositionAndRole.PositionAndRoleName != "Админ" && myUser.PositionAndRole.PositionAndRoleName != "Менеджер")
            {
                MessageBox.Show("Доступ запрещён", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            _currentProj.Status.StatusName = Status;
            try
            {
                _projectService.UpdateProject(_currentProj);
                _refreshCallback?.Invoke();
                MessageBox.Show("Проект успешно обновлеён!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении задачи: {ex.Message}");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
