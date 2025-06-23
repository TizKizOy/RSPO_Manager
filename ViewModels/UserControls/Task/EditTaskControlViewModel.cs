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
    public class EditTaskControlViewModel : INotifyPropertyChanged
    {
        private readonly tbEmployee _user;
        private readonly tbTask _task;
        private readonly Action _refreshCallback;
        private readonly TaskService _taskService;
        private readonly EmployeeService _userService;
        private readonly ProjectService _projectService;
        private readonly EmployeeService _employeerService;

        private AdminLogService _adminLogService;
        private LoggingService _loggingService;
        private PriorityTaskService _priorityTaskService;
        private StatusTaskService _statusTaskService;


        public ObservableCollection<tbEmployee> Executors { get; set; } = new ObservableCollection<tbEmployee>();
        public ObservableCollection<tbProject> Projects { get; set; } = new ObservableCollection<tbProject>();

        private string _nameTasks;
        private string _descriptionTasks;
        private tbProject _selectedProject;
        private tbEmployee _selectedExecutor;
        private string _priority;
        private string _status;
        private DateTime _deadLineDate;


        public DateTime DeadLineDate
        {
            get => _deadLineDate;
            set
            {
                _deadLineDate = value;
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

        public tbProject SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value;
                OnPropertyChanged();
            }
        }

        public tbEmployee SelectedExecutor
        {
            get => _selectedExecutor;
            set
            {
                _selectedExecutor = value;
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

        public ICommand SaveTaskCommand { get; }

        public EditTaskControlViewModel()
        {

        }

        public EditTaskControlViewModel(tbTask task, Action refreshData, tbEmployee tbEmployee, TaskService taskService, LoggingService loggingService)
        {
            _user = tbEmployee;
            _task = task;
            _refreshCallback = refreshData;
            _taskService = taskService;
            _userService = new EmployeeService();
            _projectService = new ProjectService();
            _employeerService = new EmployeeService();

            _adminLogService = new AdminLogService();
            _loggingService = loggingService;
            _priorityTaskService = new PriorityTaskService();
            _statusTaskService = new StatusTaskService();

            // Инициализация свойств из задачи
            NameTasks = task.Title;
            DescriptionTasks = task.Description;
            Priority = task.Priority.PriorityName;
            Status = task.Status.StatusName;
            DeadLineDate = task.DeadLineDate;

            SaveTaskCommand = new RelayCommand(_ => SaveTask());

            LoadExecutors();
            LoadProjects();
            LoadCurrentSelections();
        }


        public EditTaskControlViewModel(tbTask task, Action refreshData, tbEmployee tbEmployee)
        {
            _user = tbEmployee;
            _task = task;
            _refreshCallback = refreshData;
            _taskService = new TaskService();
            _userService = new EmployeeService();
            _projectService = new ProjectService();
            _employeerService = new EmployeeService();

            _adminLogService = new AdminLogService();
            _loggingService = new LoggingService();
            _priorityTaskService = new PriorityTaskService();
            _statusTaskService = new StatusTaskService();

            // Инициализация свойств из задачи
            NameTasks = task.Title;
            DescriptionTasks = task.Description;
            Priority = task.Priority.PriorityName;
            Status = task.Status.StatusName;
            DeadLineDate = task.DeadLineDate;

            SaveTaskCommand = new RelayCommand(_ => SaveTask());

            LoadExecutors();
            LoadProjects();
            LoadCurrentSelections();
        }

        public virtual void LoadCurrentSelections()
        {
            // Загрузка текущих значений проекта и исполнителя
            if (_task.ProjectID > 0)
            {
                SelectedProject = Projects.FirstOrDefault(p => p.ProjectID == _task.ProjectID);
            }

            if (_task.ExecutorID > 0)
            {
                SelectedExecutor = Executors.FirstOrDefault(e => e.EmployeeID == _task.ExecutorID);
            }
        }

        private void LoadExecutors()
        {
            var executors = _employeerService.GetAllEmployees();
            Executors.Clear();
            foreach (var creator in executors)
            {
                Executors.Add(creator);
            }
        }

        private void LoadProjects()
        {
            var projects = _projectService.GetAllProjects();
            Projects.Clear();
            foreach (var project in projects)
            {
                Projects.Add(project);
            }
        }

        public virtual async void SaveTask()
        {
            try
            {
                // Проверка на null для обязательных полей
                if (SelectedProject == null || SelectedExecutor == null)
                {
                    MessageBox.Show("Проект или исполнитель не выбраны.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                //private int projectID;
                //private int priorityID;
                //private int executorID;
                //private int creatorID;

                // Получаем необходимые идентификаторы
                int priorityId = _priorityTaskService.GetPriorityIdByName(Priority);
                int statusId = _statusTaskService.GetStatusIdByName(Status);

                // Проверка данных на корректность
                if (!ValidateTaskData(out string errorMessage))
                {
                    MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Обновляем текущую задачу
                var updatedTask = new tbTask
                {
                    TaskID = _task.TaskID,
                    CreationDate = DateTime.Now,
                    Title = NameTasks,
                    Description = DescriptionTasks,
                    ProjectID = SelectedProject.ProjectID,
                    ExecutorID = SelectedExecutor.EmployeeID,
                    PriorityID = priorityId,
                    CreatorID = _user.EmployeeID,
                    StatusID = statusId,
                    DeadLineDate = DeadLineDate
                };

                // Обновляем задачу через сервис
                _taskService.UpdateTask(updatedTask);

                // Обновляем интерфейс
                _refreshCallback?.Invoke();

                // Логируем успешное действие
                await _loggingService.LogAction(_user.EmployeeID, "Редактирование задачи", "Операции с данными", "Успех");
                MessageBox.Show("Задача успешно обновлена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                await _loggingService.LogAction(_user.EmployeeID, "Редактирование задачи", "Операции с данными", "Ошибка");
                MessageBox.Show($"Ошибка при сохранении задачи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateTaskData(out string errorMessage)
        {
            errorMessage = string.Empty;

            // Проверка на пустые обязательные поля
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
                errorMessage = "Проект не выбран.";
                return false;
            }

            if (SelectedExecutor == null)
            {
                errorMessage = "Исполнитель не выбран.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Priority))
            {
                errorMessage = "Приоритет не может быть пустым.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Status))
            {
                errorMessage = "Статус не может быть пустым.";
                return false;
            }

            if (DeadLineDate == default(DateTime))
            {
                errorMessage = "Срок выполнения не может быть пустым.";
                return false;
            }

            // Проверка на корректность даты (например, дата не может быть в прошлом)
            if (DeadLineDate < DateTime.Now)
            {
                errorMessage = "Срок выполнения не может быть в прошлом.";
                return false;
            }

            // Если все проверки пройдены успешно
            return true;
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}