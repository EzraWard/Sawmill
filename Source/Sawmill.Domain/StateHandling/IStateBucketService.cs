using DynamicData.Kernel;
using Sawmill.Domain.Settings;

namespace Sawmill.Domain.StateHandling;

/// <summary>
/// A simple means for dumping stuff to a file
/// </summary>
public interface IStateBucketService
{
    void Write(string type, string id, State state);
    Optional<State> Lookup(string type, string id);
}