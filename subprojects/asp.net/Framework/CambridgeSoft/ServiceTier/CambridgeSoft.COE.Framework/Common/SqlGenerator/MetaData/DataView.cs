using System;
using System.Xml;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.Graphs;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using CambridgeSoft.COE.Framework.Caching;
using CambridgeSoft.COE.Framework.COEConfigurationService;



namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData
{
    /// <summary>
    /// Representation of the database schema. Usually loaded from an xml.
    /// </summary>
    public class DataView : CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.INamesLookup, ICacheable
    {
        #region Variables
        /// <summary>
        /// Contains the database Relations set needed for joining the tables.
        /// </summary>
        private Graph databaseRelations;
        /// <summary>
        /// The source xml containing the database schema.
        /// </summary>
        private COEDataView coeDataView;

        [NonSerialized]
        private XmlNamespaceManager manager;
        private const string xmlNamespace = "COE";
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its members to its default values.
        /// </summary>
        public DataView()
        {
            this.databaseRelations = new Graph();
            this.coeDataView = new COEDataView();
        }

        public DataView(string dataViewXml)
            : this()
        {
            this.LoadFromXML(dataViewXml);
        }

        /// <summary>
        /// Builds a SQLGenerator.Metadata.Dataview from a COEDataview.
        /// Avoids reparsing dataview xml
        /// </summary>
        public DataView(COEDataView dataView)
        {
            this.coeDataView = dataView;
            this.databaseRelations = new Graph();
            this.BuildRelationsGraph(this.coeDataView);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the xml representation of this dataView into a string 
        /// </summary>
        /// <returns>the string containing the xml representation of the DataView</returns>
        public override string ToString()
        {
            return this.coeDataView.ToString();
        }
        /// <summary>
        /// Loads the  values of this class from an xml string.
        /// </summary>
        /// <param name="dataViewXMLString">The schema xml in a string format.</param>
        public void LoadFromXML(string dataViewXMLString)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(dataViewXMLString);
            this.LoadFromXML(xdoc);
        }

        /// <summary>
        /// Loads the  values of this class from a XMLDocument.
        /// </summary>
        /// <param name="dataViewXMLDocument">The schema xml in a XmlDocument format.</param>
        public void LoadFromXML(XmlDocument dataViewXMLDocument)
        {
            this.coeDataView.GetFromXML(dataViewXMLDocument);
            this.BuildRelationsGraph(this.coeDataView);
        }

        /// <summary>
        /// Gets the table name from its id.
        /// </summary>
        /// <param name="tableIndex">The id of the table in the xml.</param>
        /// <returns>The table name.</returns>
        public string GetTableName(int tableIndex)
        {

            string tableName = this.coeDataView.GetTableNameById(tableIndex);
            if (tableName == string.Empty)
            {
                throw new SQLGeneratorException(Resources.InvalidTableIndex + " " + tableIndex.ToString());
            }
            return tableName;
        }

        /// <summary>  
        /// Gets the table alias name from its id.
        /// </summary>
        /// <param name="tableIndex">The id of the table in the xml.</param>
        /// <returns>The table alias name.</returns>
        /// CBOE-1763 and CBOE-1764 -- Returns the Alias name of the Table.
        public string GetTableAliasName(int tableIndex)
        {

            string tableAliasName = this.coeDataView.GetTableAliasNameById(tableIndex);
            if (tableAliasName == string.Empty)
            {
                throw new SQLGeneratorException(Resources.InvalidTableIndex + " " + tableIndex.ToString());
            }
            return tableAliasName;
        }
        // END CBOE-1763 and CBOE-1764

        public int GetBaseTableId()
        {
            int baseTableId;
            baseTableId = this.coeDataView.Basetable;
            if (baseTableId != null)
                return baseTableId;

            return -1;
        }

        public Field GetBaseTablePK()
        {
            int fieldId = 0;

            fieldId = int.Parse(this.coeDataView.BaseTablePrimaryKey);
            return GetField(fieldId);
        }

        /// <summary>
        /// Gets the table from its id..
        /// </summary>
        /// <param name="tableId">The id of the table in the xml.</param>
        /// <returns>A Table instance.</returns>
        public Table GetTable(int tableId)
        {
            Table table = new Table();
            COEDataView.DataViewTable dvtbl = this.coeDataView.Tables.getById(tableId);
            if (dvtbl != null)
            {
                table.TableName = dvtbl.Name;
                table.Alias = dvtbl.Alias;
                table.Database = GetDatabaseOwnerByAlias(dvtbl.Database);
                table.TableId = dvtbl.Id;
                return table;
            }
            else
                throw new SQLGeneratorException(Resources.InvalidTableIndex + " " + tableId.ToString());
        }


        /// <summary>
        /// Gets the field name from its id.
        /// </summary>
        /// <param name="fieldId">The field id in the xml.</param>
        /// <returns>The field name.</returns>
        public string GetFieldName(int fieldId)
        {
            COEDataView.Field fld = this.coeDataView.GetFieldById(fieldId);
            if (fld != null)
            {
                return fld.Name;
            }
            else
            {
                throw new SQLGeneratorException(Resources.InvalidFieldIndex + " " + fieldId.ToString());
            }
        }

        /// <summary>
        /// Returns the FieldType of the field whose id equals fieldIndex
        /// </summary>
        /// <param name="fieldId">The field id in the xml.</param>
        /// <returns>The DbType</returns>
        public DbType GetFieldType(int fieldId)
        {

            COEDataView.Field fld = this.coeDataView.GetFieldById(fieldId);
            if (fld != null)
            {
                return TypesConversor.GetType(fld.DataType.ToString());
            }
            else
            {
                throw new SQLGeneratorException(Resources.InvalidFieldIndex + " " + fieldId.ToString());
            }
        }

        /// <summary>
        /// Returns the name of the parent table of the Field whose id is fieldIndex.
        /// </summary>
        /// <param name="fieldId">The field id in the xml.</param>
        /// <returns>The table name of its parent table.</returns>
        public string GetParentTableName(int fieldId)
        {
            return this.GetTableName(this.GetParentTableId(fieldId.ToString()));
        }

        private void BuildRelationsGraph(COEDataView dataView)
        {
            foreach (COEDataView.DataViewTable currentTable in dataView.Tables)
            {
                databaseRelations.AddTable(currentTable.Id);
            }

            foreach (COEDataView.Relationship currentDVRelation in dataView.Relationships)
            {
                Relation currentRelation = new Relation();
                currentRelation.Parent = this.GetField(currentDVRelation.ParentKey, currentDVRelation.Parent);
                currentRelation.Child = this.GetField(currentDVRelation.ChildKey, currentDVRelation.Child);

                if (currentDVRelation.JoinType == COEDataView.JoinTypes.OUTER)
                {
                    currentRelation.InnerJoin = false;
                }
                else
                {
                    currentRelation.InnerJoin = true;
                }
                databaseRelations.AddRelation(currentRelation);
            }
        }

        /// <summary>
        /// Get database owner(schema) name by alias name. Alias is the name used in config file.
        /// </summary>
        /// <param name="alias"> database alias name </param>
        /// <returns>database schema name </returns>
        private static string GetDatabaseOwnerByAlias(string alias)
        {
            return string.IsNullOrEmpty(alias) ? string.Empty : ConfigurationUtilities.GetDatabaseData(alias).Owner;
        }


        /// <summary>
        /// Gets all the relation needed to access a parent table from a child. It is the shortest path between them.
        /// </summary>
        /// <param name="parentTableIndex">The parent table id in the xml.</param>
        /// <param name="childTableIndex">the child table id in the xml.</param>
        /// <returns>A List&lt;Relation&gt; with the relationships between two tables.</returns>
        public List<Relation> GetRelations(int parentTableIndex, int childTableIndex, bool restrictParentChildDirection=false)
        {
            List<Relation> result = databaseRelations.GetPath(parentTableIndex, childTableIndex);

            foreach (Relation currentRelation in result)
            {
                if (restrictParentChildDirection)
                { 
                    var relation = this.coeDataView.Relationships.FirstOrDefault(r => r.ParentKey == currentRelation.Parent.FieldId && r.ChildKey == currentRelation.Child.FieldId);

                    if (relation != null)
                    {
                        currentRelation.Parent = this.GetField(currentRelation.Parent.FieldId, relation.Parent);
                        currentRelation.Child = this.GetField(currentRelation.Child.FieldId, relation.Child);
                    }
                    else
                    {
                        throw new SQLGeneratorException(string.Format("Invalid relationship in dataview.  ParentKey {0} and ChildKey {1} not found in dataview.", currentRelation.Parent.FieldId, currentRelation.Child.FieldId));
                    }
                }
                else
                { 
                     currentRelation.Parent = this.GetField(currentRelation.Parent.FieldId);
                     currentRelation.Child = this.GetField(currentRelation.Child.FieldId); 
                }

                if ((Table)currentRelation.Child.Table == this.GetTable(this.GetBaseTableId()))
                    currentRelation.LeftJoin = true;
            }

            return result;
        }

        /// <summary>
        /// Gets the id of the table containing a field.
        /// </summary>
        /// <param name="fieldId">The field id in the xml.</param>
        /// <returns>The table id in the xml.</returns>
        public int GetParentTableId(string fieldId)
        {

            COEDataView.Field fld = this.coeDataView.GetFieldById(int.Parse(fieldId));
            if (fld != null)
            {
                return fld.ParentTableId;
            }
            else
            {
                throw new SQLGeneratorException(Resources.InvalidFieldIndex + " " + fieldId.ToString());
            }
        }
        /// <summary>
        /// Returns a SQLGenarator.MetaData Field object from the dataView, given the field id.
        /// </summary>
        /// <param name="fieldId">The field identifier.</param>
        /// <returns>The Field.</returns>
        public Field GetField(int fieldId)
        {
            Field resultField = new Field();
            resultField.FieldId = fieldId;

            COEDataView.Field fld = this.coeDataView.GetFieldById(fieldId);
            if (fld != null)
            {
                int parentTblId = fld.ParentTableId;
                if (parentTblId == -1) throw new SQLGeneratorException("ParentTableId not found for field: " + fld.Name + " Id:" + fld.Id);
                COEDataView.DataViewTable tbl = this.coeDataView.Tables.getById(fld.ParentTableId);
                return GetField(fld, tbl);
            }
            else
            {
                throw new SQLGeneratorException(Resources.InvalidFieldIndex + " " + fieldId.ToString());
            }
            //return resultField;
        }

        /// <summary>
        /// Returns a  SQLGenerator Field object from a COEDataView Field id and itss parentTable id.
        /// </summary> 
        /// <param name="fieldId">The COEDataView field id</param>
        /// <param name="parentTableId">The id of the parent table of the COEDataView field</param>
        /// <returns>The SQLGenerator.MetaDatadata Field.</returns>
        public Field GetField(int fieldId, int parentTableId)
        {
            COEDataView.DataViewTable parentTbl = this.coeDataView.Tables.getById(parentTableId);
            if (parentTbl != null)
            {
                COEDataView.Field fld = parentTbl.Fields.getById(fieldId);
                if (fld == null) throw new SQLGeneratorException(string.Format("Invalid relationship in dataview. Field {0} not found in table {1}", fieldId, parentTableId));
                return GetField(fld, parentTbl);
            }
            else
            {
                throw new SQLGeneratorException(string.Format("Invalid relationship in dataview.  Parent table {0} not found for relationship with ChildKey {1}", parentTableId, fieldId));
            }
        }

        /// <summary>
        /// Returns a  SQLGenerator Field object from a COEDataView Field and its parent table.
        /// </summary> 
        /// <param name="fld">The COEDataView field</param>
        /// <param name="parentTbl">The parent table of the COEDataView field</param>
        /// <returns>The SQLGenerator.MetaDatadata Field.</returns>
        public static Field GetField(COEDataView.Field fld, COEDataView.DataViewTable parentTbl)
        {

            Field resultField = new Field();
            resultField.FieldId = fld.Id;
            resultField.FieldName = fld.Name;
            resultField.FieldType = TypesConversor.GetType(fld.DataType.ToString());
            resultField.MimeType = fld.MimeType;

            Table resultTable = new Table
                                    {
                                        TableId = parentTbl.Id,
                                        Alias = parentTbl.Alias,
                                        Database = GetDatabaseOwnerByAlias(parentTbl.Database),
                                        TableName = parentTbl.Name
                                    };

            resultField.Table = resultTable;
            return resultField;
        }


        /// <summary>
        /// Returns a Table object from the dataView, given the Identifier.
        /// </summary>
        /// <param name="fieldId">The field id in the xml.</param>
        /// <returns>An instance of its Parent Table.</returns>
        public Table GetParentTable(int fieldId)
        {

            Table resultTable = new Table();

            COEDataView.Field fld = this.coeDataView.GetFieldById(fieldId);
            if (fld != null)
            {
                if (fld.ParentTableId != null)
                {
                    resultTable.TableId = fld.ParentTableId;
                    COEDataView.DataViewTable tbl = this.coeDataView.Tables.getById(fld.ParentTableId);
                    resultTable.Alias = tbl.Alias;
                    resultTable.Database = GetDatabaseOwnerByAlias(tbl.Database);
                    resultTable.TableName = tbl.Name;
                }
                else
                {
                    throw new SQLGeneratorException(Resources.FieldWithoutParentTable);
                }
            }
            else
            {
                throw new SQLGeneratorException(Resources.InvalidFieldIndex + " " + fieldId.ToString());
            }

            return resultTable;
        }

        /// <summary>
        /// Returns a Column instance (Which can be a Lookup or a regular field) from the dataView, given the Identifier.
        /// </summary>
        /// <param name="fieldId">The field id in the xml.</param>
        /// <returns>An instance of the corresponding field.</returns>
        public IColumn GetColumn(int fieldId)
        {
            IColumn column = null;

            COEDataView.Field fld = this.coeDataView.GetFieldById(fieldId);
            /* CSBR-154349 CBV crashes instead of showing the error message while working from a Client
             * Changes done Jogi  
             * Comparing the fieldId with LookupFieldId and LookupDisplayFieldId if they are same raising an 
             * exception. Which will not crash the CBVN */
            if (fld != null) //CBOE-779
            {

                if ((fld.Id != fld.LookupFieldId) && (fld.Id != fld.LookupDisplayFieldId))
                {
                    /* End of CSBR-154349 */
                    if (fld.LookupFieldId != -1)
                    {
                        Lookup lookup = new Lookup();
                        lookup.FieldId = fieldId;
                        lookup.FieldName = fld.Name;
                        lookup.MimeType = fld.MimeType;
                        lookup.Table = this.GetParentTable(lookup.FieldId);
                        lookup.LookupField = this.GetColumn(fld.LookupFieldId);
                        lookup.LookupDisplayField = this.GetColumn(fld.LookupDisplayFieldId);
                        lookup.LookupTable = this.GetParentTable(lookup.LookupFieldId);
                        column = lookup;
                    }
                    else
                    {
                        Field resultField = new Field();
                        resultField.FieldId = fieldId;
                        resultField.FieldName = fld.Name;
                        resultField.MimeType = fld.MimeType;
                        resultField.FieldType = TypesConversor.GetType(fld.DataType.ToString());
                        resultField.Table = this.GetParentTable(fieldId);
                        column = resultField;
                    }
                }
                /* CSBR-154349 CBV crashes instead of showing the error message while working from a Client
                 * Changes done by Jogi  */
                else
                {
                    throw new Exception("FieldId " + fld.Id + ", Fieldname " + fld.Name + " cannot lookup to itself : Please fix your dataview. ");
                }

            }
            //else
            //{
            //    throw new SQLGeneratorException(Resources.InvalidFieldIndex + " " + fieldId.ToString());
            //}
            /* End of CSBR-154349 */
            return column;
        }

        /// <summary>
        /// Gets the alias of a column from its id.
        /// </summary>
        /// <param name="fieldId">The field id in  the xml.</param>
        /// <returns>The alias.</returns>
        public string GetColumnAlias(int fieldId)
        {
            COEDataView.Field fld = this.coeDataView.GetFieldById(fieldId);
            if (fld != null)
            {
                return fld.Alias;
            }
            return string.Empty;
        }
        #endregion

        #region ICacheable Members
        [NonSerialized]
        private COECacheDependency _cacheDependency;
        /// <summary>
        /// This object, when cached, is dependant on the dataviewBO cached item. So when the bo was modified this metadata gets removed from cache.
        /// </summary>
        public COECacheDependency CacheDependency
        {
            get
            {
                if (_cacheDependency == null)
                    _cacheDependency = new COECacheDependency(coeDataView.DataViewID.ToString(), typeof(CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBO));

                return _cacheDependency;
            }
            set
            {
                _cacheDependency = value;
            }
        }

        /// <summary>
        /// Method triggered when the object is removed from cache. Currently display information in the debug console, if in debug mode.
        /// </summary>
        /// <param name="key">The object id</param>
        /// <param name="value">The actual dataviewbo</param>
        /// <param name="reason">The reason why it was removed from cache</param>
        public void ItemRemovedFromCache(string key, object value, COECacheItemRemovedReason reason)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine("*****************************");
            System.Diagnostics.Debug.WriteLine("Item Removed from cache.");
            System.Diagnostics.Debug.WriteLine("Key: " + key);
            System.Diagnostics.Debug.WriteLine("Reason: " + reason.ToString());
            System.Diagnostics.Debug.WriteLine("Current Time: " + DateTime.Now);
            System.Diagnostics.Debug.WriteLine("*****************************");
#endif
        }

        #endregion
    }
}
