using ServiceStationV.Models;
using ServiceStationV.Repositories;
using ServiceStationV.ViewsModels;
using System;
using System.Windows;
using System.Windows.Input;

namespace ServiceStationV{
    /// <summary>
    /// Логика взаимодействия для ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        public ProfileWindow()
        {
            InitializeComponent();
            this.DataContext = new ProfileWindowViewModel();
        }
        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
            this.Activate();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        private void GetBackBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
