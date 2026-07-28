namespace Sawmill.Infrastructure;

public static class ApplicationInfo
{
    private static readonly Version Version =
        typeof(ApplicationInfo).Assembly.GetName().Version ?? new Version(0, 0, 0);

    public static string DisplayVersion { get; } = $"v{Version.ToString(3)}";
}
