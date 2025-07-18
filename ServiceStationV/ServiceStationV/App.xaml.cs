﻿using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Navigation;
using System.Data.SqlClient;
using System;
using BCrypt.Net;
using Microsoft.Data.SqlClient;
using static ServiceStationV.ValidationAttributes.UserValidationAttributes;
using ServiceStationV.Models;
using MessageBox = ServiceStationV.Views.MessageBox;

namespace ServiceStationV
{
    public partial class App : Application
    {
        public static string conStr = @"Server=ROMAN; Database=ServiceStationDB; Integrated Security = true;TrustServerCertificate=True;";
        public static string AdminLogin = "admin";
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ThemeManager.Initialize();
            LocalizationManager.SetLanguage(LocalizationManager.SupportedCultures[0]);

        }
    }

}
