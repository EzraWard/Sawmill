using System.Windows;
using System.Windows.Controls;

namespace Sawmill.Controls;

public class ExitIcon : Control
{
    static ExitIcon()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ExitIcon), new FrameworkPropertyMetadata(typeof(ExitIcon)));
    }
}