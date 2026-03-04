using System.Windows.Media;

namespace MaterialDesignColors;

public sealed class Hue
{
    public Hue(string name, Color color, Color foreground)
    {
        Name = name;
        Color = color;
        Foreground = foreground;
    }

    public string Name { get; }
    public Color Color { get; }
    public Color Foreground { get; }
}

public sealed class Swatch
{
    public Swatch(string name, bool isAccented, IEnumerable<Hue> accentHues, IEnumerable<Hue> primaryHues)
    {
        Name = name;
        IsAccented = isAccented;
        AccentHues = accentHues.ToArray();
        PrimaryHues = primaryHues.ToArray();
        AccentExemplarHue = AccentHues.FirstOrDefault() ?? PrimaryHues.First();
    }

    public string Name { get; }
    public bool IsAccented { get; }
    public IReadOnlyCollection<Hue> AccentHues { get; }
    public IReadOnlyCollection<Hue> PrimaryHues { get; }
    public Hue AccentExemplarHue { get; }
}

public sealed class SwatchesProvider
{
    public IEnumerable<Swatch> Swatches => CreateSwatches();

    private static IEnumerable<Swatch> CreateSwatches()
    {
        foreach (var (name, accent, primary, fg) in Palette)
        {
            var accentHues = new[]
            {
                new Hue("Accent200", Shift(accent, 0.20), fg),
                new Hue("Accent400", Shift(accent, 0.10), fg),
                new Hue("Accent500", accent, fg),
                new Hue("Accent700", Shift(accent, -0.15), fg)
            };
            var primaryHues = new[]
            {
                new Hue("Primary200", primary, fg),
                new Hue("Primary300", primary, fg),
                new Hue("Primary400", primary, fg),
                new Hue("Primary500", primary, fg)
            };

            yield return new Swatch(name, true, accentHues, primaryHues);
        }
    }

    private static Color Shift(Color color, double factor)
    {
        byte shift(byte c) => factor >= 0
            ? (byte)Math.Min(255, c + (255 - c) * factor)
            : (byte)Math.Max(0, c * (1 + factor));
        return Color.FromRgb(shift(color.R), shift(color.G), shift(color.B));
    }

    private static readonly (string Name, Color Accent, Color Primary, Color Foreground)[] Palette =
    [
        ("yellow", Color.FromRgb(0xF5, 0xC5, 0x42), Color.FromRgb(0xA0, 0x88, 0x2A), Colors.Black),
        ("amber", Color.FromRgb(0xFF, 0xB3, 0x00), Color.FromRgb(0x9A, 0x6A, 0x00), Colors.Black),
        ("lightgreen", Color.FromRgb(0x76, 0xD2, 0x75), Color.FromRgb(0x3D, 0x86, 0x3D), Colors.Black),
        ("green", Color.FromRgb(0x47, 0x9C, 0x61), Color.FromRgb(0x2C, 0x67, 0x3F), Colors.White),
        ("lime", Color.FromRgb(0xAA, 0xD9, 0x4A), Color.FromRgb(0x6A, 0x89, 0x2C), Colors.Black),
        ("teal", Color.FromRgb(0x33, 0xB5, 0x9E), Color.FromRgb(0x22, 0x6F, 0x63), Colors.White),
        ("cyan", Color.FromRgb(0x25, 0xB8, 0xD7), Color.FromRgb(0x1B, 0x78, 0x8D), Colors.White),
        ("lightblue", Color.FromRgb(0x5D, 0xA8, 0xFF), Color.FromRgb(0x2F, 0x5C, 0x96), Colors.White),
        ("blue", Color.FromRgb(0x3C, 0x82, 0xF6), Color.FromRgb(0x1E, 0x4F, 0x9E), Colors.White),
        ("indigo", Color.FromRgb(0x64, 0x6F, 0xE8), Color.FromRgb(0x37, 0x3E, 0x90), Colors.White),
        ("orange", Color.FromRgb(0xF5, 0x8A, 0x3D), Color.FromRgb(0x96, 0x54, 0x22), Colors.White),
        ("deeporange", Color.FromRgb(0xE8, 0x64, 0x3F), Color.FromRgb(0x8B, 0x3A, 0x25), Colors.White),
        ("pink", Color.FromRgb(0xE3, 0x69, 0xB0), Color.FromRgb(0x8A, 0x3D, 0x6A), Colors.White),
        ("red", Color.FromRgb(0xE8, 0x5A, 0x5A), Color.FromRgb(0x93, 0x2F, 0x2F), Colors.White),
        ("purple", Color.FromRgb(0xA0, 0x6C, 0xD5), Color.FromRgb(0x62, 0x40, 0x84), Colors.White),
        ("deeppurple", Color.FromRgb(0x74, 0x61, 0xC6), Color.FromRgb(0x48, 0x3C, 0x84), Colors.White),
        ("gray", Color.FromRgb(0x8A, 0x8E, 0x99), Color.FromRgb(0x5A, 0x5E, 0x67), Colors.White),
        ("bluegrey", Color.FromRgb(0x7E, 0x93, 0xAB), Color.FromRgb(0x4E, 0x5F, 0x73), Colors.White)
    ];
}
