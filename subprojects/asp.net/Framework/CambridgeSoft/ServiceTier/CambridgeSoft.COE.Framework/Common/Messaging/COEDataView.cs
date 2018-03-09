using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common.Utility;

namespace CambridgeSoft.COE.Framework.Common
{
    /// COEDataView
    /// <summary>
    /// Write something here.
    /// </summary>
    [Serializable]
    [XmlRootAttribute("COEDataView", Namespace = "COE.COEDataView", DataType = "COEDataView")]
    [XmlTypeAttribute(TypeName = "COEDataView", IncludeInSchema = true, Namespace = "COE.COEDataView")]
    public class COEDataView
    {
        #region Properties
        /// <summary>
        /// List of tables that are mapped by this dataview.
        /// </summary>
        [XmlArray("tables")]
        [XmlArrayItem("table", Type = typeof(DataViewTable))]
        public DataViewTableList Tables
        {
            get { return tables; }
            set { tables = value; }
        }

        /// <summary>
        /// Array of relationships between the tables that contains the dataview. They contain the parent and child keys, as well as the fields that relate them.
        /// </summary>
        [XmlArray("relationships", IsNullable = false)]
        public List<Relationship> Relationships
        {
            get { return relationships; }
            set { relationships = value; }
        }

        /// <summary>
        /// Base table Id for the current DataView - Default value = -1
        /// </summary>
        [XmlAttribute("basetable")]
        public int Basetable
        {
            get { return baseTable; }
            set { baseTable = value; }
        }
        /// <summary>
        /// basetable name of current dataview
        /// </summary>
        [XmlIgnore()]
        public string BaseTableName
        {
            get { return GetTableNameById(this.baseTable); }
        }


        /// <summary>
        /// Database that the dataview refers to 
        /// </summary>
        [XmlAttribute("database")]
        public string Database
        {
            get { return database; }
            set { database = value; }
        }

        /// <summary>
        /// Base Table primary key Id (just for saving effort of looking it) - NOT mapped to the xml view of this object
        /// </summary>
        [XmlIgnore()]
        public string BaseTablePrimaryKey
        {
            get { return this.GetTablePrimaryKeyById(baseTable); }
        }

        /// <summary>
        /// XML Namespace of the dataview. NOT mapped to the xml view of this object.
        /// </summary>
        [XmlIgnore]
        public string XmlNs
        {
            get { return xmlNs; }
            set { xmlNs = value; }
        }

        /// <summary>
        /// Identifier of the dataview. Default value = -1
        /// </summary>
        [XmlAttribute("dataviewid")]
        public int DataViewID
        {
            get { return dataViewID; }
            set { dataViewID = value; }
        }

        /// <summary>
        /// The name of the dataview.
        /// </summary>
        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Dataview's description.
        /// </summary>
        [XmlAttribute("description")]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Application dataview is specific to
        /// </summary>
        [XmlAttribute("application")]
        public string Application
        {
            get { return application; }
            set { application = value; }
        }

        /// <summary>
        /// Defines the behaviour of dataviews being passed to search service.
        /// </summary>
        [XmlAttribute("dataviewHandling")]
        public DataViewHandlingOptions DataViewHandling
        {
            get { return dataViewHandling; }
            set { dataViewHandling = value; }
        }
        #endregion

        #region Variables
        private DataViewTableList tables;
        private List<Relationship> relationships;
        private int baseTable;
        private string database;

        private string xmlNs;
        [NonSerialized]
        private XmlNamespaceManager manager;
        private string xmlNamespace;
        private int dataViewID;
        private string name;
        private string description;
        private string application;
        private DataViewHandlingOptions dataViewHandling;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public COEDataView()
        {
            this.tables = new DataViewTableList();
            this.relationships = new List<Relationship>();
            this.dataViewID = -1;
            this.name = string.Empty;
            this.description = string.Empty;
            this.baseTable = -1;
            this.database = string.Empty;
            this.application = string.Empty;
            this.xmlNs = "COE.COEDataView";
            this.dataViewHandling = DataViewHandlingOptions.USE_CLIENT_DATAVIEW;
            xmlNamespace = "COE";
            this.InitializeManager(new NameTable());
        }

        /// <summary>
        /// Dataview Constructor that receives the xml view of the object.
        /// </summary>
        /// <param name="doc"></param>
        public COEDataView(XmlDocument doc)
        {
            this.xmlNs = "COE.COEDataView";
            xmlNamespace = "COE";
            this.InitializeManager(doc.NameTable);
            this.GetFromXML(doc);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Fills the DataView object from it's xml representation contained in an XmlDocument.
        /// </summary>
        /// <param name="xmlDataView">xml representation of this DataView object</param>
        public void GetFromXML(XmlDocument xmlDataView)
        {
            if (this.manager == null)
                this.InitializeManager(xmlDataView.NameTable);

            XmlNode tablesNodeList = xmlDataView.SelectSingleNode("//" + xmlNamespace + ":tables", this.manager);
            XmlNode relationshipsNodeList = xmlDataView.SelectSingleNode("//" + xmlNamespace + ":relationships", this.manager);

            this.tables = new DataViewTableList();
            // Coverity Fix CID - 13127
            if (tablesNodeList != null)
            {
                foreach (XmlNode tableNode in tablesNodeList.ChildNodes)
                {
                    if (tableNode.NodeType == XmlNodeType.Element)
                    {
                        DataViewTable tbl = new DataViewTable(tableNode);
                        this.tables.Add(tbl);
                    }
                }
            }

            this.relationships = new List<Relationship>();
            if (relationshipsNodeList != null)
            {
                foreach (XmlNode relationshipNode in relationshipsNodeList.ChildNodes)
                {
                    if (relationshipNode.NodeType == XmlNodeType.Element)
                    {
                        Relationship rel = new Relationship(relationshipNode);
                        relationships.Add(rel);
                    }
                }
            }

            XmlNode dataViewNode = xmlDataView.SelectSingleNode("//" + xmlNamespace + ":COEDataView", this.manager);
            if (dataViewNode == null)
                dataViewNode = xmlDataView.SelectSingleNode("//COEDataView");

            if (dataViewNode.Attributes["xmlns"] != null && !string.IsNullOrEmpty(dataViewNode.Attributes["xmlns"].Value))
                xmlNs = dataViewNode.Attributes["xmlns"].Value;
            if (dataViewNode.Attributes["dataviewid"] != null && !string.IsNullOrEmpty(dataViewNode.Attributes["dataviewid"].Value))
                dataViewID = int.Parse(dataViewNode.Attributes["dataviewid"].Value);
            if (dataViewNode.Attributes["name"] != null && !string.IsNullOrEmpty(dataViewNode.Attributes["name"].Value))
                name = dataViewNode.Attributes["name"].Value;
            if (dataViewNode.Attributes["description"] != null && !string.IsNullOrEmpty(dataViewNode.Attributes["description"].Value))
                description = dataViewNode.Attributes["description"].Value;
            if (dataViewNode.Attributes["basetable"] != null && !string.IsNullOrEmpty(dataViewNode.Attributes["basetable"].Value))
                baseTable = int.Parse(dataViewNode.Attributes["basetable"].Value);
            if (dataViewNode.Attributes["database"] != null && !string.IsNullOrEmpty(dataViewNode.Attributes["database"].Value))
                database = dataViewNode.Attributes["database"].Value;
            if (dataViewNode.Attributes["application"] != null && !string.IsNullOrEmpty(dataViewNode.Attributes["application"].Value))
                application = dataViewNode.Attributes["application"].Value;
            if (dataViewNode.Attributes["dataviewHandling"] != null && !string.IsNullOrEmpty(dataViewNode.Attributes["dataviewHandling"].Value))
                dataViewHandling = (DataViewHandlingOptions)Enum.Parse(typeof(DataViewHandlingOptions), dataViewNode.Attributes["dataviewHandling"].Value);
        }

        /// <summary>
        /// Fills the DataView object from it's xml representation contained in a string.
        /// </summary>
        /// <param name="xmlDataView">the string containing the dataview xml representation</param>
        public void GetFromXML(string xmlDataView)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlDataView);
            this.GetFromXML(doc);
        }

        /// <summary>
        /// Converts the current DataView object to it's xml representation and returns it into a string
        /// </summary>
        /// <returns>the resulting string containing the xml representation of the DataView</returns>
        public override string ToString()
        {
            return Utilities.XmlSerialize(this);
        }

        /// <summary>
        /// returns the tableName from a tableID
        /// </summary>
        /// <param name="tableID"></param>
        /// <returns></returns>
        public string GetTableNameById(int tableID)
        {
            string tableName = String.Empty;

            foreach (COEDataView.DataViewTable t in this.Tables)
            {
                if (t.Id == tableID)
                {
                    tableName = t.Name;
                    break;
                }
            }

            return tableName;
        }

        /// <summary>
        /// returns the tableAliasName from a tableID
        /// </summary>
        /// <param name="tableID"></param>
        /// <returns></returns>
        /// CBOE-1763 and CBOE-1764 --- Returns Table Alias name.
        public string GetTableAliasNameById(int tableID)
        {
            string tableAliasName = String.Empty;

            foreach (COEDataView.DataViewTable t in this.Tables)
            {
                if (t.Id == tableID)
                {
                    tableAliasName = t.Alias;
                    break;
                }
            }

            return tableAliasName;
        }
        //END CBOE-1763 and CBOE-1764

        /// <summary>
        /// returns the table given a tableID
        /// </summary>
        /// <param name="tableID">Identifier of the requested table</param>
        /// <returns>A table or null</returns>
        private DataViewTable GetTableById(int tableID)
        {
            DataViewTable retVal = null;
            foreach (COEDataView.DataViewTable table in this.Tables)
            {
                if (table.Id == tableID)
                {
                    retVal = table;
                    break;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Gets the table primary key given a tableId
        /// </summary>
        /// <param name="tableID">Identifier of the table to search</param>
        /// <returns>The PK of the table</returns>
        private string GetTablePrimaryKeyById(int tableID)
        {
            string retVal = "-1";
            DataViewTable foundTable = this.GetTableById(tableID);
            if (foundTable != null)
                retVal = foundTable.PrimaryKey;
            return retVal;
        }        

        private void InitializeManager(XmlNameTable xmlNameTable)
        {
            manager = new XmlNamespaceManager(xmlNameTable);
            manager.AddNamespace(xmlNamespace, this.xmlNs);
        }

        /// <summary>
        /// returns the tableName from a tableID
        /// </summary>
        /// <param name="tableID"></param>
        /// <returns></returns>
        public string GetDatabaseNameById(int tableID)
        {
            string databaseName = String.Empty;

            foreach (COEDataView.DataViewTable t in this.Tables)
            {
                if (t.Id == tableID)
                {
                    databaseName = t.Database;
                    break;
                }
            }

            return databaseName;
        }


        /// <summary>
        /// Returns a field from its id
        /// </summary>
        /// <param name="fieldId">The field id.</param>
        /// <returns>The Field</returns>
        public Field GetFieldById(int fieldId)
        {
            Field retVal = null;
            foreach (DataViewTable tbl in this.Tables)
            {
                retVal = tbl.Fields.getById(fieldId);
                if (retVal != null)
                    return retVal;
            }
            return retVal;
        }

        /// <summary>
        /// Returns a field from its id. If can't find throw exception.
        /// </summary>
        /// <param name="fieldId">The field id.</param>
        /// <returns>The Field</returns>
        public Field GetFieldByIdEx(int fieldId)
        {
            foreach (DataViewTable tbl in this.Tables)
            {
                Field retVal = tbl.Fields.getById(fieldId);
                if (retVal != null)
                {
                    return retVal;
                }
            }

            throw new Exception(string.Format("Can not find a field with ID '{0}' from data view '{1}'.", fieldId, Name));
        }        

        public void RemoveNonRelationalTables()
        {
            foreach (int nonRelTabID in GetNonRelationalTablesIDs())
            {
                COEDataView.DataViewTable dvTable = this.Tables.getById(nonRelTabID);
                this.Tables.Remove(dvTable);
            }
        }

        private List<int> GetNonRelationalTablesIDs()
        {
            List<int> idNotExists = new List<int>();
            foreach (COEDataView.DataViewTable table in this.Tables)
            {
                idNotExists.Add(table.Id);
            }
            idNotExists.Remove(this.Basetable);
            COEDataView.Relationship[] relationshipArr = this.Relationships.ToArray();
            IEnumerator relationshipEnum = this.Relationships.GetEnumerator();
            while (relationshipEnum.MoveNext())
            {
                COEDataView.Relationship relation = (COEDataView.Relationship)relationshipEnum.Current;
                // if (relation.Parent == coeDataView.Basetable)
                idNotExists.Remove(relation.Child);
            }
            return idNotExists;
        }
        
        #endregion

        #region Additional Classes
        [Serializable]
        public class DataViewTableList : List<DataViewTable>
        {
            public DataViewTable this[string name]
            {
                get
                {
                    foreach (DataViewTable currentTable in this)
                    {
                        if (currentTable.Name == name)
                            return currentTable;
                    }
                    return null;
                }
            }

            public DataViewTable getById(int id)
            {
                foreach (DataViewTable currentTable in this)
                {
                    if (currentTable.Id == id)
                        return currentTable;
                }
                return null;
            }
        }
        /// Table
        /// <summary>
        /// Single DataView Table that keeps track of a database table and it's fields.
        /// </summary>
        [Serializable]
        [XmlTypeAttribute("COEDataView.Table", Namespace = "COE.COEDataView")]
        public class DataViewTable
        {
            #region Properties

            [XmlArray("tags")]
            [XmlArrayItem("tag")]
            public List<string> Tags
            {
                get { return tags; }
                set { tags = value; }
            }

            /// <summary>
            /// List of fields that contains this table
            /// </summary>
            [XmlElement("fields")]
            public DataViewFieldList Fields
            {
                get { return fields; }
                set { fields = value; }
            }

            /// <summary>
            /// Table Identifier.
            /// </summary>
            [XmlAttribute("id")]
            public int Id
            {
                get { return id; }
                set { id = value; }
            }

            /// <summary>
            /// Table Name
            /// </summary>
            [XmlAttribute("name")]
            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            /// <summary>
            /// Table Alias
            /// </summary>
            [XmlAttribute("alias")]
            public string Alias
            {
                get
                {
                    if (string.IsNullOrEmpty(alias))
                        return name;
                    else
                        return System.Web.HttpUtility.HtmlDecode(alias);
                }
                set { alias = System.Web.HttpUtility.HtmlEncode(value); }
            }

            /// <summary>
            /// Table DataBase owner name
            /// </summary>
            [XmlAttribute("database")]
            public string Database
            {
                get { return database; }
                set { database = value; }
            }

            /// <summary>
            /// Table primary key id
            /// </summary>
            [XmlAttribute("primaryKey")]
            public string PrimaryKey
            {
                get { return primaryKey; }
                set { primaryKey = value; }
            }
                      

            /// <summary>
            /// Table primary key id
            /// </summary>
            [XmlAttribute("isView")]
            [DefaultValue(false)]
            public bool IsView
            {
                get { return isView; }
                set { isView = value; }
            }
            #endregion

            #region Variables
            private DataViewFieldList fields;
            private int id;
            private string name;
            private string alias;
            private string database;
            private string primaryKey; // the primary key may be a composition of fields.
            private bool isView;
            private List<string> tags;
            #endregion

            #region Constructors
            /// <summary>
            /// Default Constructor
            /// </summary>
            public DataViewTable()
            {
                this.fields = new DataViewFieldList();
                this.id = -1;
                this.name = string.Empty;
                this.alias = string.Empty;
                this.database = string.Empty;
                this.primaryKey = string.Empty;
                this.isView = false;
            }

            /// <summary>
            /// Constructor receiving the xml portion that represents the DataViewTable.
            /// </summary>
            /// <param name="tableNode">the xml representation of the DataViewTable</param>
            public DataViewTable(XmlNode tableNode)
            {
                fields = new DataViewFieldList();
                tags = new List<string>();
                foreach (XmlNode childNode in tableNode.ChildNodes)
                {
                    if (childNode.NodeType == XmlNodeType.Element)
                    {
                        if (childNode.Name == "fields")
                        {
                            Field fld = new Field(childNode);
                            fields.Add(fld);
                        }
                        else if (childNode.Name == "tags")
                        {
                            foreach (XmlNode tagNode in childNode.ChildNodes)
                            {
                                //CSBR-157442: Table Tag values are concatenated during Dataview Creation or Cloning
                                // use the innerText of the tagNode not the childNode
                                if (tagNode.NodeType == XmlNodeType.Element && tagNode.Name == "tag")
                                    tags.Add(tagNode.InnerText);
                            }
                        }
                    }
                }
                if (tableNode.Attributes["id"] != null && tableNode.Attributes["id"].Value != string.Empty)
                    id = int.Parse(tableNode.Attributes["id"].Value);
                if (tableNode.Attributes["name"] != null && tableNode.Attributes["name"].Value != string.Empty)
                    alias = name = tableNode.Attributes["name"].Value;
                if (tableNode.Attributes["alias"] != null && tableNode.Attributes["alias"].Value != string.Empty)
                    alias = System.Web.HttpUtility.HtmlEncode(tableNode.Attributes["alias"].Value);
                if (tableNode.Attributes["database"] != null && tableNode.Attributes["database"].Value != string.Empty)
                    database = tableNode.Attributes["database"].Value;
                if (tableNode.Attributes["primaryKey"] != null && tableNode.Attributes["primaryKey"].Value != string.Empty)
                    primaryKey = tableNode.Attributes["primaryKey"].Value;
                if (tableNode.Attributes["isView"] != null && !string.IsNullOrEmpty(tableNode.Attributes["isView"].Value))
                    isView = (tableNode.Attributes["isView"].Value == "1" || tableNode.Attributes["isView"].Value.ToLower() == "true") ? true : false;

               }
            #endregion

            #region Methods

            public Field this[string name]
            {
                get
                {
                    foreach (Field currentField in this.Fields)
                        if (currentField.Name.Equals(name))
                            return currentField;

                    return null;
                }
            }
            /// <summary>
            /// Converts the DataView Table to it's xml representation and returns it into an string object.
            /// </summary>
            /// <returns>the string containing the xml representation of the DataView Table</returns>
            public override string ToString()
            {
                StringBuilder builder = new StringBuilder("<table id=\"");
                builder.Append(id);
                builder.Append("\" name=\"");
                builder.Append(name);
                builder.Append("\" alias=\"");
                builder.Append(alias);
                builder.Append("\" database=\"");
                builder.Append(database);
                builder.Append("\" isView=\"");
                builder.Append(isView ? "1" : "0");
                builder.Append("\" primaryKey=\"");
                builder.Append(primaryKey);                
                builder.Append("\">");

                for (int i = 0; i < fields.Count; i++)
                {
                    builder.Append(fields[i].ToString());
                }
                if (tags != null && tags.Count > 0)
                {
                    builder.Append("<tags>");
                    foreach (string tag in tags)
                    {
                        builder.Append("<tag>");
                        builder.Append(tag);
                        builder.Append("</tag>");
                    }
                    builder.Append("</tags>");
                }
                builder.Append("</table>");

                return builder.ToString();
            }
            #endregion
        }
       

        /// Field
        /// <summary>
        /// This class keeps track of a database field.
        /// </summary>
        [Serializable]
        [XmlTypeAttribute("fields", Namespace = "COE.COEDataView")]
        public class Field
        {
            #region Properties
            /// <summary>
            /// The identifier of the field. This id is correlative for all fields in the dataview.
            /// </summary>
            [XmlAttribute("id")]
            public int Id
            {
                get { return id; }
                set { id = value; }
            }

            /// <summary>
            /// Field Name
            /// </summary>
            [XmlAttribute("name")]
            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            /// <summary>
            /// Field Type
            /// </summary>
            [XmlAttribute("dataType")]
            public AbstractTypes DataType
            {
                get { return dataType; }
                set { dataType = value; }
            }

            /// <summary>
            /// Id that correlates this field with the lookup one. I.E. the fieldID of EmpID
            /// </summary>
            [XmlAttribute("lookupFieldId")]
            [DefaultValue(-1)]
            public int LookupFieldId
            {
                get { return lookupFieldId; }
                set { lookupFieldId = value; }
            }

            /// <summary>
            /// Id of the field to display. I.E. the fieldID of EmpName
            /// </summary>
            [XmlAttribute("lookupDisplayFieldId")]
            [DefaultValue(-1)]
            public int LookupDisplayFieldId
            {
                get { return lookupDisplayFieldId; }
                set { lookupDisplayFieldId = value; }
            }

            /// <summary>
            /// Alias of a field.
            /// </summary>
            [XmlAttribute("alias")]
            [DefaultValue("")]
            public string Alias
            {
                get
                {
                    if (string.IsNullOrEmpty(this.alias))
                        return name;
                    else
                        return System.Web.HttpUtility.HtmlDecode(alias);
                }
                set { alias = System.Web.HttpUtility.HtmlEncode(value); }
            }

            /// <summary>
            /// indexType of a field.
            /// </summary>
            [XmlAttribute("indexType")]
            public IndexTypes IndexType
            {
                get { return indexType; }
                set { indexType = value; }
            }

            /// <summary>
            /// mimeType of a field.
            /// </summary>
            [XmlAttribute("mimeType")]
            public MimeTypes MimeType
            {
                get { return mimeType; }
                set { mimeType = value; }
            }

            /// <summary>
            /// Display or not this field in a form
            /// </summary>
            [XmlAttribute("visible")]
            [DefaultValue(false)] // CSBR-163898 changed the value to "false" to serialize the field with visible attribute set to "true"
            public bool Visible
            {
                get { return _visible; }
                set { _visible = value; }
            }

            /// <summary>
            /// default field for the table
            /// </summary>
            [XmlAttribute("isDefault")]
            [DefaultValue(false)]
            public bool IsDefault
            {
                get { return _isDefault; }
                set { _isDefault = value; }
            }

            /// <summary>
            /// Sort direction for a given lookup field.
            /// </summary>
            [XmlAttribute("lookupSortOrder")]
            [DefaultValue(SortDirection.ASCENDING)]
            public SortDirection LookupSortOrder
            {
                get { return _lookupSortOrder; }
                set { _lookupSortOrder = value; }
            }

            /// <summary>
            /// Order in which we can based to present the list of fields.
            /// </summary>
            [XmlAttribute("sortOrder")]
            [DefaultValue(-1)]
            public int SortOrder
            {
                get { return _sortOrder; }
                set { _sortOrder = value; }
            }

            /// <summary>
            /// Display or not this field in a form
            /// </summary>
            [XmlAttribute("isUniqueKey")]
            [DefaultValue(false)]
            public bool IsUniqueKey
            {
                get { return _isUniqueKey; }
                set { _isUniqueKey = value; }
            }


            /// <summary>
            /// the Id of the table that contains this field
            /// </summary>
            [XmlIgnore()]
            public int ParentTableId
            {
                get { return _parentTableId; }
                set { _parentTableId = value; }
            }
            //Property added to manage the index value of field - PP on 29Jan2013
            /// <summary>
            /// Gets or sets the field indexed value
            /// </summary>
            [XmlAttribute("isIndexed")]
            [DefaultValue(false)]
            public bool IsIndexed
            {
                get
                {
                    return _isIndexed;
                }
                set
                {
                    _isIndexed = value;
                }
            }

            /// <summary>
            /// Default Query field for the table
            /// </summary>
            [XmlAttribute("isDefaultQuery")]
            [DefaultValue(false)]
            public bool IsDefaultQuery
            {
                get { return _isDefaultQuery; }
                set { _isDefaultQuery = value; }
            }
			//CBOE-529 Added property for indexName. ASV 27032013
            /// <summary>
            /// field's index name
            /// </summary>
            [XmlAttribute("indexName")]
            [DefaultValue("")]
            public string IndexName
            {
                get
                { 
                    return _indexName; 
                }
                set 
                { 
                    _indexName = value; 
                }
            }
            #endregion

            #region Variables
            private int id;
            private string name;
            private int lookupFieldId;
            private int lookupDisplayFieldId;
            private string alias;
            private AbstractTypes dataType;
            private IndexTypes indexType;
            private MimeTypes mimeType;
            private bool _visible;
            private bool _isDefault;
            private bool _isUniqueKey;
            private SortDirection _lookupSortOrder;
            private int _sortOrder;
            private int _parentTableId;
            private bool _isIndexed;
            // Variable for holding Default Query value
            private bool _isDefaultQuery;
            private string _indexName;			//CBOE-529 Added variable for indexName. ASV 27032013
            #endregion

            #region Constructors
            /// <summary>
            /// Default Constructor
            /// </summary>
            public Field()
            {
                this.id = -1;
                this.name = string.Empty;
                this.dataType = AbstractTypes.Text;
                this.mimeType = MimeTypes.NONE;
                this.indexType = IndexTypes.NONE;
                this.lookupFieldId = -1;
                this.lookupDisplayFieldId = -1;
                this.alias = string.Empty;
                this._visible = true;
                this._isDefault = false;
                this._lookupSortOrder = SortDirection.ASCENDING;
                this._sortOrder = -1;
                this._isUniqueKey = false;
                this._parentTableId = -1;
                //set default value as false for field indexed value - PP on 29Jan2013
                this._isIndexed = false;
                this._isDefaultQuery = false; //By default "_isDefaultQuery" varaible is set to "FALSE".
                this._indexName = string.Empty;  //CBOE-529 by default _indexName is set to empty. ASV 27032013
            }

            /// <summary>
            /// Constructor that fills the field object from an xml portion that represents the field.
            /// </summary>
            /// <param name="fieldNode">the xml representation of the field</param>
            public Field(XmlNode fieldNode)
            {
                if (fieldNode.Attributes["id"] != null && fieldNode.Attributes["id"].Value != string.Empty)
                    id = int.Parse(fieldNode.Attributes["id"].Value);
                if (fieldNode.Attributes["name"] != null && fieldNode.Attributes["name"].Value != string.Empty)
                    alias = name = fieldNode.Attributes["name"].Value;
                if (fieldNode.Attributes["dataType"] != null && fieldNode.Attributes["dataType"].Value != string.Empty)
                    dataType = COEConvert.ToAbstractType(fieldNode.Attributes["dataType"].Value);
                if (fieldNode.Attributes["mimeType"] != null && fieldNode.Attributes["mimeType"].Value != string.Empty)
                    mimeType = COEConvert.ToMimeType(fieldNode.Attributes["mimeType"].Value);
                if (fieldNode.Attributes["indexType"] != null && fieldNode.Attributes["indexType"].Value != string.Empty)
                    indexType = COEConvert.ToIndexType(fieldNode.Attributes["indexType"].Value);
                if (fieldNode.Attributes["lookupFieldId"] != null && fieldNode.Attributes["lookupFieldId"].Value != string.Empty)
                    lookupFieldId = int.Parse(fieldNode.Attributes["lookupFieldId"].Value);
                else
                    lookupFieldId = -1;
                if (fieldNode.Attributes["lookupDisplayFieldId"] != null && fieldNode.Attributes["lookupDisplayFieldId"].Value != string.Empty)
                    lookupDisplayFieldId = int.Parse(fieldNode.Attributes["lookupDisplayFieldId"].Value);
                else
                    lookupDisplayFieldId = -1;
                if (fieldNode.Attributes["alias"] != null && fieldNode.Attributes["alias"].Value != string.Empty)
                    alias = System.Web.HttpUtility.HtmlEncode(fieldNode.Attributes["alias"].Value);

                if (fieldNode.Attributes["visible"] != null && !string.IsNullOrEmpty(fieldNode.Attributes["visible"].Value))
                    _visible = fieldNode.Attributes["visible"].Value == "1" || fieldNode.Attributes["visible"].Value.ToUpper() == "TRUE" ? true : false;    //CBOE-1720

                if (fieldNode.Attributes["isDefault"] != null && !string.IsNullOrEmpty(fieldNode.Attributes["isDefault"].Value))
                    _isDefault = fieldNode.Attributes["isDefault"].Value == "1" || fieldNode.Attributes["isDefault"].Value.ToUpper() == "TRUE" ? true : false;      //CBOE-1720

                if (fieldNode.Attributes["lookupSortOrder"] != null && string.IsNullOrEmpty(fieldNode.Attributes["lookupSortOrder"].Value))
                {
                    if (Enum.IsDefined(typeof(SortDirection), fieldNode.Attributes["lookupSortOrder"].Value))
                        _lookupSortOrder = (SortDirection)Enum.Parse(typeof(SortDirection), fieldNode.Attributes["lookupSortOrder"].Value);
                }
                if (fieldNode.Attributes["sortOrder"] != null && fieldNode.Attributes["sortOrder"].Value != string.Empty)
                    _sortOrder = int.Parse(fieldNode.Attributes["sortOrder"].Value);
                else
                    _sortOrder = -1;

                if (fieldNode.Attributes["isUniqueKey"] != null && !string.IsNullOrEmpty(fieldNode.Attributes["isUniqueKey"].Value))
                    _isUniqueKey = fieldNode.Attributes["isUniqueKey"].Value == "1" || fieldNode.Attributes["isUniqueKey"].Value.ToUpper() == "TRUE" ? true : false;    //CBOE-1720

                if (fieldNode.ParentNode.Attributes != null && fieldNode.ParentNode.Attributes["id"] != null && fieldNode.ParentNode.Attributes["id"].Value != string.Empty)
                    _parentTableId = int.Parse(fieldNode.ParentNode.Attributes["id"].Value);

                //isIndexed value parsing - PP on 29Jan2013
                if (fieldNode.Attributes["isIndexed"] != null && !string.IsNullOrEmpty(fieldNode.Attributes["isIndexed"].Value))
                    _isIndexed = fieldNode.Attributes["isIndexed"].Value == "1" || fieldNode.Attributes["isIndexed"].Value.ToUpper() == "TRUE" ? true : false;  //CBOE-1720

                //Setting value from FIELD XML to "_isDefaultQuery" variable.
                if (fieldNode.Attributes["isDefaultQuery"] != null && !string.IsNullOrEmpty(fieldNode.Attributes["isDefaultQuery"].Value))
                    _isDefaultQuery = fieldNode.Attributes["isDefaultQuery"].Value == "1" || fieldNode.Attributes["isDefaultQuery"].Value.ToUpper() == "TRUE" ? true : false;  //CBOE-1720 

                //CBOE-529 setting value for indexName. ASV 27032013
                if (fieldNode.Attributes["indexname"] != null && !string.IsNullOrEmpty(fieldNode.Attributes["indexname"].Value))
                    _indexName = System.Web.HttpUtility.HtmlEncode(fieldNode.Attributes["indexname"].Value);

            }
            #endregion

            #region Methods
            /// <summary>
            /// Converts the DataView Field to it's xml representation and returns it into an string object.
            /// </summary>
            /// <returns>the string containing the xml representation of the DataView Field</returns>
            public override string ToString()
            {
                // <fields id="1" name="CONTAINER_ID" dataType="INTEGER"/>
                StringBuilder builder = new StringBuilder("<fields id=\"");
                builder.Append(id);
                builder.Append("\" name=\"");
                builder.Append(name);
                builder.Append("\" dataType=\"");
                builder.Append(dataType.ToString().ToUpper());
                builder.Append("\" indexType=\"");
                builder.Append(indexType.ToString().ToUpper());
                builder.Append("\" mimeType=\"");
                builder.Append(mimeType.ToString().ToUpper());
                builder.Append("\" visible=\"");
                builder.Append(_visible ? "1" : "0");
                builder.Append("\" isDefault=\"");
                builder.Append(_isDefault ? "1" : "0");
                builder.Append("\" isUniqueKey=\"");
                builder.Append(_isUniqueKey ? "1" : "0");

                builder.Append("\" isDefaultQuery=\"");
                builder.Append(_isDefaultQuery ? "1" : "0");

                //add isIndexed attribute in xml formatter - PP on 29Jan2013
                builder.Append("\" isIndexed=\"");
                builder.Append(_isIndexed ? "1" : "0");
                builder.Append("\" lookupSortOrder=\"");
                builder.Append(_lookupSortOrder.ToString().ToUpper());

                builder.Append("\"");
                if (this.LookupFieldId != -1)
                {
                    builder.Append(" lookupFieldId=\"");
                    builder.Append(lookupFieldId.ToString());
                    builder.Append("\"");
                }
                if (this.LookupDisplayFieldId != -1)
                {
                    builder.Append(" lookupDisplayFieldId=\"");
                    builder.Append(lookupDisplayFieldId.ToString());
                    builder.Append("\"");
                }
                if (this.Alias != string.Empty)
                {
                    builder.Append(" alias=\"");
                    builder.Append(alias);
                    builder.Append("\"");
                }
                if (this.SortOrder > -1)
                {
                    builder.Append(" sortOrder=\"");
                    builder.Append(_sortOrder.ToString());
                    builder.Append("\"");
                }

				//CBOE-529 Added indexName property in xml formater. ASV 27032013
				if (this.IndexName != string.Empty)
                {
                    builder.Append(" indexname=\"");
                    builder.Append(_indexName);
                    builder.Append("\"");
                }
                builder.Append(" />");

                return builder.ToString();
            }
            #endregion

            
        }
        [Serializable]
        public class DataViewFieldList : List<Field>
        {
            public Field this[string name]
            {
                get
                {
                    foreach (Field currentField in this)
                    {
                        if (currentField.Alias == name)
                            return currentField;
                    }
                    return null;
                }
            }

            public Field getById(int id)
            {               
                foreach (Field currentField in this)
                {
                    if (currentField.Id == id)
                        return currentField;
                }

                return null;
            }
        }

        /// Relationship
        /// <summary>
        /// This class keeps track of the relationships between tables, including the parent and child tables as well as the fields that act as keys for joining.
        /// </summary>
        [Serializable]
        [XmlTypeAttribute("relationship", Namespace = "COE.COEDataView")]
        public class Relationship
        {
            #region Properties
            /// <summary>
            /// The parent table key id, used for linking the tables.
            /// </summary>
            [XmlAttribute("parentkey")]
            public int ParentKey
            {
                get { return parentKey; }
                set { parentKey = value; }
            }

            /// <summary>
            /// The child table key id, used for linking the tables.
            /// </summary>
            [XmlAttribute("childkey")]
            public int ChildKey
            {
                get { return childKey; }
                set { childKey = value; }
            }

            /// <summary>
            /// The id of the parent table in the relationship.
            /// </summary>
            [XmlAttribute("parent")]
            public int Parent
            {
                get { return parent; }
                set { parent = value; }
            }

            /// <summary>
            /// The id of the child table in the relationship
            /// </summary>
            [XmlAttribute("child")]
            public int Child
            {
                get { return child; }
                set { child = value; }
            }

            /// <summary>
            /// Type of join to be performed when creating the query. Outer or inner.
            /// </summary>
            [XmlAttribute("jointype")]
            public JoinTypes JoinType
            {
                get { return joinType; }
                set { joinType = value; }
            }
            #endregion

            #region Variables
            private int parentKey;
            private int childKey;
            private int parent;
            private int child;
            private JoinTypes joinType;
            #endregion

            #region Constructors
            /// <summary>
            /// Default constructor
            /// </summary>
            public Relationship()
            {
                this.parentKey = -1;
                this.childKey = -1;
                this.parentKey = -1;
                this.child = -1;
                this.joinType = JoinTypes.INNER;
            }

            /// <summary>
            /// Constructor that fills the object from it's xml representation.
            /// </summary>
            /// <param name="relationshipNode">xml snippet containing the xml representation of the object.</param>
            public Relationship(XmlNode relationshipNode)
            {
                if (relationshipNode.Attributes["parentkey"] != null && relationshipNode.Attributes["parentkey"].Value != string.Empty)
                    parentKey = int.Parse(relationshipNode.Attributes["parentkey"].Value);
                if (relationshipNode.Attributes["childkey"] != null && relationshipNode.Attributes["childkey"].Value != string.Empty)
                    childKey = int.Parse(relationshipNode.Attributes["childkey"].Value);
                if (relationshipNode.Attributes["parent"] != null && relationshipNode.Attributes["parent"].Value != string.Empty)
                    parent = int.Parse(relationshipNode.Attributes["parent"].Value);
                if (relationshipNode.Attributes["child"] != null && relationshipNode.Attributes["child"].Value != string.Empty)
                    child = int.Parse(relationshipNode.Attributes["child"].Value);
                if (relationshipNode.Attributes["jointype"] != null && relationshipNode.Attributes["jointype"].Value != string.Empty)
                    joinType = COEConvert.ToJoinType(relationshipNode.Attributes["jointype"].Value);
            }
            #endregion

            #region Methods
            /// <summary>
            /// Converts the DataView Relationship to it's xml representation and returns it into an string object.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                StringBuilder builder = new StringBuilder("<relationship child=\"");
                builder.Append(child);
                builder.Append("\" parent=\"");
                builder.Append(parent);
                builder.Append("\" childkey=\"");
                builder.Append(childKey);
                builder.Append("\" parentkey=\"");
                builder.Append(parentKey);
                builder.Append("\" jointype=\"");
                builder.Append(joinType.ToString().ToUpper());
                builder.Append("\" />");

                return builder.ToString();
            }
            #endregion
        }
        #endregion

        #region enums
        /// <summary>
        /// Used to determine how to generate the field strings. Possible values: INTEGER, REAL, TEXT, DATE, BOOLEAN OR LOB.
        /// </summary>
        [XmlTypeAttribute("AbstractTypes", Namespace = "COE.COEDataView")]
        public enum AbstractTypes
        {
            /// <summary>
            /// An integer field type. This can be an integer, a shor or another integer database types.
            /// </summary>
            [XmlEnum("INTEGER")]
            Integer = 0,
            /// <summary>
            /// An real field type. This can be a float, a double or another real database types.
            /// </summary>
            [XmlEnum("REAL")]
            Real = 1,
            /// <summary>
            /// A text field type. This can be a text, a varchar, a char or other Text database types.
            /// </summary>
            [XmlEnum("TEXT")]
            Text = 2,
            /// <summary>
            /// A date field type. This can be a DateTime, a Date or other Date database types.
            /// </summary>
            [XmlEnum("DATE")]
            Date = 3,
            /// <summary>
            /// A boolean field type. This can be a bit or other boolean database type.
            /// </summary>
            [XmlEnum("BOOLEAN")]
            Boolean = 4,

			//CBOE-910: Added Blob & Clob datatypes  ASV 17052013
            /// <summary>
            /// A BLOB field type. This can be a image or structure database type.
            /// </summary>
            [XmlEnum("BLOB")]
            BLob = 5,

            /// <summary>
            /// A CLOB field type. This can be a image or structure database type.
            /// </summary>
            [XmlEnum("CLOB")]
            CLob = 6
        }

        /// <summary>
        /// Indicates a join blend. Possible values: OUTER INNER
        /// </summary>
        [XmlTypeAttribute("JoinTypes", Namespace = "COE.COEDataView")]
        public enum JoinTypes
        {
            /// <summary>
            /// Specifies the join to be created is an outer join
            /// </summary>
            [XmlEnum("OUTER")]
            OUTER,
            /// <summary>
            /// Specifies the join to be created is an inner join
            /// </summary>
            [XmlEnum("INNER")]
            INNER
        }

        /// <summary>
        /// Determines if the given index type is a structure type.
        /// </summary>
        /// <param name="type">the index type</param>
        /// <returns>true: structure type, false: non-structure type</returns>
        public static bool IsStructureIndexType(IndexTypes type)
        {
            return (type == IndexTypes.CS_CARTRIDGE || type == IndexTypes.DIRECT_CARTRIDGE || type==IndexTypes.JCHEM_CARTRIDGE);
        }

        /// <summary>
        /// Determines if the given filed has a structure index type.
        /// </summary>
        /// <param name="curField">
        /// The cur field.
        /// </param>
        /// <param name="lookupField">
        /// The lookup field.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsStructureIndexType(FieldBO curField, FieldBO lookupField = null)
        {
            return IsStructureIndexType(lookupField == null ? curField.IndexType : lookupField.IndexType);
        }

        /// <summary>
        /// Get the Index Type from the given field
        /// </summary>
        /// <param name="curField"></param>
        /// <param name="lookupField"></param>
        /// <returns></returns>
        public static IndexTypes GetFieldIndexType(FieldBO curField, FieldBO lookupField = null)
        {
            return lookupField == null ? curField.IndexType : lookupField.IndexType;
        }

        /// <summary>
        /// Determines if the given mime type is a structure content type.
        /// </summary>
        /// <param name="mimeType">
        /// The mime type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsStructureContentType(MimeTypes mimeType)
        {
            return mimeType == MimeTypes.CHEMICAL_X_CDX || mimeType == MimeTypes.CHEMICAL_X_DATADIRECT_CTAB
                    || mimeType == MimeTypes.CHEMICAL_X_MDLMOLFILE || mimeType == MimeTypes.CHEMICAL_X_MDL_CHIME
                    || mimeType == MimeTypes.CHEMICAL_X_SMILES || mimeType == MimeTypes.CHEMICAL_X_CML;
        }

        /// <summary>
        /// Determines if the given filed has a structure content type.
        /// </summary>
        /// <param name="curField">
        /// The current field
        /// </param>
        /// <param name="lookupField">
        /// The lookup field
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsStructureContentType(FieldBO curField, FieldBO lookupField = null)
        {
            return IsStructureContentType(lookupField == null ? curField.MimeType : lookupField.MimeType);
        }

        /// <summary>
        /// Determines if the given field is a structure type.
        /// </summary>
        /// <param name="field">data view field</param>
        /// <returns>true: structure type, false: non-structure type</returns>
        public static bool IsStructureField(FieldBO field)
        {
            return field != null
                   && (field.Alias == "Structure" || IsStructureIndexType(field.IndexType)
                       || IsStructureContentType(field.MimeType));
        }

        /// <summary>
        /// Determines if the given field is a structure type.
        /// </summary>
        /// <param name="curField">current field</param>
        /// <param name="lookupField">look up field</param>
        /// <returns>true: structure type, false: non-structure type</returns>
        public static bool IsStructureField(FieldBO curField, FieldBO lookupField)
        {
            return IsStructureField(lookupField ?? curField);
        }

        /// <summary>
        /// Determine if the given field is in CTAB format
        /// </summary>
        /// <param name="curField"></param>
        /// <param name="lookupField"></param>
        /// <returns></returns>
        public static bool IsDirectStrcuture(FieldBO curField, FieldBO lookupField)
        {
            return lookupField != null
                       ? lookupField.MimeType == MimeTypes.CHEMICAL_X_DATADIRECT_CTAB
                       : curField != null && curField.MimeType == MimeTypes.CHEMICAL_X_DATADIRECT_CTAB;
        }

        /// <summary>
        /// Indicates the type of index on a field. Possible values: CS_CARTRIDGE, DIRECT_CARTRIDGE FULL_TEXT
        /// </summary>
        [DefaultValue(NONE)]
        [XmlTypeAttribute("IndexTypes", Namespace = "COE.COEDataView")]
        public enum IndexTypes
        {
            /// <summary>
            /// Specifies no index
            /// </summary>
            [XmlEnum("NONE")]
            NONE,
            /// <summary>
            /// UNKNOWN
            /// </summary>
            [XmlEnum("UNKNOWN")]
            UNKNOWN,
            /// <summary>
            /// CS_INDEX
            /// </summary>
            [XmlEnum("CS_CARTRIDGE")]
            CS_CARTRIDGE,
            /// <summary>
            /// DIRECT_INDEX
            /// </summary>
            [XmlEnum("DIRECT_CARTRIDGE")]
            DIRECT_CARTRIDGE,
            /// <summary>
            /// JCHEM_INDEX
            /// </summary>
            [XmlEnum("JCHEM_CARTRIDGE")]
            JCHEM_CARTRIDGE,
            /// <summary>
            /// Specifies the join to be created is an inner join
            /// </summary>
            [XmlEnum("FULL_TEXT")]
            FULL_TEXT
        }       

        /// <summary>
        /// Indicates a join blend. Possible values: OUTER INNER
        /// </summary>
        [XmlTypeAttribute("MimeTypes", Namespace = "COE.COEDataView")]
        public enum MimeTypes
        {
            /// <summary>
            /// Specifies no mimetype
            /// </summary>
            NONE,
            /// <summary>
            /// Specifies UNKNOWN mimetype
            /// </summary>
            UNKNOWN,
            /// <summary>
            /// Specifies IMAGE_JPEG mimetype
            /// </summary>
            IMAGE_JPEG,
            /// <summary>
            /// Specifies IMAGE_GIF mimetype
            /// </summary>
            IMAGE_GIF,
            /// <summary>
            /// Specifies IMAGE_PNG mimetype
            /// </summary>
            IMAGE_PNG,
            /// <summary>
            /// Specifies IMAGE_X_WMF mimetype
            /// </summary>
            IMAGE_X_WMF,
            /// <summary>
            /// Specifies IMAGE/JPEG mimetype
            /// </summary>
            CHEMICAL_X_MDLMOLFILE,
            /// <summary>
            /// Specifies CHEMICAL_X_CDX mimetype
            /// </summary>
            CHEMICAL_X_CDX,
            /// <summary>
            /// Specifies CHEMICAL_X_SMILES mimetype
            /// </summary>
            CHEMICAL_X_SMILES,
            /// <summary>
            /// Specifies CHEMICAL_X_MDL_CHIME mimetype
            /// </summary>
            CHEMICAL_X_MDL_CHIME,
            /// <summary>
            /// Specifies CHEMICAL_X_DATADIRECT_CTAB mimetype
            /// </summary>
            CHEMICAL_X_DATADIRECT_CTAB,
            /// <summary>
            /// Specifies CHEMICAL_X_CML mimetype
            /// </summary>
            CHEMICAL_X_CML,
            /// <summary>
            /// Specifies TEXT_XML mimetype
            /// </summary>
            TEXT_XML,
            /// <summary>
            /// Specifies TEXT_HTML mimetype
            /// </summary>
            TEXT_HTML,
            /// <summary>
            /// Specifies IMAGE/JPEG mimetype
            /// </summary>
            TEXT_PLAIN,
            /// <summary>
            /// Specifies TEXT_RAW mimetype
            /// </summary>
            TEXT_RAW,
            /// <summary>
            /// Specifies APPLICATION_MS_EXCEL mimetype
            /// </summary>
            APPLICATION_MS_EXCEL,
            /// <summary>
            /// Specifies APPLICATION_MS_MSWORD mimetype
            /// </summary>
            APPLICATION_MS_MSWORD,
            /// <summary>
            /// Specifies APPLICATION_PDF mimetype
            /// </summary>
            APPLICATION_PDF
        }


        /// <summary>
        /// Defines the sort direction for a field that constains a LookUp
        /// </summary>
        [XmlTypeAttribute("SortDirection", Namespace = "COE.COEDataView")]
        public enum SortDirection
        {
            [XmlEnum("ASCENDING")]
            ASCENDING,
            [XmlEnum("DESCENDING")]
            DESCENDING,
        }

        /// <summary>
        /// Allowable values for dataviews behaviours being passed to search service. Default value is USE_CLIENT_DATAVIEW
        /// </summary>
        [DefaultValue(USE_CLIENT_DATAVIEW)]
        [XmlTypeAttribute("DataViewHandlingOptions", Namespace = "COE.COEDataView")]
        public enum DataViewHandlingOptions
        {
            /// <summary>
            /// Use dataview as provided by client and connect as current user
            /// </summary>
            [XmlEnum("USE_CLIENT_DATAVIEW")]
            USE_CLIENT_DATAVIEW,
            /// <summary>
            /// Fetch the dataview from the database and connect via proxy user
            /// </summary>
            [XmlEnum("USE_SERVER_DATAVIEW")]
            USE_SERVER_DATAVIEW,
            /// <summary>
            /// Fetch the dataview from the database and merge it with the information in client dataview, then connect via proxy
            /// </summary>
            [XmlEnum("MERGE_CLIENT_AND_SERVER_DATAVIEW")]
            MERGE_CLIENT_AND_SERVER_DATAVIEW
        }
        #endregion
    }

}
