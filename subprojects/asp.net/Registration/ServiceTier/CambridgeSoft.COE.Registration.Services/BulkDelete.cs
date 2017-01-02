using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Csla;

using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;

using CambridgeSoft.COE.Registration.Services;
using CambridgeSoft.COE.Registration.Services.Common;
using CambridgeSoft.COE.Registration.Services.Types;

namespace CambridgeSoft.COE.Registration.Services
{
    /// <summary>
    /// This Csla Command object supports the deletion of mulsiplt search result records via a 'hit-list'
    /// ID. All records currently associated with that hitlist will be deleted.
    /// </summary>
    [Serializable()]
    public class BulkDelete : RegistrationCommandBase
    {
        int _hitListId;

        string[] _failedRecords;
        /// <summary>
        /// Intended to provide error information for failed deletions.
        /// </summary>
        public string[] FailedRecords
        {
            get { return _failedRecords; }
        }

        bool _result;
        public bool Result
        {
            get { return _result; }
        }

        bool _isTemporary;
        /// <summary>
        /// Whether the RegistryRecord objects references are temporary registrations or not.
        /// </summary>
        public bool IsTemporary
        {
            get { return _isTemporary; } 
            set { _isTemporary = value; }
        }

        int _logId;
        public int LogId
        {
            get { return _logId; }
        }

        string _logDescription;
        public string LogDescription
        {
            get { return _logDescription; }
            set { _logDescription = value; }
        }

        /// <summary>
        /// The static executor for the Csla Command - provides the parameters for the execution itself.
        /// </summary>
        /// <param name="hitListId">the ID of the hit-list of Registrations</param>
        /// <param name="isTemporal">indicator of Temporary Registrations</param>
        /// <param name="logDescription"></param>
        /// <returns></returns>
        public static BulkDelete Execute(int hitListId, bool isTemporal, string logDescription)
        {
            try
            {
                BulkDelete cmd = new BulkDelete(hitListId, isTemporal, logDescription);
                cmd = DataPortal.Execute<BulkDelete>(cmd);
                return cmd;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        public static bool Execute(int hitListId, bool isTemporal, string logDescription, out int logId)
        {
            bool result = false;
            logId = -1;
            try
            {
                BulkDelete cmd = new BulkDelete(hitListId, isTemporal, logDescription);
                cmd = DataPortal.Execute<BulkDelete>(cmd);

                logId = cmd._logId;
                result = cmd._result;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
            return result;
        }

        private BulkDelete(int hitListId, bool isTemporal, string logDescription)
        {
            this._hitListId = hitListId;
            this._isTemporary = isTemporal;
            _logDescription = logDescription;
        }

        protected override void DataPortal_Execute()
        {
            string ids = string.Empty;
            string idsTemp = string.Empty;
            List<string> failedTempRecords=new List<string>();
            if (_isTemporary)
            {
                idsTemp = this.RegDal.GetRegistryRecordTemporaryIdList(_hitListId);
            }
            else
            {
                idsTemp = this.RegDal.GetRegistryRecordsFromHitList(_hitListId);

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(idsTemp);

                StringBuilder builder = new StringBuilder();
                foreach (XmlNode currentNode in xmlDocument.DocumentElement.SelectNodes("REGNUMBER"))
                {
                    if (!string.IsNullOrEmpty(currentNode.InnerXml))
                    {
                        if (!string.IsNullOrEmpty(builder.ToString()))
                            builder.Append(", ");

                        builder.Append(currentNode.InnerXml);
                    }
                }

                idsTemp = builder.ToString();
            }
            string[] idsTempArray = idsTemp.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string idtemp in idsTempArray)
            {
                RegistryRecord tempRegRecord =(_isTemporary?RegistryRecord.GetRegistryRecord(Convert.ToInt32(idtemp)):RegistryRecord.GetRegistryRecord(idtemp));
                if (tempRegRecord.CanEditRegistry())
                {
                    ids = (string.IsNullOrEmpty(ids) ? idtemp : ids + "," + idtemp);
                }
                else
                    failedTempRecords.Add(idtemp);
            }


            string[] failedDeletions;
            string[] idArray = ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (idArray.Length > 0)
                _logId = this.RegDal.BulkDelete(ids, _logDescription, this._isTemporary, out failedDeletions);
            else
                failedDeletions = new string[0];
            if (failedTempRecords.Count > 0)
            {
                foreach (string failedDel in failedDeletions)
                {
                    failedTempRecords.Add(failedDel);
                }
                _failedRecords = failedTempRecords.ToArray();
            }
            else
                _failedRecords = failedDeletions;

            COEHitListBO.Get(CambridgeSoft.COE.Framework.HitListType.MARKED, _hitListId).UnMarkAllHits();

            _result = idArray.Length > failedDeletions.Length;
        }
    }
}
