namespace Sawmill.Infrastructure.KeyboardNavigation;

public interface IPageProvider
{
    int PageSize { get; }
    int FirstIndex { get; }
}