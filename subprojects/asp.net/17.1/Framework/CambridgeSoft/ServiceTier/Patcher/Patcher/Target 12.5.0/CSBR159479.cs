using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Hide repeted columns in search temp but dont delete them. 
    /// </summary>
    public class CSBR159479 : BugFixBaseCommand
    {


        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string _coeFormPath = string.Empty;



            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");

                if (id == "4002")
                {

                    XmlNode rootNodetbl = null;
                    XmlNode rootNodetblColumns = null;
                    XmlNode editNode = null;


                    #region Table_1 Update
                    _coeFormPath = "//COE:listForms[@defaultForm='0']/COE:listForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='']/COE:configInfo/COE:fieldConfig/COE:tables/COE:table[@name='Table_1']"; // Path to check the Rootnode before patcher update.
                    rootNodetbl = doc.SelectSingleNode(_coeFormPath, manager);
                    rootNodetblColumns = doc.SelectSingleNode(_coeFormPath, manager);
                    
                    #region  Header style - Column Style
                    editNode = rootNodetbl.SelectSingleNode("COE:headerStyle", manager);
                    if (editNode != null)
                        editNode.InnerText = "color: #000099; background-color: #CFD8E6; font-weight: bold; font-family: Verdana; font-size: 10px;border-collapse:collapse;";

                    editNode = rootNodetbl.SelectSingleNode("COE:columnStyle", manager);
                    if (editNode != null)
                        editNode.InnerText = "color: #000000; background-color: #FFFFFF; border-color: #CFD8E6; border-style:solid; border-width:1px; font-size:10px;font-family: Verdana;vertical-align:middle;text-align:center;";
                    #endregion

                    rootNodetblColumns = rootNodetbl.SelectSingleNode(_coeFormPath + "/COE:Columns", manager);

                    #region Marked
                    try
                    {
                        editNode = GetNode("COE:Column[@name='Marked']", rootNodetblColumns, manager);
                        if (editNode != null)
                            editNode.RemoveChild(GetNode("width", editNode, manager));
                    }
                    catch (Exception ex)
                    {messages.Add(ex.Message);}
                    #endregion

                    #region Structure
                    try
                    {
                        editNode = GetNode("COE:Column[@name='Structure']", rootNodetblColumns, manager);
                        GetNode("height", editNode, manager).InnerText = "100px";
                        GetNode("width", editNode, manager).InnerText = "150px";

                        editNode = GetNode("COE:formElement[@name='Structure']/COE:configInfo/COE:fieldConfig", editNode, manager);
                        GetNode("Height", editNode, manager).InnerText = "100px";
                        GetNode("Width", editNode, manager).InnerText = "150px";

                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                    #region Mol Wt
                    try
                    {
                        editNode = GetNode("COE:Column[@name='Mol Wt']", rootNodetblColumns, manager);
                        GetNode("width", editNode, manager).InnerText = "70px";
                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                    #region  Mol Formula
                    try
                    {
                        editNode = GetNode("COE:Column[@name='Mol Formula']", rootNodetblColumns, manager);
                        GetNode("width", editNode, manager).InnerText = "70px";
                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                    #region Review Record
                    try
                    {
                        editNode = GetNode("COE:Column[@name='Review Record']", rootNodetblColumns, manager);
                        GetNode("width", editNode, manager).InnerText = "150px";
                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                    #region SCIENTIST_ID
                    try
                    {
                        editNode = GetNode("COE:Column[@name='SCIENTIST_ID']", rootNodetblColumns, manager);
                        GetNode("width", editNode, manager).InnerText = "100px";

                        editNode = GetNode("COE:formElement[@name='SCIENTIST_ID']/COE:configInfo/COE:fieldConfig", editNode, manager);
                        editNode.AppendChild(editNode.OwnerDocument.CreateNode(XmlNodeType.Element,"Width","COE.FormGroup"));
                        editNode.SelectSingleNode("COE:Width", manager).InnerText = "100px";
                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                    #region CREATION_DATE
                    try
                    {
                        editNode = GetNode("COE:Column[@name='CREATION_DATE']", rootNodetblColumns, manager);
                        GetNode("width", editNode, manager).InnerText = "100px";

                        editNode = GetNode("COE:formElement[@name='CREATION_DATE']/COE:configInfo/COE:fieldConfig", editNode, manager);
                        editNode.RemoveChild(GetNode("Height", editNode, manager));
                        editNode.AppendChild(editNode.OwnerDocument.CreateNode(XmlNodeType.Element,"Width","COE.FormGroup"));
                        GetNode("Width", editNode, manager).InnerText = "100px";
                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                    #region NOTEBOOK_TEXT
                    try
                    {
                        editNode = GetNode("COE:Column[@name='NOTEBOOK_TEXT']", rootNodetblColumns, manager);
                        GetNode("width", editNode, manager).InnerText = "100px";

                        editNode = GetNode("COE:formElement[@name='NOTEBOOK_TEXT']/COE:configInfo/COE:fieldConfig", editNode, manager);
                        editNode.AppendChild(editNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Width", "COE.FormGroup"));
                        GetNode("Width", editNode, manager).InnerText = "100px";
                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                    #region AMOUNT
                    try
                    {
                        editNode = GetNode("COE:Column[@name='AMOUNT']", rootNodetblColumns, manager);
                        GetNode("width", editNode, manager).InnerText = "50px";

                        editNode = GetNode("COE:formElement[@name='AMOUNT']/COE:configInfo/COE:fieldConfig", editNode, manager);
                        editNode.AppendChild(editNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Width", "COE.FormGroup"));
                        GetNode("Width", editNode, manager).InnerText = "50px";
                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                    #region APPEARANCE
                    try
                    {
                        editNode = GetNode("COE:Column[@name='APPEARANCE']", rootNodetblColumns, manager);
                        GetNode("width", editNode, manager).InnerText = "100px";

                        editNode = GetNode("COE:formElement[@name='APPEARANCE']/COE:configInfo/COE:fieldConfig", editNode, manager);
                        editNode.AppendChild(editNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Width", "COE.FormGroup"));
                        GetNode("Width", editNode, manager).InnerText = "100px";
                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                    #region PURITY
                    try
                    {
                        editNode = GetNode("COE:Column[@name='PURITY']", rootNodetblColumns, manager);
                        GetNode("width", editNode, manager).InnerText = "60px";

                        editNode = GetNode("COE:formElement[@name='PURITY']/COE:configInfo/COE:fieldConfig", editNode, manager);
                        editNode.AppendChild(editNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Width", "COE.FormGroup"));
                        GetNode("Width", editNode, manager).InnerText = "60px";
                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                    #region PURITY_COMMENTS
                    try
                    {
                        editNode = GetNode("COE:Column[@name='PURITY_COMMENTS']", rootNodetblColumns, manager);
                        GetNode("width", editNode, manager).InnerText = "110px";

                        editNode = GetNode("COE:formElement[@name='PURITY_COMMENTS']/COE:configInfo/COE:fieldConfig", editNode, manager);
                        editNode.AppendChild(editNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Width", "COE.FormGroup"));
                        GetNode("Width", editNode, manager).InnerText = "110px";
                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                    #region SAMPLEID
                    try
                    {
                        editNode = GetNode("COE:Column[@name='SAMPLEID']", rootNodetblColumns, manager);
                        GetNode("width", editNode, manager).InnerText = "100px";

                        editNode = GetNode("COE:formElement[@name='SAMPLEID']/COE:configInfo/COE:fieldConfig", editNode, manager);
                        editNode.AppendChild(editNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Width", "COE.FormGroup"));
                        GetNode("Width", editNode, manager).InnerText = "100px";
                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                    #region SOLUBILITY
                    try
                    {
                        editNode = GetNode("COE:Column[@name='SOLUBILITY']", rootNodetblColumns, manager);
                        GetNode("width", editNode, manager).InnerText = "100px";

                        editNode = GetNode("COE:formElement[@name='SOLUBILITY']/COE:configInfo/COE:fieldConfig", editNode, manager);
                        editNode.AppendChild(editNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Width", "COE.FormGroup"));
                        GetNode("Width", editNode, manager).InnerText = "100px";
                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                    #region REMOVE RELATION FROM PARENT TABLE
                    try
                    {
                        editNode = GetNode("COE:Column[@name='TEMPBATCHID'][@childTableName='Table_2']", rootNodetblColumns, manager);
                        rootNodetblColumns.RemoveChild(editNode);
                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                    #region BATCH_FORMULA
                    try
                    {
                        editNode = GetNode("COE:Column[@name='BATCH_FORMULA']", rootNodetblColumns, manager);
                        GetNode("width", editNode, manager).InnerText = "100px";;
                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                    #endregion 

                    #region Table_2 Update
                    _coeFormPath = "//COE:listForms[@defaultForm='0']/COE:listForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='']/COE:configInfo/COE:fieldConfig/COE:tables/COE:table[@name='Table_2']"; // Path to check the Rootnode before patcher update.
                    rootNodetbl = doc.SelectSingleNode(_coeFormPath, manager);
                    rootNodetblColumns = doc.SelectSingleNode(_coeFormPath, manager);

                    #region  Header style - Column Style
                    editNode = rootNodetbl.SelectSingleNode("COE:headerStyle", manager);
                    if (editNode != null)
                        editNode.InnerText = "color: #000099; background-color: #CFD8E6; font-weight: bold; font-family: Verdana; font-size: 10px;border-collapse:collapse;";

                    editNode = rootNodetbl.SelectSingleNode("COE:columnStyle", manager);
                    if (editNode != null)
                        editNode.InnerText = "color: #000000; background-color: #FFFFFF; border-color: #CFD8E6; border-style:solid; border-width:1px; font-size:10px;font-family: Verdana;vertical-align:middle;text-align:center;";
                    #endregion

                    rootNodetblColumns = rootNodetbl.SelectSingleNode(_coeFormPath + "/COE:Columns", manager);

                    
                    #region COMPONENT STRUCTURE
                    try
                    {
                        editNode = GetNode("COE:Column[@name='COMPONENTSTRUCTURE']", rootNodetblColumns, manager);
                        GetNode("height", editNode, manager).InnerText = "140px";
                        GetNode("width", editNode, manager).InnerText = "140px";

                        editNode = GetNode("COE:formElement[@name='Component Structure']/COE:configInfo/COE:fieldConfig", editNode, manager);
                        GetNode("Height", editNode, manager).InnerText = "100px";
                        GetNode("Width", editNode, manager).InnerText = "150px";

                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                    #region FORMULA WEIGHT
                    try
                    {
                        editNode = GetNode("COE:Column[@name='FORMULAWEIGHT']", rootNodetblColumns, manager);
                        GetNode("width", editNode, manager).InnerText = "100px";
                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                    #region  MOLECULARFORMULA
                    try
                    {
                        editNode = GetNode("COE:Column[@name='MOLECULARFORMULA']", rootNodetblColumns, manager);
                        editNode.RemoveChild(GetNode("width", editNode, manager));
                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                    #region CMP_COMMENTS
                    try
                    {
                        editNode = GetNode("COE:Column[@name='CMP_COMMENTS']", rootNodetblColumns, manager);
                        GetNode("width", editNode, manager).InnerText = "250px";

                        editNode = GetNode("COE:formElement[@name='CMP_COMMENTS']/COE:configInfo/COE:fieldConfig", editNode, manager);
                        editNode.AppendChild(editNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Width", "COE.FormGroup"));
                        editNode.SelectSingleNode("COE:Width", manager).InnerText = "250px";
                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                    #region STRUCTURE_COMMENTS_TXT
                    try
                    {
                        editNode = GetNode("COE:Column[@name='STRUCTURE_COMMENTS_TXT']", rootNodetblColumns, manager);
                        GetNode("width", editNode, manager).InnerText = "250px";

                        editNode = GetNode("COE:formElement[@name='STRUCTURE_COMMENTS_TXT']/COE:configInfo/COE:fieldConfig", editNode, manager);
                        editNode.AppendChild(editNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Width", "COE.FormGroup"));
                        GetNode("Width", editNode, manager).InnerText = "250px";
                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                    #region CHEM_NAME_AUTOGEN
                    try
                    {
                        editNode = GetNode("COE:Column[@name='CHEM_NAME_AUTOGEN']", rootNodetblColumns, manager);
                        GetNode("width", editNode, manager).InnerText = "100px";

                        editNode = GetNode("COE:formElement[@name='CHEM_NAME_AUTOGEN']/COE:configInfo/COE:fieldConfig", editNode, manager);
                        editNode.AppendChild(editNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Width", "COE.FormGroup"));
                        GetNode("Width", editNode, manager).InnerText = "100px";
                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                    #region PERCENTAGE
                    try
                    {
                        editNode = GetNode("COE:Column[@name='PERCENTAGE']", rootNodetblColumns, manager);
                        GetNode("width", editNode, manager).InnerText = "100px";

                        editNode = GetNode("COE:formElement[@name='PERCENTAGE']/COE:configInfo/COE:fieldConfig", editNode, manager);
                        editNode.AppendChild(editNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Width", "COE.FormGroup"));
                        GetNode("Width", editNode, manager).InnerText = "100px";
                    }
                    catch (Exception ex)
                    { messages.Add(ex.Message); }
                    #endregion

                   
                    #endregion // table 2
                    





                    break;
                }

            }
            if (!errorsInPatch)
                messages.Add("CSBR158773 was successfully patched");
            else
                messages.Add("CSBR158773 was patched with partial update");
            return messages;
        }
        #region Private Method


        const string PREFIX = "COE:";

        private XmlNode GetNode(string path, XmlNode rootNode, XmlNamespaceManager manager)
        {
            XmlNode childNode;
            path = (path == String.Empty) ? "NoEmptyName" : path;
            childNode = rootNode.SelectSingleNode(path, manager);
            if (childNode == null)
               childNode = rootNode.SelectSingleNode(PREFIX + path.Replace(PREFIX, "") , manager);
            if (childNode == null)
            {
                childNode = rootNode.SelectSingleNode(path.Replace(PREFIX, "") , manager);
                if (childNode == null)
                    childNode = rootNode.SelectSingleNode(path.Replace(PREFIX, "") );
            }
            return childNode;
        }

        private void createNewAttribute(string attributeName, string attributeValue, ref XmlNode node)
        {
            XmlAttribute attributes = node.OwnerDocument.CreateAttribute(attributeName);
            node.Attributes.Append(attributes);
            node.Attributes[attributeName].Value = attributeValue;
        }

        #endregion
    }
}
