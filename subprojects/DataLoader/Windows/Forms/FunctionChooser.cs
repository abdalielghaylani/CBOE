using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using CambridgeSoft.COE.DataLoader.Windows.Controls;

namespace CambridgeSoft.COE.DataLoader.Windows.Forms
{
    /// <summary>
    /// Dialog box for function choosing
    /// </summary>
    public partial class FunctionChooser : Form
    {
        #region data
        private readonly System.Windows.Forms.TreeView _FunctionTree;
        private readonly System.Windows.Forms.Button _AcceptButton;
        private readonly System.Windows.Forms.Button _CancelButton;
        private string _Function;
        #endregion

        #region properties
        /// <summary>
        /// Return the selected function
        /// </summary>
        public string Function
        {
            get
            {
                return _Function;
            } // get
            protected set
            {
                _Function = value;
                return;
            } // set
        } // Function

        /// <summary>
        /// Set choosable functions
        /// </summary>
        public string Functions
        {
            set
            {
                _FunctionTree.Nodes.Clear();
                XmlDocument oXmlDocument = new XmlDocument();
                oXmlDocument.LoadXml(value);
                XmlNode oXmlNodeFunctions = oXmlDocument.DocumentElement;
                foreach (XmlNode oXmlNodeFunction in oXmlNodeFunctions)
                {
                    string strReturns = oXmlNodeFunction.Attributes["returns"].Value.ToString();
                    string strSignature = oXmlNodeFunction.Attributes["method"].Value.ToString();
                    string strTarget = (oXmlNodeFunction.Attributes["target"] != null) ? oXmlNodeFunction.Attributes["target"].Value.ToString() : string.Empty;
                    if (strTarget != string.Empty)
                    {
                        strTarget = strTarget.Replace("System.", string.Empty);
                        if (strTarget == "Double") strTarget = "Decimal";
                        if (strTarget == "Int32") strTarget = "Integer";
                        strSignature = strTarget + "." + strSignature;
                    }
                    strSignature += "(";
                    if (oXmlNodeFunction.HasChildNodes)
                    {
                        int cArguments = 0;
                        foreach (XmlNode oXmlNodeArgument in oXmlNodeFunction.SelectSingleNode("arguments"))
                        {
                            if (cArguments > 0) strSignature += ",";
                            strSignature += oXmlNodeArgument.InnerText.Trim();
                            cArguments++;
                        }
                    }
                    strSignature += ")";
                    if (_FunctionTree.Nodes.ContainsKey(strReturns) == false)
                    {
                        _FunctionTree.Nodes.Add(strReturns, strReturns);
                    }
                    TreeNode oTreeNodeReturns = _FunctionTree.Nodes[strReturns];
                    TreeNode oTreeNodeSignature = new TreeNode(strSignature);
                    oTreeNodeReturns.Nodes.Add(oTreeNodeSignature);
                } // foreach (XmlNode oXmlField in oXmlNodeFields)
                _FunctionTree.Sort();
                return;
            } // set
        } // Functions
        #endregion

        #region constructors

        /// <summary>
        /// FunctionChooser constructor
        /// </summary>
        public FunctionChooser()
        {
            InitializeComponent();
            BackColor = UIBase.LightGray;
            FormBorderStyle = FormBorderStyle.Sizable;
            SuspendLayout();

            _FunctionTree = UIBase.GetTreeView();
            _FunctionTree.FullRowSelect = true;
            _FunctionTree.HideSelection = false;
            _FunctionTree.LabelEdit = false;
            _FunctionTree.Scrollable = true;
            Controls.Add(_FunctionTree);

            AcceptButton = _AcceptButton = UIBase.GetButton(UIBase.ButtonType.Accept);
            Controls.Add(_AcceptButton);
            CancelButton = _CancelButton = UIBase.GetButton(UIBase.ButtonType.Cancel);
            Controls.Add(_CancelButton);

            _FunctionTree.DoubleClick += new EventHandler(FunctionTree_DoubleClick);
            _AcceptButton.Click += new EventHandler(AcceptButton_Click);
            _CancelButton.Click += new EventHandler(CancelButton_Click);
            Layout += new LayoutEventHandler(FunctionChooser_Layout);
            Shown += new EventHandler(FunctionChooser_Shown);

            ResumeLayout(false);
            PerformLayout();
            return;
        } // FunctionChooser()

        #endregion

        #region event handlers

        /// <summary>
        /// Form Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FunctionChooser_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.DL;
        }

        void AcceptButton_Click(object sender, EventArgs e)
        {
            TreeNode oTreeNode = _FunctionTree.SelectedNode;
            if ((oTreeNode != null) && (oTreeNode.Parent != null))
            {
                Function = oTreeNode.Text;
            }
            Close();
            return;
        }  // AcceptButton_Click()

        void CancelButton_Click(object sender, EventArgs e)
        {
            return;
        }  // CancelButton_Click()

        private void FunctionChooser_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && ((e.AffectedProperty == "Bounds") || (e.AffectedProperty == "Visible")))
            {
                // Vertical
                int y = 0;
                _FunctionTree.Top = y;
                y += _FunctionTree.Height;
                y += UIBase.ExtraPadding.Bottom;

                _AcceptButton.Top = y;
                _CancelButton.Top = y;
                y += _AcceptButton.Height;

                // Horizontal
                int x = 0;
                int maxX = -1;
                _FunctionTree.Left = x;
                //x += _FunctionTree.Width;
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
                _FunctionTree.Width = x;
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
                _FunctionTree.Height = y;

            }
            return;
        } // FunctionChooser_Layout

        void FunctionChooser_Shown(object sender, EventArgs e)
        {
            Function = string.Empty;
            Point ptScreen = new Point(Owner.Left, Owner.Bottom);
            Left = ptScreen.X;
            Top = ptScreen.Y;
            return;
        } // FunctionChooser_Shown()

        void FunctionTree_DoubleClick(object sender, EventArgs e)
        {
            TreeView tv = sender as TreeView;
            MouseEventArgs me = e as MouseEventArgs;
            if (tv != null && me != null)
            {
                TreeNode oTreeNode = tv.GetNodeAt(me.Location);
                if ((oTreeNode != null) && (oTreeNode.Parent != null))
                {
                    Function = oTreeNode.Text;
                    Close();
                }
            }
            return;
        } // FunctionTree_DoubleClick()

        #endregion

    } // class FunctionChooser
}