using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;
using System.Globalization;
using System.Text;
using SpotfireIntegration.Common;
using COEServiceLib;

namespace FormWizard
{


    /// <summary>
    /// A panel that tracks and displays the data for a single selected query criteria field.
    /// </summary>
    internal class QueryFieldPanel : FieldPanel
    {
        #region Variables
        ComboBox comboBoxOperator = new ComboBox();
        //TextBox valueTextBox = new TextBox();
        //TextBox value2TextBox = new TextBox();
        private SelectDataForm.QueryCriteriaOperator queryCriteriaOperator;
        private ValueCriteriaEditor valueCriteriaEditor;
        //string _value;

        //fake control for displaying erro messages on UI
        Label errorProviderLabel = new Label();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the query criteria operator value
        /// </summary>
        public SelectDataForm.QueryCriteriaOperator QueryCriteriaOperator
        {
            get
            {
                if (comboBoxOperator.SelectedIndex > 0)
                {
                    queryCriteriaOperator = (SelectDataForm.QueryCriteriaOperator)comboBoxOperator.SelectedItem;
                    return queryCriteriaOperator;
                }
                return null;
            }
            set
            {
                queryCriteriaOperator = value;
                //if search value in second input control is not empty then operator needs to be set to 'between' instead of 'equal'
                if (!string.IsNullOrEmpty(SearchValue2))
                {
                    comboBoxOperator.SelectedIndex = comboBoxOperator.FindString(FormWizard.Properties.Resources.BETWEEN_OPERATOR, -1);
                }
                else
                {
                    comboBoxOperator.SelectedItem = queryCriteriaOperator;
                }
            }
        }

        /// <summary>
        /// Gets or sets the query criteria field value
        /// </summary>
        public string SearchValue1
        {
            get
            {
                return valueCriteriaEditor.SearchValue1;
            }
            set
            {
                valueCriteriaEditor.SearchValue1 = value;
            }
        }

        public string SearchValue2
        {
            get
            {
                return valueCriteriaEditor.SearchValue2;
            }
            set
            {
                valueCriteriaEditor.SearchValue2 = value;
            }
        }

        /// <summary>
        /// Gets the mol weight search value
        /// </summary>
        public string MolWeightSearchValue
        {
            get
            {
                if (QueryCriteriaOperator != null)
                    return valueCriteriaEditor.BuildMolWeightSearchValue();
                return string.Empty;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes an instance of QueryFieldPanel from existing QueryFieldPanel object
        /// </summary>
        /// <param name="queryFieldPanel">QueryFieldPanel object</param>
        public QueryFieldPanel(QueryFieldPanel queryFieldPanel)
            : base(queryFieldPanel)
        {
            this.SearchValue1 = queryFieldPanel.SearchValue1;
            this.SearchValue2 = queryFieldPanel.SearchValue2;
            this.QueryCriteriaOperator = queryFieldPanel.queryCriteriaOperator;
        }

        /// <summary>
        /// Initializes an instance of QueryFieldPanel from specified field context object
        /// </summary>
        /// <param name="fieldContext">FieldCOntext object</param>        
        public QueryFieldPanel(SelectDataForm.FieldContext fieldContext)
            : base(fieldContext)
        {
            string lkpStruct = string.Empty;
            if (fieldContext.LookupField != null)
                lkpStruct = fieldBO.Alias + "-";
            if (((SelectDataForm.QueryCriteriaFieldContext)fieldContext).fieldCriteriaType == typeof(SearchCriteria.CSMolWeightCriteria)) // typeof(ResultsCriteria.MolWeight))
            {
                this.Alias = lkpStruct + "Mol Weight";
            }
            else if (((SelectDataForm.QueryCriteriaFieldContext)fieldContext).fieldCriteriaType == typeof(SearchCriteria.CSFormulaCriteria)) //typeof(ResultsCriteria.Formula))
            {
                this.Alias = lkpStruct + "Formula";
            }
            else
            {
                this.Alias = fieldBO.Alias;
            }
            Init();
            comboBoxOperator.SelectedIndex = 0;
        }
        #endregion

        public override void Init()
        {
            base.Init();

            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));

            //create a combobox for operator values
            comboBoxOperator.Items.Clear();
            comboBoxOperator.Items.Add("Select operator");
            comboBoxOperator.Items.AddRange(SelectDataForm.GetQueryCriteriaOperators(this).ToArray());
            comboBoxOperator.Dock = DockStyle.Fill;
            comboBoxOperator.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxOperator.Enabled = true;
            comboBoxOperator.SelectedIndexChanged += new EventHandler(comboBoxOperator_SelectedIndexChanged);
            this.Controls.Add(comboBoxOperator);
            this.SetCellPosition(comboBoxOperator, new TableLayoutPanelCellPosition(2, 0));
            COEDataView.AbstractTypes dataType = base.fieldBO.DataType;
            if (fieldContext.fieldBO.LookupDisplayFieldId > 0 && fieldContext.LookupField != null)
                dataType = fieldContext.LookupField.DataType;

            string lkpStruct = string.Empty;
            if (this.fieldBO.LookupDisplayFieldId != -1 && this.lookupField != null)
                lkpStruct = fieldBO.Alias + "-";

            if (this.Alias.Equals(lkpStruct + "Mol Weight", StringComparison.OrdinalIgnoreCase))
            {
                valueCriteriaEditor = new MolWeightCriteriaEditor(COEDataView.AbstractTypes.Real);
                Control ctrl = valueCriteriaEditor.Init();
                this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
                this.Controls.Add(ctrl);
                this.SetCellPosition(ctrl, new TableLayoutPanelCellPosition(3, 0));
            }
            else if (this.Alias.Equals(lkpStruct + "Formula", StringComparison.OrdinalIgnoreCase))
            {
                valueCriteriaEditor = new FormulaCriteriaEditor(COEDataView.AbstractTypes.Real);
                Control ctrl = valueCriteriaEditor.Init();
                this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
                this.Controls.Add(ctrl);
                this.SetCellPosition(ctrl, new TableLayoutPanelCellPosition(3, 0));
            }
            else if (dataType == COEDataView.AbstractTypes.Real || dataType == COEDataView.AbstractTypes.Integer)
            {
                valueCriteriaEditor = new NumericValueCriteriaEditor(dataType);
                Control ctrl = valueCriteriaEditor.Init();
                this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
                this.Controls.Add(ctrl);
                this.SetCellPosition(ctrl, new TableLayoutPanelCellPosition(3, 0));
            }
            else if (dataType == COEDataView.AbstractTypes.Date)
            {
                valueCriteriaEditor = new DateValueCriteriaEditor();
                Control ctrl = valueCriteriaEditor.Init();
                this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
                this.Controls.Add(ctrl);
                this.SetCellPosition(ctrl, new TableLayoutPanelCellPosition(3, 0));
            }
            else
            {
                //create a text box for query value
                valueCriteriaEditor = new TextValueCriteriaEditor();
                TextBox ctrl = (TextBox)valueCriteriaEditor.Init();
                this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
                this.Controls.Add(ctrl);
                this.SetCellPosition(ctrl, new TableLayoutPanelCellPosition(3, 0));
            }
            this.ParentChanged += new EventHandler(QueryFieldPanel_ParentChanged);

            //for error provider display purpose
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20));
            errorProviderLabel.AutoSize = true;
            errorProviderLabel.Text = string.Empty;
            errorProviderLabel.Dock = DockStyle.Left;
            errorProviderLabel.TextAlign = ContentAlignment.MiddleLeft;
            this.Controls.Add(errorProviderLabel);
            this.SetCellPosition(errorProviderLabel, new TableLayoutPanelCellPosition(4, 0));
        }

        void comboBoxOperator_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBoxOperator.SelectedIndex > 0)
            {
                valueCriteriaEditor.SelectedOperator = (SelectDataForm.QueryCriteriaOperator)comboBoxOperator.SelectedItem;
                if (((SelectDataForm.QueryCriteriaOperator)comboBoxOperator.SelectedItem).ToString() == FormWizard.Properties.Resources.BETWEEN_OPERATOR)
                {
                    valueCriteriaEditor.EnableSearchValueControls(true, true);
                }
                else
                {
                    valueCriteriaEditor.EnableSearchValueControls(true, false);
                    if (!valueCriteriaEditor.IsSourceAvailable() && COEServiceHelper.COEService != null)
                    {
                        SetAutoCompleteSource(valueCriteriaEditor);
                    }
                }
            }
            else
            {
                valueCriteriaEditor.EnableSearchValueControls(false, false);
            }
        }

        private void SetAutoCompleteSource(QueryFieldPanel.ValueCriteriaEditor valueCriteriaEditor)
        {
            try
            {
                if (fieldContext.fieldBO.DataType == COEDataView.AbstractTypes.Text || fieldContext.fieldBO.LookupDisplayFieldId > 0)
                {
                    //Get the dataset for the list of field values to bind the datasource for typeahead search.
                    Cursor.Current = Cursors.WaitCursor;
                    AutoCompleteStringCollection source = null;
                    string toolTip = string.Empty;
                    try
                    {
                        //get the instance of pick list values cache
                        PickListCache theTypeAheadCache = PickListCache.Instance(COEServiceHelper.COEService);
                        //check if the field is of type lookup field
                        if (fieldContext.fieldBO.LookupFieldId != -1)
                        {
                            source = theTypeAheadCache.GetTypeAheadBindingSource(fieldContext.DataviewId, fieldContext.LookupFieldTableBOId, fieldContext.fieldBO.LookupDisplayFieldId, true);
                        }
                        else
                        {
                            //check if the picklist values can be displayed by checking 5000 row limit of base table
                            if (theTypeAheadCache.CanSetPickListValues(fieldContext.DataviewId, fieldContext.tableId))
                            {
                                source = theTypeAheadCache.GetTypeAheadBindingSource(fieldContext.DataviewId, fieldContext.tableId, fieldContext.fieldId, false);
                            }
                            else
                            {
                                toolTip = "Cannot load picklist values as picklist contains large data";
                            }
                        }
                    }
                    catch (PickListCache.PickListCacheException pickListEx)
                    {
                        //assign exception message that we have trapped
                        toolTip = pickListEx.Message;
                    }
                    catch // just ignore the exception and set the tool tip when any exception occurs
                    {
                    }
                    finally
                    {
                        valueCriteriaEditor.SetBindingSource(source, toolTip);
                        Cursor.Current = Cursors.Default;
                    }
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show(FormWizard.Properties.Resources.Invalid_Daview, FormWizard.Properties.Resources.FORM_TITLE, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        void QueryFieldPanel_ParentChanged(object sender, EventArgs e)
        {
            this.SetForm();
        }

        internal override void SetForm()
        {
            if (this.Form != null)
            {
                base.SetForm();
            }
        }

        /// <summary>
        /// Method to set the context menu items specific to the Search criteria field
        /// Make context menu items not visible other than the Remove Field.
        /// </summary>
        internal override void SetContextMenuStripItems()
        {
            //set default context menu strip
            base.SetContextMenuStripItems();
            //make rename field menu item and sort order menu item from menu strip invisible.
            this.ContextMenuStrip.Items["renameFieldToolStripTextBox"].Visible = false;
            this.ContextMenuStrip.Items["sortOrderToolStripMenuItem"].Visible = false;
        }

        private ErrorProvider TheErrorProvider
        {
            get
            {
                return ((SelectDataForm)this.FindForm()).TheErrorProvider;
            }
        }

        private bool IsOperatorSelected
        {
            get
            {
                return comboBoxOperator.SelectedIndex > 0;
            }
        }

        internal override bool ValidateControls()
        {
            bool result = true;
            base.ValidateControls();
            if (IsOperatorSelected && !valueCriteriaEditor.Validate())
            {
                TheErrorProvider.SetError(this.errorProviderLabel, valueCriteriaEditor.ErrorText); //labelFieldName
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Assigns the mol weigth search criteria value to controls
        /// </summary>
        /// <param name="molWeightSearchValue">mol weight search value</param>
        public void AssignMolWeightCriteriaDetails(string molWeightSearchValue)
        {
            if (!string.IsNullOrEmpty(molWeightSearchValue))
            {
                valueCriteriaEditor.AssignMolWeightCriteriaDetails(molWeightSearchValue);
            }
        }

        internal abstract class ValueCriteriaEditor
        {
            protected string errorMessage = string.Empty;

            internal virtual string ErrorText
            {
                get
                {
                    return errorMessage;
                }
            }

            public abstract string SearchValue1
            {
                get;
                set;
            }
            public abstract string SearchValue2
            {
                get;
                set;
            }
            public abstract Control Init();

            public virtual bool Validate()
            {
                return true;
            }

            internal abstract void EnableSearchValueControls(bool enableSearchControl1, bool enableSearchControl2);

            internal virtual void SetBindingSource(AutoCompleteStringCollection source, string toolTip)
            {
            }

            internal virtual bool IsSourceAvailable()
            {
                return false;
            }

            internal SelectDataForm.QueryCriteriaOperator SelectedOperator
            {
                get;
                set;
            }

            /// <summary>
            /// Creates the mol weight search value
            /// </summary>
            /// <returns>returns the mol weight search value</returns>
            public virtual string BuildMolWeightSearchValue()
            {
                return string.Empty;
            }

            /// <summary>
            /// Assigns the mol weight search value to appropriate controls
            /// </summary>
            /// <param name="molWeightSearchValue">mol weight search value</param>
            public virtual void AssignMolWeightCriteriaDetails(string molWeightSearchValue)
            {
            }
        }

        internal class NumericValueCriteriaEditor : ValueCriteriaEditor
        {
            TextBox value1TextBox = new TextBox();
            TextBox value2TextBox = new TextBox();

            string _value1;
            string _value2;
            private COEDataView.AbstractTypes abstractTypes;

            public override string SearchValue1
            {
                get
                {
                    double outDecimal;
                    if (double.TryParse(value1TextBox.Text.Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out outDecimal))
                    {
                        _value1 = outDecimal.ToString(CultureInfo.CurrentCulture);
                    }
                    else
                    {
                        _value1 = value1TextBox.Text.Trim();
                    }
                    return _value1;
                }
                set
                {
                    double outDecimal;
                    _value1 = value;
                    double.TryParse(_value1, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out outDecimal);
                    value1TextBox.Text = outDecimal.ToString();
                }
            }

            public override string SearchValue2
            {
                get
                {
                    double outDecimal;
                    if (value2TextBox.Enabled)
                    {
                        if (double.TryParse(value2TextBox.Text.Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out outDecimal))
                        {
                            _value2 = outDecimal.ToString(CultureInfo.CurrentCulture);
                        }
                        else
                        {
                            _value2 = value2TextBox.Text.Trim();
                        }
                    }
                    else
                    {
                        _value2 = string.Empty;
                    }
                    return _value2;
                }
                set
                {
                    double outDecimal;
                    _value2 = value;
                    if (double.TryParse(_value2, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out outDecimal))
                    {
                        //enable the second text box to ensure the between operator gets selected
                        value2TextBox.Enabled = true;
                        value2TextBox.Text = outDecimal.ToString();
                    }
                }
            }

            public NumericValueCriteriaEditor(COEDataView.AbstractTypes abstractTypes)
            {
                // TODO: Complete member initialization
                this.abstractTypes = abstractTypes;
            }

            public override Control Init()
            {
                TableLayoutPanel rangeTablePanel = new TableLayoutPanel();
                rangeTablePanel.Margin = new Padding(0);
                rangeTablePanel.ColumnStyles.Clear();
                rangeTablePanel.RowCount = 1;
                rangeTablePanel.ColumnCount = 2;
                rangeTablePanel.Dock = DockStyle.Fill;
                rangeTablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                value1TextBox.Dock = DockStyle.Fill;
                rangeTablePanel.Controls.Add(value1TextBox);
                rangeTablePanel.SetCellPosition(value1TextBox, new TableLayoutPanelCellPosition(0, 0));
                rangeTablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                value2TextBox.Dock = DockStyle.Fill;
                rangeTablePanel.Controls.Add(value2TextBox);
                rangeTablePanel.SetCellPosition(value2TextBox, new TableLayoutPanelCellPosition(1, 0));
                return rangeTablePanel;
            }

            public override bool Validate()
            {
                this.errorMessage = string.Empty;
                string val1 = value1TextBox.Text.Trim();
                string val2 = value2TextBox.Text.Trim();
                switch (abstractTypes)
                {
                    case COEDataView.AbstractTypes.Integer:
                        return ValidateInputValues(val1, val2);
                    case COEDataView.AbstractTypes.Real:
                        return ValidateInputRealValues(val1, val2);
                }
                return true;
            }


            private bool ValidateInputRealValues(string val1, string val2)
            {
                double outDecimal;
                if (base.SelectedOperator.ToString() == FormWizard.Properties.Resources.BETWEEN_OPERATOR)
                {
                    if (string.IsNullOrEmpty(val1) && string.IsNullOrEmpty(val2))
                    {
                        return true;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(val1))
                        {
                            errorMessage = "Missing value in 'From'";
                            return false;
                        }
                        if (!double.TryParse(val1, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out outDecimal))
                        {
                            errorMessage = "Invalid/missing value in 'From'";
                            return false;
                        }
                        if (string.IsNullOrEmpty(val2))
                        {
                            errorMessage = "Missing value in 'To'";
                            return false;
                        }
                        return ValidateValue2RealTextBox(val1, val2, out outDecimal);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(val1) && !double.TryParse(val1, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out outDecimal))
                    {
                        errorMessage = "Invalid/missing value in 'From'";
                        return false;
                    }
                    return true;
                }
            }

            private bool ValidateInputValues(string val1, string val2)
            {
                int outInt;
                if (base.SelectedOperator.ToString() == FormWizard.Properties.Resources.BETWEEN_OPERATOR)
                {
                    if (string.IsNullOrEmpty(val1) && string.IsNullOrEmpty(val2))
                    {
                        return true;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(val1))
                        {
                            errorMessage = "Missing value in 'From'";
                            return false;
                        }
                        if (!int.TryParse(val1, NumberStyles.Integer, CultureInfo.CurrentCulture, out outInt))
                        {
                            double outDecimal1;
                            if (double.TryParse(val1, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out outDecimal1))
                            {
                                errorMessage = "Invalid integer value in 'From'";
                                return false;
                            }
                            errorMessage = "Invalid/missing value in 'From'";
                            return false;
                        }
                        if (string.IsNullOrEmpty(val2))
                        {
                            errorMessage = "Missing value in 'To'";
                            return false;
                        }
                        return ValidateValue2IntegerTextBox(val1, val2, out outInt);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(val1) && !int.TryParse(value1TextBox.Text.Trim(), NumberStyles.Integer, CultureInfo.CurrentCulture, out outInt))
                    {
                        double outDecimal1;
                        if (double.TryParse(value1TextBox.Text.Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out outDecimal1))
                        {
                            errorMessage = "Invalid integer value in 'From'";
                            return false;
                        }
                        errorMessage = "Invalid/missing value in 'From'";
                        return false;
                    }
                    return true;
                }
            }

            private bool ValidateValue2RealTextBox(string val1, string val2, out double outDecimal)
            {
                if (!double.TryParse(val2, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out outDecimal))
                {
                    errorMessage = "Invalid/missing value in 'To'";
                    return false;
                }
                else
                {
                    if (Convert.ToDouble(val2, CultureInfo.CurrentCulture) < Convert.ToDouble(val1, CultureInfo.CurrentCulture))
                    {
                        errorMessage = "'To' value can not be less than 'From' value";
                        return false;
                    }
                    return true;
                }
            }

            private bool ValidateValue2IntegerTextBox(string val1, string val2, out int outInt)
            {
                if (!int.TryParse(val2, NumberStyles.Integer, CultureInfo.CurrentCulture, out outInt))
                {
                    double outDecimal2;
                    if (double.TryParse(val2, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out outDecimal2))
                    {
                        errorMessage = "Invalid integer value in 'To'";
                        return false;
                    }
                    errorMessage = "Invalid/missing value in 'To'";
                    return false;
                }
                else
                {
                    if (Convert.ToInt32(val2, CultureInfo.CurrentCulture) < Convert.ToInt32(val1, CultureInfo.CurrentCulture))
                    {
                        errorMessage = "'To' value can not be less than 'From' value";
                        return false;
                    }
                    return true;
                }
            }

            internal override void EnableSearchValueControls(bool enableSearchControl1, bool enableSearchControl2)
            {
                if (!enableSearchControl1)
                {
                    value1TextBox.Text = string.Empty;
                }
                if (!enableSearchControl2)
                {
                    value2TextBox.Text = string.Empty;
                }
                value1TextBox.Enabled = enableSearchControl1;
                value2TextBox.Enabled = enableSearchControl2;
            }

            internal override void SetBindingSource(AutoCompleteStringCollection source, string toolTip)
            {
                if (source != null)
                {
                    value1TextBox.AutoCompleteCustomSource = source;
                    value1TextBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    value1TextBox.AutoCompleteMode = AutoCompleteMode.Suggest;

                    value2TextBox.AutoCompleteCustomSource = source;
                    value2TextBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    value2TextBox.AutoCompleteMode = AutoCompleteMode.Suggest;
                }
                else
                {
                    ToolTip tt = new ToolTip();
                    tt.SetToolTip(value1TextBox, FormWizard.Properties.Resources.Field_LargeData_Message);
                    tt.SetToolTip(value2TextBox, FormWizard.Properties.Resources.Field_LargeData_Message);
                }
            }

            internal override bool IsSourceAvailable()
            {
                return value1TextBox.AutoCompleteMode != AutoCompleteMode.None && value2TextBox.AutoCompleteMode != AutoCompleteMode.None;
            }
        }

        internal class MolWeightCriteriaEditor : ValueCriteriaEditor
        {
            TextBox value1TextBox = new TextBox();
            TextBox value2TextBox = new TextBox();
            string _value1;
            string _value2;
            private COEDataView.AbstractTypes abstractTypes;

            public override string SearchValue1
            {
                get
                {
                    double outDecimal;
                    if (double.TryParse(value1TextBox.Text.Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out outDecimal))
                    {
                        _value1 = outDecimal.ToString(CultureInfo.CurrentCulture);
                    }
                    else
                    {
                        _value1 = value1TextBox.Text.Trim();
                    }
                    return _value1;
                }
                set
                {
                    double outDecimal;
                    _value1 = value;
                    double.TryParse(_value1, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out outDecimal);
                    value1TextBox.Text = outDecimal.ToString();
                }
            }

            public override string SearchValue2
            {
                get
                {
                    double outDecimal;
                    if (value2TextBox.Enabled)
                    {
                        if (double.TryParse(value2TextBox.Text.Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out outDecimal))
                        {
                            _value2 = outDecimal.ToString(CultureInfo.CurrentCulture);
                        }
                        else
                        {
                            _value2 = value2TextBox.Text.Trim();
                        }
                    }
                    else
                    {
                        _value2 = string.Empty;
                    }
                    return _value2;
                }
                set
                {
                    double outDecimal;
                    _value2 = value;
                    if (double.TryParse(_value2, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out outDecimal))
                    {
                        //enable the second text box to ensure the between operator gets selected
                        value2TextBox.Enabled = true;
                        value2TextBox.Text = outDecimal.ToString();
                    }
                }
            }

            public MolWeightCriteriaEditor(COEDataView.AbstractTypes abstractTypes)
            {
                this.abstractTypes = abstractTypes;
            }

            public override bool Validate()
            {
                this.errorMessage = string.Empty;
                string val1 = value1TextBox.Text.Trim();
                string val2 = value2TextBox.Text.Trim();
                return ValidateInputRealValues(val1, val2);
                #region
                //double outDecimal;
                //if (!string.IsNullOrEmpty(value1TextBox.Text.Trim()) && !double.TryParse(value1TextBox.Text.Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out outDecimal))
                //{
                //    errorMessage = "Invalid/missing value in 'From'";
                //    return false;
                //}
                //if (base.SelectedOperator.ToString() == FormWizard.Properties.Resources.BETWEEN_OPERATOR)
                //{
                //    if (!string.IsNullOrEmpty(value2TextBox.Text.Trim()))
                //    {
                //        if (string.IsNullOrEmpty(value1TextBox.Text.Trim()))
                //        {
                //            errorMessage = "Invalid/missing value in 'From'";
                //            return false;
                //        }
                //        return ValidateValue2MolwWeightBox(out outDecimal);
                //    }
                //    //else
                //    //{
                //    //    errorMessage = "Invalid/missing value in 'To'";
                //    //    return false;
                //    //}
                //}
                //else
                //{
                //    if (value2TextBox.Enabled && !string.IsNullOrEmpty(value2TextBox.Text.Trim()))
                //    {
                //        return ValidateValue2MolwWeightBox(out outDecimal);
                //    }
                //}
                //return true;
                #endregion
            }

            private bool ValidateInputRealValues(string val1, string val2)
            {
                double outDecimal;
                if (base.SelectedOperator.ToString() == FormWizard.Properties.Resources.BETWEEN_OPERATOR)
                {
                    if (string.IsNullOrEmpty(val1) && string.IsNullOrEmpty(val2))
                    {
                        return true;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(val1))
                        {
                            errorMessage = "Missing value in 'From'";
                            return false;
                        }
                        if (!double.TryParse(val1, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out outDecimal))
                        {
                            errorMessage = "Invalid/missing value in 'From'";
                            return false;
                        }
                        if (string.IsNullOrEmpty(val2))
                        {
                            errorMessage = "Missing value in 'To'";
                            return false;
                        }
                        return ValidateValue2MolwWeightBox(val1, val2, out outDecimal);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(val1) && !double.TryParse(val1, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out outDecimal))
                    {
                        errorMessage = "Invalid/missing value in 'From'";
                        return false;
                    }
                    return true;
                }
            }

            private bool ValidateValue2MolwWeightBox(string val1, string val2, out double outDecimal)
            {
                if (!double.TryParse(val2, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out outDecimal))
                {
                    errorMessage = "Invalid/missing value in 'To'";
                    return false;
                }
                else
                {
                    if (Convert.ToDouble(val2, CultureInfo.CurrentCulture) < Convert.ToDouble(val1, CultureInfo.CurrentCulture))
                    {
                        errorMessage = "'To' value can not be less than 'From' value";
                        return false;
                    }
                    return true;
                }
            }

            public override Control Init()
            {
                TableLayoutPanel rangeTablePanel = new TableLayoutPanel();
                rangeTablePanel.Margin = new Padding(0);
                rangeTablePanel.ColumnStyles.Clear();
                rangeTablePanel.RowCount = 1;
                rangeTablePanel.ColumnCount = 2;
                rangeTablePanel.Dock = DockStyle.Fill;
                rangeTablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                value1TextBox.Dock = DockStyle.Fill;
                rangeTablePanel.Controls.Add(value1TextBox);
                rangeTablePanel.SetCellPosition(value1TextBox, new TableLayoutPanelCellPosition(0, 0));
                rangeTablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                value2TextBox.Dock = DockStyle.Fill;
                rangeTablePanel.Controls.Add(value2TextBox);
                rangeTablePanel.SetCellPosition(value2TextBox, new TableLayoutPanelCellPosition(1, 0));
                return rangeTablePanel;
            }

            internal override void EnableSearchValueControls(bool enableSearchControl1, bool enableSearchControl2)
            {
                if (!enableSearchControl1)
                {
                    value1TextBox.Text = string.Empty;
                }
                if (!enableSearchControl2)
                {
                    value2TextBox.Text = string.Empty;
                }
                value1TextBox.Enabled = enableSearchControl1;
                value2TextBox.Enabled = enableSearchControl2;
            }

            internal override bool IsSourceAvailable()
            {
                return true; // Auto suggestion list is not required
            }

            /// <summary>
            /// Creates mol weight search criteria value
            /// </summary>
            /// <returns>return mol weight search criteria value</returns>
            public override string BuildMolWeightSearchValue()
            {
                StringBuilder molWeightSearchValue = new StringBuilder();
                if (base.SelectedOperator.ToString() == FormWizard.Properties.Resources.BETWEEN_OPERATOR)
                {
                    molWeightSearchValue.Append(string.Format(">{0} AND <{1}", SearchValue1, SearchValue2));
                }
                else if (base.SelectedOperator.ToString() == FormWizard.Properties.Resources.EQUAL_OPERAOTR)
                {
                    molWeightSearchValue.Append(SearchValue1);
                }
                else if (base.SelectedOperator.ToString() == FormWizard.Properties.Resources.GREATERTHAN_OPERATOR)
                {
                    molWeightSearchValue.Append(string.Format(">{0}", SearchValue1));
                }
                else if (base.SelectedOperator.ToString() == FormWizard.Properties.Resources.GREATERTHANEQUAL_OPERATOR)
                {
                    molWeightSearchValue.Append(string.Format(">{0}", SearchValue1));
                }
                else if (base.SelectedOperator.ToString() == FormWizard.Properties.Resources.LESSTHAN_OPERATOR || base.SelectedOperator.ToString() == FormWizard.Properties.Resources.LESSTHANEQUAL_OPERATOR)
                {
                    molWeightSearchValue.Append(string.Format("<{0}", SearchValue1));
                }
                return molWeightSearchValue.ToString();
            }

            /// <summary>
            /// Assigns the values to search input controls using the mol weight search value
            /// </summary>
            /// <param name="molWeightSearchValue"></param>
            public override void AssignMolWeightCriteriaDetails(string molWeightSearchValue)
            {
                double min = double.MinValue;
                double max = double.MinValue;
                string value = molWeightSearchValue.Trim();
                if (value.ToUpper().Contains("AND"))
                {
                    string[] _valueRange = value.ToUpper().Replace("AND", ";").Split(new char[] { ';' });
                    if (_valueRange[0].Trim().StartsWith(">"))
                    {
                        double.TryParse(_valueRange[0].Trim().Substring(_valueRange[0].Trim().IndexOf('>') + 1), out min);
                        double.TryParse(_valueRange[1].Trim().Substring(_valueRange[1].Trim().IndexOf('<') + 1), out max);
                        SearchValue1 = min.ToString(CultureInfo.CurrentCulture);
                        SearchValue2 = max.ToString(CultureInfo.CurrentCulture);
                    }
                }
                else if (value.Trim()[0] == '<')
                {
                    double.TryParse(value.Substring(value.IndexOf('<') + 1), out max);
                    SearchValue1 = max.ToString(CultureInfo.CurrentCulture);
                    SearchValue2 = string.Empty;
                }
                else if (value.Trim()[0] == '>')
                {
                    double.TryParse(value.Substring(value.IndexOf('>') + 1), out min);
                    SearchValue1 = min.ToString(CultureInfo.CurrentCulture);
                    SearchValue2 = string.Empty;
                }
                else
                {
                    SearchValue1 = value;
                    SearchValue2 = string.Empty;
                }
            }
        }

        internal class FormulaCriteriaEditor : ValueCriteriaEditor
        {
            TextBox valueTextBox = new TextBox();

            string _value;
            private COEDataView.AbstractTypes abstractTypes;

            public override string SearchValue1
            {
                get
                {
                    _value = valueTextBox.Text;
                    return _value;
                }
                set
                {
                    _value = value;
                    valueTextBox.Text = value;
                }
            }

            public override string SearchValue2
            {
                get
                {
                    return null;
                    //throw new InvalidOperationException("Can not get second value for text criteria search");
                }
                set
                {
                    throw new InvalidOperationException("Can not set second value for text criteria search");
                }
            }

            public FormulaCriteriaEditor(COEDataView.AbstractTypes abstractTypes)
            {
                this.abstractTypes = abstractTypes;
            }

            public override bool Validate()
            {
                if (!string.IsNullOrEmpty(valueTextBox.Text.Trim()))
                {
                    //add validation if any
                }
                return true;
            }

            public override Control Init()
            {
                TableLayoutPanel rangeTablePanel = new TableLayoutPanel();
                rangeTablePanel.Margin = new Padding(0);
                rangeTablePanel.ColumnStyles.Clear();
                rangeTablePanel.RowCount = 1;
                rangeTablePanel.ColumnCount = 1;
                rangeTablePanel.Dock = DockStyle.Fill;
                rangeTablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                valueTextBox.Dock = DockStyle.Fill;
                rangeTablePanel.Controls.Add(valueTextBox);
                rangeTablePanel.SetCellPosition(valueTextBox, new TableLayoutPanelCellPosition(0, 0));
                return rangeTablePanel;
            }

            internal override void EnableSearchValueControls(bool enableSearchControl1, bool enableSearchControl2)
            {
                if (!enableSearchControl1)
                {
                    valueTextBox.Text = string.Empty;
                }
                valueTextBox.Enabled = enableSearchControl1;
            }

            internal override bool IsSourceAvailable()
            {
                return true; // Auto suggestion list is not required
            }
        }

        internal class DateValueCriteriaEditor : ValueCriteriaEditor
        {
            DateTimePicker value1DateTimePicker = new DateTimePicker();
            DateTimePicker value2DateTimePicker = new DateTimePicker();

            string _value1;
            string _value2;

            public override string SearchValue1
            {
                get
                {
                    _value1 = value1DateTimePicker.Value.ToString(CultureInfo.InvariantCulture);
                    return _value1;
                }
                set
                {
                    _value1 = value;
                    DateTime outDt;
                    if (DateTime.TryParse(_value1, CultureInfo.InvariantCulture, DateTimeStyles.None, out outDt))
                    {
                        value1DateTimePicker.Value = outDt;
                    }
                }
            }

            public override string SearchValue2
            {
                get
                {
                    if (value2DateTimePicker.Enabled)
                    {
                        _value2 = value2DateTimePicker.Value.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        _value2 = string.Empty;
                    }
                    return _value2;
                }
                set
                {
                    _value2 = value;
                    DateTime outDt;
                    if (DateTime.TryParse(_value2, CultureInfo.InvariantCulture, DateTimeStyles.None, out outDt))
                    {
                        //enable the second value control to display the between operator
                        value2DateTimePicker.Enabled = true;
                        value2DateTimePicker.Value = outDt;
                    }
                }
            }

            public DateValueCriteriaEditor()
            {
                value1DateTimePicker.Format = DateTimePickerFormat.Short;
                value2DateTimePicker.Format = DateTimePickerFormat.Short;
            }

            public override Control Init()
            {
                TableLayoutPanel rangeTablePanel = new TableLayoutPanel();
                rangeTablePanel.Margin = new Padding(0);
                rangeTablePanel.ColumnStyles.Clear();
                rangeTablePanel.RowCount = 1;
                rangeTablePanel.ColumnCount = 2;
                rangeTablePanel.Dock = DockStyle.Fill;
                rangeTablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                value1DateTimePicker.Dock = DockStyle.Fill;
                rangeTablePanel.Controls.Add(value1DateTimePicker);
                rangeTablePanel.SetCellPosition(value1DateTimePicker, new TableLayoutPanelCellPosition(0, 0));
                rangeTablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                value2DateTimePicker.Dock = DockStyle.Fill;
                rangeTablePanel.Controls.Add(value2DateTimePicker);
                rangeTablePanel.SetCellPosition(value2DateTimePicker, new TableLayoutPanelCellPosition(1, 0));
                return rangeTablePanel;
            }

            public override bool Validate()
            {
                if (value2DateTimePicker.Enabled && (value2DateTimePicker.Value < value1DateTimePicker.Value))
                {
                    errorMessage = "'To' value can not be less than 'From' value";
                    return false;
                }
                return true;
            }

            internal override void EnableSearchValueControls(bool enableSearchControl1, bool enableSearchControl2)
            {
                value1DateTimePicker.Enabled = enableSearchControl1;
                value2DateTimePicker.Enabled = enableSearchControl2;
            }

            internal override bool IsSourceAvailable()
            {
                return true; // Auto suggestion list is not required
            }
        }

        internal class TextValueCriteriaEditor : ValueCriteriaEditor
        {
            TextBox valueTextBox = new TextBox();
            ToolTip tt = new ToolTip();
            bool isSourceRequired = false;
            string _value;

            public override string SearchValue1
            {
                get
                {
                    _value = valueTextBox.Text;
                    return _value;
                }
                set
                {
                    _value = value;
                    valueTextBox.Text = value;
                }
            }

            public override string SearchValue2
            {
                get
                {
                    return null;
                    //throw new InvalidOperationException("Can not get second value for text criteria search");
                }
                set
                {
                    throw new InvalidOperationException("Can not set second value for text criteria search");
                }
            }

            public TextValueCriteriaEditor()
            {

            }

            public override Control Init()
            {
                valueTextBox.Dock = DockStyle.Fill;
                return valueTextBox;
            }

            public override bool Validate()
            {
                //if (string.IsNullOrEmpty(valueTextBox.Text.Trim()))
                //{
                //    return false;
                //}
                return true;
            }

            internal override void EnableSearchValueControls(bool enableSearchControl1, bool enableSearchControl2)
            {
                if (!enableSearchControl1)
                {
                    valueTextBox.Text = string.Empty;
                }
                valueTextBox.Enabled = enableSearchControl1;
            }

            internal override void SetBindingSource(AutoCompleteStringCollection source, string toolTip)
            {
                if (source != null)
                {
                    valueTextBox.AutoCompleteCustomSource = source;
                    valueTextBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    valueTextBox.AutoCompleteMode = AutoCompleteMode.Suggest;
                    isSourceRequired = true;
                }
                else
                {
                    if (!string.IsNullOrEmpty(toolTip))
                    {
                        tt.SetToolTip(valueTextBox, toolTip);
                    }
                    else
                    {
                        tt.SetToolTip(valueTextBox, FormWizard.Properties.Resources.Field_LargeData_Message);
                    }
                    isSourceRequired = true;
                }
            }

            internal override bool IsSourceAvailable()
            {
                return isSourceRequired;
            }
        }
    }

    /// <summary>
    /// A panel that tracks and displays the data for a single selected result criteria field.
    /// </summary>
    internal class ResultsFieldPanel : FieldPanel
    {
        #region Variables
        private ResultsCriteria.SortDirection? _sortDirection;
        private Label labelSortOrder = new Label();
        private ComboBox comboBoxAggregation = new ComboBox();
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the sort direction of the selected result field
        /// </summary>
        public ResultsCriteria.SortDirection? SortDirection
        {
            get
            {
                return this._sortDirection;
            }
            set
            {
                this._sortDirection = value;
                switch (this._sortDirection)
                {
                    case null:
                        labelSortOrder.ImageIndex = 0;
                        break;
                    case ResultsCriteria.SortDirection.ASC:
                        labelSortOrder.ImageIndex = 1;
                        break;
                    case ResultsCriteria.SortDirection.DESC:
                        labelSortOrder.ImageIndex = 2;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the aggregation method of the child result criteria field
        /// </summary>
        public SelectDataForm.AggregationFunction Aggregation
        {
            get
            {
                if (this.comboBoxAggregation == null || comboBoxAggregation.SelectedItem == null)
                {
                    return null;
                }
                else
                {
                    return (SelectDataForm.AggregationFunction)comboBoxAggregation.SelectedItem;
                }
            }
            set
            {
                if (this.comboBoxAggregation != null)
                {
                    this.comboBoxAggregation.SelectedItem = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the table panel associated with result criteria field
        /// </summary>
        public override TablePanel tablePanel
        {
            get
            {
                return _tablePanel;
            }
            set
            {
                this._tablePanel = value;
                if (this._tablePanel == null)
                {
                    ClearOwnerTable();
                }
                else
                {
                    MakeUniqueAlias();
                    TableBO tableBO = this._tablePanel.tableBO;
                    if (tableBO.ID == this._fieldContext.tableBO.ID)
                    {
                        ClearOwnerTable();
                    }
                    else
                    {
                        SetOwnerTable();
                    }
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the ResultsFieldPanel with specified ResultsFieldPanel object
        /// </summary>
        /// <param name="resultsFieldPanel">existing ResultsFieldPanel object</param>
        public ResultsFieldPanel(ResultsFieldPanel resultsFieldPanel)
            : base(resultsFieldPanel)
        {
            this.SortDirection = resultsFieldPanel.SortDirection;
            this.Aggregation = resultsFieldPanel.Aggregation;
        }

        /// <summary>
        /// Initializes a new instance of the ResultsFieldPanel with specified field context
        /// </summary>
        /// <param name="fieldContext">FieldContext object</param>
        public ResultsFieldPanel(SelectDataForm.FieldContext fieldContext)
            : base(fieldContext)
        {
            string lkpStruct = string.Empty;
            if (fieldContext.LookupField != null)
                lkpStruct = fieldBO.Alias + "-";
            if (((FormWizard.SelectDataForm.ResultCriteriaFieldContext)fieldContext).fieldCriteriaType == typeof(ResultsCriteria.MolWeight))
            {
                this.Alias = lkpStruct + "Mol Weight";
            }
            else if (((FormWizard.SelectDataForm.ResultCriteriaFieldContext)fieldContext).fieldCriteriaType == typeof(ResultsCriteria.Formula))
            {
                this.Alias = lkpStruct + "Formula";
            }
            else
            {
                this.Alias = fieldBO.Alias;
            }
            this._sortDirection = null;
            Init();
        }
        #endregion

        public override void Init()
        {
            //increase height to add some padding to fields
            const int rowHeight = 28;

            base.Init();

            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, rowHeight));
            labelSortOrder.Dock = DockStyle.Fill;
            labelSortOrder.ImageAlign = ContentAlignment.MiddleCenter;
            labelSortOrder.ImageIndex = 0;
            this.Controls.Add(labelSortOrder);
            this.SetCellPosition(labelSortOrder, new TableLayoutPanelCellPosition(2, 0));
            labelSortOrder.MouseClick += new MouseEventHandler(labelSortOrder_MouseClick);

            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            labelOwnerTable.AutoEllipsis = true;
            labelOwnerTable.AutoSize = false;
            labelOwnerTable.Dock = DockStyle.Fill;
            labelOwnerTable.TextAlign = ContentAlignment.MiddleLeft;
            this.Controls.Add(labelOwnerTable);
            this.SetCellPosition(labelOwnerTable, new TableLayoutPanelCellPosition(3, 0));

            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            comboBoxAggregation.Items.Clear();
            comboBoxAggregation.Items.AddRange(SelectDataForm.GetAggregationFunctions(this).ToArray());
            comboBoxAggregation.Dock = DockStyle.Fill;
            comboBoxAggregation.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxAggregation.Enabled = false;
            this.Controls.Add(comboBoxAggregation);
            this.SetCellPosition(comboBoxAggregation, new TableLayoutPanelCellPosition(4, 0));

            this.ParentChanged += new EventHandler(ResultsFieldPanel_ParentChanged);

        }

        void ResultsFieldPanel_ParentChanged(object sender, EventArgs e)
        {
            SetForm();
        }

        internal override void SetForm()
        {
            if (this.Form != null)
            {
                base.SetForm();
                this.labelSortOrder.ImageList = this.Form.sortOrderImageList;
            }
        }

        private void ClearOwnerTable()
        {
            // Clear owner table and aggregation control
            labelOwnerTable.Text = "";
            comboBoxAggregation.Enabled = false;
            comboBoxAggregation.SelectedItem = null;
        }

        private void SetOwnerTable()
        {
            // Set owner table label and aggregation control
            labelOwnerTable.Text = this._fieldContext.tableBO.Alias;
            if (this._fieldContext.fieldBO.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE)
            {
                comboBoxAggregation.Enabled = false;
                comboBoxAggregation.SelectedItem = null;
            }
            else
            {
                comboBoxAggregation.Enabled = true;
                if (comboBoxAggregation.SelectedItem == null)
                {
                    comboBoxAggregation.SelectedIndex = 0;
                }
            }
        }

        void labelSortOrder_MouseClick(object sender, MouseEventArgs e)
        {
            // Toggle sort direction.
            switch (this._sortDirection)
            {
                case null:
                    this.SortDirection = ResultsCriteria.SortDirection.ASC;
                    break;
                case ResultsCriteria.SortDirection.ASC:
                    this.SortDirection = ResultsCriteria.SortDirection.DESC;
                    break;
                case ResultsCriteria.SortDirection.DESC:
                    this.SortDirection = null;
                    break;
            }
        }

        /// <summary>
        /// Method to set the context menu items specific to the Result criteria field
        /// </summary>
        internal override void SetContextMenuStripItems()
        {
            //set the default context menu strip
            base.SetContextMenuStripItems();
            //make rename field and sort order menu items visible for result criteria field and set the values accordingly
            this.ContextMenuStrip.Items["renameFieldToolStripTextBox"].Visible = true;
            this.ContextMenuStrip.Items["sortOrderToolStripMenuItem"].Visible = true;

            ((ToolStripTextBox)this.ContextMenuStrip.Items["renameFieldToolStripTextBox"]).Text = Alias;
            ToolStripDropDownItem sortOrderToolStripMenuItem = (ToolStripDropDownItem)this.ContextMenuStrip.Items["sortOrderToolStripMenuItem"];
            ((ToolStripMenuItem)sortOrderToolStripMenuItem.DropDownItems["sortAscendingToolStripMenuItem"]).Checked = SortDirection == ResultsCriteria.SortDirection.ASC;
            ((ToolStripMenuItem)sortOrderToolStripMenuItem.DropDownItems["sortDescendingToolStripMenuItem"]).Checked = SortDirection == ResultsCriteria.SortDirection.DESC;
            ((ToolStripMenuItem)sortOrderToolStripMenuItem.DropDownItems["sortNoneToolStripMenuItem"]).Checked = SortDirection == null;
        }

        internal override bool ValidateControls()
        {
            return base.ValidateControls();
        }
    }

    /// <summary>
    /// A panel that tracks and displays the data for a single selected field.
    /// </summary>
    internal class FieldPanel : TableLayoutPanel
    {
        protected SelectDataForm.FieldContext _fieldContext;
        protected TablePanel _tablePanel;

        private string _alias;
        private Boolean _isNew = true;
        private Point _draggingFrom;

        protected Label labelFieldType = new Label();
        protected Label labelFieldName = new Label();
        protected Label labelOwnerTable = new Label();

        public FieldPanel(SelectDataForm.FieldContext fieldContext)
            : base()
        {
            this._fieldContext = fieldContext;
        }

        public FieldPanel(FieldPanel source)
            : this(source.fieldContext)
        {
            this.Alias = source.Alias;
        }

        public virtual void Init()
        {
            //increase height to add some padding to fields
            const int rowHeight = 28;

            this.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.BackColor = Color.FromKnownColor(KnownColor.Window);
            this.Height = rowHeight;
            this.Margin = new Padding(0);
            this.ColumnStyles.Clear();
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, rowHeight));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));

            this.RowStyles.Clear();
            this.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeight));

            labelFieldType.Dock = DockStyle.Fill;
            labelFieldType.ImageAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(labelFieldType);
            this.SetCellPosition(labelFieldType, new TableLayoutPanelCellPosition(0, 0));
            labelFieldType.MouseDown += new MouseEventHandler(labelFieldType_MouseDown);

            labelFieldName.AutoEllipsis = true;
            labelFieldName.AutoSize = false;
            labelFieldName.Dock = DockStyle.Fill;
            labelFieldName.TextAlign = ContentAlignment.MiddleLeft;
            ApplyFieldName();
            this.Controls.Add(labelFieldName);
            this.SetCellPosition(labelFieldName, new TableLayoutPanelCellPosition(1, 0));
            labelFieldName.MouseDown += new MouseEventHandler(labelFieldName_MouseDown);
            labelFieldName.MouseMove += new MouseEventHandler(labelFieldName_MouseMove);
            labelFieldName.DoubleClick += new EventHandler(labelFieldName_DoubleClick);

            this.ParentChanged += new EventHandler(FieldPanel_ParentChanged);
        }

        public FieldBO fieldBO
        {
            get
            {
                return this._fieldContext.fieldBO;
            }
        }

        public FieldBO lookupField
        {
            get
            {
                return this._fieldContext.LookupField;
            }
        }

        public SelectDataForm.FieldContext fieldContext
        {
            get
            {
                return this._fieldContext;
            }
        }

        public SelectDataForm Form
        {
            get
            {
                return (SelectDataForm)this.FindForm();
            }
        }

        public virtual TablePanel tablePanel
        {
            get
            {
                return this._tablePanel;
            }
            set
            {
                this._tablePanel = value;
            }

        }

        public string Alias
        {
            get
            {
                return this._alias;
            }
            set
            {
                this._alias = value;
                ApplyFieldName();
            }
        }

        public Boolean IsNew
        {
            get
            {
                return this._isNew;
            }
            set
            {
                this._isNew = value;
                ApplyFieldName();
            }
        }

        private void ApplyFieldName()
        {
            if (this.IsNew)
            {
                labelFieldName.Text = this.Alias; //"*" + this.Alias;
            }
            else
            {
                labelFieldName.Text = this.Alias;
            }
        }

        void labelFieldName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this._draggingFrom = e.Location;
            }
        }

        void labelFieldName_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (shouldDoDragDrop(this._draggingFrom, e.Location))
                {
                    DoDragDrop(this, DragDropEffects.Move | DragDropEffects.Copy);
                }
            }
        }

        private static bool shouldDoDragDrop(Point origin, Point cursorPos)
        {
            //	Get the cursor position (as relative to the control's coordinate
            //	system) and determine whether it is within the drag threshold
            Rectangle dragRect = new Rectangle(origin, SystemInformation.DragSize);
            dragRect.X -= SystemInformation.DragSize.Width / 2;
            dragRect.Y -= SystemInformation.DragSize.Height / 2;
            return !dragRect.Contains(cursorPos);
        }

        void labelFieldName_DoubleClick(object sender, EventArgs e)
        {
            this.ContextMenuStrip.Show(this, 0, 0);
            ((ToolStripTextBox)this.ContextMenuStrip.Items["renameFieldToolStripTextBox"]).Focus();
        }

        void labelFieldType_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                DoDragDrop(this, DragDropEffects.Move);
            }
        }

        void FieldPanel_ParentChanged(object sender, EventArgs e)
        {
            SetForm();
        }

        internal virtual void SetForm()
        {
            if (this.Form != null)
            {
                this.ContextMenuStrip = this.Form.selectedFieldContextMenuStrip;
                this.labelFieldType.ImageList = this.Form.availableFieldsImageList;
                this.labelFieldType.ImageIndex = this.Form.GetImageIndex(fieldBO, lookupField);
            }
        }

        public void MakeUniqueAlias()
        {
            string baseAlias = this.Alias;
            int counter = 1;
            while (this.tablePanel.fields.Any(f => f.Alias == this.Alias && f != this))
            {
                this.Alias = baseAlias + (++counter);
            }
        }

        /// <summary>
        /// Method to set the context menu strip items for fields
        /// </summary>
        internal virtual void SetContextMenuStripItems()
        {
        }

        internal virtual bool ValidateControls()
        {
            return true;
        }
    }
}
