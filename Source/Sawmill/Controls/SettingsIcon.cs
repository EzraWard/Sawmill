using System.Windows;
using System.Windows.Controls;

namespace Sawmill.Controls;

public class SettingsIcon : Control
{
    static SettingsIcon()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(SettingsIcon), new FrameworkPropertyMetadata(typeof(SettingsIcon)));
    }
}