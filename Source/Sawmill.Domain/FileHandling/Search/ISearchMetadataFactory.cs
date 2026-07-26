using Sawmill.Domain.Annotations;

namespace Sawmill.Domain.FileHandling.Search;

public interface ISearchMetadataFactory
{
    SearchMetadata Create([NotNull] string searchText, bool useRegex, int index, bool filter, bool isGlobal = false);
}