using System;
using System.Xml.Serialization;
using System.Xml;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.Common
{
    [Serializable]
    [XmlRoot("COEReport", Namespace = "COE.Reporting")]
    public class COEReport
    {
        #region Attributes
        int _hitListId;
        int _dataviewId;
        COEDataView _dataview;
        ResultsCriteria _resultsCriteria;
        SearchCriteria _searchCriteria;
        string _reportLayout;
        string _category;
        int _id;
        #endregion

        #region Properties

        [XmlAttribute("ID")]
        public int ID 
        {
            get { return _id; }
            set { _id = value; }
        }

        [XmlAttribute("DataviewId")]
        public int DataViewId
        {
            get { return _dataviewId; }
            set { _dataviewId = value; }
        }

        [XmlAttribute("HitListId")]
        public int HitListId
        {
            get { return _hitListId; }
            set { _hitListId = value; }
        }

        [XmlIgnore()]
        public COEDataView Dataview
        {
            get
            {
                return _dataview;
            }
            set
            {
                _dataview = value;
            }
        }

        [XmlElement("ResultsCriteria")]
        public ResultsCriteria ResultsCriteria
        {
            get
            {
                if (_resultsCriteria == null)
                    _resultsCriteria = new ResultsCriteria();

                return _resultsCriteria;
            }
            set
            {
                _resultsCriteria = value;
            }
        }

        [XmlElement("SearchCriteria")]
        public SearchCriteria SearchCriteria
        {
            get
            {
                if (_searchCriteria == null)
                    _searchCriteria = new SearchCriteria();
                return _searchCriteria;
            }
            set
            {
                _searchCriteria = value;
            }
        }

        [XmlElement("ReportLayout")]
        public string ReportLayout
        {
            get
            {
                return _reportLayout ;
            }
            set
            {
                _reportLayout = value.Trim().Replace("\0", "");
            }
        }

        [XmlAttribute("Category")]
        public string Category
        {
            get 
            {
                return _category;
            }
            set 
            {
                _category = value;
            }
        }

        [XmlIgnore]
        public bool usesSearchManager
        {
            get
            {
                return _hitListId > 0 || _dataviewId > 0;
            }
        }

        #endregion

        #region Methods

        public COEReport()
        {
        }

        public COEReport(COEDataView dataview, ResultsCriteria resultsCriteria, SearchCriteria searchCriteria)
        {
            _dataview = dataview;
            _dataviewId = dataview.DataViewID;

            if (resultsCriteria == null)
                this.ExtractResultsCriteria();
            else
                _resultsCriteria = resultsCriteria;

            _searchCriteria = searchCriteria;
            _reportLayout = string.Empty;
        }

        public void ExtractResultsCriteria()
        {
            CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView metaDataDataView = new CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView(this.Dataview.ToString());

            this.ResultsCriteria = new ResultsCriteria();

            foreach (COEDataView.DataViewTable currentTable in this.Dataview.Tables)
            {
                if (currentTable.Id == this.Dataview.Basetable || metaDataDataView.GetRelations(this.Dataview.Basetable, currentTable.Id).Count == 1)
                {
                    ResultsCriteria.ResultsCriteriaTable resultsCriteriaTable = new ResultsCriteria.ResultsCriteriaTable(currentTable.Id);

                    foreach (COEDataView.Field currentField in currentTable.Fields)
                    {
                        ResultsCriteria.Field resultsCriteriaField = new ResultsCriteria.Field(currentField.Id);
                        resultsCriteriaField.Alias = currentField.Alias;

                        resultsCriteriaTable.Criterias.Add(resultsCriteriaField);

                    }
                    this.ResultsCriteria.Tables.Add(resultsCriteriaTable);
                }
            }
        }

        #endregion

        #region Factory Methods
        /// <summary>
        /// Initializes a new COEReport from its xml string representation.
        /// </summary>
        /// <param name="xml">The xml as string.</param>
        /// <returns>An instance of COEReportTemplateTemplate.</returns>
        public static COEReport GetCOEReport(string xml)
        {
            return Utilities.XmlDeserialize<COEReport>(xml);
        }

        /// <summary>
        /// Builds an xml string representation of the instance.
        /// </summary>
        /// <returns>The xml representation of the instance as string.</returns>
        public override string ToString()
        {
            return Utilities.XmlSerialize(this);
        }

        public static COEReport LoadFromXml(string fileName) 
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(fileName);
            }
            catch 
            {
                throw new Exception(Resources.ReadingXmlFile_ErrorMessage);
            }

            try
            {
                return GetCOEReport(xmlDoc.OuterXml);
            }
            catch 
            {
                throw new Exception(Resources.LoadReportTemplateXml_ErrorMessage);
            }
        }
        #endregion
    }
}
