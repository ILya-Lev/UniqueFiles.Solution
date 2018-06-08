using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace UniqueFiles.BL.Tests
{
    public class DuplicateCleanerTests
    {
        private readonly HashSet<string> _uniqueNames = new HashSet<string>();
        private readonly List<string> _backedUpNames = new List<string>();
        private Func<string, IEnumerable<string>> _fileNamesGenerator;
        private Func<string, IEnumerable<string>> _folderNamesGenerator;
        private readonly DuplicateCleaner _cleaner;

        public DuplicateCleanerTests()
        {
            var uniqueFileRegistry = new Mock<IUniqueFileRegistry>(MockBehavior.Strict);
            uniqueFileRegistry.Setup(r => r.Contains(It.IsAny<string>()))
                              .Returns<string>(name => _uniqueNames.Contains(name));
            uniqueFileRegistry.Setup(r => r.Add(It.IsAny<string>()))
                              .Callback<string>(name => _uniqueNames.Add(name));


            var backedUpFileRegistry = new Mock<IBackedUpFileRegistry>(MockBehavior.Strict);
            backedUpFileRegistry.Setup(r => r.Add(It.IsAny<string>()))
                                .Callback<string>(name => _backedUpNames.Add(name));

            var fileNamesProvider = new Mock<IFileSystemEntityProvider>(MockBehavior.Strict);
            fileNamesProvider.Setup(p => p.GetNames(It.IsAny<string>()))
                             .Returns<string>(path => _fileNamesGenerator(path));

            var folderNamesProvider = new Mock<IFileSystemEntityProvider>(MockBehavior.Strict);
            folderNamesProvider.Setup(p => p.GetNames(It.IsAny<string>()))
                               .Returns<string>(path => _folderNamesGenerator(path));

            _cleaner = new DuplicateCleaner(uniqueFileRegistry.Object, backedUpFileRegistry.Object, fileNamesProvider.Object, folderNamesProvider.Object);
        }

        [Fact]
        public void Clean_EmptyRootFolder_EmptyFileRegistry()
        {
            _folderNamesGenerator = path => Enumerable.Empty<string>();
            _fileNamesGenerator = path => Enumerable.Empty<string>();

            _cleaner.Clean("any path");

            _uniqueNames.Should().BeEmpty();
            _backedUpNames.Should().BeEmpty();
        }

        [Fact]
        public void Clean_NoDuplicatesFlatStructure_EmptyBackUpRegistry()
        {
            _folderNamesGenerator = path => Enumerable.Empty<string>();
            var fileNames = new[] { "a", "b", "c" };
            _fileNamesGenerator = path => fileNames;

            _cleaner.Clean("any path");

            _uniqueNames.Should().BeEquivalentTo(fileNames);
            _backedUpNames.Should().BeEmpty();
        }

        [Fact]
        public void Clean_3DuplicatesFlatStructure_3NamesInBackUpRegistry()
        {
            _folderNamesGenerator = path => Enumerable.Empty<string>();
            var fileNames = new[] { "a", "b", "a", "c", "b", "a" };
            _fileNamesGenerator = path => fileNames;

            _cleaner.Clean("any path");

            _uniqueNames.Should().BeEquivalentTo(new[] { "a", "b", "c" });
            _backedUpNames.Should().BeEquivalentTo(new[] { "a", "b", "a" });
        }

        [Fact]
        public void Clean_NoDuplicatesDeepStructure_EmptyBackUpRegistry()
        {
            _folderNamesGenerator = path =>
            {
                switch (path)
                {
                    case "root": return new[] { "f1", "f2", "f3" };
                    case "f1": return new[] { "f11", "f12", "f13" };
                    case "f2": return new[] { "f21", "f22" };
                    case "f3": return new[] { "f31" };
                    case "f31": return new[] { "f311" };
                    default: return new string[0];
                }
            };

            _fileNamesGenerator = path =>
            {
                switch (path)
                {
                    case "root": return new[] { "a", "b", "c" };
                    case "f31": return new[] { "d", "e" };
                    case "f311": return new[] { "f" };
                    default: return new string[0];
                }
            };

            _cleaner.Clean("root");

            _uniqueNames.Should().BeEquivalentTo(new[] { "a", "b", "c", "d", "e", "f" });
            _backedUpNames.Should().BeEmpty();
        }

        [Fact]
        public void Clean_3DuplicatesDeepStructure_3NamesInBackUpRegistry()
        {
            _folderNamesGenerator = path =>
            {
                switch (path)
                {
                    case "root": return new[] { "f1", "f2", "f3" };
                    case "f1": return new[] { "f11", "f12", "f13" };
                    case "f2": return new[] { "f21", "f22" };
                    case "f3": return new[] { "f31" };
                    case "f31": return new[] { "f311" };
                    default: return new string[0];
                }
            };

            _fileNamesGenerator = path =>
            {
                switch (path)
                {
                    case "root": return new[] { "a", "b", "c" };
                    case "f22": return new[] { "a", "d", "f" };
                    case "f31": return new[] { "d", "e" };
                    case "f311": return new[] { "c" };
                    default: return new string[0];
                }
            };

            _cleaner.Clean("root");

            _uniqueNames.Should().BeEquivalentTo(new[] { "a", "b", "c", "d", "f", "e" });
            _backedUpNames.Should().BeEquivalentTo(new[] { "a", "d", "c" });
        }
    }
}
