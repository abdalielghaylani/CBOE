using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Web.Http;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger.Annotations;
using CambridgeSoft.COE.Framework.COETableEditorService;
using CambridgeSoft.COE.RegistrationAdmin.Services;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COESecurityService;
using PerkinElmer.COE.Registration.Server.Code;
using PerkinElmer.COE.Registration.Server.Models;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class ViewConfigController : RegControllerBase
    {
        private static JArray GetPropertyGroups()
        {
            var propertyGroups = new JArray();
            var configurationBO = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            var propertyTypes = Enum.GetValues(typeof(ConfigurationRegistryRecord.PropertyListType)).Cast<ConfigurationRegistryRecord.PropertyListType>();
            foreach (var propertyType in propertyTypes)
            {
                propertyGroups.Add(new JObject(
                    new JProperty("groupName", propertyType.ToString()),
                    new JProperty("groupLabel", GetPropertyTypeLabel(propertyType))
                ));
            }
            return propertyGroups;
        }

        private static JArray GetAddinAssemblies()
        {
            var asemblies = new JArray();
            var configRegRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
            foreach (AddInAssembly assembly in configRegRecord.GetAssemblyList.Assemblies)
            {
                dynamic assemblyObject = new JObject();
                assemblyObject.name = assembly.Name;
                assemblyObject.classes = new JArray();

                foreach (AddInClass addinClass in assembly.ClassList)
                {
                    dynamic classObject = new JObject();
                    classObject.name = addinClass.Name;
                    classObject.eventHandlers = new JArray();

                    foreach (string eventHandler in addinClass.EventHandlerList)
                    {
                        dynamic eventHandlerObject = new JObject();
                        eventHandlerObject.name = eventHandler;
                        classObject.eventHandlers.Add(eventHandlerObject);
                    }
                    assemblyObject.classes.Add(classObject);
                }
                asemblies.Add(assemblyObject);
            }
            return asemblies;
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

        private JArray GetHomeMenuPrivileges()
        {
            var homeMenuPrivilages = new JArray();
            COEHomeSettings homeData = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetHomeData();
            COEIdentity coeIdentity = UserIdentity;
            string coeIdentifier = "Registration";

            Group group = homeData.Groups.First<Group>(item => item.Name.Equals(coeIdentifier, StringComparison.OrdinalIgnoreCase));

            foreach (LinkData linkData in group.LinksData)
            {
                bool hasPriv = coeIdentity.HasAppPrivilege(coeIdentifier.ToUpper(), linkData.PrivilegeRequired);

                var privilege = new JObject(
                    new JProperty("name", linkData.Name),
                    new JProperty("label", linkData.DisplayText),
                    new JProperty("privilegeName", linkData.PrivilegeRequired),
                    new JProperty("visibility", hasPriv),
                    new JProperty("toolTip", linkData.ToolTip));
                homeMenuPrivilages.Add(privilege);
            }

            return homeMenuPrivilages;
        }

        private JArray GetSystemSettings()
        {
            // [{ name, group, ... }, ... ]
            var systemSettings = new List<SettingData>();
            var app = GUIShellUtilities.GetApplicationName();
            AppSettingsData appSet = FrameworkUtils.GetAppConfigSettings(app);
            foreach (var g in appSet.SettingsGroup)
            {
                foreach (var s in g.Settings)
                {
                    bool isAdmin;
                    if (bool.TryParse(s.IsAdmin, out isAdmin) && isAdmin) continue;
                    systemSettings.Add(new SettingData(g, s));
                }
            }
            return JArray.FromObject(systemSettings);
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "ViewConfig/Lookups")]
        [SwaggerOperation("GetLookups")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        [SwaggerResponse(500, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetLookups()
        {
            return await CallMethod(() =>
            {
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
                    new JProperty("systemSettings", GetSystemSettings()),
                    new JProperty("addinAssemblies", GetAddinAssemblies()),
                    new JProperty("propertyGroups", GetPropertyGroups()),
                    new JProperty("homeMenuPrivileges", GetHomeMenuPrivileges())
                );
            });
        }
    }
}
