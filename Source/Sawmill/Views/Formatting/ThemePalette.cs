using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Sawmill.Views.Formatting;

/// <summary>
/// Semantic colors used by Sawmill's custom controls. Keeping the complete
/// palettes here makes every surface change together and gives us one place
/// to verify foreground/background contrast.
/// </summary>
public sealed class ThemePalette
{
    private ThemePalette(bool isDark, IDictionary<string, Color> colors)
    {
        IsDark = isDark;
        Colors = new ReadOnlyDictionary<string, Color>(colors);
    }

    public bool IsDark { get; }

    public IReadOnlyDictionary<string, Color> Colors { get; }

    public Color this[string key] => Colors[key];

    public static ThemePalette Dark { get; } = new(
        true,
        new Dictionary<string, Color>
        {
            // Core surfaces and text
            ["MaterialDesignBackground"] = Parse("#FF202020"),
            ["MaterialDesignPaper"] = Parse("#FF2B2B2B"),
            ["MaterialDesignBody"] = Parse("#FFF5F5F5"),
            ["MaterialDesignBodySecondary"] = Parse("#FFC7C7C7"),
            ["MaterialDesignTextBoxBorder"] = Parse("#FF858585"),
            ["MaterialDesignFlatButtonClick"] = Parse("#22FFFFFF"),

            // Accent surfaces always carry their paired foreground
            ["PrimaryHueMidBrush"] = Parse("#FF0F6CBD"),
            ["PrimaryHueMidForegroundBrush"] = Parse("#FFFFFFFF"),
            ["PrimaryHueDarkBrush"] = Parse("#FF004578"),
            ["PrimaryHueDarkForegroundBrush"] = Parse("#FFFFFFFF"),
            ["PrimaryHueLightBrush"] = Parse("#FF60A5FA"),
            ["PrimaryHueLightForegroundBrush"] = Parse("#FF0B1220"),
            ["SecondaryHueMidBrush"] = Parse("#FF0F6CBD"),
            ["SecondaryHueMidForegroundBrush"] = Parse("#FFFFFFFF"),
            ["AccentTextBrush"] = Parse("#FF8CC8FF"),
            ["ValidationErrorBrush"] = Parse("#FFFF99A4"),
            ["GrayBrush2"] = Parse("#FFADADAD"),

            // Window, content, and command surfaces
            ["SettingsMicaBrush"] = Parse("#FF202020"),
            ["SettingsCardBrush"] = Parse("#FF2D2D2D"),
            ["SettingsCardBorderBrush"] = Parse("#FF3D3D3D"),
            ["TabSelectedBrush"] = Parse("#FF2B2B2B"),
            ["TabHoverBrush"] = Parse("#18FFFFFF"),
            ["CaptionButtonHoverBrush"] = Parse("#22FFFFFF"),
            ["ContentAreaBrush"] = Parse("#FF2B2B2B"),
            ["ToolbarBackgroundBrush"] = Parse("#FF242424"),
            ["DividerBrush"] = Parse("#FF767676"),
            ["SubtleHoverBrush"] = Parse("#18FFFFFF"),
            ["SubtlePressedBrush"] = Parse("#26FFFFFF"),

            // Settings cards
            ["SettingsCardBackgroundBrush"] = Parse("#FF2D2D2D"),
            ["SettingsCardBackgroundPointerOverBrush"] = Parse("#FF343434"),
            ["SettingsCardBackgroundPressedBrush"] = Parse("#FF292929"),
            ["SettingsCardBackgroundDisabledBrush"] = Parse("#FF292929"),
            ["SettingsCardStrokeBrush"] = Parse("#FF3D3D3D"),
            ["SettingsCardStrokePointerOverBrush"] = Parse("#FF555555"),
            ["SettingsCardDescriptionForegroundBrush"] = Parse("#FFC7C7C7"),

            // WinUI-style toggle switch
            ["SettingsToggleTrackBrush"] = Parse("#FF2D2D2D"),
            ["SettingsToggleTrackPointerOverBrush"] = Parse("#FF3A3A3A"),
            ["SettingsToggleTrackBorderBrush"] = Parse("#FF9A9A9A"),
            ["SettingsToggleTrackBorderPointerOverBrush"] = Parse("#FFC7C7C7"),
            ["SettingsToggleThumbBrush"] = Parse("#FFC7C7C7")
        });

    public static ThemePalette Light { get; } = new(
        false,
        new Dictionary<string, Color>
        {
            // Core surfaces and text
            ["MaterialDesignBackground"] = Parse("#FFF3F3F3"),
            ["MaterialDesignPaper"] = Parse("#FFFFFFFF"),
            ["MaterialDesignBody"] = Parse("#FF1A1A1A"),
            ["MaterialDesignBodySecondary"] = Parse("#FF5D5D5D"),
            ["MaterialDesignTextBoxBorder"] = Parse("#FF737373"),
            ["MaterialDesignFlatButtonClick"] = Parse("#14000000"),

            // Accent surfaces always carry their paired foreground
            ["PrimaryHueMidBrush"] = Parse("#FF005FB8"),
            ["PrimaryHueMidForegroundBrush"] = Parse("#FFFFFFFF"),
            ["PrimaryHueDarkBrush"] = Parse("#FF004578"),
            ["PrimaryHueDarkForegroundBrush"] = Parse("#FFFFFFFF"),
            ["PrimaryHueLightBrush"] = Parse("#FFD7E8FF"),
            ["PrimaryHueLightForegroundBrush"] = Parse("#FF0F172A"),
            ["SecondaryHueMidBrush"] = Parse("#FF005FB8"),
            ["SecondaryHueMidForegroundBrush"] = Parse("#FFFFFFFF"),
            ["AccentTextBrush"] = Parse("#FF005FB8"),
            ["ValidationErrorBrush"] = Parse("#FFB10E1C"),
            ["GrayBrush2"] = Parse("#FF666666"),

            // Window, content, and command surfaces
            ["SettingsMicaBrush"] = Parse("#FFF3F3F3"),
            ["SettingsCardBrush"] = Parse("#FFFFFFFF"),
            ["SettingsCardBorderBrush"] = Parse("#FFE5E5E5"),
            ["TabSelectedBrush"] = Parse("#FFFFFFFF"),
            ["TabHoverBrush"] = Parse("#0F000000"),
            ["CaptionButtonHoverBrush"] = Parse("#14000000"),
            ["ContentAreaBrush"] = Parse("#FFFFFFFF"),
            ["ToolbarBackgroundBrush"] = Parse("#FFF7F7F7"),
            ["DividerBrush"] = Parse("#FF8A8A8A"),
            ["SubtleHoverBrush"] = Parse("#0F000000"),
            ["SubtlePressedBrush"] = Parse("#1F000000"),

            // Settings cards
            ["SettingsCardBackgroundBrush"] = Parse("#FFFFFFFF"),
            ["SettingsCardBackgroundPointerOverBrush"] = Parse("#FFF9F9F9"),
            ["SettingsCardBackgroundPressedBrush"] = Parse("#FFF0F0F0"),
            ["SettingsCardBackgroundDisabledBrush"] = Parse("#FFF8F8F8"),
            ["SettingsCardStrokeBrush"] = Parse("#FFE5E5E5"),
            ["SettingsCardStrokePointerOverBrush"] = Parse("#FFC8C8C8"),
            ["SettingsCardDescriptionForegroundBrush"] = Parse("#FF5D5D5D"),

            // WinUI-style toggle switch
            ["SettingsToggleTrackBrush"] = Parse("#FFFFFFFF"),
            ["SettingsToggleTrackPointerOverBrush"] = Parse("#FFF5F5F5"),
            ["SettingsToggleTrackBorderBrush"] = Parse("#FF737373"),
            ["SettingsToggleTrackBorderPointerOverBrush"] = Parse("#FF5D5D5D"),
            ["SettingsToggleThumbBrush"] = Parse("#FF5D5D5D")
        });

    private static Color Parse(string value)
    {
        return (Color)ColorConverter.ConvertFromString(value);
    }
}
