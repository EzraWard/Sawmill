namespace Sawmill.Domain.Infrastructure;

public interface IProperty<out T>: IDisposable
{
    T Value { get; }
}