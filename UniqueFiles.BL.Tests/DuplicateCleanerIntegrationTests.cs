using FluentAssertions;
using System.IO;
using Xunit;

namespace UniqueFiles.BL.Tests
{
    public class DuplicateCleanerIntegrationTests
    {
        //public DuplicateCleanerIntegrationTests()
        //{
        //    var testDataRoot = @"D:\Projects\pet\UniqueFiles.Solution\UniqueFiles.BL.Tests\test_data\";
        //    var testDataSetup = @"_setup";
        //}

        private DuplicateCleaner MakeDuplicateCleaner(string rootFolder)
        {
            var manager = new BackupDirectoryManager(rootFolder, null);
            manager.CreateBackupDirectory();
            return new DuplicateCleaner(rootFolder,
                                        new UniqueFileRegistry(),
                                        new BackedUpFileRegistry(manager),
                                        new FileProvider(),
                                        new DirectoryProvider(manager.BackupRoot));
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
                .GetFiles(Path.Combine(root, BackupDirectoryManager.DefaultBackUpSubFolder))
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
                .GetFiles(Path.Combine(root, BackupDirectoryManager.DefaultBackUpSubFolder))
                .Length;
            backedUpFilesAmount.Should().Be(3);

            var backedUpFoldersAmount = Directory
                .GetDirectories(Path.Combine(root, BackupDirectoryManager.DefaultBackUpSubFolder))
                .Length;
            backedUpFoldersAmount.Should().Be(1);
        }
    }
}
