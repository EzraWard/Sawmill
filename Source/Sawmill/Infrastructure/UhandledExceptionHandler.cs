using System.Windows;
using System.Windows.Threading;
using Sawmill.Domain.Infrastructure;

namespace Sawmill.Infrastructure;

public class UhandledExceptionHandler
{
    private readonly ILogger _logger;

    public UhandledExceptionHandler(ILogger logger)
    {
        _logger = logger;

        Application.Current.DispatcherUnhandledException += CurrentDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
    }

    private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = (Exception)e.ExceptionObject;
        _logger.Error(ex, ex.Message);
    }

    private void CurrentDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var ex = e.Exception;
        _logger.Error(ex, ex.Message);
        e.Handled = true;

        // If no window is visible the app is effectively headless — shut down rather than
        // leaving an invisible process running (which causes Store certification failures).
        if (!Application.Current.Windows.OfType<Window>().Any(w => w.IsVisible))
            Application.Current.Shutdown();
    }

}