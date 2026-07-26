namespace Sawmill.Domain.Formatting;

public interface IThemeProvider
{
    IObservable<Theme> Theme { get; }
    IObservable<Hue> Accent { get; }

}