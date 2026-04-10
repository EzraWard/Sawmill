using DynamicData;
using Sawmill.Infrastructure;

namespace Sawmill.Views.WindowManagement;

public interface IWindowsController
{
    IObservableCache<HeaderedView, Guid> Views { get; }

    void Register(HeaderedView item);
    void Remove(HeaderedView item);
}