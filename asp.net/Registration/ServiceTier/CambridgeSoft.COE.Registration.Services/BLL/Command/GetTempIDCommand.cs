using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Registration.Services.Common;
using CambridgeSoft.COE.Registration.Access;

namespace CambridgeSoft.COE.Registration.Services.Types {
    [Serializable()]
    public class GetTempIDCommand : CommandBase {

        private string _result;
        
        public string Result {
            get { return _result; }
        }
        

        #region Authorization Methods
        public static bool CanExecuteCommand() {
            // TODO: Determine which roles can execute this command. (GetTempIDCommand)
            //return ApplicationContext.User.IsInRole("");
            return true;
        }
        #endregion

        #region Factory Methods

        [COEUserActionDescription("GetTempID")]
        public static string Execute() {
            try
            {
                GetTempIDCommand cmd = new GetTempIDCommand();
                //cmd.BeforeServer();
                if(CanExecuteCommand())
                    cmd = DataPortal.Execute<GetTempIDCommand>(cmd);
                //cmd.AfterServer();
                return cmd.Result;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        private GetTempIDCommand() { /* require use of factory methods */ }

        #endregion

        #region Server-side Code

        [NonSerialized, NotUndoable]
        private RegistrationOracleDAL _regDal = null;
        /// <summary>
        /// DAL implementation.
        /// </summary>
        private RegistrationOracleDAL RegDal
        {
            get
            {
                if (_regDal == null)
                    DalUtils.GetRegistrationDAL(ref _regDal, Constants.SERVICENAME);
                return _regDal;
            }
        }

        protected override void DataPortal_Execute() {
            _result = RegDal.GetTemporalID();
        }
        #endregion

        

    }
}
