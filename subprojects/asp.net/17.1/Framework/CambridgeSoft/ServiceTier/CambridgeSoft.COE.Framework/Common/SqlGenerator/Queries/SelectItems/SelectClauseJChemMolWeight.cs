// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectClauseJChemMolWeight.cs" company="PerkinElmer Inc.">
//   Copyright (c) 2013 PerkinElmer Inc.,
//   940 Winter Street, Waltham, MA 02451.
//   All rights reserved.
//   This software is the confidential and proprietary information
//   of PerkinElmer Inc. ("Confidential Information"). You shall not
//   disclose such Confidential Information and may not use it in any way,
//   absent an express written license agreement between you and PerkinElmer Inc.
//   that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// This class to generate a "jc_molweight(field)" SQL Statement for JChem Cartridge.
    /// </summary>
    internal class SelectClauseJChemMolWeight : SelectClauseItem, ISelectClauseParser
    {
        #region Properties

        /// <summary>
        /// The field that contains the alias for the molweight field.
        /// </summary>
        public override string Alias
        {
            get
            {
                if (string.IsNullOrEmpty(base.Alias))
                {
                    base.Alias = "Mol Weight";
                }

                return base.Alias;
            }

            set
            {
                base.Alias = value;
            }
        }

        /// <summary>
        /// Filed name or alias
        /// </summary>
        public override string Name
        {
            get
            {
                if (this.Alias != null && this.Alias.Trim() != string.Empty)
                {
                    return this.Alias;
                }

                return DataField.GetNameString();
            }
        }

        #endregion

        #region ISelectClauseParser Members

        /// <summary>
        /// Implementation of ISelectClauseParser. Returns an Instance of SelectClauseMolWeight according to the desired xml snippet.
        /// </summary>
        /// <param name="resultsXmlNode">
        /// The results Xml Node.
        /// </param>
        /// <param name="dvnLookup">
        /// The INamesLookup interface from which the parser can obtain the names corresponding to ids in dataview.xml
        /// </param>
        /// <returns>
        /// An instance of SelectClauseMolWeight.
        /// </returns>
        public SelectClauseItem CreateInstance(System.Xml.XmlNode resultsXmlNode, MetaData.INamesLookup dvnLookup)
        {
            if (resultsXmlNode.Attributes == null)
            {
                throw new ArgumentException(@"fieldNode.Attributes can't be null!", "resultsXmlNode");
            }

            var item = new SelectClauseJChemMolWeight();

            if (resultsXmlNode.Attributes["alias"] != null)
            {
                item.Alias = resultsXmlNode.Attributes["alias"].Value;
            }

            if (resultsXmlNode.Attributes["visible"] != null &&
                resultsXmlNode.Attributes["visible"].Value != string.Empty)
            {
                this.Visible = bool.Parse(resultsXmlNode.Attributes["visible"].Value);
            }

            item.dataField = dvnLookup.GetColumn(int.Parse(resultsXmlNode.Attributes["fieldId"].Value.Trim()));

            return item;
        }

        #endregion

        #region Methods

        /// <summary>
        /// In this case returns the JChem Cartridge molweight syntax.
        /// </summary>
        /// <param name="dataBaseType">
        /// The database type.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// A string containing the select part corresponding to this clause (i.e. the jc_molweight(fieldName))
        /// </returns>
        protected override string GetDependantString(DBMSType dataBaseType, List<Value> values)
        {
            if (dataBaseType != DBMSType.ORACLE)
            {
                throw new Exception("This select clause is only working in oracle implementations.");
            }

            var builder = new StringBuilder();

            // jc_molweight will throw exception if the field is NULL, 
            // so need to use NVL function to convert it to empty if it is null, and then calculate the molweight.
            builder.AppendFormat("jc_molweight(NVL({0},''))", DataField.GetFullyQualifiedNameString());            

            return builder.ToString();
        }

        #endregion
    }
}
