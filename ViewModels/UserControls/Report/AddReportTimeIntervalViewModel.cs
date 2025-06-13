using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyKpiyapProject.ViewModels.UserControls.Report
{
    public class AddReportTimeIntervalViewModel : BaseReportViewModel
    {
        private ReportService reportService;
        private tbEmployee employee;
        private string _kolProj;
        private System.Action refreshCallback;

        public string KolProj
        {
            get => _kolProj;
            set { _kolProj = value; OnPropertyChanged(); }
        }

        private string _kolTask;
        public string KolTask
        {
            get => _kolTask;
            set { _kolTask = value; OnPropertyChanged(); }
        }

        private string _kolProsTask;
        public string KolProsTask
        {
            get => _kolProsTask;
            set { _kolProsTask = value; OnPropertyChanged(); }
        }

        private string _zatratHours;
        public string ZatratHours
        {
            get => _zatratHours;
            set { _zatratHours = value; OnPropertyChanged(); }
        }

        private DateTime creatingDate = DateTime.Now.AddDays(-30);
        public DateTime CreatingDate
        {
            get => creatingDate;
            set { creatingDate = value; OnPropertyChanged(); }
        }

        private DateTime finishDate = DateTime.Now;
        public DateTime FinishDate
        {
            get => finishDate;
            set { finishDate = value; OnPropertyChanged(); }
        }

        public AddReportTimeIntervalViewModel()
        {
            reportService = new ReportService();
            SaveReportCommand = new RelayCommand(_ => SaveReport());
        }

        public AddReportTimeIntervalViewModel(tbEmployee tbEmployee, Action refreshCallback)
        {
            employee = tbEmployee;
            reportService = new ReportService();
            SaveReportCommand = new RelayCommand(_ => SaveReport());
            this.refreshCallback = refreshCallback;
        }

        private void SaveReport()
        {
            if (!ValidateReport(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(KolProj) || string.IsNullOrEmpty(KolTask))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            var newReport = new tbReport
            {
                Title = Title,
                Description = Description,
                Status = Status,
                CountEndProject = int.Parse(KolProj),
                CountEndTask = int.Parse(KolTask),
                CountNotEndProject = int.Parse(KolProsTask),
                WorkExpenses = ZatratHours,
                CreationDate = CreatingDate,
                FinishDate = FinishDate,
                EmployeeID =  employee.EmployeeID,
            };

            reportService.AddReport(newReport);
            refreshCallback?.Invoke();
            MessageBox.Show("Отчёт за период сохранён!");
        }
    }
}
