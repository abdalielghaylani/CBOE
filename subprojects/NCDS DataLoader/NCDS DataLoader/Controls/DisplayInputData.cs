using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.NCDS_DataLoader.Common;
using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.NCDS_DataLoader.Forms;
using CambridgeSoft.COE.DataLoader.Core.FileParser.SD;
using CambridgeSoft.COE.DataLoader.Core.Workflow;
using System.IO;

namespace CambridgeSoft.NCDS_DataLoader.Controls
{
    public partial class DisplayInputData : UIBase
    {
        private Dictionary<ISourceRecord, string> records;
        private DataTable dt = new DataTable();
        private JobParameters _job;
        private string _fName;
        private int _times = 0;

        public JobParameters JOB
        {
            set { this._job = value; }
        }

        public string FileName
        {
            get
            {
                return _fName;
            }
        }

        public Dictionary<ISourceRecord, string> InputData
        {
            set { this.records = value; }
        }

        public DataTable Data
        {
            set
            {
                this.dt = value;
                if (dt.Rows.Count > 0)
                {
                    dataGridView1.DataSource = dt;
                    dataGridView1.Columns[0].Width = 40;
                    dataGridView1.Columns[1].Width = 40;
                    dataGridView1.Columns[1].HeaderText = "Marked";
                    int index = dataGridView1.Columns.Count - 1;
                    dataGridView1.Columns[index].Visible = false;
                    int columncount = dataGridView1.Columns.Count;
                    for (int i = 0; i < columncount; i++)
                    {
                        if (i != 1)
                        {
                            dataGridView1.Columns[i].ReadOnly = true;
                        }
                    }
                    _times = 0;
                    dataGridView1.Columns[1].Frozen = true;
                    dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

                }
                else
                {
                    dataGridView1.Rows.Clear();
                }
            }
        }

        public DisplayInputData()
        {
            InitializeComponent();

            // accept/cancel
            Controls.Add(AcceptButton);
            Controls.Add(CancelButton);
            AcceptButton.Top = 450;
            CancelButton.Top = 450;
            CancelButton.Click += new EventHandler(CancelButton_Click);
            AcceptButton.Click += new EventHandler(AcceptButton_Click);
        }

        private void AcceptButton_Click(object sender, EventArgs e)
        {
            string select = "Check = true";
            DataRow[] drs = dt.Select(select);

            if (drs.Length == 0)
            {
                MessageBox.Show("No record have been checked!", "NCDS DataLoader", MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            int index = 0;
            _fName = string.Empty;
            string fullPath = _job.DataSourceInformation.DerivedFileInfo.FullName;
            int indexDot = fullPath.LastIndexOf('.');

            //_fName = fullPath.Insert(indexDot, index.ToString());

            _fName = JobUtility.GetPurposedFilePath(index.ToString(), _job.DataSourceInformation.DerivedFileInfo);
            while (File.Exists(_fName))
            {
                _fName = JobUtility.GetPurposedFilePath(index.ToString(), _job.DataSourceInformation.DerivedFileInfo);
                //_fName = fullPath.Insert(indexDot, index.ToString());
                index++;
            }

            ExportFile.doExportFile(_job, drs, _fName, records);

            OnAccept();
        }


        private void CancelButton_Click(object sender, EventArgs e)
        {
            OnCancel();
        }

        private void SelectAllButton_Click(object sender, EventArgs e)
        {
            foreach (DataRow dr in dt.Rows)
            {
                dr["Check"] = true;
            }
        }

        private void _Inversebutton_Click(object sender, EventArgs e)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if ((bool)dr["Check"] == false)
                {
                    dr["Check"] = true;
                }
                else
                {
                    dr["Check"] = false;
                }
            }
        }

        private void _BatchMarkbutton_Click(object sender, EventArgs e)
        {
            using(BatchMarkForm form = new BatchMarkForm())
            {
                form.MaxValue = dataGridView1.Rows.Count;

                form.ShowDialog();
                if(form.OK)
                {
                    int[] number = form.GetNumber;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (Convert.ToInt32(dr["No"]) < number[0] ||
                            Convert.ToInt32(dr["No"]) > number[1])
                        {
                            dr["Check"] = false;
                        }
                        else
                        {
                            dr["Check"] = true;
                        }
                    }
                }
            }
        }

        private void _ExportButton_Click(object sender, EventArgs e)
        {
            string filter = string.Empty;
            switch (_job.DataSourceInformation.FileType)
            {
                case SourceFileType.MSExcel:
                case SourceFileType.CSV:
                    filter = "Text files (*.txt)|*.txt";
                    break;
                case SourceFileType.SDFile:
                    filter = "SD files (*.sdf)|*.sdf";
                    break;
                default:
                    break;
            }

            string select = "Check = true";
            DataRow[] drs = dt.Select(select);

            if (drs.Length == 0)
            {
                MessageBox.Show("No record have been checked!", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = filter;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fName = saveFileDialog.FileName;

                try
                {
                    if (File.Exists(fName))
                    {
                        File.Delete(fName);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ExportFile.doExportFile(_job, drs, fName, records);

                MessageBox.Show("Export completed!", "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (_times == 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if ((bool)dt.Rows[i]["check"] == true)
                    {
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    }
                }
                _times++;
            }
        }

        private void dataGridView1_Sorted(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (!string.IsNullOrEmpty(dataGridView1.Rows[i].Cells[2].Value.ToString()) == true)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                }
            }
        }
    }
}
