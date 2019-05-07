using CambridgeSoft.COE.Framework.COEReportingService;
using CambridgeSoft.COE.Framework.COEReportingService.Builders;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Messaging;
using Csla;
using DevExpress.XtraReports.UI;
using System.Data;
using System.Xml;
namespace CambridgeSoft.COE.Framework.ServerControls.Reporting
{
    /// <summary>
    /// Report Viewer user control controller class.  It interacts with Reporting service in order to obtain the report instances.
    /// This class, along with IReportViewer interface are meant to centralize common functionality required when coding a report viewer user control. 
    /// Sample Web and WinForms version viewers take advantage of this class.
    /// </summary>
    internal class ReportViewerController
    {
        #region Variables
        private IReportViewer _reportViewer;
        private ReportBuilderBase _reportBuilder = null;
        private ReportBuilderMeta _reportBuilderMeta = null;
        private int _hitListId = -1;
        private PagingInfo _pagingInfo;
        private XtraReport _report;
        private object _dataSource = null;
        #endregion

        #region Properties
        /// <summary>
        /// Specifies a report builder to be used for obtaining the report instance to be displayed. This property is mutually exclusive with ReportBuidlerMeta property and ReportId properties.
        /// </summary>
        public ReportBuilderBase ReportBuilder
        {
            get { return _reportBuilder; }
            set
            {
                _reportBuilder = value;

                //Wether one or the other, not both
                if (value != null)
                    ReportBuilderMeta = null;

                _report = null;
            }
        }

        /// <summary>
        /// Specifies information about a report builder to be used for obtaining the report instance to be displayed. This property is mutually exclusive with ReportBuilder and ReportId properties.
        /// </summary>
        public ReportBuilderMeta ReportBuilderMeta
        {
            get { return _reportBuilderMeta; }
            set
            {
                _reportBuilderMeta = value;

                //Wether one or the other, not both
                if (value != null)
                    ReportBuilder = null;

                _report = null;
            }
        }

        /// <summary>
        /// Specifies the id of the report (definition) to be used for generating a report instance to be displayed. This property is mutually exclusive with ReportBuilder and ReportBuilderMeta properties.
        /// </summary>
        public int ReportId
        {
            get
            {
                return _reportBuilderMeta == null ? 0 : _reportBuilderMeta.Id;
            }
            set
            {
                if (value >= 0)
                {
                    _reportBuilder = null;
                    _reportBuilderMeta = new ReportBuilderMeta(value, "", "", typeof(DataBaseReportBuilder).AssemblyQualifiedName);

                    _report = null;
                }
            }
        }


        /// <summary>
        /// Specifies the id of a hitlist representing the data that is meant to  be bound to the report instance to be displayed. This involves interaction with search service. This property is mutually exclusive with DataSource property
        /// </summary>
        public int HitListId
        {
            get
            {
                return _hitListId;
            }
            set
            {
                if (_hitListId != value)
                {
                    _hitListId = value;

                    if (value != null)
                        _dataSource = null;


                    _report = null;
                }
            }
        }
        public PagingInfo PagingInfo
        {
            get 
            {
                return _pagingInfo;
            }
            set 
            { 
                if(_pagingInfo == null || !CompareByteArray(ComputeXMLMD5(_pagingInfo.ToString()), ComputeXMLMD5(value.ToString())))
                {
                    _pagingInfo = value;
                    _dataSource = null;
                    _report = null;
                }
            }
        }

        /// <summary>
        /// Specifies the datasource to be bound to the report instance to be displayed. This propety is mutually exclusive with HitlistId property
        /// </summary>
        public object DataSource
        {
            get
            {
                return _dataSource;
            }
            set
            {
                if (DataSource == null || (DataSource != null && !DataSource.Equals(value)))
                {
                    _dataSource = value;
                    if (value != null)
                        _hitListId = -1;

                    _report = null;
                }
            }
        }

        private bool UseReportBuilderMeta
        {
            get
            {
                return ReportBuilderMeta != null && ReportBuilderMeta.Id > 0;
            }
        }

        /// <summary>
        /// True if the current report instance is read-only (this is the case of a system generated report (definition)). False otherwise.
        /// </summary>
        public bool IsReportReadOnly
        {
            get
            {
                if (ReportBuilderMeta != null)
                    return ReportBuilderMeta.ReadOnly;

                return true;
            }
        }

        /// <summary>
        /// True if the current information on the controller is enough for displaying a report. False otherwise.
        /// </summary>
        public bool ContainsReportToShow
        {
            get
            {
                return (ReportBuilderMeta != null && ReportBuilderMeta.Id > 0) || ReportBuilder != null;
            }
        }

        #endregion

        #region constructors
        public ReportViewerController(IReportViewer viewer)
        {
            _reportViewer = viewer;
        }
        #endregion

        #region Methods
        #region Public Methods
        /// <summary>
        /// Removes the current report instance loaded.
        /// </summary>
        public void Clear()
        {
            _report = null;
            this._reportBuilder = null;
            this._reportBuilderMeta = null;

            DisplayReport();
        }

        /// <summary>
        /// Interacts with COEReporting service for retrieving the report instance to display, based upon report definition and data properties.
        /// </summary>
        /// <returns></returns>
        public XtraReport GetReportInstance()
        {
            try
            {
                if (_report == null)
                {
                    if (this.UseReportBuilderMeta)
                    {
                        if (DataSource != null)
                            _report = COEReporting.GetReportInstance(ReportBuilderMeta, DataSource);
                        else
                            _report = COEReporting.GetReportInstance(ReportBuilderMeta, HitListId, PagingInfo);
                    }
                    else
                    {
                        if (DataSource != null)
                            _report = COEReporting.GetReportInstance(ReportBuilder, DataSource);
                        else
                            _report = COEReporting.GetReportInstance(ReportBuilder, HitListId, PagingInfo);

                    }
                }
                else
                    if (_report is COEXtraReport)
                        ((COEXtraReport)_report).PreProcess();

                return _report;
            }
            catch (DataPortalException exception)
            {
                throw (exception.BusinessException == null ? exception : exception.BusinessException);
            }
        }

        /// <summary>
        /// Interacts with COEReporting service for Retrieving the report (definition), based upon definition and data properties.
        /// </summary>
        /// <returns></returns>
        public COEReport GetReportDefinition()
        {
            try
            {
                if (this.UseReportBuilderMeta)
                    return COEReporting.GetReportDefinition(ReportBuilderMeta);

                return COEReporting.GetReportDefinition(ReportBuilder);
            }
            catch (DataPortalException exception)
            {
                throw (exception.BusinessException == null ? exception : exception.BusinessException);
            }
        }

        /// <summary>
        /// Instructs the IReportViewer related instance to show the current report instance.
        /// </summary>
        public void DisplayReport()
        {
            if (this.ContainsReportToShow)
                _reportViewer.ShowReport(GetReportInstance(), this.IsReportReadOnly);
            else
                _reportViewer.ShowReport(null, true);
        }
        #endregion

        #region Private Methods
        private bool CompareByteArray(byte[] operand1, byte[] operand2)
        {
            //if both are the same reference or null return true.
            if (operand1 == operand2)
                return true;

            if (operand1 == null || operand2 == null)
                return false;

            if (operand1.Length == operand2.Length)
                for (int index = 0; index < operand1.Length; index++)
                    if (operand1[index] != operand2[index])
                        return false;

            return true;
        }

        private byte[] ComputeXMLMD5(string value)
        {
            if (value != null)
            {
                // "Normalize" the xml.
                XmlDocument document = new XmlDocument();
                document.LoadXml(value);

                return CambridgeSoft.COE.Framework.Common.Utilities.ComputeHash(document.OuterXml);
            }
            return null;
        }
        #endregion
        #endregion
    }
}
