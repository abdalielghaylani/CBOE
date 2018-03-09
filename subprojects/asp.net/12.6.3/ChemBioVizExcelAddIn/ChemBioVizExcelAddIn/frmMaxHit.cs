using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ChemBioVizExcelAddIn
{
    public partial class frmMaxHit : Form
    {
        public frmMaxHit()
        {
            InitializeComponent();
            InitializeEvents();
        }       

        #region Private Methods

        private void InitializeEvents()
        {
            this.btnOK.Click+=new EventHandler(btnOK_Click);
            this.Load+=new EventHandler(frmMaxHit_Load);            
            this.btnCancel.Click+=new EventHandler(btnCancel_Click);
            this.txtMaxPrompt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMaxPrompt_KeyPress);
            this.txtMaxRecord.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMaxRecord_KeyPress);

            // 11.0.4 - KeyPress event for Max records to show Structures
            this.txtMaxStructures.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMaxStructures_KeyPress);

            this.txtMaxStructureHeight.KeyPress+=new KeyPressEventHandler(txtMaxStructureHeight_KeyPress);
            this.txtMaxStructureWidth.KeyPress+=new KeyPressEventHandler(txtMaxStructureWidth_KeyPress);
        }

        private void ONKeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !Char.IsDigit(e.KeyChar) && e.KeyChar != '\b';

            if (e.KeyChar == 13)
                this.btnOK_Click(sender, e);          
        }
        private void ONKeyPressMaxLength(object sender, KeyPressEventArgs e, TextBox txt, int maxLen)
        {
            if (txt.Text.Length < maxLen || (e.KeyChar == '\b'))
                e.Handled = false;
            else
                e.Handled = true;
        }        

        private void StoreHits(string key, string value)
        {
            AppConfigSetting.WriteSetting(key, value);
        }

        private string RetriveHits(string key)
        {
            return AppConfigSetting.ReadSetting(key);
        }

        private bool IsPromptHitGreaterThanMaxHit(double promptHits, double maxHits)
        {
            if (promptHits > maxHits)
                return true;
            else
                return false;
        }
        private bool IsStructureHeightWidthEqualToZero(double height, double width)
        {
            if (height == 0 || width == 0)
                return true;
            else
                return false;
        }
        #endregion
        
        #region Events

        private void txtMaxPrompt_KeyPress(object sender, KeyPressEventArgs e)
        {
            ONKeyPress(sender, e);
        }

        private void txtMaxRecord_KeyPress(object sender, KeyPressEventArgs e)
        {
            ONKeyPress(sender, e);
        }

        // 11.0.4 - KeyPress event for Max records to show Structures
        private void txtMaxStructures_KeyPress(object sender, KeyPressEventArgs e)
        {
            ONKeyPress(sender, e);
        }

        private void txtMaxStructureHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            ONKeyPress(sender, e);          
        }
        private void txtMaxStructureWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            ONKeyPress(sender, e);           
        }

        private void frmMaxHit_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                //Max search prompt tag
                txtMaxPrompt.Text = RetriveHits(StringEnum.GetStringValue(Global.ConfigurationKey.MAX_PROMPT_HITS));
                txtMaxRecord.Text = RetriveHits(StringEnum.GetStringValue(Global.ConfigurationKey.MAX_NO_HITS));

                // 11.0.4 - Retreiving the max hits for structure
                txtMaxStructures.Text = RetriveHits(StringEnum.GetStringValue(Global.ConfigurationKey.STRUCTURE_MAX_HITS));

                //Max size tag
                txtMaxStructureHeight.Text = RetriveHits(StringEnum.GetStringValue(Global.ConfigurationKey.STRUCTURE_MAX_HEIGHT));
                txtMaxStructureWidth.Text = RetriveHits(StringEnum.GetStringValue(Global.ConfigurationKey.STRUCTURE_MAX_WIDTH));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsPromptHitGreaterThanMaxHit(Convert.ToDouble(txtMaxPrompt.Text), Convert.ToDouble(txtMaxRecord.Text)))
                {
                    tabControl1.SelectedIndex = 0;
                    tbSearchHit.Focus();                    
                    MessageBox.Show(Properties.Resources.msgValPrompthitMaxhit, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                  
                    txtMaxPrompt.Select();
                }               
                else if (string.IsNullOrEmpty(txtMaxStructureHeight.Text) || string.IsNullOrEmpty(txtMaxStructureWidth.Text) || IsStructureHeightWidthEqualToZero(Convert.ToDouble(txtMaxStructureHeight.Text), Convert.ToDouble(txtMaxStructureWidth.Text)))
                {
                    tabControl1.SelectedIndex = 1;
                    tbStructureDisp.Focus();
                    MessageBox.Show(Properties.Resources.msgValStructureHeightWidth, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    
                   
                }
                else
                {
                    Cursor.Current = Cursors.WaitCursor;
                    // max prompt tag
                    StoreHits(StringEnum.GetStringValue(Global.ConfigurationKey.MAX_PROMPT_HITS), txtMaxPrompt.Text.Trim());
                    StoreHits(StringEnum.GetStringValue(Global.ConfigurationKey.MAX_NO_HITS), txtMaxRecord.Text.Trim());
                    
                    // 11.0.4 - Retreiving the max hits for structure
                    StoreHits(StringEnum.GetStringValue(Global.ConfigurationKey.STRUCTURE_MAX_HITS), txtMaxStructures.Text.Trim());

                    //max structure size tag
                    StoreHits(StringEnum.GetStringValue(Global.ConfigurationKey.STRUCTURE_MAX_HEIGHT), txtMaxStructureHeight.Text.Trim());
                    StoreHits(StringEnum.GetStringValue(Global.ConfigurationKey.STRUCTURE_MAX_WIDTH), txtMaxStructureWidth.Text.Trim());

                    //Cursor.Current = Cursors.Default;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }       
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        } 
        #endregion      
    }
}