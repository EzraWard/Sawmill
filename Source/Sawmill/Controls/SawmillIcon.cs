using System.Windows;
using System.Windows.Controls;

namespace Sawmill.Controls;

public class SawmillIcon : Control
{
    static SawmillIcon()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(SawmillIcon), new FrameworkPropertyMetadata(typeof(SawmillIcon)));
    }
}