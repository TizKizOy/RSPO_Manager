using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;

namespace MyKpiyapProject.UserControls
{
    /// <summary>
    /// Логика взаимодействия для AccountControl.xaml
    /// </summary>
    public partial class AccountControl : UserControl
    {
        private tbEmployee Myuser;
        private EmployeeService EmployeeService;
        public AccountControl()
        {
            InitializeComponent();
        }

        public AccountControl(tbEmployee user)
        {
            Myuser = user;
            EmployeeService = new EmployeeService();
            InitializeComponent();
            LoadUserData(user);
            LoadUserPhoto(user.Photo);
        }

        private void LoadUserData(tbEmployee user)
        {

            string Name = user.FullName.Split(' ')[0];
            string LastName = user.FullName.Split(' ')[1];
            textBoxName.Text = Name;
            textBoxLastName.Text = LastName;
            textBoxEmail.Text = user.Email;
            textBoxPosition.Text = user.PositionAndRole.PositionAndRoleName;
            textBlockName.Text = Name.ToUpper();
            textBlockLastName.Text = LastName.ToUpper();
        }

        private void LoadUserPhoto(byte[] photoData)
        {
            if (photoData != null && photoData.Length > 0)
            {
                using (var ms = new System.IO.MemoryStream(photoData))
                {
                    var image = new System.Windows.Media.Imaging.BitmapImage();
                    image.BeginInit();
                    image.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();

                    userImage.Source = image;
                }
            }
            else
            {
                userImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("../Image/default.png", UriKind.Relative));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                var NewEmployeer = new tbEmployee
                {
                    FullName = textBoxName.Text + " " + textBlockLastName.Text,
                    Email = textBoxEmail.Text,
                    PositionAndRole = Myuser.PositionAndRole,
                    EmployeeID = Myuser.EmployeeID,
                    Experience = Myuser.Experience,
                    Gender = Myuser.Gender,
                    Login = Myuser.Login,
                    Password = Myuser.Password,
                    Phone = Myuser.Phone,
                    Photo = Myuser.Photo
                };

                EmployeeService.UpdateEmployee(NewEmployeer);
                MessageBox.Show("Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
