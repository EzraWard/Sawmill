namespace Sawmill.Views.WindowManagement;

public interface IWindowFactory
{
    MainWindow Create(IEnumerable<string> files = null);
}