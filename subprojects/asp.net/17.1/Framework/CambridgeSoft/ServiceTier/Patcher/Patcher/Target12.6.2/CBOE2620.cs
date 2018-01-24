using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	class CBOE2620 : BugFixBaseCommand
	{
        #region Variables

        string _value = string.Empty;
        string coeFormElementPath = string.Empty;
        string _columnName = "BATCHNUMBER";

        #endregion

        #region Property

        private string columnName
        {
            set { _columnName = value; }
            get { return _columnName; }
        }

        #endregion

        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string coeFormPath = string.Empty;
            XmlAttribute nameAttrib;
            XmlNode formelementWidth;
            XmlNode columnWidth;

            #region Form Changes:
            foreach (XmlDocument formDoc in forms)
            {
                string id = formDoc.DocumentElement.Attributes["id"] == null ? string.Empty : formDoc.DocumentElement.Attributes["id"].Value;

                #region 4003.xml
                if (id == "4003")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(formDoc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");

                    //Set the root path 
                    coeFormPath = "//COE:listForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[1]/COE:configInfo/COE:fieldConfig/COE:tables/COE:table[@name='Table_9']/COE:Columns";

                    XmlNode rootColumnNode = formDoc.SelectSingleNode(coeFormPath, manager);
                    if (rootColumnNode == null)
                    {
                        messages.Add("  ERROR - DATAVIEW 4003: Table_9 not found!");
                        errorsInPatch = true;
                    }
                    else
                    {
                        while (columnName != null)
                        {
                            if (rootColumnNode.SelectSingleNode("COE:Column[@name='" + columnName + "']", manager) != null)
                            {

                                #region Column Width

                                XmlNode currentNode = rootColumnNode.SelectSingleNode("COE:Column[@name='" + columnName + "']", manager);

                                if (currentNode.SelectSingleNode("COE:width", manager) != null)
                                    columnWidth = currentNode.SelectSingleNode("COE:width", manager);
                                else
                                    columnWidth = currentNode.SelectSingleNode("COE:Width", manager);

                                string currentWidth = "150px";
                                string updateWidth = "200px";
                                string specialWidth = "250px"; //use when the field  header length >25 characters
                                if (columnName == "STORAGE_REQ_AND_WARNINGS")
                                    updateWidth = specialWidth;
                                if (columnWidth != null && columnWidth.InnerText != null)
                                {
                                    if (columnWidth.InnerText.Equals(currentWidth))
                                    {
                                        columnWidth.InnerText = updateWidth;
                                        messages.Add("Form[" + id + "]: The width node is updated successfully for COE:Column[@name='" + columnName + "']");
                                    }
                                    else if (columnWidth.InnerText.Equals(updateWidth) || columnWidth.InnerText.Equals(specialWidth))
                                    {
                                        messages.Add("Form[" + id + "]: The width node is already updated for COE:Column[@name='" + columnName + "']");
                                    }
                                    else
                                    {
                                        XmlAttribute colNode = currentNode.Attributes[0];
                                        int length = Convert.ToInt16(colNode.Value.Length);
                                        if (length <= 20)
                                            columnWidth.InnerText = updateWidth;
                                        else
                                            columnWidth.InnerText = specialWidth;
                                        messages.Add("Form[" + id + "]: The width node is updated successfully for COE:Column[@name='" + columnName + "']");
                                    }

                                }
                                else if (columnWidth != null && columnWidth.InnerText == null)
                                {
                                    columnWidth.InnerText = updateWidth;
                                    messages.Add("Form[" + id + "]: The width node is updated successfully for COE:Column[@name='" + columnName + "']");

                                }
                                else
                                {
                                    if (columnWidth == null)
                                    {
                                        columnWidth = currentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "width", "COE.FormGroup");
                                        currentNode.AppendChild(columnWidth);
                                    }
                                    columnWidth.InnerText = updateWidth;
                                    messages.Add("Form[" + id + "]: The width node is created successfully for COE:Column[@name='" + columnName + "']");
                                }


                                #endregion

                                #region Form Element Width

                                XmlNode rootFormElementNode = currentNode.SelectSingleNode("COE:formElement", manager);
                                XmlNode fieldConfigNode = rootFormElementNode.SelectSingleNode("COE:configInfo/COE:fieldConfig", manager);

                                if (fieldConfigNode.SelectSingleNode("COE:width", manager) != null)
                                    formelementWidth = fieldConfigNode.SelectSingleNode("COE:width", manager);
                                else
                                    formelementWidth = fieldConfigNode.SelectSingleNode("COE:Width", manager);

                                if (formelementWidth != null && formelementWidth.InnerText != null)
                                {
                                    if (formelementWidth.InnerText.Equals(currentWidth))
                                    {
                                        formelementWidth.InnerText = updateWidth;
                                        messages.Add("Form[" + id + "]: The width node is updated successfully for COE:formElement[@name='" + columnName + "']");
                                    }
                                    else if (columnWidth.InnerText.Equals(updateWidth) || columnWidth.InnerText.Equals(specialWidth))
                                    {
                                        messages.Add("Form[" + id + "]: The width node is already updated for COE:formElement[@name='" + columnName + "']");
                                    }
                                    else
                                    {
                                        XmlAttribute colNode = currentNode.Attributes[0];
                                        int length = Convert.ToInt16(colNode.Value.Length);
                                        if (length <= 20)
                                            columnWidth.InnerText = updateWidth;
                                        else
                                            columnWidth.InnerText = specialWidth;
                                        messages.Add("Form[" + id + "]: The width node is updated successfully for COE:Column[@name='" + columnName + "']");
                                    }

                                }
                                else if (formelementWidth != null && formelementWidth.InnerText == null)
                                {
                                    formelementWidth.InnerText = updateWidth;
                                    messages.Add("Form[" + id + "]: The width node is updated successfully for COE:formElement[@name='" + columnName + "']");
                                }
                                else
                                {
                                    if (formelementWidth == null)
                                    {
                                        formelementWidth = rootFormElementNode.OwnerDocument.CreateNode(XmlNodeType.Element, "width", "COE.FormGroup");
                                        fieldConfigNode.AppendChild(formelementWidth);
                                    }
                                    formelementWidth.InnerText = updateWidth;
                                    messages.Add("Form[" + id + "]: The width node is created successfully for COE:formElement[@name='" + columnName + "']");
                                }

                                #endregion

                                #region Name Attribute

                                currentNode = currentNode.NextSibling;
                                if (currentNode != null)
                                {
                                    nameAttrib = currentNode.Attributes["name"];
                                    if (nameAttrib != null)
                                        columnName = nameAttrib.Value;
                                    else
                                        columnName = null;
                                }
                                else
                                {
                                    columnName = null;
                                }

                                #endregion
                            }
                        }
                    }


                } //if(id ==4003)

                #endregion

            } //foreach
            #endregion

            if (!errorsInPatch)
                messages.Add(" CBOE2620 was successfully patched ");
            else
                messages.Add(" CBOE2620 was not patched due to errors ");
            return messages;
        } //method
	}
}
