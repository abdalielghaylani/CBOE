using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration.Provider;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Specialized;
using Microsoft.Win32;
using System.Xml;
using System.IO;
using System.Configuration;

using FormDBLib;
using ChemBioViz.NET.Exceptions;

namespace ChemBioViz.NET
{
    public class CustomSettingsProvider : SettingsProvider
    {
        #region Variables
        private XmlDocument m_settingsXML = null;
        private bool m_useNewVersion = false;
        #endregion

        #region Properties
        public bool UseNewVersion
        {
            get { return m_useNewVersion; }
            set { m_useNewVersion = value; }
        }
        //---------------------------------------------------------------------
        public override string ApplicationName
        {
            get
            {
                if (Application.ProductName.Trim().Length > 0)
                {
                    return Application.ProductName;
                }
                else
                {
                    FileInfo info = new FileInfo(Application.ExecutablePath);
                    String s = info.Name.Substring(0, info.Name.Length - info.Extension.Length);
                    return s;
                }
            }
            //Even when this setter is empty is necessary to be added here due to inheritance purposes
            set { }
        }
        //---------------------------------------------------------------------
        public override string Name
        {
            get { return CBVConstants.PORTABLE_SETTINGS_PROVIDER; }
        }
        //---------------------------------------------------------------------
        private XmlDocument SettingsXML
        {
            get
            {
                //If we dont hold an xml document, try opening one.  
                //If it doesnt exist then create a new one ready.
                if (m_settingsXML == null)
                {
                    m_settingsXML = new XmlDocument();

                    try
                    {
                        if (File.Exists(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename())))
                        {
                            m_settingsXML.Load(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename()));
                        }
                        else
                        {
                            //Create new document
                            XmlDeclaration dec = m_settingsXML.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
                            m_settingsXML.AppendChild(dec);

                            XmlNode nodeRoot = m_settingsXML.CreateNode(XmlNodeType.Element, CBVConstants.SETTINGS_XML_ROOT, string.Empty);
                            m_settingsXML.AppendChild(nodeRoot);

                            XmlNode general = m_settingsXML.CreateNode(XmlNodeType.Element, CBVConstants.SETTINGS_XML_NODE_GENERAL, string.Empty);
                            nodeRoot.AppendChild(general);

                            XmlNode searchOptions = m_settingsXML.CreateNode(XmlNodeType.Element, CBVConstants.SETTINGS_XML_NODE_SEARCH, string.Empty);
                            nodeRoot.AppendChild(searchOptions);
                        }
                    }
                    catch (FileLoadException ex)
                    {
                        throw new CustomSettingsProviderException(CBVConstants.SETTINGS_FILE_LOADING_ERROR, ex);
                    }
                }
                else if (m_settingsXML != null && m_useNewVersion == true)
                {
                    //Load file again with the available server version
                    if (File.Exists(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename())))
                    {
                        m_settingsXML.Load(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename()));
                        m_useNewVersion = false;
                    }
                }

                return m_settingsXML;
            }
        }
        #endregion

        #region Methods
        public override void Initialize(string name, NameValueCollection collection)
        {
            base.Initialize(this.ApplicationName, collection);
        }
        //---------------------------------------------------------------------
        public virtual string GetAppSettingsPath()
        {
            // Determine where to store the settings
            // The right path is  C:\Documents and Settings\All Users\Application Data\CambridgeSoft\ChemBioViz.NET\ + FILE_VERSION
            FileInfo info = new System.IO.FileInfo(string.Concat(Application.CommonAppDataPath, "\\"));
            return info.DirectoryName;
        }
        //---------------------------------------------------------------------
        public virtual string GetAppSettingsFilename()
        {
            //Determine the filename to store the settings
            return new StringBuilder(ApplicationName).Append(CBVConstants.SETTINGS_FILE_EXTENSION).ToString();
        }
        //---------------------------------------------------------------------
        public static string GetSafeMachineName()
        {
            // make machine name suitable for use as xml element: prepend underscore if starts with digit
            string envName = Environment.MachineName;
            if (envName.Length > 0 && Char.IsDigit(envName[0]))
                envName = String.Concat("_", envName);
            return envName;
        }
        //---------------------------------------------------------------------
        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection propValues)
        {
            //Iterate through the settings to be stored
            //Only dirty settings are included in propvals, and only ones relevant to this provider
            foreach (SettingsPropertyValue value in propValues)
            {
                SetValue(value, context);
            }

            try
            {
                SettingsXML.Save(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename()));
            }
            catch (Exception ex)
            {
                throw new CustomSettingsProviderException(CBVConstants.SETTINGS_FILE_SAVING_ERROR, ex);
            }
        }
        //---------------------------------------------------------------------
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection props)
        {
            //Create new collection of values
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

            //Iterate through the settings to be retrieved
            foreach (SettingsProperty setting in props)
            {
                SettingsPropertyValue value = new SettingsPropertyValue(setting);
                value.IsDirty = false; //false because the value hasn't changed
                value.SerializedValue = GetValue(setting, context);
                values.Add(value);
            }
            return values;
        }
        //---------------------------------------------------------------------
        private string GetValue(SettingsProperty setting, SettingsContext context)
        {
            string result = string.Empty;
            try
            {
                XmlNode settingNode = null;
                if (IsRoaming(setting))
                {
                    if (context["GroupName"].ToString().Equals(CBVConstants.SETTINGS_BASE_NAME_GENERAL, StringComparison.OrdinalIgnoreCase))
                    {
                        settingNode = SettingsXML.SelectSingleNode(
                            CBVConstants.SETTINGS_XML_ROOT + "/" + CBVConstants.SETTINGS_XML_NODE_GENERAL + "/" + setting.Name);
                        result = this.GetXmlNodeValue(settingNode, setting);
                    }
                    else if (context["GroupName"].ToString().Equals(CBVConstants.SETTINGS_BASE_NAME_SEARCH, StringComparison.OrdinalIgnoreCase))
                    {
                        settingNode = SettingsXML.SelectSingleNode(
                            CBVConstants.SETTINGS_XML_ROOT + "/" + CBVConstants.SETTINGS_XML_NODE_SEARCH + "/" + setting.Name);
                        result = this.GetXmlNodeValue(settingNode, setting);
                    }
                    else
                    {
                        settingNode = SettingsXML.SelectSingleNode(CBVConstants.SETTINGS_XML_ROOT + "/" + setting.Name);
                        result = this.GetXmlNodeValue(settingNode, setting);
                    }
                }
                else
                {
                    settingNode = SettingsXML.SelectSingleNode(CBVConstants.SETTINGS_XML_ROOT + "/" + CustomSettingsProvider.GetSafeMachineName() + "/" + setting.Name);
                    result = this.GetXmlNodeValue(settingNode, setting);
                }
            }
            catch (Exception ex)
            {
                throw new CustomSettingsProviderException(CBVConstants.SETTINGS_FILE_CREATION_ERROR, ex);
            }
            return result;
        }
        //---------------------------------------------------------------------
        private string GetXmlNodeValue(XmlNode node, SettingsProperty setting) 
        {
            string result = string.Empty;
            if (node != null)
            {
                result = node.InnerText;
            }
            else
            {
                if ((setting.DefaultValue != null))
                {
                    result = setting.DefaultValue.ToString();
                }
                else
                {
                    result = String.Empty;
                }
            }
            return result;
        }
        //---------------------------------------------------------------------
        private XmlElement GetXmlElement(XmlNode node)
        {
            XmlElement result = null;
            if (node != null)
            {
                result = (XmlElement)node;
            }
            else
            {
                result = null;
            }
            return result;
        }
        //---------------------------------------------------------------------
        private void SetValue(SettingsPropertyValue propVal, SettingsContext context)
        {

            XmlElement machineNode = null;
            XmlElement settingNode = null;

            //Determine if the setting is roaming.
            //If roaming then the value is stored as an element under the root
            //Otherwise it is stored under a machine name node 
            try
            {
                XmlNode node = null;
                if (IsRoaming(propVal.Property))
                {
                    if (context["GroupName"].ToString().Equals(CBVConstants.SETTINGS_BASE_NAME_GENERAL, StringComparison.OrdinalIgnoreCase))
                    {
                        node = SettingsXML.SelectSingleNode(
                            CBVConstants.SETTINGS_XML_ROOT + "/" + CBVConstants.SETTINGS_XML_NODE_GENERAL + "/" + propVal.Name);
                        settingNode = this.GetXmlElement(node);
                    }
                    else if (context["GroupName"].ToString().Equals(CBVConstants.SETTINGS_BASE_NAME_SEARCH, StringComparison.OrdinalIgnoreCase))
                    {
                        node = SettingsXML.SelectSingleNode(
                            CBVConstants.SETTINGS_XML_ROOT + "/" + CBVConstants.SETTINGS_XML_NODE_SEARCH + "/" + propVal.Name);
                        settingNode = this.GetXmlElement(node);
                    }
                    else
                    {
                        node = SettingsXML.SelectSingleNode(CBVConstants.SETTINGS_XML_ROOT + "/" + propVal.Name);
                        settingNode = this.GetXmlElement(node);
                    }
                }
                else
                {
                    node = SettingsXML.SelectSingleNode(CBVConstants.SETTINGS_XML_ROOT + "/" + CustomSettingsProvider.GetSafeMachineName() + "/" + propVal.Name);
                    settingNode = this.GetXmlElement(node);
                }
            }
            catch (NullReferenceException ex)
            {
                throw new CustomSettingsProviderException(CBVConstants.SETTINGS_FILE_CREATION_ERROR, ex);
            }

            //Check if the node exists, if so then set its new value
            if ((settingNode != null))
            {
                if (CBVUtilities.CBVUtil.IsDocHeader16(propVal.SerializedValue.ToString()))
                    settingNode.InnerText = CBVUtilities.CBVUtil.ReplaceDocHeader16(propVal.SerializedValue.ToString());
                else
                    settingNode.InnerText = propVal.SerializedValue.ToString();
            }
            else
            {
                try
                {
                    if (IsRoaming(propVal.Property))
                    {
                        //Store the value as an element of the Settings Root Node
                        settingNode = SettingsXML.CreateElement(propVal.Name);
                        settingNode.InnerText = (propVal.SerializedValue == null) ? String.Empty :   // CSBR-128053
                                                settingNode.InnerText = propVal.SerializedValue.ToString();

                        if (context["GroupName"].ToString().Equals(CBVConstants.SETTINGS_BASE_NAME_GENERAL, StringComparison.OrdinalIgnoreCase))
                        {
                            SettingsXML.SelectSingleNode(
                                CBVConstants.SETTINGS_XML_ROOT + "/" + CBVConstants.SETTINGS_XML_NODE_GENERAL).AppendChild(settingNode);
                        }
                        else if (context["GroupName"].ToString().Equals(CBVConstants.SETTINGS_BASE_NAME_SEARCH, StringComparison.OrdinalIgnoreCase))
                        {
                            SettingsXML.SelectSingleNode(
                                CBVConstants.SETTINGS_XML_ROOT + "/" + CBVConstants.SETTINGS_XML_NODE_SEARCH).AppendChild(settingNode);
                        }
                        else
                            SettingsXML.SelectSingleNode(CBVConstants.SETTINGS_XML_ROOT).AppendChild(settingNode);
                    }
                    else
                    {
                        //Its machine specific, store as an element of the machine name node,
                        //creating a new machine name node if one doesnt exist.
                        XmlNode element = SettingsXML.SelectSingleNode(CBVConstants.SETTINGS_XML_ROOT + "/" + CustomSettingsProvider.GetSafeMachineName());
                        if (element != null)
                            machineNode = (XmlElement)SettingsXML.SelectSingleNode(CBVConstants.SETTINGS_XML_ROOT + "/" + CustomSettingsProvider.GetSafeMachineName());
                        else
                        {
                            machineNode = SettingsXML.CreateElement(CustomSettingsProvider.GetSafeMachineName());
                            SettingsXML.SelectSingleNode(CBVConstants.SETTINGS_XML_ROOT).AppendChild(machineNode);
                        }

                        settingNode = SettingsXML.CreateElement(propVal.Name);
                        settingNode.InnerText = propVal.SerializedValue.ToString();
                        machineNode.AppendChild(settingNode);
                    }
                }
                catch (NullReferenceException ex)
                {
                    throw new CustomSettingsProviderException(CBVConstants.SETTINGS_FILE_STRUCTURE_ERROR, ex);
                }
            }
        }
        //---------------------------------------------------------------------
        private bool IsRoaming(SettingsProperty property)
        {
            //Determine if the setting is marked as Roaming
            foreach (DictionaryEntry entry in property.Attributes)
            {
                Attribute atribute = (Attribute)entry.Value;
                if (atribute is SettingsManageabilityAttribute)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
