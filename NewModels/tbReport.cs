using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace MyKpiyapProject.NewModels
{
    public class tbReport : INotifyPropertyChanged
    {
        private int reportID;
        private DateTime creationDate;
        private DateTime finishDate;
        private string title;
        private string description;
        private int? employeeID;
        private string employeeName;
        private string workExpenses;
        private int? projectID;
        private string projectName;
        private int? taskID;
        private string taskName;
        private string status;
        private int? countEndTask;
        private int? countEndProject;
        private int? countNotEndProjAndTask;
        private int rowNumber;

        [Key]
        public int ReportID
        {
            get { return reportID; }
            set { reportID = value; OnPropertyChanged(); }
        }

        public DateTime CreationDate
        {
            get { return creationDate; }
            set { creationDate = value; OnPropertyChanged(); }
        }
        public DateTime FinishDate
        {
            get { return finishDate; }
            set { finishDate = value; OnPropertyChanged(); }
        }

        public string Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get { return description; }
            set { description = value; OnPropertyChanged(); }
        }

        [ForeignKey("Employee")]
        public int? EmployeeID
        {
            get { return employeeID; }
            set { employeeID = value; OnPropertyChanged(); }
        }

        public int? CountEndTask
        {
            get { return countEndTask; }
            set { countEndTask = value; OnPropertyChanged(); }
        }

        public int? CountNotEndProject
        {
            get { return countNotEndProjAndTask; }
            set { countNotEndProjAndTask = value; OnPropertyChanged(); }
        }

        public int? CountEndProject
        {
            get { return countEndProject; }
            set { countEndProject = value; OnPropertyChanged(); }
        }

        public string WorkExpenses
        {
            get => workExpenses;
            set { workExpenses = value; OnPropertyChanged(); }
        }

        public string Status
        {
            get => status;
            set { status = value; OnPropertyChanged(); }
        }

        [ForeignKey("Project")]
        public int? ProjectID
        {
            get { return projectID; }
            set { projectID = value; OnPropertyChanged(); }
        }

        [ForeignKey("Task")] 
        public int? TaskID
        {
            get { return taskID; }
            set { taskID = value; OnPropertyChanged(); }
        }

        [NotMapped]
        public string EmployeeName
        {
            get { return Employee?.FullName; }
            set { employeeName = value; OnPropertyChanged(); }
        }

        [NotMapped]
        public string ProjectName
        {
            get { return Project?.Title ?? "Отсутствует"; }
            set { projectName = value; OnPropertyChanged(); }
        }

        [NotMapped]
        public string TaskName
        {
            get { return Task?.Title ?? "Отсутствует"; }
            set { taskName = value; OnPropertyChanged(); }
        }

        [NotMapped]
        public int RowNumber
        {
            get { return rowNumber; }
            set { rowNumber = value; OnPropertyChanged(); }
        }

        // Навигационные свойства
        public virtual tbEmployee Employee { get; set; }
        public virtual tbProject Project { get; set; }

        [ForeignKey("TaskID")]
        public virtual tbTask Task { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}