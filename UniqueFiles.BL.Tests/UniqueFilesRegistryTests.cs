using FluentAssertions;
using System;
using System.IO;
using UniqueFiles.BL.Registries;
using Xunit;

namespace UniqueFiles.BL.Tests
{
    public class UniqueFilesRegistryTests
    {
        private readonly UniqueFilesRegistry _registry;

        public UniqueFilesRegistryTests()
        {
            _registry = new UniqueFilesRegistry();
        }

        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("a")]
        [InlineData(@"d:\tmp")]
        [Theory]
        public void Contains_EmptyRegistry_False(string fullPath)
        {
            var fileInfo = string.IsNullOrWhiteSpace(fullPath) ? null : new FileInfo(fullPath);
            var contains = _registry.Contains(fileInfo);
            contains.Should().BeFalse();
        }

        [Fact]
        public void Overall_Add_Contains_True()
        {
            var fileName = "a";
            _registry.Add(new FileInfo(fileName));

            var contains = _registry.Contains(new FileInfo(fileName));

            contains.Should().BeTrue();
        }

        [Fact]
        public void Add_NullFileInfo_ThrowsException()
        {
            Action addition = () => _registry.Add(null);
            addition.Should().Throw<InvalidOperationException>()
                .Where(exc => exc.Message.Contains("empty file name"));
        }

        [Fact]
        public void Add_AlreadyInRegistry_ThrowsException()
        {
            var fileInfo = new FileInfo("a");
            _registry.Add(fileInfo);
            Action addition = () => _registry.Add(fileInfo);
            addition.Should().Throw<InvalidOperationException>()
                .Where(exc => exc.Message.Contains("already registered"));
        }
    }
}
