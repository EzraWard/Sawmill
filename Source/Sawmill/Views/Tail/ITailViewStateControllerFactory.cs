using Sawmill.Domain.Annotations;

namespace Sawmill.Views.Tail;

public interface ITailViewStateControllerFactory
{
    IDisposable Create([NotNull] TailViewModel tailView, bool loadDefaults);
}