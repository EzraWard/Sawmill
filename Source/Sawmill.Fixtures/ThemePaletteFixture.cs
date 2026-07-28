using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sawmill.Views.Formatting;
using System.Windows.Media;

namespace Sawmill.Fixtures;

[TestClass]
public class ThemePaletteFixture
{
    private static readonly (string Foreground, string Background)[] ReadabilityPairs =
    [
        ("MaterialDesignBody", "MaterialDesignBackground"),
        ("MaterialDesignBody", "MaterialDesignPaper"),
        ("MaterialDesignBody", "ContentAreaBrush"),
        ("MaterialDesignBody", "ToolbarBackgroundBrush"),
        ("MaterialDesignBodySecondary", "MaterialDesignPaper"),
        ("SettingsCardDescriptionForegroundBrush", "SettingsCardBackgroundBrush"),
        ("AccentTextBrush", "MaterialDesignPaper"),
        ("ValidationErrorBrush", "MaterialDesignPaper"),
        ("PrimaryHueMidForegroundBrush", "PrimaryHueMidBrush"),
        ("PrimaryHueDarkForegroundBrush", "PrimaryHueDarkBrush"),
        ("PrimaryHueLightForegroundBrush", "PrimaryHueLightBrush"),
        ("SecondaryHueMidForegroundBrush", "SecondaryHueMidBrush")
    ];

    private static readonly (string Boundary, string Surface)[] BoundaryPairs =
    [
        // Passive card strokes are intentionally subtle Fluent elevation cues.
        // Only actionable control and structural boundaries require 3:1 contrast.
        ("MaterialDesignTextBoxBorder", "MaterialDesignPaper"),
        ("DividerBrush", "ToolbarBackgroundBrush"),
        ("SettingsToggleTrackBorderBrush", "SettingsCardBackgroundBrush")
    ];

    [TestMethod]
    public void LightAndDarkPalettesShouldProvideTheSameResources()
    {
        ThemePalette.Light.Colors.Keys.Should()
            .BeEquivalentTo(ThemePalette.Dark.Colors.Keys);
    }

    [TestMethod]
    public void ThemeForegroundsShouldMeetNormalTextContrast()
    {
        foreach (var palette in new[] { ThemePalette.Light, ThemePalette.Dark })
        {
            foreach (var pair in ReadabilityPairs)
            {
                var contrast = ContrastRatio(
                    palette[pair.Foreground],
                    palette[pair.Background]);

                contrast.Should().BeGreaterThanOrEqualTo(
                    4.5,
                    $"{pair.Foreground} on {pair.Background} must remain readable in " +
                    $"{(palette.IsDark ? "dark" : "light")} mode");
            }
        }
    }

    [TestMethod]
    public void ControlAndSurfaceBoundariesShouldRemainDistinguishable()
    {
        foreach (var palette in new[] { ThemePalette.Light, ThemePalette.Dark })
        {
            foreach (var pair in BoundaryPairs)
            {
                var contrast = ContrastRatio(
                    palette[pair.Boundary],
                    palette[pair.Surface]);

                contrast.Should().BeGreaterThanOrEqualTo(
                    3,
                    $"{pair.Boundary} must distinguish {pair.Surface} in " +
                    $"{(palette.IsDark ? "dark" : "light")} mode");
            }
        }
    }

    private static double ContrastRatio(Color first, Color second)
    {
        var lighter = Math.Max(RelativeLuminance(first), RelativeLuminance(second));
        var darker = Math.Min(RelativeLuminance(first), RelativeLuminance(second));
        return (lighter + 0.05) / (darker + 0.05);
    }

    private static double RelativeLuminance(Color color)
    {
        return (0.2126 * LinearChannel(color.R)) +
               (0.7152 * LinearChannel(color.G)) +
               (0.0722 * LinearChannel(color.B));
    }

    private static double LinearChannel(byte channel)
    {
        var value = channel / 255d;
        return value <= 0.04045
            ? value / 12.92
            : Math.Pow((value + 0.055) / 1.055, 2.4);
    }
}
