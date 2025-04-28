using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ServiceStationV.Models;
using System.Windows.Controls;
using System.IO;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ServiceStationV.ViewsModels;
using ServiceStationV.Repositories;
using MessageBox = ServiceStationV.Views.MessageBox;

namespace ServiceStationV.Views.Admin
{
    /// <summary>
    /// Логика взаимодействия для AddService.xaml
    /// </summary>
    public partial class AddServiceWindow : Window
    {
        AdminViewModel _adminViewModel;
        public AddServiceWindow(AdminViewModel adminViewModel)
        {
            _adminViewModel = adminViewModel;
            InitializeComponent();
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Service service = new();
                service.ServiceName = ServiceNameTextBox.Text;
                service.ServiceNameEN = ServiceNameENTextBox.Text;
                service.SmallDescription = DescriptionTextBox.Text;
                service.SmallDescriptionEN = DescriptionENTextBox.Text;
                service.LargeDescription = LargeDescriptionTextBox.Text;
                service.LargeDescriptionEN = LargeDescriptionENTextBox.Text;
                if (decimal.TryParse(PriceTextBox.Text, out decimal parsedPrice))
                {
                    service.Price = parsedPrice;
                }
                else
                    throw new Exception("Ошибка в строке цены");
                service.ImageSrc = ImageSrcTextBox.Text;
                if (service.ValidateService())
                {
                    ServiceRepository.AddService(service);
                    _adminViewModel.ViewServices.Refresh();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении услуги: {ex.Message}", "Ошибка", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }

        }

        private void ImageBorder_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Any(file => IsImageFile(file)))
                {
                    e.Effects = DragDropEffects.Copy;
                    imageBorder.BorderBrush = Brushes.Blue;
                    return;
                }
            }
            e.Effects = DragDropEffects.None;
        }

        private void ImageBorder_DragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void ImageBorder_Drop(object sender, DragEventArgs e)
        {
            imageBorder.BorderBrush = Brushes.Gray;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var imageFile = files.FirstOrDefault(file => IsImageFile(file));

                if (imageFile != null)
                {
                    LoadImageToBorder(imageFile);
                }
            }
        }

        private bool IsImageFile(string filePath)
        {
            var extensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
            return extensions.Contains(Path.GetExtension(filePath).ToLower());
        }

        private void LoadImageToBorder(string imagePath)
        {
            try
            {

                string projectPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string imagesFolder = System.IO.Path.Combine(projectPath, "images/ServicesImages");

                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }

                string fileName = System.IO.Path.GetFileName(imagePath);
                string destinationPath = System.IO.Path.Combine(imagesFolder, fileName);

                File.Copy(imagePath, destinationPath, true);

                if (imageBorder.Child is TextBlock textBlock)
                    textBlock.Text = "Изображение успешно загружено";
                ImageSrcTextBox.Text = Path.Combine("pack://siteoforigin:,,,/images/ServicesImages", fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
            }
        }
    }
}
