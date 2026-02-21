using System.Windows.Input;

namespace TailBlazer.Views.WindowManagement;

public class StartTabViewModel
{
    public StartTabViewModel(ICommand openFileCommand)
    {
        OpenFileCommand = openFileCommand;
    }

    public ICommand OpenFileCommand { get; }
}
