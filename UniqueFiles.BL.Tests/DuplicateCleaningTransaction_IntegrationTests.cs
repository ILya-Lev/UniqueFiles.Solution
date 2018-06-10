using FluentAssertions;
using System.IO;
using Xunit;

namespace UniqueFiles.BL.Tests
{
    public class DuplicateCleaningTransaction_IntegrationTests
    {
        [Fact]
        public void Clean_ShallowUnique_NoBackup()
        {
            var root = @"..\..\..\test_data\shallow_unique";
            var initialFilesAmount = Directory.GetFiles(root).Length;

            var transaction = new DuplicateCleaningTransaction(root, null);

            transaction.Execute();

            var finalFilesAmount = Directory.GetFiles(root).Length;
            finalFilesAmount.Should().Be(initialFilesAmount);

            Directory
                .Exists(Path.Combine(root, BackupDirectoryManager.DefaultBackUpSubFolder))
                .Should()
                .BeFalse();
        }

        [Fact]
        public void Clean_DeepUnique_NoBackup()
        {
            var root = @"..\..\..\test_data\deep_unique\";
            var initialFilesAmount = Directory.GetFiles(root).Length;

            var transaction = new DuplicateCleaningTransaction(root, null);

            transaction.Execute();

            var finalFilesAmount = Directory.GetFiles(root).Length;
            finalFilesAmount.Should().Be(initialFilesAmount);

            Directory
                .Exists(Path.Combine(root, BackupDirectoryManager.DefaultBackUpSubFolder))
                .Should()
                .BeFalse();
        }

        [Fact]
        public void Clean_ShallowWithDuplicates_AllInBackup()
        {
            var root = @"..\..\..\test_data\shallow_duplicates";
            var initialFilesAmount = Directory.GetFiles(root).Length;

            var transaction = new DuplicateCleaningTransaction(root, null);

            transaction.Execute();

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

        [Fact]
        public void Clean_DeepWithDuplicates_AllInBackup()
        {
            var root = @"..\..\..\test_data\deep_duplicates\";
            var initialFilesAmount = Directory.GetFiles(root).Length;

            var transaction = new DuplicateCleaningTransaction(root, null);

            transaction.Execute();

            var finalFilesAmount = Directory.GetFiles(root).Length;
            finalFilesAmount.Should().Be(initialFilesAmount);

            var backedUpFilesAmount = Directory
                .GetFiles(Path.Combine(root, BackupDirectoryManager.DefaultBackUpSubFolder))
                .Length;
            backedUpFilesAmount.Should().Be(3);

            var backedUpFoldersAmount = Directory
                .GetDirectories(Path.Combine(root, BackupDirectoryManager.DefaultBackUpSubFolder))
                .Length;
            backedUpFoldersAmount.Should().Be(2);
        }
    }
}
