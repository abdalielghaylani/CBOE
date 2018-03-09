using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using CambridgeSoft.COE.Framework.COEDataViewService;
using Spotfire.Dxp.Application.Extension;
using Spotfire.Dxp.Data;
using Spotfire.Dxp.Data.Import;
using Spotfire.Dxp.Framework.Persistence;
using SpotfireIntegration.Common;
using CambridgeSoft.COE.Framework.Common;
using SpotfireIntegration.SpotfireAddin.Properties;
using Spotfire.Dxp.Data.Exceptions;
using COEServiceLib;
using Spotfire.Dxp.Data.Formatters;

namespace SpotfireIntegration.SpotfireAddin
{
    [Serializable]
    [PersistenceVersion(1, 0)]
    class COEMetadataTransformation : CustomDataTransformation
    {
        private COEHitList hitList;
        private int tableID;
        //added to maintain the search criteria fields order in datatable properties
        private SearchCriteriaFieldOrder searchFieldsOrder;
        private bool filterChildHits;

        public COEMetadataTransformation()
            : this(null, 0, false) { }

        public COEMetadataTransformation(COEHitList hitList)
            : this(hitList, 0, false) { }

        public COEMetadataTransformation(COEHitList hitList, int tableID, bool filterChildHits)
            : this(hitList, tableID, null, filterChildHits) { }

        public COEMetadataTransformation(COEHitList hitList, int tableID, SearchCriteriaFieldOrder searchFieldsOrder, bool filterChildHits)
            : base()
        {
            this.hitList = hitList;
            this.tableID = tableID;
            //assign the SearchCriteriaFieldOrder object
            if (searchFieldsOrder != null)
            {
                this.searchFieldsOrder = searchFieldsOrder;
            }
            else
            {
                this.searchFieldsOrder = new SearchCriteriaFieldOrder();
            }
            this.filterChildHits = filterChildHits;
        }

        public COEMetadataTransformation(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.hitList = info.GetValue("hitList", typeof(COEHitList)) as COEHitList;
            this.tableID = info.GetInt32("tableID");
            //get search criteria field order from serialization info
            this.searchFieldsOrder = info.GetValue("searchFieldsOrder", typeof(SearchCriteriaFieldOrder)) as SearchCriteriaFieldOrder;
            this.filterChildHits = info.GetBoolean("filterChildHits");
        }

        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("hitList", this.hitList);
            info.AddValue("tableID", this.tableID);
            //add search criteria field order to serialization info
            info.AddValue("searchFieldsOrder", this.searchFieldsOrder);
            info.AddValue("filterChildHits", this.filterChildHits);
        }

        protected override DataTransformationConnection ConnectCore(ImportContext importContext, DataRowReader input)
        {
            if (this.hitList == null)
            {
                throw new ImportException("The ChemOffice Enterprise metadata transformation requires a COEHitList");
            }
            return DataTransformationConnection.CreateConnection(
                delegate
                {
                    return new COEMetadataTransformationReader(importContext, input, this.hitList, this.tableID, this.searchFieldsOrder, this.filterChildHits);
                });
        }
    }

    internal class COEMetadataTransformationReader : CustomDataRowReader
    {
        private DataRowReader inputReader;
        private List<DataRowReaderColumn> columns;
        private ResultProperties resultProperties;

        public COEMetadataTransformationReader(ImportContext importContext, DataRowReader inputReader, COEHitList hitList, int tableID, SearchCriteriaFieldOrder searchFieldsOrder, bool filterChildHits)
        {
            this.inputReader = inputReader;

            // Get the DataView used for this hitlist.
            COEService service = importContext.GetService<COEService>();
            COEDataViewBO dataViewBO = service.LoadDataViewBO(hitList);
            COEDataView dataView = dataViewBO.COEDataView;

            // Find the DataViewTable using the passed-in tableID.
            if (tableID == 0)
            {
                tableID = dataView.Basetable;
            }
            COEDataView.DataViewTable dataViewTable = dataView.Tables.getById(tableID);
            ResultsCriteria.ResultsCriteriaTable rcTable = hitList.ResultsCriteria.Tables.Find(t => t.Id == tableID);

            // Create the list of columns to be returned by this reader.
            this.columns = new List<DataRowReaderColumn>();
            foreach (DataRowReaderColumn col in inputReader.Columns)
            {
                // Copy any existing column properties.
                DataColumnProperties properties = col.Properties.Propagate(importContext);
                if (rcTable != null)
                {
                    int columnID = 0;
                    COEDataView.IndexTypes indexType = COEDataView.IndexTypes.UNKNOWN;
                    COEDataView.MimeTypes mimeType = COEDataView.MimeTypes.UNKNOWN;

                    // Find the Criteria corresponding to the DataRowReaderColumn.
                    ResultsCriteria.IResultsCriteriaBase criteria = rcTable.Criterias.Find(c => c.Alias == col.Name);

                    // Find the COEDataView.Field corresponding to the DataRowReaderColumn.
                    COEDataView.Field dataViewField;
                    if (criteria is ResultsCriteria.Field)
                    {
                        columnID = ((ResultsCriteria.Field)criteria).Id;
                        dataViewField = dataViewTable.Fields.Find(f => f.Id == columnID);
                    }
                    else
                    {
                        dataViewField = dataViewTable.Fields.Find(f => f.Alias == col.Name);
                    }

                    if (dataViewField != null)
                    {
                        columnID = dataViewField.Id;
                        if (dataViewField.LookupDisplayFieldId > 0) // CSBR - 151951
                        {
                            COEDataView.Field lookupDisplayField = dataView.GetFieldById(dataViewField.LookupDisplayFieldId);
                            if (lookupDisplayField != null)
                            {
                                indexType = lookupDisplayField.IndexType;
                                mimeType = lookupDisplayField.MimeType;
                            }
                        }
                        else
                        {
                            indexType = dataViewField.IndexType;
                            mimeType = dataViewField.MimeType;
                        }
                    }

                    //if the column datatype is Currency then set the decimal places to Auto
                    if (col.DataType == DataType.Currency)
                    {
                        IDataFormatter dataFormatter = col.DataType.CreateLocalizedFormatter();
                        ((NumberFormatter)dataFormatter).DecimalDigitsMode = DecimalDigitsMode.Auto;
                        properties.SetProperty(DataColumnProperties.DefaultProperties.Formatter, dataFormatter);
                    }

                    // Set the DataColumn properties.
                    if (columnID != 0)
                    {
                        properties.SetProperty(Resources.COEColumnID_PropertyName, columnID);
                    }

                    // Set the content type for structure fields.
                    if (indexType == COEDataView.IndexTypes.CS_CARTRIDGE)
                    {
                        properties.SetProperty(DataColumnProperties.DefaultProperties.ContentType, GetMimeTypeFromDVMimeType(mimeType));
                    }
                    else
                    {
                        properties.SetProperty(DataColumnProperties.DefaultProperties.ContentType, GetImageMimeTypeFromDVMimeType(mimeType));
                    }
                }

                this.columns.Add(
                    new DataRowReaderColumn(
                        col.Name,
                        col.DataType,
                        properties,
                        col.Cursor));
            }

            // Copy and update the DataTable properties.
            resultProperties = inputReader.ResultProperties.Propagate(importContext);

            resultProperties.SetProperty(Resources.COEResultsCriteria_PropertyName, hitList.ResultsCriteria.ToString());
            resultProperties.SetProperty(Resources.COEDataView_PropertyName, dataView.ToString());
            resultProperties.SetProperty(Resources.COEDataViewID_PropertyName, hitList.DataViewID);
            resultProperties.SetProperty(Resources.COEHitListID_PropertyName, hitList.HitListID);
            resultProperties.SetProperty(Resources.COEHitListType_PropertyName, (int)hitList.HitListType);
            if (dataViewTable != null)
            {
                resultProperties.SetProperty(Resources.COETableID_PropertyName, dataViewTable.Id);
                resultProperties.SetProperty(Resources.COETablePK_PropertyName, dataViewTable.PrimaryKey);
            }
            //add search criteria property xml value
            if (hitList.SearchCriteria == null)
            {
                hitList.SearchCriteria = new SearchCriteria();
            }
            resultProperties.SetProperty(Resources.COESearchCriteria_PropertyName, hitList.SearchCriteria.ToString());

            //set search criteria field order value to datatable property
            if (searchFieldsOrder == null)
            {
                searchFieldsOrder = new SearchCriteriaFieldOrder();
            }
            resultProperties.SetProperty(Resources.COESearchCriteriaFieldOrder_PropertyName, searchFieldsOrder.ToString());
            resultProperties.SetProperty(Resources.FilterChildHits_PropertyName, filterChildHits.ToString());
        }

        protected override IEnumerable<DataRowReaderColumn> GetColumnsCore()
        {
            return this.columns;
        }

        protected override ResultProperties GetResultPropertiesCore()
        {
            return this.resultProperties;
        }

        protected override bool MoveNextCore()
        {
            return this.inputReader.MoveNext();
        }

        protected override void ResetCore()
        {            
            this.inputReader.Reset();
        }

        /// <summary>
        /// Translates COEDataView Mimetype enumeration values to valid chemical mimetype string
        /// </summary>
        /// <param name="DVMimeType">The COEDataView.MimeTypes enum value to be translated</param>
        /// <returns>A supported chemical mimetype string or blank</returns>
        private static string GetMimeTypeFromDVMimeType(COEDataView.MimeTypes DVMimeType)
        {
            switch (DVMimeType)
            {
                case COEDataView.MimeTypes.CHEMICAL_X_CDX:
                case COEDataView.MimeTypes.UNKNOWN:  //defaults unknown to cdx
                case COEDataView.MimeTypes.NONE:
                    return "chemical/x-cdx";
                case COEDataView.MimeTypes.CHEMICAL_X_MDLMOLFILE:
                    return "chemical/x-mdl-molfile";
                case COEDataView.MimeTypes.CHEMICAL_X_SMILES:
                    return "chemical/x-daylight-smiles";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Translates COEDataView Mimetype enumeration values to valid image mimetype string
        /// </summary>
        /// <param name="DVMimeType">The COEDataView.MimeTypes enum value to be translated</param>
        /// <returns>A supported image mimetype string or blank</returns>
        private static string GetImageMimeTypeFromDVMimeType(COEDataView.MimeTypes DVMimeType)
        {
            switch (DVMimeType)
            {
                case COEDataView.MimeTypes.IMAGE_GIF:
                    return "image/gif";
                case COEDataView.MimeTypes.IMAGE_JPEG:
                    return "image/jpg";
                case COEDataView.MimeTypes.IMAGE_PNG:
                    return "image/png";
                case COEDataView.MimeTypes.IMAGE_X_WMF:
                    return "image/wmf";
                default:
                    return string.Empty;
            }
        }

    }

}
