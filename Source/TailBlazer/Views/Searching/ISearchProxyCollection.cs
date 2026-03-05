using System.Collections.ObjectModel;
using TailBlazer.Domain.Infrastructure;

namespace TailBlazer.Views.Searching;

public interface ISearchProxyCollection: IDisposable
{
    IProperty<int> Count { get; }
    ReadOnlyObservableCollection<SearchOptionsProxy> Included { get; }
    ReadOnlyObservableCollection<SearchOptionsProxy> Excluded { get; }
}
