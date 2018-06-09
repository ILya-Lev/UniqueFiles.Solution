using FluentAssertions;
using System.IO;
using Xunit;

namespace UniqueFiles.BL.Tests
{
    public class DuplicateCleanerIntegrationTests
    {
        private DuplicateCleaner MakeDuplicateCleaner(string rootFolder)
        {
            return new DuplicateCleaner(rootFolder,
                                        new UniqueFileRegistry(),
                                        new BackedUpFileRegistry(rootFolder, null),
                                        new FileProvider(),
                                        new DirectoryProvider());
        }

        [Fact]
        public void Clean_ShallowUnique_NoBackup()
        {
            var root = @"D:\Projects\pet\UniqueFiles.Solution\UniqueFiles.BL.Tests\test_data\shallow_unique";
            var initialFilesAmount = Directory.GetFiles(root).Length;

            var cleaner = MakeDuplicateCleaner(root);

            cleaner.Clean();

            var finalFilesAmount = Directory.GetFiles(root).Length;
            finalFilesAmount.Should().Be(initialFilesAmount);

            var backedUpFilesAmount = Directory
                .GetFiles(Path.Combine(root, BackedUpFileRegistry.DefaultBackUpSubFolder))
                .Length;
            backedUpFilesAmount.Should().Be(0);
        }

        [Fact]
        public void Clean_ShallowWithDuplicates_AllInBackup()
        {
            var root = @"D:\Projects\pet\UniqueFiles.Solution\UniqueFiles.BL.Tests\test_data\shallow_duplicates";
            var initialFilesAmount = Directory.GetFiles(root).Length;

            var cleaner = MakeDuplicateCleaner(root);

            cleaner.Clean();

            var finalFilesAmount = Directory.GetFiles(root).Length;
            finalFilesAmount.Should().Be(initialFilesAmount);

            var backedUpFilesAmount = Directory
                .GetFiles(Path.Combine(root, BackedUpFileRegistry.DefaultBackUpSubFolder))
                .Length;
            backedUpFilesAmount.Should().Be(3);

            var backedUpFoldersAmount = Directory
                .GetDirectories(Path.Combine(root, BackedUpFileRegistry.DefaultBackUpSubFolder))
                .Length;
            backedUpFoldersAmount.Should().Be(1);
        }
    }
}
