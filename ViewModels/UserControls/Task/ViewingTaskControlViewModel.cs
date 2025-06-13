using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MyKpiyapProject.ViewModels.UserControls.Task
{
    class ViewingTaskControlViewModel : INotifyPropertyChanged
    {
        private tbEmployee myUser;
        private tbTask _currentTask;
        private Action _refreshCallback;
        private string _deadline;
        private string _taskName;
        private string _priority;
        private string _status;
        private string _taskDescription;
        private tbEmployee _creator;
        private string _creatorName;
        private tbEmployee _executor;
        private string _executorName;
        private TaskService _taskService;

        public string CreatorName
        {
            get => _creatorName;
            set { _creatorName = value; }
        }
        public string ExecutorName
        {
            get => _executorName;
            set { _executorName = value; }
        }
        public string Deadline
        {
            get => _deadline;
            set { _deadline = value; OnPropertyChanged(); }
        }
        public string TaskName
        {
            get => _taskName;
            set { _taskName = value; OnPropertyChanged(); }
        }
        public string Priority
        {
            get => _priority;
            set { _priority = value; OnPropertyChanged(); }
        }
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }
        public string TaskDescription
        {
            get => _taskDescription;
            set { _taskDescription = value; OnPropertyChanged(); }
        }
        public tbEmployee Creator
        {
            get => _creator;
            set { _creator = value; OnPropertyChanged(); }
        }
        public tbEmployee Executor
        {
            get => _executor;
            set { _executor = value; OnPropertyChanged(); }
        }

        public ICommand LoadDataTaskCommand { get; }
        public ICommand SaveDataTaskCommand { get; }

        public ViewingTaskControlViewModel() { }

        public ViewingTaskControlViewModel(tbTask tbTask, Action action, tbEmployee employee)
        {
            myUser = employee;
            _currentTask = tbTask;
            _taskService = new TaskService();
            _refreshCallback = action;
            LoadDataTaskCommand = new RelayCommand(_ => LoadDataTask());
            SaveDataTaskCommand = new RelayCommand(_ => SaveTaskData());
            LoadDataTask();
        }

        private void LoadDataTask()
        {
            if (_currentTask == null) return;

            TaskName = _currentTask.Title;
            Status = _currentTask.Status.StatusName;
            Priority = _currentTask.Priority?.ToString() ?? "Не указан";
            TaskDescription = _currentTask.Description;
            CreatorName = _currentTask.Creator?.FullName ?? "Не указан";
            ExecutorName = _currentTask.Executor?.FullName ?? "Не указан";
            Deadline = _currentTask.DeadLineDate.ToString("dd.MM.yyyy") ?? "Не указан";
        }

        private void SaveTaskData()
        {
            if (myUser.PositionAndRole.PositionAndRoleName != "Админ" &&
                myUser.PositionAndRole.PositionAndRoleName != "Менеджер" &&
                myUser.PositionAndRole.PositionAndRoleName != "Бригадир" &&
                myUser.PositionAndRole.PositionAndRoleName != "Инженер")
            {
                MessageBox.Show("Доступ запрещён", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            _currentTask.Status.StatusName = Status;
            try
            {
                _taskService.UpdateTask(_currentTask);
                _refreshCallback?.Invoke();
                MessageBox.Show("Задача успешно обновлена!");
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
