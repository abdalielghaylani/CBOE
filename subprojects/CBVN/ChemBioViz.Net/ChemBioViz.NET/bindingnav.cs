using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;

using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Messaging;

using Greatis.FormDesigner;
using Infragistics.Win.UltraWinExplorerBar;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid;

using ChemControls;
using FormDBLib;
using CBVUtilities;
using CBVControls;

namespace ChemBioViz.NET
{
    public partial class ChemBioVizForm : Form
    {
        private bool m_bInMove = false;

        #region BindingNavigator Management

        #region Methods
        /// <summary>
        ///  Add nav to the toolbar
        /// </summary>
        private void AddNavigator()
        {
            CreateBindingNavigator();
            CustomizeNavigator();
            UltraToolbar utb = mainFormUltraToolbarsManager.Toolbars[CBVConstants.TOOLBAR_NAVIGATOR];
            utb.Visible = true;
            utb.Tools["BindingNavigatorControlContainer"].Control = m_bindingNavigator;
        }
        //---------------------------------------------------------------------
        protected void AddNavigatorToChildWnd()
        {
            CreateBindingNavigator();
            CustomizeNavigator();
            this.Controls.Add(m_bindingNavigator);
        }
        //---------------------------------------------------------------------
        private void CustomizeNavigator()
        {
            m_bindingNavigator.Dock = DockStyle.Bottom;
            m_bindingNavigator.AutoSize = true;
            m_bindingNavigator.GripStyle = ToolStripGripStyle.Hidden;
        }
        //---------------------------------------------------------------------
        private void CreateBindingNavigator()
        {
            this.m_bindingNavigator = new CBVBindingNavigator(true);    // CSBR-136866 -- need our own subclass

            ToolStripItem itemA = m_bindingNavigator.Items["bindingNavigatorMovePreviousItem"];
            itemA.Click += new EventHandler(bindingNavigatorMovePreviousItem_Click);

            ToolStripItem itemB = m_bindingNavigator.Items["bindingNavigatorMoveNextItem"];
            itemB.Click += new EventHandler(bindingNavigatorMoveNextItem_Click);

            ToolStripItem itemC = m_bindingNavigator.Items["bindingNavigatorMoveFirstItem"];
            itemC.Click += new EventHandler(bindingNavigatorMoveFirstItem_Click);

            ToolStripItem itemD = m_bindingNavigator.Items["bindingNavigatorMoveLastItem"];
            itemD.Click += new EventHandler(bindingNavigatorMoveLastItem_Click);

            ToolStripTextBox itemP = (ToolStripTextBox)m_bindingNavigator.Items["bindingNavigatorPositionItem"];
            itemP.KeyPress += new KeyPressEventHandler(bindingNavigatorKeyPress);
            itemP.KeyUp += new KeyEventHandler(bindingNavigatorKeyUp);
            itemP.LostFocus += new EventHandler(bindingNavigatorLostFocus);

            m_bindingNavigator.Items.Remove(m_bindingNavigator.Items["bindingNavigatorAddNewItem"]);
            m_bindingNavigator.Items.Remove(m_bindingNavigator.Items["bindingNavigatorDeleteItem"]);
        }
        //---------------------------------------------------------------------
        private void RefreshBindingNavigator()
        {
            if (m_bindingNavigator == null || Pager == null)
            {
                mainFormUltraToolbarsManager.Toolbars[CBVConstants.TOOLBAR_NAVIGATOR].Visible = false;
                return;     // CSBR-117699 .. crashes when form empty
            }
            m_bindingNavigator.Items["bindingNavigatorCountItem"].Text = String.Concat("of ", Pager.ListSize.ToString());
            int absRec = Pager.CurrRow + 1;
            m_bindingNavigator.Items["bindingNavigatorPositionItem"].Text = absRec.ToString();

            CheckEnabling();    // CSBR-118309
        }
        //---------------------------------------------------------------------
        public void FireRecordsetChanged()
        {
            if (RecordsetChanged != null)
            {
                String currRC = this.FormDbMgr.ResultsCriteria.ToString();
                RecordsetChanged.Invoke(this, new RecordsetChangedEventArgs(currRC));
            }
        }
        //---------------------------------------------------------------------
        public void FireFormOpened()
        {
            if (FormOpened != null)
                FormOpened.Invoke(this, new FormOpenedEventArgs());
        }
        //---------------------------------------------------------------------
        public void FireFormEdited()
        {
            if (FormEdited != null)
                FormEdited.Invoke(this, new FormEditedEventArgs());
        }
        //---------------------------------------------------------------------
        public void FireCBVFormClosed()
        {
            if (CBVFormClosed != null)
                CBVFormClosed.Invoke(this, new CBVFormClosedEventArgs());
        }
        //---------------------------------------------------------------------
        internal void DoMove(Pager.MoveType type)
        {
            DoMove(type, 0); // for next, prev, ..., not goto rec
        }
        //---------------------------------------------------------------------
        public void DoMove(Pager.MoveType type, int targetRecno)
        {
            // main routine for changing records
            if (Pager == null)
                return;
            if (m_bindingSource == null)
                return;
            if (m_bInMove)      // CSBR-153391: Prevent reentrancy
                return;
            else
                m_bInMove = true;

            // move pager -- fetch new page if necessary
            // always set datasource and position in case dataset changed
            try
            {
                Pager.Move(type, targetRecno);
                m_bindingSource.DataSource = Pager.CurrDataSet; // might throw if bindings bad
                m_bindingSource.MoveFirst();
                m_bindingSource.Position = Pager.CurrRowInPage;

                // update display of navigator
                RefreshBindingNavigator();

                // select corresponding record on grid
                if (TabManager.CurrentTab is GridViewTab)
                {
                    ChemDataGrid cdGrid = TabManager.CurrentTab.Control as ChemDataGrid;
                    int absRecno = Pager.CurrRow;
                    //Coverity Bug Fix CID :12918 
                    if (cdGrid != null && cdGrid.Rows.Count > absRecno)
                    {
                        UltraGridRow row = cdGrid.Rows[absRecno];
                        cdGrid.ActiveRow = row;
                    }
                }
                // dim arrows if at start or end
                CheckEnabling();

                // if we are in formview and it has plot controls, update (rebind) them
                UpdatePlots(true);

                // also update button labels
                if (TabManager.CurrentTab is FormViewTab)
                {
                    //Coverity fix - CID 12918
                    FormViewControl frmViewControl = TabManager.CurrentTab.Control as FormViewControl;
                    if (frmViewControl != null)
                        frmViewControl.RefreshButtonLabels();
                }
                if (RecordChanged != null)
                    RecordChanged.Invoke(this, new RecordChangedEventArgs(Pager.CurrRow));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("EXCEPTION IN DO MOVE: " + ex.Message);
            }
            m_bInMove = false;
        }
        //---------------------------------------------------------------------
        private void UpdatePlots(bool bHiliteOnly)
        {
            FormViewTab ctrl = TabManager.CurrentTab as FormViewTab;
            if (ctrl != null)
            {
                FormViewControl frmControl = ctrl.Control as FormViewControl;
                //Coverity Bug Fix CID 12919 
                if (frmControl != null)
                {
                    if (!frmControl.HasPlots())
                    {
                        frmControl.UpdatePlots(bHiliteOnly);
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        private void CheckEnabling()
        {
            m_bindingNavigator.Items["bindingNavigatorMovePreviousItem"].Enabled = Pager != null && Pager.CanMove(Pager.MoveType.kmPrev);
            m_bindingNavigator.Items["bindingNavigatorMoveNextItem"].Enabled = Pager != null && Pager.CanMove(Pager.MoveType.kmNext);
            m_bindingNavigator.Items["bindingNavigatorMoveFirstItem"].Enabled = Pager != null && Pager.CanMove(Pager.MoveType.kmFirst);
            m_bindingNavigator.Items["bindingNavigatorMoveLastItem"].Enabled = Pager != null && Pager.CanMove(Pager.MoveType.kmLast);
        }
        #endregion

        #region Events
        private void bindingNavigatorMoveNextItem_Click(object sender, EventArgs e)
        {
            DoMove(Pager.MoveType.kmNext);
        }
        //---------------------------------------------------------------------
        private void bindingNavigatorMovePreviousItem_Click(object sender, EventArgs e)
        {
            DoMove(Pager.MoveType.kmPrev);
        }
        //---------------------------------------------------------------------
        private void bindingNavigatorMoveFirstItem_Click(object sender, EventArgs e)
        {
            DoMove(Pager.MoveType.kmFirst);
        }
        //---------------------------------------------------------------------
        private void bindingNavigatorMoveLastItem_Click(object sender, EventArgs e)
        {
            DoMove(Pager.MoveType.kmLast);
        }
        //---------------------------------------------------------------------
        private void bindingNavigatorKeyPress(object sender, KeyPressEventArgs e)
        {
            // disable action if key is not digit or backspace
            if (!e.KeyChar.Equals(CBVConstants.KEYCHAR_BACK) && !Char.IsDigit(e.KeyChar))
                e.Handled = true;
        }
        //---------------------------------------------------------------------
        void bindingNavigatorLostFocus(object sender, EventArgs e)
        {
            // leaving text box: make sure current record matches what it says in the binding nav text box
            // this is part of the fix for CSBR-136866
            if (this.m_bindingNavigator == null || this.Pager == null)
                return;
            String sCurVal = this.m_bindingNavigator.PositionItem.Text;
            int enteredTargetRec = CBVUtil.StrToInt(sCurVal) - 1;
            int currRec = this.Pager.CurrRow;
            if (currRec != enteredTargetRec)
                DoMove(Pager.MoveType.kmGoto, enteredTargetRec);
        }
        //---------------------------------------------------------------------
        void bindingNavigatorKeyUp(object sender, KeyEventArgs e)
        {
            // user hit CR: proceed as for lost focus
            if (e.KeyValue == CBVConstants.KEYVALUE_ENTER)
                bindingNavigatorLostFocus(sender, e);
        }
        //---------------------------------------------------------------------
        #endregion
        #endregion
    }
}
