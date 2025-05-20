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
using ServiceStationV.ViewableData;
using ServiceStationV.Repositories;
using MessageBox = ServiceStationV.Views.MessageBox;

namespace ServiceStationV.Views.Admin
{
    public partial class AddServiceWindow : Window
    {
        AdminViewModel _adminViewModel;

        public AddServiceWindow(AdminViewModel adminViewModel)
        {
            try
            {
                _adminViewModel = adminViewModel;
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации окна: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при закрытии окна: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Service service = new();

                service.ServiceName = ServiceNameTextBox.Text?.Trim();
                service.ServiceNameEN = ServiceNameENTextBox.Text?.Trim();
                service.SmallDescription = DescriptionTextBox.Text?.Trim();
                service.SmallDescriptionEN = DescriptionENTextBox.Text?.Trim();
                service.LargeDescription = LargeDescriptionTextBox.Text?.Trim();
                service.LargeDescriptionEN = LargeDescriptionENTextBox.Text?.Trim();

                if (!decimal.TryParse(PriceTextBox.Text, out decimal parsedPrice))
                    throw new FormatException("Неверный формат цены");

                service.Price = parsedPrice;
                service.ImageSrc = ImageSrcTextBox.Text?.Trim();

                if (service.ValidateService())
                {
                    try
                    {
                        ServiceRepository.AddService(service);
                        _adminViewModel.ViewServices.Refresh();
                        _adminViewModel.RefreshServicesAsync();

                        this.Close();
                    }
                    catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                    {
                        MessageBox.Show($"Ошибка при работе с базой данных или файлами: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (FormatException fex)
            {
                MessageBox.Show(fex.Message, "Ошибка формата", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении услуги: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImageBorder_DragEnter(object sender, DragEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке перетаскивания: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Effects = DragDropEffects.None;
            }
        }

        private void ImageBorder_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при DragOver: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImageBorder_Drop(object sender, DragEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при Drop: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsImageFile(string filePath)
        {
            try
            {
                var extensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
                string ext = Path.GetExtension(filePath).ToLower();
                return !string.IsNullOrEmpty(ext) && extensions.Contains(ext);
            }
            catch
            {
                return false;
            }
        }

        private void LoadImageToBorder(string imagePath)
        {
            try
            {
                string projectPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string imagesFolder = System.IO.Path.Combine(projectPath, "images", "ServicesImages");

                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }

                string fileName = Path.GetFileName(imagePath);
                string destinationPath = Path.Combine(imagesFolder, fileName);

                File.Copy(imagePath, destinationPath, overwrite: true);

                if (imageBorder.Child is TextBlock textBlock)
                {
                    textBlock.Text = "Изображение успешно загружено";
                }

                ImageSrcTextBox.Text = Path.Combine("pack://siteoforigin:,,,/images/ServicesImages", fileName);
            }
            catch (UnauthorizedAccessException uex)
            {
                MessageBox.Show($"Нет прав на запись файла: {uex.Message}", "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException ioex)
            {
                MessageBox.Show($"Ошибка ввода-вывода: {ioex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}