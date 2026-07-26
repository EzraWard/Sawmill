# Sawmill Copilot Instructions

## Build, test, and lint

Run commands from repository root.

- Build the main app:
  - `dotnet build .\Source\Sawmill\Sawmill.csproj -v minimal`
- Run all tests:
  - `dotnet test .\Source\Sawmill.Fixtures\Sawmill.Fixtures.csproj -v minimal`
- Run a single test:
  - `dotnet test .\Source\Sawmill.Fixtures\Sawmill.Fixtures.csproj --filter "FullyQualifiedName~Sawmill.Fixtures.FileDropFixture.FileDropContainerShouldReturnFileNames" -v minimal`
- Optional log generator used for manual tailing scenarios:
  - `dotnet run --project .\Source\Sawmill.TestTextFile\Sawmill.TestTextFile.csproj`

Notes:
- Building `.\Source\Sawmill.slnx` includes `SawmillPackage\SawmillPackage.wapproj`, which requires Desktop Bridge build targets (`Microsoft.DesktopBridge.props`).
- If Desktop Bridge is not installed, build/test project-level targets instead of the full solution.
- There is no dedicated lint command in this repo; compiler + MSTest analyzer warnings are surfaced during `dotnet build`/`dotnet test`.

## High-level architecture

- `Source\Sawmill` is the WPF app/UI layer (main window, view models, dialogs, formatting UI, layout/window management).
- `Source\Sawmill.Domain` contains core logic for file monitoring, search/formatting pipelines, ratings, settings/state, and reactive helpers.
- `Source\Sawmill.Fixtures` is the MSTest test suite for domain and UI-adjacent behavior.
- `Source\Sawmill.TestTextFile` is a console app that continuously writes log lines for manual testing.

Startup/composition flow:
1. `App.xaml.cs` creates a StructureMap container and resolves startup components.
2. `AppRegistry.cs` is the composition root: logging config + explicit registrations + assembly scanning.
3. `Infrastructure\AppConventions.cs` binds interfaces to concrete types by naming convention.

Runtime behavior is strongly reactive (per project README mission): file watching, scrolling, searching, and rendering are wired with Rx/DynamicData streams (see `Views\Tail\TailViewModel.cs` and `Domain\FileHandling\FileWatcher.cs`).

## Key conventions in this codebase

- DI convention: in `AppConventions`, a concrete type `X` is auto-registered for interface `IX`; many services are singleton-scoped by default.
- Any type name ending in `Job` is registered as a singleton (also enforced by conventions).
- View models/services typically own a `CompositeDisposable` and must dispose stream subscriptions/resources explicitly.
- Settings persistence uses `ISettingsStore` (`FileSettingsStore`) with XML `.setting` files under `%LocalAppData%\Sawmill`.
- View state persistence follows `IPersistentView` (`CaptureState`/`Restore`) and is coordinated in view models like `TailViewModel`.
