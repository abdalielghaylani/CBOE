using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using FormDBLib;
using CBVUtilities;

namespace ChemBioViz.NET
{

    public partial class SaveDialog : Form
    {
        #region Variables
        private enum RadioChoices
        {
            Public = 0,
            Private = 1
        }
        private string radioChoice;
        private string fileName;
        private string comments;
        private string fileNameOrig, commentsOrig;
        private bool bIsPublicOrig;
        private bool bIsPropsMode;
        #endregion

        #region Properties
        public String FileName
        {
            get { return fileName; }
        }
        public String Comments
        {
            get { return comments; }
        }
        public bool IsPublic
        {
            get { return radioChoice == "0"; }
        }
        #endregion

        #region Constructors
        public SaveDialog(String formName, String comments, bool bIsPublic, bool bPropsMode, string title)
        {
            // CSBR-127972: do not validate unless modified; limited capabilities in props mode
            fileNameOrig = formName;
            commentsOrig = comments;
            bIsPublicOrig = bIsPublic;
            bIsPropsMode = bPropsMode;
            SetDialogConfiguration(formName, comments, bIsPublic, bPropsMode, title);
        }
        #endregion

        #region Methods
        //---------------------------------------------------------------------
        private void BindRadioList()
        {
            ListBox.ObjectCollection optionsCollections = new ListBox.ObjectCollection(optionsRadioList);
            optionsCollections.Add(RadioChoices.Public.ToString().ToLower());
            optionsCollections.Add(RadioChoices.Private.ToString().ToLower());
            this.optionsRadioList.Items.AddRange(optionsCollections);
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Set dialog configuration
        /// </summary>
        /// <param name="bPropsMode">Indicates if the visibility options are enable or not</param>
        private void SetDialogConfiguration(String initialFormName, String comments, bool bIsPublic, 
            bool bPropsMode, string title)
        {
            InitializeComponent();
            this.BindRadioList();
            radioChoice = bIsPublic ? "0" : "1";

            this.Text = bPropsMode ? title : "Save Form As";
            this.optionsRadioList.SetSelected((radioChoice == "0") ? 0 : 1, true);
            this.optionsRadioList.BackColor = Color.Transparent;
            this.optionsRadioList.Enabled = !bPropsMode;
            this.CenterToParent();
            nameTextBox.Text = initialFormName;
            nameTextBox.Enabled = !bPropsMode;  // CSBR-127972
            commentsTextBox.Text = comments;
        }
        //---------------------------------------------------------------------
        private static bool IsReservedWord(String word)
        {
            return  CBVUtil.Eqstrs(word, CBVConstants.TREE_FORMS_ROOT) ||
                    CBVUtil.Eqstrs(word, CBVConstants.PUBLIC_GROUPNAME) ||
                    CBVUtil.Eqstrs(word, CBVConstants.PRIVATE_GROUPNAME);
        }
        //---------------------------------------------------------------------
        private bool Err(String sMsg, ref String errMsg)
        {
            errMsg = sMsg;
            return false;
        }
        //---------------------------------------------------------------------
        private bool ValidateInput(ref String errMsg)
        {
            errMsg = String.Empty;
            String formName = nameTextBox.Text;

            if (String.IsNullOrEmpty(formName))
                return Err("Form name is blank", ref errMsg);

            if (IsReservedWord(formName))
                return Err(String.Format("'{0}' cannot be used as a form name", formName), ref errMsg);
            // Coverity Bug Fix CID 13002  
            ChemBioVizForm form = this.Owner as ChemBioVizForm;
            if (form != null)
            {
                DbObjectBank bank = IsPublic ? form.FormDbMgr.PublicFormBank : form.FormDbMgr.PrivateFormBank;

                bool bCanSavePublic = form.FormDbMgr.PrivilegeChecker.CanSavePublic;
                if (IsPublic && !bCanSavePublic)
                    return Err("Your user privileges do not allow saving forms to the Public tree", ref errMsg);

                if (bank.HasName(formName))
                {
                    String sMsg = String.Format("Name '{0}' is already in use.  OK to overwrite?", formName);
                    if (MessageBox.Show(sMsg, "Duplicate Name", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        return true;
                    return Err("NO_MSG", ref errMsg);
                }
                return true;
            }
            return false;
        }
        //---------------------------------------------------------------------
        #endregion

        #region Events
        private void formOptionsRadioList_SelectedIndexChanged(object sender, EventArgs e)
        {
            radioChoice = optionsRadioList.SelectedIndex.ToString();
        }
        //---------------------------------------------------------------------
        private void OKUltraButton_Click(object sender, EventArgs e)
        {
            String errMsg = String.Empty;

            fileName = nameTextBox.Text.ToString();
            comments = commentsTextBox.Text.ToString();
            radioChoice = optionsRadioList.SelectedIndex.ToString();

            // CSBR-130705: always call ValidateInput, even if dialog is not modified
            if (!bIsPropsMode && !ValidateInput(ref errMsg))
            {
                if (!errMsg.Equals("NO_MSG"))
                    MessageBox.Show(errMsg, "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // leave the dialog up so user can try again
            }
            DialogResult = DialogResult.OK;
        }
        //---------------------------------------------------------------------
        private void CancelUltraButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
        //---------------------------------------------------------------------
        static public bool PromptForSaveInfo(ChemBioVizForm form)
        {
            // return false if cancelled
            SaveDialog dlg = new SaveDialog(form.FormName, form.Comments, form.IsPublic, false, "Save As");
            DialogResult result = dlg.ShowDialog(form);
            if (result == DialogResult.OK)
            {
                form.FormName = dlg.fileName;
                form.Comments = dlg.comments;
                form.FormType = dlg.IsPublic ? ChemBioVizForm.formType.Public : ChemBioVizForm.formType.Private;
            }
            return result == DialogResult.OK;
        }
        //---------------------------------------------------------------------
        static public bool EditFormProps(ref String formName, ref String comments, bool bIsPublic,
                                            ChemBioVizForm form)
        {
            // return false if cancelled or no changes made
            // this routine is not called
            SaveDialog dlg = new SaveDialog(formName, comments, bIsPublic, true, "Form Properties");
            if (dlg.ShowDialog(form) == DialogResult.OK)
            {
                if (!(formName.Equals(dlg.FileName) && comments.Equals(dlg.Comments)))
                {
                    formName = dlg.FileName;
                    comments = dlg.Comments;
                    return true;
                }
            }
            return false;
        }
        //---------------------------------------------------------------------
        #endregion
    }






}
