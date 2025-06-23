using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using MyKpiyapProject.NewModels;
using MyKpiyapProject.ViewModels.Commands;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MyKpiyapProject.UserControls;
using MyKpiyapProject.Views.UserControls;
using MyKpiyapProject.Views.UserControls.Report;


namespace MyKpiyapProject.ViewModels
{
    public class UserPageWindowViewModels : INotifyPropertyChanged
    {
        private tbEmployee _myUser;
        public string FullName => _myUser.FullName;
        public string Role => _myUser.PositionAndRole.PositionAndRoleName;
        public ImageSource UserImage { get; private set; }

        public ICommand LoadReportControlCommand { get; }
        public ICommand LoadEmployeeControlCommand { get; }
        public ICommand LoadProjectControlCommand { get; }
        public ICommand LoadTaskControlCommand { get; }
        public ICommand LoadAccountControlCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand LoadLogiControlCommand { get; }

        public event Action<UserControl> LoadControlRequested;
        public event Action LogoutRequested;

        public UserPageWindowViewModels(tbEmployee user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(tbEmployee), "tbEmployee cannot be null");
            }

            _myUser = user;


            LoadUserPhoto(user.Photo);

            LoadEmployeeControlCommand = new RelayCommand(_ => RequestLoadControl(new EmployeeControl1(_myUser)));
            LoadProjectControlCommand = new RelayCommand(_ => RequestLoadControl(new ProjectControl(_myUser)));
            LoadTaskControlCommand = new RelayCommand(_ => RequestLoadControl(new TaskControl(_myUser)));
            LoadAccountControlCommand = new RelayCommand(_ => RequestLoadControl(new AccountControl(_myUser)));
            LogoutCommand = new RelayCommand(_ => RequestLogout());
            LoadLogiControlCommand = new RelayCommand(_ => RequestLoadControl(new LogAmdinControl(_myUser)));
            LoadReportControlCommand = new RelayCommand(_ => RequestLoadControl(new ReportControl(_myUser)));

            RequestLoadControl(new ProjectControl(_myUser));
        }

        
        private void LoadUserPhoto(byte[] photoData)
        {
            if (photoData != null && photoData.Length > 0)
            {
                var image = new BitmapImage();
                using (var ms = new System.IO.MemoryStream(photoData))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                }
                UserImage = image;
            }
            else
            {
                UserImage = new BitmapImage(new Uri(@"C:\Users\TizKizOy\Downloads\Волк.jpg"));
            }
            OnPropertyChanged(nameof(UserImage));
        }

        private void RequestLoadControl(UserControl control)
        {
            LoadControlRequested?.Invoke(control);
        }

        private void RequestLogout()
        {
            LogoutRequested?.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
