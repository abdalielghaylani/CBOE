// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectClauseDirectStructure.cs" company="PerkinElmer Inc.">
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
    using System.Xml;

    using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData;

    /// <summary>
    /// This class to generate a "molfile(field)" SQL Statement for Accelry structure type.
    /// </summary>
    public class SelectClauseDirectStructure : SelectClauseItem, ISelectClauseParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectClauseDirectStructure"/> class.
        /// </summary>
        public SelectClauseDirectStructure()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return this.DataField.GetNameString();
            }
        }

        #endregion

        #region ISelectClauseParser Members

        /// <summary>
        /// Implementation of ISelectClauseParser. Returns an Instance of SelectClauseField, which at this point
        /// has only set its id.
        /// </summary>
        /// <param name="fieldNode">The field node of the search results xml definition.</param>
        /// <param name="dvnLookup">The INamesLookup interface from which the parser can obtain the names corresponding to ids in dataview.xml</param>
        /// <returns>An instance of SelectClauseField.</returns>
        public SelectClauseItem CreateInstance(XmlNode fieldNode, INamesLookup dvnLookup)
        {
            if (fieldNode.Attributes == null)
            {
                throw new ArgumentException(@"fieldNode.Attributes can't be null!", "fieldNode");
            }

            var item = new SelectClauseDirectStructure();
            if (fieldNode.Attributes["alias"] != null && fieldNode.Attributes["alias"].Value != string.Empty)
            {
                item.Alias = fieldNode.Attributes["alias"].Value;
            }
            else
            {
                item.Alias = dvnLookup.GetColumnAlias(int.Parse(fieldNode.Attributes["fieldId"].Value.Trim()));
            }

            if (fieldNode.Attributes["visible"] != null && fieldNode.Attributes["visible"].Value != string.Empty)
            {
                item.Visible = bool.Parse(fieldNode.Attributes["visible"].Value);
            }

            var lookupField = dvnLookup.GetColumn(int.Parse(fieldNode.Attributes["fieldId"].Value.Trim()));
            if (lookupField != null)
            {
                item.DataField = lookupField;
            }

            return item;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Return the string to query the field. A Direct Cartridge operator will be added to
        /// convert CTAB structure to molfile.
        /// </summary>
        /// <param name="dataBaseType">
        /// The data base type.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetDependantString(DBMSType dataBaseType, List<Value> values)
        {
            return string.Format("molfile({0})", this.dataField.GetFullyQualifiedNameString());
        }

        #endregion
    }
}
