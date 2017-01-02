using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormDBLib;

namespace SpotfireIntegration.SpotfireAddin
{
    class SpotfireCOEAuthenticationPromptView : Login
    {
        private SpotfireCOEAuthenticationPromptModel _model;

        public SpotfireCOEAuthenticationPromptView(SpotfireCOEAuthenticationPromptModel model)
            : base(model.MRUList)
        {
            this._model = model;
            this.FormClosed += new FormClosedEventHandler(SpotfireCOEAuthenticationPromptView_FormClosed);
        }

        void SpotfireCOEAuthenticationPromptView_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                this.AddOrSelectMRU(this.CurrentMRU);

                this._model.Username = this.UserName;
                this._model.Password = this.Password;
                this._model.MRUList = this.MRUList;
                this._model.Server = this.CurrentMRU.Server;
            }
        }
    }
}
