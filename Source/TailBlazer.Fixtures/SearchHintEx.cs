using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TailBlazer.Views.Searching;

namespace TailBlazer.Fixtures;

[TestClass]
public class SearchHintEx
{
    [TestMethod]
    public void ShouldAskForTextWhenTextIsEmpty()
    {
        var searchRequest = new SearchRequest("", false);

        var result = searchRequest.BuildMessage();

        result.IsValid.Should().BeTrue();
        result.Message.Should().Be("Type to search using plain text");
    }

    [TestMethod]
    public void ShouldAskForTextWhenRegexIsEmpty()
    {
        var searchRequest = new SearchRequest("", true);

        var result = searchRequest.BuildMessage();

        result.IsValid.Should().BeTrue();
        result.Message.Should().Be("Type to search using regex");
    }

    [TestMethod]
    public void ShouldBeValidWhenSearchingPlainText()
    {
        var searchRequest = new SearchRequest("[inf", false);

        var result = searchRequest.BuildMessage();

        result.IsValid.Should().BeTrue();
        result.Message.Should().Be("Hit enter to search using plain text");
    }

    [TestMethod]
    public void ShouldBeValidWhenSearchingAValidRegex()
    {
        var searchRequest = new SearchRequest("[inf]", true);

        var result = searchRequest.BuildMessage();

        result.IsValid.Should().BeTrue();
        result.Message.Should().Be("Hit enter to search using regex");
    }

    [TestMethod]
    public void ShouldBeInvalidWhenSearchingTooShortRegEx()
    {
        var searchRequest = new SearchRequest(".", true);

        var result = searchRequest.BuildMessage();

        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Regex must be at least 2 characters");
    }

    [TestMethod]
    public void ShouldBeValidWhenSearchingPlainTextExclusion()
    {
        var searchRequest = new SearchRequest("-[inf", false);

        var result = searchRequest.BuildMessage();

        result.IsValid.Should().BeTrue();
        result.Message.Should().Be("Hit enter to search using plain text");
    }

    [TestMethod]
    public void ShouldBeInvalidWhenPlainTextExclusionTextIsTooShort()
    {
        var searchRequest = new SearchRequest("-f", false);

        var result = searchRequest.BuildMessage();

        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Text must be at least 3 characters");
    }

    [TestMethod]
    public void ShouldBeInvalidWhenSearchingIrregularRegEx()
    {
        var searchRequest = new SearchRequest("[inf", true);

        var result = searchRequest.BuildMessage();

        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Invalid regular expression");
    }

    [TestMethod]
    public void ShouldBeInvalidWhenPlainTextContainsIllegalCharacter()
    {
        var searchRequest = new SearchRequest(@"[i\nf", false);

        var result = searchRequest.BuildMessage();

        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Text contains illegal characters");
    }

    [TestMethod]
    public void ShouldBeInvalidWhenPlainTextContainsOnlyWhiteSpaces()
    {
        var searchRequest = new SearchRequest("-    \t", false);

        var result = searchRequest.BuildMessage();

        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Text contains illegal characters");
    }
}

