using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace TailBlazer.Controls;

public class TailBlazerWindow : Window
{
    private const int DwmWindowAttributeSystemBackdropType = 38;
    private const int DwmWindowAttributeUseImmersiveDarkMode = 20;
    private const int DwmSystemBackdropMainWindow = 2;

    static TailBlazerWindow()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(TailBlazerWindow), new FrameworkPropertyMetadata(typeof(TailBlazerWindow)));
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        if (PresentationSource.FromVisual(this) is not HwndSource source)
        {
            return;
        }

        var backdrop = DwmSystemBackdropMainWindow;
        _ = DwmSetWindowAttribute(source.Handle, DwmWindowAttributeSystemBackdropType, ref backdrop, Marshal.SizeOf<int>());

        var darkMode = 1;
        _ = DwmSetWindowAttribute(source.Handle, DwmWindowAttributeUseImmersiveDarkMode, ref darkMode, Marshal.SizeOf<int>());
    }

    public static readonly DependencyProperty LeftHeaderContentProperty = DependencyProperty.Register(
        "LeftHeaderContent", typeof(object), typeof(TailBlazerWindow), new PropertyMetadata(default(object)));

    public static void SetLeftHeaderContent(DependencyObject element, object value)
    {
        element.SetValue(LeftHeaderContentProperty, value);
    }

    public static object GetLeftHeaderContent(DependencyObject element)
    {
        return (object)element.GetValue(LeftHeaderContentProperty);
    }

    public static readonly DependencyProperty RightHeaderContentProperty = DependencyProperty.Register(
        "RightHeaderContent", typeof(object), typeof(TailBlazerWindow), new PropertyMetadata(default(object)));

    public static void SetRightHeaderContent(DependencyObject element, object value)
    {
        element.SetValue(RightHeaderContentProperty, value);
    }

    public static object GetRightHeaderContent(DependencyObject element)
    {
        return (object)element.GetValue(RightHeaderContentProperty);
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);
}
