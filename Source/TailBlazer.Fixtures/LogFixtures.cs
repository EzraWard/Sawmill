using System.Collections.Generic;
using FluentAssertions;
using TailBlazer.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TailBlazer.Fixtures;

[TestClass]
public class LogFixtures
{

    [TestMethod]
    public void LogNameDisplaysReadablyGenerics()
    {
        var subject = new List<int>();
        var logger = new Log4NetLogger(subject.GetType());
        logger.Name.Should().Be("List<Int32>");

    }

    [TestMethod]
    public void LogNameDisplayTakesTypeNameOnly()
    {
        var logger = new Log4NetLogger(typeof(int));
        logger.Name.Should().Be("Int32");

    }
}

