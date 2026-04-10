using DynamicData;
using Sawmill.Domain.FileHandling.Search;

namespace Sawmill.Views.Formatting;

public interface IIconProvider
{
    IObservableList<IconDescription> Icons { get; }

    IDefaultIconSelector DefaultIconSelector { get; }
}