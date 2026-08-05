using System.IO;
using System.Xml.Linq;
using Microsoft.Win32;
using Sawmill.Domain.Formatting;
using System.Windows;
using System.Windows.Media;
using WpfThemeMode = System.Windows.ThemeMode;

namespace Sawmill.Views.Formatting;

internal static class StartupThemeLoader
{
    public static void ApplySavedTheme()
    {
        var theme = ReadSavedTheme();
        Application.Current.ThemeMode = theme switch
        {
            Theme.Light => WpfThemeMode.Light,
            Theme.Dark => WpfThemeMode.Dark,
            _ => WpfThemeMode.System
        };

        var isDark = theme switch
        {
            Theme.Light => false,
            Theme.Dark => true,
            _ => IsSystemDarkTheme()
        };

        var resources = Application.Current.Resources;
        AddBrush(resources, "MaterialDesignBackground", isDark ? 0x20 : 0xF3, isDark ? 0x20 : 0xF3, isDark ? 0x20 : 0xF3);
        AddBrush(resources, "MaterialDesignBody", isDark ? 0xF5 : 0x1A, isDark ? 0xF5 : 0x1A, isDark ? 0xF5 : 0x1A);
        AddBrush(resources, "ToolbarBackgroundBrush", isDark ? 0x24 : 0xF7, isDark ? 0x24 : 0xF7, isDark ? 0x24 : 0xF7);
        AddBrush(resources, "ContentAreaBrush", isDark ? 0x2B : 0xFF, isDark ? 0x2B : 0xFF, isDark ? 0x2B : 0xFF);
        AddBrush(resources, "PrimaryHueMidBrush", 0x00, isDark ? 0x6C : 0x5F, isDark ? 0xBD : 0xB8);
        AddBrush(resources, "PrimaryHueMidForegroundBrush", 0xFF, 0xFF, 0xFF);
        resources["IsDarkTheme"] = isDark;
    }

    internal static Theme ReadSavedTheme(string filePath = null)
    {
        try
        {
            filePath ??= Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Sawmill",
                "GeneralOptions.setting");

            if (!File.Exists(filePath))
                return Theme.System;

            var setting = XDocument.Load(filePath);
            var stateText = setting.Root?.Element("State")?.Value;
            if (string.IsNullOrWhiteSpace(stateText))
                return Theme.System;

            var options = XDocument.Parse(stateText);
            var themeText = options.Root?.Element("Theme")?.Value;
            return Enum.TryParse<Theme>(themeText, true, out var theme)
                ? theme
                : Theme.System;
        }
        catch
        {
            return Theme.System;
        }
    }

    private static void AddBrush(ResourceDictionary resources, string key, int red, int green, int blue)
    {
        resources[key] = new SolidColorBrush(Color.FromRgb((byte)red, (byte)green, (byte)blue));
    }

    private static bool IsSystemDarkTheme()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            return key?.GetValue("AppsUseLightTheme") is int value && value == 0;
        }
        catch
        {
            return true;
        }
    }
}
