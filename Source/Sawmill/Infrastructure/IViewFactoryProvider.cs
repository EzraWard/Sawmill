using DynamicData.Kernel;
using Sawmill.Views;

namespace Sawmill.Infrastructure;

public interface IViewFactoryProvider
{
    Optional<IViewModelFactory> Lookup(string key);
}