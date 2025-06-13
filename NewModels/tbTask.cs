using Microsoft.Office.Interop.Excel;
using MyKpiyapProject.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace MyKpiyapProject.NewModels
{
    public class tbTask : INotifyPropertyChanged
    {
        private int taskID;
        private DateTime creationDate;
        private DateTime deadlineDate;
        private string description;
        private string title;
        private int statusID;
        private int projectID;
        private int priorityID;
        private int executorID;
        private int creatorID;
        private int rowNumber;

        [NotMapped]
        public string CreatorName
        {
            get
            {
                return Creator?.FullName ?? "Неизвестно";
            }
        }
        [NotMapped]
        public string ExecutorName
        {
            get
            {
                return Executor?.FullName ?? "Неизвестно";
            }
        }
        [NotMapped]
        public string NameProject
        {
            get
            {
                return Project?.Title ?? "Неизвестно";
            }
        }
        [NotMapped]
        public int RowNumber
        {
            get { return rowNumber; }
            set { rowNumber = value; OnPropertyChanged(); }
        }

        [Key]
        public int TaskID
        {
            get { return taskID; }
            set { taskID = value; OnPropertyChanged(); }
        }

        public DateTime CreationDate
        {
            get { return creationDate; }
            set { creationDate = value; OnPropertyChanged(); }
        }

        public DateTime DeadLineDate
        {
            get => deadlineDate;
            set { deadlineDate = value; OnPropertyChanged(); }
        }

        //public string Status
        //{
        //    get { return status; }
        //    set { status = value; OnPropertyChanged(); }
        //}

        public string Description
        {
            get { return description; }
            set { description = value; OnPropertyChanged(); }
        }

        public string Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged(); }
        }

        [ForeignKey("Project")]
        public int ProjectID
        {
            get { return projectID; }
            set { projectID = value; OnPropertyChanged(); }
        }

        //public string Priority
        //{
        //    get => priority;
        //    set { priority = value; OnPropertyChanged(); }
        //}

        [ForeignKey("Executor")]
        public int ExecutorID
        {
            get => executorID;
            set { executorID = value; OnPropertyChanged(); }
        }
        [ForeignKey("Creator")]
        public int CreatorID
        {
            get => creatorID;
            set { creatorID = value; OnPropertyChanged(); }
        }

        [ForeignKey("Status")]
        public int StatusID
        {
            get => statusID;
            set { statusID = value; OnPropertyChanged(); }
        }

        [ForeignKey("Priority")]
        public int PriorityID
        {
            get => priorityID;
            set { priorityID = value; OnPropertyChanged(); }
        }

        public virtual tbStatusTask Status { get; set; }
        public virtual tbPriorityTask Priority { get; set; }
        public virtual tbProject Project { get; set; }
        public virtual tbEmployee Executor { get; set; }
        public virtual tbEmployee Creator { get; set; }
        public virtual ICollection<tbReport> Reports { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
