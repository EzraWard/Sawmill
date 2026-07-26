using System.ComponentModel;

namespace Sawmill.Views.DialogServices;

public interface IDialogViewModel : INotifyPropertyChanged
{
    bool IsDialogOpen { get; set; }
    object DialogContent { get; set; }
}