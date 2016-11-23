using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.Common;
using System.Data;
using System;

namespace COEServiceLib
{
    /// <summary>
    /// class for caching the values to be displayed in text box for base table text fields and all look up fields
    /// </summary>
    public class PickListCache
    {
        #region Variables
        List<PickListCacheModel> _typeAheadValueCacheModels;
        COEService _theCOEService;
        Dictionary<int, bool> pickListValueSettingCollection;
        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        private PickListCache()
        {
            _typeAheadValueCacheModels = new List<PickListCacheModel>();
            pickListValueSettingCollection = new Dictionary<int, bool>();
        }

        /// <summary>
        /// Method to create the instance of PickListCache
        /// </summary>
        /// <param name="theCOEService">COEService instance</param>
        /// <returns>returns the fully instantiated object of PickListCache using lazy initialization</returns>
        public static PickListCache Instance(COEService theCOEService)
        {
            PickListCache theTypeAheadCache = TypeAheadCacheGenerator.instance;
            theTypeAheadCache._theCOEService = theCOEService;
            return theTypeAheadCache;
        }

        /// <summary>
        /// Method to determine whether picklist values can be displayed based on base table row count.
        /// </summary>
        /// <param name="dataviewId">dataview id</param>
        /// <param name="tableId">table id</param>
        /// <returns>returns true if the base table has records less than 5000; ootherwise false</returns>
        public bool CanSetPickListValues(int dataviewId, int tableId)
        {
            bool result;
            //check if collection has values present for selected dataview
            if (pickListValueSettingCollection.TryGetValue(dataviewId, out result))
            {
                return result;
            }
            else
            {
                //check if the the current table is base table in the dataview
                if (IsBaseTable(tableId))
                {
                    //get the fast row count from framework
                    int baseTableRowCount = _theCOEService.GetFastRowCount();
                    if (baseTableRowCount > 5000)
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                    //add to collection for next time use
                    pickListValueSettingCollection.Add(dataviewId, result);
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the type ahead binding source for specified dataview, table and field
        /// </summary>
        /// <param name="dataviewId">id of dataview in use</param>
        /// <param name="tableId">table id</param>
        /// <param name="fieldId">current field id</param>
        /// <param name="isLookupField">boolean to check if the field is of type lookup field</param>
        /// <returns>returns the auto complete string collection from the cache if present; otherwise calls the framework method to get the values and stores in the cache for next time use</returns>
        public AutoCompleteStringCollection GetTypeAheadBindingSource(int dataviewId, int tableId, int fieldId, bool isLookupField)
        {
            //get the cache model object from cache
            PickListCacheModel theTypeAheadValueCacheModel = this[dataviewId, tableId, fieldId];
            string[] values = null;
            if (theTypeAheadValueCacheModel == null)
            {
                theTypeAheadValueCacheModel = new PickListCacheModel(dataviewId, tableId, fieldId);
                //check if the table is base table or field is a lookup field, then only load the picklist values, do not load pick list values for child tables
                if (IsBaseTable(tableId) || isLookupField)
                {
                    try
                    {
                        values = GenerateTypeAheadValues(dataviewId, tableId, fieldId, isLookupField);
                    }
                    catch (PickListCacheException pickListEx)
                    {
                        //add to collection to avoid execution of GetData method again when there is exception
                        theTypeAheadValueCacheModel.ToolTip = pickListEx.Message;
                        _typeAheadValueCacheModels.Add(theTypeAheadValueCacheModel);
                        throw;
                    }
                    catch
                    {
                        //just throw other exceptions and default tooltip will be displayed
                        throw;
                    }
                }
                theTypeAheadValueCacheModel.AddTypeAheadStringCollection(values);
                _typeAheadValueCacheModels.Add(theTypeAheadValueCacheModel);
            }
            //if cache contains tooltip value that means there was exception occured previously
            if (!string.IsNullOrEmpty(theTypeAheadValueCacheModel.ToolTip))
            {
                throw new PickListCacheException(theTypeAheadValueCacheModel.ToolTip);
            }
            return theTypeAheadValueCacheModel.TypeAheadStringCollection;
        }

        /// <summary>
        /// Checks if the specified table is base table in the dataview
        /// </summary>
        /// <param name="tableId">table id to test</param>
        /// <returns>returns true if the table is base table; otherwise false</returns>
        bool IsBaseTable(int tableId)
        {
            bool result = false;
            if (_theCOEService.TheCOEDataviewBO.COEDataView.Basetable == tableId)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Method to generate the array of the type ahead values using a result criteria
        /// </summary>
        /// <param name="dataviewId">dataview id</param>
        /// <param name="tableId">table id</param>
        /// <param name="fieldId">field id</param>
        /// <returns>returns array of the pick list values for the specified field</returns>
        string[] GenerateTypeAheadValues(int dataviewId, int tableId, int fieldId, bool isLookupField)
        {
            string[] values = null;
            System.Data.DataSet dsLookup = null;
            try
            {
                using (dsLookup = GetData(dataviewId, tableId, fieldId, isLookupField))
                {
                    //Bind the field values to autocomplete source
                    if (dsLookup != null && dsLookup.Tables.Count > 0)//Creating AutoSuggest list for text type lookup fields
                    {
                        //use LINQ method syntax to pull the first field from a DT into a string array...
                        int lookupTableIndex = 0;//If there is there is a lookup in child table then dataset contains parent data at table index 0 and child data with lookup values at table index 1
                        var stringArr = dsLookup.Tables[lookupTableIndex].Rows.Cast<System.Data.DataRow>().Select(row => row[0].ToString());
                        values = stringArr.ToArray();
                    }
                }
            }
            catch
            {
                throw;
            }
            return values;
        }

        DataSet GetData(int dataviewId, int tableId, int fieldId, bool isLookupField)
        {
            DataSet dataSet = null;
            COEDataView tempDataview = null;
            ResultsCriteria rcLookup = null;
            try
            {
                if (isLookupField)
                {
                    //prepare temporary dataview and set lookup table as base table in the DV
                    tempDataview = new COEDataView();
                    tempDataview.Basetable = tableId;
                    tempDataview.DataViewHandling = COEDataView.DataViewHandlingOptions.USE_CLIENT_DATAVIEW;
                    tempDataview.Database = _theCOEService.TheCOEDataviewBO.COEDataView.Database;
                    tempDataview.Name = "TEMP_DV";
                    tempDataview.Description = "DV for Lookup";
                    tempDataview.Application = _theCOEService.TheCOEDataviewBO.COEDataView.Application;
                    COEDataView.DataViewTable theDVTable = _theCOEService.TheCOEDataviewBO.COEDataView.Tables.getById(tableId);
                    tempDataview.Tables.Add(theDVTable);
                }

                rcLookup = new ResultsCriteria();
                ResultsCriteria.Field resultf = new ResultsCriteria.Field();
                resultf.Id = fieldId;
                ResultsCriteria.ResultsCriteriaTable rcTable = new ResultsCriteria.ResultsCriteriaTable(tableId);
                rcTable.Criterias.Add(resultf);
                rcLookup.Tables.Add(rcTable);

                if (isLookupField)
                {
                    dataSet = _theCOEService.GetDataSet(null, rcLookup, tempDataview, false);
                }
                else
                {
                    dataSet = _theCOEService.GetDataSet(null, rcLookup, dataviewId, false);
                }
            }
            catch (Csla.DataPortalException dataPortalEx)
            {
                switch (dataPortalEx.BusinessException.Message)
                {
                    //for lookup field if the base table missing the primary key
                    case "The base table primary key has not been set. This is required for searching. Please review your dataview.":
                        throw new PickListCacheException("Picklist values not available because lookup table lacks primary key");
                    //if the schema permission not available to user
                    case "ORA-00942: table or view does not exist":
                        throw new PickListCacheException("Picklist values not available because user lacks schema permissions");
                    //just throw other exceptions so that user when try next time, will get the data
                    default:
                        throw dataPortalEx;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                rcLookup = null;
                tempDataview = null;
            }
            return dataSet;
        }

        /// <summary>
        /// Indexer to get the cahce model object from cache
        /// </summary>
        /// <param name="dataviewId">dataview id</param>
        /// <param name="tableId">table id</param>
        /// <param name="fieldId">field id</param>
        /// <returns></returns>
        PickListCacheModel this[int dataviewId, int tableId, int fieldId]
        {
            get
            {
                var theTypeAheadValueCacheModel = _typeAheadValueCacheModels.FirstOrDefault(p => p.DataviewId == dataviewId && p.TableId == tableId && p.FieldId == fieldId);
                return theTypeAheadValueCacheModel;
            }
        }

        /// <summary>
        /// Nested class to implement singleton pattern to hold the values for caching
        /// </summary>
        private class TypeAheadCacheGenerator
        {
            internal static readonly PickListCache instance = new PickListCache();

            static TypeAheadCacheGenerator()
            {
            }
        }

        /// <summary>
        /// Model class to hold the picklist values in cache
        /// </summary>
        class PickListCacheModel
        {
            #region Variables
            int _dataviewId;
            int _tableId;
            int _fieldId;
            AutoCompleteStringCollection _typeAheadStringCollection;
            string _toolTip;

            #endregion

            #region Properties
            /// <summary>
            /// Gets the dataview id
            /// </summary>
            public int DataviewId
            {
                get { return _dataviewId; }
            }

            /// <summary>
            /// Gets the table id
            /// </summary>
            public int TableId
            {
                get { return _tableId; }
            }

            /// <summary>
            /// Gets the field id
            /// </summary>
            public int FieldId
            {
                get { return _fieldId; }
            }

            /// <summary>
            /// Gets the string collection source
            /// </summary>
            public AutoCompleteStringCollection TypeAheadStringCollection
            {
                get { return _typeAheadStringCollection; }
            }

            /// <summary>
            /// Gets or sets the tooltip string to display in text box control
            /// </summary>
            public string ToolTip
            {
                get { return _toolTip; }
                set { _toolTip = value; }
            }
            #endregion

            /// <summary>
            /// Initializes am instance of picklist model with specified dataview, table and field id
            /// </summary>
            /// <param name="dataviewId"></param>
            /// <param name="tableId"></param>
            /// <param name="fieldId"></param>
            public PickListCacheModel(int dataviewId, int tableId, int fieldId)
            {
                this._toolTip = string.Empty;
                this._dataviewId = dataviewId;
                this._tableId = tableId;
                this._fieldId = fieldId;
                _typeAheadStringCollection = new AutoCompleteStringCollection();
            }

            /// <summary>
            /// Method to add the collection of picklist values to auto complete source
            /// </summary>
            /// <param name="stringCollection">array of picklist values</param>
            public void AddTypeAheadStringCollection(string[] stringCollection)
            {
                if (stringCollection != null)
                    _typeAheadStringCollection.AddRange(stringCollection);
            }
        }


        /// <summary>
        /// Exception class to set the tooltip message as exception message
        /// </summary>
        public class PickListCacheException : Exception
        {
            public PickListCacheException()
                : base()
            {

            }

            public PickListCacheException(string message)
                : base(message)
            {

            }

            public PickListCacheException(string message, Exception innerException)
                : base(message, innerException)
            {

            }

            public PickListCacheException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
                : base(info, context)
            {

            }
        }
    }
}
