using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Automation;

namespace Sawmill.Controls;

/// <summary>
/// A WPF port of the Windows Community Toolkit SettingsExpander control.
/// A collapsible control to host multiple SettingsCards.
/// </summary>
[TemplatePart(Name = ItemsControlTemplatePart, Type = typeof(ItemsControl))]
public class SettingsExpander : Control
{
    private const string ItemsControlTemplatePart = "PART_ItemsRepeater";
    private ItemsControl _itemsControl;

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

    public static readonly DependencyProperty ItemsHeaderProperty = DependencyProperty.Register(
        nameof(ItemsHeader), typeof(UIElement), typeof(SettingsExpander),
        new PropertyMetadata(null));

    public static readonly DependencyProperty ItemsFooterProperty = DependencyProperty.Register(
        nameof(ItemsFooter), typeof(UIElement), typeof(SettingsExpander),
        new PropertyMetadata(null));

    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
        nameof(IsExpanded), typeof(bool), typeof(SettingsExpander),
        new PropertyMetadata(false, (d, e) => ((SettingsExpander)d).OnIsExpandedPropertyChanged((bool)e.OldValue, (bool)e.NewValue)));

    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
        nameof(Items), typeof(IList), typeof(SettingsExpander),
        new PropertyMetadata(null, OnItemsConnectedPropertyChanged));

    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource), typeof(IEnumerable), typeof(SettingsExpander),
        new PropertyMetadata(null, OnItemsConnectedPropertyChanged));

    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
        nameof(ItemTemplate), typeof(DataTemplate), typeof(SettingsExpander),
        new PropertyMetadata(null));

    public static readonly DependencyProperty ItemContainerStyleSelectorProperty = DependencyProperty.Register(
        nameof(ItemContainerStyleSelector), typeof(StyleSelector), typeof(SettingsExpander),
        new PropertyMetadata(null));

    public event EventHandler Expanded;
    public event EventHandler Collapsed;

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

    public UIElement ItemsHeader
    {
        get => (UIElement)GetValue(ItemsHeaderProperty);
        set => SetValue(ItemsHeaderProperty, value);
    }

    public UIElement ItemsFooter
    {
        get => (UIElement)GetValue(ItemsFooterProperty);
        set => SetValue(ItemsFooterProperty, value);
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

    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public StyleSelector ItemContainerStyleSelector
    {
        get => (StyleSelector)GetValue(ItemContainerStyleSelectorProperty);
        set => SetValue(ItemContainerStyleSelectorProperty, value);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        SetAccessibleName();

        if (_itemsControl != null)
        {
            _itemsControl.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
        }

        _itemsControl = GetTemplateChild(ItemsControlTemplatePart) as ItemsControl;

        if (_itemsControl != null)
        {
            _itemsControl.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
            OnItemsConnectedPropertyChanged(this, default);
        }
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new SettingsExpanderAutomationPeer(this);
    }

    protected virtual void OnIsExpandedPropertyChanged(bool oldValue, bool newValue)
    {
        OnIsExpandedChanged(oldValue, newValue);

        if (newValue)
        {
            Expanded?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Collapsed?.Invoke(this, EventArgs.Empty);
        }
    }

    private static void OnItemsConnectedPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not SettingsExpander expander || expander._itemsControl is null)
            return;

        expander._itemsControl.ItemsSource = expander.ItemsSource ?? expander.Items;
    }

    private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
    {
        if (ItemContainerStyleSelector == null || _itemsControl?.ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            return;

        foreach (var item in _itemsControl.Items)
        {
            if (item is not SettingsCard card || card.ReadLocalValue(StyleProperty) != DependencyProperty.UnsetValue)
                continue;

            var style = ItemContainerStyleSelector.SelectStyle(card, card);
            if (style != null)
            {
                card.Style = style;
            }
        }
    }

    private void SetAccessibleName()
    {
        if (!string.IsNullOrEmpty(AutomationProperties.GetName(this)))
            return;

        if (Header is string headerString && !string.IsNullOrEmpty(headerString))
        {
            AutomationProperties.SetName(this, headerString);
        }
    }

    private void OnIsExpandedChanged(bool oldValue, bool newValue)
    {
        if (UIElementAutomationPeer.FromElement(this) is SettingsExpanderAutomationPeer peer)
        {
            peer.RaiseExpandedChangedEvent(newValue);
        }
    }
}

public class SettingsExpanderAutomationPeer : FrameworkElementAutomationPeer
{
    public SettingsExpanderAutomationPeer(SettingsExpander owner)
        : base(owner)
    {
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.Group;
    }

    protected override string GetClassNameCore()
    {
        return Owner.GetType().Name;
    }

    protected override string GetNameCore()
    {
        var name = base.GetNameCore();

        if (!string.IsNullOrEmpty(name))
            return name;

        if (Owner is SettingsExpander { Header: string headerString } && !string.IsNullOrEmpty(headerString))
        {
            return headerString;
        }

        return name;
    }

    public void RaiseExpandedChangedEvent(bool newValue)
    {
        var newState = newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
        var oldState = newValue ? ExpandCollapseState.Collapsed : ExpandCollapseState.Expanded;

        RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldState, newState);
    }
}

public class SettingsExpanderItemStyleSelector : StyleSelector
{
    public Style DefaultStyle { get; set; }

    public Style ClickableStyle { get; set; }

    public override Style SelectStyle(object item, DependencyObject container)
    {
        if (item is SettingsCard { IsClickEnabled: true })
        {
            return ClickableStyle;
        }

        return DefaultStyle;
    }
}
