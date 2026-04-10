using StructureMap;
using Sawmill.Domain.FileHandling;
using Sawmill.Domain.FileHandling.Search;
using Sawmill.Domain.Formatting;
using Sawmill.Domain.Infrastructure;
using Sawmill.Domain.Settings;
using Sawmill.Infrastructure.AppState;
using Sawmill.Infrastructure.KeyboardNavigation;
using Sawmill.Views.Options;
using Sawmill.Views.Tail;
using ILogger = Sawmill.Domain.Infrastructure.ILogger;

namespace Sawmill.Infrastructure;

internal class AppRegistry : Registry
{
    public AppRegistry()
    {
        For<ILogger>().Use<SimpleFileLogger>().Ctor<Type>("type").Is(x => x.ParentType).AlwaysUnique();

        For<ISelectionMonitor>().Use<SelectionMonitor>();
        For<ISearchInfoCollection>().Use<SearchInfoCollection>();
        For<ISearchMetadataCollection>().Use<SearchMetadataCollection>().Transient();
        For<ICombinedSearchMetadataCollection>().Use<CombinedSearchMetadataCollection>().Transient();
             

        For<ITextFormatter>().Use<TextFormatter>().Transient();
        For<ILineMatches>().Use<LineMatches>();
        For<ISettingsStore>().Use<FileSettingsStore>().Singleton();
        For<IFileWatcher>().Use<FileWatcher>();


        For<GeneralOptionsViewModel>().Singleton();
        For<UhandledExceptionHandler>().Singleton();
        For<ObjectProvider>().Singleton();
        Forward<ObjectProvider, IObjectProvider>();
        Forward<ObjectProvider, IObjectRegister>();


        For<ViewFactoryService>().Singleton();
        Forward<ViewFactoryService, IViewFactoryRegister>();
        Forward<ViewFactoryService, IViewFactoryProvider>();

        For<ApplicationStateBroker>().Singleton();
        Forward<ApplicationStateBroker, IApplicationStateNotifier>();
        Forward<ApplicationStateBroker, IApplicationStatePublisher>();

            
        For<TailViewModelFactory>().Singleton();

        For<IKeyboardNavigationHandler>().Use<KeyboardNavigationHandler>();

        Scan(scanner =>
        {
            scanner.ExcludeType<ILogger>();

            //to do, need a auto-exclude these from AppConventions
            scanner.ExcludeType<SelectionMonitor>();
            scanner.ExcludeType<SearchInfoCollection>();
            scanner.ExcludeType<SearchMetadataCollection>();
            scanner.ExcludeType<CombinedSearchMetadataCollection>();
            scanner.ExcludeType<TextFormatter>();
            scanner.ExcludeType<LineMatches>();
            scanner.ExcludeType<ViewFactoryService>();

                

            scanner.ExcludeType<FileWatcher>();
            scanner.LookForRegistries();
            scanner.Convention<AppConventions>();

            scanner.AssemblyContainingType<ILogFactory>();
            scanner.AssemblyContainingType<AppRegistry>();
        });
    }

}