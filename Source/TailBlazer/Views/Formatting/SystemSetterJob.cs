using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Windows;
using System.Windows.Media.Animation;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using TailBlazer.Domain.Formatting;
using TailBlazer.Domain.Infrastructure;
using TailBlazer.Domain.Ratings;
using TailBlazer.Domain.Settings;
using Hue = MaterialDesignColors.Hue;
using UserTheme = TailBlazer.Domain.Formatting.Theme;

namespace TailBlazer.Views.Formatting;

public sealed class SystemSetterJob: IDisposable
{
    private readonly IDisposable _cleanUp;

    public SystemSetterJob(ISetting<GeneralOptions> setting,
        IRatingService ratingService,
        ISchedulerProvider schedulerProvider)
    {
        var swatches = new SwatchesProvider().Swatches.ToDictionary(s => s.Name);

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

                ModifyTheme(theme => theme.SetBaseTheme(isDark ? MaterialDesignThemes.Wpf.Theme.Dark : MaterialDesignThemes.Wpf.Theme.Light));
                ApplyAccent(isDark ? swatches["yellow"] : swatches["indigo"]);
            });

        var frameRate = ratingService.Metrics
            .Take(1)
            .Select(metrics => metrics.FrameRate)
            .Wait();

        schedulerProvider.MainThread.Schedule(() =>
        {
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = frameRate });

        });

        _cleanUp = new CompositeDisposable( themeSetter);
    }

    private static void ApplyAccent(Swatch swatch)
    {
        if (swatch.AccentExemplarHue is Hue accentHue)
        {
            ModifyTheme(theme => theme.SetSecondaryColor(accentHue.Color));
        }
    }

    private static void ModifyTheme(Action<ITheme> modificationAction)
    {
        PaletteHelper paletteHelper = new PaletteHelper();
        ITheme theme = paletteHelper.GetTheme();

        modificationAction?.Invoke(theme);

        paletteHelper.SetTheme(theme);
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