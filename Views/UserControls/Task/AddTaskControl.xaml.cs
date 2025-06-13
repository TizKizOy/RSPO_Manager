using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MyKpiyapProject.ViewModels.UserControls.Task;
using MyKpiyapProject.NewModels;

namespace MyKpiyapProject.Views.UserControls.Task
{
    /// <summary>
    /// Логика взаимодействия для AddTaskControl.xaml
    /// </summary>
    public partial class AddTaskControl : UserControl
    {
        private tbEmployee _user;
        public AddTaskControl(Action refreshData, tbEmployee user)
        {
            InitializeComponent();
            _user = user;
            DataContext = new AddTaskControlViewModel(refreshData, _user);
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
