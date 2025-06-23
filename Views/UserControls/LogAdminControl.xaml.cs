using MyKpiyapProject.NewModels;
using System.Windows.Controls;
using MyKpiyapProject.ViewModels;
using MyKpiyapProject.ViewModels.UserControls;


namespace MyKpiyapProject.Views.UserControls
{
    /// <summary>
    /// Логика взаимодействия для LogAmdinControl.xaml
    /// </summary>
    public partial class LogAmdinControl : UserControl
    {
        public LogAmdinControl(tbEmployee tbEmployee)
        {
            InitializeComponent();
            DataContext = new LogAdminControlViewModel(tbEmployee);
        }

        private void LogDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                if (e.Row.DataContext is tbAdminLog log)
                {
                    int rowNumber = e.Row.GetIndex() + 1;
                    log.RowNumber = rowNumber;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке строки: {ex.Message}");
            }
        }
    }
}
