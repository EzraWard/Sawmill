using System.Reactive.Concurrency;

namespace Sawmill.Domain.Infrastructure;

public interface ISchedulerProvider
{
    IScheduler MainThread { get; }
    IScheduler Background { get; }
}