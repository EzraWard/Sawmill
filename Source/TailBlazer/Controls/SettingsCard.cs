using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TailBlazer.Controls;

/// <summary>
/// A WPF port of the Windows Community Toolkit SettingsCard control.
/// Creates consistent settings experiences inline with the Windows 11 design language.
/// </summary>
public class SettingsCard : ContentControl
{
    static SettingsCard()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(SettingsCard),
            new FrameworkPropertyMetadata(typeof(SettingsCard)));
    }

    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header), typeof(object), typeof(SettingsCard),
        new PropertyMetadata(null));

    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        nameof(Description), typeof(object), typeof(SettingsCard),
        new PropertyMetadata(null));

    public static readonly DependencyProperty HeaderIconProperty = DependencyProperty.Register(
        nameof(HeaderIcon), typeof(object), typeof(SettingsCard),
        new PropertyMetadata(null));

    public static readonly DependencyProperty IsClickEnabledProperty = DependencyProperty.Register(
        nameof(IsClickEnabled), typeof(bool), typeof(SettingsCard),
        new PropertyMetadata(false));

    public static readonly DependencyProperty ActionIconProperty = DependencyProperty.Register(
        nameof(ActionIcon), typeof(object), typeof(SettingsCard),
        new PropertyMetadata("\uE974"));

    public static readonly DependencyProperty ActionIconToolTipProperty = DependencyProperty.Register(
        nameof(ActionIconToolTip), typeof(string), typeof(SettingsCard),
        new PropertyMetadata(null));

    public static readonly DependencyProperty IsActionIconVisibleProperty = DependencyProperty.Register(
        nameof(IsActionIconVisible), typeof(bool), typeof(SettingsCard),
        new PropertyMetadata(true));

    public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
        nameof(Click), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SettingsCard));

    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public object Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public object HeaderIcon
    {
        get => GetValue(HeaderIconProperty);
        set => SetValue(HeaderIconProperty, value);
    }

    public bool IsClickEnabled
    {
        get => (bool)GetValue(IsClickEnabledProperty);
        set => SetValue(IsClickEnabledProperty, value);
    }

    public object ActionIcon
    {
        get => GetValue(ActionIconProperty);
        set => SetValue(ActionIconProperty, value);
    }

    public string ActionIconToolTip
    {
        get => (string)GetValue(ActionIconToolTipProperty);
        set => SetValue(ActionIconToolTipProperty, value);
    }

    public bool IsActionIconVisible
    {
        get => (bool)GetValue(IsActionIconVisibleProperty);
        set => SetValue(IsActionIconVisibleProperty, value);
    }

    public event RoutedEventHandler Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);
        if (IsClickEnabled)
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent, this));
        }
    }
}
