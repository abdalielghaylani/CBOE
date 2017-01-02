using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Patcher for  changelist # 418005,419597,414377,417999 
    /// </summary>
    class AddBatch : BugFixBaseCommand
    {

        #region Variable
        int _loopCount = 0;
        #endregion

        #region Private Property
        private int LoopCount
        {
            get
            {
                return _loopCount;
            }
            set
            {
                _loopCount = _loopCount + value;
            }
        }
        #endregion

        #region Abstract Method

        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string coeFormPath = string.Empty;
            string coeDataViewPath = string.Empty;
            Hashtable hashFieldKeys = new Hashtable();

            #region Dataview Changes:
            foreach (XmlDocument dataviewDoc in dataviews)
            {
                string id = dataviewDoc.DocumentElement.Attributes["dataviewid"] == null ? string.Empty : dataviewDoc.DocumentElement.Attributes["dataviewid"].Value;
                int presentFieldId = 0;
                int batchFieldId = 0;
                int saltFieldId = 0;

                #region 4002.xml
                if (id == "4002")
                {
                    LoopCount = 1;
                    XmlNamespaceManager manager = new XmlNamespaceManager(dataviewDoc.NameTable);
                    manager.AddNamespace("COE", "COE.COEDataView");
                    coeDataViewPath = "//COE:table[@id='1']";  // Travel to root node
                    XmlNode tableTemporaryBatch = dataviewDoc.SelectSingleNode(coeDataViewPath, manager);

                    if (tableTemporaryBatch != null)
                    {
                        presentFieldId = Convert.ToInt32(tableTemporaryBatch.SelectSingleNode("COE:fields/@id[not(.<=../preceding-sibling::COE:fields/@id) and not(.<=../following-sibling::COE:fields/@id)]", manager).Value);
                        batchFieldId = presentFieldId + 1;
                        saltFieldId = batchFieldId + 1;

                        #region "Batch Prefix"
                        XmlNode fields = dataviewDoc.SelectSingleNode(coeDataViewPath + "/COE:fields[@name='BATCH_PREFIX']", manager);
                        if (fields == null)
                        {
                            fields = dataviewDoc.CreateNode(XmlNodeType.Element, "fields", "COE.COEDataView");
                            createNewAttribute("id", batchFieldId.ToString(), ref fields);
                            createNewAttribute("name", "BATCH_PREFIX", ref fields);
                            createNewAttribute("alias", "BATCH_PREFIX", ref fields);
                            createNewAttribute("dataType", "INTEGER", ref fields);
                            createNewAttribute("indexType", "NONE", ref fields);
                            createNewAttribute("mimeType", "NONE", ref fields);
                            createNewAttribute("visible", "true", ref fields);
                            createNewAttribute("isDefault", "false", ref fields);
                            createNewAttribute("isUniqueKey", "false", ref fields);
                            tableTemporaryBatch.AppendChild(fields);
                            messages.Add("Dataview[" + id + "]:This field node with id='" + batchFieldId + "' added succesfully.");
                        }
                        else
                        {
                            batchFieldId = Convert.ToInt32(fields.Attributes["id"].Value);
                            messages.Add("Dataview[" + id + "]:This field node with id='" + batchFieldId + "' already exits.");
                        }

                        #endregion

                        #region "Salt And Batch Suffix"
                        fields = dataviewDoc.SelectSingleNode(coeDataViewPath + "/COE:fields[@name='SALTANDBATCHSUFFIX']", manager);
                        if (fields == null)
                        {
                            fields = dataviewDoc.CreateNode(XmlNodeType.Element, "fields", "COE.COEDataView");
                            createNewAttribute("id", saltFieldId.ToString(), ref fields);
                            createNewAttribute("name", "SALTANDBATCHSUFFIX", ref fields);
                            createNewAttribute("alias", "SALTANDBATCHSUFFIX", ref fields);
                            createNewAttribute("dataType", "TEXT", ref fields);
                            createNewAttribute("indexType", "NONE", ref fields);
                            createNewAttribute("mimeType", "NONE", ref fields);
                            createNewAttribute("visible", "true", ref fields);
                            createNewAttribute("isDefault", "false", ref fields);
                            createNewAttribute("isUniqueKey", "false", ref fields);
                            tableTemporaryBatch.AppendChild(fields);
                            messages.Add("Dataview[" + id + "]:This field node with id='" + saltFieldId + "' added succesfully.");
                        }
                        else
                        {
                            saltFieldId = Convert.ToInt32(fields.Attributes["id"].Value);
                            messages.Add("Dataview[" + id + "]:This field node with id='" + saltFieldId + "' already exits.");
                        }
                        #endregion

                        #region Insert Field Id
                        hashFieldKeys.Add("4002_presentFieldId", presentFieldId);
                        hashFieldKeys.Add("4002_batchFieldId", batchFieldId);
                        hashFieldKeys.Add("4002_saltFieldId", saltFieldId);
                        #endregion


                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Dataview[" + id + "]:Table VW_TEMPORARYBATCH was not found.");
                    }
                }
                #endregion

                #region 4003.xml
                if (id == "4003")
                {
                    LoopCount = 1;
                    XmlNamespaceManager manager = new XmlNamespaceManager(dataviewDoc.NameTable);
                    manager.AddNamespace("COE", "COE.COEDataView");
                    coeDataViewPath = "//COE:table[@id='9']";  // Travel to root node
                    XmlNode tableTemporaryBatch = dataviewDoc.SelectSingleNode(coeDataViewPath, manager);

                    if (tableTemporaryBatch != null)
                    {
                        presentFieldId = Convert.ToInt32(tableTemporaryBatch.SelectSingleNode("COE:fields/@id[not(.<=../preceding-sibling::COE:fields/@id) and not(.<=../following-sibling::COE:fields/@id)]", manager).Value);
                        batchFieldId = presentFieldId + 1;
                        saltFieldId = batchFieldId + 1;

                        #region "Batch Prefix"
                        XmlNode fields = dataviewDoc.SelectSingleNode(coeDataViewPath + "/COE:fields[@name='BATCH_PREFIX']", manager);
                        if (fields == null)
                        {
                            fields = dataviewDoc.CreateNode(XmlNodeType.Element, "fields", "COE.COEDataView");
                            createNewAttribute("id", batchFieldId.ToString(), ref fields);
                            createNewAttribute("name", "BATCH_PREFIX", ref fields);
                            createNewAttribute("alias", "BATCH_PREFIX", ref fields);
                            createNewAttribute("dataType", "INTEGER", ref fields);
                            createNewAttribute("indexType", "NONE", ref fields);
                            createNewAttribute("mimeType", "NONE", ref fields);
                            createNewAttribute("visible", "1", ref fields);
                            tableTemporaryBatch.AppendChild(fields);
                            messages.Add("Dataview[" + id + "]:This field node with id='" + batchFieldId + "' added succesfully.");
                        }
                        else
                        {
                            batchFieldId = Convert.ToInt32(fields.Attributes["id"].Value);
                            messages.Add("Dataview[" + id + "]:This field node with id='" + batchFieldId + "' already exits.");
                        }

                        #endregion

                        #region "Salt And Batch Suffix"
                        fields = dataviewDoc.SelectSingleNode(coeDataViewPath + "/COE:fields[@name='SALTANDBATCHSUFFIX']", manager);
                        if (fields == null)
                        {
                            fields = dataviewDoc.CreateNode(XmlNodeType.Element, "fields", "COE.COEDataView");
                            createNewAttribute("id", saltFieldId.ToString(), ref fields);
                            createNewAttribute("name", "SALTANDBATCHSUFFIX", ref fields);
                            createNewAttribute("alias", "SALTANDBATCHSUFFIX", ref fields);
                            createNewAttribute("dataType", "TEXT", ref fields);
                            createNewAttribute("indexType", "NONE", ref fields);
                            createNewAttribute("mimeType", "NONE", ref fields);
                            createNewAttribute("visible", "1", ref fields);
                            tableTemporaryBatch.AppendChild(fields);
                            messages.Add("Dataview[" + id + "]:This field node with id='" + saltFieldId + "' added succesfully.");
                        }
                        else
                        {
                            saltFieldId = Convert.ToInt32(fields.Attributes["id"].Value);
                            messages.Add("Dataview[" + id + "]:This field node with id='" + saltFieldId + "' already exits.");
                        }
                        #endregion

                        #region Insert Field Id
                        hashFieldKeys.Add("4003_presentFieldId", presentFieldId);
                        hashFieldKeys.Add("4003_batchFieldId", batchFieldId);
                        hashFieldKeys.Add("4003_saltFieldId", saltFieldId);
                        #endregion

                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Dataview[" + id + "]:Table VW_TEMPORARYBATCH was not found.");
                    }
                }
                #endregion

                #region 4005.xml
                if (id == "4005")
                {
                    LoopCount = 1;
                    XmlNamespaceManager manager = new XmlNamespaceManager(dataviewDoc.NameTable);
                    manager.AddNamespace("COE", "COE.COEDataView");
                    coeDataViewPath = "//COE:table[@id='1']";  // Travel to root node
                    XmlNode tableTemporaryBatch = dataviewDoc.SelectSingleNode(coeDataViewPath, manager);

                    if (tableTemporaryBatch != null)
                    {
                        presentFieldId = Convert.ToInt32(tableTemporaryBatch.SelectSingleNode("COE:fields/@id[not(.<=../preceding-sibling::COE:fields/@id) and not(.<=../following-sibling::COE:fields/@id)]", manager).Value);
                        batchFieldId = presentFieldId + 1;
                        saltFieldId = batchFieldId + 1;

                        #region "Batch Prefix"
                        XmlNode fields = dataviewDoc.SelectSingleNode(coeDataViewPath + "/COE:fields[@name='BATCH_PREFIX']", manager);
                        if (fields == null)
                        {
                            fields = dataviewDoc.CreateNode(XmlNodeType.Element, "fields", "COE.COEDataView");
                            createNewAttribute("id", batchFieldId.ToString(), ref fields);
                            createNewAttribute("name", "BATCH_PREFIX", ref fields);
                            createNewAttribute("alias", "BATCH_PREFIX", ref fields);
                            createNewAttribute("dataType", "BOOLEAN", ref fields);
                            createNewAttribute("indexType", "NONE", ref fields);
                            createNewAttribute("mimeType", "NONE", ref fields);
                            createNewAttribute("visible", "true", ref fields);
                            createNewAttribute("isDefault", "false", ref fields);
                            createNewAttribute("isUniqueKey", "false", ref fields);
                            tableTemporaryBatch.AppendChild(fields);
                            messages.Add("Dataview[" + id + "]:This field node with id='" + batchFieldId + "' added succesfully.");
                        }
                        else
                        {
                            batchFieldId = Convert.ToInt32(fields.Attributes["id"].Value);
                            messages.Add("Dataview[" + id + "]:This field node with id='" + batchFieldId + "' already exits.");
                        }

                        #endregion

                        #region "Salt And Batch Suffix"
                        fields = dataviewDoc.SelectSingleNode(coeDataViewPath + "/COE:fields[@name='SALTANDBATCHSUFFIX']", manager);
                        if (fields == null)
                        {
                            fields = dataviewDoc.CreateNode(XmlNodeType.Element, "fields", "COE.COEDataView");
                            createNewAttribute("id", saltFieldId.ToString(), ref fields);
                            createNewAttribute("name", "SALTANDBATCHSUFFIX", ref fields);
                            createNewAttribute("alias", "SALTANDBATCHSUFFIX", ref fields);
                            createNewAttribute("dataType", "TEXT", ref fields);
                            createNewAttribute("indexType", "NONE", ref fields);
                            createNewAttribute("mimeType", "NONE", ref fields);
                            createNewAttribute("visible", "true", ref fields);
                            tableTemporaryBatch.AppendChild(fields);
                            messages.Add("Dataview[" + id + "]:This field node with id='" + saltFieldId + "' added succesfully.");
                        }
                        else
                        {
                            saltFieldId = Convert.ToInt32(fields.Attributes["id"].Value);
                            messages.Add("Dataview[" + id + "]:This field node with id='" + saltFieldId + "' already exits.");
                        }
                        #endregion

                        #region Insert Field Id
                        hashFieldKeys.Add("4005_presentFieldId", presentFieldId);
                        hashFieldKeys.Add("4005_batchFieldId", batchFieldId);
                        hashFieldKeys.Add("4005_saltFieldId", saltFieldId);
                        #endregion

                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Dataview[" + id + "]:Table VW_TEMPORARYBATCH was not found.");
                    }
                }
                #endregion

                #region 4006.xml
                if (id == "4006")
                {
                    LoopCount = 1;
                    XmlNamespaceManager manager = new XmlNamespaceManager(dataviewDoc.NameTable);
                    manager.AddNamespace("COE", "COE.COEDataView");
                    coeDataViewPath = "//COE:table[@id='9']";  // Travel to root node
                    XmlNode tableTemporaryBatch = dataviewDoc.SelectSingleNode(coeDataViewPath, manager);

                    if (tableTemporaryBatch != null)
                    {
                        presentFieldId = Convert.ToInt32(tableTemporaryBatch.SelectSingleNode("COE:fields/@id[not(.<=../preceding-sibling::COE:fields/@id) and not(.<=../following-sibling::COE:fields/@id)]", manager).Value);
                        batchFieldId = presentFieldId + 1;
                        saltFieldId = batchFieldId + 1;

                        #region "Batch Prefix"
                        XmlNode fields = dataviewDoc.SelectSingleNode(coeDataViewPath + "/COE:fields[@name='BATCH_PREFIX']", manager);
                        if (fields == null)
                        {
                            fields = dataviewDoc.CreateNode(XmlNodeType.Element, "fields", "COE.COEDataView");
                            createNewAttribute("id", batchFieldId.ToString(), ref fields);
                            createNewAttribute("name", "BATCH_PREFIX", ref fields);
                            createNewAttribute("alias", "BATCH_PREFIX", ref fields);
                            createNewAttribute("dataType", "INTEGER", ref fields);
                            createNewAttribute("indexType", "NONE", ref fields);
                            createNewAttribute("mimeType", "NONE", ref fields);
                            createNewAttribute("visible", "0", ref fields);
                            tableTemporaryBatch.AppendChild(fields);
                            messages.Add("Dataview[" + id + "]:This field node with id='" + batchFieldId + "' added succesfully.");
                        }
                        else
                        {
                            batchFieldId = Convert.ToInt32(fields.Attributes["id"].Value);
                            messages.Add("Dataview[" + id + "]:This field node with id='" + batchFieldId + "' already exits.");
                        }

                        #endregion

                        #region "Salt And Batch Suffix"
                        fields = dataviewDoc.SelectSingleNode(coeDataViewPath + "/COE:fields[@name='SALTANDBATCHSUFFIX']", manager);
                        if (fields == null)
                        {
                            fields = dataviewDoc.CreateNode(XmlNodeType.Element, "fields", "COE.COEDataView");
                            createNewAttribute("id", saltFieldId.ToString(), ref fields);
                            createNewAttribute("name", "SALTANDBATCHSUFFIX", ref fields);
                            createNewAttribute("alias", "SALTANDBATCHSUFFIX", ref fields);
                            createNewAttribute("dataType", "TEXT", ref fields);
                            createNewAttribute("indexType", "NONE", ref fields);
                            createNewAttribute("mimeType", "NONE", ref fields);
                            createNewAttribute("visible", "0", ref fields);
                            tableTemporaryBatch.AppendChild(fields);
                            messages.Add("Dataview[" + id + "]:This field node with id='" + saltFieldId + "' added succesfully.");
                        }
                        else
                        {
                            saltFieldId = Convert.ToInt32(fields.Attributes["id"].Value);
                            messages.Add("Dataview[" + id + "]:This field node with id='" + saltFieldId + "' already exits.");
                        }
                        #endregion

                        #region Insert Field Id
                        hashFieldKeys.Add("4006_presentFieldId", presentFieldId);
                        hashFieldKeys.Add("4006_batchFieldId", batchFieldId);
                        hashFieldKeys.Add("4006_saltFieldId", saltFieldId);
                        #endregion

                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Dataview[" + id + "]:Table VW_TEMPORARYBATCH was not found.");
                    }
                }
                #endregion

                #region 4019.xml
                if (id == "4019")
                {
                    LoopCount = 1;
                    XmlNamespaceManager manager = new XmlNamespaceManager(dataviewDoc.NameTable);
                    manager.AddNamespace("COE", "COE.COEDataView");
                    coeDataViewPath = "//COE:table[@id='9']";  // Travel to root node
                    XmlNode tableTemporaryBatch = dataviewDoc.SelectSingleNode(coeDataViewPath, manager);

                    if (tableTemporaryBatch != null)
                    {
                        presentFieldId = Convert.ToInt32(tableTemporaryBatch.SelectSingleNode("COE:fields/@id[not(.<=../preceding-sibling::COE:fields/@id) and not(.<=../following-sibling::COE:fields/@id)]", manager).Value);
                        batchFieldId = presentFieldId + 1;
                        saltFieldId = batchFieldId + 1;

                        #region "Batch Prefix"
                        XmlNode fields = dataviewDoc.SelectSingleNode(coeDataViewPath + "/COE:fields[@name='BATCH_PREFIX']", manager);
                        if (fields == null)
                        {
                            fields = dataviewDoc.CreateNode(XmlNodeType.Element, "fields", "COE.COEDataView");
                            createNewAttribute("id", batchFieldId.ToString(), ref fields);
                            createNewAttribute("name", "BATCH_PREFIX", ref fields);
                            createNewAttribute("alias", "BATCH_PREFIX", ref fields);
                            createNewAttribute("dataType", "INTEGER", ref fields);
                            createNewAttribute("indexType", "NONE", ref fields);
                            createNewAttribute("mimeType", "NONE", ref fields);
                            createNewAttribute("visible", "1", ref fields);
                            tableTemporaryBatch.AppendChild(fields);
                            messages.Add("Dataview[" + id + "]:This field node with id='" + batchFieldId + "' added succesfully.");
                        }
                        else
                        {
                            batchFieldId = Convert.ToInt32(fields.Attributes["id"].Value);
                            messages.Add("Dataview[" + id + "]:This field node with id='" + batchFieldId + "' already exits.");
                        }

                        #endregion

                        #region "Salt And Batch Suffix"
                        fields = dataviewDoc.SelectSingleNode(coeDataViewPath + "/COE:fields[@name='SALTANDBATCHSUFFIX']", manager);
                        if (fields == null)
                        {
                            fields = dataviewDoc.CreateNode(XmlNodeType.Element, "fields", "COE.COEDataView");
                            createNewAttribute("id", saltFieldId.ToString(), ref fields);
                            createNewAttribute("name", "SALTANDBATCHSUFFIX", ref fields);
                            createNewAttribute("alias", "SALTANDBATCHSUFFIX", ref fields);
                            createNewAttribute("dataType", "TEXT", ref fields);
                            createNewAttribute("indexType", "NONE", ref fields);
                            createNewAttribute("mimeType", "NONE", ref fields);
                            createNewAttribute("visible", "1", ref fields);
                            tableTemporaryBatch.AppendChild(fields);
                            messages.Add("Dataview[" + id + "]:This field node with id='" + saltFieldId + "' added succesfully.");
                        }
                        else
                        {
                            saltFieldId = Convert.ToInt32(fields.Attributes["id"].Value);
                            messages.Add("Dataview[" + id + "]:This field node with id='" + saltFieldId + "' already exits.");
                        }
                        #endregion

                        #region Insert Field Id
                        hashFieldKeys.Add("4019_presentFieldId", presentFieldId);
                        hashFieldKeys.Add("4019_batchFieldId", batchFieldId);
                        hashFieldKeys.Add("4019_saltFieldId", saltFieldId);
                        #endregion

                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Dataview[" + id + "]:Table VW_TEMPORARYBATCH was not found.");
                    }
                }
                #endregion

                #region Validate Loopcount
                if (LoopCount == 5)
                {
                    LoopCount = -5;
                    break;
                }
                #endregion

            }
            #endregion

            #region Form Changes:
            foreach (XmlDocument formDoc in forms)
            {
                string id = formDoc.DocumentElement.Attributes["id"] == null ? string.Empty : formDoc.DocumentElement.Attributes["id"].Value;

                #region 4002.xml
                if (id == "4002")
                {
                    LoopCount = 1;
                    XmlNamespaceManager manager = new XmlNamespaceManager(formDoc.NameTable);
                    XmlNode selectedNode;
                    manager.AddNamespace("COE", "COE.FormGroup");

                    #region For Upgrade 1210_1250
                    try
                    {
                        coeFormPath = "//COE:listForms[@defaultForm='0']/COE:listForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo";
                        selectedNode = formDoc.SelectSingleNode(coeFormPath, manager);
                        if (selectedNode.SelectSingleNode("COE:formElement[@name='BATCH_PREFIX']", manager) != null)
                            selectedNode.RemoveChild(selectedNode.SelectSingleNode("COE:formElement[@name='BATCH_PREFIX']", manager));
                    }
                    catch (Exception ex)
                    { }

                    #endregion

                    coeFormPath = "//COE:queryForms[@defaultForm='0']/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo"; // Path to check the Rootnode before patcher update.
                    selectedNode = formDoc.SelectSingleNode(coeFormPath, manager);
                    if (selectedNode != null)
                    {
                        messages.Add(createNewFormElement_BatchPrefixSearch(id, hashFieldKeys[id + "_batchFieldId"].ToString(), "1", "", coeFormPath, manager, ref selectedNode));
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[" + id + "]:layoutInfo was not found in form " + id + " to create formelement.");
                    }
                }
                #endregion

                #region 4003.xml
                if (id == "4003")
                {
                    LoopCount = 1;
                    XmlNamespaceManager manager = new XmlNamespaceManager(formDoc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");
                    coeFormPath = "//COE:queryForms[@defaultForm='0']/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:layoutInfo"; // Path to check the Rootnode before patcher update.
                    XmlNode selectedNode = formDoc.SelectSingleNode(coeFormPath, manager);
                    if (selectedNode != null)
                    {
                        messages.Add(createNewFormElement_BatchPrefixSearch(id, hashFieldKeys[id + "_batchFieldId"].ToString(), "9", "FULLREGNUMBER", coeFormPath, manager, ref selectedNode));
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[" + id + "]:layoutInfo was not found in form " + id + " to create formelement.");
                    }
                }
                #endregion

                #region 4010.xml
                if (id == "4010")
                {
                    LoopCount = 1;
                    XmlNamespaceManager manager = new XmlNamespaceManager(formDoc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");

                    #region AddMode
                    coeFormPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']/COE:addMode"; // Path to check the Rootnode before patcher update.
                    XmlNode selectedNode_addMode = formDoc.SelectSingleNode(coeFormPath, manager);
                    if (selectedNode_addMode != null)
                    {
                        messages.Add(createNewFormElement_SaltAndBatchSuffix(id, "addMode", "SCIENTIST_ID", coeFormPath, manager, ref selectedNode_addMode));
                        messages.Add(createNewFormElement_BatchPrefix(id, "addMode", coeFormPath, manager, ref selectedNode_addMode));
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[4011]:editMode was not found in form 4011 to create formelement.");
                    }
                    #endregion

                    #region EditMode
                    coeFormPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']/COE:editMode"; // Path to check the Rootnode before patcher update.
                    XmlNode selectedNode_editMode = formDoc.SelectSingleNode(coeFormPath, manager);
                    if (selectedNode_editMode != null)
                    {
                        messages.Add(createNewFormElement_SaltAndBatchSuffix(id, "editMode", "SCIENTIST_ID", coeFormPath, manager, ref selectedNode_editMode));
                        messages.Add(createNewFormElement_BatchPrefix(id, "editMode", coeFormPath, manager, ref selectedNode_editMode));
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[4011]:editMode was not found in form 4011 to create formelement.");
                    }
                    #endregion

                    #region ViewMode
                    coeFormPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']/COE:viewMode"; // Path to check the Rootnode before patcher update.
                    XmlNode selectedNode_viewMode = formDoc.SelectSingleNode(coeFormPath, manager);
                    if (selectedNode_viewMode != null)
                    {
                        messages.Add(createNewFormElement_SaltAndBatchSuffix(id, "viewMode", "SCIENTIST_ID", coeFormPath, manager, ref selectedNode_viewMode));
                        messages.Add(createNewFormElement_BatchPrefix(id, "viewMode", coeFormPath, manager, ref selectedNode_viewMode));
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[4011]:editMode was not found in form 4011 to create formelement.");
                    }
                    #endregion
                }
                #endregion

                #region 4011.xml
                if (id == "4011")
                {
                    LoopCount = 1;
                    XmlNamespaceManager manager = new XmlNamespaceManager(formDoc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");

                    #region AddMode
                    coeFormPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']/COE:addMode"; // Path to check the Rootnode before patcher update.
                    XmlNode selectedNode_addMode = formDoc.SelectSingleNode(coeFormPath, manager);
                    if (selectedNode_addMode != null)
                    {
                        messages.Add(createNewFormElement_SaltAndBatchSuffix(id, "addMode", "SCIENTIST_ID", coeFormPath, manager, ref selectedNode_addMode));
                        messages.Add(createNewFormElement_BatchPrefix(id, "addMode", coeFormPath, manager, ref selectedNode_addMode));
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[4011]:editMode was not found in form 4011 to create formelement.");
                    }
                    #endregion

                    #region EditMode
                    coeFormPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']/COE:editMode"; // Path to check the Rootnode before patcher update.
                    XmlNode selectedNode_editMode = formDoc.SelectSingleNode(coeFormPath, manager);
                    if (selectedNode_editMode != null)
                    {
                        messages.Add(createNewFormElement_SaltAndBatchSuffix(id, "editMode", "SCIENTIST_ID", coeFormPath, manager, ref selectedNode_editMode));
                        messages.Add(createNewFormElement_BatchPrefix(id, "editMode", coeFormPath, manager, ref selectedNode_editMode));
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[4011]:editMode was not found in form 4011 to create formelement.");
                    }
                    #endregion

                    #region ViewMode
                    coeFormPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']/COE:viewMode"; // Path to check the Rootnode before patcher update.
                    XmlNode selectedNode_viewMode = formDoc.SelectSingleNode(coeFormPath, manager);
                    if (selectedNode_viewMode != null)
                    {
                        messages.Add(createNewFormElement_SaltAndBatchSuffix(id, "viewMode", "Batch ID", coeFormPath, manager, ref selectedNode_viewMode));
                        messages.Add(createNewFormElement_BatchPrefix(id, "viewMode", coeFormPath, manager, ref selectedNode_viewMode));
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[4011]:editMode was not found in form 4011 to create formelement.");
                    }
                    #endregion
                }
                #endregion

                #region 4012.xml
                if (id == "4012")
                {
                    LoopCount = 1;
                    XmlNamespaceManager manager = new XmlNamespaceManager(formDoc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");

                    #region AddMode
                    coeFormPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']/COE:addMode"; // Path to check the Rootnode before patcher update.
                    XmlNode selectedNode_addMode = formDoc.SelectSingleNode(coeFormPath, manager);
                    if (selectedNode_addMode != null)
                    {
                        messages.Add(createNewFormElement_SaltAndBatchSuffix(id, "addMode", "SCIENTIST_ID", coeFormPath, manager, ref selectedNode_addMode));
                        messages.Add(createNewFormElement_BatchPrefix(id, "addMode", coeFormPath, manager, ref selectedNode_addMode));
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[4011]:editMode was not found in form 4011 to create formelement.");
                    }
                    #endregion

                    #region EditMode
                    coeFormPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']/COE:editMode"; // Path to check the Rootnode before patcher update.
                    XmlNode selectedNode_editMode = formDoc.SelectSingleNode(coeFormPath, manager);
                    if (selectedNode_editMode != null)
                    {
                        messages.Add(createNewFormElement_SaltAndBatchSuffix(id, "editMode", "SCIENTIST_ID", coeFormPath, manager, ref selectedNode_editMode));
                        messages.Add(createNewFormElement_BatchPrefix(id, "editMode", coeFormPath, manager, ref selectedNode_editMode));
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[4011]:editMode was not found in form 4011 to create formelement.");
                    }
                    #endregion

                    #region ViewMode
                    coeFormPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']/COE:viewMode"; // Path to check the Rootnode before patcher update.
                    XmlNode selectedNode_viewMode = formDoc.SelectSingleNode(coeFormPath, manager);
                    if (selectedNode_viewMode != null)
                    {
                        messages.Add(createNewFormElement_SaltAndBatchSuffix(id, "viewMode", "Batch ID", coeFormPath, manager, ref selectedNode_viewMode));
                        messages.Add(createNewFormElement_BatchPrefix(id, "viewMode", coeFormPath, manager, ref selectedNode_viewMode));
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Form[4011]:editMode was not found in form 4011 to create formelement.");
                    }
                    #endregion
                }
                #endregion

                #region Validate Loopcount
                if (LoopCount == 5)
                {
                    LoopCount = -5;
                    break;
                }
                #endregion
            }
            #endregion

            if (!errorsInPatch)
                messages.Add("Add Batch Workflow was successfully fixed.");
            else
                messages.Add("Add Batch Workflow  was fixed with partial update.");
            return messages;
        }

        #endregion

        #region Private Functions
        private string createNewFormElement_BatchPrefixSearch(string xmlId, string fieldId, string tableId, string insertBeforeText, string coeFormPath, XmlNamespaceManager manager, ref XmlNode rootSelectedNode)
        {
            string formElementName = "BATCH_PREFIX";
            string searchId = string.Empty;
            XmlNode formElementNode = rootSelectedNode.SelectSingleNode(coeFormPath + "/COE:formElement[@name='" + formElementName + "']", manager);
            if (formElementNode == null)
            {
                formElementNode = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "formElement", "COE.FormGroup");
                createNewAttribute("name", formElementName, ref formElementNode);

                // create child nodes
                XmlNode label = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "label", "COE.FormGroup");
                label.InnerText = "Batch Prefix";
                XmlNode showHelp = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "showHelp", "COE.FormGroup");
                showHelp.InnerText = "false";
                XmlNode isFileUpload = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "isFileUpload", "COE.FormGroup");
                isFileUpload.InnerText = "false";
                XmlNode pageComunicationProvider = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "pageComunicationProvider", "COE.FormGroup");
                XmlNode fileUploadBindingExpression = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "fileUploadBindingExpression", "COE.FormGroup");
                XmlNode helpText = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "helpText", "COE.FormGroup");
                XmlNode defaultValue = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "defaultValue", "COE.FormGroup");
                XmlNode bindingExpression = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "bindingExpression", "COE.FormGroup");

                switch (xmlId)
                {
                    case "4002":
                        searchId = "35";
                        break;
                    case "4003":
                        searchId = "51";
                        break;
                }
                bindingExpression.InnerText = "SearchCriteria[" + searchId + "].Criterium.Value";

                XmlNode id = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Id", null);
                id.InnerText = "BATCH_PREFIXProperty";

                XmlNode displayInfo = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "displayInfo", null);
                //  sub child nodes
                //--------------------
                displayInfo.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "cssClass", "COE.FormGroup"));
                displayInfo.SelectSingleNode("COE:cssClass", manager).InnerText = "Std20x40";
                displayInfo.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "type", "COE.FormGroup"));
                displayInfo.SelectSingleNode("COE:type", manager).InnerText = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList"; displayInfo.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "visible", "COE.FormGroup"));
                displayInfo.SelectSingleNode("COE:visible", manager).InnerText = "true";
                //--------------------

                XmlNode validationRuleList = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "validationRuleList", "COE.FormGroup");
                XmlNode configInfo = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "configInfo", "COE.FormGroup");
                // sub child
                //--------------------
                XmlNode serverEvents = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "serverEvents", "COE.FormGroup");
                XmlNode clientEvents = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "clientEvents", "COE.FormGroup");

                XmlNode fieldConfig = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "COE:fieldConfig", "COE.FormGroup");
                fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "COE:CSSLabelClass", "COE.FormGroup"));
                fieldConfig.SelectSingleNode("COE:CSSLabelClass", manager).InnerText = "FELabel";
                fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "COE:CSSClass", "COE.FormGroup"));
                fieldConfig.SelectSingleNode("COE:CSSClass", manager).InnerText = "FEDropDownList";
                fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "COE:dropDownItemsSelect", "COE.FormGroup"));
                fieldConfig.SelectSingleNode("COE:dropDownItemsSelect", manager).InnerText = "SELECT PKL.ID AS KEY, PKL.PICKLISTVALUE AS VALUE FROM REGDB.PICKLIST PKL,REGDB.PICKLISTDOMAIN PKD WHERE PKL.PICKLISTDOMAINID = PKD.ID AND UPPER(PKD.DESCRIPTION)='BATCH PREFIX' ORDER BY KEY";
                fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "COE:Enable", "COE.FormGroup"));
                fieldConfig.SelectSingleNode("COE:Enable", manager).InnerText = "true";
                configInfo.AppendChild(fieldConfig);
                //--------------------

                XmlNode dataSource = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "dataSource", "COE.FormGroup");
                XmlNode dataSourceId = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "dataSourceId", "COE.FormGroup");
                XmlNode requiredStyle = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "requiredStyle", "COE.FormGroup");

                XmlNode searchCriteriaItem = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "searchCriteriaItem", "COE.FormGroup");
                createNewAttribute("fieldid", fieldId, ref searchCriteriaItem);
                createNewAttribute("tableid", tableId, ref searchCriteriaItem);
                createNewAttribute("id", searchId, ref searchCriteriaItem);
                createNewAttribute("searchLookupByID", "true", ref searchCriteriaItem);
                createNewAttribute("aggregateFunctionName", "", ref searchCriteriaItem);

                //  sub child nodes
                //--------------------
                XmlNode numericalCriteria = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "numericalCriteria", "COE.FormGroup");
                createNewAttribute("negate", "NO", ref numericalCriteria);
                createNewAttribute("trim", "NONE", ref numericalCriteria);
                createNewAttribute("operator", "EQUAL", ref numericalCriteria);
                searchCriteriaItem.AppendChild(numericalCriteria);
                //--------------------

                XmlNode displayData = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "displayData", "COE.FormGroup");

                formElementNode.AppendChild(label);
                formElementNode.AppendChild(showHelp);
                formElementNode.AppendChild(isFileUpload);
                formElementNode.AppendChild(pageComunicationProvider);
                formElementNode.AppendChild(fileUploadBindingExpression);
                formElementNode.AppendChild(helpText);
                formElementNode.AppendChild(defaultValue);
                formElementNode.AppendChild(bindingExpression);
                formElementNode.AppendChild(id);
                formElementNode.AppendChild(displayInfo);
                formElementNode.AppendChild(validationRuleList);
                formElementNode.AppendChild(serverEvents);
                formElementNode.AppendChild(clientEvents);
                formElementNode.AppendChild(configInfo);
                formElementNode.AppendChild(dataSource);
                formElementNode.AppendChild(dataSourceId);
                formElementNode.AppendChild(requiredStyle);
                formElementNode.AppendChild(searchCriteriaItem);
                formElementNode.AppendChild(displayData);

                // attach new node at specified position or append
                XmlNode insertAfterNode = null;
                XmlNode insertBeforeNode = rootSelectedNode.SelectSingleNode(coeFormPath + "/COE:formElement[@name='" + insertBeforeText + "']", manager);
                if (insertAfterNode != null)
                {
                    rootSelectedNode.InsertAfter(formElementNode, insertAfterNode);
                }
                else if (insertBeforeNode != null)
                {
                    rootSelectedNode.InsertBefore(formElementNode, insertBeforeNode);
                }
                else
                {
                    rootSelectedNode.AppendChild(formElementNode);
                }
                return "Form[" + xmlId + ":]:The formelement node with name='" + formElementName + "' added succesfully.";
            }
            else
            {
                return "Form[" + xmlId + ":]:The formelement node with name='" + formElementName + "' already exits.";
            }
        }
        private string createNewFormElement_BatchPrefix(string xmlId, string formMode, string coeFormPath, XmlNamespaceManager manager, ref XmlNode rootSelectedNode)
        {
            string formElementName = "BATCH_PREFIX";
            XmlNode formElementNode = rootSelectedNode.SelectSingleNode(coeFormPath + "/COE:formElement[@name='" + formElementName + "']", manager);
            if (formElementNode == null)
            {
                formElementNode = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "formElement", "COE.FormGroup");
                createNewAttribute("name", formElementName, ref formElementNode);

                // create child nodes
                XmlNode label = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "label", "COE.FormGroup");
                label.InnerText = "Batch Prefix";
                XmlNode showHelp = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "showHelp", null);
                showHelp.InnerText = "false";
                XmlNode isFileUpload = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "isFileUpload", "COE.FormGroup");
                isFileUpload.InnerText = "false";
                XmlNode pageComunicationProvider = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "pageComunicationProvider", "COE.FormGroup");
                XmlNode fileUploadBindingExpression = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "fileUploadBindingExpression", "COE.FormGroup");
                XmlNode helpText = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "helpText", "COE.FormGroup");
                XmlNode defaultValue = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "defaultValue", "COE.FormGroup");
                XmlNode bindingExpression = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "bindingExpression", "COE.FormGroup");
                bindingExpression.InnerText = "PropertyList[@Name='BATCH_PREFIX'| Value]";
                XmlNode id = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Id", "COE.FormGroup");
                id.InnerText = "BATCH_PREFIXProperty";

                XmlNode displayInfo = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "displayInfo", "COE.FormGroup");
                //  sub child nodes
                //--------------------
                displayInfo.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "cssClass", "COE.FormGroup"));
                displayInfo.SelectSingleNode("COE:cssClass", manager).InnerText = "Std20x40";
                displayInfo.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "type", "COE.FormGroup"));
                displayInfo.SelectSingleNode("COE:type", manager).InnerText = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList";
                displayInfo.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "visible", "COE.FormGroup"));
                displayInfo.SelectSingleNode("COE:visible", manager).InnerText = "true";
                //--------------------

                XmlNode validationRuleList = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "validationRuleList", "COE.FormGroup");
                XmlNode validationRule = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "validationRule", "COE.FormGroup");
                // sub child
                //--------------------
                switch (formMode.ToLower())
                {
                    case "addmode":
                        createNewAttribute("validationRuleName", "requiredField", ref validationRule);
                        createNewAttribute("errorMessage", "Batch Prefix is required", ref validationRule);
                        createNewAttribute("displayPosition", "Top_Left", ref validationRule);
                        validationRule.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "params", null));
                        validationRuleList.AppendChild(validationRule);
                        break;
                    case "editmode":
                        createNewAttribute("validationRuleName", "requiredField", ref validationRule);
                        createNewAttribute("errorMessage", "Batch Prefix is required", ref validationRule);
                        createNewAttribute("displayPosition", "Top_Left", ref validationRule);
                        validationRule.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "params", null));
                        validationRuleList.AppendChild(validationRule);
                        break;
                    case "viewmode":
                        validationRule = null;
                        break;

                }
                //--------------------

                XmlNode configInfo = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "configInfo", "COE.FormGroup");
                // sub child
                //--------------------
                XmlNode fieldConfig = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "COE:fieldConfig", "COE.FormGroup");
                fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "COE:CSSLabelClass", "COE.FormGroup"));
                fieldConfig.SelectSingleNode("COE:CSSLabelClass", manager).InnerText = "FELabel";
                fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "COE:CSSClass", "COE.FormGroup"));
                fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "COE:dropDownItemsSelect", "COE.FormGroup"));
                fieldConfig.SelectSingleNode("COE:dropDownItemsSelect", manager).InnerText = "SELECT PKL.ID AS KEY, PKL.PICKLISTVALUE AS VALUE FROM REGDB.PICKLIST PKL,REGDB.PICKLISTDOMAIN PKD WHERE PKL.PICKLISTDOMAINID = PKD.ID AND UPPER(PKD.DESCRIPTION)='BATCH PREFIX' ORDER BY KEY";
                fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "COE:Enable", "COE.FormGroup"));
                switch (formMode.ToLower())
                {
                    case "addmode":
                        fieldConfig.SelectSingleNode("COE:CSSClass", manager).InnerText = "FEDropDownList";
                        fieldConfig.SelectSingleNode("COE:Enable", manager).InnerText = "true";
                        break;
                    case "editmode":
                        fieldConfig.SelectSingleNode("COE:CSSClass", manager).InnerText = "FEDropDownList";
                        fieldConfig.SelectSingleNode("COE:Enable", manager).InnerText = "true";
                        break;
                    case "viewmode":
                        fieldConfig.SelectSingleNode("COE:CSSClass", manager).InnerText = "FEDropDownListViewMode";
                        fieldConfig.SelectSingleNode("COE:Enable", manager).InnerText = "False";
                        break;
                }
                configInfo.AppendChild(fieldConfig);
                //--------------------

                XmlNode dataSource = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "dataSource", "COE.FormGroup");
                XmlNode dataSourceId = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "dataSourceId", "COE.FormGroup");
                XmlNode requiredStyle = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "requiredStyle", "COE.FormGroup");
                XmlNode displayData = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "displayData", "COE.FormGroup");

                formElementNode.AppendChild(label);
                formElementNode.AppendChild(showHelp);
                formElementNode.AppendChild(isFileUpload);
                formElementNode.AppendChild(pageComunicationProvider);
                formElementNode.AppendChild(fileUploadBindingExpression);
                formElementNode.AppendChild(helpText);
                formElementNode.AppendChild(defaultValue);
                formElementNode.AppendChild(bindingExpression);
                formElementNode.AppendChild(id);
                formElementNode.AppendChild(displayInfo);
                formElementNode.AppendChild(validationRuleList);
                formElementNode.AppendChild(configInfo);
                formElementNode.AppendChild(dataSource);
                formElementNode.AppendChild(dataSourceId);
                formElementNode.AppendChild(requiredStyle);
                formElementNode.AppendChild(displayData);

                // attach new node at specified position or append
                XmlNode insertAfterNode = null;
                XmlNode insertBeforeNode = rootSelectedNode.SelectSingleNode(coeFormPath + "/COE:formElement[@name='SALTANDBATCHSUFFIX']", manager);
                if (insertAfterNode != null)
                {
                    rootSelectedNode.InsertAfter(formElementNode, insertAfterNode);
                }
                else if (insertBeforeNode != null)
                {
                    rootSelectedNode.InsertBefore(formElementNode, insertBeforeNode);
                }
                else
                {
                    rootSelectedNode.AppendChild(formElementNode);
                }
                return "Form[" + xmlId + ":" + formMode.ToUpper() + "]:The formelement node with name='" + formElementName + "' added succesfully.";
            }
            else
            {
                return "Form[" + xmlId + ":" + formMode.ToUpper() + "]:The formelement node with name='" + formElementName + "' already exits.";
            }
        }
        private string createNewFormElement_SaltAndBatchSuffix(string xmlId, string formMode, string insertBeforeText, string coeFormPath, XmlNamespaceManager manager, ref XmlNode rootSelectedNode)
        {
            string formElementName = "SALTANDBATCHSUFFIX";
            XmlNode formElementNode = rootSelectedNode.SelectSingleNode(coeFormPath + "/COE:formElement[@name='" + formElementName + "']", manager);
            if (formElementNode == null)
            {
                formElementNode = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "formElement", "COE.FormGroup");
                createNewAttribute("name", formElementName, ref formElementNode);

                // create child nodes
                XmlNode label = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "label", "COE.FormGroup");
                label.InnerText = "Salt and Batch Suffix";
                XmlNode showHelp = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "showHelp", "COE.FormGroup");
                showHelp.InnerText = "false";
                XmlNode isFileUpload = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "isFileUpload", "COE.FormGroup");
                isFileUpload.InnerText = "false";
                XmlNode pageComunicationProvider = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "pageComunicationProvider", "COE.FormGroup");
                XmlNode fileUploadBindingExpression = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "fileUploadBindingExpression", "COE.FormGroup");
                XmlNode helpText = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "helpText", "COE.FormGroup");
                XmlNode defaultValue = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "defaultValue", null);
                XmlNode bindingExpression = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "bindingExpression", "COE.FormGroup");
                bindingExpression.InnerText = "PropertyList[@Name='SALTANDBATCHSUFFIX'| Value]";
                XmlNode id = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Id", "COE.FormGroup");
                id.InnerText = "SALTANDBATCHSUFFIXProperty";

                XmlNode displayInfo = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "displayInfo", "COE.FormGroup");
                //  sub child nodes
                //--------------------
                displayInfo.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "cssClass", "COE.FormGroup"));
                displayInfo.SelectSingleNode("COE:cssClass", manager).InnerText = "Std20x40";
                displayInfo.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "type", "COE.FormGroup"));
                displayInfo.SelectSingleNode("COE:type", manager).InnerText = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.SaltAndBacthSuffixTextBox";
                displayInfo.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "assembly", "COE.FormGroup"));
                displayInfo.SelectSingleNode("COE:assembly", manager).InnerText = "CambridgeSoft.COE.RegistrationWebApp";
                displayInfo.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "visible", "COE.FormGroup"));
                displayInfo.SelectSingleNode("COE:visible", manager).InnerText = "true";
                //--------------------

                XmlNode validationRuleList = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "validationRuleList", "COE.FormGroup");
                XmlNode serverEvents = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "serverEvents", "COE.FormGroup");
                XmlNode clientEvents = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "clientEvents", "COE.FormGroup");

                XmlNode configInfo = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "configInfo", "COE.FormGroup");
                // sub child
                //--------------------
                XmlNode fieldConfig = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "fieldConfig", "COE.FormGroup");
                fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "CSSLabelClass", "COE.FormGroup"));
                fieldConfig.SelectSingleNode("COE:CSSLabelClass", manager).InnerText = "FELabel";
                fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "CSSClass", "COE.FormGroup"));
                switch (formMode.ToLower())
                {
                    case "addmode":
                        fieldConfig.SelectSingleNode("COE:CSSClass", manager).InnerText = "FETextBox";
                        fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "TextMode", "COE.FormGroup"));
                        fieldConfig.SelectSingleNode("COE:TextMode", manager).InnerText = "SingleLine";
                        fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "LegacyFieldDisplayLimit", "COE.FormGroup"));
                        fieldConfig.SelectSingleNode("COE:LegacyFieldDisplayLimit", manager).InnerText = "100000";

                        break;
                    case "editmode":
                        fieldConfig.SelectSingleNode("COE:CSSClass", manager).InnerText = "FETextBox";
                        fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "TextMode", "COE.FormGroup"));
                        fieldConfig.SelectSingleNode("COE:TextMode", manager).InnerText = "SingleLine";
                        fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "LegacyFieldDisplayLimit", "COE.FormGroup"));
                        fieldConfig.SelectSingleNode("COE:LegacyFieldDisplayLimit", manager).InnerText = "100000";
                        break;
                    case "viewmode":
                        fieldConfig.SelectSingleNode("COE:CSSClass", manager).InnerText = "FETextBoxViewMode";
                        fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Enable", "COE.FormGroup"));
                        fieldConfig.SelectSingleNode("COE:Enable", manager).InnerText = "False";
                        fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "LegacyFieldDisplayLimit", "COE.FormGroup"));
                        fieldConfig.SelectSingleNode("COE:LegacyFieldDisplayLimit", manager).InnerText = "100000";
                        fieldConfig.AppendChild(rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "ReadOnly", "COE.FormGroup"));
                        fieldConfig.SelectSingleNode("COE:ReadOnly", manager).InnerText = "true";
                        break;
                }
                configInfo.AppendChild(fieldConfig);
                //--------------------

                XmlNode dataSource = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "dataSource", "COE.FormGroup");
                XmlNode dataSourceId = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "dataSourceId", "COE.FormGroup");
                XmlNode requiredStyle = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "requiredStyle", "COE.FormGroup");
                XmlNode displayData = rootSelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, "displayData", null);

                formElementNode.AppendChild(label);
                formElementNode.AppendChild(showHelp);
                formElementNode.AppendChild(isFileUpload);
                formElementNode.AppendChild(pageComunicationProvider);
                formElementNode.AppendChild(fileUploadBindingExpression);
                formElementNode.AppendChild(helpText);
                formElementNode.AppendChild(defaultValue);
                formElementNode.AppendChild(bindingExpression);
                formElementNode.AppendChild(id);
                formElementNode.AppendChild(displayInfo);
                formElementNode.AppendChild(validationRuleList);
                formElementNode.AppendChild(serverEvents);
                formElementNode.AppendChild(clientEvents);
                formElementNode.AppendChild(configInfo);
                formElementNode.AppendChild(dataSource);
                formElementNode.AppendChild(dataSourceId);
                formElementNode.AppendChild(requiredStyle);
                formElementNode.AppendChild(displayData);

                // attach new node at specified position or append
                XmlNode insertAfterNode = null;
                XmlNode insertBeforeNode = rootSelectedNode.SelectSingleNode(coeFormPath + "/COE:formElement[@name='" + insertBeforeText + "']", manager);
                if (insertAfterNode != null)
                {
                    rootSelectedNode.InsertAfter(formElementNode, insertAfterNode);
                }
                else if (insertBeforeNode != null)
                {
                    rootSelectedNode.InsertBefore(formElementNode, insertBeforeNode);
                }
                else
                {
                    rootSelectedNode.AppendChild(formElementNode);
                }
                return "Form[" + xmlId + ":" + formMode.ToUpper() + "]:The formelement node with name='" + formElementName + "' added succesfully.";
            }
            else
            {
                return "Form[" + xmlId + ":" + formMode.ToUpper() + "]:The formelement node with name='" + formElementName + "' already exits.";
            }
        }
        #endregion

        #region Private Method
        private void createNewAttribute(string attributeName, string attributeValue, ref XmlNode node)
        {
            XmlAttribute attributes = node.OwnerDocument.CreateAttribute(attributeName);
            node.Attributes.Append(attributes);
            node.Attributes[attributeName].Value = attributeValue;
        }

        #endregion

    }
}
