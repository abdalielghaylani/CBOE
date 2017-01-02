// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectClauseJChemFormula.cs" company="PerkinElmer Inc.">
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
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// This class to generate a "jc_formula(field)" SQL Statement for JChem Cartridge.
    /// </summary>
    internal class SelectClauseJChemFormula : SelectClauseItem, ISelectClauseParser
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectClauseJChemFormula"/> class to its default values. 
        /// </summary>
        public SelectClauseJChemFormula()
        {
            Sortable = false;
        }
        #endregion
        
        #region Properties

        /// <summary>
        /// The field that contains the alias for the formula field.
        /// </summary>
        public override string Alias
        {
            get
            {
                if (string.IsNullOrEmpty(base.Alias))
                {
                    base.Alias = "Formula";
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
                if (Alias != null && Alias.Trim() != string.Empty)
                {
                    return Alias;
                }

                return DataField.GetNameString();
            }
        }

        public bool Sortable { get; set; }

        #endregion

        #region ISelectClauseParser Members

        /// <summary>
        /// Implementation of ISelectClauseParser. Returns an Instance of SelectClauseFormula according to the desired xml snippet.
        /// </summary>
        /// <param name="resultsXmlNode">
        /// The results Xml Node.
        /// </param>
        /// <param name="dvnLookup">
        /// The INamesLookup interface from which the parser can obtain the names corresponding to ids in dataview.xml
        /// </param>
        /// <returns>
        /// An instance of SelectClauseFormula.
        /// </returns>
        public SelectClauseItem CreateInstance(System.Xml.XmlNode resultsXmlNode, MetaData.INamesLookup dvnLookup)
        {
            if (resultsXmlNode.Attributes == null)
            {
                throw new ArgumentException(@"resultsXmlNode.Attributes can't be null!", "resultsXmlNode");
            }

            var item = new SelectClauseJChemFormula();

            Debug.Assert(resultsXmlNode.Attributes != null, "resultsXmlNode.Attributes != null");

            if (resultsXmlNode.Attributes["alias"] != null)
            {
                item.Alias = resultsXmlNode.Attributes["alias"].Value;
            }

            if (resultsXmlNode.Attributes["sortable"] != null)
            {
                item.Sortable = bool.Parse(resultsXmlNode.Attributes["sortable"].Value);
            }

            if (resultsXmlNode.Attributes["visible"] != null &&
                resultsXmlNode.Attributes["visible"].Value != string.Empty)
            {
                item.Visible = bool.Parse(resultsXmlNode.Attributes["visible"].Value);
            }

            item.dataField = dvnLookup.GetColumn(int.Parse(resultsXmlNode.Attributes["fieldId"].Value.Trim()));

            return item;
        }

        #endregion
        
        #region Methods
        /// <summary>
        /// This method does the actual job.
        /// In this case returns the JChem Cartridge formula syntax.
        /// </summary>
        /// <param name="dataBaseType">
        /// The database type.
        /// </param>
        /// <param name="values">
        /// The parameters for SQL command.
        /// </param>
        /// <returns>
        /// A string containing the select part corresponding to this clause (i.e. the jc_formula(fieldName))
        /// </returns>
        protected override string GetDependantString(DBMSType dataBaseType, List<Value> values)
        {
            if (dataBaseType != DBMSType.ORACLE)
            {
                throw new Exception("This select clause is only working in oracle implementations.");
            }

            var builder = new StringBuilder("jc_formula(");
            builder.Append(DataField.GetFullyQualifiedNameString());
            builder.Append(")");

            return builder.ToString();
        }
        #endregion
    }
}
