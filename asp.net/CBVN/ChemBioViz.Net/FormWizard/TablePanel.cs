using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.COEDataViewService;
using Infragistics.Win.Misc;
using CambridgeSoft.COE.Framework.Common;
using System.Text;

namespace FormWizard
{
    /// <summary>
    /// A collapsible panel that displays a single selected table along with its fields.
    /// </summary>
    internal class TablePanel : UltraExpandableGroupBox
    {
        private TableBO _tableBO;
        public IList<FieldPanel> fields = new List<FieldPanel>();
        private FlowLayoutPanel fieldsPanel = new FlowLayoutPanel();
        private Label sizeLabel = new Label();
        private Point _draggingFrom;
        private bool isAddingTable = false;
        private bool isFieldAdded = false;

        /// <summary>
        /// Gets or sets whether table is being added
        /// </summary>
        public bool IsAddingTable
        {
            get { return isAddingTable; }
            set { isAddingTable = value; }
        }

        public ErrorProvider ErrorProvider
        {
            get { return this.Form.TheErrorProvider; }
        }

        public TablePanel(TableBO tableBO)
            : base()
        {
            this._tableBO = tableBO;
            this.SizeChanged += new EventHandler(TablePanel_SizeChanged);
            this.ParentChanged += new EventHandler(TablePanel_ParentChanged);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            this.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.ContentAreaAppearance.BackColor = Color.FromKnownColor(KnownColor.Window);
            this.HeaderClickAction = GroupBoxHeaderClickAction.None;
            this.ShowFocus = false;
            this.Text = tableBO.Alias;
            this.ViewStyle = GroupBoxViewStyle.Office2007;
            //set width of the table panel as parent panel width
            this.Width = this.Parent.Width;

            ResizeMe();
            
            this.Panel.Controls.Add(fieldsPanel);
            fieldsPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            fieldsPanel.FlowDirection = FlowDirection.TopDown;
            fieldsPanel.Padding = new Padding(0);
            fieldsPanel.Width = this.Panel.Width;
            fieldsPanel.WrapContents = false;

            fieldsPanel.Controls.Add(this.sizeLabel);
            this.sizeLabel.Margin = new Padding(0);
            this.sizeLabel.Size = new Size(this.Width, 0);

            foreach (FieldPanel fieldPanel in this.fields)
            {
                fieldPanel.SetForm();
            }

            this.MouseDown += new MouseEventHandler(TablePanel_MouseDown);
            this.MouseMove += new MouseEventHandler(TablePanel_MouseMove);
        }

        public TablePanel(TablePanel source)
            : this(source._tableBO)
        {
            int index = 0;
            foreach (FieldPanel sourceField in source.fields)
            {
                FieldPanel copyField = new FieldPanel(sourceField);
                InsertFieldPanel(copyField, index++);
            }
        }

        public override Point AutoScrollOffset
        {
            get
            {
                // Keep the panel at its current location when ScrollControlIntoView
                // is called to prevent the scroll bar from jumping around when the focus
                // changes.
                return this.Location;
            }
        }

        /// <summary>
        /// Gets the table associated with this TablePanel.
        /// </summary>
        public TableBO tableBO
        {
            get
            {
                return this._tableBO;
            }
        }

        public SelectDataForm Form
        {
            get
            {
                return (SelectDataForm)this.FindForm();
            }
        }

        public bool ContainsField(SelectDataForm.FieldContext fieldContext)
        {
            if (fieldContext.GetType() == typeof(SelectDataForm.ResultCriteriaFieldContext))
            {
                foreach (FieldPanel fieldPanel in this.fields)
                {
                    if (fieldPanel.fieldContext.tableBO.ID == fieldContext.tableBO.ID &&
                        fieldPanel.fieldContext.fieldBO.ID == fieldContext.fieldBO.ID)
                    {
                        Type type1 = ((SelectDataForm.ResultCriteriaFieldContext)fieldPanel.fieldContext).fieldCriteriaType;
                        Type type2 = ((SelectDataForm.ResultCriteriaFieldContext)fieldContext).fieldCriteriaType;
                        // Don't add structure fields if they're already there,
                        // even if they use different results criteria elements.
                        if (type1 == type2 ||
                            type1 == typeof(ResultsCriteria.Field) && type2 == typeof(ResultsCriteria.HighlightedStructure) ||
                            type1 == typeof(ResultsCriteria.HighlightedStructure) && type2 == typeof(ResultsCriteria.Field))
                        {
                            return true;
                        }
                    }
                }
            }
            else if (fieldContext.GetType() == typeof(SelectDataForm.QueryCriteriaFieldContext))
            {
                foreach (FieldPanel fieldPanel in this.fields)
                {
                    if (fieldPanel.fieldContext.tableBO.ID == fieldContext.tableBO.ID &&
                        fieldPanel.fieldContext.fieldBO.ID == fieldContext.fieldBO.ID)
                    {
                        Type type1 = ((SelectDataForm.QueryCriteriaFieldContext)fieldPanel.fieldContext).fieldCriteriaType;
                        Type type2 = ((SelectDataForm.QueryCriteriaFieldContext)fieldContext).fieldCriteriaType;
                        // Don't add structure fields if they're already there,
                        // even if they use different results criteria elements.
                        if (type1 == type2)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public FieldPanel AppendField(SelectDataForm.FieldContext fieldContext)
        {
            return InsertField(fieldContext, this.fields.Count);
        }

        /// <summary>
        /// Inserts a field into the panel at the given index and returns out value confirming that field is added
        /// </summary>
        /// <param name="fieldContext">The field to insert</param>
        /// <param name="index">The index to insert at</param>
        /// <param name="isFieldAdded">boolean value determines if the field is added or already exists</param>
        /// <returns>The inserted FieldPanel</returns>
        public FieldPanel InsertField(SelectDataForm.FieldContext fieldContext, int index, out bool isFieldAdded)
        {
            FieldPanel fieldPanel;
            fieldPanel = InsertField(fieldContext, index);
            isFieldAdded = this.isFieldAdded;
            return fieldPanel;
        }

        /// <summary>
        /// Inserts a field into the panel at the given index.
        /// </summary>
        /// <param name="fieldContext">The field to insert</param>
        /// <param name="index">The index to insert at</param>
        /// <returns>The inserted FieldPanel</returns>
        public FieldPanel InsertField(SelectDataForm.FieldContext fieldContext, int index)
        {
            FieldPanel fieldPanel;
            isFieldAdded = true;
            if (fieldContext.GetType() == typeof(SelectDataForm.ResultCriteriaFieldContext))
            {
                fieldPanel = new ResultsFieldPanel(fieldContext);
            }
            else
            {
                //for query criteria field, first try to find if the field already exists on the query tab. 
                //If field already exist then display message to user, otherwise add a new field on the query tab.
                fieldPanel = TryGetFieldPanel(fieldContext);
                if (fieldPanel == null)
                {
                    fieldPanel = new QueryFieldPanel(fieldContext);
                }
                else
                {
                    if (!isAddingTable)
                    {
                        MessageBox.Show(string.Format(FormWizard.Properties.Resources.QueryCriteria_Field_Validation, MakeDuplicateFieldDisplayString(fieldContext)), FormWizard.Properties.Resources.FORM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    isFieldAdded = false;
                    return fieldPanel;
                }
            }
            InsertFieldPanel(fieldPanel, index);
            return fieldPanel;
        }

        /// <summary>
        /// Generate duplicate field adding message string
        /// </summary>
        /// <param name="fieldContext">field context object</param>
        /// <returns>returns string in tableName.FieldName format</returns>
        string MakeDuplicateFieldDisplayString(SelectDataForm.FieldContext fieldContext)
        {
            StringBuilder strTableFieldDisplayName = new StringBuilder();
            strTableFieldDisplayName.Append(string.Format("{0}.", fieldContext.tableBO.Alias));
            string lkpStruct = string.Empty;
            if (fieldContext.LookupField != null)
                lkpStruct = fieldContext.fieldBO.Alias + "-";
            if (fieldContext.fieldCriteriaType == typeof(SearchCriteria.CSMolWeightCriteria))
            {
                strTableFieldDisplayName.Append(lkpStruct + "Mol Weight");
            }
            else if (fieldContext.fieldCriteriaType == typeof(SearchCriteria.CSFormulaCriteria))
            {
                strTableFieldDisplayName.Append(lkpStruct + "Formula");
            }
            else
            {
                strTableFieldDisplayName.Append(fieldContext.fieldBO.Alias);
            }
            return strTableFieldDisplayName.ToString();
        }

        /// <summary>
        /// Method to get the FieldPanel object for specified fieldContext
        /// </summary>
        /// <param name="fieldContext">fieldContext object for which the field panel is required</param>
        /// <returns>returns the FieldPanel corresponding to the specified fieldContext object</returns>
        public FieldPanel TryGetFieldPanel(SelectDataForm.FieldContext fieldContext)
        {
            FieldPanel theFieldPanel = null;
            //get parent of current table panel
            Panel qcFieldsPanel = this.Parent as Panel;
            if (qcFieldsPanel != null)
            {
                //loop through each table panel on parent panel to determine if field is already added
                foreach (Control tablePanelCtrl in qcFieldsPanel.Controls)
                {
                    if (tablePanelCtrl is TablePanel)
                    {
                        TablePanel tblPanel = (TablePanel)tablePanelCtrl;
                        foreach (var fldPanel in tblPanel.fields)
                        {
                            if (fldPanel.fieldContext.tableId == fieldContext.tableBO.ID && fldPanel.fieldBO.ID == fieldContext.fieldBO.ID)
                            {
                                //check for field criteria type to enable adding formula, mol weight fields on query tab
                                if (fldPanel.fieldContext.fieldCriteriaType == fieldContext.fieldCriteriaType)
                                {
                                    theFieldPanel = fldPanel;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return theFieldPanel;
        }

        /// <summary>
        /// Inserts a FieldPanel into the TablePanel at the given index.
        /// </summary>
        /// <param name="fieldPanel">The FieldPanel to insert</param>
        /// <param name="index">The index to insert at</param>
        public void InsertFieldPanel(FieldPanel fieldPanel, int index)
        {
            fieldPanel.tablePanel = this;
            this.fieldsPanel.Controls.Add(fieldPanel);
            this.fieldsPanel.Controls.SetChildIndex(fieldPanel, index);
            this.fields.Insert(index, fieldPanel);
            ResizeMe();
        }

        /// <summary>
        /// Moves a FieldPanel to another index within the TablePanel.
        /// </summary>
        /// <param name="fieldPanel">The FieldPanel to move</param>
        /// <param name="index">The index to move the FieldPanel to</param>
        public void MoveField(FieldPanel fieldPanel, int index)
        {
            this.fieldsPanel.Controls.SetChildIndex(fieldPanel, index);
            this.fields.Remove(fieldPanel);
            this.fields.Insert(index, fieldPanel);
        }

        /// <summary>
        /// Removes a FieldPanel from the TablePanel.
        /// </summary>
        /// <param name="fieldPanel">The FieldPanel to remove</param>
        public void RemoveField(FieldPanel fieldPanel)
        {
            this.fieldsPanel.Controls.Remove(fieldPanel);
            this.fields.Remove(fieldPanel);
            ResizeMe();
        }

        /// <summary>
        /// Updates the height of the TablePanel to fit its contents.
        /// </summary>
        private void ResizeMe()
        {
            if (this.Panel != null)
            {
                int panelHeight = 0;
                foreach (FieldPanel field in this.fields)
                {
                    panelHeight += field.Height;
                }
                this.Expanded = true;
                this.Height = panelHeight + this.Height - this.Panel.Height;
                this.fieldsPanel.Height = panelHeight;
            }
        }

        /// <summary>
        /// Finds the FieldPanel displayed at a given point.
        /// </summary>
        /// <param name="p">The point in screen coordinates</param>
        /// <returns>The FieldPanel displayed at that point, if any</returns>
        public FieldPanel GetFieldUnderFromScreenPoint(Point p)
        {
            Point clientPos = this.fieldsPanel.PointToClient(p);
            foreach (FieldPanel fieldPanel in this.fields)
            {
                if (fieldPanel.Bounds.Contains(clientPos))
                {
                    return fieldPanel;
                }
            }
            return null;
        }

        /// <summary>
        /// Updates the width of the items in the TablePanel when the size of the TablePanel changes.
        /// </summary>
        void TablePanel_SizeChanged(object sender, EventArgs e)
        {
            if (this.Panel != null)
            {
                this.sizeLabel.Width = this.Panel.Width;
            }
        }

        /// <summary>
        /// Begins dragging the TablePanel when the left mouse button is pressed over the panel's title bar.
        /// </summary>
        void TablePanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //DoDragDrop(this, DragDropEffects.Move);
                this._draggingFrom = e.Location;
            }
        }

        void TablePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._draggingFrom != null && e.Button == MouseButtons.Left)
            {
                Point p = new Point(e.Location.X - this._draggingFrom.X, e.Location.Y - this._draggingFrom.Y);
                if (p.X * p.X + p.Y * p.Y >= 25)
                {
                    DoDragDrop(this, DragDropEffects.Move);
                }
            }
        }

        void TablePanel_ParentChanged(object sender, EventArgs e)
        {
            if (this.Form != null)
            {
                this.ContextMenuStrip = this.Form.selectedTableContextMenuStrip;
            }
        }
    }
}
