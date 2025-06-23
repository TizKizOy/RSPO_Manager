using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BCrypt.Net;
using System.Diagnostics;

namespace MyKpiyapProject.ViewModels
{
    public class AuthWindowViewModel : INotifyPropertyChanged
    {
        private tbEmployee authUser;
        private readonly EmployeeService employeeService;
        private string _login;
        private string _password;
        private string _errorMessage;
        private bool _isLoading;

        private AdminLogService _adminLogService;
        private LoggingService _loggingService;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotLoading));
            }
        }

        public bool IsNotLoading => !IsLoading;

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand AuthCommand { get; }

        public AuthWindowViewModel()
        {
            employeeService = new EmployeeService();
            AuthCommand = new RelayCommand(Auth);

            _adminLogService = new AdminLogService();
            _loggingService = new LoggingService();
        }

        private async void Auth(object parameter)
        {
            try
            {
               


                if (!ValidateInputs())
                {
                    return;
                }

                ErrorMessage = string.Empty;

                Stopwatch searchStopwatch = Stopwatch.StartNew();
                authUser = await employeeService.GetUserByLoginAsync(Login);
                searchStopwatch.Stop();

                if (authUser == null)
                {
                    ErrorMessage = "Пользователь не найден";
                    return;
                }

                Stopwatch verifyStopwatch = Stopwatch.StartNew();
                bool isPasswordValid = VerifyPassword(authUser.Password, Password);
                verifyStopwatch.Stop();


                    
                   
                

                if (!isPasswordValid)
                {
                    ErrorMessage = "Неверный пароль";
                    return;
                }

                string welcomeMessage = $"Приветствуем {authUser.FullName + " " +authUser.PositionAndRole.PositionAndRoleName}\n\n" +
                                        $"Время поиска пользователя: {searchStopwatch.ElapsedMilliseconds} мс\n" +
                                        $"Время проверки пароля: {verifyStopwatch.ElapsedMilliseconds} мс";

                MessageBox.Show(welcomeMessage, "Добро пожаловать", MessageBoxButton.OK, MessageBoxImage.Information);

                UserPageWindow userPageWindow = new UserPageWindow(authUser);
                userPageWindow.Show();
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is AuthWindow)
                    {
                        window.Hide();
                    }
                }
                await _loggingService.LogAction(authUser.EmployeeID, "Вход пользователя", "Авторизация", "Успех");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await _loggingService.LogAction(authUser.EmployeeID, "Вход пользователя", "Авторизация", "Ошибка");
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(Login))
            {
                ErrorMessage = "Введите логин";
                return false;
            }
            if (string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Введите пароль";
                return false;
            }
            return true;
        }

        public static bool VerifyPassword(string storedHash, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

        private bool CanAuth(object parameter)
        {
            return !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
