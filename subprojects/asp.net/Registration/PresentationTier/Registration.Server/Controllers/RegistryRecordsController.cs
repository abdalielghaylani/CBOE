using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using Csla.Validation;
using Microsoft.Web.Http;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger.Annotations;
using CambridgeSoft.COE.Framework.Common.Validation;
using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Registration.Services.Types;
using PerkinElmer.COE.Registration.Server.Code;
using PerkinElmer.COE.Registration.Server.Models;
using CambridgeSoft.COE.Framework.COESecurityService;
using RegistrationWebApp.Forms.ComponentDuplicates.ContentArea;
using CambridgeSoft.COE.Framework.Common.Exceptions;
using RegistrationWebApp.Forms.ComponentDuplicates;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Registration.Services;
using Resources;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.Common;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class RegistryRecordsController : RegControllerBase
    {
        private static HitListInfo GetHitlistInfo(int? hitlistId)
        {
            HitListInfo hitListInfo = null;
            if (hitlistId.HasValue && hitlistId.Value > 0)
            {
                var hitlistBO = GetHitlistBO(hitlistId.Value);
                hitListInfo = hitlistBO.HitListInfo;
            }
            return hitListInfo;
        }
        
        private void CheckTempRecordId(int id)
        {
            var args = new Dictionary<string, object>();
            args.Add(":id", id);
            var count = Convert.ToInt32(ExtractValue("SELECT cast(count(1) as int) FROM vw_temporarycompound WHERE tempbatchid=:id", args));
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

        private void ValidateUpdatedRecord(RegistryRecord registryRecord)
        {
            if (registryRecord.ID == 0 || !string.IsNullOrEmpty(registryRecord.RedBoxWarning))
            {
                var message = !string.IsNullOrEmpty(registryRecord.RedBoxWarning) ? registryRecord.RedBoxWarning : "An unexpected error happened while saving the registry record!";
                throw new RegistrationException(message.Replace(@"<br />", " "));
            }
        }

        private JObject GetDuplicateRecords(RegistryRecord sourceRegistryRecord)
        {
            if (string.IsNullOrWhiteSpace(sourceRegistryRecord.FoundDuplicates))
                return null;

            List<string> duplicateRegIds = new List<string>();
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(sourceRegistryRecord.FoundDuplicates);
            XmlNodeList nodeList = xmldoc.GetElementsByTagName("REGNUMBER");
            var duplicateActionList = new JArray();
            foreach (XmlNode node in nodeList)
            {
                RegistryRecord duplicateRegistryRecord = RegistryRecord.GetRegistryRecord(node.InnerText);
                duplicateRegIds.Add(duplicateRegistryRecord.ID.ToString());

                DuplicatesResolver duplicatesResolver = InitializeDuplicatesResolver(sourceRegistryRecord,
                    sourceRegistryRecord.FoundDuplicates,
                    duplicateRegistryRecord.RegNum,
                    string.Empty);

                duplicatesResolver.DetermineAvailableActions();

                // Override 'AddBatch' button visibility for permanant record
                // for permanant record 'AddBatch' option(Move Batchs) should be available ony if, 'EnableMoveBatch = true, in system settings
                bool canAddBatch = sourceRegistryRecord.IsTemporal ? duplicatesResolver.CanAddBatch : RegUtilities.GetAllowMoveBatch();
              
                var duplicateActions = new JObject(
                   new JProperty("ID", duplicateRegistryRecord.ID),
                   new JProperty("REGNUMBER", duplicateRegistryRecord.RegNum),
                   new JProperty("canAddBatch", canAddBatch),
                   new JProperty("canUseCompound", duplicatesResolver.CanCreateCompoundForm),
                   new JProperty("canUseStructure", duplicatesResolver.CanUseStructure)
               );
                duplicateActionList.Add(duplicateActions);
            }

            var tableName = string.Format("vw_mixture_regnumber WHERE regid IN ({0})", string.Join(",", duplicateRegIds));
            var query = GetQuery(tableName, RecordColumns, null, "modified", "regid");

            var responseMessage = new JObject(
                new JProperty("DuplicateRecords", ExtractData(query, null, null, null)),
                new JProperty("DuplicateActions", duplicateActionList)
            );
            return responseMessage;
        }

        #region Permanent Records
        [HttpGet]
        [Route(Consts.apiPrefix + "records")]
        [SwaggerOperation("GetRecords")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        [SwaggerResponse(500, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetRecords(int? skip = null, int? count = null, string sort = null, int? hitlistId = null, bool highlightSubStructures = false)
        {
            return await CallMethod(() =>
            {
                HitListInfo hitListInfo = GetHitlistInfo(hitlistId);
                return GetRegistryRecordsListView(false, skip, count, sort, hitListInfo, null, highlightSubStructures);
            }, new string[] { "SEARCH_REG" });
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "records/databaseRecordCount")]
        [SwaggerOperation("GetDatabaseRecordCount")]
        [SwaggerResponse(200, type: typeof(int))]
        [SwaggerResponse(401, type: typeof(JObject))]
        [SwaggerResponse(500, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetDatabaseRecordCount(bool? temp)
        {
            return await CallMethod(() =>
            {
                var bo = GetGenericBO(temp.HasValue ? temp.Value : false);
                bo.RefreshDatabaseRecordCount();
                bo.KeepRecordCountSyncrhonized = true;
                return bo.DatabaseRecordCount;
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

                const string personCreatedIdPath = "/MultiCompoundRegistryRecord/PersonCreated";
                XmlNode regNode = recordXml.SelectSingleNode(personCreatedIdPath);
                int personCreatedId = Convert.ToInt32(regNode.InnerText.Trim());

                bool isLoggedInUserOwner = UserIdentity.ID == personCreatedId ? true : false;
                bool isLoggedInUserSupervisor = COEUserBO.GetUserByID(personCreatedId).SupervisorID == UserIdentity.ID ? true : false;

                return new JObject(new JProperty("data", ChemistryHelper.ConvertStructuresToCdxml(recordXml).OuterXml),
                    new JProperty("isLoggedInUserOwner", isLoggedInUserOwner),
                    new JProperty("isLoggedInUserSuperVisor", isLoggedInUserSupervisor),
                    new JProperty("inventoryContainers", GetInventoryContainerList(id)));
            });
        }

        private JObject GetInventoryContainerList(int id)
        {
            if (id <= 0)
                return null;

            if (!RegUtilities.GetInventoryIntegration())
                return null;

            var regNum = GetRegNumber(id);
            RegistryRecord registryRecord = RegistryRecord.GetRegistryRecord(regNum);

            DataSet retVal = GetInvContainers(registryRecord.RegNumber.ID, -1);

            if (retVal == null || retVal.Tables.Count == 0)
                return null;

            var containerTable = retVal.Tables[0];
            containerTable.DefaultView.Sort = "CONTAINERID DESC";
            containerTable = containerTable.DefaultView.ToTable();

            var invContainerList = InvContainerList.NewInvContainerList(containerTable, registryRecord.RegNumber.RegNum);
            JObject invList = new JObject();

            var recordList = new JArray();
            foreach (InvContainer container in invContainerList)
            {
                recordList.Add(GenerateInvContainerJSON(container));
            }
            invList.Add(new JProperty("containers", recordList));

            var batchContainerList = new JArray();
            foreach (var batch in registryRecord.BatchList)
            {
                retVal = GetInvContainers(registryRecord.RegNumber.ID, batch.BatchNumber);
                invContainerList = InvContainerList.NewInvContainerList(containerTable, registryRecord.RegNumber.RegNum);
                foreach (InvContainer container in invContainerList)
                {
                    batchContainerList.Add(GenerateInvContainerJSON(container));
                }
            }
            invList.Add(new JProperty("batchContainers", batchContainerList));

            return invList;
        }

        private JObject GenerateInvContainerJSON(InvContainer container)
        {
            var containerObject = new JObject(
            new JProperty("containerID", container.ContainerID),
            new JProperty("qtyAvailable", container.QtyAvailable),
            new JProperty("containerSize", container.ContainerSize),
            new JProperty("location", RegAppHelper.ExtractHtmlInnerText(container.Location)),
            new JProperty("requestURL", container.RequestURL),
            new JProperty("requestFromBatchURL", container.RequestFromBatchURL),
            new JProperty("requestFromContainerURL", container.RequestFromContainerURL),
            new JProperty("containerType", container.ContainerType),
            new JProperty("regBatchID", container.RegBatchID),
            new JProperty("regNumber", container.RegNumber),
            new JProperty("invBatchID", container.InvBatchID),
            new JProperty("invCompoundID", container.InvCompoundID),
            new JProperty("totalQtyAvailable", container.TotalQtyAvailable),
            new JProperty("id", RegAppHelper.ExtractHtmlInnerText(container.Barcode)),
            new JProperty("idUrl", container.Barcode));
            return containerObject;
        }

        private DataSet GetInvContainers(int regid, int batchid)
        {
            COESearch search = new COESearch();
            ResultsCriteria rc = RegUtilities.GetInvContainersRC(regid, batchid);
            SearchCriteria sc = RegUtilities.GetInvContainersSC(regid, batchid);
            PagingInfo pi = new PagingInfo();
            pi.Start = 1;
            pi.End = 100001;
            pi.RecordCount = 100000;

            SearchResponse result = search.DoSearch(sc, rc, pi, RegUtilities.GetInvContainersDataViewID());
            return result.ResultsDataSet;
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "records")]
        [SwaggerOperation("CreateRecord")]
        [SwaggerResponse(201, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        public async Task<IHttpActionResult> CreateRecord(RecordData inputData)
        {
            return await CallMethod(() =>
            {
                if (string.IsNullOrWhiteSpace(inputData.DuplicateCheckOption))
                    inputData.DuplicateCheckOption = "N";

                RegistryRecord regRecord = null;
                try
                {
                    regRecord = RegisterRecord(inputData.Data, inputData.DuplicateCheckOption);
                }
                catch (Exception ex)
                {
                    throw new RegistrationException(ex.Message.Replace(@"<br />", " "), ex);
                }

                if (string.IsNullOrWhiteSpace(regRecord.FoundDuplicates))
                {
                    var data = new JObject(
                         new JProperty("batchCount", regRecord.BatchList.Count),
                         new JProperty("batchId", regRecord.BatchList[0].ID)
                     );

                    if (!string.IsNullOrEmpty(regRecord.RedBoxWarning))
                        data.Add(new JProperty("redBoxWarning", regRecord.RedBoxWarning));

                    return new ResponseData(regRecord.ID, regRecord.RegNum, null, data);
                }
                else
                {
                    // duplicate records found, return list of duplicate records
                    return new ResponseData(null, null, null, GetDuplicateRecords(regRecord));
                }
            });
        }

        private RegistryRecord RegisterRecord(string data, string duplicateCheckOption)
        {
            var doc = new XmlDocument();
            doc.LoadXml(data);
            var recordString = ChemistryHelper.ConvertStructuresToCdx(doc).OuterXml;

            const string regIdXPath = "/MultiCompoundRegistryRecord/ID";
            XmlNode regNode = doc.SelectSingleNode(regIdXPath);

            int regId = 0;
            int.TryParse(regNode.InnerText.Trim(), out regId);
            RegistryRecord registryRecord;
            if (regId > 0)
            {
                registryRecord = RegistryRecord.GetRegistryRecord(regId);
                registryRecord.InitializeFromXml(recordString, false, false);
            }
            else
            {
                registryRecord = RegistryRecord.NewRegistryRecord();
                registryRecord.InitializeFromXml(recordString, true, false);
                registryRecord.IsDirectReg = true;
            }
            registryRecord.ModuleName = ChemDrawWarningChecker.ModuleName.REGISTRATION;
            DuplicateCheck duplicateCheck = TranslateDuplicateCheckingInstruction(duplicateCheckOption);
            return registryRecord.Register(duplicateCheck);
        }

        /// <summary>
        /// Strips a UTF8 or UTF16 byte-order mark from the incoming string.
        /// </summary>
        /// <param name="stringToClean">the string to strip any leading byte-order mark from</param>
        /// <returns>the argument string with any UTF8 or UTF16 preamble removed</returns>
        private string RemoveContentBeforeProlog(string stringToClean)
        {
            if (string.IsNullOrEmpty(stringToClean))
                return stringToClean;

            // enable fall-through for case where no BOM is found
            string bomFree = stringToClean;

            // eliminate any BOM as well as any other unexpected characters
            // forces explicit encoding declaration for xml strings, otherwise assumes UTF-8
            int index = stringToClean.IndexOf("<");
            if (index > 0)
                bomFree = stringToClean.Substring(index, stringToClean.Length - index);

            return bomFree;
        }

        /// <summary>
        /// Translates incoming string-based definitions for RegistryRecord.DuplicateCheck
        /// enumeration members;
        /// </summary>
        /// <param name="duplicateCheck"> The duplicate check option</param>
        /// <returns>A matched enumeration member of RegistryRecord.DuplicateCheck</returns>
        private DuplicateCheck TranslateDuplicateCheckingInstruction(string duplicateCheck)
        {
            DuplicateCheck checkingMechanism = DuplicateCheck.None;
            string msg =
                "Invalid duplicate checking mechanism requested. Please use 'C'(ompound), 'M'(ixture), or 'N'(one)";
            switch (duplicateCheck.ToUpper())
            {
                case "COMPOUND":
                case "COMPOUNDCHECK":
                case "C":
                    {
                        checkingMechanism = DuplicateCheck.CompoundCheck;
                        break;
                    }
                case "MIXTURE":
                case "MIXCHECK":
                case "M":
                    {
                        checkingMechanism = DuplicateCheck.MixCheck;
                        break;
                    }
                case "NONE":
                case "N":
                    {
                        checkingMechanism = DuplicateCheck.None;
                        break;
                    }
                default:
                    {
                        throw new InvalidCastException(msg);
                    }
            }
            return checkingMechanism;
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "records/{id}")]
        [SwaggerOperation("UpdateRecord")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(401, type: typeof(JObject))]
        public async Task<IHttpActionResult> UpdateRecord(int id, RecordData inputData)
        {
            return await CallMethod(() =>
            {
                if (string.IsNullOrWhiteSpace(inputData.DuplicateCheckOption))
                    inputData.DuplicateCheckOption = "N";

                DuplicateCheck duplicateCheck = TranslateDuplicateCheckingInstruction(inputData.DuplicateCheckOption);
                string errorMessage = null;
                RegistryRecord registryRecord = null;
                RegistryRecord originalRegistryRecord = null;
                try
                {
                    errorMessage = "Unable to parse the incoming data as a well-formed XML document.";
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(inputData.Data);
                    errorMessage = "Unable to process chemical structures.";
                    var xml = ChemistryHelper.ConvertStructuresToCdx(xmlDoc).OuterXml;
                    errorMessage = "Unable to determine the registry number.";
                    const string regNumXPath = "/MultiCompoundRegistryRecord/RegNumber/RegNumber";
                    XmlNode regNode = xmlDoc.SelectSingleNode(regNumXPath);
                    string regNum = regNode.InnerText.Trim();
                    errorMessage = string.Format("Unable to find the registry entry: {0}", regNum);
                    registryRecord = RegistryRecord.GetRegistryRecord(regNum);
                    if (registryRecord == null) throw new Exception();
                    errorMessage = "Record is locked and cannot be updated.";
                    if (registryRecord.Status == RegistryStatus.Locked) throw new Exception();
                    originalRegistryRecord = registryRecord.Clone();
                    errorMessage = "Unable to update the internal record.";
                    registryRecord.UpdateFromXmlEx(xml);

                    bool checkOtherMixtures = false;
                    if (inputData.Action == CopiesAction.CopyComponentWithDupCheck.ToString() || inputData.Action == CopiesAction.CopyComponent.ToString()
                        || inputData.Action == CopiesAction.CopyStructureWithDupCheck.ToString() || inputData.Action == CopiesAction.CopyStructure.ToString())
                    {
                        if (inputData.Action == CopiesAction.CopyComponent.ToString() || inputData.Action == CopiesAction.CopyComponentWithDupCheck.ToString())
                            registryRecord.CopyEditedComponents();
                        else if (inputData.Action == CopiesAction.CopyStructure.ToString() || inputData.Action == CopiesAction.CopyStructureWithDupCheck.ToString())
                            registryRecord.CopyEditedStructures();

                        if (inputData.Action == CopiesAction.CopyComponentWithDupCheck.ToString() || inputData.Action == CopiesAction.CopyStructureWithDupCheck.ToString())
                        {
                            checkOtherMixtures = false;
                            duplicateCheck = DuplicateCheck.CompoundCheck;
                        }
                        else
                        {
                            checkOtherMixtures = false;
                            duplicateCheck = DuplicateCheck.None;
                        }
                    }
                    else if (inputData.Action == "Continue")
                    {
                        // 'Continue' option
                        duplicateCheck = DuplicateCheck.CompoundCheck;
                        checkOtherMixtures = false;
                    }
                    else if (inputData.Action == "DuplicateRecords")
                    {
                        duplicateCheck = DuplicateCheck.None;
                        checkOtherMixtures = false;
                    }
                    else
                    {
                        // use case: Record is edited and updated not via Copy action ( continue, create copies).
                        // TODO: if record is updated by adding a new batch only, then CheckOtherMixturs should be set to false.
                        //       for the above use case, concider the CheckOtherMixtures parameter sent from client side    

                        registryRecord.FixBatchesFragmentsEx();
                        checkOtherMixtures = registryRecord.ComponentList.IsDirty;
                    }

                    errorMessage = "Cannot set batch prefix.";
                    registryRecord.BatchPrefixDefaultOverride(true);
                    errorMessage = "Unable to save the record.";
                    registryRecord.ModuleName = ChemDrawWarningChecker.ModuleName.REGISTRATION;
                    registryRecord.CheckOtherMixtures = checkOtherMixtures;
                    registryRecord = registryRecord.Save(duplicateCheck);

                    ValidateUpdatedRecord(registryRecord);
                    if (string.IsNullOrWhiteSpace(registryRecord.FoundDuplicates))
                    {
                        return new ResponseData(regNumber: registryRecord.RegNumber.RegNum);
                    }
                    else
                    {
                        return HandleDuplicates(registryRecord);
                    }
                }
                catch (Exception ex)
                {
                    EditAffectsOtherMixturesException editAffectsOthersException = null;
                    if (ex is EditAffectsOtherMixturesException)
                        editAffectsOthersException = (EditAffectsOtherMixturesException)ex;
                    if (ex.GetBaseException() is EditAffectsOtherMixturesException)
                        editAffectsOthersException = (EditAffectsOtherMixturesException)ex.GetBaseException();
                    if (editAffectsOthersException != null)
                    {
                        string regNumbersLocked = FindRegNumbersLocked(editAffectsOthersException);
                        return GetCopyActions(registryRecord, originalRegistryRecord, editAffectsOthersException.Message, regNumbersLocked);
                    }

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

        #region Copy Handling
        private ResponseData GetCopyActions(RegistryRecord newRegistryRecord, RegistryRecord originalRegistryRecord, string message, string regNumbersLocked)
        {
            bool isStructureChanged = false;
            JObject copyActions = null;
            string affectedRegistries = GetAffectedRegistries(newRegistryRecord, originalRegistryRecord, out isStructureChanged);
            if (message.Equals("This update causes other registrations to be updated as well."))
            {
                if (!string.IsNullOrEmpty(affectedRegistries))
                {
                    bool allowContinue = !newRegistryRecord.BatchList[0].BatchComponentList[0].BatchComponentFragmentList.IsDirty
                        && newRegistryRecord.CanPropogateComponentEdits()
                        && newRegistryRecord.CanEditComponent(0)
                        && newRegistryRecord.IsEditable
                        && newRegistryRecord.ComponentList[0].Compound.CanPropogateComponentEdits;

                    // override allowContinue if user does not have privilege to propogate edits
                    string checkLockedRecords = RegUtilities.GetLockingEnabled() ? regNumbersLocked : string.Empty;
                    string canContinueOptionToolTip = string.Empty;
                    if (!string.IsNullOrEmpty(checkLockedRecords))
                    {
                        allowContinue = false;
                        canContinueOptionToolTip = string.Format("Compound is locked in Registration : {0}" + checkLockedRecords);
                    }

                    string action = isStructureChanged ? CopiesAction.CopyComponentWithDupCheck.ToString() : CopiesAction.CopyComponent.ToString();
                    copyActions = new JObject(new JProperty("caption", "There are several registrations affected."),
                                  new JProperty("message", "Editing these components will affect all registry records that refer to them. Choose one of the following options:"),
                                  new JProperty("canContinueOptionVisibility", allowContinue),
                                  new JProperty("canCreateCopiesOptionVisibility", true),
                                  new JProperty("okOptionVisibility", false),
                                  new JProperty("action", action));
                }
                else
                {
                    newRegistryRecord.CheckOtherMixtures = false;
                    newRegistryRecord = newRegistryRecord.Save(DuplicateCheck.None);
                    return new ResponseData(regNumber: newRegistryRecord.RegNumber.RegNum);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(affectedRegistries))
                {
                    bool allowCreateCopy = false;
                    bool.TryParse(RegUtilities.GetConfigSetting("REGADMIN", "AllowCreateCopySharedStructure"), out allowCreateCopy);
                    bool allowContinue = newRegistryRecord.CanPropogateStructureEdits() && newRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.CanPropogateStructureEdits;
                    string checkLockedRecords = RegUtilities.GetLockingEnabled() ? regNumbersLocked : string.Empty;
                    string canContinueOptionToolTip = string.Empty;
                    if (!string.IsNullOrEmpty(checkLockedRecords))
                    {
                        allowContinue = false;
                        canContinueOptionToolTip = string.Format("Compound is locked in Registration : {0}" + checkLockedRecords);
                    }

                    string action = isStructureChanged ? CopiesAction.CopyStructureWithDupCheck.ToString() : CopiesAction.CopyStructure.ToString();

                    copyActions = new JObject(new JProperty("caption", "There are several components affected."),
                                  new JProperty("message", "This structure and its data are shared, so these edits will affect all components that refer to it. Choose one of the following options:"),
                                  new JProperty("canContinueOptionVisibility", allowContinue),
                                  new JProperty("canCreateCopiesOptionVisibility", allowCreateCopy),
                                  new JProperty("okOptionVisibility", false),
                                  new JProperty("action", action));
                }
                else
                {
                    newRegistryRecord.CheckOtherMixtures = false;
                    newRegistryRecord = newRegistryRecord.Save(DuplicateCheck.None);
                    return new ResponseData(regNumber: newRegistryRecord.RegNumber.RegNum);
                }
            }

            var responseMessage = new JObject(new JProperty("copyActions", copyActions));
            return new ResponseData(null, null, null, responseMessage);
        }

        private string GetAffectedRegistries(RegistryRecord newRegistryRecord, RegistryRecord originalRegistryRecord, out bool structureChanged)
        {
            // here we will look through the components that match  - ignoring newly added components
            // we need to see if any of the propertylists are dirty.  If they are not, but the structure
            // is dirty, we need to assure that the edit to the structure changed the atom count
            // only in that instance will we alert the user that they could affect other registries
            string affectedRegistries = string.Empty;
            structureChanged = false;
            foreach (CambridgeSoft.COE.Registration.Services.Types.Component component in newRegistryRecord.ComponentList)
            {
                foreach (CambridgeSoft.COE.Registration.Services.Types.Component componentOriginal in originalRegistryRecord.ComponentList)
                {
                    if (componentOriginal.Compound.RegNumber.RegNum == component.Compound.RegNumber.RegNum)
                    {
                        if (component.IsDirty && (!component.Compound.FragmentList.IsDirty && !component.Compound.IdentifierList.IsDirty && !component.Compound.PropertyList.IsDirty
                            && !component.Compound.BaseFragment.Structure.IdentifierList.IsDirty && !component.Compound.BaseFragment.Structure.PropertyList.IsDirty))
                        {
                            // we need to know if the structure has changed to know whether or not to duplicate check when the editted record is submitted
                            if (!RegUtilities.CheckIfMWAndFormulaMatch(component.Compound.BaseFragment.Structure.Value.ToString(), componentOriginal.Compound.BaseFragment.Structure.Value.ToString()))
                            {
                                structureChanged = true;
                                if (string.IsNullOrEmpty(affectedRegistries))
                                {
                                    affectedRegistries = component.Compound.RegNumber.RegNum;
                                }
                                else
                                {
                                    affectedRegistries = affectedRegistries + ", " + component.Compound.RegNumber.RegNum;
                                }
                            }
                        }

                        if (component.IsDirty && (component.Compound.FragmentList.IsDirty || component.Compound.IdentifierList.IsDirty || component.Compound.PropertyList.IsDirty
                             || component.Compound.BaseFragment.Structure.IdentifierList.IsDirty || component.Compound.BaseFragment.Structure.PropertyList.IsDirty))
                        {
                            // we need to know if the structure has changed to know whether or not to duplicate check when the editted record is submitted
                            if (component.IsDirty)
                            {
                                if (!RegUtilities.CheckIfMWAndFormulaMatch(component.Compound.BaseFragment.Structure.Value.ToString(), componentOriginal.Compound.BaseFragment.Structure.Value.ToString()))
                                {
                                    structureChanged = true;
                                }
                            }
                            if (string.IsNullOrEmpty(affectedRegistries))
                            {
                                affectedRegistries = component.Compound.RegNumber.RegNum;
                            }
                            else
                            {
                                affectedRegistries = affectedRegistries + ", " + component.Compound.RegNumber.RegNum;
                            }

                        }
                        // Check when changes made to equivalents in fragments.
                        // Note : when SBI = TRUE.(Mandatory).
                        if (string.IsNullOrEmpty(affectedRegistries) && originalRegistryRecord.SameBatchesIdentity)
                        {
                            foreach (CambridgeSoft.COE.Registration.Services.Types.Batch currentBatch in newRegistryRecord.BatchList)
                            {
                                if (currentBatch.IsDirty)
                                {
                                    foreach (BatchComponent batchComponent in currentBatch.BatchComponentList)
                                    {
                                        BatchComponentFragmentList fl = batchComponent.BatchComponentFragmentList;
                                        if (fl.IsDirty)
                                        {
                                            if (batchComponent.ComponentIndex == componentOriginal.ComponentIndex)
                                            {
                                                if (string.IsNullOrEmpty(affectedRegistries))
                                                    affectedRegistries = componentOriginal.Compound.RegNumber.RegNum;
                                                else
                                                    affectedRegistries = affectedRegistries + ", " + componentOriginal.Compound.RegNumber.RegNum;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return affectedRegistries;
        }

        private string FindRegNumbersLocked(EditAffectsOtherMixturesException editAffextExp)
        {
            string regNumbers = string.Empty;
            string listofRegnumbers = string.Empty;
            foreach (string[] valueSet in editAffextExp.SharedComponentConflicts.Values)
            {
                foreach (string regnum in valueSet)
                {
                    listofRegnumbers = string.IsNullOrEmpty(listofRegnumbers) ? "'" + regnum + "'" : listofRegnumbers + ",'" + regnum + "'";
                }
            }
            regNumbers = RegistryRecord.GetLockedRegistryRecords(listofRegnumbers);
            return regNumbers;
        }
        #endregion

        #region Duplicate Handling
        private ResponseData HandleDuplicates(RegistryRecord registryRecord)
        {
            RegUtilities.DuplicateType duplicateType = GetDuplicateType(registryRecord, registryRecord.FoundDuplicates, false);
            JObject duplicateActions = null;
            string regNumber = null;
            switch (duplicateType)
            {
                case RegUtilities.DuplicateType.Compound:
                    return new ResponseData(null, null, null, GetDuplicateRecords(registryRecord));
                case RegUtilities.DuplicateType.Mixture:
                    duplicateActions = new JObject(new JProperty("caption", "There are duplicates of this record"),
                        new JProperty("message", "You are about to duplicate records. Do you want to proceed?"),
                        new JProperty("canContinueOptionVisibility", false),
                        new JProperty("canCreateCopiesOptionVisibility", false),
                        new JProperty("okOptionVisibility", true),
                        new JProperty("action", "DuplicateRecords"));
                    break;
                case RegUtilities.DuplicateType.None:
                    regNumber = registryRecord.RegNumber.RegNum;
                    break;
                case RegUtilities.DuplicateType.DuplicatedOrCopied:
                    registryRecord.UpdateFragments();
                    registryRecord.UpdateXml();
                    registryRecord = registryRecord.Save(DuplicateCheck.None);
                    regNumber = registryRecord.RegNumber.RegNum;
                    break;
            }

            JObject responseMessage = duplicateActions == null ? null : new JObject(new JProperty("copyActions", duplicateActions));
            return new ResponseData(null, regNumber, null, responseMessage);
        }

        private RegUtilities.DuplicateType GetDuplicateType(RegistryRecord registryRecord, string duplicatesXml, bool isPreReg)
        {
            if (string.IsNullOrEmpty(duplicatesXml))
                return RegUtilities.DuplicateType.None;

            if (duplicatesXml.Contains("<COMPOUNDLIST>") && duplicatesXml.Contains("</COMPOUNDLIST>"))
            {
                int startIndex = duplicatesXml.IndexOf("<COMPOUNDLIST>");
                int endIndex = duplicatesXml.LastIndexOf("</COMPOUNDLIST>") + "</COMPOUNDLIST>".Length;
                duplicatesXml = duplicatesXml.Substring(startIndex, endIndex - startIndex);

                DuplicatesResolver duplicatesResolver = new DuplicatesResolver(registryRecord, duplicatesXml, isPreReg);

                // Note: HasUnsolvedComponents check also initializes duplicatesResolver.Duplicates list
                bool hasUnResolvedComponents = duplicatesResolver.HasUnsolvedComponents;

                if (duplicatesResolver.Duplicates == null)
                {
                    return RegUtilities.DuplicateType.DuplicatedOrCopied;
                }
                else
                {
                    if (!registryRecord.IsSingleCompound && registryRecord.CanAutoSelectComponentForDupChk())
                    {   // this looks through all the components in the mixture that are duplicated and choose the first mactching
                        // component that is stored in the duplicate resolve.  after each interaction one 
                        // of the components is removed so you need to pad the index +1 or you miss the last component.
                        for (int i = 0; i < duplicatesResolver.CompoundsToResolve.Count + 1; i++)
                        {
                            registryRecord = duplicatesResolver.AutoCreateCompoundForm();
                        }
                        // now check to see if a duplicate mixture was found after tha autoresolution
                        if (registryRecord.DalResponseMessage.Contains("mixture"))
                        {
                            return RegUtilities.DuplicateType.Mixture;
                        }
                        else
                        {
                            return RegUtilities.DuplicateType.None;
                        }
                    }
                    return RegUtilities.DuplicateType.Compound;
                }
            }

            if (duplicatesXml.Contains("<REGISTRYLIST>") && duplicatesXml.Contains("</REGISTRYLIST>"))
            {
                int startIndex = duplicatesXml.IndexOf("<REGISTRYLIST>");
                int endIndex = duplicatesXml.LastIndexOf("</REGISTRYLIST>") + "</REGISTRYLIST>".Length;
                duplicatesXml = duplicatesXml.Substring(startIndex, endIndex - startIndex);

                DuplicatesList duplicatesList = new DuplicatesList(duplicatesXml, true, registryRecord.ComponentList.Count > 1);

                if (duplicatesList.Count > 0)
                    return RegUtilities.DuplicateType.Mixture;
                else
                {
                    if (registryRecord.IsTemporal)
                        registryRecord.Register(DuplicateCheck.None);
                    else
                        registryRecord.Save(DuplicateCheck.None);

                    return RegUtilities.DuplicateType.None;
                }
            }

            return RegUtilities.DuplicateType.None;
        }
        #endregion

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
                return new ResponseData(id: id, regNumber: regNum, message: string.Format("The registry record, {0}, was deleted successfully!", regNum));
            }, new string[] { "DELETE_REG", "EDIT_COMPOUND_REG" });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "records/delete-bulk")]
        [SwaggerOperation("DeleteRecords")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> DeleteRecords(JObject data)
        {
            return await CallMethod(() =>
            {
                return BulkDeleteRecords(data, false);
            }, new string[] { "DELETE_REG", "EDIT_COMPOUND_REG" });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "records/bulk-register")]
        [SwaggerOperation("BulkRegisterRecords")]
        [SwaggerResponse(201, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        public async Task<IHttpActionResult> BulkRegisterRecords(BulkRegistrationData inputData)
        {
            return await CallMethod(() =>
            {
                DuplicateAction duplicateAction = BulkRegistrationHelper.GetDuplicateAction(inputData.DuplicateAction);
                COEHitListBO coeHitListBO = COEHitListBO.New(Consts.REGDB, HitListType.MARKED);
                coeHitListBO.Update();

                try
                {
                    foreach (string recordId in inputData.Records)
                    {
                        coeHitListBO.MarkHit(Convert.ToInt32(recordId));
                    }

                    RegistryRecordList.ModuleName = ChemDrawWarningChecker.ModuleName.REGISTRATION;
                    RegRecordListInfo reglistInfo = RegistryRecordList.LoadRegistryRecordList(duplicateAction,
                        coeHitListBO.ID,
                        RegUtilities.GetApprovalsEnabled(),
                        UserIdentity.Name,
                        inputData.Description);

                    var args = new Dictionary<string, object>();
                    args.Add(":logId", reglistInfo.IntLogId);
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select vw1.log_id logId, vw1.description description, vw1.user_id userId,");
                    sql.Append(" vw2.temp_id tempId, vw2.reg_number regNumber, vw2.action, vw2.comments");
                    sql.Append(" from vw_log_bulkregistration_ID vw1 inner join vw_log_bulkregistration vw2");
                    sql.Append(" on vw1.log_id = vw2.log_id");
                    sql.AppendFormat(" where vw1.log_id = :logId");

                    JArray records = new JArray();
                    using (var reader = GetReader(sql.ToString(), args))
                    {
                        var fieldCount = reader.FieldCount;
                        while (reader.Read())
                        {
                            var row = new JObject();
                            for (int i = 0; i < fieldCount; ++i)
                            {
                                var fieldName = reader.GetName(i);
                                var fieldType = reader.GetFieldType(i);
                                object fieldData = null;
                                switch (fieldType.Name.ToLower())
                                {
                                    case "int16":
                                    case "int32":
                                        fieldData = reader.GetInt32(i);
                                        break;
                                    case "datetime":
                                        fieldData = reader.GetDateTime(i);
                                        break;
                                    case "decimal":
                                        fieldData = reader.GetDecimal(i);
                                        break;
                                    default:
                                        fieldData = reader.GetString(i);
                                        break;
                                }

                                row.Add(new JProperty(fieldName, fieldData));
                            }
                            records.Add(row);
                        }
                    }

                    foreach (JObject record in records)
                    {
                        RegistryRecord registryRecord = null;
                        if (Convert.ToUInt32(record["TEMPID"]) > 0)
                        {
                            registryRecord = RegistryRecord.GetRegistryRecord(Convert.ToInt32(record["TEMPID"]));
                        }
                        else
                        {
                            registryRecord = RegistryRecord.GetRegistryRecord(Convert.ToString(record["REGNUMBER"]));
                            record["TEMPID"] = registryRecord.ID;
                        }

                        string structure = string.Format("{0}record/{1}?{2}", registryRecord.IsTemporal ? "temp" : string.Empty, registryRecord.ID, DateTime.Now.ToString("yyyyMMddHHmmss"));
                        record.Add(new JProperty("structure", structure));
                    }

                    var response = new JObject(new JProperty("records", records));
                    return new ResponseData(null, null, null, data: response);
                }
                catch (Exception ex)
                {
                    COEHitListBO marked = COEHitListBO.Get(HitListType.MARKED, coeHitListBO.ID);
                    foreach (string recordId in inputData.Records)
                    {
                        marked.UnMarkHit(int.Parse(recordId));
                    }
                    throw ex;
                }
            });
        }
        #endregion // Permanent Records

        #region Temporary Records
        [HttpGet]
        [Route(Consts.apiPrefix + "temp-records")]
        [SwaggerOperation("GetTempRecords")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        [SwaggerResponse(500, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetTempRecords(int? skip = null, int? count = null, string sort = null, int? hitlistId = null, bool highlightSubStructures = false)
        {
            return await CallMethod(() =>
            {
                HitListInfo hitListInfo = GetHitlistInfo(hitlistId);
                return GetRegistryRecordsListView(true, skip, count, sort, hitListInfo, null, highlightSubStructures);
            }, new string[] { "SEARCH_TEMP" });
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

                const string personCreatedIdPath = "/MultiCompoundRegistryRecord/PersonCreated";
                XmlNode regNode = recordXml.SelectSingleNode(personCreatedIdPath);
                int personCreatedId = Convert.ToInt32(regNode.InnerText.Trim());

                bool isLoggedInUserOwner = UserIdentity.ID == personCreatedId ? true : false;
                bool isLoggedInUserSupervisor = COEUserBO.GetUserByID(personCreatedId).SupervisorID == UserIdentity.ID ? true : false;

                return new JObject(new JProperty("data", ChemistryHelper.ConvertStructuresToCdxml(recordXml).OuterXml),
                    new JProperty("isLoggedInUserOwner", isLoggedInUserOwner),
                    new JProperty("isLoggedInUserSuperVisor", isLoggedInUserSupervisor));
            });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "temp-records")]
        [SwaggerOperation("CreateTempRecord")]
        [SwaggerResponse(201, type: typeof(ResponseData))]
        [SwaggerResponse(401, type: typeof(JObject))]
        public async Task<IHttpActionResult> CreateTempRecord(JObject recordData)
        {
            return await CallMethod(() =>
            {
                var doc = new XmlDocument();
                doc.LoadXml((string)recordData["data"]);
                var recordString = ChemistryHelper.ConvertStructuresToCdx(doc).OuterXml;
                RegistryRecord registryRecord = RegistryRecord.NewRegistryRecord();
                registryRecord.InitializeFromXml(recordString, true, false);
                registryRecord.BatchPrefixDefaultOverride(true);
                registryRecord.ModuleName = ChemDrawWarningChecker.ModuleName.REGISTRATION;
                RegistryRecord savedRegistryRecord = registryRecord.Save();
                ValidateUpdatedRecord(savedRegistryRecord);

                if (CheckAuthorizations("SEARCH_REG"))
                {
                    return new ResponseData(savedRegistryRecord.ID);
                }
                else
                {
                    var recordXml = new XmlDocument();
                    recordXml.LoadXml(savedRegistryRecord.XmlWithAddIns);

                    JObject newRecordData = new JObject(new JProperty("data", ChemistryHelper.ConvertStructuresToCdxml(recordXml).OuterXml),
                        new JProperty("isLoggedInUserOwner", true),
                        new JProperty("isLoggedInUserSuperVisor", false));
                    return new ResponseData(savedRegistryRecord.ID, data: newRecordData);
                }

            }, new string[] { "ADD_COMPOUND_TEMP" });
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
                    registryRecord.UpdateFromXmlEx(xml);
                    errorMessage = "Cannot set batch prefix.";
                    registryRecord.BatchPrefixDefaultOverride(true);
                    errorMessage = "Unable to save the record.";
                    registryRecord.ModuleName = ChemDrawWarningChecker.ModuleName.REGISTRATION;
                    registryRecord = registryRecord.Save();
                    ValidateUpdatedRecord(registryRecord);
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

                return new ResponseData(id: registryRecord.ID);
            }, new string[] { "EDIT_COMPOUND_TEMP" });
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "temp-records/{id}/{status}")]
        [SwaggerOperation("UpdateTempRecordStatus")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> UpdateTempRecordStatus(int id, int status)
        {
            return await CallMethod(() =>
            {
                RegistryRecord registryRecord = RegistryRecord.GetRegistryRecord(id);
                int currentStatus = (int)registryRecord.Status;
                if (status < 0 || status > (int)RegistryStatus.Registered || currentStatus == status || (status != currentStatus + 1 && status != currentStatus - 1))
                    throw new RegistrationException("The specified status is invalid");
                registryRecord.Status = (RegistryStatus)status;
                if (registryRecord.Status == RegistryStatus.Locked || registryRecord.Status == RegistryStatus.Registered)
                    registryRecord.SetLockStatus();
                else
                    registryRecord.SetApprovalStatus();
                return new ResponseData(id: id, message: "The registry record status was updated successfully!");
            });
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
                return new ResponseData(id: id, message: string.Format("The temporary record, {0}, was deleted successfully!", id));
            }, new string[] { "DELETE_TEMP", "EDIT_COMPOUND_TEMP" });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "temp-records/delete-bulk")]
        [SwaggerOperation("DeleteTempRecords")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> DeleteTempRecords(JObject data)
        {
            return await CallMethod(() =>
            {
                return BulkDeleteRecords(data, true);
            }, new string[] { "DELETE_TEMP", "EDIT_COMPOUND_TEMP" });
        }

        private object BulkDeleteRecords(JObject data, bool temporary)
        {
            List<int> records = new List<int>();
            // expected input is an array of record ids
            // {
            // "data": [{"id":"1"},{"id":"2"}]
            // }
            JArray array = (JArray)data["data"];
            if (array == null)
                throw new ArgumentNullException("Input data is invalid");

            foreach (JObject item in array)
            {
                int id = Convert.ToInt32(item.GetValue("id"));
                if (temporary)
                    CheckTempRecordId(id);
                records.Add(id);
            }

            if (!temporary)
            {
                // for permanant registry records, hitlist need to be generated using mixture id of the perm record
                var list = ExtractData(string.Format("select MIXTUREID from  VW_MIXTURE m where m.REGID in ({0})", string.Join(",", records)), null, null, null);
                records.Clear();
                foreach (JObject item in list)
                {
                    records.Add(Convert.ToInt32(item.GetValue("MIXTUREID")));
                }
            }

            if (records.Count <= 0)
                throw new ArgumentNullException("Input data is invalid");

            var coeHitListBO = COEHitListBO.New(Consts.REGDB, HitListType.MARKED);
            coeHitListBO.Update();

            foreach (int recordId in records)
            {
                coeHitListBO.MarkHit(recordId);
            }

            string message = string.Empty;
            BulkDelete command = null;
            List<string> failedRecords = new List<string>();
            try
            {
                command = BulkDelete.Execute(coeHitListBO.ID, temporary, string.Empty);
                if (command.Result)
                {
                    if (command.FailedRecords.Length > 0)
                    {
                        message = string.Format(Resource.FollowingRecordsNotDeleted_Label_Text, string.Join(", ", command.FailedRecords));
                        foreach (string id in command.FailedRecords)
                        {
                            failedRecords.Add(id);
                        }
                    }
                    else
                    {
                        message = string.Format("{0} {1} deleted successfully!", records.Count, records.Count == 1 ? "record" : "records");
                    }
                }
                else
                {
                    message = Resource.NoRecordWasDeleted_Label_Text;
                }
            }
            catch (Exception ex)
            {
                foreach (int recordId in records)
                {
                    coeHitListBO.UnMarkHit(recordId);
                }
                throw ex;
            }

            var response = new JObject(new JProperty("status", command.Result));
            if (failedRecords.Count > 0)
            {
                var recordList = new JArray();
                foreach (string id in failedRecords)
                {
                    recordList.Add(new JObject() { new JProperty("id", id) });
                }
                response = new JObject();
                response.Add(new JProperty("failedRecords", recordList));
            }

            return new ResponseData(message: message, data: response);
        }
        #endregion // Tempoary Records

        #region Duplicate Resolution

        [HttpPost]
        [Route(Consts.apiPrefix + "records/duplicate")]
        [SwaggerOperation("CreateDuplicateRecord")]
        [SwaggerResponse(201, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        public async Task<IHttpActionResult> CreateDuplicateRecord(DuplicateActionData inputData)
        {
            return await CallMethod(() =>
            {
                if (string.IsNullOrWhiteSpace(inputData.DuplicateAction))
                    throw new RegistrationException("Invalid duplicate action name");

                List<string> validDuplicateActions = new List<string>(new string[] { "AddBatch", "Duplicate", "UseComponent", "UseStructure" });
                if (!validDuplicateActions.Contains(inputData.DuplicateAction))
                {
                    throw new RegistrationException("Invalid duplicate action name");
                }

                // for `Duplicate' option, the given record is registered as  duplicate
                // for other options, the api expects a selected duplicate records reg number
                if (!inputData.DuplicateAction.Equals("Duplicate"))
                {
                    if (string.IsNullOrWhiteSpace(inputData.RegNo))
                        throw new RegistrationException("Invalid registration number");
                }

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(inputData.Data);
                const string regNumXPath = "/MultiCompoundRegistryRecord/RegNumber/RegNumber";
                XmlNode regNode = xmlDoc.SelectSingleNode(regNumXPath);

                RegistryRecord registryRecord = null;
                if (regNode == null || string.IsNullOrEmpty(regNode.InnerText))
                {
                    // use case: record is not registered yet
                    // try to register record to get the duplicates
                    registryRecord = RegisterRecord(inputData.Data, "C");
                }
                else
                {
                    // use case: record is alrady registered 
                    // try to save the record to get the duplicates
                    string regNum = regNode.InnerText.Trim();
                    registryRecord = RegistryRecord.GetRegistryRecord(regNum);
                    if (registryRecord == null) throw new Exception();
                    var recordXml = ChemistryHelper.ConvertStructuresToCdx(xmlDoc).OuterXml;
                    // errorMessage = "Record is locked and cannot be updated.";
                    // if (registryRecord.Status == RegistryStatus.Locked) throw new Exception();
                    registryRecord.UpdateFromXmlEx(recordXml);

                    registryRecord.ModuleName = ChemDrawWarningChecker.ModuleName.REGISTRATION;
                    registryRecord.CheckOtherMixtures = false;
                    registryRecord = registryRecord.Save(DuplicateCheck.CompoundCheck);
                }

                // do duplicate resolution based on the given duplicate action
                if (!string.IsNullOrWhiteSpace(registryRecord.FoundDuplicates))
                {
                    DuplicatesResolver duplicatesResolver = InitializeDuplicatesResolver(registryRecord,
                        registryRecord.FoundDuplicates,
                        inputData.RegNo,
                        inputData.DuplicateAction);

                    // re-calculate privilges for duplicate resolution actions
                    duplicatesResolver.DetermineAvailableActions();

                    switch (inputData.DuplicateAction)
                    {
                        case "AddBatch":
                            registryRecord = duplicatesResolver.AddBatch();
                            break;
                        case "Duplicate":
                            registryRecord = duplicatesResolver.CreateDuplicate();
                            break;
                        case "UseComponent":
                            registryRecord = duplicatesResolver.CreateCompoundForm();
                            break;
                        case "UseStructure":
                            registryRecord = duplicatesResolver.UseStructure();
                            break;
                        default:
                            throw new RegistrationException("Invalid duplicate action");
                    }

                    return new ResponseData(registryRecord.ID, registryRecord.RegNum, null, null);
                }
                return new ResponseData(message: "Registration of record failed due to a problem.");
            });
        }

        private DuplicatesResolver InitializeDuplicatesResolver(RegistryRecord registryRecord, string duplicateXml, string duplicateRecordRegNum, string duplicateAction)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(registryRecord.FoundDuplicates);
            XmlNodeList nodeList = xmldoc.GetElementsByTagName("REGNUMBER");

            int selectedDuplicateRecord = 0;
            // get selected dulicate records's index           
            if (!duplicateAction.Equals("Duplicate"))
            {
                foreach (XmlNode node in nodeList)
                {
                    if (node.InnerText.Equals(duplicateRecordRegNum))
                    {
                        break;
                    }
                    selectedDuplicateRecord++;
                }
            }

            DuplicatesResolver duplicatesResolver = new DuplicatesResolver(registryRecord, registryRecord.FoundDuplicates, false);

            // Note: HasUnsolvedComponents check also initializes duplicatesResolver.Duplicates list
            if (!duplicatesResolver.HasUnsolvedComponents)
            {
                throw new RegistrationException("No un-resolved components found in the duplicate records");
            }

            duplicatesResolver.Duplicates.CurrentIndex = selectedDuplicateRecord;
            return duplicatesResolver;
        }

        #endregion
    }
}
