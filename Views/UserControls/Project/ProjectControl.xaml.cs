using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.UserControls.Project;
using MyKpiyapProject.ViewModels.UserControls.Task;

namespace MyKpiyapProject.UserControls
{
    /// <summary>
    /// Логика взаимодействия для ProjectControl.xaml
    /// </summary>
    public partial class ProjectControl : UserControl
    {
        private EmployeeService employeerService = new EmployeeService();

        public ProjectControl(tbEmployee tbEmployee)
        {
            InitializeComponent();

            DataContext = new ProjectControlViewModel(tbEmployee);
            Loaded += OnLoaded;

        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProjectControlViewModel vm)
            {
                vm.PropertyChanged += (s, args) =>
                {
                    if (args.PropertyName == nameof(ProjectControlViewModel.CurrentControl) && vm.CurrentControl != null)
                    {
                        ShowInAnimation(vm.CurrentControl, FormContainer, "SlideInFromRightStoryboard");
                    }
                    if (args.PropertyName == nameof(ProjectControlViewModel.SecondControl) && vm.SecondControl != null)
                    {
                        ShowInAnimation(vm.SecondControl, TaskInfoContainer, "SlideInFromBottomStoryboard");
                    }
                };
            }
        }

        private void projectDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                if (e.Row.DataContext is tbProject project)
                {
                    int rowNumber = e.Row.GetIndex() + 1;
                    project.RowNumber = rowNumber;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке строки: {ex.Message}");
            }
        }

        private void ShowInAnimation(UserControl userControl, Grid container, string storyboardKey)
        {
            container.Children.Clear();
            container.Children.Add(userControl);

            var storyboard = (Storyboard)FindResource(storyboardKey);
            storyboard.Begin(userControl);
        }
    }
}