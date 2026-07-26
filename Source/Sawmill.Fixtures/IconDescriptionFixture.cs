using FluentAssertions;
using MaterialDesignThemes.Wpf;
using Sawmill.Views.Formatting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sawmill.Fixtures;

[TestClass]
public class IconDescriptionFixture
{
    [TestMethod]
    public void IconDescriptionShoudBeComparable()
    {
        var id1 = new IconDescription(new PackIconKind(), "test");
        var id2 = new IconDescription(new PackIconKind(), "test");
 
        var result = id1 == id2;

        result.Should().BeTrue();
    }

    [TestMethod]
    public void IconDescriptionShoudBeComparableInEqual()
    {
        var id1 = new IconDescription(new PackIconKind(), "test1");
        var id2 = new IconDescription(new PackIconKind(), "test2");

        var result = id1 == id2;

        result.Should().BeFalse();
    }

}

