using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using CambridgeSoft.COE.ConfigLoader.Data;

namespace CambridgeSoft.COE.ConfigLoader.Windows.Controls
{
    /// <summary>
    /// Control to display the current job state on the main Form
    /// </summary>
    public partial class JobSpec : UserControl
    {

        #region data (mirrored by job)
        private string _strUser = "";
        private string _strOutputType = "";
        private string _strOutputConfiguration = "";
        private string _strInputDb = "";
        private string _strInputTable = "";
        private string _strInputConfiguration = "";
        private string _strInputFieldSort = "";
        private string _strInputFieldSpec = "";
        private Int32 _nJobCount = 0;
        private Int32 _nJobStart = 0;
        private string _strMappings = "";
        private string _strOutputDb = "";
        #endregion

        #region properties (mirrored by Job)
        /// <summary>
        /// Currently logged in user
        /// </summary>
        public string User
        {
            private get
            {
                return _strUser;
            }
            set
            {
                _strUser = (value != null) ? value : "";
                TreeNodeCollection oTreeNodeCollection = Locate("User");
                if (value != null)
                {
                    TreeNode oTreeNode = oTreeNodeCollection["User"];
                    if (User.Length > 0)
                    {
                        oTreeNode.Text = "Logged in as: " + User;
                    }
                    else
                    {
                        oTreeNode.Text = "Not logged in";
                    }
                }
                else
                {
                    RemoveByKey(oTreeNodeCollection, "User");
                }
                return;
            }
        } // User

        /// <summary>
        /// The chosen output task
        /// </summary>
        public string OutputType
        {
            private get
            {
                return _strOutputType;
            }
            set
            {
                _strOutputType = value;
                TreeNodeCollection oTreeNodeCollection = Locate("Output/Type");
                if (OutputType.Length > 0)
                {
                    TreeNode oTreeNode = oTreeNodeCollection["Type"];
                    oTreeNode.Text = "Task: " + OutputType;
                }
                else
                {
                    RemoveByKey(oTreeNodeCollection, "Type");
                }
                return;
            }
        } // OutputType

        /// <summary>
        /// The current output configuration
        /// </summary>
        public string OutputConfiguration
        {
            private get
            {
                return _strOutputConfiguration;
            }
            set
            {
                _strOutputConfiguration = value;
                TreeNodeCollection oTreeNodeCollection = Locate("Output/Configuration");
                if (OutputConfiguration.Length > 0)
                {
                    TreeNode oTreeNode = oTreeNodeCollection["Configuration"];
                    ConfigurationPopulateNode(oTreeNode, OutputConfiguration);
                }
                else
                {
                    RemoveByKey(oTreeNodeCollection, "Configuration");
                }
                return;
            }
        } // OutputConfiguration

        /// <summary>
        /// The current input database
        /// </summary>
        public string InputDb
        {
            private get
            {
                return _strInputDb;
            }
            set
            {
                _strInputDb = value;
                TreeNodeCollection oTreeNodeCollection = Locate("Input/Db");
                if (InputDb.Length > 0)
                {
                    TreeNode oTreeNode = oTreeNodeCollection["Db"];
                    oTreeNode.Text = "Database: " + InputDb;
                }
                else
                {
                    RemoveByKey(oTreeNodeCollection, "Db");
                }
                return;
            }
        } // InputDb

        /// <summary>
        /// The current input table within the input database
        /// </summary>
        public string InputTable
        {
            private get
            {
                return _strInputTable;
            }
            set
            {
                _strInputTable = value;
                TreeNodeCollection oTreeNodeCollection = Locate("Input/Table");
                if (InputTable.Length > 0)
                {
                    TreeNode oTreeNode = oTreeNodeCollection["Table"];
                    oTreeNode.Text = "Table: " + InputTable;
                }
                else
                {
                    RemoveByKey(oTreeNodeCollection, "Table");
                }
                return;
            }
        } // InputTable

        /// <summary>
        /// The current input configuration
        /// </summary>
        public string InputConfiguration
        {
            private get
            {
                return _strInputConfiguration;
            }
            set
            {
                _strInputConfiguration = value;
                TreeNodeCollection oTreeNodeCollection = Locate("Input/Configuration");
                if (InputConfiguration.Length > 0)
                {
                    TreeNode oTreeNode = oTreeNodeCollection["Configuration"];
                    ConfigurationPopulateNode(oTreeNode, InputConfiguration);
                }
                else
                {
                    RemoveByKey(oTreeNodeCollection, "Configuration");
                }
                return;
            }
        } // InputConfiguration

        /// <summary>
        /// The input field sort specification for the selected table
        /// </summary>
        public string InputFieldSort
        {
            private get
            {
                return _strInputFieldSort;
            }
            set
            {
                _strInputFieldSort = value;
                TreeNodeCollection oTreeNodeCollection = Locate("Input/FieldSort");
                if (InputFieldSort.Length > 0)
                {
                    TreeNode oTreeNode = oTreeNodeCollection["FieldSort"];
                    FieldSortPopulateNode(oTreeNode, InputFieldSort);
                }
                else
                {
                    RemoveByKey(oTreeNodeCollection, "FieldSort");
                }
                return;
            }
        } // InputFieldSort

        /// <summary>
        /// The input field specification for the selected table
        /// </summary>
        public string InputFieldSpec
        {
            private get
            {
                return _strInputFieldSpec;
            }
            set
            {
                _strInputFieldSpec = value;
                TreeNodeCollection oTreeNodeCollection = Locate("Input/FieldSpec");
                if (InputFieldSpec.Length > 0)
                {
                    TreeNode oTreeNode = oTreeNodeCollection["FieldSpec"];
                    FieldSpecPopulateNode(oTreeNode, InputFieldSpec);
                }
                else
                {
                    RemoveByKey(oTreeNodeCollection, "FieldSpec");
                }
                return;
            }
        } // InputFieldSpec

        /// <summary>
        /// Get/set the count of the number of records to be processed
        /// </summary>
        public Int32 JobCount
        {
            private get
            {
                return _nJobCount;
            }
            set
            {
                _nJobCount = value;
                TreeNodeCollection oTreeNodeCollection = Locate("Configuration/JobCount");
                if (JobCount > 0)
                {
                    TreeNode oTreeNode = oTreeNodeCollection["JobCount"];
                    if (JobCount == Int32.MaxValue)
                    {
                        oTreeNode.Text = "Process Count: All Records";
                    }
                    else
                    {
                        oTreeNode.Text = "Process Count: " + JobCount.ToString() + " Records";
                    }
                }
                else
                {
                    RemoveByKey(oTreeNodeCollection, "JobCount");
                }
                return;
            }
        } // JobCount

        /// <summary>
        /// Get/set the starting record number for processing (1-relative)
        /// </summary>
        public Int32 JobStart
        {
            private get
            {
                return _nJobStart;
            }
            set
            {
                _nJobStart = value;
                TreeNodeCollection oTreeNodeCollection = Locate("Configuration/JobStart");
                if (JobStart > 0)
                {
                    TreeNode oTreeNode = oTreeNodeCollection["JobStart"];
                    oTreeNode.Text = "Process Start: Record " + JobStart.ToString();
                }
                else
                {
                    RemoveByKey(oTreeNodeCollection, "JobStart");
                }
                return;
            }
        } // JobStart

        /// <summary>
        /// The current output field mappings
        /// </summary>
        public string Mappings
        {
            private get
            {
                return _strMappings;
            }
            set
            {
                _strMappings = value;
                TreeNodeCollection oTreeNodeCollection = Locate("InputOutput/Mappings");
                if (Mappings.Length > 0)
                {
                    TreeNode oTreeNode = oTreeNodeCollection["Mappings"];
                    MappingsPopulateNode(oTreeNode, Mappings);
                }
                else
                {
                    RemoveByKey(oTreeNodeCollection, "Mappings");
                }
                return;
            }
        } // Mappings

        /// <summary>
        /// The current output database
        /// </summary>
        public string OutputDb
        {
            private get
            {
                return _strOutputDb;
            }
            set
            {
                _strOutputDb = value;
                TreeNodeCollection oTreeNodeCollection = Locate("Output/Db");
                if (OutputDb.Length > 0)
                {
                    TreeNode oTreeNode = oTreeNodeCollection["Db"];
                    oTreeNode.Text = "Database: " + OutputDb;
                }
                else
                {
                    RemoveByKey(oTreeNodeCollection, "Db");
                }
                return;
            }
        } // OutputDb

        #endregion

        #region data
        private Job _Job = null;
        private int _nPreferredHeight;
        private readonly string _kstrRoot = "Job/";   //  "Job/";
        private System.Windows.Forms.TreeView _TreeView;
        #endregion

        #region properties
        /// <summary>
        /// Connection to the <see cref="Job"/> to which we are synchronizing
        /// </summary>
        public Job Job
        {
            set
            {
                _Job = value;
                if (_Job != null)
                {
                    _Job.Changes += new Job.JobChangesEventHandler(Job_Changes);
                    _TreeView.Nodes.Clear();
                }
                return;
            }
        } // Job

        /// <summary>
        /// Let the UI know how tall we would like to be
        /// </summary>
        public int PreferredHeight
        {
            get
            {
                return _nPreferredHeight;
            }
        } // PreferredHeight
        #endregion

        #region constructors
        /// <summary>
        /// ! Constructor
        /// </summary>
        public JobSpec()
        {
            InitializeComponent();
            SuspendLayout();
            _TreeView = UIBase.GetTreeView();
            _TreeView.ShowLines = false;   // true
            _TreeView.AfterExpand += new TreeViewEventHandler(TreeView_AfterExpand);
            _TreeView.AfterCollapse += new TreeViewEventHandler(TreeView_AfterCollapse);
            _TreeView.Sorted = true;
            Controls.Add(_TreeView);
            Layout += new LayoutEventHandler(JobSpec_Layout);
            ResumeLayout(false);
            PerformLayout();
            return;
        }  // JobSpec()
        #endregion

        #region methods
        private void ComputeVisibleBounds(TreeNodeCollection vtnc, ref int rnMinLeft, ref int rnMinTop, ref int rnMaxRight, ref int rnMaxBottom)
        {
            foreach (TreeNode tn in vtnc)
            {
                int nLeft = tn.Bounds.Left;
                int nTop = tn.Bounds.Top;
                int nRight = tn.Bounds.Right;
                int nBottom = tn.Bounds.Bottom;
                if (rnMinLeft > nLeft) rnMinLeft = nLeft;
                if (rnMinTop > nTop) rnMinTop = nTop;
                if (rnMaxRight < nRight) rnMaxRight = nRight;
                if (rnMaxBottom < nBottom) rnMaxBottom = nBottom;
                if (tn.IsExpanded)
                {
                    ComputeVisibleBounds(tn.Nodes, ref rnMinLeft, ref rnMinTop, ref rnMaxRight, ref rnMaxBottom);
                }
            }
            return;
        } // ComputeVisibleBounds()

        private void ConfigurationGroupBoxPopulateNode(TreeNode vtnParent, XmlNode voXmlNode)
        {
            string strGroupBoxText;
            {
                XmlAttribute oXmlAttribute = voXmlNode.Attributes["text"];
                strGroupBoxText = oXmlAttribute.Value;
            }
            int nValue;
            {
                XmlAttribute oXmlAttribute = voXmlNode.Attributes["value"];
                nValue = Convert.ToInt32(oXmlAttribute.Value);
            }
            XmlNode oXmlNodeRadioButton = null;
            foreach (XmlNode oXmlNode in voXmlNode) {
                if (oXmlNode.Name != "RadioButton") continue;
                if (nValue == 0)
                {
                    oXmlNodeRadioButton = oXmlNode;
                    break;
                }
                nValue--;
            }
            // Should check oXmlNodeRadioButton
            string strRadioButtonText;
            {
                XmlAttribute oXmlAttribute = oXmlNodeRadioButton.Attributes["text"];
                strRadioButtonText = oXmlAttribute.Value;
            }
            TreeNode oTreeNode = new TreeNode();
            oTreeNode.Text = strGroupBoxText + ": " + strRadioButtonText;
            vtnParent.Nodes.Add(oTreeNode);
            XmlNode oXmlNodeSibling = oXmlNodeRadioButton.NextSibling;
            if (oXmlNodeSibling != null)
            {
                if (oXmlNodeSibling.Name == "GroupBox")
                {
                    ConfigurationGroupBoxPopulateNode(oTreeNode, oXmlNodeSibling);
                }
            }
            return;
        } // ConfigurationGroupBoxPopulateNode()

        private void ConfigurationPopulateNode(TreeNode vtn, string vxmlConfiguration)
        {
            vtn.Nodes.Clear();
            XmlDocument oXmlDocument = new XmlDocument();
            oXmlDocument.LoadXml(vxmlConfiguration);
            XmlNode oXmlNodeRoot = oXmlDocument.DocumentElement;
            XmlAttribute oXmlAttribute = oXmlNodeRoot.Attributes["text"];
            vtn.Text = oXmlAttribute.Value;
            foreach (XmlNode oXmlNode in oXmlNodeRoot)
            {
                switch (oXmlNode.Name)
                {
                    case "GroupBox":
                        {
                            ConfigurationGroupBoxPopulateNode(vtn, oXmlNode);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            return;
        } // ConfigurationPopulateNode()

        private void FieldSortPopulateNode(TreeNode vtnParent, string vxmlConfiguration)
        {
            XmlDocument oXmlDocument = new XmlDocument();
            oXmlDocument.LoadXml(vxmlConfiguration);
            XmlNode oXmlNodeRoot = oXmlDocument.DocumentElement;
            //XmlAttribute oXmlAttribute = oXmlNodeRoot.Attributes["text"];
            //vtn.Text = oXmlAttribute.Value;
            vtnParent.Text = "Field sort specification";
            foreach (XmlNode oXmlNode in oXmlNodeRoot)
            {
                TreeNode oTreeNode = new TreeNode();
                string strText = "";
                {
                    XmlAttribute oXmlAttribute = oXmlNode.Attributes["dbname"];
                    strText += "dbname: " + oXmlAttribute.Value;
                }
                strText += " ";
                {
                    XmlAttribute oXmlAttribute = oXmlNode.Attributes["orderby"];
                    strText += "orderby: " + oXmlAttribute.Value;
                }
                oTreeNode.Text = strText;
                vtnParent.Nodes.Add(oTreeNode);
            }
            return;
        } // FieldSortPopulateNode()

        private void FieldSpecPopulateNode(TreeNode vtnParent, string vxmlConfiguration)
        {
            vtnParent.Nodes.Clear();
            XmlDocument oXmlDocument = new XmlDocument();
            oXmlDocument.LoadXml(vxmlConfiguration);
            XmlNode oXmlNodeRoot = oXmlDocument.DocumentElement;
            //XmlAttribute oXmlAttribute = oXmlNodeRoot.Attributes["text"];
            //vtn.Text = oXmlAttribute.Value;
            vtnParent.Text = "Field specification";
            foreach (XmlNode oXmlNode in oXmlNodeRoot)
            {
                TreeNode oTreeNode = new TreeNode();
                string strText = "";
                {
                    XmlAttribute oXmlAttribute = oXmlNode.Attributes["dbname"];
                    strText += "dbname: " + oXmlAttribute.Value;
                }
                strText += " ";
                {
                    XmlAttribute oXmlAttribute = oXmlNode.Attributes["dbtype"];
                    strText += "dbtype: " + oXmlAttribute.Value;
                }
                strText += " ";
                {
                    XmlAttribute oXmlAttribute = oXmlNode.Attributes["dbtypereadonly"];
                    if ((oXmlAttribute != null) && (oXmlAttribute.Value.ToLower() == "true"))
                    {
                    strText += "dbtypereadonly ";
                    }
                }
                {
                    XmlAttribute oXmlAttribute = oXmlNode.Attributes["name"];
                    strText += "label: " + oXmlAttribute.Value;
                }
                strText += " ";
                {
                    XmlAttribute oXmlAttribute = oXmlNode.Attributes["type"];
                    strText += "type: " + oXmlAttribute.Value;
                }
                oTreeNode.Text = strText;
                vtnParent.Nodes.Add(oTreeNode);
            }
            return;
        } // FieldSpecPopulateNode()

        private void MappingsPopulateNode(TreeNode vtnParent, string vxmlConfiguration)
        {
            vtnParent.Nodes.Clear();
            XmlDocument oXmlDocument = new XmlDocument();
            oXmlDocument.LoadXml(vxmlConfiguration);
            XmlNode oXmlNodeRoot = oXmlDocument.DocumentElement;
            //XmlAttribute oXmlAttribute = oXmlNodeRoot.Attributes["text"];
            //vtn.Text = oXmlAttribute.Value;
            vtnParent.Text = "Mappings";
            foreach (XmlNode oXmlNode in oXmlNodeRoot)
            {
                TreeNode oTreeNode = new TreeNode();
                string strText = "";
                string strName = "";
                {
                    XmlAttribute oXmlAttribute = oXmlNode.Attributes["name"];
                    strName = oXmlAttribute.Value;
                }
                string strSource = "";
                {
                    XmlAttribute oXmlAttribute = oXmlNode.Attributes["source"];
                    strSource = oXmlAttribute.Value;
                }
                string strValue = "";
                {
                    XmlAttribute oXmlAttribute = oXmlNode.Attributes["value"];
                    if (oXmlAttribute != null) strValue = oXmlAttribute.Value;
                }
                strText = "name: " + strName + " " + "source: " + strSource;
                if (strValue.Length > 0)
                {
                    strText += " " + "value: " + strValue;
                }
                oTreeNode.Text = strText;
                vtnParent.Nodes.Add(oTreeNode);
            }
            return;
        } // FieldSpecPopulateNode()

        private TreeNodeCollection Locate(string vstrPath)
        {
            TreeNode tn = null;
            TreeNodeCollection tnc = null;
            string[] strPathPiece = (_kstrRoot + vstrPath).Split('/');
            for (int n = 0; n < strPathPiece.Length; n++)
            {
                tnc = (tn == null) ? _TreeView.Nodes : tn.Nodes;
                if (tnc.ContainsKey(strPathPiece[n]) == false)
                {
                    string strText = "";
                    if (n < strPathPiece.Length - 1)
                    {
                        int startIndex = (_kstrRoot.Length > 0) ? 1 : 0;
                        int count = (1 - startIndex) + n;
                        if (n == 0)
                        {
                            startIndex = 0;
                            count = 1;
                        }
                        strText = String.Join("/", strPathPiece, startIndex, count);
                    }
                    tnc.Add(strPathPiece[n], strText);
                    if ((n == 1) && (tnc.Count == 1) && (_kstrRoot.Length > 0))
                    {
                        tn.Expand();
                    }
                }
                tn = tnc[strPathPiece[n]];
            }
            return tnc;
        } // Locate

        private void RemoveByKey(TreeNodeCollection voTreeNodeCollection, string vstrKey)
        {
            TreeNode oTreeNodeParent = voTreeNodeCollection[vstrKey].Parent;
            voTreeNodeCollection.RemoveByKey(vstrKey);
            if ((oTreeNodeParent != null) && (oTreeNodeParent.Nodes.Count == 0))
            {
                TreeNodeCollection oTreeNodeCollection = (oTreeNodeParent.Parent == null) ? _TreeView.Nodes : oTreeNodeParent.Parent.Nodes;
                RemoveByKey(oTreeNodeCollection, oTreeNodeParent.Name);
            }
            return;
        } // RemoveByKey()

        private void VisibleBoundsChanged()
        {
            int nLeft = 0;
            int nTop = 0;
            int nRight = 0;
            int nBottom = 0;
            ComputeVisibleBounds(_TreeView.Nodes, ref nLeft, ref nTop, ref nRight, ref nBottom);
            int nWidth = (nRight - nLeft);
            int nHeight = (nBottom - nTop);
            bool bHscroll = false;
            bool bVscroll = false;
            bHscroll = (nWidth > MaximumSize.Width);    // Hscroll due to content
            if (bHscroll == false)
            {
                bVscroll = (nHeight > MaximumSize.Height);  // Vscroll due to content
                if (bVscroll)
                {
                    nWidth += 16;
                    bHscroll = (nWidth > MaximumSize.Width);    // Hscroll due to Vscroll
                }
            }
            if (bHscroll)
            {
                nHeight += 16;
                if (bVscroll == false)
                {
                    bVscroll = (nHeight > MaximumSize.Height);  // Vscroll dur to Hscroll
                    if (bVscroll) nWidth += 16;
                }
            }
            _nPreferredHeight = _TreeView.Margin.Top + nHeight + _TreeView.Margin.Bottom;
            if (_nPreferredHeight > MaximumSize.Height) nHeight = MaximumSize.Height - (_TreeView.Margin.Top + _TreeView.Margin.Bottom);
            _TreeView.Height = _TreeView.Margin.Top + nHeight + _TreeView.Margin.Bottom;
            return;
        } // VisibleBoundsChanged()

        #endregion

        #region events
        private void Job_Changes(object sender, Job.JobChangesEventArgs e)
        {
            Job thisJob = sender as Job;
            Job.JobChangesEventArgs je = e as Job.JobChangesEventArgs;
            string[] strProperties = je.properties.Split(';');
            Visible = true;
            foreach (string strProperty in strProperties)
            {
                Object o = thisJob.GetType().InvokeMember(strProperty, System.Reflection.BindingFlags.GetProperty, null, thisJob, null);
                try
                {
                    this.GetType().InvokeMember(strProperty, System.Reflection.BindingFlags.SetProperty, null, this, new object[] { o });
                }
                catch (Exception)
                {
                    throw;
                }
            }
            VisibleBoundsChanged();
            return;
        } // Job_Changes()

        private void JobSpec_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && (e.AffectedProperty == "Bounds"))
            {
                int y = 0;
                _TreeView.Top = y;
                {
                    int nHeight = nHeight = _nPreferredHeight - (_TreeView.Margin.Top + _TreeView.Margin.Bottom);
                    if (_nPreferredHeight > MaximumSize.Height) nHeight = MaximumSize.Height - (_TreeView.Margin.Top + _TreeView.Margin.Bottom);
                    _TreeView.Height = _TreeView.Margin.Top + nHeight + _TreeView.Margin.Bottom;
                }
                y += _TreeView.Height;
                Height = y;
                //
                int x = 0;
                _TreeView.Left = x;
                _TreeView.Width = MaximumSize.Width;
                Width = _TreeView.Width;
            }
            return;
        } // JobSpec_Layout()

        void TreeView_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            VisibleBoundsChanged();
            return;
        }

        void TreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            VisibleBoundsChanged();
            return;
        }

        #endregion
    } // class JobSpec
}
