using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
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
        var windowsModel = DataContext as WindowViewModel;
        windowsModel?.OnWindowClosing();
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
            var mousePos = PointToScreen(e.GetPosition(this));
            var restoreWidth = RestoreBounds.Width;
            var proportionalX = e.GetPosition(this).X / ActualWidth;

            WindowState = WindowState.Normal;

            Left = mousePos.X - (restoreWidth * proportionalX);
            Top = mousePos.Y - 10;

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
