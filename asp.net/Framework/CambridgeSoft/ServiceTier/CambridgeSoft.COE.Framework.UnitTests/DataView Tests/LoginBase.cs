using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COESecurityService;

namespace DataViewUnitTests
{
    public class LoginBase
    {
        #region Variables
        //private string _pathToXmls = Utilities.GetProjectBasePath("CambridgeSoft.COE.Framework.UnitTests") + @"\TestXML";
        //private string _databaseName = "SAMPLE";
        const string USERNAME = "cssadmin";
        const string PASSWORD = "cssadmin";
        //private DALFactory _dalFactory = new DALFactory();
        //private TestContext _testContextInstance;
        #endregion

        public LoginBase()
        {
            COEPrincipal.Logout();
            System.Security.Principal.IPrincipal user = Csla.ApplicationContext.User;
            bool result = COEPrincipal.Login(USERNAME, PASSWORD);
        }
    }
}
