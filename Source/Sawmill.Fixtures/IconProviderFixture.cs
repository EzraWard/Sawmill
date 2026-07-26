using System.Linq;
using FluentAssertions;
using Sawmill.Views.Formatting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sawmill.Fixtures;

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

