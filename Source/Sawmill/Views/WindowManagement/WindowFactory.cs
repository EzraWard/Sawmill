using Sawmill.Domain.Infrastructure;

namespace Sawmill.Views.WindowManagement;

public class WindowFactory : IWindowFactory
{
    private readonly IObjectProvider _objectProvider;

    public WindowFactory(IObjectProvider objectProvider)
    {
        _objectProvider = objectProvider;
    }

    public MainWindow Create(IEnumerable<string> files = null)
    {
        var window = new MainWindow();
        var model = Attach(window);
        model.OpenFiles(files);
        return window;
    }

    public WindowViewModel Attach(MainWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        var model = _objectProvider.Get<WindowViewModel>();
        window.DataContext = model;

        window.Closing += (sender, e) =>
        {
            var todispose = ((MainWindow) sender).DataContext as IDisposable;
            todispose?.Dispose();
        };

        return model;
    }
}
