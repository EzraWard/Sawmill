using System.Reactive.Linq;
using Sawmill.Domain.Annotations;

namespace Sawmill.Domain.FileHandling;

public static class ExludeLinesEx
{
    public static IObservable<ILineProvider> Exclude([NotNull] this IObservable<ILineProvider> source,
        [NotNull] Func<string, bool> exlusionFilter)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (exlusionFilter == null) throw new ArgumentNullException(nameof(exlusionFilter));

        return source
            .Select(lineProvider => new ExludedLinesProvider(lineProvider, exlusionFilter));
    }
}