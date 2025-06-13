using MyKpiyapProject.NewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace MyKpiyapProject.NewModels
{
    public class tbEmployee : INotifyPropertyChanged
    {
        private int employeeID;
        private string fullName;
        private int genderID;
        private string login;
        private string password;
        private string email;
        private string phone;
        private byte[] photo;
        private int positionAndRoleID;
        private int experienceID;
        private int rowNumber;
        private string _bgColor;

        [NotMapped]
        public string Character
        {
            get
            {
                return !string.IsNullOrEmpty(FullName) && FullName.Length > 0
                    ? FullName.Substring(0, 1).ToUpper()
                    : "U";
            }
        }
        [NotMapped]
        public int RowNumber
        {
            get { return rowNumber; }
            set { rowNumber = value; OnPropertyChanged(); }
        }
        [NotMapped]
        public string Bgcolor
        {
            get
            {
                if (string.IsNullOrEmpty(_bgColor))
                {
                    // Используем комбинацию хэш-кодов нескольких полей для выбора цвета
                    int combinedHashCode = Math.Abs(EmployeeID.GetHashCode() + FullName.GetHashCode() + Email.GetHashCode());
                    int colorIndex = combinedHashCode % _bgColors.Length;
                    _bgColor = _bgColors[colorIndex];
                }
                return _bgColor;
            }
        }

        [NotMapped]
        private static readonly string[] _bgColors =
        {
            "#4fc3f7", "#81d4fa", "#9575cd", "#7986cb", "#64b5f6", "#42a5f5", "#2196f3",
            "#4dd0e1", "#4db6ac", "#81c784", "#aed581", "#dce775", "#d4e157",
            "#fff176", "#ffe082", "#ffcc80", "#ffb74d", "#ffa726", "#ff9800", "#ff8f00",
            "#a1887f", "#e0e0e0", "#90caf9", "#b39ddb", "#7986cb", "#64b5f6", "#42a5f5",
            "#80deea", "#4dd0e1", "#26c6da", "#26a69a", "#80cbc4", "#81c784", "#a5d6a7",
            "#c8e6c9", "#e8f5e9", "#f1f8e9", "#dcedc8", "#c5e1a5", "#aed581", "#9ccc65"
        };


        [Key]
        public int EmployeeID
        {
            get { return employeeID; }
            set { employeeID = value; OnPropertyChanged(); }
        }

        public string FullName
        {
            get { return fullName; }
            set { fullName = value; OnPropertyChanged(); }
        }

        //public string Gender
        //{
        //    get { return gender; }
        //    set { gender = value; OnPropertyChanged(); }
        //}

        public string Login
        {
            get { return login; }
            set { login = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get { return email; }
            set { email = value; OnPropertyChanged(); }
        }

        public string Phone
        {
            get { return phone; }
            set { phone = value; OnPropertyChanged(); }
        }

        public byte[] Photo
        {
            get { return photo; }
            set { photo = value; OnPropertyChanged(); }
        }

        //public string PositionAndRole
        //{
        //    get { return positionAndRole; }
        //    set { positionAndRole = value; OnPropertyChanged(); }
        //}

        [ForeignKey("Experience")]
        public int ExperienceID
        {
            get { return experienceID; }
            set { experienceID = value; OnPropertyChanged(); }
        }

        [ForeignKey("Gender")]
        public int GenderID
        {
            get => genderID;
            set { genderID = value; OnPropertyChanged(); }
        }

        [ForeignKey("PositionAndRole")]
        public int PositionAndRoleID
        {
            get => positionAndRoleID;
            set { positionAndRoleID = value; OnPropertyChanged(); }
        }

        public virtual tbGender Gender { get; set; }
        public virtual tbPositionAndRole PositionAndRole { get; set; }
        public virtual tbWorkExperience Experience { get; set; }

        public virtual ICollection<tbReport> Reports { get; set; }
        public virtual ICollection<tbProject> Projects { get; set; }
        public virtual ICollection<tbTask> Tasks { get; set; }
        public virtual ICollection<tbAdminLog> AdminLogs { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
