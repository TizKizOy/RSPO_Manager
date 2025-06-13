using MyKpiyapProject.NewModels;
using MyKpiyapProject.ViewModels.UserControls.Project;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MyKpiyapProject.UserControls.EditControls
{
    public partial class EditProjectControl : UserControl
    {
        public EditProjectControl(tbProject project, Action refreshCallback, tbEmployee tbEmployee)
        {
            InitializeComponent();
            DataContext = new EditProjectControlViewModel(project, refreshCallback, tbEmployee);
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