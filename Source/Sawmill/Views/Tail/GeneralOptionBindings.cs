using System.Reactive.Disposables;
using System.Reactive.Linq;
using Sawmill.Domain.Annotations;
using Sawmill.Domain.Formatting;
using Sawmill.Domain.Infrastructure;
using Sawmill.Domain.Settings;

namespace Sawmill.Views.Tail;

public class GeneralOptionBindings: IDisposable
{
    public IProperty<bool> HighlightTail { get; }
    public IProperty<bool> UsingDarkTheme { get; }
    public IProperty<bool> ShowLineNumbers { get; }
        
    private readonly IDisposable _cleanUp;

    public GeneralOptionBindings([NotNull] ISetting<GeneralOptions> generalOptions, ISchedulerProvider schedulerProvider)
    {
        UsingDarkTheme = generalOptions.Value
            .ObserveOn(schedulerProvider.MainThread)
            .Select(options => options.Theme != Theme.Light)
            .ForBinding();

        HighlightTail = generalOptions.Value
            .ObserveOn(schedulerProvider.MainThread)
            .Select(options => options.HighlightTail)
            .ForBinding();

        ShowLineNumbers = generalOptions.Value
            .ObserveOn(schedulerProvider.MainThread)
            .Select(options => options.ShowLineNumbers)
            .ForBinding();

        _cleanUp = new CompositeDisposable(UsingDarkTheme, HighlightTail, ShowLineNumbers);
    }

    public void Dispose()
    {
        _cleanUp.Dispose();
    }
}