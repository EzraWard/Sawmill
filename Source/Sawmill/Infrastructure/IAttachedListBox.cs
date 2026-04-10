using System.Windows.Controls;
using DynamicData;
using Sawmill.Views.Tail;

namespace Sawmill.Infrastructure;

public interface IAttachedListBox
{
    void Receive(ListBox selector);
}


public interface ISelectionMonitor: IDisposable
{
    string GetSelectedText();

    IEnumerable<string> GetSelectedItems();

    IObservableList<LineProxy> Selected { get; }
}