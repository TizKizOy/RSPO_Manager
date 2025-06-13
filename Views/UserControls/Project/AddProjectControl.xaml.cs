using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.Events;
using MyKpiyapProject.ViewModels.UserControls.Project;

namespace MyKpiyapProject.UserControls
{
    /// <summary>
    /// Логика взаимодействия для AddProjectControl.xaml
    /// </summary>
    public partial class AddProjectControl : UserControl
    {
        //private string selectedFilePath;
        private string fileContent;
        //private ProjectService projectService;
        private EmployeeService employeerService = new EmployeeService();
        private List<tbEmployee> employeerList = new List<tbEmployee>();
        public event EventHandler<ProjectEventArgs> ProjectAdded;

        public AddProjectControl(Action refreshCallback, tbEmployee tbEmployee)
        {
            InitializeComponent();
            DataContext = new AddProjectControlViewModel(refreshCallback, tbEmployee);
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