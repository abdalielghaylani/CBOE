using System;
using System.Collections.Generic;
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

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class RegistryRecordsController : RegControllerBase
    {
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
                throw new RegistrationException(message);
            }
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
            }, new string[] { "SEARCH_REG" });
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
                    new JProperty("isLoggedInUserSuperVisor", isLoggedInUserSupervisor));
            });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "records")]
        [SwaggerOperation("CreateRecord")]
        [SwaggerResponse(201, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        public async Task<IHttpActionResult> CreateRecord(DuplicateResolutionData inputData)
        {
            return await CallMethod(() =>
            {
                if (string.IsNullOrWhiteSpace(inputData.DuplicateCheckOption))
                    inputData.DuplicateCheckOption = "N";

                RegistryRecord regRecord = RegisterRecord(inputData.Data, inputData.DuplicateCheckOption);

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
        public async Task<IHttpActionResult> UpdateRecord(int id, DuplicateResolutionData inputData)
        {
            return await CallMethod(() =>
            {
                var doc = new XmlDocument();
                doc.LoadXml(inputData.Data);
                var recordString = ChemistryHelper.ConvertStructuresToCdx(doc).OuterXml;
                if (string.IsNullOrWhiteSpace(inputData.DuplicateCheckOption))
                    inputData.DuplicateCheckOption = "N";

                DuplicateCheck duplicateCheck = TranslateDuplicateCheckingInstruction(inputData.DuplicateCheckOption);
                string errorMessage = null;
                RegistryRecord registryRecord = null;
                RegistryRecord originalRegistryRecord = null;
                try
                {
                    errorMessage = "Unable to parse the incoming data as a well-formed XML document.";
                    var xml = string.Empty;
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(inputData.Data);
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
                    originalRegistryRecord = registryRecord.Clone();
                    errorMessage = "Unable to update the internal record.";
                    registryRecord.UpdateFromXmlEx(xml);

                    bool checkOtherMixtures = false;

                    // If user selected 'Continue' or 'Create Copies' action from UI
                    if (inputData.CopyAction)
                    {
                        // no need to check whether other mixtures affected
                        checkOtherMixtures = false;
                        if (inputData.CreateCopies)
                        {
                            // 'Create Copies' option
                            if (IsStructureChanged(registryRecord, originalRegistryRecord))
                            {
                                registryRecord.CopyEditedStructures();
                                duplicateCheck = DuplicateCheck.CompoundCheck;
                            }
                            else
                            {
                                // TODO: CopyEdited Components method is throwing an exception
                                // registryRecord.CopyEditedComponents();
                                duplicateCheck = DuplicateCheck.None;
                            }
                        }
                        else
                        {
                            // 'Continue' option
                            duplicateCheck = DuplicateCheck.CompoundCheck;
                        }
                    }
                    else
                    {
                        // use case: Record is edited and updated not via Copy action ( contine, create copies).
                        // TODO: if record is updated by adding a new batch only, then CheckOtherMixturs should be set to false.
                        //       for the above use case, concider the CheckOtherMixtures parameter sent from client side
                        // checkOtherMixtures = inputData.CheckOtherMixtures;
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
                        // duplicate records found, return list of duplicate records
                        return new ResponseData(null, null, null, GetDuplicateRecords(registryRecord));
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

                    copyActions = new JObject(new JProperty("caption", "There are several registrations affected."),
                                  new JProperty("message", "Editing these components will affect all registry records that refer to them. Choose one of the following options:"),
                                  new JProperty("canContinueOptionVisibility", allowContinue),
                                  new JProperty("canCreateCopiesOptionVisibility", true));
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

                    copyActions = new JObject(new JProperty("caption", "There are several components affected."),
                                  new JProperty("message", "This structure and its data are shared, so these edits will affect all components that refer to it. Choose one of the following options:"),
                                  new JProperty("canContinueOptionVisibility", allowContinue),
                                  new JProperty("canCreateCopiesOptionVisibility", allowCreateCopy));
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

        private bool IsStructureChanged(RegistryRecord newRegistryRecord, RegistryRecord originalRegistryRecord)
        {
            // here we will look through the components that match  - ignoring newly added components
            // we need to see if any of the propertylists are dirty.  If they are not, but the structure
            // is dirty, we need to assure that the edit to the structure changed the atom count          
            string affectedRegistries = string.Empty;
            bool structureChanged = false;
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
                                break;
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
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return structureChanged;
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

                var duplicateActions = new JObject(
                   new JProperty("ID", duplicateRegistryRecord.ID),
                   new JProperty("REGNUMBER", duplicateRegistryRecord.RegNum),
                   new JProperty("canAddBatch", duplicatesResolver.CanAddBatch),
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
                var tableName = "vw_temporarycompound c inner join vw_temporarybatch b on c.tempbatchid = b.tempbatchid";
                var query = GetQuery(tableName, TempRecordColumns, sort, "c.datelastmodified", "tempcompoundid");
                return new JObject(
                    new JProperty("temporary", true),
                    new JProperty("startIndex", skip == null ? 0 : Math.Max(skip.Value, 0)),
                    new JProperty("totalCount", Convert.ToInt32(ExtractValue("SELECT cast(count(1) as int) c FROM " + tableName))),
                    new JProperty("rows", ExtractData(query, null, skip, count))
                );
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
                return new ResponseData(savedRegistryRecord.ID);
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

                // get duplicates for the given input record data
                RegistryRecord registryRecord = RegisterRecord(inputData.Data, "C");

                if (!string.IsNullOrWhiteSpace(registryRecord.FoundDuplicates))
                {
                    DuplicatesResolver duplicatesResolver = InitializeDuplicatesResolver(registryRecord,
                        registryRecord.FoundDuplicates,
                        inputData.RegNo,
                        inputData.DuplicateAction);

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
