using FormDBLib;

namespace SpotfireIntegration.SpotfireAddin
{
    public class SpotfireCOEAuthenticationPromptModel
    {
        internal string Username { get; set; }

        internal string Password { get; set; }

        internal MRUList MRUList { get; set; }

        internal string Server { get; set; }

        internal void Save()
        {
            Properties.Settings.Default.LoginMRUList = this.MRUList;
            Properties.Settings.Default.Save();
        }
    }
}
