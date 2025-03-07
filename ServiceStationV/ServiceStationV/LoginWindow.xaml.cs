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
            
    }
    }
}