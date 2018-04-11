using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using ChemBioViz.NET;
using CBVUtilities;
using SharedLib;

namespace SpotfireIntegration
{
    class SpotfireIntegrationAddin : ICBVAddin
    {
        private ChemBioVizForm m_form = null;

        #region Properties
        public bool AutoRefresh
        {
            get { return Properties.settings.Default.AutoRefresh; }
            set { Properties.settings.Default.AutoRefresh = value; }
        }
        public bool WarnOnRefresh
        {
            get { return Properties.settings.Default.WarnOnRefresh; }
            set { Properties.settings.Default.WarnOnRefresh = value; }
        }
        public bool WarnOnRebuild
        {
            get { return Properties.settings.Default.WarnOnRebuild; }
            set { Properties.settings.Default.WarnOnRebuild = value; }
        }
        public int MaxRows
        {
            get { return Properties.settings.Default.MaxRows; }
            set { Properties.settings.Default.MaxRows = value; }
        }
        public bool OpenAnalysis
        {
            get { return Properties.settings.Default.OpenAnalysis; }
            set { Properties.settings.Default.OpenAnalysis = value; }
        }
        #endregion

        #region ICBVAddin Implementation
        //---------------------------------------------------------------------
        public void Init(object form)
        {
            m_form = form as ChemBioVizForm;
        }
        //---------------------------------------------------------------------
        public void Execute()
        {
            if (!IsSpotfireAvailable())
                throw new AddinException("Spotfire is not available on the local machine.");

            m_form.AddinArguments = String.Empty;
            SpotfireController.GetInstance().StartIntegration(m_form);
        }
        //---------------------------------------------------------------------
        public void ExecuteWithString(String sData)
        {
            if (!IsSpotfireAvailable())
                throw new AddinException("Spotfire is not available on the local machine.");
            m_form.AddinArguments = sData;
            SpotfireController.GetInstance().StartIntegration(m_form);
        }
        //---------------------------------------------------------------------
        public void Deinit()
        {
            m_form.AddinArguments = String.Empty;
            SpotfireController.GetInstance().EndIntegration();
        }
        //---------------------------------------------------------------------
        public string GetDescription()
        {
            return "Exports the current query into Spotfire.";
        }
        //---------------------------------------------------------------------
        public IAddinMenu GetMenu()
        {
            if (!IsSpotfireAvailable())
                return null;
            return new SpotfireAddinMenu();
        }
        //---------------------------------------------------------------------
        public string GetMenuImagePath()
        {
            // get image from resources, write to temp file and return filename
            if (!IsSpotfireAvailable())
                return string.Empty;

            string imageName = string.Empty, sPath = string.Empty;
            switch (SpotfireController.GetInstance().State) {
                case SpotfireIntegration.SpotfireIntegrationState.Disconnected:
                    break;
                case SpotfireIntegration.SpotfireIntegrationState.Ready:
                    imageName = "green_both.gif";
                    break;
                case SpotfireIntegration.SpotfireIntegrationState.Connecting:
                case SpotfireIntegration.SpotfireIntegrationState.Starting:
                    imageName = "red_right.gif";
                    break;
                case SpotfireIntegration.SpotfireIntegrationState.Unsynched:
                    imageName = "grey_both.gif";
                    break;
            }
            if (!string.IsNullOrEmpty(imageName))
            {
                Assembly _assembly = Assembly.GetAssembly(typeof(SpotfireIntegration));
                System.IO.Stream stream = _assembly.GetManifestResourceStream("SpotfireIntegration.Resources." + imageName);
                if (stream != null)
                {
                    Image image = Image.FromStream(stream);
                    if (image != null)
                    {
                        sPath = Path.Combine(Path.GetTempPath(), imageName);
                        if (!File.Exists(sPath))
                            image.Save(sPath);
                    }
                }
            }
            return sPath;
        }
        //---------------------------------------------------------------------
        public bool HandleMenuCommand(string menuCmd)
        {
            bool ret = true;
            ChemBioVizForm form = m_form;
            switch (menuCmd)
            {
                case SFMenu.MENU_ITEM_CONNECT:
                    this.OpenAnalysis = false;
                    if (IsRefreshable())
                        ExecuteWithString("NOLOAD");
                    else
                        Execute();
                    break;

                case SFMenu.MENU_ITEM_DISCONNECT:
                    this.OpenAnalysis = false;
                    Deinit();
                    break;

                case SFMenu.MENU_ITEM_SFREFRESH:
                    this.OpenAnalysis = false;
                    if (form.FormDbMgr.ResultsCriteria != null)
                    {
                        String rcCurr = form.FormDbMgr.ResultsCriteria.ToString();
                        RecordsetChangedEventArgs rcArgs = new RecordsetChangedEventArgs(rcCurr);
                        rcArgs.ChangeAction = RecordsetChangedEventArgs.RecordsetChangeAction.Push; // force update
                        SpotfireController.GetInstance().CBVNRecordsetChanged(form, rcArgs);
                    }
                    break;

                case SFMenu.MENU_ITEM_SFOPEN:
                    this.OpenAnalysis = true;
                    DoOpenAnalysis();
                    break;

                case SFMenu.MENU_ITEM_AUTOREFRESH:
                    AutoRefresh = !AutoRefresh;
                    break;

                case SFMenu.MENU_ITEM_SFPROPS:
                    DoPropsDialog();
                    break;

                //CSBR - 150169 - (Saving a query to maintain the syn.)
                case SFMenu.MENU_ITEM_SFONCALLREFRESH:
                    String rcCurrSave = form.FormDbMgr.ResultsCriteria.ToString();
                    RecordsetChangedEventArgs rcArgsSave = new RecordsetChangedEventArgs(rcCurrSave);
                    rcArgsSave.ChangeAction = RecordsetChangedEventArgs.RecordsetChangeAction.Push;

                    bool isChanged = false;
                    if (Properties.settings.Default.WarnOnRefresh)
                    {
                        Properties.settings.Default.WarnOnRefresh = false;
                        isChanged = true;
                    }
                    SpotfireController.GetInstance().CBVNRecordsetChanged(form, rcArgsSave);
                    //To maintain the orignal state of  WarnOnRefresh property.
                    if (isChanged)
                        Properties.settings.Default.WarnOnRefresh = true;

                    break;

                default:
                    ret = false;
                    break;
            }
            return ret;
        }
        //---------------------------------------------------------------------
        private void DoPropsDialog()
        {
            SpotfirePropsDialog dlg = new SpotfirePropsDialog();
            DialogResult result = dlg.ShowDialog();
        }
        //---------------------------------------------------------------------
        public bool IsEnabled(string menuCmd)
        {
            switch (menuCmd)
            {
                case SFMenu.MENU_ITEM_CONNECT:
                    return m_form.FormDbMgr.SelectedDataView != null && !IsConnected();

                case SFMenu.MENU_ITEM_DISCONNECT:
                    return IsConnected();

                case SFMenu.MENU_ITEM_SFOPEN:
                //CSBR-151976.Here instead of checking whether form is selected or not at CBVN, 'Open Analysis'                  option is enabled by returning true.
                    return true;

                case SFMenu.MENU_ITEM_SFREFRESH:
                    return IsRefreshable();

                case SFMenu.MENU_ITEM_AUTOREFRESH:
                case SFMenu.MENU_ITEM_SFPROPS:
                    return true;
            }
            return false;
        }
        //---------------------------------------------------------------------
        public bool IsChecked(string menuCmd)
        {
            bool ret = (menuCmd.Equals(SFMenu.MENU_ITEM_AUTOREFRESH)) ? AutoRefresh : false;
            return ret;
        }
        //---------------------------------------------------------------------
        public string GetSettings()
        {
            // create key-value pairs for local settings, return as space-delimited string
            Dictionary<string, string> dict = new Dictionary<string, string>();

            dict.Add("AutoRefresh", AutoRefresh? "true" : "false");
            dict.Add("WarnOnRefresh", WarnOnRefresh ? "true" : "false");
            dict.Add("WarnOnRebuild", WarnOnRebuild ? "true" : "false");
            dict.Add("MaxRows", MaxRows.ToString());

            return CBVUtil.DictToString(dict);
        }
        //---------------------------------------------------------------------
        public void SetSettings(string s)
        {
            // given string of key-value pairs, parse into local vars
            Dictionary<string, string> dict = CBVUtil.StringToDict(s);
            foreach (KeyValuePair<string, string> kvp in dict)
            {
                if (kvp.Key.Equals("AutoRefresh"))
                    AutoRefresh = kvp.Value.Equals("true");
                else if (kvp.Key.Equals("WarnOnRefresh"))
                    WarnOnRefresh = kvp.Value.Equals("true");
                else if (kvp.Key.Equals("WarnOnRebuild"))
                    WarnOnRebuild = kvp.Value.Equals("true");
                else if (kvp.Key.Equals("MaxRows"))
                    MaxRows = CBVUtil.StrToInt(kvp.Value);
            }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Support Methods
        //---------------------------------------------------------------------
        public static bool IsSpotfireAvailable()
        {
            FileAssociation fileAssocation = new FileAssociation();
            string sSFExePath = fileAssocation.Get(".dxp");
            if (String.IsNullOrEmpty(sSFExePath))
                return false;
            if (!File.Exists(sSFExePath))
                return false;
            return true;
        }
        //---------------------------------------------------------------------
        public static bool IsSpotfireRunning()
        {
            // true if there is at least one spotfire process responding
            Process[] processes = Process.GetProcessesByName("Spotfire.Dxp");
            foreach (Process p in processes)
                if (p.Responding)
                    return true;
            return false;
        }
        //---------------------------------------------------------------------
        public bool IsConnected()
        {
            if (!IsSpotfireRunning())
                return false;
            //Fixed : #151098
            if (SpotfireController.GetInstance().State != SpotfireIntegration.SpotfireIntegrationState.Ready && SpotfireController.GetInstance().State != SpotfireIntegration.SpotfireIntegrationState.Unsynched)
                return false;
            return true;
        }
        //---------------------------------------------------------------------
        public bool IsRefreshable()
        {
            if (SpotfireController.GetInstance().State == SpotfireIntegration.SpotfireIntegrationState.Unsynched)
                return true;
            if (!IsConnected())
                return false;

            return true;    // TO DO: check form table/dview against SF versions
        }
        //---------------------------------------------------------------------
        private static string m_lastFilePath = String.Empty;
        private const String DXP_FILE_FILTERS = "DXP files (*.dxp)|*.dxp|All files (*.*)|*.*";

        private void DoOpenAnalysis()
        {
            OpenFileDialog dlg = new OpenFileDialog();

            if (!String.IsNullOrEmpty(m_lastFilePath))
                dlg.InitialDirectory = Path.GetDirectoryName(m_lastFilePath);
            else
                dlg.InitialDirectory = Application.CommonAppDataPath;

            dlg.Filter = DXP_FILE_FILTERS;
            dlg.FileName = "*.dxp";
            
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                m_lastFilePath = dlg.FileName;
                ExecuteWithString(m_lastFilePath);
            }
        }
        //---------------------------------------------------------------------
        #endregion
    }
}
