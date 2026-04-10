using System.Windows;
using System.Windows.Controls;

namespace Sawmill.Controls;

public class RegexIcon : Control
{
    static RegexIcon()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(RegexIcon), new FrameworkPropertyMetadata(typeof(RegexIcon)));
    }
}