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
                var isDark = userTheme == UserTheme.Dark;

                ModifyTheme(theme => theme.SetBaseTheme(isDark ? BaseTheme.Dark : BaseTheme.Light));
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
        if (swatch.ExemplarHue is Hue accentHue)
        {
            ModifyTheme(theme => theme.SetSecondaryColor(accentHue.Color));
        }
    }

    private static void ModifyTheme(Action<MaterialDesignThemes.Wpf.Theme> modificationAction)
    {
        PaletteHelper paletteHelper = new PaletteHelper();
        MaterialDesignThemes.Wpf.Theme theme = paletteHelper.GetTheme();

        modificationAction?.Invoke(theme);

        paletteHelper.SetTheme(theme);
    }


    public void Dispose()
    {
        _cleanUp.Dispose();
    }
}