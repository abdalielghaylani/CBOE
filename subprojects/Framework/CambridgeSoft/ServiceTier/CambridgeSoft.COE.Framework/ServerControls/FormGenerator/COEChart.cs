using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraCharts;
using System.Web.UI;
using System.Xml;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using System.Collections;
using System.Data;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// COE Web Control to display charts
    /// </summary>
    /// <summary>
    /// <para>
    /// This class implements a chart control that may be used inside a <see cref="COEFormGenerator"/>.
    /// </para>
    /// <para>
    /// The COEChart class accepts every XtraCharts(DexExpress) property to be set, but as ICOEGenerable control it also provides
    /// GetData(), PutData(), DefaultValue and LoadFromXml() Methods.
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>defaultValue: Value to be displayed by default</item>
    ///         <item>bindingExpression: What is the binding Attribute, relative to the datasource, of the formgenerator.</item>
    ///         <item>label: What is its label?</item>
    ///         <item>configInfo: Additional attributes like CSSClass, Style, Width, Height, ID...</item>
    ///     </list>
    /// </para>
    /// <para>
    /// <b>Example:</b>
    /// </para>
    /// <b>With XML:</b>
    /// <code lang="XML">
    ///                  &lt;formElement name="MatPrices"&gt;
    ///                      &lt;label&gt;Material prices over the time:&lt;/label&gt;
    ///                      &lt;showHelp&gt;false&lt;/showHelp&gt;
    ///                      &lt;isFileUpload&gt;false&lt;/isFileUpload&gt;
    ///                      &lt;pageComunicationProvider /&gt;
    ///                      &lt;fileUploadBindingExpression /&gt;
    ///                     &lt;showMoreDetails&gt;false&lt;/showMoreDetails&gt;
    ///                     &lt;helpText /&gt;
    ///                      &lt;bindingExpression&gt;this&lt;/bindingExpression&gt;
    ///                      &lt;Id&gt;VM_MatPrices&lt;/Id&gt;
    ///                      &lt;displayInfo&gt;
    ///                      &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEChart&lt;/type&gt;
    ///                      &lt;visible&gt;true&lt;/visible&gt;
    ///                      &lt;/displayInfo&gt;
    ///                      &lt;validationRuleList /&gt;
    ///                      &lt;clientEvents /&gt;
    ///                      &lt;configInfo&gt;
    ///                      &lt;fieldConfig&gt;
    ///                          &lt;configSetting bindingExpression="this.EnableClientSideAPI"&gt;false&lt;/configSetting&gt;
    ///                      &lt;/fieldConfig&gt;
    ///                      &lt;/configInfo&gt;
    ///                      &lt;dataSource /&gt;
    ///                      &lt;displayData /&gt;
    ///                  &lt;/formElement&gt;
    /// </code>
    /// </summary>
    [ToolboxData("<{0}:COEChart runat=server></{0}:COEChart>")]
    public class COEChart : DevExpress.XtraCharts.Web.WebChartControl, ICOEGenerableControl
    {
        #region Variables

        private string _defaultValue = string.Empty;

        #endregion

        #region ICOEGenerableControl Members

        public new object GetData()
        {
            return null;
        }

        public void PutData(object data)
        {
            if (data is DataTable)
            {
                //This control requires an specific obj as datasource to display, that's why we need the conversion.
                this.CreateSeries(data);
            }
        }

        /// <summary>
        /// Method intended to delegate specific configurations to the underlying control. This config info will
        /// come inside an xml tag <b>&lt;configInfo&gt;</b>.
        /// </summary>
        /// <param name="xmlDataAsString">A string with xml format that contains control's configurations</param>
        public void LoadFromXml(string xmlDataAsString)
        {
            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(xmlDataAsString);
            XmlNamespaceManager manager = new XmlNamespaceManager(xmlData.NameTable);
            manager.AddNamespace("COE", xmlData.DocumentElement.NamespaceURI);
            foreach (XmlNode configSetting in xmlData.SelectNodes("//COE:configSetting", manager))
            {
                if (configSetting != null && configSetting.InnerText.Length > 0)
                {
                    string bindingExpression = configSetting.Attributes["bindingExpression"].Value;
                    if (!string.IsNullOrEmpty(bindingExpression))
                    {
                        try
                        {   //Using COEDatabinder to find the object property given a bindingExpression in the current object
                            COEDataBinder dataBinder = new COEDataBinder(this);
                            dataBinder.SetProperty(bindingExpression, configSetting.InnerText);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }
        }

        public COEChart()
        {
           
        }

        /// <summary>
        /// Creates the series of points to display by the control
        /// </summary>
        /// <param name="ds">The datasource to convert.</param>
        private void CreateSeries(object ds)
        {
            if (ds is DataTable)
            {
                Series serie = new Series();
                serie.Label.Visible = true;
                serie.ArgumentScaleType = ScaleType.Numerical;

                LineSeriesView seriesView = new LineSeriesView();
                seriesView.LineMarkerOptions.Visible = true;
                serie.View = seriesView;

                foreach(DataRow row in ((DataTable)ds).Rows) //Here is the real conversion
                    serie.Points.Add(new SeriesPoint(row[0], row[1]));
                this.Series.Add(serie);
            }
        }

        /// <summary>
        /// Gets or sets the default value of the control.
        /// </summary>
        /// <value></value>
        public string DefaultValue
        {
            get
            {
                return _defaultValue;
            }
            set
            {
                _defaultValue = value;
            }
        }

        #endregion
    }
}
