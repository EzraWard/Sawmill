using System.Linq;
using FluentAssertions;
using TailBlazer.Views.Formatting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TailBlazer.Fixtures;

[TestClass]
public class IconProviderFixture
{
    [TestMethod]
    public void IconProviderShouldHaveIcons()
    {
        using (var provider = new IconProvider(new DefaultIconSelector()))
        {
            var result = provider.Icons;

            result.Items.Any().Should().BeTrue();
        }
    }
}

