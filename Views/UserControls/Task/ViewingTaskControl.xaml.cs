using MyKpiyapProject.NewModels;
using MyKpiyapProject.ViewModels.UserControls.Task;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MyKpiyapProject.Views.UserControls.Task
{
    /// <summary>
    /// Логика взаимодействия для ViewingTask.xaml
    /// </summary>
    public partial class ViewingTask : UserControl
    {
        public ViewingTask()
        {
            InitializeComponent();
        }

        public ViewingTask(tbTask tbTask, Action action, tbEmployee employee)
        {
            InitializeComponent();
            DataContext = new ViewingTaskControlViewModel(tbTask, action, employee);
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
