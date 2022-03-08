namespace Somewhere2.Shared.DataTypes
{
    public enum RecentType
    {
        // Operational
        Folder,
        File,
        // Data type
        Database
    }
    public struct Recent
    {
        public string Value { get; set; }
        public RecentType Annotation { get; set; }
    }
}