namespace Sawmill.Domain.FileHandling;

public interface IHasLimitationOfLines
{
    bool HasReachedLimit { get; }

    int Maximum { get; }
}