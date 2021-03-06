namespace Somewhere2.Shared
{
    public class WebHostInfo
    {
        public int Port { get; set; }
        public string Address { get; set; }
        public bool ShouldLog { get; set; }

        #region Accessor - Endpoints
        public string ItemsURL => $"{Address}/Items";
        public string NotesURL => $"{Address}/Notes";
        #endregion
    }
}