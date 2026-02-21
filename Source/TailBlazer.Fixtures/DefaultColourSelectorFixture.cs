using System.Linq;
using FluentAssertions;
using TailBlazer.Views.Formatting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TailBlazer.Fixtures;

[TestClass]
public class DefaultColourSelectorFixture
{
    [TestMethod]
    public void DefaultColourSelectorLookupShouldWork()
    {
        var provider = new ColourProvider();
        var selector = new DefaultColourSelector(provider);
        var key = provider.Hues.First().Key;

        var result = selector.Lookup(key);

        result.Key.Should().Be(key);
    }

    [TestMethod]
    public void DefaultColourSelectorSelectShouldWork()
    {
        var provider = new ColourProvider();
        var selector = new DefaultColourSelector(provider);

        var result = selector.Select("DEBUG");

        result.Should().NotBeNull();
    }
}

