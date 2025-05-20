using ServiceStationV.Models;
using ServiceStationV.Repositories;
using ServiceStationV.ViewableData;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using MessageBox = ServiceStationV.Views.MessageBox;

namespace ServiceStationV.Views.Admin
{
    public partial class EditServiceWindow : Window
    {
        private Service _service;
        private AdminViewModel _adminViewModel;

        public EditServiceWindow(Service service, AdminViewModel adminViewModel)
        {
            try
            {
                InitializeComponent();
                _adminViewModel = adminViewModel;
                LoadServiceAsync(service.ServiceId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации окна: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
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

                ServiceNameTextBox.Text = _service.ServiceName ?? string.Empty;
                ServiceNameENTextBox.Text = _service.ServiceNameEN ?? string.Empty;
                DescriptionTextBox.Text = _service.SmallDescription ?? string.Empty;
                DescriptionENTextBox.Text = _service.SmallDescriptionEN ?? string.Empty;
                LargeDescriptionTextBox.Text = _service.LargeDescription ?? string.Empty;
                LargeDescriptionENTextBox.Text = _service.LargeDescriptionEN ?? string.Empty;
                PriceTextBox.Text = _service.Price.ToString();

                ImageSrcTextBox.Text = _service.ImageSrc ?? string.Empty;

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

                _service.ServiceName = ServiceNameTextBox.Text?.Trim();
                _service.ServiceNameEN = ServiceNameENTextBox.Text?.Trim();
                _service.SmallDescription = DescriptionTextBox.Text?.Trim();
                _service.SmallDescriptionEN = DescriptionENTextBox.Text?.Trim();
                _service.LargeDescription = LargeDescriptionTextBox.Text?.Trim();
                _service.LargeDescriptionEN = LargeDescriptionENTextBox.Text?.Trim();

                if (!decimal.TryParse(PriceTextBox.Text?.Trim(), out decimal price))
                {
                    MessageBox.Show("Некорректное значение цены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                _service.Price = price;

                _service.ImageSrc = ImageSrcTextBox.Text?.Trim();

                var selectedServiceType = ServiceTypeComboBox.SelectedItem as ComboBoxItem;
                if (selectedServiceType == null)
                {
                    MessageBox.Show("Выберите тип услуги.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (Enum.TryParse(typeof(ServiceTypes), selectedServiceType.Content?.ToString(), out object? parsedType))
                {
                    _service.ServiceType = (ServiceTypes)parsedType;
                }
                else
                {
                    MessageBox.Show("Не удалось распознать тип услуги.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (!_service.ValidateService())
                    return;
                bool updated = await ServiceRepository.UpdateService(_service);
                if (updated)
                {
                    MessageBox.Show("Услуга успешно обновлена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    await _adminViewModel.RefreshServicesAsync();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Не удалось обновить услугу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (UnauthorizedAccessException uex)
            {
                MessageBox.Show($"Ошибка доступа: {uex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException ioex)
            {
                MessageBox.Show($"Ошибка ввода-вывода: {ioex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImageBorder_DragEnter(object sender, DragEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при DragEnter: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при Drop: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsImageFile(string filePath)
        {
            try
            {
                var extensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
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
                string projectPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string imagesFolder = Path.Combine(projectPath, "images", "ServicesImages");

                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }

                string fileName = Guid.NewGuid() + Path.GetExtension(imagePath);
                string destinationPath = Path.Combine(imagesFolder, fileName);

                File.Copy(imagePath, destinationPath, overwrite: true);

                if (ImageBorder.Child is TextBlock textBlock)
                {
                    textBlock.Text = "Изображение успешно загружено";
                }

                ImageSrcTextBox.Text = $"pack://siteoforigin:,,,/images/ServicesImages/{fileName}";
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