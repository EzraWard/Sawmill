using Sawmill.Domain.FileHandling.Search;

namespace Sawmill.Views.Searching;

public interface ISearchProxyCollectionFactory
{
    ISearchProxyCollection Create(ISearchMetadataCollection metadataCollection, Guid id, Action<SearchMetadata> changeScopeAction);
}