using System;
using Csla;
using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Registration.Access;

namespace PerkinElmer.COE.Registration.Server.Models
{
    [Serializable]
    public class CriteriaLog
    {
        [NonSerialized, NotUndoable]
        private RegistrationOracleDAL coeRegistrationDAL = null;

        private bool isUpdateDescription = false;

        [NonSerialized]
        private int logId;

        public int LogId
        {
            get { return logId; }
            set { logId = value; }
        }

        private DuplicateAction strDuplicateAction;

        public DuplicateAction StrDuplicateAction
        {
            get { return strDuplicateAction; }
            set { strDuplicateAction = value; }
        }

        private string strUserId;

        public string StrUserId
        {
            get { return strUserId; }
            set { strUserId = value; }
        }

        private string strLogDescription;

        public string StrLogDescription
        {
            get { return strLogDescription; }
            set { strLogDescription = value; }
        }

        public CriteriaLog(int logId)
        {
            this.logId = logId;
        }

        public CriteriaLog(int logId, string strLogDescription)
        {
            this.isUpdateDescription = true;
            this.logId = logId;
            this.strLogDescription = strLogDescription;
            LoadDAL();
        }

        public CriteriaLog(DuplicateAction strDuplicateAction, string strUserId, string strLogDescription)
        {
            this.isUpdateDescription = false;
            this.strDuplicateAction = strDuplicateAction;
            this.strUserId = strUserId;
            this.strLogDescription = strLogDescription;
            LoadDAL();
        }

        public static void SetCriteriaLog(int logId, string strLogDescription)
        {
            DataPortal.Update(new CriteriaLog(logId, strLogDescription));
        }

        public static int InsertCriteriaLog(DuplicateAction strDuplicateAction, string strUserId, string strLogDescription)
        {
            CriteriaLog tmpCriteriaLog = new CriteriaLog(strDuplicateAction, strUserId, strLogDescription);
            DataPortal.Update(tmpCriteriaLog);
            return tmpCriteriaLog.LogId;
        }

        private void LoadDAL()
        {
            if (coeRegistrationDAL == null)
                DalUtils.GetRegistrationDAL(ref coeRegistrationDAL, Constants.SERVICENAME);
        }

        private void DataPortal_Update()
        {
            if (isUpdateDescription)
                coeRegistrationDAL.UpdateLogInfo(logId, strLogDescription);
            else
                logId = coeRegistrationDAL.InsertLogInfo(strDuplicateAction, strUserId, strLogDescription);
        }
    }

}