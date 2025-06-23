using MyKpiyapProject.NewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MyKpiyapProject.ViewModels.UserControls.Report;
using MyKpiyapProject.ViewModels.UserControls.Task;
using System.Windows.Media.Animation;
using MyKpiyapProject.ViewModels.UserControls.Project;

namespace MyKpiyapProject.Views.UserControls.Report
{
    /// <summary>
    /// Логика взаимодействия для ReportControl.xaml
    /// </summary>
    public partial class ReportControl : UserControl
    {
        public ReportControl()
        {
            InitializeComponent();
        }
        public ReportControl(tbEmployee user)
        {
            InitializeComponent();
            DataContext = new ReportControlViewModel(user);
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ReportControlViewModel vm)
            {
                vm.PropertyChanged += (s, args) =>
                {
                    if (args.PropertyName == nameof(ReportControlViewModel.CurrentControl) && vm.CurrentControl != null)
                    {
                        if (vm.CurrentControl is ChoosingReportControl)
                        {
                            ShowInAnimation(vm.CurrentControl, FormContainer, "SlideInFromRightStoryboard");
                        }
                        else
                        {
                            ShowInAnimation(vm.CurrentControl, FormContainer, "SlideInFromRightStoryboard");
                        }
                    }

                    if (args.PropertyName == nameof(ViewingReportControlViewModel.SecondControl) && vm.SecondControl != null)
                    {
                        ShowInAnimation(vm.SecondControl, TaskInfoContainer, "SlideInFromBottomStoryboard");
                    }
                };
            }
        }

        private void ShowInAnimation(UserControl userControl, Grid container, string storyboardKey)
        {

            if (userControl.Parent != null)
            {
                var currentParent = userControl.Parent as Panel;
                currentParent?.Children.Remove(userControl);
            }

            container.Children.Clear();
            container.Children.Add(userControl);

            var storyboard = (Storyboard)FindResource(storyboardKey);
            storyboard.Begin(userControl);
        }



        private void projectDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                if (e.Row.DataContext is tbReport project)
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
    }
}
