using DynamicData;
using Sawmill.Views.Recent;

namespace Sawmill.Views.Searching;

public interface IRecentSearchCollection
{
    IObservableList<RecentSearch> Items { get; }

    void Add(RecentSearch file);

    void Remove(RecentSearch file);
}