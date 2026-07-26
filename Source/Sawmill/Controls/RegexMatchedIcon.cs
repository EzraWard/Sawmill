using System.Windows;
using System.Windows.Controls;

namespace Sawmill.Controls;

public class RegexMatchedIcon : Control
{
    static RegexMatchedIcon()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(RegexMatchedIcon), new FrameworkPropertyMetadata(typeof(RegexMatchedIcon)));
    }
}