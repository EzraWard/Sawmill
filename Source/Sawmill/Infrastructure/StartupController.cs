using System.Reflection;
using Sawmill.Domain.FileHandling.Recent;
using Sawmill.Domain.FileHandling.TextAssociations;
using Sawmill.Domain.Formatting;
using Sawmill.Domain.Infrastructure;
using Sawmill.Domain.Settings;
using Sawmill.Domain.StateHandling;
using Sawmill.Infrastructure.AppState;
using Sawmill.Views.Formatting;
using Sawmill.Views.Recent;
using Sawmill.Views.Tail;
using Sawmill.Views.WindowManagement;

namespace Sawmill.Infrastructure;

public class StartupController
{
    public StartupController(IObjectProvider objectProvider, ILogger logger,
        IApplicationStatePublisher applicationStatePublisher)
    {
        applicationStatePublisher.Publish(ApplicationState.Startup);

        logger.Info($"Starting Sawmill version v{Assembly.GetEntryAssembly().GetName().Version}");
        logger.Info($"at {DateTime.UtcNow}");


        //run start up jobs
        objectProvider.Get<FileHeaderNamingJob>();
        objectProvider.Get<UhandledExceptionHandler>();

        var settingsRegister = objectProvider.Get<ISettingsRegister>();
        settingsRegister.Register(new GeneralOptionsConverter(), "GeneralOptions");
        settingsRegister.Register(new RecentFilesToStateConverter(), "RecentFiles");
        settingsRegister.Register(new StateBucketConverter(), "BucketOfState");
        settingsRegister.Register(new RecentSearchToStateConverter(), "RecentSearch");
        settingsRegister.Register(new TextAssociationToStateConverter(), "TextAssociation");
        settingsRegister.Register(new SearchMetadataToStateConverter(), "GlobalSearch");

        //TODO: Need type scanner then this code is not required
        var viewFactoryRegister = objectProvider.Get<IViewFactoryRegister>();
        viewFactoryRegister.Register<TailViewModelFactory>();

        objectProvider.Get<SystemSetterJob>();

        logger.Info("Starting complete");
    }
}