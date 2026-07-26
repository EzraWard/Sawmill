using System.Reactive.Linq;
using Sawmill.Domain.Formatting;
using Sawmill.Domain.Settings;

namespace Sawmill.Domain.Ratings;

public class RatingService: IRatingService
{
    public IObservable<RatingsMetaData> Metrics { get; }

    public RatingService(ISetting<GeneralOptions> setting, IRatingsMetricsLookup ratingMetricsLookup)
    {
        Metrics = setting.Value.Select(options => options.Rating)
            .DistinctUntilChanged()
            .Select(ratingMetricsLookup.Lookup);
    }
}