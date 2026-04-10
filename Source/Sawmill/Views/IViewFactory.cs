using Sawmill.Domain.Settings;
using Sawmill.Infrastructure;

namespace Sawmill.Views;

public interface IViewModelFactory
{
    HeaderedView Create(ViewState state);

    string Key { get; }
}