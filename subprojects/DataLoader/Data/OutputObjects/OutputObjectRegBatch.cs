using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.DataLoader.Common;

namespace CambridgeSoft.COE.DataLoader.Data.OutputObjects
{
    /// <summary>
    /// <see cref="OutputObject"/> for adding and updating batches in registration
    /// </summary>
    class OutputObjectRegBatch : OutputObjectForm
    {
        private const string _fmtNoSuchBatchId_1 = "No such batch ID as {0:G}";
        private const string _fmtUpdatedPendingReview_1 = "Updated pending review batch ID {0:G}";
        private const string _fmtUpdatedRegistered_2 = "Updated registered batch ID {0:G} number {0:G}";
        private const string _fmtUpdatedRegistered_3 = "Batch ID {0:G} was Locked, Not Updated";
        private const string _batchBindingRoot = "batchlistcsladatasource";
        private Batch _oBatch;
        protected RegistryRecord _objregistryRecord;
        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OutputObjectRegBatch()
        {
            OutputType = "Update batch data in registration";

            bool hasRegTempPermission = false;
            bool hasRegPermPermission = false;

            if (Csla.ApplicationContext.User.Identity.IsAuthenticated == true)
            {
                hasRegTempPermission = 
                    Csla.ApplicationContext.User.IsInRole("ADD_COMPOUND_TEMP");

                hasRegPermPermission = (
                    Csla.ApplicationContext.User.IsInRole("ADD_COMPONENT")
                    || Csla.ApplicationContext.User.IsInRole("EDIT_COMPOUND_REG")
                    || Csla.ApplicationContext.User.IsInRole("ADD_COMPOUND_TEMP")
                );

                if ((hasRegTempPermission == false) && (hasRegPermPermission == false))
                {
                    string message = "Not logged in or user has insufficient privileges.";
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Output, -1, message);
                }
                else
                {
                    IsValid = true;
                }

                //JED: 03-MAR-2010
                //This feature cannot be repaired in the time allotted so it is suspended at David G's request
                //IsValid = false;

            }
            else
            {
                string message = "Not valid or no user is logged in.";
                AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Output, -1, message);
            }

            WritePermittedActionsConfiguration(hasRegTempPermission, hasRegPermPermission);
        }

        /// <summary>
        /// Requested registry batch action
        /// </summary>
        private enum RegistryBatchAction
        {
            /// <summary>
            /// Temporary pending review
            /// </summary>
            Temp,
            /// <summary>
            /// Register
            /// </summary>
            Reg,
        };

        public override string Configuration
        {
            set
            {
                ConfigurationSet(value, 4011, CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.DisplayMode.Edit, "Batch ID", "Integer;;map|calculation", string.Empty);
                return;
            } // set
        } // Configuration

        private RegistryBatchAction _eRegistryBatchAction = RegistryBatchAction.Temp;
        private RegistryBatchAction RegBatchAction
        {
            get
            {
                return _eRegistryBatchAction;
            }
            set
            {
                _eRegistryBatchAction = value;
                return;
            }
        } // RegBatchAction

        #region >Methods<

        protected override bool BindStart(int vnTransaction, DataRow voOutputDataRow, int totalRows)
        {
            try
            {
                string batchIdColumnName = _batchBindingRoot + ":" + "Batch ID";
                int batchId = Convert.ToInt32(voOutputDataRow[batchIdColumnName.ToUpper()].ToString());
                _oBatch = Batch.GetBatch(batchId, _eRegistryBatchAction == RegistryBatchAction.Temp);   // WJC Hard-coded Batch ID field
                if (_oBatch == null)
                {
                    OutputObjectForm.batchId = "NoBatchID";
                    OutputObjectForm.batchOutputDataRow = voOutputDataRow[batchIdColumnName].ToString();
                    return true;
                }
                if (OutputType.ToString() == "Update batch data in registration")
                {
                    _objregistryRecord = RegistryRecord.GetRegistryRecordByBatch(batchId);
                    if (_objregistryRecord.Status.ToString() == "Locked")
                     {
                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Output, vnTransaction, _fmtUpdatedRegistered_3, voOutputDataRow[batchIdColumnName].ToString());
                        return true;
                      }
                                      
                }
            }
            catch (Exception ex)
            {
            }
            return (_oBatch == null);
        } // BindStart()

        protected override bool BindAble(string vKey)
        {
            bool bRet = false; // Need reflection to get the right object
            bRet |= (vKey.StartsWith("Batch"));
            return bRet;
        } // BindAble(string vKey)

        protected override Object BindObject(string vKey)
        {
            Object obj = null; // Need reflection to get the right object
            if (vKey == _batchBindingRoot + "List") obj = _oBatch;
            if (vKey == _batchBindingRoot + "List" + "@IdentifierList") obj = _oBatch.IdentifierList;
            return obj;
        } // BindObject(string vKey)

        // TOTAL KLUGE FOR DAVE LEVY DEMO SLICE
        protected override void BindKluge(string vKey, Dictionary<string, string> vBindings)
        {
            return;
        } // BindKluge()

        protected override void BindWrite(int vnTransaction)
        {
            Batch retBatch;
            {
                try
                {
                    retBatch = _oBatch.Save();
                    if (retBatch.IsValid)
                    {
                        if (retBatch.IsTemporal)
                        {
                            AddMessage(LogMessage.LogSeverity.Information, LogMessage.LogSource.Output, vnTransaction, _fmtUpdatedPendingReview_1, retBatch.ID.ToString());
                        }
                        else
                        {
                            AddMessage(LogMessage.LogSeverity.Information, LogMessage.LogSource.Output, vnTransaction, _fmtUpdatedRegistered_2, retBatch.ID.ToString(), retBatch.BatchNumber.ToString());
                        }
                    }
                    else
                    {
                        AddMessage(LogMessage.LogSeverity.Warning, LogMessage.LogSource.Output, vnTransaction, "internal error needs reporting) - _oBatch.Save()");
                    }
                }
                catch (Exception ex)
                {
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Output, vnTransaction, _fmtException_2, "Batch.Save", ex.ToString());
                }
            }
            return;
        } // BindWrite()

        private void WritePermittedActionsConfiguration(bool hasRegTempPermission, bool hasRegPermPermission)
        {
            if (hasRegTempPermission == true || hasRegPermPermission == true)
            {
                COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
                oCOEXmlTextWriter.WriteStartElement("OutputConfiguration");
                oCOEXmlTextWriter.WriteAttributeString("text", "Configuration");
                {
                    oCOEXmlTextWriter.WriteStartElement("GroupBox");
                    oCOEXmlTextWriter.WriteAttributeString("text", "Edit batches in");
                    oCOEXmlTextWriter.WriteAttributeString("member", "_eRegistryBatchAction");
                    {
                        if (hasRegTempPermission == true || hasRegPermPermission == true)
                        {
                            oCOEXmlTextWriter.WriteStartElement("RadioButton");
                            oCOEXmlTextWriter.WriteAttributeString("text", "Pending review");
                            oCOEXmlTextWriter.WriteEndElement();
                        }
                        if (hasRegPermPermission)
                        {
                            oCOEXmlTextWriter.WriteStartElement("RadioButton");
                            oCOEXmlTextWriter.WriteAttributeString("text", "Registered");
                            oCOEXmlTextWriter.WriteEndElement();
                        }
                    }
                    oCOEXmlTextWriter.WriteEndElement();
                }
                oCOEXmlTextWriter.WriteEndElement();

                UnboundConfiguration = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
                oCOEXmlTextWriter.Close();
            }

        }

        #endregion

    }
}
