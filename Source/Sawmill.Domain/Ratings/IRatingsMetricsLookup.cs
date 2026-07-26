using Sawmill.Domain.Settings;

namespace Sawmill.Domain.Ratings;

public interface IRatingsMetricsLookup
{
    RatingsMetaData Lookup(int rating);
}