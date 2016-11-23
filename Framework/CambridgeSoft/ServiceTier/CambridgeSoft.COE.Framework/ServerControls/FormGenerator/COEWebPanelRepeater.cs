using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;
using System.Xml;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.ServerControls.FormGenerator;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using Infragistics.WebUI.UltraWebTab;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Messaging;
using System.Data;



    [ToolboxData("<{0}:WebPanelContainer runat=server></{0}:WebPanelContainer>")]
    public class COEWebPanelRepeater : Label, ICOEGenerableControl
    {
        XmlNamespaceManager manager = null;
        private string _defaultValue = string.Empty;
        private XmlNodeList _xmlNodeList= null;
        Dictionary<ICOEGenerableControl, FormGroup.FormElement> formElements = new Dictionary<ICOEGenerableControl, FormGroup.FormElement>();
        private string _imageClass = string.Empty;
        private DataSet _dataSet = null;

        //webpanel setting
        private string _expHeaderStyle = string.Empty;
        private string _colHeaderStyle = string.Empty;
        private string _panelStyle = string.Empty;
        private string _expImageCSSClass = string.Empty;
        private string _height = string.Empty;
        private string _width = string.Empty;
        private string _cssClass = string.Empty;
        private string _headerField = string.Empty;
        private string _headerExp = string.Empty;
        private string _headerCol = string.Empty;
        private string _toolTip = string.Empty;
        private string _initialExpandedState = string.Empty;
        private string _table = string.Empty;
        private string _displayField = string.Empty;



        public COEWebPanelRepeater()
        {
            
        }
        /// <summary>
        /// <para>Allows to set the default Text for the control.</para>
        /// </summary>
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

        /// <summary>
        /// <para>Gets the Text of the control.</para>
        /// </summary>
        /// <returns>A string with the control's text.</returns>
        public object GetData()
        {

            return null;
        }

        /// <summary>
        /// Sets the control's text.
        /// </summary>
        /// <param name="data">A string with the desired text of the control.</param>
        public void PutData(object data)
        {

            _dataSet = (DataSet)data;

           
        }

        /// <summary>Loads its specific configuration from an xml in the form:
        /// <code lang="Xml">
        ///   &lt;fieldConfig&gt;
        ///   &lt;CSSClass&gt;COELabelTitle&lt;/CSSClass&gt;
        ///   &lt;ID&gt;CompoundTitleLabel&lt;/ID&gt;
        ///   &lt;/fieldConfig&gt;
        /// </code>
        /// </summary>
        /// <param name="xmlDataAsString">The configInfo xml snippet</param>
        public void LoadFromXml(string xmlDataAsString)
        {
            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(xmlDataAsString);

            manager = new XmlNamespaceManager(xmlData.NameTable);
            manager.AddNamespace("COE", xmlData.DocumentElement.NamespaceURI);

            //Try to avoid the use of Style, Width and Height; instead of it, define all in a CSSClass.
            XmlNode style = xmlData.SelectSingleNode("//COE:Style", manager);
            if (style != null && style.InnerText.Length > 0)
            {
                string[] styles = style.InnerText.Split(new char[1] { ';' });
                for (int i = 0; i < styles.Length; i++)
                {
                    if (styles[i].Length > 0)
                    {
                        string[] styleDef = styles[i].Split(new char[1] { ':' });
                        string styleId = styleDef[0].Trim();
                        string styleValue = styleDef[1].Trim();
                        this.Style.Add(styleId, styleValue);
                    }
                }
            }


            XmlNode expHeaderStyle = xmlData.SelectSingleNode("//COE:ExpandedHeaderCssClass", manager);
            if (expHeaderStyle != null && expHeaderStyle.InnerText.Length > 0)
            {
                _expHeaderStyle = expHeaderStyle.InnerText;
            }

            XmlNode colHeaderStyle = xmlData.SelectSingleNode("//COE:CollapsedHeaderCssClass", manager);
            if (colHeaderStyle != null && colHeaderStyle.InnerText.Length > 0)
            {
                _colHeaderStyle = colHeaderStyle.InnerText;
                
            }

            XmlNode panelStyle = xmlData.SelectSingleNode("//COE:PanelCssClass", manager);
            if (panelStyle != null && panelStyle.InnerText.Length > 0)
            {
                _panelStyle = panelStyle.InnerText;
                //this.PanelStyle.CssClass = panelStyle.InnerText;
            }
            
            XmlNode expImageCSSClass = xmlData.SelectSingleNode("//COE:ExpImageCssClass", manager);
            if (expImageCSSClass != null && expImageCSSClass.InnerText.Length > 0)
            {
                
                _expImageCSSClass = expImageCSSClass.InnerText;
            }
            

            XmlNode width = xmlData.SelectSingleNode("//COE:Width", manager);
            if (width != null && width.InnerText.Length > 0)
            {
                _width = width.InnerText;
            }

            XmlNode height = xmlData.SelectSingleNode("//COE:Height", manager);
            if (height != null && height.InnerText.Length > 0)
            {
                _height = height.InnerText;
            }

            XmlNode cssclass = xmlData.SelectSingleNode("//COE:CssClass", manager);
            if (cssclass != null && cssclass.InnerText.Length > 0)
            {
                _cssClass = cssclass.InnerText;
            }

           


         

            XmlNode headerExp = xmlData.SelectSingleNode("//COE:HeaderExpandedImage", manager);
            if (headerExp != null && headerExp.InnerText.Length > 0)
            {
                _headerExp = headerExp.InnerText;

            }

            //coverity fix
            XmlNode headerCol = xmlData.SelectSingleNode("//COE:HeaderCollapsedImage", manager);
            if (headerCol != null && _headerExp.Length > 0)
            {
                _headerCol = headerCol.InnerText;
                

            }


           XmlNode toolTip = xmlData.SelectSingleNode("//COE:ToolTip", manager);
           if (toolTip != null && toolTip.InnerText.Length > 0)
            {
                _toolTip = toolTip.InnerText;

            }

            XmlNode iniState = xmlData.SelectSingleNode("//COE:InitialStateExpanded", manager);
            if (iniState != null && iniState.InnerText.Length > 0)
            {
                _initialExpandedState = iniState.InnerText;

            }


            XmlNode headerField = xmlData.SelectSingleNode("//COE:HeaderField", manager);
            if (headerField != null && headerField.InnerText.Length > 0)
            {
                _headerField = headerField.InnerText;

            }

            XmlNode table = xmlData.SelectSingleNode("//COE:Table", manager);
            if (table != null && table.InnerText.Length > 0)
            {
                _table = table.InnerText;

            }

            XmlNode displayField = xmlData.SelectSingleNode("//COE:DisplayField", manager);
            if (displayField != null && displayField.InnerText.Length > 0)
            {
                _displayField = displayField.InnerText;

            }

            string errorMsg;
            //foreach (XmlNode tab in xmlData.SelectNodes("//COE:controls/COE:control", manager))
            //{
            //    ICOEGenerableControl formElement = COEFormGenerator.GetCOEGenerableControl(tab.SelectSingleNode("./COE:formElement", manager).OuterXml, out errorMsg);
            //    formElements.Add(formElement, FormGroup.FormElement.GetFormElement(tab.SelectSingleNode("./COE:formElement", manager).OuterXml));
            //    this.Controls.Add((Control)formElement);

            //}




        }

 

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            //LoadFromXml();

         
        }

        protected override void OnPreRender(EventArgs e)
        {            //after the control is loaded we can go back a fix the stupid expansion image issue
            //((Infragistics.WebUI.WebControls.Image)(((Infragistics.WebUI.Misc.WebPanel)(this)).Header.ExpansionIndicator)).Attributes.Add("class", _expImageCSSClass);
            //((Infragistics.WebUI.WebControls.Image)(((Infragistics.WebUI.Misc.WebPanel)(this)).Header.ExpansionIndicator)).Attributes.Remove("style");


            base.OnPreRender(e);
        }


        protected override void CreateChildControls()
        {
           
            base.CreateChildControls();
        }
        protected override void Render(HtmlTextWriter writer)
        {
            for (int i = 0; i < _dataSet.Tables[_table].Rows.Count - 1; i++)
            {
                DataRow dr = _dataSet.Tables[_table].Rows[i];
                Infragistics.WebUI.Misc.WebPanel thePanel = new Infragistics.WebUI.Misc.WebPanel();
                Label content = new Label();
                
                thePanel.Header.Text = dr[_headerField].ToString();
                 
                thePanel.Header.ExpandedAppearance.Style.CssClass = _expHeaderStyle;
                thePanel.Header.ExpansionIndicator.ExpandedImageUrl = _headerCol;
                thePanel.Header.ExpansionIndicator.CollapsedImageUrl = _headerExp;
                thePanel.Header.CollapsedAppearance.Style.CssClass = _colHeaderStyle;
                thePanel.ExpandEffect = Infragistics.WebUI.Misc.ExpandEffect.None;
                thePanel.PanelStyle.CssClass = _panelStyle;
                thePanel.Width = new Unit(_width);
                thePanel.Height = new Unit(_height);
                thePanel.CssClass = _cssClass;
                thePanel.ToolTip = _toolTip;
                thePanel.Expanded = bool.Parse(_initialExpandedState);
                content.Text = dr[_displayField].ToString();
                thePanel.Controls.Add(content);

                // Fix Converity: CID-28973 Comment never used variable
                ////Literal pageBreak = new Literal();
                ////pageBreak.Text = "</br>";
                this.Controls.Add(thePanel);
                //this.Controls.Add(pageBreak);
                ((Infragistics.WebUI.WebControls.Image)(((Infragistics.WebUI.Misc.WebPanel)(thePanel)).Header.ExpansionIndicator)).Attributes.Add("class", _expImageCSSClass);
                ((Infragistics.WebUI.WebControls.Image)(((Infragistics.WebUI.Misc.WebPanel)(thePanel)).Header.ExpansionIndicator)).Attributes.Remove("style");

            }
           
          base.Render(writer);
        }
    }

