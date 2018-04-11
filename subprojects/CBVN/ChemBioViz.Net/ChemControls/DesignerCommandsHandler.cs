using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Greatis.FormDesigner;
using Infragistics.Win.UltraWinToolbars;
using System.ComponentModel.Design;
using System.Collections;

namespace ChemControls
{
    public class DesignerCommandsHandler
    {
        public DesignerCommandsHandler()
        {
        }

        public void UpdateToggleDesignMode(UltraToolbarsManager manager, Panel p)
        {
            Designer designer = p.Tag as Designer;

            StateButtonTool tool = manager.Tools["ToggleDesignMode"] as StateButtonTool;
            //Coverity Bug Fix CID 11421 
            if (tool != null)
            {
                tool.SharedProps.Enabled = designer != null;
                if (designer == null)
                    tool.Checked = false;
                else if (tool.Checked != designer.Active)
                    tool.Checked = designer.Active;
            }
        }

        public void ExecuteToggleDesignMode(Panel p)
        {
            Designer designer = p.Tag as Designer;
            //Coverity Bug Fix CID 11420 
            if (designer != null)
                designer.Active = !designer.Active;
        }

        Designer GetDesigner(Panel p) { return p.Tag as Designer; }

        int SelectionCount(Panel p)
        {
            Designer designer = GetDesigner(p);
            if (designer == null)
                return 0;
            if (!designer.Active)
                return 0;

            ISelectionService ss = (ISelectionService)designer.DesignerHost.GetService(typeof(ISelectionService));
            int selCount = ss.SelectionCount;
            return selCount;
        }

        public void UpdateCut(UltraToolbarsManager manager, Panel p) { manager.Tools["Cut"].SharedProps.Enabled = SelectionCount(p) > 0; }
        public void UpdateCopy(UltraToolbarsManager manager, Panel p) { manager.Tools["Copy"].SharedProps.Enabled = SelectionCount(p) > 0; }
        public void UpdatePaste(UltraToolbarsManager manager, Panel p) { manager.Tools["Paste"].SharedProps.Enabled = true; }

        public void ExecuteCut(Panel p)
        {
            Designer designer = GetDesigner(p);
            if (designer == null)
                return;
            if (!designer.Active)
                return;

            designer.CopyControlsToClipboard();
            designer.DeleteSelected();
        }
        public void ExecuteCopy(Panel p)
        {
            Designer designer = GetDesigner(p);
            if (designer == null)
                return;
            if (!designer.Active)
                return;
            designer.CopyControlsToClipboard();
        }

        public void ExecutePaste(Panel p)
        {
            Designer designer = GetDesigner(p);
            if (designer == null)
                return;
            if (!designer.Active)
                return;
            designer.PasteControlsFromClipboard();
        }

        public bool EnableAlign(Panel p) { return SelectionCount(p) > 1; }

        public void UpdateAlignLeft(UltraToolbarsManager manager, Panel p) { manager.Tools["AlignLeft"].SharedProps.Enabled = EnableAlign(p); }
        public void ExecuteAlignLeft(Panel p) { Align(p, AlignType.Left); }
        public void UpdateAlignRight(UltraToolbarsManager manager, Panel p) { manager.Tools["AlignRight"].SharedProps.Enabled = EnableAlign(p); }
        public void ExecuteAlignRight(Panel p) { Align(p, AlignType.Right); }
        public void UpdateAlignHCenter(UltraToolbarsManager manager, Panel p) { manager.Tools["AlignHCenter"].SharedProps.Enabled = EnableAlign(p); }
        public void ExecuteAlignHCenter(Panel p) { Align(p, AlignType.Center); }

        public void UpdateAlignTop(UltraToolbarsManager manager, Panel p) { manager.Tools["AlignTop"].SharedProps.Enabled = EnableAlign(p); }
        public void ExecuteAlignTop(Panel p) { Align(p, AlignType.Top); }
        public void UpdateAlignBottom(UltraToolbarsManager manager, Panel p) { manager.Tools["AlignBottom"].SharedProps.Enabled = EnableAlign(p); }
        public void ExecuteAlignBottom(Panel p) { Align(p, AlignType.Bottom); }
        public void UpdateAlignVCenter(UltraToolbarsManager manager, Panel p) { manager.Tools["AlignVCenter"].SharedProps.Enabled = EnableAlign(p); }
        public void ExecuteAlignVCenter(Panel p) { Align(p, AlignType.Middle); }

        void Align(Panel p, AlignType align)
        {
            Designer designer = p.Tag as Designer;
            if (designer == null)
                return;

            if (!designer.Active)
                return;

            ISelectionService ss = (ISelectionService)designer.DesignerHost.GetService(typeof(ISelectionService));
            if (ss.SelectionCount < 2)
                return;

            IComponentChangeService ccs = (IComponentChangeService)designer.DesignerHost.GetService(typeof(IComponentChangeService));
            DesignerTransaction dt = designer.DesignerHost.CreateTransaction("Align");
            using (dt)
            {
                ICollection selectedControls = ss.GetSelectedComponents();
                int minX = 1000000, maxX = -1000000, minY = 1000000, maxY = -1000000;
                foreach (Control c in selectedControls)
                {
                    minX = System.Math.Min(minX, c.Left);
                    minY = System.Math.Min(minY, c.Top);
                    maxX = System.Math.Max(maxX, c.Right);
                    maxY = System.Math.Max(maxY, c.Bottom);
                }

                foreach (Control c in selectedControls)
                {
                    if ((align & AlignType.Left) != 0)
                        c.Left = minX;
                    else if ((align & AlignType.Right) != 0)
                        c.Left = maxX - c.Width;
                    else if ((align & AlignType.Center) != 0)
                        c.Left = (minX + maxX) / 2 - c.Width / 2;

                    if ((align & AlignType.Top) != 0)
                        c.Top = minY;
                    else if ((align & AlignType.Bottom) != 0)
                        c.Top = maxY - c.Height;
                    else if ((align & AlignType.Middle) != 0)
                        c.Top = (minY + maxY) / 2 - c.Height / 2;

                    ccs.OnComponentChanging(c, null);
                    ccs.OnComponentChanged(c, null, null, null);
                }

                dt.Commit();
            }
        }

        public void UpdateMakeSameWidth(UltraToolbarsManager manager, Panel p) { manager.Tools["MakeSameWidth"].SharedProps.Enabled = EnableAlign(p); }
        public void UpdateMakeSameHeight(UltraToolbarsManager manager, Panel p) { manager.Tools["MakeSameHeight"].SharedProps.Enabled = EnableAlign(p); }

        public void ExecuteMakeSameWidth(Panel p) { MakeSameSize(p, ResizeType.SameWidth); }
        public void ExecuteMakeSameHeight(Panel p) { MakeSameSize(p, ResizeType.SameHeight); }

        void CollectNumber(Dictionary<int, int> values, int v)
        {
            if (values.ContainsKey(v))
                values[v] += 1;
            else
                values[v] = 1;
        }

        int MostFrequentValue(Dictionary<int, int> values)
        {
            int frequencyMax = 0;
            int value = 0;

            foreach (int v in values.Keys)
            {
                if (values[v] > frequencyMax)
                {
                    value = v;
                    frequencyMax = values[v];
                }
            }

            return value;
        }

        void MakeSameSize(Panel p, ResizeType resizeType)
        {
            Designer designer = p.Tag as Designer;
            if (designer == null)
                return;

            if (!designer.Active)
                return;

            ISelectionService ss = (ISelectionService)designer.DesignerHost.GetService(typeof(ISelectionService));
            if (ss.SelectionCount < 2)
                return;

            IComponentChangeService ccs = (IComponentChangeService)designer.DesignerHost.GetService(typeof(IComponentChangeService));
            DesignerTransaction dt = designer.DesignerHost.CreateTransaction("MakeSameSize");
            using (dt)
            {
                ICollection selectedControls = ss.GetSelectedComponents();

                Dictionary<int, int> widths = new Dictionary<int, int>();
                Dictionary<int, int> heights = new Dictionary<int, int>();

                foreach (Control c in selectedControls)
                {
                    CollectNumber(widths, c.Width);
                    CollectNumber(heights, c.Height);
                }

                int width = MostFrequentValue(widths);
                int height = MostFrequentValue(heights);

                foreach (Control c in selectedControls)
                {
                    if ((resizeType & ResizeType.SameWidth) != 0)
                        c.Width = width;
                    if ((resizeType & ResizeType.SameHeight) != 0)
                        c.Height = height;

                    ccs.OnComponentChanging(c, null);
                    ccs.OnComponentChanged(c, null, null, null);
                }

                dt.Commit();
            }
        }

        public bool EnableUndo(Panel p)
        {
            Designer designer = p.Tag as Designer;
            if (designer == null)
                return false;
            if (!designer.Active)
                return false;
            return designer.UndoCount != 0;
        }

        public void UpdateUndo(UltraToolbarsManager manager, Panel p)
        {
            manager.Tools["Undo"].SharedProps.Enabled = EnableUndo(p);
        }

        public bool EnableRedo(Panel p)
        {
            Designer designer = p.Tag as Designer;
            if (designer == null)
                return false;
            if (!designer.Active)
                return false;
            return designer.RedoCount != 0;
        }

        public void UpdateRedo(UltraToolbarsManager manager, Panel p)
        {
            manager.Tools["Redo"].SharedProps.Enabled = EnableRedo(p);
        }

        public void ExecuteUndo(Panel p)
        {
            Designer designer = p.Tag as Designer;
            if (designer == null)
                return;
            if (!designer.Active)
                return;
            designer.Undo();
        }

        public void ExecuteRedo(Panel p)
        {
            Designer designer = p.Tag as Designer;
            if (designer == null)
                return;
            if (!designer.Active)
                return;
            designer.Redo();
        }

        public void UpdateBringToFront(UltraToolbarsManager manager, Panel p) { manager.Tools["BringToFront"].SharedProps.Enabled = SelectionCount(p) > 0; }
        public void UpdateSendToBack(UltraToolbarsManager manager, Panel p) { manager.Tools["SendToBack"].SharedProps.Enabled = SelectionCount(p) > 0; }


        void MoveControls(Panel p, bool bringToFront)
        {
            Designer designer = p.Tag as Designer;
            if (designer == null)
                return;

            if (!designer.Active)
                return;

            ISelectionService ss = (ISelectionService)designer.DesignerHost.GetService(typeof(ISelectionService));
            if (ss.SelectionCount < 1)
                return;

            IComponentChangeService ccs = (IComponentChangeService)designer.DesignerHost.GetService(typeof(IComponentChangeService));
            DesignerTransaction dt = designer.DesignerHost.CreateTransaction("MoveControls");
            using (dt)
            {
                ICollection selectedControls = ss.GetSelectedComponents();

                foreach (Control c in selectedControls)
                {

                    if (bringToFront)
                        c.BringToFront();
                    else
                        c.SendToBack();

                    ccs.OnComponentChanging(c, null);
                    ccs.OnComponentChanged(c, null, null, null);
                }

                dt.Commit();
            }
        }

        public void ExecuteBringToFront(Panel p) { MoveControls(p, true); }
        public void ExecuteSendToBack(Panel p) { MoveControls(p, false); }
    }
}
