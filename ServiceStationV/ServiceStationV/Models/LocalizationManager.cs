using System.Windows;
using System.Globalization;
using System.Linq;

public static class LocalizationManager
{
    public static CultureInfo CurrentCulture { get; private set; } = CultureInfo.CurrentCulture;
    public static bool IsEnglish => CurrentCulture.TwoLetterISOLanguageName == "en";


    public static readonly CultureInfo[] SupportedCultures = {
        new CultureInfo("en-US"),
        new CultureInfo("ru-RU")
    };

    public static event EventHandler LanguageChanged;

    public static void SetLanguage(CultureInfo culture)
    {
        if (!SupportedCultures.Contains(culture))
            culture = SupportedCultures[0];

        CurrentCulture = culture;
        UpdateResourceDictionary();
        LanguageChanged?.Invoke(null, EventArgs.Empty); ;
    }

    private static void UpdateResourceDictionary()
    {
        var dict = new ResourceDictionary
        {
            Source = new Uri($"/Localization/Strings.{CurrentCulture.Name}.xaml",
                          UriKind.RelativeOrAbsolute)
        };

        var oldDict = Application.Current.Resources.MergedDictionaries
            .FirstOrDefault(d => d.Source?.OriginalString.StartsWith("/Localization/Strings.") == true);

        if (oldDict != null)
            Application.Current.Resources.MergedDictionaries.Remove(oldDict);

        Application.Current.Resources.MergedDictionaries.Add(dict);
    }

    public static string GetString(string key)
    {
        return Application.Current.TryFindResource(key) as string ?? $"[{key}]";
    }
}