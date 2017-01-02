// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureFilterPanelControlBase.cs" company="PerkinElmer Inc.">
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

namespace CBVNStructureFilter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using System.Globalization;
    using System.Drawing;
    using log4net;

    using Spotfire.Dxp.Framework;
    using Spotfire.Dxp.Framework.License;
    using Spotfire.Dxp.Framework.Preferences;
    using Properties;
    using CBVNStructureFilterSupport.ChemDraw;
    using CBVNStructureFilterSupport.Framework;
    using CBVNStructureFilterSupport.EditorBase;
    using CBVNStructureFilterSupport;

    /// <summary>
    /// 
    /// </summary>
    public partial class StructureFilterPanelControlBase : UserControl, IStructureFilterView
    {
        private enum ToolStripButtonState
        {
            Default,
            EnableOnlyConfigButton
        }

        private const int VerticalAdjustment = 10;
        private const int ScrollBarAdjustment = 22;
        private const int HeightAdjustment = 5;
        private const int EditorButtonIndex = 0;
        private const int RendererButtonIndex = 2;

        protected static readonly ILog Log = LogManager.GetLogger(typeof(StructureFilterPanelControl));

        protected readonly StructureFilterPresenter Presenter;
        private StructureControlBase _structureControl;
        private bool _moleculeImageSizing;
        private bool _updatingFilterSettings;
        /// <summary>
        /// Current preferences
        /// </summary>
        private readonly StructureFilterPreference _preferences;
        private readonly ToolTip _toolTip = new ToolTip();
        private bool _simPercentChanging = false;

        protected Guid TimerTable;
        protected Guid ProgressTable;

        // IStructureFilterView properties

        /// <summary>
        /// 
        /// </summary>
        public StructureControlBase StructureControl
        {
            get
            {
                if (_structureControl == null)
                {
                    // Default to ChemDraw if preferences are not yet available
                    _structureControl = StructureControlFactory.Create(_preferences == null ? ControlType.ChemDraw : _preferences.RendererType, true);
                    if (_structureControl == null)
                    {
                        return null;
                    }
                    _structureControl.Init(null);
                    _structureControl.HydrogenDisplayMode = Presenter.HydrogenDisplayMode;
                }

                return _structureControl;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SimPercent
        {
            get { return simPercent.Value.ToString(CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string PercentLabel
        {
            get { return percentLabel.Text; }
            set { percentLabel.Text = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RGroupDecomposition
        {
            get { return rGroupDecomposition.Checked; }
            set { rGroupDecomposition.Checked = value; }
        }

        public bool StructureAlignment
        {
            get { return structureAlignment.Checked; }
            set { structureAlignment.Checked = value; }
        }

        protected bool BusyTimerEnabled
        {
            get { return timer1.Enabled; }
            set { timer1.Enabled = value; }
        }

        internal StructureFilterPanelControlBase()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editModel"></param>
        /// <param name="searchModel"></param>
        /// <param name="filterModel"></param>
        public StructureFilterPanelControlBase(IStructureEditModel editModel, IStructureSearchModel searchModel, IStructureFilterModel filterModel)
        {
            CBVNStructureFilterSupport.GeneralClass.LDInstallationPath = GeneralClass.LDInstallationPath;

            InitializeComponent();

            Presenter = new StructureFilterPresenter(editModel, searchModel, filterModel, this);

            if (Presenter.NodeContext != null)
            {
                var preferenceManager = TryGetPreferenceManager();
                _preferences = preferenceManager != null && preferenceManager.PreferenceExists<StructureFilterPreference>()
                                   ? preferenceManager.GetPreference<StructureFilterPreference>()
                                   : new StructureFilterPreference();
                var dictionary = TryGetGlobalRenderSettings();
                foreach (var pair in dictionary)
                {
                    _preferences.SetRenderSettings(pair.Key, pair.Value.ChemDrawRenderSettings);
                    _preferences.SetRenderStyleFileName(pair.Key, pair.Value.ChemDrawStyleFileName);
                }
            }
            else
            {
                _preferences = new StructureFilterPreference();
            }

            InitializePreferences();
            InitializeRenderersMenu();
            InitializeEditorsMenus();
            InitializeSearchControls();

            SetImage();

            toolStrip1.Focus();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // If it's in edit mode set it back to enabled, the user is closing the panel
                // with the editor open, if they re-open the panel we want it enabled
                if (GetPanelState() == DataTableInfoMgr.DataTableInfo.SearchStateEnum.Editting)
                {
                    DataTableInfoMgr.SetSearchState(Presenter.ModelId,
                                                    DataTableInfoMgr.DataTableInfo.SearchStateEnum.Enabled);
                }

                if (components != null)
                {
                    components.Dispose();
                }

                if (_structureControl != null)
                {
                    _structureControl.Dispose();
                    _structureControl = null;
                }

                if (_preferences != null)
                {
                    _preferences.Save();
                }
            }

            base.Dispose(disposing);
        }

        private PreferenceManager TryGetPreferenceManager()
        {
            PreferenceManager preferenceManager = null;
            try
            {
                //CID:20271
                Spotfire.Dxp.Framework.DocumentModel.INodeContext nodeContext = Presenter.NodeContext;
                if (nodeContext != null)
                {
                    preferenceManager = nodeContext.GetService<PreferenceManager>();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return preferenceManager;
        }

        private Dictionary<string, StructureColumnRendererModel> TryGetGlobalRenderSettings()
        {
            var dictionary = new Dictionary<string, StructureColumnRendererModel>();
            try
            {
                dictionary =
                    RendererUtilities.GetGlobalRenderSettings(Presenter.NodeContext, false, null,
                                                              ChemDrawRendererIdentifiers.ChemDrawRenderer);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return dictionary;
        }

        private LicenseManager TryGetLicenseManager()
        {
            LicenseManager licenseManager = null;
            try
            {
                //CID:20270
                Spotfire.Dxp.Framework.DocumentModel.INodeContext nodeContext = Presenter.NodeContext;
                if (nodeContext != null)
                {
                    licenseManager = nodeContext.GetService<LicenseManager>();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return licenseManager;
        }

        internal void UpdateUIFromModelChanged(IStructureFilterModel filterModel,
                                               IStructureSearchModel searchModel, IStructureEditModel editModel)
        {
            if (_updatingFilterSettings)
            {
                return;
            }

            _updatingFilterSettings = true;
            try
            {
                SetSearchState(searchModel, editModel);

                // Set the search type
                foreach (SearchTypeItem item in searchOption.Items)
                {
                    if (item.Value == searchModel.SearchType)
                    {
                        searchOption.SelectedItem = item;
                        break;
                    }
                }

                rGroupDecomposition.Enabled = !string.IsNullOrEmpty(editModel.StructureString) &&
                                              searchOption.SelectedIndex == 1;

                structureAlignment.Enabled = !string.IsNullOrEmpty(editModel.StructureString);


                // Set the similarity search properties
                SetSimPercentFromModel(searchModel);
                // Set the R-Group properties
                SetRGDFromModel(filterModel);
                nicknames.Checked = filterModel.NickNames;

                _preferences.SetShowHydrogens(_preferences.RendererType, editModel.ShowHydrogens);

                // Update the structure control
                if (_structureControl != null)
                {
                    _structureControl.HydrogenDisplayMode = editModel.ShowHydrogens;
                    _structureControl.Refresh();
                    SetImage();
                }

                SetPanelState();

                // Update the list of saved searches
                UpdateSavedSearchesList();
            }
            finally
            {
                _updatingFilterSettings = false;
            }
        }

        internal void ShowEditor(ControlType controlType)
        {
            using (var editingHost = StructureControlFactory.GetEditingHost(controlType))
            {
                if (editingHost == null)
                {
                    return;
                }

                editingHost.Start();
                var originalState = GetPanelState();
                var originalId = Presenter.ModelId;

                // Display progressbar while loading the editor
                progressLabel.Text = Resources.LoadingEditor;
                DataTableInfoMgr.SetSearchState(originalId, DataTableInfoMgr.DataTableInfo.SearchStateEnum.Busy);
                button1.Visible = false;
                progressBar1.Value = 0;
                SetPanelState();

                try
                {
                    StructureStringType structureType;
                    var structureData = GetEditStructureString(controlType, out structureType);

                    editingHost.SendCommand(SetParameterCommandHandler.Command, Constants.StructureFormat,
                                            GetStructureTypeStringFromEnum(structureType));
                    if (Disposing || IsDisposed) return;    // After every command returns we need to make sure the user didn't close the panel
                    editingHost.SendCommand(SetParameterCommandHandler.Command, Constants.Structure, structureData);
                    if (Disposing || IsDisposed) return;
                    editingHost.SendCommand(SetParameterCommandHandler.Command, Constants.Handle, Handle.ToInt32());
                    if (Disposing || IsDisposed) return;

                    progressBar1.Value = 100;
                    // Disable the panel while the editor is open so the user can't do something stupid
                    // like launch the editor a second time
                    DataTableInfoMgr.SetSearchState(originalId, DataTableInfoMgr.DataTableInfo.SearchStateEnum.Editting);
                    SetPanelState();
                    editingHost.SendCommand(ShowEditorCommandHandler.Command);
                    if (Disposing || IsDisposed) return;
                    if (originalId != Guid.Empty)
                    {
                        DataTableInfoMgr.SetSearchState(originalId, originalState);
                    }
                    SetPanelState();

                    var result =
                        editingHost.SendCommand(GetResultCommandHandler.Command, Constants.DialogResult).First();
                    if (Disposing || IsDisposed) return;
                    DialogResult dialogResult;
                    if (!Enum.TryParse(result, true, out dialogResult))
                    {
                        dialogResult = DialogResult.Cancel;
                    }

                    if (dialogResult != DialogResult.OK)
                    {
                        return;
                    }

                    var editedStructure =
                        editingHost.SendCommand(GetResultCommandHandler.Command, Constants.Structure).First();
                    if (Disposing || IsDisposed) return;
                    // ChemDraw currently always returns CDX no matter what type of input is given
                    // if that behavior changes, this will break
                    if (controlType == ControlType.ChemDraw)
                    {
                        structureType = StructureStringType.CDX;
                    }

                    SetEditedStructure(structureType, editedStructure);
                }
                catch (ArgumentNullException ane)
                {
                    MessageBox.Show(controlType == ControlType.Marvin ? Resources.JRENotFound : ane.Message,
                                    Resources.Error, MessageBoxButtons.OK);
                }
                catch (Exception e)
                {
                    // Display the error message to the user
                    MessageBox.Show(e.Message, Resources.Error, MessageBoxButtons.OK);
                }
                finally
                {
                    editingHost.SendCommand(string.Empty);
                }
            }
        }

        private void DisplayEditOnly()
        {
            toolStrip1.Items.Remove(toolStripButtonClear);
            toolStrip1.Items.Remove(toolStripButtonConfigure);

            searchOption.Visible = false;
            simPercent.Visible = false;
            rGroupDecomposition.Visible = false;
            nicknames.Visible = false;
            percentLabel.Visible = false;
            percent.Visible = false;
            minLabel.Visible = false;
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            ClearStructureFilter();
        }

        /// <summary>
        /// Method for resetting Structure filter controls to default values
        /// </summary>
        public void ClearStructureFilter()
        {
            Presenter.ClearFilter();
            searchOption.SelectedIndex = 1; //Display SubStructure as by Default
            simPercent.Value = 0;
            percentLabel.Text = simPercent.Value.ToString(CultureInfo.InvariantCulture);
        }

        private void OnRendererClicked(object sender, EventArgs e)
        {
            var controlType = (ControlType)((ToolStripMenuItem)sender).Tag;

            if (_preferences.RendererType != controlType)
            {
                _preferences.RendererType = controlType;
                _preferences.Save();

                // force recreation of StructureControl
                _structureControl = null;
                SetImage();
            }

            CheckMenuItem(RendererButtonIndex, _preferences.RendererType);
        }

        private void OnEditClicked(object sender, EventArgs e)
        {
            var controlType = sender.GetType() == typeof(ToolStripMenuItem)
                                          ? (ControlType)((ToolStripMenuItem)sender).Tag
                                          : (ControlType)((ToolStripButton)sender).Tag;
            ShowEditor(controlType);
            if (_preferences.EditorType != controlType)
            {
                _preferences.EditorType = controlType;
                _preferences.Save();
            }

            CheckMenuItem(EditorButtonIndex, _preferences.EditorType);
        }

        private void OnConfigureClicked(object sender, EventArgs e)
        {
            string oldDataColumnReference = Presenter.GetDataColumnReference();
            Presenter.ShowStructureSourceConfigDialog();
            if (!oldDataColumnReference.Equals(Presenter.GetDataColumnReference()))
            {
                Presenter.ClearFilter();
            }
            SetPanelState();
        }

        private void OnPercentLabelKeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar);
            if (e.KeyChar != (char)Keys.Return)
            {
                return;
            }
            if (string.IsNullOrEmpty(percentLabel.Text))
            {
                percentLabel.Text = Resources.SimilarityPercent_0;
            }
            if (int.Parse(percentLabel.Text) > 100)
            {
                percentLabel.Text = Resources.SimilarityPercent_100;
            }

            simPercent.Value = int.Parse(percentLabel.Text);
            percentLabel.ReadOnly = true;
            percentLabel.Cursor = Cursors.Default;
            percentLabel.BackColor = ColorTranslator.FromHtml("#D2E8F6");
        }

        private void OnPercentLabelDoubleClicked(object sender, MouseEventArgs e)
        {
            percentLabel.ReadOnly = false;
            percentLabel.Cursor = Cursors.IBeam;
            percentLabel.BackColor = Color.White;
        }

        private void OnRGroupDecompositionChecked(object sender, EventArgs e)
        {
            UpdateRGroupSettings();

            showStructures.Enabled = rGroupDecomposition.Checked;
            nicknames.Enabled = rGroupDecomposition.Checked;
            MoleculeImage.Enabled = !rGroupDecomposition.Checked;
            searchOption.Enabled = !rGroupDecomposition.Checked;
            EnableToolStripButtons(ToolStripButtonState.Default);
            rgdPanel.Enabled = !rGroupDecomposition.Checked;
        }

        private void OnSettingsClicked(object sender, EventArgs e)
        {
            if (Presenter == null)
            {
                return;
            }

            var controlType = _preferences.RendererType;
            var showHydrogens = _preferences.GetShowHydrogens(controlType);

            // get the rendering settings from preferences
            StructureStringType structureType;
            GetEditStructureString(controlType, out structureType);
            var contentType = GetStructureTypeStringFromEnum(structureType);
            var renderSettings = _preferences.GetRenderSettings(contentType);
            var renderStyleFileName = _preferences.GetRenderStyleFileName(contentType);
            var dlg = new StructureColumnRendererSettings(
                controlType, StructureStringType.Molfile, showHydrogens, Presenter.GetModulesService(), renderStyleFileName);
            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            var cmRenderSettings = string.Empty;
            if (controlType == ControlType.ChemDraw)
            {
                var chemDrawRenderSettings = dlg.RenderSettings as ChemDrawRenderSettings;
                cmRenderSettings = ChemDrawUtilities.Base64SerializeChemDrawRenderSettings(chemDrawRenderSettings);
            }

            if (showHydrogens != dlg.ShowHydrogens || (!string.IsNullOrEmpty(cmRenderSettings) && !renderSettings.Equals(cmRenderSettings)))
            {
                //undo block    
                Presenter.ExecuteTransaction(
                    delegate
                    {
                        _preferences.SetShowHydrogens(controlType, dlg.ShowHydrogens);
                        Presenter.HydrogenDisplayMode = dlg.ShowHydrogens;

                        // set the rendering settings to preferences
                        if (!string.IsNullOrEmpty(cmRenderSettings))
                        {
                            _preferences.SetRenderSettings(contentType, cmRenderSettings);
                            _preferences.SetRenderStyleFileName(contentType, dlg.StyleFileName);
                        }

                        // update structureControl
                        _structureControl.HydrogenDisplayMode = dlg.ShowHydrogens;
                        _structureControl.Refresh();
                        SetImage();
                    });
            }
        }

        private void OnSimPercentValueChanged(object sender, EventArgs e)
        {
            percentLabel.Text = simPercent.Value.ToString(CultureInfo.InvariantCulture);
            UpdateSimularityPercent();
        }

        private void OnSimPercentMouseDown(object sender, MouseEventArgs e)
        {
            _simPercentChanging = true;
        }

        private void OnSimPercentMouseUp(object sender, MouseEventArgs e)
        {
            _simPercentChanging = false;
            UpdateSimularityPercent();
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            CancelOperation();
            Presenter.ClearFilter();
        }

        private void OnMoleculeImageSizeChanged(object sender, EventArgs e)
        {
            SetImageSize();
        }

        private void OnMoleculeImageDoubleClicked(object sender, EventArgs e)
        {
            if (GetPanelState() == DataTableInfoMgr.DataTableInfo.SearchStateEnum.Editting || rGroupDecomposition.Checked)
            {
                return;
            }

            var editor = GetDefaultEditor();
            if (editor != ControlType.None)
            {
                ShowEditor(editor);
            }
        }

        private void OnBusyTimerTick(object sender, EventArgs e)
        {
            if (TimerTable != Guid.Empty)
            {
                DataTableInfoMgr.SetSearchState(TimerTable, DataTableInfoMgr.DataTableInfo.SearchStateEnum.Busy);
                DisableTimer1();
                SetPanelState();
            }
        }

        private void OnProgressTimerTick(object sender, EventArgs e)
        {
            ProgressMethod();
        }

        private void OnSetFocus(object sender, EventArgs e)
        {
            Focus();
        }

        private void OnStructureFilterPanelControlEnter(object sender, EventArgs e)
        {
            ChangeBackgroundColor(true);
        }

        private void OnStructureFilterPanelControlLeave(object sender, EventArgs e)
        {
            ChangeBackgroundColor(false);
        }

        private void OnStructureFilterPanelControlResize(object sender, EventArgs e)
        {
            panel2.Height = Size.Height - toolStrip1.Height - HeightAdjustment;
            panel2.Width = Size.Width;
            searchTypePanel.Focus();
            SetImageSize();
        }

        private void OnSearchOptionSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updatingFilterSettings)
            {
                try
                {
                    _updatingFilterSettings = true;
                    Presenter.SearchType = ((SearchTypeItem)searchOption.SelectedItem).Value;
                }
                finally
                {
                    _updatingFilterSettings = false;
                }
            }

            SetComponentsVisible();
        }

        private void OnPreviousSearchesSelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedSearch = (SavedSearchSettings)previousSearches.SelectedItem;

            if (!HasSearchChanged(selectedSearch))
            {
                return;
            }

            SetFilterToSelectedSearch(selectedSearch);
        }

        private void UpdateSimularityPercent()
        {
            if (_updatingFilterSettings || _simPercentChanging)
            {
                return;
            }
            try
            {
                _updatingFilterSettings = true;
                Presenter.PercentSimilarity = Convert.ToInt32(simPercent.Value);
            }
            finally
            {
                _updatingFilterSettings = false;
            }
        }

        private bool HasSearchChanged(SavedSearchSettings selectedSearch)
        {
            if (Presenter.FilterModel.NickNames == selectedSearch.RGroupNicknames
                && Presenter.FilterModel.RGroupDecomposition == selectedSearch.RGroupDecomposition
                && Presenter.SearchType == selectedSearch.FilterMode
                && Presenter.SearchModel.PercentSimilarity == selectedSearch.SimularityPercent
                && Presenter.StructureString == selectedSearch.FilterStructure
                && ((StructureFilterModel)Presenter.FilterModel).DataColumnReference.Name == selectedSearch.StructureColumn)
            {
                return false;
            }
            return true;
        }

        private void SetFilterToSelectedSearch(SavedSearchSettings selectedSearch)
        {
            Presenter.ClearFilter();
            Presenter.SetFilterSettings(selectedSearch);
        }

        private void OnPreviousSearchesDrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= previousSearches.Items.Count)
            {
                return;
            }

            HistoricalDropDown.DrawItem(e, previousSearches, _structureControl);
        }

        private void OnPreviousSearchesMeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= previousSearches.Items.Count)
            {
                return;
            }

            e.ItemHeight = HistoricalDropDown.ImageHeight;
            e.ItemWidth = previousSearches.DropDownWidth;
        }

        private void OnNickNamesChecked(object sender, EventArgs e)
        {
            if (!_updatingFilterSettings)
            {
                try
                {
                    _updatingFilterSettings = true;

                    // Re-apply the R-Group decomposition
                    Presenter.RemoveRGroupDecompositionColumnsFromDocument();
                    Presenter.SetRGroupDecomposition(true, nicknames.Checked);
                }
                finally
                {
                    _updatingFilterSettings = false;
                }
            }
        }

        private void OnStructureAlignmentChecked(object sender, EventArgs e)
        {
            UpdateAlignmentSettings();
        }

        private void OnShowStructureMouseHover(object sender, EventArgs e)
        {
            _toolTip.SetToolTip(showStructures, Resources.ShowStructuresToolTip);
        }

        private void OnShowNickNamesMouseHover(object sender, EventArgs e)
        {
            _toolTip.SetToolTip(nicknames, Resources.ShowNicknamesToolTip);
        }

        private void InitializePreferences()
        {
            _preferences.RendererType = GetDefaultRenderer();
            _preferences.EditorType = GetDefaultEditor();
        }

        private void InitializeRenderersMenu()
        {
            var renderers = GetInstalledRenderers();
            var defaultRenderer = GetDefaultRenderer();

            if (renderers != null)
            {
                foreach (var renderer in renderers)
                {
                    var item =
                        new ToolStripMenuItem(
                            RendererIdentifierConverter.ToCustomTypeIdentifier(renderer.Key).DisplayName)
                            {
                                Enabled = true,
                                Checked = defaultRenderer == renderer.Key,
                                Tag = renderer.Key
                            };
                    item.Click += OnRendererClicked;
                    rendererToolStripMenuItem.DropDownItems.Add(item);
                }
            }

            settingsToolStripMenuItem.Enabled = true;
        }

        private void InitializeEditorsMenus()
        {
            // Create a list of the available editors 
            var renderers = GetInstalledRenderers();
            if (renderers == null)
            {
                return;
            }
            var editors = (from renderer in renderers where renderer.Value select renderer.Key).ToList();
            var defaultEditor = GetDefaultEditor();

            var editButton = (ToolStripDropDownButton)toolStrip1.Items[0];

            // If there are no editors available disable the buttons
            if (editors.Count <= 0)
            {
                toolStrip1.Items[EditorButtonIndex].Enabled = false;
            }
            // If there is only one editor remove the drop down button and replace it with a regular button
            else if (editors.Count == 1)
            {
                toolStrip1.Items.RemoveAt(EditorButtonIndex);

                var button = new ToolStripButton(Resources.New_Editor)
                    {
                        ToolTipText = Resources.EditButtonTooltip,
                        Tag = editors[0]
                    };
                button.Click += OnEditClicked;
                toolStrip1.Items.Insert(0, button);
                Log.InfoFormat("One editor is available: {0}", editors[0]);
            }
            // Add the list of available editors to the drop down list
            else
            {
                foreach (var editor in editors)
                {
                    var item = new ToolStripMenuItem(RendererIdentifierConverter.ToCustomTypeIdentifier(editor).DisplayName)
                        {
                            Tag = editor,
                            Checked = defaultEditor == editor,
                        };
                    item.Click += OnEditClicked;
                    editButton.DropDownItems.Add(item);
                }
            }
        }

        protected void InitializeSearchControls()
        {
            if (Presenter.SearchModel == null)
            {
                DisplayEditOnly();
                return;
            }

            _updatingFilterSettings = true;
            searchOption.Items.Clear();
            searchOption.Items.Add(new SearchTypeItem
            {
                Text = Resources.ExactSearch,
                Value = StructureFilterSettings.FilterModeEnum.FullStructure
            });
            searchOption.Items.Add(new SearchTypeItem
            {
                Text = Resources.SubStructureSearch,
                Value = StructureFilterSettings.FilterModeEnum.SubStructure
            });
            searchOption.Items.Add(new SearchTypeItem
            {
                Text = Resources.SimilaritySearch,
                Value = StructureFilterSettings.FilterModeEnum.Simularity
            });
            noStructureColumnsLabel.Text = Resources.NoStructureColumnsMessage;

            foreach (var item in searchOption.Items.Cast<object>().Where(item => ((SearchTypeItem)item).Value == Presenter.SearchType))
            {
                searchOption.SelectedItem = item;
                break;
            }

            SetSimPercentFromModel(Presenter.SearchModel);
            SetRGDFromModel(Presenter.FilterModel);

            nicknames.Checked = Presenter.FilterModel.NickNames;
            _updatingFilterSettings = false;
            minLabel.Visible = true;

            SetComponentsVisible();

            searchOption.SelectedIndexChanged += OnSearchOptionSelectedIndexChanged;
            simPercent.ValueChanged += OnSimPercentValueChanged;
            previousSearches.SelectedIndexChanged += OnPreviousSearchesSelectedIndexChanged;
        }

        private void SetComponentsVisible()
        {
            var selectedItem = (SearchTypeItem)searchOption.SelectedItem;
            //rGroupDecomposition.Visible = selectedItem.Value == StructureFilterSettings.FilterModeEnum.SubStructure;
            //rGroupDecomposition.Enabled = selectedItem.Value == StructureFilterSettings.FilterModeEnum.SubStructure && Presenter.HasStructureString();
            //RGroupBox.Visible = selectedItem.Value == StructureFilterSettings.FilterModeEnum.SubStructure;
            //showStructures.Visible = selectedItem.Value == StructureFilterSettings.FilterModeEnum.SubStructure;
            //showStructures.Enabled = rGroupDecomposition.Checked;
            //nicknames.Visible = selectedItem.Value == StructureFilterSettings.FilterModeEnum.SubStructure;
            //nicknames.Enabled = rGroupDecomposition.Checked;
            simPercent.Visible = selectedItem.Value == StructureFilterSettings.FilterModeEnum.Simularity;
            percentLabel.Visible = selectedItem.Value == StructureFilterSettings.FilterModeEnum.Simularity;
            percent.Visible = selectedItem.Value == StructureFilterSettings.FilterModeEnum.Simularity;
            minLabel.Visible = selectedItem.Value == StructureFilterSettings.FilterModeEnum.Simularity;
            //structureAlignment.Visible = true;
            //structureAlignment.Enabled = Presenter.HasStructureString();
            previousSearches.Visible = false;
            rGroupDecomposition.Visible = false;
            nicknames.Visible = false;
        }

        private void SetSimPercentFromModel(IStructureSearchModel changedModel)
        {
            // Set the similarity search properties
            simPercent.Value = changedModel == null ? 0 : changedModel.PercentSimilarity;
            percentLabel.Text = simPercent.Value.ToString(CultureInfo.InvariantCulture);
        }

        private void SetRGDFromModel(IStructureFilterModel changedModel)
        {
            // Set the RGD properties
            rGroupDecomposition.Checked = changedModel != null && changedModel.RGroupDecomposition;
        }

        protected void ProgressMethod()
        {
            if (IsDisposed)
            {
                return;
            }

            var currentProgressInfo = DataTableInfoMgr.GetProgressInfo(ProgressTable);
            if (currentProgressInfo == null || DataTableInfoMgr.IsCancelled(ProgressTable))
            {
                return;
            }

            Monitor.Enter(currentProgressInfo);
            if (progressLabel != null)
            {
                progressLabel.Text = currentProgressInfo.ProgressCurVal == 0 ? Resources.WaitingToStart : currentProgressInfo.ProgressInformation;
            }
            if (progressBar1 != null)
            {
                progressBar1.Maximum = currentProgressInfo.ProgressMaxVal;
                progressBar1.Value = currentProgressInfo.ProgressCurVal;
            }
            Monitor.Exit(currentProgressInfo);
        }

        private static string GetStructureTypeStringFromEnum(StructureStringType sStringType)
        {
            // TODO why are these handled differently?
            switch (sStringType)
            {
                case StructureStringType.Molfile:
                    return StructureStringType.Molfile.ToString();
                case StructureStringType.CDX:
                    return Identifiers.CDXContentType;
                default:
                    throw new InvalidOperationException("Invalid string type");
            }
        }

        protected virtual void CancelOperation()
        {
        }

        private DataTableInfoMgr.DataTableInfo.SearchStateEnum GetPanelState()
        {
            return Presenter.ModelId != Guid.Empty
                       ? DataTableInfoMgr.GetSearchState(Presenter.ModelId)
                       : DataTableInfoMgr.DataTableInfo.SearchStateEnum.NoStructureColumns;
        }

        private void SetRenderSettings()
        {
            // set the rendering settings to structureControl
            var controlType = _preferences.RendererType;
            if (controlType != ControlType.ChemDraw)
            {
                return;
            }
            StructureStringType structureType;
            GetEditStructureString(controlType, out structureType);
            var contentType = GetStructureTypeStringFromEnum(structureType);
            var chemDrawRenderSettings = ChemDrawUtilities
                .Base64DeserializeChemDrawRenderSettings(
                    _preferences.GetRenderSettings(contentType));
            //LD-919 Bad rendering of the structures when default Chemdraw template is used. 
            //LD-1029 Using the same render setting(default setting), the structure of  table and structure viewer is different 
            //StructureControl.RenderSettings = chemDrawRenderSettings;
            //Commented out China team code for chem draw to render right size of image
            //END 
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetImage()
        {
            // Can't set the image if this gets called before the filterModel has been created
            if (Presenter == null || StructureControl == null)
            {
                return;
            }

            StructureControl.Size = MoleculeImage.Size;

            if (!string.IsNullOrEmpty(Presenter.RGroupStructure))
            {
                SetRGroupImage();
            }
            else
            {
                SetSearchImage();
            }
            SetRenderSettings();
            if (_structureControl.Image == null)
            {
                // LD-1033 If the image is null create a white bitmap so the blue background doesn't show through
                var bmp = new Bitmap(MoleculeImage.Width, MoleculeImage.Height);
                var grph = Graphics.FromImage(bmp);
                grph.FillRectangle(Brushes.White, 0, 0, MoleculeImage.Width, MoleculeImage.Height);
                grph.Dispose();
                MoleculeImage.Image = bmp;
            }
            else
            {
                MoleculeImage.Image = _structureControl.Image;
            }
        }

        private void SetImageSize()
        {
            // Can't resize if there is no panel yet!
            // Shouldn't resize if we are already resizing
            if (Presenter == null || _moleculeImageSizing)
            {
                return;
            }

            _moleculeImageSizing = true;

            MoleculeImage.Height = (int)Math.Round(((double)130 / 177) * panel2.Size.Width) -
                                   (panel2.VerticalScroll.Visible ? VerticalAdjustment : 0);
            MoleculeImage.Width = panel2.Size.Width - (panel2.VerticalScroll.Visible ? ScrollBarAdjustment : 5);

            SetImage();
            _moleculeImageSizing = false;
        }

        private void SetRGroupImage()
        {
            // currently the CCCLR is returning specific strings for each type
            if (Presenter.RGroupStructureType == StructureStringType.CDX)
            {
                if (_structureControl.CtrlType == ControlType.ChemDraw)
                {
                    _structureControl.CDXData = Convert.FromBase64String(Presenter.RGroupStructure);
                }
                else
                {
                    _structureControl.MolFileString = ConvertCdxToMol(Presenter.RGroupStructure);
                }
            }
            else
            {
                _structureControl.MolFileString = Presenter.RGroupStructure;
            }
        }

        private void SetSearchImage()
        {
            if (Presenter.CdxStructure != null && Presenter.CdxStructure.Length > 0)
            {
                if (_structureControl.CtrlType == ControlType.ChemDraw)
                {
                    _structureControl.CDXData = Presenter.CdxStructure;
                }
                else
                {
                    _structureControl.MolFileString = ConvertCdxToMol(Convert.ToBase64String(Presenter.CdxStructure));
                }
            }
            else
            {
                _structureControl.MolFileString = Presenter.StructureString ?? string.Empty;
            }
        }

        protected void SetPanelState()
        {
            var state = DataTableInfoMgr.GetSearchState(Presenter.ModelId);

            searchTypePanel.Enabled = state == DataTableInfoMgr.DataTableInfo.SearchStateEnum.Enabled;
            EnableToolStripButtons(state == DataTableInfoMgr.DataTableInfo.SearchStateEnum.NoStructureColumns
                                       ? ToolStripButtonState.EnableOnlyConfigButton
                                       : ToolStripButtonState.Default);
            ShowNoStructureColumnLabel(state == DataTableInfoMgr.DataTableInfo.SearchStateEnum.NoStructureColumns);
            Panel1.Visible = state == DataTableInfoMgr.DataTableInfo.SearchStateEnum.Busy;
            DisableTimer1();
            timer2.Enabled = state == DataTableInfoMgr.DataTableInfo.SearchStateEnum.Busy;
            EnableFilterControls(state == DataTableInfoMgr.DataTableInfo.SearchStateEnum.Enabled);

            if (state == DataTableInfoMgr.DataTableInfo.SearchStateEnum.Busy)
            {
                ProgressMethod();
            }
        }

        protected void UpdatePreviousSearchesList()
        {
            var settings = new SavedSearchSettings(Presenter.SearchType, Presenter.SearchModel.PercentSimilarity,
                                                   Presenter.FilterModel.RGroupDecomposition, Presenter.StructureString, Presenter.FilterModel.NickNames)
                {
                    CDXStructureData = Presenter.CdxStructure,
                    RGDStructure = Presenter.FilterModel.RGroupStructure,
                    RGDCDXStructureData =
                        Presenter.FilterModel.RGroupDecomposition
                            ? _structureControl.CDXData
                            : null,
                    StructureColumn = ((StructureFilterModel)Presenter.FilterModel).DataColumnReference.Name,
                    Name = DateTime.Now.ToString(CultureInfo.InvariantCulture)
                };

            // Is the search already in the list, if so remove it and we'll re-add it
            var listItem = previousSearches.Items.Cast<object>().Where(item => ((SavedSearchSettings)item).CompareTo(settings) == 0);
            var enumerable = listItem as IList<object> ?? listItem.ToList();
            if (enumerable.Any())
            {
                previousSearches.Items.Remove(enumerable.First());
            }

            if (previousSearches.Items.Count == FilterUtilities.PreviousSearchesMax)
            {
                previousSearches.Items.RemoveAt(FilterUtilities.PreviousSearchesMax - 1);
            }

            previousSearches.Items.Insert(0, settings);
            previousSearches.SelectedItem = previousSearches.Items[0];

            DataTableInfoMgr.AddSavedSearch(Presenter.ModelId, settings);
        }

        private void EnableToolStripButtons(ToolStripButtonState state)
        {
            if (state == ToolStripButtonState.EnableOnlyConfigButton)
            {
                toolStripButtonClear.Enabled = false;
                rendererToolStripMenuItem.Enabled = false;
                toolStripButtonConfigure.Enabled = true;
                toolStrip1.Items[0].Enabled = false;   // Edit button or drop-down edit button
            }
            else
            {
                toolStripButtonEdit.Enabled = !rGroupDecomposition.Checked;
                toolStripButtonClear.Enabled = true;
                rendererToolStripMenuItem.Enabled = true;
                toolStripButtonConfigure.Enabled = !rGroupDecomposition.Checked;
                // LD-976 - If you click the edit button when Spotfire doesn't have
                // focus the editor menu will open even though the button is disabled.  
                // If Spotfire has focus when you click on the button it acts as a disabled 
                // button should (ie. does nothing).  It doesn't seem to be possible to
                // prevent it from opening so we'll do the next best thing and disable it.
                toolStripButtonEdit.DropDown.Enabled = !rGroupDecomposition.Checked;
                toolStrip1.Items[0].Enabled = !rGroupDecomposition.Checked;
            }
        }

        private void ShowNoStructureColumnLabel(bool show)
        {
            noStructureColumnsLabel.Visible = show;
            rgdPanel.Visible = !show;
            MoleculeImage.Visible = !show;
            searchTypePanel.Visible = !show;
            noStructureColumnsLabel.Text = show ? Resources.NoStructureColumnsMessage : string.Empty;
        }

        private void EnableFilterControls(bool enabled)
        {
            toolStrip1.Enabled = enabled;
            searchTypePanel.Enabled = enabled;
            MoleculeImage.Enabled = enabled;
            percentLabel.Enabled = enabled;
            rgdPanel.Enabled = enabled;
        }

        private string GetEditStructureString(ControlType editorType, out StructureStringType structureTypeString)
        {
            var structureType = StructureControl.CDXData == null ? StructureStringType.Molfile : StructureStringType.CDX;
            string structureData;

            if (StructureControl.CDXData != null)
            {
                if (editorType == ControlType.ChemDraw)
                {
                    structureData = Convert.ToBase64String(StructureControl.CDXData);
                    structureData = IsNonEmptyCDXStructure(structureData) ? structureData : string.Empty;
                    structureTypeString = StructureStringType.CDX;
                    return structureData;
                }

                // If we have CDX data but the editor isn't ChemDraw we need to convert the data to a mol string
                StructureControl.MolFileString = ConvertCdxToMol(Convert.ToBase64String(StructureControl.CDXData));
                structureType = StructureStringType.Molfile;
            }

            structureData = IsNonEmptyStructure(StructureControl.MolFileString, structureType) ? StructureControl.MolFileString : string.Empty;
            structureTypeString = StructureStringType.Molfile;
            return structureData;
        }

        private void SetEditedStructure(StructureStringType structureType, string editedStructure)
        {
            string structureString;
            byte[] cdxStructure = null;
            // Check that we don't have an "empty" molecule
            if (IsNonEmptyStructure(editedStructure, structureType))
            {
                structureString = structureType == StructureStringType.CDX ? ConvertCdxToMol(editedStructure) : editedStructure;

                if (_preferences.RendererType == ControlType.ChemDraw && structureType == StructureStringType.CDX)
                {
                    StructureControl.CDXData = Convert.FromBase64String(editedStructure);
                    cdxStructure = StructureControl.CDXData;
                }
                else
                {
                    StructureControl.CDXData = null;
                    StructureControl.MolFileString = structureString;
                }
            }
            else
            {
                structureString = string.Empty;
                StructureControl.CDXData = null;
                StructureControl.MolFileString = string.Empty;
            }

            Presenter.SetFilterSettings(Presenter.SearchType, structureString, cdxStructure);
            Presenter.SetStructureAlignment(structureAlignment.Checked);
            StructureControl.Refresh();
            SetRenderSettings();
            SetImage();
        }

        private string ConvertCdxToMol(string cdx)
        {
            return Presenter.ConvertCdxToMol(cdx);
        }

        private static bool IsNonEmptyStructure(string structureData, StructureStringType structureType)
        {
            return structureType == StructureStringType.Molfile ? IsNonEmptyMolStructure(structureData) : IsNonEmptyCDXStructure(structureData);
        }

        private static bool IsNonEmptyMolStructure(string structureData)
        {
            if (string.IsNullOrEmpty(structureData))
            {
                return false;
            }
            var structureRows = structureData.Split(new[] { "\n" }, StringSplitOptions.None);
            if (structureRows.Length < 4)
            {
                return false;
            }
            return !structureRows[3].StartsWith("  0  0  0  0  0  0");
        }

        private static bool IsNonEmptyCDXStructure(string structureData)
        {
            var cdxData = Convert.FromBase64String(structureData);

            // Skip the 22-byte header then look for the first bounding rectangle
            var i = 22;
            while (i < cdxData.Length - 20)
            {
                if (cdxData[i] == 4 && cdxData[i + 1] == 2)
                {
                    // Skip the length, it's always 16 bytes
                    i += 3;
                    for (var j = 0; j < 16; j++)
                    {
                        if (i + j >= cdxData.Length)
                        {
                            return false;
                        }
                        // If the bounding rectangle is all zeros it's an empty structure
                        if (cdxData[i + j] != 0)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                i++;
            }
            return false;
        }

        private void UpdateRGroupSettings()
        {
            if (_updatingFilterSettings)
            {
                return;
            }
            _updatingFilterSettings = true;
            try
            {
                if (!rGroupDecomposition.Checked)
                {
                    // Clear the R Group template structure
                    Presenter.RGroupStructure = string.Empty;
                    SetImage();

                    Presenter.SetRGroupDecomposition(false, false);

                    // Remove the virtual columns
                    Presenter.RemoveRGroupDecompositionColumnsFromDocument();
                }
                else
                {
                    Presenter.SetRGroupDecomposition(true, nicknames.Checked);
                }
            }
            finally
            {
                _updatingFilterSettings = false;
            }
        }


        private void UpdateAlignmentSettings()
        {
            if (!structureAlignment.Checked)
            {
                Presenter.SetStructureAlignment(false);
            }
            else
            {
                Presenter.SetStructureAlignment(true);
            }
        }

        private void ChangeBackgroundColor(bool selected)
        {
            var colour = ColorTranslator.FromHtml("#D2E8F6");
            searchTypePanel.BackColor = selected ? colour : Color.White;
            MoleculeImage.BackColor = selected ? colour : Color.White;
            percentLabel.BackColor = selected ? colour : Color.White;
            panel2.BackColor = selected ? colour : Color.White;
            rgdPanel.BackColor = selected ? colour : Color.White;
        }

        protected void DisableTimer1()
        {
            timer1.Enabled = false;
        }

        private void CheckMenuItem(int toolbarButtonIndex, ControlType type)
        {
            try
            {
                var button = toolStrip1.Items[toolbarButtonIndex] as ToolStripDropDownButton;
                if (button == null)
                {
                    return;
                }
                var items = button.DropDownItems;

                foreach (var menuItem in items.OfType<ToolStripMenuItem>().Select(item => item).Where(menuItem => menuItem.Tag != null))
                {
                    menuItem.Checked = (ControlType)menuItem.Tag == type;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private Dictionary<ControlType, bool> GetInstalledRenderers()
        {
            if (Presenter == null || Presenter.NodeContext == null)
            {
                return null;
            }

            var licenseManager = TryGetLicenseManager();
            if (licenseManager == null)
            {
                return null;
            }

            return
                Enum.GetValues(typeof(ControlType))
                    .Cast<ControlType>()
                    .Where(
                        controlType =>
                        controlType != ControlType.None &&
                        ChemistryLicense.IsControlTypeLicensed(Presenter.NodeContext.GetService<LicenseManager>(),
                                                               controlType) &&
                        StructureControlFactory.IsControlTypeInstalled(controlType))
                    .ToDictionary(controlType => controlType,
                                  StructureControlFactory.ControlTypeSupportsEditing);
        }

        private ControlType GetDefaultRenderer()
        {
            var renderers = GetInstalledRenderers();
            if (renderers == null)
            {
                return ControlType.None;
            }

            // If the renderer set in the preferences exists return it
            if (renderers.ContainsKey(_preferences.RendererType))
            {
                return _preferences.RendererType;
            }

            // The renderer from the preferences isn't installed so choose ChemDraw if installed otherwise the first available renderer
            _preferences.RendererType = renderers.ContainsKey(ControlType.ChemDraw) ? ControlType.ChemDraw : renderers.First().Key;
            _preferences.Save();
            return _preferences.RendererType;
        }

        private ControlType GetDefaultEditor()
        {
            var renderers = GetInstalledRenderers();
            if (renderers == null)
            {
                return ControlType.None;
            }

            var editors = (from renderer in renderers where renderer.Value select renderer.Key).ToList();

            if (editors.Count == 0)
            {
                MessageBox.Show(Resources.Err_NoEditor, Resources.Error);
                return ControlType.None;
            }

            // If the editor set in the preferences exists return it
            if (editors.Contains(_preferences.EditorType))
            {
                return _preferences.EditorType;
            }

            // Is the default renderer also an editor
            var defaultRenderer = GetDefaultRenderer();
            if (renderers[defaultRenderer])
            {
                _preferences.EditorType = defaultRenderer;
                _preferences.Save();
                return _preferences.EditorType;
            }

            // The editor from the preferences isn't installed and the default renderer isn't an editor 
            // so choose ChemDraw if installed otherwise the first available editor
            _preferences.EditorType = editors.Contains(ControlType.ChemDraw) ? ControlType.ChemDraw : editors.First();
            _preferences.Save();
            return _preferences.EditorType;
        }

        private static void SetSearchState(IStructureSearchModel searchModel, IStructureEditModel editModel)
        {
            if (!DataTableInfoMgr.ContainsKey(editModel.ModelId))
            {
                DataTableInfoMgr.Add(editModel.ModelId, editModel.NodeContext.GetAncestor<Document>(), searchModel.ValidDataTable());
            }
            else
            {
                if (searchModel.ValidDataTable() &&
                    DataTableInfoMgr.GetSearchState(editModel.ModelId) ==
                    DataTableInfoMgr.DataTableInfo.SearchStateEnum.NoStructureColumns)
                {
                    DataTableInfoMgr.SetSearchState(editModel.ModelId,
                                                    DataTableInfoMgr.DataTableInfo.SearchStateEnum.Enabled);
                }

                if (!searchModel.ValidDataTable())
                {
                    DataTableInfoMgr.SetSearchState(editModel.ModelId,
                                                    DataTableInfoMgr.DataTableInfo.SearchStateEnum
                                                                    .NoStructureColumns);
                }
            }
        }

        protected void FillPreviousSearchesCombo()
        {
            var searches = DataTableInfoMgr.GetSavedSearches(Presenter.ModelId);
            if (searches == null)
            {
                return;
            }

            // Add the saved searches to the combo box
            foreach (var search in searches)
            {
                previousSearches.Items.Add(search);
            }
            if (previousSearches.Items.Count > 0)
            {
                previousSearches.SelectedItem = previousSearches.Items[0];
            }
        }

        protected void UpdateSavedSearchesList()
        {
            if (!Presenter.FilterModel.UpdateSavedSearchList)
            {
                return;
            }

            var searches = DataTableInfoMgr.GetSavedSearches(Presenter.ModelId);
            if (searches == null)
            {
                return;
            }

            previousSearches.SelectedIndexChanged -= OnPreviousSearchesSelectedIndexChanged;
            previousSearches.Items.Clear();
            foreach (var item in searches)
            {
                previousSearches.Items.Add(item);
            }

            if (previousSearches.SelectedItem == null && previousSearches.Items.Count > 0)
            {
                previousSearches.SelectedItem = previousSearches.Items[0];
            }
            previousSearches.SelectedIndexChanged += OnPreviousSearchesSelectedIndexChanged;
        }

        private class SearchTypeItem
        {
            public string Text { private get; set; }
            public StructureFilterSettings.FilterModeEnum Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        /// <summary>
        /// Hiding controls which are not required by external application.
        /// </summary>
        public void HideControls()
        {
            toolStripButtonConfigure.Visible = false;
            rGroupDecomposition.Visible = false;

            RGroupBox.Visible = false;
            showStructures.Visible = false;
            nicknames.Visible = false;
            structureAlignment.Visible = false;
            previousSearches.Visible = false;
        }

        /// <summary>
        /// Verifying whether Renderers are Licensed and Installed
        /// </summary>
        /// <param name="theAnalysisApplication">Analysis Application object</param>
        /// <returns>Number of Renderers installed</returns>
        public static int IsRendererInstalled(AnalysisApplication theAnalysisApplication)
        {
            int rendererCount = 0;
            try
            {
                CBVNStructureFilterSupport.GeneralClass.LDInstallationPath = GeneralClass.LDInstallationPath;
                LicenseManager licenseManager = theAnalysisApplication.GetService<LicenseManager>();
                if (licenseManager == null)
                {
                    return 0;
                }

                rendererCount = RendererInstalled(licenseManager).Count;
            }
            catch
            {
            }
            return rendererCount;
        }

        private static Dictionary<ControlType, bool> RendererInstalled(LicenseManager licenseManager)
        {
            return
                Enum.GetValues(typeof(ControlType))
                    .Cast<ControlType>()
                    .Where(
                        controlType =>
                        controlType != ControlType.None &&
                        ChemistryLicense.IsControlTypeLicensed(licenseManager,
                                                               controlType) &&
                        StructureControlFactory.IsControlTypeInstalled(controlType))
                    .ToDictionary(controlType => controlType,
                                  StructureControlFactory.ControlTypeSupportsEditing);
        }
    }
}
