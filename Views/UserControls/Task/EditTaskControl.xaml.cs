using MyKpiyapProject.NewModels;
using MyKpiyapProject.ViewModels.UserControls.Task;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MyKpiyapProject.Views.UserControls.Task
{
    /// <summary>
    /// Логика взаимодействия для EditTaskControl.xaml
    /// </summary>
    public partial class EditTaskControl : UserControl
    {
        private readonly Action _refreshData;

        public EditTaskControl(tbTask task, Action refreshData, tbEmployee tbEmployee)
        {
            InitializeComponent();
            _refreshData = refreshData;
            DataContext = new EditTaskControlViewModel(task, refreshData, tbEmployee);
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
