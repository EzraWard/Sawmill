using DynamicData;

namespace Sawmill.Domain.FileHandling.Search;

public interface ICombinedSearchMetadataCollection : IDisposable
{
    IObservableCache<SearchMetadata, string> Combined { get; }
    ISearchMetadataCollection Local { get; }
    ISearchMetadataCollection Global { get; }
}