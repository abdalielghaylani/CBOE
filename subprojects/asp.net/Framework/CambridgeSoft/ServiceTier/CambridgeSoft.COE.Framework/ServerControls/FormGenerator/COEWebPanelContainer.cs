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
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Reflection;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    [ToolboxData("<{0}:WebPanelContainer runat=server></{0}:WebPanelContainer>")]
    public class COEWebPanelContainer : Infragistics.WebUI.Misc.WebPanel, ICOEGenerableControl, ICOEFullDatasource
    {
        #region Properties
        private object DataSource
        {
            get
            {
                object o = ViewState["DataSource"];
                return (o == null) ? (object) new DataSet() : (object) o;
            }
            set
            {
                ViewState["DataSource"] = value;
            }
        }

        private string ColumnIDValueForMarked
        {
            get
            {
                return ViewState["ColumnIDValueForMarked"] as string;
            }
            set
            {
                ViewState["ColumnIDValueForMarked"] = value;
            }
        }
        #endregion

        #region ICOEFullDatasource Members
        public object FullDatasource
        {
            set
            {
                ViewState["FullDatasource"] = value;
                foreach(KeyValuePair<ICOEGenerableControl, FormGroup.FormElement> keyVal in formElements)
                {
                    if(keyVal.Key is ICOEFullDatasource)
                        ((ICOEFullDatasource) keyVal.Key).FullDatasource = value;
                }
            }
            get { return ViewState["FullDatasource"]; }
        }
        #endregion

        #region Variables
        XmlNamespaceManager manager = null;
        private string _defaultValue = string.Empty;
        private XmlNodeList _xmlNodeList = null;
        Dictionary<ICOEGenerableControl, FormGroup.FormElement> formElements = new Dictionary<ICOEGenerableControl, FormGroup.FormElement>();
        List<Panel> children = new List<Panel>();
        private string _imageClass = string.Empty;
        private string _table = string.Empty;
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEFormGenerator");
        #endregion

        #region Constructors
        public COEWebPanelContainer() { }
        #endregion

        #region ICOEGenerableControl Members
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

            COEDataBinder binder = new COEDataBinder(this.FullDatasource);
            if(!string.IsNullOrEmpty(this.Attributes["bindingExpression"]))
                binder = new COEDataBinder(binder.RetrieveProperty(this.Attributes["bindingExpression"]));
            foreach(KeyValuePair<ICOEGenerableControl, FormGroup.FormElement> keyVal in formElements)
            {
                if(!string.IsNullOrEmpty(keyVal.Value.BindingExpression))
                {
                    ((WebControl) keyVal.Key).Attributes["BindingExpression"] = keyVal.Value.BindingExpression;
                    if(keyVal.Value.BindingExpression == "this")
                        binder.RootObject = keyVal.Key.GetData();
                    else
                    {
                        binder.SetProperty(keyVal.Value.BindingExpression, keyVal.Key.GetData());
                        if(keyVal.Key is ICOEFullDatasource)
                        {
                            this.FullDatasource = ((ICOEFullDatasource) keyVal.Key).FullDatasource;
                        }
                    }
                }
            }
            return binder.RootObject;
        }

        /// <summary>
        /// Sets the control's text.
        /// </summary>
        /// <param name="data">A string with the desired text of the control.</param>
        public void PutData(object data)
        {
            this.DataSource = data;
            
            COEDataBinder binder = new COEDataBinder(DataSource);
            object obj2 = null;
            string str = this._table;
            foreach(KeyValuePair<ICOEGenerableControl, FormGroup.FormElement> pair in this.formElements)
            {
                ICOEGenerableControl key = pair.Key;
                FormGroup.FormElement element = pair.Value;
                if(!(string.IsNullOrEmpty(element.BindingExpression) || (element.BindingExpression.IndexOf('|') <= 0)))
                {
                    string[] strArray = element.BindingExpression.Split(new char[] { '|' });
                    this._table = strArray[0];
                    element.BindingExpression = strArray[1];
                }
                if(!string.IsNullOrEmpty(element.BindingExpression))
                {

                    if(!string.IsNullOrEmpty(_table))
                    {
                        if(this._table == "PASS_ALL")
                            binder = new COEDataBinder(DataSource);
                        else
                        {
                            if(((DataSet) DataSource).Tables[this._table] != null && ((DataSet) DataSource).Tables[this._table].Rows.Count > 0)
                                binder = new COEDataBinder(((DataSet) DataSource).Tables[this._table].Rows[0]);
                            else
                                binder = null;
                        }
                    }
                    try
                    {
                        obj2 = binder != null ? binder.RetrieveProperty(element.BindingExpression) : string.Empty;
                        if(obj2 == null)
                            obj2 = string.Empty;
                    }
                    catch(Exception)
                    {
                        obj2 = string.Format("Invalid Property '{0}'", element.BindingExpression);
                    }
                }
                else
                    obj2 = element.DefaultValue;
                // Coverity Fix CID - 10468 (from local server)
                if(obj2 != null)
                    key.PutData(obj2);
                this._table = str;
                
                if(key is ICOEHitMarker)
                {
                    ColumnIDValueForMarked = ((ICOEHitMarker) key).ColumnIDValue = binder != null ? binder.RetrieveProperty(((ICOEHitMarker) key).ColumnIDBindingExpression).ToString() : string.Empty;
                }
            }
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
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlDataAsString);
            this.manager = new XmlNamespaceManager(document.NameTable);
            this.manager.AddNamespace("COE", document.DocumentElement.NamespaceURI);
            XmlNode styleNode = document.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:Style", this.manager);
            if((styleNode != null) && (styleNode.InnerText.Length > 0))
            {
                string[] strArray = styleNode.InnerText.Split(new char[] { ';' });
                for(int i = 0; i < strArray.Length; i++)
                {
                    if(strArray[i].Length > 0)
                    {
                        string[] strArray2 = strArray[i].Split(new char[] { ':' });
                        string key = strArray2[0].Trim();
                        string str2 = strArray2[1].Trim();
                        base.Style.Add(key, str2);
                    }
                }
            }

            XmlNode expandedHeaderClassNode = document.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:ExpandedHeaderCssClass", this.manager);
            if((expandedHeaderClassNode != null) && (expandedHeaderClassNode.InnerText.Length > 0))
            {
                base.Header.ExpandedAppearance.Style.CssClass = expandedHeaderClassNode.InnerText;
            }
            XmlNode collapsedHeaderClassNode = document.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:CollapsedHeaderCssClass", this.manager);
            if((collapsedHeaderClassNode != null) && (collapsedHeaderClassNode.InnerText.Length > 0))
            {
                base.Header.CollapsedAppearance.Style.CssClass = collapsedHeaderClassNode.InnerText;
            }
            XmlNode panelClass = document.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:PanelCssClass", this.manager);
            if((panelClass != null) && (panelClass.InnerText.Length > 0))
            {
                base.PanelStyle.CssClass = panelClass.InnerText;
            }
            XmlNode expandedImageClass = document.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:ExpImageCssClass", this.manager);
            if((expandedImageClass != null) && (expandedImageClass.InnerText.Length > 0))
            {
                this._imageClass = expandedImageClass.InnerText;
            }
            XmlNode tableNode = document.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:Table", this.manager);
            if((tableNode != null) && (tableNode.InnerText.Length > 0))
            {
                this._table = tableNode.InnerText;
            }
            XmlNode showHeaderNode = document.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:ShowHeader", this.manager);
            if((showHeaderNode != null) && (showHeaderNode.InnerText.Length > 0))
            {
                this.Header.Visible = showHeaderNode.InnerText.ToLower() == "true" || showHeaderNode.InnerText.ToLower() == "yes";
            }
            XmlNode titleNode = document.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:Title", this.manager);
            if((titleNode != null) && (titleNode.InnerText.Length > 0))
            {
                this.Text = titleNode.InnerText;
            }
            XmlNode widthNode = document.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:Width", this.manager);
            if((widthNode != null) && (widthNode.InnerText.Length > 0))
            {
                this.Width = new Unit(widthNode.InnerText);
            }
            XmlNode heightNode = document.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:Height", this.manager);
            if((heightNode != null) && (heightNode.InnerText.Length > 0))
            {
                this.Height = new Unit(heightNode.InnerText);
            }
            XmlNode classNode = document.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:CssClass", this.manager);
            if((classNode != null) && (classNode.InnerText.Length > 0))
            {
                this.CssClass = classNode.InnerText;
            }
            XmlNode headerTextNode = document.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:HeaderText", this.manager);
            if((headerTextNode != null) && (headerTextNode.InnerText.Length > 0))
            {
                base.Header.Text = headerTextNode.InnerText;
            }
            XmlNode headerCollapsedImageNode = document.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:HeaderCollapsedImage", this.manager);
            if((headerCollapsedImageNode != null) && (headerCollapsedImageNode.InnerText.Length > 0))
            {
                base.Header.ExpansionIndicator.ExpandedImageUrl = headerCollapsedImageNode.InnerText;
            }
            XmlNode headerExpandedImageNode = document.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:HeaderExpandedImage", this.manager);
            if((headerExpandedImageNode != null) && (headerExpandedImageNode.InnerText.Length > 0))
            {
                base.Header.ExpansionIndicator.CollapsedImageUrl = headerExpandedImageNode.InnerText;
            }
            base.ExpandEffect = Infragistics.WebUI.Misc.ExpandEffect.None;
            XmlNode tooltipNode = document.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:ToolTip", this.manager);
            if((tooltipNode != null) && (tooltipNode.InnerText.Length > 0))
            {
                this.ToolTip = tooltipNode.InnerText;
            }
            XmlNode initialStateExpandedNode = document.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:InitialStateExpanded", this.manager);
            if((initialStateExpandedNode != null) && (initialStateExpandedNode.InnerText.Length > 0))
            {
                base.Expanded = bool.Parse(initialStateExpandedNode.InnerText);
            }
            foreach(XmlNode controlNode in document.SelectNodes("./COE:configInfo/COE:fieldConfig/COE:controls/COE:control", this.manager))
            {
                string str3;
                ICOEGenerableControl cOEGenerableControl = COEFormGenerator.GetCOEGenerableControl(controlNode.SelectSingleNode("./COE:formElement", manager).OuterXml, out str3);

                FormGroup.FormElement formElement = FormGroup.FormElement.GetFormElement(controlNode.SelectSingleNode("./COE:formElement", this.manager).OuterXml);
                this.formElements.Add(cOEGenerableControl, formElement);
                Panel child = new Panel();
                child.ID = formElement.Id + "Parent";
                if(!string.IsNullOrEmpty(formElement.DisplayInfo.CSSClass))
                    child.CssClass = formElement.DisplayInfo.CSSClass;
                else
                    child.CssClass = formElement.Id + "_class";
                if(!string.IsNullOrEmpty(formElement.DisplayInfo.Style))
                    child.Style.Value = formElement.DisplayInfo.Style;
                if(!string.IsNullOrEmpty(formElement.DisplayInfo.Top))
                    child.Style.Add(HtmlTextWriterStyle.Top, formElement.DisplayInfo.Top);
                if(!string.IsNullOrEmpty(formElement.DisplayInfo.Left))
                    child.Style.Add(HtmlTextWriterStyle.Left, formElement.DisplayInfo.Left);
                if(!string.IsNullOrEmpty(formElement.DisplayInfo.Position))
                    child.Style.Add(HtmlTextWriterStyle.Position, formElement.DisplayInfo.Position);
                if(!string.IsNullOrEmpty(formElement.DisplayInfo.Height))
                    child.Height = new Unit(formElement.DisplayInfo.Height);
                if(!string.IsNullOrEmpty(formElement.DisplayInfo.Width))
                    child.Height = new Unit(formElement.DisplayInfo.Width);
                child.Visible = formElement.DisplayInfo.Visible;

                child.Controls.Add((Control) cOEGenerableControl);
                this.children.Add(child);
            }
            _coeLog.LogEnd(methodSignature);
        }
        #endregion

        #region LifeCycle overriden methods
        protected override void CreateChildControls()
        {
            this.Controls.Clear();

            //after the control is loaded we can go back a fix the stupid expansion image issue
            //((WebControl) this.Header.ExpansionIndicator).Attributes.Add("class", _imageClass);
            //((WebControl) this.Header.ExpansionIndicator).Attributes.Remove("style");
            foreach(Panel pan in this.children)
            {
                this.Controls.Add(pan);
            }


            COEDataBinder binder = new COEDataBinder(DataSource);
            object obj2 = null;
            string str = this._table;
            foreach(KeyValuePair<ICOEGenerableControl, FormGroup.FormElement> pair in this.formElements)
            {
                ICOEGenerableControl key = pair.Key;
                FormGroup.FormElement element = pair.Value;
                if(!(string.IsNullOrEmpty(element.BindingExpression) || (element.BindingExpression.IndexOf('|') <= 0)))
                {
                    string[] strArray = element.BindingExpression.Split(new char[] { '|' });
                    this._table = strArray[0];
                    element.BindingExpression = strArray[1];
                }
                if(!string.IsNullOrEmpty(element.BindingExpression))
                {

                    if(!string.IsNullOrEmpty(_table))
                    {
                        if(this._table == "PASS_ALL")
                            binder = new COEDataBinder(DataSource);
                        else
                        {
                            if(((DataSet) DataSource).Tables[this._table] != null && ((DataSet) DataSource).Tables[this._table].Rows.Count > 0)
                                binder = new COEDataBinder(((DataSet) DataSource).Tables[this._table].Rows[0]);
                            else
                                binder = null;
                        }
                    }
                }

                if(key is ICOEHitMarker)
                {
                    ColumnIDValueForMarked = ((ICOEHitMarker) key).ColumnIDValue = binder != null ? binder.RetrieveProperty(((ICOEHitMarker) key).ColumnIDBindingExpression).ToString() : string.Empty;
                }
            }

            this.ChildControlsCreated = true;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            //this.EnableViewState = false;
            //LoadFromXml();
        }

        protected override void OnPreRender(EventArgs e)
        {            
            base.OnPreRender(e);
        }
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }
        #endregion
    }
}
