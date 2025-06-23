using MyKpiyapProject.NewModels;
using MyKpiyapProject.ViewModels.UserControls.Report;
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

namespace MyKpiyapProject.Views.UserControls.Report
{
    /// <summary>
    /// Логика взаимодействия для ViewingReportControl.xaml
    /// </summary>
    public partial class ViewingReportControl : UserControl
    {
        public ViewingReportControl()
        {
            InitializeComponent();
        }
        public ViewingReportControl(tbReport tbReport)
        {
            InitializeComponent();
            DataContext = new ViewingReportControlViewModel(tbReport);
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewingReportControlViewModel viewModel)
            {
                viewModel.SetRichTextBox(richTextBox);
                viewModel.LoadData(null); // Загружаем содержимое автоматически
            }
        }


    }
}
