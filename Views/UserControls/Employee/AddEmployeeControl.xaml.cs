using MyKpiyapProject.NewModels;
using MyKpiyapProject.ViewModels.UserControls.Employee;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MyKpiyapProject.UserControls
{
    public partial class AddEmployeeControl : UserControl
    {
        public AddEmployeeControl(Action refreshCallback, tbEmployee tbEmployee)
        {
            InitializeComponent();
           
            DataContext = new AddEmployeeViewModel(refreshCallback, tbEmployee);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddEmployeeViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
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