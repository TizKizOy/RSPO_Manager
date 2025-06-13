using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MyKpiyapProject.ViewModels.UserControls.Task
{
    public class AddTaskControlViewModel : INotifyPropertyChanged
    {
        private readonly tbEmployee _user;
        private readonly Action _refreshCallback;
        private readonly TaskService _taskService;
        private readonly EmployeeService _userService;
        private readonly ProjectService _projectService;

        private AdminLogService _adminLogService;
        private LoggingService _loggingService;
        private StatusTaskService _statusTaskService;
        private PriorityTaskService _priorityTaskService;

        public ObservableCollection<tbEmployee> Executors { get; } = new ObservableCollection<tbEmployee>();
        public ObservableCollection<tbProject> Projects { get; } = new ObservableCollection<tbProject>();

        private string _nameTasks;
        private string _descriptionTasks;
        private tbProject _selectedProject;
        private int _projectId;
        private string _projectName;
        private tbEmployee _selectedExecutor;
        private int _executorId;
        private string _executorName;
        private int _createrId;
        private string _createrName;
        private string _priority = "Средний";
        private string _status = "Открыт";
        private DateTime _deadLineDate = DateTime.Today;


        public DateTime DeadLineDate
        {
            get => _deadLineDate;
            set
            {
                _deadLineDate = value;
                OnPropertyChanged();
            }
        }
        public tbProject SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value;
                _projectId = value.ProjectID;
                _projectName = value.Title;
                OnPropertyChanged();
            }
        }
        public string NameTasks
        {
            get => _nameTasks;
            set
            {
                _nameTasks = value;
                OnPropertyChanged();
            }
        }
        public string DescriptionTasks
        {
            get => _descriptionTasks;
            set
            {
                _descriptionTasks = value;
                OnPropertyChanged();
            }
        }

        public string ProjectName
        {
            get => _projectName;
            set
            {
                _projectName = value;
                OnPropertyChanged();
            }
        }
        public tbEmployee SelectedExecutor
        {
            get => _selectedExecutor;
            set
            {
                _selectedExecutor = value;
                _executorId = value?.EmployeeID ?? 0;
                _executorName = value?.FullName;
                OnPropertyChanged();
            }
        }
        public string ExecutorName
        {
            get => _executorName;
            set
            {
                _executorName = value;
                OnPropertyChanged();
            }
        }
        public int CreaterId
        {
            get => _createrId;
            set
            {
                _createrId = value;
                OnPropertyChanged();
            }
        }
        public string CreaterName
        {
            get => _createrName;
            set
            {
                _createrName = value;
                OnPropertyChanged();
            }
        }
        public string Priority
        {
            get => _priority;
            set
            {
                _priority = value;
                OnPropertyChanged();
            }
        }
        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }
        public tbEmployee Creator
        {
            get => _user;
            set
            {
                _createrId = value?.EmployeeID ?? 0;
                _createrName = value?.FullName;
                OnPropertyChanged();
            }
        }

        public ICommand SaveTaskCommand { get; }

        public AddTaskControlViewModel(Action refreshData, tbEmployee user, TaskService taskService, EmployeeService userService, ProjectService projectService, LoggingService loggingService)
        {
            _user = user;
            _createrId = user.EmployeeID;
            _createrName = user.FullName;
            _refreshCallback = refreshData;
            _taskService = taskService;
            _userService = userService;
            _projectService = projectService;


            _adminLogService = new AdminLogService();
            _loggingService = loggingService;

            SaveTaskCommand = new RelayCommand(SaveTask, CanSaveTask);

            LoadExecutors();
            LoadProjects();
        }

        public AddTaskControlViewModel(Action refreshData, tbEmployee user)
        {
            _user = user;
            _createrId = user.EmployeeID;
            _createrName = user.FullName;
            _refreshCallback = refreshData;
            _taskService = new TaskService();
            _userService = new EmployeeService();
            _projectService = new ProjectService();
            _statusTaskService = new StatusTaskService();
            _priorityTaskService = new PriorityTaskService();

            _adminLogService = new AdminLogService();
            _loggingService = new LoggingService();

            SaveTaskCommand = new RelayCommand(SaveTask, CanSaveTask);

            LoadExecutors();
            LoadProjects();
        }

        public bool CanSaveTask(object parameter)
        {
            return !string.IsNullOrWhiteSpace(NameTasks) &&
                   !string.IsNullOrWhiteSpace(ProjectName) &&
                   !string.IsNullOrWhiteSpace(Status) &&
                   !string.IsNullOrWhiteSpace(Priority) &&
                    _selectedExecutor != null;
        }

        public bool ValidateTask(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(NameTasks))
            {
                errorMessage = "Название задачи не может быть пустым.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(DescriptionTasks))
            {
                errorMessage = "Описание задачи не может быть пустым.";
                return false;
            }

            if (SelectedProject == null)
            {
                errorMessage = "Проект должен быть выбран.";
                return false;
            }

            if (SelectedExecutor == null)
            {
                errorMessage = "Исполнитель должен быть выбран.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Priority))
            {
                errorMessage = "Приоритет задачи не может быть пустым.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Status))
            {
                errorMessage = "Статус задачи не может быть пустым.";
                return false;
            }

            if (DeadLineDate < DateTime.Now)
            {
                errorMessage = "Срок выполнения не может быть раньше текущей даты.";
                return false;
            }

            return true;
        }

        private void LoadExecutors()
        {
            var executors = _userService.GetAllEmployees();
            Executors.Clear();
            foreach (var creator in executors)
            {
                Executors.Add(creator);
            }
        }
        private void LoadProjects()
        {
            var project = _projectService.GetAllProjects();
            Projects.Clear();
            foreach (var creator in project)
            {
                Projects.Add(creator);
            }
        }

        public virtual async void SaveTask(object parameter)
        {
            int statusId = _statusTaskService.GetStatusIdByName(Status);
            int priorityId = _priorityTaskService.GetPriorityIdByName(Priority);

            if (!ValidateTask(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newTask = new tbTask
            {
                Title = NameTasks,
                Description = DescriptionTasks,
                ExecutorID = SelectedExecutor.EmployeeID, 
                ProjectID = SelectedProject.ProjectID, 
                CreatorID = _user.EmployeeID,
                PriorityID = priorityId,
                StatusID = statusId,
                CreationDate = DateTime.Now,
                DeadLineDate = DeadLineDate
            };

            try
            {
                _taskService.AddTask(newTask);

                // Обновить список задач в проекте
                if (SelectedProject.Tasks == null)
                {
                    SelectedProject.Tasks = new ObservableCollection<tbTask>();
                }
                SelectedProject.Tasks.Add(newTask);

                _refreshCallback?.Invoke();

                await _loggingService.LogAction(_user.EmployeeID, "Добавление задачи", "Операции с данными", "Успех");

                MessageBox.Show("Задача успешно добавлена к проекту");
            }
            catch (Exception ex)
            {
                await _loggingService.LogAction(_user.EmployeeID, "Добавление задачи", "Операции с данными", "Ошибка");
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}