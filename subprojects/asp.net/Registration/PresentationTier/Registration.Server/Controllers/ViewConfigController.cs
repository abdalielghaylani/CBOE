using System;
using System.Web.Http;
using System.Collections.Generic;
using System.Reflection;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using Newtonsoft.Json.Linq;
using CambridgeSoft.COE.Framework.COEChemDrawConverterService;
using CambridgeSoft.COE.Framework.COETableEditorService;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.RegistrationAdmin.Services;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.Common;
using PerkinElmer.COE.Registration.Server.Code;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    public class ViewConfigController : RegControllerBase
    {
        [Route(Consts.apiPrefix + "ViewConfig/Lookups")]
        public JObject GetLookups()
        {
            CheckAuthentication();
            return new JObject(
                new JProperty("users", ExtractData("SELECT * FROM VW_PEOPLE")),
                new JProperty("fragments", ExtractData("SELECT fragmentid, fragmenttypeid, ('fragment/' || code || '?' || to_char(modified, 'YYYYMMDDHH24MISS')) structure, code, description, molweight, formula FROM VW_FRAGMENT")),
                new JProperty("fragmentTypes", ExtractData("SELECT * FROM VW_FRAGMENTTYPE")),
                new JProperty("identifierTypes", ExtractData("SELECT * FROM VW_IDENTIFIERTYPE")),
                new JProperty("notebooks", ExtractData("SELECT * FROM VW_NOTEBOOK")),
                new JProperty("pickList", ExtractData("SELECT * FROM VW_PICKLIST")),
                new JProperty("pickListDomains", ExtractData("SELECT * FROM VW_PICKLISTDOMAIN")),
                new JProperty("projects", ExtractData("SELECT * FROM VW_PROJECT")),
                new JProperty("sequences", ExtractData("SELECT * FROM VW_SEQUENCE")),
                new JProperty("sites", ExtractData("SELECT * FROM VW_SITES")),
                new JProperty("units", ExtractData("SELECT * FROM VW_UNIT")),
                new JProperty("formGroups", GetFormGroups()),
                new JProperty("customTables", GetCustomTables()),
                new JProperty("systemSettings", GetSystemSettings())

            );
        }

        private static JArray GetFormGroups()
        {
            var formGroups = new JArray();
            var configRegRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            foreach (COEFormHelper.COEFormGroups formGroupType in Enum.GetValues(typeof(COEFormHelper.COEFormGroups)))
            {
                configRegRecord.COEFormHelper.Load(formGroupType);
                formGroups.Add(new JObject(
                    new JProperty("name", formGroupType.ToString()),
                    new JProperty("data", configRegRecord.FormGroup.ToString())
                ));
            }
            return formGroups;
        }

        private static JArray GetCustomTables()
        {
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
            return customTables;
        }

        private JArray GetSystemSettings()
        {
            // [{ group: .., settings: [ ] }, ... ]
            var systemSettings = new JArray();
            var app = GUIShellUtilities.GetApplicationName();
            AppSettingsData appSet = FrameworkUtils.GetAppConfigSettings(app);
            foreach (var g in appSet.SettingsGroup)
            {
                var settings = new JArray();
                foreach (var s in g.Settings)
                {
                    var setting = new JObject(
                        new JProperty("name", s.Name),
                        new JProperty("value", s.Value),
                        new JProperty("description", s.Description),
                        new JProperty("allowedValues", s.AllowedValues),
                        new JProperty("controlType", s.ControlType),
                        new JProperty("isAdmin", s.IsAdmin),
                        new JProperty("isHidden", s.IsHidden),
                        new JProperty("picklistDatabaseName", s.PicklistDatabaseName),
                        new JProperty("picklistType", s.PicklistType),
                        new JProperty("processorClass", s.ProcessorClass)
                    );
                    settings.Add(setting);
                }
                var groupSettings = new JObject(
                    new JProperty("name", g.Name),
                    new JProperty("title", g.Title),
                    new JProperty("description", g.Description),
                    new JProperty("settings", settings)
                );
                systemSettings.Add(groupSettings);
            }
            return systemSettings;
        }
    }
}
