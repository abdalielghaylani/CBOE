using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Windows.Forms.Layout;

using Greatis.FormDesigner;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.COEExportService;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COESecurityService;

using Infragistics.Win;
using Infragistics.Win.UltraWinExplorerBar;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinDock;
using Infragistics.Win.UltraWinTree;

using ChemControls;
using FormDBLib;
using CBVUtilities;
using CBVControls;
using Utilities;
using ChemBioViz.NET.Exceptions;
using FormDBLib.Exceptions;
using System.Drawing.Printing;
using Infragistics.Win.Printing;
using Infragistics.Win.UltraWinToolTip;
using SearchPreferences;

namespace ChemBioViz.NET
{
    public partial class ChemBioVizForm : Form
    {
        #region Trees event handlers
        /// <summary>
        ///  Dim Tree context menu
        /// </summary>
        private void DimTreeContextMenu()
        {
            // prepare both form and query tree context menus based on clicked node
            // this is a lousy way to handle this -- should use OnContextMenuOpening routines
            ITreeNode sNode = null;

            TreeConfig treeConf = null;
            if (ActivePaneName.Equals(CBVConstants.PUBLIC_GROUPNAME) || ActivePaneName.Equals(CBVConstants.PRIVATE_GROUPNAME))
                treeConf = m_FTreeConf;
            else if (ActivePaneName.Equals(CBVConstants.QUERIES_GROUPNAME))
                treeConf = m_QTreeConf;

            string nodeKey = GetTreeNodeKey();
            //Coverity Bug Fix : CID 12994 
            if (treeConf != null && !string.IsNullOrEmpty(nodeKey))
                sNode = treeConf.MainTree.GetNodeFromListByKey(nodeKey);

            // remove all items and return if no node selected
            foreach (ToolStripItem item in this.FormContextMenuStrip.Items)
                item.Available = (sNode != null);
            foreach (ToolStripItem item in this.queryContextMenuStrip.Items)
                item.Available = (sNode != null);
            if (sNode == null)
                return;

            // Sometimes we need to hide items (not dim them)
            if (sNode is TreeLeaf)
            {
                this.FormContextMenuStrip.Items[CBVConstants.CMENU_NEWFOLDER].Available = false;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_NEWFOLDER].Available = false;

                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_RESTORE_HITLIST].Available = true;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_RERUN].Available = true;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_RESTORE].Available = true;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_RUN_ON_OPENING].Available = true;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_PROPERTIES].Available = true;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_SRCHOVERTHIS].Available = true;

                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_SEPARATOR1].Available = true;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_SEPARATOR2].Available = true;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_SEPARATOR3].Available = true;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_SEPARATOR4].Available = true;
            }
            else
            {
                // Any user could create their own tree structure
                this.FormContextMenuStrip.Items[CBVConstants.CMENU_NEWFOLDER].Available = true;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_NEWFOLDER].Available = true;

                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_RESTORE_HITLIST].Available = false;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_RERUN].Available = false;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_RESTORE].Available = false;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_RUN_ON_OPENING].Available = false;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_KEEP].Available = false;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_KEEP_ALL].Available = false;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_PROPERTIES].Available = false;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_SRCHOVERTHIS].Available = false;

                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_SELECT_FOR_MERGE].Available = false;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_MERGE_WITH].Available = false;

                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_SEPARATOR1].Available = false;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_SEPARATOR2].Available = false;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_SEPARATOR3].Available = false;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_SEPARATOR4].Available = false;
            }

            // check for disabled features
            if (!this.FeatEnabler.CanCreateTreeFolder())
            {
                this.FormContextMenuStrip.Items[CBVConstants.CMENU_NEWFOLDER].Available = false;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_NEWFOLDER].Available = false;
            }
            if (!this.FeatEnabler.CanSearchOverCurrList())
            {
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_SRCHOVERTHIS].Available = false;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_SEPARATOR8].Available = false;
            }

            // check privilege for making changes to Public Form trees
            if (!FormDbMgr.PrivilegeChecker.CanSavePublic && IsPublicPaneActive)
            {
                this.FormContextMenuStrip.Items[CBVConstants.CMENU_RENAME].Enabled = false;
                this.FormContextMenuStrip.Items[CBVConstants.CMENU_REMOVE].Enabled = false;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_RENAME].Enabled = false;
                this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_DELETE].Enabled = false;
            }
            else
            {
                if (treeConf != null && treeConf.IsRWord(sNode.Name))
                {
                    // Cannot remove or rename predefined nodes
                    this.FormContextMenuStrip.Items[CBVConstants.CMENU_RENAME].Enabled = false;
                    this.FormContextMenuStrip.Items[CBVConstants.CMENU_REMOVE].Enabled = false;
                    this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_RENAME].Enabled = false;
                    this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_DELETE].Enabled = false;
                }
                else
                {
                    this.FormContextMenuStrip.Items[CBVConstants.CMENU_RENAME].Enabled = true;
                    this.FormContextMenuStrip.Items[CBVConstants.CMENU_REMOVE].Enabled = true;
                    this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_RENAME].Enabled = true;
                    this.queryContextMenuStrip.Items[CBVConstants.CMENU_Q_DELETE].Enabled = true;
                }
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///   Choose forms from nav panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param na
        /// me="e"></param>
        private void formTree_DblClick(object sender, EventArgs e)
        {
            m_tooltipManager.HideToolTip();
            UltraTree treeView = (UltraTree)sender;
            UltraTreeNode selNode = treeView.ActiveNode;

            Point cursorPos = treeView.PointToClient(Cursor.Position);
            if (!treeView.DisplayRectangle.Contains(cursorPos)) // CSBR-128304
                return;


            if (!m_FTreeConf.IsFolder(selNode.Key))
            {
                if (IsEditingForm()) EndFormEdit();
                if (!CheckForSaveOnClose())
                    return;

                // double-click on the tree
                //CBVTimer.StartTimer(true, "Choose form from group", true);
                //CBVUtil.BeginUpdate(this);

                int formID = m_FTreeConf.MainTree.GetObjectID(selNode.Key);
                if (formID >= 0)
                {
                    CBVUtil.BeginUpdate(this);
                    string sXml = this.ActiveDBBank.Retrieve(formID);
                    if (string.IsNullOrEmpty(sXml))
                        MessageBox.Show(String.Concat("Unable to retrieve form ", selNode.Text), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        // TO DO: make a method that stores Form Info in an obj for current form
                        this.UnloadForm();
                        this.m_formType = IsPublicPaneActive ? formType.Public : formType.Private;
                        if (LoadForm(sXml, selNode.Text, formID))
                            this.FormName = selNode.Text;
                    }
                    CBVUtil.EndUpdate(this);
                    if (BindingNavigator != null)
                        BindingNavigator.Refresh();

                    CBVStatMessage.ShowReadyMsg();
                }
            }
        }
        //---------------------------------------------------------------------
        private void tree_MouseDown(object sender, MouseEventArgs e)
        {
            m_lastMouseDownPoint = new Point(e.X, e.Y);
            if (e.Button == MouseButtons.Right)
            {
                DimTreeContextMenu();
            }
        }
        //---------------------------------------------------------------------
        private void tree_KeyDown(object sender, KeyEventArgs e)
        {
            m_tooltipManager.HideToolTip();
            UltraTree tree = this.GetTreeFromGroup(ActivePaneName);
            m_lastMouseDownPoint = Point.Empty; // invalidate, allows to get the last active node (when navigating with arrow keys)
            if (e.KeyCode == Keys.Delete)
                DeleteNodeMenuItem_Click(sender, e);
        }
        //---------------------------------------------------------------------
        private void tree_BeforeLabelEdit(object sender, CancelableNodeEventArgs e)
        {
            TreeConfig treeConf = null;
            m_isPublicPane = ActivePaneName.Equals(CBVConstants.PUBLIC_GROUPNAME); // storing previous activated pane.
            if (ActivePaneName.Equals(CBVConstants.PUBLIC_GROUPNAME) || ActivePaneName.Equals(CBVConstants.PRIVATE_GROUPNAME))
                treeConf = m_FTreeConf;
            else if (ActivePaneName.Equals(CBVConstants.QUERIES_GROUPNAME))
                treeConf = m_QTreeConf;

            string treeNodeText = string.Empty;
            if (e.TreeNode.Text.ToLower().StartsWith(CBVConstants.RETRIEVE_ALL.ToLower()))
                treeNodeText = CBVConstants.RETRIEVE_ALL;
            else
                treeNodeText = e.TreeNode.Text;

            // Check if user has privileges or if it's a predefined node
            // CSBR-129136: prefix with !
            //Coverity Bug Fix CID 12998 
            if (treeConf != null && ((!FormDbMgr.PrivilegeChecker.CanSavePublic && IsPublicPaneActive) || treeConf.IsRWord(treeNodeText)))
            {
                e.Cancel = true;
                if (treeNodeText.ToLower().Equals(CBVConstants.RETRIEVE_ALL.ToLower()))
                    FormDbMgr.PrivilegeChecker.ShowRestrictionMessage(CBVConstants.CAN_RENAME, false);
                else
                    FormDbMgr.PrivilegeChecker.ShowRestrictionMessage(CBVConstants.CAN_SAVE_PUBLIC_FORM, false);
            }
        }
        //---------------------------------------------------------------------
        private void tree_AfterLabelEdit(object sender, NodeEventArgs e)
        {
            TreeConfig treeConf = null;
            bool m_isSameActivePane = m_isPublicPane && ActivePaneName.Equals(CBVConstants.PUBLIC_GROUPNAME); // CSBR-164773, Checking whether before and after edit Activation pane is same or not.
            if (m_isSameActivePane)
            {
                if (ActivePaneName.Equals(CBVConstants.PUBLIC_GROUPNAME) || ActivePaneName.Equals(CBVConstants.PRIVATE_GROUPNAME))
                    treeConf = m_FTreeConf;
                else if (ActivePaneName.Equals(CBVConstants.QUERIES_GROUPNAME))
                    treeConf = m_QTreeConf;
            }
            //Coverity Bug Fix CID :18695 
            if (treeConf != null && treeConf.MainTree != null)
            {
                ITreeNode sNode = treeConf.MainTree.GetNodeFromListByKey(e.TreeNode.Key);
                if (sNode != null)
                {
                    string nodeName = sNode.Name;
                    string newName = e.TreeNode.Text;

                    UltraTree tree = GetTreeFromGroup(ActivePaneName);
                    UltraTreeNode sUINode = tree.GetNodeByKey(GetTreeNodeKey());
                    // Verify the source node is not a predefined node (like root nodes) 
                    if (!treeConf.IsRWord(nodeName))
                    {
                        RenameTreeNode(treeConf, sNode, nodeName, newName, sUINode);
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder("You cannot use");
                        sb.AppendFormat(" \"{0}\". It's a reserved word. Specify another name.", newName);
                        MessageBox.Show(sb.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        // Restore previous value
                        UpdateNodeInPanel(sUINode, nodeName);
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Rename any tree node 
        /// </summary>
        /// <param name="treeConf"></param>
        /// <param name="sNode"></param>
        /// <param name="nodeName"></param>
        /// <param name="newName"></param>
        /// <param name="sUINode"></param>
        private void RenameTreeNode(TreeConfig treeConf, ITreeNode sNode, string nodeName, string newName, UltraTreeNode sUINode)
        {
            // Verify the new name is not a reserved word, not null and not equal to the previous one
            if (!treeConf.IsRWord(newName) && !string.IsNullOrEmpty(newName) && newName.CompareTo(nodeName) != 0)
            {
                // Verify existence of siblings with the same name
                if (!treeConf.ExistSiblingWithSameName(newName, sUINode.Key, (sNode is TreeLeaf), sUINode.Parent))
                {
                    // Folder
                    if (sNode is TreeNode)
                    {
                        treeConf.UpdateNode(sNode.Key, newName, sNode.Comments);
                        UpdateNodeInPanel(sUINode, newName);
                        if (ActivePaneName.Equals(CBVConstants.QUERIES_GROUPNAME))
                            Modified = true;
                    }
                    else
                    {
                        //Form
                        if (ActivePaneName.Equals(CBVConstants.PRIVATE_GROUPNAME) || ActivePaneName.Equals(CBVConstants.PUBLIC_GROUPNAME))
                        {
                            try
                            {
                                RenameFormNode(nodeName, newName, (sNode is TreeLeaf) ? ((TreeLeaf)sNode).Id : -1);
                                treeConf.UpdateNode(sNode.Key, newName, sNode.Comments);
                                UpdateNodeInPanel(sUINode, newName);
                            }
                            catch (FormDBLib.Exceptions.ObjectBankException ex)
                            {
                                CBVUtil.ReportError(ex);
                            }
                        }
                        else if (ActivePaneName.Equals(CBVConstants.QUERIES_GROUPNAME))
                        {
                            //Query
                            //Query names are unique
                            Query q = m_queries.FindByID(((TreeLeaf)sNode).Id);
                            if (!m_queries.ExistQuery(newName))
                            {
                                RenameQuery(q, (TreeLeaf)sNode, newName);
                                treeConf.UpdateNode(sNode.Key, newName, q.GetQueryDesc());
                                // Restore complete txt: name + description
                                UpdateNodeInPanel(sUINode, q.GetQueryText());
                            }
                            else
                            {
                                StringBuilder sb = new StringBuilder("The name");
                                sb.AppendFormat(" \"{0}\" already exists. Specify a new one.", newName);
                                MessageBox.Show(sb.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                // Restore previous value
                                UpdateNodeInPanel(sUINode, q.GetQueryText());
                            }
                        }
                    }
                } // Verify existence of siblings with the same name
                else
                {
                    StringBuilder sb = new StringBuilder("The name");
                    sb.AppendFormat(" \"{0}\" already exists. Specify a new one.", newName);
                    MessageBox.Show(sb.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Restore previous value
                    UpdateNodeInPanel(sUINode, nodeName);
                }

            } // Verify the new name is not a reserved word, not null and not the previous one
            else
            {
                if (sUINode != null)
                {
                    // Restore previous value
                    UpdateNodeInPanel(sUINode, nodeName);
                }
            }
        }
        //---------------------------------------------------------------------
        private void tree_DragOver(object sender, DragEventArgs e)
        {
            UltraTree tree = sender as UltraTree;

            //	Get the cursor position (as relative to the control's coordinate system) 
            //  and determine whether there is a node under it.
            //  PointToClient: Computes the location of the specified screen point into client coordinates.
            Point cursorPos = tree.PointToClient(new Point(e.X, e.Y));
            UltraTreeNode nodeAtPoint = tree.GetNodeFromPoint(cursorPos);

            //	If there is no node under the cursor, show the "no drop" cursor
            if (nodeAtPoint == null)
                e.Effect = DragDropEffects.None;
            else
            {
                //	If there is a node under the cursor, show either the 'Move' cursor
                e.Effect = DragDropEffects.Move;

                //	Select the node under the cursor, to provide a visual
                //	cue as to what node will receive the drop operation,
                //	and refresh the display so the selection is shown immediately.
                nodeAtPoint.Selected = true;
                tree.Refresh();

            }
        }
        //---------------------------------------------------------------------
        private void tree_MouseMove(object sender, MouseEventArgs e)
        {
            UltraTree tree = sender as UltraTree;

            //	We are only interested in this event when the left
            //	mouse button is pressed
            if (e.Button == MouseButtons.Left)
            {
                //	Get the cursor position (as relative to the control's coordinate system)
                //	and determine whether it is within the drag threshold
                Point cursorPos = new Point(e.X, e.Y);

                if (!tree.DisplayRectangle.Contains(cursorPos)) // CSBR-128066
                    return;

                Rectangle dragRect = new Rectangle(this.m_lastMouseDownPoint, SystemInformation.DragSize);
                dragRect.X -= SystemInformation.DragSize.Width / 2;
                dragRect.Y -= SystemInformation.DragSize.Height / 2;

                //	If it is within the drag threshold, initiate the drag/drop operation,
                //	specifying the ActiveNode as the "drag node"
                //  DragDropEffects.All: the data is copied, removed from the drag source, and scrolled in the drop target
                if (dragRect.Contains(cursorPos))
                {
                    tree.DoDragDrop(tree.ActiveNode, DragDropEffects.All);
                }
            }
        }
        //---------------------------------------------------------------------
        private void FormTree_DragDrop(object sender, DragEventArgs e)
        {
            // Removed the coverity Fix CID- 18773 as the issue is reproduced because of it.
            UltraTree tree = sender as UltraTree;
            //	Retrieve the drag data from the IDataObject and cast it to an UltraTreeNode
            UltraTreeNode sourceNode = e.Data.GetData(typeof(UltraTreeNode)) as UltraTreeNode;
            //	Get the cursor position (as relative to the control's coordinate system) 
            Point cursorPos = tree.PointToClient(new Point(e.X, e.Y));
            UltraTreeNode destinationNode = tree.GetNodeFromPoint(cursorPos);
            cursorPos = DragDropTreeNode(tree, sourceNode, cursorPos, destinationNode);
        }
        //---------------------------------------------------------------------
        private Point DragDropTreeNode(UltraTree tree, UltraTreeNode sourceNode, Point cursorPos, UltraTreeNode destinationNode)
        {
            m_lastMouseDownPoint = cursorPos;
            bool nodeMoved = false;
            TreeConfig treeConf = null;
            if (ActivePaneName.Equals(CBVConstants.PUBLIC_GROUPNAME) || ActivePaneName.Equals(CBVConstants.PRIVATE_GROUPNAME))
                treeConf = m_FTreeConf;
            else if (ActivePaneName.Equals(CBVConstants.QUERIES_GROUPNAME))
                treeConf = m_QTreeConf;
            //Coverity Bug Fix : CID 12993 
            if (treeConf != null && destinationNode != null && sourceNode != null)
            {
                if (!treeConf.ExistSiblingWithSameName(sourceNode.Text, destinationNode.Key, !treeConf.IsFolder(sourceNode.Key), destinationNode))
                {
                    nodeMoved = treeConf.MoveNodeInList(sourceNode.Key, destinationNode.Key);
                    if (nodeMoved)
                    {
                        //	The end user might want to begin working with the tree that received the drop -> for more than one tree on UI
                        tree.Select();
                        tree.ActiveNode = destinationNode;
                        sourceNode.Reposition(destinationNode.Nodes);
                        destinationNode.ExpandAll(ExpandAllType.OnlyNodesWithChildren);
                    }
                }
                else
                    MessageBox.Show("You cannot add that object. There is one with the same name in that folder", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return cursorPos;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Handle the tooltips for tree nodes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tree_MouseEnterElement(object sender, UIElementEventArgs e)
        {
            UltraTree tree = GetTreeFromGroup(ActivePaneName);
            TreeConfig treeConf = null;
            if (ActivePaneName.Equals(CBVConstants.PUBLIC_GROUPNAME) || ActivePaneName.Equals(CBVConstants.PRIVATE_GROUPNAME))
                treeConf = m_FTreeConf;
            else if (ActivePaneName.Equals(CBVConstants.QUERIES_GROUPNAME))
                treeConf = m_QTreeConf;

            if (e.Element is NodeTextUIElement)
            {
                Point p = new Point(e.Element.Rect.X, e.Element.Rect.Y);
                UltraTreeNode node = tree.GetNodeFromPoint(p);
                ITreeNode sNode = null;

                StringBuilder tipTitle = new StringBuilder(string.Empty);
                StringBuilder tipText = new StringBuilder(string.Empty);
                //Coverity Bug Fix CID 19017 
                if (treeConf != null && node != null)
                {
                    sNode = treeConf.MainTree.GetNodeFromListByKey(node.Key);
                    if (sNode != null)
                    {
                        if (sNode.Type.ToString().Equals(CBVConstants.NodeType.Query.ToString()))
                            tipText.Append("Query");
                        else if (sNode.Type.ToString().Equals(CBVConstants.NodeType.MergedQuery.ToString()))
                        {
                            tipText.Append("Merged query. ID: ");
                            tipText.Append(((TreeLeaf)sNode).Id);
                        }
                        else if (sNode.Type.ToString().Equals(CBVConstants.NodeType.Form.ToString()))
                        {
                            tipText.Append("Form. ID: ");
                            tipText.Append(((TreeLeaf)sNode).Id);
                        }
                        else if (sNode.Type.ToString().Equals(CBVConstants.NodeType.Folder.ToString()))
                            tipText.Append("Folder");

                        tipText.Append(" ");
                        tipTitle.Append(node.Text);
                        tipTitle.Append(" ");

                        if (ChemBioViz.NET.Properties.Settings.Default.ShowTooltips)
                        {
                            m_tooltipManager.DisplayStyle = Infragistics.Win.ToolTipDisplayStyle.WindowsVista;
                            //m_tooltipManager.AutoPopDelay = 600; //miliseconds
                            //Coverity Bug Fix CID 19038 
                            using (UltraToolTipInfo tipInfo = new UltraToolTipInfo(
                                 tipText.ToString(), Infragistics.Win.ToolTipImage.Info, tipTitle.ToString(), Infragistics.Win.DefaultableBoolean.True))
                            {
                                tipInfo.ToolTipTextStyle = ToolTipTextStyle.Default;

                                m_tooltipManager.SetUltraToolTip(tree, tipInfo);
                            }
                            m_tooltipManager.ShowToolTip(tree);
                            m_tooltipManager.GetUltraToolTip(tree);
                        }
                    }
                }
            }
            else
            {
                m_tooltipManager.HideToolTip();
                m_tooltipManager.SetUltraToolTip(tree, null);
            }
        }
        /// <summary>
        /// Handles the double click event on Queries tree. Restores or reruns the selected query.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void queryTree_DoubleClick(object sender, EventArgs e)
        {
            m_tooltipManager.HideToolTip();
            // restore if possible, otherwise rerun
            Query selQuery = GetSelectedQueryFromTree(true);
            if (selQuery != null && ((selQuery is RetrieveAllQuery) || selQuery.CanRestoreHitlist))
                restoreHitlistToolStripMenuItem_Click(sender, e);
            else
                RerunQueryMenuItem_Click(sender, e);
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Handles the UltraTree's DragDrop event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void queryTree_DragDrop(object sender, DragEventArgs e)
        {
            UltraTree tree = sender as UltraTree;
            //	Retrieve the drag data from the IDataObject and cast it to an UltraTreeNode

            //	Get the cursor position (as relative to the control's coordinate system) 
            Point cursorPos = tree.PointToClient(new Point(e.X, e.Y));
            UltraTreeNode destinationNode = tree.GetNodeFromPoint(cursorPos);
            // If it's a query over another query, let's merge them

            UltraTreeNode sourceNode = e.Data.GetData(typeof(UltraTreeNode)) as UltraTreeNode;
                if (sourceNode != null && destinationNode != null)
                {
                    if (!m_QTreeConf.IsFolder(sourceNode.Key) && !m_QTreeConf.IsFolder(destinationNode.Key))
                        MergeQueries(sender, e);
                    else
                        // else, just relocate the node
                        DragDropTreeNode(tree, sourceNode, cursorPos, destinationNode);
                    Modified = true;
                }
            //using (UltraTreeNode sourceNode = e.Data.GetData(typeof(UltraTreeNode)) as UltraTreeNode)
            //{
            //    if (sourceNode != null && destinationNode != null)
            //    {
            //        if (!m_QTreeConf.IsFolder(sourceNode.Key) && !m_QTreeConf.IsFolder(destinationNode.Key))
            //            MergeQueries(sender, e);
            //        else
            //            // else, just relocate the node
            //            DragDropTreeNode(tree, sourceNode, cursorPos, destinationNode);
            //        Modified = true;
            //    }
            //}
        }
        //---------------------------------------------------------------------
        private void MergeQueries(object sender, DragEventArgs e)
        {
            Query q1 = null; //It'll get the drag node data
            Query q2 = null; //It'll get the drop node data

            UltraTree tree = sender as UltraTree;

            q1 = GetSelectedQueryFromTree(false);

            //	Get the cursor position (as relative to the control's coordinate system) 
            //  and determine whether there is a node under it.
            Point cursorPos = tree.PointToClient(new Point(e.X, e.Y));
            UltraTreeNode destinationNode = tree.GetNodeFromPoint(cursorPos);
            //  Set as the last mouse down point, the one where we do the drop action.
            //  This avoid working strictly with the active node.
            m_lastMouseDownPoint = cursorPos;

            if (destinationNode != null)
            {
                //	Retrieve the drag data from the IDataObject and cast it to an UltraTreeNode
                //Coverity Bug Fix CID 18774 
                UltraTreeNode dragNode = e.Data.GetData(typeof(UltraTreeNode)) as UltraTreeNode;
                    if (dragNode != null)
                    {
                        //	The end user might want to begin working with the tree that received the drop.
                        tree.Select();
                        tree.ActiveNode = destinationNode;

                        q2 = GetSelectedQueryFromTree(false);

                        //Call to Merging Dialog .. if OK, form routine does the merge
                        if (q1 != null && q2 != null)
                        {
                            MergeListDialog mergeDialog = new MergeListDialog(q1, q2);
                            if (mergeDialog.ShowDialog() == DialogResult.OK)
                                MergeQueries(q1, q2, mergeDialog.LogicChoice);
                        }
                    }

                //using (UltraTreeNode dragNode = e.Data.GetData(typeof(UltraTreeNode)) as UltraTreeNode)
                //{
                //    if (dragNode != null)
                //    {
                //        //	The end user might want to begin working with the tree that received the drop.
                //        tree.Select();
                //        tree.ActiveNode = destinationNode;

                //        q2 = GetSelectedQueryFromTree(false);

                //        //Call to Merging Dialog .. if OK, form routine does the merge
                //        if (q1 != null && q2 != null)
                //        {
                //            MergeListDialog mergeDialog = new MergeListDialog(q1, q2);
                //            if (mergeDialog.ShowDialog() == DialogResult.OK)
                //                MergeQueries(q1, q2, mergeDialog.LogicChoice);
                //        }
                //    }
                //}
            }
        }
        //---------------------------------------------------------------------
        public FormViewControl GetQueryFormView(Query query)
        {
            // get formview of tab attached to query
            FormViewControl formView = null;
            int index = m_tabManager.FindTab(query.TabName);
            if (index != -1)
            {
                FormTab tab = m_tabManager.GetTab(index);
                formView = tab.Control as FormViewControl;
            }
            return formView;
        }
        //---------------------------------------------------------------------
        private String GetTreeNodeKey()
        {
            UltraTreeNode selNode = GetTreeNode(m_lastMouseDownPoint);
            return (selNode == null) ? string.Empty : selNode.Key;
        }
        //---------------------------------------------------------------------
        private UltraTreeNode GetTreeNode(Point lastPoint)
        {
            UltraTree treeView = this.GetTreeFromGroup(ActivePaneName);
            UltraTreeNode selNode = null;
            if (!lastPoint.IsEmpty)
                selNode = treeView.GetNodeFromPoint(lastPoint);
            else if (treeView.SelectedNodes.Count > 0)
                selNode = treeView.SelectedNodes[0];    // CSBR-118065
            return selNode;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Deactivate the node previously selected
        /// </summary>
        /// <param name="nodeCollection"></param>
        private void DeactivateTreeNode(TreeNodesCollection nodeCollection)
        {
            if (nodeCollection.Count > 0)
            {
                for (int i = 0; i < nodeCollection.Count; i++)
                {
                    if (nodeCollection[i].Nodes.Count > 0)
                        DeactivateTreeNode(nodeCollection[i].Nodes);
                    else
                        nodeCollection[i].Override.NodeAppearance.FontData.Bold = DefaultableBoolean.False;
                }
            }
        }
        //---------------------------------------------------------------------
        private void DeactivateTreeNode()
        {
            // String groupName = ActivePaneName.Equals(CBVConstants.PUBLIC_GROUPNAME) ? CBVConstants.PUBLIC_GROUPNAME : CBVConstants.PRIVATE_GROUPNAME;
            String groupName = ActivePaneName;
            UltraTree tree = this.GetTree(groupName);
            if (tree != null)
                DeactivateTreeNode(tree.Nodes);
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Activate a certain tree node
        /// </summary>
        /// <param name="paneName"></param>
        /// <param name="paneTitle"></param>
        private void ActivateTreeNode(string groupName, string nodeKey)
        {
            // first make sure the named pane is active
            if (!String.IsNullOrEmpty(groupName) && groupName.Equals(ActivePaneName))
            {
                UltraExplorerBarGroup exBarGroup = ultraExplorerBar1.Groups[groupName];
                exBarGroup.Selected = true;

                UltraTree tree = GetTreeFromGroup(groupName);
                if (tree != null && tree.Nodes.Count > 0)
                {
                    DeactivateTreeNode(tree.Nodes);
                    String formTreeName = groupName.Equals(CBVConstants.PUBLIC_GROUPNAME) ? CBVConstants.PRIVATE_GROUPNAME : CBVConstants.PUBLIC_GROUPNAME;
                    UltraTree formTree = this.GetTree(formTreeName);
                    if (formTree != null)
                        DeactivateTreeNode(formTree.Nodes);
                    ActiveTreeNode(nodeKey, tree);
                }
            }
        }
        //---------------------------------------------------------------------
        private void ActiveTreeNode(string nodeKey, UltraTree tree)
        {
            UltraTreeNode node = null;
            if (String.IsNullOrEmpty(nodeKey))
            {
                if (tree.Nodes.Count > 0)
                    node = tree.Nodes[0];
            }
            else
            {
                node = tree.GetNodeByKey(nodeKey);
            }

            if (node != null)
            {
                node.Selected = true;
                node.Override.NodeAppearance.FontData.Bold = DefaultableBoolean.True; //highlight the form
                node.BringIntoView(); //scroll to make the form visible 
                node.Expanded = true;
                tree.ActiveNode = node;
            }
        }
        //---------------------------------------------------------------------
        private void SetTitles(String groupName, String paneTitle, String appTitle)
        {
            SetPaneTitle(paneTitle, groupName);
            AppendToAppTitle(String.Concat(groupName, " - ", appTitle), false);
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Get the tree from a given group
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private UltraTree GetTree(string groupName)
        {
            UltraExplorerBarGroup group = ultraExplorerBar1.Groups[groupName];
            UltraExplorerBarContainerControl ubcc = group.Container;
            UltraTree tree = null;

            foreach (Control ctrl in ubcc.Controls)
            {
                if (ctrl is UltraTree)
                {
                    if (groupName.Equals(CBVConstants.PUBLIC_GROUPNAME) || groupName.Equals(CBVConstants.PRIVATE_GROUPNAME))
                        tree = ((UltraTree)(ctrl));
                }
            }
            return tree;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Get the tree from a given group <paramref name="groupName"/>
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private UltraTree GetTreeFromGroup(string groupName)
        {
            UltraExplorerBarGroup group = ultraExplorerBar1.Groups[groupName];
            UltraExplorerBarContainerControl ubcc = group.Container;
            UltraTree tree = null;

            foreach (Control ctrl in ubcc.Controls)
            {
                if (ctrl is UltraTree)
                {
                    tree = ((UltraTree)(ctrl));
                    break;
                }
            }
            return tree;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Get the node full path from the ultraTree
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public Dictionary<int, string> GetPathsCollection(string groupName)
        {
            Dictionary<int, string> paths = new Dictionary<int, string>();
            UltraTree tree = this.GetTree(groupName);
            if (tree != null && tree.Nodes.Count > 0)
                paths = GetPathsCollection(groupName, tree.Nodes, paths);
            return paths;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Gets the node path without its root
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="nodes"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public Dictionary<int, string> GetPathsCollection(string groupName,
            Infragistics.Win.UltraWinTree.TreeNodesCollection nodes, Dictionary<int, string> paths)
        {
            foreach (UltraTreeNode n in nodes)
            {
                int startIndex = n.FullPath.IndexOf(CBVConstants.TREE_PATH_SEPARATOR);
                if (n.HasNodes)
                {
                    paths = GetPathsCollection(groupName, n.Nodes, paths);
                }
                else
                {
                    if (startIndex != -1)
                    {
                        ITreeNode snode = m_FTreeConf.MainTree.GetNodeFromListByKey(n.Key);
                        if (snode != null && snode is TreeLeaf && ((TreeLeaf)snode).Id > 0)
                            paths.Add(((TreeLeaf)snode).Id, n.FullPath.Substring(startIndex + 1));
                    }
                }
            }

            return paths;
        }
        //---------------------------------------------------------------------        
        /// <summary>
        ///  For query tree: add new leaf under given parent
        /// </summary>
        /// <param name="groupName">Group which contains the tree</param>
        private void AddChildLeafToTree(string groupName, int leafID, int parentID)
        {
            Debug.Assert(groupName.Equals(CBVConstants.QUERIES_GROUPNAME));
            TreeConfig treeConf = m_QTreeConf;
            UltraTree tree = GetTreeFromGroup(groupName);
            if (tree == null) return;

            TreeLeaf leaf = ((QueriesTreeConfig)treeConf).CreateLeaf(m_queries, leafID);
            leaf.ParentId = parentID;

            UltraTreeNode newNode = treeConf.CreateUINode(leaf, groupName);
            if(newNode != null)
            {

                //UltraTreeNode newNode = null;
                //using (UltraTreeNode newNode1 = treeConf.CreateUINode(leaf, groupName))// adds leafID as node tag
                //{
                //    newNode = newNode1;
                //}
                ////CID:20286
                //if (newNode == null)
                //{
                //    return;
                //}

                //Coverity fix - CID 20286
                ITreeNode parentINode = treeConf.MainTree.GetLeafFromListById(parentID);
                if (newNode != null && parentINode != null)
                {
                    UltraTreeNode parentNode = tree.GetNodeByKey(parentINode.Key);                    
                    if (parentNode != null)
                    {
                        // new leaf is added under its parent leaf in the tree
                        treeConf.AddUINode(parentNode, newNode);

                        // but in the treeconfig, it is added at the same level, so they are both in the same folder
                        treeConf.AddNewChildLeafToList(treeConf, parentNode, newNode, leaf, groupName);
                    }
                    
                }
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Add a leaf to the <paramref name="groupName"/> panel
        /// </summary>
        /// <param name="groupName">Group which contains the tree</param>
        private void AddLeafToTree(string groupName, int leafID, bool bPublic)
        {
            TreeConfig treeConf = null;
            if (groupName.Equals(CBVConstants.PUBLIC_GROUPNAME) || groupName.Equals(CBVConstants.PRIVATE_GROUPNAME))
                treeConf = m_FTreeConf;
            else if (groupName.Equals(CBVConstants.QUERIES_GROUPNAME))
                treeConf = m_QTreeConf;
            else
                return;

            UltraTree tree = GetTreeFromGroup(groupName);
            if (tree != null && treeConf != null && treeConf.MainTree!= null)
            {
                // Always add new forms to root node
                // Root node has a reserved word as name
                ITreeNode mainNode = treeConf.MainTree.GetNodeFromListByName(groupName);
                if (mainNode == null)
                    mainNode = treeConf.CreateRoot(groupName);

                UltraTreeNode sourceUINode = tree.GetNodeByKey(treeConf.MainTree.GetFolderKey(mainNode.Key));

                TreeLeaf leaf = null;
                if (groupName.Equals(CBVConstants.PUBLIC_GROUPNAME) || groupName.Equals(CBVConstants.PRIVATE_GROUPNAME))
                {
                    //instead of type conversin use the class level variables
                    //Coverity fix - CID 12909
                    leaf = m_FTreeConf.CreateLeaf(FormName, leafID);
                    //FormsTreeConfig ctrl = (FormsTreeConfig)treeConf;
                    //leaf = ctrl != null ? ctrl.CreateLeaf(FormName, leafID) : null;
                }
                else if (groupName.Equals(CBVConstants.QUERIES_GROUPNAME))
                {
                    //instead of type conversin use the class level variables
                    //Coverity fix - CID 12909
                    leaf = m_QTreeConf.CreateLeaf(m_queries, leafID);
                    //QueriesTreeConfig ctrl = (QueriesTreeConfig)treeConf;
                    //leaf = ctrl != null ? ctrl.CreateLeaf(m_queries, leafID) : null;
                }
                //Coverity Bug Fix CID 19016 
                if (leaf != null)
                {
                    UltraTreeNode newNode = treeConf.CreateUINode(leaf, groupName);
                    treeConf.AddNode(sourceUINode, newNode, leaf, groupName);

                    if (groupName.Equals(CBVConstants.PUBLIC_GROUPNAME) || groupName.Equals(CBVConstants.PRIVATE_GROUPNAME))
                    {
                        m_formType = bPublic ? formType.Public : formType.Private;
                        m_formName = newNode.Text;
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Remove node
        /// </summary>
        private void RemoveTreeNode(String nodeKey, bool bPublic, TreeConfig treeConf)
        {
            ITreeNode node = treeConf.MainTree.GetNodeFromListByKey(nodeKey);
            if (node != null)
            {
                if (ActivePaneName.Equals(CBVConstants.PUBLIC_GROUPNAME) || ActivePaneName.Equals(CBVConstants.PRIVATE_GROUPNAME))
                {
                    // Remove all children from DB if exist
                    if (node is TreeLeaf && ((TreeLeaf)node).Id == m_formID)
                        UnloadForm();
                    RemoveChildrenFromDB(node);
                }
                else if (ActivePaneName.Equals(CBVConstants.QUERIES_GROUPNAME))
                {
                    m_QTreeConf.RemoveChildQueries(m_queries, node);
                    Modified = true;
                }
                // Remove node from UI and structure
                UltraTreeNode childNode = GetTreeNode(m_lastMouseDownPoint);
                //Coverity Bug Fix : CID 13094 
                if (childNode != null)
                {
                    UltraTreeNode parentNode = childNode.Parent;
                    treeConf.RemoveNode(childNode, parentNode);
                }
                this.SetPaneTitle(ActivePaneName, CBVConstants.EXPLORER_PANE_NAME);
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///   Check if the node has children and remove them from the DB
        /// </summary>
        /// <param name="node"></param>
        public void RemoveChildrenFromDB(ITreeNode node)
        {
            if (node is TreeLeaf)
                this.ActiveDBBank.Delete(((TreeLeaf)node).Id);
            else
            {
                if (((TreeNode)node).Nodes.Count > 0)
                {
                    foreach (ITreeNode n in ((TreeNode)node).Nodes)
                    {
                        if (n is TreeLeaf)
                        {
                            this.ActiveDBBank.Delete(((TreeLeaf)n).Id);
                        }
                        else
                        {
                            RemoveChildrenFromDB(n);
                        }
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        private UltraTreeNode FindQNode(UltraTree tree, int queryID)
        {
            foreach (UltraTreeNode nodeRoot in tree.Nodes)
            {
                foreach (UltraTreeNode node in nodeRoot.Nodes)
                {
                    if (node != null && node.Tag != null)   // 5/31/11 .. while investigating CSBR-141273
                    {
                        int nodeTag = (int)node.Tag;
                        if (nodeTag == queryID)
                            return node;
                    }
                }
            }
            return null;
        }
        //---------------------------------------------------------------------
        private static List<Query> GetChildQueries(Query qParent, QueryCollection queries)
        {
            List<Query> list = new List<Query>();
            foreach (Query q in queries)
                if (q != qParent && q.ParentQueryID == qParent.ID)
                    list.Add(q);
            return list;
        }
        //---------------------------------------------------------------------
        private bool AddChildQueries(UltraTree tree, Query qParent, UltraTreeNode parentTreeNode, QueriesTreeConfig qtConfig)
        { // recursive

            bool hasChildQs = false;
            List<Query> childQs = GetChildQueries(qParent, m_queries);
            if (childQs.Count > 0)
            {
                hasChildQs = true;
                foreach (Query qChild in childQs)
                {
                    String key = qtConfig.KGenerator.GetKey();
                    String text = String.Format("{0}: {1}", qChild.Name, qChild.GetQueryDesc());

                    UltraTreeNode nodeChild = new UltraTreeNode(key, text);
                    nodeChild.LeftImages.Add(ChemBioViz.NET.Properties.Resources.File_Right);
                    nodeChild.Tag = qChild.ID;

                    parentTreeNode.Nodes.Add(nodeChild);
                    parentTreeNode.Expanded = true;

                    // CSBR-141787: add a new treeleaf item to qtConfig, at root level
                    TreeLeaf newLeaf = new TreeLeaf(key, qChild.Name, CBVConstants.NodeType.Query);
                    newLeaf.Id = qChild.ID;     // added 6/2/11; CSBR-142926
                    TreeNode rootNode = qtConfig.FirstNode;
                    Debug.Assert(rootNode != null);
                    //CID:20279
                    if (rootNode != null)
                    {
                        rootNode.Add(newLeaf);
                    }
                    // recurse
                    AddChildQueries(tree, qChild, nodeChild, qtConfig);
                }
            }
            return hasChildQs;
        }
        //---------------------------------------------------------------------
        private void AddChildQueriesToTree(UltraTree tree, QueriesTreeConfig qtConfig)
        {
            foreach (Query q in m_queries)
            {
                if (q is RetrieveAllQuery)
                    continue;
                UltraTreeNode parentTreeNode = FindQNode(tree, q.ID);
                if (parentTreeNode != null)
                    AddChildQueries(tree, q, parentTreeNode, qtConfig);
            }
        }
        //---------------------------------------------------------------------
        #endregion
    }
}
