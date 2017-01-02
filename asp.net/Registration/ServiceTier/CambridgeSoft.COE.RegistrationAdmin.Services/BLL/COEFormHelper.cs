using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;
using System.Xml;

using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.IniParser;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.ExceptionHandling;

using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Registration.Services.BLL;

using CambridgeSoft.COE.RegistrationAdmin.Services.Common;
using CambridgeSoft.COE.Registration;

namespace CambridgeSoft.COE.RegistrationAdmin.Services
{
    //TODO: This is a 4k line class. Break this sucker up into manageable pieces! -- JED

    public class COEFormHelper
    {

        #region Variables
        private PropertyList _rootProperties = PropertyList.NewPropertyList();
        private PropertyList _compoundProperties = PropertyList.NewPropertyList();
        private PropertyList _structureProperties = PropertyList.NewPropertyList();
        private PropertyList _batchProperties = PropertyList.NewPropertyList();
        private PropertyList _batchComponentProperties = PropertyList.NewPropertyList();
        private COEFormBO _formGroupBO;
        private SearchCriteria _searchCriteria;

        private const int DETAILFORMINDEX = 0;
        private const int QUERYFORMINDEX = 0;
        private const int LISTFORMINDEX = 0;
        public const int MIXTURESUBFORMINDEX = 1000;
        public const int COMPOUNDSUBFORMINDEX = 1001;
        public const int STRUCTURESUBFORMINDEX = 1;//id of the coeform in which formelements for Structure level properties reside
        //LJB: the form 2 should only be modified for review/register 4011. modifying 4012 has unintended consequences and must be avoided
        public const int STRUCTUREVIEWMODESUBFORMINDEX = 2;//id of the coeform in which formelements for Structure level properties reside but in view mode - 
        
        public const int BATCHSUBFORMINDEX = 1002;
        public const int BATCHCOMPONENTSUBFORMINDEX = 1003;

        private const int TEMPORARYBASEFORM = 0;
        private const int TEMPORARYCHILDFORM = 1;

        private const int MIXTURESEARCHFORM = 0;
        private const int COMPOUNDSEARCHFORM = 1;
        private const int BATCHSEARCHFORM = 2;
        private const int BATCHCOMPONENTSEARCHFORM = 3;
        private const int STRUCTURESEARCHFORM = 4;
        private const string MIXTURETABLENAME = "VW_MIXTURE_REGNUMBER";
        private const string COMPOUNDTABLENAME = "VW_MIXTURE_STRUCTURE";
        private const string STRUCTURETABLENAME = "VW_MIXTURE_STRUCTURE";//TODO:what is the table name in dataview for Stucture 
        private const string BATCHTABLENAME = "VW_BATCH";
        private const string BATCHCOMPONENTTABLENAME = "VW_MIXTURE_BATCHCOMPONENT";
        private const string TEMPBATCHTABLENAME = "VW_TEMPORARYBATCH";
        private const string TEMPCOMPOUNDTABLENAME = "VW_TEMPORARYCOMPOUND";

        private const int RESERVEDAMOUNTCUSTOMDTFIELDID = 500;

        private Dictionary<string, string>[] _formElementsLabels;

        #endregion

        #region Form Properties

        private int SUBMITMIXTUREFORMID
        { get { return this.GetFormID("SubmitRegistryFormGroupId"); } }

        private int REGISTERMIXTUREFORMID
        { get { return this.GetFormID("ReviewRegisterRegistryFormGroupId"); } }

        private int VIEWMIXTUREFORMID
        { get { return this.GetFormID("ViewRegistryFormGroupId"); } }

        private int SEARCHTEMPFORMID
        { get { return this.GetFormID("ReviewRegisterSearchFormGroupId"); } }

        private int SEARCHPERMFORMID
        { get { return this.GetFormID("ViewRegistrySearchFormGroupId"); } }

        private int ELNSEARCHTEMPFORMID
        { get { return this.GetFormID("ELNReviewRegisterSearchFormGroupId"); } }

        private int ELNSEARCHPERMFORMID
        { get { return this.GetFormID("ELNViewRegistrySearchFormGroupId"); } }

        private int DATALOADERFORMID
        { get { return this.GetFormID("DataLoaderFormGroupId", "REGADMIN"); } }

        private int COMPONENTDUPLICATESFORMID
        { get { return this.GetFormID("ComponentDuplicatesFormGroupId"); } }

        private int REGISTRYDUPLICATESFORMID
        { get { return this.GetFormID("RegistryDuplicatesFormGroupId"); } }

        private int SENDTOREGISTRATIONFORMID
        { get { return this.GetFormID("SendToRegistrationFormGroupId"); } }

        private int DELETELOGFORMID
        { get { return this.GetFormID("DeleteLogFormGroupId"); } }

        private int SEARCHCOMPONENTSTOADDFORMID
        { get { return this.GetFormID("SearchComponentsToAddFormGroupId"); } }

        private int SEARCHCOMPONENTSTOADDRRFORMID
        { get { return this.GetFormID("SearchComponentsToAddRRFormGroupId"); } }

        public FormGroup FormGroup
        {
            get
            {
                try
                {
                    if (_formGroupBO == null)
                    {
                        _formGroupBO = COEFormBO.Get(SUBMITMIXTUREFORMID);
                    }
                    return _formGroupBO.COEFormGroup;
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form MixtureForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != SUBMITMIXTUREFORMID)
                {
                    _formGroupBO = COEFormBO.Get(SUBMITMIXTUREFORMID);
                }
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.DetailsForms, 0, MIXTURESUBFORMINDEX);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form CompoundForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != SUBMITMIXTUREFORMID)
                    _formGroupBO = COEFormBO.Get(SUBMITMIXTUREFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.DetailsForms, 0, COMPOUNDSUBFORMINDEX);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form BatchForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != SUBMITMIXTUREFORMID)
                    _formGroupBO = COEFormBO.Get(SUBMITMIXTUREFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.DetailsForms, 0, BATCHSUBFORMINDEX);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form BatchComponentForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != SUBMITMIXTUREFORMID)
                    _formGroupBO = COEFormBO.Get(SUBMITMIXTUREFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.DetailsForms, 0, BATCHCOMPONENTSUBFORMINDEX);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form SearchTempBaseQueryForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != SEARCHTEMPFORMID)
                    _formGroupBO = COEFormBO.Get(this.SEARCHTEMPFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.QueryForms, QUERYFORMINDEX, TEMPORARYBASEFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form SearchTempChildQueryForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != SEARCHTEMPFORMID)
                    _formGroupBO = COEFormBO.Get(this.SEARCHTEMPFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.QueryForms, QUERYFORMINDEX, TEMPORARYCHILDFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form SearchTempDetailsBaseForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != SEARCHTEMPFORMID)
                    _formGroupBO = COEFormBO.Get(this.SEARCHTEMPFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.DetailsForms, DETAILFORMINDEX, TEMPORARYBASEFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form SearchTempDetailsChildForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != SEARCHTEMPFORMID)
                    _formGroupBO = COEFormBO.Get(this.SEARCHTEMPFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.DetailsForms, DETAILFORMINDEX, TEMPORARYCHILDFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form SearchPermMixtureQueryForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != SEARCHPERMFORMID)
                    _formGroupBO = COEFormBO.Get(this.SEARCHPERMFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.QueryForms, 0, MIXTURESEARCHFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form SearchPermCompoundQueryForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != SEARCHPERMFORMID)
                    _formGroupBO = COEFormBO.Get(this.SEARCHPERMFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.QueryForms, QUERYFORMINDEX, COMPOUNDSEARCHFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form SearchPermBatchQueryForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != SEARCHPERMFORMID)
                    _formGroupBO = COEFormBO.Get(this.SEARCHPERMFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.QueryForms, QUERYFORMINDEX, BATCHSEARCHFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form SearchPermBatchComponentQueryForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != SEARCHPERMFORMID)
                    _formGroupBO = COEFormBO.Get(this.SEARCHPERMFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.QueryForms, QUERYFORMINDEX, BATCHCOMPONENTSEARCHFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form SearchPermMixtureDetailForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != SEARCHPERMFORMID)
                    _formGroupBO = COEFormBO.Get(this.SEARCHPERMFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.DetailsForms, DETAILFORMINDEX, MIXTURESEARCHFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form SearchPermCompoundDetailForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != SEARCHPERMFORMID)
                    _formGroupBO = COEFormBO.Get(this.SEARCHPERMFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.DetailsForms, DETAILFORMINDEX, COMPOUNDSEARCHFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form SearchPermBatchDetailForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != SEARCHPERMFORMID)
                    _formGroupBO = COEFormBO.Get(this.SEARCHPERMFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.DetailsForms, DETAILFORMINDEX, BATCHSEARCHFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form SearchPermBatchComponentDetailForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != SEARCHPERMFORMID)
                    _formGroupBO = COEFormBO.Get(this.SEARCHPERMFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.DetailsForms, 0, BATCHCOMPONENTSEARCHFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form SearchTempListForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != SEARCHTEMPFORMID)
                    _formGroupBO = COEFormBO.Get(this.SEARCHTEMPFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.ListForms, LISTFORMINDEX, 0);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form SearchPermanentListForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != SEARCHPERMFORMID)
                    _formGroupBO = COEFormBO.Get(this.SEARCHPERMFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.ListForms, LISTFORMINDEX, 0);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form ELNSearchTempBaseQueryForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != ELNSEARCHTEMPFORMID)
                    _formGroupBO = COEFormBO.Get(this.ELNSEARCHTEMPFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.QueryForms, QUERYFORMINDEX, TEMPORARYBASEFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form ELNSearchTempChildQueryForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != ELNSEARCHTEMPFORMID)
                    _formGroupBO = COEFormBO.Get(this.ELNSEARCHTEMPFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.QueryForms, QUERYFORMINDEX, TEMPORARYCHILDFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form ELNSearchTempDetailsBaseForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != ELNSEARCHTEMPFORMID)
                    _formGroupBO = COEFormBO.Get(this.ELNSEARCHTEMPFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.DetailsForms, DETAILFORMINDEX, TEMPORARYBASEFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form ELNSearchTempDetailsChildForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != ELNSEARCHTEMPFORMID)
                    _formGroupBO = COEFormBO.Get(this.ELNSEARCHTEMPFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.DetailsForms, DETAILFORMINDEX, TEMPORARYCHILDFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form ELNSearchPermMixtureQueryForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != ELNSEARCHPERMFORMID)
                    _formGroupBO = COEFormBO.Get(this.ELNSEARCHPERMFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.QueryForms, QUERYFORMINDEX, MIXTURESEARCHFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form ELNSearchPermCompoundQueryForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != ELNSEARCHPERMFORMID)
                    _formGroupBO = COEFormBO.Get(this.ELNSEARCHPERMFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.QueryForms, QUERYFORMINDEX, COMPOUNDSEARCHFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form ELNSearchPermBatchQueryForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != ELNSEARCHPERMFORMID)
                    _formGroupBO = COEFormBO.Get(this.ELNSEARCHPERMFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.QueryForms, QUERYFORMINDEX, BATCHSEARCHFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form ELNSearchPermBatchComponentQueryForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != ELNSEARCHPERMFORMID)
                    _formGroupBO = COEFormBO.Get(this.ELNSEARCHPERMFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.QueryForms, QUERYFORMINDEX, BATCHCOMPONENTSEARCHFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form ELNSearchPermMixtureDetailForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != ELNSEARCHPERMFORMID)
                    _formGroupBO = COEFormBO.Get(this.ELNSEARCHPERMFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.DetailsForms, DETAILFORMINDEX, MIXTURESEARCHFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form ELNSearchPermCompoundDetailForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != ELNSEARCHPERMFORMID)
                    _formGroupBO = COEFormBO.Get(this.ELNSEARCHPERMFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.DetailsForms, DETAILFORMINDEX, COMPOUNDSEARCHFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form ELNSearchPermBatchDetailForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != ELNSEARCHPERMFORMID)
                    _formGroupBO = COEFormBO.Get(this.ELNSEARCHPERMFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.DetailsForms, DETAILFORMINDEX, BATCHSEARCHFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form ELNSearchPermBatchComponentDetailForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != ELNSEARCHPERMFORMID)
                    _formGroupBO = COEFormBO.Get(this.ELNSEARCHPERMFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.DetailsForms, 0, BATCHCOMPONENTSEARCHFORM);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form ELNSearchTempListForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != ELNSEARCHTEMPFORMID)
                    _formGroupBO = COEFormBO.Get(this.ELNSEARCHTEMPFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.ListForms, LISTFORMINDEX, 0);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form ELNSearchPermanentListForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != ELNSEARCHPERMFORMID)
                    _formGroupBO = COEFormBO.Get(this.ELNSEARCHPERMFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.ListForms, LISTFORMINDEX, 0);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
        }

        public FormGroup.Form DataLoaderForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != DATALOADERFORMID)
                    _formGroupBO = COEFormBO.Get(this.DATALOADERFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.DetailsForms, DETAILFORMINDEX, 0);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }

        }

        public FormGroup.Form RegistryDuplicatesForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != REGISTRYDUPLICATESFORMID)
                    _formGroupBO = COEFormBO.Get(this.REGISTRYDUPLICATESFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.DetailsForms, DETAILFORMINDEX, 0);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }

        }

        public FormGroup.Form ComponentDuplicatesForm
        {
            get
            {
                if (_formGroupBO == null || _formGroupBO.ID != COMPONENTDUPLICATESFORMID)
                    _formGroupBO = COEFormBO.Get(this.COMPONENTDUPLICATESFORMID);
                try
                {
                    return _formGroupBO.GetForm(_formGroupBO.COEFormGroup.DetailsForms, DETAILFORMINDEX, 0);
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }

        }

        #endregion

        #region Constructor
        [COEUserActionDescription("CreateCOEFormHelper")]
        public COEFormHelper(ConfigurationRegistryRecord configRecord)
        {
            try
            {
                _rootProperties = configRecord.PropertyList.GetSortedPropertyList();
                _compoundProperties = configRecord.CompoundPropertyList.GetSortedPropertyList();
                _structureProperties = configRecord.StructurePropertyList.GetSortedPropertyList();
                _batchProperties = configRecord.BatchPropertyList.GetSortedPropertyList();
                _batchComponentProperties = configRecord.BatchComponentList.GetSortedPropertyList();
                _formElementsLabels = configRecord.PropertiesLabels;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }
        #endregion

        #region Private Methods
        private List<FormGroup.ValidationRuleInfo> ConvertToClientValidations(ValidationRuleList validationRuleList)
        {
            List<FormGroup.ValidationRuleInfo> result = new List<FormGroup.ValidationRuleInfo>();
            foreach (CambridgeSoft.COE.Registration.Services.Types.ValidationRule rule in validationRuleList)
            {
                if (!string.IsNullOrEmpty(rule.Name))
                {
                    FormGroup.ValidationRuleInfo clientRule = new FormGroup.ValidationRuleInfo();
                    clientRule.ValidationRuleName = GetClientValidationRuleNameFromServerValidation(rule.Name);
                    clientRule.ErrorMessage = rule.Error;
                    foreach (CambridgeSoft.COE.Registration.Services.BLL.Parameter param in rule.Parameters)
                    {
                        FormGroup.Parameter clientParam = new FormGroup.Parameter();
                        clientParam.Name = param.Name;
                        clientParam.Value = param.Value;
                        clientRule.Params.Add(clientParam);
                    }
                    result.Add(clientRule);
                }
            }
            return result;
        }

        private FormGroup.ValidationRuleEnum GetClientValidationRuleNameFromServerValidation(string ruleName)
        {
            string nameLowered = ruleName.ToLower();
            if (nameLowered == FormGroup.ValidationRuleEnum.Float.ToString().ToLower())
            {
                return FormGroup.ValidationRuleEnum.Float;
            }
            if (nameLowered == FormGroup.ValidationRuleEnum.TextLength.ToString().ToLower())
            {
                return FormGroup.ValidationRuleEnum.TextLength;
            }
            else if (nameLowered == FormGroup.ValidationRuleEnum.Date.ToString().ToLower())
            {
                return FormGroup.ValidationRuleEnum.Date;
            }
            else if (nameLowered == FormGroup.ValidationRuleEnum.Double.ToString().ToLower())
            {
                return FormGroup.ValidationRuleEnum.Double;
            }
            else if (nameLowered == FormGroup.ValidationRuleEnum.Integer.ToString().ToLower())
            {
                return FormGroup.ValidationRuleEnum.Integer;
            }
            else if (nameLowered == FormGroup.ValidationRuleEnum.NumericRange.ToString().ToLower())
            {
                return FormGroup.ValidationRuleEnum.NumericRange;
            }
            else if (nameLowered == FormGroup.ValidationRuleEnum.Custom.ToString().ToLower())
            {
                return FormGroup.ValidationRuleEnum.Custom;
            }
            //else if (nameLowered == FormGroup.ValidationRuleEnum.OnlyChemicalContentAllowed.ToString().ToLower())
            //{
            //    return FormGroup.ValidationRuleEnum.OnlyChemicalContentAllowed;
            //}
            else if (nameLowered == FormGroup.ValidationRuleEnum.PositiveInteger.ToString().ToLower())
            {
                return FormGroup.ValidationRuleEnum.PositiveInteger;
            }
            else if (nameLowered == FormGroup.ValidationRuleEnum.RequiredField.ToString().ToLower())
            {
                return FormGroup.ValidationRuleEnum.RequiredField;
            }
            else if (nameLowered == FormGroup.ValidationRuleEnum.WordListEnumeration.ToString().ToLower())
            {
                return FormGroup.ValidationRuleEnum.WordListEnumeration;
            }
            else if (nameLowered == FormGroup.ValidationRuleEnum.NotEmptyStructure.ToString().ToLower())
            {
                return FormGroup.ValidationRuleEnum.NotEmptyStructure;
            }
            else if (nameLowered == FormGroup.ValidationRuleEnum.NotEmptyStructureAndNoText.ToString().ToLower())
            {
                return FormGroup.ValidationRuleEnum.NotEmptyStructureAndNoText;
            }
            return FormGroup.ValidationRuleEnum.TextLength;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurationPropertyList"></param>
        /// <param name="propertyListType"></param>
        /// <param name="addAndEditFormElements"></param>
        /// <param name="viewFormElements"></param>
        /// <param name="formElementsIdToRemove"></param>
        /// <param name="baseBindingExpression"></param>
        private void BuildFormElementsList(PropertyList configurationPropertyList, PropertyListType propertyListType, ref List<FormGroup.FormElement> addAndEditFormElements, ref List<FormGroup.FormElement> viewFormElements, ref List<string> formElementsIdToRemove, string baseBindingExpression)
        {
            foreach (ConfigurationProperty configurationProperty in configurationPropertyList)
            {
                if (IsInFormElementLabels(propertyListType, configurationProperty)  && (configurationProperty.IsNew || configurationProperty.IsDirty))
                {
                    FormGroup.FormElement addAndEditElement = new FormGroup.FormElement();
                    FormGroup.FormElement viewElement = new FormGroup.FormElement();
                    viewElement.BindingExpression = addAndEditElement.BindingExpression = baseBindingExpression + "[@Name='" + configurationProperty.Name + "'| Value]";
                    addAndEditElement.ValidationRuleList = ConvertToClientValidations(configurationProperty.ValidationRuleList);

                    viewElement.Label = addAndEditElement.Label = this.GetFormElementLabel(propertyListType, configurationProperty);
                    viewElement.Name = addAndEditElement.Name = configurationProperty.Name;
                    viewElement.Id = addAndEditElement.Id = configurationProperty.Name + "Property";
                    addAndEditElement.DisplayInfo.Type = GetDefaultControlType(configurationProperty.Name, propertyListType, FormGroup.DisplayMode.Add);
                    if(addAndEditElement.DisplayInfo.Type.Contains("COECheckBox"))
                        addAndEditElement.DefaultValue = "false";
                    viewElement.DisplayInfo.Type = GetDefaultControlType(configurationProperty.Name, propertyListType, FormGroup.DisplayMode.View);
                    if (addAndEditElement.DisplayInfo.Type.Contains("COEDatePicker"))
                        addAndEditElement.DisplayInfo.CSSClass = "";
                    else
                        addAndEditElement.DisplayInfo.CSSClass = RegAdminUtils.GetFormElementCSSClass(addAndEditElement.DisplayInfo.Type);
                    
                    viewElement.DisplayInfo.CSSClass = RegAdminUtils.GetFormElementCSSClass(viewElement.DisplayInfo.Type);
                    addAndEditElement.DisplayInfo.Visible = viewElement.DisplayInfo.Visible = true;

                    viewElement.ConfigInfo = this.BuildConfigInfo(viewElement, configurationProperty, FormGroup.DisplayMode.View, propertyListType);
                    addAndEditElement.ConfigInfo = this.BuildConfigInfo(addAndEditElement, configurationProperty, FormGroup.DisplayMode.All, propertyListType);

                    viewFormElements.Add(viewElement);
                    addAndEditFormElements.Add(addAndEditElement);
                }
            }
            foreach (ConfigurationProperty configProperty in configurationPropertyList.GetDeletedList())
            {
                formElementsIdToRemove.Add(configProperty.Name + "Property");
            }
        }

        /// <summary>
        /// For each new ConfigurationProperty in <paramref name="propertyListA"/> or <paramref name="propertyListB"/>,
        /// add a new FormElement instance to <paramref name="queryFormElements"/>
        /// </summary>
        /// <param name="table"></param>
        /// <param name="queryFormElements">a FormElement list to be populated</param>
        /// <param name="propertyListA"></param>
        /// <param name="propListAType"></param>
        /// <param name="propertyListB"></param>
        /// <param name="propListBType"></param>
        private void BuildFormElementsListForSearch(COEDataView.DataViewTable table, ref List<FormGroup.FormElement> queryFormElements, PropertyList propertyListA, PropertyListType propListAType, PropertyList propertyListB, PropertyListType propListBType)
        {
            foreach (COEDataView.Field field in table.Fields)
            {
                Property prop = Property.NewProperty();
                prop.Name = field.Name;
                if ((propertyListA.CheckExistingNames(prop.Name, false) && propertyListA[field.Name].IsNew) || (propertyListB.CheckExistingNames(prop.Name, false) && propertyListB[field.Name].IsNew))
                {
                    FormGroup.FormElement queryElement = new FormGroup.FormElement();
                    SearchCriteria.SearchCriteriaItem searchCriteria = new SearchCriteria.SearchCriteriaItem();
                    searchCriteria.FieldId = field.Id;
                    searchCriteria.TableId = table.Id;
                    //calculate next ID value
                    int maxIDValue = 0;
                    foreach (SearchCriteria.SearchExpression item in _searchCriteria.Items)
                    {
                        SearchCriteria.SearchCriteriaItem criteriaItem = item as SearchCriteria.SearchCriteriaItem;
                        if (criteriaItem != null && criteriaItem.ID > maxIDValue) maxIDValue = criteriaItem.ID;
                    }
                    searchCriteria.ID = maxIDValue + 1;

                    queryElement.BindingExpression = string.Format("SearchCriteria[{0}].Criterium.Value", searchCriteria.ID);
                    //determine where to get the label and validation rule list
                    if (propertyListA.CheckExistingNames(prop.Name, false))
                    {
                        queryElement.Label = this.GetFormElementLabel(propListAType, prop);
                    }
                    else
                    {
                        queryElement.Label = this.GetFormElementLabel(propListBType, prop);
                    }
                    queryElement.Name = field.Name;
                    queryElement.Id = field.Name + "Property";
                    queryElement.DisplayInfo.Visible = true;
                    queryElement.DisplayInfo.CSSClass = RegAdminUtils.GetFormElementCSSClass(queryElement.DisplayInfo.Type);

                    if (field.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE)
                    {
                        queryElement.DisplayInfo.Type = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEStructureQuery";
                        SearchCriteria.StructureCriteria structureCriteria = new SearchCriteria.StructureCriteria();
                        structureCriteria.CartridgeSchema = "CsCartridge";
                        searchCriteria.Criterium = structureCriteria;
                    }
                    else
                    {
                        queryElement.DisplayInfo.Type = GetDefaultControlType(prop.Name, propListAType, FormGroup.DisplayMode.All);
                        if (queryElement.DisplayInfo.Type == string.Empty)
                            queryElement.DisplayInfo.Type = GetDefaultControlType(prop.Name, propListBType, FormGroup.DisplayMode.All);
                        switch (field.DataType)
                        {
                            case COEDataView.AbstractTypes.Text:
                                SearchCriteria.TextCriteria textCriteria = new SearchCriteria.TextCriteria();
                                textCriteria.Operator = SearchCriteria.COEOperators.LIKE;
                                searchCriteria.Criterium = textCriteria;
                                break;
                            case COEDataView.AbstractTypes.Real:
                            case COEDataView.AbstractTypes.Integer:
                                SearchCriteria.NumericalCriteria numericalCriteria = new SearchCriteria.NumericalCriteria();
                                numericalCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
                                searchCriteria.Criterium = numericalCriteria;
                                break;
                            case COEDataView.AbstractTypes.Date:
                                SearchCriteria.DateCriteria dateCriteria = new SearchCriteria.DateCriteria();
                                dateCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
                                searchCriteria.Criterium = dateCriteria;
                                break;
                            case COEDataView.AbstractTypes.Boolean:
                                textCriteria = new SearchCriteria.TextCriteria();
                                textCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
                                searchCriteria.Criterium = textCriteria;
                                break;
                        }
                    }

                    //JED: Create the SearchCriteriaItem here so the subsequent ConfigInfo can differentiate
                    //     data-input forms from query forms
                    queryElement.SearchCriteriaItem = searchCriteria;
                    queryElement.ConfigInfo = this.BuildConfigInfo(queryElement, prop, FormGroup.DisplayMode.All, propListAType);

                    queryFormElements.Add(queryElement);
                    _searchCriteria.Items.Add(searchCriteria);
                }
            }
        }

        /// <summary>
        /// Given the Form and DisplayMode, fetch those FormElement whose validation rules need to be updated.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="displayMode"></param>
        /// <param name="table"></param>
        /// <param name="queryFormElements">List of FormElement to be populated</param>
        /// <param name="propertyListA"></param>
        /// <param name="propListAType"></param>
        /// <param name="propertyListB"></param>
        /// <param name="propListBType"></param>
        private void BuildFormElementsListForSearch(FormGroup.Form form,FormGroup.DisplayMode displayMode, COEDataView.DataViewTable table, ref List<FormGroup.FormElement> queryFormElements, PropertyList propertyListA, PropertyListType propListAType, PropertyList propertyListB, PropertyListType propListBType)
        {
            foreach (COEDataView.Field field in table.Fields)
            {
                if ((propertyListA.CheckExistingNames(field.Name, false) && propertyListA[field.Name].ValRuleListIsUpdate)
                    || (propertyListB.CheckExistingNames(field.Name, false) && propertyListB[field.Name].ValRuleListIsUpdate))
                {
                    List<FormGroup.FormElement> existingElements = form.GetFormElements(displayMode);
                    foreach(FormGroup.FormElement element in existingElements)
                    {
                        if (element.Name == field.Name)
                        {
                            ValidationRuleList serverValRuleList=null;
                            if (propertyListA.CheckExistingNames(field.Name, false))
                                serverValRuleList=propertyListA[field.Name].ValRuleList;
                            else
                                serverValRuleList=propertyListB[field.Name].ValRuleList;
                            //element.ValidationRuleList = ConvertToClientValidations(serverValRuleList);
                            queryFormElements.Add(element);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Build new FormElement instances for a Display in which <paramref name="detailResultsCriteriaTable"/> resides.
        /// </summary>
        /// <remarks>
        /// Not only for detailsForms as its name suggests.
        /// </remarks>
        /// <param name="detailResultsCriteriaTable"></param>
        /// <param name="detailsFormElementsTemporaryBatch">A FormElement instances list to be populated</param>
        /// <param name="propertyListA">a PropertyList used to create new FormElement</param>
        /// <param name="propListAType"></param>
        /// <param name="propertyListB">another PropertyList used to create new FormElement</param>
        /// <param name="propListBType"></param>
        private void BuildDetailFormElements(ResultsCriteria.ResultsCriteriaTable detailResultsCriteriaTable, ref List<FormGroup.FormElement> detailsFormElementsTemporaryBatch, PropertyList propertyListA, PropertyListType propListAType, PropertyList propertyListB, PropertyListType propListBType, int RegFormID = 0)
        {
            detailsFormElementsTemporaryBatch = new List<FormGroup.FormElement>();
            if (detailResultsCriteriaTable != null && detailResultsCriteriaTable.Criterias != null)
            {
                foreach (ResultsCriteria.IResultsCriteriaBase criteria in detailResultsCriteriaTable.Criterias)
                {
                    if (criteria is ResultsCriteria.Field && !string.IsNullOrEmpty(((ResultsCriteria.Field)criteria).Alias))
                    {
                        ResultsCriteria.Field field = criteria as ResultsCriteria.Field;
                        Property prop = Property.NewProperty();
                        prop.Name = field.Alias;

                        if ((propertyListA.CheckExistingNames(prop.Name, false) && propertyListA[prop.Name].IsNew) || (propertyListB.CheckExistingNames(prop.Name, false) && propertyListB[prop.Name].IsNew))
                        {
                            FormGroup.FormElement formElement = new FormGroup.FormElement();

                            formElement.BindingExpression = "this['" + prop.Name + "']";
                            formElement.Name = prop.Name;

                            formElement.Label = this.GetFormElementLabel(propListAType, prop);

                            formElement.Id = prop.Name + "Property";
                            //Get the property type to use in the below code
                            prop.Type = GetPropertyType(prop.Name, propListAType);
                            // CBOE-1963 Remove the display info in the list view form of 4003.xml to make sure the control will accept any number of characters 
                            if (RegFormID == SEARCHPERMFORMID && (prop.Type != "BOOLEAN" && prop.Type != "PICKLISTDOMAIN"))
                            {
                                XmlDocument configInfo = new XmlDocument();
                                string xmlns = "COE.FormGroup";
                                string xmlprefix = "COE";
                                configInfo.AppendChild(configInfo.CreateNode(XmlNodeType.Element, xmlprefix, "configInfo", xmlns));
                                configInfo.FirstChild.AppendChild(configInfo.CreateNode(XmlNodeType.Element, xmlprefix, "fieldConfig", xmlns));
                                configInfo.FirstChild.FirstChild.AppendChild(configInfo.CreateNode(XmlNodeType.Element, xmlprefix, "CSSLabelClass", xmlns)).InnerText = "FETableItem";
                                formElement.ConfigInfo = configInfo;

                            }
                            else
                            {
                                formElement.DisplayInfo.Type = GetDefaultControlType(prop.Name, propListAType, FormGroup.DisplayMode.View);
                                if (formElement.DisplayInfo.Type == string.Empty)
                                    formElement.DisplayInfo.Type = GetDefaultControlType(prop.Name, propListBType, FormGroup.DisplayMode.View);

                                formElement.DisplayInfo.Visible = true;
                                if (formElement.DisplayInfo.Type.Contains("COEDatePicker"))
                                    formElement.DisplayInfo.CSSClass = "";
                                else
                                    formElement.DisplayInfo.CSSClass = RegAdminUtils.GetFormElementCSSClass(formElement.DisplayInfo.Type);

                                formElement.ConfigInfo = this.BuildConfigInfo(formElement, prop, FormGroup.DisplayMode.View, propListAType);
                            }
                            detailsFormElementsTemporaryBatch.Add(formElement);
                        }
                    }
                }
            }
        }

        private List<FormGroup.FormElement> PutFormElementsIntoGridView(bool view, string orderIndexBindingExpression, List<FormGroup.FormElement> formElementsToAdd, FormGroup.FormElement oldGridView)
        {
            List<FormGroup.FormElement> gridElement = new List<FormGroup.FormElement>();

            FormGroup.FormElement gridFormElement = new FormGroup.FormElement();
            gridFormElement.DisplayInfo.Type = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEGridView";
            gridFormElement.Id = "BatchComponentGrid";
            gridFormElement.BindingExpression = "BatchComponentList";
            gridFormElement.DisplayInfo.Position = "relative";
            XmlDocument xmlData = new XmlDocument();
            string xmlns = "COE.FormGroup";
            string xmlprefix = "COE";
            xmlData.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "configInfo", xmlns));
            xmlData.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "fieldConfig", xmlns));
            xmlData.FirstChild.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "tables", xmlns));
            xmlData.FirstChild.FirstChild.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "table", xmlns));
            xmlData.FirstChild.FirstChild.FirstChild.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "CSSClass", xmlns)).InnerText = "myTableClass";
            xmlData.FirstChild.FirstChild.FirstChild.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "headerStyle", xmlns)).InnerText = "color: #FFF; background-color: rgb(0, 153, 255); font-weight: bold; font-family: Verdana; font-size: 10px;";
            xmlData.FirstChild.FirstChild.FirstChild.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "columnStyle", xmlns)).InnerText = "width:10%;color: #000000; border-color: #CFD8E6; border-style:solid; border-width:1px; font-size:10px;font-family: Verdana;vertical-align:middle;text-align:center;";
            xmlData.FirstChild.FirstChild.FirstChild.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Style", xmlns)).InnerText = "margin-left:5px;margin-top:5px;margin-bottom:5px;";
            xmlData.FirstChild.FirstChild.FirstChild.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "ReadOnly", xmlns)).InnerText = "false";
            XmlNode columnsNode = xmlData.FirstChild.FirstChild.FirstChild.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Columns", xmlns));

            FormGroup.FormElement orderIndex = new FormGroup.FormElement();
            orderIndex.BindingExpression = orderIndexBindingExpression;
            orderIndex.Id = orderIndexBindingExpression;
            orderIndex.DisplayInfo.Type = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELabel";


            XmlNode orderIndexColumn = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Column", xmlns);
            orderIndexColumn.Attributes.Append(xmlData.CreateAttribute("name"));
            orderIndexColumn.Attributes["name"].Value = "Component"; // orderIndexBindingExpression Contains [Displaykey];
            XmlNode orderIndexFormElement = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "formElement", xmlns);
            orderIndexFormElement.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Label", xmlns)).InnerText = orderIndexColumn.Attributes["name"].Value; // Newly added.
            orderIndexFormElement.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Id", xmlns)).InnerText = orderIndex.Id;
            orderIndexFormElement.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "bindingExpression", xmlns)).InnerText = orderIndex.BindingExpression;
            XmlNode orderIndexDisplayInfo = orderIndexFormElement.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "displayInfo", xmlns));
            orderIndexDisplayInfo.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "type", xmlns)).InnerText = orderIndex.DisplayInfo.Type;

            orderIndexColumn.AppendChild(orderIndexFormElement);
            columnsNode.AppendChild(orderIndexColumn);

            XmlNamespaceManager nsManager = new XmlNamespaceManager(oldGridView.ConfigInfo.OwnerDocument.NameTable);
            nsManager.AddNamespace("COE", "COE.FormGroup");
            XmlNodeList oldColumns = oldGridView.ConfigInfo.FirstChild.FirstChild.FirstChild.SelectSingleNode("./COE:Columns", nsManager).SelectNodes("./COE:Column", nsManager);
           
            foreach (Property prop in _batchComponentProperties.GetSortedPropertyList())
            {
                foreach (XmlNode oldColumn in oldColumns)
                {
                    if (oldColumn.Attributes["name"].Value == prop.Name)
                    {
                        if (prop.ValRuleListIsUpdate)
                        oldColumn.SelectSingleNode("//COE:formElement/COE:validationRuleList", nsManager).InnerXml = prop.ValRuleList.UpdateSelfConfig(false);
                        columnsNode.AppendChild(xmlData.ImportNode(oldColumn, true));
                    }
                }
                if (!prop.ValRuleListIsUpdate || prop.IsNew) //If the update is only adding new columns but not validation change.
                {
                    foreach (FormGroup.FormElement element in formElementsToAdd)
                    {
                        if (element.Name == prop.Name)
                        {
                            element.BindingExpression = "PropertyList[@Name='" + element.Name + "' | Value]";
                            element.ValidationRuleList = ConvertToClientValidations(prop.ValRuleList);
                            XmlNode column = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Column", xmlns);
                            column.Attributes.Append(xmlData.CreateAttribute("name"));
                            column.Attributes["name"].Value = element.Name;                        
                            string label = element.Label;
                            element.Label = string.Empty;
                            element.Name = string.Empty;                          
                            column.InnerXml = element.ToString().Remove(0, element.ToString().IndexOf(">") + 1);
                            column.InsertBefore(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "headerText", xmlns), column.FirstChild).InnerText = label;
                            columnsNode.AppendChild(column);
                        }
                    }
                }
            }

            gridFormElement.ConfigInfo = xmlData.FirstChild;
            gridElement.Add(gridFormElement);

            return gridElement;
        }

        private void UpdateMixtureFormUI(int formID, ref COEFormBO formGroupBO)
        {
            List<FormGroup.FormElement> addAndEditFormElements = new List<FormGroup.FormElement>();
            List<FormGroup.FormElement> viewFormElements = new List<FormGroup.FormElement>();
            List<string> formElementsIdToRemove = new List<string>();
            BuildFormElementsList(_rootProperties, PropertyListType.MixturePropertyList, ref addAndEditFormElements, ref viewFormElements, ref formElementsIdToRemove, "PropertyList");
            formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, MIXTURESUBFORMINDEX, FormGroup.DisplayMode.Add, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, addAndEditFormElements);
            formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, MIXTURESUBFORMINDEX, FormGroup.DisplayMode.Edit, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, addAndEditFormElements);
            formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, MIXTURESUBFORMINDEX, FormGroup.DisplayMode.View, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, viewFormElements);
            formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, MIXTURESUBFORMINDEX, FormGroup.DisplayMode.Add, formElementsIdToRemove);
            formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, MIXTURESUBFORMINDEX, FormGroup.DisplayMode.Edit, formElementsIdToRemove);
            formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, MIXTURESUBFORMINDEX, FormGroup.DisplayMode.View, formElementsIdToRemove);
            this.UpdateFormElements(_rootProperties, FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, MIXTURESUBFORMINDEX, FormGroup.DisplayMode.Add, ref formGroupBO);
            this.UpdateFormElements(_rootProperties, FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, MIXTURESUBFORMINDEX, FormGroup.DisplayMode.Edit, ref formGroupBO);
            this.UpdateFormElements(_rootProperties, FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, MIXTURESUBFORMINDEX, FormGroup.DisplayMode.View, ref formGroupBO);

            bool sortOrderIsUpdate = false;

            foreach (Property prop in _rootProperties)
            {
                if (prop.SortOrderIsUpdate)
                    sortOrderIsUpdate = true;
            }

            if (sortOrderIsUpdate)
            {
                List<string> formElementsIds = new List<string>();

                foreach (Property prop in _rootProperties)
                {
                    formElementsIds.Add(prop.Name + "Property");
                }

                formGroupBO.UpdateFormElemetsSortOrder(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, MIXTURESUBFORMINDEX, formElementsIds);
            }
        }

        private void UpdateCompoundFormUI(int formID, ref COEFormBO formGroupBO)
        {
            List<FormGroup.FormElement> addAndEditFormElements = new List<FormGroup.FormElement>();
            List<FormGroup.FormElement> viewFormElements = new List<FormGroup.FormElement>();
            List<string> formElementsIdToRemove = new List<string>();
            BuildFormElementsList(_compoundProperties, PropertyListType.CompoundPropertyList, ref addAndEditFormElements, ref viewFormElements, ref formElementsIdToRemove, "Compound.PropertyList");
            formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, COMPOUNDSUBFORMINDEX, FormGroup.DisplayMode.Add, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, addAndEditFormElements);
            formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, COMPOUNDSUBFORMINDEX, FormGroup.DisplayMode.Edit, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, addAndEditFormElements);
            formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, COMPOUNDSUBFORMINDEX, FormGroup.DisplayMode.View, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, viewFormElements);
            formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, COMPOUNDSUBFORMINDEX, FormGroup.DisplayMode.Add, formElementsIdToRemove);
            formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, COMPOUNDSUBFORMINDEX, FormGroup.DisplayMode.Edit, formElementsIdToRemove);
            formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, COMPOUNDSUBFORMINDEX, FormGroup.DisplayMode.View, formElementsIdToRemove);

            this.UpdateFormElements(_compoundProperties, FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, COMPOUNDSUBFORMINDEX, FormGroup.DisplayMode.Add, ref formGroupBO);
            this.UpdateFormElements(_compoundProperties, FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, COMPOUNDSUBFORMINDEX, FormGroup.DisplayMode.Edit, ref formGroupBO);
            this.UpdateFormElements(_compoundProperties, FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, COMPOUNDSUBFORMINDEX, FormGroup.DisplayMode.View, ref formGroupBO);

            bool sortOrderIsUpdate = false;

            foreach (Property prop in _compoundProperties)
            {
                if (prop.SortOrderIsUpdate)
                    sortOrderIsUpdate = true;
            }

            if (sortOrderIsUpdate)
            {
                List<string> formElementsIds = new List<string>();

                foreach (Property prop in _compoundProperties)
                {
                    formElementsIds.Add(prop.Name + "Property");
                }

                formGroupBO.UpdateFormElemetsSortOrder(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, COMPOUNDSUBFORMINDEX, formElementsIds);
            }
        }

        private void UpdateStructureFormUI(int formID, ref COEFormBO formGroupBO)
        {
            List<FormGroup.FormElement> addAndEditFormElements = new List<FormGroup.FormElement>();
            List<FormGroup.FormElement> viewFormElements = new List<FormGroup.FormElement>();
            List<string> formElementsIdToRemove = new List<string>();
            BuildFormElementsList(_structureProperties, PropertyListType.StructurePropertyList, ref addAndEditFormElements, ref viewFormElements, ref formElementsIdToRemove, "Compound.BaseFragment.Structure.PropertyList");
            formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, STRUCTURESUBFORMINDEX, FormGroup.DisplayMode.Add, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, addAndEditFormElements);
            formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, STRUCTURESUBFORMINDEX, FormGroup.DisplayMode.Edit, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, addAndEditFormElements);
            formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, STRUCTURESUBFORMINDEX, FormGroup.DisplayMode.View, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, viewFormElements);
            if (formID == 4011) //LJB only coeform="2" should be modified for review register. there are unintended consequences modifying this in other forms.
            {
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, STRUCTUREVIEWMODESUBFORMINDEX, FormGroup.DisplayMode.View, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, viewFormElements); // Refer CL # 436218
            }
            formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, STRUCTURESUBFORMINDEX, FormGroup.DisplayMode.Add, formElementsIdToRemove);
            formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, STRUCTURESUBFORMINDEX, FormGroup.DisplayMode.Edit, formElementsIdToRemove);
            formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, STRUCTURESUBFORMINDEX, FormGroup.DisplayMode.View, formElementsIdToRemove);
            if (formID == 4011) //LJB: only coeform="2" should be modified for review register. there are unintended consequences modifying this in other forms.
            {
                formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, STRUCTUREVIEWMODESUBFORMINDEX, FormGroup.DisplayMode.View, formElementsIdToRemove);// Refer CL # 436218
            }
            this.UpdateFormElements(_structureProperties, FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, STRUCTURESUBFORMINDEX, FormGroup.DisplayMode.Add, ref formGroupBO);
            this.UpdateFormElements(_structureProperties, FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, STRUCTURESUBFORMINDEX, FormGroup.DisplayMode.Edit, ref formGroupBO);
            this.UpdateFormElements(_structureProperties, FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, STRUCTURESUBFORMINDEX, FormGroup.DisplayMode.View, ref formGroupBO);
            if (formID == 4011) //LJB only coeform="2" should be modified for review register. there are unintended consequences modifying this in other forms.
            {
                this.UpdateFormElements(_structureProperties, FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, STRUCTUREVIEWMODESUBFORMINDEX, FormGroup.DisplayMode.View, ref formGroupBO);// Refer CL # 436218
            }
            bool sortOrderIsUpdate = false;

            foreach (Property prop in _structureProperties)
            {
                if (prop.SortOrderIsUpdate)
                    sortOrderIsUpdate = true;
            }

            if (sortOrderIsUpdate)
            {
                List<string> formElementsIds = new List<string>();

                foreach (Property prop in _structureProperties)
                {
                    formElementsIds.Add(prop.Name + "Property");
                }

                formGroupBO.UpdateFormElemetsSortOrder(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, STRUCTURESUBFORMINDEX, formElementsIds);
                if (formID == 4011) //LJB only coeform="2" should be modified for review register. there are unintended consequences modifying this in other forms.
                {
                    formGroupBO.UpdateFormElemetsSortOrder(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, STRUCTUREVIEWMODESUBFORMINDEX, formElementsIds);// Refer CL # 436218
                }
            }
        }

        private void UpdateBatchFormUI(int formID, ref COEFormBO formGroupBO)
        {
            List<FormGroup.FormElement> addAndEditFormElements = new List<FormGroup.FormElement>();
            List<FormGroup.FormElement> viewFormElements = new List<FormGroup.FormElement>();
            List<string> formElementsIdToRemove = new List<string>();
            BuildFormElementsList(_batchProperties, PropertyListType.BatchPropertyList, ref addAndEditFormElements, ref viewFormElements, ref formElementsIdToRemove, "PropertyList");
            formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHSUBFORMINDEX, FormGroup.DisplayMode.Add, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, addAndEditFormElements);
            formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHSUBFORMINDEX, FormGroup.DisplayMode.Edit, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, addAndEditFormElements);
            formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHSUBFORMINDEX, FormGroup.DisplayMode.View, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, viewFormElements);
            formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHSUBFORMINDEX, FormGroup.DisplayMode.Add, formElementsIdToRemove);
            formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHSUBFORMINDEX, FormGroup.DisplayMode.Edit, formElementsIdToRemove);
            formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHSUBFORMINDEX, FormGroup.DisplayMode.View, formElementsIdToRemove);

            this.UpdateFormElements(_batchProperties, FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHSUBFORMINDEX, FormGroup.DisplayMode.Add, ref formGroupBO);
            this.UpdateFormElements(_batchProperties, FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHSUBFORMINDEX, FormGroup.DisplayMode.View, ref formGroupBO);
            this.UpdateFormElements(_batchProperties, FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHSUBFORMINDEX, FormGroup.DisplayMode.Edit, ref formGroupBO);

            bool sortOrderIsUpdate = false;

            foreach (Property prop in _batchProperties)
            {
                if (prop.SortOrderIsUpdate)
                    sortOrderIsUpdate = true;
            }

            if (sortOrderIsUpdate)
            {
                List<string> formElementsIds = new List<string>();

                foreach (Property prop in _batchProperties)
                {
                    formElementsIds.Add(prop.Name + "Property");
                }

                formGroupBO.UpdateFormElemetsSortOrder(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHSUBFORMINDEX, formElementsIds);
            }
        }

        private void UpdateBatchComponentFormUI(int formID, ref COEFormBO formGroupBO)
        {
            List<FormGroup.FormElement> addAndEditFormElements = new List<FormGroup.FormElement>();
            List<FormGroup.FormElement> viewFormElements = new List<FormGroup.FormElement>();
            List<string> formElementsIdToRemove = new List<string>();
            BuildFormElementsList(_batchComponentProperties, PropertyListType.BatchComponentPropertyList, ref addAndEditFormElements, ref viewFormElements, ref formElementsIdToRemove, "PropertyList");
            string orderIndexBindingExpression = string.Empty;
            if (formGroupBO.ID == SUBMITMIXTUREFORMID || formGroupBO.ID == REGISTERMIXTUREFORMID || formGroupBO.ID == VIEWMIXTUREFORMID || formGroupBO.ID == COMPONENTDUPLICATESFORMID)
            {
                orderIndexBindingExpression = "DisplayKey";
            }
            else
            {
                orderIndexBindingExpression = "OrderIndex";
            }

            // For BatchComponent, there's no property addition actually. Any new property will be regarded as an update to the
            // existing sole form element, the grid.
            FormGroup.Form batchComponentForm = formGroupBO.GetForm(formGroupBO.COEFormGroup.DetailsForms, 0, BATCHCOMPONENTSUBFORMINDEX);

            if (batchComponentForm != null)
            {
                if (batchComponentForm.EditMode.Count > 0)
                {
                    FormGroup.FormElement addAndEditOldGridView = batchComponentForm.EditMode[0];
                    List<FormGroup.FormElement> addAndEditCoeGridView = PutFormElementsIntoGridView(false, orderIndexBindingExpression, addAndEditFormElements, addAndEditOldGridView);

                    List<string> idtoRemove = new List<string>();
                    idtoRemove.Add(addAndEditCoeGridView[0].Id);

                    formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHCOMPONENTSUBFORMINDEX, FormGroup.DisplayMode.Add, idtoRemove);
                    formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHCOMPONENTSUBFORMINDEX, FormGroup.DisplayMode.Edit, idtoRemove);
              
                    formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHCOMPONENTSUBFORMINDEX, FormGroup.DisplayMode.Add, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, addAndEditCoeGridView);
                    formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHCOMPONENTSUBFORMINDEX, FormGroup.DisplayMode.Edit, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, addAndEditCoeGridView);
                  }

                if (batchComponentForm.ViewMode.Count > 0)
                {
                    FormGroup.FormElement viewOldGridView = batchComponentForm.ViewMode[0];
                    List<FormGroup.FormElement> viewCoeGridView = PutFormElementsIntoGridView(true, orderIndexBindingExpression, viewFormElements, viewOldGridView);
                    List<string> idtoRemoveView = new List<string>();
                    idtoRemoveView.Add(viewCoeGridView[0].Id);
                    formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHCOMPONENTSUBFORMINDEX, FormGroup.DisplayMode.View, idtoRemoveView);
                    formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHCOMPONENTSUBFORMINDEX, FormGroup.DisplayMode.View, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, viewCoeGridView);
                }

                this.UpdateFormElements(_batchComponentProperties, FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHCOMPONENTSUBFORMINDEX, FormGroup.DisplayMode.Add, ref formGroupBO);
                this.UpdateFormElements(_batchComponentProperties, FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHCOMPONENTSUBFORMINDEX, FormGroup.DisplayMode.View, ref formGroupBO);
                this.UpdateFormElements(_batchComponentProperties, FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHCOMPONENTSUBFORMINDEX, FormGroup.DisplayMode.Edit, ref formGroupBO);
            }
        }

        /// <summary>
        /// Update the FormGroup according to updated property list 
        /// </summary>
        /// <param name="formID"></param>
        private void UpdateRegistrationFormGroup(int formID)
        {
            COEFormBO formGroupBO = COEFormBO.Get(formID);
            UpdateMixtureFormUI(formID, ref formGroupBO);
            UpdateCompoundFormUI(formID, ref formGroupBO);
            UpdateStructureFormUI(formID, ref formGroupBO);
            UpdateBatchFormUI(formID, ref formGroupBO);
            UpdateBatchComponentFormUI(formID, ref formGroupBO);
            _formGroupBO = formGroupBO.Save();
        }

        /// <summary>
        /// update search temp forms with id equals to <paramref name="formId"/>,also update COEDataView with the same id
        /// </summary>
        /// <param name="formId">id of COEForm to be udpated</param>
        private void UpdateSearchTemporaryFormGroup(int formId)
        {
            COEFormBO formGroupBO = COEFormBO.Get(formId);
            _searchCriteria = this.GetSearchCriteria(formGroupBO.COEFormGroup.QueryForms[0]);

            //FormElement to be added or updated for queryForms
            //FormElement in queryForms are categorized in two groups
            //Those for Compound and BatchComponent in one group, those for Mixture and Batch in another
            List<FormGroup.FormElement> queryFormElementsTemporaryBatch = new List<FormGroup.FormElement>();
            List<FormGroup.FormElement> queryFormElementsTemporaryBatch_ValRuleListUpdated = new List<FormGroup.FormElement>();
            List<FormGroup.FormElement> queryFormElementsTemporaryCompound = new List<FormGroup.FormElement>();//to add FormElement for Temporary compound and batch component
            List<FormGroup.FormElement> queryFormElementsTemporaryCompound_ValRuleListUpdated = new List<FormGroup.FormElement>();////to update FormElement for Temporary compound and batch component

            //Id of FormElement to be removed from queryForms
            //First two lists are for Mixture and Batch.(that is , to be removed from //queryForms/queryForm[@id=0]/coeForms/coeForm[@id=0]/layoutInfo)
            //The other three lists are for Compound,BatchComponent and Structure.(to be removed from //queryForms/queryForm[@id=0]/coeForms/coeForm[@id=1]/layoutInfo)
            List<string>[] formElementsIdToRemove = new List<string>[5];
            List<string> formElementsIdToRemoveFromBase = new List<string>();
            List<string> formElementsIdToRemoveFromChild = new List<string>();

            //FormElement to be added into detailsForms
            //FormElement in detailsForms are categorized in two groups
            //Those for Compound and BatchComponent in one group, those for Mixture and Batch in another
            List<FormGroup.FormElement> detailsFormElementsTemporaryBatch = new List<FormGroup.FormElement>();
            List<FormGroup.FormElement> detailsFormElementsTemporaryCompound = new List<FormGroup.FormElement>();
            
            //FormElement to be added for listForms
            List<FormGroup.FormElement>[] listFormElements = new List<FormGroup.FormElement>[5];

            ResultsCriteria listFormResultCriteria = new ResultsCriteria();//resultsCriteria of first listForm
            ResultsCriteria resultCriteria = new ResultsCriteria();//resultsCriteria of first detailsForm

            if (formGroupBO.COEFormGroup.ListForms.Displays.Count > 0)
                listFormResultCriteria = ((FormGroup.ListDisplay)formGroupBO.COEFormGroup.ListForms[0]).ResultsCriteria;
            if (formGroupBO.COEFormGroup.DetailsForms.Displays.Count > 0)
                resultCriteria = ((FormGroup.DetailsDisplay)formGroupBO.COEFormGroup.DetailsForms[0]).ResultsCriteria;

            COEDataView dataview = CreateTemporaryDataViewAndResultCriteria(ref formElementsIdToRemove, ref resultCriteria, ref listFormResultCriteria, formId);

            for (int i = 0; i < 5; i++)
            {
                if (formElementsIdToRemove[i] != null)
                {
                    foreach (string id in formElementsIdToRemove[i])
                    {
                        if (id != null && i < 2)
                            formElementsIdToRemoveFromBase.Add(id);
                        else if (id != null)
                            formElementsIdToRemoveFromChild.Add(id);
                    }
                }
            }

            #region vw_temporarybatch

            BuildFormElementsListForSearch(dataview.Tables[TEMPBATCHTABLENAME], ref queryFormElementsTemporaryBatch, _rootProperties, PropertyListType.MixturePropertyList, _batchProperties, PropertyListType.BatchPropertyList);
            BuildFormElementsListForSearch(formGroupBO.GetForm(formGroupBO.COEFormGroup.QueryForms, QUERYFORMINDEX, TEMPORARYBASEFORM),
                    FormGroup.DisplayMode.All,
                    dataview.Tables[TEMPBATCHTABLENAME],
                    ref queryFormElementsTemporaryBatch_ValRuleListUpdated,
                    _rootProperties, PropertyListType.MixturePropertyList, _batchProperties, PropertyListType.BatchPropertyList);
            BuildDetailFormElements(resultCriteria[dataview.Tables[TEMPBATCHTABLENAME].Id], ref detailsFormElementsTemporaryBatch, _rootProperties, PropertyListType.MixturePropertyList, _batchProperties, PropertyListType.BatchPropertyList);
            BuildDetailFormElements(listFormResultCriteria[dataview.Tables[TEMPBATCHTABLENAME].Id], ref listFormElements[0], _rootProperties, PropertyListType.MixturePropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
            BuildDetailFormElements(listFormResultCriteria[dataview.Tables[TEMPBATCHTABLENAME].Id], ref listFormElements[1], _batchProperties, PropertyListType.BatchPropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
            //update queryForms
            if (formGroupBO.COEFormGroup.QueryForms.Displays.Count > 0)
            {
                //remove
                formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, TEMPORARYBASEFORM, FormGroup.DisplayMode.All, formElementsIdToRemoveFromBase);
                //add
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, TEMPORARYBASEFORM, FormGroup.DisplayMode.All, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, queryFormElementsTemporaryBatch);
                //update client validation rules
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, TEMPORARYBASEFORM, FormGroup.DisplayMode.All, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, queryFormElementsTemporaryBatch_ValRuleListUpdated);
            }
            //update detailsForms
            if (formGroupBO.COEFormGroup.DetailsForms.Displays.Count > 0)
            {
                //remove
                formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, TEMPORARYBASEFORM, FormGroup.DisplayMode.View, formElementsIdToRemoveFromBase);
                //add
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, TEMPORARYBASEFORM, FormGroup.DisplayMode.View, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, detailsFormElementsTemporaryBatch);
            }
            bool sortOrderIsUpdate = false;

            foreach (Property prop in _rootProperties)
            {
                if (prop.SortOrderIsUpdate)
                    sortOrderIsUpdate = true;
            }

            foreach (Property prop in _batchProperties)
            {
                if (prop.SortOrderIsUpdate)
                    sortOrderIsUpdate = true;
            }

            if (sortOrderIsUpdate)
            {
                List<string> temporaryBatchFormElementsIds = new List<string>();

                foreach (Property prop in _rootProperties)
                {
                    temporaryBatchFormElementsIds.Add(prop.Name + "Property");
                }

                foreach (Property prop in _batchProperties)
                {
                    temporaryBatchFormElementsIds.Add(prop.Name + "Property");
                }

                if (formGroupBO.COEFormGroup.QueryForms.Displays.Count > 0)
                    formGroupBO.UpdateFormElemetsSortOrder(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, TEMPORARYBASEFORM, temporaryBatchFormElementsIds);

                if (formGroupBO.COEFormGroup.DetailsForms.Displays.Count > 0)
                    formGroupBO.UpdateFormElemetsSortOrder(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, TEMPORARYBASEFORM, temporaryBatchFormElementsIds);

                sortOrderIsUpdate = false;
            }

            #endregion

            #region vw_temporarycompound

            BuildFormElementsListForSearch(dataview.Tables[TEMPCOMPOUNDTABLENAME], ref queryFormElementsTemporaryCompound, _compoundProperties, PropertyListType.CompoundPropertyList, _batchComponentProperties, PropertyListType.BatchComponentPropertyList);
            BuildFormElementsListForSearch(dataview.Tables[TEMPCOMPOUNDTABLENAME], ref queryFormElementsTemporaryCompound, _structureProperties, PropertyListType.StructurePropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
            BuildFormElementsListForSearch(formGroupBO.GetForm(formGroupBO.COEFormGroup.QueryForms, QUERYFORMINDEX, TEMPORARYCHILDFORM),
                   FormGroup.DisplayMode.All,
                   dataview.Tables[TEMPCOMPOUNDTABLENAME],
                   ref queryFormElementsTemporaryCompound_ValRuleListUpdated,
                   _compoundProperties, PropertyListType.CompoundPropertyList, _batchComponentProperties, PropertyListType.BatchComponentPropertyList);
            BuildFormElementsListForSearch(formGroupBO.GetForm(formGroupBO.COEFormGroup.QueryForms, QUERYFORMINDEX, TEMPORARYCHILDFORM),
                   FormGroup.DisplayMode.All,
                   dataview.Tables[TEMPCOMPOUNDTABLENAME],
                   ref queryFormElementsTemporaryCompound_ValRuleListUpdated,
                   _structureProperties, PropertyListType.StructurePropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
            BuildDetailFormElements(resultCriteria[dataview.Tables[TEMPCOMPOUNDTABLENAME].Id], ref detailsFormElementsTemporaryCompound, _compoundProperties, PropertyListType.CompoundPropertyList, _batchComponentProperties, PropertyListType.BatchComponentPropertyList);
            BuildDetailFormElements(resultCriteria[dataview.Tables[TEMPCOMPOUNDTABLENAME].Id], ref detailsFormElementsTemporaryCompound, _structureProperties, PropertyListType.StructurePropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
            BuildDetailFormElements(listFormResultCriteria[dataview.Tables[TEMPCOMPOUNDTABLENAME].Id], ref listFormElements[2], _compoundProperties, PropertyListType.CompoundPropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
            BuildDetailFormElements(listFormResultCriteria[dataview.Tables[TEMPCOMPOUNDTABLENAME].Id], ref listFormElements[3], _batchComponentProperties, PropertyListType.BatchComponentPropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
            BuildDetailFormElements(listFormResultCriteria[dataview.Tables[TEMPCOMPOUNDTABLENAME].Id], ref listFormElements[4], _structureProperties, PropertyListType.StructurePropertyList, PropertyList.NewPropertyList(), PropertyListType.None);


            //update queryForms
            if (formGroupBO.COEFormGroup.QueryForms.Displays.Count > 0)
            {
                //remove
                formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, TEMPORARYCHILDFORM, FormGroup.DisplayMode.All, formElementsIdToRemoveFromChild);
                //add
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, TEMPORARYCHILDFORM, FormGroup.DisplayMode.All, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, queryFormElementsTemporaryCompound);
                //update
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, TEMPORARYCHILDFORM, FormGroup.DisplayMode.All, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, queryFormElementsTemporaryCompound_ValRuleListUpdated);
            }
            //update detailsForms
            if (formGroupBO.COEFormGroup.DetailsForms.Displays.Count > 0)
            {
                List<string> gridToRemove = new List<string>();
                List<FormGroup.FormElement> gridView = null;
                gridView = GetGridViewGridViewDetailsForms(formGroupBO.COEFormGroup.DetailsForms[0].Forms[1].ViewMode[0], detailsFormElementsTemporaryCompound, formElementsIdToRemoveFromChild, "Table_" + dataview.Tables[1].Id, "FilteredDataSet");
                //remove original FormElement
                gridToRemove.Add(formGroupBO.COEFormGroup.DetailsForms[0].Forms[1].ViewMode[0].Id);
                formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, TEMPORARYCHILDFORM, FormGroup.DisplayMode.View, gridToRemove);
                //add new one
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, TEMPORARYCHILDFORM, FormGroup.DisplayMode.View, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, gridView);
            }

            foreach (Property prop in _compoundProperties)
            {
                if (prop.SortOrderIsUpdate)
                    sortOrderIsUpdate = true;
            }

            foreach (Property prop in _structureProperties)
            {
                if (prop.SortOrderIsUpdate)
                    sortOrderIsUpdate = true;
            }

            foreach (Property prop in _batchComponentProperties)
            {
                if (prop.SortOrderIsUpdate)
                    sortOrderIsUpdate = true;
            }

            if (sortOrderIsUpdate)
            {
                List<string> temporaryCompoundFormElementsIds = new List<string>();

                foreach (Property prop in _compoundProperties.GetSortedPropertyList())
                {
                    temporaryCompoundFormElementsIds.Add(prop.Name + "Property");
                }

                foreach (Property prop in _structureProperties.GetSortedPropertyList())
                {
                    temporaryCompoundFormElementsIds.Add(prop.Name + "Property");
                }

                foreach (Property prop in _batchComponentProperties.GetSortedPropertyList())
                {
                    temporaryCompoundFormElementsIds.Add(prop.Name + "Property");
                }

                if (formGroupBO.COEFormGroup.QueryForms.Displays.Count > 0)
                    formGroupBO.UpdateFormElemetsSortOrder(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, TEMPORARYCHILDFORM, temporaryCompoundFormElementsIds);
            }

            #endregion

            List<FormGroup.FormElement> webGrid = null;
            List<string> webGridToRemove = new List<string>();

            //update listForms
            if (formGroupBO.COEFormGroup.ListForms.Displays.Count > 0)
            {
                webGrid = GetWebGridListFormsForTemp(formGroupBO.COEFormGroup.ListForms[0].Forms[0].LayoutInfo[0], listFormElements, formElementsIdToRemove, "Dataset");
                webGridToRemove.Add(formGroupBO.COEFormGroup.ListForms[0].Forms[0].LayoutInfo[0].Id);
                formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.ListForm, LISTFORMINDEX, 0, FormGroup.DisplayMode.All, webGridToRemove);
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.ListForm, LISTFORMINDEX, 0, FormGroup.DisplayMode.All, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, webGrid);
            }

            //Fix for CSBR: 147743 - Clear validation rules from temp search page
            formGroupBO.ClearValidationRulesFromSearch();
            _formGroupBO = formGroupBO.Save();
        }

        /// <summary>
        /// Create a new FormElement from <paramref name="originalGridView"/>,with new column from <paramref name="formElementsToPutInGrid"/>
        /// and with column corresponding to <paramref name="formElementsToPutInGrid"/> eliminated
        /// </summary>
        /// <param name="originalGridView">Original FormElement from which new instance is constructure</param>
        /// <param name="formElementsToPutInGrid">used to create new column</param>
        /// <param name="formElementsIdToRemoveFromGrid">used to delete columns</param>
        /// <param name="tableName"></param>
        /// <param name="bindingExpression"></param>
        /// <returns></returns>
        private List<FormGroup.FormElement> GetGridViewGridViewDetailsForms(FormGroup.FormElement originalGridView, List<FormGroup.FormElement> formElementsToPutInGrid, List<string> formElementsIdToRemoveFromGrid, string tableName, string bindingExpression)
        {
            //Remove FormElementsIdToRemove
            //Remove FormElementsToPut
            //Add FormElementsToPut
            XmlNamespaceManager nsManager = new XmlNamespaceManager(originalGridView.ConfigInfo.OwnerDocument.NameTable);
            nsManager.AddNamespace("COE", "COE.FormGroup");
            XmlNode tableNode = originalGridView.ConfigInfo.FirstChild.SelectSingleNode("//COE:table", nsManager);
            List<string> avoidAddingThisElements = new List<string>();

            List<FormGroup.FormElement> gridElement = new List<FormGroup.FormElement>();
            FormGroup.FormElement gridFormElement = new FormGroup.FormElement();
            gridFormElement.DisplayInfo.Type = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEGridView";
            gridFormElement.Id = tableName + "Grid";
            gridFormElement.BindingExpression = bindingExpression;
            gridFormElement.DisplayInfo.Position = "relative";
            XmlDocument xmlData = new XmlDocument();
            string xmlns = "COE.FormGroup";
            string xmlprefix = "COE";
            XmlAttribute tableNameAttr = xmlData.CreateAttribute("name");
            tableNameAttr.Value = tableName;
            xmlData.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "configInfo", xmlns));
            xmlData.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "fieldConfig", xmlns));
            xmlData.FirstChild.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "tables", xmlns));
            xmlData.FirstChild.FirstChild.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "table", xmlns)).Attributes.Append(tableNameAttr);
            xmlData.FirstChild.FirstChild.FirstChild.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "CSSClass", xmlns)).InnerText = "myTableClass";
            xmlData.FirstChild.FirstChild.FirstChild.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "headerStyle", xmlns)).InnerText = "color: #FFF; background-color: rgb(0, 153, 255); font-weight: bold; font-family: Verdana; font-size: 10px;";
            xmlData.FirstChild.FirstChild.FirstChild.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Style", xmlns)).InnerText = "margin-left:5px;margin-top:5px;margin-bottom:5px;";
            xmlData.FirstChild.FirstChild.FirstChild.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "ReadOnly", xmlns)).InnerText = "false";
            XmlNode columnsNode = xmlData.FirstChild.FirstChild.FirstChild.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Columns", xmlns));

            foreach (XmlNode columnNode in tableNode.SelectNodes("./COE:Columns/COE:Column", nsManager))
            {
                if (columnNode.Attributes["name"] == null || string.IsNullOrEmpty(columnNode.Attributes["name"].Value))
                {
                    columnsNode.AppendChild(xmlData.ImportNode(columnNode, true));
                }
                else if (!formElementsIdToRemoveFromGrid.Contains(columnNode.Attributes["name"].Value + "Property"))
                    columnsNode.AppendChild(xmlData.ImportNode(columnNode, true));
                else
                {
                    foreach (FormGroup.FormElement element in formElementsToPutInGrid)
                    {
                        if (columnNode.Attributes["name"].Value == element.Id.Replace("Property", string.Empty))
                            avoidAddingThisElements.Add(element.Id);
                    }
                }
            }

            foreach (FormGroup.FormElement element in formElementsToPutInGrid)
            {
                if (!avoidAddingThisElements.Contains(element.Id))
                {
                    XmlNode column = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Column", xmlns);
                    column.Attributes.Append(xmlData.CreateAttribute("name"));
                    column.Attributes["name"].Value = element.Id.Replace("Property", string.Empty);
                    element.Name = column.Attributes["name"].Value;
                    element.BindingExpression = string.Empty;
                    //element.Label = string.Empty;
                    column.InnerXml = element.ToString().Remove(0, element.ToString().IndexOf(">") + 1);
                    XmlNode headerNode = xmlData.CreateNode(XmlNodeType.Element, "headerText", xmlns);
                    //headerNode.InnerText = column.Attributes["name"].Value.Remove(0, 3);
                    string label = element.Label;
                    element.Label = string.Empty;
                    column.InnerXml = element.ToString().Remove(0, element.ToString().IndexOf(">") + 1);
                    headerNode.InnerText = label;
                    column.InsertBefore(headerNode, column.FirstChild);
                    columnsNode.AppendChild(column);
                }
            }

            gridFormElement.ConfigInfo = xmlData.FirstChild;
            gridElement.Add(gridFormElement);

            return gridElement;
        }

        private List<FormGroup.FormElement> GetWebGridListForms(FormGroup.FormElement originalWebGrid, List<FormGroup.FormElement>[] formElementsToPutInWebGrid, List<string>[] formElementsIdToRemoveFromWebGrid, string bindingExpression)
        {
            //Remove FormElementsIdToRemove
            //Remove FormElementsToPut
            //Add FormElementsToPut
            XmlNamespaceManager nsManager = new XmlNamespaceManager(originalWebGrid.ConfigInfo.OwnerDocument.NameTable);
            nsManager.AddNamespace("COE", "COE.FormGroup");
            List<string> avoidAddingThisElements = new List<string>();
            List<FormGroup.FormElement> webGridElement = new List<FormGroup.FormElement>();
            FormGroup.FormElement webGridFormElement = new FormGroup.FormElement();
            webGridFormElement.DisplayInfo.Type = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGrid";
            webGridFormElement.Id = "ListView";
            webGridFormElement.BindingExpression = bindingExpression;
            //webGridFormElement.DisplayInfo.Position = "relative";
            XmlDocument xmlData = new XmlDocument();
            string xmlns = "COE.FormGroup";
            string xmlprefix = "COE";
            XmlAttribute tableNameAttr = xmlData.CreateAttribute("name");
            XmlNodeList tablesNode = originalWebGrid.ConfigInfo.SelectNodes("//COE:fieldConfig/COE:tables/COE:table", nsManager);
            int tableIndex = 0;
            xmlData.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "configInfo", xmlns));
            XmlNode gridConfNode = xmlData.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "GridConfigSettings", xmlns));
            gridConfNode.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "AllowSorting", xmlns)).InnerText = "true";
            gridConfNode.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "AllowChildSorting", xmlns)).InnerText = "true";
            gridConfNode.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "IsReadOnly", xmlns)).InnerText = "true";
            gridConfNode.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "FixedHeader", xmlns)).InnerText = "true";
            gridConfNode.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "ShowExpandCollapseImage", xmlns)).InnerText = "false";
            gridConfNode.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "ShowChildDataOnly", xmlns)).InnerText = "false";
            XmlNode fieldConfNode = xmlData.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "fieldConfig", xmlns));
            fieldConfNode.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "tables", xmlns));

            List<PropertyList> sortedPropertyListList = new List<PropertyList>();

            sortedPropertyListList.Add(_rootProperties.GetSortedPropertyList());
            sortedPropertyListList.Add(_compoundProperties.GetSortedPropertyList());
            sortedPropertyListList.Add(_batchProperties.GetSortedPropertyList());
            sortedPropertyListList.Add(_batchComponentProperties.GetSortedPropertyList());
            sortedPropertyListList.Add(_structureProperties.GetSortedPropertyList());

            foreach (XmlNode table in tablesNode)//TODO: add a new COETable for listForm of search perm COEForm
            {
                XmlNode newTableNode = fieldConfNode.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "table", xmlns));
                newTableNode.Attributes.Append(newTableNode.OwnerDocument.CreateAttribute("name")).Value = table.Attributes["name"].Value;
                //xmlData.FirstChild.FirstChild.FirstChild.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "CSSClass", xmlns)).InnerText = "myTableClass";
                newTableNode.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "headerStyle", xmlns)).InnerText = "color: #000099; background-color: #CFD8E6; font-weight: bold; font-family: Verdana; font-size: 10px;border-collapse:collapse;";
                newTableNode.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "columnStyle", xmlns)).InnerText = "color: #000000; background-color: #FFFFFF; border-color: #CFD8E6; border-style:solid; border-width:1px; font-size:10px;font-family: Verdana;vertical-align:middle;text-align:center;";
                //xmlData.FirstChild.FirstChild.FirstChild.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "", xmlns)).InnerText = "false";
                XmlNode columnsNode = newTableNode.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Columns", xmlns));

                foreach (XmlNode columnNode in table.SelectNodes("./COE:Columns/COE:Column", nsManager))
                {
                    if (columnNode.Attributes["name"] == null || string.IsNullOrEmpty(columnNode.Attributes["name"].Value))
                    {
                        columnsNode.AppendChild(xmlData.ImportNode(columnNode, true));
                    }
                    else if (!formElementsIdToRemoveFromWebGrid[tableIndex].Contains(columnNode.Attributes["name"].Value + "Property"))
                    {
                        columnsNode.AppendChild(xmlData.ImportNode(columnNode, true));
                    }
                    else
                    {
                        if (formElementsToPutInWebGrid[tableIndex] != null)
                        {
                            foreach (FormGroup.FormElement element in formElementsToPutInWebGrid[tableIndex])
                            {
                                if (element.Id != null)
                                {
                                    if (columnNode.Attributes["name"].Value == element.Id.Replace("Property", string.Empty))
                                        avoidAddingThisElements.Add(element.Id);
                                }
                            }
                        }
                    }
                }

                if (formElementsToPutInWebGrid[tableIndex] != null)
                {
                    foreach (FormGroup.FormElement element in formElementsToPutInWebGrid[tableIndex])
                    {
                        if (element.Id != null)
                        {

                            foreach (Property prop in sortedPropertyListList[tableIndex])
                            {
                                if (element.Name == prop.Name)
                                {
                                    if (!avoidAddingThisElements.Contains(element.Id))
                                    {
                                        XmlNode column = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Column", xmlns);
                                        column.Attributes.Append(xmlData.CreateAttribute("name"));
                                        column.Attributes["name"].Value = element.Id.Replace("Property", string.Empty);
                                        //element.Label = column.Attributes["name"].Value.Remove(0,3);
                                        column.Attributes.Append(xmlData.CreateAttribute("hidden"));
                                        column.Attributes["hidden"].Value = IsColumnToHide(prop);
                                        if (tableIndex == 0)
                                            element.BindingExpression = "this['" + element.Name + "']";
                                        else
                                            element.BindingExpression = element.Name;
                                        element.Label = string.Empty;
                                        column.InnerXml = element.ToString().Remove(0, element.ToString().IndexOf(">") + 1);
                                        XmlNode headerNode = xmlData.CreateNode(XmlNodeType.Element, "headerText", xmlns);
                                        switch (tableIndex)
                                        {
                                            case 0:
                                                headerNode.InnerText = _formElementsLabels[0][prop.Name];
                                                break;
                                            case 1:
                                                headerNode.InnerText = _formElementsLabels[1][prop.Name];
                                                break;
                                            case 2:
                                                headerNode.InnerText = _formElementsLabels[2][prop.Name];
                                                break;
                                            case 3:
                                                headerNode.InnerText = _formElementsLabels[3][prop.Name];
                                                break;
                                            case 4:
                                                headerNode.InnerText = _formElementsLabels[4][prop.Name];
                                                break;
                                        }
                                        XmlNode widthNode = xmlData.CreateNode(XmlNodeType.Element, "width", xmlns);
                                        widthNode.InnerText = "200px"; // CBOE-1963 Header text width is increase to handle 30 characters
                                        column.InsertBefore(widthNode, column.FirstChild);
                                        column.InsertBefore(headerNode, column.FirstChild);

                                        if (tableIndex == 0)
                                            columnsNode.InsertBefore(column, columnsNode.ChildNodes[columnsNode.ChildNodes.Count - 3]);
                                        else
                                            columnsNode.AppendChild(column);
                                    }
                                }
                            }
                        }
                    }
                }


                tableIndex++;
            }

            webGridFormElement.ConfigInfo = xmlData.FirstChild;
            webGridElement.Add(webGridFormElement);

            return webGridElement;
        }

        /// <summary>
        /// Create a new FormElement from <paramref name="originalWebGrid"/>,with new column corresponding to <paramref name="formElementsToPutInWebGrid"/> 
        /// and with column correponding to <paramref name="formElementsIdToRemoveFromWebGrid"/> eliminated.
        /// </summary>
        /// <param name="originalWebGrid">Original FormElelement form which new instance are created</param>
        /// <param name="formElementsToPutInWebGrid"></param>
        /// <param name="formElementsIdToRemoveFromWebGrid"></param>
        /// <param name="bindingExpression"></param>
        /// <returns></returns>
        private List<FormGroup.FormElement> GetWebGridListFormsForTemp(FormGroup.FormElement originalWebGrid, List<FormGroup.FormElement>[] formElementsToPutInWebGrid, List<string>[] formElementsIdToRemoveFromWebGrid, string bindingExpression)
        {
            //Remove FormElementsIdToRemove
            //Remove FormElementsToPut
            //Add FormElementsToPut        

            XmlNamespaceManager nsManager = new XmlNamespaceManager(originalWebGrid.ConfigInfo.OwnerDocument.NameTable);
            nsManager.AddNamespace("COE", "COE.FormGroup");
            List<string> avoidAddingThisElements = new List<string>();
            List<FormGroup.FormElement> webGridElement = new List<FormGroup.FormElement>();
            FormGroup.FormElement webGridFormElement = new FormGroup.FormElement();
            webGridFormElement.DisplayInfo.Type = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGrid";
            webGridFormElement.Id = "ListView";
            webGridFormElement.BindingExpression = bindingExpression;
            //webGridFormElement.DisplayInfo.Position = "relative";
            XmlDocument xmlData = new XmlDocument();
            string xmlns = "COE.FormGroup";
            string xmlprefix = "COE";
            XmlAttribute tableNameAttr = xmlData.CreateAttribute("name");
            XmlNodeList tablesNode = originalWebGrid.ConfigInfo.SelectNodes("//COE:fieldConfig/COE:tables/COE:table", nsManager);
            xmlData.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "configInfo", xmlns));
            XmlNode gridConfNode = xmlData.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "GridConfigSettings", xmlns));
            gridConfNode.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "AllowSorting", xmlns)).InnerText = "true";
            gridConfNode.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "AllowChildSorting", xmlns)).InnerText = "true";
            gridConfNode.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "IsReadOnly", xmlns)).InnerText = "true";
            gridConfNode.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "FixedHeader", xmlns)).InnerText = "true";
            gridConfNode.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "ShowExpandCollapseImage", xmlns)).InnerText = "false";
            gridConfNode.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "ShowChildDataOnly", xmlns)).InnerText = "false";
            XmlNode fieldConfNode = xmlData.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "fieldConfig", xmlns));
            fieldConfNode.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "tables", xmlns));

            XmlNode[] previousCustomColumns = new XmlNode[5];
            for (int i = 0; i < 5; i++)
            {
                previousCustomColumns[i] = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Columns", xmlns);
            }

            XmlNode[] previousColumns = new XmlNode[2];
            for (int i = 0; i < 2; i++)
            {
                previousColumns[i] = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Columns", xmlns);
            }



            //get original columns of table 0 into previousColumns[0] (or previousCustomColumns[0] and previousCustomColumns[1])
            foreach (XmlNode column in tablesNode[0].SelectNodes("./COE:Columns/COE:Column", nsManager))
            {
                bool isInherent = true;
                string colformElementName = string.Empty;
                if (column.SelectSingleNode("./COE:formElement", nsManager) != null)
                   if(column.SelectSingleNode("./COE:formElement", nsManager).Attributes["name"] != null)
                       colformElementName = column.SelectSingleNode("./COE:formElement", nsManager).Attributes["name"].Value;
                if (!formElementsIdToRemoveFromWebGrid[0].Contains(column.Attributes["name"].Value + "Property") &&
                    !formElementsIdToRemoveFromWebGrid[1].Contains(column.Attributes["name"].Value + "Property"))
                {
                    if (column.Attributes["name"] == null || string.IsNullOrEmpty(column.Attributes["name"].Value))
                    {
                        previousColumns[0].AppendChild(previousColumns[0].OwnerDocument.ImportNode(column, true));
                    }
                    else
                    {
                        foreach (Property prop in _rootProperties.GetSortedPropertyList())
                        {
                            if (column.Attributes["name"].Value == prop.Name || colformElementName == prop.Name)
                            {
                                previousCustomColumns[0].AppendChild(previousCustomColumns[0].OwnerDocument.ImportNode(column, true));
                                isInherent = false;
                            }

                        }

                        foreach (Property prop in _batchProperties.GetSortedPropertyList())
                        {
                            if (column.Attributes["name"].Value == prop.Name || colformElementName == prop.Name)
                            {
                                previousCustomColumns[1].AppendChild(previousCustomColumns[1].OwnerDocument.ImportNode(column, true));
                                isInherent = false;
                            }
                        }

                        if (isInherent)
                        {
                            previousColumns[0].AppendChild(previousColumns[0].OwnerDocument.ImportNode(column, true));
                        }
                    }
                }

            }
            //get original columns of table 1 into previousColumns[1] (or previousCustomColumns[2],previousCustomColumns[3],and previousCustomColumns[4])
            foreach (XmlNode column in tablesNode[1].SelectNodes("./COE:Columns/COE:Column", nsManager))
            {
                bool isInherent = true;
                string colformElementName = string.Empty;
                if (column.SelectSingleNode("./COE:formElement", nsManager) != null)
                    if (column.SelectSingleNode("./COE:formElement", nsManager).Attributes["name"] != null)
                        colformElementName = column.SelectSingleNode("./COE:formElement", nsManager).Attributes["name"].Value;
                if (!formElementsIdToRemoveFromWebGrid[2].Contains(column.Attributes["name"].Value + "Property") &&
                    !formElementsIdToRemoveFromWebGrid[3].Contains(column.Attributes["name"].Value + "Property") &&
                    !formElementsIdToRemoveFromWebGrid[4].Contains(column.Attributes["name"].Value + "Property"))
                {
                    if (column.Attributes["name"] == null || string.IsNullOrEmpty(column.Attributes["name"].Value))
                    {
                        previousColumns[1].AppendChild(previousColumns[1].OwnerDocument.ImportNode(column, true));
                    }
                    else
                    {
                        foreach (Property prop in _compoundProperties.GetSortedPropertyList())
                        {
                            if (column.Attributes["name"].Value == prop.Name || colformElementName == prop.Name)
                            {
                                previousCustomColumns[2].AppendChild(previousCustomColumns[2].OwnerDocument.ImportNode(column, true));
                                isInherent = false;
                            }
                        }

                        foreach (Property prop in _batchComponentProperties.GetSortedPropertyList())
                        {
                            if (column.Attributes["name"].Value == prop.Name || colformElementName == prop.Name)
                            {
                                previousCustomColumns[3].AppendChild(previousCustomColumns[3].OwnerDocument.ImportNode(column, true));
                                isInherent = false;
                            }
                        }

                        foreach (Property prop in _structureProperties.GetSortedPropertyList())
                        {
                            if (column.Attributes["name"].Value == prop.Name || colformElementName == prop.Name)
                            {
                                previousCustomColumns[4].AppendChild(previousCustomColumns[4].OwnerDocument.ImportNode(column, true));
                                isInherent = false;
                            }
                        }

                        if (isInherent)
                        {
                            previousColumns[1].AppendChild(previousColumns[1].OwnerDocument.ImportNode(column, true));
                        }
                    }
                }
            }

            for (int i = 0; i < 2; i++)
            {
                XmlNode newTableNode = fieldConfNode.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "table", xmlns));
                newTableNode.Attributes.Append(newTableNode.OwnerDocument.CreateAttribute("name")).Value = tablesNode[i].Attributes["name"].Value;
                //xmlData.FirstChild.FirstChild.FirstChild.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "CSSClass", xmlns)).InnerText = "myTableClass";
                newTableNode.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "headerStyle", xmlns)).InnerText = "color: #000099; background-color: #CFD8E6; font-weight: bold; font-family: Verdana; font-size: 10px;border-collapse:collapse;";
                newTableNode.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "columnStyle", xmlns)).InnerText = "color: #000000; background-color: #FFFFFF; border-color: #CFD8E6; border-style:solid; border-width:1px; font-size:10px;font-family: Verdana;vertical-align:middle;text-align:center;";
                //xmlData.FirstChild.FirstChild.FirstChild.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "", xmlns)).InnerText = "false";
                XmlNode columnsNode = newTableNode.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Columns", xmlns));

                foreach (XmlNode column in previousColumns[i])
                {
                    columnsNode.AppendChild(xmlData.ImportNode(column, true));
                }

                if (i == 0)
                {
                    foreach (Property prop in _rootProperties.GetSortedPropertyList())
                    {

                        foreach (XmlNode customCol in previousCustomColumns[0])
                        {
                            if (customCol.Attributes["name"].Value == prop.Name)
                                columnsNode.InsertBefore(customCol, columnsNode.ChildNodes[columnsNode.ChildNodes.Count - 1]);
                        }

                    }
                    foreach (Property prop in _rootProperties.GetSortedPropertyList())
                    {

                        foreach (FormGroup.FormElement elem in formElementsToPutInWebGrid[0])
                        {
                            if (elem.Name == prop.Name)
                            {
                                XmlNode column = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Column", xmlns);
                                column.Attributes.Append(xmlData.CreateAttribute("name"));
                                column.Attributes["name"].Value = elem.Id.Replace("Property", string.Empty);
                                column.Attributes.Append(xmlData.CreateAttribute("hidden"));
                                column.Attributes["hidden"].Value = IsColumnToHide(prop);
                                elem.Label = string.Empty;
                                elem.BindingExpression = elem.Name;
                                column.InnerXml = elem.ToString().Remove(0, elem.ToString().IndexOf(">") + 1);
                                XmlNode headerNode = xmlData.CreateNode(XmlNodeType.Element, "headerText", xmlns);
                                //headerNode.InnerText = column.Attributes["name"].Value.Remove(0,3);
                                headerNode.InnerText = _formElementsLabels[0][prop.Name];
                                XmlNode widthNode = xmlData.CreateNode(XmlNodeType.Element, "width", xmlns);
                                //widthNode.InnerText = "5px";
                                column.InsertBefore(widthNode, column.FirstChild);
                                column.InsertBefore(headerNode, column.FirstChild);
                                columnsNode.InsertBefore(column, columnsNode.ChildNodes[columnsNode.ChildNodes.Count - 1]);

                            }
                        }
                    }

                    foreach (Property prop in _batchProperties.GetSortedPropertyList())
                    {
                        foreach (XmlNode customCol in previousCustomColumns[1])
                        {
                            if (customCol.Attributes["name"].Value == prop.Name)
                                columnsNode.AppendChild(customCol);
                               // columnsNode.InsertBefore(customCol, columnsNode.ChildNodes[columnsNode.ChildNodes.Count - 1]);
                        }
                    }
                    foreach (Property prop in _batchProperties.GetSortedPropertyList())
                    {

                        foreach (FormGroup.FormElement elem in formElementsToPutInWebGrid[1])
                        {
                            if (elem.Name == prop.Name)
                            {
                                XmlNode column = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Column", xmlns);
                                column.Attributes.Append(xmlData.CreateAttribute("name"));
                                column.Attributes["name"].Value = elem.Id.Replace("Property", string.Empty);
                                column.Attributes.Append(xmlData.CreateAttribute("hidden"));
                                column.Attributes["hidden"].Value = IsColumnToHide(prop);
                                elem.Label = string.Empty;
                                elem.BindingExpression = elem.Name;
                                column.InnerXml = elem.ToString().Remove(0, elem.ToString().IndexOf(">") + 1);
                                XmlNode headerNode = xmlData.CreateNode(XmlNodeType.Element, "headerText", xmlns);
                                //headerNode.InnerText = column.Attributes["name"].Value.Remove(0,3);
                                headerNode.InnerText = _formElementsLabels[2][prop.Name];
                                XmlNode widthNode = xmlData.CreateNode(XmlNodeType.Element, "width", xmlns);
                                //widthNode.InnerText = "5px";
                                column.InsertBefore(widthNode, column.FirstChild);
                                column.InsertBefore(headerNode, column.FirstChild);
                                columnsNode.AppendChild(column);
                                //columnsNode.InsertBefore(column, columnsNode.ChildNodes[columnsNode.ChildNodes.Count - 1]);

                            }
                        }
                    }
                }
                else
                {
                    foreach (Property prop in _compoundProperties.GetSortedPropertyList())
                    {
                        foreach (XmlNode customCol in previousCustomColumns[2])
                        {
                            if (customCol.Attributes["name"].Value == prop.Name)
                                columnsNode.AppendChild(customCol);
                        }
                    }

                    foreach (Property prop in _compoundProperties.GetSortedPropertyList())
                    {

                        foreach (FormGroup.FormElement elem in formElementsToPutInWebGrid[2])
                        {
                            if (elem.Name == prop.Name)
                            {
                                XmlNode column = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Column", xmlns);
                                column.Attributes.Append(xmlData.CreateAttribute("name"));
                                column.Attributes["name"].Value = elem.Id.Replace("Property", string.Empty);
                                column.Attributes.Append(xmlData.CreateAttribute("hidden"));
                                column.Attributes["hidden"].Value = IsColumnToHide(prop);
                                elem.Label = string.Empty;
                                elem.BindingExpression = elem.Name;
                                column.InnerXml = elem.ToString().Remove(0, elem.ToString().IndexOf(">") + 1);
                                XmlNode headerNode = xmlData.CreateNode(XmlNodeType.Element, "headerText", xmlns);
                                //headerNode.InnerText = column.Attributes["name"].Value.Remove(0,3);
                                headerNode.InnerText = _formElementsLabels[1][prop.Name];
                                XmlNode widthNode = xmlData.CreateNode(XmlNodeType.Element, "width", xmlns);
                                //widthNode.InnerText = "5px";
                                column.InsertBefore(widthNode, column.FirstChild);
                                column.InsertBefore(headerNode, column.FirstChild);
                                columnsNode.AppendChild(column);
                            }
                        }
                    }

                    foreach (Property prop in _batchComponentProperties.GetSortedPropertyList())
                    {

                        foreach (XmlNode customCol in previousCustomColumns[3])
                        {
                            if (customCol.Attributes["name"].Value == prop.Name)
                                columnsNode.AppendChild(customCol);
                        }
                    }

                    foreach (Property prop in _batchComponentProperties.GetSortedPropertyList())
                    {
                        foreach (FormGroup.FormElement elem in formElementsToPutInWebGrid[3])
                        {
                            if (elem.Name == prop.Name)
                            {
                                XmlNode column = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Column", xmlns);
                                column.Attributes.Append(xmlData.CreateAttribute("name"));
                                column.Attributes["name"].Value = elem.Id.Replace("Property", string.Empty);
                                column.Attributes.Append(xmlData.CreateAttribute("hidden"));
                                column.Attributes["hidden"].Value = IsColumnToHide(prop);
                                elem.Label = string.Empty;
                                elem.BindingExpression = elem.Name;
                                column.InnerXml = elem.ToString().Remove(0, elem.ToString().IndexOf(">") + 1);
                                XmlNode headerNode = xmlData.CreateNode(XmlNodeType.Element, "headerText", xmlns);
                                //headerNode.InnerText = column.Attributes["name"].Value.Remove(0,3);
                                headerNode.InnerText = _formElementsLabels[3][prop.Name];
                                XmlNode widthNode = xmlData.CreateNode(XmlNodeType.Element, "width", xmlns);
                                //widthNode.InnerText = "5px";
                                column.InsertBefore(widthNode, column.FirstChild);
                                column.InsertBefore(headerNode, column.FirstChild);
                                columnsNode.AppendChild(column);
                            }
                        }
                    }

                    foreach (Property prop in _structureProperties.GetSortedPropertyList())
                    {
                        foreach (XmlNode customCol in previousCustomColumns[4])
                        {
                            if (customCol.Attributes["name"].Value == prop.Name)
                                columnsNode.AppendChild(customCol);
                        }
                    }

                    foreach (Property prop in _structureProperties.GetSortedPropertyList())
                    {

                        foreach (FormGroup.FormElement elem in formElementsToPutInWebGrid[4])
                        {
                            if (elem.Name == prop.Name)
                            {
                                XmlNode column = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Column", xmlns);
                                column.Attributes.Append(xmlData.CreateAttribute("name"));
                                column.Attributes["name"].Value = elem.Id.Replace("Property", string.Empty);
                                column.Attributes.Append(xmlData.CreateAttribute("hidden"));
                                column.Attributes["hidden"].Value = IsColumnToHide(prop);
                                elem.Label = string.Empty;
                                elem.BindingExpression = elem.Name;
                                column.InnerXml = elem.ToString().Remove(0, elem.ToString().IndexOf(">") + 1);
                                XmlNode headerNode = xmlData.CreateNode(XmlNodeType.Element, "headerText", xmlns);
                                //headerNode.InnerText = column.Attributes["name"].Value.Remove(0,3);
                                headerNode.InnerText = _formElementsLabels[4][prop.Name];
                                XmlNode widthNode = xmlData.CreateNode(XmlNodeType.Element, "width", xmlns);
                                //widthNode.InnerText = "5px";
                                column.InsertBefore(widthNode, column.FirstChild);
                                column.InsertBefore(headerNode, column.FirstChild);
                                columnsNode.AppendChild(column);
                            }
                        }
                    }
                }

            }

            webGridFormElement.ConfigInfo = xmlData.FirstChild;
            webGridElement.Add(webGridFormElement);

            return webGridElement;
        }

        /// <summary>
        /// For each new/deleted ConfigurationProperty, update COEDataView (add/delete field), 
        /// as well as resultsCriteria (add a new Field criteria or delete an existing one) of detailsForms and listForms.
        /// It also construct an array (length=5) of list of FormElement ids to be removed (as a ref parameter)
        /// </summary>
        /// <remarks>
        /// For Mixture and Batch level custom properties,update <see cref="TEMPBATCHTABLENAME"/> table of COEDataView;
        /// For Compound,BatchComponent and Structure level custom properties, update <see cref="TEMPCOMPOUNDTABLENAME"/> table of COEDataView
        /// </remarks>
        /// <param name="formElementsIdToRemove">id of formelements to be removed</param>
        /// <param name="resultsCriteria"></param>
        /// <param name="listFormResultCriteria"></param>
        /// <param name="formId"></param>
        /// <returns>Saved COEDataView</returns>
        private COEDataView CreateTemporaryDataViewAndResultCriteria(ref List<string>[] formElementsIdToRemove, ref ResultsCriteria resultsCriteria, ref ResultsCriteria listFormResultCriteria, int formId)
        {
            COEDataViewBO tempDataView = COEDataViewBO.Get(formId);

            #region Removing vw_temporarybatch fields

            formElementsIdToRemove[0] = new List<string>();

            foreach (ConfigurationProperty prop in _rootProperties.GetDeletedList())
            {
                FieldBO field = tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].Fields.GetField("REGDB", prop.Name);
                if (field != null)
                {
                    tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].Fields.Remove(field);
                    formElementsIdToRemove[0].Add(prop.Name + "Property");

                    ResultsCriteria.Field criteriaToRemove = null;
                    if (resultsCriteria != null && resultsCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID] != null)
                    {
                        foreach (ResultsCriteria.IResultsCriteriaBase criteria in resultsCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID].Criterias)
                        {
                            if (criteria is ResultsCriteria.Field && ((ResultsCriteria.Field)criteria).Alias == prop.Name)
                            {
                                criteriaToRemove = (ResultsCriteria.Field)criteria;
                                break;
                            }
                        }
                        if (criteriaToRemove != null)
                        {
                            resultsCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID].Criterias.Remove(criteriaToRemove);
                        }
                    }

                    criteriaToRemove = null;
                    if (listFormResultCriteria != null && listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID] != null)
                    {
                        foreach (ResultsCriteria.IResultsCriteriaBase criteria in listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID].Criterias)
                        {
                            if (criteria is ResultsCriteria.Field && ((ResultsCriteria.Field)criteria).Alias == prop.Name)
                            {
                                criteriaToRemove = (ResultsCriteria.Field)criteria;
                                break;
                            }
                        }
                        if (criteriaToRemove != null)
                        {
                            listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID].Criterias.Remove(criteriaToRemove);
                        }
                    }
                }
            }

            formElementsIdToRemove[1] = new List<string>();
            foreach (ConfigurationProperty prop in _batchProperties.GetDeletedList())
            {
                FieldBO field = tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].Fields.GetField("REGDB", prop.Name);
                if (field != null)
                {
                    tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].Fields.Remove(field);

                    formElementsIdToRemove[1].Add(prop.Name + "Property");

                    ResultsCriteria.Field criteriaToRemove = null;
                    if (resultsCriteria != null && resultsCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID] != null)
                    {
                        foreach (ResultsCriteria.IResultsCriteriaBase criteria in resultsCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID].Criterias)
                        {
                            if (criteria is ResultsCriteria.Field && ((ResultsCriteria.Field)criteria).Alias == prop.Name)
                            {
                                criteriaToRemove = (ResultsCriteria.Field)criteria;
                                break;
                            }
                        }
                        if (criteriaToRemove != null)
                        {
                            resultsCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID].Criterias.Remove(criteriaToRemove);
                        }
                    }

                    criteriaToRemove = null;
                    if (listFormResultCriteria != null && listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID] != null)
                    {
                        foreach (ResultsCriteria.IResultsCriteriaBase criteria in listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID].Criterias)
                        {
                            if (criteria is ResultsCriteria.Field && ((ResultsCriteria.Field)criteria).Alias == prop.Name)
                            {
                                criteriaToRemove = (ResultsCriteria.Field)criteria;
                                break;
                            }
                        }
                        if (criteriaToRemove != null)
                        {
                            listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID].Criterias.Remove(criteriaToRemove);
                        }
                    }
                }
            }

            #endregion

            #region Removing vw_temporarycompound fields

            formElementsIdToRemove[2] = new List<string>();
            foreach (ConfigurationProperty prop in _compoundProperties.GetDeletedList())
            {
                FieldBO field = tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].Fields.GetField("REGDB", prop.Name);
                if (field != null)
                {
                    tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].Fields.Remove(field);

                    formElementsIdToRemove[2].Add(prop.Name + "Property");

                    ResultsCriteria.Field criteriaToRemove = null;
                    if (resultsCriteria != null && resultsCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID] != null)
                    {
                        foreach (ResultsCriteria.IResultsCriteriaBase criteria in resultsCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID].Criterias)
                        {
                            if (criteria is ResultsCriteria.Field && ((ResultsCriteria.Field)criteria).Alias == prop.Name)
                            {
                                criteriaToRemove = (ResultsCriteria.Field)criteria;
                                break;
                            }
                        }
                        if (criteriaToRemove != null)
                        {
                            resultsCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID].Criterias.Remove(criteriaToRemove);
                        }
                    }

                    criteriaToRemove = null;
                    if (listFormResultCriteria != null && listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID] != null)
                    {
                        foreach (ResultsCriteria.IResultsCriteriaBase criteria in listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID].Criterias)
                        {
                            if (criteria is ResultsCriteria.Field && ((ResultsCriteria.Field)criteria).Alias == prop.Name)
                            {
                                criteriaToRemove = (ResultsCriteria.Field)criteria;
                                break;
                            }
                        }
                        if (criteriaToRemove != null)
                        {
                            listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID].Criterias.Remove(criteriaToRemove);
                        }
                    }
                }
            }
            formElementsIdToRemove[3] = new List<string>();
            foreach (ConfigurationProperty prop in _batchComponentProperties.GetDeletedList())
            {
                FieldBO field = tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].Fields.GetField("REGDB", prop.Name);
                if (field != null)
                {
                    tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].Fields.Remove(field);

                    formElementsIdToRemove[3].Add(prop.Name + "Property");

                    ResultsCriteria.Field criteriaToRemove = null;
                    if (resultsCriteria != null && resultsCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID] != null)
                    {
                        foreach (ResultsCriteria.IResultsCriteriaBase criteria in resultsCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID].Criterias)
                        {
                            if (criteria is ResultsCriteria.Field && ((ResultsCriteria.Field)criteria).Alias == prop.Name)
                            {
                                criteriaToRemove = (ResultsCriteria.Field)criteria;
                                break;
                            }
                        }
                        if (criteriaToRemove != null)
                        {
                            resultsCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID].Criterias.Remove(criteriaToRemove);
                        }
                    }

                    criteriaToRemove = null;
                    if (listFormResultCriteria != null && listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID] != null)
                    {
                        foreach (ResultsCriteria.IResultsCriteriaBase criteria in listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID].Criterias)
                        {
                            if (criteria is ResultsCriteria.Field && ((ResultsCriteria.Field)criteria).Alias == prop.Name)
                            {
                                criteriaToRemove = (ResultsCriteria.Field)criteria;
                                break;
                            }
                        }
                        if (criteriaToRemove != null)
                        {
                            listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID].Criterias.Remove(criteriaToRemove);
                        }
                    }
                }
            }
            formElementsIdToRemove[4] = new List<string>();
            foreach (ConfigurationProperty prop in _structureProperties.GetDeletedList())
            {
                FieldBO field = tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].Fields.GetField("REGDB", prop.Name);
                if (field != null)
                {
                    tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].Fields.Remove(field);

                    formElementsIdToRemove[4].Add(prop.Name + "Property");

                    ResultsCriteria.Field criteriaToRemove = null;
                    if (resultsCriteria != null && resultsCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID] != null)
                    {
                        foreach (ResultsCriteria.IResultsCriteriaBase criteria in resultsCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID].Criterias)
                        {
                            if (criteria is ResultsCriteria.Field && ((ResultsCriteria.Field)criteria).Alias == prop.Name)
                            {
                                criteriaToRemove = (ResultsCriteria.Field)criteria;
                                break;
                            }
                        }
                        if (criteriaToRemove != null)
                        {
                            resultsCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID].Criterias.Remove(criteriaToRemove);
                        }
                    }

                    criteriaToRemove = null;
                    if (listFormResultCriteria != null && listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID] != null)
                    {
                        foreach (ResultsCriteria.IResultsCriteriaBase criteria in listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID].Criterias)
                        {
                            if (criteria is ResultsCriteria.Field && ((ResultsCriteria.Field)criteria).Alias == prop.Name)
                            {
                                criteriaToRemove = (ResultsCriteria.Field)criteria;
                                break;
                            }
                        }
                        if (criteriaToRemove != null)
                        {
                            listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID].Criterias.Remove(criteriaToRemove);
                        }
                    }
                }
            }
            #endregion

            #region Adding vw_temporarybatch fields

            foreach (ConfigurationProperty prop in _rootProperties)
            {
                if (tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].Fields.GetField("REGDB", prop.Name) == null)
                {
                    COEDataView.Field dvField = new COEDataView.Field();
                    dvField.DataType = this.ConvertToAbstractType(prop.Type);
                    dvField.Name = prop.Name;
                    //dvField.Id = (tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].Fields.HighestID + fieldsAdded++);
                    dvField.Id = this.GetCustomizeHighestFieldId(tempDataView, TEMPBATCHTABLENAME);
                    tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].Fields.Add(FieldBO.NewField(dvField, "REGDB"));
                    if (!ExistsResultCriteria(resultsCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID], prop.Name))
                    {
                        ResultsCriteria.Field result = new ResultsCriteria.Field(dvField.Id);
                        result.Alias = prop.Name;
                        if (resultsCriteria != null && resultsCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID] != null)
                            resultsCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID].Criterias.Add(result);
                        if (listFormResultCriteria != null && listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID] != null)
                            listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID].Criterias.Add(result);
                    }
                }
            }
            foreach (ConfigurationProperty prop in _batchProperties)
            {
                if (tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].Fields.GetField("REGDB", prop.Name) == null)
                {
                    COEDataView.Field dvField = new COEDataView.Field();
                    dvField.DataType = this.ConvertToAbstractType(prop.Type);
                    dvField.Name = prop.Name;
                    //dvField.Id = (tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].Fields.HighestID + fieldsAdded++);
                    dvField.Id = this.GetCustomizeHighestFieldId(tempDataView, TEMPBATCHTABLENAME);
                    tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].Fields.Add(FieldBO.NewField(dvField, "REGDB"));
                    if (!ExistsResultCriteria(resultsCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID], prop.Name))
                    {
                        ResultsCriteria.Field result = new ResultsCriteria.Field(dvField.Id);
                        result.Alias = prop.Name;
                        if (resultsCriteria != null && resultsCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID] != null)
                            resultsCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID].Criterias.Add(result);
                        if (listFormResultCriteria != null && listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID] != null)
                            listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPBATCHTABLENAME].ID].Criterias.Add(result);
                    }
                }
            }
            #endregion

            #region Adding vw_temporarycompound fields
            foreach (ConfigurationProperty prop in _compoundProperties)
            {
                if (tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].Fields.GetField("REGDB", prop.Name) == null)
                {
                    COEDataView.Field dvField = new COEDataView.Field();
                    dvField.DataType = this.ConvertToAbstractType(prop.Type);
                    dvField.Name = prop.Name;
                    //dvField.Id = (tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].Fields.HighestID + fieldsAdded++);
                    dvField.Id = this.GetCustomizeHighestFieldId(tempDataView, TEMPCOMPOUNDTABLENAME);
                    tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].Fields.Add(FieldBO.NewField(dvField, "REGDB"));
                    if (!ExistsResultCriteria(resultsCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID], prop.Name))
                    {
                        ResultsCriteria.Field result = new ResultsCriteria.Field(dvField.Id);
                        result.Alias = prop.Name;
                        if (resultsCriteria != null && resultsCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID] != null)
                            resultsCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID].Criterias.Add(result);
                        if (listFormResultCriteria != null && listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID] != null)
                            listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID].Criterias.Add(result);
                    }
                }
            }
            foreach (ConfigurationProperty prop in _batchComponentProperties)
            {
                if (tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].Fields.GetField("REGDB", prop.Name) == null)
                {
                    COEDataView.Field dvField = new COEDataView.Field();
                    dvField.DataType = this.ConvertToAbstractType(prop.Type);
                    dvField.Name = prop.Name;
                    //dvField.Id = (tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].Fields.HighestID + fieldsAdded++);
                    dvField.Id = this.GetCustomizeHighestFieldId(tempDataView, TEMPCOMPOUNDTABLENAME);
                    tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].Fields.Add(FieldBO.NewField(dvField, "REGDB"));
                    if (!ExistsResultCriteria(resultsCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID], prop.Name))
                    {
                        ResultsCriteria.Field result = new ResultsCriteria.Field(dvField.Id);
                        result.Alias = prop.Name;
                        if (resultsCriteria != null && resultsCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID] != null)
                            resultsCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID].Criterias.Add(result);
                        if (listFormResultCriteria != null && listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID] != null)
                            listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID].Criterias.Add(result);
                    }
                }
            }
            foreach (ConfigurationProperty prop in _structureProperties)
            {
                if (tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].Fields.GetField("REGDB", prop.Name) == null)
                {
                    COEDataView.Field dvField = new COEDataView.Field();
                    dvField.DataType = this.ConvertToAbstractType(prop.Type);
                    dvField.Name = prop.Name;
                    //dvField.Id = (tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].Fields.HighestID + fieldsAdded++);
                    dvField.Id = this.GetCustomizeHighestFieldId(tempDataView, TEMPCOMPOUNDTABLENAME);
                    tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].Fields.Add(FieldBO.NewField(dvField, "REGDB"));
                    if (!ExistsResultCriteria(resultsCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID], prop.Name))
                    {
                        ResultsCriteria.Field result = new ResultsCriteria.Field(dvField.Id);
                        result.Alias = prop.Name;
                        if (resultsCriteria != null && resultsCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID] != null)
                            resultsCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID].Criterias.Add(result);
                        if (listFormResultCriteria != null && listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID] != null)
                            listFormResultCriteria[tempDataView.DataViewManager.Tables[TEMPCOMPOUNDTABLENAME].ID].Criterias.Add(result);
                    }
                }
            }
            #endregion

            return tempDataView.SaveFromDataViewManager().COEDataView;
        }

        private bool ExistsResultCriteria(ResultsCriteria.ResultsCriteriaTable resultsCriteriaTable, string alias)
        {
            bool result = false;
            if (resultsCriteriaTable != null)
            {
                foreach (ResultsCriteria.IResultsCriteriaBase criteria in resultsCriteriaTable.Criterias)
                {
                    if (criteria is ResultsCriteria.Field && ((ResultsCriteria.Field)criteria).Alias == alias)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        private void UpdateSearchPermanentFormGroup(int formId)
        {
            COEFormBO formGroupBO = COEFormBO.Get(formId);
            _searchCriteria = this.GetSearchCriteria(formGroupBO.COEFormGroup.QueryForms[0]);
            List<FormGroup.FormElement> queryFormElements = new List<FormGroup.FormElement>();
            List<FormGroup.FormElement> queryFormElements_ValRuleListUpdated = new List<FormGroup.FormElement>();
            List<FormGroup.FormElement> detailsFormElements = new List<FormGroup.FormElement>();
            List<string> formElementsIdToRemoveFromMixture = new List<string>();
            List<string> formElementsIdToRemoveFromCompound = new List<string>();
            List<string> formElementsIdToRemoveFromStructure = new List<string>();
            List<string> formElementsIdToRemoveFromBatch = new List<string>();
            List<string> formElementsIdToRemoveFromBatchComponent = new List<string>();
            List<string>[] formElementsIdToRemoveFromListForm = new List<string>[5];
            List<FormGroup.FormElement>[] listFormElements = new List<FormGroup.FormElement>[5];

            ResultsCriteria listResultCriteria = new ResultsCriteria();
            ResultsCriteria resultCriteria = new ResultsCriteria();
            if (formGroupBO.COEFormGroup.ListForms.Displays.Count > 0)
                listResultCriteria = ((FormGroup.ListDisplay)formGroupBO.COEFormGroup.ListForms[0]).ResultsCriteria;
            if (formGroupBO.COEFormGroup.DetailsForms.Displays.Count > 0)
                resultCriteria = ((FormGroup.DetailsDisplay)formGroupBO.COEFormGroup.DetailsForms[0]).ResultsCriteria;


            COEDataView dataview = CreatePermanentDataViewAndResultCriteria(ref formElementsIdToRemoveFromMixture, ref formElementsIdToRemoveFromCompound, ref formElementsIdToRemoveFromStructure, ref formElementsIdToRemoveFromBatch, ref formElementsIdToRemoveFromBatchComponent, ref resultCriteria, ref listResultCriteria, formId);

            formElementsIdToRemoveFromListForm[MIXTURESEARCHFORM] = formElementsIdToRemoveFromMixture;
            formElementsIdToRemoveFromListForm[COMPOUNDSEARCHFORM] = formElementsIdToRemoveFromCompound;
            formElementsIdToRemoveFromListForm[BATCHSEARCHFORM] = formElementsIdToRemoveFromBatch;
            formElementsIdToRemoveFromListForm[BATCHCOMPONENTSEARCHFORM] = formElementsIdToRemoveFromBatchComponent;
            formElementsIdToRemoveFromListForm[STRUCTURESEARCHFORM] = formElementsIdToRemoveFromStructure;

            #region vw_mixture

            if (dataview.Tables[MIXTURETABLENAME] != null)
            {
                BuildFormElementsListForSearch(dataview.Tables[MIXTURETABLENAME], ref queryFormElements, _rootProperties, PropertyListType.MixturePropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
                BuildFormElementsListForSearch(formGroupBO.GetForm(formGroupBO.COEFormGroup.QueryForms, QUERYFORMINDEX, MIXTURESEARCHFORM),
                    FormGroup.DisplayMode.All,
                    dataview.Tables[MIXTURETABLENAME],
                    ref queryFormElements_ValRuleListUpdated,
                    _rootProperties, PropertyListType.MixturePropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
                if (resultCriteria[dataview.Tables[MIXTURETABLENAME].Id] != null)
                    BuildDetailFormElements(resultCriteria[dataview.Tables[MIXTURETABLENAME].Id], ref detailsFormElements, _rootProperties, PropertyListType.MixturePropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
            }

            if (formGroupBO.COEFormGroup.QueryForms.Displays.Count > 0 && formGroupBO.COEFormGroup.QueryForms[0][MIXTURETABLENAME] != null)
            {
                formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, MIXTURESEARCHFORM, FormGroup.DisplayMode.All, formElementsIdToRemoveFromMixture);
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, MIXTURESEARCHFORM, FormGroup.DisplayMode.All, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, queryFormElements);
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, MIXTURESEARCHFORM, FormGroup.DisplayMode.All, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, queryFormElements_ValRuleListUpdated);
            }

            if (formGroupBO.COEFormGroup.DetailsForms.Displays.Count > 0 && formGroupBO.COEFormGroup.DetailsForms[0][MIXTURETABLENAME] != null)
            {
                formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, MIXTURESEARCHFORM, FormGroup.DisplayMode.View, formElementsIdToRemoveFromMixture);
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, MIXTURESEARCHFORM, FormGroup.DisplayMode.View, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, detailsFormElements);
            }

            bool sortOrderIsUpdate = false;

            List<string> formElementsIds = new List<string>();


            foreach (Property prop in _rootProperties)
            {
                if (prop.SortOrderIsUpdate)
                    sortOrderIsUpdate = true;
            }

            if (sortOrderIsUpdate)
            {
                foreach (Property prop in _rootProperties)
                {
                    formElementsIds.Add(prop.Name + "Property");
                }

                if (formGroupBO.COEFormGroup.QueryForms.Displays.Count > 0)
                    formGroupBO.UpdateFormElemetsSortOrder(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, MIXTURESEARCHFORM, formElementsIds);
                if (formGroupBO.COEFormGroup.DetailsForms.Displays.Count > 0)
                    formGroupBO.UpdateFormElemetsSortOrder(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, MIXTURESEARCHFORM, formElementsIds);
                sortOrderIsUpdate = false;

                formElementsIds.Clear();
            }

            listFormElements[0] = new List<FormGroup.FormElement>();

            if (listResultCriteria[dataview.Tables[MIXTURETABLENAME].Id] != null)
				// CBOE-1963 FormID is passed as argument to handle the form in the definition-- At registry level
                BuildDetailFormElements(listResultCriteria[dataview.Tables[MIXTURETABLENAME].Id], ref listFormElements[0], _rootProperties, PropertyListType.MixturePropertyList, PropertyList.NewPropertyList(), PropertyListType.None, formId);

            #endregion

            List<FormGroup.FormElement> gridView = null;
            List<string> gridToRemove = new List<string>();

            #region vw_compound

            queryFormElements.Clear();
            queryFormElements_ValRuleListUpdated.Clear();
            detailsFormElements.Clear();

            if (dataview.Tables[COMPOUNDTABLENAME] != null)
            {
                BuildFormElementsListForSearch(dataview.Tables[COMPOUNDTABLENAME], ref queryFormElements, _compoundProperties, PropertyListType.CompoundPropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
                BuildFormElementsListForSearch(formGroupBO.GetForm(formGroupBO.COEFormGroup.QueryForms, QUERYFORMINDEX, COMPOUNDSEARCHFORM),
                    FormGroup.DisplayMode.All,
                    dataview.Tables[COMPOUNDTABLENAME],
                    ref queryFormElements_ValRuleListUpdated, 
                    _compoundProperties, PropertyListType.CompoundPropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
                if (resultCriteria[dataview.Tables[COMPOUNDTABLENAME].Id] != null)
                    BuildDetailFormElements(resultCriteria[dataview.Tables[COMPOUNDTABLENAME].Id], ref detailsFormElements, _compoundProperties, PropertyListType.CompoundPropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
            }

            if (formGroupBO.COEFormGroup.QueryForms.Displays.Count > 0 && formGroupBO.COEFormGroup.QueryForms[0][COMPOUNDTABLENAME] != null)
            {
                formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, COMPOUNDSEARCHFORM, FormGroup.DisplayMode.All, formElementsIdToRemoveFromCompound);
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, COMPOUNDSEARCHFORM, FormGroup.DisplayMode.All, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, queryFormElements);
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, COMPOUNDSEARCHFORM, FormGroup.DisplayMode.All, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, queryFormElements_ValRuleListUpdated);
            }

            if (formGroupBO.COEFormGroup.DetailsForms.Displays.Count > 0 && formGroupBO.COEFormGroup.DetailsForms[0][COMPOUNDTABLENAME] != null)
            {
                gridView = GetGridViewGridViewDetailsForms(formGroupBO.COEFormGroup.DetailsForms[0][COMPOUNDTABLENAME].ViewMode[0], detailsFormElements, formElementsIdToRemoveFromCompound, "Table_" + dataview.Tables[COMPOUNDTABLENAME].Id, "FilteredDataSet");
                gridToRemove.Add(formGroupBO.COEFormGroup.DetailsForms[0][COMPOUNDTABLENAME].ViewMode[0].Id);
                formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, COMPOUNDSEARCHFORM, FormGroup.DisplayMode.View, gridToRemove);
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, COMPOUNDSEARCHFORM, FormGroup.DisplayMode.View, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, gridView);
            }
            foreach (Property prop in _compoundProperties)
            {
                if (prop.SortOrderIsUpdate)
                    sortOrderIsUpdate = true;
            }

            if (sortOrderIsUpdate)
            {
                foreach (Property prop in _compoundProperties)
                {
                    formElementsIds.Add(prop.Name + "Property");
                }

                if (formGroupBO.COEFormGroup.QueryForms.Displays.Count > 0)
                    formGroupBO.UpdateFormElemetsSortOrder(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, COMPOUNDSEARCHFORM, formElementsIds);

                sortOrderIsUpdate = false;

                formElementsIds.Clear();
            }

            listFormElements[1] = new List<FormGroup.FormElement>();

            if (listResultCriteria[dataview.Tables[COMPOUNDTABLENAME].Id] != null)
                BuildDetailFormElements(listResultCriteria[dataview.Tables[COMPOUNDTABLENAME].Id], ref listFormElements[1], _compoundProperties, PropertyListType.CompoundPropertyList, PropertyList.NewPropertyList(), PropertyListType.None);

            #endregion

            #region vw_batch

            queryFormElements.Clear();
            queryFormElements_ValRuleListUpdated.Clear();
            detailsFormElements.Clear();
            gridToRemove.Clear();

            if (dataview.Tables[BATCHTABLENAME] != null)
            {
                BuildFormElementsListForSearch(dataview.Tables[BATCHTABLENAME], ref queryFormElements, _batchProperties, PropertyListType.BatchPropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
                BuildFormElementsListForSearch(formGroupBO.GetForm(formGroupBO.COEFormGroup.QueryForms, QUERYFORMINDEX, BATCHSEARCHFORM),
                    FormGroup.DisplayMode.All,
                    dataview.Tables[BATCHTABLENAME],
                    ref queryFormElements_ValRuleListUpdated, 
                    _batchProperties, PropertyListType.BatchPropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
                if (resultCriteria[dataview.Tables[BATCHTABLENAME].Id] != null)
                    BuildDetailFormElements(resultCriteria[dataview.Tables[BATCHTABLENAME].Id], ref detailsFormElements, _batchProperties, PropertyListType.BatchPropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
            }
            if (formGroupBO.COEFormGroup.QueryForms.Displays.Count > 0 && formGroupBO.COEFormGroup.QueryForms[0][BATCHTABLENAME] != null)
            {
                formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, BATCHSEARCHFORM, FormGroup.DisplayMode.All, formElementsIdToRemoveFromBatch);
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, BATCHSEARCHFORM, FormGroup.DisplayMode.All, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, queryFormElements);
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, BATCHSEARCHFORM, FormGroup.DisplayMode.All, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, queryFormElements_ValRuleListUpdated);
            }

            if (formGroupBO.COEFormGroup.DetailsForms.Displays.Count > 0 && formGroupBO.COEFormGroup.DetailsForms[0][BATCHTABLENAME] != null)
            {
                gridView = GetGridViewGridViewDetailsForms(formGroupBO.COEFormGroup.DetailsForms[0][BATCHTABLENAME].ViewMode[0], detailsFormElements, formElementsIdToRemoveFromBatch, "Table_" + dataview.Tables[BATCHTABLENAME].Id, "FilteredDataSet");
                gridToRemove.Add(formGroupBO.COEFormGroup.DetailsForms[0][BATCHTABLENAME].ViewMode[0].Id);
                formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHSEARCHFORM, FormGroup.DisplayMode.View, gridToRemove);
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHSEARCHFORM, FormGroup.DisplayMode.View, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, gridView);
            }

            foreach (Property prop in _batchProperties)
            {
                if (prop.SortOrderIsUpdate)
                    sortOrderIsUpdate = true;
            }

            if (sortOrderIsUpdate)
            {
                foreach (Property prop in _batchProperties)
                {
                    formElementsIds.Add(prop.Name + "Property");
                }

                if (formGroupBO.COEFormGroup.QueryForms.Displays.Count > 0)
                    formGroupBO.UpdateFormElemetsSortOrder(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, BATCHSEARCHFORM, formElementsIds);

                sortOrderIsUpdate = false;

                formElementsIds.Clear();
            }

            listFormElements[2] = new List<FormGroup.FormElement>();

            if (listResultCriteria[dataview.Tables[BATCHTABLENAME].Id] != null)
                // CBOE-1963 FormID is passed as argument to handle the form in the definition-- At Batch level
                BuildDetailFormElements(listResultCriteria[dataview.Tables[BATCHTABLENAME].Id], ref listFormElements[2], _batchProperties, PropertyListType.BatchPropertyList, PropertyList.NewPropertyList(), PropertyListType.None, formId);
            #endregion

            #region vw_batchcomponent

            queryFormElements.Clear();
            queryFormElements_ValRuleListUpdated.Clear();
            detailsFormElements.Clear();
            gridToRemove.Clear();

            if (dataview.Tables[BATCHCOMPONENTTABLENAME] != null)
            {
                BuildFormElementsListForSearch(dataview.Tables[BATCHCOMPONENTTABLENAME], ref queryFormElements, _batchComponentProperties, PropertyListType.BatchComponentPropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
                BuildFormElementsListForSearch(formGroupBO.GetForm(formGroupBO.COEFormGroup.QueryForms, QUERYFORMINDEX, BATCHCOMPONENTSEARCHFORM),
                    FormGroup.DisplayMode.All,
                    dataview.Tables[BATCHCOMPONENTTABLENAME],
                    ref queryFormElements_ValRuleListUpdated, 
                    _batchComponentProperties, PropertyListType.BatchComponentPropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
                if (resultCriteria[dataview.Tables[BATCHCOMPONENTTABLENAME].Id] != null)
                    BuildDetailFormElements(resultCriteria[dataview.Tables[BATCHCOMPONENTTABLENAME].Id], ref detailsFormElements, _batchComponentProperties, PropertyListType.BatchComponentPropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
            }

            if (formGroupBO.COEFormGroup.QueryForms.Displays.Count > 0 && formGroupBO.COEFormGroup.QueryForms[0][BATCHCOMPONENTTABLENAME] != null)
            {
                formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, BATCHCOMPONENTSEARCHFORM, FormGroup.DisplayMode.All, formElementsIdToRemoveFromBatchComponent);
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, BATCHCOMPONENTSEARCHFORM, FormGroup.DisplayMode.All, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, queryFormElements);
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, BATCHCOMPONENTSEARCHFORM, FormGroup.DisplayMode.All, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, queryFormElements_ValRuleListUpdated);
            }

            if (formGroupBO.COEFormGroup.DetailsForms.Displays.Count > 0 && formGroupBO.COEFormGroup.DetailsForms[0][BATCHCOMPONENTTABLENAME] != null)
            {
                gridView = GetGridViewGridViewDetailsForms(formGroupBO.COEFormGroup.DetailsForms[0][BATCHCOMPONENTTABLENAME].ViewMode[0], detailsFormElements, formElementsIdToRemoveFromBatchComponent, "Table_" + dataview.Tables[BATCHCOMPONENTTABLENAME].Id, "FilteredDataSet");
                gridToRemove.Add(formGroupBO.COEFormGroup.DetailsForms[0][BATCHCOMPONENTTABLENAME].ViewMode[0].Id);
                formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHCOMPONENTSEARCHFORM, FormGroup.DisplayMode.View, gridToRemove);
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, BATCHCOMPONENTSEARCHFORM, FormGroup.DisplayMode.View, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, gridView);
            }

            foreach (Property prop in _batchComponentProperties)
            {
                if (prop.SortOrderIsUpdate)
                    sortOrderIsUpdate = true;
            }

            if (sortOrderIsUpdate)
            {
                foreach (Property prop in _batchComponentProperties)
                {
                    formElementsIds.Add(prop.Name + "Property");
                }

                if (formGroupBO.COEFormGroup.QueryForms.Displays.Count > 0)
                    formGroupBO.UpdateFormElemetsSortOrder(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, BATCHCOMPONENTSEARCHFORM, formElementsIds);

                sortOrderIsUpdate = false;
            }

            listFormElements[3] = new List<FormGroup.FormElement>();

            if (listResultCriteria[dataview.Tables[BATCHCOMPONENTTABLENAME].Id] != null)
                BuildDetailFormElements(listResultCriteria[dataview.Tables[BATCHCOMPONENTTABLENAME].Id], ref listFormElements[3], _batchComponentProperties, PropertyListType.BatchComponentPropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
            #endregion

            #region vw_structure

            queryFormElements.Clear();
            queryFormElements_ValRuleListUpdated.Clear();
            detailsFormElements.Clear();

            if (dataview.Tables[STRUCTURETABLENAME] != null)
            {
                BuildFormElementsListForSearch(dataview.Tables[STRUCTURETABLENAME], ref queryFormElements, _structureProperties, PropertyListType.StructurePropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
                BuildFormElementsListForSearch(formGroupBO.GetForm(formGroupBO.COEFormGroup.QueryForms, QUERYFORMINDEX, STRUCTURESEARCHFORM),
                    FormGroup.DisplayMode.All,
                    dataview.Tables[STRUCTURETABLENAME],
                    ref queryFormElements_ValRuleListUpdated,
                    _structureProperties, PropertyListType.StructurePropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
                if (resultCriteria[dataview.Tables[STRUCTURETABLENAME].Id] != null)
                    BuildDetailFormElements(resultCriteria[dataview.Tables[STRUCTURETABLENAME].Id], ref detailsFormElements, _structureProperties, PropertyListType.StructurePropertyList, PropertyList.NewPropertyList(), PropertyListType.None);
            }

            if (formGroupBO.COEFormGroup.QueryForms.Displays.Count > 0 && formGroupBO.COEFormGroup.QueryForms[0][STRUCTURETABLENAME] != null)
            {
                formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, STRUCTURESEARCHFORM, FormGroup.DisplayMode.All, formElementsIdToRemoveFromStructure);
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, STRUCTURESEARCHFORM, FormGroup.DisplayMode.All, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, queryFormElements);
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, STRUCTURESEARCHFORM, FormGroup.DisplayMode.All, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, queryFormElements_ValRuleListUpdated);
            }

            if (formGroupBO.COEFormGroup.DetailsForms.Displays.Count > 0 && formGroupBO.COEFormGroup.DetailsForms[0][STRUCTURETABLENAME] != null)
            {
                gridView = GetGridViewGridViewDetailsForms(formGroupBO.COEFormGroup.DetailsForms[0][STRUCTURETABLENAME].ViewMode[0], detailsFormElements, formElementsIdToRemoveFromStructure, "Table_" + dataview.Tables[STRUCTURETABLENAME].Id, "FilteredDataSet");
                gridToRemove.Add(formGroupBO.COEFormGroup.DetailsForms[0][STRUCTURETABLENAME].ViewMode[0].Id);
                formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, STRUCTURESEARCHFORM, FormGroup.DisplayMode.View, gridToRemove);
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, STRUCTURESEARCHFORM, FormGroup.DisplayMode.View, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, gridView);
            }
            foreach (Property prop in _structureProperties)
            {
                if (prop.SortOrderIsUpdate)
                    sortOrderIsUpdate = true;
            }

            if (sortOrderIsUpdate)
            {
                foreach (Property prop in _structureProperties.GetSortedPropertyList())
                {
                    formElementsIds.Add(prop.Name + "Property");
                }

                if (formGroupBO.COEFormGroup.QueryForms.Displays.Count > 0)
                    formGroupBO.UpdateFormElemetsSortOrder(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, STRUCTURESEARCHFORM, formElementsIds);

                sortOrderIsUpdate = false;

                formElementsIds.Clear();
            }

            listFormElements[4] = new List<FormGroup.FormElement>();

            if (listResultCriteria[dataview.Tables[STRUCTURETABLENAME].Id] != null)
                BuildDetailFormElements(listResultCriteria[dataview.Tables[STRUCTURETABLENAME].Id], ref listFormElements[4], _structureProperties, PropertyListType.StructurePropertyList, PropertyList.NewPropertyList(), PropertyListType.None);

            #endregion


            //Updating SearchPermanent ListForm for all property list

            List<FormGroup.FormElement> webGrid = null;
            List<string> webGridToRemove = new List<string>();
            if (formGroupBO.COEFormGroup.ListForms.Displays.Count > 0)
            {
                webGrid = GetWebGridListForms(formGroupBO.COEFormGroup.ListForms[0].Forms[0].LayoutInfo[0], listFormElements, formElementsIdToRemoveFromListForm, "Dataset");
                webGridToRemove.Add(formGroupBO.COEFormGroup.ListForms[0].Forms[0].LayoutInfo[0].Id);
                formGroupBO.RemoveFormElementsFromForm(FormGroup.CurrentFormEnum.ListForm, LISTFORMINDEX, 0, FormGroup.DisplayMode.All, webGridToRemove);
                formGroupBO.AddFormElementsToForm(FormGroup.CurrentFormEnum.ListForm, LISTFORMINDEX, 0, FormGroup.DisplayMode.All, COEFormBO.FormAddBehavior.DisallowDuplicateFormElements, webGrid);
            }
            //Fix for CSBR: 147743 - Clear validation rules from permanent search page
            formGroupBO.ClearValidationRulesFromSearch();
            _formGroupBO = formGroupBO.Save();
        }

        private COEDataView CreatePermanentDataViewAndResultCriteria(ref List<string> formElementsIdToRemoveFromMixture, ref List<string> formElementsIdToRemoveFromCompound, ref List<string> formElementsIdToRemoveFromStructure, ref List<string> formElementsIdToRemoveFromBatch, ref List<string> formElementsIdToRemoveFromBatchComponent, ref ResultsCriteria resultsCriteria, ref ResultsCriteria listResultsCriteria, int formId)
        {
            COEDataViewBO permDataView = COEDataViewBO.Get(formId);
            #region Removing vw_mixture fields

            if (permDataView.DataViewManager.Tables[MIXTURETABLENAME] != null)
            {
                foreach (ConfigurationProperty prop in _rootProperties.GetDeletedList())
                {
                    FieldBO field = permDataView.DataViewManager.Tables[MIXTURETABLENAME].Fields.GetField("REGDB", prop.Name);
                    if (field != null)
                    {
                        permDataView.DataViewManager.Tables[MIXTURETABLENAME].Fields.Remove(field);
                        formElementsIdToRemoveFromMixture.Add(prop.Name + "Property");
                    }

                    ResultsCriteria.Field criteriaToRemoveFromDetail = null;
                    if (resultsCriteria != null && resultsCriteria[permDataView.DataViewManager.Tables[MIXTURETABLENAME].ID] != null)
                    {
                        if (resultsCriteria[permDataView.DataViewManager.Tables[MIXTURETABLENAME].ID] != null)
                        {
                            foreach (ResultsCriteria.IResultsCriteriaBase criteria in resultsCriteria[permDataView.DataViewManager.Tables[MIXTURETABLENAME].ID].Criterias)
                            {
                                if (criteria is ResultsCriteria.Field && ((ResultsCriteria.Field)criteria).Alias == prop.Name)
                                {
                                    criteriaToRemoveFromDetail = (ResultsCriteria.Field)criteria;
                                    break;
                                }
                            }

                            if (criteriaToRemoveFromDetail != null)
                            {
                                foreach (ResultsCriteria.ResultsCriteriaTable table in resultsCriteria.Tables)
                                {
                                    if (table.Id == permDataView.DataViewManager.Tables[MIXTURETABLENAME].ID)
                                        table.Criterias.Remove(criteriaToRemoveFromDetail);
                                }
                            }
                        }
                    }

                    ResultsCriteria.Field criteriaToRemoveFromList = null;
                    if (listResultsCriteria != null && listResultsCriteria[permDataView.DataViewManager.Tables[MIXTURETABLENAME].ID] != null)
                    {
                        if (listResultsCriteria[permDataView.DataViewManager.Tables[MIXTURETABLENAME].ID] != null)
                        {
                            foreach (ResultsCriteria.IResultsCriteriaBase criteria in listResultsCriteria[permDataView.DataViewManager.Tables[MIXTURETABLENAME].ID].Criterias)
                            {
                                if (criteria is ResultsCriteria.Field && ((ResultsCriteria.Field)criteria).Alias == prop.Name)
                                {
                                    criteriaToRemoveFromList = (ResultsCriteria.Field)criteria;
                                    break;
                                }
                            }
                            if (criteriaToRemoveFromList != null)
                            {

                                foreach (ResultsCriteria.ResultsCriteriaTable table in listResultsCriteria.Tables)
                                {
                                    if (table.Id == permDataView.DataViewManager.Tables[MIXTURETABLENAME].ID)
                                        table.Criterias.Remove(criteriaToRemoveFromList);
                                }

                            }
                        }
                    }
                }
            }
            #endregion
            #region Removing vw_compound fields

            if (permDataView.DataViewManager.Tables[COMPOUNDTABLENAME] != null)
            {
                foreach (ConfigurationProperty prop in _compoundProperties.GetDeletedList())
                {
                    FieldBO field = permDataView.DataViewManager.Tables[COMPOUNDTABLENAME].Fields.GetField("REGDB", prop.Name);
                    if (field != null)
                    {
                        permDataView.DataViewManager.Tables[COMPOUNDTABLENAME].Fields.Remove(field);
                        formElementsIdToRemoveFromCompound.Add(prop.Name + "Property");
                    }

                    ResultsCriteria.Field criteriaToRemoveFromDetail = null;
                    if (resultsCriteria != null && resultsCriteria[permDataView.DataViewManager.Tables[COMPOUNDTABLENAME].ID] != null)
                    {
                        if (resultsCriteria[permDataView.DataViewManager.Tables[COMPOUNDTABLENAME].ID] != null)
                        {
                            foreach (ResultsCriteria.IResultsCriteriaBase criteria in resultsCriteria[permDataView.DataViewManager.Tables[COMPOUNDTABLENAME].ID].Criterias)
                            {
                                if (criteria is ResultsCriteria.Field && ((ResultsCriteria.Field)criteria).Alias == prop.Name)
                                {
                                    criteriaToRemoveFromDetail = (ResultsCriteria.Field)criteria;
                                    break;
                                }
                            }

                            if (criteriaToRemoveFromDetail != null)
                            {
                                foreach (ResultsCriteria.ResultsCriteriaTable table in resultsCriteria.Tables)
                                {
                                    if (table.Id == permDataView.DataViewManager.Tables[COMPOUNDTABLENAME].ID)
                                        table.Criterias.Remove(criteriaToRemoveFromDetail);
                                }
                            }
                        }
                    }
                    ResultsCriteria.Field criteriaToRemoveFromList = null;
                    if (listResultsCriteria != null && listResultsCriteria[permDataView.DataViewManager.Tables[COMPOUNDTABLENAME].ID] != null)
                    {
                        foreach (ResultsCriteria.IResultsCriteriaBase criteria in listResultsCriteria[permDataView.DataViewManager.Tables[COMPOUNDTABLENAME].ID].Criterias)
                        {
                            if (criteria is ResultsCriteria.Field && ((ResultsCriteria.Field)criteria).Alias == prop.Name)
                            {
                                criteriaToRemoveFromList = (ResultsCriteria.Field)criteria;
                                break;
                            }
                        }
                        if (criteriaToRemoveFromList != null)
                        {

                            foreach (ResultsCriteria.ResultsCriteriaTable table in listResultsCriteria.Tables)
                            {
                                if (table.Id == permDataView.DataViewManager.Tables[COMPOUNDTABLENAME].ID)
                                    table.Criterias.Remove(criteriaToRemoveFromList);
                            }

                        }
                    }
                }
            }
            #endregion
            #region Removing vw_structure fields

            if (permDataView.DataViewManager.Tables[STRUCTURETABLENAME] != null)
            {
                foreach (ConfigurationProperty prop in _structureProperties.GetDeletedList())
                {
                    FieldBO field = permDataView.DataViewManager.Tables[STRUCTURETABLENAME].Fields.GetField("REGDB", prop.Name);
                    if (field != null)
                    {
                        permDataView.DataViewManager.Tables[STRUCTURETABLENAME].Fields.Remove(field);
                        formElementsIdToRemoveFromStructure.Add(prop.Name + "Property");
                    }

                    ResultsCriteria.Field criteriaToRemoveFromDetail = null;
                    if (resultsCriteria != null && resultsCriteria[permDataView.DataViewManager.Tables[STRUCTURETABLENAME].ID] != null)
                    {
                        if (resultsCriteria[permDataView.DataViewManager.Tables[STRUCTURETABLENAME].ID] != null)
                        {
                            foreach (ResultsCriteria.IResultsCriteriaBase criteria in resultsCriteria[permDataView.DataViewManager.Tables[STRUCTURETABLENAME].ID].Criterias)
                            {
                                if (criteria is ResultsCriteria.Field && ((ResultsCriteria.Field)criteria).Alias == prop.Name)
                                {
                                    criteriaToRemoveFromDetail = (ResultsCriteria.Field)criteria;
                                    break;
                                }
                            }

                            if (criteriaToRemoveFromDetail != null)
                            {
                                foreach (ResultsCriteria.ResultsCriteriaTable table in resultsCriteria.Tables)
                                {
                                    if (table.Id == permDataView.DataViewManager.Tables[STRUCTURETABLENAME].ID)
                                        table.Criterias.Remove(criteriaToRemoveFromDetail);
                                }
                            }
                        }
                    }
                    ResultsCriteria.Field criteriaToRemoveFromList = null;
                    if (listResultsCriteria != null && listResultsCriteria[permDataView.DataViewManager.Tables[STRUCTURETABLENAME].ID] != null)
                    {
                        foreach (ResultsCriteria.IResultsCriteriaBase criteria in listResultsCriteria[permDataView.DataViewManager.Tables[STRUCTURETABLENAME].ID].Criterias)
                        {
                            if (criteria is ResultsCriteria.Field && ((ResultsCriteria.Field)criteria).Alias == prop.Name)
                            {
                                criteriaToRemoveFromList = (ResultsCriteria.Field)criteria;
                                break;
                            }
                        }
                        if (criteriaToRemoveFromList != null)
                        {

                            foreach (ResultsCriteria.ResultsCriteriaTable table in listResultsCriteria.Tables)
                            {
                                if (table.Id == permDataView.DataViewManager.Tables[STRUCTURETABLENAME].ID)
                                    table.Criterias.Remove(criteriaToRemoveFromList);
                            }

                        }
                    }
                }
            }
            #endregion
            #region Removing vw_batch fields

            if (permDataView.DataViewManager.Tables[BATCHTABLENAME] != null)
            {
                foreach (ConfigurationProperty prop in _batchProperties.GetDeletedList())
                {
                    FieldBO field = permDataView.DataViewManager.Tables[BATCHTABLENAME].Fields.GetField("REGDB", prop.Name);
                    if (field != null)
                    {
                        permDataView.DataViewManager.Tables[BATCHTABLENAME].Fields.Remove(field);
                        formElementsIdToRemoveFromBatch.Add(prop.Name + "Property");
                    }

                    ResultsCriteria.Field criteriaToRemoveFromDetail = null;
                    if (resultsCriteria != null && resultsCriteria[permDataView.DataViewManager.Tables[BATCHTABLENAME].ID] != null)
                    {
                        foreach (ResultsCriteria.IResultsCriteriaBase criteria in resultsCriteria[permDataView.DataViewManager.Tables[BATCHTABLENAME].ID].Criterias)
                        {
                            if (criteria is ResultsCriteria.Field && ((ResultsCriteria.Field)criteria).Alias == prop.Name)
                            {
                                criteriaToRemoveFromDetail = (ResultsCriteria.Field)criteria;
                                break;
                            }
                        }

                        if (criteriaToRemoveFromDetail != null)
                        {
                            foreach (ResultsCriteria.ResultsCriteriaTable table in resultsCriteria.Tables)
                            {
                                if (table.Id == permDataView.DataViewManager.Tables[BATCHTABLENAME].ID)
                                    table.Criterias.Remove(criteriaToRemoveFromDetail);
                            }
                        }
                    }
                    ResultsCriteria.Field criteriaToRemoveFromList = null;
                    if (listResultsCriteria != null && listResultsCriteria[permDataView.DataViewManager.Tables[BATCHTABLENAME].ID] != null)
                    {
                        foreach (ResultsCriteria.IResultsCriteriaBase criteria in listResultsCriteria[permDataView.DataViewManager.Tables[BATCHTABLENAME].ID].Criterias)
                        {
                            if (criteria is ResultsCriteria.Field && ((ResultsCriteria.Field)criteria).Alias == prop.Name)
                            {
                                criteriaToRemoveFromList = (ResultsCriteria.Field)criteria;
                                break;
                            }
                        }

                        if (criteriaToRemoveFromList != null)
                        {

                            foreach (ResultsCriteria.ResultsCriteriaTable table in listResultsCriteria.Tables)
                            {
                                if (table.Id == permDataView.DataViewManager.Tables[BATCHTABLENAME].ID)
                                    table.Criterias.Remove(criteriaToRemoveFromList);
                            }

                        }
                    }
                }
            }
            #endregion
            #region Removing vw_batchcomponent fields

            if (permDataView.DataViewManager.Tables[BATCHCOMPONENTTABLENAME] != null)
            {
                foreach (ConfigurationProperty prop in _batchComponentProperties.GetDeletedList())
                {
                    FieldBO field = permDataView.DataViewManager.Tables[BATCHCOMPONENTTABLENAME].Fields.GetField("REGDB", prop.Name);
                    if (field != null)
                    {
                        permDataView.DataViewManager.Tables[BATCHCOMPONENTTABLENAME].Fields.Remove(field);
                        formElementsIdToRemoveFromBatchComponent.Add(prop.Name + "Property");
                    }

                    ResultsCriteria.Field criteriaToRemoveFromDetail = null;
                    if (resultsCriteria != null && resultsCriteria[permDataView.DataViewManager.Tables[BATCHCOMPONENTTABLENAME].ID] != null)
                    {
                        foreach (ResultsCriteria.IResultsCriteriaBase criteria in resultsCriteria[permDataView.DataViewManager.Tables[BATCHCOMPONENTTABLENAME].ID].Criterias)
                        {
                            if (criteria is ResultsCriteria.Field && ((ResultsCriteria.Field)criteria).Alias == prop.Name)
                            {
                                criteriaToRemoveFromDetail = (ResultsCriteria.Field)criteria;
                                break;
                            }
                        }

                        if (criteriaToRemoveFromDetail != null)
                        {
                            foreach (ResultsCriteria.ResultsCriteriaTable table in resultsCriteria.Tables)
                            {
                                if (table.Id == permDataView.DataViewManager.Tables[BATCHCOMPONENTTABLENAME].ID)
                                    table.Criterias.Remove(criteriaToRemoveFromDetail);
                            }
                        }
                    }
                    ResultsCriteria.Field criteriaToRemoveFromList = null;
                    if (listResultsCriteria != null && listResultsCriteria[permDataView.DataViewManager.Tables[BATCHCOMPONENTTABLENAME].ID] != null)
                    {
                        foreach (ResultsCriteria.IResultsCriteriaBase criteria in listResultsCriteria[permDataView.DataViewManager.Tables[BATCHCOMPONENTTABLENAME].ID].Criterias)
                        {
                            if (criteria is ResultsCriteria.Field && ((ResultsCriteria.Field)criteria).Alias == prop.Name)
                            {
                                criteriaToRemoveFromList = (ResultsCriteria.Field)criteria;
                                break;
                            }
                        }
                        if (criteriaToRemoveFromList != null)
                        {

                            foreach (ResultsCriteria.ResultsCriteriaTable table in listResultsCriteria.Tables)
                            {
                                if (table.Id == permDataView.DataViewManager.Tables[BATCHCOMPONENTTABLENAME].ID)
                                    table.Criterias.Remove(criteriaToRemoveFromList);
                            }

                        }
                    }
                }
            }
            #endregion
            #region Adding vw_mixture fields

            if (permDataView.DataViewManager.Tables[MIXTURETABLENAME] != null)
            {
                foreach (ConfigurationProperty prop in _rootProperties)
                {
                    if (permDataView.DataViewManager.Tables[MIXTURETABLENAME].Fields.GetField("REGDB", prop.Name) == null)
                    {
                        COEDataView.Field dvField = new COEDataView.Field();
                        dvField.DataType = this.ConvertToAbstractType(prop.Type);
                        dvField.Name = prop.Name;
                        //dvField.Id = (permDataView.DataViewManager.Tables[MIXTURETABLENAME].Fields.HighestID + fieldsAdded++);
                        dvField.Id = this.GetCustomizeHighestFieldId(permDataView, MIXTURETABLENAME);
                        permDataView.DataViewManager.Tables[MIXTURETABLENAME].Fields.Add(FieldBO.NewField(dvField, "REGDB"));
                        if (resultsCriteria[permDataView.DataViewManager.Tables[MIXTURETABLENAME].ID] != null &&
                            !ExistsResultCriteria(resultsCriteria[permDataView.DataViewManager.Tables[MIXTURETABLENAME].ID], prop.Name))
                        {
                            ResultsCriteria.Field result = new ResultsCriteria.Field(dvField.Id);
                            result.Alias = prop.Name;
                            if (resultsCriteria != null && resultsCriteria.Tables != null)
                            {
                                foreach (ResultsCriteria.ResultsCriteriaTable table in resultsCriteria.Tables)
                                {
                                    if (table.Id == permDataView.DataViewManager.Tables[MIXTURETABLENAME].ID)
                                        table.Criterias.Add(result);
                                }
                            }
                        }
                        if (listResultsCriteria[permDataView.DataViewManager.Tables[MIXTURETABLENAME].ID] != null &&
                            !ExistsResultCriteria(listResultsCriteria[permDataView.DataViewManager.Tables[MIXTURETABLENAME].ID], prop.Name))
                        {
                            ResultsCriteria.Field listResult = new ResultsCriteria.Field(dvField.Id);
                            listResult.Alias = prop.Name;
                            if (listResultsCriteria != null && listResultsCriteria.Tables != null)
                            {
                                foreach (ResultsCriteria.ResultsCriteriaTable table in listResultsCriteria.Tables)
                                {
                                    if (table.Id == permDataView.DataViewManager.Tables[MIXTURETABLENAME].ID)
                                        table.Criterias.Add(listResult);
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region Adding vw_compound fields

            if (permDataView.DataViewManager.Tables[COMPOUNDTABLENAME] != null)
            {
                foreach (ConfigurationProperty prop in _compoundProperties)
                {
                    if (permDataView.DataViewManager.Tables[COMPOUNDTABLENAME].Fields.GetField("REGDB", prop.Name) == null)
                    {
                        COEDataView.Field dvField = new COEDataView.Field();
                        dvField.DataType = this.ConvertToAbstractType(prop.Type);
                        dvField.Name = prop.Name;
                        //dvField.Id = (permDataView.DataViewManager.Tables[COMPOUNDTABLENAME].Fields.HighestID + fieldsAdded++);
                        dvField.Id = this.GetCustomizeHighestFieldId(permDataView, COMPOUNDTABLENAME);
                        permDataView.DataViewManager.Tables[COMPOUNDTABLENAME].Fields.Add(FieldBO.NewField(dvField, "REGDB"));

                        if (resultsCriteria[permDataView.DataViewManager.Tables[COMPOUNDTABLENAME].ID] != null &&
                            !ExistsResultCriteria(resultsCriteria[permDataView.DataViewManager.Tables[COMPOUNDTABLENAME].ID], prop.Name))
                        {
                            ResultsCriteria.Field result = new ResultsCriteria.Field(dvField.Id);
                            result.Alias = prop.Name;
                            if (resultsCriteria != null && resultsCriteria.Tables != null)
                            {
                                foreach (ResultsCriteria.ResultsCriteriaTable table in resultsCriteria.Tables)
                                {
                                    if (table.Id == permDataView.DataViewManager.Tables[COMPOUNDTABLENAME].ID)
                                        table.Criterias.Add(result);
                                }
                            }
                        }
                        if (listResultsCriteria[permDataView.DataViewManager.Tables[COMPOUNDTABLENAME].ID] != null &&
                      !ExistsResultCriteria(listResultsCriteria[permDataView.DataViewManager.Tables[COMPOUNDTABLENAME].ID], prop.Name))
                        {
                            ResultsCriteria.Field listResult = new ResultsCriteria.Field(dvField.Id);
                            listResult.Alias = prop.Name;
                            if (listResultsCriteria != null && listResultsCriteria.Tables != null)
                            {
                                foreach (ResultsCriteria.ResultsCriteriaTable table in listResultsCriteria.Tables)
                                {
                                    if (table.Id == permDataView.DataViewManager.Tables[COMPOUNDTABLENAME].ID)
                                        table.Criterias.Add(listResult);
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region Adding vw_structure fields

            if (permDataView.DataViewManager.Tables[STRUCTURETABLENAME] != null)
            {
                foreach (ConfigurationProperty prop in _structureProperties)
                {
                    if (permDataView.DataViewManager.Tables[STRUCTURETABLENAME].Fields.GetField("REGDB", prop.Name) == null)
                    {
                        COEDataView.Field dvField = new COEDataView.Field();
                        dvField.DataType = this.ConvertToAbstractType(prop.Type);
                        dvField.Name = prop.Name;
                        //dvField.Id = (permDataView.DataViewManager.Tables[STRUCTURETABLENAME].Fields.HighestID + fieldsAdded++);
                        dvField.Id = this.GetCustomizeHighestFieldId(permDataView, STRUCTURETABLENAME);
                        permDataView.DataViewManager.Tables[STRUCTURETABLENAME].Fields.Add(FieldBO.NewField(dvField, "REGDB"));

                        if (resultsCriteria[permDataView.DataViewManager.Tables[STRUCTURETABLENAME].ID] != null &&
                            !ExistsResultCriteria(resultsCriteria[permDataView.DataViewManager.Tables[STRUCTURETABLENAME].ID], prop.Name))
                        {
                            ResultsCriteria.Field result = new ResultsCriteria.Field(dvField.Id);
                            result.Alias = prop.Name;
                            if (resultsCriteria != null && resultsCriteria.Tables != null)
                            {
                                foreach (ResultsCriteria.ResultsCriteriaTable table in resultsCriteria.Tables)
                                {
                                    if (table.Id == permDataView.DataViewManager.Tables[STRUCTURETABLENAME].ID)
                                        table.Criterias.Add(result);
                                }
                            }
                        }
                        if (listResultsCriteria[permDataView.DataViewManager.Tables[STRUCTURETABLENAME].ID] != null &&
                      !ExistsResultCriteria(listResultsCriteria[permDataView.DataViewManager.Tables[STRUCTURETABLENAME].ID], prop.Name))
                        {
                            ResultsCriteria.Field listResult = new ResultsCriteria.Field(dvField.Id);
                            listResult.Alias = prop.Name;
                            if (listResultsCriteria != null && listResultsCriteria.Tables != null)
                            {
                                foreach (ResultsCriteria.ResultsCriteriaTable table in listResultsCriteria.Tables)
                                {
                                    if (table.Id == permDataView.DataViewManager.Tables[STRUCTURETABLENAME].ID)
                                        table.Criterias.Add(listResult);
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region Adding vw_batch fields

            if (permDataView.DataViewManager.Tables[BATCHTABLENAME] != null)
            {
                foreach (ConfigurationProperty prop in _batchProperties)
                {
                    if (permDataView.DataViewManager.Tables[BATCHTABLENAME].Fields.GetField("REGDB", prop.Name) == null)
                    {
                        COEDataView.Field dvField = new COEDataView.Field();
                        dvField.DataType = this.ConvertToAbstractType(prop.Type);
                        dvField.Name = prop.Name;
                        //dvField.Id = (permDataView.DataViewManager.Tables[BATCHTABLENAME].Fields.HighestID + fieldsAdded++);
                        dvField.Id = this.GetCustomizeHighestFieldId(permDataView, BATCHTABLENAME);
                        permDataView.DataViewManager.Tables[BATCHTABLENAME].Fields.Add(FieldBO.NewField(dvField, "REGDB"));
                        if (resultsCriteria[permDataView.DataViewManager.Tables[BATCHTABLENAME].ID] != null &&
                            !ExistsResultCriteria(resultsCriteria[permDataView.DataViewManager.Tables[BATCHTABLENAME].ID], prop.Name))
                        {
                            ResultsCriteria.Field result = new ResultsCriteria.Field(dvField.Id);
                            result.Alias = prop.Name;
                            if (resultsCriteria != null && resultsCriteria.Tables != null)
                            {
                                foreach (ResultsCriteria.ResultsCriteriaTable table in resultsCriteria.Tables)
                                {
                                    if (table.Id == permDataView.DataViewManager.Tables[BATCHTABLENAME].ID)
                                        table.Criterias.Add(result);
                                }
                            }
                        }
                        if (listResultsCriteria[permDataView.DataViewManager.Tables[BATCHTABLENAME].ID] != null &&
                           !ExistsResultCriteria(resultsCriteria[permDataView.DataViewManager.Tables[BATCHTABLENAME].ID], prop.Name))
                        {
                            ResultsCriteria.Field listResult = new ResultsCriteria.Field(dvField.Id);
                            listResult.Alias = prop.Name;
                            if (listResultsCriteria != null && listResultsCriteria.Tables != null)
                            {
                                foreach (ResultsCriteria.ResultsCriteriaTable table in listResultsCriteria.Tables)
                                {
                                    if (table.Id == permDataView.DataViewManager.Tables[BATCHTABLENAME].ID)
                                        table.Criterias.Add(listResult);
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region Adding vw_batchcomponent fields

            if (permDataView.DataViewManager.Tables[BATCHCOMPONENTTABLENAME] != null)
            {
                foreach (ConfigurationProperty prop in _batchComponentProperties)
                {
                    if (permDataView.DataViewManager.Tables[BATCHCOMPONENTTABLENAME].Fields.GetField("REGDB", prop.Name) == null)
                    {
                        COEDataView.Field dvField = new COEDataView.Field();
                        dvField.DataType = this.ConvertToAbstractType(prop.Type);
                        dvField.Name = prop.Name;
                        //dvField.Id = (permDataView.DataViewManager.Tables[BATCHCOMPONENTTABLENAME].Fields.HighestID + fieldsAdded++);
                        dvField.Id = this.GetCustomizeHighestFieldId(permDataView, BATCHCOMPONENTTABLENAME);
                        permDataView.DataViewManager.Tables[BATCHCOMPONENTTABLENAME].Fields.Add(FieldBO.NewField(dvField, "REGDB"));
                        if (resultsCriteria[permDataView.DataViewManager.Tables[BATCHCOMPONENTTABLENAME].ID] != null &&
                            !ExistsResultCriteria(resultsCriteria[permDataView.DataViewManager.Tables[BATCHCOMPONENTTABLENAME].ID], prop.Name))
                        {
                            ResultsCriteria.Field result = new ResultsCriteria.Field(dvField.Id);
                            result.Alias = prop.Name;
                            if (resultsCriteria != null && resultsCriteria.Tables != null)
                            {
                                foreach (ResultsCriteria.ResultsCriteriaTable table in resultsCriteria.Tables)
                                {
                                    if (table.Id == permDataView.DataViewManager.Tables[BATCHCOMPONENTTABLENAME].ID)
                                        table.Criterias.Add(result);
                                }
                            }
                        }
                        if (listResultsCriteria[permDataView.DataViewManager.Tables[BATCHCOMPONENTTABLENAME].ID] != null &&
                           !ExistsResultCriteria(listResultsCriteria[permDataView.DataViewManager.Tables[BATCHCOMPONENTTABLENAME].ID], prop.Name))
                        {
                            ResultsCriteria.Field listResult = new ResultsCriteria.Field(dvField.Id);
                            listResult.Alias = prop.Name;
                            if (listResultsCriteria != null && listResultsCriteria.Tables != null)
                            {
                                foreach (ResultsCriteria.ResultsCriteriaTable table in listResultsCriteria.Tables)
                                {
                                    if (table.Id == permDataView.DataViewManager.Tables[BATCHCOMPONENTTABLENAME].ID)
                                        table.Criterias.Add(listResult);
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            return permDataView.SaveFromDataViewManager().COEDataView;
        }

        private COEDataView.AbstractTypes ConvertToAbstractType(string dataType)
        {
            if (dataType.ToLower().Contains("text") || dataType.ToLower().Contains("url"))
            {
                return COEDataView.AbstractTypes.Text;
            }
            else if (dataType.ToLower().Contains("number"))
            {
                if (dataType.Substring(dataType.Length - 3, 1) != "0")
                    return COEDataView.AbstractTypes.Real;
                else
                    return COEDataView.AbstractTypes.Integer;
            }
            else if (dataType.ToLower().Contains("bool"))
            {
                return COEDataView.AbstractTypes.Boolean;
            }
            else if (dataType.ToLower().Contains("date"))
            {
                return COEDataView.AbstractTypes.Date;
            }
            return COEDataView.AbstractTypes.Text;
        }

        /// <summary>
        /// Get a copy of all SearchCriteriaItem in the Display
        /// </summary>
        /// <param name="display">a Display from which to fetch the SearchCriteriaItem</param>
        /// <returns>a SearchCriteria object that contains all fetched SearchCriteriaItem</returns>
        private SearchCriteria GetSearchCriteria(FormGroup.Display display)
        {
            SearchCriteria criteria = new SearchCriteria();

            foreach (FormGroup.Form form in display.Forms)
            {
                foreach (FormGroup.FormElement element in form.LayoutInfo)
                {
                    if (element.SearchCriteriaItem != null)
                    {
                        SearchCriteria.SearchCriteriaItem newItem = SearchCriteria.SearchCriteriaItem.GetSearchCriteriaItem(CambridgeSoft.COE.Framework.Common.Utilities.XmlSerialize(element.SearchCriteriaItem));
                        newItem.Modifier = element.BindingExpression;
                        criteria.Items.Add(newItem);
                    }
                }
            }

            return criteria;
        }

        // returns the type of the property
        private string GetPropertyType(string propertyName, PropertyListType propertyListType)
        {
            string propertyType = string.Empty;           

            switch (propertyListType)
            {
                case PropertyListType.MixturePropertyList:
                    foreach (Property prop in _rootProperties)
                    {
                        if (prop.Name == propertyName)
                        {
                            propertyType = prop.Type;
                            
                        }
                    }
                    break;

                case PropertyListType.BatchPropertyList:
                    foreach (Property prop in _batchProperties)
                    {
                        if (prop.Name == propertyName)
                        {
                            propertyType = prop.Type;
                          
                        }
                    }
                    break;

                case PropertyListType.BatchComponentPropertyList:
                    foreach (Property prop in _batchComponentProperties)
                    {
                        if (prop.Name == propertyName)
                        {
                            propertyType = prop.Type;
                           
                        }
                    }
                    break;

                case PropertyListType.CompoundPropertyList:
                    foreach (Property prop in _compoundProperties)
                    {
                        if (prop.Name == propertyName)
                        {
                            propertyType = prop.Type;
                            
                        }
                    }
                    break;
                case PropertyListType.StructurePropertyList:
                    foreach (Property prop in _structureProperties)
                    {
                        if (prop.Name == propertyName)
                        {
                            propertyType = prop.Type;
                            
                        }
                    }
                    break;
            }
            return propertyType;

        }

        /// <summary>
        /// Get a server control type for UI display
        /// </summary>
        /// <param name="propertyName">name of a ConfigurationProperty instance</param>
        /// <param name="propertyListType">Identify where to find the ConfigurationProperty</param>
        /// <param name="displayMode">How to display this custom property</param>
        /// <returns>type of server control used to display this custom property</returns>
        private string GetDefaultControlType(string propertyName, PropertyListType propertyListType, FormGroup.DisplayMode displayMode)
        {
            string controlType = string.Empty;
            string propertyType = string.Empty;
            string propertySubType = string.Empty;

            switch (propertyListType)
            {
                case PropertyListType.MixturePropertyList:
                    foreach (Property prop in _rootProperties)
                    {
                        if (prop.Name == propertyName)
                        {
                            propertyType = prop.Type;
                            if (!string.IsNullOrEmpty(prop.SubType)) propertySubType = prop.SubType;
                        }
                    }
                    break;

                case PropertyListType.BatchPropertyList:
                    foreach (Property prop in _batchProperties)
                    {
                        if (prop.Name == propertyName)
                        {
                            propertyType = prop.Type;
                            if (!string.IsNullOrEmpty(prop.SubType)) propertySubType = prop.SubType;
                        }
                    }
                    break;

                case PropertyListType.BatchComponentPropertyList:
                    foreach (Property prop in _batchComponentProperties)
                    {
                        if (prop.Name == propertyName)
                        {
                            propertyType = prop.Type;
                            if (!string.IsNullOrEmpty(prop.SubType)) propertySubType = prop.SubType;
                        }
                    }
                    break;

                case PropertyListType.CompoundPropertyList:
                    foreach (Property prop in _compoundProperties)
                    {
                        if (prop.Name == propertyName)
                        {
                            propertyType = prop.Type;
                            if (!string.IsNullOrEmpty(prop.SubType)) propertySubType = prop.SubType;
                        }
                    }
                    break;
                case PropertyListType.StructurePropertyList:
                    foreach (Property prop in _structureProperties)
                    {
                        if (prop.Name == propertyName)
                        {
                            propertyType = prop.Type;
                            if (!string.IsNullOrEmpty(prop.SubType)) propertySubType = prop.SubType;
                        }
                    }
                    break;
            }

            if (propertyType != string.Empty)
            {

                switch (propertyType)
                {
                    case "NUMBER":
                    case "TEXT":
                        switch (displayMode)
                        {
                            case FormGroup.DisplayMode.Add:
                            case FormGroup.DisplayMode.Edit:
                            case FormGroup.DisplayMode.All:
                                if (!string.IsNullOrEmpty(propertySubType))
                                {
                                   controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBox";
                                }
                                else
                                {
                                    if(propertyType == "NUMBER")
                                        controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COENumericTextBox";
                                    else
                                        controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBox";
                                }
                                break;
                            case FormGroup.DisplayMode.View:
                                if (!string.IsNullOrEmpty(propertySubType) && propertySubType == "URL")
                                {
                                    if (propertySubType.ToUpper() == "URL")
                                        controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELink";
                                    else
                                        controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly";
                                }
                                else
                                {
                                    if (propertyType == "NUMBER")
                                        controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COENumericTextBox";
                                    else
                                        controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly";
                                }
                                break;
                        }
                        break;

                    case "DATE":
                        switch (displayMode)
                        {
                            case FormGroup.DisplayMode.Add:
                            case FormGroup.DisplayMode.Edit:
                            case FormGroup.DisplayMode.All:
                                controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePicker";
                                break;
                            case FormGroup.DisplayMode.View:
                                controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePickerReadOnly";
                                break;
                        }
                        break;

                    case "BOOLEAN":
                        switch (displayMode)
                        {
                            case FormGroup.DisplayMode.Add:
                            case FormGroup.DisplayMode.Edit:
                            case FormGroup.DisplayMode.All:
                                controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COECheckBox";
                                break;
                            case FormGroup.DisplayMode.View:
                                controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COECheckBoxReadOnly";
                                break;
                        }
                        break;
                    case "PICKLISTDOMAIN":
                        controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList";
                        break;

                }
            }
            return controlType;
        }

        /// <summary>
        /// Gets the COEFormID from configuration given a key
        /// </summary>
        /// <param name="formKey">COEForm key</param>
        /// <returns>Found coeform ID</returns>
        private int GetFormID(string formKey)
        {
            int retVal = 0;
            if (!string.IsNullOrEmpty(FrameworkUtils.GetAppConfigSetting("REGISTRATION", "CBV", formKey)))
                int.TryParse(FrameworkUtils.GetAppConfigSetting("REGISTRATION", "CBV", formKey), out retVal);
            return retVal;
        }

        /// <summary>
        /// Gets the COEFormID from configuration given a key
        /// </summary>
        /// <param name="formKey">COEForm key</param>
        /// <returns>Found coeform ID</returns>
        private int GetFormID(string formKey, string section)
        {
            int retVal = 0;
            if (!string.IsNullOrEmpty(FrameworkUtils.GetAppConfigSetting("REGISTRATION", section, formKey)))
                int.TryParse(FrameworkUtils.GetAppConfigSetting("REGISTRATION", section, formKey), out retVal);
            return retVal;
        }

        /// <summary>
        /// Update client validation rule if validation rule list of a property is changed.
        /// </summary>
        /// <param name="configurationPropertyList">a ConfigurationProperty to be checked whether validation rule list is changed</param>
        /// <param name="currentForm"></param>
        /// <param name="formIndex"></param>
        /// <param name="subFormIndex"></param>
        /// <param name="displayMode"></param>
        /// <param name="formGroupBO">FormGoup instance to be updated</param>
        private void UpdateFormElements(PropertyList configurationPropertyList, FormGroup.CurrentFormEnum currentForm, int formIndex, int subFormIndex, FormGroup.DisplayMode displayMode, ref COEFormBO formGroupBO)
        {
            FormGroup.Form formToModify = null;
            switch (currentForm)
            {
                case FormGroup.CurrentFormEnum.DetailForm:
                    formToModify = formGroupBO.GetForm(formGroupBO.COEFormGroup.DetailsForms, formIndex, subFormIndex);
                    break;
                case FormGroup.CurrentFormEnum.ListForm:
                    formToModify = formGroupBO.GetForm(formGroupBO.COEFormGroup.ListForms, formIndex, subFormIndex);
                    break;
                case FormGroup.CurrentFormEnum.QueryForm:
                    formToModify = formGroupBO.GetForm(formGroupBO.COEFormGroup.QueryForms, formIndex, subFormIndex);
                    break;
            }
            if (formToModify != null)
            {
                foreach (Property prop in configurationPropertyList)
                {
                    if (prop.ValRuleList.IsDirty && !prop.IsNew)
                    {
                        switch (displayMode)
                        {
                            case FormGroup.DisplayMode.Add:
                                for (int i = 0; i < formToModify.AddMode.Count; i++)
                                {
                                    if (formToModify.AddMode[i].Id == prop.Name + "Property")
                                    {
                                        formToModify.AddMode[i].ValidationRuleList.Clear();
                                        formToModify.AddMode[i].ValidationRuleList = this.ConvertToClientValidations(prop.ValRuleList);
                                        // RAG : CBOE-1423, It will set default value if requiredField validation rule added for this property
                                        formToModify.AddMode[i].DefaultValue = prop.DefaultValue;
                                    }
                                }
                                break;
                            case FormGroup.DisplayMode.Edit:
                                for (int i = 0; i < formToModify.EditMode.Count; i++)
                                {
                                    if (formToModify.EditMode[i].Id == prop.Name + "Property")
                                    {
                                        formToModify.EditMode[i].ValidationRuleList.Clear();
                                        formToModify.EditMode[i].ValidationRuleList = this.ConvertToClientValidations(prop.ValRuleList);
                                    }
                                }
                                break;
                            case FormGroup.DisplayMode.All:
                                for (int i = 0; i < formToModify.LayoutInfo.Count; i++)
                                {
                                    if (formToModify.LayoutInfo[i].Id == prop.Name + "Property")
                                    {
                                        formToModify.LayoutInfo[i].ValidationRuleList.Clear();
                                        formToModify.LayoutInfo[i].ValidationRuleList = this.ConvertToClientValidations(prop.ValRuleList);
                                    }
                                }
                                break;
                        }
                    }
                }
                formGroupBO.SetForm(currentForm, formIndex, subFormIndex, formToModify);
            }
        }

        private int GetCustomizeHighestFieldId(COEDataViewBO dataview, string tableName)
        {
            int highestId = dataview.DataViewManager.Tables[tableName].GetHighestIdField();

            int tableId = dataview.DataViewManager.Tables[tableName].ID;

            int tableMinFieldId = int.Parse(tableId.ToString() + RESERVEDAMOUNTCUSTOMDTFIELDID.ToString());

            if (highestId < tableMinFieldId)
                return tableMinFieldId;
            else
                return highestId + 1;
        }

        internal string ExportSearchPermForm()
        {
            return COEFormBO.Get(this.SEARCHPERMFORMID).COEFormGroup.ToString();
        }

        internal string ExportSearchTempForm()
        {
            return COEFormBO.Get(this.SEARCHTEMPFORMID).COEFormGroup.ToString();
        }

        internal string ExportViewMixtureForm()
        {
            return COEFormBO.Get(this.VIEWMIXTUREFORMID).COEFormGroup.ToString();
        }

        internal string ExportReviewMixtureForm()
        {
            return COEFormBO.Get(this.REGISTERMIXTUREFORMID).COEFormGroup.ToString();
        }

        internal string ExportSubmitMixtureForm()
        {
            return COEFormBO.Get(this.SUBMITMIXTUREFORMID).COEFormGroup.ToString();
        }

        internal string ExportELNSearchTempForm()
        {
            return COEFormBO.Get(this.ELNSEARCHTEMPFORMID).COEFormGroup.ToString();
        }

        internal string ExportELNSearchPermForm()
        {
            return COEFormBO.Get(this.ELNSEARCHPERMFORMID).COEFormGroup.ToString();
        }

        internal string ExportDataLoaderForm()
        {
            return COEFormBO.Get(this.DATALOADERFORMID).COEFormGroup.ToString();
        }

        internal string ExportComponentDuplicatesForm()
        {
            return COEFormBO.Get(this.COMPONENTDUPLICATESFORMID).COEFormGroup.ToString();
        }

        internal string ExportRegistryDuplicatesForm()
        {
            return COEFormBO.Get(this.REGISTRYDUPLICATESFORMID).COEFormGroup.ToString();

        }

        internal string ExportSendToRegistrationForm()
        {
            return COEFormBO.Get(this.SENDTOREGISTRATIONFORMID).COEFormGroup.ToString();
        }

        internal string ExportDeleteLogFormGroupId()
        {
            return COEFormBO.Get(this.DELETELOGFORMID).COEFormGroup.ToString();
        }

        internal string ExportSearchComponentsToAddForm()
        {
            return COEFormBO.Get(this.SEARCHCOMPONENTSTOADDFORMID).COEFormGroup.ToString();
        }

        internal string ExportSearchComponentsToAddRRForm()
        {
            return COEFormBO.Get(this.SEARCHCOMPONENTSTOADDRRFORMID).COEFormGroup.ToString();
        }

        /// <summary>
        /// Build a ConfigInfo XmlNode to be added to the input FormElement
        /// </summary>
        /// <param name="formElement">A FormElement for which the ConfigInfo node is build</param>
        /// <param name="configurationProperty">ConfigurationProperty instance corresponding to the inpurt FormElement</param>
        /// <param name="mode">Where the FormElement is to be display</param>
        /// <param name="propertyListType">type of ConfigurationProperty</param>
        /// <returns>a ConfigInfo XmlNode to be added to the input FormElement</returns>
        private XmlNode BuildConfigInfo(FormGroup.FormElement formElement, Property configurationProperty, FormGroup.DisplayMode mode, PropertyListType propertyListType)
        {
            XmlDocument configInfo = new XmlDocument();
            string xmlns = "COE.FormGroup";
            string xmlprefix = "COE";
            configInfo.AppendChild(configInfo.CreateNode(XmlNodeType.Element, xmlprefix, "configInfo", xmlns));
            configInfo.FirstChild.AppendChild(configInfo.CreateNode(XmlNodeType.Element, xmlprefix, "fieldConfig", xmlns));
            configInfo.FirstChild.FirstChild.AppendChild(configInfo.CreateNode(XmlNodeType.Element, xmlprefix, "CSSLabelClass", xmlns)).InnerText = "FELabel";


            
            if (formElement.DisplayInfo.Type == "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COECheckBox")
            {
                //Distinguish query forms from other forms
                if (formElement.SearchCriteriaItem != null)
                {
                    configInfo.FirstChild.FirstChild.AppendChild(configInfo.CreateNode(XmlNodeType.Element, xmlprefix, "FalseString", xmlns)).InnerText = "null";
                }
            }

            string defaultControlStyle = RegAdminUtils.GetDefaultControlStyle(formElement.DisplayInfo.Type, mode);
            if (!string.IsNullOrEmpty(defaultControlStyle))
                configInfo.FirstChild.FirstChild.AppendChild(configInfo.CreateNode(XmlNodeType.Element, xmlprefix, "CSSClass", xmlns)).InnerText = defaultControlStyle;
			// CBOE-1963 Make sure the list view of 4003.xml file has width of 200px for the controls, Inorder to match with header
            if(FormGroup.Id==SEARCHPERMFORMID && mode==FormGroup.DisplayMode.View)
                configInfo.FirstChild.FirstChild.AppendChild(configInfo.CreateNode(XmlNodeType.Element, xmlprefix, "width", xmlns)).InnerText = "200px";
           
            if (formElement.DisplayInfo.Type == "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList")
            {
                string pickListDomainId = configurationProperty.PickListDomainId;
                if (configurationProperty.PickListDomainId == null)
                {
                    switch (propertyListType)
                    {
                        case PropertyListType.MixturePropertyList:
                            if (_rootProperties.CheckExistingNames(configurationProperty.Name, false))
                                pickListDomainId = _rootProperties[configurationProperty.Name].PickListDomainId;
                            if (pickListDomainId == null)
                            {
                                if (_batchProperties.CheckExistingNames(configurationProperty.Name, false))
                                    pickListDomainId = _batchProperties[configurationProperty.Name].PickListDomainId;
                            }
                            break;
                        case PropertyListType.CompoundPropertyList:
                            if (_compoundProperties.CheckExistingNames(configurationProperty.Name, false))
                                pickListDomainId = _compoundProperties[configurationProperty.Name].PickListDomainId;
                            if (pickListDomainId == null)
                            {
                                if (_batchProperties.CheckExistingNames(configurationProperty.Name, false))
                                    pickListDomainId = _batchComponentProperties[configurationProperty.Name].PickListDomainId;
                            }
                            break;
                        case PropertyListType.StructurePropertyList:
                            if (_structureProperties.CheckExistingNames(configurationProperty.Name, false))
                                pickListDomainId = _structureProperties[configurationProperty.Name].PickListDomainId;
                            if (pickListDomainId == null)
                            {
                                //TODO:where to get PickListDomainId if structure level structure property doest not have one
                            }
                            break;
                        case PropertyListType.BatchPropertyList:
                            if (_batchProperties.CheckExistingNames(configurationProperty.Name, false))
                                pickListDomainId = _batchProperties[configurationProperty.Name].PickListDomainId;
                            if (pickListDomainId == null)
                            {
                                if (_batchProperties.CheckExistingNames(configurationProperty.Name, false))
                                    pickListDomainId = _rootProperties[configurationProperty.Name].PickListDomainId;
                            }
                            break;
                        case PropertyListType.BatchComponentPropertyList:
                            if (_batchComponentProperties.CheckExistingNames(configurationProperty.Name, false))
                                pickListDomainId = _batchComponentProperties[configurationProperty.Name].PickListDomainId;
                            if (pickListDomainId == null)
                            {
                                if (_batchProperties.CheckExistingNames(configurationProperty.Name, false))
                                    pickListDomainId = _compoundProperties[configurationProperty.Name].PickListDomainId;
                            }
                            break;
                    }
                }

                #region old code for picklistdomain
                //if (pickListDomainId != null)
                //    configInfo.FirstChild.FirstChild.AppendChild(configInfo.CreateNode(XmlNodeType.Element, xmlprefix, "dropDownItemsSelect", xmlns)).InnerText = HttpUtility.HtmlDecode(this.GetSqlClauseFromPickListDomainId(pickListDomainId));
                #endregion

                
                if (pickListDomainId != null)
                {
                    configInfo.FirstChild.FirstChild.AppendChild(configInfo.CreateNode(XmlNodeType.Element, xmlprefix, "dropDownItemsSelect", xmlns)).InnerText = string.Empty;
                    configInfo.FirstChild.FirstChild.AppendChild(configInfo.CreateNode(XmlNodeType.Element, xmlprefix, "PickListDomain", xmlns)).InnerText = pickListDomainId;
                }

                if (mode == FormGroup.DisplayMode.View)
                    configInfo.FirstChild.FirstChild.AppendChild(configInfo.CreateNode(XmlNodeType.Element, xmlprefix, "Enable", xmlns)).InnerText = bool.FalseString;
                else
                    configInfo.FirstChild.FirstChild.AppendChild(configInfo.CreateNode(XmlNodeType.Element, xmlprefix, "Enable", xmlns)).InnerText = bool.TrueString;
            }
            else if (formElement.DisplayInfo.Type == "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COENumericTextBox")
            {
               if (mode == FormGroup.DisplayMode.View)
                {
                    configInfo.FirstChild.FirstChild.AppendChild(configInfo.CreateNode(XmlNodeType.Element, xmlprefix, "ReadOnly", xmlns)).InnerText = bool.TrueString;
                }
                else
                {
                    configInfo.FirstChild.FirstChild.AppendChild(configInfo.CreateNode(XmlNodeType.Element, xmlprefix, "ReadOnly", xmlns)).InnerText = bool.FalseString;
                }
            }
            else if (formElement.DisplayInfo.Type == "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELink")
            {
                if (mode == FormGroup.DisplayMode.View)
                    configInfo.FirstChild.FirstChild.AppendChild(configInfo.CreateNode(XmlNodeType.Element, xmlprefix, "Target", xmlns)).InnerText = RegAdminUtils.GetLinkTarget;
            }

            return configInfo.FirstChild;
        }

        private string GetSqlClauseFromPickListDomainId(string pickListDomainId)
        {
            PicklistDomain pickListDomain = PicklistDomain.GetPicklistDomain(int.Parse(pickListDomainId));
            return pickListDomain.PickListDomainSql;
        }

        private string GetFormElementLabel(PropertyListType propertyListType, Property configurationProperty)
        {
            string label = string.Empty;
            switch (propertyListType)
            {
                case PropertyListType.MixturePropertyList:
                    if (_formElementsLabels[0].Count > 0)
                        label = _formElementsLabels[0][configurationProperty.Name];
                    break;
                case PropertyListType.CompoundPropertyList:
                    if (_formElementsLabels[1].Count > 0)
                        label = _formElementsLabels[1][configurationProperty.Name];
                    break;
                case PropertyListType.BatchPropertyList:
                    if (_formElementsLabels[2].Count > 0)
                        label = _formElementsLabels[2][configurationProperty.Name];
                    break;
                case PropertyListType.BatchComponentPropertyList:
                    if (_formElementsLabels[3].Count > 0)
                        label = _formElementsLabels[3][configurationProperty.Name];
                    break;
                case PropertyListType.StructurePropertyList:
                    if (_formElementsLabels[4].Count > 0)
                        label = _formElementsLabels[4][configurationProperty.Name];
                    break;
            }
            return label;
        }

        private bool IsInFormElementLabels(PropertyListType propertyListType, Property configurationProperty)
        {
            bool retVal = false;
            switch (propertyListType)
            {
                case PropertyListType.MixturePropertyList:
                    if (_formElementsLabels[0].Count > 0)
                        if (_formElementsLabels[0].ContainsKey(configurationProperty.Name))
                            retVal = true;
                    break;
                case PropertyListType.CompoundPropertyList:
                    if (_formElementsLabels[1].ContainsKey(configurationProperty.Name))
                        retVal = true;
                    break;
                case PropertyListType.BatchPropertyList:
                    if (_formElementsLabels[2].ContainsKey(configurationProperty.Name))
                        retVal = true;
                    break;
                case PropertyListType.BatchComponentPropertyList:
                    if (_formElementsLabels[3].ContainsKey(configurationProperty.Name))
                        retVal = true;
                    break;
                case PropertyListType.StructurePropertyList:
                    if (_formElementsLabels[4].ContainsKey(configurationProperty.Name))
                        retVal = true;
                    break;
            }
            return retVal;
        }

        #endregion

        #region Public Methods
        public void UpdateRegistrationFormGroups()
        {
            try
            {
                UpdateRegistrationFormGroup(this.SUBMITMIXTUREFORMID);
                UpdateRegistrationFormGroup(this.REGISTERMIXTUREFORMID);
                UpdateRegistrationFormGroup(this.VIEWMIXTUREFORMID);
                UpdateSearchTemporaryFormGroup(SEARCHTEMPFORMID);
                UpdateSearchTemporaryFormGroup(ELNSEARCHTEMPFORMID);
                UpdateSearchPermanentFormGroup(SEARCHPERMFORMID);
                UpdateSearchPermanentFormGroup(ELNSEARCHPERMFORMID);
                UpdateRegistrationFormGroup(this.DATALOADERFORMID);
                UpdateRegistrationFormGroup(this.COMPONENTDUPLICATESFORMID);
                UpdateRegistrationFormGroup(this.REGISTRYDUPLICATESFORMID);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        public void ApplyFormEditing(FormGroup.Form mixtureForm, FormGroup.Form compoundForm, FormGroup.Form batchForm, FormGroup.Form batchComponentForm)
        {
            try
            {
                COEFormBO formGroup = COEFormBO.Get(SUBMITMIXTUREFORMID);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, 0, MIXTURESUBFORMINDEX, mixtureForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, 0, COMPOUNDSUBFORMINDEX, compoundForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, 0, BATCHSUBFORMINDEX, batchForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, 0, BATCHCOMPONENTSUBFORMINDEX, batchComponentForm);
                formGroup.Save();

                formGroup = COEFormBO.Get(REGISTERMIXTUREFORMID);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, 0, MIXTURESUBFORMINDEX, mixtureForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, 0, COMPOUNDSUBFORMINDEX, compoundForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, 0, BATCHSUBFORMINDEX, batchForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, 0, BATCHCOMPONENTSUBFORMINDEX, batchComponentForm);
                formGroup.Save();

                formGroup = COEFormBO.Get(VIEWMIXTUREFORMID);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, 0, MIXTURESUBFORMINDEX, mixtureForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, 0, COMPOUNDSUBFORMINDEX, compoundForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, 0, BATCHSUBFORMINDEX, batchForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, 0, BATCHCOMPONENTSUBFORMINDEX, batchComponentForm);
                formGroup.Save();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        public void ApplyFormEditingToSearchTemp(FormGroup.Form searchTempBatchForm, FormGroup.Form searchTempCompoundForm, FormGroup.Form searchTempDetailsBaseForm, FormGroup.Form searchTempDetailsChildForm, FormGroup.Form searchTempListForm)
        {
            try
            {
                COEFormBO formGroup = COEFormBO.Get(SEARCHTEMPFORMID);
                formGroup.SetForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, TEMPORARYBASEFORM, searchTempBatchForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, TEMPORARYCHILDFORM, searchTempCompoundForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, TEMPORARYBASEFORM, searchTempDetailsBaseForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, TEMPORARYCHILDFORM, searchTempDetailsChildForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.ListForm, LISTFORMINDEX, 0, searchTempListForm);
                formGroup.Save();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        public void ApplyFormEditingToELNSearchTemp(FormGroup.Form elnSearchTempBatchForm, FormGroup.Form elnSearchTempCompoundForm, FormGroup.Form elnSearchTempDetailsBaseForm, FormGroup.Form elnSearchTempDetailsChildForm, FormGroup.Form elnSearchTempListForm)
        {
            try
            {
                COEFormBO formGroup = COEFormBO.Get(ELNSEARCHTEMPFORMID);
                formGroup.SetForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, TEMPORARYBASEFORM, elnSearchTempBatchForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.QueryForm, QUERYFORMINDEX, TEMPORARYCHILDFORM, elnSearchTempCompoundForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, TEMPORARYBASEFORM, elnSearchTempDetailsBaseForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, DETAILFORMINDEX, TEMPORARYCHILDFORM, elnSearchTempDetailsChildForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.ListForm, LISTFORMINDEX, 0, elnSearchTempListForm);
                formGroup.Save();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        public void ApplyFormEditingToSearchPerm(FormGroup.Form searchPermMixtureForm, FormGroup.Form searchPermCompoundForm, FormGroup.Form searchPermBatchForm, FormGroup.Form searchPermBatchComponentForm, FormGroup.Form searchPermanentListForm,
            FormGroup.Form searchPermanentMixtureDetailForm, FormGroup.Form searchPermanentCompoundDetailForm, FormGroup.Form searchPermanentBatchDetailForm, FormGroup.Form searchPermanentBatchComponentDetailForm)
        {
            try
            {
                COEFormBO formGroup = COEFormBO.Get(SEARCHPERMFORMID);
                formGroup.SetForm(FormGroup.CurrentFormEnum.QueryForm, 0, MIXTURESEARCHFORM, searchPermMixtureForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.QueryForm, 0, COMPOUNDSEARCHFORM, searchPermCompoundForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.QueryForm, 0, BATCHSEARCHFORM, searchPermBatchForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.QueryForm, 0, BATCHCOMPONENTSEARCHFORM, searchPermBatchComponentForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.ListForm, 0, 0, searchPermanentListForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, 0, MIXTURESEARCHFORM, searchPermanentMixtureDetailForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, 0, COMPOUNDSEARCHFORM, searchPermanentCompoundDetailForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, 0, BATCHSEARCHFORM, searchPermanentBatchDetailForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, 0, BATCHCOMPONENTSEARCHFORM, searchPermanentBatchComponentDetailForm);
                formGroup.Save();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        public void ApplyFormEditingToELNSearchPerm(FormGroup.Form elnSearchPermMixtureForm, FormGroup.Form elnSearchPermCompoundForm, FormGroup.Form elnSearchPermBatchForm, FormGroup.Form elnSearchPermBatchComponentForm, FormGroup.Form elnSearchPermanentListForm,
      FormGroup.Form enlSearchPermanentMixtureDetailForm, FormGroup.Form enlSearchPermanentCompoundDetailForm, FormGroup.Form enlSearchPermanentBatchDetailForm, FormGroup.Form enlSearchPermanentBatchComponentDetailForm)
        {
            try
            {
                COEFormBO formGroup = COEFormBO.Get(ELNSEARCHPERMFORMID);
                formGroup.SetForm(FormGroup.CurrentFormEnum.QueryForm, 0, MIXTURESEARCHFORM, elnSearchPermMixtureForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.QueryForm, 0, COMPOUNDSEARCHFORM, elnSearchPermCompoundForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.QueryForm, 0, BATCHSEARCHFORM, elnSearchPermBatchForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.QueryForm, 0, BATCHCOMPONENTSEARCHFORM, elnSearchPermBatchComponentForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.ListForm, 0, 0, elnSearchPermanentListForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, 0, MIXTURESEARCHFORM, enlSearchPermanentMixtureDetailForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, 0, COMPOUNDSEARCHFORM, enlSearchPermanentCompoundDetailForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, 0, BATCHSEARCHFORM, enlSearchPermanentBatchDetailForm);
                formGroup.SetForm(FormGroup.CurrentFormEnum.DetailForm, 0, BATCHCOMPONENTSEARCHFORM, enlSearchPermanentBatchComponentDetailForm);
                formGroup.Save();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        [COEUserActionDescription("LoadCOEFormGroups")]
        public void Load(COEFormGroups formGroupType)
        {
            try
            {
                switch (formGroupType)
                {
                    case COEFormGroups.SubmitMixture:
                        _formGroupBO = COEFormBO.Get(SUBMITMIXTUREFORMID);
                        break;
                    case COEFormGroups.ReviewRegisterMixture:
                        _formGroupBO = COEFormBO.Get(REGISTERMIXTUREFORMID);
                        break;
                    case COEFormGroups.ViewMixture:
                        _formGroupBO = COEFormBO.Get(VIEWMIXTUREFORMID);
                        break;
                    case COEFormGroups.SearchTemporary:
                        _formGroupBO = COEFormBO.Get(SEARCHTEMPFORMID);
                        break;
                    case COEFormGroups.SearchPermanent:
                        _formGroupBO = COEFormBO.Get(SEARCHPERMFORMID);
                        break;
                    case COEFormGroups.ELNSearchTempForm:
                        _formGroupBO = COEFormBO.Get(ELNSEARCHTEMPFORMID);
                        break;
                    case COEFormGroups.ELNSearchPermForm:
                        _formGroupBO = COEFormBO.Get(ELNSEARCHPERMFORMID);
                        break;
                    case COEFormGroups.DataLoaderForm:
                        _formGroupBO = COEFormBO.Get(DATALOADERFORMID);
                        break;
                    case COEFormGroups.RegistryDuplicatesForm:
                        _formGroupBO = COEFormBO.Get(REGISTRYDUPLICATESFORMID);
                        break;
                    case COEFormGroups.ComponentDuplicatesForm:
                        _formGroupBO = COEFormBO.Get(COMPONENTDUPLICATESFORMID);
                        break;
                    case COEFormGroups.DeleteLogFrom:
                        _formGroupBO = COEFormBO.Get(DELETELOGFORMID);
                        break;
                    case COEFormGroups.SendToRegistrationForm:
                        _formGroupBO = COEFormBO.Get(SENDTOREGISTRATIONFORMID);
                        break;
                    case COEFormGroups.SearchComponentToAddForm:
                        _formGroupBO = COEFormBO.Get(SEARCHCOMPONENTSTOADDFORMID);
                        break;
                    case COEFormGroups.SearchComponentToAddFormRR:
                        _formGroupBO = COEFormBO.Get(SEARCHCOMPONENTSTOADDRRFORMID);
                        break;
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        [COEUserActionDescription("SaveFormGroup")]
        public void SaveFormGroup(string newFormGroup)
        {
            try
            {
                if (_formGroupBO == null)
                {
                    throw new Exception("There is no formgroup selected, please use Load() method first");
                }
                _formGroupBO.COEFormGroup = FormGroup.GetFormGroup(newFormGroup);
                _formGroupBO.Save();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        [COEUserActionDescription("UpdateFormGroupFromIniFormFields")]
        public void UpdateFormGroupFromIniFormFields(IniSettingInfo iniCfserver)
        {
            try
            {
                ImportIniFormFieldResult importResult = new ImportIniFormFieldResult(((IniRegistrationSection)iniCfserver.IniSections[IniSection.REGISTRATION_SECTION_NAME]).FormFields.Values);

                string[] customFromGroupsIds = RegAdminUtils.GetRegCustomFormGroupsIds();
                string formElementPath = string.Empty;

                foreach (string formGroupId in customFromGroupsIds)
                {
                    formElementPath = string.Format("Form Group (Id = {0})", formGroupId);

                    COEFormBO formBO = COEFormBO.Get(int.Parse(formGroupId));

                    UpdateDisplayCollectionFromIniFormFields(formBO.COEFormGroup.DetailsForms, iniCfserver, formElementPath + @"\DetailsForms\", importResult);
                    UpdateDisplayCollectionFromIniFormFields(formBO.COEFormGroup.QueryForms, iniCfserver, formElementPath + @"\QueryForms\", importResult);
                    UpdateDisplayCollectionFromIniFormFields(formBO.COEFormGroup.ListForms, iniCfserver, formElementPath + @"\ListForms\", importResult);

                    formBO.Save();
                }

                importResult.LogFormFieldImportResult();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        private void UpdateDisplayCollectionFromIniFormFields(FormGroup.DisplayCollection displayCollection, IniSettingInfo iniCfserver, string formElementPath, ImportIniFormFieldResult importResult)
        {
            string formElementPathOriginal = formElementPath;
            IniFormField formField = null;

            for (int i = 0; i < displayCollection.Displays.Count; i++)
            {
                foreach (FormGroup.Form coeForm in displayCollection.Displays[i].Forms)
                {
                    formElementPath = formElementPathOriginal + string.Format("COE Form (Id = {0})", coeForm.Id);

                    foreach (KeyValuePair<string, IniFormField> formFieldSetting in ((IniRegistrationSection)iniCfserver.IniSections[IniSection.REGISTRATION_SECTION_NAME]).FormFields)
                    {
                        formField = formFieldSetting.Value;

                        UpdateFormElementsFromIniFormField(formField, coeForm.AddMode, formElementPath + @"\AddMode", importResult);
                        UpdateFormElementsFromIniFormField(formField, coeForm.EditMode, formElementPath + @"\EditMode", importResult);
                        UpdateFormElementsFromIniFormField(formField, coeForm.ViewMode, formElementPath + @"\ViewMode", importResult);
                        UpdateFormElementsFromIniFormField(formField, coeForm.LayoutInfo, formElementPath + @"\LayoutInfo", importResult);
                    }
                }
            }
        }

        private static void UpdateFormElementsFromIniFormField(IniFormField formField, List<FormGroup.FormElement> formElements, string formElementPath, ImportIniFormFieldResult importResult)
        {
            foreach (FormGroup.FormElement formElement in formElements)
            {
                if (string.Compare(formField.FieldKey, formElement.Name, true) == 0 ||
                    string.Compare(formField.FieldKey, formElement.Id.Replace("Property", string.Empty), true) == 0
                    )
                {
                    // Update field label
                    if (formElement.DisplayInfo.Type == "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEGridView")
                    {

                    }
                    else if (formElement.DisplayInfo.Type == "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEWebGrid")
                    {

                    }
                    else
                    {
                        if (string.Compare(formElement.Label, formField.FieldLabel, true) != 0)
                        {
                            formField.IsSkipped = false;
                            formElement.Label = formField.FieldLabel;
                        }
                    }

                    // Update visibility
                    if (formElement.DisplayInfo.Visible != formField.IsVisible)
                    {
                        formField.IsSkipped = false;
                        formElement.DisplayInfo.Visible = formField.IsVisible;
                    }

                    // Update requiredness status
                    if (formField.IsRequired)
                    {
                        SetFormElementAsRequired(formField, formElement);
                    }

                    importResult.FormFieldImportInfo[formField].Add(formElementPath + "\\" + formElement.Name);
                    //LogSuccessfulFormFieldImport(formField.FieldKey, formElementPath + "\\" + formElement.Name);

                    break;
                }
            }
        }

        /// <summary>
        /// Sets this form field as a mandatory one, by adding a new ValidationRule of enum type RequiredField
        /// </summary>
        /// <param name="formElement">The form field to be set as required</param>
        private static void SetFormElementAsRequired(IniFormField formField, FormGroup.FormElement formElement)
        {
            // First check if there's already the "required" validation rule for this field
            foreach (FormGroup.ValidationRuleInfo validationRule in formElement.ValidationRuleList)
            {
                if (validationRule.ValidationRuleName == FormGroup.ValidationRuleEnum.RequiredField)
                    return;
            }

            formField.IsSkipped = false;

            FormGroup.ValidationRuleInfo requiredValidationRule = new FormGroup.ValidationRuleInfo();
            requiredValidationRule.ValidationRuleName = FormGroup.ValidationRuleEnum.RequiredField;
            requiredValidationRule.ErrorMessage = string.Format("{0} is required", formElement.Label);
            requiredValidationRule.DisplayPosition = FormGroup.DisplayPosition.Top_Left;

            formElement.ValidationRuleList.Add(requiredValidationRule);
        }

        private FormGroup.FormElement UpdateGridViewFEConfig(FormGroup.FormElement gridView, Dictionary<string, string> formElementsLabels, List<string> formElementsIdsToHide)
        {
            XmlNamespaceManager nsManager = new XmlNamespaceManager(gridView.ConfigInfo.OwnerDocument.NameTable);
            nsManager.AddNamespace("COE", "COE.FormGroup");
            XmlNode gridViewConfInfo = gridView.ConfigInfo;
            XmlNodeList tablesNodeList = gridViewConfInfo.SelectNodes("//COE:table", nsManager);
            foreach (XmlNode table in tablesNodeList)
            {
                XmlNodeList columsNodeList = table.SelectNodes("//COE:Column", nsManager);

                foreach (XmlNode column in columsNodeList)
                {
                    if (column.SelectSingleNode("./COE:formElement", nsManager) != null)
                    {
                        FormGroup.FormElement formElement = FormGroup.FormElement.GetFormElement(column.SelectSingleNode("./COE:formElement", nsManager).OuterXml);
                        foreach (string formElementId in formElementsLabels.Keys)
                        {
                            if (formElement.Id == formElementId || formElement.Id == formElementId + "Property")
                            {
                                column.SelectSingleNode("COE:headerText", nsManager).InnerText = formElementsLabels[formElementId];
                            }
                        }
                    }
                }
            }

            return gridView;
        }
        private FormGroup.FormElement UpdateWebGridFEConfig(FormGroup.FormElement webGrid, Dictionary<string, string> formElementsLabels, List<string> formElementsIdsToHide)
        {
            XmlNamespaceManager nsManager = new XmlNamespaceManager(webGrid.ConfigInfo.OwnerDocument.NameTable);
            nsManager.AddNamespace("COE", "COE.FormGroup");
            XmlNode webGridConfInfo = webGrid.ConfigInfo;
            XmlNodeList tablesNodeList = webGridConfInfo.SelectNodes("//COE:table", nsManager);
            foreach (XmlNode table in tablesNodeList)
            {
                XmlNodeList columsNodeList = table.SelectNodes("//COE:Column", nsManager);

                foreach (XmlNode column in columsNodeList)
                {
                    if (column.SelectSingleNode("./COE:formElement", nsManager) != null)
                    {
                        FormGroup.FormElement formElement = FormGroup.FormElement.GetFormElement(column.SelectSingleNode("./COE:formElement", nsManager).OuterXml);
                        if (formElementsIdsToHide.Contains(formElement.Id) || formElementsIdsToHide.Contains(formElement.Id.Replace("Property", string.Empty)))
                        {
                            if (column.Attributes["hidden"] == null)
                            {
                                column.Attributes.Append(column.OwnerDocument.CreateAttribute("hidden"));
                            } 
                            column.Attributes["hidden"].Value = "false";

                        }
                        foreach (string formElementId in formElementsLabels.Keys)
                        {
                            if (formElement.Id == formElementId || formElement.Id == formElementId + "Property")
                            {
                                column.SelectSingleNode("COE:headerText", nsManager).InnerText = formElementsLabels[formElementId];
                            }

                        }
                    }
                }
            }
            return webGrid;
        }

        private string IsColumnToHide(Property property)
        {
            string retVal = "true";
            bool breakLoop = false;  

            foreach (Property prop in _rootProperties)
            {
                if (breakLoop)
                    break;
                if (property.Name.Equals(prop.Name) && property.ID.Equals(prop.ID))
                {
                    retVal = "false";
                    breakLoop = true;
                }
            }
            foreach (Property prop in _batchProperties)
            {
                if (breakLoop)
                    break;
                if (property.Name.Equals(prop.Name) && property.ID.Equals(prop.ID))
                {
                    retVal = "false";
                    breakLoop = true;
                }
            }

            return retVal;
        }

        #endregion

        #region Enums

        public enum COEFormGroups
        {
            SubmitMixture,
            ReviewRegisterMixture,
            ViewMixture,
            SearchTemporary,
            SearchPermanent,
            ELNSearchTempForm,
            ELNSearchPermForm,
            DataLoaderForm,
            RegistryDuplicatesForm,
            ComponentDuplicatesForm,
            SendToRegistrationForm,
            DeleteLogFrom,
            SearchComponentToAddForm,
            SearchComponentToAddFormRR
        }

        private enum PropertyListType
        {
            MixturePropertyList,
            BatchPropertyList,
            BatchComponentPropertyList,
            CompoundPropertyList,
            StructurePropertyList,
            None
        }
        #endregion
    }
}
