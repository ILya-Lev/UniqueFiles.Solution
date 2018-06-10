namespace UniqueFiles.BL.Interfaces
{
    public interface IBackupDirectoryManager
    {
        string BackupRoot { get; }
        void CreateBackupDirectory(string subFolder = null);
        void DeleteBackupDirectory(string subFolder = null);
    }
}