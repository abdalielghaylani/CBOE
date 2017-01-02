using System;
using System.ComponentModel;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework;

namespace SpotfireIntegration.SpotfireAddin
{
    public partial class COEHitListPromptView : Form
    {
        private COEHitListPromptModel promptModel;

        private COEHitListPromptView()
        {
            InitializeComponent();

            this.comboHitListType.Items.Clear();
            this.comboHitListType.Items.Add(HitListType.SAVED);
            this.comboHitListType.Items.Add(HitListType.TEMP);
            this.comboHitListType.SelectedIndex = 0;
        }

        public COEHitListPromptView(COEHitListPromptModel model)
            : this()
        {
            this.promptModel = model;
        }

        private void textHitListID_Validating(object sender, CancelEventArgs e)
        {
            int hitListID;
            if (int.TryParse(this.textHitListID.Text, out hitListID))
            {
                this.textHitListID.Text = hitListID.ToString();
            }
            else
            {
                e.Cancel = true;
                this.textHitListID.Clear();
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.promptModel.HitListID = int.Parse(this.textHitListID.Text);
            this.promptModel.HitListType = (HitListType)this.comboHitListType.SelectedItem;

            this.DialogResult = DialogResult.OK;
            this.Hide();
        }
    }
}
