using System;
using System.Drawing;
using System.Xml;
using System.Windows.Forms;
using CambridgeSoft.COE.DataLoader.Windows.Controls;

namespace CambridgeSoft.COE.DataLoader.Windows.Forms
{
    /// <summary>
    /// Dialog box for choosing fields
    /// </summary>
    public partial class FieldChooser : Form
    {
        #region data
        private readonly System.Windows.Forms.ListView _FieldList;
        private readonly System.Windows.Forms.Button _AcceptButton;
        private readonly System.Windows.Forms.Button _CancelButton;
        private string _Field;
        #endregion

        #region properties
        /// <summary>
        /// Get / set field
        /// </summary>
        public string Field
        {
            get
            {
                return _Field;
            } // get
            protected set
            {
                _Field = value;
                return;
            } // set
        } // Field

        /// <summary>
        /// Set fields
        /// </summary>
        public string Fields
        {
            set
            {
                _FieldList.Items.Clear();
                XmlDocument oXmlDocument = new XmlDocument();
                oXmlDocument.LoadXml(value);
                XmlNode oXmlNodeFields = oXmlDocument.DocumentElement;
                string[] strName = new string[oXmlNodeFields.ChildNodes.Count];
                string[] strType = new string[oXmlNodeFields.ChildNodes.Count];
                int cFields = 0;
                foreach (XmlNode oXmlNodeField in oXmlNodeFields)
                {
                    strName[cFields] = oXmlNodeField.Attributes["name"].Value.ToString();
                    strType[cFields] = oXmlNodeField.Attributes["type"].Value.ToString();
                    cFields++;
                } // foreach (XmlNode oXmlField in oXmlNodeFields)
                Array.Sort(strName, strType);
                int[] maxX = new int[] { -1, -1 };
                for (int index = 0; index < cFields; index++)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = strName[index];
                    int x = TextRenderer.MeasureText(lvi.Text, lvi.Font).Width;
                    if (maxX[0] < x) maxX[0] = x;
                    lvi.SubItems.Add(strType[index]);
                    x = TextRenderer.MeasureText(strType[index], lvi.Font).Width;
                    if (maxX[1] < x) maxX[1] = x;
                    _FieldList.Items.Add(lvi);
                } // for (int index = 0; index < cFields; index++)
                _FieldList.Columns[0].Width = UIBase.ExtraPadding.Left + maxX[0] + UIBase.ExtraPadding.Right;
                _FieldList.Columns[1].Width = UIBase.ExtraPadding.Left + maxX[1] + UIBase.ExtraPadding.Right;
                return;
            } // set
        } // Fields
        #endregion

        #region constructors

        /// <summary>
        /// FieldChooser constructor
        /// </summary>
        public FieldChooser()
        {
            InitializeComponent();
            BackColor = UIBase.LightGray;
            FormBorderStyle = FormBorderStyle.Sizable;
            SuspendLayout();

            _FieldList = UIBase.GetListView();
            _FieldList.LabelEdit = false;
            _FieldList.FullRowSelect = true;
            _FieldList.HideSelection = false;
            _FieldList.MultiSelect = false;
            _FieldList.View = View.Details;
            _FieldList.Columns.Add("Input Field");
            _FieldList.Columns.Add("Type");
            Controls.Add(_FieldList);

            AcceptButton = _AcceptButton = UIBase.GetButton(UIBase.ButtonType.Accept);
            Controls.Add(_AcceptButton);
            CancelButton = _CancelButton = UIBase.GetButton(UIBase.ButtonType.Cancel);
            Controls.Add(_CancelButton);

            _FieldList.DoubleClick += new EventHandler(FieldList_DoubleClick);
            _AcceptButton.Click += new EventHandler(AcceptButton_Click);
            _CancelButton.Click += new EventHandler(CancelButton_Click);
            Layout += new LayoutEventHandler(FieldChooser_Layout);
            Shown += new EventHandler(FieldChooser_Shown);

            ResumeLayout(false);
            PerformLayout();
        } // FieldChooser()

        #endregion

        #region event handlers

        /// <summary>
        /// Form Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FieldChooser_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.DL;
        }

        void AcceptButton_Click(object sender, EventArgs e)
        {
            if (_FieldList.SelectedItems != null)
            {
                Field = _FieldList.SelectedItems[0].Text;
            }
            Close();
            return;
        }  // AcceptButton_Click()

        void CancelButton_Click(object sender, EventArgs e)
        {
            return;
        }  // CancelButton_Click()

        private void FieldChooser_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && ((e.AffectedProperty == "Bounds") || (e.AffectedProperty == "Visible")))
            {
                // Vertical
                int y = 0;
                _FieldList.Top = y;
                y += _FieldList.Height;
                y += UIBase.ExtraPadding.Bottom;
                _AcceptButton.Top = y;
                _CancelButton.Top = y;
                y += _AcceptButton.Height;

                // Horizontal
                int x = 0;
                int maxX = -1;
                _FieldList.Left = x;
                x += _FieldList.Width;
                if (maxX < x) maxX = x;
                x = 0;
                _AcceptButton.Left = x;
                x += _AcceptButton.Width;
                _CancelButton.Left = x;
                x += _CancelButton.Width;
                x += _CancelButton.Height; // For gripper
                if (maxX < x) maxX = x;

                if ((ClientSize.Width < maxX) || (ClientSize.Height < y)) ClientSize = new Size(maxX, y);

                x = ClientSize.Width;
                _FieldList.Width = x;
                x -= _CancelButton.Height; // For gripper
                x -= _CancelButton.Width;
                _CancelButton.Left = x;
                x -= _AcceptButton.Width;
                _AcceptButton.Left = x;

                y = ClientSize.Height;
                y -= _CancelButton.Height;
                _CancelButton.Top = y;
                _AcceptButton.Top = y;
                y -= UIBase.ExtraPadding.Bottom;
                _FieldList.Height = y;
            }
            return;
        } // FieldChooser_Layout

        void FieldChooser_Shown(object sender, EventArgs e)
        {
            Field = string.Empty;
            Point ptScreen = new Point(Owner.Left, Owner.Bottom);
            Left = ptScreen.X;
            Top = ptScreen.Y;
            return;
        } // FieldChooser_Shown()

        void FieldList_DoubleClick(object sender, EventArgs e)
        {
            ListView lv = sender as ListView;
            if (lv != null)
            {
                if (lv.SelectedItems != null)
                {
                    Field = lv.SelectedItems[0].Text;
                    Close();
                }
            }
            return;
        } // FieldList_DoubleClick()

        #endregion

    } // class FieldChooser
}