using Microsoft.Office.Interop.Excel;
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
    public class AddReportViewModel : BaseReportViewModel
    {
        private ProjectService projectService;
        private ReportService reportService;
        private System.Action refreshCallback;

        private tbEmployee employee;

        private string _zatratHours;
        public string ZatratHours
        {
            get => _zatratHours;
            set { _zatratHours = value; OnPropertyChanged(); }
        }

        private tbProject _selectedProject;
        public tbProject SelectedProject
        {
            get => _selectedProject;
            set { _selectedProject = value; OnPropertyChanged(); }
        }

        private ObservableCollection<tbProject> _projects;
        public ObservableCollection<tbProject> Projects
        {
            get => _projects;
            set { _projects = value; OnPropertyChanged(); }
        }

        public AddReportViewModel()
        {
            SaveReportCommand = new RelayCommand((_) => SaveReport());
            LoadProjects();
        }

        public AddReportViewModel(tbEmployee tbEmployee, System.Action oldAction)
        {
            employee = tbEmployee;
            refreshCallback = oldAction;
            SaveReportCommand = new RelayCommand((_) => SaveReport());
            LoadProjects();
        }

        private void LoadProjects()
        {
            projectService = new ProjectService();
            reportService = new ReportService();
            Projects = new ObservableCollection<tbProject>(projectService.GetAllProjects());
        }

        private void SaveReport()
        {
            if (!ValidateReport(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(Title))
            {
                MessageBox.Show("Введите название отчёта!");
                return;
            }

            // Сохранение в БД
            var newReport = new tbReport
            {
                Title = Title,
                FinishDate = EndDate ,
                Description = Description,
                Status = Status,
                ProjectID = SelectedProject?.ProjectID ?? 0,
                WorkExpenses = ZatratHours,
                CreationDate = DateTime.Now,
                EmployeeID = employee.EmployeeID,
                CountEndTask = 0,
                CountNotEndProject = 0,
                CountEndProject = 0
            };

            reportService.AddReport(newReport);
            refreshCallback?.Invoke();
            MessageBox.Show("Отчёт по проекту сохранён!");
        }
    }
}
