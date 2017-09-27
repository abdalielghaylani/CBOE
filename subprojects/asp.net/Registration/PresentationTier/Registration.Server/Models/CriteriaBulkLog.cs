using System;
using Csla;
using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Registration.Access;

namespace PerkinElmer.COE.Registration.Server.Models
{
    [Serializable]
    public class CriteriaBulkLog
    {
        [NonSerialized, NotUndoable]
        private RegistrationOracleDAL coeRegistrationDAL = null;

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

        private string strLogDescription;

        public string StrLogDescription
        {
            get { return strLogDescription; }
            set { strLogDescription = value; }
        }

        private string strtempID;

        public string StrTempID
        {
            get { return strtempID; }
            set { strtempID = value; }
        }

        private string strregNumber;

        public string StrRegNumber
        {
            get { return strregNumber; }
            set { strregNumber = value; }
        }

        private int batchID;

        public int BatchID
        {
            get { return batchID; }
            set { batchID = value; }
        }

        public CriteriaBulkLog()
        {
        }

        public CriteriaBulkLog(int logId, string tempID, DuplicateAction strDuplicateAction, string regNumber, int batchID, string strLogDescription)
        {
            this.logId = logId;
            this.strDuplicateAction = strDuplicateAction;
            this.strLogDescription = strLogDescription;
            this.strtempID = tempID;
            this.strregNumber = regNumber;
            this.batchID = batchID;
            LoadDAL();
        }

        public static void InsertCriteriaBulkLog(int logId, string tempId, DuplicateAction strDuplicateAction, string regNumber, int batchId, string strLogDescription)
        {
            CriteriaBulkLog tmpCriteriaBulkLog = new CriteriaBulkLog(logId, tempId, strDuplicateAction, regNumber, batchId, strLogDescription);
            DataPortal.Update(tmpCriteriaBulkLog);
        }

        private void LoadDAL()
        {
            if (coeRegistrationDAL == null)
                DalUtils.GetRegistrationDAL(ref coeRegistrationDAL, Constants.SERVICENAME);
        }

        private void DataPortal_Update()
        {
            coeRegistrationDAL.LogBulkRegistration(logId, strtempID, strDuplicateAction, strregNumber, batchID, strLogDescription);
        }
    }
}