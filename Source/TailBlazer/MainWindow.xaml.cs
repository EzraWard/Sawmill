using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
    }

    private void MainWindow_Closing(object sender, CancelEventArgs e)
    {

        var windowsModel = DataContext as WindowViewModel;
        windowsModel?.OnWindowClosing();
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

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (IsFromInteractiveControl(e.OriginalSource as DependencyObject))
            return;

        if (e.ClickCount == 2)
        {
            MaximizeRestoreButton_Click(sender, e);
            return;
        }

        DragMove();
    }

    private static bool IsFromInteractiveControl(DependencyObject source)
    {
        while (source != null)
        {
            if (source is Button || source is TabItem)
                return true;

            source = VisualTreeHelper.GetParent(source);
        }

        return false;
    }
}
