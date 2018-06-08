namespace UniqueFiles.BL
{
    public interface IUniqueFileRegistry
    {
        bool Contains(string filePath);
        void Add(string filePath);
    }
}