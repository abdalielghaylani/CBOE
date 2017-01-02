using System.Xml;
using System.Collections.Generic;
using System;

namespace CambridgeSoft.COE.Patcher.Utilities
{
    /// <summary>
    /// The utility class that defines some commonly used members by many Patcher classes.
    /// </summary>
    public static class PatcherUtility
    {
        public const string NAMESPACE_URI = "COE.FormGroup";

        private static XmlNamespaceManager _manager = null;

        /// <summary>
        /// Serves as the sole holder of the XmlNamespaceManager corresponding to currently running code.
        /// </summary>
        public static XmlNamespaceManager XmlNamespaceManager
        {
            get 
            {
                if (string.IsNullOrEmpty(_manager.DefaultNamespace))
                    _manager.AddNamespace(string.Empty, NAMESPACE_URI);

                if (!_manager.HasNamespace("COE"))
                    _manager.AddNamespace("COE", NAMESPACE_URI);

                return _manager; 
            }
        }

        /// <summary>
        /// Sets the current XmlNamespaceManager
        /// </summary>
        /// <param name="doc">The COEFormGroup container</param>
        public static void SetXmlNamespaceManager(XmlDocument doc)
        {
            _manager = new XmlNamespaceManager(doc.NameTable);
            _manager.AddNamespace("COE", NAMESPACE_URI);
        }

        /// <summary>
        /// Returns the coeFormGroup document by its Id attribute.
        /// </summary>
        /// <param name="coeForms">The collection of all coeFormGroups</param>
        /// <param name="coeFormId">The Id of the required coeFormGroup</param>
        /// <returns>The coeFormGroup document with the specified Id value</returns>
        public static XmlDocument GetCoeFormGroupById(List<XmlDocument> coeFormGroups, string coeFormGroupId)
        {
            foreach (XmlDocument coeForm in coeFormGroups)
            {
                string id = coeForm.DocumentElement.Attributes["id"] == null ? 
                    string.Empty : coeForm.DocumentElement.Attributes["id"].Value;

                if (id == coeFormGroupId)
                {
                    return coeForm;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the COEDataView document by its Id attribute.
        /// </summary>
        /// <param name="coeForms">The collection of all COEDataViews</param>
        /// <param name="coeFormId">The Id of the required COEDataView</param>
        /// <returns>The COEDataView document with the specified Id value</returns>
        public static XmlDocument GetCOEDataViewById(List<XmlDocument> coeDataViews, string coeDataViewId)
        {
            foreach (XmlDocument coeDataView in coeDataViews)
            {
                string id = coeDataView.DocumentElement.Attributes["dataviewid"] == null ?
                    string.Empty : coeDataView.DocumentElement.Attributes["dataviewid"].Value;

                if (id == coeDataViewId)
                {
                    return coeDataView;
                }
            }

            return null;
        }

        /// <summary>
        /// Updates the title node of the specified coeForm to the new value.
        /// </summary>
        /// <param name="coeForm">The coeForm node to change title value for</param>
        /// <param name="newTitle">New title value</param>
        public static void ChangeCOEFormTitle(XmlNode coeForm, string newTitle)
        {
            XmlNode titleNode = coeForm.SelectSingleNode("./COE:title", XmlNamespaceManager);
            if(titleNode != null)
                titleNode.InnerXml = newTitle;
        }

        /// <summary>
        /// Removes the formElement node from the specified parent node
        /// </summary>
        /// <param name="parentNode">The parent node containing the formElement</param>
        /// <param name="formElementName">The name of the form element to remove</param>
        public static void RemoveFormElement(XmlNode parentNode, string formElementName)
        {
            if (parentNode != null)
            {
                XmlNode formElementToRemove = parentNode.SelectSingleNode(
                    string.Format("./COE:formElement[@name='{0}']", formElementName),
                    PatcherUtility.XmlNamespaceManager);

                if(formElementToRemove != null)
                    parentNode.RemoveChild(formElementToRemove);
            }
        }

        /// <summary>
        /// Inserts the specified form element after the specified reference node.
        /// </summary>
        /// <param name="root">The root element used to create the new form element</param>
        /// <param name="formElementName">The name of the new form element</param>
        /// <param name="formElementInnerXml">The inner xml of the new form element</param>
        /// <param name="refChild">The reference child node</param>
        public static void InsertFormElementAfter(XmlDocument root, XmlNode parent, string formElementName, string formElementInnerXml, XmlNode refChild)
        {
            if (parent == null && refChild == null)
                throw new ArgumentNullException("parent and refChild", "parent and refChild can't be both null");

            XmlNode newFormElementNode = root.CreateElement("formElement", NAMESPACE_URI);
            XmlAttribute nameAttr = root.CreateAttribute("name");
            nameAttr.Value = formElementName;
            newFormElementNode.Attributes.Append(nameAttr);

            XmlNode parentNode = parent == null ? refChild.ParentNode : parent;
                
            if (parentNode.SelectSingleNode("./COE:formElement[@name='" + formElementName + "']", XmlNamespaceManager) == null)
            {
                if (refChild == null)
                    parentNode.AppendChild(newFormElementNode);
                else
                    parentNode.InsertAfter(newFormElementNode, refChild);

                newFormElementNode.InnerXml = formElementInnerXml;
            }
        }

        /// <summary>
        /// Prepend the specified form element as the first child of the specified parent node.
        /// </summary>
        /// <param name="root">The root element used to create the new form element</param>
        /// <param name="formElementName">The name of the new form element</param>
        /// <param name="formElementInnerXml">The inner xml of the new form element</param>
        /// <param name="parentNode">The parent node of the new node to prepend</param>
        public static void PrependFormElement(XmlDocument root, string formElementName, string formElementInnerXml, XmlNode parentNode)
        {
            XmlNode newFormElementNode = root.CreateElement("formElement", NAMESPACE_URI);
            XmlAttribute nameAttr = root.CreateAttribute("name");
            nameAttr.Value = formElementName;
            newFormElementNode.Attributes.Append(nameAttr);

            parentNode.PrependChild(newFormElementNode);

            newFormElementNode.InnerXml = formElementInnerXml;
        }
    }
}
