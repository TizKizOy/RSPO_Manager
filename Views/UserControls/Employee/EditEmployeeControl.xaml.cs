using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MyKpiyapProject.ViewModels.UserControls.Employee;
using MyKpiyapProject.NewModels;

namespace MyKpiyapProject.UserControls
{
    /// <summary>
    /// Логика взаимодействия для EditEmployeeControl.xaml
    /// </summary>
    public partial class EditEmployeeControl : UserControl
    {
        public EditEmployeeControl(tbEmployee employeer, Action refreshCallback, tbEmployee tbEmployee)
        {
            InitializeComponent();
            var viewModel = new EditEmployeeViewModel(employeer, refreshCallback, tbEmployee);
            viewModel.RequestClose += StartCloseAnimation;
            DataContext = viewModel;
        }

        private void StartCloseAnimation()
        {
            var slideOut = new ThicknessAnimation
            {
                From = new Thickness(0),
                To = new Thickness(ActualWidth, 0, 0, 0),
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            slideOut.Completed += (s, e) =>
            {
                if (Parent is Panel parentPanel)
                {
                    parentPanel.Children.Remove(this);
                }
            };

            BeginAnimation(MarginProperty, slideOut);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is EditEmployeeViewModel vm)
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
