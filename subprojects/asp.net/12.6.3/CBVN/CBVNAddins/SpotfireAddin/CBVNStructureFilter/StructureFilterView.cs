// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureFilterView.cs" company="PerkinElmer Inc.">
// Copyright © 2013 PerkinElmer Inc. 
// 940 Winter Street, Waltham, MA 02451.
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
    using Spotfire.Dxp.Framework;
    using CBVNStructureFilterSupport.Framework;

    interface IStructureFilterView
    {
        StructureControlBase StructureControl { get; }
        bool RGroupDecomposition { get; set; }
        string PercentLabel { get; set; }
        string SimPercent { get; }

        void SetImage();
    }
}
