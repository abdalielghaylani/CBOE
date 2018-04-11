using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
using System.Xml;

namespace CustomMolWeightCriteria {
    public class MolWeightWhereClause : WhereClauseNAryOperation, IWhereClauseParser {
        private string _cartridgeSchema;
        private int _minMass;
        private int _maxMass;

        public string CartridgeSchema {
            get {
                return _cartridgeSchema;
            }
            set {
                _cartridgeSchema = value;
            }
        }

        public int MinMass {
            get {
                return _minMass;
            }
            set {
                _minMass = value;
            }
        }

        public int MaxMass {
            get {
                return _maxMass;
            }
            set {
                _maxMass = value;
            }
        }

        #region WhereClauseItem Members
        protected override string GetDependantString(DBMSType databaseType, List<CambridgeSoft.COE.Framework.Common.SqlGenerator.Value> values) {
            if(databaseType != DBMSType.ORACLE) {
                throw new Exception("This clause only works in Oracle implementations");
            }

            StringBuilder builder = new StringBuilder(_cartridgeSchema);
            builder.Append(".MolWeightContains(");
            builder.Append(this.dataField.GetFullyQualifiedNameString());
            builder.Append(", ");

            builder.Append(this.ParameterHolder);
            if(this.UseParametersByName)
                builder.Append(values.Count);
            values.Add(new Value(_minMass.ToString(), System.Data.DbType.Double));

            builder.Append(", ");

            builder.Append(this.ParameterHolder);
            if(this.UseParametersByName)
                builder.Append(values.Count);
            values.Add(new Value(_maxMass.ToString(), System.Data.DbType.Double));

            builder.Append(", ");

            builder.Append(this.ParameterHolder);
            if(this.UseParametersByName)
                builder.Append(values.Count);
            values.Add(new Value("", System.Data.DbType.String));

            builder.Append(")=1");

            return builder.ToString();
        }
        #endregion

        #region IWhereClauseParser Members
        public WhereClauseItem CreateInstance(System.Xml.XmlNode criteriaXmlNode, Field dataField) {
            // <customCriteria Value="50-101">
            //    <customMolWeight xmlns="COE.SearchCriteria">
            //        <implementation>CsCartridge</implementation>
            //        <cartridgeSchema>CSCartridge</cartridgeSchema>
            //    </customMolWeight>
            //</customCriteria>
            MolWeightWhereClause item = new MolWeightWhereClause();
            XmlNamespaceManager manager = new XmlNamespaceManager(criteriaXmlNode.OwnerDocument.NameTable);
            manager.AddNamespace("COE", "COE.SearchCriteria");
            item.CartridgeSchema = criteriaXmlNode.SelectSingleNode("./COE:customMolWeight/COE:cartridgeSchema", manager).InnerText;
            item.DataField = dataField;
            ParseValue(criteriaXmlNode.Attributes["Value"], item);
            return item;
        }
        #endregion

        #region Private methods
        private void ParseValue(XmlAttribute xmlAttribute, MolWeightWhereClause item) {
            if(xmlAttribute != null && !string.IsNullOrEmpty(xmlAttribute.Value)) {
                string[] values = xmlAttribute.Value.Trim().Split("-".ToCharArray());
                item.MinMass = int.Parse(values[0]);
                if(values.Length > 1 && !string.IsNullOrEmpty(values[1]))
                    item.MaxMass = int.Parse(values[1]);
                else {
                    item.MaxMass = MinMass;
                }
            }
        }
        #endregion
    }
}
