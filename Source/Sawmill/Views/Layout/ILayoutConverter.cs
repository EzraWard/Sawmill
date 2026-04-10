using System.Xml.Linq;

namespace Sawmill.Views.Layout;

public interface ILayoutConverter
{
    XElement CaptureState();
    void Restore(XElement element);
}