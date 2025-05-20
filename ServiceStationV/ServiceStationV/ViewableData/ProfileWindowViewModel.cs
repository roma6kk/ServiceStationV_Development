using ServiceStationV.Models;
using ServiceStationV.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStationV.ViewableData
{
    public class ProfileWindowViewModel
    {
        public string ProfileName { get; } = UserRepository.CurrentUser.FullName;
        public string Login { get; } = UserRepository.CurrentUser.Login;
        public string PhoneNumber { get; } = UserRepository.CurrentUser.PhoneNum;
        public ProfileWindowViewModel() 
        {
        
        }
    }
}
