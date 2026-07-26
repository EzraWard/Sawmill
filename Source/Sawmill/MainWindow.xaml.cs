using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Sawmill.Infrastructure;
using Sawmill.Views.WindowManagement;

namespace Sawmill;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const int DwmWindowAttributeSystemBackdropType = 38;
    private const int DwmWindowAttributeUseImmersiveDarkMode = 20;
    private const int DwmWindowAttributeWindowCornerPreference = 33;
    private const int DwmSystemBackdropMainWindow = 2;
    private const int DwmWindowCornerPreferenceRound = 2;
    private const int GWL_STYLE = -16;
    private const int WS_POPUP = unchecked((int)0x80000000);
    private const int WS_THICKFRAME = 0x00040000;
    private const int WS_CAPTION = 0x00C00000;
    private const int WS_MAXIMIZEBOX = 0x00010000;
    private const int WS_MINIMIZEBOX = 0x00020000;
    private const int WS_SYSMENU = 0x00080000;
    private const int WM_NCCALCSIZE = 0x0083;
    private const int SM_CXSIZEFRAME = 32;
    private const int SM_CYSIZEFRAME = 33;
    private const int SM_CXPADDEDBORDER = 92;
    private const int SM_CXDOUBLECLK = 36;
    private const int SM_CYDOUBLECLK = 37;
    private Point? _tabDragStartPoint;
    private HeaderedView _tabDragSource;
    private WindowViewModel _currentWindowViewModel;
    private Point? _lastTitleBarTouchPoint;
    private int _lastTitleBarTouchTimestamp;

    public MainWindow()
    {
        InitializeComponent();

        // Safe runtime load for icon and brand image - prevents XAML parse-time failures if resources are missing
        try
        {
            var iconUri = new Uri("pack://application:,,,/Sawmill;component/sawmill.ico", UriKind.Absolute);
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
            var imgUri = new Uri("pack://application:,,,/Sawmill;component/Images/sawmill.png", UriKind.Absolute);
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
        DataContextChanged += MainWindow_DataContextChanged;
        StateChanged += (_, _) => UpdateMaximizedContentMargin();
    }

    private void MainWindow_SourceInitialized(object sender, EventArgs e)
    {
        if (PresentationSource.FromVisual(this) is not HwndSource source)
            return;

        source.CompositionTarget.BackgroundColor = Colors.Transparent;

        var hwnd = source.Handle;
        source.AddHook(WindowProc);

        var style = GetWindowLong(hwnd, GWL_STYLE);
        style = (style & ~(WS_POPUP | WS_SYSMENU)) | WS_THICKFRAME | WS_CAPTION | WS_MAXIMIZEBOX | WS_MINIMIZEBOX;
        SetWindowLong(hwnd, GWL_STYLE, style);
        RefreshWindowFrame(hwnd);

        var cornerPreference = DwmWindowCornerPreferenceRound;
        _ = DwmSetWindowAttribute(hwnd, DwmWindowAttributeWindowCornerPreference, ref cornerPreference, Marshal.SizeOf<int>());

        var backdrop = DwmSystemBackdropMainWindow;
        _ = DwmSetWindowAttribute(hwnd, DwmWindowAttributeSystemBackdropType, ref backdrop, Marshal.SizeOf<int>());

        var darkMode = Application.Current.TryFindResource("IsDarkTheme") is true ? 1 : 0;
        _ = DwmSetWindowAttribute(hwnd, DwmWindowAttributeUseImmersiveDarkMode, ref darkMode, Marshal.SizeOf<int>());

        Dispatcher.BeginInvoke(() =>
        {
            RefreshWindowFrame(hwnd);
            UpdateMaximizedContentMargin();
        }, DispatcherPriority.Loaded);
    }

    private static void RefreshWindowFrame(IntPtr hwnd)
    {
        SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
    }

    private void UpdateMaximizedContentMargin()
    {
        RootContent.Margin = WindowState == WindowState.Maximized
            ? GetResizeFrameThickness()
            : new Thickness(0);
    }

    private Thickness GetResizeFrameThickness()
    {
        var frameX = GetSystemMetrics(SM_CXSIZEFRAME) + GetSystemMetrics(SM_CXPADDEDBORDER);
        var frameY = GetSystemMetrics(SM_CYSIZEFRAME) + GetSystemMetrics(SM_CXPADDEDBORDER);

        if (PresentationSource.FromVisual(this) is HwndSource source)
        {
            var topLeft = source.CompositionTarget.TransformFromDevice.Transform(new Point(frameX, frameY));
            return new Thickness(topLeft.X, topLeft.Y, topLeft.X, topLeft.Y);
        }

        return new Thickness(frameX, frameY, frameX, frameY);
    }

    private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (_currentWindowViewModel != null)
        {
            _currentWindowViewModel.PropertyChanged -= WindowViewModel_PropertyChanged;
        }

        _currentWindowViewModel = e.NewValue as WindowViewModel;

        if (_currentWindowViewModel != null)
        {
            _currentWindowViewModel.PropertyChanged += WindowViewModel_PropertyChanged;
            SetSettingsOverlayState(_currentWindowViewModel.IsShowingSettings, false);
        }
    }

    private void WindowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(WindowViewModel.IsShowingSettings) && sender is WindowViewModel viewModel)
        {
            SetSettingsOverlayState(viewModel.IsShowingSettings, true);
        }
    }

    private void SetSettingsOverlayState(bool isShowing, bool animate)
    {
        if (isShowing)
        {
            SettingsOverlay.Visibility = Visibility.Visible;
            AnimateSettingsOverlay(1, 0, animate ? 250 : 0, EasingMode.EaseOut);
        }
        else if (animate)
        {
            var opacityAnimation = CreateSettingsAnimation(0, 167, EasingMode.EaseIn);
            opacityAnimation.Completed += (_, _) => SettingsOverlay.Visibility = Visibility.Collapsed;
            SettingsOverlay.BeginAnimation(OpacityProperty, opacityAnimation);
            SettingsOverlayTranslate.BeginAnimation(TranslateTransform.YProperty, CreateSettingsAnimation(16, 167, EasingMode.EaseIn));
        }
        else
        {
            SettingsOverlay.BeginAnimation(OpacityProperty, null);
            SettingsOverlayTranslate.BeginAnimation(TranslateTransform.YProperty, null);
            SettingsOverlay.Opacity = 0;
            SettingsOverlayTranslate.Y = 24;
            SettingsOverlay.Visibility = Visibility.Collapsed;
        }
    }

    private void AnimateSettingsOverlay(double opacity, double y, int milliseconds, EasingMode easingMode)
    {
        SettingsOverlay.BeginAnimation(OpacityProperty, CreateSettingsAnimation(opacity, milliseconds, easingMode));
        SettingsOverlayTranslate.BeginAnimation(TranslateTransform.YProperty, CreateSettingsAnimation(y, milliseconds, easingMode));
    }

    private static DoubleAnimation CreateSettingsAnimation(double to, int milliseconds, EasingMode easingMode)
    {
        return new DoubleAnimation
        {
            To = to,
            Duration = TimeSpan.FromMilliseconds(milliseconds),
            EasingFunction = new CubicEase { EasingMode = easingMode },
            FillBehavior = FillBehavior.HoldEnd
        };
    }

    public static void SetDwmDarkMode(MainWindow window, bool isDark)
    {
        if (PresentationSource.FromVisual(window) is not HwndSource source)
            return;

        var darkMode = isDark ? 1 : 0;
        _ = DwmSetWindowAttribute(source.Handle, DwmWindowAttributeUseImmersiveDarkMode, ref darkMode, Marshal.SizeOf<int>());
    }

    private const int WM_NCLBUTTONDOWN = 0x00A1;
    private const int HTCAPTION = 0x0002;
    private const int WM_GETMINMAXINFO = 0x0024;
    private const uint MONITOR_DEFAULTTONEAREST = 2;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_FRAMECHANGED = 0x0020;

    [DllImport("user32.dll")]
    private static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hwnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hwnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int cx, int cy, uint flags);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public int dwFlags;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [DllImport("user32.dll")]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);

    [DllImport("user32.dll")]
    private static extern uint GetDoubleClickTime();

    private void MainWindow_Closing(object sender, CancelEventArgs e)
    {
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

    private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        switch (msg)
        {
            case WM_NCCALCSIZE:
                handled = true;
                return IntPtr.Zero;

            case WM_GETMINMAXINFO:
                WmGetMinMaxInfo(hwnd, lParam);
                handled = true;
                return IntPtr.Zero;
        }

        return IntPtr.Zero;
    }

    private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
    {
        var mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);
        var monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
        if (monitor != IntPtr.Zero)
        {
            var mi = new MONITORINFO { cbSize = Marshal.SizeOf<MONITORINFO>() };
            if (GetMonitorInfo(monitor, ref mi))
            {
                var frameX = GetSystemMetrics(SM_CXSIZEFRAME) + GetSystemMetrics(SM_CXPADDEDBORDER);
                var frameY = GetSystemMetrics(SM_CYSIZEFRAME) + GetSystemMetrics(SM_CXPADDEDBORDER);

                mmi.ptMaxPosition.X = mi.rcWork.Left - mi.rcMonitor.Left - frameX;
                mmi.ptMaxPosition.Y = mi.rcWork.Top - mi.rcMonitor.Top - frameY;
                mmi.ptMaxSize.X = mi.rcWork.Right - mi.rcWork.Left + (frameX * 2);
                mmi.ptMaxSize.Y = mi.rcWork.Bottom - mi.rcWork.Top + (frameY * 2);
            }
        }

        Marshal.StructureToPtr(mmi, lParam, false);
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

        // Send WM_NCLBUTTONDOWN with HTCAPTION to start a system-managed caption drag.
        // This properly handles restore-from-maximized when dragging, unlike DragMove()
        // which can fail with custom WS_OVERLAPPEDWINDOW + WM_NCCALCSIZE interop.
        var hwnd = new WindowInteropHelper(this).Handle;
        ReleaseCapture();
        SendMessage(hwnd, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
        e.Handled = true;
    }

    private void TitleBar_PreviewTouchDown(object sender, TouchEventArgs e)
    {
        if (IsFromInteractiveControl(e.OriginalSource as DependencyObject))
            return;

        var touchPoint = e.GetTouchPoint(TitleBarGrid).Position;
        if (IsTitleBarDoubleTap(touchPoint, e.Timestamp))
        {
            _lastTitleBarTouchPoint = null;
            MaximizeRestoreButton_Click(sender, e);
            e.Handled = true;
            return;
        }

        _lastTitleBarTouchPoint = touchPoint;
        _lastTitleBarTouchTimestamp = e.Timestamp;

        var hwnd = new WindowInteropHelper(this).Handle;
        ReleaseCapture();
        SendMessage(hwnd, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
        e.Handled = true;
    }

    private bool IsTitleBarDoubleTap(Point touchPoint, int timestamp)
    {
        if (_lastTitleBarTouchPoint is not { } previousPoint)
            return false;

        var elapsed = timestamp - _lastTitleBarTouchTimestamp;
        if (elapsed < 0 || elapsed > GetDoubleClickTime())
            return false;

        var doubleTapBounds = GetDoubleTapBounds();
        return Math.Abs(touchPoint.X - previousPoint.X) <= doubleTapBounds.Width &&
               Math.Abs(touchPoint.Y - previousPoint.Y) <= doubleTapBounds.Height;
    }

    private Size GetDoubleTapBounds()
    {
        var bounds = new Vector(GetSystemMetrics(SM_CXDOUBLECLK), GetSystemMetrics(SM_CYDOUBLECLK));
        var transformedBounds = PresentationSource.FromVisual(this)?.CompositionTarget?.TransformFromDevice.Transform(bounds) ?? bounds;
        return new Size(transformedBounds.X, transformedBounds.Y);
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

    private void TitleBarTabs_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        ClearPendingTabDrag();

        if (e.OriginalSource is DependencyObject source && FindAncestor<Button>(source) != null)
            return;

        if (e.OriginalSource is not DependencyObject original)
            return;

        var tabItem = FindAncestor<TabItem>(original);
        if (tabItem?.DataContext is not HeaderedView headeredView)
            return;

        _tabDragStartPoint = e.GetPosition(TitleBarTabs);
        _tabDragSource = headeredView;
    }

    private void TitleBarTabs_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        ClearPendingTabDrag();
    }

    private void TitleBarTabs_LostMouseCapture(object sender, MouseEventArgs e)
    {
        ClearPendingTabDrag();
    }

    private void ContentArea_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        ClearPendingTabDrag();
    }

    private void CloseTabButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not Button { Command: { } command } button)
            return;

        var parameter = button.CommandParameter;
        if (!command.CanExecute(parameter))
            return;

        command.Execute(parameter);
        e.Handled = true;
    }

    private void TitleBarTabs_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed || _tabDragStartPoint == null || _tabDragSource == null)
            return;

        var current = e.GetPosition(TitleBarTabs);
        var delta = current - _tabDragStartPoint.Value;
        if (Math.Abs(delta.X) < SystemParameters.MinimumHorizontalDragDistance &&
            Math.Abs(delta.Y) < SystemParameters.MinimumVerticalDragDistance)
            return;

        try
        {
            DragDrop.DoDragDrop(TitleBarTabs, _tabDragSource, DragDropEffects.Move);
        }
        finally
        {
            ClearPendingTabDrag();
        }
    }

    private void TitleBarTabs_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetData(typeof(HeaderedView)) is HeaderedView ? DragDropEffects.Move : DragDropEffects.None;
        e.Handled = true;
    }

    private void TitleBarTabs_Drop(object sender, DragEventArgs e)
    {
        ClearPendingTabDrag();

        if (DataContext is not WindowViewModel vm)
            return;

        if (e.Data.GetData(typeof(HeaderedView)) is not HeaderedView source)
            return;

        var target = GetTabHeaderedViewFromPoint(e.GetPosition(TitleBarTabs));
        if (target == null || source.Equals(target))
            return;

        var oldIndex = vm.Views.IndexOf(source);
        var newIndex = vm.Views.IndexOf(target);
        if (oldIndex < 0 || newIndex < 0 || oldIndex == newIndex)
            return;

        vm.Views.Move(oldIndex, newIndex);
        vm.Selected = source;
    }

    private void ClearPendingTabDrag()
    {
        _tabDragStartPoint = null;
        _tabDragSource = null;
    }

    private HeaderedView GetTabHeaderedViewFromPoint(Point point)
    {
        var element = TitleBarTabs.InputHitTest(point) as DependencyObject;
        var tabItem = FindAncestor<TabItem>(element);
        return tabItem?.DataContext as HeaderedView;
    }

    private static T FindAncestor<T>(DependencyObject source) where T : DependencyObject
    {
        while (source != null)
        {
            if (source is T match)
                return match;
            source = VisualTreeHelper.GetParent(source);
        }

        return null;
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
