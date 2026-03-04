using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace TailBlazer.Controls;

/// <summary>
/// A WPF port of the Windows Community Toolkit SettingsExpander control.
/// A collapsible control to host multiple SettingsCards.
/// </summary>
public class SettingsExpander : Control
{
    static SettingsExpander()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(SettingsExpander),
            new FrameworkPropertyMetadata(typeof(SettingsExpander)));
    }

    public SettingsExpander()
    {
        Items = new ObservableCollection<object>();
    }

    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header), typeof(object), typeof(SettingsExpander),
        new PropertyMetadata(null));

    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        nameof(Description), typeof(object), typeof(SettingsExpander),
        new PropertyMetadata(null));

    public static readonly DependencyProperty HeaderIconProperty = DependencyProperty.Register(
        nameof(HeaderIcon), typeof(object), typeof(SettingsExpander),
        new PropertyMetadata(null));

    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
        nameof(Content), typeof(object), typeof(SettingsExpander),
        new PropertyMetadata(null));

    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
        nameof(IsExpanded), typeof(bool), typeof(SettingsExpander),
        new PropertyMetadata(false));

    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
        nameof(Items), typeof(IList), typeof(SettingsExpander),
        new PropertyMetadata(null));

    public static readonly DependencyProperty ItemsFooterProperty = DependencyProperty.Register(
        nameof(ItemsFooter), typeof(object), typeof(SettingsExpander),
        new PropertyMetadata(null));

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

    public object Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public IList Items
    {
        get => (IList)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public object ItemsFooter
    {
        get => GetValue(ItemsFooterProperty);
        set => SetValue(ItemsFooterProperty, value);
    }
}
