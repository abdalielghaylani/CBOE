using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace ChemBioVizExcelAddIn
{
    public partial class frmSheetProperties : Form
    {
        Nullable<bool> sslReadOnlyCheck = null ;       
        public frmSheetProperties()
        {
            InitializeComponent();
            InitializeEvents();
        }
        public void InitializeEvents()
        {
            this.Load +=new EventHandler(frmSheetProperties_Load);
            this.cbxSSL.CheckedChanged+=new EventHandler(cbxSSL_CheckedChanged);
        }

        public void DeAttachEvents()
        {   
            this.cbxSSL.CheckedChanged -= new EventHandler(cbxSSL_CheckedChanged);
        }
        private void frmSheetProperties_Load(object sender, EventArgs e)
        {
            try
            {
                DisplaySheetProperties();             
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
        private void cbxSSL_CheckedChanged(object sender, EventArgs e)
        {
            this.cbxSSL.Checked = (bool)sslReadOnlyCheck;
        }

        private void DisplaySheetProperties()
        {
            try
            {
                CBVExcel CBVExcel = new CBVExcel();
                Excel::Worksheet nHideenSheet = GlobalCBVExcel.Get_CSHidden();
                Excel::Worksheet currentSheet = Global._ExcelApp.ActiveSheet as Excel::Worksheet;
                //Coverity fix - CID 18752
                if (currentSheet == null)
                    throw new System.NullReferenceException();

                string serializeData = CBVExcel.GetDataFromHiddenSheet(currentSheet.Name, (int)Global.CBVHiddenSheetHeader.SerializeSheetProperties);             

                if (string.IsNullOrEmpty(serializeData))
                    return;

                string modifiedUser = CBVExcel.GetDataFromHiddenSheet(Global.UID, (int)Global.CBVHiddenSheetHeader.ModifiedUser);

                
                CBVSheetProperties sheetProperties = new CBVSheetProperties();
                sheetProperties = (CBVSheetProperties)sheetProperties.GetDeSeralizeSheetProperties(serializeData);
          
                lblDispSheetName.Text = Global.CurrentWorkSheetName;
                
                lblConnmode.Text = sheetProperties.CBVServermode;

                //Make SSL box invisible n 2-Tier mode
                if(sheetProperties.CBVServermode.Equals(StringEnum.GetStringValue(Global.MRUListConstant.MRU_2T),StringComparison.OrdinalIgnoreCase))
                    cbxSSL.Visible=false;

                lblServername.Text = (CBVUtil.StartsWith(sheetProperties.CBVServername, StringEnum.GetStringValue(Global.MRUListConstant.SSL)) || CBVUtil.StartsWith(sheetProperties.CBVServername, StringEnum.GetStringValue(Global.MRUListConstant.MRU_2T))) ? CBVUtil.AfterDelimiter(sheetProperties.CBVServername, '/') : sheetProperties.CBVServername;

                sslReadOnlyCheck = CBVUtil.StartsWith(sheetProperties.CBVServername, StringEnum.GetStringValue(Global.MRUListConstant.SSL)) ? true : false;
                cbxSSL.Checked = (bool)sslReadOnlyCheck;
              

                lblDispSheetCreatedBy.Text = sheetProperties.CBVSheetCreatedBy;
                lblDispCreatedOn.Text = sheetProperties.CBVSheetCreatedOn.ToString();
                lblDispDataview.Text = sheetProperties.CBVDataviewname;
               
                lblDispLogPath.Text = AppConfigSetting.ReadSetting(StringEnum.GetStringValue(Global.ConfigurationKey.SQL_LOGGING_FILE));
                                
                if (!Global.WorkSheetChange)
                    lblDispModifiedBy.Text = modifiedUser;
                else
                {
                    lblDispModifiedBy.Text = Global.CurrentUser;
                    //11.0.3
                    CBVExcel.UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.ModifiedUser, Global.CurrentWorkSheetName, Global.CurrentUser);
                }               
             
                lblLoginUser.Text = Global.CurrentUser;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }  
    }


    #region _  ChemBioViz Sheet Properties base class  _

    [Serializable()]
    abstract class SheetProperties
    {
        protected int cbvUID = 0;
        protected string cbvSheetname = null;
        protected string cbvSheetCreatedBy = null;
        protected string cbvSheetModifiedBy = null;
        protected DateTime cbvSheetCreatedOn;
        protected string cbvDataviewname = null;
        protected string cbvServername = null;
        protected string cbvServermode = null;

        public abstract int CBVUID { get;set; }
        public abstract string CBVSheetname { get;set; }
        public abstract string CBVSheetCreatedBy { get;set;}
        public abstract string CBVSheetModifiedBy { get;set;}
        public abstract DateTime CBVSheetCreatedOn { get;set;}
        public abstract string CBVDataviewname { get;set;}
        public abstract string CBVServername { get;set;}
        public abstract string CBVServermode { get;set;}

        public abstract string SetSeralizeSheetProperties(int uid, string dvName, string createByUser, string modifiedByUser, string servername, string servermode, DateTime currentDT);

        public abstract object GetDeSeralizeSheetProperties(string serializeData);

    }
    #endregion _  ChemBioViz Sheet Properties base class  _

    #region _  ChemBioViz Sheet Properties class  _
    [Serializable()]
    class CBVSheetProperties : SheetProperties, IDisposable
    {

        #region .ctor and Dispose methods
        private bool _Disposed;

        public CBVSheetProperties() { }
        public virtual void Dispose()
        {
            if (!_Disposed)
            {

                GC.SuppressFinalize(this);
                _Disposed = true;
            }
        }
        #endregion

        public override int CBVUID
        {
            get { return cbvUID; }
            set { cbvUID = value; }
        }

        public override string CBVSheetname
        {
            get { return cbvSheetname; }
            set { cbvSheetname = value; }
        }

        public override string CBVSheetCreatedBy
        {
            get { return cbvSheetCreatedBy; }
            set { cbvSheetCreatedBy = value; }
        }

        public override string CBVSheetModifiedBy
        {
            get { return cbvSheetModifiedBy; }
            set { cbvSheetModifiedBy = value; }
        }

        public override DateTime CBVSheetCreatedOn
        {
            get { return cbvSheetCreatedOn; }
            set { cbvSheetCreatedOn = value; }
        }

        public override string CBVDataviewname
        {
            get { return cbvDataviewname; }
            set { cbvDataviewname = value; }
        }
        public override string CBVServername
        {
            get { return cbvServername; }
            set { cbvServername = value; }
        }
        public override string CBVServermode
        {
            get { return cbvServermode; }
            set { cbvServermode = value; }
        }

        public override string SetSeralizeSheetProperties(int uid, string dvName, string createByUser, string modifiedByUser, string servername, string servemode, DateTime currentDT)
        {
            try
            {
                this.cbvUID = uid;              
                this.CBVDataviewname = dvName;
                this.CBVSheetCreatedBy = createByUser;
                this.CBVSheetModifiedBy = modifiedByUser;
                this.CBVSheetCreatedOn = currentDT;
                this.CBVServername = servername;
                this.CBVServermode = servemode;
                return SerializeDeserialize.Serialize(this);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override object GetDeSeralizeSheetProperties(string serializeData)
        {
            try
            {
                return SerializeDeserialize.Deserialize(serializeData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    #endregion _  ChemBioViz Sheet Properties class  _
}