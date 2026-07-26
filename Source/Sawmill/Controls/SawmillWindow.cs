using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Sawmill.Controls;

public class SawmillWindow : Window
{
    private const int DwmWindowAttributeSystemBackdropType = 38;
    private const int DwmWindowAttributeUseImmersiveDarkMode = 20;
    private const int DwmSystemBackdropMainWindow = 2;

    private const int GWL_STYLE = -16;
    private const int WS_POPUP = unchecked((int)0x80000000);
    private const int WS_THICKFRAME = 0x00040000;
    private const int WS_CAPTION = 0x00C00000;
    private const int WS_MAXIMIZEBOX = 0x00010000;
    private const int WS_MINIMIZEBOX = 0x00020000;
    private const int WS_SYSMENU = 0x00080000;
    private const int WM_NCCALCSIZE = 0x0083;
    private const int WM_GETMINMAXINFO = 0x0024;
    private const int SM_CXSIZEFRAME = 32;
    private const int SM_CYSIZEFRAME = 33;
    private const int SM_CXPADDEDBORDER = 92;
    private const uint MONITOR_DEFAULTTONEAREST = 2;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_FRAMECHANGED = 0x0020;

    static SawmillWindow()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(SawmillWindow), new FrameworkPropertyMetadata(typeof(SawmillWindow)));
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        if (PresentationSource.FromVisual(this) is not HwndSource source)
            return;

        source.CompositionTarget.BackgroundColor = Colors.Transparent;

        var hwnd = source.Handle;
        source.AddHook(WndProc);

        // Replace WS_POPUP with standard overlapped styles so DWM provides
        // native minimize / maximize / close animations. WindowChrome still
        // handles hit-testing for the custom caption area.
        var style = GetWindowLong(hwnd, GWL_STYLE);
        style = (style & ~(WS_POPUP | WS_SYSMENU | WS_MAXIMIZEBOX | WS_MINIMIZEBOX)) | WS_THICKFRAME | WS_CAPTION;
        SetWindowLong(hwnd, GWL_STYLE, style);
        RefreshWindowFrame(hwnd);

        var backdrop = DwmSystemBackdropMainWindow;
        _ = DwmSetWindowAttribute(hwnd, DwmWindowAttributeSystemBackdropType, ref backdrop, Marshal.SizeOf<int>());

        var darkMode = Application.Current.TryFindResource("IsDarkTheme") is true ? 1 : 0;
        _ = DwmSetWindowAttribute(hwnd, DwmWindowAttributeUseImmersiveDarkMode, ref darkMode, Marshal.SizeOf<int>());

        Dispatcher.BeginInvoke(() => RefreshWindowFrame(hwnd), DispatcherPriority.Loaded);
    }

    private static void RefreshWindowFrame(IntPtr hwnd)
    {
        SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        switch (msg)
        {
            case WM_NCCALCSIZE:
                // Tell Windows the entire window is the client area, hiding
                // the default titlebar / border that WS_CAPTION would draw.
                handled = true;
                return IntPtr.Zero;

            case WM_GETMINMAXINFO:
                // Constrain the maximized rect to the monitor's working area
                // so the window doesn't extend behind the taskbar.
                WmGetMinMaxInfo(hwnd, lParam);
                handled = true;
                break;
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

    /// <summary>
    /// Toggles DWM immersive dark mode for the window's title bar and frame.
    /// </summary>
    public static void SetDwmDarkMode(SawmillWindow window, bool isDark)
    {
        if (PresentationSource.FromVisual(window) is not HwndSource source) return;
        var darkMode = isDark ? 1 : 0;
        _ = DwmSetWindowAttribute(source.Handle, DwmWindowAttributeUseImmersiveDarkMode, ref darkMode, Marshal.SizeOf<int>());
    }

    public static readonly DependencyProperty LeftHeaderContentProperty = DependencyProperty.Register(
        "LeftHeaderContent", typeof(object), typeof(SawmillWindow), new PropertyMetadata(default(object)));

    public object LeftHeaderContent
    {
        get => GetValue(LeftHeaderContentProperty);
        set => SetValue(LeftHeaderContentProperty, value);
    }

    public static void SetLeftHeaderContent(DependencyObject element, object value)
    {
        element.SetValue(LeftHeaderContentProperty, value);
    }

    public static object GetLeftHeaderContent(DependencyObject element)
    {
        return (object)element.GetValue(LeftHeaderContentProperty);
    }

    public static readonly DependencyProperty RightHeaderContentProperty = DependencyProperty.Register(
        "RightHeaderContent", typeof(object), typeof(SawmillWindow), new PropertyMetadata(default(object)));

    public object RightHeaderContent
    {
        get => GetValue(RightHeaderContentProperty);
        set => SetValue(RightHeaderContentProperty, value);
    }

    public static void SetRightHeaderContent(DependencyObject element, object value)
    {
        element.SetValue(RightHeaderContentProperty, value);
    }

    public static object GetRightHeaderContent(DependencyObject element)
    {
        return (object)element.GetValue(RightHeaderContentProperty);
    }

    #region Win32 interop

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

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hwnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hwnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int cx, int cy, uint flags);

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [DllImport("user32.dll")]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);

    #endregion
}
