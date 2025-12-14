using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static readonly string LogFile = Path.Combine(AppContext.BaseDirectory, "sawmill_simulation.log");
    static readonly Random Rand = new();

    static async Task Main()
    {
        Console.WriteLine($"Writing log lines to: {LogFile}");
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (s, e) => { e.Cancel = true; cts.Cancel(); };

        Directory.CreateDirectory(Path.GetDirectoryName(LogFile) ?? AppContext.BaseDirectory);

        while (!cts.IsCancellationRequested)
        {
            var line = GenerateLogLine();
            await File.AppendAllTextAsync(LogFile, line + Environment.NewLine, cts.Token);
            Console.WriteLine(line);
            var delayMs = NextDelayMs();
            try { await Task.Delay(TimeSpan.FromMilliseconds(delayMs), cts.Token); }
            catch (TaskCanceledException) { break; }
        }

        Console.WriteLine("Stopping.");
    }

    static string GenerateLogLine()
    {
        var now = DateTime.Now;
        var ts = now.ToString("MMM dd HH:mm:ss");
        var host = Environment.MachineName;
        var prog = "sawmill";
        var pid = Rand.Next(1000, 99999);
        var levels = new[] { "INFO", "WARN", "ERROR", "DEBUG", "NOTICE" };
        var messages = new[]
        {
            "Started processing queue",
            "Finished processing item",
            "Connection to database established",
            "Connection to database lost",
            "User login succeeded",
            "User login failed",
            "File rotated successfully",
            "Unexpected condition encountered"
        };
        var level = levels[Rand.Next(levels.Length)];
        var msg = messages[Rand.Next(messages.Length)];
        return $"{ts} {host} {prog}[{pid}]: {level}: {msg}";
    }

    static int NextDelayMs()
    {
        // exponential with mean 2000ms, clipped 200..10000
        var u = Rand.NextDouble();
        var delay = -Math.Log(1 - u) * 2000.0;
        if (delay < 200) delay = 200;
        if (delay > 10000) delay = 10000;
        return (int)delay;
    }
}
