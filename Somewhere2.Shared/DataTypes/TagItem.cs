namespace Somewhere2.Shared.DataTypes
{
    public enum ItemType
    {
        File,
        Folder,
        Note
    }
    public class TagItem
    {
        public string Path { get; set; }
        public ItemType Type {get; set; }
        public string[] Tags { get; set; }
        public string Notes { get; set; }

        public TagItem()
        {
            Notes = string.Empty;
        }

        public TagItem(string path, string[] tags, string notes)
        {
            Path = path;
            Tags = tags;
            Notes = notes ?? string.Empty;
        }
    }
}