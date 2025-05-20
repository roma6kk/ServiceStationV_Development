using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ServiceStationV.Views
{
    public partial class MessageBox : Window
    {

        public MessageBox()
        {
            InitializeComponent();
        }

        void AddButtons(MessageBoxButton buttons, MessageBoxResult defaultResult)
        {
            switch (buttons)
            {
                case MessageBoxButton.OK:
                    AddButton("OK", MessageBoxResult.OK, isDefault: true);
                    break;
                case MessageBoxButton.OKCancel:
                    AddButton("OK", MessageBoxResult.OK, isDefault: defaultResult == MessageBoxResult.OK);
                    AddButton("Cancel", MessageBoxResult.Cancel, isCancel: true, isDefault: defaultResult == MessageBoxResult.Cancel);
                    break;
                case MessageBoxButton.YesNo:
                    AddButton("Yes", MessageBoxResult.Yes, isDefault: defaultResult == MessageBoxResult.Yes);
                    AddButton("No", MessageBoxResult.No, isDefault: defaultResult == MessageBoxResult.No);
                    break;
                case MessageBoxButton.YesNoCancel:
                    AddButton("Yes", MessageBoxResult.Yes, isDefault: defaultResult == MessageBoxResult.Yes);
                    AddButton("No", MessageBoxResult.No, isDefault: defaultResult == MessageBoxResult.No);
                    AddButton("Cancel", MessageBoxResult.Cancel, isCancel: true, isDefault: defaultResult == MessageBoxResult.Cancel);
                    break;
                default:
                    throw new ArgumentException("Unknown button value", nameof(buttons));
            }
        }

        void AddButton(string text, MessageBoxResult result, bool isCancel = false, bool isDefault = false)
        {
            var button = new Button()
            {
                Content = text,
                IsCancel = isCancel,
                Margin = new Thickness(5, 0, 5, 0),
                Padding = new Thickness(10, 2, 10, 2),
                MinWidth = 80
            };

            if (isDefault)
            {
                button.IsDefault = true;
                button.Focus();
            }

            button.Click += (o, args) =>
            {
                Result = result;
                DialogResult = true;
            };

            ButtonContainer.Children.Add(button);
        }

        void SetImage(MessageBoxImage image)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();

            switch (image)
            {
                case MessageBoxImage.Error:
                    bitmap.UriSource = new Uri("pack://application:,,,/images/MessageBox/Info.png");
                    break;
                case MessageBoxImage.Question:
                    bitmap.UriSource = new Uri("pack://application:,,,/images/MessageBox/Question.png");
                    break;
                case MessageBoxImage.Warning:
                    bitmap.UriSource = new Uri("pack://application:,,,/images/MessageBox/Question.png");
                    break;
                case MessageBoxImage.Information:
                    bitmap.UriSource = new Uri("pack://application:,,,/images/MessageBox/Info.png");
                    break;
                default:
                    ImageContainer.Visibility = Visibility.Collapsed;
                    return;
            }

            bitmap.EndInit();
            ImageContainer.Source = bitmap;
            ImageContainer.Visibility = Visibility.Visible;
        }

        MessageBoxResult Result { get; set; } = MessageBoxResult.None;

        public static MessageBoxResult Show(string message)
        {
            return Show(Application.Current.MainWindow, message, string.Empty, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK);
        }

        public static MessageBoxResult Show(string message, string caption)
        {
            return Show(Application.Current.MainWindow, message, caption, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK);
        }

        public static MessageBoxResult Show(Window owner, string message, string caption)
        {
            return Show(owner, message, caption, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK);
        }

        public static MessageBoxResult Show(string message, string caption, MessageBoxButton buttons)
        {
            return Show(Application.Current.MainWindow, message, caption, buttons, MessageBoxImage.None, MessageBoxResult.OK);
        }

        public static MessageBoxResult Show(string message, string caption, MessageBoxButton buttons, MessageBoxImage image)
        {
            return Show(Application.Current.MainWindow, message, caption, buttons, image, MessageBoxResult.OK);
        }

        private static readonly string soundPath = System.IO.Path.GetFullPath(@"..\..\..\images\ErrorSound.wav");
        private static SoundPlayer player = new SoundPlayer(soundPath);

        public static MessageBoxResult Show(Window owner, string message, string caption, MessageBoxButton buttons, MessageBoxImage image, MessageBoxResult defaultResult)
        {
            var dialog = new MessageBox()
            {
                Title = caption,
                Owner = owner,
                WindowStartupLocation = owner == null ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner
            };

            dialog.MessageContainer.Text = message;
            dialog.SetImage(image);
            dialog.AddButtons(buttons, defaultResult);
            if (!System.IO.File.Exists(soundPath))
            {
                System.Windows.MessageBox.Show($"Файл не найден: {soundPath}");
                return System.Windows.MessageBoxResult.OK;
            }

            player.Play();

            dialog.ShowDialog();
            return dialog.Result;
        }
    }
}