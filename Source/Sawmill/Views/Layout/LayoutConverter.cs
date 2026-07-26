using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using DynamicData.Kernel;
using Sawmill.Domain.Annotations;
using Sawmill.Domain.Infrastructure;
using Sawmill.Domain.Settings;
using Sawmill.Infrastructure;
using Sawmill.Views.WindowManagement;
using Sawmill.Views.Options;

namespace Sawmill.Views.Layout;
//Store:

//1. -Root = string only
//2.  --Shell [size etc]
//3.  --Branch [proportion within tab page]
//4.  --View details [view state is passed ]
public class LayoutConverter : ILayoutConverter
{
    private readonly IWindowFactory _windowFactory;
    private readonly IViewFactoryProvider _viewFactoryProvider;
    private readonly ISchedulerProvider _schedulerProvider;
    private readonly GeneralOptionsViewModel _generalOptionsViewModel;
    private readonly ILogger _logger;

    private static class XmlStructure
    {
        public const string Root = "LayoutRoot";

        public static class ShellNode
        {
            public const string Shells = "Shells";
            public const string Shell = "Shell";
            public const string WindowsState = "WindowsState";
            public const string Top = "Top";
            public const string Left = "Left";
            public const string Width = "Width";
            public const string Height = "Height";
        }

        public static class BranchNode
        {
            public const string Branches = "Branches";
            public const string Branch = "Branch";
            public const string Orientation = "Orientation";
            public const string Proportion = "Proportion";
        }

        public static class ViewNode
        {
            public const string Children = "Children";
            public const string ViewState = "ViewState";
            public const string Version = "Version";
            public const string Key = "Key";
        }
    }

    public LayoutConverter([NotNull] IWindowFactory windowFactory,
        [NotNull] IViewFactoryProvider viewFactoryProvider,
        [NotNull] ISchedulerProvider schedulerProvider,
        [NotNull] GeneralOptionsViewModel generalOptionsViewModel,
        [NotNull] ILogger logger)
    {
        _windowFactory = windowFactory ?? throw new ArgumentNullException(nameof(windowFactory));
        _viewFactoryProvider = viewFactoryProvider ?? throw new ArgumentNullException(nameof(viewFactoryProvider));
        _schedulerProvider = schedulerProvider ?? throw new ArgumentNullException(nameof(schedulerProvider));
        _generalOptionsViewModel = generalOptionsViewModel ?? throw new ArgumentNullException(nameof(generalOptionsViewModel));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Capture state

    public XElement CaptureState()
    {
        var root = new XElement(XmlStructure.Root);
        var shells = new XElement(XmlStructure.ShellNode.Shells);

        foreach (var window in Application.Current.Windows.OfType<MainWindow>())
        {
            var bounds = window.RestoreBounds;
            var shellNode = new XElement(new XElement(XmlStructure.ShellNode.Shell));
            shellNode.Add(new XElement(XmlStructure.ShellNode.WindowsState, window.WindowState));
            shellNode.Add(new XElement(XmlStructure.ShellNode.Top, bounds.Top));
            shellNode.Add(new XElement(XmlStructure.ShellNode.Left, bounds.Left));
            shellNode.Add(new XElement(XmlStructure.ShellNode.Width, bounds.Right - bounds.Left));
            shellNode.Add(new XElement(XmlStructure.ShellNode.Height, bounds.Bottom - bounds.Top));

            shells.Add(shellNode);

            if (window.DataContext is WindowViewModel windowViewModel)
            {
                AddChildren(shellNode, windowViewModel.Views);
            }
        }

        root.Add(shells);
        return root;
    }

    private static void AddChildren(XElement stateNode, IEnumerable<HeaderedView> views)
    {
        var tabStates = views
            .Select(item => item.Content).OfType<IPersistentView>()
            .Select(provider => provider.CaptureState())
            .Select(state =>
            {
                var viewState = new XElement(XmlStructure.ViewNode.ViewState, new XAttribute(XmlStructure.ViewNode.Key, state.Key));
                viewState.SetAttributeValue(XmlStructure.ViewNode.Version, state.State.Version);
                viewState.Add(state.State.Value);
                return viewState;
            }).ToArray();

        var elements = new XElement(XmlStructure.ViewNode.Children, tabStates);
        stateNode.Add(elements);
    }

    #endregion

    #region Restore state

    public void Restore([NotNull] XElement element)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));

        element.Elements(XmlStructure.ShellNode.Shells)
            .SelectMany(shells => shells.Elements(XmlStructure.ShellNode.Shell))
            .Select((shellState, index) =>
            {
                var winState = shellState.ElementOrThrow(XmlStructure.ShellNode.WindowsState).ParseEnum<WindowState>().Value;
                var top = shellState.ElementOrThrow(XmlStructure.ShellNode.Top).ParseDouble().Value;
                var left = shellState.ElementOrThrow(XmlStructure.ShellNode.Left).ParseDouble().Value;
                var width = shellState.ElementOrThrow(XmlStructure.ShellNode.Width).ParseDouble().Value;
                var height = shellState.ElementOrThrow(XmlStructure.ShellNode.Height).ParseDouble().Value;

                var main = Application.Current.Windows.OfType<MainWindow>().First();
                var window = index == 0 ? main : _windowFactory.Create();

                // Validate restored bounds and ensure window is visible on at least one monitor.
                var virtualLeft = SystemParameters.VirtualScreenLeft;
                var virtualTop = SystemParameters.VirtualScreenTop;
                var virtualRight = virtualLeft + SystemParameters.VirtualScreenWidth;
                var virtualBottom = virtualTop + SystemParameters.VirtualScreenHeight;

                double intersectWidth = Math.Min(left + width, virtualRight) - Math.Max(left, virtualLeft);
                double intersectHeight = Math.Min(top + height, virtualBottom) - Math.Max(top, virtualTop);

                var reasonableSize = width >= 100 && height >= 100;
                var onScreen = intersectWidth > 100 && intersectHeight > 100;

                if (reasonableSize && onScreen)
                {
                    window.WindowStartupLocation = WindowStartupLocation.Manual;
                    if (winState == WindowState.Minimized)
                    {
                        _logger.Info($"Restored window {index} had Minimized state; forcing Normal to ensure visibility.");
                        window.WindowState = WindowState.Normal;
                    }
                    else
                    {
                        window.WindowState = winState;
                    }

                    window.Left = left;
                    window.Top = top;
                    window.Width = Math.Max(width, 100);
                    window.Height = Math.Max(height, 100);
                }
                else
                {
                    _logger.Warn($"Saved window bounds for shell {index} appear off-screen or invalid (left={left},top={top},w={width},h={height},state={winState}); falling back to centered Normal window.");
                    // Fallback to centered, normal window if the saved state looks invalid or off-screen.
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    window.WindowState = WindowState.Normal;
                }

                window.Show();
                return new { window, shellState };
            })
            .ForEach(x =>
            {
                if (_generalOptionsViewModel.OpenRecentOnStartup)
                    RestoreChildren(x.window, x.shellState);
            });
    }
        
    private void RestoreChildren(MainWindow window, XElement element)
    {
        //NEED TO GET A BETTER HANDLE ON WINDOWS CONTROLLER - Currently done via WindowsViewModel
        var windowViewModel = (IViewOpener)window.DataContext;

        GetViews(element)
            .ForEach(view =>
            {
                windowViewModel.OpenView(view);
            });
    }

    private IEnumerable<HeaderedView> GetViews(XElement element)
    {
        return GetChildrenState(element)
            .AsParallel()
            .AsOrdered()
            .Select(state =>
            {
                var key = state.Key;
                var factory = _viewFactoryProvider.Lookup(key);
                return !factory.HasValue ? null : factory.Value.Create(state);

            }).Where(view => view != null)
            .ToArray();
    }

    private IEnumerable<ViewState> GetChildrenState(XElement element)
    {
        return element.Elements(XmlStructure.ViewNode.Children)
            .SelectMany(shells => shells.Elements(XmlStructure.ViewNode.ViewState))
            .Select(viewStateElement =>
            {
                var key = viewStateElement.AttributeOrThrow(XmlStructure.ViewNode.Key);
                var version = viewStateElement.AttributeOrThrow(XmlStructure.ViewNode.Version).ParseInt().Value;
                var state = viewStateElement.Value;
                var viewstate = new ViewState(key, new State(version, state));
                return viewstate;
            });
    }

    #endregion
}
