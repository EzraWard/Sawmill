using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using Sawmill;
using Sawmill.Controls;
using Sawmill.Domain.Formatting;
using Sawmill.Domain.Infrastructure;
using Sawmill.Domain.Ratings;
using Sawmill.Domain.Settings;
using UserTheme = Sawmill.Domain.Formatting.Theme;
using WpfThemeMode = System.Windows.ThemeMode;

namespace Sawmill.Views.Formatting;

public sealed class SystemSetterJob: IDisposable
{
    private readonly IDisposable _cleanUp;
    private UserTheme _selectedTheme;

    public SystemSetterJob(ISetting<GeneralOptions> setting,
        IRatingService ratingService,
        ISchedulerProvider schedulerProvider)
    {
        var themeSetter = setting.Value.Select(options => options.Theme)
            .DistinctUntilChanged()
            .ObserveOn(schedulerProvider.MainThread)
            .Subscribe(userTheme =>
            {
                _selectedTheme = userTheme;
                ApplyTheme(userTheme);
            });

        UserPreferenceChangedEventHandler systemThemeChanged = (_, args) =>
        {
            if (_selectedTheme != UserTheme.System ||
                args.Category is not (UserPreferenceCategory.General or
                    UserPreferenceCategory.VisualStyle or
                    UserPreferenceCategory.Color))
            {
                return;
            }

            schedulerProvider.MainThread.Schedule(() =>
            {
                if (_selectedTheme == UserTheme.System)
                    ApplyCustomPalette(IsSystemDarkTheme());
            });
        };
        SystemEvents.UserPreferenceChanged += systemThemeChanged;

        var frameRate = ratingService.Metrics
            .Take(1)
            .Select(metrics => metrics.FrameRate)
            .Wait();

        schedulerProvider.MainThread.Schedule(() =>
        {
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = frameRate });
        });

        _cleanUp = new CompositeDisposable(
            themeSetter,
            Disposable.Create(() => SystemEvents.UserPreferenceChanged -= systemThemeChanged));
    }

    private static void ApplyTheme(UserTheme userTheme)
    {
        Application.Current.ThemeMode = userTheme switch
        {
            UserTheme.Light => WpfThemeMode.Light,
            UserTheme.Dark => WpfThemeMode.Dark,
            _ => WpfThemeMode.System
        };

        var isDark = userTheme switch
        {
            UserTheme.Light => false,
            UserTheme.Dark => true,
            _ => IsSystemDarkTheme()
        };

        ApplyCustomPalette(isDark);
    }

    private static void ApplyCustomPalette(bool isDark)
    {
        var resources = Application.Current.Resources;
        var palette = isDark ? ThemePalette.Dark : ThemePalette.Light;

        foreach (var color in palette.Colors)
            SetBrush(resources, color.Key, color.Value);

        resources["IsDarkTheme"] = isDark;

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

    private static void SetBrush(ResourceDictionary resources, string key, Color color)
    {
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
