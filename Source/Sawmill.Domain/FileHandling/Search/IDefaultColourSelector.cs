using Sawmill.Domain.Formatting;

namespace Sawmill.Domain.FileHandling.Search;

public interface IDefaultColourSelector
{
    Hue Select(string text);
    Hue Lookup(HueKey key);
}