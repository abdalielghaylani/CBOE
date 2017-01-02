using System.Drawing;
using System.Web.UI.WebControls;
using System.Xml;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.Controls.Reporting;
using DevExpress.XtraReports.UI;

namespace CambridgeSoft.COE.Framework.COEReportingService.Builders
{
    /// <summary>
    /// Creates a (very basic and ugly) report from a coeForm List view. This is only meant as a sample for ReportBuilders and system generated reports.
    /// </summary>
    public class ListViewReportBuilder : ReportBuilderBase
    {
        #region Variables
        private int _formGroupId;
        private System.Data.DataSet _dataSet;
        #endregion

        #region Properties
        public int FormGroupId
        {
            get
            {
                return _formGroupId;
            }
            set
            {
                _formGroupId = value;
            }
        }
        #endregion

        #region Constructors
        public ListViewReportBuilder()
        {
        }

        public ListViewReportBuilder(int formGroupId)
            : this()
        {
            _formGroupId = formGroupId;
        }
        #endregion

        #region Methods
        public override CambridgeSoft.COE.Framework.Common.COEReport GetReport()
        {
            COEReport template = new COEReport();

            XtraReport report = COEReporting.GetReportInstance(61);
            COEFormBO theCOEFormBO = COEFormBO.Get(this.FormGroupId);
            // Coverity Fix CID - 10838 (from local server)           
            if (theCOEFormBO != null)
            {
                FormGroup formGroup = theCOEFormBO.COEFormGroup;

                if (formGroup != null)
                {
                    template.ResultsCriteria = ((FormGroup.ListDisplay)formGroup.ListForms[formGroup.ListForms.DefaultForm]).ResultsCriteria;
                    template.DataViewId = formGroup.DataViewId;

                    XmlNode configInfo = null;
                    foreach (FormGroup.Form currentForm in formGroup.ListForms[formGroup.ListForms.DefaultForm].Forms)
                    {
                        foreach (FormGroup.FormElement currentFormElement in currentForm.GetFormElements(FormGroup.DisplayMode.View))
                            if (currentFormElement.DisplayInfo.Type == typeof(COEWebGrid).FullName)
                            {
                                configInfo = currentFormElement.ConfigInfo;
                                break;
                            }
                    }
                    // Coverity Fix CID - 10837 (from local server)           
                    if (configInfo != null)
                        buildDetailReports(report, template.ResultsCriteria, configInfo.OuterXml);
                }
            }
            // Coverity Fix CID - 10838 (from local server)
            if(report != null)
                template.ReportLayout = COEReporting.GetReportLayout(report);

            return template;
        }

        private void buildDetailReports(XtraReport report, ResultsCriteria resultsCriteria, string configInfo)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(configInfo);

            XmlNamespaceManager xmlManager = new XmlNamespaceManager(document.NameTable);
            xmlManager.AddNamespace("COE", document.DocumentElement.NamespaceURI);

            XmlNodeList tableNodes = document.SelectNodes("//COE:table", xmlManager);
            //foreach (XmlNode currentTableNode in tableNodes)
            //{
            XmlNode currentTableNode = tableNodes[0];
                if (currentTableNode.Attributes["name"] != null && !string.IsNullOrEmpty(currentTableNode.Attributes["name"].Value))
                {
                    GroupHeaderBand groupHeaderBand = new GroupHeaderBand();
                    groupHeaderBand.Borders = DevExpress.XtraPrinting.BorderSide.All;
                    groupHeaderBand.BorderColor = Color.Black;
                    groupHeaderBand.BorderWidth = 2;
                    groupHeaderBand.HeightF = 50;
                    report.Bands.Add(groupHeaderBand);
                    groupHeaderBand.Controls.Add(GetTableHeader(currentTableNode, report.PrintingSystem.PageSettings.UsablePageSize.Width, xmlManager));
                    

                    DetailBand detailBand = (DetailBand)report.Bands[BandKind.Detail];
                    detailBand.Borders = DevExpress.XtraPrinting.BorderSide.All;
                    detailBand.BorderColor = Color.Black;
                    detailBand.BorderWidth = 2;
                    report.DataMember = currentTableNode.Attributes["name"].Value;
                    XRTable tableBody = GetTableBody(currentTableNode, report.PrintingSystem.PageSettings.UsablePageSize.Width, xmlManager);
                    detailBand.Controls.Add(tableBody);
                    report.WidthF = detailBand.WidthF = tableBody.WidthF;
                }
            //}
        }

        string allowedTypes = "PlainCell\nCambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEChemDrawEmbedReadOnly\nCambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEStateControl";
        private XRTable GetTableBody(XmlNode tableNode, float width, XmlNamespaceManager xmlManager)
        {
            XRTable listResultsTable = new XRTable();
            listResultsTable.AnchorVertical = VerticalAnchorStyles.Top;
            listResultsTable.SizeF = new System.Drawing.SizeF(width, listResultsTable.HeightF);
            
            XRTableRow tableRow = new XRTableRow();
            tableRow.CanGrow = true;

            listResultsTable.Controls.Add(tableRow);

            string tableName = tableNode.Attributes["name"].Value;
            XmlNodeList columnsList = tableNode.SelectNodes("COE:Columns/COE:Column", xmlManager);
            //Coverity Fix CID 19203
            if (columnsList != null)
            {
                foreach (XmlNode currentColumn in columnsList)
                {
                    string type = "PlainCell";

                    XmlNode xmlNodeType =currentColumn.SelectSingleNode("COE:formElement/COE:displayInfo/COE:type", xmlManager);
                    if (xmlNodeType != null)
                        type = xmlNodeType.InnerText;

                    if (this.allowedTypes.IndexOf(type) >= 0)
                    {
                        XRTableCell tableCell = new XRTableCell();

                        string cellName = tableCell.Name = currentColumn.Attributes["name"].Value;
                        string bindingExpression = string.Format("{0}.{1}", tableName, cellName);

                        XmlNode xmlNodeHeight =  currentColumn.SelectSingleNode("COE:height", xmlManager);
                        if (xmlNodeHeight != null)
                            tableCell.HeightF = GetUnit(xmlNodeHeight.InnerText);

                         XmlNode xmlNodeWidth= currentColumn.SelectSingleNode("COE:width", xmlManager);
                         if (xmlNodeWidth != null)
                            tableCell.WidthF = GetUnit(xmlNodeWidth.InnerText);


                        if (type == typeof(COEChemDrawEmbedReadOnly).FullName)
                        {
                            XRChemDrawEmbed xrChemDrawEmbed = new XRChemDrawEmbed();
                            xrChemDrawEmbed.HeightF = tableCell.HeightF;
                            xrChemDrawEmbed.WidthF = tableCell.WidthF;
                            xrChemDrawEmbed.AnchorVertical = VerticalAnchorStyles.Both;
                            xrChemDrawEmbed.CanGrow = true;
                            xrChemDrawEmbed.DataBindings.Add(new XRBinding("InlineData", null, bindingExpression));
                            tableCell.Controls.Add(xrChemDrawEmbed);
                        }
                        else
                            tableCell.DataBindings.Add(new XRBinding("Text", null, bindingExpression));

                        tableRow.Controls.Add(tableCell);
                    }
                }
            }
            return listResultsTable;
        }

        private float GetUnit(string unitString)
        {
            return (float)Unit.Parse(unitString).Value;

        }

        private XRTable GetTableHeader(XmlNode tableNode, float width, XmlNamespaceManager xmlManager)
        {
            XRTable listResultsTable = new XRTable();
            listResultsTable.AnchorVertical = VerticalAnchorStyles.Top;
            listResultsTable.SizeF = new System.Drawing.SizeF(width, listResultsTable.HeightF);
            listResultsTable.CanGrow = true;

            XRTableRow tableRow = new XRTableRow();
            tableRow.CanGrow = true;

            listResultsTable.Controls.Add(tableRow);

            //string tableName = tableNode.Attributes["name"].Value + "Header";
            XmlNodeList columnsList = tableNode.SelectNodes("COE:Columns/COE:Column", xmlManager);
            if (columnsList != null)
            {
                foreach (XmlNode currentColumn in columnsList)
                {
                    //Coverity Fix CID 19204
                    if (currentColumn != null)
                    {
                        XRTableCell tableCell = new XRTableCell();

                        string cellName = tableCell.Name = currentColumn.Attributes["name"].Value + "Header";
                        string type = "PlainCell";

                        XmlNode typeNode = currentColumn.SelectSingleNode("COE:formElement/COE:displayInfo/COE:type", xmlManager);

                        if (typeNode != null)
                            type = typeNode.InnerText;

                        if (this.allowedTypes.IndexOf(type) >= 0)
                        {
                            XmlNode xmlWidthNode = currentColumn.SelectSingleNode("COE:width", xmlManager);
                            if (xmlWidthNode != null)
                                tableCell.WidthF = GetUnit(xmlWidthNode.InnerText);

                            XmlNode xmlheaderNode = currentColumn.SelectSingleNode("COE:headerText", xmlManager);

                            if (xmlheaderNode != null)
                            {
                                tableCell.Text = xmlheaderNode.InnerText;
                            }
                            else
                            {
                                tableCell.Text = cellName;
                            }

                            tableCell.CanGrow = true;
                            tableRow.Controls.Add(tableCell);
                        }
                    }
                }
            }
            return listResultsTable;
        }
        #endregion
    }
}
