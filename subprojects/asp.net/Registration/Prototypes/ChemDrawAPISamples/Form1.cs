using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;

namespace GenerateFilesFromChemDraw
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        OleDbConnection cn = new OleDbConnection();
        OleDbCommand cmd = new OleDbCommand();
        DataSet ds = new DataSet();
        OleDbDataAdapter da = new OleDbDataAdapter();

        DBLayer DBManip = new DBLayer();

        OpenFileDialog browFile;
        FolderBrowserDialog openfld;

        //ChemDrawControl10.ChemDrawCtl ChemDrawCtl1 = new ChemDrawControl10.ChemDrawCtl();
        ChemDrawControl11.ChemDrawCtl ChemDrawCtl1 = new ChemDrawControl11.ChemDrawCtl();

        private void btnBrowseGet_Click(object sender, EventArgs e)
        {
            openfld = new FolderBrowserDialog();
            if (openfld.ShowDialog() == DialogResult.OK)
                txtGetFilesPath.Text = openfld.SelectedPath.ToString();
        }
        private object CheckUserSettingDocument(bool chk, string SetDoc)
        {
            object mysettingsdata = new object();
            if (chk == true)
            {
                try
                {
                    ChemDrawCtl1.Open(SetDoc, false);
                    mysettingsdata = ChemDrawCtl1.get_Data("chemical/x-cdx");
                }
                catch
                { }
            }
            return mysettingsdata;
        }
        private void ApplyUserSettingDocument(bool chk, object mysettingsdata)
        {
            if (chk == true)
            {
                try
                {
                    ChemDrawCtl1.Objects.Settings.ApplySettings("chemical/x-cdx", mysettingsdata);
                }
                catch
                { }
            }
        }
        private void CheckFontType(bool chk)
        {
            if (chk == true)
            {
                try
                {
                    ChemDrawCtl1.Settings.LabelFont = txtAtomFT.Text;
                }
                catch
                { }
            }
        }
        private void CheckFontSize(bool chk)
        {
            if (chk == true)
            {
                try
                {
                    ChemDrawCtl1.Settings.LabelSize = Convert.ToDouble(txtAtomFS.Text);
                }
                catch
                { }
            }
        }
        private string CalcProcessDuration(DateTime dtStartTime)
        {
            try
            {
                System.TimeSpan diff = DateTime.Now.TimeOfDay - dtStartTime.TimeOfDay;

                return ("Total time taken :: " + diff.Hours.ToString() + ":Hrs " + diff.Minutes.ToString() + ":Min " + diff.Seconds.ToString() + ":Sec");
            }
            catch (SystemException Ex)
            {
                throw Ex;
            }
        }

        private void CreateFile(string cdxFile, string fileType, string putfilePath, string Res, bool chkdbops, bool chkmwmf, bool chkmw, bool chkmf, string MolWT, string MolFR, string MolWTFR)
        {
            object saveasheight;
            double saveaswidth;
            object gifpath;
            string Extension;
            int lastpart;
            string gifname;
            string gifnoext;

            string[] gifarray;
            string strInsert;
            gifname = cdxFile;
            gifarray = gifname.Split('.');
            lastpart = gifarray.GetUpperBound(0);
            Extension = gifarray.GetValue(lastpart).ToString();

            gifnoext = gifname.Substring(0, gifname.Length - (Extension.Length + 1));
            gifname = gifnoext + "." + fileType;

            gifpath = putfilePath + gifname;

            ChemDrawCtl1.Application.Preferences.AntialiasedGIFs = false;

            saveaswidth = ChemDrawCtl1.Objects.Width;
            saveasheight = ChemDrawCtl1.Objects.Height;

            object gifpathobj = (object)gifpath;
            object emtobj = (object)"";
            object txtResobj = (object)Res;
            //object txtResobj = (object)"";
            object saveaswidthobj = (object)saveaswidth;
            object saveasheightobj = (object)saveasheight;

            try
            {
                ChemDrawCtl1.SaveAs(ref gifpath, ref emtobj, ref txtResobj, ref saveaswidthobj, ref saveasheight);
                ChemDrawCtl1.Objects.Clear();

            }
            catch (SystemException ex)
            {
                throw ex;
            }

            if (chkdbops == true)
            {
                try
                {
                    //requries existing table
                    if ((!chkmwmf == true) && (chkmw == true))
                    {
                        strInsert = "Insert Into " + MolWT + " values('" + gifnoext + "', " + ChemDrawCtl1.Objects.MolecularWeight + ")";
                        //this must be wrong ?
                        cmd = new OleDbCommand(strInsert, cn);
                    }
                    if ((!chkmwmf == true) && (chkmf == true))
                    {
                        strInsert = "Insert Into " + MolFR + " values('" + gifnoext + "', '" + ChemDrawCtl1.Objects.Formula + "')";
                        cmd = new OleDbCommand(strInsert, cn);
                    }

                    if (chkmwmf == true)
                    {
                        strInsert = "Insert Into " + MolWTFR + " values('" + gifnoext + "', " + ChemDrawCtl1.Objects.MolecularWeight + ", '" + ChemDrawCtl1.Objects.Formula + "')";
                        cmd = new OleDbCommand(strInsert, cn);
                    }
                    cmd.ExecuteNonQuery();
                }
                catch
                { }
            }
        }

        string molPath = "c:\\sdf.mol";
        string strBreak = "$$$$";
        string strRow = "";
        string Base64_Cdx = "";
        bool chkId = false;
        string IDN = "";

        private void SDFBase64CDXProcess(string ID, string DBTableName, bool ChkNewExistDB, string SdfFilePath, string DBPath)
        {
            try
            {
                string ConnStr = @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                @"Data source=" + DBPath;
                DBManip.ConnectToAccess(ConnStr);
                if (!ChkNewExistDB)
                {
                    string Qry = "Create Table " + DBTableName + "(ID Text(150), Base64_CDX Memo)";
                    DBManip.ExecuteQuery(Qry);
                }
                DBManip.ClearDataSetTables();
                DBManip.FillTable("Select * from " + DBTableName + " where 1=2", "STRUCT");
                ChemDrawCtl1.DataEncoded = true;
                IDN = ID;

                StringWriter strWrite = new StringWriter();
                StreamReader strRead = File.OpenText(SdfFilePath);

                while (strRead.EndOfStream == false)
                {
                    strRow = strRead.ReadLine().ToString();
                    strWrite.WriteLine(strRow);

                    if (chkId == true)
                    {
                        ID = strRow;
                        chkId = false;
                    }

                    if (strRow.Contains("<" + IDN + ">"))
                    {
                        chkId = true;
                    }

                    if (strRow.Trim() == strBreak)
                    {
                        string mystring = strWrite.ToString();

                        ChemDrawCtl1.Objects.Clear();
                        ChemDrawCtl1.set_Data("chemical/x-mdl-molfile", mystring);

                        Base64_Cdx = ChemDrawCtl1.get_Data("chemical/x-cdx").ToString();
                        DBManip.AddNewRecord("STRUCT", ID, Base64_Cdx);
                        //Initilize the string writer
                        strWrite = new StringWriter();
                        lblGifName.Text = ID;
                        System.Windows.Forms.Application.DoEvents();
                    }
                    strRow = "";
                    Base64_Cdx = "";
                }
                //Data inserted into table
                //DBManip.DBUpdate("STRUCT");
            }
            catch (SystemException Ex)
            {
                throw Ex;
            }
        }
        private void SDFOutputDirectoryProcess(string ID, string SdfFilePath, string OutPutPath)
        {
            try
            {
                IDN = ID;
                StringWriter strWrite = new StringWriter();
                StreamReader strRead = File.OpenText(SdfFilePath);
                object mysettingsdata = new object();
                mysettingsdata = CheckUserSettingDocument(chkUseSetDoc.Checked, txtSetDoc.Text);
                while (strRead.EndOfStream == false)
                {
                    strRow = strRead.ReadLine().ToString();
                    strWrite.WriteLine(strRow);

                    if (chkId == true)
                    {
                        ID = strRow;
                        chkId = false;
                    }

                    if (strRow.Contains("<" + IDN + ">"))
                    {
                        chkId = true;
                    }

                    if (strRow.Trim() == strBreak)
                    {
                        string mystring = strWrite.ToString();

                        ChemDrawCtl1.set_Data("chemical/x-mdl-molfile", mystring);

                        // Checking the structure exist or not
                        if (ChemDrawCtl1.Objects.Count > 0)
                        {
                            //Apply chemdraw setting
                            ApplyUserSettingDocument(chkUseSetDoc.Checked, mysettingsdata);
                            //Apply font size and font type.
                            CheckFontSize(chkChangeFS.Checked);
                            CheckFontType(chkChangeFT.Checked);

                            //Create the output file type 
                            CreateFile(ID + ".cdx", txtType.Text, txtPutFilePath.Text, txtRes.Text, chkDoDBOps.Checked, chkmwandmf.Checked, chkMWs.Checked, chkMFs.Checked, txtMWtbl.Text, txtMFtbl.Text, txtMWMFtbl.Text);
                        }
                        strWrite = new StringWriter();

                        lblGifName.Text = ID;
                        System.Windows.Forms.Application.DoEvents();
                    }
                    strRow = "";
                }
            }
            catch (SystemException Ex)
            {
                throw Ex;
            }
        }
        private void SDFMolFilesProcess(string ID, string SdfFilePath, string OutPutPath)
        {
            try
            {
                string molDir = OutPutPath;
                IDN = ID;
                StringWriter strWrite = new StringWriter();
                StreamReader strRead = File.OpenText(SdfFilePath);

                while (strRead.EndOfStream == false)
                {
                    strRow = strRead.ReadLine().ToString();
                    strWrite.WriteLine(strRow);

                    if (chkId == true)
                    {
                        ID = strRow;
                        chkId = false;
                    }
                    if (strRow.Contains("<" + IDN + ">"))
                    {
                        chkId = true;
                    }

                    if (strRow.Trim() == strBreak)
                    {
                        molPath = molDir + ID + ".mol";
                        FileInfo f = new FileInfo(molPath);
                        //Create mol file
                        StreamWriter stWrit = f.CreateText();
                        stWrit.Close();

                        File.WriteAllText(molPath, strWrite.ToString());
                        strWrite = new StringWriter();
                        molPath = "";

                        lblGifName.Text = ID;
                        System.Windows.Forms.Application.DoEvents();
                    }
                    strRow = "";
                }
            }
            catch (SystemException Ex)
            {
                throw Ex;
            }
        }

        public void SDFInputEnableDisable(bool SdfInput, bool SdfAccess, bool SdfId, bool SdfTable)
        {
            txtInputSDF.Enabled = SdfInput;
            txtSdfAccessDB.Enabled = SdfAccess;
            txtSdfIDField.Enabled = SdfId;
            txtSdfTableName.Enabled = SdfTable;
        }
        public void TabsMessage()
        {
            if (tabOptions.SelectedIndex == 0)
            {
                lblTabMessage.Text = "Use this tab to set the general properties";
            }
            else if (tabOptions.SelectedIndex == 1)
            {
                lblTabMessage.Text = "Use this tab to set the advanced properties";
            }

            else if (tabOptions.SelectedIndex == 2)
            {
                lblTabMessage.Text = "Fill in this tab to use an access database as input.  The database table you choose must have base64 or mol data stored in the table";
            }
            else if (tabOptions.SelectedIndex == 3)
            {
                lblTabMessage.Text = "Fill in this tab to use SD file as input.  The process will output either a database or files to a directory based on the information you fill in below";
            }
            else if (tabOptions.SelectedIndex == 4)
            {
                lblTabMessage.Text = "Use this tab to output Chemical Properties to a database table";
            }
        }

        private void SelectSDFFileProcess()
        {
            if (cmbSdfSelect.SelectedIndex == 0)
            {
                SDFInputEnableDisable(true, true, true, true);
                lblSdfProcess.Text = "Output base64_cdx to database";
                lblSdfDirectoryMessage.Text = "";
            }
            else if (cmbSdfSelect.SelectedIndex == 1)
            {
                SDFInputEnableDisable(true, false, true, false);
                lblSdfProcess.Text = "Output molfiles to database";
                lblSdfDirectoryMessage.Text = "";
            }
            else if (cmbSdfSelect.SelectedIndex == 2)
            {
                SDFInputEnableDisable(true, false, false, false);
                lblSdfProcess.Text = "Output files to the directory";
                lblSdfDirectoryMessage.Text = "Use the options on the \"General Output Options\" and \"Advanced Output Options\" to change the files being output files being generated";
            }
        }

        private void SelectProcessTab()
        {
            if (cmbProcessTab.SelectedIndex == 0)
            {
                tabOptions.SelectedIndex = 0;
            }
            else if (cmbProcessTab.SelectedIndex == 1)
            {
                tabOptions.SelectedIndex = 2;
            }
            else if (cmbProcessTab.SelectedIndex == 2)
            {
                tabOptions.SelectedIndex = 3;
            }
        }

        private void cmdCreateFiles_Click(object sender, EventArgs e)
        {
            try
            {
                lblProcessing.Text = "Processing...";
                lblGifName.Text = "";
                System.Windows.Forms.Application.DoEvents();

                DateTime dtStartTime;
                dtStartTime = DateTime.Now;

                if (cmbProcessTab.SelectedIndex == 1)
                {
                    if (txtoutputID.Text == "" || txttheStructure.Text == "" || txtoutTable.Text == "" || txtConnStr2.Text == "")
                    {
                        MessageBox.Show("Parameter missing in DB Input tab");
                        return;
                    }
                    //Open databse connction
                    DBManip.ConnOpen(true, txtConnStr2.Text);
                    string Qry = "";

                    // Query 
                    if (chkCurrQry.Checked == true)
                        Qry = txtQryBox.Text;
                    else
                        // Qry = "Select Mol_ID as " + txtoutputID.Text + "," + "BASE64_CDX as " + txttheStructure.Text + " from " + txtoutTable.Text;
                        Qry = "Select " + txtoutputID.Text + " as Mol_ID " + ", " + txttheStructure.Text + " as BASE64_CDX " + "from " + txtoutTable.Text;

                    // Records are retrieved into datatable
                    DBManip.ClearDataSetTables();
                    DataTable TotRecs = DBManip.FillTable(Qry);
                    string stringbase64 = "";

                    ChemDrawCtl1.DataEncoded = true;
                    object mysettingsdata = new object();
                    mysettingsdata = CheckUserSettingDocument(chkUseSetDoc.Checked, txtSetDoc.Text);
                    foreach (DataRow drRow in TotRecs.Rows)
                    {
                        ChemDrawCtl1.Objects.Clear();
                        stringbase64 = drRow["BASE64_CDX"].ToString();
                        ChemDrawCtl1.set_Data("chemical/x-cdx", stringbase64);

                        if ((ChemDrawCtl1.Objects.Count > 0) || ((ChemDrawCtl1.Objects.Count == 0) && (chkEmtStructure.Checked == false)))
                        {
                            //Apply chemdraw setting
                            ApplyUserSettingDocument(chkUseSetDoc.Checked, mysettingsdata);
                            //Apply font size and font type
                            CheckFontSize(chkChangeFS.Checked);
                            CheckFontType(chkChangeFT.Checked);

                            //Create output file type 
                            CreateFile(drRow["Mol_ID"].ToString() + ".cdx", txtType.Text, txtPutFilePath.Text, txtRes.Text, chkDoDBOps.Checked, chkmwandmf.Checked, chkMWs.Checked, chkMFs.Checked, txtMWtbl.Text, txtMFtbl.Text, txtMWMFtbl.Text);
                        }

                        lblGifName.Text = drRow["Mol_ID"].ToString() + "." + txtType.Text;
                        System.Windows.Forms.Application.DoEvents();
                    }
                    //Connection close
                    DBManip.ConnClose(true);
                }
                else if (cmbProcessTab.SelectedIndex == 0)
                {
                    //All files from input directory into directoryinfo
                    DirectoryInfo cdxFolder = new DirectoryInfo(txtGetFilesPath.Text);
                    //open database connection
                    DBManip.ConnOpen(chkDoDBOps.Checked, txtConnString.Text);
                    object mysettingsdata = new object();
                    mysettingsdata = CheckUserSettingDocument(chkUseSetDoc.Checked, txtSetDoc.Text);
                    foreach (FileInfo cdxFile in cdxFolder.GetFiles())
                    {
                        ChemDrawCtl1.Objects.Clear();
                        ChemDrawCtl1.Open(cdxFile.FullName, false);
                        //Apply chemdraw setting
                        ApplyUserSettingDocument(chkUseSetDoc.Checked, mysettingsdata);
                        //Apply font size and font type
                        CheckFontSize(chkChangeFS.Checked);
                        CheckFontType(chkChangeFT.Checked);

                        //Create output type file 
                        CreateFile(cdxFile.Name, txtType.Text, txtPutFilePath.Text, txtRes.Text, chkDoDBOps.Checked, chkmwandmf.Checked, chkMWs.Checked, chkMFs.Checked, txtMWtbl.Text, txtMFtbl.Text, txtMWMFtbl.Text);
                        lblGifName.Text = cdxFile.Name.Replace(cdxFile.Extension, "") + "." + txtType.Text;
                        System.Windows.Forms.Application.DoEvents();
                    }
                    //Connection close
                    DBManip.ConnClose(chkDoDBOps.Checked);
                }

                else if (cmbProcessTab.SelectedIndex == 2)
                {
                    if (cmbSdfSelect.SelectedIndex == 0)
                    {

                        if (txtInputSDF.Text == "" || txtSdfAccessDB.Text == "" || txtSdfTableName.Text == "" || txtSdfIDField.Text == "")
                        {
                            MessageBox.Show("Can't empty any input filed, please fill the empty field");
                            return;
                        }
                        SDFBase64CDXProcess(txtSdfIDField.Text, txtSdfTableName.Text, chkSdfAppExistTable.Checked, txtInputSDF.Text, txtSdfAccessDB.Text);

                    }
                    else if (cmbSdfSelect.SelectedIndex == 1)
                    {
                        if (txtInputSDF.Text == "" || txtSdfIDField.Text == "")
                        {
                            MessageBox.Show("The Sdf input directory or ID field can't be left blank");
                            return;
                        }

                        SDFMolFilesProcess(txtSdfIDField.Text, txtInputSDF.Text, txtPutFilePath.Text);
                    }
                    else if (cmbSdfSelect.SelectedIndex == 2)
                    {
                        if (txtInputSDF.Text == "")
                        {
                            MessageBox.Show("The Sdf input directory can't be left blank");
                            return;
                        }
                        SDFOutputDirectoryProcess(txtSdfIDField.Text, txtInputSDF.Text, txtPutFilePath.Text);
                    }
                }
                lblProcessing.Text = "DONE";
                lblGifName.Text = CalcProcessDuration(dtStartTime);
                System.Windows.Forms.Application.DoEvents();
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message);
                lblProcessing.Text = ex.Message;
                lblGifName.Text = "";
                System.Windows.Forms.Application.DoEvents();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Control.CheckForIllegalCrossThreadCalls = false;
            cmbSdfSelect.SelectedIndex = 0;
            cmbProcessTab.SelectedIndex = 0;

            txtInputSDF.Enabled = false;
            txtSdfTableName.Enabled = false;
            txtSdfIDField.Enabled = false;
            txtSdfAccessDB.Enabled = false;

            TabsMessage();
            SelectSDFFileProcess();
            SelectProcessTab();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnBrowseSdfFile_Click(object sender, EventArgs e)
        {
            browFile = new OpenFileDialog();
            browFile.AddExtension = true;
            browFile.Filter = "SDF File(*.sdf)|*.sdf|All files(*.*)|*.*";
            if (browFile.ShowDialog(this) == DialogResult.OK)
                txtInputSDF.Text = browFile.FileName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            browFile = new OpenFileDialog();
            browFile.AddExtension = true;
            browFile.Filter = "XML File(*.mdb)|*.mdb|All files(*.*)|*.*";
            if (browFile.ShowDialog(this) == DialogResult.OK)
                txtSdfAccessDB.Text = browFile.FileName;
        }

        private void btnBrowsePut_Click(object sender, EventArgs e)
        {
            openfld = new FolderBrowserDialog();
            if (openfld.ShowDialog() == DialogResult.OK)
                txtPutFilePath.Text = openfld.SelectedPath.ToString() + "\\";
        }

        private void cmbSdfSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectSDFFileProcess();
        }

        private void tabOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabsMessage();
        }

        private void cmbProcessTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectProcessTab();
        }
    }

    public class DBLayer
    {
        OleDbConnection conn;
        OleDbDataAdapter da;
        OleDbCommand cmd = new OleDbCommand();
        DataSet ds = new DataSet();

        public void ExecuteQuery(string Qry)
        {
            try
            {
                //conn = new OleDbConnection(ConnStr);
                da = new OleDbDataAdapter();
                cmd = new System.Data.OleDb.OleDbCommand(Qry, conn);
                conn.Open();
                cmd.CommandType = CommandType.Text;
                da.SelectCommand = cmd;
                cmd.ExecuteNonQuery();
                cmd.Cancel();
                conn.Close();
            }
            catch (SystemException Ex)
            {
                throw Ex;
            }
            finally
            {
                conn.Close();
            }
        }

        public void ConnectToAccess(string InputPath)
        {
            conn = new OleDbConnection();
            conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                @"Data source=" + InputPath;
            try
            {
                conn.Open();
            }
            catch (SystemException Ex)
            {
                conn.Close();
                throw Ex;
            }
            finally
            {
                conn.Close();
            }
        }


        public void ConnOpen(bool chk, string connstr)
        {
            try
            {
                if (chk == true)
                {
                    conn = new OleDbConnection(connstr);
                    conn.Open();
                }
            }
            catch
            { }
        }
        public void ConnClose(bool chk)
        {
            try
            {
                if (chk == true)
                    conn.Close();
            }
            catch
            { }
        }
        public void AddNewRecord(string DataTable, string ID, string Base64_CDX)
        {
            try
            {
                DataRow drTemp = ds.Tables[DataTable].NewRow();
                drTemp["ID"] = ID;
                drTemp["Base64_CDX"] = Base64_CDX;
                ds.Tables[DataTable].Rows.Add(drTemp);
                da.Update(ds, "STRUCT");
                ds.Tables["STRUCT"].Rows.Clear();
            }
            catch (SystemException Ex)
            {
                throw Ex;
            }
        }

        public void FillTable(string Qry, string Datatable)
        {
            try
            {
                da = new OleDbDataAdapter();
                da.SelectCommand = new OleDbCommand(Qry, conn);
                System.Data.OleDb.OleDbCommandBuilder cbDB = new OleDbCommandBuilder(da);
                da.Fill(ds, Datatable);
            }
            catch (SystemException Ex)
            {
                throw Ex;
            }
        }
        public DataTable FillTable(string Qry)
        {
            try
            {
                da = new OleDbDataAdapter(Qry, conn);
                da.SelectCommand = new OleDbCommand(Qry, conn);
                da.Fill(ds, "Temp");
                return ds.Tables["Temp"];
            }
            catch (SystemException ex)
            {
                throw ex;
            }
        }

        public void DBUpdate(string Datatable)
        {
            try
            {
                if (ds.HasChanges() == true)
                {
                    da.Update(ds, Datatable);
                }
            }
            catch (SystemException ex)
            {
                throw ex;
            }
        }
        public void ClearDataSetTables()
        {
            ds.Tables.Clear();
        }
    }
}

