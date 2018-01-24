using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COELoggingService;


namespace CambridgeSoft.COE.Framework.COEExportService
{
    public class FormatterBase 
    {
        [NonSerialized]
         static COELog _coeLog = COELog.GetSingleton("COEExport");
        /// <summary>
        /// Method called by export servic for modifying results criteria. This is the default that finds structure fields.  this method
        /// can be overriden in the formatter
        /// </summary>
        /// <param name="resultsCriteria">Results criteria to modify</param>
        /// <param name="dataView">dataview to modify</param>
        /// <returns>modifed results criteria</returns>
        internal virtual ResultsCriteria ModifyResultsCriteria(ResultsCriteria resultsCriteria, COEDataView dataView)
        {
            int fieldID = -1;
            int baseTableID = -1;
            bool bDoNotExportBase64 = true;     // added by JD
            bool rawBase64Found = false;
            //Remove Grand Child
            resultsCriteria = resultsCriteria.RemoveGrandChild(dataView, resultsCriteria);
            Common.SqlGenerator.MetaData.DataView _dataView = new CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView();
            _dataView.LoadFromXML(dataView.ToString());
            baseTableID = _dataView.GetBaseTableId();

            ResultsCriteria.ResultsCriteriaTable baseTableRC = resultsCriteria[baseTableID];
            ResultsCriteria.HighlightedStructure hlStructure = null;

            if (baseTableRC != null)
            {
                for (int j = 0; j < baseTableRC.Criterias.Count; j++)
                {
                    if (baseTableRC.Criterias[j] is ResultsCriteria.Field)
                    {
                        fieldID = ((ResultsCriteria.Field)baseTableRC.Criterias[j]).Id;
                        if (fieldID != -1)
                        {
                            //if is first structure field then modify the resultsCriteria else break loop.
                            //here is add the first structure field from convert the base64 string structure field
                            if (IsStructureIndexType(baseTableID, fieldID, dataView))
                            {
                                // remove the raw structure data from the export .. JD 6/12/09
                                if (bDoNotExportBase64)
                                    baseTableRC.Criterias.RemoveAt(j);

                                Modify(baseTableRC, fieldID);
                                rawBase64Found = true;
                                break;
                            }
                        }
                    }
                    else if (baseTableRC.Criterias[j] is ResultsCriteria.HighlightedStructure)
                        hlStructure = baseTableRC.Criterias[j] as ResultsCriteria.HighlightedStructure;
                }
                
                if (!rawBase64Found && hlStructure != null)
                {
                    if (bDoNotExportBase64)
                        baseTableRC.Criterias.Remove(hlStructure);

                    Modify(baseTableRC, hlStructure.Id);
                }
            }

            
            return resultsCriteria;

        }
        /// <summary>
        /// helper method for finding a tableid/fieldid in a dataview to determine if it is a structure field.
        /// </summary>
        /// <param name="tableID">table id from results criteria to look up in dataview</param>
        /// <param name="fieldID">field id from results criteria to look up in dataview</param>
        /// <param name="dataView">dataview being examined</param>
        /// <returns>false if not a structure field, true if it is.</returns>
        protected virtual  bool IsStructureIndexType(int tableID, int fieldID, COEDataView dataView)
        {
            bool isStruc = false;
            for (int i = 0; i < dataView.Tables.Count; i++)
            {
                if (dataView.Tables[i].Id == tableID)
                {
                    for (int j = 0; j < dataView.Tables[i].Fields.Count; j++)
                    {
                        if ((dataView.Tables[i].Fields[j].Id == fieldID))
                        {
                            COEDataView.Field fld = null;
                            if (dataView.Tables[i].Fields[j].LookupDisplayFieldId > 0)
                                fld = dataView.GetFieldById(dataView.Tables[i].Fields[j].LookupDisplayFieldId);
                            else
                                fld = dataView.Tables[i].Fields[j];

                            isStruc = COEDataView.IsStructureIndexType(fld.IndexType);
                            break;
                        }
                    }
                }
                if (isStruc == true)
                {
                    break;
                }
            }
            return isStruc;
        }
        /// <summary>
        /// This method by default simply returns the originating dataview. However, it can be overridden in the formatter class and used
        /// to modify results criteria
        /// </summary>
        /// <param name="resultsCriteria">Results Criteria to be used in GetData call</param>
        /// <param name="dataView">DataView used in GetDataCall</param>
        /// <returns>results criteria object</returns>
        protected virtual void Modify(ResultsCriteria.ResultsCriteriaTable resultsCriteriaTable, int fieldID)
        {
            //the default method does not do anything to the results criteria.
        }
        /// <summary>
        /// This method is factored from FormatDataSet methods; allows handling child tables without field aliases
        /// </summary>
        /// <param name="fieldElement">Field element from resultsCriteria XML</param>
        /// <param name="_dataView">DataView generated from COEDataView</param>
        /// <returns>field alias or name if any</returns>
        protected static string GetChildFieldName(XmlElement fieldElement, Common.SqlGenerator.MetaData.DataView _dataView)
        {
            string sFieldName = fieldElement.GetAttribute("alias");
            if (String.IsNullOrEmpty(sFieldName))
            {
                // if we have no alias, get the field name from the dataview
                string sFieldID = fieldElement.Attributes["fieldId"].Value;
                int nFieldID = 0;
                if (Int32.TryParse(sFieldID, out nFieldID) && nFieldID > 0)
                    sFieldName = _dataView.GetFieldName(nFieldID);
            }
            return sFieldName;
        }
    }
}
