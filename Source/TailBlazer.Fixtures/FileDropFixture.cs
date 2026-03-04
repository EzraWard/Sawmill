using System.IO;
using System.Reactive.Subjects;
using System.Windows;
using FluentAssertions;
using TailBlazer.Views.FileDrop;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TailBlazer.Fixtures;

[TestClass]
public class FileDropFixture
{
    [TestMethod]
    public void FileDropContainerShouldReturnFileNames()
    {
        var input = new[] { @"c:\temp\file1.txt", @"c:\temp\file2.txt" };
        var expectedResult = new[] { @"file1.txt", @"file2.txt" };
        var container = new FileDropContainer(input);

        container.Files.Should().BeEquivalentTo(expectedResult);
    }

    [TestMethod]
    public void FileDropContainerShouldHandleNulls()
    {
        var input = new[] { @"c:\temp\file1.txt", null, @"c:\temp\file2.txt" };
        var expectedResult = new[] { @"file1.txt", @"file2.txt" };
        var container = new FileDropContainer(input);

        container.Files.Should().BeEquivalentTo(expectedResult);
    }

    [TestMethod]
    public void FileDropContainerShouldHandleEmpty()
    {
        var input = new string[0];

        var container = new FileDropContainer(input);

        container.Files.Should().BeEmpty();
    }

    [TestMethod]
    public void FileDropContainerShouldHandleNull()
    {
        var container = new FileDropContainer(null);

        container.Files.Should().BeEmpty();
    }

    [TestMethod]
    public void FileDropMonitorShouldHandleNull()
    {
        var monitor = new FileDropMonitor();

        monitor.Receive(null);

    }

    [TestMethod]
    public void FileDropMonitorShouldOnlyUseUiElement()
    {
        var monitor = new FileDropMonitor();

        monitor.Receive(new DependencyObject());

        monitor.Dropped.Should().BeOfType<Subject<FileInfo>>();

        ((Subject<FileInfo>)monitor.Dropped).HasObservers.Should().Be(false);
    }

    [TestMethod]
    public void FileDropMonitorShouldOnlyObserveWithUiElement()
    {
        var monitor = new FileDropMonitor();

        monitor.Receive(new UIElement());

        monitor.Dropped.Should().BeOfType<Subject<FileInfo>>();

        ((Subject<FileInfo>)monitor.Dropped).HasObservers.Should().Be(false);
    }
}

