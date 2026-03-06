using System.IO;
using TailBlazer.Domain.Infrastructure;

namespace TailBlazer.Infrastructure;

public class SimpleFileLogger : ILogger
{
    private readonly string _name;
    private static readonly Lock _lock = new();
    private static readonly string LogPath;

    public string Name => _name;

    static SimpleFileLogger()
    {
        var logDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TailBlazer",
            "logs");
        Directory.CreateDirectory(logDir);
        LogPath = Path.Combine(logDir, "logger.log");
    }

    public SimpleFileLogger(Type type)
    {
        var name = type.Name;
        var genericArgs = type.GenericTypeArguments;

        if (!genericArgs.Any())
        {
            _name = name;
        }
        else
        {
            var startOfGeneric = name.IndexOf("`", StringComparison.Ordinal);
            name = name.Substring(0, startOfGeneric);
            var generics = string.Join(",", genericArgs.Select(t => t.Name));
            _name = $"{name}<{generics}>";
        }
    }

    public SimpleFileLogger(string name)
    {
        _name = name;
    }

    public void Debug(string message, params object[] values) => Write("DEBUG", Format(message, values));
    public void Info(string message, params object[] values) => Write("INFO", Format(message, values));
    public void Warn(string message, params object[] values) => Write("WARN", Format(message, values));
    public void Fatal(string message, params object[] values) => Write("FATAL", Format(message, values));

    public void Error(Exception ex, string message, params object[] values)
    {
        Write("ERROR", $"{Format(message, values)} {ex}");
    }

    private void Write(string level, string formatted)
    {
        var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss,fff} {level} {Environment.CurrentManagedThreadId} {_name} {formatted}";

        System.Diagnostics.Debug.WriteLine($"[Sawmill] {level} {DateTime.Now:HH:mm:ss,fff} - {formatted}");

        try
        {
            lock (_lock)
            {
                File.AppendAllText(LogPath, line + Environment.NewLine);
            }
        }
        catch
        {
            // Swallow file write errors to avoid crashing the app
        }
    }

    private static string Format(string message, object[] values)
    {
        if (values.Length == 0) return message;
        try { return string.Format(message, values); }
        catch { return message; }
    }
}
