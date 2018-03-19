using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using Microsoft.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger.Annotations;
using PerkinElmer.COE.Registration.Server.Code;
using PerkinElmer.COE.Registration.Server.Models;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Registration;
using Csla.Validation;
using CambridgeSoft.COE.Framework.Common.Validation;
using Resources;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class BatchController : RegControllerBase
    {
        [HttpPost]
        [Route(Consts.apiPrefix + "batches")]
        [SwaggerOperation("AddBatchToRecord")]
        [SwaggerResponse(HttpStatusCode.Created, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> AddBatchToRecord(BatchData batchData)
        {
            return await CallMethod(() =>
            {
                string errorMessage = null;
                RegistryRecord registryRecord = null;
                try
                {
                    errorMessage = "Unable to parse the incoming data as a well-formed XML document.";
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(batchData.Data);
                    registryRecord = RegistryRecord.GetRegistryRecord(batchData.RegNum);
                    if (registryRecord == null) throw new Exception("Registration record not found");
                    errorMessage = "Record is locked and cannot be updated.";
                    if (registryRecord.Status == RegistryStatus.Locked) throw new Exception();

                    registryRecord.BeginEdit();
                    registryRecord.AddBatch();
                    registryRecord.ApplyEdit();

                    errorMessage = "Unable to update the internal record.";
                    registryRecord.BatchList.UpdateFromXmlEx(xmlDoc.FirstChild);

                    // Fix for CBOE-6440 Add Batch: FW, MW and Percent Active values are not calculated correctly for newly added batches (SBI-True)
                    registryRecord.FixBatchesFragmentsEx();

                    if (registryRecord.BatchList.Count > 0)
                    {
                        CambridgeSoft.COE.Registration.Services.Types.Batch firstBatch = registryRecord.BatchList[0];
                        List<CambridgeSoft.COE.Registration.Services.Types.Batch> modifiedBatchList = (from bl in registryRecord.BatchList
                                                                                                       where bl.IsDirty == true
                                                                                                       select bl).ToList();
                        registryRecord.BatchList.Clear();
                        if (modifiedBatchList != null && modifiedBatchList.Count > 0)
                        {
                            foreach (CambridgeSoft.COE.Registration.Services.Types.Batch batchItem in modifiedBatchList)
                            {
                                registryRecord.BatchList.Add(batchItem);
                            }
                        }
                        else
                        {
                            // keep at least one batch when there is no modification in batch to avoid exception
                            registryRecord.BatchList.Add(firstBatch);
                        }
                    }

                    errorMessage = "Cannot set batch prefix.";
                    registryRecord.BatchPrefixDefaultOverride(true);
                    errorMessage = "Unable to save the record.";
                    registryRecord.ModuleName = ChemDrawWarningChecker.ModuleName.REGISTRATION;
                    registryRecord.CheckOtherMixtures = false;
                    registryRecord = registryRecord.Save(DuplicateCheck.None);

                    var xml = registryRecord.XmlWithAddIns;
                    var recordXml = new XmlDocument();
                    recordXml.LoadXml(xml);
                    XmlElement ele = (XmlElement)recordXml.SelectSingleNode("/MultiCompoundRegistryRecord/BatchList/Batch[last()]");
                    JObject data = new JObject(new JProperty("data", ele.OuterXml));
                    return new ResponseData(regNumber: registryRecord.RegNumber.RegNum, data: data);
                }
                catch (Exception ex)
                {
                    if (registryRecord != null && ex is ValidationException)
                    {
                        List<BrokenRuleDescription> brokenRuleDescriptionList = registryRecord.GetBrokenRulesDescription();
                        brokenRuleDescriptionList.ForEach(rd =>
                        {
                            errorMessage += "\n" + string.Join(", ", rd.BrokenRulesMessages);
                        });
                    }
                    if (ex is RegistrationException)
                    {
                        if (errorMessage.EndsWith(".")) errorMessage.Substring(0, errorMessage.Length - 1);
                        errorMessage += " - " + ex.Message;
                    }
                    throw new RegistrationException(errorMessage, ex);
                }
            }, new string[] { "ADD_BATCH_PERM" });
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "batches")]
        [SwaggerOperation("UpdateBatchRecord")]
        [SwaggerResponse(HttpStatusCode.Created, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> UpdateBatchRecord(BatchData batchData)
        {
            return await CallMethod(() =>
            {
                string errorMessage = null;
                RegistryRecord registryRecord = null;
                try
                {
                    errorMessage = "Unable to parse the incoming data as a well-formed XML document.";
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(batchData.Data);
                    registryRecord = RegistryRecord.GetRegistryRecord(batchData.RegNum);
                    if (registryRecord == null) throw new Exception("Registration record not found");
                    errorMessage = "Record is locked and cannot be updated.";
                    if (registryRecord.Status == RegistryStatus.Locked) throw new Exception();

                    errorMessage = "Unable to update the internal record.";
                    registryRecord.BatchList.UpdateFromXmlEx(xmlDoc.FirstChild);

                    registryRecord.FixBatchesFragmentsEx();

                    errorMessage = "Cannot set batch prefix.";
                    registryRecord.BatchPrefixDefaultOverride(true);
                    errorMessage = "Unable to save the record.";
                    registryRecord.ModuleName = ChemDrawWarningChecker.ModuleName.REGISTRATION;
                    registryRecord.CheckOtherMixtures = false;
                    registryRecord = registryRecord.Save(DuplicateCheck.None);

                    return new ResponseData(regNumber: registryRecord.RegNumber.RegNum);
                }
                catch (Exception ex)
                {
                    if (registryRecord != null && ex is ValidationException)
                    {
                        List<BrokenRuleDescription> brokenRuleDescriptionList = registryRecord.GetBrokenRulesDescription();
                        brokenRuleDescriptionList.ForEach(rd =>
                        {
                            errorMessage += "\n" + string.Join(", ", rd.BrokenRulesMessages);
                        });
                    }
                    if (ex is RegistrationException)
                    {
                        if (errorMessage.EndsWith(".")) errorMessage.Substring(0, errorMessage.Length - 1);
                        errorMessage += " - " + ex.Message;
                    }
                    throw new RegistrationException(errorMessage, ex);
                }
            });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "batches/{id}")]
        [SwaggerOperation("MoveBatchRecord")]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> MoveBatchRecord(int id, MoveBatchData data)
        {
            return await CallMethod(() =>
            {
                RegistryRecord sourceRegistryRecord = null;
                try
                {
                    sourceRegistryRecord = RegistryRecord.GetRegistryRecord(data.SourceRegNum);
                    if (sourceRegistryRecord == null)
                        throw new RegistrationException(string.Format("cannot find registry record {0}", data.SourceRegNum));

                    RegistryRecord targetRegistryRecord = RegistryRecord.GetRegistryRecord(data.TargetRegNum);
                    if (targetRegistryRecord == null)
                        throw new RegistrationException(string.Format("the registry number {0} doesent exist", data.TargetRegNum));

                    if (id == 0)
                        throw new RegistrationException(string.Format("batch id '{0}' is not valid", id));

                    CambridgeSoft.COE.Registration.Services.Types.Batch.MoveBatch(id, data.TargetRegNum);

                    sourceRegistryRecord.BatchList.Remove(sourceRegistryRecord.BatchList.GetBatchById(id));
                }
                catch (Exception ex)
                {
                    Exception baseEx = ex.GetBaseException();
                    bool moveBatchSuccss = false;
                    if (baseEx != null)
                    {
                        if (baseEx.Message.Contains("Source_Destination_Match"))
                        {
                            throw new RegistrationException(Resource.SourceAndDestinationMatch_ErrorMessage);
                        }
                        else if (baseEx.Message.Contains("ORA-20030"))
                        {
                            throw new RegistrationException(Resource.CannotMoveBatch_Label_Text);
                        }
                        else if (baseEx.Message.Contains("ORA-20037") || baseEx.Message.Contains("ORA-20038") || baseEx.Message.Contains("ORA-20035"))
                        {
                            if (sourceRegistryRecord != null)
                            {
                                sourceRegistryRecord.BatchList.Remove(sourceRegistryRecord.BatchList.GetBatchById(id));
                                moveBatchSuccss = true;
                            }
                        }
                        else if (baseEx.Message.Contains("ORA-20031"))
                        {
                            throw new RegistrationException(string.Format("the target registry number '{0}' does not exist", data.TargetRegNum));
                        }
                    }
                    if (!moveBatchSuccss)
                        throw new RegistrationException(ex.Message);
                }
                return new ResponseData(message: string.Format("The batch was successfully moved into Registry Record {0}!", data.TargetRegNum));
            }, new string[] { "DELETE_BATCH_REG" });
        }

        [HttpDelete]
        [Route(Consts.apiPrefix + "batches/{id}")]
        [SwaggerOperation("DeleteBatch")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> DeleteBatch(int id)
        {
            return await CallMethod(() =>
            {
                CambridgeSoft.COE.Registration.Services.Types.Batch.DeleteBatch(id);
                return new ResponseData(message: string.Format("The batch #{0} was deleted successfully!", id));

            }, new string[] { "DELETE_BATCH_REG" });
        }
    }
}
