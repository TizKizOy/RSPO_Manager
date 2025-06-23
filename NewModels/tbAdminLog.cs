using MyKpiyapProject.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace MyKpiyapProject.NewModels
{
    public class tbAdminLog : INotifyPropertyChanged
    {
        private int logID;
        private int? employeeID;
        private string action;
        private string eventType;
        private DateTime dateTime;
        private string strDateTime;
        private string status;
        private int rowNumber;

        private EmployeeService employeeService = new EmployeeService();

        [NotMapped]
        public int RowNumber
        {
            get { return rowNumber; }
            set { rowNumber = value; OnPropertyChanged(); }
        }
        [NotMapped]
        public string EmployeeName
        {
            get
            {
                return employeeService.GetEmployeeById(EmployeeID).FullName;
            }
        }

        [Key]
        public int LogID
        {
            get { return logID; }
            set { logID = value; OnPropertyChanged(); }
        }

        [ForeignKey("Employee")]
        public int? EmployeeID
        {
            get { return employeeID; }
            set { employeeID = value; OnPropertyChanged(); }
        }

        public string Action
        {
            get { return action; }
            set { action = value; OnPropertyChanged(); }
        }

        public string EventType
        {
            get { return eventType; }
            set { eventType = value; OnPropertyChanged(); }
        }

        [NotMapped]
        public string StrDateTime
        {
            get => dateTime.ToString("dd/MM/yyyy HH:mm");
            set { strDateTime = value; OnPropertyChanged(); }
        }

        public DateTime DateTime
        {
            get { return dateTime; }
            set { dateTime = value; OnPropertyChanged(); }
        }

        public string Status
        {
            get { return status; }
            set { status = value; OnPropertyChanged(); }
        }

        public virtual tbEmployee Employee { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
