using System.Web.Http;
using CambridgeSoft.COE.Framework.COEChemDrawConverterService;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using CambridgeSoft.COE.Framework.COETableEditorService;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.RegistrationAdmin.Services;
using CambridgeSoft.COE.Framework.COESecurityService;
using System;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    public class ViewConfigController : RegControllerBase
    {
        [Route("api/ViewConfig/Lookups")]
        public JObject GetLookups()
        {
            CheckAuthentication();
            var formGroups = new JArray();
            var configRegRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            configRegRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.SubmitMixture);
            formGroups.Add(new JObject(
                new JProperty("name", COEFormHelper.COEFormGroups.SubmitMixture.ToString()),
                new JProperty("data", configRegRecord.FormGroup.ToString())
            ));
            configRegRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.ReviewRegisterMixture);
            formGroups.Add(new JObject(
                new JProperty("name", COEFormHelper.COEFormGroups.ReviewRegisterMixture.ToString()),
                new JProperty("data", configRegRecord.FormGroup.ToString())
            ));
            configRegRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.ViewMixture);
            formGroups.Add(new JObject(
                new JProperty("name", COEFormHelper.COEFormGroups.ViewMixture.ToString()),
                new JProperty("data", configRegRecord.FormGroup.ToString())
            ));
            configRegRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchTemporary);
            formGroups.Add(new JObject(
                new JProperty("name", COEFormHelper.COEFormGroups.SearchTemporary.ToString()),
                new JProperty("data", configRegRecord.FormGroup.ToString())
            ));
            configRegRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchPermanent);
            formGroups.Add(new JObject(
                new JProperty("name", COEFormHelper.COEFormGroups.SearchPermanent.ToString()),
                new JProperty("data", configRegRecord.FormGroup.ToString())
            ));
            configRegRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.ELNSearchTempForm);
            formGroups.Add(new JObject(
                new JProperty("name", COEFormHelper.COEFormGroups.ELNSearchTempForm.ToString()),
                new JProperty("data", configRegRecord.FormGroup.ToString())
            ));
            configRegRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.ELNSearchPermForm);
            formGroups.Add(new JObject(
                new JProperty("name", COEFormHelper.COEFormGroups.ELNSearchPermForm.ToString()),
                new JProperty("data", configRegRecord.FormGroup.ToString())
            ));
            configRegRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.DataLoaderForm);
            formGroups.Add(new JObject(
                new JProperty("name", COEFormHelper.COEFormGroups.DataLoaderForm.ToString()),
                new JProperty("data", configRegRecord.FormGroup.ToString())
            ));
            configRegRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.RegistryDuplicatesForm);
            formGroups.Add(new JObject(
                new JProperty("name", COEFormHelper.COEFormGroups.RegistryDuplicatesForm.ToString()),
                new JProperty("data", configRegRecord.FormGroup.ToString())
            ));
            configRegRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.ComponentDuplicatesForm);
            formGroups.Add(new JObject(
                new JProperty("name", COEFormHelper.COEFormGroups.ComponentDuplicatesForm.ToString()),
                new JProperty("data", configRegRecord.FormGroup.ToString())
            ));
            configRegRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.SendToRegistrationForm);
            formGroups.Add(new JObject(
                new JProperty("name", COEFormHelper.COEFormGroups.SendToRegistrationForm.ToString()),
                new JProperty("data", configRegRecord.FormGroup.ToString())
            ));
            configRegRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.DeleteLogFrom);
            formGroups.Add(new JObject(
                new JProperty("name", COEFormHelper.COEFormGroups.DeleteLogFrom.ToString()),
                new JProperty("data", configRegRecord.FormGroup.ToString())
            ));
            configRegRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchComponentToAddForm);
            formGroups.Add(new JObject(
                new JProperty("name", COEFormHelper.COEFormGroups.SearchComponentToAddForm.ToString()),
                new JProperty("data", configRegRecord.FormGroup.ToString())
            ));
            configRegRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchComponentToAddFormRR);
            formGroups.Add(new JObject(
                new JProperty("name", COEFormHelper.COEFormGroups.SearchComponentToAddFormRR.ToString()),
                new JProperty("data", configRegRecord.FormGroup.ToString())
            ));
            var customTables = new JArray();
            var tables = COETableEditorUtilities.getTables();
            foreach (var key in tables.Keys)
            {
                var table = new JObject(
                    new JProperty("tableName", key),
                    new JProperty("label", tables[key])
                );
                customTables.Add(table);
            }

            return new JObject(
                new JProperty("users", ExtractData("SELECT * FROM VW_PEOPLE")),
                new JProperty("fragments", ExtractData("SELECT * FROM VW_FRAGMENT")),
                new JProperty("fragmentTypes", ExtractData("SELECT * FROM VW_FRAGMENTTYPE")),
                new JProperty("identifierTypes", ExtractData("SELECT * FROM VW_IDENTIFIERTYPE")),
                new JProperty("notebooks", ExtractData("SELECT * FROM VW_NOTEBOOK")),
                new JProperty("pickList", ExtractData("SELECT * FROM VW_PICKLIST")),
                new JProperty("pickListDomains", ExtractData("SELECT * FROM VW_PICKLISTDOMAIN")),
                new JProperty("projects", ExtractData("SELECT * FROM VW_PROJECT")),
                new JProperty("sequences", ExtractData("SELECT * FROM VW_SEQUENCE")),
                new JProperty("sites", ExtractData("SELECT * FROM VW_SITES")),
                new JProperty("units", ExtractData("SELECT * FROM VW_UNIT")),
                new JProperty("formGroups", formGroups),
                new JProperty("customTables", customTables)
            );
        }
    }
}
