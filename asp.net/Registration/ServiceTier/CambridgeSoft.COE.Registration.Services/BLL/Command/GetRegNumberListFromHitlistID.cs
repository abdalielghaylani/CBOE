using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration.Access;

namespace CambridgeSoft.COE.Registration.Services.BLL
{
    [Serializable()]
    public class GetRegNumberListFromHitlistID : RegistrationCommandBase
    {
        public static int HitListID;

        #region Authorization Methods
        public static bool CanExecuteCommand()
        {
            // TODO: customize to check user role
            //return ApplicationContext.User.IsInRole("");
            return true;
        }

        #endregion

        #region Client-side Code

        // TODO: add your own fields and properties
        string _result;

        public string Result
        {
            get { return _result; }
        }

        private void BeforeServer()
        {
            // TODO: implement code to run on client
            // before server is called
        }

        private void AfterServer()
        {
            // TODO: implement code to run on client
            // after server is called
        }

        #endregion

        #region Factory Methods

        [COEUserActionDescription("GetRegNumberListFromHitlistID")]
        public static string Execute()
        {
            try
            {
                GetRegNumberListFromHitlistID cmd = new GetRegNumberListFromHitlistID();
                cmd.BeforeServer();
                cmd = DataPortal.Execute<GetRegNumberListFromHitlistID>(cmd);
                cmd.AfterServer();
                return cmd.Result;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        private GetRegNumberListFromHitlistID()
        { /* require use of factory methods */ }

        #endregion

        #region Server-side Code

        protected override void DataPortal_Execute()
        {
            _result = this.RegDal.GetRegIDListFromHitList(HitListID);
        }

        #endregion
    }
}
