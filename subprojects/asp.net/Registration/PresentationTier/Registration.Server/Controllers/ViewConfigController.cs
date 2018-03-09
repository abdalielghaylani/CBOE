using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Text;
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
using CambridgeSoft.COE.Registration.Services.Common;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.COEPageControlSettingsService;
using Resources;
using CambridgeSoft.COE.Framework.COESearchService;

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
            var formGroupTypes = Enum.GetValues(typeof(COEFormHelper.COEFormGroups));
            foreach (COEFormHelper.COEFormGroups formGroupType in formGroupTypes)
            {
                if (formGroupType <= COEFormHelper.COEFormGroups.SearchPermanent)
                {
                    configRegRecord.COEFormHelper.Load(formGroupType);
                    formGroups.Add(new JObject(
                        new JProperty("name", formGroupType.ToString()),
                        new JProperty("data", configRegRecord.FormGroup.ToString())
                    ));
                }
            }

            return formGroups;
        }

        private static JArray GetDisabledControls(string appName)
        {
            var disabledControlArray = new JArray();
            var disabledControls = COEPageControlSettings.GetControlListToDisableForCurrentUser(appName);
            string invGroupFieldSettings = GetInvIntegrationGroupFieldSettings(appName);
            foreach (var disabledControl in disabledControls)
            {
                var pageId = disabledControl.PageID.Replace("_ASPX", string.Empty);
                pageId = pageId.Substring(pageId.LastIndexOf('_') + 1);

                if ((disabledControl.ID.Equals("ReqMaterial") || disabledControl.ID.Equals("RequestFromBatchURL"))
                    && !string.IsNullOrEmpty(invGroupFieldSettings) && invGroupFieldSettings.Contains(disabledControl.ID))
                {
                    // do not add into disabledControls list
                }
                else
                {
                    disabledControlArray.Add(new JObject(
                            new JProperty("id", disabledControl.ID),
                            new JProperty("action", disabledControl.Action),
                            new JProperty("formId", disabledControl.COEFormID),
                            new JProperty("parentControlId", disabledControl.ParentControlId),
                            new JProperty("placeHolderId", disabledControl.PlaceHolderID),
                            new JProperty("pageId", pageId)
                            ));

                }
            }
            return disabledControlArray;
        }

        private static string GetInvIntegrationGroupFieldSettings(string appName)
        {
            if (!appName.ToUpper().Equals(Consts.REGISTRATIONAPLPICATIONNAME))
                return string.Empty;

            StringBuilder str = new StringBuilder();
            if (RegUtilities.GetInventoryIntegration())
            {
                COESearch simpleSearch = new COESearch(RegUtilities.GetInvGroupingFieldsDataViewID());
                ResultPageInfo rpi = new ResultPageInfo(0, 100, 1, 101);
                DataResult dr = simpleSearch.GetDataPage(rpi, new string[] { "GROUPINGFIELDS.FIELDNAME" });
                if (dr != null && dr.ResultSet != null)
                {
                    if (dr.ResultSet.Contains("REG_ID_FK"))
                    {
                        str.Append("ReqMaterial");
                    }

                    if (dr.ResultSet.Contains("BATCH_NUMBER_FK"))
                    {
                        str.Append("RequestFromBatchURL");
                    }
                }
            }
            return str.ToString();
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
            Dictionary<string, List<string>> appUserPrivileges = UserPrivileges;
            foreach (string appName in appUserPrivileges.Keys)
            {
                var app = new JArray();
                foreach (string privilege in appUserPrivileges[appName.ToUpper()])
                {
                    app.Add(new JObject(new JProperty("name", privilege)));
                }
                appUserPrivilages.Add(new JObject(new JProperty("appName", appName), new JProperty("privileges", app)));
            }
            return appUserPrivilages;
        }

        private JArray GetDisabledControls()
        {
            var appName = COEAppName.Get().ToUpper();
            var disabledControlArray = GetDisabledControls(appName);

            // CBOE-6343 'Component ID' and 'Structure ID' fields should not be available in detail view of temp records
            // TODO: Visibility of these fields can be decided by updating 'REVIEWREGISTERMIXTURE' XML
            disabledControlArray.Add(new JObject(
                new JProperty("id", "CIDTextBox"),
                new JProperty("action", 0),
                new JProperty("formId", 2),
                new JProperty("parentControlId", string.Empty),
                new JProperty("placeHolderId", "mixtureInformationHolder"),
                new JProperty("pageId", "REVIEWREGISTERMIXTURE")
                ));

            disabledControlArray.Add(new JObject(
                new JProperty("id", "CSID"),
                new JProperty("action", 0),
                new JProperty("formId", 2),
                new JProperty("parentControlId", string.Empty),
                new JProperty("placeHolderId", "mixtureInformationHolder"),
                new JProperty("pageId", "REVIEWREGISTERMIXTURE")
                ));

            disabledControlArray.Merge(GetDisabledControls("CHEMBIOVIZ"));
            return disabledControlArray;
        }

        /// <summary>
        /// Returns the assembly file version
        /// </summary>
        /// <returns>Assembly file version</returns>
        private string GetFrameworkFileVersion()
        {
            string retVal = "Unknown";
            var assembly = Assembly.GetAssembly(typeof(COEConnectionInfo));
            if (assembly != null)
            {
                var fileversionAttributte = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
                if (fileversionAttributte != null && fileversionAttributte.Length > 0)
                    retVal = ((AssemblyFileVersionAttribute)fileversionAttributte[0]).Version;
            }
            return retVal;
        }

        /// <summary>
        /// Returns the assembly File version attribute.
        /// </summary>
        /// <returns>File version text</returns>
        private string GetRegistrationServicesFileVersion()
        {
            string retVal = "Unknown";
            var assembly = Assembly.GetAssembly(typeof(RegSvcUtilities));
            if (assembly != null)
            {
                FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
                if (fileVersion != null)
                    retVal = fileVersion.FileVersion;
            }
            return retVal;
        }

        /// <summary>
        /// Get System Information 
        /// </summary>
        /// <returns>Object with system data</returns>
        private JObject GetSystemInformation()
        {
            return new JObject(
                new JProperty("supportLabelText", Resource.Support_Label_Text),
                new JProperty("supportContentLabelText", Resource.SupportContent_Label_Text),
                new JProperty("ordersLabelText", Resource.Orders_Label_Text),
                new JProperty("ordersContentLabelText", Resource.OrdersContent_Label_Text),
                new JProperty("versionLabelText", Resource.Version_Label_Text),
                new JProperty("versionContentLabelText", string.Format(Resource.VersionContent_Label_Text,
                    GetFrameworkFileVersion(),
                    GetRegistrationServicesFileVersion()))
            );
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
                    ExtractData("SELECT P.PERSON_ID as PERSONID, P.USER_ID as USERID, P.SITE_ID as SITEID, DECODE(P.ACTIVE,1,'T',0,'F','0') AS ACTIVE, SUPERVISOR_INTERNAL_ID as SUPERVISORID FROM COEDB.PEOPLE P"),
                    ExtractData("SELECT fragmentid, fragmenttypeid, ('fragment/' || fragmentid || '?' || to_char(modified, 'YYYYMMDDHH24MISS')) structure, code, description, molweight, formula FROM VW_FRAGMENT"),
                    ExtractData("SELECT * FROM VW_FRAGMENTTYPE"),
                    ExtractData("SELECT * FROM VW_IDENTIFIERTYPE"),
                    ExtractData("SELECT * FROM VW_PICKLIST"),
                    ExtractPickListDomain(),
                    ExtractData("SELECT * FROM VW_PROJECT"),
                    GetFormGroups(),
                    RegAppHelper.RetrieveSettings(),
                    GetAddinAssemblies(),
                    GetPropertyGroups(),
                    GetHomeMenuPrivileges(),
                    GetUserPrivileges(),
                    GetDisabledControls(),
                    GetSystemInformation()
                );
            }, cacheControl: new CacheControlHeaderValue { NoCache = true });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "ViewConfig/query")]
        [SwaggerOperation("GetQueryValues")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetQueryValues(JObject data)
        {
            return await CallMethod(() =>
            {
                string query = data["sql"].ToString();

                if (!query.Trim().ToLower().StartsWith("select"))
                    throw new RegistrationException("only select statements are allowed in the query input");

                var response = new JObject();
                response.Add(new JProperty("data", ExtractData(query, null, null, null)));
                return new ResponseData(data: response);
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
