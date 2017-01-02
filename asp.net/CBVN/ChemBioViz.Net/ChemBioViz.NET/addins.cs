using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Xml;
using System.Reflection;
using System.IO;

using FormDBLib;
using CBVUtilities;
using SharedLib;

namespace ChemBioViz.NET
{
    #region CBVAddin
    public class CBVAddin : ICBVAddin
    {
        #region Variables
        protected Type m_type;
        protected Object m_addinObj;
        protected IAddinMenu m_menu;
        protected ChemBioVizForm m_form;
        #endregion

        #region Constructor
        //---------------------------------------------------------------------
        public CBVAddin(ChemBioVizForm form, String assyName, String typeName)
        {
            m_form = form;
            m_type = null;
            m_addinObj = null;
            m_menu = null;
            try
            {
                Assembly a = Assembly.Load(assyName);
                Type t = (a == null) ? null : a.GetType(typeName);
                if (t != null && !t.IsAbstract && t.GetMethod("Init") != null)
                {
                    m_type = t;
                    m_addinObj = t.Assembly.CreateInstance(t.ToString());
                    this.Init(m_form);
                    m_menu = this.GetMenu();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error loading type={0} assy={1}\n{2}", typeName, assyName, ex.Message));
            }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Properties
        //---------------------------------------------------------------------
        public ChemBioVizForm Form
        {
            get { return m_form; }
        }
        //---------------------------------------------------------------------
        public Type Type
        {
            get { return m_type; }
        }
        //---------------------------------------------------------------------
        public Object AddinObj
        {
            get { return m_addinObj; }
        }
        //---------------------------------------------------------------------
        public IAddinMenu Menu
        {
            get { return m_menu; }
        }
        //---------------------------------------------------------------------
        #endregion

        #region ICBVAddin Implementation
        //---------------------------------------------------------------------
        public void Init(object form)                   { Invoke("Init", form); }
        public void Execute()                           { Invoke("Execute"); }    
        public void ExecuteWithString(string s)         { Invoke("ExecuteWithString", s); }
        public void Deinit()                            { Invoke("Deinit"); }
        public string GetDescription()                  { return Invoke("GetDescription").ToString(); }
        public IAddinMenu GetMenu()                     { return Invoke("GetMenu") as IAddinMenu; }
        public string GetMenuImagePath()                { return Invoke("GetMenuImagePath").ToString(); }
        public bool HandleMenuCommand(string menuCmd)   { return (bool)Invoke("HandleMenuCommand", menuCmd); }
        public bool IsEnabled(string menuCmd)           { return (bool)Invoke("IsEnabled", menuCmd); }
        public bool IsChecked(string menuCmd)           { return (bool)Invoke("IsChecked", menuCmd); }
        public string GetSettings()                     
        {
            object settings = Invoke("GetSettings");
            return settings != null ? settings.ToString() : string.Empty; 
        }
        public void SetSettings(string s)               { Invoke("SetSettings", s); }    
        //---------------------------------------------------------------------
        #endregion

        #region Methods
        //---------------------------------------------------------------------
        public object Invoke(String methodName)
        {
            return Invoke(methodName, null, null);
        }
        //---------------------------------------------------------------------
        public object Invoke(String methodName, Object arg0)
        {
            return Invoke(methodName, arg0, null);
        }
        //---------------------------------------------------------------------
        public object Invoke(String methodName, Object arg0, Object arg1)
        {
            object ans = null;
            if (m_type != null && m_addinObj != null)
            {
                MethodInfo mi = m_type.GetMethod(methodName);
                if (mi == null)
                {
                    Debug.WriteLine(String.Format("Method not found: {0}", methodName));
                }
                else
                {
                    int nParams = mi.GetParameters().Length;
                    Object[] oparams = new Object[nParams];
                    if (nParams > 0) oparams[0] = arg0;
                    if (nParams > 1) oparams[1] = arg1;
                    try
                    {
                        ans = mi.Invoke(m_addinObj, oparams);
                    }
                    catch (AddinException ex)
                    {
                        Debug.WriteLine(String.Format("Error invoking method={0} type={1}: {2}", methodName, m_type.ToString(), ex.Message));
                        throw ex;
                    }
                }
            }
            return ans;
        }
        //---------------------------------------------------------------------
        public void EnableMenuCommands()
        {
            if (this.Menu != null && this.Form != null)
            {
                foreach (IAddinMenuItem item in Menu.Items)
                {
                    if (item.Separator) continue;
                    Form.EnableMenuItem(CBVConstants.TOOLBAR_MAIN, Menu.Title, item.Command, this.IsEnabled(item.Command));
                    Form.CheckMenuItem(CBVConstants.TOOLBAR_MAIN, Menu.Title, item.Command, this.IsChecked(item.Command));
                }
            }
        }
        //---------------------------------------------------------------------
        public void CreateMenu()
        {
            if (this.Menu != null && this.Form != null)
                Form.CreateAddinMenu(Menu);
        }
        //---------------------------------------------------------------------
        public void UpdateMenuImage()
        {
            if (this.Menu != null && this.Form != null)
            {
                String sImagePath = this.GetMenuImagePath();
                Image image = String.IsNullOrEmpty(sImagePath) ? null : Image.FromFile(sImagePath);
                Form.UpdateMenuTitle(CBVConstants.TOOLBAR_MAIN, Menu.Title, Menu.Title, image);
            }
       }
        //---------------------------------------------------------------------
        #endregion
    }
    #endregion

    #region CBVAddinsManager
    public class CBVAddinsManager
    {
        #region Variables
        private List<CBVAddin> m_addins;
        private ChemBioVizForm m_form;
        #endregion

        #region Constructor
        //---------------------------------------------------------------------
        public CBVAddinsManager(ChemBioVizForm form)
        {
            m_form = form;
            m_addins = null;
        }
        //---------------------------------------------------------------------
        #endregion

        #region Properties
        //---------------------------------------------------------------------
        public List<CBVAddin> Addins
        {
            get { return m_addins; }
        }
        //---------------------------------------------------------------------
        public ChemBioVizForm Form
        {
            get { return m_form; }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Methods
        //---------------------------------------------------------------------
        public void Load()
        {
            m_addins = new List<CBVAddin>();

            int maxAddins = 20;
            for (int i = 1; i < maxAddins; ++i)
            {
                String akey = String.Format("Addin{0}", i);
                String fullkey = ConfigurationManager.AppSettings.Get(akey);
                if (String.IsNullOrEmpty(fullkey))
                    break;
                String typeName = CBVUtil.BeforeFirstDelimiter(fullkey, ',');
                String assyName = CBVUtil.AfterFirstDelimiter(fullkey, ',');

                CBVAddin addin = new CBVAddin(Form, assyName, typeName);
                m_addins.Add(addin);
            }
        }
        //---------------------------------------------------------------------
        public CBVAddin FindByMenuTitle(String title)
        {
            foreach (CBVAddin addin in Addins)
                if (addin.Menu != null && addin.Menu.Title.Equals(title))
                    return addin;
            return null;
        }
        //---------------------------------------------------------------------
        public CBVAddin FindByTypeName(String typeName)
        {
            foreach (CBVAddin addin in Addins)
                if (addin.Type != null && addin.Type.Name.Equals(typeName))
                    return addin;
            return null;
        }
        //---------------------------------------------------------------------
        public void CreateMenus()
        {
            foreach (CBVAddin addin in Addins)
                addin.CreateMenu();
        }
        //---------------------------------------------------------------------
        public void EnableMenuCommands()
        {
            foreach (CBVAddin addin in Addins)
                addin.EnableMenuCommands();
        }
        //---------------------------------------------------------------------
        public String GetAddinsXml()
        {
            // return string of xml (no header) with settings for all addins; to be stored in prefs before saving
            // called from ConfigureSettingsOnClosing
            XmlDocument xdoc = new XmlDocument();
            XmlElement root = xdoc.CreateElement("addins");
            XmlNode rootNode = xdoc.AppendChild(root);

            foreach (CBVAddin addin in Addins)
            {
                String sSettings = addin.GetSettings();
                if (!String.IsNullOrEmpty(sSettings))
                {
                    XmlElement xmlAddin = xdoc.CreateElement(addin.Type.Name);
                    xmlAddin.InnerText = sSettings;
                    rootNode.AppendChild(xmlAddin);
                }
            }
            String s = CBVUtil.XmlDocToString(xdoc);
            String sReduced = CBVUtil.RemoveDocHeader16(s);
            return sReduced;
        }
        //---------------------------------------------------------------------
        public void SetAddinsXml(String sXml)
        {
            // given string of xml stored in prefs, set props of all addins
            // called from InitForm

            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(sXml);
            XmlNode root = xdoc.DocumentElement;
            if (root != null && CBVUtil.Eqstrs(root.Name, "addins"))
            {
                foreach (XmlNode node in root.ChildNodes)
                {
                    String typeName = node.Name;
                    CBVAddin addin = FindByTypeName(typeName);
                    if (addin != null)
                        addin.SetSettings(node.InnerText);
                }
            }
        }
        //---------------------------------------------------------------------
        #endregion
    }
    #endregion
}
