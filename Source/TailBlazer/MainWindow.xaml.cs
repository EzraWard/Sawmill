using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
