using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using CambridgeSoft.COE.Framework.COEDataViewService;

namespace CambridgeSoft.COE.Framework.Common
{
    public class DataviewCommand
    {
        /// <summary>
        /// adding table and relations to the existing dataview and save to database.
        /// It will also genereates Ids for tables and fields.
        /// </summary>
        /// <param name="dataviewID">existing dataview Id</param>
        /// <param name="tableXml">xmlDocument object of tableXml</param>
        /// <param name="relationshipXml">xmlDocument object of relationshipXml</param>
        public void AddTableToDataview(int dataviewID, XmlDocument tableXml, XmlDocument relationshipXml)
        {
            ////get the existing dataview
            //COEDataViewBO dv = COEDataViewBO.Get(dataviewID);
            //COEDataView coeDataView = dv.COEDataView;

            ////add table to dataview and save to database
            ////coeDataView.GetFromTableXML(tableXml);
            ////dv.COEDataView = coeDataView;
            ////dv.Save(); // after adding the table save dataview to the db to generate the table Ids and field Ids. 

            ////dv.DataViewManager.Tables.Add(TableBO.NewTable(new COEDataView.DataViewTable(tableXml.DocumentElement)));
            //COEDataView.DataViewTable dt = coeDataView.GetFromTableXML(tableXml);
            //TableBO tbl = TableBO.NewTable(dt);
            ////dv.DataViewManager.Tables.Add(tbl);//validating table data using dataview manager            
            //coeDataView.Tables.Add(dt);
            ////dv.COEDataView = coeDataView;
            ////dv.SaveFromDataViewManager();
            ////dv.DataViewManager.Save();

            ////add relationship to dataview and save to database
            //int parentId = 0;       // Parent table Id
            //int childId = 0;        // Child table Id
            //int parentKeyId = 0;    // Parent table field Id
            //int childKeyId = 0;     // Child table field Id

            ////child table attributes like child, tablename and tableId
            //if (relationshipXml.DocumentElement.Attributes["child"] == null)
            //{
            //    if (relationshipXml.DocumentElement.Attributes["tablename"] != null)
            //    {
            //        childId = coeDataView.GetTableIdByName(relationshipXml.DocumentElement.Attributes["tablename"].Value.ToString());
            //    }
            //    else if (relationshipXml.DocumentElement.Attributes["tableid"] != null)
            //    {
            //        int.TryParse(relationshipXml.DocumentElement.Attributes["tableid"].Value.ToString(), out childId);
            //        if (childId == 0)
            //            childId = coeDataView.GetTableIdByName(relationshipXml.DocumentElement.Attributes["tableid"].Value.ToString());
            //    }
            //}
            //else
            //{
            //    childId = int.Parse(relationshipXml.Attributes["child"].Value);
            //}

            ////parent table attributes like parent, parenttable and parenttableid
            //if (relationshipXml.DocumentElement.Attributes["parent"] == null)
            //{
            //    if (relationshipXml.DocumentElement.Attributes["parenttable"] != null)
            //    {
            //        parentId = coeDataView.GetTableIdByName(relationshipXml.DocumentElement.Attributes["parenttable"].Value.ToString());
            //    }
            //    else if (relationshipXml.DocumentElement.Attributes["parenttableid"] != null)
            //    {
            //        int.TryParse(relationshipXml.DocumentElement.Attributes["parenttableid"].Value.ToString(), out parentId);
            //        if (parentId == 0)
            //            parentId = coeDataView.GetTableIdByName(relationshipXml.DocumentElement.Attributes["parenttableid"].Value.ToString());
            //    }
            //}
            //else
            //{
            //    parentId = int.Parse(relationshipXml.Attributes["parent"].Value);
            //}

            ////child table id like childkey, fieldname and fieldId
            //if (childId > 0)
            //{
            //    if (relationshipXml.DocumentElement.Attributes["childkey"] == null)
            //    {
            //        if (relationshipXml.DocumentElement.Attributes["fieldname"] != null)
            //        {
            //            childKeyId = coeDataView.GetFieldIdByName(childId, relationshipXml.DocumentElement.Attributes["fieldname"].Value.ToString());
            //        }
            //        else if (relationshipXml.DocumentElement.Attributes["fieldid"] != null)
            //        {
            //            int.TryParse(relationshipXml.DocumentElement.Attributes["fieldid"].Value.ToString(), out childKeyId);
            //            if (childKeyId == 0)
            //                childKeyId = coeDataView.GetTableIdByName(relationshipXml.DocumentElement.Attributes["fieldid"].Value.ToString());
            //        }
            //    }
            //    else
            //    {
            //        childKeyId = int.Parse(relationshipXml.Attributes["childkey"].Value);
            //    }
            //}

            ////parent table field id like parentkey, parentfield and parentfieldid
            //if (parentId > 0)
            //{
            //    if (relationshipXml.DocumentElement.Attributes["parentkey"] == null)
            //    {
            //        if (relationshipXml.DocumentElement.Attributes["parentfield"] != null)
            //        {
            //            parentKeyId = coeDataView.GetFieldIdByName(parentId, relationshipXml.DocumentElement.Attributes["parentfield"].Value.ToString());
            //        }
            //        else if (relationshipXml.DocumentElement.Attributes["parentfieldid"] != null)
            //        {
            //            int.TryParse(relationshipXml.DocumentElement.Attributes["parentfieldid"].Value.ToString(), out parentKeyId);
            //            if (parentKeyId == 0)
            //                parentKeyId = coeDataView.GetTableIdByName(relationshipXml.DocumentElement.Attributes["parentfieldid"].Value.ToString());
            //        }
            //    }
            //    else
            //    {
            //        parentKeyId = int.Parse(relationshipXml.Attributes["parentkey"].Value);
            //    }
            //}
            ////coeDataView.GetFromRelationship(relationshipXml, parentId, childId, parentKeyId, childKeyId);
            ////dv.COEDataView = coeDataView;            
            ////dv.Save();
            //COEDataView.Relationship rs = coeDataView.GetFromRelationship(relationshipXml, parentId, childId, parentKeyId, childKeyId);
            //RelationshipBO rel = RelationshipBO.NewRelationship(rs);
            ////dv.DataViewManager.Relationships.Add(rel);
            //TableListBO TList = TableListBO.NewTableListBO();
            //TList.Add(tbl);
            //RelationshipListBO RList = RelationshipListBO.NewRelationShipListBO();
            //RList.Add(rel);
            //dv.DataViewManager.Merge(TList, RList);
            ////coeDataView.Relationships.Add(rs);
            ////dv.COEDataView = coeDataView;
            //foreach (TableBO tb in dv.DataViewManager.Tables)
            //{
            //    tb.FromMasterSchema = false;
            //    foreach (FieldBO fb in tb.Fields)
            //    {
            //        fb.FromMasterSchema = false;
            //    }
            //}
            //foreach (RelationshipBO rb in dv.DataViewManager.Relationships)
            //{
            //    rb.FromMasterSchema = false;
            //}            
            //dv = dv.SaveFromDataViewManager();
            ////dv.DataViewManager.Save();
        }

        public void AddTableToDataview(int dataviewID, COEDataView.DataViewTable tbl, COEDataView.Relationship rel)
        {
            ////get the existing dataview
            //COEDataViewBO dv = COEDataViewBO.Get(dataviewID);
            //COEDataView coeDataView = dv.COEDataView;

            ////add table to dataview and save to database
            //coeDataView.AddTable(tbl);

            ////add relationship to dataview and save to database
            //coeDataView.AddRelationship(rel);

            //dv.COEDataView = coeDataView;
            //dv.Save();

        }
    }
}
