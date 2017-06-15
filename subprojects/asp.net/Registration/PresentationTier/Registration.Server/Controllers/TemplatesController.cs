﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using Csla.Validation;
using Microsoft.Web.Http;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger.Annotations;
using CambridgeSoft.COE.Framework.Common.Validation;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.COEGenericObjectStorageService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COESecurityService;
using PerkinElmer.COE.Registration.Server.Code;
using PerkinElmer.COE.Registration.Server.Models;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class TemplatesController : RegControllerBase
    {
        private string CurrentUserName
        {
            get
            {
                var sessionToken = GetSessionToken();
                var args = new object[] { sessionToken };
                var identity = (COEIdentity)typeof(COEIdentity).GetMethod(
                    "GetIdentity",
                    BindingFlags.Static | BindingFlags.NonPublic,
                    null,
                    new System.Type[] { typeof(string) },
                    null
                ).Invoke(null, args);
                return identity.Name;
            }
        }

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
                var userName = CurrentUserName;
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
                    templateList.Add(template);
                }

                return templateList;
            });
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
                var userName = CurrentUserName;
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

                return new ResponseData(message: string.Format("The template, {0}, was saved successfully!", data.Name));
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
                var compoundFormListForCurrentUser = COEGenericObjectStorageBOList.GetList(CurrentUserName, 2, true);
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
