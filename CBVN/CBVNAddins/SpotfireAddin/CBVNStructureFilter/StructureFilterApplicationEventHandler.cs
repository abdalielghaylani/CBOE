// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureFilterApplicationEventHandler.cs" company="PerkinElmer Inc.">
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

using Spotfire.Dxp.Application;
using Spotfire.Dxp.Application.Extension;

namespace CBVNStructureFilter
{
    class StructureFilterApplicationEventHandler : CustomApplicationEventHandler
    {
        protected override void OnApplicationInstanceCreated(AnalysisApplication application)
        {
            base.OnApplicationInstanceCreated(application);

            application.DocumentClosing += (o, e) =>
            {
                var app = o as AnalysisApplication;
                if (app != null && app.Document != null)
                {
                    ClearFilter();
                }
            };

            application.DocumentSaving += (o, e) =>
            {
                var app = o as AnalysisApplication;
                if (app != null && app.Document != null)
                {
                    SaveCoreChemistryClr(app.Document);
                    SaveSavedSearches(app.Document);
                }
            };

            application.DocumentChanged += (o, e) =>
            {
                var app = o as AnalysisApplication;
                if (app != null && app.Document != null)
                {
                    ApplyFilter(app);
                }
            };

            // If you open Spotfire by launching a dxp file, this function isn't 
            // called until after the document has been loaded so the DocumentChanged
            // event isn't fired and the filtering isn't applied.  It's also the only
            // scenario where we can get here without application.Document being null so
            // apply the filter
            if (application.Document != null)
            {
                ApplyFilter(application);
            }
        }

        private static StructureFilterPanel FindPanel(AnalysisApplication app)
        { 
            // Check for any filter panels
            foreach (var page in app.Document.Pages)
            {
                StructureFilterPanel panel;
                if (page.Panels.TryGetPanel(out panel))
                {
                    return panel;
                }
            }
            return null;
        }

        private static void ApplyFilter(AnalysisApplication app)
        {
            var panel = FindPanel(app);
            if (panel == null)
            {
                // this means the panel has never been opened with this file, so there is nothing to do
                return;
            }

            // ensure structure control constructed, note we are adding a panel if it has been closed
            panel.Visible = true;
            //DataTableInfoMgr.LoadCoreChemistryClr(app.Document);
            DataTableInfoMgr.LoadSavedSearches(app.Document);

            foreach (var table in app.Document.Data.Tables)
            {
                panel.ApplyFilter(table);
            }
        }

        private static void ClearFilter()
        {
            DataTableInfoMgr.Clear();
        }

        private static void SaveCoreChemistryClr(Document doc)
        {
            //DataTableInfoMgr.SaveCoreChemistryClr(doc);
        }

        private static void SaveSavedSearches(Document doc)
        {
            DataTableInfoMgr.SaveSavedSearches(doc);
        }
    }
}
