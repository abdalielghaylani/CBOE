using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Greatis.FormDesigner;

namespace ChemControls
{
    public partial class ChemForm : UserControl
    {
        public ChemForm()
        {
            InitializeComponent();
        }

        public void LoadFromFile(string fileName)
        {
            System.IO.StreamReader reader = new System.IO.StreamReader(fileName);
            string xml = reader.ReadToEnd();
            reader.Close();
            LoadFromXML(xml);
        }

        public void LoadFromXML(string xml)
        {
            Treasury treasury = new Treasury();
            Designer designer = new Designer();

            treasury.LoadMode = LoadModes.EraseForm;

            designer.FormTreasury = treasury;
            designer.DesignedForm = this;

            designer.LayoutXML = xml;
        }

        static string FindBindingField(BindingSource bindingSource, Binding b)
        {
            string oldTableName = (b.DataSource as BindingSource).DataMember;
            string oldColumnName = b.BindingMemberInfo.BindingField;
            DataSet dataset = bindingSource.DataSource as DataSet;
            //Coverity Bug Fix CID 11414 
            if (dataset != null && dataset.Tables.Count>0)
            {
                foreach (DataTable t in dataset.Tables)
                {
                    string tname = t.ExtendedProperties["name"] as string;
                    if (tname != oldTableName)
                        continue;
                    foreach (DataColumn c in t.Columns)
                    {
                        string cname = c.ExtendedProperties["name"] as string;
                        if (cname == oldColumnName)
                            return c.ColumnName;
                    }
                }
            }
            return null;
        }

        void RebindControls(BindingSource bindingSource)
        {
            foreach (Control c in Controls)
            {
                if (c.DataBindings.Count == 0)
                    continue;
                Binding b = c.DataBindings[0];
                c.DataBindings.Clear();
                string s = FindBindingField(bindingSource, b);
                if (s == null)
                    continue;
                c.DataBindings.Add(b.PropertyName, bindingSource, s);
            }
        }

        public void ConnectToDataSource(BindingSource bindingSource)
        {
            if (bindingSource == null)
                return;

            RebindControls(bindingSource);
            BindingNavigator navigator = null;

            foreach (Control c in Controls)
            {
                if (c is BindingNavigator)
                    navigator = c as BindingNavigator;
            }

            if (navigator == null)
                navigator = new BindingNavigator(true);

            navigator.Visible = true;
            Controls.Add(navigator);
            navigator.BindingSource = bindingSource;

#if NOT_USED
return;
            int bottom = 0;
            foreach (Control c in Controls)
                if (c is TextBox || c is Label)
                    if (c.Bottom > bottom)
                        bottom = c.Bottom;

            cd.Top = bottom + 5;
            cd.Height = Height - cd.Top;
            cd.Left = 0;
            cd.Width = Width;

            cd.Visible = true;
            Controls.Add(cd);

            bindingSource.PositionChanged += new EventHandler(bindingSource_PositionChanged);

            bindingSource_PositionChanged(this, new EventArgs());
#endif
        }

        void bindingSource_PositionChanged(object sender, EventArgs e)
        {
#if NOT_USED
return;
            ChemDraw cd = null;
            TextBox cdtext = null;

            foreach (Control c in Controls)
            {
                if (c is ChemDraw)
                    cd = c as ChemDraw;
                else if (c is TextBox && c.Text[0] == 'V' && c.Text[1] == 'm' && c.Text[2] == 'p' && c.Text[3] == 'D')
                    cdtext = c as TextBox;
            }

            if (cd == null || cdtext == null)
                return;

            cd.Base64 = cdtext.Text;
#endif
        }
    }
}
