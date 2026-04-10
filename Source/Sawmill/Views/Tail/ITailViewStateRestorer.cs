using Sawmill.Domain.Settings;

namespace Sawmill.Views.Tail;

public interface ITailViewStateRestorer
{
    void Restore(TailViewModel view, State state);
    void Restore(TailViewModel view, TailViewState tailviewstate);
}