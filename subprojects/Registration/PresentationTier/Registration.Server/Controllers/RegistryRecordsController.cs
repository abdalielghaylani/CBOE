using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;
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
        private static bool GetSubStructureHighlight(int hitlistId, bool temporary)
        {
            var queryData = SearchController.GetHitlistQueryInternal(hitlistId, temporary);
            var searchCriteria = new SearchCriteria();
            searchCriteria.GetFromXML(queryData.SearchCriteria);
            var structureCriteria = searchCriteria.FindStructCriteria() as CambridgeSoft.COE.Framework.Common.SearchCriteria.StructureCriteria;
            return structureCriteria == null || structureCriteria.Similar == SearchCriteria.COEBoolean.No;
        }

        private void CheckTempRecordId(int id)
        {
            var args = new Dictionary<string, object>();
            args.Add(":id", id);
            var count = Convert.ToInt32(ExtractValue("SELECT cast(count(1) as int) FROM vw_temporarycompound WHERE tempbatchid=:id", args));
            if (count == 0)
                throw new IndexOutOfRangeException(string.Format("Cannot find temporary Compound Id: {0}", id));
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

        private int GetMixtureId(int id)
        {
            var args = new Dictionary<string, object>();
            args.Add(":id", id);
            var mixtureID = (int)ExtractValue("SELECT mixtureid FROM vw_mixture_regnumber WHERE regid=:id", args);
            if (mixtureID == 0)
                throw new IndexOutOfRangeException(string.Format("Cannot find registration ID, {0}", id));
            return mixtureID;
        }

        /// <summary>
        /// Gets regid of a record using register number
        /// </summary>
        /// <param name="regNumber">register number</param>
        private long GetPermanentRecordRegId(string regNumber)
        {
            var args = new Dictionary<string, object>();
            args.Add(":regNumber", regNumber);
            var regId = Convert.ToInt32(ExtractValue("SELECT regid FROM vw_mixture_regnumber WHERE regnumber=:regNumber", args));
            if (regId == 0)
                throw new IndexOutOfRangeException(string.Format("Cannot find registration number, {0}", regNumber));

            return regId;
        }

        private void ValidateUpdatedRecord(RegistryRecord registryRecord)
        {
            if (registryRecord.ID == 0 || !string.IsNullOrEmpty(registryRecord.RedBoxWarning))
            {
                var message = !string.IsNullOrEmpty(registryRecord.RedBoxWarning) ? registryRecord.RedBoxWarning : "An unexpected error happened while saving the registry record!";
                throw new RegistrationException(message.Replace(@"<br />", " "));
            }
        }
        
        /// <summary>
        /// Gets details of duplciate records
        /// </summary>
        /// <param name="sourceRegistryRecord">source registry record</param>
        /// <param name="recordColumns">record columns; optional</param>
        /// <param name="sortField">sort field; optional</param>
        /// <param name="sortOrder">sort order; optional</param>
        /// <returns>details of duplciate records</returns>
        private JObject GetDuplicateRecords(RegistryRecord sourceRegistryRecord, RecordColumn[] recordColumns = null, string sortField = "modified", string sortOrder = "desc")
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
                long regId = GetPermanentRecordRegId(node.InnerText);
                duplicateRegIds.Add(regId.ToString());
                DuplicatesResolver duplicatesResolver = InitializeDuplicatesResolver(sourceRegistryRecord,
                    sourceRegistryRecord.FoundDuplicates,
                    node.InnerText,
                    string.Empty);

                duplicatesResolver.DetermineAvailableActions();

                // Override 'AddBatch' button visibility for permanant record
                // for permanant record 'AddBatch' option(Move Batchs) should be available ony if, 'EnableMoveBatch = true, in system settings
                bool canAddBatch = sourceRegistryRecord.IsTemporal ? duplicatesResolver.CanAddBatch : RegUtilities.GetAllowMoveBatch();
                var duplicateActions = new JObject(
                   new JProperty("ID", regId),
                   new JProperty("REGNUMBER", node.InnerText),
                   new JProperty("canAddBatch", canAddBatch),
                   new JProperty("canUseCompound", duplicatesResolver.CanCreateCompoundForm),
                   new JProperty("canUseStructure", duplicatesResolver.CanUseStructure)
               );
                duplicateActionList.Add(duplicateActions);
            }

            var tableName = string.Format("vw_mixture_regnumber WHERE regid IN ({0})", string.Join(",", duplicateRegIds));

            var query = string.Empty;
            if (recordColumns == null) recordColumns = RecordColumns;
            sortField = sortField.ToLower();
            if (sortOrder.ToUpper().Trim().Equals("DESC"))
            {
                // null for sortTerms which sorts in DESC by default.
                query = GetQuery(tableName, recordColumns, null, sortField, "regid");
            }
            else
            {
                // can specify sort terms as each sortby field with sort order, separated by comma.. e.g.: "regnumber asc"
                sortOrder = sortField + " " + sortOrder;
                query = GetQuery(tableName, recordColumns, sortOrder, string.Empty, "regid");
            }

            var responseMessage = new JObject(
                new JProperty("DuplicateRecords", ExtractData(query, null, null, null)),
                new JProperty("DuplicateActions", duplicateActionList)
            );
            return responseMessage;
        }

        /// <summary>
        /// Gets the same document with elements sorted based on attribute value/element text
        /// </summary>
        /// <param name="xDoc">input document</param>
        /// <param name="isAscending">is ascending or not</param>
        /// <param name="columnP">primary sort column name</param>
        /// <param name="columnPType">primary sort column type</param>
        /// <param name="columnS">secondary sort column name</param>
        /// <param name="columnSType">secondary sort column type</param>
        /// <returns>Returns document with elements sorted</returns>
        private XDocument SortDuplicateXmlNodes(XDocument xDoc, bool isAscending, string columnPrimary, Type columnTypePrimary, string columnSecondary, Type columnTypeSecondary)
        {
            const string REGNUMBER = "REGNUMBER";
            const string REGISTRYLIST = "REGISTRYLIST";
            const string COMPOUNDLIST = "COMPOUNDLIST";
            const string COMPOUND = "COMPOUND";
            const string STRING = "string";
            const string INT32 = "int32";
            const string DATETIME = "datetime";
            IOrderedEnumerable<XElement> empty = null;
            var firstSortedData = empty;
            var finalSortedData = empty;
            IEnumerable<XElement> regnumberElements = xDoc.Element(COMPOUNDLIST).Element(COMPOUND).Element(REGISTRYLIST).Elements(REGNUMBER);
            XElement registryListElement = xDoc.Element(COMPOUNDLIST).Element(COMPOUND).Element(REGISTRYLIST);


            if (columnTypePrimary.Name.ToLower().Equals(REGNUMBER.ToLower()))
            {
                if (isAscending)
                    firstSortedData = regnumberElements.OrderBy(element => (string)element.Value);
                else
                    firstSortedData = regnumberElements.OrderByDescending(element => (string)element.Value);
            }
            else
            {
                if (columnTypePrimary.Name.ToLower().Equals(STRING))
                {
                    if (isAscending)
                        firstSortedData = regnumberElements.OrderBy(element => (string)element.Attribute(columnPrimary));
                    else
                        firstSortedData = regnumberElements.OrderByDescending(element => (string)element.Attribute(columnPrimary));
                }
                else if (columnTypePrimary.Name.ToLower().Equals(INT32))
                {
                    if (isAscending)
                        firstSortedData = regnumberElements.OrderBy(element => (int)element.Attribute(columnPrimary));
                    else
                        firstSortedData = regnumberElements.OrderByDescending(element => (int)element.Attribute(columnPrimary));
                }
                else if (columnTypePrimary.Name.ToLower().Equals(DATETIME))
                {
                    if (isAscending)
                        firstSortedData = regnumberElements.OrderBy(element => (DateTime)element.Attribute(columnPrimary));
                    else
                        firstSortedData = regnumberElements.OrderByDescending(element => (DateTime)element.Attribute(columnPrimary));
                }
            }

            if (columnTypeSecondary.Name.ToLower().Equals(STRING))
            {
                if (isAscending)
                    finalSortedData = firstSortedData.ThenBy(element => (string)element.Attribute(columnSecondary));
                else
                    finalSortedData = firstSortedData.ThenByDescending(element => (string)element.Attribute(columnSecondary));
            }
            else if (columnTypeSecondary.Name.ToLower().Equals(INT32))
            {
                if (isAscending)
                    finalSortedData = firstSortedData.ThenBy(element => (int)element.Attribute(columnSecondary));
                else
                    finalSortedData = firstSortedData.ThenByDescending(element => (int)element.Attribute(columnSecondary));
            }
            else if (columnTypeSecondary.Name.ToLower().Equals(DATETIME))
            {
                if (isAscending)
                    finalSortedData = firstSortedData.ThenBy(element => (DateTime)element.Attribute(columnSecondary));
                else
                    finalSortedData = firstSortedData.ThenByDescending(element => (DateTime)element.Attribute(columnSecondary));
            }

            XElement compoundElement = xDoc.Element(COMPOUNDLIST).Element(COMPOUND);
            compoundElement.Element(REGISTRYLIST).Remove();
            registryListElement = new XElement(REGISTRYLIST, finalSortedData);
            compoundElement.Add(registryListElement);

            return xDoc;
        }

        /// <summary>
        /// Just initialize and return a record object using provided record xml
        /// </summary>
        /// <param name="data">xml data</param>
        /// <returns>registry record object</returns>
        private RegistryRecord InitializeRecordFromXml(string data)
        {
            const string REGIDXPATH = "/MultiCompoundRegistryRecord/ID";
            int regId = 0;
            var doc = new XmlDocument();
            doc.LoadXml(data);
            XmlNode regNode = doc.SelectSingleNode(REGIDXPATH);
            
            int.TryParse(regNode.InnerText.Trim(), out regId);
            RegistryRecord registryRecord;
            if (regId > 0)
            {
                string regNum = string.Empty;
                const string REGNUMBERXPATH = "/MultiCompoundRegistryRecord/RegNumber/RegNumber";
                regNode = doc.SelectSingleNode(REGNUMBERXPATH);
                regNum = regNode.InnerText.Trim();
                if (!string.IsNullOrEmpty(regNum))
                {
                    registryRecord = RegistryRecord.GetRegistryRecord(regNum);                    
                }
                else
                {
                    registryRecord = RegistryRecord.GetRegistryRecord(regId);               
                }
                registryRecord.InitializeFromXml(data, false, false);
            }
            else
            {
                registryRecord = RegistryRecord.NewRegistryRecord();
                registryRecord.InitializeFromXml(data, true, false);
                registryRecord.IsDirectReg = true;
            }

            return registryRecord;
        }

        /// <summary>
        /// Return only the required list of sorted duplicate reg records in xml from the entire list
        /// </summary>
        /// <param name="duplicateSummaryDoc">xml document contains list of duplicate reg records</param>
        /// <param name="duplicatesRetrieveCount">required number of duplicates</param>
        /// <param name="duplicatesSkipCount">node position in xml after which required number of duplicates are to be retrieved</param>
        /// <param name="countAfterTrim">count of actual records returned after trimming</param>
        /// <returns>required list of duplicate reg records</returns>
        private string GetTrimmedDuplicateSummary(XmlDocument duplicateSummaryDoc, int duplicatesRetrieveCount, int duplicatesSkipCount, out int countAfterTrim)
        {
            countAfterTrim = 0;
            XmlNode regListNode = null;
            XmlNodeList requiredDuplicateNodes = duplicateSummaryDoc.SelectNodes(
                "//COMPOUNDLIST/COMPOUND/REGISTRYLIST/REGNUMBER[position()>" + duplicatesSkipCount.ToString() +
                " and position()<=" + (duplicatesSkipCount + duplicatesRetrieveCount).ToString() + "]");

            if (requiredDuplicateNodes != null && requiredDuplicateNodes.Count > 1)
            {
                XmlNode parentNode = duplicateSummaryDoc.DocumentElement as XmlNode ;
                XmlDocument documentClone = new XmlDocument();
                documentClone.AppendChild(documentClone.ImportNode(parentNode, true));
                regListNode = documentClone.SelectSingleNode("//COMPOUNDLIST/COMPOUND/REGISTRYLIST");
                regListNode.RemoveAll();
                foreach (XmlNode regNode in requiredDuplicateNodes)
                {
                    regListNode.AppendChild(documentClone.ImportNode(regNode, true));
                    countAfterTrim++;
                }
                string trimmedDuplicatesString = regListNode.OwnerDocument.InnerXml;
                documentClone = null; parentNode = null; regListNode = null;

                return trimmedDuplicatesString;
            }
            countAfterTrim = GetDuplicatesCountFromXml(duplicateSummaryDoc);
            return duplicateSummaryDoc.InnerXml;
        }

        /// <summary>
        /// Returns the count of duplicates identified
        /// </summary>
        /// <param name="duplicateSummaryDoc">xml document contains list of duplicate reg records</param>
        /// <returns>count of duplicates</returns>
        private int GetDuplicatesCountFromXml(XmlDocument duplicateSummaryDoc)
        {
            return duplicateSummaryDoc.SelectNodes("//COMPOUNDLIST/COMPOUND/REGISTRYLIST/REGNUMBER").Count;
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
                if (highlightSubStructures && hitlistId != null && hitlistId.Value > 0)
                    highlightSubStructures = GetSubStructureHighlight(hitlistId.Value, false);
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
                string regNum = string.Empty;
                if (id < 0)
                {
                    record = service.RetrieveNewRegistryRecord();
                }
                else
                {
                    regNum = GetRegNumber(id);
                    record = service.RetrieveRegistryRecord(regNum);
                }

                var recordXml = new XmlDocument();
                recordXml.LoadXml(record);

                const string personCreatedIdPath = "/MultiCompoundRegistryRecord/PersonCreated";
                XmlNode regNode = recordXml.SelectSingleNode(personCreatedIdPath);

                int personCreatedId;
                bool isLoggedInUserOwner = false;
                bool isLoggedInUserSupervisor = false;
                if (int.TryParse(regNode.InnerText.Trim(), out personCreatedId)) 
                {
                    isLoggedInUserOwner = UserIdentity.ID == personCreatedId ? true : false;
                    isLoggedInUserSupervisor = COEUserBO.GetUserByID(personCreatedId).SupervisorID == UserIdentity.ID ? true : false;
                }

                return new JObject(new JProperty("data", record),
                    new JProperty("isLoggedInUserOwner", isLoggedInUserOwner),
                    new JProperty("isLoggedInUserSuperVisor", isLoggedInUserSupervisor),
                    new JProperty("inventoryContainers", GetInventoryContainerList(regNum)));
            });
        }

        private JObject GetInventoryContainerList(string regNum)
        {
            if (string.IsNullOrEmpty(regNum))
                return null;

            if (!RegUtilities.GetInventoryIntegration())
                return null;
         
            RegistryRecord registryRecord = RegistryRecord.GetRegistryRecord(regNum);
            // get container list for registry record level
            DataSet retVal = GetInvContainers(registryRecord.RegNumber.ID, -1);

            if (retVal == null || retVal.Tables.Count == 0)
                return null;

            var containerTable = retVal.Tables[0];
            if (containerTable.Rows.Count == 0)
                return null;

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

            // get continerlist for each batch
            var batchContainerList = new JArray();            
            List<string> uniqueBatchList = new List<string>();
          
            foreach (InvContainer container in invContainerList)
            {
                if (uniqueBatchList.Contains(container.RegBatchID))
                    continue;

                var batch = registryRecord.BatchList.GetBatchFromFullRegNum(container.RegBatchID);
                retVal = GetInvContainers(registryRecord.RegNumber.ID, batch.BatchNumber);
                if (retVal == null || retVal.Tables.Count == 0)
                {
                    continue;
                }
                containerTable = retVal.Tables[0];
                containerTable.DefaultView.Sort = "CONTAINERID DESC";
                containerTable = containerTable.DefaultView.ToTable();
                var batchInvContainerList = InvContainerList.NewInvContainerList(containerTable, registryRecord.RegNumber.RegNum);
                foreach (InvContainer batchContainer in batchInvContainerList)
                {
                    batchContainerList.Add(GenerateInvContainerJSON(batchContainer));
                }
                uniqueBatchList.Add(container.RegBatchID);
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
                RegistryRecord regRecord = null;
                if (string.IsNullOrWhiteSpace(inputData.DuplicateCheckOption))
                    inputData.DuplicateCheckOption = "N";

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
                    XmlDocument duplicateSummaryDoc = new XmlDocument();
                    try
                    {
                        duplicateSummaryDoc.LoadXml(regRecord.FoundDuplicates);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    JObject duplicateInfo = new JObject();
                    duplicateInfo.Add("TotalDuplicateCount", GetDuplicatesCountFromXml(duplicateSummaryDoc));

                    return new ResponseData(null, null, null, duplicateInfo);
                }
            });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "get-duplicate-records")]
        [SwaggerOperation("GetDuplicateRegRecordsOnDemand")]
        [SwaggerResponse(201, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetDuplicateRegRecordsOnDemand(FetchDuplicatesOnDemandOptions inputDataAndOptions)
        {
            return await CallMethod(() =>
            {
                int countAfterTrim;
                const string CREATOR = "CREATOR";
                const string ID = "ID";
                const string REGID = "REGID";
                RegistryRecord regRecord = null;
                Dictionary<string, string> regRecordDBTablePropertyMappings = new Dictionary<string, string>() {
                    {"NAME", "name"}, {"CREATED", "created"}, {"MODIFIED", "modified"}, {CREATOR, "personcreated"}, 
                    {"REGNUMBER", "regnumber"}, {"STATUS", "statusid"}, {"APPROVED", "approved"}, {ID, "regid"}
                };
                Dictionary<string, Type> regRecordDBTablePropertyTypeMappings = new Dictionary<string, Type>() {
                    {"NAME", typeof(string)}, {"CREATED", typeof(DateTime)}, {"MODIFIED", typeof(DateTime)}, {"PERSONCREATED", typeof(string)}, 
                    {"REGNUMBER", typeof(string)}, {"STATUSID", typeof(int)}, {"APPROVED", typeof(string)}, {REGID, typeof(int)}
                };

                if (string.IsNullOrEmpty(inputDataAndOptions.Sort))
                    inputDataAndOptions.Sort = "MODIFIED";
                if (string.IsNullOrEmpty(inputDataAndOptions.SortOrder))
                    inputDataAndOptions.SortOrder = "DESC";
                inputDataAndOptions.Sort = inputDataAndOptions.Sort.Trim();
                inputDataAndOptions.SortOrder = inputDataAndOptions.SortOrder.Trim().ToUpper();
                
                XmlDocument duplicateSummaryDoc = new XmlDocument();
                try
                {
                    // Just initialize a rcord boject with given data
                    regRecord = InitializeRecordFromXml(inputDataAndOptions.Data);

                    // Call stored procedure and get duplicate summary xml
                    string duplicateSummaryXml = RegDal.GetRegRecordDuplicatesSummary(regRecord.Xml);

                    var xDoc = XDocument.Parse(duplicateSummaryXml);
                    string sortColumn;
                    if (!regRecordDBTablePropertyMappings.TryGetValue(inputDataAndOptions.Sort.ToUpper(), out sortColumn))
                        throw new Exception(@"Error: Sort column '" + inputDataAndOptions.Sort.ToUpper() + @"' is not identified.");

                    inputDataAndOptions.Sort = sortColumn;
                    // Sort the regnumber nodes in the xml based on the sort columns and sort order
                    XDocument sortedDocument = SortDuplicateXmlNodes(xDoc, 
                        (inputDataAndOptions.SortOrder.Equals("ASC")),
                        inputDataAndOptions.Sort.ToUpper(), 
                        regRecordDBTablePropertyTypeMappings[inputDataAndOptions.Sort.ToUpper()],
                        regRecordDBTablePropertyMappings[ID].ToUpper(),
                        regRecordDBTablePropertyTypeMappings[REGID]);

                    duplicateSummaryXml = string.Join(string.Empty, sortedDocument.Elements().Select(element => element.ToString()));
                    duplicateSummaryDoc.LoadXml(duplicateSummaryXml);
                    
                    // Get only required range of sorted records
                    regRecord.FoundDuplicates = GetTrimmedDuplicateSummary(duplicateSummaryDoc, inputDataAndOptions.Count, inputDataAndOptions.Skip, 
                        out countAfterTrim);
                }
                catch (Exception ex)
                {
                    if (ex is RegistrationException)
                        throw new RegistrationException(ex.Message.Replace(@"<br />", " "), ex);
                    throw ex;
                }

                string creatorNameRetrievingQuery = @"(select userid from vw_people where personid=" + regRecordDBTablePropertyMappings[CREATOR] + @")";
                if (inputDataAndOptions.Sort.ToUpper().Equals(CREATOR))
                    inputDataAndOptions.Sort = creatorNameRetrievingQuery;

                int sourceArrayLength = RecordColumns.Length;
                RecordColumn[] recordColumnsCustomized = new RecordColumn[sourceArrayLength + 1];
                Array.Copy(RecordColumns, recordColumnsCustomized, sourceArrayLength);
                recordColumnsCustomized[sourceArrayLength] = new RecordColumn()
                {
                    Definitions = creatorNameRetrievingQuery,
                    Label = "creatorName", Sortable = true
                };
                JObject duplicateRecords = GetDuplicateRecords(regRecord, recordColumnsCustomized, inputDataAndOptions.Sort, inputDataAndOptions.SortOrder);
                duplicateRecords.Add("TotalDuplicateCount", GetDuplicatesCountFromXml(duplicateSummaryDoc));
                duplicateRecords.Add("RequestedDuplicateCount", inputDataAndOptions.Count);
                duplicateRecords.Add("ReturningDuplicateCount", countAfterTrim);

                return new ResponseData(null, null, null, duplicateRecords);
            });
        }

        private RegistryRecord RegisterRecord(string data, string duplicateCheckOption)
        {
            var doc = new XmlDocument();
            doc.LoadXml(data);

            const string regIdXPath = "/MultiCompoundRegistryRecord/ID";
            XmlNode regNode = doc.SelectSingleNode(regIdXPath);

            int regId = 0;
            int.TryParse(regNode.InnerText.Trim(), out regId);
            RegistryRecord registryRecord;
            if (regId > 0)
            {
                registryRecord = RegistryRecord.GetRegistryRecord(regId);
                registryRecord.InitializeFromXml(data, false, false);
            }
            else
            {
                registryRecord = RegistryRecord.NewRegistryRecord();
                registryRecord.InitializeFromXml(data, true, false);
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
                    var xml = inputData.Data;
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xml);
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
                bool isStructureIdentifierListChanged = IsStructureIdentifierListChanged(component);
                foreach (CambridgeSoft.COE.Registration.Services.Types.Component componentOriginal in originalRegistryRecord.ComponentList)
                {
                    if (componentOriginal.Compound.RegNumber.RegNum == component.Compound.RegNumber.RegNum)
                    {
                        if (component.IsDirty && (!component.Compound.FragmentList.IsDirty && !component.Compound.IdentifierList.IsDirty && !component.Compound.PropertyList.IsDirty
                            && !isStructureIdentifierListChanged && !component.Compound.BaseFragment.Structure.PropertyList.IsDirty))
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
                        if (component.IsDirty && component.Compound.FragmentList.IsDirty && !component.Compound.IdentifierList.IsDirty && !component.Compound.PropertyList.IsDirty
                            && !isStructureIdentifierListChanged && !component.Compound.BaseFragment.Structure.PropertyList.IsDirty && !originalRegistryRecord.SameBatchesIdentity)
                        {
                            continue;
                        }
                        else if (component.IsDirty && (component.Compound.FragmentList.IsDirty || component.Compound.IdentifierList.IsDirty || component.Compound.PropertyList.IsDirty
                             || isStructureIdentifierListChanged || component.Compound.BaseFragment.Structure.PropertyList.IsDirty))
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

        /// <summary>
        /// IsStructureIdentifierListChanged method
        /// </summary>
        /// <param name="component">component object</param>
        /// <returns>true or false</returns>
        private bool IsStructureIdentifierListChanged(CambridgeSoft.COE.Registration.Services.Types.Component component)
        {
            if (component.Compound.BaseFragment.Structure.IdentifierList.IsDirty)
                return true;
           
            foreach (CambridgeSoft.COE.Registration.Services.Types.Identifier identifier in component.Compound.BaseFragment.Structure.IdentifierList)
            {
                //this is required as any newly added structure identifier don't mark the entire structure identifier list dirty
                if (identifier.IsNew) // is true in case any identifier is newly added or value modified
                    return true;
            }

            return false;           
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
                    XmlDocument duplicateSummaryDoc = new XmlDocument();
                    try
                    {
                        duplicateSummaryDoc.LoadXml(registryRecord.FoundDuplicates);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    JObject duplicateInfo = new JObject();
                    duplicateInfo.Add("TotalDuplicateCount", GetDuplicatesCountFromXml(duplicateSummaryDoc));
                    return new ResponseData(null, null, null, duplicateInfo);
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
                var record = RegistryRecord.GetRegistryRecord(regNum);
                if (!record.CanEditRegistry())
                    throw new PrivilegeNotHeldException(string.Format("Not allowed to delete the registered record, {0}", regNum));
                var mixtureId = GetMixtureId(id);
                RegistryRecord.DeleteRegistryRecord(regNum);
                var markedHitList = GetMarkedHitListBO(false);
                markedHitList.UnMarkHit(mixtureId);
                return new ResponseData(id: id, regNumber: regNum, message: string.Format("The registry record, {0}, was deleted successfully!", regNum));
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
                var formGroup = GetFormGroup(true);
                var markedHitList = COEHitListBO.GetMarkedHitList(Consts.REGDB, COEUser.Name, formGroup.Id);

                try
                {
                    RegistryRecordList.ModuleName = ChemDrawWarningChecker.ModuleName.REGISTRATION;
                    RegRecordListInfo reglistInfo = RegistryRecordList.LoadRegistryRecordList(duplicateAction,
                        markedHitList.ID,
                        RegUtilities.GetApprovalsEnabled(),
                        UserIdentity.Name,
                        inputData.Description);

                    var args = new Dictionary<string, object>();
                    args.Add(":logId", reglistInfo.IntLogId);
                    StringBuilder sql = new StringBuilder();

                    sql.Append("select a.logid logId, a.description description, decode(a.userid,null,p1.user_id,a.userid) userid,");
                    sql.Append(" a.tempid, a.regNumber, a.action, a.comments, a.regid from");
                    sql.Append(" (select b.logId, b.description, b.regNumber,b.userId,decode(b.userid,null,a.personcreated,0) p1,");
                    sql.Append(" b.action, b.comments, b.tempid, b.regid from");
                    sql.Append(" (select vw1.log_id logId, vw1.description description, p.user_id userId,");
                    sql.Append(" vw2.temp_id tempId, vw2.reg_number regNumber, vw2.action, vw2.comments,");
                    sql.Append(" (select regid from vw_mixture_regnumber where regnumber = vw2.reg_number) regId");
                    sql.Append(" from vw_log_bulkregistration_ID vw1 ,  vw_log_bulkregistration vw2, vw_temporarycompound vw3, coedb.people p");
                    sql.Append(" where vw1.log_id = vw2.log_id and vw2.temp_id=vw3.tempcompoundid(+)");
                    sql.Append(" and p.person_id(+)=vw3.personcreated and vw1.log_id = :logId)b,vw_mixture_regnumber a");
                    sql.Append(" where a.regnumber(+)=b.regnumber) a, coedb.people p1");
                    sql.Append(" where p1.person_id(+)=a.p1");

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
                        int recordId = 0;
                        bool temp = false;
                        if (Convert.ToUInt32(record["TEMPID"]) > 0)
                        {
                            recordId = Convert.ToInt32(record["TEMPID"]);
                            temp = true;
                        }
                        else
                        {
                            recordId = Convert.ToInt32(record["REGID"]);
                            // TEMPID field is used in client UI to navigate to record details page for both temp and perm record
                            record["TEMPID"] = recordId;
                        }
                        var structure = GetStructureData(string.Format("{0}record", temp ? "temp" : string.Empty), recordId);
                        record.Add(new JProperty("structure", structure));
                    }

                    var response = new JObject(new JProperty("records", records));
                    return new ResponseData(null, null, null, data: response);
                }
                catch (Exception ex)
                {
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
                if (highlightSubStructures && hitlistId != null && hitlistId.Value > 0)
                    highlightSubStructures = GetSubStructureHighlight(hitlistId.Value, true);
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

                int userID = UserIdentity.ID;
                bool isLoggedInUserOwner = userID == personCreatedId ? true : false;
                bool isLoggedInUserSupervisor = COEUserBO.GetUserByID(personCreatedId).SupervisorID == userID ? true : false;

                if (id < 0) 
                {
                    const string sequenceIdPath = "/MultiCompoundRegistryRecord/RegNumber/SequenceID";
                    XmlNode sequenceNode = recordXml.SelectSingleNode(sequenceIdPath);
                    sequenceNode.InnerText = "-1";
                    record = recordXml.InnerXml;
                }

                return new JObject(new JProperty("data", record),
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
                var recordString = (string)recordData["data"];
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
                    JObject newRecordData = new JObject(new JProperty("data", savedRegistryRecord.XmlWithAddIns),
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
                var record = RegistryRecord.GetRegistryRecord(id);
                if (!record.CanEditRegistry())
                    throw new PrivilegeNotHeldException(string.Format("Not allowed to delete the temporary record, {0}", id));
                RegistryRecord.DeleteRegistryRecord(id);
                var markedHitList = GetMarkedHitListBO(true);
                markedHitList.UnMarkHit(id);
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

                var xml = inputData.Data;
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                const string regNumXPath = "/MultiCompoundRegistryRecord/RegNumber/RegNumber";
                XmlNode regNode = xmlDoc.SelectSingleNode(regNumXPath);

                RegistryRecord registryRecord = null;
                if (regNode == null || string.IsNullOrEmpty(regNode.InnerText))
                {
                    // use case: record is not registered yet
                    // try to register record to get the duplicates
                    registryRecord = RegisterRecord(xml, "C");
                }
                else
                {
                    // use case: record is already registered 
                    // try to save the record to get the duplicates
                    string regNum = regNode.InnerText.Trim();
                    registryRecord = RegistryRecord.GetRegistryRecord(regNum);
                    if (registryRecord == null) throw new Exception();
                    // errorMessage = "Record is locked and cannot be updated.";
                    // if (registryRecord.Status == RegistryStatus.Locked) throw new Exception();
                    registryRecord.UpdateFromXmlEx(xml);

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
