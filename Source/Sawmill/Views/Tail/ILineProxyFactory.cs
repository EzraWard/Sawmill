using Sawmill.Domain.FileHandling;

namespace Sawmill.Views.Tail;

public interface ILineProxyFactory
{
    LineProxy Create(Line line);
}