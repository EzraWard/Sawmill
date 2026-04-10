using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using StructureMap;
using Sawmill.Domain.Infrastructure;
using Sawmill.Infrastructure;
using Sawmill.Infrastructure.AppState;
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

        var container = new Container(x => x.AddRegistry<AppRegistry>());
        container.Configure(x => x.For<Dispatcher>().Add(Dispatcher.CurrentDispatcher));
        container.GetInstance<StartupController>();

        var factory = container.GetInstance<WindowFactory>();
        var window = factory.Create(e.Args);

        var layoutServce = container.GetInstance<ILayoutService>();
        layoutServce.Restore();
        window.Show();

        var appStatePublisher = container.GetInstance<IApplicationStatePublisher>();
        Exit += (sender, e) => appStatePublisher.Publish(ApplicationState.ShuttingDown);

        base.OnStartup(e);
    }

}