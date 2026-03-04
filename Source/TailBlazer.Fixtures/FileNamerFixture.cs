using System.Linq;
using TailBlazer.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.IO.Path;

namespace TailBlazer.Fixtures;

[TestClass]
public class FileNamerFixture
{
    [TestMethod]
    public void ReturnsCorrectDistinctPath()
    {
        var paths = new[]
        {
            Combine("logger.log"),
            Combine("Debug", "logger.log"),
            Combine("bin", "Debug", "logger.log"),
            Combine("C:\\", "App", "bin", "Debug", "logger.log"),
            Combine("D:\\", "App", "bin", "Debug", "logger.log"),
            Combine("C:\\", "App", "bin", "Release", "logger.log"),
            Combine("C:\\", "App", "obj", "Release", "logger.log")
        };

        var expected = new[]
        {
            Combine("logger.log"),
            Combine("Debug", "logger.log"),
            Combine("bin", "..", "logger.log"),
            Combine("C:\\", "..", "logger.log"),
            Combine("D:\\", "..", "logger.log"),
            Combine("bin", "..", "logger.log"),
            Combine("obj", "..", "logger.log")
        };

        var trie = new FileNamer(paths);

        var result = paths.Select(path => trie.GetName(path)).ToArray();

        CollectionAssert.AreEqual(expected, result);
    }
}

