using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

[assembly: XmlnsDefinition("http://materialdesigninxaml.net/winfx/xaml/themes", "MaterialDesignThemes.Wpf")]

namespace MaterialDesignThemes.Wpf;

public enum PackIconKind
{
    Information,
    InformationOutline,
    ArrowRightBold,
    Magnify,
    Regex,
    Eraser,
    Undo,
    FileOutline,
    Folder,
    ClipboardOutline,
    FormatPaint,
    FilterRemoveOutline,
    Download,
    Upload,
    Bug,
    AlertOutline,
    SquareRootBox,
    ExitToApp,
    Bank,
    Account,
    AccountMultiple,
    CurrencyUsd,
    CurrencyGbp,
    CurrencyEur,
    EmoticonDevil,
    EmoticonPoop
}

public class PackIcon : TextBlock
{
    public static readonly DependencyProperty KindProperty =
        DependencyProperty.Register(nameof(Kind), typeof(object), typeof(PackIcon), new PropertyMetadata(PackIconKind.Information, OnKindChanged));

    public object Kind
    {
        get => GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    private static void OnKindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PackIcon icon)
        {
            var key = e.NewValue?.ToString() ?? PackIconKind.Information.ToString();
            icon.Text = Glyphs.TryGetValue(key, out var glyph) ? glyph : "•";
            icon.FontFamily = new FontFamily("Segoe UI Symbol");
            icon.VerticalAlignment = VerticalAlignment.Center;
            icon.HorizontalAlignment = HorizontalAlignment.Center;
        }
    }

    private static readonly Dictionary<string, string> Glyphs = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Information"] = "ⓘ",
        ["InformationOutline"] = "ⓘ",
        ["ArrowRightBold"] = "➜",
        ["Magnify"] = "⌕",
        ["Regex"] = "∑",
        ["Eraser"] = "⌫",
        ["Undo"] = "↶",
        ["FileOutline"] = "📄",
        ["Folder"] = "📁",
        ["ClipboardOutline"] = "📋",
        ["FormatPaint"] = "🎨",
        ["FilterRemoveOutline"] = "⛔",
        ["Download"] = "↓",
        ["Upload"] = "↑"
    };
}

public class DialogHost : ContentControl
{
    public static readonly RoutedCommand CloseDialogCommand = new();

    public static readonly DependencyProperty IdentifierProperty =
        DependencyProperty.Register(nameof(Identifier), typeof(object), typeof(DialogHost), new PropertyMetadata(default(object)));
    public static readonly DependencyProperty DialogContentProperty =
        DependencyProperty.Register(nameof(DialogContent), typeof(object), typeof(DialogHost), new PropertyMetadata(default(object)));
    public static readonly DependencyProperty IsOpenProperty =
        DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(DialogHost), new PropertyMetadata(false));
    public static readonly DependencyProperty CloseOnClickAwayProperty =
        DependencyProperty.Register(nameof(CloseOnClickAway), typeof(bool), typeof(DialogHost), new PropertyMetadata(false));
    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(DialogHost), new PropertyMetadata(default(CornerRadius)));

    public object Identifier { get => GetValue(IdentifierProperty); set => SetValue(IdentifierProperty, value); }
    public object DialogContent { get => GetValue(DialogContentProperty); set => SetValue(DialogContentProperty, value); }
    public bool IsOpen { get => (bool)GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }
    public bool CloseOnClickAway { get => (bool)GetValue(CloseOnClickAwayProperty); set => SetValue(CloseOnClickAwayProperty, value); }
    public CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }

    public static Task<object?> Show(object content, object? dialogIdentifier = null) => Task.FromResult<object?>(null);
}

public class DrawerHost : ContentControl
{
    public static readonly DependencyProperty LeftDrawerContentProperty =
        DependencyProperty.Register(nameof(LeftDrawerContent), typeof(object), typeof(DrawerHost), new PropertyMetadata(default(object)));
    public static readonly DependencyProperty IsLeftDrawerOpenProperty =
        DependencyProperty.Register(nameof(IsLeftDrawerOpen), typeof(bool), typeof(DrawerHost), new PropertyMetadata(false));

    public object LeftDrawerContent { get => GetValue(LeftDrawerContentProperty); set => SetValue(LeftDrawerContentProperty, value); }
    public bool IsLeftDrawerOpen { get => (bool)GetValue(IsLeftDrawerOpenProperty); set => SetValue(IsLeftDrawerOpenProperty, value); }
}

public enum PopupBoxPopupMode { Click, MouseOver }

public class PopupBox : ContentControl
{
    public static readonly DependencyProperty ToggleContentProperty =
        DependencyProperty.Register(nameof(ToggleContent), typeof(object), typeof(PopupBox), new PropertyMetadata(default(object)));
    public static readonly DependencyProperty PopupModeProperty =
        DependencyProperty.Register(nameof(PopupMode), typeof(PopupBoxPopupMode), typeof(PopupBox), new PropertyMetadata(PopupBoxPopupMode.Click));
    public static readonly DependencyProperty PlacementModeProperty =
        DependencyProperty.Register(nameof(PlacementMode), typeof(PlacementMode), typeof(PopupBox), new PropertyMetadata(PlacementMode.Bottom));

    public object ToggleContent { get => GetValue(ToggleContentProperty); set => SetValue(ToggleContentProperty, value); }
    public PopupBoxPopupMode PopupMode { get => (PopupBoxPopupMode)GetValue(PopupModeProperty); set => SetValue(PopupModeProperty, value); }
    public PlacementMode PlacementMode { get => (PlacementMode)GetValue(PlacementModeProperty); set => SetValue(PlacementModeProperty, value); }
}

public class Transitioner : TabControl { }
public class TransitionerSlide : ContentControl
{
    public Collection<TransitionEffect> OpeningEffects { get; } = [];
}
public class TransitionEffect : DependencyObject
{
    public static readonly DependencyProperty KindProperty = DependencyProperty.Register(nameof(Kind), typeof(string), typeof(TransitionEffect));
    public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(nameof(Duration), typeof(Duration), typeof(TransitionEffect));
    public static readonly DependencyProperty OffsetTimeProperty = DependencyProperty.Register(nameof(OffsetTime), typeof(TimeSpan), typeof(TransitionEffect));
    public string Kind { get => (string)GetValue(KindProperty); set => SetValue(KindProperty, value); }
    public Duration Duration { get => (Duration)GetValue(DurationProperty); set => SetValue(DurationProperty, value); }
    public TimeSpan OffsetTime { get => (TimeSpan)GetValue(OffsetTimeProperty); set => SetValue(OffsetTimeProperty, value); }
}

public class Ripple : ContentControl
{
    public static readonly DependencyProperty FeedbackProperty =
        DependencyProperty.Register(nameof(Feedback), typeof(Brush), typeof(Ripple), new PropertyMetadata(default(Brush)));
    public Brush Feedback { get => (Brush)GetValue(FeedbackProperty); set => SetValue(FeedbackProperty, value); }
}

public static class HintAssist
{
    public static readonly DependencyProperty HintProperty = DependencyProperty.RegisterAttached("Hint", typeof(object), typeof(HintAssist), new PropertyMetadata(default(object)));
    public static readonly DependencyProperty IsFloatingProperty = DependencyProperty.RegisterAttached("IsFloating", typeof(bool), typeof(HintAssist), new PropertyMetadata(false));
    public static readonly DependencyProperty HintOpacityProperty = DependencyProperty.RegisterAttached("HintOpacity", typeof(double), typeof(HintAssist), new PropertyMetadata(0.56d));
    public static object GetHint(DependencyObject obj) => obj.GetValue(HintProperty);
    public static void SetHint(DependencyObject obj, object value) => obj.SetValue(HintProperty, value);
    public static bool GetIsFloating(DependencyObject obj) => (bool)obj.GetValue(IsFloatingProperty);
    public static void SetIsFloating(DependencyObject obj, bool value) => obj.SetValue(IsFloatingProperty, value);
    public static double GetHintOpacity(DependencyObject obj) => (double)obj.GetValue(HintOpacityProperty);
    public static void SetHintOpacity(DependencyObject obj, double value) => obj.SetValue(HintOpacityProperty, value);
}

public static class ValidationAssist
{
    public static readonly DependencyProperty UsePopupProperty = DependencyProperty.RegisterAttached("UsePopup", typeof(bool), typeof(ValidationAssist), new PropertyMetadata(false));
    public static readonly DependencyProperty OnlyShowOnFocusProperty = DependencyProperty.RegisterAttached("OnlyShowOnFocus", typeof(bool), typeof(ValidationAssist), new PropertyMetadata(false));
    public static readonly DependencyProperty SuppressProperty = DependencyProperty.RegisterAttached("Suppress", typeof(bool), typeof(ValidationAssist), new PropertyMetadata(false));
    public static bool GetUsePopup(DependencyObject obj) => (bool)obj.GetValue(UsePopupProperty);
    public static void SetUsePopup(DependencyObject obj, bool value) => obj.SetValue(UsePopupProperty, value);
    public static bool GetOnlyShowOnFocus(DependencyObject obj) => (bool)obj.GetValue(OnlyShowOnFocusProperty);
    public static void SetOnlyShowOnFocus(DependencyObject obj, bool value) => obj.SetValue(OnlyShowOnFocusProperty, value);
    public static bool GetSuppress(DependencyObject obj) => (bool)obj.GetValue(SuppressProperty);
    public static void SetSuppress(DependencyObject obj, bool value) => obj.SetValue(SuppressProperty, value);
}

public static class ShadowAssist
{
    public static readonly DependencyProperty ShadowDepthProperty = DependencyProperty.RegisterAttached("ShadowDepth", typeof(object), typeof(ShadowAssist), new PropertyMetadata(default(object)));
    public static object GetShadowDepth(DependencyObject obj) => obj.GetValue(ShadowDepthProperty);
    public static void SetShadowDepth(DependencyObject obj, object value) => obj.SetValue(ShadowDepthProperty, value);
}

public static class TextFieldAssist
{
    public static readonly DependencyProperty DecorationVisibilityProperty =
        DependencyProperty.RegisterAttached("DecorationVisibility", typeof(Visibility), typeof(TextFieldAssist), new PropertyMetadata(Visibility.Collapsed));
    public static Visibility GetDecorationVisibility(DependencyObject obj) => (Visibility)obj.GetValue(DecorationVisibilityProperty);
    public static void SetDecorationVisibility(DependencyObject obj, Visibility value) => obj.SetValue(DecorationVisibilityProperty, value);
}

public class SmartHint : TextBlock
{
    public static readonly DependencyProperty HintProperty =
        DependencyProperty.Register(nameof(Hint), typeof(object), typeof(SmartHint), new PropertyMetadata(default(object), OnHintChanged));
    public static readonly DependencyProperty HintProxyProperty =
        DependencyProperty.Register(nameof(HintProxy), typeof(object), typeof(SmartHint), new PropertyMetadata(default(object)));
    public static readonly DependencyProperty HintOpacityProperty =
        DependencyProperty.Register(nameof(HintOpacity), typeof(double), typeof(SmartHint), new PropertyMetadata(0.56d));
    public static readonly DependencyProperty UseFloatingProperty =
        DependencyProperty.Register(nameof(UseFloating), typeof(bool), typeof(SmartHint), new PropertyMetadata(false));

    public object Hint { get => GetValue(HintProperty); set => SetValue(HintProperty, value); }
    public object HintProxy { get => GetValue(HintProxyProperty); set => SetValue(HintProxyProperty, value); }
    public double HintOpacity { get => (double)GetValue(HintOpacityProperty); set => SetValue(HintOpacityProperty, value); }
    public bool UseFloating { get => (bool)GetValue(UseFloatingProperty); set => SetValue(UseFloatingProperty, value); }
    public bool IsContentNullOrEmpty => string.IsNullOrEmpty(Hint?.ToString());

    private static void OnHintChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SmartHint hint)
            hint.Text = e.NewValue?.ToString() ?? string.Empty;
    }
}

public class Underline : Border
{
    public static readonly DependencyProperty IsActiveProperty =
        DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(Underline), new PropertyMetadata(false));
    public bool IsActive { get => (bool)GetValue(IsActiveProperty); set => SetValue(IsActiveProperty, value); }
}

public class PopupEx : Popup { }
public class ComboBoxPopup : Popup
{
    public static readonly DependencyProperty DefaultVerticalOffsetProperty = DependencyProperty.Register(nameof(DefaultVerticalOffset), typeof(double), typeof(ComboBoxPopup), new PropertyMetadata(0d));
    public static readonly DependencyProperty DownVerticalOffsetProperty = DependencyProperty.Register(nameof(DownVerticalOffset), typeof(double), typeof(ComboBoxPopup), new PropertyMetadata(0d));
    public static readonly DependencyProperty UpVerticalOffsetProperty = DependencyProperty.Register(nameof(UpVerticalOffset), typeof(double), typeof(ComboBoxPopup), new PropertyMetadata(0d));
    public double DefaultVerticalOffset { get => (double)GetValue(DefaultVerticalOffsetProperty); set => SetValue(DefaultVerticalOffsetProperty, value); }
    public double DownVerticalOffset { get => (double)GetValue(DownVerticalOffsetProperty); set => SetValue(DownVerticalOffsetProperty, value); }
    public double UpVerticalOffset { get => (double)GetValue(UpVerticalOffsetProperty); set => SetValue(UpVerticalOffsetProperty, value); }
}

public sealed class HintProxyFabricConverter : IValueConverter
{
    public static HintProxyFabricConverter Instance { get; } = new();
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => null;
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
}

public enum Theme
{
    Dark,
    Light
}

public interface ITheme
{
    Theme BaseTheme { get; set; }
    Color SecondaryColor { get; set; }
    void SetBaseTheme(Theme theme);
    void SetSecondaryColor(Color color);
}

internal sealed class StubTheme : ITheme
{
    public Theme BaseTheme { get; set; } = Theme.Dark;
    public Color SecondaryColor { get; set; } = Colors.SteelBlue;
    public void SetBaseTheme(Theme theme) => BaseTheme = theme;
    public void SetSecondaryColor(Color color) => SecondaryColor = color;
}

public sealed class PaletteHelper
{
    private static readonly StubTheme ThemeState = new();
    public ITheme GetTheme() => ThemeState;
    public void SetTheme(ITheme theme)
    {
        ThemeState.BaseTheme = theme.BaseTheme;
        ThemeState.SecondaryColor = theme.SecondaryColor;
    }
}
