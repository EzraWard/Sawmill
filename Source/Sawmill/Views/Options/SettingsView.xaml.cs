using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace Sawmill.Views.Options;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();

        // Load header icon at runtime to avoid XAML parse-time failure if resource is missing
        try
        {
            var imgUri = new Uri("pack://application:,,,/Sawmill;component/Images/sawmill.png", UriKind.Absolute);
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = imgUri;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.EndInit();
            SettingsSawmillImage.Source = bi;
        }
        catch
        {
            // ignore
        }
    }

    private void SettingsScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (sender is not ScrollViewer scrollViewer)
            return;

        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - (e.Delta * 0.45));
        e.Handled = true;
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        e.Handled = true;
    }
}
