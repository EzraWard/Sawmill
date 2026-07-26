using DynamicData.Kernel;
using Sawmill.Domain.Infrastructure;
using Sawmill.Views;

namespace Sawmill.Infrastructure;

public class ViewFactoryService : IViewFactoryRegister, IViewFactoryProvider
{
    private readonly IObjectProvider _objectProvider;
    private readonly IDictionary<string, IViewModelFactory> _viewFactories = new Dictionary<string, IViewModelFactory>();

    public ViewFactoryService(IObjectProvider objectProvider)
    {
        _objectProvider = objectProvider;
    }

    public void Register<T>()
        where T:IViewModelFactory
    {
        var register = (IViewModelFactory)_objectProvider.Get<T>();

        _viewFactories[register.Key] = register;
    }

    public Optional<IViewModelFactory> Lookup(string key)
    {
        return _viewFactories.Lookup(key);
    }
}