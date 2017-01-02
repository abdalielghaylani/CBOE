using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;
using System.Xml.Serialization;
using System.IO;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Configuration;
using System.Xml;
using Oracle.DataAccess.Client;


namespace CambridgeSoft.COE.Framework.COETableEditorService
{
    public static class COETableEditorUtilities
    {
        #region Properties

        static COEConfigurationManager configurationManager;

        private static COEConfigurationManager GetConfigurationManager()
        {
            if (configurationManager == null)
            {
                configurationManager = new COEConfigurationManager();
            }

            return configurationManager;
        }

        #endregion

        #region Static methods

        #region Static methods - TableEditor
        /// <summary>
        /// The function need to be called by DAL in order to set the name of table.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="tableNameString"></param>
        /// <param name="tableName"></param>
        public static void BuildCOETableEditorTableName(string tableNameString, ref string tableName)
        {
            if (!(tableNameString == string.Empty || tableNameString == null))
            {
                    tableName = tableNameString;
              
                ConfigurationManager.AppSettings.Set("TableEditorTableName", tableName);
            }
            else
            {
                if (ConfigurationManager.AppSettings.Get("TableEditorTableName") != null)
                    tableName = ConfigurationManager.AppSettings.Get("TableEditorTableName").ToString();
            }

        }

        /// <summary>
        /// To get all tables in TableEditor.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string,string> getTables()
        {
            COEConfigurationSettings coeConfigSettings = (COEConfigurationSettings)GetConfigurationManager().GetSection(null, "CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COENamedElementCollection<ApplicationData> myApps = coeConfigSettings.Applications;
            ApplicationData appName = myApps.Get(COEAppName.Get());
            IEnumerator<COETableEditor> en = appName.TableEditor.GetEnumerator();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            while (en.MoveNext())
            {
                COETableEditor te = (COETableEditor)en.Current;
                if (!te.DisableTable)
                    dict.Add(te.Name, string.IsNullOrEmpty(te.DisplayName) ? te.Name : te.DisplayName);
            }
            return dict;
        }

        /// <summary>
        /// The field is has RequiredField Validation
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool isRequied(string tableName, string fieldName)
        {
            List<ValidationRule> validationRuleList = COETableEditorUtilities.getValidationRuleList(tableName, fieldName);
            if (validationRuleList != null)
            {
                foreach (ValidationRule validationRule in validationRuleList)
                {
                    if (validationRule.Name.ToLower() == "requiredField".ToLower())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// The field is has ChemicallyValid Validation
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool GetHasChemValidValidation(string tableName, string fieldName)
        {
            List<ValidationRule> validationRuleList = COETableEditorUtilities.getValidationRuleList(tableName, fieldName);
            if (validationRuleList != null)
            {
                foreach (ValidationRule validationRule in validationRuleList)
                {
                    if (validationRule.Name.ToLower() == "chemicallyValid".ToLower())
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public static string GetAppDataBase()
        {
            COEConfigurationSettings coeConfigSettings = (COEConfigurationSettings)GetConfigurationManager().GetSection(null, "CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COENamedElementCollection<ApplicationData> myApps = coeConfigSettings.Applications;
            ApplicationData appName = myApps.Get(COEAppName.Get());
            return appName.Database;
        }

        #endregion

        #region Static methods - Table

        /// <summary>
        /// To get column's list of the table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static List<Column> getColumnList(string tableName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            IEnumerator<COETableEditorData> en = te.TableEditorData.GetEnumerator();
            List<Column> ColList = new List<Column>();

            while (en.MoveNext())
            {
                COETableEditorData teData = (COETableEditorData)en.Current;
                Column col = null;
                switch (teData.DataType.ToLower())//updated on 2008/04/03 for table manager debug
                {
                    case "string":
                    case "formula":
                        col = new Column(teData.Name, DbType.AnsiString);
                        break;
                    case "number":
                    case "molweight":
                        col = new Column(teData.Name, DbType.Double);
                        break;
                    case "date":
                        col = new Column(teData.Name, DbType.DateTime);
                        break;
                }
                ColList.Add(col);
            }
            return ColList;
        }

        /// <summary>
        /// To get the IdFieldName of the table. 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string getIdFieldName(string tableName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            return te.PrimaryKey;
        }
        
        /// <summary>
        /// Get's the maximum page size allowed.
        /// </summary>
        /// <param name="tableName">The table name for which the value is to be retrieved</param>
        /// <returns>The maximum page size allowed if configured, int.MaxValue otherwhise</returns>
        public static int GetMaxPageSize(string tableName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            if(!string.IsNullOrEmpty(te.MaxPageSize))
                return int.Parse(te.MaxPageSize);
            else
                return int.MaxValue;
        }

        /// <summary>
        ///  ValidationRule property of field
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static List<ValidationRule> getValidationRuleList(string tableName, string fieldName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            COETableEditorData teData = te.TableEditorData.Get(fieldName);
            List<ValidationRule> validationRuleList = new List<ValidationRule>();

            if (teData.ValidationRule.Count > 0)
            {
                IEnumerator<ValidationRule> en = teData.ValidationRule.GetEnumerator();
                while (en.MoveNext())
                {
                    ValidationRule validationRule = (ValidationRule)en.Current;
                    validationRuleList.Add(validationRule);
                }
            }
            else
            {
                validationRuleList = null;
            }
            return validationRuleList;
        }

        /// <summary>
        /// Parameter property of field
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static List<Parameter> getParameterList(string tableName, string fieldName, string validationRuleName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            COETableEditorData teData = te.TableEditorData.Get(fieldName);
            List<Common.Parameter> parameterList = new List<Common.Parameter>();
            if (teData.ValidationRule.Get(validationRuleName).Parameter.Count > 0)
            {
                IEnumerator<Parameter> en = teData.ValidationRule.Get(validationRuleName).Parameter.GetEnumerator();
                while (en.MoveNext())
                {
                    Parameter parameter = (Parameter)en.Current;
                    parameterList.Add(parameter);
                }
            }
            else
            {
                parameterList = null;
            }
            return parameterList;
        }


        /// <summary>
        /// IsStructure property of field
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool getIsStructure(string tableName, string fieldName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            COETableEditorData teData = te.TableEditorData.Get(fieldName);
            if (teData.IsStructure.ToLower().Equals("true"))
                return true;
            else
                return false;
        }
        
        /// <summary>
        /// IsStructure property of field
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool GetIsMolWeight(string tableName, string fieldName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            COETableEditorData teData = te.TableEditorData.Get(fieldName);
            return teData.DataType.ToLower() == "molweight";
        }

        /// <summary>
        /// IsStructure property of field
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool GetIsFormula(string tableName, string fieldName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            COETableEditorData teData = te.TableEditorData.Get(fieldName);
            return teData.DataType.ToLower() == "formula";
        }
        /// <summary>
        /// SequenceName property of table
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string getSequenceName(string tableName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            return te.SequenceName;
        }
        
        /// <summary>
        /// SequenceName property of table
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string getTableDescription(string tableName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            return te.Description;
        }

        /// <summary>
        /// get COETableEditor
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static COETableEditor GetCOETableEditor(string tableName)
        {
            COEConfigurationSettings coeConfigSettings = (COEConfigurationSettings)GetConfigurationManager().GetSection(null, "CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COENamedElementCollection<ApplicationData> myApps = coeConfigSettings.Applications;
            ApplicationData appName = myApps.Get(COEAppName.Get());
            COETableEditor te = appName.TableEditor.Get(tableName);
            return te;
        }

        /// <summary>
        /// Indicates if current user can edit a given table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool HasEditPrivileges(string tableName)
        {
            COETableEditor tableInfo = GetCOETableEditor(tableName);
            if(tableInfo != null)
            {
                if(string.IsNullOrEmpty(tableInfo.EditPrivileges))
                    return true;

                foreach(string priv in tableInfo.EditPrivileges.Split(new string[] { "||" }, StringSplitOptions.None))
                {
                    if(!string.IsNullOrEmpty(priv.Trim()))
                    {
                        if(CambridgeSoft.COE.Framework.COESecurityService.COEPrincipal.HasPrivilege(priv.Trim(), string.Empty))
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Indicates if current user can add rows to a given table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool HasAddPrivileges(string tableName)
        {
            COETableEditor tableInfo = GetCOETableEditor(tableName);
            if(tableInfo != null)
            {
                if(string.IsNullOrEmpty(tableInfo.EditPrivileges))
                    return true;

                foreach(string priv in tableInfo.AddPrivileges.Split(new string[] { "||" }, StringSplitOptions.None))
                {
                    if(!string.IsNullOrEmpty(priv.Trim()))
                    {
                        if(CambridgeSoft.COE.Framework.COESecurityService.COEPrincipal.HasPrivilege(priv.Trim(), string.Empty))
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Indicates if current user can delete rows from a given table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool HasDeletePrivileges(string tableName)
        {
            COETableEditor tableInfo = GetCOETableEditor(tableName);
            if(tableInfo != null)
            {
                if(string.IsNullOrEmpty(tableInfo.EditPrivileges))
                    return true;

                foreach(string priv in tableInfo.DeletePrivileges.Split(new string[] { "||" }, StringSplitOptions.None))
                {
                    if(!string.IsNullOrEmpty(priv.Trim()))
                    {
                        if(CambridgeSoft.COE.Framework.COESecurityService.COEPrincipal.HasPrivilege(priv.Trim(), string.Empty))
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///  Get alias of column.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string GetAlias(string tableName, string fieldName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            COETableEditorData teData = te.TableEditorData.Get(fieldName);
            if (teData.Alias == null || teData.Alias.Length == 0)
                return null;
            return teData.Alias;
        }

        #endregion

        #region Static methods - Lookup
        /// <summary>
        /// LookupField for the Particular Field.
        /// </summary>
        /// <param name="tableName">Table name whose field need to be resolved</param>
        /// <param name="fieldName">Field Name for which lookup need to be resolved</param>
        /// <returns></returns>
        public static string getLookupField(string tableName, string fieldName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            COETableEditorData teData = te.TableEditorData.Get(fieldName);

            if (teData.LookupField.Length != 0)
            {
                return teData.LookupField.Substring(teData.LookupField.LastIndexOf('.') + 1);
            }
            return teData.LookupField;
        }

        /// <summary>
        /// LookupFilter for lookupFields.
        /// </summary>
        /// <param name="tableName">Table name whose field need to be resolved</param>
        /// <param name="fieldName">Field Name for which lookup need to be resolved</param>
        /// <returns></returns>
        public static string getLookupFilter(string tableName, string fieldName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            COETableEditorData teData = te.TableEditorData.Get(fieldName);

            if (string.IsNullOrEmpty(teData.LookupFilter))
            {
                return string.Empty;
            }
            return teData.LookupFilter;
        }

        /// <summary>
        /// To get LookupTableName for the Particular Field.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string getLookupTableName(string tableName, string fieldName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            COETableEditorData teData = te.TableEditorData.Get(fieldName);
            if (teData.LookupField.Length != 0)
            {
                return teData.LookupField.Substring(0, teData.LookupField.LastIndexOf('.'));
            }
            
            return teData.LookupField;
        }

        /// <summary>
        /// To get LookupID for the Particular Field.
        /// </summary>
        /// <param name="tableName">Table name whose field need to be resolved</param>
        /// <param name="fieldName">Field Name for which lookup need to be resolved</param>
        /// <returns></returns>
        public static string getLookupID(string tableName, string fieldName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            COETableEditorData teData = te.TableEditorData.Get(fieldName);
            if (teData.LookupID.Length != 0)
            {
                return teData.LookupID.Substring(teData.LookupID.LastIndexOf('.') + 1);
            }
            return teData.LookupID;
        }

        /// <summary>
        /// To get LookupColumnList for the Particular Field.
        /// </summary>
        /// <param name="tableName">Table name whose field need to be resolved</param>
        /// <param name="fieldName">Field Name for which lookup need to be resolved</param>
        /// <returns></returns>
        public static List<Column> getLookupColumnList(string tableName, string FieldName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            COETableEditorData teData = te.TableEditorData.Get(FieldName);
            List<Column> ColList = new List<Column>();

            //Currently is assumed that All the ID field need to be looked up at this point. All the field would be resolved as String Field.

            Column colLookupID = new Column(getLookupID(tableName, FieldName), DbType.Int16);
            Column colLookupField = new Column(getLookupField(tableName, FieldName), DbType.AnsiString);

            ColList.Add(colLookupID);
            ColList.Add(colLookupField);
            return ColList;
        }

        /// <summary>
        /// IsStructureLookupField property of field
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool getIsStructureLookupField(string tableName, string fieldName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            COETableEditorData teData = te.TableEditorData.Get(fieldName);
            if (teData.IsStructureLookupField.ToLower().Equals("true"))
                return true;
            else
                return false;

        }

        /// <summary>
        /// Defaultvalue property of field
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string getDefaultValue(string tableName, string fieldName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            COETableEditorData teData = te.TableEditorData.Get(fieldName);
            return  FrameworkUtils.IsAValidToken(teData.DefaultValue) ? FrameworkUtils.ReplaceSpecialTokens(teData.DefaultValue, false) : teData.DefaultValue; // Replace if any special tokens used in defaultValue.
        }

        /// <summary>
        /// LookupLocation property of field 
        /// </summary>
        /// <param name="tableName">Table name whose field need to be resolved</param>
        /// <param name="fieldName">Field Name for which lookup need to be resolved</param>
        /// <returns></returns>
        public static string getLookupLocation(string tableName, string fieldName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            COETableEditorData teData = te.TableEditorData.Get(fieldName);

            return teData.LookupLocation;
        }

        /// <summary>
        /// Hidden property of field 
        /// </summary>
        /// <param name="tableName">Table name whose field need to be resolved</param>
        /// <param name="fieldName">Field Name for which visibility need to be resolved</param>
        /// <returns></returns>
        public static bool GetHiddenProperty(string tableName, string fieldName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            COETableEditorData teData = te.TableEditorData.Get(fieldName);

            return teData.Hidden;
        }

        /// <summary>
        /// Is Unique property
        /// </summary>
        /// <returns></returns>
        public static bool GetIsUniqueProperty(string tableName, string fieldName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            COETableEditorData teData = te.TableEditorData.Get(fieldName);
            return teData.IsUnique;
        }
        
        /// <summary>
        /// IsUsedCheck property
        /// </summary>
        /// <param name="tableName">Table name whose field need to be resolved</param>
        /// <param name="fieldName">Field Name for which visibility need to be resolved</param>
        /// <returns></returns>
        public static string GetIsUsedCheckProperty(string tableName, string fieldName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            COETableEditorData teData = te.TableEditorData.Get(fieldName);
            return teData.IsUsedCheck;
        }

        /// <summary>
        /// LookupLocation property of field 
        /// </summary>
        /// <param name="tableName">Table name whose field need to be resolved</param>
        /// <param name="fieldName">Field Name for which lookup need to be resolved</param>
        /// <returns></returns>
        public static List<ID_Column> getId_Column_List(string tableName, string fieldName)
        {
            string lookupLocation = COETableEditorUtilities.getLookupLocation(tableName, fieldName);
            string lookupTableName = lookupLocation.Substring(lookupLocation.IndexOf("_") + 1);
            List<ID_Column> Id_Column_List = new List<ID_Column>();
            COEConfigurationSettings coeConfigSettings = (COEConfigurationSettings)GetConfigurationManager().GetSection(null, "CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COENamedElementCollection<ApplicationData> myApps = coeConfigSettings.Applications;
            ApplicationData appName = myApps.Get(COEAppName.Get());

            InnerXml inXml = appName.InnerXml.Get(lookupTableName);
            if (inXml.InnerXmlData.Count > 0)
            {
                IEnumerator<InnerXmlData> en = inXml.InnerXmlData.GetEnumerator();
                while (en.MoveNext())
                {
                    InnerXmlData tmpInnerXmlData = (InnerXmlData)en.Current;
                    Column column = new Column("Test", DbType.AnsiString);
                    column.FieldValue = tmpInnerXmlData.Display;
                    Id_Column_List.Add(new ID_Column(tmpInnerXmlData.Value, column));
                }
            }
            else
            {
                Id_Column_List = null;
            }
            return Id_Column_List;
        }

        /// <summary>
        /// getting lookup value from xml file in coeFrameWork when the lookuplocation property is a xml file
        /// </summary>
        /// <param name="tableName">Table name whose field need to be resolved</param>
        /// <param name="fieldValue">the value of related cell in the webgrid</param>
        /// <param name="fieldName">Field Name for which lookup need to be resolved</param>
        /// <returns></returns>
        public static string getLookupValueFromInnerXML(string tableName, string fieldValue, string fieldName)
        {
            string lookupLocation = COETableEditorUtilities.getLookupLocation(tableName, fieldName);
            string lookupTableName = lookupLocation.Substring(lookupLocation.IndexOf("_") + 1);

            string returnValue = string.Empty;
            string strLookupIdValue = string.Empty;

            COEConfigurationSettings coeConfigSettings = (COEConfigurationSettings)GetConfigurationManager().GetSection(null, "CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COENamedElementCollection<ApplicationData> myApps = coeConfigSettings.Applications;
            ApplicationData appName = myApps.Get(COEAppName.Get());

            InnerXml inXml = appName.InnerXml.Get(lookupTableName);

            if (inXml.InnerXmlData.Count > 0)
            {
                IEnumerator<InnerXmlData> en = inXml.InnerXmlData.GetEnumerator();
                while (en.MoveNext())
                {
                    InnerXmlData tmpInnerXmlData = (InnerXmlData)en.Current;
                    strLookupIdValue = tmpInnerXmlData.Value;
                    if (strLookupIdValue.ToLower() == fieldValue.ToLower())
                        returnValue = tmpInnerXmlData.Display;
                }
            }
            return returnValue;
        }

        /// <summary>
        /// getting lookup value from xml file when the lookuplocation property is a xml file
        /// </summary>
        /// <param name="tableName">Table name whose field need to be resolved</param>
        /// <param name="fieldValue">the value of related cell in the webgrid</param>
        /// <param name="fieldName">Field Name for which lookup need to be resolved</param>
        /// <returns></returns>
        public static string getLookupValueFromXML(string tableName, string fieldValue, string fieldName)
        {
            string lookupID = COETableEditorUtilities.getLookupID(tableName, fieldName);
            lookupID = lookupID.Substring(lookupID.LastIndexOf(".") + 1);
            string lookupField = COETableEditorUtilities.getLookupField(tableName, fieldName);
            lookupField = lookupField.Substring(lookupField.LastIndexOf(".") + 1);
            string lookupLocation = COETableEditorUtilities.getLookupLocation(tableName, fieldName);

            string returnValue = string.Empty;
            XmlNode idNode = null;
            string strLookupIdValue = string.Empty;

            XmlDocument _xmlDocument = new XmlDocument();
            _xmlDocument.Load(lookupLocation);
            XmlNodeList rowXmlNodeList = _xmlDocument.DocumentElement.ChildNodes;

            foreach (XmlNode rowElement in rowXmlNodeList)
            {
                idNode = rowElement.SelectSingleNode(lookupID);
                strLookupIdValue = idNode.InnerText.Trim();
                if (strLookupIdValue.ToLower() == fieldValue.ToLower())
                    returnValue = rowElement.SelectSingleNode(lookupField).InnerText.Trim();
            }
            return returnValue;
        }

        #endregion

        #region Static methods - ChildTable

        /// <summary>
        /// To determine whether the table has ChildTable.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool GetIsHasChildTable(string tableName)
        {
            COETableEditor te = COETableEditorUtilities.GetCOETableEditor(tableName);
            if (te.ChildTable.Count > 0)
                return true;
            else
                return false;
        }
        
		/// <summary>
        /// Get ChildTable.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns>COEChildTable</returns>
        public static COEChildTable GetChildTable(string tableName, int index)
        {
            COEConfigurationSettings coeConfigSettings = (COEConfigurationSettings)GetConfigurationManager().GetSection(null, "CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COENamedElementCollection<ApplicationData> myApps = coeConfigSettings.Applications;
            ApplicationData appName = myApps.Get(COEAppName.Get());
            COETableEditor te = appName.TableEditor.Get(tableName);

            return te.ChildTable.Get(index);
        }

        /// <summary>
        /// Verifies if the child tables are enabled/disabled 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns>boolean true if Child Tables are enabled</returns>
        public static bool ChildTablesEnabled(string tableName)
        {
            COEConfigurationSettings coeConfigSettings = (COEConfigurationSettings)GetConfigurationManager().GetSection(null, "CambridgeSoft.COE.Framework", COEConfigurationSettings.SectionName);
            COENamedElementCollection<ApplicationData> myApps = coeConfigSettings.Applications;
            ApplicationData appName = myApps.Get(COEAppName.Get());
            COETableEditor te = appName.TableEditor.Get(tableName);

            if (te.DisableChildTables == true)
                return false;
            else
                return true;
        }

        /// <summary>
        /// To get the colunm list of ChildTable.
        /// </summary>
        /// <param name="Childtable"></param>
        /// <returns></returns>
        public static List<Column> GetChildTableColumnList(COEChildTable Childtable)
        {
            IEnumerator<COEChildTableData> Enumerator = Childtable.ChildTableData.GetEnumerator();
            List<Column> ColumnList = new List<Column>();

            while (Enumerator.MoveNext())
            {
                COEChildTableData ChildTableData = Enumerator.Current;
                Column col = null;
                switch (ChildTableData.DataType.ToLower())
                {
                    case "string":
                        col = new Column(ChildTableData.Name, DbType.AnsiString);
                        break;
                    case "number":
                        col = new Column(ChildTableData.Name, DbType.Double);
                        break;
                    case "date":
                        col = new Column(ChildTableData.Name, DbType.DateTime);
                        break;
                }
                ColumnList.Add(col);
            }
            return ColumnList;
        }

        /// <summary>
        /// To get the name of LinkTable.
        /// </summary>
        /// <param name="Childtable"></param>
        /// <returns></returns>
        public static string GetLinkTableName(COEChildTable Childtable)
        {
            string childPK = Childtable.ChildPK;
            return childPK.Substring(0, childPK.LastIndexOf("."));
        }

        #endregion 

        #endregion
    }
}