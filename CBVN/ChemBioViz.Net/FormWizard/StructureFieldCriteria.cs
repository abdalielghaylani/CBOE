using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;
using Spotfire.Dxp.Application;
using CBVNStructureFilter;

namespace FormWizard
{
    public partial class StructureFieldCriteria : UserControl
    {
        #region Variables

        /// <summary>
        /// Variable holds an object of the class AnalysisApplication
        /// </summary>
        AnalysisApplication _theAnalysisApplication;

        /// <summary>
        /// ariable hodls an object of the class StructureSearchModel
        /// </summary>
        StructureSearchModel _theStructureFilterModel;

        /// <summary>
        /// Variable holds an object of class SearchCriteria.ISearchCriteriaBase
        /// </summary>
        private SearchCriteria.ISearchCriteriaBase _structureFieldCriteria;

        /// <summary>
        /// Variable holds an selected Structure Field for Structure search
        /// </summary>
        private ComboInfo _selectedStructureField;

        /// <summary>
        /// Variable holds the StructureFilterPanelControlBase class object
        /// </summary>
        public StructureFilterPanelControlBase theStructureFilterPanelControlBase;

        #endregion

        #region Properties

        /// <summary>
        /// Get/Set selected Structure Field object in Structure Field ComboBox
        /// </summary>
        public ComboInfo SelectedStructureField
        {
            get
            {
                _selectedStructureField = (ComboInfo)SFComboBox.SelectedItem;
                return _selectedStructureField;
            }
            set
            {
                _selectedStructureField = value;
                if (_selectedStructureField != null)
                {
                    bool isStructureFieldAdded = false;
                    for (int iCoutnter = 0; iCoutnter < SFComboBox.Items.Count; iCoutnter++)
                    {
                        ComboInfo cmbItem = SFComboBox.Items[iCoutnter] as ComboInfo;
                        ComboInfo cmbItemKey = cmbItem.Key as ComboInfo;
                        ComboInfo cmbItemKeySelected = _selectedStructureField.Key as ComboInfo;
                        if ((int)cmbItemKey.Key == (int)cmbItemKeySelected.Key && (int)cmbItemKey.Value == (int)cmbItemKeySelected.Value)
                        {
                            SFComboBox.SelectedIndex = iCoutnter;
                            isStructureFieldAdded = true;
                            break;
                        }
                    }
                    if (!isStructureFieldAdded)
                    {
                        this.SFComboBox.DisplayMember = "Value";
                        this.SFComboBox.ValueMember = "Key";
                        this.SFComboBox.Items.Add(_selectedStructureField);
                        SFComboBox.SelectedIndex = SFComboBox.Items.Count - 1;
                    }
                    if (theStructureFilterPanelControlBase != null)
                        theStructureFilterPanelControlBase.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Get/Set Structure Field criteria
        /// </summary>
        public SearchCriteria.ISearchCriteriaBase SFCriteria
        {
            get
            {
                SearchCriteria.StructureCriteria _structureFieldCriteriaTemp = (_structureFieldCriteria as SearchCriteria.StructureCriteria);

                if (_structureFieldCriteriaTemp == null)
                {
                    _structureFieldCriteriaTemp = new SearchCriteria.StructureCriteria();
                }

                string _structure = string.Empty;
                if (_theStructureFilterModel != null && _theStructureFilterModel.StructureString != null)
                {
                    if (_theStructureFilterModel.StructureString.Contains("ChemDraw"))
                    {
                        _structure = Convert.ToBase64String(_theStructureFilterModel.CdxStructure);
                    }
                    else
                    {
                        _structure = _theStructureFilterModel.StructureString;
                    }

                    if (!string.IsNullOrEmpty(_structure))
                    {
                        _structureFieldCriteriaTemp.Implementation = "cscartridge";
                        _structureFieldCriteriaTemp.Structure = _structure;
                        if (_theStructureFilterModel.SearchType == StructureFilterSettings.FilterModeEnum.FullStructure)
                        {
                            _structureFieldCriteriaTemp.FullSearch = SearchCriteria.COEBoolean.Yes;
                            _structureFieldCriteriaTemp.Similar = SearchCriteria.COEBoolean.No;
                            _structureFieldCriteriaTemp.SimThreshold = 0;
                        }
                        else if (_theStructureFilterModel.SearchType == StructureFilterSettings.FilterModeEnum.Simularity)
                        {
                            _structureFieldCriteriaTemp.FullSearch = SearchCriteria.COEBoolean.No;
                            _structureFieldCriteriaTemp.Similar = SearchCriteria.COEBoolean.Yes;
                            _structureFieldCriteriaTemp.SimThreshold = _theStructureFilterModel.PercentSimilarity;
                        }
                        else
                        {
                            _structureFieldCriteriaTemp.FullSearch = SearchCriteria.COEBoolean.No;
                            _structureFieldCriteriaTemp.Similar = SearchCriteria.COEBoolean.No;
                            _structureFieldCriteriaTemp.SimThreshold = 0;
                        }
                        _structureFieldCriteria = _structureFieldCriteriaTemp;
                    }
                    else
                    {
                        _structureFieldCriteria = null;
                    }
                }
                return _structureFieldCriteria;
            }
            set
            {
                _structureFieldCriteria = value;
            }
        }

        #endregion

        #region Consturctor

        /// <summary>
        /// Initializes an instance of the class StructureFieldCriteria
        /// </summary>
        /// <param name="theAnalysisApplication">An object of AnalysisApplication class</param>
        public StructureFieldCriteria(AnalysisApplication theAnalysisApplication)
        {
            InitializeComponent();
            this.Load += new EventHandler(StructureFieldCriteria_Load);
            this._theAnalysisApplication = theAnalysisApplication;
        }

        #endregion

        #region Events

        void StructureFieldCriteria_Load(object sender, EventArgs e)
        {
            if (_theStructureFilterModel == null)
            {
                _theStructureFilterModel = new StructureSearchModel(this._theAnalysisApplication);
                theStructureFilterPanelControlBase = new StructureFilterPanelControlBase(_theStructureFilterModel, _theStructureFilterModel, _theStructureFilterModel);
                theStructureFilterPanelControlBase.HideControls();
                theStructureFilterPanelControlBase.Dock = DockStyle.Fill;
                tableLayoutPanel1.Controls.Add(theStructureFilterPanelControlBase, 0, 1);
            }

            if (theStructureFilterPanelControlBase != null)
            {
                if (this.SFComboBox.Items.Count > 0)
                {
                    theStructureFilterPanelControlBase.Enabled = true;
                }
                else
                {
                    theStructureFilterPanelControlBase.Enabled = false;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method for filling the list of available Structure Fields into a Structure Field ComboBox.
        /// </summary>
        public void FillAvailableStructureFields(List<ComboInfo> _availableStructureFields)
        {
            try
            {
                this.SFComboBox.DisplayMember = "Value";
                this.SFComboBox.ValueMember = "Key";

                this.SFComboBox.Items.Clear();
                foreach (ComboInfo _stuctureField in _availableStructureFields)
                {
                    this.SFComboBox.Items.Add(_stuctureField);
                }

                if (this.SFComboBox.Items.Count > 0)
                {
                    this.SFComboBox.SelectedIndex = 0;
                }
                else
                {
                    this.SFComboBox.SelectedItem = null;
                }

                if (theStructureFilterPanelControlBase != null)
                {

                    if (this.SFComboBox.Items.Count > 0)
                    {
                        theStructureFilterPanelControlBase.Enabled = true;
                    }
                    else
                    {
                        theStructureFilterPanelControlBase.Enabled = false;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Method for setting last saved Structure criteria to Structure search usercontrol
        /// </summary>
        public void SetStructureCriteria()
        {
            try
            {
                if (_theStructureFilterModel == null)
                {
                    _theStructureFilterModel = new StructureSearchModel(this._theAnalysisApplication);

                    //Setting saved structure criteria information on GUI
                    if (_structureFieldCriteria != null && !string.IsNullOrEmpty(_structureFieldCriteria.Value))
                    {
                        if (_structureFieldCriteria.Value.StartsWith("VmpDRD"))
                        {
                            this._theStructureFilterModel.CdxStructure = Convert.FromBase64String(_structureFieldCriteria.Value);
                        }
                        this._theStructureFilterModel.StructureString = _structureFieldCriteria.Value;

                        if ((_structureFieldCriteria as SearchCriteria.StructureCriteria).FullSearch == SearchCriteria.COEBoolean.Yes)
                        {
                            this._theStructureFilterModel.SearchType = StructureFilterSettings.FilterModeEnum.FullStructure;
                        }
                        else if ((_structureFieldCriteria as SearchCriteria.StructureCriteria).Similar == SearchCriteria.COEBoolean.Yes)
                        {
                            this._theStructureFilterModel.SearchType = StructureFilterSettings.FilterModeEnum.Simularity;
                            this._theStructureFilterModel.PercentSimilarity = (_structureFieldCriteria as SearchCriteria.StructureCriteria).SimThreshold;
                        }
                        else
                        {
                            this._theStructureFilterModel.SearchType = StructureFilterSettings.FilterModeEnum.SubStructure;
                        }
                    }


                    theStructureFilterPanelControlBase = new StructureFilterPanelControlBase(_theStructureFilterModel, _theStructureFilterModel, _theStructureFilterModel);
                    theStructureFilterPanelControlBase.HideControls();
                    theStructureFilterPanelControlBase.Dock = DockStyle.Fill;
                    tableLayoutPanel1.Controls.Add(theStructureFilterPanelControlBase, 0, 1);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Method for clearing Structure Filter
        /// </summary>
        public void ClearStructureFilter()
        {
            try
            {
                if (theStructureFilterPanelControlBase != null && _theStructureFilterModel != null)
                {
                    _theStructureFilterModel.ClearCurrentFilter();
                    theStructureFilterPanelControlBase.ClearStructureFilter();
                }
            }
            catch
            {
                throw;
            }
        }

        #endregion

        private void SFComboBox_DropDown(object sender, EventArgs e)
        {
            try
            {
                ComboBox senderComboBox = (ComboBox)sender;
                int width = 243; // senderComboBox.DropDownWidth;
                //CID:20645
                using (System.Drawing.Graphics g = senderComboBox.CreateGraphics())
                {
                    using (System.Drawing.Font font = senderComboBox.Font)
                    {
                        int vertScrollBarWidth =
                            (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems)
                            ? SystemInformation.VerticalScrollBarWidth : 0;

                        int newWidth;
                        foreach (ComboInfo s in ((ComboBox)sender).Items)
                        {
                            newWidth = (int)g.MeasureString(s.Value.ToString().Trim(), font).Width
                               + vertScrollBarWidth;
                            if (width < newWidth)
                            {
                                width = newWidth;
                            }
                        }
                        senderComboBox.DropDownWidth = width;
                    }
                }
            }
            catch
            {
                //Do nothing
            }
        }
    }


    /// <summary>
    /// A class for Filling a Display Text and Value in ComboBox 
    /// </summary>
    public class ComboInfo
    {
        public object Key { get; set; }
        public object Value { get; set; }
    }
}
