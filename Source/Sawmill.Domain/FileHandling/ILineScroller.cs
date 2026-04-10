using System.Reactive.Linq;
using DynamicData;
using DynamicData.Aggregation;
using Sawmill.Domain.Annotations;

namespace Sawmill.Domain.FileHandling;

public interface ILineScroller : IDisposable
{
    IObservableCache<Line, LineKey> Lines { get; }
}

public static class LineScrollerEx
{
    public static IObservable<int> MaximumLines([NotNull] this ILineScroller source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.Lines.Connect()
            .Maximum(l => l.Text?.Length ?? 0)
            .StartWith(0)
            .DistinctUntilChanged();
    }
}