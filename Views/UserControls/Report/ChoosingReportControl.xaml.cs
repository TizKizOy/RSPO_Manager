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
using MyKpiyapProject.ViewModels.UserControls.Report;
using MyKpiyapProject.Views.UserControls.Report;

namespace MyKpiyapProject.Views.UserControls.Report
{
    /// <summary>
    /// Логика взаимодействия для ChoosingReportControl.xaml
    /// </summary>
    public partial class ChoosingReportControl : UserControl
    {
        private readonly ReportControlViewModel _viewModel;
        public ChoosingReportControl()
        {
            InitializeComponent();
        }
        public ChoosingReportControl(ReportControlViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
        }

        private void Border_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;
            string reportType = (string)border.Tag;

            switch (reportType)
            {
                case "AddProjectWindow":
                    _viewModel.LoadProjectReportFormCommand.Execute(null);
                    break;
                case "AddTaskWindow":
                    _viewModel.LoadTaskReportFormCommand.Execute(null);
                    break;
                case "AddReportWindow":
                    _viewModel.LoadTimeReportFormCommand.Execute(null);
                    break;
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)FindResource("SlideOutFromRightStoryboard");
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
