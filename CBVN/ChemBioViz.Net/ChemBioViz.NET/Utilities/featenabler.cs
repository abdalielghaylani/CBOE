using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FormDBLib;
using CBVUtilities;

namespace ChemBioViz.NET
{
    /// <summary>
    ///   Feature Enabler: turns off features as specified in app settings
    /// </summary>
    public class FeatEnabler
    {
        #region Variables
        public enum Feature {   Plotting = 0x01, GrandChild = 0x02, DrillDown = 0x04, TreeFolder = 0x08,
                                ActionMenus = 0x10, QueryTextBox = 0x20, SubformQueryBoxes = 0x40,
                                CardView = 0x80, Export = 0x100, 
                                ComboBoxes = 0x200, SearchOverCurrList = 0x400, // disabled for 11.0.2
                                Addins = 0x800, NumericUnits = 0x1000
        };
        #endregion

        #region Methods
        //---------------------------------------------------------------------
        public bool IsEnabled(Feature f) 
        {
            uint dbits = Properties.Settings.Default.FeatsDisabled;
            return (dbits == 0)? true : (dbits & (uint)f) == 0;
        }
        //---------------------------------------------------------------------
        public bool CanPlot()
        {
            return IsEnabled(Feature.Plotting);
        }
        //---------------------------------------------------------------------
        public bool CanCreateGrandchildForm()
        {
            return IsEnabled(Feature.GrandChild);
        }
        //---------------------------------------------------------------------
        public bool CanCreateDrillDownForm()
        {
            return IsEnabled(Feature.DrillDown);
        }
        //---------------------------------------------------------------------
        public bool CanCreateTreeFolder()
        {
            return IsEnabled(Feature.TreeFolder);
        }
        //---------------------------------------------------------------------
        public bool CanCreateActionMenus()
        {
            return IsEnabled(Feature.ActionMenus);
        }
        //---------------------------------------------------------------------
        public bool CanCreateQueryTextBox()
        {
            return IsEnabled(Feature.QueryTextBox);
        }
        //---------------------------------------------------------------------
        public bool CanCreateSubformQueryBoxes()
        {
            return IsEnabled(Feature.SubformQueryBoxes);
        }
        //---------------------------------------------------------------------
        public bool CanCreateCardView()
        {
            return IsEnabled(Feature.CardView);
        }
        //---------------------------------------------------------------------
        public bool CanExport()
        {
            return IsEnabled(Feature.Export);
        }
        //---------------------------------------------------------------------
        public bool CanCreateComboBoxes()
        {
            return IsEnabled(Feature.ComboBoxes);
        }
        //---------------------------------------------------------------------
        public bool CanSearchOverCurrList()
        {
            return IsEnabled(Feature.SearchOverCurrList);
        }
        //---------------------------------------------------------------------
        public bool CanUseAddins()
        {
            return IsEnabled(Feature.Addins);
        }
        //---------------------------------------------------------------------
        public bool CanUseNumericUnits()
        {
            return IsEnabled(Feature.NumericUnits);
        }
        //---------------------------------------------------------------------
        #endregion

        #region Constructors
        public FeatEnabler()
        {
        }
        //---------------------------------------------------------------------
        #endregion
    }
}
