using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MyKpiyapProject.ViewModels.UserControls.Task;
using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.Views.UserControls.Task;

namespace MyKpiyapProject.UserControls
{
    public partial class TaskControl : UserControl
    {
        private tbEmployee _myUser;
        private EmployeeService _userService;
        private EmployeeService _employeerService;
        public TaskControl(tbEmployee User)
        {
            InitializeComponent();
            _myUser = User;
            DataContext = new TaskControlViewModel(_myUser);
            Loaded += OnLoaded;

            _userService = new EmployeeService();
            _employeerService = new EmployeeService();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is TaskControlViewModel vm)
            {
                vm.PropertyChanged += (s, args) =>
                {
                    if (args.PropertyName == nameof(TaskControlViewModel.CurrentControl) && vm.CurrentControl != null)
                    {
                        ShowInAnimation(vm.CurrentControl, FormContainer, "SlideInFromRightStoryboard");
                    }
                    if (args.PropertyName == nameof(TaskControlViewModel.SecondControl) && vm.SecondControl != null)
                    {
                        ShowInAnimation(vm.SecondControl, TaskInfoContainer, "SlideInFromBottomStoryboard");
                    }
                };
            }
        }

        private void ShowInAnimation(UserControl userControl, Grid container, string storyboardKey)
        {
            container.Children.Clear();
            container.Children.Add(userControl);

            var storyboard = (Storyboard)FindResource(storyboardKey);
            storyboard.Begin(userControl);
        }

        private void tasksDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                if (e.Row.DataContext is tbTask task)
                {
                    int rowNumber = e.Row.GetIndex() + 1;
                    task.RowNumber = rowNumber;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке строки: {ex.Message}");
            }
        }
    }
}
