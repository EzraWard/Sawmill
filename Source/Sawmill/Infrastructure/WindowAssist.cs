using System.Windows;

namespace Sawmill.Infrastructure;

public delegate void ApplicationExitingDelegate();

public static class WindowAssist
{
    public static readonly DependencyProperty ApplicationClosingProperty = DependencyProperty.RegisterAttached("ApplicationClosing", typeof (ApplicationExitingDelegate), typeof (WindowAssist), 
        new PropertyMetadata(default(ApplicationExitingDelegate),OnClosingDelegateSet));

    public static void SetApplicationClosing(Window element, ApplicationExitingDelegate value)
    {
        element.SetValue(ApplicationClosingProperty, value);
    }

    public static ApplicationExitingDelegate GetApplicationClosing(Window element)
    {
        return (ApplicationExitingDelegate)element.GetValue(ApplicationClosingProperty);
    }

    public static void OnClosingDelegateSet(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        // Intentionally empty: MainWindow_Closing invokes the delegate directly,
        // in the correct order relative to OnWindowClosing() and Shutdown().
    }
}