namespace Sawmill.Domain.Infrastructure;

public interface ILogFactory
{
    ILogger Create(string name);
    ILogger Create<T>();
}