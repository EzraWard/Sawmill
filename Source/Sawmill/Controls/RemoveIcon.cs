using System.Windows;
using System.Windows.Controls;

namespace Sawmill.Controls;

public class RemoveIcon : Control
{
    static RemoveIcon()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(RemoveIcon), new FrameworkPropertyMetadata(typeof(RemoveIcon)));
    }
}