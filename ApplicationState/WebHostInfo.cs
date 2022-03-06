namespace Somewhere2.ApplicationState
{
    public class WebHostInfo
    {
        public int Port { get; set; }
        public string Address { get; set; }
        public bool ShouldLog { get; set; }

        #region Accessor - Endpoints
        public string NotesURL => $"{Address}/Notes";
        #endregion
    }
}