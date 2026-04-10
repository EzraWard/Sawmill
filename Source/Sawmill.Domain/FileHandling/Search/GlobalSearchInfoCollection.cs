using Sawmill.Domain.Infrastructure;

namespace Sawmill.Domain.FileHandling.Search;

public sealed class GlobalSearchInfoCollection //: ISearchInfoCollection
{
    private readonly ISearchMetadataCollection _searchMetadataCollection;

    public GlobalSearchInfoCollection(ISearchMetadataCollection searchMetadataCollection, ILogger logger)
    {
        _searchMetadataCollection = searchMetadataCollection;

           
    }
}