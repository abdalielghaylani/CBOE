// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimilarityModel.cs" company="PerkinElmer Inc.">
//   Copyright © 2012 PerkinElmer Inc. 
// 100 CambridgePark Drive, Cambridge, MA 02140. 
// All rights reserved. 
// This software is the confidential and proprietary information 
// of PerkinElmer Inc. ("Confidential Information"). You shall not 
// disclose such Confidential Information and may not use it in any way, 
// absent an express written license agreement between you and PerkinElmer Inc. 
// that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CBVNStructureFilter
{
    using System.Collections.Generic;

    using Spotfire.Dxp.Data.Import;
    using CBVNStructureFilter.Properties;
    using CBVNStructureFilter;
    using CBVNStructureFilterSupport.Framework;

    /// <summary>
    /// The base model object for similarity search parameters.
    /// </summary>
    internal class SimilarityModel : StructureSearchModel
    {
        #region Constants and Fields

        /// <summary>The lower bound.
        /// </summary>
        private int lowerBound;

        /// <summary>The upper bound.
        /// </summary>
        private int upperBound;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="SimilarityModel"/> class.
        /// </summary>
        public SimilarityModel()
        {
            this.SetBounds(75, 100);
        }

        #endregion

        #region Properties

        /// <summary>Gets the lower bound.
        /// </summary>
        /// <value>The lower bound.</value>
        public int LowerBound
        {
            get
            {
                return this.lowerBound;
            }
        }

        /// <summary>Gets the upper bound.
        /// </summary>
        /// <value>The upper bound.</value>
        public int UpperBound
        {
            get
            {
                return this.upperBound;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Populates the information link parameters.
        /// </summary>
        /// <param name="searchLink">The search link.</param>
        /// <param name="informationLinkParameters">The information link parameters.</param>
        public override void PopulateInformationLinkParameters(
            InformationLinkDescriptor searchLink, ICollection<InformationLinkParameter> informationLinkParameters)
        {
            base.PopulateInformationLinkParameters(searchLink, informationLinkParameters);

            informationLinkParameters.Add(
                InformationLinkParameter.CreateNamedParameter(
                    Identifiers.ILSimilarityLowerParameter, new object[] { this.lowerBound }));

            informationLinkParameters.Add(
                InformationLinkParameter.CreateNamedParameter(
                    Identifiers.ILSimilarityUpperParameter, new object[] { this.upperBound }));
        }

        /// <summary>Sets the bounds.
        /// </summary>
        /// <param name="newLowerBound">The new lower bound.</param>
        /// <param name="newUpperBound">The new upper bound.</param>
        public void SetBounds(int newLowerBound, int newUpperBound)
        {
            //Robustness.ValidateArgumentRange("newLowerBound", 0, newUpperBound, newLowerBound);
            //Robustness.ValidateArgumentRange("newUpperBound", newLowerBound, 100, newUpperBound);
            //this.lowerBound = newLowerBound;
            //this.upperBound = newUpperBound;
        }

        /// <summary>
        /// Gets the result list description.
        /// </summary>
        /// <param name="searchLink">The search link.</param>
        /// <param name="retrieveLink">The retrieve link.</param>
        /// <returns>
        /// The description for the information links used.
        /// </returns>
        public override string GetResultListDescription(InformationLinkDescriptor searchLink, InformationLinkDescriptor retrieveLink)
        {
            return string.Format(Resources.DefaultListDescriptionForSimilarity, searchLink.Path, retrieveLink.Path, this.lowerBound, this.upperBound);
        }

        #endregion
    }
}
