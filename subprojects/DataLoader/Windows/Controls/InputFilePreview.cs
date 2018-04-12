using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.DataLoader.Windows.Controls
{
    /// <summary>
    /// GUI class for import data-preview.
    /// </summary>
    public partial class InputFilePreview : UserControl
    {

        #region >Properties and Provate Data<

        private string _inputFieldSpec = string.Empty;
        private XmlDocument _inputFieldSpecDoc;
        private DataSet _previewDataSet;
        private BindingManagerBase _bindingManagerBase;

        /// <summary>
        /// Get property to return XML of the input field list
        /// Set property to initialize the UI with the input field list
        /// </summary>
        public string InputFieldSpec
        {
            get
            {
                /*
                COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
                oCOEXmlTextWriter.WriteStartElement("fieldlist");
                for (int nInputField = 0; nInputField < _MainLabel.GetLength(0); nInputField++)
                {
                    string[] strDbInfo = ((string)_MainLabel[nInputField].Tag).Split(';');
                    oCOEXmlTextWriter.WriteStartElement("field");
                    oCOEXmlTextWriter.WriteAttributeString("dbname", strDbInfo[0]);
                    oCOEXmlTextWriter.WriteAttributeString("dbtype", strDbInfo[1]);
                    if (_MainComboBox[nInputField].Enabled == false) {
                        oCOEXmlTextWriter.WriteAttributeString("dbtypereadonly", "true");
                    }
                    oCOEXmlTextWriter.WriteAttributeString("name", _MainLabel[nInputField].Text.Trim());
                    oCOEXmlTextWriter.WriteAttributeString("type", _MainComboBox[nInputField].SelectedItem.ToString());
                    oCOEXmlTextWriter.WriteEndElement();
                }
                oCOEXmlTextWriter.WriteEndElement();
                string xmlFieldSpec = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
                oCOEXmlTextWriter.Close();
                return xmlFieldSpec;
                */
                return _inputFieldSpec;
            }
            set
            {
                _inputFieldSpec = value;
                ExtractInputFieldSpec();
            }
        }

        /// <summary>
        /// Preview DataSet object
        /// </summary>
        public DataSet PreviewDataSet
        {
            set
            {
                _previewDataSet = value;
                this.dgvPreviewSet.DataSource = _previewDataSet.Tables[0];

                _bindingManagerBase = this.BindingContext[this._previewDataSet.Tables[0], null];
                _bindingManagerBase.Position = 0;

                _bindingManagerBase.PositionChanged += _bindingManagerBase_PositionChanged;

                BindRecordData();
            }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public InputFilePreview()
        {
            InitializeComponent();

            this.dgvPreviewSet.CellFormatting += dgvRowDetails_CellFormatting;
            this.dgvPreviewSet.DataError += dgvRowDetails_DataError;
            this.dgvPreviewSet.Sorted += dgvPreviewSet_Sorted;
        }

        /// <summary>
        /// Digests the input field specifications in order to generate the UI
        /// </summary>
        private void ExtractInputFieldSpec()
        {
            if (string.IsNullOrEmpty(_inputFieldSpec))
                return;

            XmlDocument fieldSpec = new XmlDocument();
            fieldSpec.LoadXml(_inputFieldSpec);
            _inputFieldSpecDoc = fieldSpec;

            XmlNode fieldList = fieldSpec.DocumentElement;

            foreach (XmlNode field in fieldList)
            {
                string name = field.Attributes["dbname"].Value;
                string type = field.Attributes["dbtype"].Value;


            }
            this.textBox1.Text = Utilities.FormatXmlString(_inputFieldSpec);
        }

        /// <summary>
        /// Provides a concurrent view of the selected record (horizontally formatted)
        /// in a separate control (vertically formatted).
        /// </summary>
        private void BindRecordData()
        {
            //Manually apply DataRow values to the template field spec
            foreach (DataColumn dc in _previewDataSet.Tables[0].Columns)
            {
                DataRowView drv = (DataRowView)_bindingManagerBase.Current;
                DataRow dr = drv.Row;

                foreach (XmlNode n in this._inputFieldSpecDoc.DocumentElement.ChildNodes)
                {
                    if (n.Attributes["dbname"].Value.ToLower() == dc.ColumnName.ToLower())
                    {
                        XmlAttribute a = this._inputFieldSpecDoc.CreateAttribute("value");
                        string aValue = String.Empty;
                        if (dc.DataType.IsArray)
                        {
                            byte[] rawValue = (byte[])dr[dc];
                            string byteString = string.Empty;
                            string peekValue =
                                Convert.ToBase64String(rawValue, 0, 32, Base64FormattingOptions.InsertLineBreaks);

                            if (peekValue.StartsWith("VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACA"))
                            {
                                byteString = Convert.ToBase64String(rawValue);
                            }
                            else
                            {
                                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                                byteString = encoding.GetString(rawValue);
                            }
                            aValue = byteString;

                        }
                        else
                            aValue = dr[dc].ToString();

                        a.Value = aValue;
                        n.Attributes.Append(a);
                    }
                }
            }

            //Push the updated xml data into the grid
            byte[] byteArray = Encoding.ASCII.GetBytes(_inputFieldSpecDoc.OuterXml);
            DataSet ds = new DataSet();
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray))   // Coverity Fix- CBOE-1941
            {
                ds.ReadXml(stream, XmlReadMode.InferSchema);
            }
            if (ds != null && ds.Tables.Count > 0)     // Coverity Fix- CBOE-1941
            {
                this.dgvRowDetails.DataSource = ds.Tables[0];

                this.dgvRowDetails.Columns["dbname"].HeaderText = "Field";
                this.dgvRowDetails.Columns["dbtype"].HeaderText = "Type";
                this.dgvRowDetails.Columns["value"].HeaderText = "Preview value";
            }
        }

        //Control event-handlers

        private void _bindingManagerBase_PositionChanged(object sender, EventArgs e)
        {
            BindRecordData();
        }

        private void dgvRowDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (dgv != null)
            {
                if (dgv.Columns[e.ColumnIndex].CellType != null)
                {
                    if (dgv.Columns[e.ColumnIndex].CellType.Name != null)
                    {
                        if (dgv.Columns[e.ColumnIndex].CellType.Name == "DataGridViewImageCell")
                        {
                            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                            string byteString = encoding.GetString((byte[])e.Value);
                            e.Value = byteString;

                            dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = byteString;

                            e.FormattingApplied = true;
                        }
                    }
                }
            }
        }

        private void dgvRowDetails_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        /// <summary>
        /// handles the file preview grid 'Sorted' event; re-binds the details grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvPreviewSet_Sorted(object sender, EventArgs e)
        {
            BindRecordData();
        }

    }
}
