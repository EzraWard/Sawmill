using System.Windows;
using System.Windows.Controls;

namespace TailBlazer.Views.WindowManagement;

public partial class StartTabView : UserControl
{
    public StartTabView()
    {
        InitializeComponent();
    }

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}
