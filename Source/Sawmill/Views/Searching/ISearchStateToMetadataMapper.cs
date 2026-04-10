using Sawmill.Domain.FileHandling.Search;

namespace Sawmill.Views.Searching;

public interface ISearchStateToMetadataMapper
{
    SearchMetadata Map(SearchState state, bool isGlobal=false);
    SearchState Map(SearchMetadata search);
}