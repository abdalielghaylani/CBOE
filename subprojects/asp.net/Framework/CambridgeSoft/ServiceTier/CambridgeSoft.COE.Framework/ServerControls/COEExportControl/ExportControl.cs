#region Namespace
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml;
using CambridgeSoft.COE.Framework.Common;
using System.Data;
using System.IO;
using CambridgeSoft.COE.Framework.COEExportService;
using System.Collections;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Reflection;
#endregion

namespace CambridgeSoft.COE.Framework.Controls
{
    #region Export Control
    [DefaultProperty("CurrentPage")]
    [ToolboxData("<{0}:ExportControl runat=server></{0}:ExportControl>")]
    [Description("Displays a Export control.")]
    public class ExportControl : CompositeControl
    {
        #region Variables
        private ImageButton _export;
        private ImageButton _cancel;
        private Label _currentTemp;
        private Label _curTemplate = new Label();
        private ImageButton _edit;
        private ImageButton _delete;
        private Panel panEditTemp;
        private Panel panDeleteTemp;
        private ImageButton _templateList;
        private ImageButton _createTemplate;
        private Label _expFormat;
        private Label _exportFormat = new Label();
        private ImageButton _exportFormatList;
        private GridView _grdList = new GridView();
        private GridView _grdFields = new GridView();
        private Xml _resCriteria;
        private ImageButton _expand;
        private ImageButton _collapse;
        private static XmlDocument m_doc = new XmlDocument();
        private Label _tempName;
        private TextBox _templateName = new TextBox();
        private CheckBox _ispublic = new CheckBox();
        private Label _temDesc;
        private TextBox _templateDescription = new TextBox();
        private Panel pantest = new Panel();
        private Panel pantest2 = new Panel();
        private ImageButton _createNewTempOk = new ImageButton();
        private LinkButton _createNewTempSave = new LinkButton();
        private ImageButton _createNewTempCancel;
        private Panel _pancreatetemp = new Panel();
        private HiddenField _hdnTemplateid = new HiddenField();
        private HiddenField _hdnFormatid = new HiddenField();
        private COEDataView _pageDataview = new COEDataView();
        private ResultsCriteria _pageResultsCriteria = new ResultsCriteria();
        LinkButton headpoplink = new LinkButton();
        LinkButton lnkFormatList = new LinkButton();
        List<int> hiddenCounts = new List<int>();
        private static string _currentTemplate = "Default Template"; 
        private static string exportFlag = "export";
        int count = 0;

        //for export
        private bool _exportedHits = false;
        private COEFormGroup _formGroup;

        COEExportTemplateBO ExporttempBo = new COEExportTemplateBO();
        COEExportTemplateBOList templist = null;

        [NonSerialized]
        private XmlNamespaceManager manager;

        public delegate void ExportControlEventHandler(object sender, ExportControlEventArgs eventArgs);
        public event ExportControlEventHandler ExportList;

        #endregion


        #region Properties
        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        #region Image Path

        public string ExportButtonImageURL
        {
            get
            {
                if (Page.Session["ExportButtonImageURL"] != null)
                    return (string)Page.Session["ExportButtonImageURL"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.export_btn.png");
            }
            set
            {
                Page.Session["ExportButtonImageURL"] = value;
            }
        }

        public string CancelButtonImageURL
        {
            get
            {
                if (Page.Session["CancelButtonImageURL"] != null)
                    return (string)Page.Session["CancelButtonImageURL"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.cancel_btn.png");
            }
            set
            {
                Page.Session["CancelButtonImageURL"] = value;
            }
        }

        public string EditImageURL
        {
            get
            {
                if (Page.Session["EditImageURL"] != null)
                    return (string)Page.Session["EditImageURL"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.File_Edit.png");
            }
            set
            {
                Page.Session["EditImageURL"] = value;
            }
        }

        public string AddImageURL
        {
            get
            {
                if (Page.Session["AddImageURL"] != null)
                    return (string)Page.Session["AddImageURL"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.Add_h.png");
            }
            set
            {
                Page.Session["AddImageURL"] = value;
            }
        }

        public string FolderImageUrl
        {
            get
            {
                if (Page.Session["FolderImageUrl"] != null)
                    return (string)Page.Session["FolderImageUrl"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.Folder.png");
            }
            set
            {
                Page.Session["FolderImageUrl"] = value;
            }
        }

        public string RemoveImageURL
        {
            get
            {
                if (Page.Session["RemoveImageURL"] != null)
                    return (string)Page.Session["RemoveImageURL"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.Remove_h.png");
            }
            set
            {
                Page.Session["RemoveImageURL"] = value;
            }
        }

        public string FileAddImageUrl
        {
            get
            {
                if (Page.Session["FileAddImageUrl"] != null)
                    return (string)Page.Session["FileAddImageUrl"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.File_Add.png");
            }
            set
            {
                Page.Session["FileAddImageUrl"] = value;
            }
        }

        //-New Images
        public string Add_DocumentImageUrl
        {
            get
            {
                if (Page.Session["Add_Document"] != null)
                    return (string)Page.Session["Add_Document"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.Add_Document.png");
            }
            set
            {
                Page.Session["Add_Document"] = value;
            }
        }
        public string Arrow_Down_B
        {
            get
            {
                if (Page.Session["Arrow_Down_B"] != null)
                    return (string)Page.Session["Arrow_Down_B"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.Arrow_Down_B.png");
            }
            set
            {
                Page.Session["Arrow_Down_B"] = value;
            }
        }
        public string Arrow_Up_B
        {
            get
            {
                if (Page.Session["Arrow_Up_B"] != null)
                    return (string)Page.Session["Arrow_Up_B"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.Arrow_Up_B.png");
            }
            set
            {
                Page.Session["Arrow_Up_B"] = value;
            }
        }
        public string BlockRed
        {
            get
            {
                if (Page.Session["BlockRed"] != null)
                    return (string)Page.Session["BlockRed"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.Block.png");
            }
            set
            {
                Page.Session["BlockRed"] = value;
            }
        }
        public string CheckRight
        {
            get
            {
                if (Page.Session["CheckRight"] != null)
                    return (string)Page.Session["CheckRight"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.Check.png");
            }
            set
            {
                Page.Session["CheckRight"] = value;
            }
        }
        public string Close_Window
        {
            get
            {
                if (Page.Session["Close_Window"] != null)
                    return (string)Page.Session["Close_Window"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.Close_Window.png");
            }
            set
            {
                Page.Session["Close_Window"] = value;
            }
        }
        public string ExportDetails
        {
            get
            {
                if (Page.Session["ExportDetails"] != null)
                    return (string)Page.Session["ExportDetails"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.Details.png");
            }
            set
            {
                Page.Session["ExportDetails"] = value;
            }
        }
        public string ExportEdit
        {
            get
            {
                if (Page.Session["ExportEdit"] != null)
                    return (string)Page.Session["ExportEdit"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.Edit.png");
            }
            set
            {
                Page.Session["ExportEdit"] = value;
            }
        }
        public string ExportDelete
        {
            get
            {
                if (Page.Session["ExportDelete"] != null)
                    return (string)Page.Session["ExportDelete"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.Stop.png");
            }
            set
            {
                Page.Session["ExportDelete"] = value;
            }
        }
        public string ExportArrow
        {
            get
            {
                if (Page.Session["ExportArrow"] != null)
                    return (string)Page.Session["ExportArrow"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.Export.png");
            }
            set
            {
                Page.Session["ExportArrow"] = value;
            }
        }
        public string SaveExport
        {
            get
            {
                if (Page.Session["SaveExport"] != null)
                    return (string)Page.Session["SaveExport"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.Save.png");
            }
            set
            {
                Page.Session["SaveExport"] = value;
            }
        }
        public string StopExport
        {
            get
            {
                if (Page.Session["StopExport"] != null)
                    return (string)Page.Session["StopExport"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.Stop.png");
            }
            set
            {
                Page.Session["StopExport"] = value;
            }
        }

        #endregion

        #region Export/Cancel Buttons

        public ImageButton Export
        {
            get
            {
                _export = new ImageButton();
                _export.ID = "ExportImageButton";
                _export.ImageUrl = ExportButtonImageURL;
                _export.ToolTip = "Export";
                _export.Click += new ImageClickEventHandler(_export_Click);
                return _export;
            }
        }

        void _export_Click(object sender, ImageClickEventArgs e)
        {
            if (exportFlag == "export")
            {
                ResultsCriteria res = new ResultsCriteria();
                ResultsCriteria resMerge = new ResultsCriteria();

                resMerge = MergeExport();
                int dataviewId = PageDataview.DataViewID;
                string exportformat = ExportFormatSelected.Text.ToString();

                ExportControlEventArgs args = new ExportControlEventArgs(dataviewId, resMerge, exportformat);

                ExportList(sender, args);
            }
            else
            {
                string message = "Save or Update the Template before Export";
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("<script type = 'text/javascript'>");
                sb.Append("window.onload=function(){");
                sb.Append("alert('");
                sb.Append(message);
                sb.Append("')};");
                sb.Append("</script>");
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", sb.ToString());
            }
        }

        public ImageButton Cancel
        {
            get
            {
                _cancel = new ImageButton();
                _cancel.ID = "ExportCancelButton";
                _cancel.ToolTip = "Cancel";
                _cancel.ImageUrl = CancelButtonImageURL;
                _cancel.OnClientClick = "return flipVisibility(document.getElementById('" + this.ClientID + "'), '" + this.ID + "');";

                return _cancel;
            }
        }

        #endregion

        #region Current Template

        public Label CurrentTemp
        {
            get
            {
                _currentTemp = new Label();
                _currentTemp.ID = "CurrentTempLabel";
                _currentTemp.Text = "Current Template:";
                _currentTemp.Font.Bold = true;
                return _currentTemp;
            }
        }

        public Label CurrentSelectedTemplate
        {
            get
            {
                _curTemplate.ID = "CurrentSelectedTemplated";
                _curTemplate.Text = "Default Template";

                return _curTemplate;
            }
        }

        public ImageButton Edit
        {
            get
            {
                _edit = new ImageButton();
                _edit.ID = "EditImgButton";
                _edit.ImageUrl = ExportEdit;
                _edit.Height = 15;
                _edit.Width = 15;
                _edit.ToolTip = "Edit Template";
                _edit.Click += new ImageClickEventHandler(_edit_Click);

                return _edit;
            }
        }

        public ImageButton Delete
        {
            get
            {
                _delete = new ImageButton();
                _delete.ID = "DeleteImgButton";
                _delete.ImageUrl = ExportDelete;
                _delete.Height = 15;
                _delete.Width = 15;
                _delete.ToolTip = "Delete Export Template";
                _delete.Click += new ImageClickEventHandler(_delete_Click);
                string DelConfirmationScript = "return confirm('Are you sure want to delete this template?');";
                _delete.Attributes.Add("onclick", DelConfirmationScript);
                return _delete;
            }
        }

        public static string currentTemplate
        {
            get
            {
                return _currentTemplate;
            }
            set
            {
                _currentTemplate = value;
            }
        }

        void _edit_Click(object sender, ImageClickEventArgs e)
        {
            EditClickEvent();
        }
        void _delete_Click(object sender, ImageClickEventArgs e)
        {
            DeleteClickEvent();
        }
        private void EditClickEvent()
        {
            exportFlag = "save";
            CreateNewTempSave.Text = "&nbsp;Update";

            if (Page.Session["ExportTemplateId"] != null)
            {

                ExporttempBo = COEExportTemplateBO.Get(Convert.ToInt32(Page.Session["ExportTemplateId"]));

                Page.Session["ExporttempBo"] = ExporttempBo;

                TemplateName.Text = ExporttempBo.Name;
                TemplateDescription.Text = ExporttempBo.Description;

                IsPublic.Checked = ExporttempBo.IsPublic;

                PanCreateTemp.Visible = true;
                Page.Session["editclick"] = "true";
                BindGrid(true, GridViewList, 0, null);

            }
        }
        private void DeleteClickEvent()
        {
            if (Page.Session["ExportTemplateId"] != null)
            {
                int currentTemplateId = Convert.ToInt32(Page.Session["ExportTemplateId"]);
                COEExportTemplateBO.Delete(currentTemplateId);
                CurrentSelectedTemplate.Text = "Default Template";
                currentTemplate = "Default Template";
                panEditTemp.Enabled = false;
                panDeleteTemp.Enabled = false;
                LinkButton popLink = (LinkButton)pantest.FindControl("Template - " + currentTemplateId);
                pantest.Controls.Remove(popLink.Parent);
                PanCreateTemp.Visible = false;
                Page.Session["editclick"] = null;
                BindGrid(false, GridViewList, 0, null);
            }
        }

        public Panel AddPopupTemplateList()
        {
            List<COEExportTemplateBO> lstCOEExportTemplateBO = null;
            pantest = new Panel();
            pantest.ID = "AddpopTempList";
            pantest.Width = 130;
            pantest.Style.Add("position", "absolute");
            pantest.Style.Add("top", "85px");
            pantest.Style.Add("left", "195px");
            pantest.Style.Add("z-index", "1");
            pantest.Style.Add("display", "none");
            pantest.BackColor = System.Drawing.Color.White;
            pantest.BorderColor = System.Drawing.Color.Navy;
            pantest.BorderWidth = 1;
            pantest.ForeColor = System.Drawing.Color.Navy;
            pantest.ScrollBars = ScrollBars.Vertical;

            if (templist == null)
            {
                templist = COEExportTemplateBOList.GetUserTemplatesByDataViewId(PageDataview.DataViewID, COEUser.Name, true);
                ExporttempBo.ID = 0;
                ExporttempBo.Name = "Default Template";
                ExporttempBo.Description = "Default Template";
                templist.Add(ExporttempBo);
            }

            lstCOEExportTemplateBO = new List<COEExportTemplateBO>(templist);
            lstCOEExportTemplateBO.Reverse();

            LinkButton poplink;
            if (lstCOEExportTemplateBO.Count > 8)
                pantest.Height = 200;

            if (lstCOEExportTemplateBO.Count != 0)
            {
                for (int i = 0; i < lstCOEExportTemplateBO.Count; i++)
                {
                    poplink = new LinkButton();
                    poplink.ID = "Template - " + lstCOEExportTemplateBO[i].ID.ToString();
                    poplink.Text = lstCOEExportTemplateBO[i].Name;
                    poplink.ToolTip = "Template - " + lstCOEExportTemplateBO[i].Name;
                    poplink.Click += new EventHandler(poplink_Click);
                    poplink.OnClientClick = "javascript:var pop = document.getElementById(this.id.replace('" + poplink.ClientID + "','" + HdnTemplateId.ClientID + "'));pop.value= '" + lstCOEExportTemplateBO[i].ID.ToString() + "';";
                    poplink.Style.Add("text-decoration", "none");
                    poplink.ForeColor = System.Drawing.Color.Navy;
                    poplink.Attributes.Add("OnMouseOver", "javascript:this.style.backgroundColor = '#cedae6';this.style.textDecoration ='';");
                    poplink.Attributes.Add("OnMouseOut", "javascript:this.style.backgroundColor = '';this.style.textDecoration ='none';");
                    System.Web.UI.HtmlControls.HtmlGenericControl dynDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("DIV");
                    dynDiv.Controls.Add(poplink);
                    pantest.Controls.Add(dynDiv);
                }
            }
            return pantest;
        }

        void poplink_Click(object sender, EventArgs e)
        {
            Page.Session["ExportTemplateId"] = HdnTemplateId.Value;
            COEExportTemplateBO templateBO = new COEExportTemplateBO();
            templateBO = COEExportTemplateBO.Get(Convert.ToInt32(HdnTemplateId.Value));
            CurrentSelectedTemplate.Text = templateBO.Name;
            currentTemplate = (templateBO.Name != string.Empty) ? templateBO.Name : "Default Template";
            panEditTemp.Enabled = true;
            panDeleteTemp.Enabled = true;
            if (Page.Session["editclick"] == "true")
            {
                Page.Session["ExporttempBo"] = templateBO;
                TemplateName.Text = templateBO.Name;
                TemplateDescription.Text = templateBO.Description;
                IsPublic.Checked = templateBO.IsPublic;
            }
            if (templateBO.Name == string.Empty)
            {
                _curTemplate.ID = "CurrentSelectedTemplated";
                _curTemplate.Text = "Default Template";
                panEditTemp.Enabled = false;
                panDeleteTemp.Enabled = false;
            }
            BindGrid(true, GridViewList, 0, null);

        }

        public HiddenField HdnTemplateId
        {
            get
            {
                _hdnTemplateid.ID = "hdntemplateid";

                return _hdnTemplateid;
            }
            set
            {
                _hdnTemplateid = value;
            }
        }

        public HiddenField HdnFormatId
        {
            get
            {
                _hdnFormatid.ID = "hdnformatid";

                return _hdnFormatid;
            }
            set
            {
                _hdnFormatid = value;
            }
        }

        public ImageButton TemplateList
        {
            get
            {
                _templateList = new ImageButton();
                _templateList.ID = "TemplateListImgButton";
                _templateList.ImageUrl = ExportDetails;
                _templateList.Height = 15;
                _templateList.Width = 15;
                _templateList.ToolTip = "Template List";
                _templateList.OnClientClick = "javascript:var popup = document.getElementById(this.id.replace('TemplateListImgButton','" + AddPopup().ClientID + "'));var popTemp = document.getElementById(this.id.replace('TemplateListImgButton','" + AddPopupTemplateList().ClientID + "')); if(popTemp.style.display == ''){popTemp.style.display = 'none'}else{popTemp.style.display = ''};popup.style.display = 'none';return false;";

                return _templateList;
            }
        }

        public ImageButton CreateNewTemplate
        {
            get
            {
                _createTemplate = new ImageButton();
                _createTemplate.ID = "CreateNewTemplate";
                _createTemplate.ImageUrl = Add_DocumentImageUrl;
                _createTemplate.Height = 15;
                _createTemplate.Width = 15;
                _createTemplate.ToolTip = "Create New Template";
                _createTemplate.Click += new ImageClickEventHandler(_createTemplate_Click);

                return _createTemplate;
            }
        }

        void _createTemplate_Click(object sender, ImageClickEventArgs e)
        {
            AddClickEvent();
        }
        private void AddClickEvent()
        {
            CreateNewTempSave.Text = "&nbsp;Save";
            TemplateName.Text = "";
            TemplateDescription.Text = "";
            PanCreateTemp.Visible = true;
            Page.Session["editclick"] = "true";
            CurrentSelectedTemplate.Text = "Default Template";
            currentTemplate = "Default Template";
            panEditTemp.Enabled = false;
            panDeleteTemp.Enabled = false;
            Page.Session["ExportTemplateId"] = null;
            BindGrid(true, GridViewList, 0, null);
            exportFlag = "save";
        }

        #endregion

        #region Format Template
        public Label ExportFormatLabel
        {
            get
            {
                _expFormat = new Label();
                _expFormat.ID = "ExportFormatLabel";
                _expFormat.Text = "Export Format :";
                _expFormat.Font.Bold = true;

                return _expFormat;
            }
        }

        public Label ExportFormatSelected
        {
            get
            {
                _exportFormat.ID = "ExportFormatSelected";

                if (HdnFormatId.Value != null && HdnFormatId.Value != "")
                    _exportFormat.Text = HdnFormatId.Value;
                else
                    _exportFormat.Text = "SDF Flat";

                return _exportFormat;
            }
        }

        public Panel AddPopup()
        {
            pantest2 = new Panel();
            pantest2.ID = "Addpoppan";
            pantest2.Width = 190;
            pantest2.Style.Add("position", "absolute");
            pantest2.Style.Add("top", "52px");
            pantest2.Style.Add("left", "370px");

            pantest2.Style.Add("z-index", "1");
            pantest2.Style.Add("display", "none");
            pantest2.BackColor = System.Drawing.Color.White;
            pantest2.BorderColor = System.Drawing.Color.Navy;
            pantest2.BorderWidth = 1;
            pantest2.ForeColor = System.Drawing.Color.Navy;
            pantest2.ScrollBars = ScrollBars.Vertical;

            //Head Panel
            for (int j = 0; j < 1; j++)
            {
                Panel headpan = new Panel();
                headpan.ID = "AddpoppanHeadpan" + j;
                headpan.Font.Bold = true;
                headpan.Style.Add("margin-left", "20px");
                Label lbl = new Label();
                headpoplink = new LinkButton();
                if (j == 0)
                {
                    lbl.ID = "lblSDF" + j;
                    lbl.Text = "SDF";
                    headpoplink.ID = "lnkSDF" + j;
                    headpoplink.Text = "+";
                }

                headpoplink.OnClientClick = "javascript:var headpan = document.getElementById(this.id.replace('" + headpoplink.ClientID + "','" + headpan.ClientID + "'));if(headpan != ''){if(headpan.style.display == ''){headpan.style.display ='none';this.value='-';}else{headpan.style.display ='';this.value='+';}};return false;";

                LinkButton poplink;
                for (int i = 0; i < 2; i++)
                {

                    poplink = new LinkButton();
                    poplink.ID = "lnkExpFormatlist" + j + i;
                    if (j == 0)
                    {
                        if (i == 0)
                            poplink.Text = "SDF Nested";
                        else if (i == 1)
                            poplink.Text = "SDF Flat";
                    }

                    poplink.ToolTip = poplink.Text.ToString() + " Format";
                    poplink.Click += new EventHandler(poplink_Click2);
                    poplink.OnClientClick = "javascript:var pop = document.getElementById(this.id.replace('" + poplink.ClientID + "','hdnformatid'));pop.value= '" + poplink.Text.ToString() + "';";
                    poplink.Style.Add("text-decoration", "none");
                    poplink.ForeColor = System.Drawing.Color.Navy;
                    poplink.Attributes.Add("OnMouseOver", "javascript:this.style.backgroundColor = '#cedae6';this.style.textDecoration ='';");
                    poplink.Attributes.Add("OnMouseOut", "javascript:this.style.backgroundColor = '';this.style.textDecoration ='none';");

                    headpan.Controls.Add(poplink);
                    headpan.Controls.Add(new LiteralControl("<BR />"));
                }

                pantest2.Controls.Add(headpoplink);
                pantest2.Controls.Add(lbl);
                pantest2.Controls.Add(headpan);
                pantest2.Controls.Add(new LiteralControl("<BR />"));
            }

            return pantest2;
        }

        void poplink_Click2(object sender, EventArgs e)
        {
            ExportFormatSelected.Text = HdnFormatId.Value;
            ExporttempBo = COEExportTemplateBO.Get(Convert.ToInt32(Page.Session["ExportTemplateId"]));
        }
        #endregion

        #region Create New Template

        public Panel PanCreateTemp
        {
            get
            {
                _pancreatetemp.ID = "pancreate";
                _pancreatetemp.Width = new Unit("100%");
                _pancreatetemp.BackColor = System.Drawing.Color.FromArgb(225, 225, 225);
                _pancreatetemp.Visible = false;
                return _pancreatetemp;
            }
            set
            {
                _pancreatetemp = value;
            }
        }

        public ImageButton ExportFormatList
        {
            get
            {
                _exportFormatList = new ImageButton();
                _exportFormatList.ID = "ExportFormatListImageButton";
                _exportFormatList.ImageUrl = ExportDetails;
                _exportFormatList.Height = 15;
                _exportFormatList.Width = 15;
                _exportFormatList.ToolTip = " Export Format List ";
                AddPopup();
                _exportFormatList.OnClientClick = "javascript:var popup = document.getElementById(this.id.replace('ExportFormatListImageButton','" + AddPopup().ClientID + "'));var popTemp = document.getElementById(this.id.replace('ExportFormatListImageButton','" + AddPopupTemplateList().ClientID + "')); if(popup.style.display == ''){popup.style.display = 'none'}else{popup.style.display = ''};popTemp.style.display = 'none';return false;";

                return _exportFormatList;
            }
        }

        public GridView GridViewList
        {
            get
            {
                _grdList.Visible = true;
                _grdList.ID = "GridviewList";
                _grdList.AutoGenerateColumns = false;
                _grdList.EmptyDataText = "No Data is Rendered. Please check the Dataset";
                _grdList.BackColor = System.Drawing.Color.LightGray;
                _grdList.BorderColor = System.Drawing.Color.Navy;
                _grdList.Width = 500;

                return _grdList;
            }
            set
            {
                _grdList = value;
            }
        }

        public ImageButton Expand
        {
            get
            {
                _expand = new ImageButton();
                _expand.ID = "grdExpand";
                _expand.ImageUrl = Arrow_Up_B;
                _expand.Height = 15;
                _expand.Width = 15;
                _expand.ToolTip = "Expand the Table";
                return _expand;
            }
        }

        public Label TempName
        {
            get
            {
                _tempName = new Label();
                _tempName.ID = "TempName";
                _tempName.Text = "Template Name";
                _tempName.Font.Bold = true;

                return _tempName;
            }
        }

        public TextBox TemplateName
        {
            get
            {
                _templateName.ID = "TemplateName";

                return _templateName;
            }
        }

        public CheckBox IsPublic
        {
            get
            {
                _ispublic.ID = "ispublic";
                _ispublic.Text = " Is Public";
                _ispublic.TextAlign = TextAlign.Right;
                _ispublic.Font.Bold = true;

                return _ispublic;
            }
        }

        public Label TempDesc
        {
            get
            {
                _temDesc = new Label();
                _temDesc.ID = "TempDesc";
                _temDesc.Text = "Template Description";
                _temDesc.Font.Bold = true;

                return _temDesc;
            }
        }

        public TextBox TemplateDescription
        {
            get
            {
                _templateDescription.ID = "TemplateDesc";

                return _templateDescription;
            }
        }

        public ImageButton CreateTempOK
        {
            get
            {
                _createNewTempOk.ID = "CreateNewTempOkBtn";
                _createNewTempOk.Height = 15;
                _createNewTempOk.Width = 15;
                _createNewTempOk.ToolTip = "Ok";
                _createNewTempOk.ImageUrl = SaveExport;
                _createNewTempOk.Style.Add("float", "left");
                _createNewTempOk.OnClientClick = "javascript:return fnUpdate(this)";
                _createNewTempOk.Click += new ImageClickEventHandler(_createNewTempOk_Click);

                return _createNewTempOk;
            }
        }


        public LinkButton CreateNewTempSave
        {
            get
            {
                _createNewTempSave.ID = "createNewTempSave";
                _createNewTempSave.Text = "&nbsp;Save";
                _createNewTempSave.OnClientClick = "javascript:return fnUpdate(this)";
                _createNewTempSave.Click += new EventHandler(_createNewTempSave_Click);
                return _createNewTempSave;
            }
        }

        void _createNewTempSave_Click(object sender, EventArgs e)
        {
            SaveorUpdateData();
        }

        void _createNewTempOk_Click(object sender, EventArgs e)
        {
            SaveorUpdateData();
        }

        private void SaveorUpdateData()
        {
            exportFlag = "export";
            COEDataView liveDV = new COEDataView();
            string xmlNS = "COE.ResultsCriteria";
            string xmlNamespace = "COE";
            manager = new XmlNamespaceManager(new NameTable());
            manager.AddNamespace(xmlNamespace, xmlNS);

            XmlDocument doc2 = new XmlDocument();
            doc2.LoadXml(PageDataview.ToString());
            liveDV.GetFromXML(doc2);

            ResultsCriteria resSend = new ResultsCriteria();
            resSend = MergeResInsert();

            //Calling Save Functionaity
            #region Save/Update Functionality
            COEExportTemplateBO cetb;
            cetb = (COEExportTemplateBO)Page.Session["ExporttempBo"];

            if (Page.Session["ExportTemplateId"] != null)
            {
                cetb.Name = TemplateName.Text;
                cetb.Description = TemplateDescription.Text;
                cetb.IsPublic = IsPublic.Checked;
                cetb.UserName = COEUser.Name;
                cetb.DataViewId = liveDV.DataViewID;
                cetb.ResultCriteria = resSend;
                cetb.Update();
                Page.Session["ExportTemplateId"] = cetb.ID;
                CurrentSelectedTemplate.Text = cetb.Name;
                ExportControl.currentTemplate = cetb.Name;
            }
            else
            {
                COEExportTemplateBO exporttemp = new COEExportTemplateBO();
                exporttemp.Name = TemplateName.Text;
                exporttemp.Description = TemplateDescription.Text;
                exporttemp.IsPublic = IsPublic.Checked;
                exporttemp.UserName = COEUser.Name;
                exporttemp.DataViewId = liveDV.DataViewID;
                exporttemp.ResultCriteria = resSend;
                exporttemp.Save();
                Page.Session["ExportTemplateId"] = exporttemp.ID.ToString();
                CurrentSelectedTemplate.Text = exporttemp.Name;
                ExportControl.currentTemplate = exporttemp.Name;
            }
            #endregion

            //after save functinality
            PanCreateTemp.Visible = false;
            Page.Session["editclick"] = null;
            panEditTemp.Enabled = true;
            panDeleteTemp.Enabled = true;
            BindGrid(false, GridViewList, 0, null);
        }

        #region Build Result Criteria for Insert/Update into Database
        public ResultsCriteria CheckedGridValues()
        {
            // GridView grd = new GridView();
            ArrayList arrfieldid = new ArrayList();
            ArrayList aliasnames = new ArrayList();
            ArrayList arrtextalias = new ArrayList();
            string structureid = "";

            try
            {
                GridViewRowCollection rowCollection = GridViewList.Rows;

                string xmlNS = "COE.COEDataView";
                string xmlNamespace = "COE";
                manager = new XmlNamespaceManager(new NameTable());
                manager.AddNamespace(xmlNamespace, xmlNS);

                XmlDocument doc2 = new XmlDocument();
                doc2.LoadXml(PageDataview.ToString());

                #region Looping Grid for selected values
                foreach (GridViewRow gridRow in rowCollection)
                {
                    //Get all the cells contained in the row
                    TableCellCollection rowCell = gridRow.Cells;
                    if (gridRow.HasControls() == true)
                    {
                        GridView grd = (GridView)gridRow.FindControl("GridViewColumn");
                        GridViewRowCollection rowCol = grd.Rows;
                        //CheckBox chk = new CheckBox();
                        //HiddenField hdn = new HiddenField();
                        //Label lbl = new Label();
                        //TextBox txt = new TextBox();
                        foreach (GridViewRow gvColRow in rowCol)
                        {
                            if (gvColRow.HasControls() == true)
                            {
                                CheckBox chk = (CheckBox)gvColRow.FindControl("checkvisibile");
                                if (chk.Checked == true)
                                {
                                    HiddenField hdn = (HiddenField)gvColRow.FindControl("hiddenvisibile");
                                    Label lbl = (Label)gvColRow.FindControl("lbltempID2");
                                    TextBox txt = (TextBox)gvColRow.FindControl("txtailasgrdvalue");

                                    if (hdn != null)
                                        arrfieldid.Add(hdn.Value);

                                    if (txt != null)
                                        arrtextalias.Add(txt.Text);

                                    if (lbl != null)
                                    {
                                        aliasnames.Add(lbl.Text);
                                    }

                                    if (hdn != null && txt != null)
                                    {
                                        XmlNode fields = doc2.SelectSingleNode("//" + xmlNamespace + ":tables/COE:table/COE:fields[@id='" + hdn.Value + "']", manager);
                                        if (fields.Attributes["visible"] == null)
                                        {
                                            fields.Attributes.Append(fields.OwnerDocument.CreateAttribute("visible"));
                                        }
                                        fields.Attributes["visible"].Value = "1";
                                        fields.Attributes["alias"].Value = txt.Text;
                                    }
                                }

                            }
                        }
                    }
                }
                #endregion

                ResultsCriteria template = new ResultsCriteria();
                XmlNode tablesNodeList = doc2.SelectSingleNode("//" + xmlNamespace + ":tables", manager);

                #region Looping Dataview to Create Result Criteria from checked values in Grid
                // Coverity Fix CID - 13129
                if (tablesNodeList != null)
                {
                    foreach (XmlNode tnode in tablesNodeList.ChildNodes)
                    {
                        ResultsCriteria.ResultsCriteriaTable tabRes = new ResultsCriteria.ResultsCriteriaTable();
                        tabRes.Id = Convert.ToInt32(tnode.Attributes["id"].Value);

                        foreach (XmlNode node in tnode.ChildNodes)
                        {
                            if (arrfieldid.Contains(node.Attributes["id"].Value))
                            {
                                tabRes.Criterias.Add(BuildResultCriteriafield(node));
                            }
                        }
                        template.Add(tabRes);
                    }
                }
                #endregion


                return template;
            }
            catch
            {
                throw;
            }
            finally
            {
                // Coverity Fix CID - 10869 (from local server)
                //grd.Dispose();
            }
        }
        public ResultsCriteria.Field BuildResultCriteriafield(XmlNode node)
        {
            ResultsCriteria.Field field = new ResultsCriteria.Field();

            field.Id = Convert.ToInt32(node.Attributes["id"].Value);
            field.Alias = node.Attributes["alias"].Value;

            field.Visible = true;

            if (node.Attributes["sortOrder"] != null)
                field.OrderById = Convert.ToInt32(node.Attributes["sortOrder"].Value);
            else
                field.OrderById = 0;

            if (node.Attributes["lookupSortOrder"] != null && node.Attributes["lookupSortOrder"].Value == "ASCENDING")
                field.Direction = ResultsCriteria.SortDirection.ASC;
            else
                field.Direction = ResultsCriteria.SortDirection.DESC;

            return field;
        }

        public ResultsCriteria.Field BuildResultCriteriafield(XmlNode node, ArrayList ListOfIds, ArrayList ListOfAliases)
        {
            ResultsCriteria.Field field = new ResultsCriteria.Field();

            field.Id = Convert.ToInt32(node.Attributes["id"].Value);
            field.Alias = node.Attributes["alias"].Value;
            field.Visible = true;

            if (node.Attributes["sortOrder"] != null)
                field.OrderById = Convert.ToInt32(node.Attributes["sortOrder"].Value);
            else
                field.OrderById = 0;

            if (node.Attributes["lookupSortOrder"] != null && node.Attributes["lookupSortOrder"].Value == "ASCENDING")
                field.Direction = ResultsCriteria.SortDirection.ASC;
            else
                field.Direction = ResultsCriteria.SortDirection.DESC;

            return field;
        }


        #endregion

        public ImageButton CreateTempCancel
        {
            get
            {
                _createNewTempCancel = new ImageButton();
                _createNewTempCancel.ID = "CreateNewTempCancelBtn";
                _createNewTempCancel.Width = 15;
                _createNewTempCancel.Height = 15;
                _createNewTempCancel.ToolTip = "Cancel";
                _createNewTempCancel.ImageUrl = BlockRed;
                _createNewTempCancel.Click += new ImageClickEventHandler(_createNewTempCancel_Click);

                return _createNewTempCancel;
            }
        }

        void _createNewTempCancel_Click(object sender, EventArgs e)
        {
            PanCreateTemp.Visible = false;
            Page.Session["editclick"] = null;
            BindGrid(false, GridViewList, 0, null);
        }

        #endregion

        #region Dataview/Result Criteria
        public string ResultCriteriaTest
        {
            get
            {
                if (Page.Session["ResultCriteriaTest"] != null)
                    return (string)Page.Session["ResultCriteriaTest"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.resultcriteria.xml");
            }
            set
            {
                Page.Session["ResultCriteriaTest"] = value;
            }
        }

        public string LiveResultCriteria
        {
            get
            {
                if (Page.Session["LiveResultCriteria"] != null)
                    return (string)Page.Session["LiveResultCriteria"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.LiveResultCriteria.xml");
            }
            set
            {
                Page.Session["LiveResultCriteria"] = value;
            }
        }

        public string LiveDataview
        {
            get
            {
                if (Page.Session["LiveDataview"] != null)
                    return (string)Page.Session["LiveDataview"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(ExportControl), "CambridgeSoft.COE.Framework.ServerControls.COEExportControl.LiveDataview.xml");
            }
            set
            {
                Page.Session["LiveDataview"] = value;
            }
        }

        public COEDataView PageDataview
        {
            get
            {
                if (Page.Session["PageDataview"] != null)
                    return (COEDataView)Page.Session["PageDataview"];
                else
                    return _pageDataview;
            }
            set
            {
                // removing nonrelational tables from dataview.
                COEDataView clone = new COEDataView();
                clone.GetFromXML(value.ToString());
                clone.RemoveNonRelationalTables();
                Page.Session["PageDataview"] = clone;
            }
        }

        public ResultsCriteria PageResultsCriteria
        {
            get
            {
                if (Page.Session["PageResultsCriteria"] != null)
                    return (ResultsCriteria)Page.Session["PageResultsCriteria"];
                else
                    return _pageResultsCriteria;
            }
            set
            {
                Page.Session["PageResultsCriteria"] = value;
            }
        }
        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Creation of Child controls
        /// </summary>
        protected override void CreateChildControls()
        {

            this.Controls.Clear();

            #region Initial Declarations
            Panel pan = new Panel();
            pan.ID = "grdPan";
            pan.Width = 550;
            pan.Height = 300;
            pan.ScrollBars = ScrollBars.Vertical;

            //New table is created
            HtmlTable table = new HtmlTable();
            HtmlTableRow newRow;
            HtmlTableCell newCell;
            table.Border = 0;
            table.Style.Add("DISPLAY", "inline");
            table.Style.Add("VERTICAL-ALIGN", "middle");
            table.Style.Add("margin", "10px");
            table.BgColor = "#EFEFEF";
            table.Width = "550px";
            #endregion
            //-------------------------First Row is added -------------------------------------------

            #region First Row in the table
            // new Row tr is created
            newRow = new HtmlTableRow();

            //filling the cell td is created and added the controls to it
            newCell = new HtmlTableCell();
            newCell.Controls.Add(Export);
            newCell.Width = "80px";
            newCell.Align = "left";
            newRow.Cells.Add(newCell);

            newCell = new HtmlTableCell();
            newCell.Controls.Add(Cancel);
            newCell.Align = "left";
            newCell.Width = "80px";
            newRow.Cells.Add(newCell);

            newCell = new HtmlTableCell();
            newCell.Controls.Add(ExportFormatLabel);
            newCell.Align = "right";
            newCell.Width = "135px";
            newRow.Cells.Add(newCell);

            newCell = new HtmlTableCell();
            newCell.Controls.Add(ExportFormatList);
            newCell.Width = "25px";
            newCell.Align = "center";
            newRow.Cells.Add(newCell);

            newCell = new HtmlTableCell();
            newCell.Controls.Add(ExportFormatSelected);
            newCell.ColSpan = 3;
            newCell.Width = "250px";
            newCell.Align = "left";
            newRow.Cells.Add(newCell);

            if (newRow.Cells.Count > 0)
                table.Rows.Add(newRow);
            #endregion

            //-------------------------Space is added -------------------------------------------
            #region Space is added

            newRow = new HtmlTableRow();
            Panel panspace = new Panel();
            panspace.ID = "panSpace";
            newCell = new HtmlTableCell();
            newCell.Height = "10px";
            newCell.ColSpan = 7;
            newCell.Controls.Add(panspace);
            newRow.Cells.Add(newCell);

            if (newRow.Cells.Count > 0)
                table.Rows.Add(newRow);
            #endregion
            //-------------------------Second Row is added -------------------------------------------

            #region Second row in the table
            // new Row tr is created
            newRow = new HtmlTableRow();

            //filling the cell td is created and added the controls to it
            newCell = new HtmlTableCell();
            newCell.Align = "right";
            newCell.Width = "100px";
            newCell.ColSpan = 2;
            newCell.Controls.Add(CurrentTemp);
            newRow.Cells.Add(newCell);

            Panel panTemplist = new Panel();
            panTemplist.ID = "panTempListdivt";
            panTemplist.Style.Add("float", "left");
            panTemplist.Style.Add("margin-left", "5px");
            panTemplist.Width = new Unit(20);
            panTemplist.Controls.Add(TemplateList);


            newCell = new HtmlTableCell();
            newCell.Width = "250px";
            newCell.Controls.Add(panTemplist);
            newCell.Controls.Add(CurrentSelectedTemplate);
            newRow.Cells.Add(newCell);



            newCell = new HtmlTableCell();
            newCell.ColSpan = 4;

            Panel panCreatetemplate = new Panel();
            panCreatetemplate.ID = "panCreatetemp";
            panCreatetemplate.Style.Add("border", "1px solid #EFEFEF");
            panCreatetemplate.Attributes.Add("onmouseover", "this.style.border='1px solid #CCC'");
            panCreatetemplate.Attributes.Add("onmouseout", "this.style.border='1px solid #EFEFEF'");
            panCreatetemplate.Style.Add("float", "left");
            panCreatetemplate.Width = new Unit(65);


            LinkButton lnkAdd = new LinkButton();
            lnkAdd.Text = " Add";
            lnkAdd.ID = "Addiid";
            lnkAdd.Click += new EventHandler(lnkAdd_Click);

            panCreatetemplate.Controls.Add(CreateNewTemplate);
            panCreatetemplate.Controls.Add(lnkAdd);
            newCell.Controls.Add(panCreatetemplate);

            panEditTemp = new Panel();
            panEditTemp.ID = "panEdittemp";
            panEditTemp.Style.Add("border", "1px solid #EFEFEF");
            panEditTemp.Attributes.Add("onmouseover", "this.style.border='1px solid #CCC'");
            panEditTemp.Attributes.Add("onmouseout", "this.style.border='1px solid #EFEFEF'");
            panEditTemp.Style.Add("float", "left");
            panEditTemp.Width = new Unit(65);

            newCell.Align = "left";
            LinkButton lnkEdit = new LinkButton();
            lnkEdit.Text = " Edit";
            lnkEdit.ID = "Editiid";
            lnkEdit.Click += new EventHandler(lnkEdit_Click);

            panEditTemp.Controls.Add(Edit);
            panEditTemp.Controls.Add(lnkEdit);
            panEditTemp.Enabled = false;
            newCell.Controls.Add(panEditTemp);

            //Delete Panel Image and link Button
            panDeleteTemp = new Panel();
            panDeleteTemp.ID = "panDeleteTemp";
            panDeleteTemp.Style.Add("border", "1px solid #EFEFEF");
            panDeleteTemp.Attributes.Add("onmouseover", "this.style.border='1px solid #CCC'");
            panDeleteTemp.Attributes.Add("onmouseout", "this.style.border='1px solid #EFEFEF'");
            panDeleteTemp.Style.Add("float", "left");
            panDeleteTemp.Width = new Unit(65);

            LinkButton lnkDelete = new LinkButton();
            lnkDelete.Text = " Delete";
            lnkDelete.ID = "Deleteiid";
            lnkDelete.Click += new EventHandler(lnkDelete_Click);
            string DelConfirmationScript = "return confirm('Are you sure want to delete this template?');";
            lnkDelete.Attributes.Add("onclick", DelConfirmationScript);

            panDeleteTemp.Controls.Add(Delete);
            panDeleteTemp.Controls.Add(lnkDelete);
            panDeleteTemp.Enabled = false;
            newCell.Controls.Add(panDeleteTemp);

            newRow.Cells.Add(newCell);

            if (newRow.Cells.Count > 0)
                table.Rows.Add(newRow);
            #endregion
            //-------------------------Space is added -------------------------------------------
            #region Space is added

            newRow = new HtmlTableRow();
            panspace = new Panel();
            panspace.ID = "pansSpace";
            newCell = new HtmlTableCell();
            newCell.Height = "10px";
            newCell.ColSpan = 7;
            newCell.Controls.Add(panspace);
            newRow.Cells.Add(newCell);

            if (newRow.Cells.Count > 0)
                table.Rows.Add(newRow);
            #endregion

            //---------------------------Create New Template-----------------------------------------

            #region Create New Template
            //Create new Template popup
            HtmlTable tableCr = new HtmlTable();
            tableCr.Height = "75px";
            HtmlTableRow newRowCr = new HtmlTableRow();

            //Template name Label
            HtmlTableCell newCellCr = new HtmlTableCell();
            newCellCr.Controls.Add(TempName);
            newRowCr.Controls.Add(newCellCr);

            //Template name Textbox
            newCellCr = new HtmlTableCell();
            newCellCr.Controls.Add(TemplateName);
            newRowCr.Controls.Add(newCellCr);

            //Is public checkbox
            newCellCr = new HtmlTableCell();
            newCellCr.Width = "150px";
            newCellCr.Align = "right";
            newCellCr.Controls.Add(IsPublic);
            newRowCr.Controls.Add(newCellCr);

            if (newRowCr.Cells.Count > 0)
                tableCr.Controls.Add(newRowCr);

            //Template Description Label
            newRowCr = new HtmlTableRow();
            newCellCr = new HtmlTableCell();
            newCellCr.Controls.Add(TempDesc);
            newRowCr.Controls.Add(newCellCr);

            //Template Description Textbox
            newCellCr = new HtmlTableCell();
            newCellCr.Controls.Add(TemplateDescription);
            newCellCr.ColSpan = 3;
            newRowCr.Controls.Add(newCellCr);

            if (newRowCr.Cells.Count > 0)
                tableCr.Controls.Add(newRowCr);

            newRowCr = new HtmlTableRow();
            newCellCr = new HtmlTableCell();
            newCellCr.ColSpan = 2;

            Panel panTempOK = new Panel();
            panTempOK.ID = "panTempOK";
            panTempOK.Style.Add("border", "1px solid #E1E1E1");
            panTempOK.Attributes.Add("onmouseover", "this.style.border='1px solid #CCC'");
            panTempOK.Attributes.Add("onmouseout", "this.style.border='1px solid #E1E1E1'");
            panTempOK.Style.Add("float", "left");
            panTempOK.Width = new Unit("70");

            //Template Save/Update Button 
            panTempOK.Controls.Add(CreateTempOK);
            panTempOK.Controls.Add(CreateNewTempSave);
            newCellCr.Controls.Add(panTempOK);
            newRowCr.Controls.Add(newCellCr);

            Panel panTempCancel = new Panel();
            panTempCancel.ID = "panTempCancel";
            panTempCancel.Style.Add("border", "1px solid #E1E1E1");
            panTempCancel.Attributes.Add("onmouseover", "this.style.border='1px solid #CCC'");
            panTempCancel.Attributes.Add("onmouseout", "this.style.border='1px solid #E1E1E1'");
            panTempCancel.Width = new Unit("70px");
            panTempCancel.Style.Add("float", "left");

            //Template Cancel Button 
            LinkButton lnkCanceltemp = new LinkButton();
            lnkCanceltemp.Text = "&nbsp;Cancel";
            lnkCanceltemp.ID = "lnkCanceltemplate";
            lnkCanceltemp.Click += new EventHandler(lnkCanceltemp_Click);

            panTempCancel.Controls.Add(CreateTempCancel);
            panTempCancel.Controls.Add(lnkCanceltemp);
            newCellCr.Controls.Add(panTempCancel);

            newRowCr.Controls.Add(newCellCr);


            newCellCr = new HtmlTableCell();
            newCellCr.ColSpan = 2;
            newRowCr.Controls.Add(newCellCr);

            if (newRowCr.Cells.Count > 0)
                tableCr.Controls.Add(newRowCr);

            PanCreateTemp.Controls.Add(tableCr);


            newRow = new HtmlTableRow();
            newCell = new HtmlTableCell();
            newCell.ColSpan = 7;
            newCell.Controls.Add(PanCreateTemp);
            newRow.Controls.Add(newCell);
            table.Rows.Add(newRow);
            #endregion
            //---------------------------Grid Control-----------------------------------------

            #region Grid Control
            // new Row tr is created
            newRow = new HtmlTableRow();
            newCell = new HtmlTableCell();

            BindGrid(false, GridViewList, 0, null);
            pan.Controls.Add(GridViewList);

            newCell.Controls.Add(pan);
            newCell.ColSpan = 7;


            newRow.Cells.Add(newCell);

            if (newRow.Cells.Count > 0)
                table.Rows.Add(newRow);
            #endregion

            HtmlTable tableH = new HtmlTable();
            HtmlTableRow newRowH = new HtmlTableRow();
            HtmlTableCell newCellH = new HtmlTableCell();
            tableH.Border = 1;
            tableH.BgColor = "#EFEFEF";
            tableH.Style.Add("DISPLAY", "inline");
            tableH.Style.Add("VERTICAL-ALIGN", "middle");

            Label lblHeading = new Label();
            lblHeading.ID = "lblHeading";
            lblHeading.Text = "&nbsp;&nbsp;Export Control ";
            lblHeading.ForeColor = System.Drawing.Color.White;
            lblHeading.Font.Bold = true;
            newCellH.BgColor = "#54BCF0";
            newCellH.BorderColor = "#0505FA";
            newCellH.Controls.Add(lblHeading);
            newRowH.Controls.Add(newCellH);
            tableH.Rows.Add(newRowH);

            newRowH = new HtmlTableRow();
            newCellH = new HtmlTableCell();
            newCellH.BorderColor = "#0505FA";
            newCellH.Controls.Add(table);
            newRowH.Controls.Add(newCellH);
            tableH.Rows.Add(newRowH);


            this.Controls.Add(tableH);
            this.Controls.Add(AddPopup());
            this.Controls.Add(AddPopupTemplateList());
            this.Controls.Add(HdnTemplateId);
            this.Controls.Add(HdnFormatId);
            this.ChildControlsCreated = true;
            ValidationControls();
            RegisteringScripts();
            ((GUIShell.GUIShellMaster)this.Page.Master).MakeCtrlShowProgressModal(this._export.ClientID, "Exporting...", string.Empty, false);
        }

        void lnkEdit_Click(object sender, EventArgs e)
        {
            EditClickEvent();
        }

        void lnkDelete_Click(object sender, EventArgs e)
        {
            DeleteClickEvent();
        }

        void lnkAdd_Click(object sender, EventArgs e)
        {
            AddClickEvent();
        }

        void lnkCanceltemp_Click(object sender, EventArgs e)
        {

            exportFlag = "export";
            PanCreateTemp.Visible = false;
            Page.Session["editclick"] = null;
            BindGrid(false, GridViewList, 0, null);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeComponent();
        }

        #region Grid View Events
        /// <summary>
        /// Grid View Events Intitiliaze
        /// </summary>
        private void InitializeComponent()
        {
            GridViewList.RowDataBound += new GridViewRowEventHandler(GridViewList_RowDataBound);
        }

        void GridViewList_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridView dlist = (GridView)e.Row.FindControl("GridViewColumn");
                Label lblid = (Label)e.Row.FindControl("lbltempID");

                Panel pangrid = (Panel)e.Row.FindControl("GridPanel");
                ImageButton ibtnExapand = (ImageButton)e.Row.FindControl("ExpandGrid");
                if (dlist != null && lblid != null)
                {
                    if (Page.Session["editclick"] != null && Page.Session["editclick"].ToString() == "true")
                        BindGrid(true, dlist, 1, lblid.Text);
                    else
                        BindGrid(false, dlist, 1, lblid.Text);
                }
                if (ibtnExapand != null && pangrid != null)
                {
                    ibtnExapand.ImageUrl = Arrow_Up_B;
                    ibtnExapand.Height = 15;
                    ibtnExapand.Width = 15;
                    ibtnExapand.OnClientClick = "javascript:var pop = document.getElementById(this.id.replace('" + ibtnExapand.ClientID + "','" + pangrid.ClientID + "')); if(pop == null){ pop = document.getElementById(this.id.replace('" + ibtnExapand.ClientID + "','" + pangrid.ClientID + "'));} var img = document.getElementById(this.id.replace('" + ibtnExapand.ClientID + "','" + ibtnExapand.ClientID + "')); if(img == null){ img = document.getElementById(this.id.replace('" + ibtnExapand.ClientID + "','" + ibtnExapand.ClientID + "'))} if(pop != null && pop.style.display == ''){pop.style.display = 'none';img.src='" + Arrow_Down_B + "';}else if(pop!=null){pop.style.display = '';img.src='" + Arrow_Up_B + "';};return false;";
                }
                CheckBox checkTable = (CheckBox)e.Row.FindControl("CheckRows");
                if (checkTable != null && pangrid != null && dlist != null)              // Coverity Fix CID - 11651
                {
                    checkTable.Attributes.Add("OnClick", "javascript: selectAll(this.id,'" + dlist.ClientID + "'," + hiddenCounts[e.Row.RowIndex] + ");");
                }
            }
        }
        #endregion

        /// <summary>
        /// Grid View Binding 
        /// </summary>
        /// <param name="edit">Boolean</param>
        /// <param name="grvLoad">Grid view to be bound</param>
        /// <param name="tab">Table number(Parent/child)</param>
        /// <param name="val">Parent node id used for generating child node</param>
        private void BindGrid(bool edit, GridView grvLoad, int tab, string val)
        {
            #region Variable
            COEDataView liveDV = new COEDataView();
            string xmlNS = "COE.COEDataView";
            string xmlNamespace = "COE";
            string xmlNSres = "COE.ResultsCriteria";
            manager = new XmlNamespaceManager(new NameTable());
            XmlNamespaceManager managerRes = new XmlNamespaceManager(new NameTable());
            manager.AddNamespace(xmlNamespace, xmlNS);
            managerRes.AddNamespace(xmlNamespace, xmlNSres);
            int basetableid = PageDataview.Basetable;

            XmlDocument doc2 = new XmlDocument();

            //Implementes Page Dataview
            doc2.LoadXml(AddMolWt(PageDataview).ToString());

            liveDV.GetFromXML(doc2);
            #endregion

            //----------------------------------------------------------------
            // Get Result criteria and Update dataview fields based on Template

            #region Get Result Criteria / Template
            XmlDocument templateCr = new XmlDocument();
            if (Page.Session["ExportTemplateId"] != null && Page.Session["ExportTemplateId"] != "")
            {
                ExporttempBo = COEExportTemplateBO.Get(Convert.ToInt32(Page.Session["ExportTemplateId"]));
                if (ExporttempBo.ResultCriteria != null)
                {

                    templateCr.Load(new StringReader(ExporttempBo.ResultCriteria.ToString()));
                }

                bool isStructureIndex = false;
                bool isStructureMimeType = false;
                bool isStructureColumn = false;

                XmlNode templateDVNodelist = doc2.SelectSingleNode("//" + xmlNamespace + ":tables", manager);
                for (int i = 0; i < templateDVNodelist.ChildNodes.Count; i++)
                {
                    XmlNode tdvnode = templateDVNodelist.ChildNodes[i];
                    for (int j = 0; j < tdvnode.ChildNodes.Count; j++)
                    {
                        XmlNode tFielddvnode = tdvnode.ChildNodes[j];

                        XmlNodeList dvfNodeList = doc2.SelectNodes("//" + xmlNamespace + ":tables/COE:table/COE:fields[@id='" + tFielddvnode.Attributes["id"].Value + "']", manager);
                        if (dvfNodeList.Count == 3)//structure,Mol wt and Mol formula having same ids
                        {
                            XmlNodeList resfNodeList = templateCr.SelectNodes("//" + xmlNamespace + ":tables/COE:table/COE:field[@fieldId='" + tFielddvnode.Attributes["id"].Value + "']", managerRes);

                            for (int k = 0; k < dvfNodeList.Count; k++)
                            {
                                if (resfNodeList.Count > 0)
                                {
                                    for (int l = 0; l < resfNodeList.Count; l++)
                                    {
                                        //false
                                        if (dvfNodeList[k].Attributes["visible"] == null)
                                        {
                                            dvfNodeList[k].Attributes.Append(dvfNodeList[k].OwnerDocument.CreateAttribute("visible"));
                                        }
                                        dvfNodeList[k].Attributes["visible"].Value = "0";
                                        if (dvfNodeList[k].Attributes["alias"].Value.ToLower() == resfNodeList[l].Attributes["alias"].Value.ToLower())
                                        {
                                            //if true then break;
                                            dvfNodeList[k].Attributes["visible"].Value = "1";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    // For Default Template make sure structure,Mol wt and Mol formula are not uncheked
                                    if (ExportControl.currentTemplate != "Default Template")
                                    {
                                        // structure,Mol wt and Mol formula must be unchecked if not saved in template
                                        dvfNodeList[k].Attributes["visible"].Value = "0";
                                    }
                                }
                            }
                            j = j + 2; // escaping remaing 2 nodes which are relaed to structure, that are managing above.

                        }
                        else
                        {
                            XmlNode resFeildNode = templateCr.SelectSingleNode("//" + xmlNamespace + ":tables/COE:table/COE:field[@fieldId='" + tFielddvnode.Attributes["id"].Value + "']", managerRes);
                            XmlNode fields = doc2.SelectSingleNode("//" + xmlNamespace + ":tables/COE:table/COE:fields[@id='" + tFielddvnode.Attributes["id"].Value + "']", manager);
                            if (fields != null)
                            {
                                if (fields.Attributes["visible"] == null)
                                {
                                    fields.Attributes.Append(fields.OwnerDocument.CreateAttribute("visible"));
                                }
                                fields.Attributes["visible"].Value = "0";
                                if (resFeildNode != null)
                                {
                                    fields.Attributes["visible"].Value = "1";

                                    if (fields.Attributes["indexType"] != null && fields.Attributes["indexType"].Value.ToUpper() == "CS_CARTRIDGE")
                                        isStructureIndex = true;
                                    else
                                        isStructureIndex = false;

                                    if (fields.Attributes["mimeType"] != null && fields.Attributes["mimeType"].Value.ToUpper() == "CHEMICAL_X_CDX")
                                        isStructureMimeType = true;
                                    else
                                        isStructureMimeType = false;

                                    isStructureColumn = (isStructureIndex || isStructureMimeType);


                                    if (!(isStructureColumn))
                                        fields.Attributes["alias"].Value = resFeildNode.Attributes["alias"].Value;
                                }
                            }
                        }
                        //}
                    }
                }

            }
            #endregion

            //Loop Dataview and bring the base table to top
            #region Create new Dataview with Base Table on Top
            COEDataView pgDataview = new COEDataView();
            pgDataview.GetFromXML(doc2);
            COEDataView resultDataview = new COEDataView();
            resultDataview = getDataViewClone(pgDataview);
            resultDataview.Tables = new COEDataView.DataViewTableList();

            COEDataView basetableDataview = new COEDataView();
            COEDataView childtableDataview = new COEDataView();
            foreach (COEDataView.DataViewTable coeTable in pgDataview.Tables)
            {
                if (coeTable.Id == basetableid)
                {
                    basetableDataview.Tables.Add(GetDVTableNode(coeTable));
                }
                else
                {
                    childtableDataview.Tables.Add(GetDVTableNode(coeTable));
                }
                if (count < pgDataview.Tables.Count)
                {
                    hiddenCounts.Add(coeTable.Fields.Count);
                    count++;
                }
            }
            //Merge base table and child table
            resultDataview.Tables.AddRange(basetableDataview.Tables);
            resultDataview.Tables.AddRange(childtableDataview.Tables);

            doc2.LoadXml(resultDataview.ToString());
            #endregion
            //----------------------------------------------------------------

            #region Load Grid View
            XmlNode tablesNodeList = doc2.SelectSingleNode("//" + xmlNamespace + ":tables", manager);
            if (val != null && val != "")
            {
                tablesNodeList = GetExactNode(val, tablesNodeList);
            }

            DataSet dss = new DataSet("rr");
            XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(tablesNodeList.OuterXml));
            dss.ReadXml(reader);

            grvLoad.DataSource = "";
            grvLoad.Columns.Clear();

            TemplateField tfield;
            if (grvLoad.ID == "GridviewList")
            {
                tfield = new TemplateField();
                tfield.ItemTemplate = new GridViewTemplate(ListItemType.Footer, "alias");
                grvLoad.Columns.Add(tfield);
                tfield = null;
            }
            else if (grvLoad.ID == "GridViewColumn")
            {
                if (edit == false)
                {
                    tfield = new TemplateField();
                    tfield.ItemTemplate = new GridViewTemplate(ListItemType.Header, "alias");
                    grvLoad.Columns.Add(tfield);
                }
                else
                {
                    tfield = new TemplateField();
                    tfield.ItemTemplate = new GridViewTemplate(ListItemType.EditItem, "alias");
                    grvLoad.Columns.Add(tfield);
                }

                tfield = new TemplateField();
                tfield.ItemTemplate = new GridViewTemplate(ListItemType.Item, "visible");
                tfield.ItemStyle.Width = new Unit(75);
                grvLoad.Columns.Add(tfield);
            }

            grvLoad.ShowHeader = false;
            grvLoad.DataSource = dss.Tables[tab];
            grvLoad.DataBind();
            #endregion
        }
        private COEDataView getDataViewClone(COEDataView pgDataview)
        {
            COEDataView newDataview = new COEDataView();
            newDataview.Application = pgDataview.Application;
            newDataview.Basetable = pgDataview.Basetable;

            newDataview.Database = pgDataview.Database;
            newDataview.DataViewHandling = pgDataview.DataViewHandling;
            newDataview.DataViewID = pgDataview.DataViewID;
            newDataview.Description = pgDataview.Description;
            newDataview.Name = pgDataview.Name;
            newDataview.Relationships = pgDataview.Relationships;
            newDataview.Tables = pgDataview.Tables;
            newDataview.XmlNs = pgDataview.XmlNs;
            return newDataview;
        }
        private COEDataView.DataViewTable GetTableNode(COEDataView.DataViewTable coeTable)
        {
            COEDataView.DataViewTable newCoeTable = new COEDataView.DataViewTable();
            newCoeTable = GetDVTableNode(coeTable);
            newCoeTable.Fields = new COEDataView.DataViewFieldList();
            for (int i = 0; i < coeTable.Fields.Count; i++)
            {
                COEDataView.Field xnode = new COEDataView.Field();
                xnode = coeTable.Fields[i];
                newCoeTable.Fields.Add(xnode);

                //only add the mw and mf virtual columns if we have reason to believe that this is a structure column
                if ((coeTable.Fields[i].IndexType == COEDataView.IndexTypes.CS_CARTRIDGE) ||
                        (coeTable.Fields[i].MimeType == COEDataView.MimeTypes.CHEMICAL_X_CDX))
                {
                    newCoeTable.Fields.Add(GetFieldNode(xnode, "Mol Wt"));
                    newCoeTable.Fields.Add(GetFieldNode(xnode, "Mol Formula"));
                }
            }

            return newCoeTable;
        }
        private COEDataView.Field GetFieldNode(COEDataView.Field node, string alias)
        {
            COEDataView.Field newnode = new COEDataView.Field();
            newnode.Alias = alias;
            newnode.DataType = node.DataType;
            newnode.Id = node.Id;
            newnode.IndexType = node.IndexType;
            newnode.LookupDisplayFieldId = node.LookupDisplayFieldId;
            newnode.LookupFieldId = node.LookupFieldId;
            newnode.LookupSortOrder = node.LookupSortOrder;
            newnode.MimeType = node.MimeType;
            newnode.Name = node.Name;
            newnode.ParentTableId = node.ParentTableId;
            newnode.SortOrder = node.SortOrder;
            newnode.Visible = node.Visible;

            return newnode;
        }
        private COEDataView.DataViewTable GetDVTableNode(COEDataView.DataViewTable node)
        {
            COEDataView.DataViewTable newnode = new COEDataView.DataViewTable();
            newnode.Alias = node.Alias;
            newnode.Database = node.Database;
            newnode.Fields = node.Fields;
            newnode.Id = node.Id;
            newnode.IsView = node.IsView;
            newnode.Name = node.Name;
            newnode.PrimaryKey = node.PrimaryKey;

            return newnode;
        }

        private XmlNode GetExactNode(string id, XmlNode xnode)
        {
            XmlNode Rnode = xnode;
            foreach (XmlNode node in xnode.ChildNodes)
            {
                if (node.Attributes["alias"].Value == id)
                {
                    Rnode = node;
                    break;
                }
            }
            return Rnode;
        }

        #endregion

        #region Exporting Functionality


        public ResultsCriteria MergeExport()
        {
            #region Variable Declaration
            //GridView grd = new GridView();
            ArrayList arrfieldid = new ArrayList();
            ArrayList aliasnames = new ArrayList();
            ArrayList arrtextalias = new ArrayList();
            string structureid = "";
            int basetableid = PageDataview.Basetable;
            GridViewRowCollection rowCollection = GridViewList.Rows;

            string xmlNS = "COE.COEDataView";
            string xmlNSres = "COE.ResultsCriteria";
            string xmlNamespace = "COE";
            manager = new XmlNamespaceManager(new NameTable());
            manager.AddNamespace(xmlNamespace, xmlNS);

            XmlDocument doc2 = new XmlDocument();

            doc2.LoadXml(AddMolWt(PageDataview).ToString());
            #endregion

            #region Looping Grid for selected values
            foreach (GridViewRow gridRow in rowCollection)
            {
                //Get all the cells contained in the row
                TableCellCollection rowCell = gridRow.Cells;
                if (gridRow.HasControls() == true)
                {
                    GridView grd = (GridView)gridRow.FindControl("GridViewColumn");
                    GridViewRowCollection rowCol = grd.Rows;
                    //CheckBox chk = new CheckBox();
                    //HiddenField hdn = new HiddenField();
                    //Label lbl = new Label();
                    //TextBox txt = new TextBox();
                    foreach (GridViewRow gvColRow in rowCol)
                    {
                        if (gvColRow.HasControls() == true)
                        {
                            CheckBox chk = (CheckBox)gvColRow.FindControl("checkvisibile");
                            if (chk.Checked == true)
                            {
                                HiddenField hdn = (HiddenField)gvColRow.FindControl("hiddenvisibile");
                                Label lbl = (Label)gvColRow.FindControl("lbltempID2");
                                TextBox txt = (TextBox)gvColRow.FindControl("txtailasgrdvalue");

                                if (hdn != null)
                                    arrfieldid.Add(hdn.Value);

                                if (txt != null)
                                    arrtextalias.Add(txt.Text);

                                if (lbl != null)
                                {
                                    aliasnames.Add(lbl.Text);
                                }

                                //--------------
                                if (hdn != null && txt != null)
                                {
                                    XmlNode fields = doc2.SelectSingleNode("//" + xmlNamespace + ":tables/COE:table/COE:fields[@id='" + hdn.Value + "']", manager);
                                    if (fields != null && fields.Attributes["visible"] == null)
                                    {
                                        fields.Attributes.Append(fields.OwnerDocument.CreateAttribute("visible"));
                                    }
                                    fields.Attributes["visible"].Value = "1";
                                    fields.Attributes["alias"].Value = txt.Text;
                                }
                                //--------------
                            }

                        }
                    }
                }
            }
            #endregion

            //------------Loooop ------------

            #region LoopResultCriteria

            bool templatestruc = false;
            string tempStuctID = "";
            XmlNode templateNode = null;
            XmlNode sqlfuncNode = null;
            XmlNode sqlMolForNode = null;
            XmlNode sqlMolWtNode = null;
            string fieldid = null;
            ResultsCriteria completeres = new ResultsCriteria();

            XmlDocument templateCr = new XmlDocument();
            templateCr.LoadXml(PageResultsCriteria.ToString());

            manager.AddNamespace(xmlNamespace, xmlNSres);
            XmlNode templateNodeList = templateCr.SelectSingleNode("//" + xmlNamespace + ":tables", manager);

            if (templateNodeList != null)
            {
                #region Checking for sturcture column within  Template
                foreach (XmlNode tnode in templateNodeList.ChildNodes)
                {
                    foreach (XmlNode node in tnode.ChildNodes)
                    {
                        if (node.Attributes["alias"].Value.ToLower().Contains("structure"))
                        {
                            templatestruc = true;
                            sqlfuncNode = node.Clone();
                            tempStuctID = node.Attributes["alias"].Value;
                            fieldid = node.Attributes["fieldId"].Value;
                        }

                        if (node.Attributes["alias"].Value.ToLower().Contains("mol wt") || node.Attributes["alias"].Value.ToLower().Contains("mol formula"))
                        {
                            if (sqlfuncNode == null)
                                sqlfuncNode = node.Clone();
                            else
                                sqlfuncNode.AppendChild(node.Clone());


                            if (node.Attributes["alias"].Value.ToLower().Contains("mol wt"))
                                sqlMolWtNode = node.Clone();
                            else if (node.Attributes["alias"].Value.ToLower().Contains("mol formula"))
                                sqlMolForNode = node.Clone();
                        }
                    }
                }
                #endregion

                ResultsCriteria template4 = new ResultsCriteria();

                manager.AddNamespace(xmlNamespace, xmlNS);
                XmlNode tablesNodeList = doc2.SelectSingleNode("//" + xmlNamespace + ":tables", manager);

                #region Looping Dataview to Create Result Criteria from checked values in Grid
                ResultsCriteria parentres = new ResultsCriteria();
                ResultsCriteria childres = new ResultsCriteria();

                if (tablesNodeList != null)
                {
                    foreach (XmlNode tnode in tablesNodeList.ChildNodes)
                    {
                        if (tnode.Attributes["id"].Value == basetableid.ToString())
                            parentres.Tables.AddRange(BaseTableCheckOld(tnode, template4.Clone(), arrfieldid, aliasnames, templatestruc, sqlfuncNode, sqlMolWtNode, sqlMolForNode).Tables);
                        else
                            childres.Tables.AddRange(BaseTableCheckOld(tnode, template4.Clone(), arrfieldid, aliasnames, templatestruc, sqlfuncNode, sqlMolWtNode, sqlMolForNode).Tables);
                    }

                    completeres = parentres.Clone();
                    completeres.Tables.AddRange(childres.Tables);
                }
                #endregion
            }

            #endregion

            return completeres;
        }

        private ResultsCriteria MergeResInsert()
        {
            #region Variable Declaration
            GridView grd = new GridView();
            ArrayList arrfieldid = new ArrayList();
            ArrayList aliasnames = new ArrayList();
            ArrayList arrtextalias = new ArrayList();
            ArrayList arrjag = new ArrayList();
            #endregion
            try
            {
                string structureid = "";
                int basetableid = PageDataview.Basetable;
                GridViewRowCollection rowCollection = GridViewList.Rows;

                string xmlNS = "COE.COEDataView";
                string xmlNSres = "COE.ResultsCriteria";
                string xmlNamespace = "COE";
                manager = new XmlNamespaceManager(new NameTable());
                manager.AddNamespace(xmlNamespace, xmlNS);

                XmlDocument doc2 = new XmlDocument();
                //Add mol wt and mol formula to Dataview
                doc2.LoadXml(AddMolWt(PageDataview).ToString());


                #region Looping Grid for selected values
                foreach (GridViewRow gridRow in rowCollection)
                {
                    //Get all the cells contained in the row
                    TableCellCollection rowCell = gridRow.Cells;
                    if (gridRow.HasControls() == true)
                    {
                        grd = (GridView)gridRow.FindControl("GridViewColumn");
                        GridViewRowCollection rowCol = grd.Rows;
                        //CheckBox chk = new CheckBox();
                        //HiddenField hdn = new HiddenField();
                        //Label lbl = new Label();
                        //TextBox txt = new TextBox();
                        foreach (GridViewRow gvColRow in rowCol)
                        {
                            if (gvColRow.HasControls() == true)
                            {
                                CheckBox chk = (CheckBox)gvColRow.FindControl("checkvisibile");
                                if (chk.Checked == true)
                                {
                                    HiddenField hdn = (HiddenField)gvColRow.FindControl("hiddenvisibile");
                                    Label lbl = (Label)gvColRow.FindControl("lbltempID2");
                                    TextBox txt = (TextBox)gvColRow.FindControl("txtailasgrdvalue");

                                    if (hdn != null)
                                        arrfieldid.Add(hdn.Value);

                                    if (txt != null)
                                        arrtextalias.Add(txt.Text);

                                    if (lbl != null)
                                    {
                                        aliasnames.Add(lbl.Text);
                                    }

                                    //--------------
                                    if (hdn != null && txt != null)
                                    {
                                        // Structure, Mol Weight, Formula will have same ID, Check the alias and set the visible attrib
                                        XmlNodeList fieldList = doc2.SelectNodes("//" + xmlNamespace + ":tables/COE:table/COE:fields[@id='" + hdn.Value + "']", manager);
                                        if (fieldList.Count == 3)
                                        {
                                            for (int i = 0; i < fieldList.Count; i++)
                                            {
                                                if (fieldList[i].Attributes["alias"].Value == txt.Text)
                                                {
                                                    fieldList[i].Attributes["visible"].Value = "1";
                                                }

                                            }
                                        }
                                        else
                                        {
                                            XmlNode fields = doc2.SelectSingleNode("//" + xmlNamespace + ":tables/COE:table/COE:fields[@id='" + hdn.Value + "']", manager);
                                            if (fields != null && fields.Attributes["visible"] == null)
                                            {
                                                fields.Attributes.Append(fields.OwnerDocument.CreateAttribute("visible"));
                                            }
                                            fields.Attributes["visible"].Value = "1";
                                            if (!(fields.Attributes["alias"].Value.ToLower().Contains("structure") || fields.Attributes["alias"].Value.ToLower().Contains("mol wt") || fields.Attributes["alias"].Value.ToLower().Contains("mol formula")))
                                                fields.Attributes["alias"].Value = txt.Text;
                                        }
                                    }
                                    //--------------
                                }

                            }
                        }
                    }
                }
                #endregion

                ResultsCriteria completeres = new ResultsCriteria();

                manager.AddNamespace(xmlNamespace, xmlNS);
                XmlNode tablesNodeList = doc2.SelectSingleNode("//" + xmlNamespace + ":tables", manager);

                if (tablesNodeList != null)
                {
                    #region Looping Dataview to Create Result Criteria from checked values in Grid
                    ResultsCriteria parentres = new ResultsCriteria();
                    ResultsCriteria childres = new ResultsCriteria();

                    foreach (XmlNode tnode in tablesNodeList.ChildNodes)
                    {

                        if (tnode.Attributes["id"].Value == basetableid.ToString())
                            parentres.Tables.AddRange(BaseTableCheck(tnode, arrfieldid, arrtextalias).Tables);
                        else
                            childres.Tables.AddRange(BaseTableCheck(tnode, arrfieldid, arrtextalias).Tables);
                    }

                    completeres = parentres.Clone();
                    completeres.Tables.AddRange(childres.Tables);

                    #endregion
                }
                return completeres;
            }
            catch
            {
                throw;
            }
            finally
            {
                // Coverity Fix CID - 10871 (from local server)
                grd.Dispose();
            }
        }


        private COEDataView AddMolWt(COEDataView pgDataview)
        {
            int basetableid = PageDataview.Basetable;
            COEDataView resultDataview = new COEDataView();
            resultDataview = getDataViewClone(pgDataview);
            resultDataview.Tables = new COEDataView.DataViewTableList();
            COEDataView basetableDataview = new COEDataView();
            COEDataView childtableDataview = new COEDataView();
            foreach (COEDataView.DataViewTable coeTable in pgDataview.Tables)
            {
                if (coeTable.Id == basetableid)
                {
                    basetableDataview.Tables.Add(GetTableNode(coeTable));
                }
                else
                {
                    childtableDataview.Tables.Add(GetTableNode(coeTable));
                }
            }
            //Merge base table and child table
            resultDataview.Tables.AddRange(basetableDataview.Tables);
            resultDataview.Tables.AddRange(childtableDataview.Tables);

            return resultDataview;
        }

        private ResultsCriteria BaseTableCheckOld(XmlNode tnode, ResultsCriteria template4, ArrayList arrfieldid, ArrayList aliasnames, bool templatestruc, XmlNode sqlfuncNode, XmlNode molwt, XmlNode molform)
        {
            ResultsCriteria template2 = new ResultsCriteria();
            ResultsCriteria.ResultsCriteriaTable tabRes4 = new ResultsCriteria.ResultsCriteriaTable();
            tabRes4.Id = Convert.ToInt32(tnode.Attributes["id"].Value);

            bool isStructureIndex = false;
            bool isStructureMimeType = false;
            bool isStructureColumn = false;

            foreach (XmlNode node in tnode.ChildNodes)
            {

                //is there a structure index
                if (node.Attributes["indexType"] != null && node.Attributes["indexType"].Value.ToUpper() == "CS_CARTRIDGE")
                    isStructureIndex = true;
                else
                    isStructureIndex = false;

                if (node.Attributes["mimeType"] != null && node.Attributes["mimeType"].Value.ToUpper() == "CHEMICAL_X_CDX")
                    isStructureMimeType = true;
                else
                    isStructureMimeType = false;

                isStructureColumn = (isStructureIndex || isStructureMimeType);


                if (arrfieldid.Contains(node.Attributes["id"].Value))
                {
                    //if (!((node.Attributes["alias"].Value.ToLower().Contains("structure") && node.Attributes["alias"].Value.ToLower().Contains("mol wt") && node.Attributes["alias"].Value.ToLower().Contains("mol formula")) || node.Attributes["alias"].Value.ToLower().Contains("mol wt") || node.Attributes["alias"].Value.ToLower().Contains("mol formula")))
                    //if it is not a structure column
                    if (!isStructureColumn)
                    {
                        tabRes4.Criterias.Add(BuildResultCriteriafield(node.Clone(), arrfieldid, aliasnames));
                    }
                }

                #region Loop for SQLFunction
                //if Template has Structure append SQl function completely
                if (Page.Session["ExportTemplateId"] != null && templatestruc == true)
                {
                    if (isStructureColumn && arrfieldid.Contains(node.Attributes["id"].Value))
                    {
                        if (sqlfuncNode != null)
                        {
                            foreach (XmlNode unode in sqlfuncNode.ChildNodes)
                            {
                                ResultsCriteria.IResultsCriteriaBase criteria = ResultsCriteria.ResultsCriteriaBuilder.BuildCriteria(unode.Clone());
                                tabRes4.Criterias.Add(criteria);
                            }
                        }
                    }
                }
                //Else append the values individually
                else
                {
                    if (isStructureColumn && !node.Attributes["alias"].Value.ToLower().Contains("mol wt") && !node.Attributes["alias"].Value.ToLower().Contains("mol formula") && arrfieldid.Contains(node.Attributes["id"].Value))
                    {
                        tabRes4.Criterias.Add(BuildResultCriteriafield(node.Clone()));
                    }
                    if (isStructureColumn && node.Attributes["alias"].Value.ToLower().Contains("mol wt") && arrfieldid.Contains(node.Attributes["id"].Value) && aliasnames.Contains(node.Attributes["alias"].Value))
                    {
                        ResultsCriteria.MolWeight mwCriteria = new ResultsCriteria.MolWeight();
                        mwCriteria.Alias = node.Attributes["alias"].Value;
                        mwCriteria.Id = Convert.ToInt32(node.Attributes["id"].Value);
                        mwCriteria.Visible = true;
                        tabRes4.Criterias.Add(mwCriteria);
                    }
                    if (isStructureColumn && node.Attributes["alias"].Value.ToLower().Contains("mol formula") && arrfieldid.Contains(node.Attributes["id"].Value) && aliasnames.Contains(node.Attributes["alias"].Value))
                    {
                        ResultsCriteria.Formula formulaCriteria = new ResultsCriteria.Formula();
                        formulaCriteria.Alias = node.Attributes["alias"].Value;
                        formulaCriteria.Id = Convert.ToInt32(node.Attributes["id"].Value);
                        formulaCriteria.Visible = true;
                        tabRes4.Criterias.Add(formulaCriteria);
                    }
                }
                #endregion
            }
            template4.Add(tabRes4);
            template2.Add(tabRes4);

            //Removing Empty tables else bug raised while exporting the data
            if (tabRes4.Criterias.Count <= 0)
                template2 = new ResultsCriteria();

            return template2.Clone();
        }

        private ResultsCriteria BaseTableCheck(XmlNode tnode, ArrayList arrfieldid, ArrayList aliasnames)
        {
            ResultsCriteria template2 = new ResultsCriteria();
            ResultsCriteria.ResultsCriteriaTable tabRes4 = new ResultsCriteria.ResultsCriteriaTable();
            tabRes4.Id = Convert.ToInt32(tnode.Attributes["id"].Value);

            foreach (XmlNode node in tnode.ChildNodes)
            {
                if (arrfieldid.Contains(node.Attributes["id"].Value) && !node.Attributes["alias"].Value.ToLower().Contains("mol wt") && !node.Attributes["alias"].Value.ToLower().Contains("mol formula"))
                {
                    // Check the visible attr otherwise it might add the wrong fields with same ID's to resultcriteria
                    // In case of Structure, Mol Weight and Formula
                    if (node.Attributes["visible"].Value == "1")
                    {
                        tabRes4.Criterias.Add(BuildResultCriteriafield(node.Clone(), arrfieldid, aliasnames));
                    }
                }

                #region Loop for SQLFunction

                if (node.Attributes["alias"].Value.ToLower().Contains("mol wt") && arrfieldid.Contains(node.Attributes["id"].Value) && aliasnames.Contains(node.Attributes["alias"].Value))
                {
                    // Check the visible attr otherwise it might add the wrong fields with same ID's to resultcriteria
                    // In case of Structure, Mol Weight and Formula
                    if (node.Attributes["visible"].Value == "1")
                    {
                        tabRes4.Criterias.Add(BuildResultCriteriafield(node.Clone()));
                    }
                }
                if (node.Attributes["alias"].Value.ToLower().Contains("mol formula") && arrfieldid.Contains(node.Attributes["id"].Value) && aliasnames.Contains(node.Attributes["alias"].Value))
                {
                    // Check the visible attr otherwise it might add the wrong fields with same ID's to resultcriteria
                    // In case of Structure, Mol Weight and Formula
                    if (node.Attributes["visible"].Value == "1")
                    {
                        tabRes4.Criterias.Add(BuildResultCriteriafield(node.Clone()));
                    }
                }
                #endregion
            }
            template2.Add(tabRes4);
            return template2.Clone();
        }

        #endregion

        private void ValidationControls()
        {
            if (!Page.ClientScript.IsClientScriptBlockRegistered("Validationfunction"))
            {
                string script;
                script = @"function fnUpdate(obj){ 
                        var TemplateName = document.getElementById('" + TemplateName.ClientID + @"');
                        var TemplateDesc = document.getElementById('" + TemplateDescription.ClientID + @"');
                        if(TemplateName == null || TemplateName.value =='')
                        {
                            alert('Please enter Template Name');
                            return false;
                        }
                        if(TemplateDesc == null || TemplateDesc.value ==''){
                            alert('Please enter Template Description');
                            return false;
                        }
                        }";

                this.Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Validationfunction", script, true);
            }
        }

        private void RegisteringScripts()
        {
            if (!Page.ClientScript.IsClientScriptBlockRegistered("SelectUnselectAll"))
            {
                string script1;
                script1 = @"function selectAll(mainCheckboxID,gridViewColumnID,fieldCount)
                           {       
                                var mainCheckbox = document.getElementById(mainCheckboxID);       
                                var id=mainCheckboxID.replace('CheckRows','');               
                                var i=2;
                                var subCheckbox;
                                // the hard coded values used here are never going to be change
                                for(i=2;i<=fieldCount+1;i++)
                                {         
                                    if(i<10)
                                    {
                                        var id1 = 'GridViewColumn_ctl0' + i + '_checkvisibile';
                                    }
                                    else
                                    {
                                        var id1 = 'GridViewColumn_ctl' + i + '_checkvisibile';
                                    }
                                    id = id + id1;         
                                    subCheckbox = document.getElementById(id);
                                    if(mainCheckbox.checked)
                                    {
                                        subCheckbox.checked = true ;
                                    }
                                    else
                                    {
                                        subCheckbox.checked = false ;
                                    }  
                                    id = id.replace(id1,'');         
                                }       
                            }";

                this.Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "SelectUnselectAll", script1, true);
            }

            if (!Page.ClientScript.IsClientScriptBlockRegistered("FlipVisibilityFunction"))
            {
                string script2;
                script2 = @"function flipVisibility(obj,name)
                           {    
                             var expcontrolDiv = document.getElementById(obj.id.replace(name,'ExportControlDiv'));
                             var hdnvisibility = document.getElementById(obj.id.replace(name,'expcontrolvisibility'));
    
                             if(expcontrolDiv.style.display == 'block')
                             {
                                expcontrolDiv.style.display = 'none';
                                hdnvisibility.value = 'false';
                                ShowChemDraws();
                             }
                             else if(expcontrolDiv.style.display == 'none')
                             {
                                expcontrolDiv.style.display = 'block';
                                hdnvisibility.value = 'true';
                                HideChemDraws();
                             }    
                             return false;
                           }";

                this.Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "FlipVisibilityFunction", script2, true);
            }
        }
    }

    public class ExportControlEventArgs : EventArgs
    {
        public ResultsCriteria outputRes;
        public int Dataviewid;
        public string ExportFormat;
        /// <summary>
        /// Output parameters to be passed
        /// </summary>
        /// <param name="id">Dataview id </param>
        /// <param name="res">Output ResultsCriteria for Export</param>
        /// <param name="expformat">Format Type</param>
        public ExportControlEventArgs(int id, ResultsCriteria res, string expformat)
        {
            outputRes = res;
            Dataviewid = id;
            ExportFormat = expformat;

        }
    }

    #endregion

    #region Grid view ITemplate Class
    //A customized class for displaying the Template Column
    public class GridViewTemplate : ITemplate
    {
        #region Variables
        //A variable to hold the type of ListItemType.
        ListItemType _templateType;

        //A variable to hold the column name.
        string _columnName;
        string indexType;
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEFormGenerator");

        //Constructor where we define the template type and column name.
        public GridViewTemplate(ListItemType type, string colname)
        {
            //Stores the template type.
            _templateType = type;

            //Stores the column name.
            _columnName = colname;
        }
        #endregion

        #region ITemplate container
        void ITemplate.InstantiateIn(System.Web.UI.Control container)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            switch (_templateType)
            {
                case ListItemType.Header:
                    //Creates a new label control and add it to the container.
                    Label lbl = new Label();            //Allocates the new label object.
                    lbl.ID = "lbltempID2";
                    lbl.Text = _columnName;             //Assigns the name of the column in the lable.
                    lbl.Font.Size = new FontUnit(8);
                    lbl.DataBinding += new EventHandler(lbl_DataBinding);
                    container.Controls.Add(lbl);        //Adds the newly created label control to the container.
                    break;

                case ListItemType.Item:

                    CheckBox chk = new CheckBox();
                    chk.ID = "checkvisibile";
                    chk.DataBinding += new EventHandler(chk_DataBinding);
                    container.Controls.Add(chk);

                    HiddenField hdn = new HiddenField();
                    hdn.ID = "hiddenvisibile";
                    hdn.DataBinding += new EventHandler(hdn_DataBinding);
                    container.Controls.Add(hdn);

                    break;

                case ListItemType.EditItem:
                    TextBox tb1 = new TextBox();
                    tb1.ID = "txtailasgrdvalue";
                    tb1.DataBinding += new EventHandler(tb1_DataBinding);
                    tb1.Columns = 20;
                    container.Controls.Add(tb1);

                    break;


                case ListItemType.Footer:

                    ImageButton ibtnExpand = new ImageButton();
                    ibtnExpand.AlternateText = "Expand";
                    ibtnExpand.ID = "ExpandGrid";
                    container.Controls.Add(ibtnExpand);


                    Label lblspace = new Label();
                    lblspace.ID = "lblnewspace";
                    lblspace.Text = "    ";
                    lblspace.Width = new Unit(5);
                    container.Controls.Add(lblspace);

                    _columnName = "alias";
                    lbl = new Label();
                    lbl.ID = "lbltempID";
                    lbl.DataBinding += new EventHandler(lbl_DataBinding);
                    lbl.Style.Add("valign", "top");
                    lbl.Width = new Unit(369);
                    lbl.Font.Bold = true;
                    lbl.Font.Size = new FontUnit(10);
                    lbl.ForeColor = System.Drawing.Color.Black;
                    container.Controls.Add(lbl);

                    CheckBox tableCheckBox = new CheckBox();
                    tableCheckBox.ID = "CheckRows";
                    tableCheckBox.Checked = false;
                    tableCheckBox.Width = new Unit(10);
                    container.Controls.Add(tableCheckBox);

                    Panel pangrd = new Panel();
                    pangrd.ID = "GridPanel";
                    GridView grdfield = new GridView();
                    grdfield.ID = "GridViewColumn";
                    grdfield.AutoGenerateColumns = false;
                    grdfield.EmptyDataText = "-NA-";
                    grdfield.BackColor = System.Drawing.Color.White;
                    grdfield.BorderColor = System.Drawing.Color.Navy;
                    grdfield.AlternatingRowStyle.BackColor = System.Drawing.Color.FromArgb(225, 225, 225);
                    grdfield.RowStyle.BackColor = System.Drawing.Color.White;
                    grdfield.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(191, 191, 191);
                    grdfield.HeaderStyle.ForeColor = System.Drawing.Color.Navy;
                    grdfield.Width = 465;

                    pangrd.Controls.Add(grdfield);
                    container.Controls.Add(pangrd);


                    break;


            }
            _coeLog.LogEnd(methodSignature);
        }

        #endregion

        #region Events
        /// <summary>
        /// his is the event, which will be raised when Label binding happens.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lbl_DataBinding(object sender, EventArgs e)
        {
            Label lbldata = (Label)sender;
            GridViewRow container = (GridViewRow)lbldata.NamingContainer;
            object dataValue = DataBinder.Eval(container.DataItem, _columnName);
            if (dataValue != null && dataValue != Convert.DBNull)
                lbldata.Text = dataValue.ToString();
            else
                lbldata.Text = string.Empty;
        }

        /// <summary>
        /// This is the event, which will be raised when Grid View binding happens.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void grdfield_DataBinding(object sender, EventArgs e)
        {
            GridView txtdata = (GridView)sender;
            GridViewRow container = (GridViewRow)txtdata.NamingContainer;
            object dataValue = DataBinder.Eval(container.DataItem, _columnName);

        }

        /// <summary>
        /// This is the event, which will be raised when Textbox binding happens.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tb1_DataBinding(object sender, EventArgs e)
        {
            _columnName = "alias";
            TextBox txtdata = (TextBox)sender;
            GridViewRow container = (GridViewRow)txtdata.NamingContainer;
            object dataValue = DataBinder.Eval(container.DataItem, _columnName);

            if (dataValue != null && dataValue != Convert.DBNull)
            {
                txtdata.Text = dataValue.ToString();
                //if (dataValue.ToString().ToLower().Contains("structure") || dataValue.ToString().ToLower().Contains("mol wt") || dataValue.ToString().ToLower().Contains("mol formula"))
                //Coverity fix
                string _indexType = string.Empty;
                string _mimeType = string.Empty;
                object indexType = DataBinder.Eval(container.DataItem, "indexType");
                object mimeType = DataBinder.Eval(container.DataItem, "mimeType");
                if (indexType != null)
                {
                    _indexType = indexType.ToString();
                }
                if (mimeType != null)
                {
                    _mimeType = mimeType.ToString();
                }
                //if ((DataBinder.Eval(container.DataItem, "indexType") != null && DataBinder.Eval(container.DataItem, "indexType").ToString() == "CS_CARTRIDGE") || (DataBinder.Eval(container.DataItem, "mimeType") != null && DataBinder.Eval(container.DataItem, "mimeType").ToString() == "CHEMICAL_X_CDX"))
                if (_indexType == "CS_CARTRIDGE" || _mimeType == "CHEMICAL_X_CDX")
                {
                    txtdata.Enabled = false;
                }
            }

        }


        /// <summary>
        /// This is the event, which will be raised when Checkbox binding happens.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void chk_DataBinding(object sender, EventArgs e)
        {
            _columnName = "visible";
            indexType = "indexType";
            CheckBox txtdata = (CheckBox)sender;
            GridViewRow container = (GridViewRow)txtdata.NamingContainer;
            if (((System.Data.DataRowView)(container.DataItem)).Row.Table.Columns["visible"] != null)
            {
                // checkbox should always be unchecked, Do not check for visible attrib to check and uncheck 
                object dataValue = DataBinder.Eval(container.DataItem, _columnName);
                if (dataValue != null && dataValue != Convert.DBNull)
                {
                    if (ExportControl.currentTemplate != "Default Template")
                        txtdata.Checked = Convert.ToBoolean(dataValue);
                    else
                        if (((System.Data.DataRowView)(container.DataItem)).Row.Table.Columns["indexType"] != null)
                        {
                            object dValue = DataBinder.Eval(container.DataItem, indexType);
                            if (dValue != null && dValue != Convert.DBNull)
                            {
                                if (dValue.ToString() == "CS_CARTRIDGE")
                                    txtdata.Checked = Convert.ToBoolean("true");
                            }
                            else
                            {
                                txtdata.Checked = Convert.ToBoolean("false");
                            }
                        }
                }
                else
                    txtdata.Checked = false; //absence of visible attribute means it is not visible.
            }
            else
            {
                txtdata.Checked = false;
            }
        }

        void hdn_DataBinding(object sender, EventArgs e)
        {
            _columnName = "id";
            HiddenField hdndata = (HiddenField)sender;
            GridViewRow container = (GridViewRow)hdndata.NamingContainer;
            object dataValue = DataBinder.Eval(container.DataItem, _columnName);
            if (dataValue != null && dataValue != Convert.DBNull)
                hdndata.Value = dataValue.ToString();
            else
                hdndata.Value = string.Empty;
        }
        #endregion
    }
    #endregion

}
