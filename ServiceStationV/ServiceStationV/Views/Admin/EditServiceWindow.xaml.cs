using ServiceStationV.Models;
using ServiceStationV.Repositories;
using ServiceStationV.ViewsModels;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace ServiceStationV.Views.Admin
{
    public partial class EditServiceWindow : Window
    {
        private Service _service;
        private AdminViewModel _adminViewModel;
        public EditServiceWindow(Service service, AdminViewModel adminViewModel)
        {
            InitializeComponent();
            _adminViewModel = adminViewModel;
            LoadServiceAsync(service.ServiceId);

        }

        private async void LoadServiceAsync(int serviceId)
        {
            try
            {
                _service = await ServiceRepository.GetFullServiceById(serviceId);

                if (_service == null)
                {
                    MessageBox.Show("Услуга не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                ServiceNameTextBox.Text = _service.ServiceName;
                ServiceNameENTextBox.Text = _service.ServiceNameEN;
                DescriptionTextBox.Text = _service.SmallDescription;
                DescriptionENTextBox.Text = _service.SmallDescriptionEN;
                LargeDescriptionTextBox.Text = _service.LargeDescription;
                LargeDescriptionENTextBox.Text = _service.LargeDescriptionEN;
                PriceTextBox.Text = _service.Price.ToString();
                ImageSrcTextBox.Text = _service.ImageSrc;

                ServiceTypeComboBox.SelectedIndex = (int)_service.ServiceType;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_service == null)
                {
                    MessageBox.Show("Услуга не загружена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _service.ServiceName = ServiceNameTextBox.Text;
                _service.ServiceNameEN = ServiceNameENTextBox.Text;
                _service.SmallDescription = DescriptionTextBox.Text;
                _service.SmallDescriptionEN = DescriptionENTextBox.Text;
                _service.LargeDescription = LargeDescriptionTextBox.Text;
                _service.LargeDescriptionEN = LargeDescriptionENTextBox.Text;

                if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
                {
                    MessageBox.Show("Некорректное значение цены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                _service.Price = price;

                _service.ImageSrc = ImageSrcTextBox.Text;

                var selectedServiceType = ServiceTypeComboBox.SelectedItem as ComboBoxItem;
                if (selectedServiceType == null)
                {
                    MessageBox.Show("Выберите тип услуги.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                _service.ServiceType = (ServiceTypes)Enum.Parse(typeof(ServiceTypes), selectedServiceType.Content.ToString());

                if (await ServiceRepository.UpdateService(_service))
                {
                    MessageBox.Show("Услуга успешно обновлена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    await _adminViewModel.RefreshServicesAsync();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ImageBorder_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Any(IsImageFile))
                {
                    e.Effects = DragDropEffects.Copy;
                    ImageBorder.BorderBrush = Brushes.Blue;
                    return;
                }
            }
            e.Effects = DragDropEffects.None;
        }

        // Обработка перетаскивания над областью
        private void ImageBorder_DragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        // Обработка завершения перетаскивания
        private void ImageBorder_Drop(object sender, DragEventArgs e)
        {
            ImageBorder.BorderBrush = Brushes.Gray;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var imageFile = files.FirstOrDefault(IsImageFile);

                if (imageFile != null)
                {
                    LoadImageToBorder(imageFile);
                }
            }
        }

        // Проверка, является ли файл изображением
        private bool IsImageFile(string filePath)
        {
            var extensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
            return extensions.Contains(Path.GetExtension(filePath).ToLower());
        }

        // Загрузка изображения в Border и обновление пути
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

                // Генерируем уникальное имя файла
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imagePath);
                string destinationPath = System.IO.Path.Combine(imagesFolder, fileName);

                File.Copy(imagePath, destinationPath, true);



                if (ImageBorder.Child is TextBlock textBlock)
                    textBlock.Text = "Изображение успешно загружено";

                // Сохраняем путь к изображению
                ImageSrcTextBox.Text = $"pack://siteoforigin:,,,/images/ServicesImages/{fileName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
            }
        }
    
}
}