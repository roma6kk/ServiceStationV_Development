using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServiceStationV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void GoToRegBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            RegistrationWindow RegWindow = new(this);
            RegWindow.Show();
        }

        private void LoginBTN_Click(object sender, RoutedEventArgs e)
        {
            if (LoginTB.Text != "" && PasswordTB.Password != "")
            {
                foreach (var user in UserRepository.Users)
                {
                    if (user.Login == LoginTB.Text && user.Password == PasswordTB.Password)
                    {
                        UserRepository.CurrentUser = user;
                        MainMenuWindow MMWindow = new MainMenuWindow();
                        Application.Current.MainWindow = MMWindow;
                        MMWindow.Show();
                        this.Close();
                        return;
                    }
                }
            }
            MessageBox.Show("Неверный логин и/или пароль!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}