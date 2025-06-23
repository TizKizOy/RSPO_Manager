using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MyKpiyapProject.ViewModels.Commands;


namespace MyKpiyapProject.ViewModels.UserControls.Report
{
    public abstract class BaseReportViewModel : INotifyPropertyChanged
    {
        // Общие свойства для всех форм
        private string _title;
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        private string _status = "Выполнено";
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        private DateTime _endDate = DateTime.Now;
        public DateTime EndDate
        {
            get => _endDate;
            set { _endDate = value; OnPropertyChanged(); }
        }

        public bool ValidateReport(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Title))
            {
                errorMessage = "Название отчета не может быть пустым.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Description))
            {
                errorMessage = "Описание отчета не может быть пустым.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Status))
            {
                errorMessage = "Статус отчета не может быть пустым.";
                return false;
            }

            if (EndDate > DateTime.Now)
            {
                errorMessage = "Дата окончания не может быть в будущем.";
                return false;
            }

            return true;
        }


        // Общая команда сохранения (может быть переопределена)
        public ICommand SaveReportCommand { get; protected set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
