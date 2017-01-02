using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ChemBioViz.NET;
using FormDBLib;
using CBVControls;
using Utilities;
using CBVUtilities;

namespace ChemBioViz.NET
{
    public partial class ButtonPropsDialog : Form
    {
        #region Variables and Enums
        private CBVButtonProps m_buttonProps;
        private CBVButton m_button;
        private static TextBox activeTextBox = null;
        private enum RadioChoice { krCurrField, krField, krVariable };
        private bool m_bModified;
        private bool m_bNoDialogOnSelect;
        #endregion

        #region Constructors
        public ButtonPropsDialog(CBVButtonProps buttonProps, CBVButton button)
        {
            m_bNoDialogOnSelect = true;
            InitializeComponent();
            m_button = button;
            m_buttonProps = buttonProps;

            argTextBox.Text = buttonProps.ActionArgs;
            displayTextBox.Text = buttonProps.DisplayLabel;
            currFieldTextBox.Text = GetCurrBoundFieldName();
            tooltipTextBox.Text = buttonProps.TooltipText;

            FillActionCombo();
            actionComboBox.SelectedIndex = (int)buttonProps.ActionType;
            FillFieldnameCombo();
            FillVariablesCombo();
            activeTextBox = argTextBox.Focused ? argTextBox : displayTextBox.Focused ? displayTextBox :
                            tooltipTextBox.Focused ? tooltipTextBox : null;
            SetRadio(String.IsNullOrEmpty(currFieldTextBox.Text) ? RadioChoice.krField : RadioChoice.krCurrField);

            CheckButtons();
            CenterToParent();

            m_bModified = false;
            m_bNoDialogOnSelect = false;
        }
        #endregion

        #region Methods
        private void SetRadio(RadioChoice rc)
        {
            currFieldRadioButton.Checked = rc == RadioChoice.krCurrField;
            dataFieldRadioButton.Checked = rc == RadioChoice.krField;
            variableRadioButton.Checked = rc == RadioChoice.krVariable;
        }
        //---------------------------------------------------------------------
        private void CheckButtons()
        {
            bool bHasNoInsertable = dataFieldComboBox.SelectedIndex == -1 && variableComboBox.SelectedIndex == -1;
            if (currFieldRadioButton.Checked && !String.IsNullOrEmpty(currFieldTextBox.Text))
                bHasNoInsertable = false;

            bool bDimInsert = activeTextBox == null || bHasNoInsertable;
            this.insertButton.Enabled = !bDimInsert;

            bool bCantSelCurrField = String.IsNullOrEmpty(currFieldTextBox.Text) || CBVUtil.Eqstrs(currFieldTextBox.Text, "(unbound)");
            if (currFieldRadioButton.Checked && bCantSelCurrField)
                SetRadio(RadioChoice.krField);
            currFieldRadioButton.Enabled = !bCantSelCurrField;
        }
        //---------------------------------------------------------------------
        private void SetDesignerDirty()
        {
            // tell the designer it's modified
            FormViewControl fvc = (m_button == null) ? null : m_button.Parent.Parent.Parent.Parent as FormViewControl;
            if (fvc != null)
                fvc.Designer.SetDirty();
        }
        //---------------------------------------------------------------------
        private String GetCurrBoundFieldName()
        {
            Debug.Assert(m_button != null);
            return m_button.BoundFieldName;
        }
        //---------------------------------------------------------------------
        private void FillActionCombo()
        {
            ChemBioVizForm form1 = m_button.TopLevelControl as ChemBioVizForm;
            //Coverity Bug Fix CID 12921 
            if (form1 != null)
            {
                for (int i = 0; i < 99; ++i)
                {
                    String typeNameI = CBVButton.ActionTypeName((CBVButton.ActionType)i);
                    if (String.IsNullOrEmpty(typeNameI)) break;
                    if (CBVUtil.StartsWith(typeNameI, "Add"))
                    {
                        if (!form1.FeatEnabler.CanUseAddins()) continue;
                        typeNameI += "...";
                    }
                    actionComboBox.Items.Add(typeNameI);
                }
            }
        }
        //---------------------------------------------------------------------
        private void FillFieldnameCombo()
        {
            ChemBioVizForm form1 = m_button.TopLevelControl as ChemBioVizForm;
            bool bWithChild = true, bWithGrandchild = true;
            //Coverity Bug Fix CID 
            if (form1 != null)
            {
                List<String> fieldNames = form1.FormDbMgr.GetFieldNames(bWithChild, bWithGrandchild);
                foreach (String s in fieldNames)
                    this.dataFieldComboBox.Items.Add(s);
            }
        }
        //---------------------------------------------------------------------
        private void FillVariablesCombo()
        {
            List<String> names = FormUtil.GetInternalVariableNames();
            foreach (String s in names)
                this.variableComboBox.Items.Add(s);
        }
        #endregion

        #region Events
        private void OKbutton_Click(object sender, EventArgs e)
        {
            m_buttonProps.ActionArgs = argTextBox.Text;
            m_buttonProps.DisplayLabel = displayTextBox.Text;
            m_buttonProps.TooltipText = tooltipTextBox.Text;
            m_buttonProps.ActionType = (CBVButton.ActionType)actionComboBox.SelectedIndex;

            if (m_bModified)
                SetDesignerDirty(); // CSBR-128735
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        //---------------------------------------------------------------------
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //---------------------------------------------------------------------
        private void DoAddinsDialog()
        {
            ChemBioVizForm form1 = m_button.TopLevelControl as ChemBioVizForm;
            //Coverity Bug FIx CID :12920 
            if (form1 != null)
            {
                CBVAddinsManager mgr = form1.AddinsManager;

                if (mgr != null && mgr.Addins.Count > 0)
                {
                    AddinsDialog dlg = new AddinsDialog(mgr);
                    DialogResult dResult = dlg.ShowDialog();
                    if (dResult == DialogResult.OK)
                    {
                        String sSelTypeName = dlg.GetSelectedItem();
                        argTextBox.Text = sSelTypeName;
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        private void actionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            String actionMessage = CBVButton.ActionDescription((CBVButton.ActionType)actionComboBox.SelectedIndex);
            this.descrTextBox.Text = actionMessage;
            m_bModified = true;

            String actionTypeName = CBVButton.ActionTypeName((CBVButton.ActionType)actionComboBox.SelectedIndex);
            if (!m_bNoDialogOnSelect && CBVUtil.StartsWith(actionTypeName, "Add"))
            {
                // user chose Addins... bring up dialog and get choice
                DoAddinsDialog();
            }
        }
        //---------------------------------------------------------------------
        private void insertButton_Click(object sender, EventArgs e)
        {
            String textToInsert = "";
            if (currFieldRadioButton.Checked)
                textToInsert = "%%";
            else if (dataFieldRadioButton.Checked)
                textToInsert = String.Concat("%", dataFieldComboBox.SelectedItem, "%");
            else if (variableRadioButton.Checked)
                textToInsert = String.Concat("%", variableComboBox.SelectedItem, "%");

            if (activeTextBox != null && !String.IsNullOrEmpty(textToInsert))
            {
                activeTextBox.SelectedText = textToInsert;
                activeTextBox.Focus();  // remove focus from button after use
                m_bModified = true;
            }
        }
        //---------------------------------------------------------------------
        private void argTextBox_Enter(object sender, EventArgs e)
        {
            activeTextBox = argTextBox;
            CheckButtons();
            m_bModified = true;
        }
        //---------------------------------------------------------------------
        private void argTextBox_Leave(object sender, System.EventArgs e)
        {
            if (!this.insertButton.Focused)
                activeTextBox = null;
            CheckButtons();
        }
        //---------------------------------------------------------------------
        private void displayTextBox_Enter(object sender, EventArgs e)
        {
            activeTextBox = displayTextBox;
            CheckButtons();
            m_bModified = true;
        }
        //---------------------------------------------------------------------
        private void displayTextBox_Leave(object sender, System.EventArgs e)
        {
            if (!this.insertButton.Focused) // do not disable active box when clicking on Insert
                activeTextBox = null;
            CheckButtons();
        }
        //---------------------------------------------------------------------
        private void tooltipTextBox_Enter(object sender, EventArgs e)
        {
            activeTextBox = tooltipTextBox;
            CheckButtons();
            m_bModified = true;
        }
        //---------------------------------------------------------------------
        private void tooltipTextBox_Leave(object sender, System.EventArgs e)
        {
            if (!this.insertButton.Focused)
                activeTextBox = null;
            CheckButtons();
        }
        //---------------------------------------------------------------------
        private void currFieldRadioButton_Click(object sender, EventArgs e)
        {
            CheckButtons();
        }
        //---------------------------------------------------------------------
        private void dataFieldRadioButton_Click(object sender, EventArgs e)
        {
            CheckButtons();
        }
        //---------------------------------------------------------------------
        private void variableRadioButton_Click(object sender, EventArgs e)
        {
            CheckButtons();
        }
        //---------------------------------------------------------------------
        private void dataFieldComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetRadio(RadioChoice.krField);
            CheckButtons();
            m_bModified = true;
        }
        //---------------------------------------------------------------------
        private void variableComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetRadio(RadioChoice.krVariable);
            CheckButtons();
            m_bModified = true;
        }
        //---------------------------------------------------------------------
        #endregion
    }
}
