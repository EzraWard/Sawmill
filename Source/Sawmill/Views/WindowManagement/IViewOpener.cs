using Sawmill.Infrastructure;

namespace Sawmill.Views.WindowManagement;

public interface IViewOpener
{
    void OpenView(HeaderedView headeredView);
}