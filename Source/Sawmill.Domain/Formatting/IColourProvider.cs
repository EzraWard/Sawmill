using DynamicData.Kernel;

namespace Sawmill.Domain.Formatting;

public interface IColourProvider
{
    IEnumerable<Hue> Hues { get; }

    Hue DefaultAccent { get; }

    Optional<Hue> Lookup(HueKey key);

    Hue GetAccent(Theme theme);
}