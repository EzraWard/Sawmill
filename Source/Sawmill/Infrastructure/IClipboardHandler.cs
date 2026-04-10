using Sawmill.Domain.Annotations;

namespace Sawmill.Infrastructure;

public interface IClipboardHandler
{
    void WriteToClipboard([NotNull] string text);
    void WriteToClipboard([NotNull] IEnumerable<string> items);
}