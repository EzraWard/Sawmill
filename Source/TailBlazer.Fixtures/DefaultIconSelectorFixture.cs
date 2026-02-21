using FluentAssertions;
using TailBlazer.Views.Formatting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TailBlazer.Fixtures;

public class DefaultIconSelectorFixture
{
    [DataTestMethod,
     DataRow("DEBUG", false),
     DataRow("DEBUG", true),
     DataRow(null, true),
     DataRow(null, false)
    ]
    public void GetIconForShouldWork(string text, bool useRegex)
    {
        var selector = new DefaultIconSelector();

        var result = selector.GetIconFor(text, useRegex);

        result.Should().NotBeNullOrEmpty();
    }

    [DataTestMethod,
     DataRow("DEBUG", true, "INFO"),
     DataRow("DEBUG", false, "INFO"),
     DataRow("DEBUG", true, "xxxxxxx"),
     DataRow("DEBUG", false, "xxxxxxx"),
     DataRow("Bug", false, "xxxxxxx")
    ]
    public void GetIconOrDefaultShouldWork(string text, bool useRegex, string iconKind)
    {
        var selector = new DefaultIconSelector();

        var result = selector.GetIconOrDefault(text, useRegex, iconKind);

        result.Should().NotBeNullOrEmpty();
    }
}

