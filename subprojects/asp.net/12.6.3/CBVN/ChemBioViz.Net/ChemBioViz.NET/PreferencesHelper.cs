using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Xml;
using System.Configuration;

using CambridgeSoft.COE.Framework.Common;

using FormDBLib;
using SearchPreferences;
using CBVUtilities;
using FormDBLib.Exceptions;


namespace ChemBioViz.NET
{
    public class PreferencesHelper
    {
        #region Variables
        private static PreferencesHelper m_preferencesHelperInstance = new PreferencesHelper();
        private CustomSettingsProvider m_customProvider = new CustomSettingsProvider();
        private string m_previousSettings = string.Empty;
        private SettingsObjectBank m_sBank;
        private StylesObjectBank m_stylesBank;

        private string m_settingsObjName = string.Empty;
        private string m_appSettingsPath;
        private string m_appSettingsFileName;

        private bool   m_getPrefsFromServer = true;
        private bool   m_askSavingPrefs;

        private string m_defaultFormName;
        private ChemBioVizForm.formType m_defaultFormType;
        private int m_defaultFormID;
        private string m_lastOpenFormName;
        private ChemBioVizForm.formType m_lastOpenFormType;
        private int m_lastOpenFormID;

        private string m_navigationPane;
        private string m_openMode;
        private string m_styleLibrary;
        private string m_username;
        private string m_password;
        private int    m_loadingFlag;
        private string m_infraSettings;
        private bool   m_writeLog;
        private string m_logPath;
        private decimal m_pageSize;
        private string m_treeConfig;
        #endregion

        #region Properties
        public static PreferencesHelper PreferencesHelperInstance
        {
            get { return PreferencesHelper.m_preferencesHelperInstance; }
            set { PreferencesHelper.m_preferencesHelperInstance = value; }
        }
        public int    LoadingFlag
        {
            get { return m_loadingFlag; }
            set { m_loadingFlag = value; }
        }
        public bool   GetPrefsFromServer
        {
            get { return m_getPrefsFromServer; }
            set { m_getPrefsFromServer = value; }
        }
        public string DefaultFormName
        {
            get { return m_defaultFormName; }
            set { m_defaultFormName = value; }
        }
        public int DefaultFormID
        {
            get { return m_defaultFormID; }
            set { m_defaultFormID = value; }
        }
        public ChemBioVizForm.formType DefaultFormType
        {
            get { return m_defaultFormType; }
            set { m_defaultFormType = value; }
        }
        public string LastOpenFormName
        {
            get { return m_lastOpenFormName; }
            set { m_lastOpenFormName = value; }
        }
        public int LastOpenFormID
        {
            get { return m_lastOpenFormID; }
            set { m_lastOpenFormID = value; }
        }
        public ChemBioVizForm.formType LastOpenFormType
        {
            get { return m_lastOpenFormType; }
            set { m_lastOpenFormType = value; }
        }
        public string NavigationPane
        {
            get { return m_navigationPane; }
            set 
            { 
                m_navigationPane = value;
                Properties.Settings.Default.NavigationPane = value;
            }
        }
        public string OpenMode
        {
            get { return m_openMode; }
            set { m_openMode = value; }
        }
        public string StyleLibrary
        {
            get { return m_styleLibrary; }
            set { m_styleLibrary = value; }
        }
        public string InfraSettings
        {
            get { return m_infraSettings; }
            set { m_infraSettings = value; }
        }
        public bool   WriteLog
        {
            get { return m_writeLog; }
            set { m_writeLog = value; }
        }
        public string LogPath
        {
            get { return m_logPath; }
            set { m_logPath = value; }
        }
        public decimal PageSize
        {
            get { return m_pageSize; }
            set { m_pageSize = value; }
        }
        public StylesObjectBank StylesBank
        {
            get { return m_stylesBank; }
        }
        public string TreeConfig
        {
            get { return m_treeConfig; }
            set 
            { 
                m_treeConfig = value;
                Properties.Settings.Default.TreeConfig = value;
            }
        }
        #endregion

        #region Methods
        public PreferencesHelper()
        {
        }
        /// <summary>
        /// Sets the Settings Provider to a Custom SettingsProvider
        /// </summary>
        public void SetCustomSettingsProvider()
        {
            this.SetSettingsProvider(Properties.Settings.Default, m_customProvider);
            this.SetSettingsProvider(SearchOptionsSettings.Default, m_customProvider);

            m_appSettingsPath = ((CustomSettingsProvider)Properties.Settings.Default.Providers[CBVConstants.PORTABLE_SETTINGS_PROVIDER]).GetAppSettingsPath();
            m_appSettingsFileName = ((CustomSettingsProvider)Properties.Settings.Default.Providers[CBVConstants.PORTABLE_SETTINGS_PROVIDER]).GetAppSettingsFilename();

           ((CustomSettingsProvider)Properties.Settings.Default.Providers[CBVConstants.PORTABLE_SETTINGS_PROVIDER]).GetPropertyValues(
               Properties.Settings.Default.Context, Properties.Settings.Default.Properties);

           ((CustomSettingsProvider)SearchOptionsSettings.Default.Providers[CBVConstants.PORTABLE_SETTINGS_PROVIDER]).GetPropertyValues(
               SearchOptionsSettings.Default.Context, SearchOptionsSettings.Default.Properties);
            
        }
        /// <summary>
        /// Sets the Settings Provider to LocalFileSettingsProvider. This used to be the default one. 
        /// </summary>
        public void SetLocalSettingsProvider()
        {
            SettingsProvider provider = Properties.Settings.Default.Providers["LocalFileSettingsProvider"];
            //There is always a LocalFileSettingsProvider in the collection because it's the default one
            this.SetSettingsProvider(Properties.Settings.Default, provider);
            this.SetSettingsProvider(SearchOptionsSettings.Default, provider);
            //Force GetPrefsFromServer to be False. It is set with True value by default. 
            //As we're reasigning the Provider to the LocalSettings, then, it'll take the default values for starters.
            Properties.Settings.Default.GetPrefsFromServer = false;
        }
        /// <summary>
        /// Sets the Custom Settigns Provider to each application properties
        /// </summary>
        /// <param name="settingsBase"></param>
        public void SetSettingsProvider(ApplicationSettingsBase settingsBase, SettingsProvider provider)
        {
            //ApplicationSettingsBase appSettingsBase = settingsBase;
            if (!this.ValidateProvider(settingsBase, provider))
                settingsBase.Providers.Add(provider);

            foreach (SettingsProperty property in settingsBase.Properties)
            {
                property.Provider = provider;
            }
        }
        /// <summary>
        /// Validates is the current provider already exists in the ApplicationSettingsBase collection
        /// </summary>
        /// <param name="settingsBase"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        private bool ValidateProvider(ApplicationSettingsBase settingsBase, SettingsProvider provider)
        {
            bool exists = false;
            foreach (SettingsProvider aProvider in settingsBase.Providers)
            {
                if (aProvider.Name.Equals(provider.Name))
                {
                    exists = true;
                }
            }
            return exists;
        }
        /// <summary>
        /// Returns directory name where local style files are stored
        /// </summary>
        public static String GetStylesDir()
        {
            String s = Path.Combine(Application.CommonAppDataPath, CBVConstants.STYLES_FOLDER_NAME /*"Styles"*/);
            return s;
        }
        /// <summary>
        /// Sets all neccesary features to load settings properly at startup
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void InitializePrefsHelper(string username, string password)
        {
            // this routine is called twice; first call has empty username/password
            this.SetCustomSettingsProvider();
            if (File.Exists(Path.Combine(m_customProvider.GetAppSettingsPath(), m_customProvider.GetAppSettingsFilename())))
            {
                Properties.Settings.Default.Reload();
            }
            m_getPrefsFromServer = Properties.Settings.Default.GetPrefsFromServer;

            m_username = username.ToUpper();
            m_password = password;

            if (m_getPrefsFromServer && !String.IsNullOrEmpty(m_username))
            {
                this.RetrieveSettingsFromServer();
                ((CustomSettingsProvider)Properties.Settings.Default.Providers[CBVConstants.PORTABLE_SETTINGS_PROVIDER]).
                    UseNewVersion = true;

                if (File.Exists(Path.Combine(m_customProvider.GetAppSettingsPath(), m_customProvider.GetAppSettingsFilename())))
                {
                    Properties.Settings.Default.Reload();
                    SearchOptionsSettings.Default.Reload();
                }
            }

            this.LoadGeneralPreferences();
            this.LoadSearchPreferences();

            CBVUtil.Log      = m_writeLog;
            CBVUtil.LogPath  = m_logPath;
            CBVUtil.PageSize = (int) m_pageSize;

            if (File.Exists(Path.Combine(m_appSettingsPath, m_appSettingsFileName)))
            {
                m_previousSettings = this.GetSettingsFileAsString();
                this.m_loadingFlag = 1;
            }
            else
            {
                //There is no settings file
                ((CustomSettingsProvider)Properties.Settings.Default.Providers[CBVConstants.PORTABLE_SETTINGS_PROVIDER]).SetPropertyValues(
               Properties.Settings.Default.Context, Properties.Settings.Default.PropertyValues);

                ((CustomSettingsProvider)SearchOptionsSettings.Default.Providers[CBVConstants.PORTABLE_SETTINGS_PROVIDER]).SetPropertyValues(
                    SearchOptionsSettings.Default.Context, SearchOptionsSettings.Default.PropertyValues);
            }

            if (this.m_loadingFlag == 0 && !String.IsNullOrEmpty(m_username))
                MessageBox.Show("Settings could not be loaded properly from the server", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }
        /// <summary>
        /// Loads all the general preferences set by the user
        /// </summary>
        public void LoadGeneralPreferences()
        {
            // copies values from global settings into our members
            m_loadingFlag        = Properties.Settings.Default.LoadingFlag;
            m_getPrefsFromServer = Properties.Settings.Default.GetPrefsFromServer;
            m_askSavingPrefs     = Properties.Settings.Default.AskSaving;
           
            // Default Form
            // If form name has the default combo message, just replace it with an empty string.
            m_defaultFormID = Properties.Settings.Default.DefaultFormID;
            m_defaultFormName = (
                CBVUtil.Eqstrs(Properties.Settings.Default.DefaultFormName, CBVConstants.DEFAULT_MESSAGE_FORM_SELECTION) // "Select a form"
                || CBVUtil.Eqstrs(Properties.Settings.Default.OpenMode, CBVConstants.OPEN_BLANK_FORM)) // "Blank form"
                ? string.Empty : Properties.Settings.Default.DefaultFormName;
            if (m_defaultFormName.EndsWith(".xml")) // shouldn't ever happen
                m_defaultFormName = m_defaultFormName.Remove(m_defaultFormName.Length - 4);
            m_defaultFormType = CBVUtil.Eqstrs(Properties.Settings.Default.DefaultFormType, CBVConstants.PUBLIC) // "Public"
                     ? ChemBioVizForm.formType.Public : ChemBioVizForm.formType.Private;

            // Last opened form
            m_lastOpenFormID = Properties.Settings.Default.LastOpenFormID;
            m_lastOpenFormName = Properties.Settings.Default.LastOpenFormName;
            m_lastOpenFormType = CBVUtil.Eqstrs(Properties.Settings.Default.LastOpenFormType, CBVConstants.PUBLIC)
                     ? ChemBioVizForm.formType.Public : ChemBioVizForm.formType.Private;

            m_navigationPane     = Properties.Settings.Default.NavigationPane;
            m_openMode           = Properties.Settings.Default.OpenMode;
            m_styleLibrary       = Properties.Settings.Default.StyleLibrary;
            m_infraSettings      = Properties.Settings.Default.InfraSettings;
            
            m_writeLog           = Properties.Settings.Default.WriteLog;
            m_logPath            = Properties.Settings.Default.LogPath;
            CBVUtil.Log = m_writeLog;
            CBVUtil.LogPath = m_logPath;

            m_pageSize           = Properties.Settings.Default.PageSize;
            CBVUtil.PageSize = (int)m_pageSize;
            m_treeConfig = Properties.Settings.Default.TreeConfig;
        }
        /// <summary>
        /// Loads all the preferences needed at searching time.
        /// </summary>
        public void LoadSearchPreferences()
        {
            #region DateCriteria
            
            #endregion
            
            #region FormulaCriteria
            
            #endregion

            #region MolWeightCriteria
            
            #endregion

            #region NumericalCriteria
            
            #endregion

            #region StructureCriteria

            // copy global settings into static search criteria instance
            SearchCriteria.StructureCriteria sCriteria = SearchOptions.SearchOptionsInstance.StructureCriteria;
            SearchOptionsSettings settings = SearchOptionsSettings.Default;

            // NOTE: this statement is different -- it modifies settings, not sCriteria
            settings.Substructure = !(settings.FullSearch || settings.Similar || settings.Exact);
            
            sCriteria.FullSearch = BoolToCOEBoolean(settings.FullSearch);
            sCriteria.Similar = BoolToCOEBoolean(settings.Similar);
            sCriteria.SimThreshold = settings.SimThreshold;
            sCriteria.Identity = BoolToCOEBoolean(settings.Exact);

            sCriteria.Tautometer/*misspelled in COE*/ = BoolToCOEBoolean(settings.Tautomer);
            sCriteria.Implementation = "cscartridge";

            sCriteria.PermitExtraneousFragments =
            sCriteria.PermitExtraneousFragmentsIfRXN = BoolToCOEBoolean(settings.PermitExtraneousFragments);
            sCriteria.FragmentsOverlap = BoolToCOEBoolean(settings.FragmentsOverlap);

            sCriteria.HitAnyChargeHetero = BoolToCOEBoolean(settings.HitAnyChargeHetero);
            sCriteria.HitAnyChargeCarbon = BoolToCOEBoolean(settings.HitAnyChargeCarbon);
            sCriteria.ReactionCenter = BoolToCOEBoolean(settings.ReactionCenter);
     //     sCriteria.IgnoreImplicitHydrogens = BoolToCOEBoolean(settings.IgnoreImplicitH);  DEPRECATED (CSBR-110210)

            SearchOptions.SearchOptionsInstance.MatchStereochemistry = settings.MatchStereochemistry;   // bool
            sCriteria.TetrahedralStereo = GetTetrahedralSetting(settings);                  // same, either, any

            sCriteria.RelativeTetStereo = SearchCriteria.COEBoolean.No;
            sCriteria.DoubleBondStereo = BoolToCOEBoolean(settings.SameDoubleBondStereo);
            sCriteria.AbsoluteHitsRel = BoolToCOEBoolean(settings.AbsoluteHitsRel);

            #endregion

            #region TextCriteria
            
            #endregion
        }
        /// <summary>
        /// Gets the right SearchCriteria.TetrahedralStereoMatching option to be stored in the general SearchOptions instance
        /// </summary>
        /// <param name="tetrahedralSetting"></param>
        /// <returns></returns>
        private SearchCriteria.TetrahedralStereoMatching GetTetrahedralSetting(SearchOptionsSettings settings)
        {
            return 
                settings.SameTetStereo? SearchCriteria.TetrahedralStereoMatching.Same :
                settings.EitherTetStereo?  SearchCriteria.TetrahedralStereoMatching.Either :
                SearchCriteria.TetrahedralStereoMatching.Any;
        }

        public void BackupSettings()
        {
            // stash a copy of settings in case of cancel
            Properties.Settings.Default.Save();
            SearchOptionsSettings.Default.Save();
        }
        public void RestoreBackupSettings()
        {
            // cancel changes, go back to previous
            if (File.Exists(Path.Combine(m_customProvider.GetAppSettingsPath(), m_customProvider.GetAppSettingsFilename())))
            {
                Properties.Settings.Default.Reload();
                SearchOptionsSettings.Default.Reload();
            }
        }
        /// <summary>
        ///   Save user customizations made on Infragistics controls
        /// </summary>
        public void SaveUserCustomization(Stream settingsStream)
        {
            Properties.Settings.Default.InfraSettings = CBVUtil.StreamToString(settingsStream);
        }
        /// <summary>
        /// Saves General and Search Preferences
        /// </summary>
        public void SaveAllPreferences()
        {
            //This flag helps to notice when the settings couldn't get loaded due to XML internal problems;
            // in that case it will always takes its default value equals to zero
            Properties.Settings.Default.LoadingFlag = 1;
            // If form name has the default combo message, just replace it with an empty string.
            if (String.Equals(Properties.Settings.Default.DefaultFormName, CBVConstants.DEFAULT_MESSAGE_FORM_SELECTION, StringComparison.InvariantCultureIgnoreCase))
                Properties.Settings.Default.DefaultFormName = string.Empty;

            Properties.Settings.Default.Save();
            SearchOptionsSettings.Default.Save();
        }
        /// <summary>
        /// Stores the user's Settings file on the server
        /// </summary>
        public bool StoreSettingsOnServer() 
        {
            bool ret = true;
            bool overwrite = true;
            if (m_sBank != null)
            {
                string xSettings = this.GetSettingsFileAsString();
                //Evaluates if the user did made a change
                if (!m_previousSettings.Equals(xSettings, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        if (m_sBank.HasName(m_settingsObjName))
                        {
                            if (m_askSavingPrefs)
                            {
                                // display message if file exists on the server
                                DialogResult userOption = MessageBox.Show(new StringBuilder(
                                    "Your settings differ from those on the server. Do you want to update the server settings?").AppendLine().AppendLine(
                                    "[Click YES to update; NO to save to the local machine; CANCEL to discard changes]").ToString(),
                                    "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                                if (userOption == DialogResult.Yes)
                                {
                                    try
                                    {
                                        m_sBank.Delete(m_settingsObjName);
                                    }
                                    catch (ObjectBankException)
                                    {
                                        throw;  // This throw the same caught exception to the upper level
                                    }
                                }
                                else if (userOption == DialogResult.No)
                                {
                                    //Save the settings locally
                                    overwrite = false;
                                    this.SaveSettingsFile(xSettings);
                                }
                                else
                                {
                                    //Discard any changes
                                    overwrite = false;
                                    ret = false;    // return false when cancelled
                                }
                            }
                        }
                        if (overwrite)
                        {
                            try
                            {
                                m_sBank.Store(m_settingsObjName, GetSettingsFileAsString());
                            }
                            catch (ObjectBankException ex)
                            {
                                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //If the settings could not be saved, the app'll use the default values
                        MessageBox.Show(new StringBuilder(
                            "There was an error. The application settings couldn't be saved on the server").AppendLine().AppendLine(ex.Message).ToString());
                    }
                }
            }
            return ret;
        }
        /// <summary>
        /// Retrieves user's Settings file from the server and saves it on hard disk as a .settings file
        /// </summary>
        public void RetrieveSettingsFromServer()
        {
            string xSettings = string.Empty;
            m_sBank = new SettingsObjectBank(m_username);
            m_settingsObjName = string.Concat(m_username, "Settings");
            try
            {
                xSettings = m_sBank.Retrieve(m_settingsObjName);
                if (File.Exists(Path.Combine(m_appSettingsPath, m_appSettingsFileName)) && xSettings != null)
                {
                    File.Delete(Path.Combine(m_appSettingsPath, m_appSettingsFileName));
                    this.SaveSettingsFile(xSettings);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(new StringBuilder(
                    "There was an error while retrieving the settings from the server").AppendLine().AppendLine(ex.StackTrace).ToString());
            }
        }
        /// <summary>
        /// Saves Settings file in the local machine
        /// </summary>
        /// <param name="xSettings"></param>
        private void SaveSettingsFile(string xSettings) 
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xSettings);
            try
            {
                FileStream fStream = new FileStream(Path.Combine(m_appSettingsPath, m_appSettingsFileName), 
                    FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                xDoc.Save(fStream);
                fStream.Close();
            }
            catch (Exception ex)
            {
                //If the settings could not be loaded, the app'll use the default values
                MessageBox.Show(new StringBuilder(
                    "There was an error. The application settings couldn't be retrieved from the server").AppendLine().AppendLine(ex.StackTrace).ToString());
            }
            FormDBLib.SecurityUtils.GrantAccess(Path.Combine(m_appSettingsPath, m_appSettingsFileName));
        }
        /// <summary>
        /// Saves a copy of the local settings on disk just.
        /// </summary>
        public void BackupLocalSettings() 
        {
            if (File.Exists(Path.Combine(m_appSettingsPath, m_appSettingsFileName)))
            {
                if (!File.Exists(Path.Combine(m_appSettingsPath, string.Concat(CBVConstants.SETTINGS_FILE_PREFIX_LOCAL, m_appSettingsFileName)).ToString()))
                {
                    File.Move(Path.Combine(m_appSettingsPath, m_appSettingsFileName).ToString(),
                        Path.Combine(m_appSettingsPath, string.Concat(CBVConstants.SETTINGS_FILE_PREFIX_LOCAL, m_appSettingsFileName)).ToString());
                }
            }
        }
        /// <summary>
        /// Restores the local settings file copy if exists. 
        /// </summary>
        public bool RestoreLocalSettings()
        {
            bool restored = false;
            if (File.Exists(Path.Combine(m_appSettingsPath, string.Concat(CBVConstants.SETTINGS_FILE_PREFIX_LOCAL, m_appSettingsFileName))))
            {
                //Delete the version brought from the server
                File.Delete(Path.Combine(m_appSettingsPath, m_appSettingsFileName));
                //Rename the local version
                File.Move(Path.Combine(m_appSettingsPath, string.Concat(CBVConstants.SETTINGS_FILE_PREFIX_LOCAL, m_appSettingsFileName)).ToString(), Path.Combine(m_appSettingsPath, m_appSettingsFileName).ToString());

                restored = true;
            }
            return restored;
        }
        /// <summary>
        /// Configure the Custom Provider to start reading the new properties
        /// </summary>
        public void ReloadNewFileVersion()
        {
            ((CustomSettingsProvider)Properties.Settings.Default.Providers[CBVConstants.PORTABLE_SETTINGS_PROVIDER]).
                        UseNewVersion = true;
            if (File.Exists(Path.Combine(m_appSettingsPath, m_appSettingsFileName)))
            {
                Properties.Settings.Default.Reload();
                SearchOptionsSettings.Default.Reload();
            }
        }
        /// <summary>
        /// Gets the settings file into a string
        /// </summary>
        /// <returns></returns>
        private string GetSettingsFileAsString()
        {
            String resultXML = String.Empty;

            XmlDocument settingsXML = new XmlDocument();
            if (File.Exists(Path.Combine(m_customProvider.GetAppSettingsPath(), m_customProvider.GetAppSettingsFilename())))
            {
                settingsXML.Load(Path.Combine(m_appSettingsPath, m_appSettingsFileName));
                resultXML = settingsXML.OuterXml;
            }

            return resultXML;
        }

        /// <summary>
        /// Saves all the preferences and keeps them in Search Options and Preferences Helper instances
        /// </summary>
        public void ApplyAllSettings()
        {
            this.SaveAllPreferences();
            this.LoadGeneralPreferences();
            this.LoadSearchPreferences();
        }
        /// <summary>
        /// Resets all settings to their default values
        /// </summary>
        public void ResetAllSettingsToDefault()
        {
            SetLocalSettingsProvider();
            Properties.Settings.Default.Reset();
            SearchOptionsSettings.Default.Reset();
            this.LoadGeneralPreferences();
            this.LoadSearchPreferences();
        }
        /// <summary>
        /// Resets general settings to their default values
        /// </summary>
        public void ResetGeneralSettingsToDefault() 
        {
            SetLocalSettingsProvider();
            Properties.Settings.Default.Reset();
            this.LoadGeneralPreferences();
        }
        /// <summary>
        /// Resets search settings to their default values
        /// </summary>
        public void ResetSearchSettingsToDefault()
        {
            SetLocalSettingsProvider();
            SearchOptionsSettings.Default.Reset();
            this.LoadSearchPreferences();
        }
        /// <summary>
        /// Translates a COEBoolean value to its bool counterpart
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool COEBooleanToBool(SearchCriteria.COEBoolean value)
        {
            return value.ToString().Equals(CBVConstants.SEARCHCRITERIA_COEBOOLEAN_YES) ? true : false;
        }
        /// <summary>
        /// Translates a bool value to its COEBoolean counterpart
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SearchCriteria.COEBoolean BoolToCOEBoolean(bool value)
        {
            return value ? SearchCriteria.COEBoolean.Yes : SearchCriteria.COEBoolean.No;
        }
        /// <summary>
        /// Stores the last opened form name in the settings file
        /// </summary>
        /// <param name="formName"></param>
        public void StoreLastOpenedForm(int formID, string formName, ChemBioVizForm.formType ftype)
        {
            if (!String.IsNullOrEmpty(formName) && !formName.Equals("Untitled"))
            {
                Properties.Settings.Default.LastOpenFormID = formID;
                Properties.Settings.Default.LastOpenFormName = formName;
                Properties.Settings.Default.LastOpenFormType = ChemBioVizForm.GetFormTypeName(ftype);
            }
        }
        /// <summary>
        ///  Store default form
        /// </summary>
        /// <param name="formID"></param>
        /// <param name="formName"></param>
        /// <param name="ftype"></param>
        public void StoreDefaultForm(int formID, string formName, ChemBioVizForm.formType ftype)
        {
            if (!String.IsNullOrEmpty(formName))
            {
                Properties.Settings.Default.DefaultFormID = formID;
                Properties.Settings.Default.DefaultFormName = formName;
                Properties.Settings.Default.DefaultFormType= ChemBioVizForm.GetFormTypeName(ftype);
            }
        }
        public void LoadStyle(String styleName)
        {
            if (m_stylesBank == null)
                m_stylesBank = new StylesObjectBank();

            // if style has been saved locally, load from there
            // if not, retrieve from bank, save to local file, then load
            String localStylesFolderPath = PreferencesHelper.GetStylesDir();
            CBVUtil.FindOrCreateDir(localStylesFolderPath);
            if (!string.IsNullOrEmpty(styleName) && !CBVUtil.Eqstrs(styleName, CBVConstants.STYLE_NOT_SET))
            {
                String styleFilePath = Path.Combine(localStylesFolderPath, styleName);
                if (!styleFilePath.EndsWith(".isl")) styleFilePath += ".isl";
                if (!File.Exists(styleFilePath))
                {
                    // retrieve from styles bank and write to local file
                    String xml = this.m_stylesBank.Retrieve(styleName);
                    if (String.IsNullOrEmpty(xml))
                    {
                        MessageBox.Show(String.Concat("Unable to load style '", styleName, "'.\nChoose a different style in Preferences."));
                        return;
                    }
                    CBVUtil.StringToFile(xml, styleFilePath);
                }
                Infragistics.Win.AppStyling.StyleManager.Load(styleFilePath);
            }
            else
            {
                Infragistics.Win.AppStyling.StyleManager.Reset();   // CSBR-133977
            }
        }
        #endregion
    }
}
