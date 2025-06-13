using MyKpiyapProject.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using MyKpiyapProject.ViewModels.UserControls.Employee;
using System.Windows.Media.Animation;
using MyKpiyapProject.Events;
using MyKpiyapProject.NewModels;

namespace MyKpiyapProject.UserControls
{
    /// <summary>
    /// Логика взаимодействия для EmployeeControl1.xaml
    /// </summary>
    public partial class EmployeeControl1 : UserControl
    {
        public EmployeeControl1(tbEmployee tbEmployee)
        {
            InitializeComponent();

            DataContext = new EmployeeViewModel(tbEmployee);
            Loaded += OnLoaded;
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MembersDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is tbEmployee employee)
            {
                int rowNumber = e.Row.GetIndex() + 1;
                employee.RowNumber = rowNumber;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is EmployeeViewModel vm)
            {
                vm.PropertyChanged += (s, args) =>
                {
                    if (args.PropertyName == nameof(EmployeeViewModel.CurrentControl) && vm.CurrentControl != null)
                    {
                        ShowInAnimation(vm.CurrentControl);
                    }
                };
            }
        }

        private void ShowInAnimation(UserControl userControl)
        {
            // Подготовка контрола к анимации
            userControl.Margin = new Thickness(500, 0, 0, 0);
            userControl.HorizontalAlignment = HorizontalAlignment.Right;

            // Очистка и добавление нового контрола
            FormContainer.Children.Clear();
            FormContainer.Children.Add(userControl);

            // Запуск анимации
            var storyboard = (Storyboard)FindResource("SlideInFromRightStoryboard");
            storyboard.Begin(userControl);
        }
    }
}