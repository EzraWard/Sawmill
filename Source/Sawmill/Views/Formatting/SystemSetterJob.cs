using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Sawmill;
using Sawmill.Controls;
using Sawmill.Domain.Formatting;
using Sawmill.Domain.Infrastructure;
using Sawmill.Domain.Ratings;
using Sawmill.Domain.Settings;
using UserTheme = Sawmill.Domain.Formatting.Theme;

namespace Sawmill.Views.Formatting;

public sealed class SystemSetterJob: IDisposable
{
    private readonly IDisposable _cleanUp;

    public SystemSetterJob(ISetting<GeneralOptions> setting,
        IRatingService ratingService,
        ISchedulerProvider schedulerProvider)
    {
        var themeSetter = setting.Value.Select(options => options.Theme)
            .DistinctUntilChanged()
            .ObserveOn(schedulerProvider.MainThread)
            .Subscribe(userTheme =>
            {
                var isDark = userTheme switch
                {
                    UserTheme.Light => false,
                    UserTheme.Dark => true,
                    _ => IsSystemDarkTheme()
                };

                ApplyTheme(isDark);
            });

        var frameRate = ratingService.Metrics
            .Take(1)
            .Select(metrics => metrics.FrameRate)
            .Wait();

        schedulerProvider.MainThread.Schedule(() =>
        {
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = frameRate });
        });

        _cleanUp = new CompositeDisposable(themeSetter);
    }

    private static void ApplyTheme(bool isDark)
    {
        var resources = Application.Current.Resources;

        if (isDark)
        {
            // Core palette
            SetBrush(resources, "MaterialDesignBackground", "#FF1F2024");
            SetBrush(resources, "MaterialDesignPaper", "#FF2A2D33");
            SetBrush(resources, "MaterialDesignBody", "#FFF3F3F3");
            SetBrush(resources, "MaterialDesignTextBoxBorder", "#66FFFFFF");
            SetBrush(resources, "MaterialDesignFlatButtonClick", "#22FFFFFF");

            // Primary / secondary
            SetBrush(resources, "PrimaryHueMidBrush", "#FF3C82F6");
            SetBrush(resources, "PrimaryHueMidForegroundBrush", "#FFFFFFFF");
            SetBrush(resources, "PrimaryHueDarkBrush", "#FF1E4F9E");
            SetBrush(resources, "PrimaryHueDarkForegroundBrush", "#FFF4F6FB");
            SetBrush(resources, "PrimaryHueLightBrush", "#FF78A9FF");
            SetBrush(resources, "PrimaryHueLightForegroundBrush", "#FF0E1624");
            SetBrush(resources, "SecondaryHueMidBrush", "#FF46B5A7");
            SetBrush(resources, "SecondaryHueMidForegroundBrush", "#FF06211E");
            SetBrush(resources, "ValidationErrorBrush", "#FFE85A5A");
            SetBrush(resources, "GrayBrush2", "#888888");

            // Settings
            SetBrush(resources, "SettingsMicaBrush", "#CC202124");
            SetBrush(resources, "SettingsCardBrush", "#B324272E");
            SetBrush(resources, "SettingsCardBorderBrush", "#66FFFFFF");

            // Tab / titlebar
            SetBrush(resources, "TabSelectedBrush", "#FF282828");
            SetBrush(resources, "TabHoverBrush", "#15FFFFFF");
            SetBrush(resources, "CaptionButtonHoverBrush", "#22FFFFFF");
            SetBrush(resources, "ContentAreaBrush", "#FF282828");

            // Settings card states
            SetBrush(resources, "SettingsCardBackgroundBrush", "#0DFFFFFF");
            SetBrush(resources, "SettingsCardBackgroundPointerOverBrush", "#15FFFFFF");
            SetBrush(resources, "SettingsCardBackgroundPressedBrush", "#08FFFFFF");
            SetBrush(resources, "SettingsCardBackgroundDisabledBrush", "#05FFFFFF");
            SetBrush(resources, "SettingsCardStrokeBrush", "#19FFFFFF");
            SetBrush(resources, "SettingsCardStrokePointerOverBrush", "#24FFFFFF");
            SetBrush(resources, "SettingsCardDescriptionForegroundBrush", "#9EFFFFFF");
        }
        else
        {
            // Core palette — light mode
            SetBrush(resources, "MaterialDesignBackground", "#FFF3F3F3");
            SetBrush(resources, "MaterialDesignPaper", "#FFFFFFFF");
            SetBrush(resources, "MaterialDesignBody", "#FF1A1A1A");
            SetBrush(resources, "MaterialDesignTextBoxBorder", "#33000000");
            SetBrush(resources, "MaterialDesignFlatButtonClick", "#15000000");

            // Primary / secondary (keep accent, adjust foregrounds)
            SetBrush(resources, "PrimaryHueMidBrush", "#FF3C82F6");
            SetBrush(resources, "PrimaryHueMidForegroundBrush", "#FFFFFFFF");
            SetBrush(resources, "PrimaryHueDarkBrush", "#FF1E4F9E");
            SetBrush(resources, "PrimaryHueDarkForegroundBrush", "#FF1A1A1A");
            SetBrush(resources, "PrimaryHueLightBrush", "#FF78A9FF");
            SetBrush(resources, "PrimaryHueLightForegroundBrush", "#FF0E1624");
            SetBrush(resources, "SecondaryHueMidBrush", "#FF46B5A7");
            SetBrush(resources, "SecondaryHueMidForegroundBrush", "#FF06211E");
            SetBrush(resources, "ValidationErrorBrush", "#FFD32F2F");
            SetBrush(resources, "GrayBrush2", "#FF888888");

            // Settings
            SetBrush(resources, "SettingsMicaBrush", "#CCF3F3F3");
            SetBrush(resources, "SettingsCardBrush", "#FFFFFFFF");
            SetBrush(resources, "SettingsCardBorderBrush", "#FFE0E0E0");

            // Tab / titlebar
            SetBrush(resources, "TabSelectedBrush", "#FFFFFFFF");
            SetBrush(resources, "TabHoverBrush", "#0A000000");
            SetBrush(resources, "CaptionButtonHoverBrush", "#15000000");
            SetBrush(resources, "ContentAreaBrush", "#FFFFFFFF");

            // Settings card states
            SetBrush(resources, "SettingsCardBackgroundBrush", "#FFFBFBFB");
            SetBrush(resources, "SettingsCardBackgroundPointerOverBrush", "#FFF5F5F5");
            SetBrush(resources, "SettingsCardBackgroundPressedBrush", "#FFEFEFEF");
            SetBrush(resources, "SettingsCardBackgroundDisabledBrush", "#FFF9F9F9");
            SetBrush(resources, "SettingsCardStrokeBrush", "#FFE0E0E0");
            SetBrush(resources, "SettingsCardStrokePointerOverBrush", "#FFD0D0D0");
            SetBrush(resources, "SettingsCardDescriptionForegroundBrush", "#99000000");
        }

        // Update DWM dark mode on all SawmillWindow instances
        foreach (Window window in Application.Current.Windows)
        {
            if (window is SawmillWindow sawmillWindow)
            {
                SawmillWindow.SetDwmDarkMode(sawmillWindow, isDark);
            }
            else if (window is MainWindow mainWindow)
            {
                MainWindow.SetDwmDarkMode(mainWindow, isDark);
            }
        }
    }

    private static void SetBrush(ResourceDictionary resources, string key, string colorHex)
    {
        var color = (Color)ColorConverter.ConvertFromString(colorHex);
        if (resources[key] is SolidColorBrush existing && !existing.IsFrozen)
        {
            existing.Color = color;
        }
        else
        {
            resources[key] = new SolidColorBrush(color);
        }
    }

    private static bool IsSystemDarkTheme()
    {
        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            var value = key?.GetValue("AppsUseLightTheme");
            return value is int i && i == 0;
        }
        catch
        {
            return true;
        }
    }

    public void Dispose()
    {
        _cleanUp.Dispose();
    }
}
