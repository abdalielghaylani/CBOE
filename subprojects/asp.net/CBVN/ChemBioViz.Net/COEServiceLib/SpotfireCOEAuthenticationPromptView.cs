using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormDBLib;

namespace COEServiceLib
{
    public class SpotfireCOEAuthenticationPromptView : Login
    {
        private SpotfireCOEAuthenticationPromptModel _model;

        public SpotfireCOEAuthenticationPromptView(SpotfireCOEAuthenticationPromptModel model)
            : base(model.MRUList)
        {
            this._model = model;
            //try to set the current server entry from list of MRU
            if (!string.IsNullOrEmpty(this._model.CurrentMRUEntry.Server))
            {
                this.SelectMRUFromCombo(this._model.CurrentMRUEntry);
                this.SetServerComboState(this._model.EnableServerSelectionCombo);
            }
            this.FormClosed += new FormClosedEventHandler(SpotfireCOEAuthenticationPromptView_FormClosed);
        }

        void SpotfireCOEAuthenticationPromptView_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                this.AddOrSelectMRU(this.CurrentMRU);

                this._model.CurrentMRUEntry = this.CurrentMRU;
                this._model.Username = this.UserName;
                this._model.Password = this.Password;
                this._model.MRUList = this.MRUList;
                this._model.Server = this.CurrentMRU.Server;
            }
        }
    }
}
