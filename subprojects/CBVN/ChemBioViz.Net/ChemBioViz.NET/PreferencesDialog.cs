using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Linq;

using FormDBLib;
using CBVUtilities;
using ChemBioViz.NET.Exceptions;

using CambridgeSoft.COE.Framework.Common;



namespace ChemBioViz.NET
{
    public partial class PreferencesDialog : Form
    {
        #region Constants
        private const String FORM_TITLE = "Preferences";
        private const String VALIDATION_OPEN_MODE_MESSAGE = "You have to select a default form below. Otherwise, you'll have to select to open a blank form or the last viewed one";
        #endregion

        #region Variables
        private String[] m_barGroupKeys;
        private Dictionary<int, string> m_publicPaths;
        private Dictionary<int, string> m_userPaths;
        #endregion

        #region Constructors
        public PreferencesDialog(String[] barGroupKeys, Dictionary<int, string> publicPaths, Dictionary<int, string> userPaths)
        {
            InitializeComponent();
            this.m_barGroupKeys = barGroupKeys;
            this.m_publicPaths = publicPaths;
            this.m_userPaths= userPaths;
        }
        #endregion

        #region Methods
        private void SetDialogConfiguration()
        {
            this.CenterToParent();
            this.Text = FORM_TITLE;

            this.SetVisibilityRadioButtons();
            this.SetOpeningWaysRadioButtons();

            this.simPercentUpDown.Value = SearchOptionsSettings.Default.SimThreshold;
            this.SetSimilarityControls();

            this.BindNavigationPaneComboBox();
            this.BindFormNameComboBox();
            this.BindStyleComboBox();

            this.SetDefaultLogPath();
            this.filterChildCheckBox.Checked = Properties.Settings.Default.FilterChildHits;
            this.showHilitesCheckBox.Checked = Properties.Settings.Default.ShowSSSHilites;
        }
        //---------------------------------------------------------------------
        private void SetVisibilityRadioButtons()
        {
            bool isPublic = PreferencesHelper.PreferencesHelperInstance.DefaultFormType == ChemBioVizForm.formType.Public;
            if (isPublic)
                publicRadioButton.Checked = true;
            else
                privateRadioButton.Checked = true;
        }
        //---------------------------------------------------------------------
        private void SetOpeningWaysRadioButtons()
        {
            blankFormRadioButton.Text = CBVConstants.OPEN_BLANK_FORM;
            defaultFormRadioButton.Text = CBVConstants.OPEN_DEFAULT_FORM;
            lastFormRadioButton.Text = CBVConstants.OPEN_LAST_FORM;

            switch (PreferencesHelper.PreferencesHelperInstance.OpenMode)
            {
                case CBVConstants.OPEN_BLANK_FORM:
                    blankFormRadioButton.Checked = true;
                    this.EnableDisableDefaultFormControls(false);
                    break;
                case CBVConstants.OPEN_DEFAULT_FORM:
                    defaultFormRadioButton.Checked = true;
                    this.EnableDisableDefaultFormControls(true);
                    break;
                case CBVConstants.OPEN_LAST_FORM:
                    lastFormRadioButton.Checked = true;
                    this.EnableDisableDefaultFormControls(false);
                    break;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Enable or Diable similarity control
        /// </summary>
        private void SetSimilarityControls() 
        {
            simPercentUpDown.Enabled = similarButton.Checked;
            showHilitesCheckBox.Enabled = true; // !similarButton.Checked;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///   Populate the Default Pane dropdown list
        /// </summary>
        private void BindNavigationPaneComboBox()
        {
            int index = 0;
            this.navigationPaneComboBox.Items.AddRange(m_barGroupKeys);
            // Edit Pane is not selectable
            if (this.navigationPaneComboBox.Items.Contains(CBVConstants.FORMEDIT_GROUPNAME))
            {
                index = this.navigationPaneComboBox.Items.IndexOf(CBVConstants.FORMEDIT_GROUPNAME);
                this.navigationPaneComboBox.Items.RemoveAt(index);
            }
            navigationPaneComboBox.SelectedItem = Properties.Settings.Default.NavigationPane;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Populate the Default Form dropdown list
        /// </summary>
        private void BindFormNameComboBox()
        {
            this.formNameComboBox.Items.Clear();
            this.formNameComboBox.Items.Add(CBVConstants.DEFAULT_MESSAGE_FORM_SELECTION);

            bool bPublic = publicRadioButton.Checked ? true : false;

            if (bPublic)
                AddPathsToFormNameComboBox(m_publicPaths);
            else
                AddPathsToFormNameComboBox(m_userPaths);

            // select item according to Settings.Default.DefaultFormName
            string selItemPath = string.Empty;
            int id = Properties.Settings.Default.DefaultFormID;
            if(id > 0)
                selItemPath = bPublic ? GetSelItemPath(m_publicPaths, id) : GetSelItemPath(m_userPaths, id);
            if(string.IsNullOrEmpty(selItemPath))
                Properties.Settings.Default.DefaultFormName = String.Empty;

            formNameComboBox.SelectedItem = String.IsNullOrEmpty(Properties.Settings.Default.DefaultFormName) ?
                CBVConstants.DEFAULT_MESSAGE_FORM_SELECTION : selItemPath;
        }
        //---------------------------------------------------------------------
        private void AddPathsToFormNameComboBox(Dictionary <int, string> paths)
        {
            if (paths.Count > 0)
            {
                string[] pArray = new string [paths.Count];
                paths.Values.CopyTo(pArray, 0);
                this.formNameComboBox.Items.AddRange(pArray);
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Populate the Style dropdown list
        /// </summary>
        private void BindStyleComboBox()
        {
            this.styleComboBox.Items.Add(CBVConstants.STYLE_NOT_SET);
            this.styleComboBox.Items.AddRange(GetStyleLibrariesList());
            this.styleComboBox.SelectedItem = Properties.Settings.Default.StyleLibrary;
        }
        //---------------------------------------------------------------------
        private string GetSelItemPath(Dictionary<int, string> paths, int id)
        {
            string nodePath = string.Empty;
            if (paths.ContainsKey(id))
                nodePath = paths[id];
            return nodePath;
        }
        //---------------------------------------------------------------------
        private bool ValidateDefaultForm()
        { 
            bool result = true;
            if (defaultFormRadioButton.Checked &&
                (formNameComboBox.SelectedItem.Equals(CBVConstants.DEFAULT_MESSAGE_FORM_SELECTION) || formNameComboBox.SelectedItem == null))
                result = false;
            return result;
        }
        //---------------------------------------------------------------------
        private String[] GetStyleLibrariesList()
        {
            bool bUseDbStyles = true;  // styles are now retrieved from db
            if (!bUseDbStyles) 
            {
                // get styles from folder under common app data dir (previously was under StartupPath)
                String styleLibraryPath = PreferencesHelper.GetStylesDir();
                String[] libraries = System.IO.Directory.GetFiles(styleLibraryPath);
                if (libraries.Length > 0) {
                    for (int i = 0; i < libraries.Length; i++)
                        libraries[i] = libraries[i].Remove(0, styleLibraryPath.Length);
                }
                return libraries;
            }
            // new way: get styles from database
            List<String> dbStylesList = PreferencesHelper.PreferencesHelperInstance.StylesBank.GetNameList();
            String[] libraries1 = new String[dbStylesList.Count];    // safe if value is zero
            int j = 0;
            foreach (String s in dbStylesList)
                libraries1[j++] = s;

            return libraries1;
        }
        //---------------------------------------------------------------------
        private void ReloadFormNameCombobox()
        {
            //As FormNameComboBox is going to be reload with forms from other bank, then we need to reset the previous form name 
            //in  order to avoid inconsistencies later when Loading it from the main window.
            Properties.Settings.Default.DefaultFormName = String.Empty;
            this.BindFormNameComboBox();
        }
        //---------------------------------------------------------------------
        private void EnableDisableDefaultFormControls(bool defaultButtonState)
        {
            formNameLabel.Enabled = defaultButtonState;
            formNameComboBox.Enabled = defaultButtonState;
            publicRadioButton.Enabled = defaultButtonState;
            privateRadioButton.Enabled = defaultButtonState;
            formNameComboBox.SelectedItem = CBVConstants.DEFAULT_MESSAGE_FORM_SELECTION;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// This method serializes current state of the controls in the .settings file
        /// </summary>
        private void SerializeSettings()
        {
            SearchOptionsSettings.Default.FullSearch = fullstructureButton.Checked;
            SearchOptionsSettings.Default.Similar = similarButton.Checked;
            SearchOptionsSettings.Default.Substructure = substructureButton.Checked;
            SearchOptionsSettings.Default.Exact = exactButton.Checked;
            SearchOptionsSettings.Default.SimThreshold = (int)simPercentUpDown.Value;

            Properties.Settings.Default.FilterChildHits = this.filterChildCheckBox.Checked;
            Properties.Settings.Default.ShowSSSHilites = this.showHilitesCheckBox.Checked;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Checks if there is a path already stored on the settings, 
        ///  if not the default path will be what Application.CommonAppDataPath indicates
        /// </summary>
        private void SetDefaultLogPath()
        {
            // CSBR-115292: if path is faulty, turn off checkbox
            logPathTextBox.Text = CBVUtil.LogPath;
            if (!CBVUtil.IsValidOutputPath(CBVUtil.LogPath))
                logCheckBox.Checked = false;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///   Reset to default Open Mode
        /// </summary>
        private void ResetOpenMode()
        {
            blankFormRadioButton.Checked = true;
            this.EnableDisableDefaultFormControls(false);
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///   Reset to default Structure Search
        /// </summary>
        private void ResetStructureSearch()
        {
            substructureButton.Checked = true;
            this.SetSimilarityControls();
        }
        #endregion

        #region Events
        private void Preferences_Load(object sender, EventArgs e)
        {
            // squirrel away a copy of current settings in case of cancel
            PreferencesHelper.PreferencesHelperInstance.BackupSettings();
            this.SetDialogConfiguration();
        }
        //---------------------------------------------------------------------
        private void styleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (styleComboBox.SelectedItem != null)
                PreferencesHelper.PreferencesHelperInstance.LoadStyle(styleComboBox.SelectedItem.ToString());
        }
        //---------------------------------------------------------------------
        private void formNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (formNameComboBox.SelectedItem != null && !string.IsNullOrEmpty(formNameComboBox.SelectedItem.ToString()) 
                && !formNameComboBox.SelectedItem.Equals(CBVConstants.DEFAULT_MESSAGE_FORM_SELECTION))
            {
                int id = -1;
                string groupName = publicRadioButton.Checked ? CBVConstants.PUBLIC_GROUPNAME : groupName = CBVConstants.PRIVATE_GROUPNAME;
                string selItem = formNameComboBox.SelectedItem.ToString();
                string nodeName = selItem.Substring(selItem.LastIndexOf(CBVConstants.TREE_PATH_SEPARATOR) + 1);
                ChemBioVizForm.formType fType = publicRadioButton.Checked ? ChemBioVizForm.formType.Public : ChemBioVizForm.formType.Private;

                Dictionary<int, string> paths = publicRadioButton.Checked ? m_publicPaths : m_userPaths;
                id = GetNodeID(paths);

                PreferencesHelper.PreferencesHelperInstance.StoreDefaultForm(id, nodeName, fType);
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Gets a dictionary key from its value
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        private int GetNodeID(Dictionary<int, string> paths)
        {
            int id = -1;
            string selNode = formNameComboBox.SelectedItem.ToString();
            if(paths.ContainsValue(selNode))
            {
                id = (from k in paths
                           where string.Compare(k.Value, selNode, true) == 0
                           select k.Key).FirstOrDefault();
            }
            return id;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Reset controls to their default values depending on the selected tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetToDefaultUltraButton_Click(object sender, EventArgs e)
        {
            // Reset to Default button works for all tabs except for the Search one<z
            if (AdvancedUltraTabControl.SelectedTab.Key.Equals(CBVConstants.PREFERENCES_MAIN) 
                || AdvancedUltraTabControl.SelectedTab.Key.Equals(CBVConstants.PREFERENCES_STYLES)
                || AdvancedUltraTabControl.SelectedTab.Key.Equals(CBVConstants.PREFERENCES_LOG)
                || AdvancedUltraTabControl.SelectedTab.Key.Equals(CBVConstants.PREFERENCES_ADVANCED))
            {
                this.ResetOpenMode();
                PreferencesHelper.PreferencesHelperInstance.ResetGeneralSettingsToDefault();
            }
            else if (AdvancedUltraTabControl.SelectedTab.Key.Equals(CBVConstants.PREFERENCES_SEARCH))
            {
                this.ResetStructureSearch();
                PreferencesHelper.PreferencesHelperInstance.ResetSearchSettingsToDefault();
            }

            this.BindFormNameComboBox();
        }
        //---------------------------------------------------------------------
        private void OKUltraButton_Click(object sender, EventArgs e)
        {
            if (this.ValidateDefaultForm())
            {
                try
                {
                    SerializeSettings();
                    PreferencesHelper.PreferencesHelperInstance.ApplyAllSettings();
                    this.Close();
                }
                catch (CustomSettingsProviderException ex)
                {
                    MessageBox.Show(new StringBuilder(ex.Message).AppendLine().AppendLine(ex.StackTrace).ToString(),
                                 "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(VALIDATION_OPEN_MODE_MESSAGE, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //---------------------------------------------------------------------
        private void CancelUltraButton_Click(object sender, EventArgs e)
        {
            // restore settings saved in Preferences_Load
            PreferencesHelper.PreferencesHelperInstance.RestoreBackupSettings();
            this.Close();
        }
        //---------------------------------------------------------------------
        private void ApplyUltraButton_Click(object sender, EventArgs e)
        {
            if (this.ValidateDefaultForm())
            {
                try
                {
                    SerializeSettings();
                    PreferencesHelper.PreferencesHelperInstance.ApplyAllSettings();
                }
                catch (CustomSettingsProviderException ex)
                {
                    MessageBox.Show(new StringBuilder(ex.Message).AppendLine().AppendLine(ex.StackTrace).ToString(),
                                 "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(VALIDATION_OPEN_MODE_MESSAGE, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //---------------------------------------------------------------------
        private void advancedButton_Click(object sender, EventArgs e)
        {
            AdvancedSearchDialog advancedSearchDialog = new AdvancedSearchDialog();
            advancedSearchDialog.ShowDialog();
        }
        //---------------------------------------------------------------------
        private void publicRadioButton_Click(object sender, EventArgs e)
        {
            if (publicRadioButton.Checked)
            {
                Properties.Settings.Default.DefaultFormType = CBVConstants.FormOptions.Public.ToString();
                this.ReloadFormNameCombobox();
            }
        }
        //---------------------------------------------------------------------
        private void privateRadioButton_Click(object sender, EventArgs e)
        {
            if (privateRadioButton.Checked)
            {
                Properties.Settings.Default.DefaultFormType = CBVConstants.FormOptions.Private.ToString();
                this.ReloadFormNameCombobox();
            }
        }
        //---------------------------------------------------------------------
        private void blankFormRadioButton_Click(object sender, EventArgs e)
        {
            if (blankFormRadioButton.Checked)
            {
                Properties.Settings.Default.OpenMode = CBVConstants.OPEN_BLANK_FORM;
                this.EnableDisableDefaultFormControls(false);
            }
        }
        //---------------------------------------------------------------------
        private void defaultFormRadioButton_Click(object sender, EventArgs e)
        {
            if (defaultFormRadioButton.Checked)
            {
                Properties.Settings.Default.OpenMode = CBVConstants.OPEN_DEFAULT_FORM;
                this.EnableDisableDefaultFormControls(true);
            }
        }
        //---------------------------------------------------------------------
        private void lastFormRadioButton_Click(object sender, EventArgs e)
        {
            if (lastFormRadioButton.Checked)
            {
                Properties.Settings.Default.OpenMode = CBVConstants.OPEN_LAST_FORM;
                this.EnableDisableDefaultFormControls(false);
            }
        }
        //---------------------------------------------------------------------
        private void similarButton_Click(object sender, EventArgs e)
        {
            SetSimilarityControls();
        }
        //---------------------------------------------------------------------
        private void fullstructureButton_Click(object sender, EventArgs e)
        {
            SetSimilarityControls();
        }
        //---------------------------------------------------------------------
        private void exactButton_Click(object sender, EventArgs e)
        {
            SetSimilarityControls();
        }
        //---------------------------------------------------------------------
        private void substructureButton_Click(object sender, EventArgs e)
        {
            SetSimilarityControls();
        }
        //---------------------------------------------------------------------
        private void fileLocationCheckBox_Click(object sender, EventArgs e)
        {
            if (fileLocationCheckBox.Checked)
            {
                //Get the server version
                if (MessageBox.Show(String.Concat("Settings will be loaded from the server. Continue? "),
                                "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    PreferencesHelper.PreferencesHelperInstance.BackupLocalSettings();
                    PreferencesHelper.PreferencesHelperInstance.RetrieveSettingsFromServer();
                    PreferencesHelper.PreferencesHelperInstance.ReloadNewFileVersion();
                }
                else
                {
                    fileLocationCheckBox.Checked = false;
                }
            }
            else
            {
                //Get the local version
                if (PreferencesHelper.PreferencesHelperInstance.RestoreLocalSettings())
                {
                    PreferencesHelper.PreferencesHelperInstance.ReloadNewFileVersion();
                }
            }
        }
        //---------------------------------------------------------------------
        private void browseButton_Click(object sender, EventArgs e)
        {
            String logPath = logPathTextBox.Text;

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = String.IsNullOrEmpty(logPath) ? CBVConstants.ERROR_LOG_FILE : Path.GetFileName(logPath);
            dlg.InitialDirectory = String.IsNullOrEmpty(logPath) ? Application.CommonAppDataPath : Path.GetDirectoryName(logPath);
            dlg.Filter = CBVConstants.TXT_FILE_FILTERS;
            dlg.DefaultExt = ".txt";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                logPathTextBox.Text = dlg.FileName;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Enables controls to add a log path or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // CSBR-115292: if user checks box but path is invalid, uncheck and enable controls
            bool bEnable = logCheckBox.Checked;
            if (bEnable && !CBVUtil.IsValidOutputPath(logPathTextBox.Text))
            {
                if (!CBVUtil.EndsWith(logPathTextBox.Text, " [invalid]"))
                    logPathTextBox.Text += " [invalid]";
                logCheckBox.Checked = false;
            }
            logLabel.Enabled = bEnable;
            logPathTextBox.Enabled = bEnable;
            browseButton.Enabled = bEnable;
            resetButton.Enabled = bEnable;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Remove the log file content
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(logPathTextBox.Text.ToString()))
            {
                using (StreamWriter logWriter = new StreamWriter(logPathTextBox.Text.ToString(), false))
                    logWriter.Write(string.Empty);
            }
        }
        //---------------------------------------------------------------------
        public void SelectSearchTab()
        {
            AdvancedUltraTabControl.SelectedTab = AdvancedUltraTabControl.Tabs[CBVConstants.PREFERENCES_SEARCH];
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Changes the text of the Reset Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvancedUltraTabControl_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {
            if (AdvancedUltraTabControl.SelectedTab.Key.Equals(CBVConstants.PREFERENCES_MAIN)
                || AdvancedUltraTabControl.SelectedTab.Key.Equals(CBVConstants.PREFERENCES_STYLES)
                || AdvancedUltraTabControl.SelectedTab.Key.Equals(CBVConstants.PREFERENCES_LOG)
                || AdvancedUltraTabControl.SelectedTab.Key.Equals(CBVConstants.PREFERENCES_ADVANCED))
            {
                resetToDefaultUltraButton.Text = "Reset general preferences";
            }
            else if (AdvancedUltraTabControl.SelectedTab.Key.Equals(CBVConstants.PREFERENCES_SEARCH))
            {
                resetToDefaultUltraButton.Text = "Reset search preferences";
            }
        }
        #endregion
    }
}
