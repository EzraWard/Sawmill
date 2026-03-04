#region Usings

using FluentAssertions;
using TailBlazer.Domain.Infrastructure;
using TailBlazer.Domain.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TailBlazer.Fixtures;

[TestClass]
public class SettingsStoreFixture
{
    [TestMethod]
    public void WriteState()
    {
        var state = new State(1, "Test");

        var store = new FileSettingsStore(new NullLogger());
        store.Save("testfile", state);

        var restored = store.Load("testfile");
        restored.Should().Be(state);
    }

    [TestMethod]
    public void WriteComplexState()
    {
        var state = new State(1, "<<something weird<> which breaks xml {}");

        var store = new FileSettingsStore(new NullLogger());
        store.Save("wierdfile", state);

        var restored = store.Load("wierdfile");
        restored.Should().Be(state);
    }
}

