using FormDBLib;

namespace COEServiceLib
{
    public class SpotfireCOEAuthenticationPromptModel
    {
        bool enableServerSelectionCombo = true;

        public string Username { get; set; }

        public string Password { get; set; }

        public MRUList MRUList { get; set; }

        public string Server { get; set; }

        public MRUEntry CurrentMRUEntry { get; set; }

        /// <summary>
        /// Property to set the server name drop down state
        /// </summary>
        public bool EnableServerSelectionCombo
        {
            get
            {
                return enableServerSelectionCombo;
            }
            set
            {
                enableServerSelectionCombo = value;
            }
        }

        public void Save()
        {
            Properties.Settings.Default.LoginMRUList = this.MRUList;
            Properties.Settings.Default.Save();
        }
    }
}
