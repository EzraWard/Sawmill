using System.Collections.ObjectModel;
using Sawmill.Domain.Infrastructure;

namespace Sawmill.Views.Searching;

public interface ISearchProxyCollection: IDisposable
{
    IProperty<int> Count { get; }
    ReadOnlyObservableCollection<SearchOptionsProxy> Included { get; }
    ReadOnlyObservableCollection<SearchOptionsProxy> Excluded { get; }
}
