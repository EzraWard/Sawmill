
using Sawmill.Views;

namespace Sawmill.Infrastructure;

public interface IViewFactoryRegister
{
    void Register<T>()
        where T:IViewModelFactory;
}