using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Properties;
using System.Xml;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems {
    public class SelectClauseMarkedHit : SelectClauseItem, ISelectClauseParser {
        #region Variables
        private int _dataviewID;
        private Field _baseTablePK;
        #endregion
        #region Properties

        /// <summary>
        /// The field that contains the alias for the formula field.
        /// </summary>
        public override string Alias
        {
            get
            {
                if ((base.Alias == null) || (base.Alias == string.Empty))
                {
                    base.Alias = "Marked";
                }
                return base.Alias;
            }
            set
            {
                base.Alias = value;
            }
        }

        public override string Name
        {
            get
            {
                if (this.Alias != null && this.Alias.Trim() != string.Empty)
                        return this.Alias;

                return Resources.CentralizedStorageDB + "." + Resources.COESavedHitListTableName + ".ID";

            }

        }

        public int DataViewID {
            get { return _dataviewID; }
            set { _dataviewID = value; }
        }

        internal Field BaseTablePK
        {
            get
            {
                return _baseTablePK;
            }
            set
            {
                _baseTablePK = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its values to its default values.
        /// </summary>
        public SelectClauseMarkedHit()
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// This method does the actual job.
        /// In this case returns the cscartridge formula sintaxys.
        /// </summary>
        /// <param name="dataBaseType">The database type.</param>
        /// <returns>A string containing the select part corresponding to this clause (i.e. the CsCartridge.Formula(fieldName,''))</returns>
        protected override string GetDependantString(DBMSType dataBaseType, List<Value> values)
        {
            if (dataBaseType != DBMSType.ORACLE)
                throw new Exception("This select clause is only working in oracle implementations.");
            if(_dataviewID == -1)
                throw new Exception("DataViewID was not specified for the mark result criteria");
            //(select count(*)
            //from 	coesavedhitlist s,
            //        coesavedhitlistid si
            //where 	s.hitlistid=si.hitlistid
            //and 	si.type='MARKED'
            //and 	si.user_id='CSSADMIN'
            //and		si.dataview_id=5002
            //and 	hitlist.id=s.id
            //and 	rownum < 2)

            StringBuilder builder = new StringBuilder("(");
            builder.Append("select count(*) from ");
            builder.Append(Resources.CentralizedStorageDB);
            builder.Append(".");
            builder.Append(Resources.COESavedHitListTableName);
            builder.Append(" s, ");
            builder.Append(Resources.CentralizedStorageDB);
            builder.Append(".");
            builder.Append(Resources.COESavedHitListIDTableName);
            builder.Append(" sid ");
            builder.Append("where s.hitlistid=sid.hitlistid and sid.type=:");
            builder.Append(values.Count);

            values.Add(new Value("MARKED", System.Data.DbType.AnsiString));
            
            builder.Append(" and sid.user_id=:");
            builder.Append(values.Count);

            values.Add(new Value(COEUser.Name.ToUpper(), System.Data.DbType.AnsiString));

            builder.Append(" and sid.dataview_id=:");
            builder.Append(values.Count);

            values.Add(new Value(_dataviewID.ToString(), System.Data.DbType.Int32));

            builder.Append(" and s.id = ");
            builder.Append(_baseTablePK.GetFullyQualifiedNameString());
            builder.Append(" and rownum < 2");
            builder.Append(")");

            return builder.ToString();
        }
        #endregion

        #region ISelectClauseParser Members
        /// <summary>
        /// Implementation of ISelectClauseParser. Returns an Instance of SelectClauseFormula according to the desired xml snippet.
        /// </summary>
        /// <param name="fieldNode">The CsCartridge Formula node of the search results xml definition.</param>
        /// <param name="dvnLookup">The INamesLookup interface from wich the parser can obtain the names corresponding to ids in dataview.xml</param>
        /// <returns>An instance of SelectClauseFormula.</returns>



        public SelectClauseItem CreateInstance(XmlNode resultsXmlNode, CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.INamesLookup dvnLookup)
        {
            SelectClauseMarkedHit item = new SelectClauseMarkedHit();
            if (resultsXmlNode.Attributes["alias"] != null)
                item.Alias = resultsXmlNode.Attributes["alias"].Value.ToString();

            if (resultsXmlNode.Attributes["visible"] != null && resultsXmlNode.Attributes["visible"].Value != string.Empty)
                item.Visible = bool.Parse(resultsXmlNode.Attributes["visible"].Value);

            item.DataViewID = resultsXmlNode.Attributes["dataViewID"] != null ? int.Parse(resultsXmlNode.Attributes["dataViewID"].Value) : -1;
            item._baseTablePK = dvnLookup.GetBaseTablePK();
            return item;
        }
        #endregion
    }
}
