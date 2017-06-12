﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using Csla.Validation;
using Microsoft.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger.Annotations;
using CambridgeSoft.COE.Framework.Common.Validation;
using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Registration.Services.Types;
using PerkinElmer.COE.Registration.Server.Code;
using PerkinElmer.COE.Registration.Server.Models;
using CambridgeSoft.COE.Framework.COEGenericObjectStorageService;
using CambridgeSoft.COE.Framework.Common;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class RegistryRecordsController : RegControllerBase
    {
        private void CheckTempRecordId(int id)
        {
            var args = new Dictionary<string, object>();
            args.Add(":id", id);
            var count = Convert.ToInt32(ExtractValue("SELECT cast(count(1) as int) FROM vw_temporarycompound WHERE tempcompoundid=:id", args));
            if (count == 0)
                throw new IndexOutOfRangeException(string.Format("Cannot find temporary compompound ID, {0}", id));
        }

        private string GetNodeText(XmlNode node)
        {
            return node.InnerText;
        }

        private string GetNodeText(XmlDocument doc, string path)
        {
            var node = doc.SelectSingleNode(path);
            return node == null ? null : GetNodeText(node);
        }

        private string GetRegNumber(int id)
        {
            var args = new Dictionary<string, object>();
            args.Add(":id", id);
            var regNum = (string)ExtractValue("SELECT regnumber FROM vw_mixture_regnumber WHERE regid=:id", args);
            if (string.IsNullOrEmpty(regNum))
                throw new IndexOutOfRangeException(string.Format("Cannot find registration ID, {0}", id));
            return regNum;
        }

        #region Permanent Records
        [HttpGet]
        [Route(Consts.apiPrefix + "records")]
        [SwaggerOperation("GetRecords")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        [SwaggerResponse(500, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetRecords(int? skip = null, int? count = null, string sort = null)
        {
            return await CallMethod(() =>
            {
                var tableName = "vw_mixture_regnumber";
                var query = GetQuery(tableName, RecordColumns, sort, "modified", "regid");
                return new JObject(
                    new JProperty("temporary", false),
                    new JProperty("startIndex", skip == null ? 0 : Math.Max(skip.Value, 0)),
                    new JProperty("totalCount", Convert.ToInt32(ExtractValue("SELECT cast(count(1) as int) c FROM " + tableName))),
                    new JProperty("rows", ExtractData(query, null, skip, count))
                );
            });
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "records/{id}")]
        [SwaggerOperation("GetRecord")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        [SwaggerResponse(404, type: typeof(JObject))]
        [SwaggerResponse(500, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetRecord(int id)
        {
            return await CallServiceMethod((service) =>
            {
                string record;
                if (id < 0)
                {
                    record = service.RetrieveNewRegistryRecord();
                }
                else
                {
                    var regNum = GetRegNumber(id);
                    record = service.RetrieveRegistryRecord(regNum);
                }

                var recordXml = new XmlDocument();
                recordXml.LoadXml(record);
                return new JObject(new JProperty("data", ChemistryHelper.ConvertStructuresToCdxml(recordXml).OuterXml));
            });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "records")]
        [SwaggerOperation("CreateRecord")]
        [SwaggerResponse(201, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        public async Task<IHttpActionResult> CreateRecord(JObject recordData)
        {
            return await CallServiceMethod((service) =>
            {
                var doc = new XmlDocument();
                doc.LoadXml((string)recordData["data"]);
                var recordString = ChemistryHelper.ConvertStructuresToCdx(doc).OuterXml;
                var resultString = service.CreateRegistryRecord(recordString, "N");
                doc.LoadXml(resultString);
                var errorMessage = GetNodeText(doc, "//ErrorMessage");
                if (!string.IsNullOrEmpty(errorMessage))
                    throw new RegistrationException(errorMessage);
                var id = Convert.ToInt32(GetNodeText(doc, "//RegID"));
                var regNum = GetNodeText(doc, "//RegNum");
                var data = new JObject(
                    new JProperty("batchCount", Convert.ToInt32(GetNodeText(doc, "//BatchNumber"))),
                    new JProperty("batchId", Convert.ToInt32(GetNodeText(doc, "//BatchID")))
                );
                var redBoxWarning = GetNodeText(doc, "//RedBoxWarning");
                if (!string.IsNullOrEmpty(redBoxWarning))
                    data.Add(new JProperty("redBoxWarning", redBoxWarning));
                return new ResponseData(id, regNum, null, data);
            });
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "records/{id}")]
        [SwaggerOperation("UpdateRecord")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(401, type: typeof(JObject))]
        public async Task<IHttpActionResult> UpdateRecord(int id, JObject recordData)
        {
            return await CallMethod(() =>
            {
                string errorMessage = null;
                RegistryRecord registryRecord = null;
                try
                {
                    errorMessage = "Unable to parse the incoming data as a well-formed XML document.";
                    var xml = (string)recordData["data"];
                    var doc = new XmlDocument();
                    doc.LoadXml(xml);
                    errorMessage = "Unable to process chemical structures.";
                    xml = ChemistryHelper.ConvertStructuresToCdx(doc).OuterXml;
                    errorMessage = "Unable to determine the registry number.";
                    const string regNumXPath = "/MultiCompoundRegistryRecord/RegNumber/RegNumber";
                    XmlNode regNode = doc.SelectSingleNode(regNumXPath);
                    string regNum = regNode.InnerText.Trim();
                    errorMessage = string.Format("Unable to find the registry entry: {0}", regNum);
                    registryRecord = RegistryRecord.GetRegistryRecord(regNum);
                    if (registryRecord == null) throw new Exception();
                    errorMessage = "Record is locked and cannot be updated.";
                    if (registryRecord.Status == RegistryStatus.Locked) throw new Exception();
                    errorMessage = "Unable to update the internal record.";
                    registryRecord.UpdateFromXml(xml);
                    errorMessage = "Cannot set batch prefix.";
                    registryRecord.BatchPrefixDefaultOverride(true);
                    errorMessage = "Unable to save the record.";
                    registryRecord.Save(DuplicateCheck.CompoundCheck);
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

                    throw new RegistrationException(errorMessage, ex);
                }

                return new ResponseData(regNumber: registryRecord.RegNumber.RegNum);
            }, new string[] { "EDIT_COMPOUND_REG" });
        }

        [HttpDelete]
        [Route(Consts.apiPrefix + "records/{id}")]
        [SwaggerOperation("DeleteRecord")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> DeleteRecord(int id)
        {
            return await CallMethod(() =>
            {
                var regNum = GetRegNumber(id);
                RegistryRecord.DeleteRegistryRecord(regNum);
                return new ResponseData(id: id, regNumber: regNum, message: string.Format("The registry record, {0}, was deleted successfully.", regNum));
            }, new string[] { "DELETE_TEMP" });
        }

        #endregion // Permanent Records

        #region Temporary Records
        [HttpGet]
        [Route(Consts.apiPrefix + "temp-records")]
        [SwaggerOperation("GetTempRecords")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        [SwaggerResponse(500, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetTempRecords(int? skip = null, int? count = null, string sort = null)
        {
            return await CallMethod(() =>
            {
                var tableName = "vw_temporarycompound";
                var query = GetQuery(tableName, TempRecordColumns, sort, "datelastmodified", "tempcompoundid");
                return new JObject(
                    new JProperty("temporary", true),
                    new JProperty("startIndex", skip == null ? 0 : Math.Max(skip.Value, 0)),
                    new JProperty("totalCount", Convert.ToInt32(ExtractValue("SELECT cast(count(1) as int) c FROM " + tableName))),
                    new JProperty("rows", ExtractData(query, null, skip, count))
                );
            });
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "temp-records/{id}")]
        [SwaggerOperation("GetTempRecord")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        [SwaggerResponse(404, type: typeof(JObject))]
        [SwaggerResponse(500, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetTempRecord(int id)
        {
            return await CallServiceMethod((service) =>
            {
                string record;
                if (id < 0)
                {
                    record = service.RetrieveNewRegistryRecord();
                }
                else
                {
                    CheckTempRecordId(id);
                    record = service.RetrieveTemporaryRegistryRecord(id);
                }

                var recordXml = new XmlDocument();
                recordXml.LoadXml(record);
                return new JObject(new JProperty("data", ChemistryHelper.ConvertStructuresToCdxml(recordXml).OuterXml));
            });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "temp-records")]
        [SwaggerOperation("CreateTempRecord")]
        [SwaggerResponse(201, type: typeof(ResponseData))]
        [SwaggerResponse(401, type: typeof(JObject))]
        public async Task<IHttpActionResult> CreateTempRecord(JObject recordData)
        {
            return await CallServiceMethod((service) =>
            {
                var doc = new XmlDocument();
                doc.LoadXml((string)recordData["data"]);
                var recordString = ChemistryHelper.ConvertStructuresToCdx(doc).OuterXml;
                var resultString = service.CreateTemporaryRegistryRecord(recordString);
                return new ResponseData(Convert.ToInt32(resultString));
            });
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "temp-records/{id}")]
        [SwaggerOperation("UpdateTempRecord")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(401, type: typeof(JObject))]
        public async Task<IHttpActionResult> UpdateTempRecord(int id, JObject recordData)
        {
            return await CallMethod(() =>
            {
                string errorMessage = null;
                RegistryRecord registryRecord = null;
                try
                {
                    errorMessage = "Unable to parse the incoming data as a well-formed XML document.";
                    var xml = (string)recordData["data"];
                    var doc = new XmlDocument();
                    doc.LoadXml(xml);
                    errorMessage = "Unable to process chemical structures.";
                    xml = ChemistryHelper.ConvertStructuresToCdx(doc).OuterXml;
                    errorMessage = "Unable to determine the registry ID.";
                    const string regIdXPath = "/MultiCompoundRegistryRecord/ID";
                    XmlNode regNode = doc.SelectSingleNode(regIdXPath);
                    int regId = Convert.ToInt32(regNode.InnerText.Trim());
                    errorMessage = string.Format("Unable to find the registry entry: {0}", regId);
                    registryRecord = RegistryRecord.GetRegistryRecord(regId);
                    if (registryRecord == null) throw new Exception();
                    errorMessage = "Unable to update the internal record.";
                    registryRecord.InitializeFromXml(xml, false, false);
                    errorMessage = "Cannot set batch prefix.";
                    registryRecord.BatchPrefixDefaultOverride(true);
                    errorMessage = "Unable to save the record.";
                    registryRecord = registryRecord.Save();
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

                    throw new RegistrationException(errorMessage, ex);
                }

                return new ResponseData(id: registryRecord.ID);
            }, new string[] { "EDIT_COMPOUND_TEMP" });
        }

        [HttpDelete]
        [Route(Consts.apiPrefix + "temp-records/{id}")]
        [SwaggerOperation("DeleteTempRecord")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> DeleteTempRecord(int id)
        {
            return await CallMethod(() =>
            {
                CheckTempRecordId(id);
                RegistryRecord.DeleteRegistryRecord(id);
                return new ResponseData(id: id, message: string.Format("The temporary record, {0}, was deleted successfully.", id));
            }, new string[] { "DELETE_TEMP" });
        }
        #endregion // Tempoary Records

        #region Templates

        [HttpGet]
        [Route(Consts.apiPrefix + "templates/{username}")]
        [SwaggerOperation("GetTemplates")]
        [SwaggerResponse(200, type: typeof(List<TemplateData>))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetTemplates(string username)
        {
            return await CallMethod(() =>
            {
                var templateList = new List<TemplateData>();

                var compoundFormListForCurrentUser = COEGenericObjectStorageBOList.GetList(username, 2, true);
                foreach (COEGenericObjectStorageBO tempalateItem in compoundFormListForCurrentUser)
                {
                    TemplateData template = new TemplateData();
                    template.Id = tempalateItem.ID;
                    template.Name = tempalateItem.Name;
                    template.DateCreated = tempalateItem.DateCreated;
                    template.Description = tempalateItem.Description;
                    template.IsPublic = tempalateItem.IsPublic;
                    template.Username = tempalateItem.UserName;
                    templateList.Add(template);
                }

                var compoundFormListPublic = COEGenericObjectStorageBOList.GetList(username, true, 2, true);
                foreach (COEGenericObjectStorageBO tempalateItem in compoundFormListPublic)
                {
                    TemplateData template = new TemplateData();
                    template.Id = tempalateItem.ID;
                    template.Name = tempalateItem.Name;
                    template.DateCreated = tempalateItem.DateCreated;
                    template.Description = tempalateItem.Description;
                    template.IsPublic = tempalateItem.IsPublic;
                    template.Username = tempalateItem.UserName;
                    templateList.Add(template);
                }

                return templateList;
            });
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "templates/templates/{username}/{id}")]
        [SwaggerOperation("GetTemplate")]
        [SwaggerResponse(200, type: typeof(TemplateData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetTemplate(string username, int id)
        {
            return await CallMethod(() =>
            {
                TemplateData template = null;
                bool found = false;
                var compoundFormListForCurrentUser = COEGenericObjectStorageBOList.GetList(username, 2, true);

                foreach (COEGenericObjectStorageBO tempalateItem in compoundFormListForCurrentUser)
                {
                    if (id != tempalateItem.ID) continue;

                    template = new TemplateData();
                    template.Id = tempalateItem.ID;
                    template.Name = tempalateItem.Name;
                    template.DateCreated = tempalateItem.DateCreated;
                    template.Description = tempalateItem.Description;
                    template.IsPublic = tempalateItem.IsPublic;
                    template.Username = tempalateItem.UserName;

                    found = true;
                    break;
                }

                if (!found)
                {
                    var compoundFormListPublic = COEGenericObjectStorageBOList.GetList(username, true, 2, true);
                    foreach (COEGenericObjectStorageBO tempalateItem in compoundFormListPublic)
                    {
                        if (id != tempalateItem.ID) continue;

                        template = new TemplateData();
                        template.Id = tempalateItem.ID;
                        template.Name = tempalateItem.Name;
                        template.DateCreated = tempalateItem.DateCreated;
                        template.Description = tempalateItem.Description;
                        template.IsPublic = tempalateItem.IsPublic;
                        template.Username = tempalateItem.UserName;

                        found = true;
                        break;
                    }
                }

                if (!found)
                    throw new RegistrationException(string.Format("template with id '{0}' not found.", id));

                COEGenericObjectStorageBO genericStorageBO = COEGenericObjectStorageBO.Get(template.Id);
                string coeGenericObjectXml = genericStorageBO.COEGenericObject;
                // if xml is coming from a stored template - reset the default scientistID
                string propertyPath = string.Format("MultiCompoundRegistryRecord/BatchList/Batch/PropertyList/Property[@name='{0}']", "SCIENTIST_ID");
                XmlDocument recordXml = new XmlDocument();
                recordXml.LoadXml(coeGenericObjectXml);
                XmlNode xNode = recordXml.SelectSingleNode(propertyPath);
                if (xNode != null)
                {
                    if (xNode.InnerText == string.Empty)
                    {
                        xNode.InnerText = COEUser.ID.ToString();
                    }
                }

                template.Data = ChemistryHelper.ConvertStructuresToCdxml(recordXml).OuterXml;
                return template;
            });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "templates")]
        [SwaggerOperation("CreateTemplates")]
        [SwaggerResponse(201, type: typeof(TemplateData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> CreateTemplates(TemplateData data)
        {
            return await CallMethod(() =>
            {
                if (string.IsNullOrEmpty(data.Name))
                    throw new RegistrationException("Invalid template name");

                string errorMessage = null;
                RegistryRecord registryRecord = null;
                try
                {
                    errorMessage = "Unable to parse the incoming data as a well-formed XML document.";
                    string xml = data.Data;
                    var doc = new XmlDocument();
                    doc.LoadXml(xml);
                    errorMessage = "Unable to process chemical structures.";
                    xml = ChemistryHelper.ConvertStructuresToCdx(doc).OuterXml;

                    registryRecord = RegistryRecord.NewRegistryRecord();
                    errorMessage = "Unable to initialize the internal record.";
                    registryRecord.InitializeFromXml(xml, true, false);

                    errorMessage = "Unable to save the template.";
                    if (registryRecord != null)
                        registryRecord.SaveTemplate(data.Name, data.Description, data.IsPublic, 2);
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

                    throw new RegistrationException(errorMessage, ex);
                }

                return new ResponseData(message: string.Format("The template, {0}, was saved successfully.", data.Name));
            });
        }

        [HttpDelete]
        [Route(Consts.apiPrefix + "templates/{username}/{id}")]
        [SwaggerOperation("DeleteTemplate")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> DeleteTemplate(string username, int id)
        {
            return await CallMethod(() =>
            {
                if (string.IsNullOrEmpty(username))
                    throw new RegistrationException("Invalid user name.");

                var compoundFormListForCurrentUser = COEGenericObjectStorageBOList.GetList(username, 2, true);
                COEGenericObjectStorageBO selected = null;
                foreach (COEGenericObjectStorageBO tempalateItem in compoundFormListForCurrentUser)
                {
                    if (tempalateItem.ID != id)
                        continue;

                    selected = tempalateItem;
                    break;
                }

                if (selected == null)
                    throw new IndexOutOfRangeException(string.Format("The template, {0}, was not found.", id));

                COEGenericObjectStorageBO.Delete(id);

                return new ResponseData(message: string.Format("The template, {0}, was deleted successfully.", id));
            });
        }
        #endregion

        #region Check Duplicate

        [HttpGet]
        [Route(Consts.apiPrefix + "getDuplicateResolution")]
        [SwaggerOperation("GetDuplicateResolution")]
        [SwaggerResponse(201, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetDuplicateResolution()
        {
            return await CallServiceMethod((service) =>
            {
                var responseMessage = new JObject(
                new JProperty("DuplicateCheck", DuplicateCheck.CompoundCheck.ToString(), DuplicateCheck.MixCheck.ToString(), DuplicateCheck.None.ToString(), DuplicateCheck.PreReg.ToString())
                );
                return new ResponseData(null, null, null, responseMessage);
            });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "duplicateResolution")]
        [SwaggerOperation("DuplicateResolution")]
        [SwaggerResponse(201, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        public async Task<IHttpActionResult> DuplicateResolution(DuplicateResolutionData data)
        {
            return await CallServiceMethod((service) =>
            {
                var doc = new XmlDocument();
                doc.LoadXml(data.Data);

                // TODO: check data xml is valid and throw proper error message

                var regRecordXml = ChemistryHelper.ConvertStructuresToCdx(doc).OuterXml;
                var result = service.CheckUniqueRegistryRecord(regRecordXml, data.DuplicateCheckOption);
                if (!string.IsNullOrEmpty(result))
                    throw new RegistrationException(result);
                return new ResponseData(null, null, result, null);
            });
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "getDuplicateAction")]
        [SwaggerOperation("GetDuplicateAction")]
        [SwaggerResponse(201, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetDuplicateAction()
        {
            return await CallServiceMethod((service) =>
            {
                var responseMessage = new JObject(
                new JProperty("DuplicateActions", DuplicateAction.Batch.ToString(), DuplicateAction.Compound.ToString(), DuplicateAction.Duplicate.ToString(), DuplicateAction.None.ToString(), DuplicateAction.Temporary.ToString())
                );
                return new ResponseData(null, null, null, responseMessage);
            });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "createDuplicateRecord")]
        [SwaggerOperation("CreateDuplicateRecord")]
        [SwaggerResponse(201, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        public async Task<IHttpActionResult> CreateDuplicateRecord(DuplicateResolutionData data)
        {
            return await CallServiceMethod((service) =>
            {
                var doc = new XmlDocument();
                doc.LoadXml(data.Data);
                var regRecordXml = ChemistryHelper.ConvertStructuresToCdx(doc).OuterXml;
                var result = service.CreateRegRecordWithUserPreference(regRecordXml, data.DuplicateCheckOption);
                if (!string.IsNullOrEmpty(result))
                    throw new RegistrationException(result);
                return new ResponseData(null, null, result, null);
            });
        }
        #endregion
    }
}
