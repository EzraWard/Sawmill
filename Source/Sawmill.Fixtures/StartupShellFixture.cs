using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sawmill.Views.WindowManagement;

namespace Sawmill.Fixtures;

[TestClass]
public class StartupShellFixture
{
    [TestMethod]
    public void StartupShellQueuesActionsUntilHydration()
    {
        var model = new StartupShellViewModel(
            [@"C:\logs\from-command-line.log"],
            () => @"C:\logs\picked.log");

        model.OpenFileCommand.Execute(null);
        model.NewTabCommand.Execute(null);
        model.NewTabCommand.Execute(null);
        model.OpenSettingsCommand.Execute(null);

        var actions = model.DrainActions();

        actions.Files.Should().Equal(
            @"C:\logs\from-command-line.log",
            @"C:\logs\picked.log");
        actions.NewTabCount.Should().Be(2);
        actions.OpenSettings.Should().BeTrue();
    }

    [TestMethod]
    public void DrainingStartupActionsIsDestructive()
    {
        var model = new StartupShellViewModel([@"C:\logs\once.log"]);

        model.DrainActions();
        var secondDrain = model.DrainActions();

        secondDrain.Files.Should().BeEmpty();
        secondDrain.NewTabCount.Should().Be(0);
        secondDrain.OpenSettings.Should().BeFalse();
    }
}
