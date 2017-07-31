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
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.COEPageControlSettingsService;

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
                if (propertyType.ToString().Equals("AddIns") || propertyType.ToString().Equals("None"))
                    continue;

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

        private static JArray GetDisabledControls(string appName)
        {
            var disabledControlArray = new JArray();
            var disabledControls = COEPageControlSettings.GetControlListToDisableForCurrentUser(appName);
            foreach (var disabledControl in disabledControls)
            {
                var pageId = disabledControl.PageID.Replace("_ASPX", string.Empty);
                pageId = pageId.Substring(pageId.LastIndexOf('_') + 1);
                disabledControlArray.Add(new JObject(
                    new JProperty("id", disabledControl.ID),
                    new JProperty("action", disabledControl.Action),
                    new JProperty("formId", disabledControl.COEFormID),
                    new JProperty("parentControlId", disabledControl.ParentControlId),
                    new JProperty("placeHolderId", disabledControl.PlaceHolderID),
                    new JProperty("pageId", pageId)
                ));
            }
            return disabledControlArray;
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

        private JArray GetUserPrivileges()
        {
            var appUserPrivilages = new JArray();
            string coeIdentifier = "Registration";
            Dictionary<string, List<string>> appUserPrivileges = UserPrivileges;
            if (appUserPrivileges.ContainsKey(coeIdentifier.ToUpper()))
            {
                foreach (string privilege in appUserPrivileges[coeIdentifier.ToUpper()])
                {
                    appUserPrivilages.Add(new JObject(new JProperty("name", privilege)));
                }
            }
            return appUserPrivilages;
        }

        private JArray GetDisabledControls()
        {
            var appName = COEAppName.Get().ToUpper();
            var disabledControlArray = GetDisabledControls(appName);
            disabledControlArray.Merge(GetDisabledControls("CHEMBIOVIZ"));
            return disabledControlArray;
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "ViewConfig/Lookups")]
        [SwaggerOperation("GetLookups")]
        [SwaggerResponse(200, type: typeof(LookupData))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetLookups()
        {
            return await CallMethod(() =>
            {
                return new LookupData(
                    ExtractData("SELECT * FROM VW_PEOPLE"),
                    ExtractData("SELECT fragmentid, fragmenttypeid, ('fragment/' || code || '?' || to_char(modified, 'YYYYMMDDHH24MISS')) structure, code, description, molweight, formula FROM VW_FRAGMENT"),
                    ExtractData("SELECT * FROM VW_FRAGMENTTYPE"),
                    ExtractData("SELECT * FROM VW_IDENTIFIERTYPE"),
                    ExtractData("SELECT * FROM VW_PICKLIST"),
                    ExtractPickListDomain(),
                    ExtractData("SELECT * FROM VW_PROJECT"),
                    ExtractData("SELECT * FROM VW_SITES"),
                    GetFormGroups(),
                    GetCustomTables(),
                    RegAppHelper.RetrieveSettings(),
                    GetAddinAssemblies(),
                    GetPropertyGroups(),
                    GetHomeMenuPrivileges(),
                    GetUserPrivileges(),
                    GetDisabledControls()
                );
            });
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "ViewConfig/recordsView/{id}/{registryType}/{pageMode}")]
        [SwaggerOperation("GetRecordsView")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetRecordsView(int id, string registryType, string pageMode)
        {
            return await CallMethod(() =>
            {
                RegistryRecord registryRecord = RegistryRecord.GetRegistryRecord(id);

                if (registryRecord == null)
                    throw new RegistrationException(string.Format("registry record not found for id = {0}", id));

                RegistryTypes currentRegistryType = (RegistryTypes)Enum.Parse(typeof(RegistryTypes), registryType, true);
                PageState pageState = (PageState)Enum.Parse(typeof(PageState), pageMode, true);

                dynamic jsonObject = new JObject();

                // add a {'buttonStatus'} node in the JSON 
                jsonObject.buttonStatus = new JArray() as dynamic;

                ControlState registerButtonState = ViewConfigHelper.GetRegisterButtonState(registryRecord, pageState, currentRegistryType);
                dynamic button = new JObject();
                button.name = registerButtonState.Name;
                button.visible = registerButtonState.Visible;
                button.tooltip = registerButtonState.Tooltip;
                jsonObject.buttonStatus.Add(button);

                ControlState submitButtonState = ViewConfigHelper.GetSubmitButtonState(registryRecord, pageState, currentRegistryType);
                button = new JObject();
                button.name = submitButtonState.Name;
                button.visible = submitButtonState.Visible;
                button.tooltip = submitButtonState.Tooltip;
                jsonObject.buttonStatus.Add(button);

                // add a {'formGroupStatus'} node in the JSON 
                jsonObject.formGroupStatus = new JArray() as dynamic;

                List<ControlState> formStates = ViewConfigHelper.GetFormsState(registryRecord, pageState, currentRegistryType);
                foreach (ControlState controlState in formStates)
                {
                    dynamic form = new JObject();
                    form.name = controlState.Name;
                    form.visible = controlState.Visible;
                    jsonObject.formGroupStatus.Add(form);
                }
                return jsonObject;
            });
        }

    }
}
