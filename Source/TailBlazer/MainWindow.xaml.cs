using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Dragablz;
using TailBlazer.Infrastructure;
using TailBlazer.Views.WindowManagement;

namespace TailBlazer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();

        // Safe runtime load for icon and brand image - prevents XAML parse-time failures if resources are missing
        try
        {
            var iconUri = new Uri("pack://application:,,,/TailBlazer;component/sawmill.ico", UriKind.Absolute);
            var iconStream = Application.GetResourceStream(iconUri)?.Stream;
            if (iconStream != null)
            {
                Icon = BitmapFrame.Create(iconStream);
            }
        }
        catch
        {
            // ignore - missing icon should not stop startup
        }

        try
        {
            var imgUri = new Uri("pack://application:,,,/TailBlazer;component/Images/sawmill.png", UriKind.Absolute);
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = imgUri;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.EndInit();
            BrandImage.Source = bi;
        }
        catch
        {
            // ignore - missing image should not stop startup
        }

        Closing += MainWindow_Closing;
        Loaded += MainWindow_Loaded;
        SourceInitialized += MainWindow_SourceInitialized;
    }

    private void MainWindow_SourceInitialized(object sender, EventArgs e)
    {
        var handle = new WindowInteropHelper(this).Handle;
        HwndSource.FromHwnd(handle)?.AddHook(WindowProc);
    }

    private const int WM_GETMINMAXINFO = 0x0024;

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT { public int x, y; }

    [StructLayout(LayoutKind.Sequential)]
    private struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [DllImport("user32.dll")]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT { public int Left, Top, Right, Bottom; }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }

    private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_GETMINMAXINFO)
        {
            var mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);

            var monitor = MonitorFromWindow(hwnd, 0x00000002); // MONITOR_DEFAULTTONEAREST
            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO { cbSize = Marshal.SizeOf<MONITORINFO>() };
                GetMonitorInfo(monitor, ref monitorInfo);

                var work = monitorInfo.rcWork;
                var mon = monitorInfo.rcMonitor;

                mmi.ptMaxPosition = new POINT { x = work.Left - mon.Left, y = work.Top - mon.Top };
                mmi.ptMaxSize = new POINT { x = work.Right - work.Left, y = work.Bottom - work.Top };
            }

            Marshal.StructureToPtr(mmi, lParam, true);
            handled = true;
        }
        return IntPtr.Zero;
    }

    private void MainWindow_Closing(object sender, CancelEventArgs e)
    {
        // Let Dragablz handle window closure during tab drag operations.
        if (TabablzControl.GetIsClosingAsPartOfDragOperation(this))
            return;

        var windowsModel = DataContext as WindowViewModel;

        // Publish ShuttingDown BEFORE disposing views so LayoutConverter.CaptureState()
        // can still walk Application.Current.Windows and read live tab state.
        // DistinctUntilChanged in ApplicationStateBroker suppresses any duplicate
        // publishes that fire when subsequent windows close during shutdown.
        windowsModel?.WindowExiting?.Invoke();

        windowsModel?.OnWindowClosing();

        // Explicit shutdown ensures the process always exits when any window is closed,
        // including scenarios where multiple windows exist (layout restore, tab tear-out).
        Application.Current.Shutdown();
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        TitleBarTabs.SelectionChanged += (_, _) => UpdateSelectedTabConnectionGap();
        TitleBarTabs.SizeChanged += (_, _) => UpdateSelectedTabConnectionGap();
        SizeChanged += (_, _) => UpdateSelectedTabConnectionGap();

        Dispatcher.BeginInvoke(UpdateSelectedTabConnectionGap, DispatcherPriority.Loaded);
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void TitleBar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (IsFromInteractiveControl(e.OriginalSource as DependencyObject))
            return;

        if (e.ClickCount == 2)
        {
            MaximizeRestoreButton_Click(sender, e);
            e.Handled = true;
            return;
        }

        if (WindowState == WindowState.Maximized)
        {
            // Get cursor position relative to window before restoring
            var cursorPos = e.GetPosition(this);
            var proportionalX = cursorPos.X / ActualWidth;

            WindowState = WindowState.Normal;

            // Position window so cursor stays at same proportional X, near top
            Left = cursorPos.X + Left - (ActualWidth * proportionalX);
            Top = 0;

            if (Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        else
        {
            DragMove();
        }
    }

    private static bool IsFromInteractiveControl(DependencyObject source)
    {
        while (source != null)
        {
            if (source is Button || source is TabItem || source is ScrollBar)
                return true;

            source = VisualTreeHelper.GetParent(source);
        }

        return false;
    }

    private void TitleBarTabItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Middle)
            return;

        if (sender is not TabItem tabItem || tabItem.DataContext is not HeaderedView headeredView)
            return;

        if (DataContext is not WindowViewModel vm || !vm.CloseViewCommand.CanExecute(headeredView))
            return;

        vm.CloseViewCommand.Execute(headeredView);
        e.Handled = true;
    }

    private void TabScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (sender is ScrollViewer sv)
        {
            sv.ScrollToHorizontalOffset(sv.HorizontalOffset - e.Delta);
            e.Handled = true;
        }
    }

    private void UpdateSelectedTabConnectionGap()
    {
        if (TitleBarTabs.SelectedItem == null)
        {
            SelectedTabTopGap.Visibility = Visibility.Collapsed;
            return;
        }

        if (TitleBarTabs.ItemContainerGenerator.ContainerFromItem(TitleBarTabs.SelectedItem) is not TabItem selectedTab)
        {
            SelectedTabTopGap.Visibility = Visibility.Collapsed;
            return;
        }

        if (!selectedTab.IsLoaded || !ContentFrameHost.IsLoaded)
        {
            SelectedTabTopGap.Visibility = Visibility.Collapsed;
            return;
        }

        var tabTopLeft = selectedTab.TransformToVisual(ContentFrameHost).Transform(new Point(0, 0));
        var gapWidth = Math.Max(0, selectedTab.ActualWidth - 2);

        if (gapWidth <= 0)
        {
            SelectedTabTopGap.Visibility = Visibility.Collapsed;
            return;
        }

        SelectedTabTopGap.Margin = new Thickness(Math.Max(0, tabTopLeft.X + 1), 0, 0, 0);
        SelectedTabTopGap.Width = gapWidth;
        SelectedTabTopGap.Visibility = Visibility.Visible;
    }
}
