using StructureMap;
using TailBlazer.Domain.FileHandling;
using TailBlazer.Domain.FileHandling.Search;
using TailBlazer.Domain.Formatting;
using TailBlazer.Domain.Infrastructure;
using TailBlazer.Domain.Settings;
using TailBlazer.Infrastructure.AppState;
using TailBlazer.Infrastructure.KeyboardNavigation;
using TailBlazer.Views.Options;
using TailBlazer.Views.Tail;
using ILogger = TailBlazer.Domain.Infrastructure.ILogger;

namespace TailBlazer.Infrastructure;

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