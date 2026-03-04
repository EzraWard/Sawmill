using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using TailBlazer.Domain.FileHandling;
using TailBlazer.Domain.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TailBlazer.Fixtures;

[TestClass]
public class FileSearchFixture
{
    [TestMethod]
    public void EmptyFile()
    {
        var pulse = new Subject<Unit>();
        var scheduler = new TestScheduler();
        using (var file = new TestFile())
        {
            FileSearchResult fileSearchResult = null;

            using (file.Info
                       .Search(str => str.Contains("9"), scheduler)
                       .Subscribe(x => fileSearchResult = x))
            {
                scheduler.AdvanceByMilliSeconds(250);
                pulse.Once();
                fileSearchResult.Segments.Should().Be(1);
                fileSearchResult.SegmentsCompleted.Should().Be(0);
                fileSearchResult.IsSearching.Should().Be(false);
                fileSearchResult.Count.Should().Be(0);
            }
        }
    }

    [TestMethod]
    public void NotExistingFile()
    {
        var pulse = new Subject<Unit>();

        using (var file = new TestFile())
        {
            file.Delete();

            FileSearchResult fileSearchResult = null;

            using (file.Info.WatchFile(pulse)
                       .Search(str => str.Contains("9"))
                       .Subscribe(x => fileSearchResult = x))
            {
                fileSearchResult.Should().BeNull();
            }
        }
    }

    //[TestMethod]
    //Cannot recreate the file as something is hanging on to it.
    public void CreateFileLater()
    {
        var pulse = new Subject<Unit>();

        using (var file = new TestFile())
        {
            file.Delete();

            FileSearchResult fileSearchResult = null;

            using (file.Info.WatchFile(pulse)
                       .Search(str => str.Contains("9"))
                       .Subscribe(x => fileSearchResult = x))
            {
                fileSearchResult.Should().Be(FileSearchResult.None);
                file.Create();
                pulse.Once();
                fileSearchResult.Should().NotBe(FileSearchResult.None);
            }
        }
    }

    [TestMethod]
    public void SearchOnDataWhenFileIsPopulated()
    {
        var pulse = new Subject<Unit>();

        using (var file = new TestFile())
        {
            FileSearchResult fileSearchResult = null;


            using (file.Info.WatchFile(pulse)
                       .Search(str => str.Contains("9"))
                       .Subscribe(x => fileSearchResult = x))
            {
                file.Append(Enumerable.Range(1, 100).Select(i => i.ToString()).ToArray());
                pulse.Once();
                pulse.Once();
                fileSearchResult.Matches.Length.Should().NotBe(0);
            }
        }
    }
    [TestMethod]
    public void InitiallyEmptyThenLinesAdded()
    {
        var scheduler = new TestScheduler();

        using (var file = new TestFile())
        {
            FileSearchResult fileSearchResult = null;
                
            using (file.Info.WatchFile(scheduler:scheduler)
                       .Search(str => str.Contains("9"))
                       .Subscribe(x => fileSearchResult = x))
            {
                scheduler.AdvanceBy(1);
                file.Append(Enumerable.Range(1, 100).Select(i => i.ToString()).ToArray());
                scheduler.AdvanceByMilliSeconds(250);
                fileSearchResult.Matches.Length.Should().Be(19);
            }
        }
    }

    [TestMethod]
    public void WillContinuallyTail()
    {
        var scheduler = new TestScheduler();
        var pulse = new Subject<Unit>();
            
        using (var file = new TestFile())
        {
            FileSearchResult fileSearchResult = null;
            file.Append(Enumerable.Range(1, 100).Select(i => i.ToString()).ToArray());

            using (file.Info.WatchFile(pulse)
                       .Search(str => str.Contains("9"), scheduler)
                       .Subscribe(x => fileSearchResult = x))
            {
                pulse.Once();
                fileSearchResult.Matches.Length.Should().Be(19);

                file.Append(new [] {"9","20"});
                pulse.Once();
                fileSearchResult.Matches.Length.Should().Be(20);

                file.Append(new[] { "9999" });
                pulse.Once();
                fileSearchResult.Matches.Length.Should().Be(21);
            }
        }
    }

    public void CanReadLinesBack()
    {
        var scheduler = new TestScheduler();
        var pulse = new Subject<Unit>();

        using (var file = new TestFile())
        {
            FileSearchResult fileSearchResult = null;
            file.Append(Enumerable.Range(1, 100).Select(i => i.ToString()).ToArray());

            using (file.Info.WatchFile(pulse)
                       .Search(str => str.Contains("9"), scheduler)
                       .Subscribe(x => fileSearchResult = x))
            {
                pulse.Once();
                fileSearchResult.Matches.Length.Should().Be(19);

                file.Append(new[] { "9", "20" });
                pulse.Once();
                fileSearchResult.Matches.Length.Should().Be(20);

                file.Append(new[] { "9999" });
                pulse.Once();
                fileSearchResult.Matches.Length.Should().Be(21);
            }
        }
    }

    [TestMethod]
    public void CanReadLinesBackInLargeFile()
    {
        var scheduler = new TestScheduler();
        var pulse = new Subject<Unit>();

        using (var file = new TestFile())
        {
            IEnumerable<Line> lines = null;
            file.Append(Enumerable.Range(1, 100000).Select(i => i.ToString()).ToArray());

            using (file.Info.WatchFile(pulse)
                       .Search(str => str.Contains("9"), scheduler)
                       .Select(result => file.Info.ReadLinesByPosition(result.Matches).ToArray())
                       .Subscribe(x => lines=x))
            {
                pulse.Once();
                // fileSearchResult.Matches.Length.Should().Be(19);

                file.Append(new[] { "9", "20" });
                pulse.Once();
                //  fileSearchResult.Matches.Length.Should().Be(20);

                file.Append(new[] { "9999" });
                pulse.Once();
                //   fileSearchResult.Matches.Length.Should().Be(21);
            }
        }
    }
}

