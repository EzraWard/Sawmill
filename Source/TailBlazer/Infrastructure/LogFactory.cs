using TailBlazer.Domain.Infrastructure;

namespace TailBlazer.Infrastructure;

public class LogFactory : ILogFactory
{
    public ILogger Create(string name)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));
        return new SimpleFileLogger(name);
    }
    public ILogger Create<T>()
    {
        return new SimpleFileLogger(typeof(T));
    }
}