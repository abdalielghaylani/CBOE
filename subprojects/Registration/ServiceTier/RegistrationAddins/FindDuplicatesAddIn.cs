using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Registration.Services.AddIns;
using System.Xml;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Registration.Services.Types;
using System.Data;

namespace CambridgeSoft.COE.Registration.Services.RegistrationAddins
{

    /// <summary>
    /// Determines if matching components exist in the user's permanent registry, based on
    /// Registration System Settings.
    /// <para>
    /// Actually calls a public method of the RegistryRecord instance, for the time being.
    /// </para>
    /// </summary>
    [Serializable]
    public class FindDuplicatesAddIn : IAddIn
    {
        private IRegistryRecord _registryRecord;
        private bool IsUpdateFlow = false;
        
        /// <summary>
        /// Checks the database for matches based on the structure, a single component property,
        /// or a single component identifier of the proposed registry record.
        /// </summary>
        /// <param name="sender">the registryRecord object to take action against</param>
        /// <param name="args">in this case, empty event arguments</param>
        public void OnRegisteringHandler(object sender, EventArgs args)
        {
            _registryRecord = (IRegistryRecord)sender;
            //do not go through this path for the simplied mixture workflow
            if ((_registryRecord.AllowUnregisteredComponents == false && _registryRecord.ComponentList.Count == 1) || (_registryRecord.AllowUnregisteredComponents == true))
            {
                if (_registryRecord.IsDirty && (int)_registryRecord.CheckDuplicates != 3)
                    FindComponentDuplicates();
                else if (_registryRecord.IsDirty)
                    FindAndmarkDuplicate(); //Fix for CSBR-161146 CustomFieldDuplicates Addin: Duplicates table is not updated with the duplicate records
            }
        }

        /// <summary>
        /// Checks the database for matches based on the structure, a single component property,
        /// or a single component identifier of an existing registry record.
        /// </summary>
        /// <param name="sender">the registryRecord object to take action against</param>
        /// <param name="args">in this case, empty event arguments</param>
        public void OnUpdatingPermHandler(object sender, EventArgs args)
        {
            _registryRecord = (IRegistryRecord)sender;
            IsUpdateFlow = true;
            if ((_registryRecord.AllowUnregisteredComponents == false && _registryRecord.ComponentList.Count == 1) || (_registryRecord.AllowUnregisteredComponents == true))
            {
                if (!_registryRecord.IsNew && _registryRecord.IsDirty && (int)_registryRecord.CheckDuplicates != 3)
                    FindComponentDuplicates();
            }
        }

        /// <summary>
        /// Performs the underlying search for duplicates, and sets the response on the Registryrecord instance.
        /// </summary>
        private void FindComponentDuplicates()
        {
            string duplicateResponse = string.Empty;

            List<DuplicateCheckResponse> responses = RegistryRecord.FindDuplicates(_registryRecord.DataStrategy);
            int matches = 0;

            // make sure none of the responses have matches
            foreach (DuplicateCheckResponse response in responses)
            {
                //csbr-162125  the following code is a workaround due to a limitation in this addin. there is no way
                //to stop duplicate checking when we know the structure has not changed. however, in this case the 
                //duplicate will always be itself and we therefore know that the structure was not editted and can ignore the duplicate check
                if (IsUpdateFlow)
                {
                    if (response.MatchedRegistrations.Count == 1 && response.MatchedRegistrations[0].RegistryNumber == RegistryRecord.RegNumber.RegNum)
                        response.MatchedRegistrations.Clear();// Clear the response as its refering current object as a duplicate.
                    else
                    {
                        // Remove the matched response when its refering current object as duplicate record in a given list of duplicate matched responses.
                        List<DuplicateCheckResponse.MatchedRegistration> matchedRegistration = new List<DuplicateCheckResponse.MatchedRegistration>();
                        matchedRegistration.AddRange(response.MatchedRegistrations);
                        int removeIndex = 0;

                        foreach (DuplicateCheckResponse.MatchedRegistration regs in matchedRegistration)
                        {
                            if (RegistryRecord.RegNumber.RegNum == regs.RegistryNumber)
                            {
                                response.MatchedRegistrations.RemoveAt(removeIndex);
                            }
                            removeIndex += 1;
                        }
                    }
                }
            }

            foreach (DuplicateCheckResponse response in responses)
            {
                if (response.MatchedRegistrations.Count > 0)
                    matches++;
            }

            // if we have found matches, compose a document which will mimic the INSERT DB response
            if (matches > 0)
                duplicateResponse = ComposeDuplicateXml(responses, matches);

            //set the response, even if it is still empty ('empty' signifies no matches)
            _registryRecord.SetDuplicateResponse(duplicateResponse);
        }
                /// <summary>
        /// Performs the underlying search for duplicates, and  frame the duplicate xml and set that to registry record.
        /// CSBR-161146: CustomFieldDuplicates Addin: Duplicates table is not updated with the duplicate records
        /// </summary>
        private void FindAndmarkDuplicate()
        {
            
            StringBuilder duplicateResponse = new StringBuilder("<ROWSET>");
            List<DuplicateCheckResponse> responses = RegistryRecord.FindDuplicates(_registryRecord.DataStrategy);

            foreach (DuplicateCheckResponse response in responses)
            {
               {
                    if (response.MatchedRegistrations.Count == 1 && response.MatchedRegistrations[0].RegistryNumber == RegistryRecord.RegNumber.RegNum)
                        response.MatchedRegistrations.Clear();// Clear the response as its refering current object as a duplicate.
                    else
                    {
                        List<DuplicateCheckResponse.MatchedRegistration> matchedRegistration = new List<DuplicateCheckResponse.MatchedRegistration>();
                        matchedRegistration.AddRange(response.MatchedRegistrations);
                        int removeIndex = 0;

                        foreach (DuplicateCheckResponse.MatchedRegistration regs in matchedRegistration)
                        {
                            if (RegistryRecord.RegNumber.RegNum == regs.RegistryNumber)
                            {
                                response.MatchedRegistrations.RemoveAt(removeIndex);
                            }
                            else
                            {
                                duplicateResponse.AppendLine("<REGNUMBER NEW='NEW_COMPOUNDID'>" + regs.MatchedComponents[0].RegistryNumber + "</REGNUMBER>");
                            }
                            removeIndex += 1;
                        }
                    }
                }
            }
            duplicateResponse.AppendLine("</ROWSET>");
            _registryRecord.SetCustomDuplicateResponse(duplicateResponse.ToString());
        }

        /// <summary>
        /// Convert the newer OO duplicate messages into the older-style XML, for now, to avoid
        /// breaking consumer methods.
        /// </summary>
        /// <param name="responses"></param>
        /// <param name="numComponentsMatched"></param>
        /// <returns></returns>
        private string ComposeDuplicateXml(List<DuplicateCheckResponse> responses, int numComponentsMatched)
        {
            StringBuilder buf = new StringBuilder();

            buf.AppendFormat("<Response message=\"{0} duplicated components.\">", numComponentsMatched.ToString());
            buf.Append("<Error>");
            buf.Append("<COMPOUNDLIST>");

            foreach (DuplicateCheckResponse response in responses)
            {
                if (response.MatchedRegistrations.Count > 0)
                {
                    int responseIndex = responses.IndexOf(response);
                    int componentIndex = _registryRecord.ComponentList[responseIndex].Compound.ID;

                    buf.Append("<COMPOUND>");
                    buf.AppendFormat("<TEMPCOMPOUNDID>{0}</TEMPCOMPOUNDID>", componentIndex);
                    buf.Append("<REGISTRYLIST>");
                   
                    foreach (DuplicateCheckResponse.MatchedRegistration reg in response.MatchedRegistrations)
                    {
                       buf.AppendFormat("<REGNUMBER CompoundID=\"{0}\" SAMEFRAGMENT=\"{1}\" SAMEEQUIVALENT=\"{2}\">{3}</REGNUMBER>"
                            , reg.MatchedComponents[0].Id.ToString()
                            , reg.MatchedComponents[0].SameFragments == "True" ? "True": "False"
                            , reg.MatchedComponents[0].SameEquivalents
                            , reg.RegistryNumber
                        );

                    }

                    buf.Append("</REGISTRYLIST>");
                    buf.Append("</COMPOUND>");
                }
            }

            buf.Append("</COMPOUNDLIST>");
            buf.Append("</Error>");
            buf.Append("<Result></Result>");
            buf.Append(@"</Response>");
            string responseXml = buf.ToString();

            return responseXml;
        }

        #region IAddIn Members

        /// <summary>
        /// Contains the instance to take actions on.
        /// </summary>
        public IRegistryRecord RegistryRecord
        {
            get
            {
                return _registryRecord;
            }
            set
            {
                _registryRecord = value;
            }
        }

        /// <summary>
        /// Provides settings, as necessary, via an xml document for the add-in.
        /// </summary>
        /// <param name="xmlConfiguration">an XML string</param>
        public void Initialize(string xmlConfiguration)
        {
            if (!string.IsNullOrEmpty(xmlConfiguration))
            {
                try
                {
                    XmlDocument config = new XmlDocument();
                    config.LoadXml(xmlConfiguration);
                }
                catch
                {
                    //NOTE: Do nothing...there is no separate configuration for this at this time!
                }
            }
        }

        #endregion

        //#region Events Handlers

        //public void OnRegisteringHandler(object sender, EventArgs args)
        //{

        //    _registryRecord = (IRegistryRecord)sender;
        //    // Bypass call to FindDuplicates if CheckDuplicates is None
        //    // TODO:  Use actual Enum value rather than hardcode integer 3
        //    if (_registryRecord.IsDirty && (int)_registryRecord.CheckDuplicates != 3)
        //    {
        //        string duplicateResponse = FindDuplicates();
        //        _registryRecord.SetDuplicateResponse(duplicateResponse);
        //    }
        //}

        //public void OnUpdatingPermHandler(object sender, EventArgs args)
        //{
        //    _registryRecord = (IRegistryRecord)sender;
        //    if (!_registryRecord.IsNew && _registryRecord.IsDirty && (int)_registryRecord.CheckDuplicates != 3)
        //    {
        //        string duplicateResponse = FindDuplicates();
        //        _registryRecord.SetDuplicateResponse(duplicateResponse);
        //    }
        //}

        //#endregion

        //#region Variables
        
        //private IRegistryRecord _registryRecord;
        //private Behavior _behavior = Behavior.NotSet;
        //private int _dataViewID = -1;
        //private string _fieldName = string.Empty;
        //private string _searchCriteria = string.Empty;
        //private string[] _resultFields = { "" };
        //private readonly string _splitter = "|";
        //private readonly string COMPOUNDPROPERTYKEY = "Compound_Property_Value";
        //private readonly string TEMPCOMPOUNDIDKEY = "TEMPCOMPOUNDID";
        //private readonly string COMPOUNDIDKEY = "COMPOUNDID";
        //private readonly string REGNUMBERKEY = "REGNUMBER";

        //private int _noStructureID = -2; //Default value.
        //private DataTable _foundDulicatesTable = new DataTable("FoundDuplicates");

        ///// <summary>
        ///// Behavior of the addIn (object to apply duplicates checking)
        ///// </summary>
        //public enum Behavior
        //{
        //    CompoundStructure,
        //    CompoundProperty,
        //    NotSet,
        //}

        //#endregion

        //#region IAddIn Members

        //public IRegistryRecord RegistryRecord
        //{
        //    get
        //    {
        //        return _registryRecord;
        //    }
        //    set
        //    {
        //        _registryRecord = value;
        //    }
        //}

        //public void Initialize(string xmlConfiguration)
        //{
        //    try
        //    {
        //        XmlDocument document = new XmlDocument();
        //        document.LoadXml(xmlConfiguration);
                
        //        //Default behavior of this AddIn
        //        XmlNode currentNode = document.SelectSingleNode("AddInConfiguration/Behavior");
        //        if (currentNode != null && currentNode.InnerText.Length > 0)
        //        {
        //            if (Enum.IsDefined(typeof(Behavior), currentNode.InnerText))
        //                _behavior = (Behavior)Enum.Parse(typeof(Behavior), currentNode.InnerText);
        //        }
        //        //Required for the search (except CompoundStructure which is done in the DB by default.
        //        currentNode = document.SelectSingleNode("AddInConfiguration/DataViewID");
        //        if (currentNode != null && currentNode.InnerText.Length > 0)
        //            _dataViewID = int.Parse(currentNode.InnerText);

        //        //Required for the search (except CompoundStructure which is done in the DB by default.
        //        currentNode = document.SelectSingleNode("AddInConfiguration/PropertyName");
        //        if (currentNode != null && currentNode.InnerText.Length > 0)
        //            _fieldName = currentNode.InnerText;

        //        currentNode = document.SelectSingleNode("AddInConfiguration/SearchCriteria");
        //        if (currentNode != null && currentNode.InnerText.Length > 0)
        //            _searchCriteria = currentNode.InnerText;

        //        currentNode = document.SelectSingleNode("AddInConfiguration/ResultFields");
        //        if (currentNode != null && currentNode.InnerText.Length > 0)
        //            _resultFields = RegAddInsUtilities.ParseArray(currentNode.InnerText, _splitter.ToCharArray());

        //        currentNode = document.SelectSingleNode("AddInConfiguration/NoStructureID");
        //        if (currentNode != null && currentNode.InnerText.Length > 0)
        //            _noStructureID = int.Parse(currentNode.InnerText);

        //    }
        //    catch
        //    {
        //        //Default Settings.
        //        _behavior = Behavior.CompoundStructure;
        //    }
        //}

        //#endregion

        //#region Methods

        ///// <summary>
        ///// Find the duplicates given a behavior
        ///// </summary>
        ///// <returns>List of found duplicates or an empty string</returns>
        //private string FindDuplicates()
        //{
        //    string retVal = string.Empty;

        //    switch (_behavior)
        //    {
        //        case Behavior.CompoundProperty:
        //            this.FindDuplicatesByCompProperty(ref retVal);
        //            break;
        //        case Behavior.CompoundStructure:
        //            this.FindDuplicatesByCompStructure(ref retVal);
        //            break;
        //        default:
        //            this.FindDuplicatesByCompStructure(ref retVal);
        //            break;
        //    }

        //    return retVal;
        //}

        ///// <summary>
        ///// Finds the duplicates inside of the Compound Propertylist
        ///// </summary>
        ///// <param name="output">Found duplicates</param>
        //private void FindDuplicatesByCompProperty(ref string output)
        //{
        //    if (this._foundDulicatesTable.Columns.Count == 0) 
        //        this.SetResultsColumns();
        //    foreach (Component component in _registryRecord.ComponentList)
        //    {
        //        //Just for nostructure ids (not for any component)
        //        if (component.Compound.BaseFragment.Structure.ID == _noStructureID)
        //        {
        //            //DO search.
        //            _searchCriteria = RegAddInsUtilities.FormatSearchCriteria(_searchCriteria, COMPOUNDPROPERTYKEY, _fieldName, component);
        //            if (!string.IsNullOrEmpty(_searchCriteria))
        //            {
        //                DataSet results = this.SearchDuplicates();
        //                if (results != null)
        //                    this.AddToResults(results, component.Compound.ID.ToString());
        //            }
        //        }
        //    }
            
        //    if(_foundDulicatesTable.Rows.Count > 0)
        //        output = RegAddInsUtilities.FormatResponse(_foundDulicatesTable, _registryRecord);
        //}

        ///// <summary>
        ///// Set default results columns
        ///// </summary>
        //private void SetResultsColumns()
        //{
        //    switch (_behavior)
        //    {
        //        case Behavior.CompoundProperty:
        //            _foundDulicatesTable.Columns.Add(TEMPCOMPOUNDIDKEY);
        //            _foundDulicatesTable.Columns.Add(COMPOUNDIDKEY);
        //            _foundDulicatesTable.Columns.Add(REGNUMBERKEY);
        //            break;
        //    }
        //}

        ///// <summary>
        ///// Convert from one datasource to another kind for easier handling later on
        ///// </summary>
        ///// <param name="tempResults"></param>
        ///// <param name="compoundID"></param>
        //private void AddToResults(DataSet tempResults, string compoundID)
        //{
        //    for(int i = 0; i < tempResults.Tables[0].Rows.Count; i++)
        //    {
        //        DataRow currentDataRow = _foundDulicatesTable.NewRow();
        //        currentDataRow[TEMPCOMPOUNDIDKEY] = compoundID.ToString();
        //        if (tempResults.Tables[0].Rows[i][COMPOUNDIDKEY] != null)
        //            currentDataRow[COMPOUNDIDKEY] = tempResults.Tables[0].Rows[i][COMPOUNDIDKEY].ToString();
        //        if (tempResults.Tables[1].Rows[i][REGNUMBERKEY] != null)
        //            currentDataRow[REGNUMBERKEY] = tempResults.Tables[1].Rows[i][REGNUMBERKEY].ToString();
        //        _foundDulicatesTable.Rows.Add(currentDataRow);
        //    }
        //}
    
        ///// <summary>
        ///// Find duplicates
        ///// </summary>
        ///// <returns></returns>
        //private DataSet SearchDuplicates()
        //{
        //    DataSet ds = new DataSet("Duplicates");
        //    if (!string.IsNullOrEmpty(_fieldName) && _dataViewID > -1)
        //    {
        //        DataResult dataResult = RegAddInsUtilities.DoSearch(_dataViewID, _searchCriteria, _resultFields);
        //        if (!dataResult.Status.Contains("FAILURE"))
        //            RegAddInsUtilities.ConvertToDataTable(dataResult, ref ds);
        //    }
        //    return ds.Tables.Count > 0 ? ds : null;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks>By Default the DB has this kind of checking, so we can avoid it (or we should change the scripts)</remarks>
        //private void FindDuplicatesByCompStructure(ref string output)
        //{
        //    output = string.Empty;
        //}

        //#endregion
    }
}
