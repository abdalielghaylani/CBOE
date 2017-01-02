using System;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems;
using System.Text;

namespace CustomSelectClause {
    public class CastSelectClause : SelectClauseItem, ISelectClauseParser {
        private string _destinationType;
        private string _fieldName;

        public string DestinationType {
            get {
                return _destinationType;
            }
        }

        protected override string GetDependantString(CambridgeSoft.COE.Framework.Common.DBMSType dataBaseType, System.Collections.Generic.List<CambridgeSoft.COE.Framework.Common.SqlGenerator.Value> values) {
            /*
             CAST(field AS DESTINATIONTYPE)
             */
            StringBuilder builder = new StringBuilder("CAST(");
            builder.Append(_fieldName);
            builder.Append(" AS ");
            builder.Append(_destinationType);
            builder.Append(")");

            return builder.ToString();
        }

        public override string Name {
            get {
                return this.Alias;
            }
        }


        #region ISelectClauseParser Members

        public SelectClauseItem CreateInstance(System.Xml.XmlNode resultsXmlNode, CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.INamesLookup dvnLookup) {
            /*
             <cast fieldId="" dataType"" alias="" />
            */
            
            // dvnLookup comes to play when a fieldid is to be received in the clause, dvnLookup provides methods for looking up field names,
            // tables names, columns, parent tables, and so on based on the dataview being used.
            // as in this example we do not use a field, dvnLookup is not needed.
            
            CastSelectClause castClause = new CastSelectClause();
            if(resultsXmlNode.Attributes["alias"] != null)
                castClause.Alias = resultsXmlNode.Attributes["alias"].Value;
            castClause._fieldName = dvnLookup.GetColumn(int.Parse(resultsXmlNode.Attributes["fieldId"].Value)).GetFullyQualifiedNameString();
            castClause._destinationType = resultsXmlNode.Attributes["dataType"] == null ? "NUMBER(9)" : resultsXmlNode.Attributes["dataType"].Value;

            return castClause;
        }

        #endregion
    }
}
