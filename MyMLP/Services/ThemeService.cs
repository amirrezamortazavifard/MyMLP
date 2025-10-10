using System;
using System.Linq;
using System.Windows;

namespace MyMLP.Services
{
    public static class ThemeService
    {
        public enum Theme { Light, Dark }

        public static void ApplyTheme(Theme theme)
        {
            var app = Application.Current;
            if (app == null) return;

            app.Resources.MergedDictionaries.Clear();

            ResourceDictionary themeDictionary = new ResourceDictionary();

            switch (theme)
            {
                case Theme.Light:
                    themeDictionary.Source = new Uri("Themes/LightTheme.xaml", UriKind.Relative);
                    break;
                case Theme.Dark:
                default:
                    themeDictionary.Source = new Uri("Themes/DarkTheme.xaml", UriKind.Relative);
                    break;
            }

            app.Resources.MergedDictionaries.Add(themeDictionary);

            ResourceDictionary converters = new ResourceDictionary
            {
                Source = new Uri("Converters/Converters.xaml", UriKind.Relative)
            };
            app.Resources.MergedDictionaries.Add(converters);
        }
    }
}