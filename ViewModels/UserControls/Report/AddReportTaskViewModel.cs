using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyKpiyapProject.ViewModels.UserControls.Report
{
    public class AddReportTaskViewModel : BaseReportViewModel
    {
        private ProjectService projectService;
        private ReportService reportService;
        private TaskService taskService;
        private tbEmployee employee;
        private string _zatratHours;
        private System.Action refreshCallback;
        public string ZatratHours
        {
            get => _zatratHours;
            set { _zatratHours = value; OnPropertyChanged(); }
        }

        private tbProject _selectedProject;
        public tbProject SelectedProject
        {
            get => _selectedProject;
            set { _selectedProject = value; OnPropertyChanged(); LoadTasks(); }
        }

        private tbTask _selectedTask;
        public tbTask SelectedTask
        {
            get => _selectedTask;
            set { _selectedTask = value; OnPropertyChanged(); }
        }

        private ObservableCollection<tbProject> _projects;
        public ObservableCollection<tbProject> Projects
        {
            get => _projects;
            set { _projects = value; OnPropertyChanged(); }
        }

        private ObservableCollection<tbTask> _tasks;
        public ObservableCollection<tbTask> Tasks
        {
            get => _tasks;
            set { _tasks = value; OnPropertyChanged(); }
        }

        public AddReportTaskViewModel()
        {
            projectService = new ProjectService();
            reportService = new ReportService();
            taskService = new TaskService();
            SaveReportCommand = new RelayCommand(_ => SaveReport());
            LoadProjects();
        }

        public AddReportTaskViewModel(tbEmployee tbEmployee, System.Action action)
        {
            employee = tbEmployee;
            refreshCallback = action;
            projectService = new ProjectService();
            reportService = new ReportService();
            taskService = new TaskService();
            SaveReportCommand = new RelayCommand(_ => SaveReport());
            LoadProjects();
        }

        private void LoadProjects()
        {
            Projects = new ObservableCollection<tbProject>(projectService.GetAllProjects());
        }

        private void LoadTasks()
        {
            if (SelectedProject == null) return;
            Tasks = new ObservableCollection<tbTask>(taskService.GetTasksByProject(SelectedProject.ProjectID));
        }

        private void SaveReport()
        {
            if (!ValidateReport(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SelectedTask == null)
            {
                MessageBox.Show("Выберите задачу!");
                return;
            }

            var newReport = new tbReport
            {
                Title = Title,
                Description = Description,
                FinishDate = EndDate,
                Status = Status,
                TaskID = SelectedTask.TaskID,
                ProjectID = SelectedProject.ProjectID,
                WorkExpenses = ZatratHours,
                CreationDate = DateTime.Now,
                EmployeeID = employee.EmployeeID,
                CountEndTask = 0,
                CountNotEndProject = 0,
                CountEndProject = 0
            };

            reportService.AddReport(newReport);
            refreshCallback?.Invoke();
            MessageBox.Show("Отчёт по задаче сохранён!");
        }
    }
}
