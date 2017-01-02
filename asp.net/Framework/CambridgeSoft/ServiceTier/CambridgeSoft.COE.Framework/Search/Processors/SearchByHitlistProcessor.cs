using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;

namespace CambridgeSoft.COE.Framework.COESearchService.Processors
{
    public class SearchByHitlistProcessor : SearchProcessor
    {
        #region Variables
        private List<Relation> _sourceRelationList = new List<Relation>();
        #endregion

        #region Constructors
        public SearchByHitlistProcessor(CambridgeSoft.COE.Framework.Common.SearchCriteria.SearchCriteriaItem item) : base(item) { }
        #endregion

        #region Methods
        /// <summary>
        /// Template method Preprocess - Builds an internal list of relations composed of all the relations in the source dv from the source
        /// joining field to the basetable and then adds to that list a relation from the source primary key to the hitlist.ID and a relation
        /// from the target field to the source joining field.
        /// That relations are going to be added to the query in the process method.
        /// </summary>
        /// <param name="searchDAL"></param>
        /// <param name="dataview"></param>
        public override void PreProcess(DAL searchDAL, COEDataView dataview)
        {
            //Coverity Bug Fix :- CID : 11584  Jira Id :CBOE-194
            CambridgeSoft.COE.Framework.Common.SearchCriteria.HitlistCriteria hitlistCriteria = _searchCriteriaItem.Criterium == null ? null : _searchCriteriaItem.Criterium as CambridgeSoft.COE.Framework.Common.SearchCriteria.HitlistCriteria;
            //Do the fetching only if sourceFieldID is provided and disctinct than the target field.
            if (hitlistCriteria != null && hitlistCriteria.SourceFieldId > 0)
            {
                //Since the source field is provided, we need to fetch the source dataview to determine the joins to the target field.
                //Those include:
                //  - The join from basetable pk in source dv to the hitlist.ID table.
                //  - The join from the source field to the target field.
                //  - And as the source field may be in a child table, we also need to fetch the relations from that child table to the basetable in the source dv.

                //Fetch the source dataview from the hitlist
                COEDataViewBO dataviewBO = COEDataViewBO.Get(hitlistCriteria.HitlistType, int.Parse(hitlistCriteria.Value));
                //Build source dataview metadata
                DataView sourceDv = new DataView(dataviewBO.COEDataView);
                Field sourcePK = sourceDv.GetBaseTablePK();
                Field sourceJoiningField = sourceDv.GetField(hitlistCriteria.SourceFieldId);

                //If sourceFieldID = sourcePK then no need for joining thought the source dv, and should behave as if no sourcefieldid were provided
                if (sourceJoiningField.FieldId == sourcePK.FieldId)
                    AddNoSourceDVJoins(dataview, hitlistCriteria);
                else
                {
                    if (sourcePK.FieldId != hitlistCriteria.SourceFieldId)
                    {
                        _sourceRelationList = sourceDv.GetRelations(((Table)sourcePK.Table).TableId, ((Table)sourceJoiningField.Table).TableId);
                    }

                    //Add a relation from hitlist to source base table
                    Relation hitlistRelation = new Relation();
                    hitlistRelation.Parent = sourcePK;
                    hitlistRelation.Child = new Field("ID", System.Data.DbType.Int32);
                    Table hitlistTable = new Table(hitlistCriteria.HitlistType == HitListType.TEMP ? "COETEMPHITLIST" : "COESAVEDHITLIST");
                    hitlistTable.Database = "COEDB";
                    hitlistTable.Alias = "hitlist1";
                    hitlistRelation.Child.Table = hitlistTable;
                    _sourceRelationList.Add(hitlistRelation);

                    //Add a relation from joiningfield and destination field
                    //Do the adding only if sourceFieldID is provided
                    Relation joinRelation = new Relation();
                    COEDataView.Field targetField = dataview.GetFieldById(_searchCriteriaItem.FieldId);
                    joinRelation.Parent = new Field(targetField.Name, TypesConversor.GetType(targetField.DataType.ToString()));
                    COEDataView.DataViewTable targetTable = dataview.Tables.getById(targetField.ParentTableId);
                    Table parentTable = new Table(targetTable.Name);
                    parentTable.Alias = targetTable.Alias;
                    parentTable.Database = targetTable.Database;
                    joinRelation.Parent.Table = parentTable;
                    joinRelation.Child = sourceJoiningField;
                    _sourceRelationList.Add(joinRelation);
                    hitlistCriteria.SourceJoiningFieldStr = sourceJoiningField.GetFullyQualifiedNameString();
                }
            }
            else
            {
                AddNoSourceDVJoins(dataview, hitlistCriteria);
            }

        }

        /// <summary>
        /// Template Method Process - Adds the previuoly fetched relations to the query as joins.
        /// </summary>
        /// <param name="query"></param>
        public override void Process(Query query)
        {
            //Add missing joins to query, from the previoulsy fetched.
            CambridgeSoft.COE.Framework.Common.SearchCriteria.HitlistCriteria hitlistCriteria = _searchCriteriaItem.Criterium as CambridgeSoft.COE.Framework.Common.SearchCriteria.HitlistCriteria;
            foreach (Relation rel in _sourceRelationList)
            {
                query.AddJoinRelation(rel);
            }
        }

        /// <summary>
        /// Template Method PostProcess - Nothings needs to be cleaned up.
        /// </summary>
        /// <param name="searchDAL"></param>
        public override void PostProcess(DAL searchDAL) { }

        private void AddNoSourceDVJoins(COEDataView dataview, CambridgeSoft.COE.Framework.Common.SearchCriteria.HitlistCriteria hitlistCriteria)
        {
            //Since we don't have a field id, we assume the primary key can be join directly to the target field.
            //There's no need to get the dataview
            //Add a relation from hitlist to target field

            //Coverity Bug Fix :- CID : 11584  Jira Id :CBOE-194
            if (hitlistCriteria != null)
            {
                Relation hitlistRelation = new Relation();
                COEDataView.Field targetField = dataview.GetFieldById(_searchCriteriaItem.FieldId);
                hitlistRelation.Parent = new Field(targetField.Name, TypesConversor.GetType(targetField.DataType.ToString()));
                COEDataView.DataViewTable targetTable = dataview.Tables.getById(targetField.ParentTableId);
                Table parentTable = new Table(targetTable.Name);
                parentTable.Alias = targetTable.Alias;
                parentTable.Database = targetTable.Database;
                hitlistRelation.Parent.Table = parentTable;
                hitlistRelation.Child = new Field("ID", System.Data.DbType.Int32);
                Table hitlistTable = new Table(hitlistCriteria.HitlistType == HitListType.TEMP ? "COETEMPHITLIST" : "COESAVEDHITLIST");
                hitlistTable.Database = "COEDB";
                hitlistTable.Alias = "hitlist1";
                hitlistRelation.Child.Table = hitlistTable;
                _sourceRelationList.Add(hitlistRelation);
            }
        }
        #endregion
    }
}
