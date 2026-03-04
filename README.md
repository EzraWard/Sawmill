# Sawmill

A modern, fast log file viewer for Windows — built with WPF, .NET 10, and Windows 11 Fluent design.

Sawmill is a fork of [TailBlazer](https://github.com/RolandPheasant/TailBlazer) by Roland Pheasant, modernized with a contemporary UI, updated dependencies, and new features.

## Features

- **Real-time log tailing** with virtual scrolling for files of any size
- **Drag and drop** files to start tailing instantly
- **Tabbed interface** — monitor multiple files side by side
- **Search and highlight** — filter lines by text or regex
- **Multi-color highlighting** with customizable icons per search term
- **Global and local search** with toggleable result views
- **Inline search results** — view matches in their original file position
- **New line highlighting** — spot new entries at a glance
- **Keyboard shortcuts** — Ctrl+T for new tab, Ctrl+O for open file
- **Large file support** — tested with files up to 47 GB
- **Settings persistence** — remembers your preferences across sessions

## Getting Started

### Requirements

- Windows 10/11
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later

### Build from Source

```
git clone https://github.com/EzraWard/TailBlazer.git
cd TailBlazer
dotnet build .\Source\TailBlazer\TailBlazer.csproj
```

### Run

```
dotnet run --project .\Source\TailBlazer\TailBlazer.csproj
```

Or build and run `Source\TailBlazer\bin\Debug\net10.0-windows10.0.26100.0\TailBlazer.exe` directly.

### Run Tests

```
dotnet test .\Source\TailBlazer.Fixtures\TailBlazer.Fixtures.csproj
```

## Architecture

| Project | Purpose |
|---|---|
| `TailBlazer` | WPF app — UI, view models, window management |
| `TailBlazer.Domain` | Core logic — file monitoring, search, settings, reactive pipelines |
| `TailBlazer.Fixtures` | MSTest test suite |
| `TailBlazer.TestTextFile` | Console app that writes log lines for manual testing |

The app is built on reactive streams ([Rx](https://github.com/dotnet/reactive) + [DynamicData](https://github.com/reactivemarbles/DynamicData)) — file watching, scrolling, searching, and rendering are all wired reactively.

## Acknowledgements

Sawmill would not exist without [TailBlazer](https://github.com/RolandPheasant/TailBlazer) by [Roland Pheasant](https://github.com/RolandPheasant). His work on reactive collections, file tailing, and the original application architecture is the foundation this project is built on.

Open source libraries that make this possible:

- [DynamicData](https://github.com/reactivemarbles/DynamicData) — reactive collections by Roland Pheasant
- [MaterialDesignInXamlToolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit) — Material Design for WPF
- [Dragablz](https://github.com/ButchersBoy/Dragablz) — tab control with drag and dock
- [StructureMap](https://github.com/structuremap/structuremap) — dependency injection

## License

See [LICENSE](LICENSE) for details.
