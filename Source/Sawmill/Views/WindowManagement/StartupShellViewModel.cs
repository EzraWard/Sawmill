using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Sawmill.Infrastructure;

namespace Sawmill.Views.WindowManagement;

/// <summary>
/// Minimal, interactive model used only for the first frame. Expensive session
/// services are composed in parallel and replace this model when ready.
/// </summary>
public sealed class StartupShellViewModel
{
    private readonly Func<string> _pickFile;
    private readonly List<string> _pendingFiles = [];
    private int _pendingNewTabs;
    private bool _pendingSettings;

    public StartupShellViewModel(IEnumerable<string> startupFiles = null, Func<string> pickFile = null)
    {
        _pickFile = pickFile ?? PickFile;
        if (startupFiles != null)
            _pendingFiles.AddRange(startupFiles.Where(path => !string.IsNullOrWhiteSpace(path)));

        OpenFileCommand = new Command(OpenFile);
        OpenSettingsCommand = new Command(() => _pendingSettings = true);
        NewTabCommand = new Command(() => _pendingNewTabs++);
        ExitCommmand = new Command(() => Application.Current.Shutdown());
    }

    public ICommand OpenFileCommand { get; }
    public ICommand OpenSettingsCommand { get; }
    public ICommand NewTabCommand { get; }
    public ICommand ExitCommmand { get; }

    public StartupActions DrainActions()
    {
        var actions = new StartupActions(_pendingFiles.ToArray(), _pendingNewTabs, _pendingSettings);
        _pendingFiles.Clear();
        _pendingNewTabs = 0;
        _pendingSettings = false;
        return actions;
    }

    public void ReplayActions(WindowViewModel target)
    {
        ArgumentNullException.ThrowIfNull(target);

        var actions = DrainActions();
        target.OpenFiles(actions.Files);

        for (var i = 0; i < actions.NewTabCount; i++)
            target.NewTabCommand.Execute(null);

        if (actions.OpenSettings)
            target.OpenSettingsCommand.Execute(null);
    }

    private void OpenFile()
    {
        var file = _pickFile();
        if (!string.IsNullOrWhiteSpace(file))
            _pendingFiles.Add(file);
    }

    private static string PickFile()
    {
        var dialog = new OpenFileDialog { Filter = "All files (*.*)|*.*" };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

}

public sealed record StartupActions(IReadOnlyList<string> Files, int NewTabCount, bool OpenSettings);
