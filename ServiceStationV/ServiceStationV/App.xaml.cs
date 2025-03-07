using System.Configuration;
using System.Data;
using System.Windows;

namespace ServiceStationV
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
    }
    public static class UserRepository
    {
        public static List<User> Users = new List<User>();
    }
    public class User
    {
        string FullName;
        string PhoneNum;
        string Login;
        string Password;
        public User(string fn, string pn, string l, string p)
        { 
            this.FullName = fn;
            this.PhoneNum = pn;
            this.Login = l; 
            this.Password = p;
        }
    }

}
