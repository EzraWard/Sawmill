using Sawmill.Domain.Settings;

namespace Sawmill.Domain.Ratings;

public interface IRatingService
{
    IObservable<RatingsMetaData> Metrics { get; }
}