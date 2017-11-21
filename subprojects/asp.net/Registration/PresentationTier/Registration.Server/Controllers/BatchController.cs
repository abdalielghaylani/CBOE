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
        protected static RecordColumn[] BatchRecordColumns
        {
            get
            {
                return new RecordColumn[]
                {
                    new RecordColumn { Definitions = "BATCHID", Sortable = true },
                    new RecordColumn { Definitions = "TEMPBATCHID", Sortable = true }, 
                    new RecordColumn { Definitions = "BATCHNUMBER", Sortable = true }, 
                    new RecordColumn { Definitions = "FULLREGNUMBER", Sortable = true }, 
                    new RecordColumn { Definitions = "DATECREATED", Sortable = true }, 
                    new RecordColumn { Definitions = "PERSONCREATED", Sortable = true }, 
                    new RecordColumn { Definitions = "PERSONREGISTERED", Sortable = true }, 
                    new RecordColumn { Definitions = "PERSONAPPROVED", Sortable = true }, 
                    new RecordColumn { Definitions = "DATELASTMODIFIED", Sortable = true }
                };
            }
        }

        /// <summary>
        /// Returns the list of all batches
        /// </summary>
        /// <remarks>Returns the list of all batches corresponding to the specified parameters
        /// </remarks>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid request</response>
        /// <returns>A promise to return an <see cref="IHttpActionResult"/> object</returns>
        [HttpGet]
        [Route(Consts.apiPrefix + "batches")]
        [SwaggerOperation("GetAllBatches")]
        [SwaggerResponse(200, type: typeof(JArray))]
        [SwaggerResponse(400, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetAllBatches()
        {
            return await CallMethod(() =>
           {
               var data = ExtractData("SELECT BATCHID, TEMPBATCHID, BATCHNUMBER, FULLREGNUMBER, DATECREATED, PERSONCREATED, PERSONREGISTERED, PERSONAPPROVED, DATELASTMODIFIED FROM REGDB.VW_BATCH");
               if (data.Count() == 0)
                   throw new IndexOutOfRangeException(string.Format("Cannot find the Batches."));
               return data;
           });
        }

        /// <summary>
        /// Returns the list of all batches for the specified registration ID
        /// </summary>
        /// <response code="200">Successful operation</response>
        /// <param name="id">The ID of the registration record</param>
        /// <param name="skip">The number of items to skip</param>
        /// <param name="count">The maximum number of items to return</param>
        /// <param name="sort">The sorting information</param>
        /// <returns>The promise to return an <see cref="IHttpActionResult"/> object</returns>
        [HttpGet]
        [Route(Consts.apiPrefix + "batches/{id}/records")]
        [SwaggerOperation("GetBatches")]
        [SwaggerResponse(200, type: typeof(List<JObject>))]
        public async Task<IHttpActionResult> GetBatches(int? id = null, int? skip = null, int? count = null, string sort = null)
        {
            CheckAuthentication();
            var tableName = "VW_BATCH";
            var whereClause = " WHERE REGID=:regId";
            var query = GetQuery(tableName + whereClause, BatchRecordColumns, sort, "BATCHID", "TEMPBATCHID");
            var args = new Dictionary<string, object>();
            args.Add(":regId", id);
            var responseMessage = Request.CreateResponse(HttpStatusCode.OK, new JObject(
                new JProperty("batch", false),
                new JProperty("regId", id),
                new JProperty("totalCount", Convert.ToInt32(ExtractValue("SELECT cast(count(1) as int) c FROM " + tableName + whereClause, args))),
                new JProperty("startIndex", skip == null ? 0 : Math.Max(skip.Value, 0)),
                new JProperty("rows", ExtractData(query, args, skip, count))
            ));

            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }

        /// <summary>
        /// Returns the list of all batches by BatchID
        /// </summary>
        /// <param name="id">Batch ID</param>
        /// <returns>The array of batch objects</returns>
        [HttpGet]
        [Route(Consts.apiPrefix + "batches/{id}")]
        [SwaggerOperation("GetBatch")]
        [SwaggerResponse(200, type: typeof(JArray))]
        [SwaggerResponse(404, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetBatch(int id)
        {
            return await CallMethod(() =>
           {
               var args = new Dictionary<string, object>();
               args.Add(":Id", id);
               var data = ExtractData("SELECT BATCHID, TEMPBATCHID, BATCHNUMBER, FULLREGNUMBER, DATECREATED, PERSONCREATED, PERSONREGISTERED, PERSONAPPROVED, DATELASTMODIFIED FROM REGDB.VW_BATCH WHERE BATCHID=:Id", args);
               if (data.Count() == 0)
                   throw new IndexOutOfRangeException(string.Format("Cannot find the batch number, {0}", id));
               return data;
           });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "records/{regNum}/batches")]
        [SwaggerOperation("AddBatchToRecord")]
        [SwaggerResponse(HttpStatusCode.Created, type: typeof(ResponseData))]
        [SwaggerResponse(HttpStatusCode.BadRequest, type: typeof(JObject))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, type: typeof(JObject))]
        public async Task<IHttpActionResult> AddBatchToRecord(string regNum, BatchData batchData)
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
                    registryRecord = RegistryRecord.GetRegistryRecord(regNum);
                    if (registryRecord == null) throw new Exception();
                    errorMessage = "Record is locked and cannot be updated.";
                    if (registryRecord.Status == RegistryStatus.Locked) throw new Exception();

                    registryRecord.BeginEdit();
                    registryRecord.AddBatch();
                    registryRecord.ApplyEdit();

                    errorMessage = "Unable to update the internal record.";
                    registryRecord.BatchList.UpdateFromXmlEx(xmlDoc.FirstChild);
                  
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
            }, new string[] { "ADD_BATCH_PERM" });
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "records/{regNum}/batches")]
        [SwaggerOperation("UpdateBatchRecord")]
        [SwaggerResponse(HttpStatusCode.Created, type: typeof(ResponseData))]
        [SwaggerResponse(HttpStatusCode.BadRequest, type: typeof(JObject))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, type: typeof(JObject))]
        public async Task<IHttpActionResult> UpdateBatchRecord(string regNum, BatchData batchData)
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
                    registryRecord = RegistryRecord.GetRegistryRecord(regNum);
                    if (registryRecord == null) throw new Exception();
                    errorMessage = "Record is locked and cannot be updated.";
                    if (registryRecord.Status == RegistryStatus.Locked) throw new Exception();

                    errorMessage = "Unable to update the internal record.";
                    registryRecord.BatchList.UpdateFromXmlEx(xmlDoc.FirstChild);

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
        [Route(Consts.apiPrefix + "batches/{id}/{sourceRegNum}/{targetRegNum}")]
        [SwaggerOperation("MoveBatchRecord")]
        [SwaggerResponse(HttpStatusCode.Created, type: typeof(ResponseData))]
        [SwaggerResponse(HttpStatusCode.BadRequest, type: typeof(JObject))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, type: typeof(JObject))]
        public async Task<IHttpActionResult> MoveBatchRecord(int id, string sourceRegNum, string targetRegNum)
        {
            return await CallMethod(() =>
            {
                RegistryRecord sourceRegistryRecord = null;
                try
                {
                    sourceRegistryRecord = RegistryRecord.GetRegistryRecord(sourceRegNum);
                    if (sourceRegistryRecord == null)
                        throw new RegistrationException(string.Format("cannot find registry record {0}", sourceRegNum));

                    RegistryRecord targetRegistryRecord = RegistryRecord.GetRegistryRecord(targetRegNum);
                    if (targetRegistryRecord == null)
                        throw new RegistrationException(string.Format("the registry number {0} doesent exist", targetRegNum));

                    if (id == 0)
                        throw new RegistrationException(string.Format("batch id '{0}' is not valid", targetRegNum));

                    CambridgeSoft.COE.Registration.Services.Types.Batch.MoveBatch(id, targetRegNum);

                    sourceRegistryRecord.BatchList.Remove(sourceRegistryRecord.BatchList.GetBatchById(id));
                }
                catch (Exception ex)
                {
                    Exception baseEx = ex.GetBaseException();
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
                            }
                        }
                    }
                }
                return new ResponseData(message: string.Format("The batch was successfully moved into Registry Record {0}", targetRegNum));
            }, new string[] { "DELETE_BATCH_REG" });
        }

        [HttpDelete]
        [Route(Consts.apiPrefix + "batches/{id}")]
        [SwaggerOperation("DeleteBatch")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
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
