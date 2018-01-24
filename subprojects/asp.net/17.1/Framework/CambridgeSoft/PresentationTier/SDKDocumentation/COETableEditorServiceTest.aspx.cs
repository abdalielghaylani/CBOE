using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COETableEditorService;

public partial class TableEditorServiceTest : System.Web.UI.Page
{
    private ModeOfAction CurrentMode = ModeOfAction.Update;
    private string CurrentTable;

    public enum ModeOfAction
    {
        Add, Update, Delete
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            List<string> listApps = COEConfiguration.GetAppByDatabase(DBMSType.ORACLE);
           
            DataTable AppTable = new DataTable();
            DataColumn Application = new DataColumn("Application");
            AppTable.Columns.Add(Application);

            for (int i = 0; i < listApps.Count; i++)
            {
                DataRow row = AppTable.NewRow();
                row[0] = listApps[i].ToString();
                AppTable.Rows.Add(row);
            }

            cmbAppName.DataSource = AppTable;
            cmbAppName.DataTextField = "Application";
            cmbAppName.DataValueField = "Application";
            cmbAppName.DataBind();
        }
        try
        {
            if (cmbAppName.SelectedCell != null)
            {
                Csla.ApplicationContext.GlobalContext["AppName"] = cmbAppName.SelectedCell.Text;
                if (cmbTables.SelectedCell != null && cmbRecord.SelectedCell != null)
                {
                    this.Create_Form();
                    //       grpBoxTableEditor.Visible = true;
                }
                else
                {
                    //     grpBoxTableEditor.Visible = false;
                }
            }
        }
        catch (Exception ex)
        { 
        }
    }
    protected void cmbAppName_SelectedRowChanged(object sender, Infragistics.WebUI.WebCombo.SelectedRowChangedEventArgs e)
    {
        Csla.ApplicationContext.GlobalContext["AppName"] = cmbAppName.SelectedCell.Text;
        List<string> TablesList = null;

        TablesList = COETableEditorBOList.getTables();

        //for (int i = 0; i < TablesList.Count; i++)
        //{
        //    cmbTables.Items.Add(TablesList[i]);
        //    //    drpTablesList.Columns.Add(TablesList[i]);
        //}

        DataTable TablesListTable = new DataTable();
        DataColumn Table = new DataColumn("Table");
        TablesListTable.Columns.Add(Table);

        for (int i = 0; i < TablesList.Count; i++)
        {
            DataRow row = TablesListTable.NewRow();
            row[0] = TablesList[i].ToString();
            TablesListTable.Rows.Add(row);
        }

        cmbTables.DataSource = TablesListTable;
        cmbTables.DataTextField = "Table";
        cmbTables.DataValueField = "Table";
        cmbTables.DataBind();
        cmbRecord.Rows.Clear();
        cmbRecord.DataSource = null;
        cmbRecord.DataBind();

    }
    protected void PopulateRecords()
    {
        this.cmbRecord.Rows.Clear();

        COETableEditorBOList objList = COETableEditorBOList.NewList();
        objList.TableName = cmbTables.SelectedCell.Text;

        List<ID_Column> records = null;
        records = COETableEditorBOList.GetID_columnList();

        string fieldToShow = COETableEditorBOList.getFieldToShow(cmbTables.SelectedCell.Text);

        lblRecord.Text = fieldToShow;

        DataTable RecordsTable = new DataTable();
        DataColumn record = new DataColumn(fieldToShow);
        RecordsTable.Columns.Add(record);

        for (int i = 0; i < records.Count; i++)
        {
            DataRow row = RecordsTable.NewRow();
            row[0] = records[i].ID.ToString();
            RecordsTable.Rows.Add(row);
        }

        cmbRecord.DataSource = RecordsTable;
        cmbRecord.DataTextField = fieldToShow;
        cmbRecord.DataValueField = fieldToShow;
        cmbRecord.DataBind();

    }

    protected void cmbTables_SelectedRowChanged(object sender, Infragistics.WebUI.WebCombo.SelectedRowChangedEventArgs e)
    {
        PopulateRecords();

    }
    protected void cmbRecord_SelectedRowChanged(object sender, Infragistics.WebUI.WebCombo.SelectedRowChangedEventArgs e)
    {
        Csla.ApplicationContext.GlobalContext["AppName"] = cmbAppName.SelectedCell.Text;

        UltraWebTab1.SelectedTab = 1;
        COETableEditorBO obj = COETableEditorBO.Get(Convert.ToInt32(cmbRecord.SelectedCell.Text));
        CurrentMode = ModeOfAction.Update;
        if (UltraWebTab1.Tabs.GetTab(1).ContentPane.Controls.Count == 1)
        {
            this.Create_Form();
        }

    }

    void Create_Form()
    {
        CurrentTable = cmbTables.SelectedCell.Text;
        COETableEditorBO DataItem = null;
        if(cmbRecord.SelectedCell != null)
        DataItem = COETableEditorBO.Get(Convert.ToInt32(cmbRecord.SelectedCell.Text));

        Infragistics.WebUI.UltraWebTab.Tab tb = UltraWebTab1.Tabs.GetTab(UltraWebTab1.SelectedTab);

        tb.ContentPane.Controls.Clear();

        Table tblContainer = new Table();
        if (DataItem != null)
        {
            //Saving ID field from the Current Record.
            HiddenField IDField = new HiddenField();
            IDField.ID = "IDField";
            IDField.Value = DataItem.ID.ToString();
            tb.ContentPane.Controls.Add(IDField);

            //For each column/Field in side the Current Record.
            foreach (Column pd in DataItem.Columns)
            {
                TableRow tblRowContainer = new TableRow();

                TableCell tblCellContainer1 = new TableCell();
                tblCellContainer1.Style.Add("width", "150px");
                tblCellContainer1.Style.Add("height", "30px");

                TableCell tblCellContainer2 = new TableCell();
                tblCellContainer2.Style.Add("width", "150px");
                tblCellContainer2.Style.Add("height", "30px");

                //TableCell tblCellContainer3 = new TableCell();
                //tblCellContainer2.Style.Add("width", "150px");
                //tblCellContainer2.Style.Add("height", "30px");

                Control Contr = null;
                if (COETableEditorBOList.getLookupField(CurrentTable, pd.FieldName).Length != 0)
                {
                    List<ID_Column> LookupDataSource = COETableEditorBOList.GetLookupFieldList(DataItem.ID, pd.FieldName);
                    Infragistics.WebUI.WebCombo.WebCombo wbc = new Infragistics.WebUI.WebCombo.WebCombo();
                    wbc.DataValueField = "ID";
                    wbc.DataTextField = "PColumn";
                    wbc.DataSource = LookupDataSource;
                    wbc.DataBind();
                    wbc.ID = pd.FieldName;
                    Contr = wbc;
                    wbc.Columns.FromKey("ID").Hidden = true;
                    if (CurrentMode != ModeOfAction.Add)
                    {

                        foreach (Infragistics.WebUI.UltraWebGrid.UltraGridRow row in wbc.Rows)
                        {
                            //Here it is assumed that the Field is ID and hence an integer.
                            if (Convert.ToInt32(row.Cells.FromKey("ID").Value) == Convert.ToInt32(pd.FieldValue))
                            {
                                wbc.SelectedRow = row;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (wbc.Rows.Count != 0)
                            wbc.SelectedIndex = 1;
                    }
                }
                else
                {
                    //Check for the CDX field so that ChemDraw can be used.
                    COETableEditorBOList boList = COETableEditorBOList.NewList();
                    if (boList.IsCDX(pd.FieldName))
                    //if (pd.FieldName.ToUpper().Contains("_CDX"))
                    {
                        CambridgeSoft.COE.Framework.Controls.ChemDraw.COEChemDrawEmbed CoeChemControl = new CambridgeSoft.COE.Framework.Controls.ChemDraw.COEChemDrawEmbed();
                        CoeChemControl.EnableViewState = false;
                        if (CurrentMode != ModeOfAction.Add)
                            CoeChemControl.InlineData = pd.FieldValue == null ? "" : pd.FieldValue.ToString(); ;
                        CoeChemControl.ID = pd.FieldName;
                        Contr = CoeChemControl;
                    }
                    else
                    {
                        //Selecting the Control according to the type of the of the Column/Field
                        if (pd.FieldType == System.Data.DbType.Boolean)
                        {
                            CheckBox chk = new CheckBox();
                            if (CurrentMode != ModeOfAction.Add)
                                chk.Checked = Convert.ToBoolean(pd.FieldValue);
                            chk.ID = pd.FieldName;
                            Contr = chk;
                        }
                        else if (pd.FieldType == System.Data.DbType.DateTime)
                        {
                            Infragistics.WebUI.WebSchedule.WebDateChooser wb = new Infragistics.WebUI.WebSchedule.WebDateChooser();
                            if (CurrentMode != ModeOfAction.Add && pd.FieldValue.ToString().Length != 0)
                                wb.Value = Convert.ToDateTime(pd.FieldValue.ToString());
                            else
                            {
                                wb.Value = System.DateTime.Now;
                                wb.ReadOnly = true;
                            }
                            wb.ID = pd.FieldName;
                            Contr = wb;
                        }
                        else
                        {
                            TextBox tbt = new TextBox();
                            tbt.EnableViewState = false;
                            if (CurrentMode != ModeOfAction.Add)
                            {
                                tbt.Text = pd.FieldValue == null ? "" : pd.FieldValue.ToString();
                            }
                            tbt.ID = pd.FieldName;
                            Contr = tbt;
                        }
                    }
                }
                Label lblp = new Label();
                lblp.ForeColor = System.Drawing.Color.CadetBlue;
                lblp.Width = 150;
                //This lable need to be removed. These are added only as the Test of the Field type.
                //Label lblType = new Label();
                //lblType.Text = pd.FieldName.ToString();
                //lblType.Font.Bold = false;
                //lblType.Font.Size = 10;

                lblp.Text = pd.FieldName;

                //Adding the control set Created for the column/field.

                tblCellContainer1.Controls.Add(lblp);
                tblCellContainer2.Controls.Add(Contr);
                //tblCellContainer3.Controls.Add(lblType);
                tblRowContainer.Cells.Add(tblCellContainer1);
                tblRowContainer.Cells.Add(tblCellContainer2);
                //tblRowContainer.Cells.Add(tblCellContainer3);
                tblContainer.Rows.Add(tblRowContainer);
            }
            //Adding the Container Table on the Content Pane of the Current Tab.
            tb.ContentPane.Controls.Add(tblContainer);
        }
        //Table tblContainer = new Table();

        ////Saving ID field from the Current Record.


        //HiddenField IDField = new HiddenField();
        //IDField.ID = "IDField";
        //if (DataItem != null)
        //{
        //    IDField.Value = DataItem.ID.ToString();
        //}
        //tb.ContentPane.Controls.Add(IDField);

        ////For each column/Field in side the Current Record.
        //foreach (Column pd in DataItem.Columns)
        //{
        //    TableRow tblRowContainer = new TableRow();

        //    TableCell tblCellContainer1 = new TableCell();
        //    tblCellContainer1.Style.Add("width", "150px");
        //    tblCellContainer1.Style.Add("height", "30px");

        //    TableCell tblCellContainer2 = new TableCell();
        //    tblCellContainer2.Style.Add("width", "150px");
        //    tblCellContainer2.Style.Add("height", "30px");

        //    //TableCell tblCellContainer3 = new TableCell();
        //    //tblCellContainer2.Style.Add("width", "150px");
        //    //tblCellContainer2.Style.Add("height", "30px");

        //    Control Contr = null;

        //    //Selecting the Control according to the type of the of the Column/Field
        //    if (pd.FieldType == System.Data.DbType.Boolean)
        //    {
        //        CheckBox chk = new CheckBox();
        //        if (CurrentMode != ModeOfAction.Add)
        //            chk.Checked = Convert.ToBoolean(pd.FieldValue);
        //        Contr = chk;
        //    }
        //    else if (pd.FieldType == System.Data.DbType.DateTime)
        //    {
        //        Infragistics.WebUI.WebSchedule.WebDateChooser wb = new Infragistics.WebUI.WebSchedule.WebDateChooser();
        //        if (CurrentMode != ModeOfAction.Add && pd.FieldValue.ToString().Length != 0)
        //            wb.Value = Convert.ToDateTime(pd.FieldValue.ToString());
        //        else
        //        {
        //            wb.Value = System.DateTime.Now;
        //            wb.ReadOnly = true;
        //        }
        //        Contr = wb;
        //    }
        //    else
        //    {
        //        TextBox tbt = new TextBox();
        //        tbt.EnableViewState = false;
        //        if (CurrentMode != ModeOfAction.Add)
        //        {
        //            tbt.Text = pd.FieldValue == null ? "" : pd.FieldValue.ToString();
        //        }
        //        Contr = tbt;
        //    }

        //    Label lblp = new Label();
        //    lblp.ForeColor = System.Drawing.Color.CadetBlue;
        //    lblp.Width = 150;
        //    //This lable need to be removed. These are added only as the Test of the Field type.
        //    Label lblType = new Label();
        //    lblType.Text = pd.FieldName.ToString();
        //    lblType.Font.Bold = false;
        //    lblType.Font.Size = 10;

        //    Contr.ID = pd.FieldName;
        //    lblp.Text = pd.FieldName;

        //    //Adding the control set Created for the column/field.

        //    tblCellContainer1.Controls.Add(lblp);
        //    tblCellContainer2.Controls.Add(Contr);
        //    tblCellContainer3.Controls.Add(lblType);
        //    tblRowContainer.Cells.Add(tblCellContainer1);
        //    tblRowContainer.Cells.Add(tblCellContainer2);
        //    //tblRowContainer.Cells.Add(tblCellContainer3);
        //    tblContainer.Rows.Add(tblRowContainer);
        //}
        ////Adding the Container Table on the Content Pane of the Current Tab.
        //tb.ContentPane.Controls.Add(tblContainer);

        switch (UltraWebTab1.SelectedTab)
        {
            case 0:
                btnPerformAction.Text = "Add";
                break;

            case 1:
                btnPerformAction.Text = "Update";
                break;

            case 2:
                btnPerformAction.Text = "Delete";
                break;

        }
    }

    protected void UltraWebTab1_TabClick(object sender, Infragistics.WebUI.UltraWebTab.WebTabEvent e)
    {
        if (cmbRecord.SelectedCell != null)
        {

            COETableEditorBO obj = COETableEditorBO.Get(Convert.ToInt32(cmbRecord.SelectedCell.Text));
        }
        CurrentMode = (ModeOfAction)UltraWebTab1.SelectedTab;
        this.Create_Form();
        
        switch (UltraWebTab1.SelectedTab)
        {
            case 0:
                btnPerformAction.Text = "Add";
                break;

            case 1:
                btnPerformAction.Text = "Update";

                break;

            case 2:
                btnPerformAction.Text = "Delete";
                break;

        }

    }

    protected void Process(COETableEditorBO obj)
    {
        CurrentTable = cmbTables.SelectedCell.Text;

        Infragistics.WebUI.UltraWebTab.Tab tb = UltraWebTab1.Tabs.GetTab(UltraWebTab1.SelectedTab);
        List<Column> colList = obj.Columns;

        foreach (Column col in colList)
        {
            if (COETableEditorBOList.getLookupField(CurrentTable, col.FieldName).Length == 0)
            {
                COETableEditorBOList boList = COETableEditorBOList.NewList();
                if (boList.IsCDX(col.FieldName))
                //if(col.FieldName.ToUpper().Contains("_CDX"))
                {
                    string Base64_CDXData = ((CambridgeSoft.COE.Framework.Controls.ChemDraw.COEChemDrawEmbed)tb.FindControl(col.FieldName)).OutputData;
                    col.FieldValue = Base64_CDXData;
                }
                else
                {
                    switch (col.FieldType)
                    {
                        case DbType.Int16:
                            col.FieldValue = Convert.ToInt16(((TextBox)tb.FindControl(col.FieldName)).Text);
                            break;

                        case DbType.AnsiString:
                            col.FieldValue = ((TextBox)tb.FindControl(col.FieldName)).Text;
                            break;

                        case DbType.Boolean:
                            col.FieldValue = ((CheckBox)tb.FindControl(col.FieldName)).Checked;
                            break;

                        case DbType.DateTime:
                            col.FieldValue = ((Infragistics.WebUI.WebSchedule.WebDateChooser)tb.FindControl(col.FieldName)).Value;
                            break;
                    }
                }
            }
            else
            {
                col.FieldValue = Convert.ToInt16(((Infragistics.WebUI.WebCombo.WebCombo)tb.FindControl(col.FieldName)).DataValue);
            }
        }

        obj.Columns = colList;
        obj.Save();
    }


    protected void btnPerformAction_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
    {
        COETableEditorBO tblObj = null;

        switch (UltraWebTab1.SelectedTab)
        {
            case 0:
                tblObj = COETableEditorBO.New();
                Process(tblObj);
                this.PopulateRecords(); 
                break;

            case 1:
                if (cmbRecord.SelectedCell != null)
                {
                    tblObj = COETableEditorBO.Get(Convert.ToInt32(cmbRecord.SelectedCell.Text));
                    Process(tblObj);
                }
                break;

            case 2:
                if (cmbRecord.SelectedCell != null)
                {
                    COETableEditorBO.Delete(Convert.ToInt32(cmbRecord.SelectedCell.Text));
                    this.PopulateRecords();
                }
                break;

        }
    }
}