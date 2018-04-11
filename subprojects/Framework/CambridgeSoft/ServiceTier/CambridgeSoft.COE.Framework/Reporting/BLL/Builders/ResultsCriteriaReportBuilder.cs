using CambridgeSoft.COE.Framework.COEReportingService;
using CambridgeSoft.COE.Framework.COEReportingService.Builders;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Controls.Reporting;
using DevExpress.XtraReports.UI;
using System.Text;
using System.ComponentModel;
using Csla;

namespace CambridgeSoft.COE.Framework.COEReportingService.Builders
{
    /// <summary>
    /// Creates a Report based upon a provided resultsCriteria and a dataview.
    /// </summary>
    public class ResultsCriteriaReportBuilder : ReportBuilderBase
    {
        #region Variables
        private int _dataViewId;
        private ResultsCriteria _resultsCriteria;
        private int _baseReportTemplateId;
        private COEReport _reportTemplate;
        #endregion

        #region Properties
        public int DataViewId
        {
            get 
            {
                return _dataViewId;
            }
            set 
            {
                _dataViewId = value;
            }
        }

        [TypeConverterAttribute(typeof(ResultsCriteriaTypeConverter))]
        public ResultsCriteria ResultsCriteria
        {
            get 
            {
                return this._resultsCriteria;
            }
            set 
            {
                this._resultsCriteria = value;
            }
        }

        public int BaseReportTemplateId
        {
            get 
            {
                return _baseReportTemplateId;
            }
            set 
            {
                _baseReportTemplateId = value;
            }
        }

        #endregion

        #region Methods
        #region Constructors
        public ResultsCriteriaReportBuilder()
        { 
        }

        public ResultsCriteriaReportBuilder(int dataviewId, ResultsCriteria resultsCriteria, int baseTemplateId)
        {
            _dataViewId = dataviewId;
            _resultsCriteria = resultsCriteria;
            _baseReportTemplateId = baseTemplateId;
        }
        #endregion

        #region TemplateBuilderBase implementation
        public override CambridgeSoft.COE.Framework.Common.COEReport GetReport()
        {
            _reportTemplate = new COEReport();

            XtraReport report;
            try
            {
                report = COEReporting.GetReportInstance(_baseReportTemplateId);
                // Coverity Fix CID - 10839 (from local server)           
                if (report == null)
                    report = new COEXtraReport();
            }
            catch (DataPortalException)
            {
                report = new COEXtraReport();
            }

            if (_resultsCriteria == null)
                throw new System.Exception("You should provide a resultsCriteria");

            _reportTemplate.ResultsCriteria = _resultsCriteria;
            _reportTemplate.DataViewId = _dataViewId;

            COEReporting.NormalizeReport(_reportTemplate);

            foreach (ResultsCriteria.ResultsCriteriaTable currentTable in this._resultsCriteria.Tables)
            {
                BandCollection parentBandCollection = null;
                float tableMargin = 0.0f;

                if (_reportTemplate.Dataview.Basetable == currentTable.Id)
                {
                    parentBandCollection = report.Bands;
                    report.DataMember = string.Format("Table_{0}", currentTable.Id);
                    tableMargin = 0.0f;
                    
                }
                else
                {
                    DetailReportBand detailReportBand = new DetailReportBand();
                    detailReportBand.DataMember = detailReportBand.Name = string.Format("Table_{0}", currentTable.Id);
                    report.Bands.Add(detailReportBand);
                    parentBandCollection = detailReportBand.Bands;
                    tableMargin = 100.0f;
                }


                float tableWidth = report.PrintingSystem.PageSettings.UsablePageSize.Width - tableMargin;

                BuildResultsCriteriaTableDetail(currentTable, parentBandCollection, tableWidth, tableMargin);
                BuildResultsCriteriaTableHeaderGroup(currentTable, parentBandCollection, tableWidth, tableMargin);
            }

            _reportTemplate.ReportLayout = COEReporting.GetReportLayout(report);

            // Fix Coverity: CID-28969 Resource Leak
            report.Dispose();

            return _reportTemplate;
        }

        private void BuildResultsCriteriaTableHeaderGroup(ResultsCriteria.ResultsCriteriaTable currentTable, BandCollection parentBandCollection, float tableWidth, float tableMargin)
        {
            XRLabel label = new XRLabel();
            label.Text = _reportTemplate.Dataview.Tables.getById(currentTable.Id).Alias;

            XRTable tableHeader = this.BuildTableHeader(currentTable);
            tableHeader.WidthF = tableWidth;
            tableHeader.LeftF = tableMargin;
            tableHeader.TopF = label.HeightF;

            //Coverity Fixes: CBOE-194 : CID-11735
            Band band = parentBandCollection.GetBandByType(typeof(GroupHeaderBand));
            if (band == null)
                parentBandCollection.Add(new GroupHeaderBand());

            GroupHeaderBand groupHeader = (GroupHeaderBand)band;
            if (groupHeader != null)
            {
                groupHeader.HeightF = tableHeader.HeightF;
                groupHeader.Controls.Add(label);
                groupHeader.Controls.Add(tableHeader);
            }
        }

        private void BuildResultsCriteriaTableDetail(ResultsCriteria.ResultsCriteriaTable currentTable, BandCollection parentBandCollection, float tableWidth, float tableMargin)
        {
            XRTable tableBody = this.BuildTableBody(currentTable);
            tableBody.WidthF = tableWidth;
            tableBody.LeftF = tableMargin;

            //Coverity Fixes: CBOE-194 : CID-11734
            Band band = parentBandCollection.GetBandByType(typeof(DetailBand));
            if (band == null)
                parentBandCollection.Add(new DetailBand());

            DetailBand detailBand = (DetailBand)band;
            if (detailBand != null)
            {
                detailBand.Controls.Add(tableBody);
                detailBand.HeightF = tableBody.HeightF;
            }
        }

        private XRTable BuildTableBody(ResultsCriteria.ResultsCriteriaTable currentTable)
        {
            XRTable table = new XRTable();
            table.Name = string.Format("Table_{0}XRTableHeader", currentTable.Id);
            table.AnchorVertical = VerticalAnchorStyles.Top;
            table.CanGrow = true;

            XRTableRow tableRow = new XRTableRow();
            tableRow.Name = string.Format("Table_{0}TableHeaderRow", currentTable.Id);
            tableRow.CanGrow = true;

            table.Controls.Add(tableRow);

            foreach (ResultsCriteria.IResultsCriteriaBase currentCriteria in currentTable.Criterias)
            {
                XRTableCell tableCell = this.BuildTableCell(currentTable, currentCriteria);

                tableRow.Controls.Add(tableCell);
            }

            return table;
        }


        private XRTable BuildTableHeader(ResultsCriteria.ResultsCriteriaTable currentTable)
        {
            XRTable table = new XRTable();
            table.Name = string.Format("Table_{0}XRTableBody", currentTable.Id);
            table.CanGrow = true;

            XRTableRow row = new XRTableRow();
            row.Name = string.Format("Table_{0}TableRow", currentTable.Id);
            row.CanGrow = true;

            table.Controls.Add(row);

            foreach (ResultsCriteria.IResultsCriteriaBase currentCriteria in currentTable.Criterias)
            {
                XRTableCell tableCell = new XRTableCell();
                tableCell.CanGrow = true;
                tableCell.Text = this.NormalizeHeaderName(currentCriteria.Alias);
                tableCell.Name = currentCriteria.Alias + "XRCell";

                row.Controls.Add(tableCell);
            }

            return table;
        }

        private string NormalizeHeaderName(string headerName)
        {
            StringBuilder builder = new StringBuilder();
            //return headerName.Replace("_", " ").ToLowerInvariant();

            string[] tokens = headerName.ToLower().Replace("_", " ").Split(new char[] { ' ' });

            foreach (string currentToken in tokens)
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                    builder.Append(" ");

                builder.Append(currentToken.Substring(0, 1).ToUpper());
                builder.Append(currentToken.Substring(1, currentToken.Length - 1));
            }

            return builder.ToString();
        }

        private XRTableCell BuildTableCell(ResultsCriteria.ResultsCriteriaTable currentTable, ResultsCriteria.IResultsCriteriaBase currentCriteria)
        {
            XRTableCell tableCell = new XRTableCell();
            tableCell.CanGrow = true;

            string bindingExpression = string.Format("Table_{0}.{1}", currentTable.Id, currentCriteria.Alias);
            // Coverity Fix CID - 10495 (from local server)
            COEDataView.DataViewTable currentDataViewTable = _reportTemplate.Dataview.Tables.getById(currentTable.Id);
            if (currentDataViewTable != null)
            {
                COEDataView.Field currentField = currentDataViewTable.Fields.getById(((ResultsCriteria.Field)currentCriteria).Id);

                if (currentField != null && currentCriteria is ResultsCriteria.Field &&  currentField.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE)
                {
                    XRChemDrawEmbed xrChemDrawEmbed = new XRChemDrawEmbed();
                    /*xrChemDrawEmbed.HeightF = tableCell.HeightF;
                    xrChemDrawEmbed.WidthF = tableCell.WidthF;*/
                    xrChemDrawEmbed.AnchorVertical = VerticalAnchorStyles.Both;
                    xrChemDrawEmbed.CanGrow = true;
                    xrChemDrawEmbed.CanShrink = true;
                    xrChemDrawEmbed.DataBindings.Add(new XRBinding("InlineData", null, bindingExpression));
                    tableCell.Controls.Add(xrChemDrawEmbed);
                }
                else
                {
                    tableCell.DataBindings.Add("Text", null, bindingExpression);
                    tableCell.Name = currentCriteria.Alias + "XRCell";
                }
            }
            return tableCell;
        }
        #endregion
        #endregion
    }
}
