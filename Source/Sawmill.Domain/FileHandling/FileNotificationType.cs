namespace Sawmill.Domain.FileHandling;

[Flags]
public enum FileNotificationType
{
    None ,
    CreatedOrOpened,
    Changed,
    Missing,
    Error
}