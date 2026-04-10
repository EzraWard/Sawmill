using Sawmill.Domain.FileHandling;
using Sawmill.Domain.FileHandling.Search;

namespace Sawmill.Views.Tail;

public interface IInlineViewerFactory
{
    InlineViewer Create(ICombinedSearchMetadataCollection combinedSearchMetadataCollection, IObservable<ILineProvider> lineProvider,IObservable<LineProxy> selectedChanged);
}