using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;


namespace MyKpiyapProject.ViewModels.UserControls.Employee
{
    public class AddEmployeeViewModel : INotifyPropertyChanged
    {
        private tbEmployee myUser;
        private readonly EmployeeService _employeerService;
        private readonly Action _refreshCallback;

        private AdminLogService _adminLogService;
        private LoggingService _loggingService;
        private GenderService _genderService;
        private PositionAndRoleService _positionAndRoleService;
        private WorkExperienceServices _workExperienceServices;

        private string _fullName;
        private string _email;
        private string _phone;
        private string _password;
        private string _gender = "Мужской";
        private string _status = "Рабочий";
        private string _experience = "Менее 1 года";
        private string _selectedFilePath;
        private byte[] _imageData;
        private tbEmployee newEmployeer;

        private string _defaultImagePath = @"Source\Wolf.jpg";

        public string FullName
        {
            get => _fullName ;
            set { _fullName = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _email ;
            set { _email = value; OnPropertyChanged(); }
        }

        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string Gender
        {
            get => _gender;
            set { _gender = value; OnPropertyChanged(); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public string Experience
        {
            get => _experience;
            set { _experience = value; OnPropertyChanged(); }
        }

        public string SelectedFilePath
        {
            get => _selectedFilePath;
            set { _selectedFilePath = value; OnPropertyChanged(); }
        }

        public bool IsMale
        {
            get => _gender == "Мужской";
            set { if (value) Gender = "Мужской"; }
        }

        public bool IsFemale
        {
            get => _gender == "Женский";
            set { if (value) Gender = "Женский"; }
        }

        public ICommand SaveEmployeeCommand { get; }
        public ICommand SelectImageCommand { get; }

        public AddEmployeeViewModel(Action refreshCallback, tbEmployee tbEmployee, EmployeeService employeeService, LoggingService loggingService)
        {
            myUser = tbEmployee;
            _employeerService = employeeService;
            _refreshCallback = refreshCallback;
            _loggingService = loggingService;
            _genderService = new GenderService();
            _positionAndRoleService = new PositionAndRoleService();
            _workExperienceServices = new WorkExperienceServices();


            SaveEmployeeCommand = new RelayCommand(_ => SaveEmployee(), CanSaveEmployee);
            SelectImageCommand = new RelayCommand(_ => SelectImage());
        }



        public AddEmployeeViewModel(Action refreshCallback, tbEmployee tbEmployee)
        {
            myUser = tbEmployee;
            _employeerService = new EmployeeService();
            _refreshCallback = refreshCallback;

            _adminLogService = new AdminLogService();
            _loggingService = new LoggingService();
            _genderService = new GenderService();
            _positionAndRoleService = new PositionAndRoleService();
            _workExperienceServices = new WorkExperienceServices();

            SaveEmployeeCommand = new RelayCommand(_ => SaveEmployee(), CanSaveEmployee);
            SelectImageCommand = new RelayCommand(_ => SelectImage());
        }

        public bool CanSaveEmployee(object parameter)
        {
            return !string.IsNullOrWhiteSpace(FullName) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Phone) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        private void SelectImage()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SelectedFilePath = openFileDialog.FileName;
                _imageData = System.IO.File.ReadAllBytes(SelectedFilePath);
            }
            if (_imageData != null)
                MessageBox.Show("Файл выбран" + SelectedFilePath);
        }

        public bool ValidateData(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(FullName))
            {
                errorMessage = "Имя и фамилия не могут быть пустыми.";
                return false;
            }

            string[] nameParts = FullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (nameParts.Length < 2)
            {
                errorMessage = "Имя и фамилия должны содержать два слова.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@") || !Email.Contains("."))
            {
                errorMessage = "Некорректный адрес электронной почты.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Phone))
            {
                errorMessage = "Номер телефона не может быть пустым.";
                return false;
            }

            if (!Phone.StartsWith("+"))
            {
                errorMessage = "Номер телефона должен начинаться со знака '+'.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Gender))
            {
                errorMessage = "Выберите нужный пол";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Status))
            {
                errorMessage = "Должность не может быть пустой.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Experience))
            {
                errorMessage = "Опыт не может быть пустым.";
                return false;
            }

            if (Password.Length < 5)
            {
                errorMessage = "Пароль не может быть меньше пяти символов.";
                return false;
            }

            return true;
        }


        public virtual async void SaveEmployee()
        {
            try
            {
                // Получаем идентификаторы для пола и роли/позиции
                int genderId = _genderService.GetOrCreateGenderId(Gender);
                int positionAndRoleId = _positionAndRoleService.GetOrCreatePositionAndRoleId(Status);
                int experienceId = _workExperienceServices.GetOrCreateExperienceId(Experience);

                if (!ValidateData(out string errorMessage))
                {
                    MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (_imageData == null && !string.IsNullOrWhiteSpace(_defaultImagePath))
                {
                    try
                    {
                        _imageData = System.IO.File.ReadAllBytes(_defaultImagePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при загрузке изображения по умолчанию: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                newEmployeer = new tbEmployee
                {
                    FullName = FullName,
                    Email = Email,
                    Login = Email.Split("@")[0],
                    Phone = Phone,
                    GenderID = genderId,
                    PositionAndRoleID = positionAndRoleId,
                    ExperienceID = experienceId,
                    Password = HashPassword(Password),
                    Photo = _imageData
                };

                _employeerService.AddEmployee(newEmployeer);
                _refreshCallback?.Invoke();

                await _loggingService.LogAction(myUser.EmployeeID, "Сохранение рабочих", "Операции с данными", "Успех");

                MessageBox.Show("Сотрудник успешно добавлен", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                await _loggingService.LogAction(myUser.EmployeeID, "Сохранение рабочих", "Операции с данными", "Ошибка");

                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public static string HashPassword(string password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);
            return hashedPassword;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
