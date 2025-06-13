using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using MyKpiyapProject.NewModels;
using MyKpiyapProject.UserControls;
using MyKpiyapProject.ViewModels;

namespace MyKpiyapProject
{
    /// <summary>
    /// Логика взаимодействия для UserPageWindow.xaml
    /// </summary>  
    public partial class UserPageWindow : Window
    {


        public UserPageWindow(tbEmployee user)
        {
            InitializeComponent();
            var viewModel = new UserPageWindowViewModels(user);
            DataContext = viewModel;

            viewModel.LoadControlRequested += (control) => LoadControl(control);
            viewModel.LogoutRequested += Logout;

            LoadControl(new ProjectControl(user));
        }

        private void LoadControl(UserControl control)
        {
            mainGrid.Children.Clear();
            control.Opacity = 0;
            mainGrid.Children.Add(control);

            var storyboard = new Storyboard();
            var animation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.5)
            };
            Storyboard.SetTarget(animation, control);
            Storyboard.SetTargetProperty(animation, new PropertyPath(UIElement.OpacityProperty));
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        private void Logout()
        {
            MessageBoxResult result = MessageBox.Show(
                "Вы уверены, что хотите выйти из аккаунта?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                AuthWindow authWindow = new AuthWindow();
                authWindow.Show();
                this.Close();
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private bool isFullScreen = false;

        private void ToggleFullScreen_Click(object sender, RoutedEventArgs e)
        {
            if (!isFullScreen)
            {
                // Переход в полноэкранный режим
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
                FullScreenIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.FullscreenExit;
            }
            else
            {
                WindowState = WindowState.Normal;
                Height = 720;
                Width = 1080;
                FullScreenIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.Fullscreen;
            }
            isFullScreen = !isFullScreen;
        }


        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Вы уверены, что хотите выйти из приложения?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }


        private void TopThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Height - e.VerticalChange > this.MinHeight)
            {
                this.Top += e.VerticalChange;
                this.Height -= e.VerticalChange;
            }
        }

        private void BottomThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Height + e.VerticalChange > this.MinHeight)
            {
                this.Height += e.VerticalChange;
            }
        }

        private void LeftThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Width - e.HorizontalChange > this.MinWidth)
            {
                this.Left += e.HorizontalChange;
                this.Width -= e.HorizontalChange;
            }
        }

        private void RightThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Width + e.HorizontalChange > this.MinWidth)
            {
                this.Width += e.HorizontalChange;
            }
        }

        private void TopLeftThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Width - e.HorizontalChange > this.MinWidth)
            {
                this.Left += e.HorizontalChange;
                this.Width -= e.HorizontalChange;
            }
            if (this.Height - e.VerticalChange > this.MinHeight)
            {
                this.Top += e.VerticalChange;
                this.Height -= e.VerticalChange;
            }
        }

        private void TopRightThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Width + e.HorizontalChange > this.MinWidth)
            {
                this.Width += e.HorizontalChange;
            }
            if (this.Height - e.VerticalChange > this.MinHeight)
            {
                this.Top += e.VerticalChange;
                this.Height -= e.VerticalChange;
            }
        }

        private void BottomLeftThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Width - e.HorizontalChange > this.MinWidth)
            {
                this.Left += e.HorizontalChange;
                this.Width -= e.HorizontalChange;
            }
            if (this.Height + e.VerticalChange > this.MinHeight)
            {
                this.Height += e.VerticalChange;
            }
        }

        private void BottomRightThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Width + e.HorizontalChange > this.MinWidth)
            {
                this.Width += e.HorizontalChange;
            }
            if (this.Height + e.VerticalChange > this.MinHeight)
            {
                this.Height += e.VerticalChange;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}