using System;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;

using Csla;
using Csla.Core;
using Csla.Data;
using Csla.Validation;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.IniParser;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.Common.Configuration;
using CambridgeSoft.COE.Framework.ExceptionHandling;

using CambridgeSoft.COE.RegistrationAdmin.Services.Common;

using CambridgeSoft.COE.Registration.Services.AddIns;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Framework.COEConfigurationService;

namespace CambridgeSoft.COE.RegistrationAdmin.Services
{
    /// <summary>
    /// TODO: Document why this service class exists and give a brief overview of what it does
    /// </summary>
    [Serializable()]
    public class ConfigurationRegistryRecord : RegAdminBusinessBase<ConfigurationRegistryRecord>
    {
        #region Variables

        private PropertyList _batchPropertyList;
        private PropertyList _batchComponentList;
        private PropertyList _compoundPropertyList;
        private PropertyList _propertyList;
        private PropertyList _structurePropertyList;
        private AddInList _addInList;
        private PropertyListType _selectedPropertyList;
        private string _errorLog = string.Empty;
        private string _errorLogToShow = string.Empty;
        private string[] _eventNames;
        private AssemblyList _addInsAssemblyList;
        private string _xml;
        [NonSerialized, NotUndoable]
        private DALFactory _dalFactory = new DALFactory();
        [NonSerialized]
        private string _serviceName = "COERegistrationAdmin";
        private Dictionary<string, string>[] _propertiesLabels = { 
            new Dictionary<string, string>(),
            new Dictionary<string, string>(),
            new Dictionary<string, string>(),
            new Dictionary<string, string>(),
            new Dictionary<string, string>()
        };
        private COEFormHelper _coeFormHelper;
        private bool _isImported = false;
        private List<string> _databaseReservedWords = null;
        private List<string> _batchPropertyColumnList = null;
        private List<string> _batchComponentColumnList = null;
        private List<string> _compoundPropertyColumnList = null;
        private List<string> _propertyColumnList = null;
        private List<string> _structurePropertyColumnList = null;

        private Dictionary<string, string> _checkPropertyStatusInDB = new Dictionary<string, string>(); //  Enum Selected propertyListType  <string [PropertyName], string [ErrorLog]>.
        private string _propertyStatusLog = string.Empty;
        private string _selectedPropertyName = string.Empty;
        private string _defalutValueForProp = string.Empty;
        private bool _saveDefaultValue = false;
        private bool _isConfigSavedSuccessfully = true;

        private const string COEFORMSFOLDERNAME = "COEForms";
        private const string COEDATAVIEWSFOLDERNAME = "COEDataViews";
        private const string COETABLESFORLDERNAME = "COETables";
        private const string COEOBJECTCONFIGFILENAME = "COEObjectConfig.xml";
        private const string CONFIGSETTINGSFILENAME = "ConfigurationSettings.xml";
        private const string IMPORTFILESPATH = "\\Config\\default";
        private const string EXPORTFILESPATH = "\\COERegistrationExportFiles\\";
        private const string IMPORTINIFILEPATH = "\\Config\\COE10Migration";
        private const string EXCEPTIONCODE = "LocalImportError ";
        private const string FIXEDINSTALLPATH = "Registration";
        private const string FILE_SEARCH_PATTERN = "*.xml";
        private bool _forceImport = false;
        #endregion

        #region Properties

        /// <summary>
        /// Contains an Database reserved words list
        /// </summary>
        public List<string> DatabaseReservedWords 
        {
            get {return _databaseReservedWords;}
        }

        public bool IsImported
        {
            get
            {
                return _isImported;
            }
            set
            {
                _isImported = value;
            }
        }

        public bool IsConfigSavedSuccessfully
        {
            get
            {
                return _isConfigSavedSuccessfully;
            }
            set
            {
                _isConfigSavedSuccessfully = value;
            }
        }

        private bool SaveDefaultValue
        {
            get
            {
                return _saveDefaultValue;
            }
            set
            {
                _saveDefaultValue = value;
            }
        }

        public void  CleanUpConfiguration()
        {
            // Clear Validation Business rules
            _checkPropertyStatusInDB.Clear();
            this.PropertyStatusLog = string.Empty;
            this.SelectedPropertyName = string.Empty;
            this.DefalutValue = string.Empty;
        }

        private string PropertyStatusLog
        {
            get
            {
                return _propertyStatusLog;
            }
            set
            {
                _propertyStatusLog = value;
            }
        }

        public string SelectedPropertyName
        {
            get
            {
                CanReadProperty(true);
                return _selectedPropertyName;
            }
            set
            {
                CanWriteProperty(true);
                _selectedPropertyName = value;
            }
        }

        public string DefalutValue
        {
            get
            {
                CanReadProperty(true);
                return _defalutValueForProp;
            }
            set
            {
                CanWriteProperty (true);
                _defalutValueForProp = value;
                SaveDefaultValue = (_defalutValueForProp.Trim().Length > 0) ? true : false;
            }

        }

        public bool IsPropValidToAddValidator
        {
            get
            {
                CanReadProperty(true);
                if (SelectedPropertyName.Trim().Length == 0)
                {
                    this.MarkDirty();
                    return true;
                }
                return GetPropertyStatus(this.SelectedPropertyList, SelectedPropertyName);
            }
        }

        
        public string GetSaveErrorMessage
        {
            get
            {
                return _errorLogToShow;
            }
        }

        public Dictionary<string, string>[] PropertiesLabels
        {
            get
            {
                return _propertiesLabels;
            }
        }

        public override bool IsSavable
        {
            get
            {
                if (IsDirty)
                    return true;
                else
                    return false;

            }
        }

        public override bool IsDirty
        {
            get
            {
                return _addInList.IsDirty || _batchComponentList.IsDirty || _batchPropertyList.IsDirty || _compoundPropertyList.IsDirty || _propertyList.IsDirty || _structurePropertyList.IsDirty;
            }
        }

        public AssemblyList GetAssemblyList
        {
            get
            {
                return _addInsAssemblyList;
            }
        }

        public string[] EventNames
        {
            get
            {
                return _eventNames;
            }
        }

        public PropertyListType SelectedPropertyList
        {
            get
            {
                CanReadProperty(true);
                return _selectedPropertyList;
            }
            set
            {
                CanWriteProperty(true);
                _selectedPropertyList = value;
                PropertyHasChanged();
                this.MarkClean();
            }
        }

        public PropertyList GetSelectedPropertyList
        {
            get
            {
                CanReadProperty(true);
                switch ((int)_selectedPropertyList)
                {
                    case 0: return _batchPropertyList;
                    case 1: return _batchComponentList;
                    case 2: return _compoundPropertyList;
                    case 5: return _propertyList;
                    case 6: return _structurePropertyList;
                    default: return null;
                }
            }
            set
            {
                if (value != null)
                {
                    CanWriteProperty(true);
                    switch ((int)_selectedPropertyList)
                    {
                        case 0:
                            _batchPropertyList = value;
                            break;
                        case 1:
                            _batchComponentList = value;
                            break;
                        case 2:
                            _compoundPropertyList = value;
                            break;
                        case 5:
                            _propertyList = value;
                            break;
                        case 6:
                            _structurePropertyList = value;
                            break;
                    }

                }
            }
        }

        public List<string> PropertyColumnList
        {
            get
            {
                CanReadProperty(true);
                return _propertyColumnList;
            }
        }

        public List<string> BatchPropertyColumnList
        {
            get
            {
                CanReadProperty(true);
                return _batchPropertyColumnList;
            }
        }

        public List<string> BatchComponentColumnList
        {
            get
            {
                CanReadProperty(true);
                return _batchComponentColumnList;
            }
        }

        public List<string> CompoundPropertyColumnList
        {
            get
            {
                CanReadProperty(true);
                return _compoundPropertyColumnList;
            }
        }

        public List<string> StructurePropertyColumnList
        {
            get
            {
                CanReadProperty(true);
                return _structurePropertyColumnList;
            }
        }

        public PropertyList PropertyList
        {
            get
            {
                CanReadProperty(true);
                return _propertyList;
            }
        }

        public PropertyList BatchPropertyList
        {
            get
            {
                CanReadProperty(true);
                return _batchPropertyList;
            }
        }

        public PropertyList BatchComponentList
        {
            get
            {
                CanReadProperty(true);
                return _batchComponentList;
            }
        }

        public PropertyList CompoundPropertyList
        {
            get
            {
                CanReadProperty(true);
                return _compoundPropertyList;
            }
        }

        public PropertyList StructurePropertyList
        {
            get
            {
                CanReadProperty(true);
                return _structurePropertyList;
            }
        }

        public AddInList AddInList
        {
            get
            {
                CanReadProperty(true);
                return _addInList;
            }
        }

        public FormGroup FormGroup
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.FormGroup;
            }
        }

        public FormGroup.Form MixtureForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.MixtureForm;
            }
        }

        public FormGroup.Form CompoundForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.CompoundForm;
            }
        }

        public FormGroup.Form BatchForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.BatchForm;
            }
        }

        public FormGroup.Form BatchComponentForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.BatchComponentForm;
            }
        }

        public FormGroup.Form SearchTempBaseQueryForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.SearchTempBaseQueryForm;
            }
        }

        public FormGroup.Form SearchTempChildQueryForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.SearchTempChildQueryForm;
            }
        }

        public FormGroup.Form SearchTempDetailsBaseForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.SearchTempDetailsBaseForm;
            }
        }

        public FormGroup.Form SearchTempDetailsChildForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.SearchTempDetailsChildForm;
            }
        }

        public FormGroup.Form SearchPermMixtureQueryForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.SearchPermMixtureQueryForm;
            }
        }

        public FormGroup.Form SearchPermCompoundQueryForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.SearchPermCompoundQueryForm;
            }
        }

        public FormGroup.Form SearchPermBatchQueryForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.SearchPermBatchQueryForm;
            }
        }

        public FormGroup.Form SearchPermBatchComponentQueryForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.SearchPermBatchComponentQueryForm;
            }
        }

        public FormGroup.Form SearchPermMixtureDetailForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.SearchPermMixtureDetailForm;
            }
        }

        public FormGroup.Form SearchPermCompoundDetailForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.SearchPermCompoundDetailForm;
            }
        }

        public FormGroup.Form SearchPermBatchDetailForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.SearchPermBatchDetailForm;
            }
        }

        public FormGroup.Form SearchPermBatchComponentDetailForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.SearchPermBatchComponentDetailForm;
            }
        }

        public FormGroup.Form SearchTempListForm
        {
            get
            {
                if (_coeFormHelper == null)
                    _coeFormHelper = new COEFormHelper(this);
                return _coeFormHelper.SearchTempListForm;
            }
        }

        public FormGroup.Form SerachPermanentListForm
        {
            get
            {
                if (_coeFormHelper == null)
                    _coeFormHelper = new COEFormHelper(this);
                return _coeFormHelper.SearchPermanentListForm;
            }
        }

        public FormGroup.Form ELNSearchTempBaseQueryForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.ELNSearchTempBaseQueryForm;
            }
        }

        public FormGroup.Form ELNSearchTempChildQueryForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.ELNSearchTempChildQueryForm;
            }
        }

        public FormGroup.Form ELNSearchTempDetailsBaseForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.ELNSearchTempDetailsBaseForm;
            }
        }

        public FormGroup.Form ELNSearchTempDetailsChildForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.ELNSearchTempDetailsChildForm;
            }
        }

        public FormGroup.Form ELNSearchPermMixtureQueryForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.ELNSearchPermMixtureQueryForm;
            }
        }

        public FormGroup.Form ELNSearchPermCompoundQueryForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.ELNSearchPermCompoundQueryForm;
            }
        }

        public FormGroup.Form ELNSearchPermBatchQueryForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.ELNSearchPermBatchQueryForm;
            }
        }

        public FormGroup.Form ELNSearchPermBatchComponentQueryForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.ELNSearchPermBatchComponentQueryForm;
            }
        }

        public FormGroup.Form ELNSearchPermMixtureDetailForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.ELNSearchPermMixtureDetailForm;
            }
        }

        public FormGroup.Form ELNSearchPermCompoundDetailForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.ELNSearchPermCompoundDetailForm;
            }
        }

        public FormGroup.Form ELNSearchPermBatchDetailForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.ELNSearchPermBatchDetailForm;
            }
        }

        public FormGroup.Form ELNSearchPermBatchComponentDetailForm
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper.ELNSearchPermBatchComponentDetailForm;
            }
        }

        public FormGroup.Form ELNSearchTempListForm
        {
            get
            {
                if (_coeFormHelper == null)
                    _coeFormHelper = new COEFormHelper(this);
                return _coeFormHelper.ELNSearchTempListForm;
            }
        }

        public FormGroup.Form ELNSerachPermanentListForm
        {
            get
            {
                if (_coeFormHelper == null)
                    _coeFormHelper = new COEFormHelper(this);
                return _coeFormHelper.ELNSearchPermanentListForm;
            }
        }

        public FormGroup.Form DataLoaderForm
        {
            get
            {
                if (_coeFormHelper == null)
                    _coeFormHelper = new COEFormHelper(this);
                return _coeFormHelper.DataLoaderForm;
            }
        }

        public FormGroup.Form ComponentDuplicatesFormGroup
        {
            get
            {
                if (_coeFormHelper == null)
                    _coeFormHelper = new COEFormHelper(this);
                return _coeFormHelper.ComponentDuplicatesForm;
            }
        }

        public FormGroup.Form RegistryDuplicates
        {
            get
            {
                if (_coeFormHelper == null)
                    _coeFormHelper = new COEFormHelper(this);
                return _coeFormHelper.RegistryDuplicatesForm;
            }
        }

        public COEFormHelper COEFormHelper
        {
            get
            {
                if (_coeFormHelper == null)
                {
                    _coeFormHelper = new COEFormHelper(this);
                }
                return _coeFormHelper;
            }
        }

        public string[] GetTableNames
        {
            get
            {
                return FrameworkUtils.GetAppConfigSetting("REGISTRATION", "REGADMIN", "TableNameList").Split('|');
            }
        }

        #endregion

        #region Business Method

        private void CheckErrorLog()
        {
            StringBuilder errorBuilder = new StringBuilder();
            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(_errorLog);
            XmlNode errors = xmlData.SelectSingleNode("./ErrorList");

            PropertyList registriesPropToDelete = PropertyList.NewPropertyList();
            PropertyList componentPropToDelete = PropertyList.NewPropertyList();
            PropertyList batchPropToDelete = PropertyList.NewPropertyList();
            PropertyList batchCompPropToDelete = PropertyList.NewPropertyList();
            PropertyList structurePropToDelete = PropertyList.NewPropertyList();

            string existingType = string.Empty;
            string existingPrecision = string.Empty;
            //string diferentPrecisionMessage = " property can´t be added because already existed on database and their presicion is:"; TODO
            //string diferentTypeMessage = " property can´t be added because already exist on database and their type is:"; TODO
            string diferentPrecisionMessage = " property was added but already existed on database and their presicion is:";
            string diferentTypeMessage = " property was added but already existed on database and their type is:";

            const string DIFPRECISIONERRORCODE = "-20003";
            const string DIFTYPEERRORCODE = "-20004";

            foreach (XmlNode error in errors)
            {
                string operation = error.SelectSingleNode("./Operation").InnerText;

                string field = error.SelectSingleNode("./Field").InnerText;

                string section = error.SelectSingleNode("./Section").InnerText;

                string errorCode = error.SelectSingleNode("./ErrorCode").InnerText;

                string message = error.SelectSingleNode("./Message").InnerText;

                int deletedCount;
                List<int> toUnDelete;
                switch (operation)
                {
                    case "dropping":
                        switch (section)
                        {
                            case "MIXTURE":
                                deletedCount = _propertyList.GetDeletedList().Count;
                                toUnDelete = new List<int>();
                                for (int i = 0; i < deletedCount; i++)
                                {
                                    if (_propertyList.GetDeletedList()[i].Name == field)
                                    {
                                        toUnDelete.Add(i);
                                        errorBuilder.Append("Registry - " + _propertyList.GetDeletedList()[i].FriendlyName + " can´t be deleted <br/>");
                                        IsConfigSavedSuccessfully = false;
                                    }
                                }
                                foreach (int index in toUnDelete)
                                {
                                    _propertyList.GetDeletedList()[index].SortOrder = -1;
                                    _propertyList.AddProperty(_propertyList.GetDeletedList()[index]);
                                    _propertyList.GetDeletedList().RemoveAt(index);
                                }
                                break;

                            case "COMPOUND":
                                deletedCount = _compoundPropertyList.GetDeletedList().Count;
                                toUnDelete = new List<int>();
                                for (int i = 0; i < deletedCount; i++)
                                {
                                    if (_compoundPropertyList.GetDeletedList()[i].Name == field)
                                    {
                                        toUnDelete.Add(i);
                                        errorBuilder.Append("Compound - " + _compoundPropertyList.GetDeletedList()[i].FriendlyName + " can´t be deleted <br/>");
                                        IsConfigSavedSuccessfully = false;
                                    }
                                }
                                foreach (int index in toUnDelete)
                                {
                                    _compoundPropertyList.GetDeletedList()[index].SortOrder = -1;
                                    _compoundPropertyList.AddProperty(_compoundPropertyList.GetDeletedList()[index]);
                                    _compoundPropertyList.GetDeletedList().RemoveAt(index);
                                }
                                break;

                            case "BATCH":
                                deletedCount = _batchPropertyList.GetDeletedList().Count;
                                toUnDelete = new List<int>();
                                for (int i = 0; i < deletedCount; i++)
                                {
                                    if (_batchPropertyList.GetDeletedList()[i].Name == field)
                                    {
                                        toUnDelete.Add(i);
                                        errorBuilder.Append("Batch - " + _batchPropertyList.GetDeletedList()[i].FriendlyName + " can´t be deleted <br/>");
                                        IsConfigSavedSuccessfully = false;
                                    }
                                }
                                foreach (int index in toUnDelete)
                                {
                                    _batchPropertyList.GetDeletedList()[index].SortOrder = -1;
                                    _batchPropertyList.AddProperty(_batchPropertyList.GetDeletedList()[index]);
                                    _batchPropertyList.GetDeletedList().RemoveAt(index);
                                }
                                break;

                            case "BATCHCOMPONENT":
                                deletedCount = _batchComponentList.GetDeletedList().Count;
                                toUnDelete = new List<int>();
                                for (int i = 0; i < deletedCount; i++)
                                {
                                    if (_batchComponentList.GetDeletedList()[i].Name == field)
                                    {
                                        errorBuilder.Append("Batch Component - " + _batchComponentList.GetDeletedList()[i].FriendlyName + " can´t be deleted <br/>");
                                        toUnDelete.Add(i);
                                        IsConfigSavedSuccessfully = false;
                                    }
                                }
                                foreach (int index in toUnDelete)
                                {
                                    _batchComponentList.GetDeletedList()[index].SortOrder = -1;
                                    _batchComponentList.AddProperty(_batchComponentList.GetDeletedList()[index]);
                                    _batchComponentList.GetDeletedList().RemoveAt(index);
                                }
                                break;

                            case "STRUCTURE":
                                deletedCount = _structurePropertyList.GetDeletedList().Count;
                                toUnDelete = new List<int>();
                                for (int i = 0; i < deletedCount; i++)
                                {
                                    if (_structurePropertyList.GetDeletedList()[i].Name == field)
                                    {
                                        errorBuilder.Append("Structure - " + _structurePropertyList.GetDeletedList()[i].FriendlyName + " can´t be deleted <br/>");
                                        toUnDelete.Add(i);
                                        IsConfigSavedSuccessfully = false;
                                    }
                                }
                                foreach (int index in toUnDelete)
                                {
                                    _structurePropertyList.GetDeletedList()[index].SortOrder = -1;
                                    _structurePropertyList.AddProperty(_structurePropertyList.GetDeletedList()[index]);
                                    _structurePropertyList.GetDeletedList().RemoveAt(index);
                                }
                                break;
                        }
                        break;

                    case "adding":
                        switch (section)
                        {
                            case "MIXTURE":
                                foreach (Property prop in this._propertyList)
                                {
                                    if (prop.Name == field)
                                    {
                                        switch (errorCode)
                                        {
                                            case DIFPRECISIONERRORCODE:
                                                existingPrecision = message.Substring(message.IndexOf('(')).Replace("(The data length more big is ", string.Empty).Replace(")", string.Empty);
                                                errorBuilder.Append("Registry - " + prop.FriendlyName + diferentPrecisionMessage + existingPrecision + "<br/>");
                                        IsConfigSavedSuccessfully = false;
                                                //registriesPropToDelete.Add(prop);
                                                break;
                                            case DIFTYPEERRORCODE:
                                                existingType = message.Substring(message.IndexOf('(')).Replace("(The type is ", string.Empty).Replace(")", string.Empty);
                                                errorBuilder.Append("Registry - " + prop.FriendlyName + diferentTypeMessage + existingType + "<br/>");
                                        IsConfigSavedSuccessfully = false;
                                                //registriesPropToDelete.Add(prop);
                                                break;
                                        }

                                    }
                                }
                                break;
                            case "COMPOUND":
                                foreach (Property prop in this._compoundPropertyList)
                                {
                                    if (prop.Name == field)
                                    {
                                        switch (errorCode)
                                        {
                                            case DIFPRECISIONERRORCODE:
                                                existingPrecision = existingPrecision = message.Substring(message.IndexOf('(')).Replace("(The data length more big is ", string.Empty).Replace(")", string.Empty);
                                                errorBuilder.Append("Compound - " + prop.FriendlyName + diferentPrecisionMessage + existingPrecision + "<br/>");
                                        IsConfigSavedSuccessfully = false;
                                                //componentPropToDelete.Add(prop);
                                                break;
                                            case DIFTYPEERRORCODE:
                                                existingType = message.Substring(message.IndexOf('(')).Replace("(The type is ", string.Empty).Replace(")", string.Empty);
                                                errorBuilder.Append("Compound - " + prop.FriendlyName + diferentTypeMessage + existingType + "<br/>");
                                        IsConfigSavedSuccessfully = false;
                                                //componentPropToDelete.Add(prop);
                                                break;
                                        }
                                    }
                                }
                                break;
                            case "BATCH":
                                foreach (Property prop in this._batchPropertyList)
                                {
                                    if (prop.Name == field)
                                    {
                                        switch (errorCode)
                                        {
                                            case DIFPRECISIONERRORCODE:
                                                existingPrecision = existingPrecision = message.Substring(message.IndexOf('(')).Replace("(The data length more big is ", string.Empty).Replace(")", string.Empty);
                                                errorBuilder.Append("Batch - " + prop.FriendlyName + diferentPrecisionMessage + existingPrecision + "<br/>");
                                        IsConfigSavedSuccessfully = false;
                                                //batchPropToDelete.Add(prop);
                                                break;
                                            case DIFTYPEERRORCODE:
                                                existingType = message.Substring(message.IndexOf('(')).Replace("(The type is ", string.Empty).Replace(")", string.Empty);
                                                errorBuilder.Append("Batch - " + prop.FriendlyName + diferentTypeMessage + existingType + "<br/>");
                                        IsConfigSavedSuccessfully = false;
                                                //batchPropToDelete.Add(prop);
                                                break;
                                        }
                                    }
                                }
                                break;
                            case "BATCHCOMPONENT":
                                foreach (Property prop in this._batchComponentList)
                                {
                                    if (prop.Name == field)
                                    {
                                        switch (errorCode)
                                        {
                                            case DIFPRECISIONERRORCODE:
                                                existingPrecision = existingPrecision = message.Substring(message.IndexOf('(')).Replace("(The data length more big is ", string.Empty).Replace(")", string.Empty);
                                                errorBuilder.Append("Batch Component - " + prop.FriendlyName + diferentPrecisionMessage + existingPrecision + "<br/>");
                                        IsConfigSavedSuccessfully = false;
                                                //batchCompPropToDelete.Add(prop);
                                                break;
                                            case DIFTYPEERRORCODE:
                                                existingType = message.Substring(message.IndexOf('(')).Replace("(The type is ", string.Empty).Replace(")", string.Empty);
                                                errorBuilder.Append("Batch Component - " + prop.FriendlyName + diferentTypeMessage + existingType + "<br/>");
                                        IsConfigSavedSuccessfully = false;
                                                //batchCompPropToDelete.Add(prop);
                                                break;
                                        }
                                    }
                                }
                                break;

                            case "STRUCTURE":
                                foreach (Property prop in this._structurePropertyList)
                                {
                                    if (prop.Name == field)
                                    {
                                        switch (errorCode)
                                        {
                                            case DIFPRECISIONERRORCODE:
                                                existingPrecision = existingPrecision = message.Substring(message.IndexOf('(')).Replace("(The data length more big is ", string.Empty).Replace(")", string.Empty);
                                                errorBuilder.Append("Structure - " + prop.FriendlyName + diferentPrecisionMessage + existingPrecision + "<br/>");
                                        IsConfigSavedSuccessfully = false;
                                                //batchCompPropToDelete.Add(prop);
                                                break;
                                            case DIFTYPEERRORCODE:
                                                existingType = message.Substring(message.IndexOf('(')).Replace("(The type is ", string.Empty).Replace(")", string.Empty);
                                                errorBuilder.Append("Structure - " + prop.FriendlyName + diferentTypeMessage + existingType + "<br/>");
                                        IsConfigSavedSuccessfully = false;
                                                //batchCompPropToDelete.Add(prop);
                                                break;
                                        }
                                    }
                                }
                                break;
                        }
                        break;
                }
            }

            foreach (Property propToDelete in registriesPropToDelete)
            {
                if (_propertyList.Contains(propToDelete))
                    _propertyList.RemoveAt(_propertyList.GetPropertyIndex(propToDelete.Name));
            }

            foreach (Property propToDelete in componentPropToDelete)
            {
                if (_compoundPropertyList.Contains(propToDelete))
                    _compoundPropertyList.RemoveAt(_compoundPropertyList.GetPropertyIndex(propToDelete.Name));
            }

            foreach (Property propToDelete in batchPropToDelete)
            {
                if (_batchPropertyList.Contains(propToDelete))
                    _batchPropertyList.RemoveAt(_batchPropertyList.GetPropertyIndex(propToDelete.Name));
            }

            foreach (Property propToDelete in batchCompPropToDelete)
            {
                if (_batchComponentList.Contains(propToDelete))
                    _batchComponentList.RemoveAt(_batchComponentList.GetPropertyIndex(propToDelete.Name));
            }

            foreach (Property propToDelete in structurePropToDelete)
            {
                if (structurePropToDelete.Contains(propToDelete))
                    structurePropToDelete.RemoveAt(_structurePropertyList.GetPropertyIndex(propToDelete.Name));
            }

            _errorLogToShow = errorBuilder.ToString();
        }

        private void FillAddInAssemblyList()
        {
            string exceptionMessage = string.Empty;
            this._addInsAssemblyList = new AssemblyList();

            AppDomain domain = AppDomain.CurrentDomain;

            Assembly[] asseblyList = domain.GetAssemblies();

            foreach (Assembly ass in asseblyList)
            {
                //Temporary try catch to get more information about the exception
                try
                {
                    Type[] types;
                    try
                    {
                        types = ass.GetTypes();
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    AddInAssembly addInAssembly = null;
                    if (types != null)
                    {
                        for (int i = 0; i < types.Length; i++)
                        {
                            if (types[i].GetInterface(typeof(IAddIn).FullName) != null)
                            {
                                if (!_addInsAssemblyList.ContainsAssembly(ass.FullName))
                                {
                                    addInAssembly = new AddInAssembly(ass);

                                    _addInsAssemblyList.Assemblies.Add(addInAssembly);
                                }
                            }
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    for (int i = 0; i < ex.LoaderExceptions.Length; i++)
                        exceptionMessage = ex.LoaderExceptions[i].Message;

                    throw new Exception(exceptionMessage);

                }
            }
        }

        protected override object GetIdValue()
        {
            return Guid.NewGuid();
        }

        [COEUserActionDescription("GetConfigurationRegistryRecordXml")]
        public string UpdateSelf()
        {
            StringBuilder builder = new StringBuilder("");

            try
            {
                builder.Append("<ConfigurationRegistryRecord>");

                builder.Append(this._propertyList.UpdateSelfConfig());

                builder.Append("<Compound>");
                builder.Append(this._compoundPropertyList.UpdateSelfConfig());
                builder.Append("</Compound>");

                builder.Append("<Structure>");
                builder.Append(this._structurePropertyList.UpdateSelfConfig());
                builder.Append("</Structure>");

                builder.Append("<Batch>");
                builder.Append(this._batchPropertyList.UpdateSelfConfig());
                builder.Append("</Batch>");

                builder.Append("<BatchComponent>");
                builder.Append(this._batchComponentList.UpdateSelfConfig());
                builder.Append("</BatchComponent>");

                builder.Append("<AddIns>");
                builder.Append(this._addInList.UpdateSelf());
                builder.Append("</AddIns>");

                builder.Append("</ConfigurationRegistryRecord>");

                this._xml = builder.ToString();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }

            return builder.ToString();
        }

        private void FillEventNames()
        {
            List<string> eventNames = new List<string>();
            foreach (EventInfo eventInfo in typeof(RegistryRecord).GetEvents())
                eventNames.Add(eventInfo.Name);
            _eventNames = eventNames.ToArray();
        }

        public override ConfigurationRegistryRecord Save()
        {
            UpdateSelf();
            return base.Save();
        }

        [COEUserActionDescription("ImportConfigurations")]
        public void ImportCustomization(string appRootInstallPath, string folderPath, bool forceImport)
        {
            _forceImport = forceImport;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(this.CreateConfigFromLocal(appRootInstallPath, folderPath));

                XmlNode customForms = xmlDoc.DocumentElement.SelectSingleNode("//customForms");
                ImportCustomizedProperties(xmlDoc.OuterXml);

                this.RegDal.DALManager.BeginTransaction();
                foreach (XmlNode node in customForms.ChildNodes)
                {
                    this.ImportForm(node.OuterXml);
                }

                XmlNode customDataviewList = xmlDoc.DocumentElement.SelectSingleNode("//customDataViewList");
                int dataviewId;
                foreach (XmlNode dataview in customDataviewList)
                {
                    if (dataview.Attributes["dataviewid"] != null)
                    {
                        dataviewId = int.Parse(dataview.Attributes["dataviewid"].Value);
                        this.ImportDataView(dataview.OuterXml.ToString(), dataviewId);
                    }
                    if (dataview.Attributes["id"] != null)
                    {
                        dataviewId = int.Parse(dataview.Attributes["id"].Value);
                        dataview.Attributes.Remove(dataview.Attributes["id"]);
                        dataview.Attributes.Append(xmlDoc.CreateAttribute("dataviewid"));
                        dataview.Attributes["dataviewid"].Value = dataviewId.ToString();
                        this.ImportDataView(dataview.OuterXml.ToString(), dataviewId);
                    }
                }

                XmlNode tableList = xmlDoc.DocumentElement.SelectSingleNode("//tableList");
                this.ImportTableList(tableList.OuterXml);

                XmlNode configSettingsNode = xmlDoc.DocumentElement.SelectSingleNode("//configurationSettings");
                if (configSettingsNode != null)
                    this.ImportConfigurationSettings(configSettingsNode.InnerXml);
                
                this.RegDal.DALManager.CommitTransaction();
            }
            catch
            {
                this.RegDal.DALManager.RollbackTransaction();
                throw;
            }
        }

        private string CreateConfigFromLocal(string appRootInstallPath, string folderPath)
        {
            string configFilesUrl = string.Empty;
            if (!string.IsNullOrEmpty(folderPath))
                configFilesUrl = folderPath;
            else
                configFilesUrl = appRootInstallPath + IMPORTFILESPATH;

            string[] formsFilesNames;
            string[] dataViewsFilesNames;
            string[] tablesFilesNames;

            //If any of these folders are missing, this should throw an error
            formsFilesNames = Directory.GetFiles(configFilesUrl + "\\" + COEFORMSFOLDERNAME, FILE_SEARCH_PATTERN);
            dataViewsFilesNames = Directory.GetFiles(configFilesUrl + "\\" + COEDATAVIEWSFOLDERNAME, FILE_SEARCH_PATTERN);
            tablesFilesNames = Directory.GetFiles(configFilesUrl + "\\" + COETABLESFORLDERNAME, FILE_SEARCH_PATTERN);

            XmlDocument coeObjectConfigXml = new XmlDocument();
            //If this file is missing, there will be an exception
            coeObjectConfigXml.Load(configFilesUrl + "\\" + COEOBJECTCONFIGFILENAME);

            XmlNode formsNode = coeObjectConfigXml.DocumentElement.AppendChild(coeObjectConfigXml.CreateElement("customForms"));
            foreach (string formFileName in formsFilesNames)
            {
                XmlDocument formXml = new XmlDocument();
                formXml.Load(formFileName);
                formsNode.AppendChild(coeObjectConfigXml.ImportNode(formXml.DocumentElement, true));
            }

            XmlNode dataViewsNode = coeObjectConfigXml.DocumentElement.AppendChild(coeObjectConfigXml.CreateElement("customDataViewList"));
            foreach (string dataViewFileName in dataViewsFilesNames)
            {
                XmlDocument dataViewXml = new XmlDocument();
                dataViewXml.Load(dataViewFileName);
                if (dataViewXml.DocumentElement.Attributes["dataviewid"] == null && dataViewXml.DocumentElement.Attributes["id"] == null) //Exported with LR 11.0.1, no id set in xml.
                {
                    string idFromFilename = dataViewFileName.Substring(dataViewFileName.LastIndexOf('\\') + 1, (dataViewFileName.LastIndexOf('.') - dataViewFileName.LastIndexOf('\\') - 1));
                    XmlAttribute dataviewidAttr = dataViewXml.CreateAttribute("dataviewid");
                    dataviewidAttr.Value = idFromFilename;
                    dataViewXml.DocumentElement.Attributes.Append(dataviewidAttr);
                }
                dataViewsNode.AppendChild(coeObjectConfigXml.ImportNode(dataViewXml.DocumentElement, true));
            }

            XmlNode tablesNode = coeObjectConfigXml.DocumentElement.AppendChild(coeObjectConfigXml.CreateElement("tableList"));
            foreach (string tableFileName in tablesFilesNames)
            {
                XmlDocument tableXml = new XmlDocument();
                tableXml.Load(tableFileName);
                foreach (XmlNode rowNode in tableXml.DocumentElement)
                    tablesNode.AppendChild(coeObjectConfigXml.ImportNode(rowNode, true));
            }

            string configSettingsFile = configFilesUrl + "\\" + CONFIGSETTINGSFILENAME;
            if (File.Exists(configSettingsFile))
            {
                XmlDocument configSettingsXml = new XmlDocument();
                configSettingsXml.Load(configSettingsFile);
                if (configSettingsXml.HasChildNodes)
                {
                    coeObjectConfigXml.DocumentElement.AppendChild(coeObjectConfigXml.CreateElement("configurationSettings"));
                    coeObjectConfigXml.DocumentElement.LastChild.InnerXml = configSettingsXml.DocumentElement.InnerXml;
                }
            }

            return coeObjectConfigXml.InnerXml.ToString();
        }

        private void ImportCustomizedProperties(string xml)
        {
            try
            {
                this.IsImported = true;
                if (!_forceImport)
                {
                    //TODO: We might be more smart and add new properties instead...
                    ConfigurationRegistryRecord existingConfig = NewConfigurationRegistryRecord();
                    if(!IsEmpty(existingConfig))
                        return;
                }

                ConfigurationRegistryRecord importedCrr = ConfigurationRegistryRecord.NewConfigurationRegistryRecord(xml);

                PropertyList propertyList = PropertyList.NewPropertyList();
                foreach (Property prop in _propertyList)
                    propertyList.Add(prop);

                PropertyList compoundPropertyList = PropertyList.NewPropertyList();
                foreach (Property prop in _compoundPropertyList)
                    compoundPropertyList.Add(prop);

                PropertyList batchPropertyList = PropertyList.NewPropertyList();
                foreach (Property prop in _batchPropertyList)
                    batchPropertyList.Add(prop);

                PropertyList batchComponentList = PropertyList.NewPropertyList();
                foreach (Property prop in _batchComponentList)
                    batchComponentList.Add(prop);

                PropertyList structurePropertyList = PropertyList.NewPropertyList();
                foreach (Property prop in _structurePropertyList)
                    structurePropertyList.Add(prop);

                if (importedCrr._propertyList.Count == 0)
                {
                    foreach (Property prop in propertyList)
                        _propertyList.Remove(prop);
                }
                else
                {

                    foreach (Property prop in propertyList)
                    {
                        if (!importedCrr._propertyList.CheckExistingNames(prop.Name,false))
                            this._propertyList.Remove(prop);
                    }
                }

                if (importedCrr._batchPropertyList.Count == 0)
                {
                    foreach (Property prop in batchPropertyList)
                        _batchPropertyList.Remove(prop);
                }
                else
                {
                    foreach (Property prop in batchPropertyList)
                    {
                        if (!importedCrr._batchPropertyList.CheckExistingNames(prop.Name,false))
                            this._batchPropertyList.Remove(prop);
                    }
                }

                if (importedCrr._batchComponentList.Count == 0)
                {
                    foreach (Property prop in batchComponentList)
                        _batchComponentList.Remove(prop);
                }
                else
                {
                    foreach (Property prop in batchComponentList)
                    {
                        if (!importedCrr._batchComponentList.CheckExistingNames(prop.Name,false))
                            this._batchComponentList.Remove(prop);
                    }
                }

                if (importedCrr._compoundPropertyList.Count == 0)
                {
                    foreach (Property prop in compoundPropertyList)
                        _compoundPropertyList.Remove(prop);
                }
                else
                {

                    foreach (Property prop in compoundPropertyList)
                    {
                        if (!importedCrr._compoundPropertyList.CheckExistingNames(prop.Name,false))
                            this._compoundPropertyList.Remove(prop);
                    }
                }

                if (importedCrr._structurePropertyList.Count == 0)
                {
                    foreach (Property prop in structurePropertyList)
                        _structurePropertyList.Remove(prop);
                }
                else
                {
                    foreach (Property prop in structurePropertyList)
                    {
                        if (!importedCrr._structurePropertyList.CheckExistingNames(prop.Name, false))
                            this._structurePropertyList.Remove(prop);
                    }
                }

                if (importedCrr._addInList.Count == 0)
                {
                    foreach (AddIn addIn in AddInList)
                        _addInList.Remove(addIn);
                }
                else
                {
                    foreach (AddIn addIn in AddInList)
                    {
                        if (!importedCrr._addInList.ExistAddIndCheck(addIn))
                            this._addInList.Remove(addIn);
                    }
                }

                foreach (Property prop in importedCrr._propertyList)
                {
                    if (prop.Type.ToUpper() == "NUMBER")
                        prop.Precision = RegAdminUtils.ConvertPrecision(prop.Precision, false);
                    this._propertyList.AddProperty(prop);
                }

                foreach (Property prop in importedCrr._compoundPropertyList)
                {
                    if (prop.Type.ToUpper() == "NUMBER")
                        prop.Precision = RegAdminUtils.ConvertPrecision(prop.Precision, false);
                    this._compoundPropertyList.AddProperty(prop);
                }

                foreach (Property prop in importedCrr._batchPropertyList)
                {
                    if (prop.Type.ToUpper() == "NUMBER")
                        prop.Precision = RegAdminUtils.ConvertPrecision(prop.Precision, false);
                    this._batchPropertyList.AddProperty(prop);
                }

                foreach (Property prop in importedCrr._batchComponentList)
                {
                    if (prop.Type.ToUpper() == "NUMBER")
                        prop.Precision = RegAdminUtils.ConvertPrecision(prop.Precision, false);
                    this._batchComponentList.AddProperty(prop);
                }

                foreach (Property prop in importedCrr._structurePropertyList)
                {
                    if (prop.Type.ToUpper() == "NUMBER")
                        prop.Precision = RegAdminUtils.ConvertPrecision(prop.Precision, false);
                    this._structurePropertyList.AddProperty(prop);
                }

                foreach (AddIn addIn in importedCrr._addInList)
                {
                    this._addInList.AddAddin(addIn);
                }

                this.Save();
                this.IsImported = false;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        private bool IsEmpty(ConfigurationRegistryRecord existingConfig)
        {
            return  existingConfig == null || 
                    (existingConfig.AddInList.Count == 0 && existingConfig.BatchComponentList.Count == 0 &&
                    existingConfig.BatchPropertyList.Count == 0 && existingConfig.CompoundPropertyList.Count == 0 &&
                    existingConfig.StructurePropertyList.Count == 0);
        }

        private void ImportAddInList(AddInList addInList)
        {   // this is remarkably silly.  why not just replace the addins with the new addins so you can pick up things
            // whether the add in is enabled, events were added etc.
            // I am chaning what htis method does. It is only done during force option

            this.AddInList.Clear();
            foreach (AddIn importAddIn in addInList)
            {
                //if (!this.AddInList.ExistAddInFriendlyName(importAddIn.FriendlyName))
                //{
                    importAddIn.MarkAsNew();
                    this.AddInList.Add(importAddIn);
                //}
            }
        }

        /// <summary>
        /// Given the contents of a classic Windoews INI file in a System.IO.Stream,
        /// performs the following:
        /// <list type="bullet">
        /// <item>
        ///     <description>extracts the settings from COE v.10</description>
        /// </item>
        /// <item>
        ///     <description>, applies them to the current COE v.11 business objects, and</description>
        /// </item>
        /// <item>
        ///     <description>propagates those settings to their data-repositories.</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="iniFile">Contents of the COE v.10 INI file</param>
        [COEUserActionDescription("ImportIniFiles")]
        public void ImportIniFile(Stream streamCfserver, Stream streamReg)
        {
            try
            {
                this.LogImportMessage(Environment.NewLine);
                this.LogImportMessage(ImportIniResult.ImportStartMessageTemplate);
                IniSettingInfo iniCfserver = new IniSettingInfo(streamCfserver);

                iniCfserver.Parse();

                IniSettingInfo iniReg = new IniSettingInfo(streamReg);
                iniReg.Parse();

                List<string> requiredFieldList = ExtractRequiredFieldList(iniReg);
                UpdateFormFieldRequiredness(iniCfserver, requiredFieldList);

                ImportIniAppSettings(iniCfserver);
                ImportIniFormFields(iniCfserver);

                this.LogImportMessage(ImportIniResult.ImportEndMessageTemplate);
                this.LogImportMessage(Environment.NewLine);
            }
            catch (Exception ex)
            {
                //this.LogImportMessage(ImportIniResult.ImportErrorMessageTemplate, ex.ToString());
                //throw;
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        /// <summary>
        /// Update the form field's requiredness status based on Reg.ini file.
        /// We only need to care about 2 sections for now: [REG_CTRBT_FORM_GROUP] and [BATCH_CTRBT_FORM_GROUP].
        /// </summary>
        /// <param name="iniCfserver">The Ini setting containing the master form fields information</param>
        /// <param name="iniReg">The Reg.ini information about form field's requiredness</param>
        private void UpdateFormFieldRequiredness(IniSettingInfo iniCfserver, List<string> requiredFieldList)
        {
            IniRegistrationSection iniRegistrationSection = (IniRegistrationSection)iniCfserver.IniSections[IniSection.REGISTRATION_SECTION_NAME];

            foreach (string requiredFieldName in requiredFieldList)
            {
                if (iniRegistrationSection.FormFields.ContainsKey(requiredFieldName))
                {
                    iniRegistrationSection.FormFields[requiredFieldName].IsRequired = true;
                }
            }
        }

        /// <summary>
        /// Extract out the required field list from the Reg.ini information
        /// </summary>
        /// <param name="iniReg">The Reg.ini setting information</param>
        /// <returns>A list of required field name</returns>
        private List<string> ExtractRequiredFieldList(IniSettingInfo iniReg)
        {
            List<string> requiredFieldList = new List<string>();

            IniSection regSection = iniReg.IniSections["REG_CTRBT_FORM_GROUP"];
            IniSection batchSection = iniReg.IniSections["BATCH_CTRBT_FORM_GROUP"];

            AddSectionRequiredFieldList(requiredFieldList, regSection);
            AddSectionRequiredFieldList(requiredFieldList, batchSection);

            return requiredFieldList;
        }

        private void AddSectionRequiredFieldList(List<string> requiredFieldList, IniSection iniSection)
        {
            string requiredFieldName = string.Empty;
            if (iniSection.AppSettingItems != null) //Coverity fix - CID 11792 
            {
                string[] requiredFieldArray = iniSection.AppSettingItems["REQUIRED_FIELDS"].RawValue.Split(",".ToCharArray());

                foreach (string requiredField in requiredFieldArray)
                {
                    if (string.Compare(requiredField, "null", true) == 0)
                        continue;

                    requiredFieldName = requiredField.Split(";".ToCharArray())[0].Split(".".ToCharArray())[1];

                    // Ignore Structure field
                    if (string.Compare(requiredFieldName, "structure", true) != 0)
                    {
                        requiredFieldList.Add(requiredFieldName.ToUpper());
                    }
                }
            }
        }

        /// <summary>
        /// Imports all COE form fields settings.
        /// </summary>
        /// <param name="iniSettingInfo">The INI setting information containing all COE form fields setting items</param>
        private void ImportIniFormFields(IniSettingInfo iniCfserver)
        {
            _coeFormHelper = new COEFormHelper(this);
            _coeFormHelper.UpdateFormGroupFromIniFormFields(iniCfserver);
        }

        /// <summary>
        /// Imports the app settings from INI content to XML configuration.
        /// </summary>
        /// <param name="iniSettingInfo">The INI setting information containing all INI app setting items.</param>
        private void ImportIniAppSettings(IniSettingInfo iniCfserver)
        {
            ImportIniFileMapperSection mapperSection = new ImportIniFileMapperSection();

            ReadImportIniMapperFile(mapperSection);

            foreach (IniSectionMapperElement iniSectionMapperElement in mapperSection.IniSectionMapperElementCollection)
            {
                foreach (IniSettingMapperElement iniSettingMapperElement in iniSectionMapperElement.IniSettingsMapperElementCollection)
                {
                    // Check if there's such a specified INI section existing
                    if (!iniCfserver.IniSections.ContainsKey(iniSectionMapperElement.SectionName))
                    {
                        LogFailedAppSettingImportAndStopImport(ImportIniAppSettingResult.ImportIniAppSettingFailureTemplate.INI_SECTION_NOT_FOUND,
                                iniSectionMapperElement.SectionName);
                    }

                    // Check if there's such a specified INI setting existing
                    if (!iniCfserver.IniSections[iniSectionMapperElement.SectionName].AppSettingItems.ContainsKey(iniSettingMapperElement.IniSettingName))
                    {
                        LogFailedAppSettingImportAndStopImport(ImportIniAppSettingResult.ImportIniAppSettingFailureTemplate.INI_SETTING_NOT_FOUND,
                                iniSettingMapperElement.IniSettingName,
                                iniSectionMapperElement.SectionName
                                );
                    }

                    ImportAppSetting(iniCfserver.IniSections[iniSectionMapperElement.SectionName].AppSettingItems[iniSettingMapperElement.IniSettingName], iniSettingMapperElement);
                }
            }
        }

        /// <summary>
        /// Reads file ImportIniFileMapper.xml and loads it into an ImportIniFileMapperSection object.
        /// </summary>
        /// <param name="mapperSection">The ImportIniFileMapperSection object to load mapper settings to</param>
        private void ReadImportIniMapperFile(ImportIniFileMapperSection mapperSection)
        {
            TextReader mapperFileReader = null;
            XmlReader reader = null;

            try
            {
                XmlReaderSettings readerSettings = new XmlReaderSettings();
                readerSettings.CloseInput = true;
                readerSettings.IgnoreComments = true;
                readerSettings.IgnoreWhitespace = true;

                mapperFileReader = new StreamReader(HttpContext.Current.Server.MapPath(@"..\Xml\ImportIniFileMapper.xml"));
                reader = XmlReader.Create(mapperFileReader, readerSettings);

                MethodInfo info = mapperSection.GetType().GetMethod("DeserializeSection", BindingFlags.NonPublic | BindingFlags.Instance);
                info.Invoke(mapperSection, new object[] { reader });
            }
            catch (Exception ex)
            {
                throw new Exception("Error while reading the mapper file", ex);
            }
            finally
            {
                if (mapperFileReader != null)
                    mapperFileReader.Close();
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>
        /// Imports the app settings from INI content to XML configuration.
        /// </summary>
        /// <param name="iniSettingItem">The single INI app setting item</param>
        /// <param name="iniSettingMapperElement">The mapper defining how to import this item</param>
        /// <remarks>
        /// The IniSettingMapperElement looks like this:
        /// <![CDATA[
        /// <add type="appSetting" IniSettingName="BATCH_LEVEL" XMLSettingName="SameBatchesIdentity" appName="REGISTRATION" group="REGADMIN">
		///			<mappers>
		///				<add IniSettingValue="COMPOUND" XMLSettingValue="True"/>
		///				<add IniSettingValue="BATCH" XMLSettingValue="False"/>
		///			</mappers>
		///	</add>
        /// ]]>
        /// </remarks>
        private void ImportAppSetting(IniSettingItem iniSettingItem, IniSettingMapperElement iniSettingMapperElement)
        {
            // This is the object representation of the target XML configuration
            AppSettingsData appSettingData = FrameworkUtils.GetAppConfigSettings(iniSettingMapperElement.AppName);
            bool hasFoundIniSettingValue = false;

            if (iniSettingMapperElement.MapperElementCollection.Count > 0)
            {
                foreach (MapperElement mapperElement in iniSettingMapperElement.MapperElementCollection)
                {
                    if (string.Compare(mapperElement.IniSettingValue, iniSettingItem.RawValue, true) == 0)
                    {
                        hasFoundIniSettingValue = true;

                        if (string.Compare(appSettingData.SettingsGroup.Get(iniSettingMapperElement.Group).Settings.Get(iniSettingMapperElement.XMLSettingName).Value, mapperElement.XMLSettingValue, true) == 0)
                        {
                            LogIgnorableAppSettingImport(iniSettingMapperElement.IniSettingName);
                        }
                        else
                        {
                            appSettingData.SettingsGroup.Get(iniSettingMapperElement.Group).Settings.Get(iniSettingMapperElement.XMLSettingName).Value = mapperElement.XMLSettingValue;
                            FrameworkUtils.SaveAppConfigSettings(iniSettingMapperElement.AppName, appSettingData);
                            LogSuccessfulAppSettingImport(iniSettingMapperElement.IniSettingName, mapperElement.IniSettingValue, iniSettingMapperElement.XMLSettingName, mapperElement.XMLSettingValue);
                        }

                        break;
                    }
                }

                if (!hasFoundIniSettingValue)
                {
                    LogFailedAppSettingImportAndStopImport(ImportIniAppSettingResult.ImportIniAppSettingFailureTemplate.INI_SETTING_VALUE_NOT_MATCH,
                        iniSettingItem.SettingName,
                        iniSettingItem.RawValue);
                }
            }
            else
            {
                if (string.Compare(appSettingData.SettingsGroup.Get(iniSettingMapperElement.Group).Settings.Get(iniSettingMapperElement.XMLSettingName).Value, iniSettingItem.RawValue, true) == 0)
                {
                    LogIgnorableAppSettingImport(iniSettingMapperElement.IniSettingName);
                }
                else
                {
                    appSettingData.SettingsGroup.Get(iniSettingMapperElement.Group).Settings.Get(iniSettingMapperElement.XMLSettingName).Value = iniSettingItem.RawValue;
                    FrameworkUtils.SaveAppConfigSettings(iniSettingMapperElement.AppName, appSettingData);
                    LogSuccessfulAppSettingImport(iniSettingMapperElement.IniSettingName, iniSettingItem.RawValue, iniSettingMapperElement.XMLSettingName, iniSettingItem.RawValue);
                }
            }
        }

        /// <summary>
        /// Writes general log message for INI import processing.
        /// </summary>
        private void LogImportMessage(string messageTemplate, params string[] messages)
        {
            ImportIniResult result = new ImportIniResult();

            result.LogResult(messageTemplate, messages);
        }

        /// <summary>
        /// Adds a successful log entry to the log file.
        /// </summary>
        /// <param name="iniSettingName"></param>
        /// <param name="iniSettingValue"></param>
        /// <param name="xmlSettingName"></param>
        /// <param name="xmlSettingValue"></param>
        private void LogSuccessfulAppSettingImport(string iniSettingName, string iniSettingValue, string xmlSettingName, string xmlSettingValue)
        {
            ImportIniResult importIniResult = new ImportIniAppSettingResult();

            importIniResult.LogResult(importIniResult.SuccessMessageTemplate,
                iniSettingName,
                iniSettingValue,
                xmlSettingName,
                xmlSettingValue);
        }

        /// <summary>
        /// Adds an ignorable log entry to the log file.
        /// </summary>
        /// <param name="iniSettingName">The name of the INI setting that already gets imported.</param>
        private void LogIgnorableAppSettingImport(string iniSettingName)
        {
            ImportIniResult importIniResult = new ImportIniAppSettingResult();
            
            importIniResult.LogResult(importIniResult.SkipMessageTemplate, iniSettingName);
        }

        /// <summary>
        /// Adds an ignorable log entry to the log file.
        /// </summary>
        /// <param name="failureMessageTemplate"></param>
        private void LogFailedAppSettingImportAndStopImport(string failureMessageTemplate, params string[] messageComponents)
        {
            ImportIniResult importIniResult = new ImportIniAppSettingResult();
            importIniResult.FailureMessageTemplate += failureMessageTemplate;

            importIniResult.LogResult(importIniResult.FailureMessageTemplate, messageComponents);

            TerminateImportProcess();
        }

        private void TerminateImportProcess()
        {
            throw new Exception("There's error importing the INI settings. Please refer to the log file for details.");
        }

        [COEUserActionDescription("GetConfigurationRegistryRecordXml")]
        public string ExportCustomizedProperties()
        {
            try
            {
                return this.UpdateSelf();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        [COEUserActionDescription("ExportSearchPermForm")]
        public string ExportSearchPermForm()
        {
            try
            {
                return this.COEFormHelper.ExportSearchPermForm();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        [COEUserActionDescription("ExportSearchTempForm")]
        public string ExportSearchTempForm()
        {
            try
            {
                return this.COEFormHelper.ExportSearchTempForm();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        [COEUserActionDescription("ExportELNSearchPermForm")]
        public string ExportELNSearchPermForm()
        {
            try
            {
                return this.COEFormHelper.ExportELNSearchPermForm();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        [COEUserActionDescription("ExportELNSearchTempForm")]
        public string ExportELNSearchTempForm()
        {
            try
            {
                return this.COEFormHelper.ExportELNSearchTempForm();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        [COEUserActionDescription("ExportViewMixtureForm")]
        public string ExportViewMixtureForm()
        {
            try
            {
                return this.COEFormHelper.ExportViewMixtureForm();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }
        [COEUserActionDescription("ExportReviewMixtureForm")]
        public string ExportReviewMixtureForm()
        {
            try
            {
                return this.COEFormHelper.ExportReviewMixtureForm();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }
        [COEUserActionDescription("ExportSubmitMixtureForm")]
        public string ExportSubmitMixtureForm()
        {
            try
            {
                return this.COEFormHelper.ExportSubmitMixtureForm();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        [COEUserActionDescription("ExportDataLoaderForm")]
        public string ExportDataLoaderForm()
        {
            try
            {
                return this.COEFormHelper.ExportDataLoaderForm();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        [COEUserActionDescription("ExportComponentDuplicatesForm")]
        public string ExportComponentDuplicatesForm()
        {
            try
            {
                return this.COEFormHelper.ExportComponentDuplicatesForm();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        [COEUserActionDescription("ExportRegistryDuplicatesForm")]
        public string ExportRegistryDuplicatesForm()
        {
            try
            {
                return this.COEFormHelper.ExportRegistryDuplicatesForm();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        [COEUserActionDescription("ExportSendToRegistrationForm")]
        public string ExportSendToRegistrationForm()
        {
            try
            {
                return this.COEFormHelper.ExportSendToRegistrationForm();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        [COEUserActionDescription("ExportDeleteLogForm")]
        public string ExportDeleteLogForm()
        {
            try
            {
                return this.COEFormHelper.ExportDeleteLogFormGroupId();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        [COEUserActionDescription("ExportSearchComponentsToAddForm")]
        public string ExportSearchComponentsToAddForm()
        {
            try
            {
                return this.COEFormHelper.ExportSearchComponentsToAddForm();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        [COEUserActionDescription("ExportSearchComponentsToAddRRForm")]
        public string ExportSearchComponentsToAddRRForm()
        {
            try
            {
                return this.COEFormHelper.ExportSearchComponentsToAddRRForm();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        [COEUserActionDescription("ImportForm")]
        public void ImportForm(string coeFormGroup)
        {
            try
            {
                COEFormBO coeFormBO = COEFormBO.New(COEDatabaseName.Get());
                FormGroup formGroup = FormGroup.GetFormGroup(coeFormGroup);
                try
                {
                    coeFormBO = COEFormBO.Get(formGroup.Id);
                    if (!_forceImport)
                        return;
                }
                catch
                {
                    coeFormBO.Application = COEAppName.Get();
                    coeFormBO.FormGroupId = formGroup.Id;
                    coeFormBO.ID = formGroup.Id;
                    coeFormBO.UserName = "COEDB";
                    if (string.IsNullOrEmpty(coeFormBO.DatabaseName))
                        coeFormBO.DatabaseName = "COEDB";
                    coeFormBO.IsPublic = true;
                    //TODO add the following props to the messaging type and xml
                    //coeFormBO.Description = formGroup.Description;
                    //coeFormBO.Name = formGroup.Name;
                    //coeFormBO.FormTypeId = formGroup.FormTypId;

                    coeFormBO.Description = string.Format("Desc for {0}", formGroup.Id);
                    coeFormBO.Name = string.Format("Name for {0}", formGroup.Id);
                    coeFormBO.FormTypeId = (formGroup.Id > 4009 && formGroup.Id < 4016) ? 2 : 1;
                }

                coeFormBO.COEFormGroup = formGroup;
                coeFormBO.Save();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        [COEUserActionDescription("ImportDataView")]
        public void ImportDataView(string coeDataView, int dataviewId)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(coeDataView);
                COEDataView dataview = new COEDataView(xmlDoc);

                COEDataViewBO dataviewBO = COEDataViewBO.New();
                try
                {
                    dataviewBO = COEDataViewBO.Get(dataviewId);
                    if (!_forceImport)
                    {
                        return;
                    }
                }
                catch 
                {
                    dataviewBO.Name = dataview.Name;
                    dataviewBO.Description = dataview.Description;
                    dataviewBO.DatabaseName = dataview.Database;
                    dataviewBO.ID = dataview.DataViewID;
                    dataviewBO.IsPublic = true;
                }

                dataviewBO.COEDataView = dataview;
                dataviewBO.Save();
            }
            catch (Exception ex)
            {
                if(!ex.Message.Contains("ORA-00001: unique constraint"))
                    COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        private void ClearDeletedList()
        {
            _addInList.ClearDeletedList();
            _propertyList.ClearDeletedList();
            _batchComponentList.ClearDeletedList();
            _batchPropertyList.ClearDeletedList();
            _compoundPropertyList.ClearDeletedList();
            _structurePropertyList.ClearDeletedList();
        }

        private string UpdatePropertiesSortOrder()
        {
            XmlDocument sortedXml = new XmlDocument();
            sortedXml.LoadXml(this._xml);
            XmlNode propertyListNode = sortedXml.SelectSingleNode("//PropertyList");
            foreach (Property prop in _propertyList)
            {
                foreach (XmlNode xmlProp in propertyListNode)
                {
                    if (xmlProp.Attributes["name"].Value == prop.Name)
                    {
                        xmlProp.Attributes["sortOrder"].Value = prop.SortOrder.ToString();
                        xmlProp.Attributes.Remove(xmlProp.Attributes["insert"]);
                        xmlProp.Attributes.Remove(xmlProp.Attributes["delete"]);
                        xmlProp.Attributes.Append(xmlProp.OwnerDocument.CreateAttribute("update"));
                        xmlProp.Attributes["update"].Value = "sortOrder";
                    }
                }
            }

            XmlNode compoundListNode = sortedXml.SelectSingleNode("//Compound/PropertyList");
            foreach (Property prop in _compoundPropertyList)
            {
                foreach (XmlNode xmlProp in compoundListNode)
                {
                    if (xmlProp.Attributes["name"].Value == prop.Name)
                    {
                        xmlProp.Attributes["sortOrder"].Value = prop.SortOrder.ToString();
                        xmlProp.Attributes.Remove(xmlProp.Attributes["insert"]);
                        xmlProp.Attributes.Remove(xmlProp.Attributes["delete"]);
                        xmlProp.Attributes.Append(xmlProp.OwnerDocument.CreateAttribute("update"));
                        xmlProp.Attributes["update"].Value = "sortOrder";
                    }
                }
            }

            XmlNode batchListNode = sortedXml.SelectSingleNode("//Batch/PropertyList");
            foreach (Property prop in _batchPropertyList)
            {
                foreach (XmlNode xmlProp in batchListNode)
                {
                    if (xmlProp.Attributes["name"].Value == prop.Name)
                    {
                        xmlProp.Attributes["sortOrder"].Value = prop.SortOrder.ToString();
                        xmlProp.Attributes.Remove(xmlProp.Attributes["insert"]);
                        xmlProp.Attributes.Remove(xmlProp.Attributes["delete"]);
                        xmlProp.Attributes.Append(xmlProp.OwnerDocument.CreateAttribute("update"));
                        xmlProp.Attributes["update"].Value = "sortOrder";
                    }
                }
            }

            XmlNode batchCompListNode = sortedXml.SelectSingleNode("//BatchComponent/PropertyList");
            foreach (Property prop in _batchComponentList)
            {
                foreach (XmlNode xmlProp in batchCompListNode)
                {
                    if (xmlProp.Attributes["name"].Value == prop.Name)
                    {
                        xmlProp.Attributes["sortOrder"].Value = prop.SortOrder.ToString();
                        xmlProp.Attributes.Remove(xmlProp.Attributes["insert"]);
                        xmlProp.Attributes.Remove(xmlProp.Attributes["delete"]);
                        xmlProp.Attributes.Append(xmlProp.OwnerDocument.CreateAttribute("update"));
                        xmlProp.Attributes["update"].Value = "sortOrder";
                    }
                }
            }

            XmlNode structurePropertyListNode = sortedXml.SelectSingleNode("//Structure/PropertyList");
            foreach (Property prop in _structurePropertyList)
            {
                foreach (XmlNode xmlProp in structurePropertyListNode)
                {
                    if (xmlProp.Attributes["name"].Value == prop.Name)
                    {
                        xmlProp.Attributes["sortOrder"].Value = prop.SortOrder.ToString();
                        xmlProp.Attributes.Remove(xmlProp.Attributes["insert"]);
                        xmlProp.Attributes.Remove(xmlProp.Attributes["delete"]);
                        xmlProp.Attributes.Append(xmlProp.OwnerDocument.CreateAttribute("update"));
                        xmlProp.Attributes["update"].Value = "sortOrder";
                    }
                }
            }

            XmlNode addinListNode = sortedXml.SelectSingleNode("//AddIns");
            foreach (AddIn addin in AddInList)
            {
                foreach (XmlNode xmladdin in addinListNode)
                {
                    if (xmladdin.Attributes["assembly"].Value == addin.Assembly && xmladdin.Attributes["class"].Value == addin.ClassName)
                    {
                        xmladdin.Attributes.Remove(xmladdin.Attributes["insert"]);
                        xmladdin.Attributes.Remove(xmladdin.Attributes["delete"]);
                    }
                }
            }

            return sortedXml.OuterXml.ToString();
        }

        public string GetTableList(List<string> tableNameList)
        {
            DataSet dsTables = this.RegDal.GetTableList(tableNameList);
            return dsTables.GetXml();
        }

        [COEUserActionDescription("GetTable")]
        public string GetTable(string tableName)
        {
            try
            {
                DataSet dsTable = this.RegDal.GetTable(tableName);
                string tableInnerText = dsTable.GetXml();
                XmlDocument tableXml = new XmlDocument();
                tableXml.AppendChild(tableXml.CreateElement(tableName));
                tableXml.DocumentElement.InnerXml = tableInnerText;
                return tableXml.DocumentElement.InnerXml;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        [COEUserActionDescription("GetConfigurationSettingsXml")]
        public string GetConfigurationSettingsXml()
        {
            try
            {
                return FrameworkUtils.GetAppConfigSettingsXml("Registration");
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        [COEUserActionDescription("ImportTableList")]
        public void ImportTableList(string xmlTableList)
        {
            try
            {
                this.RegDal.ImportTableList(xmlTableList);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        [COEUserActionDescription("ImportConfigurationSettings")]
        public void ImportConfigurationSettings(string configSettings)
        {
            try
            {
                string appName = COEAppName.Get();
                string configName = ConfigurationManager.AppSettings["ConfigurationName"];
                COEConfigurationBO bo = COEConfigurationBO.Get(appName, configName);
                if (_forceImport || bo == null)
                    FrameworkUtils.SaveConfigurationSettingsFromXml(appName, configSettings);
                else
                {
                    bo = COEConfigurationBO.New(appName, configName);
                    //bo can remain null if server cache does not have any value within it
                    if (bo != null) //Coverity fix - CID 11794 
                    {
                        bo.SetConfigurationSettingsFromXml(configSettings);
                        bo.Save();
                    }
                }

                // Update Auditing status explicitly, as the import process didn't cover that.
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(configSettings);
                if (doc.SelectNodes("Registration/applicationSettings/groups/add[@name='REGADMIN']/settings/add[@name='EnableAuditing']").Count > 0)
                {
                    string auditingStatus = doc.SelectSingleNode(
                        "Registration/applicationSettings/groups/add[@name='REGADMIN']/settings/add[@name='EnableAuditing']"
                        ).Attributes["value"].Value;

                    AuditingConfigurationProcessor processor = new AuditingConfigurationProcessor();
                    processor.Process("EnableAuditing", "", auditingStatus);
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Gets database reserved words list
        /// </summary>
        private void GetDatabaseReservedWords() 
        {
            this._databaseReservedWords = new List<string>();
            using (SafeDataReader sd = this.RegDal.GetDatabaseReservedWords())
            {
                while (sd.Read())
                {
                    _databaseReservedWords.Add(sd.GetString("KEYWORD"));
                }
            }
        }

        /// <summary>
        /// Gets column names
        /// </summary>
        private void GetTableColumns()
        {
            List<string> addColumnsToList = new List<string>();
            this._batchPropertyColumnList = new List<string>();
            this._batchComponentColumnList = new List<string>();
            this._compoundPropertyColumnList = new List<string>();
            this._propertyColumnList = new List<string>();
            this._structurePropertyColumnList = new List<string>();

            PropertyTable[] tableNames = (PropertyTable[])Enum.GetValues(typeof(PropertyTable));
            foreach (PropertyTable table in tableNames)
            {
                using (SafeDataReader sd = this.RegDal.GetColumns(table.ToString()))
                {
                    while (sd.Read())
                    {
                        addColumnsToList.Add(sd.GetString("COLUMNNAME"));
                    }
                }
                switch (table)
                {
                    case PropertyTable.Mixtures:
                       _propertyColumnList.AddRange(addColumnsToList);
                        break;
                    case PropertyTable.Batches:
                        _batchPropertyColumnList.AddRange(addColumnsToList);
                        break;
                    case PropertyTable.Compound_Molecule:
                        _compoundPropertyColumnList.AddRange(addColumnsToList);
                        break;
                    case PropertyTable.BatchComponent:
                        _batchComponentColumnList.AddRange(addColumnsToList);
                        break;
                    case PropertyTable.Structures:
                        _structurePropertyColumnList.AddRange(addColumnsToList);
                        break;
                }

                addColumnsToList.Clear();
             
            }
        }


        /// <summary>
        /// Validates or Updates the log xml of any property for which user is trying to add validations. 
        /// </summary>
        /// <returns>True property is valid and can proceed to add validations, False property is invalid and cannot proceed to add validations without default value</returns>
        private bool IsValidPropertyLog()
        {
            bool retVal = true;
            string proListType = string.Empty;

            try
            {
                StringBuilder errorBuilder = new StringBuilder();
                XmlDocument xmlData = new XmlDocument();
                XmlNode errors = null;
                if (PropertyStatusLog.Trim().Length > 0)
                {
                    xmlData.LoadXml(PropertyStatusLog);
                    errors = xmlData.SelectSingleNode("./ErrorList");
                    foreach (XmlNode error in errors)
                    {
                        string operation = error.SelectSingleNode("./Operation").InnerText;
                        string field = error.SelectSingleNode("./Field").InnerText;
                        string fieldType = error.SelectSingleNode("./Fieldtype").InnerText;
                        string table = error.SelectSingleNode("./Object").InnerText;
                        string deFaultValue = error.SelectSingleNode("./DefaultValue").InnerText;
                        string errorCode = error.SelectSingleNode("./ErrorCode").InnerText;
                        switch (operation.ToUpper())
                        {
                            case "VALIDATING":
                                switch (errorCode)
                                {
                                    case "-20002":
                                        if (_defalutValueForProp.Trim().Length > 0)
                                        {
                                            error.SelectSingleNode("./DefaultValue").InnerText = DefalutValue;
                                            retVal = true;
                                        }
                                        else
                                            retVal = false;
                                        break;
                                }
                                break;
                        }
                    }
                }
                else
                {
                    retVal = false;
                }
                PropertyStatusLog = xmlData.OuterXml;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                retVal = false;
            }
            return retVal;
        }

        /// <summary>
        /// Informs, Updates the errorlog of a property, column for which user trying to add validations
        /// </summary>
        /// <param name="propertyListType">Property type Ex: Batch, Mixture, Compound</param>
        /// <param name="PropertyName">Property name or column name to which user is trying to add validations</param>
        /// <returns>Returns true if valid. </returns>
        private bool GetPropertyStatus(PropertyListType propertyListType, string propertyName)
        {
            bool retVal = true;
            string proListType = string.Empty;
            PropertyStatusLog = string.Empty;

            try
            {
                if (propertyListType == PropertyListType.PropertyList)
                    proListType = "MIXTURE";
                else
                    proListType = propertyListType.ToString();

                if (_checkPropertyStatusInDB.Count > 0)
                {
                    if (_checkPropertyStatusInDB.ContainsKey(propertyName))
                        PropertyStatusLog = _checkPropertyStatusInDB[propertyName];
                }

                if (!(PropertyStatusLog.Trim().Length > 0))
                {
                    PropertyStatusLog = this.RegDal.GetPropertyStatusInDB(proListType, propertyName);
                    _checkPropertyStatusInDB.Add(propertyName, PropertyStatusLog);
                }

                if (!this.IsValidPropertyLog())
                {
                    retVal = false;
                }
                else
                    PropertyStatusLog = string.Empty;  
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                retVal = false;
            }
            return retVal;
        }


        /// <summary>
        /// Calls the DAL layer to Update the default value for columns in DB using error log which contains table, column, value information.
        /// </summary>
        /// <param name="statusLog">Error log used in procedure</param>
        /// <returns>True if updated succesfully, False if fails to update</returns>
        private bool UpdatePropertyStatusLog()
        {
            bool retVal = true;
            try
            {
                if (this.IsValidPropertyLog() && this.SaveDefaultValue)
                {
                    retVal = this.RegDal.UpdatePropertyStatusInDB(PropertyStatusLog);
                    if (retVal)
                    {
                        if (_checkPropertyStatusInDB.ContainsKey(this.SelectedPropertyName))
                            _checkPropertyStatusInDB.Remove(this.SelectedPropertyName);
                    }
                }
                else
                {
                    if (_checkPropertyStatusInDB.ContainsKey(this.SelectedPropertyName))
                        _checkPropertyStatusInDB.Remove(this.SelectedPropertyName);
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                retVal = false;
            }
            return retVal;
        }

        /// <summary>
        /// Extract out the required field list from the Reg.ini information
        /// </summary>
        /// <param name="iniReg">The Reg.ini setting information</param>
        /// <returns>A list of required field name</returns>
        public DataSet GetPickListValues(string pickListDomainId)
        {
            DataSet dsTable = new DataSet();
            try
            {
                PicklistDomain pickListDomain = PicklistDomain.GetPicklistDomain(int.Parse(pickListDomainId));
                dsTable = this.RegDal.ExecuteSqlQuery(pickListDomain.PickListDomainSql);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return dsTable;
        }

        private ConfigurationRegistryRecord(PropertyList propertyList, PropertyList batchPropertyList, PropertyList batchComponentList,
            PropertyList compoundPropertyList, AddInList addInList)
        {

            _propertyList = PropertyList.NewPropertyList();
            foreach (Property prop in propertyList)
            {
                ConfigurationProperty confProp = ConfigurationProperty.NewConfigurationProperty(propertyList[prop.Name], true, false);
                _propertyList.Add(confProp);

            }

            _batchPropertyList = PropertyList.NewPropertyList();
            foreach (Property prop in batchPropertyList)
            {
                ConfigurationProperty confProp = ConfigurationProperty.NewConfigurationProperty(batchPropertyList[prop.Name], true, false);
                _batchPropertyList.Add(confProp);

            }
            _batchComponentList = PropertyList.NewPropertyList();
            foreach (Property prop in batchComponentList)
            {
                ConfigurationProperty confProp = ConfigurationProperty.NewConfigurationProperty(batchComponentList[prop.Name], true, false);
                _batchComponentList.Add(confProp);
            }
            _compoundPropertyList = PropertyList.NewPropertyList();
            foreach (Property prop in compoundPropertyList)
            {
                ConfigurationProperty confProp = ConfigurationProperty.NewConfigurationProperty(compoundPropertyList[prop.Name], true, false);
                _compoundPropertyList.Add(confProp);

            }
            _addInList = addInList;
            FillAddInAssemblyList();
            FillEventNames();            
            SelectedPropertyList = PropertyListType.None;
        }

        private ConfigurationRegistryRecord()
        {
            _propertyList = PropertyList.NewPropertyList();
            _batchPropertyList = PropertyList.NewPropertyList();
            _batchComponentList = PropertyList.NewPropertyList();
            _compoundPropertyList = PropertyList.NewPropertyList();
            _structurePropertyList = PropertyList.NewPropertyList();
            _addInList = AddInList.NewAddInList();
            FillAddInAssemblyList();
            FillEventNames();
            _selectedPropertyList = PropertyListType.None;
        }

        private ConfigurationRegistryRecord(string xml)
            : this()
        {
            InitializeFromXml(xml, false);
        }

        [COEUserActionDescription("CreateConfigurationRegistryRecord")]
        public static ConfigurationRegistryRecord NewConfigurationRegistryRecord(string xml)
        {
            try
            {
                ConfigurationRegistryRecord crr = new ConfigurationRegistryRecord(xml);
                crr.MarkNew();
                return crr;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        [COEUserActionDescription("CreateConfigurationRegistryRecord")]
        public static ConfigurationRegistryRecord NewConfigurationRegistryRecord()
        {
            try
            {
                ConfigurationRegistryRecord crr = new ConfigurationRegistryRecord();
                crr.DataPortal_Fetch(new object());
                crr.MarkOld();
                crr.MarkClean();
                return crr;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        private void InitializeFromXml(string xml, bool fromDatabase)
        {
            PropertyList propertyList;
            PropertyList compoundPropertyList;
            PropertyList batchPropertyList;
            PropertyList batchComponentList;
            CambridgeSoft.COE.Registration.Services.Types.PropertyList structurePropertyList;

            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();


            XPathNodeIterator xIterator = xNavigator.Select("ConfigurationRegistryRecord/PropertyList");
            if (xIterator.MoveNext())
            {
                propertyList = PropertyList.NewPropertyList(xIterator.Current.OuterXml, true);
                _propertyList = PropertyList.NewPropertyList();

                foreach (Property prop in propertyList)
                {
                    _propertyList.Add(ConfigurationProperty.NewConfigurationProperty(prop, true, !fromDatabase));
                }
            }


            xIterator = xNavigator.Select("ConfigurationRegistryRecord/Compound/PropertyList");
            if (xIterator.MoveNext())
            {
                compoundPropertyList = PropertyList.NewPropertyList(xIterator.Current.OuterXml, true);
                _compoundPropertyList = PropertyList.NewPropertyList();


                foreach (Property prop in compoundPropertyList)
                {
                    _compoundPropertyList.Add(ConfigurationProperty.NewConfigurationProperty(prop, true, !fromDatabase));
                }
            }

            xIterator = xNavigator.Select("ConfigurationRegistryRecord/Batch/PropertyList");
            if (xIterator.MoveNext())
            {
                batchPropertyList = PropertyList.NewPropertyList(xIterator.Current.OuterXml, true);
                _batchPropertyList = PropertyList.NewPropertyList();


                foreach (Property prop in batchPropertyList)
                {
                    _batchPropertyList.Add(ConfigurationProperty.NewConfigurationProperty(prop, true, !fromDatabase));
                }
            }

            xIterator = xNavigator.Select("ConfigurationRegistryRecord/BatchComponent/PropertyList");
            if (xIterator.MoveNext())
            {

                batchComponentList = PropertyList.NewPropertyList(xIterator.Current.OuterXml, true);
                _batchComponentList = PropertyList.NewPropertyList();


                foreach (Property prop in batchComponentList)
                {
                    _batchComponentList.Add(ConfigurationProperty.NewConfigurationProperty(prop, true, !fromDatabase));
                }
            }

            xIterator = xNavigator.Select("ConfigurationRegistryRecord/Structure/PropertyList");
            if (xIterator.MoveNext())
            {
                structurePropertyList = CambridgeSoft.COE.Registration.Services.Types.PropertyList.NewPropertyList(
                    xIterator.Current.OuterXml, true);
                _structurePropertyList = CambridgeSoft.COE.Registration.Services.Types.PropertyList.NewPropertyList();

                foreach (Property prop in structurePropertyList)
                {
                    _structurePropertyList.Add(ConfigurationProperty.NewConfigurationProperty(prop, true, !fromDatabase));
                }
            }

            _addInList = AddInList.NewAddInList();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNode addInsNode = doc.SelectSingleNode("ConfigurationRegistryRecord/AddIns");
            if (addInsNode != null)
            {
                AddInList addInsList = AddInList.NewAddInList(addInsNode);
                this._addInList = addInsList;
            }
            foreach (Dictionary<string, string> propLabels in _propertiesLabels)
                propLabels.Clear();
        }

        #endregion

        #region Data Access

        [Transactional(TransactionalTypes.Manual)]
        protected override void DataPortal_Insert()
        {
            string errorLog = string.Empty;
            _errorLog = this.RegDal.InsertConfigurationRegistryRecord(_xml);
        }

        [Transactional(TransactionalTypes.Manual)]
        protected override void DataPortal_Update()
        {
            _errorLog = this.RegDal.InsertConfigurationRegistryRecord(_xml);

            XmlDocument errorXml = new XmlDocument();
            errorXml.LoadXml(_errorLog);

            this.CheckErrorLog(); // Checks the error log and updates 'IsConfigSavedSuccessfully PROP' if  COEObjectConfig was saved succesfully or not.

            if (!IsImported) 
            {
                _coeFormHelper = new COEFormHelper(this);
                _coeFormHelper.UpdateRegistrationFormGroups();
            }

            // Configuration object saved in session must need to update to default.
            IsImported = false;  
            IsConfigSavedSuccessfully = true;

            this.ClearDeletedList();
            this.GetTableColumns();
            this.UpdatePropertyStatusLog();

            if (errorXml.SelectSingleNode("//ErrorList").HasChildNodes)
            {
                this.UpdateSelf();

                _errorLog = this.RegDal.InsertConfigurationRegistryRecord(this.UpdatePropertiesSortOrder());

                this.InitializeFromXml(this.RegDal.GetConfigurationRegistryRecord(), true);
            }
            else
                this.InitializeFromXml(this.RegDal.GetConfigurationRegistryRecord(), true);
        }

        [Transactional(TransactionalTypes.Manual)]
        protected override void DataPortal_Fetch(object criteria)
        {
            this.GetDatabaseReservedWords();
            this.ClearDeletedList();
            this.GetTableColumns();

            this.InitializeFromXml(this.RegDal.GetConfigurationRegistryRecord(), true);

        }

        #endregion

        #region Enums

        public enum PropertyListType
        {
            Batch = 0,
            BatchComponent = 1,
            Compound = 2,
            AddIns = 3,
            PropertyList = 5,
            Structure = 6,
            None = 4
        }

        [Serializable]
        public enum PropertyTypeEnum
        {
            [XmlEnum("text")]
            Text,
            [XmlEnum("date")]
            Date,
            [XmlEnum("boolean")]
            Boolean,
            [XmlEnum("number")]
            Number,
            [XmlEnum("pickListDomain")]
            PickListDomain,
        }

        public enum IniSettingsTypes
        {
            AppSetting,
            FieldConfig
        }

        private enum PropertyTable
        {
            Mixtures,
            Compound_Molecule,
            Structures,
            Batches,
            BatchComponent,
        }

        #endregion
    }
}
