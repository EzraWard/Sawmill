using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Sawmill.Views.Tail;

/// <summary>
/// Interaction logic for TailView.xaml
/// </summary>
public partial class TailView : UserControl
{
    public TailView()
    {
        InitializeComponent();
        IsVisibleChanged += (sender, e) =>
        {
            FocusSearchTextBox();
        };

        // Load inline icon image at runtime to avoid XAML parse errors if resource missing
        try
        {
            var imgUri = new Uri("pack://application:,,,/Sawmill;component/Images/sawmill.png", UriKind.Absolute);
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = imgUri;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.EndInit();
            TailSawmillImage.Source = bi;
        }
        catch
        {
            // ignore
        }
    }

    private void FocusSearchTextBox()
    {
        Dispatcher.BeginInvoke(new Action(() =>
        {
            SearchTextBox.Focus();
            MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }));
    }

    private void ApplicationCommandFind_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        FocusSearchTextBox();
    }

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}