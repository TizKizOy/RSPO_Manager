using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Specialized;
using MyKpiyapProject.UserControls;
using MyKpiyapProject.ViewModels.Commands;
using Microsoft.Office.Interop.Excel;

namespace MyKpiyapProject.ViewModels.UserControls.Employee
{
    public class EmployeeViewModel : INotifyPropertyChanged
    {
        private tbEmployee myUser;
        private readonly EmployeeService _employeerService;
        private ObservableCollection<tbEmployee> _employeers;
        private string _employeesCountText;
        private UserControl _currentControl;
        private tbEmployee _selectedEmployee;
        private bool _isLoading;
        private ObservableCollection<tbEmployee> _allEmployeers;
        private string _currentFilterRole = "Все";
        private string _selectedRole = "Все";
        private string _searchText;

        private AdminLogService _adminLogService;
        private LoggingService _loggingService;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }
        public string SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<tbEmployee> Employeers
        {
            get => _employeers;
            private set
            {
                if (_employeers != null)
                    _employeers.CollectionChanged -= Employeers_CollectionChanged;

                _employeers = value;

                if (_employeers != null)
                    _employeers.CollectionChanged += Employeers_CollectionChanged;

                OnPropertyChanged();
                UpdateRecordCount();
            }
        }
        public tbEmployee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested(); // Обновляем состояние команд
            }
        }
        public UserControl CurrentControl
        {
            get => _currentControl;
            set
            {
                _currentControl = value;
                OnPropertyChanged();
            }
        }
        public string EmployeesCountText
        {
            get => _employeesCountText;
            private set
            {
                _employeesCountText = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadAddEmployeeControlCommand { get; }
        public ICommand LoadEditEmployeeControlCommand { get; }
        public ICommand LoadEmployeeCommand { get; }
        public ICommand RemoveEmployeeCommand { get; }
        public ICommand FilterByRoleCommand { get; }
        public EmployeeViewModel()
        { }

        public EmployeeViewModel(tbEmployee employee, EmployeeService employeeService, LoggingService loggingService)
        {
            myUser = employee ?? throw new ArgumentNullException(nameof(employee));
            _employeerService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            Employeers = new ObservableCollection<tbEmployee>();

            _adminLogService = new AdminLogService();
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));

            LoadEmployeeCommand = new RelayCommand(async (_) => await LoadEmployeesAsync(), (_) => !_isLoading);
            RemoveEmployeeCommand = new RelayCommand(_ => RemoveEmployee(), (_) => SelectedEmployee != null);
            LoadAddEmployeeControlCommand = new RelayCommand(_ => LoadAddEmployeeControl());
            LoadEditEmployeeControlCommand = new RelayCommand(_ => LoadEditEmployeeControl(), (_) => SelectedEmployee != null);
            FilterByRoleCommand = new RelayCommand(FilterByRole);
        }

        //public EmployeeViewModel(tbEmployee employee, EmployeeService employeeService, LoggingService loggingService)
        //{
        //    myUser = employee;
        //    _employeerService = employeeService;
        //    Employeers = new ObservableCollection<tbEmployee>();

        //    _adminLogService = new AdminLogService();
        //    _loggingService = loggingService;

        //    LoadEmployeeCommand = new RelayCommand(async (_) => await LoadEmployeesAsync(),
        //       (_) => !_isLoading);

        //    RemoveEmployeeCommand = new RelayCommand(_ => RemoveEmployee(),
        //        (_) => SelectedEmployee != null);

        //    LoadAddEmployeeControlCommand = new RelayCommand(_ => LoadAddEmployeeControl());

        //    LoadEditEmployeeControlCommand = new RelayCommand(_ => LoadEditEmployeeControl(),
        //        (_) => SelectedEmployee != null);
        //    FilterByRoleCommand = new RelayCommand(FilterByRole);

        //}
        public EmployeeViewModel(tbEmployee tbEmployee)
        {
            myUser = tbEmployee;
            _employeerService = new EmployeeService();
            Employeers = new ObservableCollection<tbEmployee>();

            _adminLogService = new AdminLogService();
            _loggingService = new LoggingService();

            LoadEmployeeCommand = new RelayCommand(async (_) => await LoadEmployeesAsync(),
                (_) => !_isLoading);

            RemoveEmployeeCommand = new RelayCommand(_ => RemoveEmployee(),
                (_) => SelectedEmployee != null);

            LoadAddEmployeeControlCommand = new RelayCommand(_ => LoadAddEmployeeControl());

            LoadEditEmployeeControlCommand = new RelayCommand(_ => LoadEditEmployeeControl(),
                (_) => SelectedEmployee != null);
            FilterByRoleCommand = new RelayCommand(FilterByRole);

            // Первоначальная загрузка данных
            LoadEmployeeCommand.Execute(null);
        }

        private async System.Threading.Tasks.Task LoadEmployeesAsync()
        {
            try
            {
                _isLoading = true;
                CommandManager.InvalidateRequerySuggested();

                var employees = await System.Threading.Tasks.Task.Run(() => _employeerService.GetAllEmployees());
                _allEmployeers = new ObservableCollection<tbEmployee>(employees);
                ApplyFilters();
                await _loggingService.LogAction(myUser.EmployeeID, "Загрузка рабочих", "Операции с данными", "Успех");
            }
            catch
            {
                _isLoading = false;
                CommandManager.InvalidateRequerySuggested();
                await _loggingService.LogAction(myUser.EmployeeID, "Загрузка рабочих", "Операции с данными", "Ошибка");
            }
        }

        private void FilterByRole(object role)
        {
            _currentFilterRole = role?.ToString() ?? "Все";
            SelectedRole = _currentFilterRole; // Обновляем выбранную роль
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (_allEmployeers == null) return;

            IEnumerable<tbEmployee> filtered = _allEmployeers;

            // Фильтрация по роли
            if (!string.IsNullOrEmpty(_selectedRole) && _selectedRole != "Все")
            {
                filtered = filtered.Where(e => e.PositionAndRole.PositionAndRoleName == _selectedRole);
            }

            // Фильтрация по тексту
            if (!string.IsNullOrEmpty(SearchText))
            {
                var searchTextLower = SearchText.ToLower();
                filtered = filtered.Where(e =>
                    e.FullName.ToLower().Contains(searchTextLower) ||
                    e.PositionAndRole.PositionAndRoleName.ToLower().Contains(searchTextLower) ||
                    e.Email.ToLower().Contains(searchTextLower) ||
                    e.Phone.ToLower().Contains(searchTextLower));
            }

            Employeers = new ObservableCollection<tbEmployee>(filtered);
        }

        private void RefreshData()
        {
            LoadEmployeeCommand.Execute(null);
            CurrentControl = null;
            ApplyFilters();
        }

        private void Employeers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateRecordCount();
        }

        private void UpdateRecordCount()
        {
            int count = Employeers.Count;
            string ending = count % 100 is >= 11 and <= 14 ? "Сотрудников" :
                (count % 10) switch
                {
                    1 => "Сотрудник",
                    2 or 3 or 4 => "Сотрудника",
                    _ => "Сотрудников"
                };

            EmployeesCountText = $"{count} {ending}";
        }

        private void LoadAddEmployeeControl()
        {
            if (myUser.PositionAndRole.PositionAndRoleName != "Админ")
            {
                MessageBox.Show("Доступ запрещён", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            CurrentControl = new AddEmployeeControl(RefreshData, myUser)
            {
                DataContext = new AddEmployeeViewModel(RefreshData, myUser)
            };
        }

        private void LoadEditEmployeeControl()
        {
            if (myUser.PositionAndRole.PositionAndRoleName != "Админ")
            {
                MessageBox.Show("Доступ запрещён", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            CurrentControl = new EditEmployeeControl(SelectedEmployee, RefreshData, myUser)
            {
                DataContext = new EditEmployeeViewModel(SelectedEmployee, RefreshData, myUser)
            };
        }

        public virtual async void RemoveEmployee()
        {
            if (myUser.PositionAndRole.PositionAndRoleName != "Админ")
            {
                MessageBox.Show("Доступ запрещён", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                if (MessageBox.Show($"Удалить сотрудника {SelectedEmployee.FullName}?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _employeerService.DeleteEmployee(SelectedEmployee.EmployeeID);
                    RefreshData();
                    await _loggingService.LogAction(myUser.EmployeeID, "Удаление рабочих", "Операции с данными", "Успех");
                }
            }
            catch
            {
                await _loggingService.LogAction(myUser.EmployeeID, "Удаление рабочих", "Операции с данными", "Ошибка");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}