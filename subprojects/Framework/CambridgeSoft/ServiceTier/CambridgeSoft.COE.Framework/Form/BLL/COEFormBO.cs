using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;
using Csla.Validation;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Xml;
using CambridgeSoft.COE.Framework.Common.Messaging;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Caching;


namespace CambridgeSoft.COE.Framework.COEFormService
{
    [Serializable()]
    public class COEFormBO : Csla.BusinessBase<COEFormBO>, ICacheable
    {
        #region Member variables
        //declare members
        private int _id = 0;
        private string _name = string.Empty;
        private SmartDate _dateCreated = new SmartDate(true);
        private string _description = string.Empty;
        private bool _isPublic = false;
        private string _userName = string.Empty;
        private int _formGroupId = 0;
        private FormGroup _coeForm;
        private string _databaseName = string.Empty;
        private string _application;
        private string _formType;
        private int _formTypeId;
        private COEAccessRightsBO _coeAccessRights;
        
        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        [NonSerialized]
        private string _serviceName = "COEForm";
        [NonSerialized]
        private static ApplicationData _appConfigData;
        [NonSerialized]
        private string _specialFolder = COEConfigurationBO.ConfigurationBaseFilePath + @"SimulationFolder\Framework\";

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEForm");

        private byte[] _coeFormHash = null;
        #endregion

        #region Properties
        /// <summary>
        /// The database name that is stored with the object.
        /// </summary>
        public string Application
        {
            get { return _application; }
            set { _application = value; }
        }

        /// <summary>
        /// Form group id of the current data.
        /// </summary>
        public int FormTypeId
        {
            get
            {
                CanReadProperty("FormTypeId", true);
                return _formTypeId;
            }
            set
            {
                if(!_formTypeId.Equals(value))
                {
                    _formTypeId = value;
                    PropertyHasChanged("FormTypeId");
                }
            }
        }

        /// <summary>
        /// The database name that is stored with the object.
        /// </summary>
        public string DatabaseName
        {
            get { return _databaseName; }
            set { _databaseName = value; }
        }
        /// <summary>
        /// Unique identifier.
        /// </summary>
        [System.ComponentModel.DataObjectField(true, false)]
        public int ID
        {
            get
            {
                CanReadProperty("ID", true);
                return _id;
            }
            set { _id = value; }
        }
        /// <summary>
        /// A name for the form being stored.
        /// </summary>
        public string Name
        {
            get
            {
                CanReadProperty("Name", true);
                return _name;
            }
            set
            {
                if (value == null) value = string.Empty;
                if (!_name.Equals(value))
                {
                    _name = value;
                    PropertyHasChanged("Name");
                }
            }
        }
        /// <summary>
        /// The date when the data was originally stored.
        /// </summary>
        public DateTime DateCreated
        {
            get
            {
                CanReadProperty("DateCreated", true);
                return _dateCreated.Date;
            }
        }

        /// <summary>
        /// A description for the form.
        /// </summary>
        public string Description
        {
            get
            {
                CanReadProperty("Description", true);
                return _description;
            }
            set
            {
                if (value == null) value = string.Empty;
                if (!_description.Equals(value))
                {
                    _description = value;
                    PropertyHasChanged("Description");
                }
            }
        }
        /// <summary>
        /// Indicates if only the creator can access this record or if everyone can.
        /// </summary>
        public bool IsPublic
        {
            get
            {
                CanReadProperty("IsPublic", true);
                return _isPublic;
            }
            set
            {

                if (!_isPublic.Equals(value))
                {
                    _isPublic = value;
                    PropertyHasChanged("IsPublic");
                }
            }
        }
        /// <summary>
        /// The user name storing/accessing the data.
        /// </summary>
        public string UserName
        {
            get
            {
                CanReadProperty("UserName", true);
                return _userName;
            }
            set
            {
                if (value == null) value = string.Empty;
                if (!_userName.Equals(value))
                {
                    _userName = value;
                    PropertyHasChanged("UserName");
                }
            }
        }

        /// <summary>
        /// Form group id of the current data.
        /// </summary>
        public int FormGroupId
        {
            get
            {
                CanReadProperty("FormGroupId", true);
                return _formGroupId;
            }
            set
            {
                if (!_formGroupId.Equals(value))
                {
                    _formGroupId = value;
                    PropertyHasChanged("FormGroupId");
                }
            }
        }

        /// <summary>
        /// The coform object being saved/accessed.
        /// </summary>
        public FormGroup COEFormGroup
        {
            get
            {
                CanReadProperty("COEForm", true);
                return _coeForm;
            }
            set
            {
                if (_coeForm == null || !_coeForm.Equals(value))
                {
                    _coeForm = value;
                    PropertyHasChanged("COEForm");
                }
            }
        }

        protected override object GetIdValue()
        {
            return _id;
        }

        public COEAccessRightsBO COEAccessRights
        {
            get
            {
                return _coeAccessRights;
            }
            set
            {
                _coeAccessRights = value;
            }
        }

        private static ApplicationData AppConfigData
        {
            get
            {
                if (_appConfigData == null && !string.IsNullOrEmpty(COEAppName.Get()))
                    _appConfigData = ConfigurationUtilities.GetApplicationData(COEAppName.Get());

                return _appConfigData;
            }
        }

        private static CacheItemData CacheConfig
        {
            get
            {
                try
                {
                    if (AppConfigData != null && AppConfigData.CachingData != null && AppConfigData.CachingData.Form != null)
                    {
                        return AppConfigData.CachingData.Form;
                    }
                }
                catch (Exception)
                {
                }
                return new CacheItemData();
            }
        }
        #endregion

        #region Constructors

        private COEFormBO()
        { /* require use of factory method */ }

        //constructor to be called from queryCriteriaList as well as any other services that needs to construct this object
        internal COEFormBO(int id, string name, string description, string userID, bool isPublic, int formGroup, SmartDate dateCreated, FormGroup coeFormGroup, string databaseName, string application, int formtypeid, string formtype)
        {
            _id = id;
            _name = name;
            _description = description;
            _userName = userID;
            _dateCreated = dateCreated;
            _isPublic = isPublic;
            _formGroupId = formGroup;
            _coeForm = coeFormGroup;
            _databaseName = databaseName;
            _application = application;
            _formTypeId = formtypeid;
            _formType = formtype;
        }

        internal COEFormBO(FormGroup coeFormGroup)
        {
            _coeForm = coeFormGroup;
        }

        #endregion

        #region Validation Rules

        private void AddCommonRules()
        {
            //
            // QueryName
            //
            ValidationRules.AddRule(CommonRules.StringRequired, "Name");
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Name", 50));
            //
            // DateCreated
            //
            ValidationRules.AddRule(CommonRules.MinValue<SmartDate>, new CommonRules.MinValueRuleArgs<SmartDate>("DateCreated", new SmartDate("1/1/2005")));
            ValidationRules.AddRule(CommonRules.RegExMatch, new CommonRules.RegExRuleArgs("DateCreated", @"(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d"));
        }

        protected override void AddBusinessRules()
        {
            AddCommonRules();
        }
        #endregion //Validation Rules

        #region Factory Methods

        //this method must be called prior to any other method inorder to set the database that the dal will use
        internal static void SetDatabaseName()
        {
            COEDatabaseName.Set(Resources.CentralizedStorageDB);
        }

        internal static void SetDatabaseName(string databaseName)
        {
            COEDatabaseName.Set(databaseName);
        }

        public static COEFormBO New(string databaseName)
        {
            SetDatabaseName();

            if (!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEFormBO");
            return DataPortal.Create<COEFormBO>(new CreateNewCriteria(databaseName));
        }

        public static COEFormBO Get(int id)
        {
            string idString = id.ToString();
            SetDatabaseName();
            COEFormBO result = null;
            if (!CanGetObject(id))
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForViewObject + " COEFormBO");

            switch (CacheConfig.Cache)
            {
                case CacheType.Disabled:
                    result = DataPortal.Fetch<COEFormBO>(new Criteria(id));
                    break;
                case CacheType.ClientCache:
                    result = LocalCache.Get(idString, typeof(COEFormBO)) as COEFormBO;
                    if (result == null)
                    {
                        result = DataPortal.Fetch<COEFormBO>(new Criteria(id));
                        LocalCache.Add(idString, typeof(COEFormBO), result, CacheConfig.AbsoluteExpiration, CacheConfig.SlidingExpiration, CacheConfig.DefaultPriority);
                    }
                    break;
                case CacheType.ServerCache:
                    result = ServerCache.Get(idString, typeof(COEFormBO)) as COEFormBO;
                    if (result == null)
                    {
                        result = DataPortal.Fetch<COEFormBO>(new Criteria(id));
                    }
                    break;
                case CacheType.ServerAndClientCache:
                    result = LocalCache.Get(idString, typeof(COEFormBO)) as COEFormBO;
                    //if (!ServerCache.Exists(idString, typeof(COEFormBO)) && result == null) //Exists is first to refresh the sliding time
                    //{
                    //    result = DataPortal.Fetch<COEFormBO>(new Criteria(id));
                    //    LocalCache.Add(idString, typeof(COEFormBO), result, CacheConfig.AbsoluteExpiration, CacheConfig.SlidingExpiration, CacheConfig.DefaultPriority);
                    //}
                    //else if (result == null)
                    //{
                    //    result = ServerCache.Get(idString, typeof(COEFormBO)) as COEFormBO;
                    //    LocalCache.Add(idString, typeof(COEFormBO), result, CacheConfig.AbsoluteExpiration, CacheConfig.SlidingExpiration, CacheConfig.DefaultPriority);
                    //}
                    // Coverity Fix CID - 10818 (from local server)
                    if (result == null) //Exists is first to refresh the sliding time
                    {
                        result = ServerCache.Get(idString, typeof(COEFormBO)) as COEFormBO;                        
                        if (result == null)
                        {
                            result = DataPortal.Fetch<COEFormBO>(new Criteria(id));                            
                        }
                        LocalCache.Add(idString, typeof(COEFormBO), result, CacheConfig.AbsoluteExpiration, CacheConfig.SlidingExpiration, CacheConfig.DefaultPriority);
                    }                    
                    break;
            }

            return result;
        }

        public static COEFormBO Get(int id, bool includeAccessRights)
        {
            COEFormBO result = Get(id);
            // Coverity Fix CID - 10818 (from local server)
            if (result != null && includeAccessRights)
            {
                result.COEAccessRights = COEAccessRightsBO.Get(COEAccessRightsBO.ObjectTypes.COEGENERICOBJECT, id);
            }
            return result;
        }

        public static void Delete(int id)
        {
            SetDatabaseName();
            if (!CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEFormBO");

            switch (CacheConfig.Cache)
            {
                case CacheType.ClientCache:
                    LocalCache.Remove(id.ToString(), typeof(COEFormBO));
                    break;
                case CacheType.ServerCache:
                    if (ServerCache.Exists(id.ToString(), typeof(COEFormBO)))
                        ServerCache.Remove(id.ToString(), typeof(COEFormBO));
                    break;
                case CacheType.ServerAndClientCache:
                    LocalCache.Remove(id.ToString(), typeof(COEFormBO));
                    if (ServerCache.Exists(id.ToString(), typeof(COEFormBO)))
                        ServerCache.Remove(id.ToString(), typeof(COEFormBO));
                    break;
            }
            DataPortal.Delete(new Criteria(id));
        }

        public override COEFormBO Save()
        {
           
            if (IsDeleted && !CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEFormBO");
            else if (IsNew && !CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEFormBO");
            else if (!CanEditObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForEditObject + " COEFormBO");
            if (!this.CompareByteArray(_coeFormHash, CambridgeSoft.COE.Framework.Common.Utilities.ComputeHash(this.COEFormGroup.ToString())))
                this.MarkDirty();

            return base.Save();
        }

        public static bool CanAddObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        public static bool CanGetObject(int id)
        {
            return new CanGetFormCommand(id).Execute();
        }

        public static bool CanEditObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        public static bool CanDeleteObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }

        #endregion //Factory Methods

        #region Business Methods

        /// <summary>
        /// Update form elements in Add,Edit,View mode, 
        /// so that Id of those form elements in the one mode are in the same order as <typeparamref name="formElementsIds"/>
        /// </summary>
        /// <param name="currentForm"></param>
        /// <param name="formIndex"></param>
        /// <param name="subFormIndex"></param>
        /// <param name="formElementsIds">required order</param>
        public void UpdateFormElemetsSortOrder(FormGroup.CurrentFormEnum currentForm, int formIndex, int subFormIndex, List<string> formElementsIds)
        {
            FormGroup.Form tempForm = new FormGroup.Form();
            FormGroup.Form formToModify = new FormGroup.Form();

            switch (currentForm)
            {
                case FormGroup.CurrentFormEnum.DetailForm:
                    formToModify = GetForm(_coeForm.DetailsForms, formIndex, subFormIndex);

                    //Saving form elements on temporary form
                    foreach (FormGroup.FormElement formElement in formToModify.AddMode)
                    {
                        tempForm.AddMode.Add(formElement);
                    }

                    foreach (FormGroup.FormElement formElement in formToModify.EditMode)
                    {
                        tempForm.EditMode.Add(formElement);
                    }

                    foreach (FormGroup.FormElement formElement in formToModify.ViewMode)
                    {
                        tempForm.ViewMode.Add(formElement);
                    }

                    //Deleting form elements from current form
                    foreach (string formElementId in formElementsIds)
                    {
                        foreach (FormGroup.FormElement formElement in tempForm.AddMode)
                        {
                            if (formElement.Id == formElementId)
                                formToModify.AddMode.Remove(formElement);
                        }
                    }

                    foreach (string formElementId in formElementsIds)
                    {
                        foreach (FormGroup.FormElement formElement in tempForm.EditMode)
                        {
                            if (formElement.Id == formElementId)
                                formToModify.EditMode.Remove(formElement);
                        }
                    }

                    foreach (string formElementId in formElementsIds)
                    {
                        foreach (FormGroup.FormElement formElement in tempForm.ViewMode)
                        {
                            if (formElement.Id == formElementId)
                                formToModify.ViewMode.Remove(formElement);
                        }
                    }

                    //Adding form elements to current form in the specific sort order
                    foreach (string formElementId in formElementsIds)
                    {
                        foreach (FormGroup.FormElement formElement in tempForm.AddMode)
                        {
                            if (formElement.Id == formElementId)
                            {
                                formToModify.AddMode.Add(formElement);
                            }
                        }
                    }

                    foreach (string formElementId in formElementsIds)
                    {
                        foreach (FormGroup.FormElement formElement in tempForm.EditMode)
                        {
                            if (formElement.Id == formElementId)
                            {
                                formToModify.EditMode.Add(formElement);
                            }
                        }
                    }

                    foreach (string formElementId in formElementsIds)
                    {
                        foreach (FormGroup.FormElement formElement in tempForm.ViewMode)
                        {
                            if (formElement.Id == formElementId)
                            {
                                formToModify.ViewMode.Add(formElement);
                            }
                        }
                    }
                    break;

                case FormGroup.CurrentFormEnum.QueryForm:
                    formToModify = GetForm(_coeForm.QueryForms, formIndex, subFormIndex);

                    //Saving form elements on temporary form
                    foreach (FormGroup.FormElement formElement in formToModify.LayoutInfo)
                    {
                        tempForm.LayoutInfo.Add(formElement);
                    }

                    //Deleting form elements from current form
                    foreach (string formElementId in formElementsIds)
                    {
                        foreach (FormGroup.FormElement formElement in tempForm.LayoutInfo)
                        {
                            if (formElement.Id == formElementId)
                                formToModify.LayoutInfo.Remove(formElement);
                        }
                    }

                    //Adding form elements to current form in the specific sort order
                    foreach (string formElementId in formElementsIds)
                    {
                        foreach (FormGroup.FormElement formElement in tempForm.LayoutInfo)
                        {
                            if (formElement.Id == formElementId)
                            {
                                formToModify.LayoutInfo.Add(formElement);
                            }
                        }
                    }
                    break;
            }
        }

        public void AddFormElementsToForm(FormGroup.CurrentFormEnum currentForm, int formIndex, int subFormIndex, FormGroup.DisplayMode displayMode, FormAddBehavior addBehavior, List<FormGroup.FormElement> formElementsToAdd)
        {
            FormGroup.Form formToModify = null;
            switch (currentForm)
            {
                case FormGroup.CurrentFormEnum.DetailForm:
                    formToModify = GetForm(_coeForm.DetailsForms, formIndex, subFormIndex);
                    break;
                case FormGroup.CurrentFormEnum.ListForm:
                    formToModify = GetForm(_coeForm.ListForms, formIndex, subFormIndex);
                    break;
                case FormGroup.CurrentFormEnum.QueryForm:
                    formToModify = GetForm(_coeForm.QueryForms, formIndex, subFormIndex);
                    break;
            }
            //Coverity Bug Fix CID 11527 
            if (formToModify != null)
            {
                switch (addBehavior)
                {
                    case FormAddBehavior.AllowDuplicateFormElements:
                        switch (displayMode)
                        {
                            case FormGroup.DisplayMode.Add:
                                formToModify.AddMode.AddRange(formElementsToAdd);
                                break;
                            case FormGroup.DisplayMode.Edit:
                                formToModify.EditMode.AddRange(formElementsToAdd);
                                break;
                            case FormGroup.DisplayMode.View:
                                formToModify.ViewMode.AddRange(formElementsToAdd);
                                break;
                            case FormGroup.DisplayMode.All:
                                formToModify.LayoutInfo.AddRange(formElementsToAdd);
                                break;
                        }
                        break;

                    case FormAddBehavior.DisallowDuplicateFormElements:
                        foreach (FormGroup.FormElement formElement in formElementsToAdd)
                        {
                            bool isAlready = false;
                            List<FormGroup.FormElement> searchableCollection = null;
                            if (formToModify.LayoutInfo.Count > 0)
                            {
                                searchableCollection = formToModify.LayoutInfo;
                            }
                            else
                            {
                                switch (displayMode)
                                {
                                    case FormGroup.DisplayMode.Add:
                                        searchableCollection = formToModify.AddMode;
                                        break;
                                    case FormGroup.DisplayMode.Edit:
                                        searchableCollection = formToModify.EditMode;
                                        break;
                                    case FormGroup.DisplayMode.View:
                                        searchableCollection = formToModify.ViewMode;
                                        break;
                                }
                            }
                            if (searchableCollection != null)
                            {
                                foreach (FormGroup.FormElement existingFormElement in searchableCollection)
                                {
                                    if (formElement.BindingExpression == existingFormElement.BindingExpression)
                                    {
                                        isAlready = true;
                                        break;
                                    }
                                }
                            }
                            if (!isAlready)
                            {
                                switch (displayMode)
                                {
                                    case FormGroup.DisplayMode.Add:
                                        formToModify.AddMode.Add(formElement);
                                        break;
                                    case FormGroup.DisplayMode.Edit:
                                        formToModify.EditMode.Add(formElement);
                                        break;
                                    case FormGroup.DisplayMode.View:
                                        formToModify.ViewMode.Add(formElement);
                                        break;
                                    case FormGroup.DisplayMode.All:
                                        formToModify.LayoutInfo.Add(formElement);
                                        break;
                                }
                            }
                            else
                            {
                                for (int i = 0; i < searchableCollection.Count; i++)
                                {
                                    if (formElement.BindingExpression == searchableCollection[i].BindingExpression)
                                    {
                                        searchableCollection[i].ValidationRuleList = formElement.ValidationRuleList;
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }

        public void RemoveFormElementsFromForm(FormGroup.CurrentFormEnum currentForm, int formIndex, int subFormIndex, FormGroup.DisplayMode displayMode, List<string> formElementsToRemove)
        {
            FormGroup.Form formToModify = null;
            switch (currentForm)
            {
                case FormGroup.CurrentFormEnum.DetailForm:
                    formToModify = GetForm(_coeForm.DetailsForms, formIndex, subFormIndex);
                    break;
                case FormGroup.CurrentFormEnum.ListForm:
                    formToModify = GetForm(_coeForm.ListForms, formIndex, subFormIndex);
                    break;
                case FormGroup.CurrentFormEnum.QueryForm:
                    formToModify = GetForm(_coeForm.QueryForms, formIndex, subFormIndex);
                    break;
            }
            //Coverity Bug Fix CID 11534 
            if (formToModify != null)
            {
                switch (displayMode)
                {
                    case FormGroup.DisplayMode.Add:
                        foreach (string formElementid in formElementsToRemove)
                        {
                            FormGroup.FormElement formElementToDelete = null;
                            foreach (FormGroup.FormElement existingFormElement in formToModify.AddMode)
                            {
                                if (formElementid == existingFormElement.Id)
                                {
                                    formElementToDelete = existingFormElement;
                                    break;
                                }
                            }
                            formToModify.AddMode.Remove(formElementToDelete);
                        }
                        break;
                    case FormGroup.DisplayMode.Edit:
                        foreach (string formElementid in formElementsToRemove)
                        {
                            FormGroup.FormElement formElementToDelete = null;
                            foreach (FormGroup.FormElement existingFormElement in formToModify.EditMode)
                            {
                                if (formElementid == existingFormElement.Id)
                                {
                                    formElementToDelete = existingFormElement;
                                    break;
                                }
                            }
                            formToModify.EditMode.Remove(formElementToDelete);
                        }
                        break;
                    case FormGroup.DisplayMode.View:
                        foreach (string formElementid in formElementsToRemove)
                        {
                            FormGroup.FormElement formElementToDelete = null;
                            foreach (FormGroup.FormElement existingFormElement in formToModify.ViewMode)
                            {
                                if (formElementid == existingFormElement.Id)
                                {
                                    formElementToDelete = existingFormElement;
                                    break;
                                }
                            }
                            formToModify.ViewMode.Remove(formElementToDelete);
                        }
                        break;
                    case FormGroup.DisplayMode.All:
                        foreach (string formElementid in formElementsToRemove)
                        {
                            FormGroup.FormElement formElementToDelete = null;
                            foreach (FormGroup.FormElement existingFormElement in formToModify.LayoutInfo)
                            {
                                if (formElementid == existingFormElement.Id)
                                {
                                    formElementToDelete = existingFormElement;
                                    break;
                                }
                            }
                            formToModify.LayoutInfo.Remove(formElementToDelete);
                        }
                        break;
                }
            }
        }

        public void RemoveAllFormElementsFromForm(FormGroup.CurrentFormEnum currentForm, int formIndex, int subFormIndex, FormGroup.DisplayMode displayMode)
        {
            FormGroup.Form formToModify = null;
            switch (currentForm)
            {
                case FormGroup.CurrentFormEnum.DetailForm:
                    formToModify = GetForm(_coeForm.DetailsForms, formIndex, subFormIndex);
                    break;
                case FormGroup.CurrentFormEnum.ListForm:
                    formToModify = GetForm(_coeForm.ListForms, formIndex, subFormIndex);
                    break;
                case FormGroup.CurrentFormEnum.QueryForm:
                    formToModify = GetForm(_coeForm.QueryForms, formIndex, subFormIndex);
                    break;
            }
            //Coverity Bug Fix CID 11533 
            if (formToModify != null)
            {
                switch (displayMode)
                {
                    case FormGroup.DisplayMode.Add:
                        formToModify.AddMode.Clear();
                        break;
                    case FormGroup.DisplayMode.Edit:
                        formToModify.EditMode.Clear();
                        break;
                    case FormGroup.DisplayMode.View:
                        formToModify.ViewMode.Clear();
                        break;
                    case FormGroup.DisplayMode.All:
                        formToModify.LayoutInfo.Clear();
                        break;
                }
            }
        }

        public void ReorderSearchCriteriaBindingExpressions()
        {
            int bindingExpressionSubIndex = 0;
            foreach (FormGroup.Display display in this.COEFormGroup.QueryForms.Displays)
            {
                foreach (FormGroup.Form form in display.Forms)
                {
                    for (int i = 0; i < form.LayoutInfo.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(form.LayoutInfo[i].BindingExpression))
                        {
                            form.LayoutInfo[i].BindingExpression = Regex.Replace(form.LayoutInfo[i].BindingExpression, @"\[\d{1,4}\]", "[" + bindingExpressionSubIndex++ + "]");
                        }
                    }
                }
            }
        }

        public FormGroup.Form GetForm(FormGroup.DisplayCollection collection, int formIndex, int subFormIndex)
        {
            foreach (FormGroup.Form form in collection[formIndex].Forms)
            {
                if (form.Id == subFormIndex)
                    return form;
            }
            throw new Exception("The coeform with id " + subFormIndex + " is not present in the current form collection.");
        }

        public FormGroup.Form TryGetForm(FormGroup.DisplayCollection collection, int formIndex, int subFormIndex)
        {
            if (collection.Displays.Count == 0)
                return null;

            FormGroup.Form returnForm = null;

            foreach (FormGroup.Form form in collection[formIndex].Forms)
            {
                if (form.Id == subFormIndex)
                    returnForm = form;
            }

            return returnForm;
        }

        public void SetForm(FormGroup.CurrentFormEnum currentFormEnum, int formIndex, int subFormIndex, FormGroup.Form form)
        {
            switch (currentFormEnum)
            {
                case FormGroup.CurrentFormEnum.DetailForm:
                    try
                    {
                        for (int i = 0; i < this.COEFormGroup.DetailsForms[formIndex].Forms.Count; i++)
                        {
                            if (this.COEFormGroup.DetailsForms[formIndex].Forms[i].Id == subFormIndex)
                                this.COEFormGroup.DetailsForms[formIndex].Forms[i] = form;
                        }
                    }
                    catch { }
                    break;
                case FormGroup.CurrentFormEnum.ListForm:
                    try
                    {
                        for (int i = 0; i < this.COEFormGroup.ListForms[formIndex].Forms.Count; i++)
                        {
                            if (this.COEFormGroup.ListForms[formIndex].Forms[i].Id == subFormIndex)
                                this.COEFormGroup.ListForms[formIndex].Forms[i] = form;
                        }
                    }
                    catch { }
                    break;
                case FormGroup.CurrentFormEnum.QueryForm:
                    try
                    {
                        for (int i = 0; i < this.COEFormGroup.QueryForms[formIndex].Forms.Count; i++)
                        {
                            if (this.COEFormGroup.QueryForms[formIndex].Forms[i].Id == subFormIndex)
                                this.COEFormGroup.QueryForms[formIndex].Forms[i] = form;
                        }
                    }
                    catch { }
                    break;
            }
        }

        private void CreateCoeFormHash()
        {
            _coeFormHash = CambridgeSoft.COE.Framework.Common.Utilities.ComputeHash(this.COEFormGroup.ToString());
        }

        private bool CompareByteArray(byte[] operand1, byte[] operand2)
        {
            //if both are the same reference or null return true.
            if (operand1 == operand2)
                return true;

            if(operand1 == null || operand2 == null)
                return false;

            if (operand1.Length == operand2.Length)
                for (int index = 0; index < operand1.Length; index++)
                    if (operand1[index] != operand2[index])
                        return false;

            return true;
        }

        /// <summary>
        /// Get rid of requied "validationRule" element under "layoutInfo" element for search permanent form group xml.
        /// </summary>
        /// <param name="formGroup">The specific formGroup object</param>
        public void ClearRequiredValidationRule()
        {
            foreach (FormGroup.Display coeForm in _coeForm.QueryForms.Displays)
            {
                foreach (FormGroup.Form form in coeForm.Forms)
                {
                    foreach (FormGroup.FormElement formElement in form.LayoutInfo)
                    {
                        foreach (FormGroup.ValidationRuleInfo validationRule in formElement.ValidationRuleList)
                        {
                            if (
                                validationRule.ValidationRuleName.Equals(
                                FormGroup.ValidationRuleEnum.RequiredField)
                                )
                            {
                                formElement.ValidationRuleList.Remove(validationRule);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// CSBR: To fix 147743
        /// Get rid of all "validationRule" elements under "layoutInfo" element for search form group xml.
        /// </summary>
        /// <param name="formGroup">The specific formGroup object</param>
        public void ClearValidationRulesFromSearch()
        {
            foreach (FormGroup.Display coeForm in _coeForm.QueryForms.Displays)
            {
                foreach (FormGroup.Form form in coeForm.Forms)
                {
                    foreach (FormGroup.FormElement formElement in form.LayoutInfo)
                    {
                        formElement.ValidationRuleList.Clear();// Clear all validation rules from the search pages
                    }
                }
            }
        }

        #endregion

        #region Data Access
        protected override void DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
        {
            throw ex;
        }

        #region Criteria

        [Serializable()]
        private class Criteria
        {
            #region Variables
            internal int _id;
            internal bool _includeAccessRights;
            #endregion

            #region Constructors
            public Criteria(int id)
            {
                _id = id;
                _includeAccessRights = false;
            }
            
            public Criteria(int id, bool includeAccessRights)
            {
                _id = id;
                _includeAccessRights = includeAccessRights;
            }
            #endregion
        }
        #endregion //Criteria

        #region CreateNewCriteria classes

        [Serializable()]
        protected class CreateNewCriteria
        {
            internal string _database;
            public CreateNewCriteria(string database)
            {
                _database = database;
            }
        }

        #endregion

        #region Data Access - Create
        [RunLocal]
        private void DataPortal_Create(CreateNewCriteria criteria)
        {
            _coeForm = null;
            _databaseName = criteria._database;
        }

        #endregion //Data Access - Create

        #region Data Access - Fetch
        private void DataPortal_Fetch(Criteria criteria)
        {
            if (CambridgeSoft.COE.Framework.Common.Utilities.SimulationMode())
            {
                XmlDocument xmlDocument = new XmlDocument();

                xmlDocument.Load(_specialFolder + this.GetType().Name + "_" + criteria._id + @".xml");
                _id = criteria._id;
                _coeForm = FormGroup.GetFormGroup(xmlDocument.InnerXml);
            }
            else
            {
                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11529 
                if (_coeDAL != null)
                {
                    using (SafeDataReader dr = _coeDAL.Get(criteria._id))
                    {
                        FetchObject(dr);
                    }
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }

            CreateCoeFormHash();
        }

        private void FetchObject(SafeDataReader dr)
        {
            try
            {
                if (dr.Read())
                {
                    _id = dr.GetInt32("ID");
                    _name = dr.GetString("NAME");
                    _dateCreated = dr.GetSmartDate("DATE_CREATED", _dateCreated.EmptyIsMin);
                    _description = dr.GetString("DESCRIPTION");
                    _isPublic = (dr.GetString("IS_PUBLIC").Equals("1") ? true : false);
                    _userName = dr.GetString("USER_ID");
                    _formGroupId = dr.GetInt32("FORMGROUP");
                    _formTypeId = dr.GetInt32("FORMTYPEID");
                    _formType = dr.GetString("FORMTYPE");
                    _application = dr.GetString("APPLICATION");
                    _coeForm = FormGroup.GetFormGroup(dr.GetString("COEFORM"));
                    _databaseName = dr.GetString("DATABASE"); //if we want to support different storage locations then this would be set by the input paramter

                    if (CacheConfig.Cache == CacheType.ServerCache || CacheConfig.Cache == CacheType.ServerAndClientCache)
                    {
                        // As dependencies are not serializable they are created on the dataportal_fetch calls (on server side)
                        _cacheDependency = _coeDAL.GetCacheDependency(_id);
                        // Moreover and due to the same fact, we dont want to use the server cache because that involves a CSLA command and serialization
                        // and would anyway lead to the current tier.
                        // So, local cache is used.
                        LocalCache.Add(this.ID.ToString(), this.GetType(), this, CacheConfig.AbsoluteExpiration, CacheConfig.SlidingExpiration, CacheConfig.DefaultPriority);
                    }
                }
                else throw new Exception("Could not read COEFormBO");
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        #endregion //Data Access - Fetch

        #region Data Access - Insert

        //when already on the server, this can be called
        internal void Insert(DAL coeDAL)
        {
            if (!IsDirty) 
                return;
            if(_coeDAL == null)
                LoadDAL();
            if(_coeAccessRights != null)
                _isPublic = false;
            
            _id = this.GenerateID();
            coeDAL.Insert(_formGroupId, _name, _isPublic, _description, _userName, _coeForm, _databaseName, _id, _formTypeId, _application);

            //call internal methods of COEAccessRightsBO to update accessRights
            if(_coeAccessRights != null)
            {
                _coeAccessRights.ObjectID = _id;
                _coeAccessRights.ObjectType = COEAccessRightsBO.ObjectTypes.COEGENERICOBJECT;
                _coeAccessRights.Save();
            }
            MarkOld();
            MarkClean();
        }

        private int GenerateID()
        {
            if(_id > 0)
                return _id;
            else
                return _coeDAL.GetNewID();
        }

        protected override void DataPortal_Insert()
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11530
            if (_coeDAL != null)
            {
                if (_coeAccessRights != null)
                    _isPublic = false;

                _id = this.GenerateID();

                _coeDAL.Insert(_formGroupId, _name, _isPublic, _description, _userName, _coeForm, _databaseName, _id, _formTypeId, _application);
                if (_coeAccessRights != null)
                {
                    _coeAccessRights.ObjectID = _id;
                    _coeAccessRights.ObjectType = COEAccessRightsBO.ObjectTypes.COEGENERICOBJECT;
                    _coeAccessRights.Save();
                }
                MarkOld();
                MarkClean();
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }
        #endregion

        #region Data Access - Update
        //called by other services
        internal void Update(DAL coeDataViewDAL)
        {
            if (!IsDirty) return;
            string serializedCOEForm = _coeForm.ToString();
            coeDataViewDAL.Update(_id, serializedCOEForm, _name, _description, _isPublic, _databaseName);
            MarkOld();
        }

        protected override void DataPortal_Update()
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11531
            if (_coeDAL != null)
            {
                if (base.IsDirty)
                {
                    string serializedCOEForm = _coeForm.ToString();
                    _coeDAL.Update(_id, serializedCOEForm, _name, _description, _isPublic, _databaseName);
                }
            }
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }
        #endregion //Data Access - Update

        #region Data Access - Delete
        //called by other services
        internal void DeleteSelf(DAL _coeDAL)
        {
            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11532 
            if (_coeDAL != null)            
                _coeDAL.Delete(_id);
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));            
        }

        protected override void DataPortal_DeleteSelf()
        {
            DataPortal_Delete(new Criteria(_id));
        }

        private void DataPortal_Delete(Criteria criteria)
        {

            if (_coeDAL == null) { LoadDAL(); }
            // Coverity Fix CID - 11528
            if (_coeDAL != null)
                _coeDAL.Delete(criteria._id);
            else
                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
        }
        #endregion //Data Access - Delete

        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
        }
        #endregion //Data Access

        public enum FormAddBehavior
        {
            AllowDuplicateFormElements = 0,
            DisallowDuplicateFormElements = 1
        }

        #region ICacheable Members

        [NonSerialized]
        private COECacheDependency _cacheDependency;
        /// <summary>
        /// Cache dependency that is build from the dal at dataportal_fetch time. Is the mechanism to get the cache updated when the underlying
        /// record changed in database.
        /// </summary>
        public COECacheDependency CacheDependency
        {
            get { return _cacheDependency; }
            set { _cacheDependency = value; }
        }

        /// <summary>
        /// Method triggered when the object is removed from cache. Currently display information in the debug console, if in debug mode.
        /// </summary>
        /// <param name="key">The object id</param>
        /// <param name="value">The actual dataviewbo</param>
        /// <param name="reason">The reason why it was removed from cache</param>
        public void ItemRemovedFromCache(string key, object value, COECacheItemRemovedReason reason)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine("*****************************");
            System.Diagnostics.Debug.WriteLine("Item Removed from cache.");
            System.Diagnostics.Debug.WriteLine("Key: " + key);
            System.Diagnostics.Debug.WriteLine("Reason: " + reason.ToString());
            System.Diagnostics.Debug.WriteLine("Current Time: " + DateTime.Now);
            System.Diagnostics.Debug.WriteLine("*****************************");
#endif
        }

        #endregion


        [Serializable]
        internal class CanGetFormCommand : CommandBase
        {
            private int _id;
            private string _serviceName = "COEForm";
            [NonSerialized]
            private DAL _coeDAL;
            [NonSerialized]
            private DALFactory _dalFactory;

            internal bool HasPermissions;
            public CanGetFormCommand(int id)
            {
                _id = id;
                HasPermissions = false;
            }


            protected override void DataPortal_Execute()
            {
                if (_coeDAL == null)
                    LoadDAL();
                // Coverity Fix CID - 11526 
                if (_coeDAL != null)
                    HasPermissions = _coeDAL.CanGetForm(_id);
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }

            private void LoadDAL()
            {

                if (_dalFactory == null) { _dalFactory = new DALFactory(); }
                _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
            }

            internal bool Execute()
            {
                return DataPortal.Execute<CanGetFormCommand>(this).HasPermissions;
            }
        }
    }

}
