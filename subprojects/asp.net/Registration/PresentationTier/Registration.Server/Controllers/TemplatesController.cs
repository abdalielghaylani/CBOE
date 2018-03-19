using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using Csla.Validation;
using Microsoft.Web.Http;
using Swashbuckle.Swagger.Annotations;
using CambridgeSoft.COE.Framework.Common.Validation;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.COEGenericObjectStorageService;
using CambridgeSoft.COE.Framework.Common;
using PerkinElmer.COE.Registration.Server.Code;
using PerkinElmer.COE.Registration.Server.Models;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class TemplatesController : RegControllerBase
    {
        #region Templates

        [HttpGet]
        [Route(Consts.apiPrefix + "templates")]
        [SwaggerOperation("GetTemplates")]
        [SwaggerResponse(200, type: typeof(List<TemplateData>))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetTemplates()
        {
            return await CallMethod(() =>
            {
                var userName = UserIdentity.Name;
                var templateList = new List<TemplateData>();
                var compoundFormListForCurrentUser = COEGenericObjectStorageBOList.GetList(userName, 2, true);
                foreach (COEGenericObjectStorageBO tempalateItem in compoundFormListForCurrentUser)
                {
                    TemplateData template = new TemplateData();
                    template.Id = tempalateItem.ID;
                    template.Name = tempalateItem.Name;
                    template.DateCreated = tempalateItem.DateCreated;
                    template.Description = tempalateItem.Description;
                    template.IsPublic = tempalateItem.IsPublic;
                    template.Username = tempalateItem.UserName;

                    if (!string.IsNullOrEmpty(tempalateItem.COEGenericObject))
                    {
                        var document = new XmlDocument();
                        document.LoadXml(tempalateItem.COEGenericObject);
                        var xmlNode = document.DocumentElement.SelectSingleNode("//StructureAggregation ");
                        if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText))
                        {
                            template.Data = xmlNode.InnerText;
                        }
                    }

                    templateList.Add(template);
                }

                var compoundFormListPublic = COEGenericObjectStorageBOList.GetList(userName, true, 2, true);
                foreach (COEGenericObjectStorageBO tempalateItem in compoundFormListPublic)
                {
                    TemplateData template = new TemplateData();
                    template.Id = tempalateItem.ID;
                    template.Name = tempalateItem.Name;
                    template.DateCreated = tempalateItem.DateCreated;
                    template.Description = tempalateItem.Description;
                    template.IsPublic = tempalateItem.IsPublic;
                    template.Username = tempalateItem.UserName;
                    if (!string.IsNullOrEmpty(tempalateItem.COEGenericObject))
                    {
                        var document = new XmlDocument();
                        document.LoadXml(tempalateItem.COEGenericObject);
                        var xmlNode = document.DocumentElement.SelectSingleNode("//StructureAggregation ");
                        if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText))
                        {
                            template.Data = xmlNode.InnerText;
                        }
                    }
                    templateList.Add(template);
                }

                return templateList;
            }, new string[] { "LOAD_SAVE_RECORD" });
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "templates/{id}")]
        [SwaggerOperation("GetTemplate")]
        [SwaggerResponse(200, type: typeof(TemplateData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetTemplate(int id)
        {
            return await CallMethod(() =>
            {
                var userName = UserIdentity.Name;
                TemplateData template = null;
                bool found = false;
                var compoundFormListForCurrentUser = COEGenericObjectStorageBOList.GetList(userName, 2, true);

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
                    var compoundFormListPublic = COEGenericObjectStorageBOList.GetList(userName, true, 2, true);
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

                COEGenericObjectStorageBO genericStorageBO = COEGenericObjectStorageBO.Get(template.Id.Value);
                string coeGenericObjectXml = genericStorageBO.COEGenericObject;
                // if XML is coming from a stored template - reset the default scientistID
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
                template.Data = recordXml.OuterXml;
                return template;
            }, new string[] { "LOAD_SAVE_RECORD" });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "templates")]
        [SwaggerOperation("CreateTemplate")]
        [SwaggerResponse(201, type: typeof(TemplateData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> CreateTemplate(TemplateData data)
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

                    registryRecord = RegistryRecord.NewRegistryRecord();
                    errorMessage = "Unable to initialize the internal record.";
                    registryRecord.InitializeFromXml(xml, true, false);

                    errorMessage = "Unable to save the template.";
                    if (registryRecord != null)
                        registryRecord.SaveTemplate(data.Name, data.Description, data.IsPublic != null && data.IsPublic.Value, 2);
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
                    if (ex is CambridgeSoft.COE.Framework.ExceptionHandling.COEBusinessLayerException)
                    {
                        if (errorMessage.EndsWith(".")) errorMessage = errorMessage.Substring(0, errorMessage.Length - 1);

                        if (ex.GetBaseException() != null)
                        {
                            errorMessage += " - " + ex.GetBaseException().Message;
                        }
                        else
                        {
                            errorMessage += " - " + ex.Message;
                        }
                    }
                    
                    throw new RegistrationException(errorMessage, ex);
                }

                return new ResponseData(message: string.Format("The template, {0}, was saved successfully!", data.Name));
            });
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "templates/{id}")]
        [SwaggerOperation("UpdateTemplate")]
        [SwaggerResponse(201, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> UpdateTemplate(int id, TemplateData data)
        {
            return await CallMethod(() =>
            {                
                COEGenericObjectStorageBO genericStorageBO = COEGenericObjectStorageBO.Get(id);

                if (genericStorageBO == null)
                    throw new IndexOutOfRangeException(string.Format("The template, {0}, was not found.", id));

                string errorMessage = null;
                RegistryRecord registryRecord = null;
                try
                {
                    errorMessage = "Unable to parse the incoming data as a well-formed XML document.";
                    string xml = data.Data;

                    registryRecord = RegistryRecord.NewRegistryRecord();
                    errorMessage = "Unable to initialize the internal record.";
                    registryRecord.InitializeFromXml(xml, true, false);

                    registryRecord.UpdateDrawingType();
                    registryRecord.ApplyAddIns();
                    registryRecord.OnSaving(registryRecord, new EventArgs());

                    errorMessage = "Unable to save the template.";
                    genericStorageBO.COEGenericObject = registryRecord.XmlWithAddIns;
                    genericStorageBO.Save();
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

                return new ResponseData(message: string.Format("The template, {0}, was saved successfully!", genericStorageBO.Name));
            });
        }

        [HttpDelete]
        [Route(Consts.apiPrefix + "templates/{id}")]
        [SwaggerOperation("DeleteTemplate")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> DeleteTemplate(int id)
        {
            return await CallMethod(() =>
            {
                var compoundFormListForCurrentUser = COEGenericObjectStorageBOList.GetList(UserIdentity.Name, 2, true);
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

                return new ResponseData(message: string.Format("The template, {0}, was deleted successfully!", id));
            });
        }
        #endregion
    }
}
