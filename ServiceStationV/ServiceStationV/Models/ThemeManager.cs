using System;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using MessageBox = ServiceStationV.Views.MessageBox;

namespace ServiceStationV.Models
{
    public static class ThemeManager
    {
        private static string _currentTheme;
        public static event Action ThemeChanged;

        public static List<string> AvailableThemes { get; } = new List<string>
        {
            "M",
            "RS",
            "AMG"
        };

        public static string CurrentTheme
        {
            get => _currentTheme ?? AvailableThemes.First();
            private set
            {
                if (_currentTheme == value) return;
                _currentTheme = value;
                ThemeChanged?.Invoke();
            }
        }

        public static void Initialize()
        {
            LoadTheme("M");
        }

        public static void LoadTheme(string themeName)
        {
            try
            {
                if (!AvailableThemes.Contains(themeName))
                    throw new ArgumentException($"Theme '{themeName}' not found");

                var themeDictionaries = Application.Current.Resources.MergedDictionaries
                    .Where(d => d.Source?.OriginalString.Contains("-Theme.xaml") == true)
                    .ToList();

                foreach (var dictt in themeDictionaries)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(dictt);
                }

                var dict = new ResourceDictionary
                {
                    Source = new Uri($"/Themes/{themeName}-Theme.xaml", UriKind.Relative)
                };

                Application.Current.Resources.MergedDictionaries.Add(dict);

                CurrentTheme = themeName;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading theme: {ex.Message}");
                LoadFallbackTheme();
            }
        }

        public static void ToggleTheme()
        {
            var currentIndex = AvailableThemes.IndexOf(CurrentTheme);
            var newIndex = (currentIndex + 1) % AvailableThemes.Count;
            LoadTheme(AvailableThemes[newIndex]);
        }

        private static void LoadFallbackTheme()
        {
            try
            {
                var dict = new ResourceDictionary
                {
                    Source = new Uri("/Themes/BMW-M-Theme.xaml", UriKind.Relative)
                };
                Application.Current.Resources.MergedDictionaries.Add(dict);
                CurrentTheme = "BMW-M";
            }
            catch
            {
                Application.Current.Resources.MergedDictionaries.Clear();
            }
        }


        public static ResourceDictionary GetCurrentTheme()
        {
            return Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source?.OriginalString.Contains("-Theme.xaml") == true);
        }
    }
}