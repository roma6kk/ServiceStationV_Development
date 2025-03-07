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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ServiceStationV
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        LoginWindow loginWindow;
        public RegistrationWindow(LoginWindow lw)
        {
            InitializeComponent();
            loginWindow = lw;
        }
        private void GetBackBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RegWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            loginWindow.Show();
        }

        private void RegBTN_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
