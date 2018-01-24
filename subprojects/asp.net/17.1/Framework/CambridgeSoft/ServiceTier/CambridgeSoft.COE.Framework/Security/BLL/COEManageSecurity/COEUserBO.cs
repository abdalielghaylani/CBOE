using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Xml;
using Csla;
using Csla.Data;
using Csla.Validation;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Text.RegularExpressions;

namespace CambridgeSoft.COE.Framework.COESecurityService
{

    [Serializable()]
    public class COEUserBO : BusinessBase<COEUserBO>
    {
        #region Member Variables
        private int _personID;
        private string _userID;
        private int _supervisorID;
        private int _siteID;
        private bool _active = false;
        private string _originalPassword = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _userCode = string.Empty;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _middleName = string.Empty;
        private string _title = string.Empty;
        private string _department = string.Empty;
        private string _address = string.Empty;
        private string _telephone = string.Empty;
        private string _email = string.Empty;
        private COERoleBOList _roles = null;
        private COERoleBOList _originalRoles = null;
        private COERoleBOList _availableRoles = null;
        private bool _idSet;
        private bool _isDBMSUser = false;
        private bool _isLDAP = false;
        private bool _isExempt = false;
        private bool _activatingUser = false;
        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COESecurity";
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESecurity");
        #endregion

        #region Constructors
        //constructor to be called from  as well as any other services that needs to construct this object
        internal COEUserBO(int personID, string userCode, string userID, int supervisorID, string title, string firstName, string middleName, string lastName, int siteID, string department, string telephone, string email, bool active, string address, COERoleBOList roles, COERoleBOList availableRoles)
        {
            _personID = personID;
            _userCode = userCode;
            _userID = userID;
            _supervisorID = supervisorID;
            _title = title;
            _firstName = firstName;
            _middleName = middleName;
            _lastName = lastName;
            _siteID = siteID;
            _department = department;
            _telephone = telephone;
            _email = email;
            _active = active;
            _address = address;
            _roles = roles;
            _availableRoles = availableRoles;
            _activatingUser = false;
        }

        internal COEUserBO(int personID, string userCode, string userID, int supervisorID, string title, string firstName, string middleName, string lastName, int siteID, string department, string telephone, string email, bool active, string address)
        {
            _personID = personID;
            _userCode = userCode;
            _userID = userID;
            _supervisorID = supervisorID;
            _title = title;
            _firstName = firstName;
            _middleName = middleName;
            _lastName = lastName;
            _siteID = siteID;
            _department = department;
            _telephone = telephone;
            _email = email;
            _active = active;
            _address = address;
            _roles = null;
            _availableRoles = null;
            _activatingUser = false;
        }

        internal COEUserBO(string userID)
        {
            _personID = 0;
            _userCode = string.Empty;
            _userID = userID;
            _supervisorID = 0;
            _title = string.Empty;
            _firstName = string.Empty;
            _middleName = string.Empty;
            _lastName = string.Empty;
            _siteID = 0;
            _department = string.Empty;
            _telephone = string.Empty;
            _email = string.Empty;
            _active = true;
            _address = string.Empty;
            _roles = null;
            _availableRoles = null;
            _activatingUser = false;
        }
        #endregion

        #region Properties
        public int PersonID
        {
            get { return _personID; }
            set
            {
                CanWriteProperty(true);
                if(!_personID.Equals(value))
                {
                    _idSet = true;
                    _personID = value;
                    PropertyHasChanged();
                }
            }
        }

        public string UserID
        {
            get { return _userID; }
            set
            {
                CanWriteProperty(true);
                if(value == null)
                    value = string.Empty;

                _userID = value;
                PropertyHasChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                CanWriteProperty(true);
                if(value == null)
                    value = string.Empty;

                _password = value;
                PropertyHasChanged();
            }
        }

        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                CanWriteProperty(true);
                if(value == null)
                    value = string.Empty;

                _confirmPassword = value;
                PropertyHasChanged();
            }
        }

        public int SupervisorID
        {
            get { return _supervisorID; }
            set
            {
                CanWriteProperty(true);
                if(!_supervisorID.Equals(value))
                {
                    _supervisorID = value;
                    PropertyHasChanged();
                }
            }
        }
        public bool Active
        {
            get { return _active; }
            set
            {
                CanWriteProperty(true);
                if(!_active.Equals(value))
                {
                    _active = value;
                    PropertyHasChanged();
                }
            }
        }

        public bool IsDBMSUser
        {
            get { return _isDBMSUser; }
            set
            {
                CanWriteProperty(true);
                if(!_isDBMSUser.Equals(value))
                {
                    _isDBMSUser = value;
                    PropertyHasChanged();
                }
            }
        }

        public bool IsLDAP
        {
            get { return _isLDAP; }
            set
            {
                CanWriteProperty(true);
                if(!_isLDAP.Equals(value))
                {
                    _isLDAP = value;
                    PropertyHasChanged();
                }
            }
        }

        public bool IsExempt
        {
            get { return _isExempt; }
            set
            {
                CanWriteProperty(true);
                if(!_isExempt.Equals(value))
                {
                    _isExempt = value;
                    PropertyHasChanged();
                }
            }
        }

        public string UserCode
        {
            get { return _userCode; }
            set
            {
                CanWriteProperty(true);
                if(value == null)
                    value = string.Empty;
                if(!_userCode.Equals(value))
                {
                    _userCode = value;
                    PropertyHasChanged();
                }
            }
        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                CanWriteProperty(true);
                if(value == null)
                    value = string.Empty;
                if(!_firstName.Equals(value))
                {
                    _firstName = value;
                    PropertyHasChanged();
                }
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                CanWriteProperty(true);
                if(value == null)
                    value = string.Empty;
                if(!_lastName.Equals(value))
                {
                    _lastName = value;
                    PropertyHasChanged();
                }
            }
        }

        public string MiddleName
        {
            get { return _middleName; }
            set
            {
                CanWriteProperty(true);
                if(value == null)
                    value = string.Empty;
                if(!_middleName.Equals(value))
                {
                    _middleName = value;
                    PropertyHasChanged();
                }
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                CanWriteProperty(true);
                if(value == null)
                    value = string.Empty;
                if(!_title.Equals(value))
                {
                    _title = value;
                    PropertyHasChanged();
                }
            }
        }

        public string Department
        {
            get { return _department; }
            set
            {
                CanWriteProperty(true);
                if(value == null)
                    value = string.Empty;
                if(!_department.Equals(value))
                {
                    _department = value;
                    PropertyHasChanged();
                }
            }
        }

        public string Address
        {
            get { return _address; }
            set
            {
                CanWriteProperty(true);
                if(value == null)
                    value = string.Empty;
                if(!_address.Equals(value))
                {
                    _address = value;
                    PropertyHasChanged();
                }
            }
        }

        public string Telephone
        {
            get { return _telephone; }
            set
            {
                CanWriteProperty(true);
                if(value == null)
                    value = string.Empty;
                if(!_telephone.Equals(value))
                {
                    _telephone = value;
                    PropertyHasChanged();
                }
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                CanWriteProperty(true);
                if(value == null)
                    value = string.Empty;
                if(!_email.Equals(value))
                {
                    _email = value;
                    PropertyHasChanged();
                }
            }
        }

        public int SiteID
        {
            get { return _siteID; }
            set
            {
                CanWriteProperty(true);
                if(value == null)
                    value = -1;
                if(!_siteID.Equals(value))
                {
                    _siteID = value;
                    PropertyHasChanged();
                }
            }
        }

        public COERoleBOList Roles
        {
            get { return _roles; }
            set
            {
                CanWriteProperty(true);
                if(value == null)
                    value = new COERoleBOList();
                _roles = value;
                //PropertyHasChanged();
            }
        }

        public COERoleBOList AvailableRoles
        {
            get { return _availableRoles; }
            set
            {
                CanWriteProperty(true);
                if(value == null)
                    value = new COERoleBOList();
                _availableRoles = value;
                //PropertyHasChanged();
            }
        }

        protected override object GetIdValue()
        {
            return _personID;
        }
        #endregion

        #region Factory Methods
        //this method must be called prior to any other method inorder to set the database that the dal will use
        internal static void SetDatabaseName()
        {
            COEDatabaseName.Set(Resources.CentralizedStorageDB);
        }

        internal static void SetDatabaseName(string databaseName)
        {
            COEDatabaseName.Set(Resources.CentralizedStorageDB);
        }

        public static COEUserBO GetLDAPUser(string UserName)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COEUserBO>(new CriteriaLDAP(UserName));
        }

        public static COEUserBO Get(string UserName)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COEUserBO>(new Criteria(UserName));
        }

        public static COEUserBO GetUserByID(int personID)
        {
            SetDatabaseName();
            return DataPortal.Fetch<COEUserBO>(new CriteriaByID(personID));
        }

        private COEUserBO()
        { /* require use of factory methods */ }

        public static COEUserBO New()
        {
            if(!CanAddObject())
                throw new System.Security.SecurityException(
                  "User not authorized to add a project");
            return DataPortal.Create<COEUserBO>();
        }

        public static void Delete(string userName)
        {
            SetDatabaseName();
            if(!CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEUserBO");
            DataPortal.Delete(new Criteria(userName));
        }

        public override COEUserBO Save()
        {
            SetDatabaseName();
            if(IsDeleted && !CanDeleteObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForDeleteObject + " COEUserBO");
            else if(IsNew && !CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEUserBO");
            else if(!CanEditObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForEditObject + " COEUserBO");
            ValidationRules.CheckRules();
            return base.Save();
        }

        public static bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            try
            {
                ChangePasswordCommand result;
                //this is returned the DoSearchCommand object so you can call methods within it
                result = DataPortal.Execute<ChangePasswordCommand>(new ChangePasswordCommand(userName, oldPassword, newPassword));
                return result.Success;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Validation Rules

        protected override void AddBusinessRules()
        {
            ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, "UserID");
            /* CSBR-154421 : Not possible to change passwords for the users, whose username exceeds 20 characters (users were imported from external data)
                * Changed by Jogi 
                  Changed the max length to 30 as the it is the maximum limit of username supported by oracle */
            ValidationRules.AddRule(Csla.Validation.CommonRules.StringMaxLength, new Csla.Validation.CommonRules.MaxLengthRuleArgs("UserID", 30));
            /* End of CSBR-154421 */
            ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, "LastName");
            ValidationRules.AddRule<COEUserBO>(RoleCount<COEUserBO>, "Roles");
            ValidationRules.AddRule<COEUserBO>(PasswordRequired<COEUserBO>, "Password");
            ValidationRules.AddRule<COEUserBO>(PasswordAndConfirmEqual<COEUserBO>, "ConfirmPassword");
            ValidationRules.AddRule(FirstLastSpaceCharacters, new Csla.Validation.RuleArgs("UserID"));
            ValidationRules.AddRule(ValidCharacters, new Csla.Validation.RuleArgs("UserID"));
            ValidationRules.AddRule(ValidCharacters, new Csla.Validation.RuleArgs("FirstName"));
            ValidationRules.AddRule(ValidCharacters, new Csla.Validation.RuleArgs("LastName"));
            ValidationRules.AddRule(ValidCharacters, new Csla.Validation.RuleArgs("MiddleName"));
        }

        protected override void AddInstanceBusinessRules()
        {
            ValidationRules.AddInstanceRule(NoDuplicates, "UserID");
        }

        private bool NoDuplicates(object target, Csla.Validation.RuleArgs e)
        {
            bool retVal = true;
            if(this.Parent != null)
            {
                COEUserReadOnlyBOList parent = (COEUserReadOnlyBOList) this.Parent;
                foreach(COEUserReadOnlyBO item in parent)
                    if(item.UserID == _userID && !ReferenceEquals(item, this))
                    {
                        e.Description = "UserId must be unique";
                        retVal = false;
                    }
            }
            return retVal;
        }
        /// <summary>
        /// CSBR-141632
        /// This function will get the first and last character of the user id 
        /// and checks for white-space  exits at frist and last position
        /// If exists throws a message
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static bool FirstLastSpaceCharacters(object target, Csla.Validation.RuleArgs args)
        {
            string value = target.GetType().GetProperty(args.PropertyName).GetValue(target, null) as string;
            
			string firstChar = string.Empty;
            string lastChar = string.Empty;
            if (!string.IsNullOrEmpty(value)) //Coverity Fix CID 20248 ASV
            {
                firstChar = value.Substring(0,1);
                lastChar = value.Substring(value.Length-1,1);
            }
            Regex regexfirstChar = new Regex(@"\s");
            if (regexfirstChar.IsMatch(firstChar))
            {
                args.Description = string.Format("First character should not be a white-space for {0}.\n", args.PropertyName);
                return false;
            }
            else if (regexfirstChar.IsMatch(lastChar))
            {
                args.Description = string.Format("Last character should not be a white-space for {0}.\n", args.PropertyName);
                return false;
            }
            else
            {
                return true;

            }
        }
        
        private static bool ValidCharacters(object target, Csla.Validation.RuleArgs args)
        {
            string value = target.GetType().GetProperty(args.PropertyName).GetValue(target, null) as string;
            if (string.IsNullOrEmpty(value)) value = string.Empty;  //Coverity Fix CID 20248 ASV
            //CSBR-138237, added the '.' character in regular expression.
            // which will allow the '.' in user name.
            //CSBR-148868, added '-' character in regular exp, which will allow '-' in the user name
            Regex regex = new Regex(@"[^\w\s'.-]");
                  
            if(!regex.IsMatch(value))
                return true;
            else
            {
                args.Description = string.Format("You've entered invalid characters for {0}.\n", args.PropertyName);
                return false;
            }
          
        }
        private static bool PasswordAndConfirmEqual<T>(T target, Csla.Validation.RuleArgs e) where T : COEUserBO
        {
            if(target._isLDAP == false || (target._isLDAP == true && target._isExempt == true))
            {
                if(target._password != target._confirmPassword)
                {
                    e.Description =
                      "password and confirm password must be equal";
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        private static bool PasswordRequired<T>(T target, Csla.Validation.RuleArgs e) where T : COEUserBO
        {
            if(target._isLDAP == false || (target._isLDAP == true && target._isExempt == true))
            {
                if(target.IsDBMSUser == false)
                {
                    if(target._password.Length == 0)
                    {
                        e.Description =
                          "password cannot be empty";
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        private static bool RoleCount<T>(T target, Csla.Validation.RuleArgs e) where T : COEUserBO
        {
            if(target._roles.Count == 0)
            {
                e.Description =
                  "user must have a minimum of 1 assigned role";
                return false;
            }
            else
                return true;
        }


        #endregion

        #region Authorization Rules

        protected override void AddAuthorizationRules() { }

        public static bool CanAddObject() { return true; }

        public static bool CanGetObject() { return true; }

        public static bool CanDeleteObject() { return true; }

        public static bool CanEditObject() { return true; }

        #endregion


        #region Criteria
        [Serializable()]
        private class Criteria
        {
            #region Variables
            internal string _userName;
            #endregion

            #region Constructors
            public Criteria(string userName)
            {
                _userName = userName;
            }
            #endregion
        }

        [Serializable()]
        private class CriteriaLDAP
        {
            #region Variables
            internal string _userName;
            #endregion

            #region Constructors
            public CriteriaLDAP(string userName)
            {
                _userName = userName;
            }
            #endregion
        }

        [Serializable()]
        private class CriteriaByID
        {
            #region Variables
            internal int _id;
            #endregion

            #region Constructors
            public CriteriaByID(int id)
            {
                _id = id;
            }
            #endregion
        }
        #endregion //Criteria

        #region Data Access
        [RunLocal()]
        protected override void DataPortal_Create()
        {
            _personID = 0;
            _roles = new COERoleBOList();
            _roles.Add(new COERoleBO("CSS_USER"));
            _availableRoles = COERoleBOList.GetNewUserAvailableRoles();
            _active = true;
            _isLDAP = IsLDAPAuthentication();
            _isExempt = false;
            _activatingUser = false;
            this.MarkNew();
        }

        internal void GetUserRoles()
        {
            try
            {
                _roles = COERoleBOList.GetListByUser(this.UserID);
                _originalRoles = _roles;
                _availableRoles = COERoleBOList.GetUserAvailableRoles(this.UserID);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        private void DataPortal_Fetch(Criteria criteria)
        {
            DataTable dt = new DataTable();
            try
            {
                _coeLog.LogStart(string.Empty, 1);
                if(_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11626
                if (_coeDAL != null)
                {
                    dt = _coeDAL.GetUser(criteria._userName);
                    FetchObject(dt);
                    if(!COERoleBO.roleUser)
                    GetUserRoles();
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
                
                _coeLog.LogEnd(string.Empty, 1);
            }
            catch(Exception ex)
            {
                if(!ex.Message.Contains("ORA-01403: no data found"))
                    throw;
                else
                    this.UserID = null;
            }
        }

        private void DataPortal_Fetch(CriteriaByID criteria)
        {
            DataTable dt = new DataTable();
            try
            {
                _coeLog.LogStart(string.Empty, 1);
                if (_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11627
                if (_coeDAL != null)
                {
                    string userName = _coeDAL.GetUserNameById(criteria._id);
                    dt = _coeDAL.GetUser(userName);
                    FetchObject(dt);
                    GetUserRoles();
                }
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));

                _coeLog.LogEnd(string.Empty, 1);
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("ORA-01403: no data found"))
                    throw;
                else
                    this.UserID = null;
            }
        }

        private void DataPortal_Fetch(CriteriaLDAP criteria)
        {
            try
            {
                _coeLog.LogStart(string.Empty, 1);
                coesinglesignon.SingleSignOn singleSignOn = new coesinglesignon.SingleSignOn();
                string url = ConfigurationUtilities.GetSingleSignOnURL();
                if(url != string.Empty)
                {
                    singleSignOn.Url = url;
                    XmlNode xmlNode = singleSignOn.GetUserInfo(criteria._userName);

                    this.IsDBMSUser = false;
                    this.IsLDAP = true;
                    this.IsExempt = false;
                    this.MarkNew();
                    foreach(XmlElement element in xmlNode)
                    {
                        if(element.Name == "username")
                        {
                            this.UserCode = element.InnerText;
                            this.UserID = element.InnerText;
                        }
                        else
                        {
                            string mapToAttr = element.Attributes.GetNamedItem("mapTo").Value;
                            if(mapToAttr.ToUpper() == "LASTNAME")
                            {
                                _lastName = element.InnerText;
                            }
                            if(mapToAttr.ToUpper() == "MIDDLENAME")
                            {
                                _middleName = element.InnerText;
                            }
                            if(mapToAttr.ToUpper() == "FIRSTNAME")
                            {
                                _firstName = element.InnerText;
                            }
                            if(mapToAttr.ToUpper() == "EMAIL")
                            {
                                _email = element.InnerText;
                            }
                        }
                    }
                }
                _coeLog.LogEnd(string.Empty, 1);
            }
            catch(Exception)
            {
                throw new Exception("User not found in LDAP");
            }
        }

        private void FetchObject(DataTable dt)
        {
            try
            {
                _personID = System.Convert.ToInt32(dt.Rows[0]["PERSONID"]);
                _password = System.Convert.ToString(dt.Rows[0]["PASSWORD"]);
                _confirmPassword = _password;
                _originalPassword = _password;
                _userCode = System.Convert.ToString(dt.Rows[0]["USERCODE"]);
                _userID = System.Convert.ToString(dt.Rows[0]["USERNAME"]);
                if(dt.Rows[0]["SUPERVISORID"] != DBNull.Value)
                    _supervisorID = System.Convert.ToInt32(dt.Rows[0]["SUPERVISORID"]);
                _title = System.Convert.ToString(dt.Rows[0]["TITLE"]);
                _firstName = System.Convert.ToString(dt.Rows[0]["FIRSTNAME"]);
                _middleName = System.Convert.ToString(dt.Rows[0]["MIDDLENAME"]);
                _lastName = System.Convert.ToString(dt.Rows[0]["LASTNAME"]);
                if(dt.Rows[0]["SITEID"] != DBNull.Value)
                    _siteID = System.Convert.ToInt32(dt.Rows[0]["SITEID"]);
                _department = System.Convert.ToString(dt.Rows[0]["DEPARTMENT"]);
                _telephone = System.Convert.ToString(dt.Rows[0]["TELEPHONE"]);
                _email = System.Convert.ToString(dt.Rows[0]["EMAIL"]);
                _address = System.Convert.ToString(dt.Rows[0]["ADDRESS"]);
                _active = System.Convert.ToBoolean(dt.Rows[0]["ISACTIVE"]);

                if(IsLDAPAuthentication())
                {
                    _isLDAP = true;
                    _isExempt = IsUserExempt(_userID);
                }
            }
            catch(System.Exception) { }
        }

        protected override void DataPortal_Update()
        {
            if(_coeDAL == null) { LoadDAL(); }
            Update(_coeDAL);
        }

        internal void Update(DAL coeDAL)
        {
            try
            {
                string rolesGranted = string.Empty;
                string rolesRevoked = string.Empty;
                foreach(COERoleBO role in this._roles)
                {
                    if(role.IsDeleted == false)
                    {
                        if(rolesGranted == string.Empty)
                        {
                            rolesGranted = role.RoleName;
                        }
                        else
                        {
                            rolesGranted = rolesGranted + "," + role.RoleName;
                        }
                    }
                }

                foreach(COERoleBO role in this._originalRoles)
                {
                    if(!this._roles.Contains(role))
                    {
                        if(rolesRevoked == string.Empty)
                        {
                            rolesRevoked = role.RoleName;
                        }
                        else
                        {
                            rolesRevoked = rolesRevoked + "," + role.RoleName;
                        }
                    }
                }
                if(this._isLDAP && this._isExempt == false)
                {
                    this._password = null;
                }
                else
                {
                    if(this._password == this._originalPassword)
                    {
                        this._password = null;
                    }
                }

                coeDAL.UpdateUser(this._userID, this._password, rolesGranted, rolesRevoked, this._firstName, this._middleName, this._lastName, this._telephone, this._email, this._address, this._userCode, this._supervisorID, this._siteID, this._active);
            }
            catch(Exception) { throw; }
        }


        protected override void DataPortal_Insert()
        {
            if(_coeDAL == null) { LoadDAL(); }
            Insert(_coeDAL);
        }

        internal void Insert(DAL coeDAL)
        {
            try
            {
                string rolesGranted = string.Empty;
                foreach(COERoleBO role in this._roles)
                {
                    if(rolesGranted == string.Empty)
                    {
                        rolesGranted = role.RoleName;
                    }
                    else
                    {
                        rolesGranted = rolesGranted + "," + role.RoleName;
                    }
                }
                //if (this._siteID == 0) { this._siteID = 1; }
                //if (this._supervisorID == 0) { this._supervisorID = 1; }
                if(this._isLDAP && this._isExempt == false)
                {
                    this._password = GenerateLDAPPassword(this._userID);
                    this._isDBMSUser = false;
                    coeDAL.CreateUser(this._userID, System.Convert.ToInt32(this._isDBMSUser), this._password, rolesGranted, this._firstName, this._middleName, this._lastName, this._telephone, this._email, this._address, this._userCode, this._supervisorID, this._siteID, this._active, _activatingUser);
                }
                else
                {
                    coeDAL.CreateUser(this._userID, System.Convert.ToInt32(this._isDBMSUser), this._password, rolesGranted, this._firstName, this._middleName, this._lastName, this._telephone, this._email, this._address, this._userCode, this._supervisorID, this._siteID, this._active, _activatingUser);
                    if(this._isLDAP && this._isExempt == true)
                    {//add exempt user
                        MakeUserExempt(this._userID, this._password);
                    }
                }
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                _activatingUser = false;
            }
        }

        private void DataPortal_Delete(Criteria criteria)
        {
            try
            {
                if(_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11624
	            if (_coeDAL != null)            
                    _coeDAL.DeleteUser(criteria._userName);
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
            catch(Exception)
            {
                throw;
            }
        }

        protected override void DataPortal_DeleteSelf()
        {
            try
            {
                if(_coeDAL == null) { LoadDAL(); }
                // Coverity Fix CID - 11625
                if (_coeDAL != null) 
                    DeleteSelf(_coeDAL);
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
            catch(Exception)
            {
                throw;
            }
        }

        internal void DeleteSelf(DAL coeDAL)
        {
            try
            {
                coeDAL.DeleteUser(this.UserID);
            }
            catch(Exception)
            {
                throw;
            }
        }


        private void LoadDAL()
        {
            if(_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
        }


        private bool IsLDAPAuthentication()
        {

            COEPrincipal principal = (COEPrincipal) Csla.ApplicationContext.User;
            COEIdentity myIdentity = (COEIdentity) principal.Identity;
            if(myIdentity.IsLDAP == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsUserExempt(string userName)
        {
            bool result = false;
            if(userName != null)
            {
                if(userName.Length > 0)
                {
                    coesinglesignon.SingleSignOn singleSignOn = new coesinglesignon.SingleSignOn();
                    singleSignOn.Url = ConfigurationUtilities.GetSingleSignOnURL();

                    try
                    {
                        result = Convert.ToBoolean(singleSignOn.IsExemptUser(userName));

                    }
                    catch(System.Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            return result;
        }

        private void MakeUserExempt(string userName, string passWord)
        {
            string userNameResult = string.Empty;
            if(userName != null)
            {
                if(userName.Length > 0)
                {
                    coesinglesignon.SingleSignOn singleSignOn = new coesinglesignon.SingleSignOn();
                    singleSignOn.Url = ConfigurationUtilities.GetSingleSignOnURL();

                    try
                    {
                        userNameResult = Convert.ToString(singleSignOn.AddExemptUser(userName, passWord, "CSSECURITY"));
                        if(userNameResult.Length == 0)
                        {
                            throw new System.Exception("Exempt User could not be added");
                        }
                    }
                    catch(System.Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Generates an uppercased password. This is mainly used by classic applications.
        /// Existing systems that uses Oracle 11 and LDAP will need migration to change the passwords to uppercase.
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>An uppercased autogenerated password</returns>
        private string GenerateLDAPPassword(string userName)
        {
            Array arr = userName.ToUpper().ToCharArray();
            Array.Reverse(arr); // reverse the string
            char[] c = (char[]) arr;
            string newString = (new string(c));

            return "7" + newString + "11C";
        }
        #endregion

        #region ChangePasswordCommand
        [Serializable]
        private class ChangePasswordCommand : CommandBase
        {
            #region Variables
            private string _userName = string.Empty;
            private string _oldPassword = string.Empty;
            private string _newPassword = string.Empty;
            private bool _success = false;
            [NonSerialized]
            private DAL _coeDAL = null;
            [NonSerialized]
            private DALFactory _dalFactory = new DALFactory();
            private string _serviceName = "COESecurity";
            #endregion

            #region Constructor
            public ChangePasswordCommand(string userName, string oldPassword, string newPassword)
            {
                _userName = userName;
                _oldPassword = oldPassword;
                _newPassword = newPassword;
            }
            #endregion

            #region Properties
            public bool Success
            {
                get { return _success; }
                set { _success = value; }
            }
            #endregion

            #region DataPortal Methods
            protected override void DataPortal_Execute()
            {
                try
                {
                    bool authenticated = false;
                    coesinglesignon.SingleSignOn singleSignOn = new coesinglesignon.SingleSignOn();
                    string url = ConfigurationUtilities.GetSingleSignOnURL();
                    if(url != string.Empty)
                    {
                        singleSignOn.Url = url;
                        authenticated = singleSignOn.Authenticate(_userName, _oldPassword);
                        if(authenticated == true)
                        {
                            COEUserBO myUser = COEUserBO.Get(_userName);
                            COEDatabaseName.Set(Resources.CentralizedStorageDB);
                            if(_coeDAL == null) { LoadDAL(); }
                            // Coverity Fix CID - 18762
                            if (_coeDAL != null)
                                this.Success = _coeDAL.ChangePassword(_userName, myUser._originalPassword, _newPassword);
                            else
                                throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
                        }
                        else
                        {
                            throw new InvalidCredentials("username or password is incorrect");
                        }
                    }
                }
                catch(Exception)
                {
                    throw;
                }
            }
            #endregion

            #region Private Methods
            private void LoadDAL()
            {
                if(_dalFactory == null) { _dalFactory = new DALFactory(); }
                _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
            }
            #endregion
        }
        #endregion

        public COEUserBO Activate()
        {
            _activatingUser = true;
            return this.Save();
        }
    }
}
