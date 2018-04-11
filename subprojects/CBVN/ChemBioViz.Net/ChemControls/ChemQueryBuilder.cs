using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ChemControls
{
    public class TableView : Control
    {

        public class TableForm : Form
        {
            CheckedListBox listbox;
            DataTable datatable;

            public DataTable DataTable { get { return datatable; } set { datatable = value; DataTableChanged(); } }

            protected override void OnCreateControl()
            {
                base.OnCreateControl();

                listbox = new CheckedListBox();
                listbox.Dock = DockStyle.Fill;
                listbox.Visible = true;

                Controls.Add(listbox);
            }

            void DataTableChanged()
            {
                Text = datatable.TableName;
                foreach (DataColumn c in datatable.Columns)
                {
                    listbox.Items.Add(c.ColumnName);
                }
            }

            protected override void OnMove(EventArgs e)
            {
                base.OnMove(e);
                TableView v = Parent as TableView;
                //Coverity Bug Fix CID 11415 
                if (v != null)
                    v.RedrawRelations();
            }
        }

        List<DataRelation> relations;
        List<Point[]> relationLines;

        public TableView()
        {
            AllowDrop = true;
            relations = new List<DataRelation>();
            relationLines = new List<Point[]>();
        }

        void RedrawRelations()
        {
            //Coverity Bug Fix CID 11798 
            using (Graphics g = Graphics.FromHwnd(Handle))
            {
                DrawRelations(g, Color.White);

                relationLines.Clear();
                foreach (DataRelation r in relations)
                    AddRelationLines(r);

                DrawRelations(g, Color.Black);
            }

        }

        bool FindTableName(string tableName)
        {
            foreach (Control control in Controls)
            {
                Form f = control as Form;
                if (f == null)
                    continue;
                if (f.Text != tableName)
                    continue;
                return true;
            }
            return false;
        }

        Rectangle FindColumnRectangle(string tableName, string columnName)
        {
            foreach (Control control in Controls)
            {
                Form f = control as Form;
                if (f == null)
                    continue;

                if (f.Text != tableName)
                    continue;

                CheckedListBox listbox = f.Controls[0] as CheckedListBox;
                if (listbox == null)
                    continue;

                int i, n = listbox.Items.Count;
                int pos = 0;

                for (i = 0; i < n; i++)
                {
                    string s = listbox.Items[i] as string;
                    int height = listbox.GetItemHeight(i);
                    int width = f.ClientRectangle.Width;
                    if (s == columnName)
                    {
                        Rectangle r = new Rectangle(0, pos, width, height);
                        r = listbox.RectangleToScreen(r);
                        r = this.RectangleToClient(r);
                        return r;
                    }
                    else
                        pos += height;
                }

            }
            return new Rectangle();
        }

        Point[] PointsLeftToRight(int x1, int y1, int x2, int y2)
        {
            Point[] p = new Point[4];
            p[0] = new Point(x1, y1);
            p[1] = new Point(x1 + 10, y1);
            p[2] = new Point(x2 - 10, y2);
            p[3] = new Point(x2, y2);

            return p;
        }

        Point[] PointsLeftToLeft(int x1, int y1, int x2, int y2)
        {
            int x = System.Math.Min(x1, x2);
            Point[] p = new Point[4];
            p[0] = new Point(x1, y1);
            p[1] = new Point(x - 10, y1);
            p[2] = new Point(x - 10, y2);
            p[3] = new Point(x2, y2);

            return p;
        }

        void DrawRelations(Graphics g, Color color)
        {
            int i, n = relationLines.Count;
            for (i = 0; i < n; i++)
                g.DrawLines(new Pen(color), relationLines[i]);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, Width, Height);
            DrawRelations(e.Graphics, Color.Black);

        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);
            drgevent.Effect = DragDropEffects.Copy;
        }

        void AddRelationLines(DataRelation r)
        {
            Rectangle r1 = FindColumnRectangle(r.ParentTable.TableName, r.ParentColumns[0].ColumnName);
            Rectangle r2 = FindColumnRectangle(r.ChildTable.TableName, r.ChildColumns[0].ColumnName);

            int y1 = r1.Top + r1.Height / 2;
            int y2 = r2.Top + r2.Height / 2;

            Point[] p = new Point[4];

            if (r2.Left > r1.Right)
                p = PointsLeftToRight(r1.Right, r1.Top + r1.Height / 2, r2.Left, r2.Top + r2.Height / 2);
            else if (r1.Left > r2.Right)
                p = PointsLeftToRight(r2.Right, r2.Top + r2.Height / 2, r1.Left, r1.Top + r1.Height / 2);
            else
                p = PointsLeftToLeft(r1.Left, r1.Top + r1.Height / 2, r2.Left, r2.Top + r2.Height / 2);

            relationLines.Add(p);
        }

        void AddRelation(DataRelation r)
        {
            relations.Add(r);
            AddRelationLines(r);
        }

        void CheckRelations(DataSet dataset)
        {
            if (dataset == null)
                return;

            relations.Clear();
            relationLines.Clear();

            foreach (DataRelation r in dataset.Relations)
                if (FindTableName(r.ParentTable.TableName) && FindTableName(r.ChildTable.TableName))
                    AddRelation(r);

        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);

            IDataObject idata = drgevent.Data;
            string[] formats = idata.GetFormats();
            if (!idata.GetDataPresent("System.Windows.Forms.TreeNode"))
                return;
            Object o = idata.GetData("System.Windows.Forms.TreeNode");
            if (o == null)
                return;
            if (o.GetType().Name != "TreeNode")
                return;

            TreeNode node = o as TreeNode;

            if (!(node.Tag is DataTable))
                return;

            DataTable t = node.Tag as DataTable;

            TableForm f = new TableForm();
            f.TopLevel = false;

            Controls.Add(f);        // can not be added until toplevel set to false

            f.Text = t.TableName;

            Point p = this.PointToClient(Control.MousePosition);

            f.Left = p.X;
            f.Top = p.Y;
            f.Width = 100;
            f.Height = t.Columns.Count * 20 + 40;
            f.Visible = true;

            f.DataTable = t;

            CheckRelations(t.DataSet);
            Invalidate();

        }

    }

    public partial class ChemQueryBuilder : UserControl
    {
        DataSet schema;
        public DataSet Schema { get { return schema; } set { schema = value; SchemaChanged(); } }

        public ChemQueryBuilder()
        {
            InitializeComponent();

            TableView tableView = new TableView();
            tableView.Dock = DockStyle.Fill;
            splitContainer2.Panel1.Controls.Add(tableView);

            schemaBrowser.ItemDrag += new ItemDragEventHandler(schemaBrowser_ItemDrag);
        }

        void schemaBrowser_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Copy);
        }

        void SchemaChanged()
        {
            schemaBrowser.Nodes.Clear();
            if (schema == null)
                return;

            TreeNode root = schemaBrowser.Nodes.Add("Tables");
            foreach (DataTable t in schema.Tables)
            {
                TreeNode node = root.Nodes.Add(t.TableName);
                node.Tag = t;
            }
        }
    }
}
