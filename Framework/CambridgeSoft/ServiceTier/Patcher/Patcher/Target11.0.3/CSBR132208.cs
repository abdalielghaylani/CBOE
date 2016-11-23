using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-132208: There is no option to Search by Project name in Search Temp and Review Register forms.
    /// 
    /// Steps to Reproduce:
    /// 
    /// 1. Login to CBOE application
    /// 2. Click on Search Temp/Review register
    /// 3. View the Serach Temporory records form
    /// 
    /// Bug: There is no Project name field to search the records on Project name.
    /// Expected result: The Project name field should be available to Search the records on Project name search criteria.
    /// 
    /// Note: In Search permanent records form, Project name field is available to have a search on Project name.
    /// </summary>
    public class CSBR132208 : BugFixBaseCommand
    {
        /// <summary>
        /// Manual steps to fix
        /// 
        /// Goto customize forms and add the following formElements in your query forms for search temp and permanent:
        /// (Note: You must change the form elements if sufix Temp by Perm and, on permanent from group the Batch_Projects form element must         ///  be located on coeFrom id =2 
        /// &lt;formElement name="REGISTRY_PROJECT"&gt;
        ///     &lt;label&gt;Project Name&lt;/label&gt;
        ///     &lt;showHelp&gt;false&lt;/showHelp&gt;
        ///     &lt;helpText/&gt;
        ///     &lt;defaultValue/&gt;
        ///     &lt;bindingExpression&gt;SearchCriteria.[30].Criterium.Value&lt;/bindingExpression&gt;
        ///     &lt;Id&gt;REGISTRY_PROJECTDropDownListTemp&lt;/Id&gt;
        ///     &lt;displayInfo&gt;
        ///     	&lt;cssClass&gt;Std25x40&lt;/cssClass&gt;
        ///     	&lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList&lt;/type&gt;
        ///     	&lt;visible&gt;true&lt;/visible&gt;
        ///     &lt;/displayInfo&gt;
        ///     &lt;validationRuleList/&gt;
        ///     &lt;serverEvents/&gt;
        ///     &lt;clientEvents/&gt;
        ///     &lt;configInfo&gt;
        ///     	&lt;fieldConfig&gt;
        ///     		&lt;dropDownItemsSelect&gt;SELECT PROJECTID as key, NAME as value FROM REGDB.VW_PROJECT WHERE (TYPE ='R' OR TYPE='A')&lt;/dropDownItemsSelect&gt;
        ///     		&lt;CSSLabelClass&gt;FELabel&lt;/CSSLabelClass&gt;
        ///     		&lt;CSSClass&gt;FEDropDownList&lt;/CSSClass&gt;
        ///     		&lt;Enable&gt;True&lt;/Enable&gt;
        ///     		&lt;ID&gt;REGISTRY_PROJECTDropDownList&lt;/ID&gt;
        ///     		&lt;AutoPostBack&gt;False&lt;/AutoPostBack&gt;
        ///     	&lt;/fieldConfig&gt;
        ///     &lt;/configInfo&gt;
        ///     &lt;dataSource/&gt;
        ///     &lt;dataSourceId/&gt;
        ///     &lt;searchCriteriaItem fieldid="602" id="30" tableid="6"&gt;
        ///     	&lt;numericalCriteria negate="NO" trim="NONE" operator="EQUAL"/&gt;
        ///     &lt;/searchCriteriaItem&gt;
        ///     &lt;displayData/&gt;
        /// &lt;/formElement&gt;
        /// &lt;formElement name="BATCH_PROJECT"&gt;
        ///     &lt;label&gt;Project Name&lt;/label&gt;
        ///     &lt;showHelp&gt;false&lt;/showHelp&gt;
        ///     &lt;helpText/&gt;
        ///     &lt;defaultValue/&gt;
        ///     &lt;bindingExpression&gt;SearchCriteria.[31].Criterium.Value&lt;/bindingExpression&gt;
        ///     &lt;Id&gt;BATCH_PROJECTDropDownListTemp&lt;/Id&gt;
        ///     &lt;displayInfo&gt;
        ///         &lt;cssClass&gt;Std25x40&lt;/cssClass&gt;
        ///         &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList&lt;/type&gt;
        ///         &lt;visible&gt;true&lt;/visible&gt;
        ///     &lt;/displayInfo&gt;
        ///     &lt;validationRuleList/&gt;
        ///     &lt;serverEvents/&gt;
        ///     &lt;clientEvents/&gt;
        ///     &lt;configInfo&gt;
        ///         &lt;fieldConfig&gt;
        ///             &lt;dropDownItemsSelect&gt;SELECT PROJECTID as key, NAME as value FROM REGDB.VW_PROJECT WHERE (TYPE ='B' OR TYPE='A')&lt;/dropDownItemsSelect&gt;
        ///             &lt;CSSLabelClass&gt;FELabel&lt;/CSSLabelClass&gt;
        ///             &lt;CSSClass&gt;FEDropDownList&lt;/CSSClass&gt;
        ///             &lt;Enable&gt;True&lt;/Enable&gt;
        ///             &lt;ID&gt;Inner_BATCH_PROJECTDropDownList&lt;/ID&gt;
        ///             &lt;AutoPostBack&gt;False&lt;/AutoPostBack&gt;
        ///         &lt;/fieldConfig&gt;
        ///     &lt;/configInfo&gt;
        ///     &lt;dataSource/&gt;
        ///     &lt;dataSourceId/&gt;
        ///     &lt;searchCriteriaItem fieldid="602" id="31" tableid="6"&gt;
        ///         &lt;numericalCriteria negate="NO" trim="NONE" operator="EQUAL"/&gt;
        ///     &lt;/searchCriteriaItem&gt;
        ///     &lt;displayData/&gt;
        /// &lt;/formElement&gt;
        /// 
        /// Then edit dataview 4002, adding the following tables:
        /// 
        /// &lt;table id="5" name="VW_PROJECT" alias="VW_PROJECT" database="REGDB" primaryKey="500"&gt;
        /// 	&lt;fields id="500" name="PROJECTID" dataType="INTEGER" alias="PROJECTID" indexType="NONE" mimeType="NONE" visible="1"/&gt;
        /// 	&lt;fields id="501" name="NAME" dataType="TEXT" alias="NAME" indexType="NONE" mimeType="NONE" visible="1"/&gt;
        /// 	&lt;fields id="502" name="ACTIVE" dataType="INTEGER" alias="Active" indexType="NONE" mimeType="NONE" visible="1"/&gt;
        /// &lt;/table&gt;
        /// &lt;table id="6" name="VW_TEMPORARYBATCHPROJECT" alias="VW_TEMPORARYBATCHPROJECT" database="REGDB" primaryKey="600"&gt;
        ///     &lt;fields id="600" name="ID" dataType="INTEGER" alias="ID" indexType="NONE" mimeType="NONE" visible="1"/&gt;
        /// 	&lt;fields id="601" name="TEMPBATCHID" dataType="INTEGER" alias="TEMPBATCHID" indexType="NONE" mimeType="NONE" visible="1"/&gt;
        /// 	&lt;fields id="602" name="PROJECTID" dataType="INTEGER" lookupFieldId="500" lookupDisplayFieldId="501" alias="PROJECTID" indexType="NONE" mimeType="NONE" visible="1"/&gt;
        /// &lt;/table&gt;
        /// 
        /// And the following relationships:
        /// 
        /// &lt;relationship child="5" parent="6" childkey="500" parentkey="602" jointype="INNER"/&gt;
        /// &lt;relationship child="6" parent="1" childkey="601" parentkey="100" jointype="INNER"/&gt;
        /// 
        /// Finaly edit dataview 4003, adding the following table:
        /// 
        /// &lt;table id="13" name="VW_BATCH_PROJECT" alias="VW_BATCH_PROJECT" database="REGDB" primaryKey="1300"&gt;
        ///     &lt;fields id="1300" name="ID" dataType="INTEGER" alias="ID" indexType="NONE" mimeType="NONE" visible="1"/&gt;
        ///     &lt;fields id="1301" name="BATCHID" dataType="INTEGER" alias="BATCHID" indexType="NONE" mimeType="NONE" visible="1"/&gt;
        ///     &lt;fields id="1302" name="PROJECTID" dataType="INTEGER" lookupFieldId="1200" lookupDisplayFieldId="1201" alias="PROJECTID" indexType="NONE" mimeType="NONE" visible="1"/&gt;
        /// &lt;/table&gt;
        /// 
        /// And the following relationship:
        /// 
        /// &lt;relationship child="13" parent="9" childkey="1301" parentkey="900" jointype="INNER"/&gt;
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="dataviews"></param>
        /// <param name="configurations"></param>
        /// <param name="objectConfig"></param>
        /// <param name="frameworkConfig"></param>
        /// <returns></returns>
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            #region COEForms
            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                if (id == "4002")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");

                    XmlNode layoutInfo = doc.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo", manager);
                    XmlNode creationDate = layoutInfo.SelectSingleNode("./COE:formElement[@name='CREATION_DATE']", manager);

                    XmlNode registryProjectElement = layoutInfo.SelectSingleNode("./COE:formElement[@name='REGISTRY_PROJECT']", manager);
                    if (registryProjectElement != null)
                    {
                        errorsInPatch = true;
                        messages.Add("Registry_Project item was already present in form id=4002");
                    }
                    else
                    {
                        registryProjectElement = doc.CreateNode(XmlNodeType.Element, "formElement", doc.DocumentElement.NamespaceURI);
                        XmlAttribute nameRP = doc.CreateAttribute("name");
                        nameRP.Value = "REGISTRY_PROJECT";
                        registryProjectElement.Attributes.Append(nameRP);
                        registryProjectElement.InnerXml = @"<label xmlns=""COE.FormGroup"">Registry Project Name</label>
<showHelp xmlns=""COE.FormGroup"">false</showHelp>
<helpText xmlns=""COE.FormGroup""/>
<defaultValue xmlns=""COE.FormGroup""/>
<bindingExpression xmlns=""COE.FormGroup"">SearchCriteria.[30].Criterium.Value</bindingExpression>
<Id xmlns=""COE.FormGroup"">REGISTRY_PROJECTDropDownListTemp</Id>
<displayInfo xmlns=""COE.FormGroup"">
<cssClass xmlns=""COE.FormGroup"">Std25x40</cssClass>
<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList</type>
<visible xmlns=""COE.FormGroup"">true</visible>
</displayInfo>
<validationRuleList xmlns=""COE.FormGroup""/>
<serverEvents xmlns=""COE.FormGroup""/>
<clientEvents xmlns=""COE.FormGroup""/>
<configInfo xmlns=""COE.FormGroup"">
<fieldConfig xmlns=""COE.FormGroup"">
<dropDownItemsSelect xmlns=""COE.FormGroup"">SELECT PROJECTID as key, NAME as value FROM REGDB.VW_PROJECT WHERE (TYPE ='R' OR TYPE='A') AND ACTIVE = 'T'</dropDownItemsSelect>
<CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
<CSSClass xmlns=""COE.FormGroup"">FEDropDownList</CSSClass>
<Enable xmlns=""COE.FormGroup"">True</Enable>
<ID xmlns=""COE.FormGroup"">REGISTRY_PROJECTDropDownList</ID>
<AutoPostBack xmlns=""COE.FormGroup"">False</AutoPostBack>
</fieldConfig>
</configInfo>
<dataSource xmlns=""COE.FormGroup""/>
<dataSourceId xmlns=""COE.FormGroup""/>
<searchCriteriaItem fieldid=""702"" id=""30"" tableid=""7"" xmlns=""COE.FormGroup"">
<numericalCriteria negate=""NO"" trim=""NONE"" operator=""EQUAL"" xmlns=""COE.FormGroup""/>
</searchCriteriaItem>
<displayData xmlns=""COE.FormGroup""/>";
                        if (creationDate != null)
                        {
                            layoutInfo.InsertBefore(registryProjectElement, creationDate);
                            messages.Add("Registry_Project dropdownlist added to form id=4002");
                        }
                        else
                        {
                            layoutInfo.AppendChild(registryProjectElement);
                            messages.Add("Registry_Project dropdownlist added to form id=4002 but in a different position than expected");
                            errorsInPatch = true;
                        }

                        XmlNode batchProjectElement = layoutInfo.SelectSingleNode("./COE:formElement[@name='BATCH_PROJECT']", manager);
                        if (batchProjectElement != null)
                        {
                            errorsInPatch = true;
                            messages.Add("Batch_Project item was already present in form id=4002");
                        }
                        else
                        {
                            batchProjectElement = doc.CreateNode(XmlNodeType.Element, "formElement", doc.DocumentElement.NamespaceURI);
                            XmlAttribute nameBP = doc.CreateAttribute("name");
                            nameBP.Value = "BATCH_PROJECT";
                            batchProjectElement.Attributes.Append(nameBP);
                            batchProjectElement.InnerXml = @"<label xmlns=""COE.FormGroup"">Batch Project Name</label>
<showHelp xmlns=""COE.FormGroup"">false</showHelp>
<helpText xmlns=""COE.FormGroup""/>
<defaultValue xmlns=""COE.FormGroup""/>
<bindingExpression xmlns=""COE.FormGroup"">SearchCriteria.[31].Criterium.Value</bindingExpression>
<Id xmlns=""COE.FormGroup"">BATCH_PROJECTDropDownListTemp</Id>
<displayInfo xmlns=""COE.FormGroup"">
<cssClass xmlns=""COE.FormGroup"">Std25x40</cssClass>
<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList</type>
<visible xmlns=""COE.FormGroup"">true</visible>
</displayInfo>
<validationRuleList xmlns=""COE.FormGroup""/>
<serverEvents xmlns=""COE.FormGroup""/>
<clientEvents xmlns=""COE.FormGroup""/>
<configInfo xmlns=""COE.FormGroup"">
<fieldConfig xmlns=""COE.FormGroup"">
<dropDownItemsSelect xmlns=""COE.FormGroup"">SELECT PROJECTID as key, NAME as value FROM REGDB.VW_PROJECT WHERE (TYPE ='B' OR TYPE='A') AND ACTIVE = 'T'</dropDownItemsSelect>
<CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
<CSSClass xmlns=""COE.FormGroup"">FEDropDownList</CSSClass>
<Enable xmlns=""COE.FormGroup"">True</Enable>
<ID xmlns=""COE.FormGroup"">BATCH_PROJECTDropDownList</ID>
<AutoPostBack xmlns=""COE.FormGroup"">False</AutoPostBack>
</fieldConfig>
</configInfo>
<dataSource xmlns=""COE.FormGroup""/>
<dataSourceId xmlns=""COE.FormGroup""/>
<searchCriteriaItem fieldid=""602"" id=""31"" tableid=""6"" xmlns=""COE.FormGroup"">
<numericalCriteria negate=""NO"" trim=""NONE"" operator=""EQUAL"" xmlns=""COE.FormGroup""/>
</searchCriteriaItem>
<displayData xmlns=""COE.FormGroup""/>";
                            if (creationDate != null)
                            {
                                layoutInfo.InsertBefore(batchProjectElement, creationDate);
                                messages.Add("Batch_Project dropdownlist added to form id=4002");
                            }
                            else
                            {
                                layoutInfo.AppendChild(batchProjectElement);
                                errorsInPatch = true;
                                messages.Add("Batch_Project dropdownlist added to form id=4002 but in a different position than expected");
                            }
                        }
                    }
                }
                else if (id == "4003")
                {
                    XmlNamespaceManager managerPerm = new XmlNamespaceManager(doc.NameTable);
                    managerPerm.AddNamespace("COE", "COE.FormGroup");
                    XmlNode layoutInfoPerm = doc.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo", managerPerm);

                    XmlNode oldProject = layoutInfoPerm.SelectSingleNode("./COE:formElement[@name='PROJECT']", managerPerm);


                    XmlNode registryProjectElementPerm = layoutInfoPerm.SelectSingleNode("./COE:formElement[@name='REGISTRY_PROJECT']", managerPerm);
                    if (registryProjectElementPerm != null)
                    {
                        errorsInPatch = true;
                        messages.Add("Registry_Project item was already present in form id=4003");
                    }
                    else
                    {
                        registryProjectElementPerm = doc.CreateNode(XmlNodeType.Element, "formElement", doc.DocumentElement.NamespaceURI);
                        XmlAttribute nameRP = doc.CreateAttribute("name");
                        nameRP.Value = "REGISTRY_PROJECT";
                        registryProjectElementPerm.Attributes.Append(nameRP);
                        registryProjectElementPerm.InnerXml = @"<label xmlns=""COE.FormGroup"">Registry Project Name</label>
<showHelp xmlns=""COE.FormGroup"">false</showHelp>
<helpText xmlns=""COE.FormGroup""/>
<defaultValue xmlns=""COE.FormGroup""/>
<bindingExpression xmlns=""COE.FormGroup"">SearchCriteria[6].Criterium.Value</bindingExpression>
<Id xmlns=""COE.FormGroup"">REGISTRY_PROJECTDropDownListPerm</Id>
<displayInfo xmlns=""COE.FormGroup""><cssClass>Std25x40</cssClass>
<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList</type>
<visible xmlns=""COE.FormGroup"">true</visible>
</displayInfo>
<validationRuleList xmlns=""COE.FormGroup""/>
<serverEvents xmlns=""COE.FormGroup""/>
<clientEvents xmlns=""COE.FormGroup""/>
<configInfo xmlns=""COE.FormGroup"">
<fieldConfig xmlns=""COE.FormGroup"">
<dropDownItemsSelect xmlns=""COE.FormGroup"">SELECT PROJECTID as key, NAME as value FROM REGDB.VW_PROJECT WHERE (TYPE ='R' OR TYPE='A') AND ACTIVE = 'T'</dropDownItemsSelect>
<CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
<CSSClass xmlns=""COE.FormGroup"">FEDropDownList</CSSClass>
<Enable xmlns=""COE.FormGroup"">True</Enable>
<ID xmlns=""COE.FormGroup"">REGISTRY_PROJECTDropDownListPerm</ID>
<AutoPostBack xmlns=""COE.FormGroup"">False</AutoPostBack>
</fieldConfig>
</configInfo>
<dataSource xmlns=""COE.FormGroup""/>
<dataSourceId xmlns=""COE.FormGroup""/>
<searchCriteriaItem fieldid=""302"" id=""6"" tableid=""3"" xmlns=""COE.FormGroup"">
<numericalCriteria negate=""NO"" trim=""NONE"" operator=""EQUAL"" xmlns=""COE.FormGroup""/>
</searchCriteriaItem>
<displayData xmlns=""COE.FormGroup""/>";

                        if (oldProject != null)
                        {
                            layoutInfoPerm.ReplaceChild(registryProjectElementPerm, oldProject);
                            messages.Add("Project dropdownlist replaced by Registry_Project dropdownlist on form id=4003");
                        }
                        else
                        {
                            layoutInfoPerm.AppendChild(registryProjectElementPerm);
                            errorsInPatch = true;
                            messages.Add("Registry_Project dropdownlist added to form id=4003 but in a different position than expected");
                        }
                    }

                    XmlNode layoutInfoPerm2 = doc.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:layoutInfo", managerPerm);
                    XmlNode creationDate = layoutInfoPerm2.SelectSingleNode("./COE:formElement[@name='CREATION_DATE']", managerPerm);

                    XmlNode batchProjectElementPerm = layoutInfoPerm2.SelectSingleNode("./COE:formElement[@name='BATCH_PROJECT']", managerPerm);
                    if (batchProjectElementPerm != null)
                    {
                        errorsInPatch = true;
                        messages.Add("Batch_Project item was already present in form id=4003");
                    }
                    else
                    {
                        batchProjectElementPerm = doc.CreateNode(XmlNodeType.Element, "formElement", doc.DocumentElement.NamespaceURI);
                        XmlAttribute nameBPP = doc.CreateAttribute("name");
                        nameBPP.Value = "BATCH_PROJECT";
                        batchProjectElementPerm.Attributes.Append(nameBPP);
                        batchProjectElementPerm.InnerXml = @"<label xmlns=""COE.FormGroup"">Batch Project Name</label>
<showHelp xmlns=""COE.FormGroup"">false</showHelp>
<helpText xmlns=""COE.FormGroup""/>
<defaultValue xmlns=""COE.FormGroup""/>
<bindingExpression xmlns=""COE.FormGroup"">SearchCriteria[38].Criterium.Value</bindingExpression>
<Id xmlns=""COE.FormGroup"">BATCH_PROJECTDropDownListPerm</Id>
<displayInfo xmlns=""COE.FormGroup"">
<cssClass xmlns=""COE.FormGroup"">Std25x40</cssClass>
<type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList</type>
<visible xmlns=""COE.FormGroup"">true</visible>
</displayInfo>
<validationRuleList xmlns=""COE.FormGroup""/>
<serverEvents xmlns=""COE.FormGroup""/>
<clientEvents xmlns=""COE.FormGroup""/>
<configInfo xmlns=""COE.FormGroup"">
<fieldConfig xmlns=""COE.FormGroup"">
<dropDownItemsSelect xmlns=""COE.FormGroup"">SELECT PROJECTID as key, NAME as value FROM REGDB.VW_PROJECT WHERE (TYPE ='B' OR TYPE='A') AND ACTIVE = 'T'</dropDownItemsSelect>
<CSSLabelClass xmlns=""COE.FormGroup"">FELabel</CSSLabelClass>
<CSSClass xmlns=""COE.FormGroup"">FEDropDownList</CSSClass>
<Enable xmlns=""COE.FormGroup"">True</Enable>
<ID xmlns=""COE.FormGroup"">Inner_BATCH_PROJECTDropDownListPerm</ID>
<AutoPostBack xmlns=""COE.FormGroup"">False</AutoPostBack>
</fieldConfig>
</configInfo>
<dataSource xmlns=""COE.FormGroup""/>
<dataSourceId xmlns=""COE.FormGroup""/>
<searchCriteriaItem fieldid=""1302"" id=""38"" tableid=""13"" xmlns=""COE.FormGroup"">
<numericalCriteria negate=""NO"" trim=""NONE"" operator=""EQUAL"" xmlns=""COE.FormGroup""/>
</searchCriteriaItem>
<displayData xmlns=""COE.FormGroup""/>";

                        if (creationDate != null)
                        {
                            layoutInfoPerm2.InsertBefore(batchProjectElementPerm, creationDate);
                            messages.Add("Batch_Project dropdownlist added to form id=4003");
                        }
                        else
                        {
                            errorsInPatch = true;
                            layoutInfoPerm2.AppendChild(batchProjectElementPerm);
                            messages.Add("Batch_Project dropdownlist added to form id=4003 but in a different position than expected");
                        }
                    }
                }
            }

            #endregion

            #region Dataviews
            foreach (XmlDocument doc in dataviews)
            {
                string id = doc.DocumentElement.Attributes["dataviewid"] == null ? string.Empty : doc.DocumentElement.Attributes["dataviewid"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.COEDataView");
                #region Dataview 4002
                if (id == "4002")
                {
                    XmlNode tables = doc.SelectSingleNode("//COE:tables", manager);
                    XmlNode tableProject = doc.SelectSingleNode("//COE:table[@id='5']", manager);
                    if (tableProject != null)
                    {
                        errorsInPatch = true;
                        messages.Add("Table with id=5 was already present in dataview 4002 and was not added");
                    }
                    else
                    {
                        tableProject = doc.CreateElement("table", doc.DocumentElement.NamespaceURI);
                        XmlAttribute idAttr = doc.CreateAttribute("id");
                        idAttr.Value = "5";
                        XmlAttribute name = doc.CreateAttribute("name");
                        name.Value = "VW_PROJECT";
                        XmlAttribute alias = doc.CreateAttribute("alias");
                        alias.Value = "VW_PROJECT";
                        XmlAttribute database = doc.CreateAttribute("database");
                        database.Value = "REGDB";
                        XmlAttribute primaryKey = doc.CreateAttribute("primaryKey");
                        primaryKey.Value = "500";

                        tableProject.Attributes.Append(idAttr);
                        tableProject.Attributes.Append(name);
                        tableProject.Attributes.Append(alias);
                        tableProject.Attributes.Append(database);
                        tableProject.Attributes.Append(primaryKey);

                        tableProject.InnerXml = @"<fields id=""500"" name=""PROJECTID"" dataType=""INTEGER"" alias=""PROJECTID"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" xmlns=""COE.COEDataView""/>
			<fields id=""501"" name=""NAME"" dataType=""TEXT"" alias=""NAME"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" xmlns=""COE.COEDataView""/>
			<fields id=""502"" name=""ACTIVE"" dataType=""INTEGER"" alias=""Active"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" xmlns=""COE.COEDataView""/>";

                        tables.AppendChild(tableProject);

                        messages.Add("Table with id=5 was successfully added to dataview 4002");
                    }

                    XmlNode tableBatch = doc.SelectSingleNode("//COE:table[@id='6']", manager);
                    if (tableBatch != null)
                    {
                        errorsInPatch = true;
                        messages.Add("Table with id=6 was already present in dataview 4002 and was not added");
                    }
                    else
                    {
                        tableBatch = doc.CreateElement("table", doc.DocumentElement.NamespaceURI);
                        XmlAttribute idAttr = doc.CreateAttribute("id");
                        idAttr.Value = "6";
                        XmlAttribute name = doc.CreateAttribute("name");
                        name.Value = "VW_TEMPORARYBATCHPROJECT";
                        XmlAttribute alias = doc.CreateAttribute("alias");
                        alias.Value = "VW_TEMPORARYBATCHPROJECT";
                        XmlAttribute database = doc.CreateAttribute("database");
                        database.Value = "REGDB";
                        XmlAttribute primaryKey = doc.CreateAttribute("primaryKey");
                        primaryKey.Value = "600";

                        tableBatch.Attributes.Append(idAttr);
                        tableBatch.Attributes.Append(name);
                        tableBatch.Attributes.Append(alias);
                        tableBatch.Attributes.Append(database);
                        tableBatch.Attributes.Append(primaryKey);

                        tableBatch.InnerXml = @"<fields id=""600"" name=""ID"" dataType=""INTEGER"" alias=""ID"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" xmlns=""COE.COEDataView""/>
			<fields id=""601"" name=""TEMPBATCHID"" dataType=""INTEGER"" alias=""TEMPBATCHID"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" xmlns=""COE.COEDataView""/>
			<fields id=""602"" name=""PROJECTID"" dataType=""INTEGER"" lookupFieldId=""500"" lookupDisplayFieldId=""501"" alias=""PROJECTID"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" xmlns=""COE.COEDataView""/>";

                        tables.AppendChild(tableBatch);

                        messages.Add("Table with id=6 was successfully added to dataview 4002");
                    }

                    XmlNode relationships = doc.SelectSingleNode("//COE:relationships", manager);
                    XmlNode table5Rel = doc.SelectSingleNode("//COE:relationship[@child='5']", manager);
                    if (table5Rel != null)
                    {
                        errorsInPatch = true;
                        messages.Add("Relationship for table id=5 was already added in dataview 4002");
                    }
                    else
                    {
                        //<relationship child="5" parent="6" childkey="500" parentkey="602" jointype="INNER"/>
                        table5Rel = doc.CreateElement("relationship", doc.DocumentElement.NamespaceURI);
                        XmlAttribute child = doc.CreateAttribute("child");
                        child.Value = "5";
                        XmlAttribute parent = doc.CreateAttribute("parent");
                        parent.Value = "6";
                        XmlAttribute childKey = doc.CreateAttribute("childkey");
                        childKey.Value = "500";
                        XmlAttribute parentkey = doc.CreateAttribute("parentkey");
                        parentkey.Value = "602";
                        XmlAttribute joinType = doc.CreateAttribute("jointype");
                        joinType.Value = "INNER";

                        table5Rel.Attributes.Append(child);
                        table5Rel.Attributes.Append(parent);
                        table5Rel.Attributes.Append(childKey);
                        table5Rel.Attributes.Append(parentkey);
                        table5Rel.Attributes.Append(joinType);

                        relationships.AppendChild(table5Rel);

                        messages.Add("Relationship for table 5 as child succesfully added in dataview 4002");
                    }
                    XmlNode table6Rel = doc.SelectSingleNode("//COE:relationship[@child='6']", manager);
                    if (table6Rel != null)
                    {
                        errorsInPatch = true;
                        messages.Add("Relationship for table id=6 was already added in dataview 4002");
                    }
                    else
                    {
                        //<relationship child="6" parent="1" childkey="601" parentkey="100" jointype="INNER"/>
                        table6Rel = doc.CreateElement("relationship", doc.DocumentElement.NamespaceURI);
                        XmlAttribute child = doc.CreateAttribute("child");
                        child.Value = "6";
                        XmlAttribute parent = doc.CreateAttribute("parent");
                        parent.Value = "1";
                        XmlAttribute childKey = doc.CreateAttribute("childkey");
                        childKey.Value = "601";
                        XmlAttribute parentkey = doc.CreateAttribute("parentkey");
                        parentkey.Value = "100";
                        XmlAttribute joinType = doc.CreateAttribute("jointype");
                        joinType.Value = "INNER";

                        table6Rel.Attributes.Append(child);
                        table6Rel.Attributes.Append(parent);
                        table6Rel.Attributes.Append(childKey);
                        table6Rel.Attributes.Append(parentkey);
                        table6Rel.Attributes.Append(joinType);

                        relationships.AppendChild(table6Rel);

                        messages.Add("Relationship for table 6 as child succesfully added in dataview 4002");
                    }
                }
                #endregion
                #region Dataview 4003
                else if (id == "4003")
                {
                    XmlNode tables = doc.SelectSingleNode("//COE:tables", manager);
                    XmlNode tableBatchProject = doc.SelectSingleNode("//COE:table[@id='13']", manager);
                    if (tableBatchProject != null)
                    {
                        errorsInPatch = true;
                        messages.Add("Table with id=13 was already present in dataview 4003 and was not added");
                    }
                    else
                    {
                        tableBatchProject = doc.CreateElement("table", doc.DocumentElement.NamespaceURI);
                        XmlAttribute idAttr = doc.CreateAttribute("id");
                        idAttr.Value = "13";
                        XmlAttribute name = doc.CreateAttribute("name");
                        name.Value = "VW_BATCH_PROJECT";
                        XmlAttribute alias = doc.CreateAttribute("alias");
                        alias.Value = "VW_BATCH_PROJECT";
                        XmlAttribute database = doc.CreateAttribute("database");
                        database.Value = "REGDB";
                        XmlAttribute primaryKey = doc.CreateAttribute("primaryKey");
                        primaryKey.Value = "1300";

                        tableBatchProject.Attributes.Append(idAttr);
                        tableBatchProject.Attributes.Append(name);
                        tableBatchProject.Attributes.Append(alias);
                        tableBatchProject.Attributes.Append(database);
                        tableBatchProject.Attributes.Append(primaryKey);

                        tableBatchProject.InnerXml = @"<fields id=""1300"" name=""ID"" dataType=""INTEGER"" alias=""ID"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" xmlns=""COE.COEDataView""/>
			<fields id=""1301"" name=""BATCHID"" dataType=""INTEGER"" alias=""BATCHID"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" xmlns=""COE.COEDataView""/>
			<fields id=""1302"" name=""PROJECTID"" dataType=""INTEGER"" lookupFieldId=""1200"" lookupDisplayFieldId=""1201"" alias=""PROJECTID"" indexType=""NONE"" mimeType=""NONE"" visible=""1"" xmlns=""COE.COEDataView""/>";

                        tables.AppendChild(tableBatchProject);

                        messages.Add("Table with id=13 was successfully added to dataview 4003");
                    }

                    XmlNode relationships = doc.SelectSingleNode("//COE:relationships", manager);
                    XmlNode table13Rel = doc.SelectSingleNode("//COE:relationship[@child='13']", manager);
                    if (table13Rel != null)
                    {
                        errorsInPatch = true;
                        messages.Add("Relationship for table id=13 was already added in dataview 4003");
                    }
                    else
                    {
                        //<relationship child="13" parent="9" childkey="1301" parentkey="900" jointype="INNER"/>
                        table13Rel = doc.CreateElement("relationship", doc.DocumentElement.NamespaceURI);
                        XmlAttribute child = doc.CreateAttribute("child");
                        child.Value = "13";
                        XmlAttribute parent = doc.CreateAttribute("parent");
                        parent.Value = "9";
                        XmlAttribute childKey = doc.CreateAttribute("childkey");
                        childKey.Value = "1301";
                        XmlAttribute parentkey = doc.CreateAttribute("parentkey");
                        parentkey.Value = "900";
                        XmlAttribute joinType = doc.CreateAttribute("jointype");
                        joinType.Value = "INNER";

                        table13Rel.Attributes.Append(child);
                        table13Rel.Attributes.Append(parent);
                        table13Rel.Attributes.Append(childKey);
                        table13Rel.Attributes.Append(parentkey);
                        table13Rel.Attributes.Append(joinType);

                        relationships.AppendChild(table13Rel);

                        messages.Add("Relationship for table 5 as child succesfully added in dataview 4003");
                    }
                }
                #endregion
            }
            #endregion

            if (!errorsInPatch)
            {
                messages.Add("CSBR132208 was successfully patched");
            }
            else
                messages.Add("CSBR132208 was patched with errors");
            return messages;
        }
    }
}

