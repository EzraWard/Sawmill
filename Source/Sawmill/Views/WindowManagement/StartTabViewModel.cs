using System.Windows.Input;

namespace Sawmill.Views.WindowManagement;

public class StartTabViewModel
{
    public StartTabViewModel(ICommand openFileCommand)
    {
        OpenFileCommand = openFileCommand;
    }

    public ICommand OpenFileCommand { get; }
}
