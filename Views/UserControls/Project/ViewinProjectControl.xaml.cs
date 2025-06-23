using MyKpiyapProject.NewModels;
using MyKpiyapProject.ViewModels.UserControls.Project;
using MyKpiyapProject.ViewModels.UserControls.Task;
using MyKpiyapProject.Views.UserControls.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyKpiyapProject.Views.UserControls.Project
{
    /// <summary>
    /// Логика взаимодействия для ViewinProjectControl.xaml
    /// </summary>
    public partial class ViewinProjectControl : UserControl
    {
        public ViewinProjectControl()
        {
            InitializeComponent();
        }
        public ViewinProjectControl(tbProject tbProj, Action action, tbEmployee employee)
        {
            InitializeComponent();
            DataContext = new ViewingProjectViewModel(tbProj, action, employee);
            Loaded += OnLoaded;
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

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewingProjectViewModel vm)
            {
                vm.PropertyChanged += (s, args) =>
                {
                    if (args.PropertyName == nameof(ViewingProjectViewModel.SecondControl) && vm.SecondControl != null)
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

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)FindResource("SlideOutToTopStoryboard");
            if (storyboard != null)
            {
                storyboard.Begin(this);

                storyboard.Completed += (s, args) =>
                {
                    var parent = this.Parent as Grid;
                    if (parent != null)
                    {
                        parent.Children.Remove(this);
                    }
                };
            }
        }
    }
}
