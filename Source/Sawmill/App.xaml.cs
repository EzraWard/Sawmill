using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using StructureMap;
using Sawmill.Infrastructure;
using Sawmill.Infrastructure.AppState;
using Sawmill.Views.Formatting;
using Sawmill.Views.Layout;
using Sawmill.Views.WindowManagement;

namespace Sawmill;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FreeConsole();

    protected override void OnStartup(StartupEventArgs e)
    {
        FreeConsole();

        var dispatcher = Dispatcher.CurrentDispatcher;
        StartupThemeLoader.ApplySavedTheme();

        var startupShell = new StartupShellViewModel(e.Args);
        var startupWindow = new StartupWindow { DataContext = startupShell };
        startupWindow.Show();

        EventHandler firstFrameRendered = null;
        firstFrameRendered = (_, _) =>
        {
            startupWindow.ContentRendered -= firstFrameRendered;
            var containerTask = Task.Run(() => CreateContainer(dispatcher));
            _ = HydrateSessionAsync(containerTask, startupWindow, startupShell);
        };
        startupWindow.ContentRendered += firstFrameRendered;

        base.OnStartup(e);
    }

    private static IContainer CreateContainer(Dispatcher dispatcher)
    {
        var container = new Container(x => x.AddRegistry<AppRegistry>());
        container.Configure(x => x.For<Dispatcher>().Add(dispatcher));
        return container;
    }

    private async Task HydrateSessionAsync(
        Task<IContainer> containerTask,
        StartupWindow startupWindow,
        StartupShellViewModel startupShell)
    {
        try
        {
            var container = await containerTask;
            if (!startupWindow.IsVisible)
            {
                container.Dispose();
                return;
            }

            // These services touch WPF resources and therefore resolve on the UI thread.
            container.GetInstance<StartupController>();
            LoadApplicationResources();

            var factory = container.GetInstance<WindowFactory>();
            var window = new MainWindow
            {
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = startupWindow.Left,
                Top = startupWindow.Top,
                Width = startupWindow.ActualWidth,
                Height = startupWindow.ActualHeight,
                WindowState = startupWindow.WindowState
            };
            var model = factory.Attach(window);
            startupShell.ReplayActions(model);

            var appStatePublisher = container.GetInstance<IApplicationStatePublisher>();
            Exit += (_, _) => appStatePublisher.Publish(ApplicationState.ShuttingDown);

            window.Show();
            startupWindow.Close();

            // Give the hydrated Start tab a render opportunity before restoring
            // potentially expensive file tabs from the previous session.
            await Dispatcher.Yield(DispatcherPriority.Background);
            container.GetInstance<ILayoutService>().Restore();
        }
        catch (Exception ex)
        {
            new SimpleFileLogger(typeof(App)).Error(ex, "Startup hydration failed");
            MessageBox.Show(
                "Sawmill could not finish starting. See the application log for details.",
                "Sawmill",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(-1);
        }
    }

    private void LoadApplicationResources()
    {
        Resources.MergedDictionaries.Add(new ResourceDictionary
        {
            Source = new Uri(
                "pack://application:,,,/Sawmill;component/Themes/ApplicationResources.xaml",
                UriKind.Absolute)
        });
    }
}
