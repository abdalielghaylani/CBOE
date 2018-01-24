using System;
using System.Collections.Generic;
using System.Collections;
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
    [ToolboxData("<{0}:COETabRepeater runat=server></{0}:COETabRepeater>")]
    public class COETabRepeater : UltraWebTab, ICOEGenerableControl, ICOEFullDatasource
    {
        #region Properties
        private object DataSource
        {
            get
            {
                object o = ViewState["DataSource"];
                return (o == null) ? (object)new DataSet() : (object)o;
            }
            set
            {
                ViewState["DataSource"] = value;
            }
        }
        #endregion

        #region ICOEFullDatasource Members
        public object FullDatasource
        {
            set
            {
                ViewState["FullDatasource"] = value;
                foreach (KeyValuePair<ICOEGenerableControl, FormGroup.FormElement> keyVal in formElements)
                {
                    if (keyVal.Key is ICOEFullDatasource)
                        ((ICOEFullDatasource)keyVal.Key).FullDatasource = value;
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
        Dictionary<ICOEGenerableControl, FormGroup.FormElement> dynamicFormElements = new Dictionary<ICOEGenerableControl, FormGroup.FormElement>();
        private string _tabsDataMember = string.Empty;
        private string _tabsDataValue = string.Empty;
        private int _staticCounter = 0;
        #endregion
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEFormGenerator");

        #region Constructor
        public COETabRepeater()
        {
        }
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
            if (!string.IsNullOrEmpty(this.Attributes["BindingExpression"]))
                binder = new COEDataBinder(binder.RetrieveProperty(this.Attributes["BindingExpression"]));
            foreach (KeyValuePair<ICOEGenerableControl, FormGroup.FormElement> keyVal in formElements)
            {
                if (!string.IsNullOrEmpty(keyVal.Value.BindingExpression))
                {
                    ((WebControl)keyVal.Key).Attributes["BindingExpression"] = keyVal.Value.BindingExpression;
                    if (keyVal.Value.BindingExpression == "this")
                        binder.RootObject = keyVal.Key.GetData();
                    else
                    {
                        binder.SetProperty(keyVal.Value.BindingExpression, keyVal.Key.GetData());
                        if (keyVal.Key is ICOEFullDatasource)
                        {
                            this.FullDatasource = ((ICOEFullDatasource)keyVal.Key).FullDatasource;
                        }
                    }
                }
            }
            return binder.RootObject;
        }

        public void NormalizeIDs(ControlCollection controls,int index)
        {
            foreach (Control control in controls)
            {
                //return name + _pageIndex.ToString() + "_of" + _pagesCount.ToString();

                control.ID += "_" + index.ToString() + "_";
                NormalizeIDs(control.Controls, index);
            }
        }

        /// <summary>
        /// Sets the control's text.
        /// </summary>
        /// <param name="data">A string with the desired text of the control.</param>
        public void PutData(object data)
        {
            object value = null;
            this.DataSource = data;

            COEDataBinder dataBinder = new COEDataBinder(data);

            bool firstRow = true;
            for (int i = 0; i < ((IList)dataBinder.RootObject).Count; i++)
            {
                string newText = string.Empty;
                string newKey = string.Empty;

                if (_tabsDataMember != string.Empty)
                    newText = (string)dataBinder.RetrieveProperty(string.Format("[{0}].", i.ToString()) + _tabsDataMember);

                if (_tabsDataValue != string.Empty)
                    newKey = dataBinder.RetrieveProperty(string.Format("[{0}].", i.ToString()) + _tabsDataValue).ToString();

                Tab newTab = null;
                if (!firstRow)
                {
                    // Create a new tab cloning the first tab
                    newTab = CloneTab(this.Tabs[_staticCounter], i.ToString());
                    
                    //NormalizeIDs(newTab.ContentPane.Controls, 1);
                }
                else
                {
                    newTab = (Tab)this.Tabs[_staticCounter];
                    
                    //NormalizeIDs(newTab.ContentPane.Controls, 0);
                    firstRow = false;
                }

                if(!string.IsNullOrEmpty(newText))
                    newTab.Text = newText;
                if (!string.IsNullOrEmpty(newKey))
                    newTab.Key = newKey;

            }

            //here we need to do the right thing and push the correct data down to the sub formelements
            dataBinder = new COEDataBinder(this.FullDatasource);

            foreach (KeyValuePair<ICOEGenerableControl, FormGroup.FormElement> keyVal in formElements)
            {
                ICOEGenerableControl formGroupControl = keyVal.Key;
                FormGroup.FormElement formElement = keyVal.Value;

                if (!string.IsNullOrEmpty(formElement.BindingExpression))
                {
                    try
                    {
                        value = dataBinder.RetrieveProperty(formElement.BindingExpression);
                        if (value is Csla.Core.IEditableCollection)
                        {
                            int index = int.Parse(((Control)formGroupControl).ID.Substring(((Control)formGroupControl).ID.IndexOf("T_T") + 3));
                            value = dataBinder.RetrieveProperty(formElement.BindingExpression + string.Format("[{0}]", index.ToString()));
                        }

                        if (value == null)
                            value = string.Empty;
                        formGroupControl.PutData(value);
                    }
                    catch (Exception exception)
                    {
                        value = string.Format("Invalid Property '{0}'", formElement.BindingExpression);
                    }
                }
                else if (!string.IsNullOrEmpty(formElement.DefaultValue))
                {
                    formGroupControl.PutData(formElement.DefaultValue);
                }

                if (formGroupControl is ICOEHitMarker)
                {
                    ((ICOEHitMarker)formGroupControl).ColumnIDValue = dataBinder.RetrieveProperty(((ICOEHitMarker)formGroupControl).ColumnIDBindingExpression).ToString();
                }
            }

            this.RecreateChildControls();
        }

        private Tab CloneTab(TabItem oldTab, string addedID)
        {
            Tab newTab = new Tab();
            // This is the original method to clone first manufacturing tab, it doesn't work because the objects are the same instance
            //newTab.ContentPane = ((Tab)oldTab).ContentPane;


            // Trying to clone child controls directly
            // It doesn't work, the original tab doesn't have child controls.
            CloneControls(((Tab)oldTab).ContentPane.Controls, ((Tab)newTab).ContentPane.Controls);
            
            // In this line try to clone tab using CloneObject method that call CloneControls to clone child controls
            // It doesn't work because some child controls don't exist at this point.
            //newTab = (Tab)CloneObject(oldTab);
            
            this.Tabs.Insert(this.Tabs.Count, newTab);

            foreach (KeyValuePair<ICOEGenerableControl, FormGroup.FormElement> dynElement in dynamicFormElements)
            {
                string newID = ((Control)dynElement.Key).ID.Remove(((Control)dynElement.Key).ID.IndexOf("T_T"));
                ((Control)dynElement.Key).ID = newID + "T_T" + addedID;
                newID = dynElement.Value.Id.Remove(dynElement.Value.Id.IndexOf("T_T"));
                dynElement.Value.Id = newID + "T_T" + addedID;

                if (!formElements.ContainsKey(dynElement.Key))
                    formElements.Add(dynElement.Key, dynElement.Value);
            }
            return newTab;
        }

        /// <summary>
        /// Clone a control collection
        /// </summary>
        /// <param name="org">Original controls collection to be cloned</param>
        /// <param name="dest">Destiny collection that contains cloned controls</param>
        private void CloneControls(ControlCollection org, ControlCollection dest)
        {
            foreach (Control ctrl in org)
            {
                dest.Add((Control)CloneObject(ctrl));
                if (ctrl.GetType().Name.Contains("Container"))
                {
                    //CloneControls(((COEWebPanelContainer)ctrl).ContentPane.Controls, dest[dest.Count - 1].ContentPane.Controls);
                }
                else
                {
                    CloneControls(ctrl.Controls, dest[dest.Count - 1].Controls);
                }
            }
        }

        /// <summary>
        /// Clone an object and its properties
        /// </summary>
        /// <param name="o">The object to be cloned</param>
        /// <returns>A new instance of the object with same properties</returns>
        private object CloneObject(object o)
        {
            Type t = o.GetType();
            PropertyInfo[] properties = t.GetProperties();

            Object p = t.InvokeMember("", System.Reflection.BindingFlags.CreateInstance, null, o, null);

            foreach (PropertyInfo pi in properties)
            {
                if (pi.CanWrite)
                {
                    pi.SetValue(p, pi.GetValue(o, null), null);
                }
            }

            return p;
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

            XmlNode width = xmlData.SelectSingleNode("//COE:Width", manager);
            if (width != null && width.InnerText.Length > 0)
            {
                this.Width = new Unit(width.InnerText);
            }

            XmlNode height = xmlData.SelectSingleNode("//COE:Height", manager);
            if (height != null && height.InnerText.Length > 0)
            {
                this.Height = new Unit(height.InnerText);
            }

            XmlNode cssclass = xmlData.SelectSingleNode("//COE:CssClass", manager);
            if (cssclass != null && cssclass.InnerText.Length > 0)
            {
                this.CssClass = cssclass.InnerText;

            }

            XmlNode tabOrientation = xmlData.SelectSingleNode("//COE:TabOrientation", manager);
            if (tabOrientation != null && tabOrientation.InnerText.Length > 0)
            {
                switch (tabOrientation.InnerText.ToLower())
                {
                    case "bottomleft":
                        this.TabOrientation = TabOrientation.BottomLeft;
                        break;
                    case "bottomright":
                        this.TabOrientation = TabOrientation.BottomRight;
                        break;
                    case "leftbottom":
                        this.TabOrientation = TabOrientation.LeftBottom;
                        break;
                    case "lefttop":
                        this.TabOrientation = TabOrientation.LeftTop;
                        break;
                    case "rightbottom":
                        this.TabOrientation = TabOrientation.RightBottom;
                        break;
                    case "righttop":
                        this.TabOrientation = TabOrientation.RightTop;
                        break;
                    case "topleft":
                        this.TabOrientation = TabOrientation.TopLeft;
                        break;
                    case "topright":
                        this.TabOrientation = TabOrientation.TopRight;
                        break;

                    default:
                        this.TabOrientation = TabOrientation.TopLeft;
                        break;
                }
            }

            XmlNode textOrientation = xmlData.SelectSingleNode("//COE:TextOrientation", manager);
            if (textOrientation != null && textOrientation.InnerText.Length > 0)
            {
                switch (textOrientation.InnerText.ToLower())
                {
                    case "horizontal":
                        this.TextOrientation = TextOrientation.Horizontal;
                        break;
                    case "vertical":
                        this.TextOrientation = TextOrientation.Vertical;
                        break;
                    default:
                        this.TextOrientation = TextOrientation.Horizontal;
                        break;
                }
            }

            XmlNode borderStyle = xmlData.SelectSingleNode("//COE:BorderStyle", manager);
            if (borderStyle != null && borderStyle.InnerText.Length > 0)
            {
                switch (borderStyle.InnerText.ToLower())
                {
                    case "dashed":
                        this.BorderStyle = BorderStyle.Dashed;
                        break;
                    case "solid":
                        this.BorderStyle = BorderStyle.Solid;
                        break;
                    case "double":
                        this.BorderStyle = BorderStyle.Double;
                        break;
                    case "groove":
                        this.BorderStyle = BorderStyle.Groove;
                        break;
                    case "inset":
                        this.BorderStyle = BorderStyle.Inset;
                        break;
                    case "none":
                        this.BorderStyle = BorderStyle.None;
                        break;
                    case "outset":
                        this.BorderStyle = BorderStyle.Outset;
                        break;
                    case "ridge":
                        this.BorderStyle = BorderStyle.Ridge;
                        break;
                    default:
                        this.BorderStyle = BorderStyle.Solid;
                        break;
                }
            }

            XmlNode borderColor = xmlData.SelectSingleNode("//COE:BorderColor", manager);
            if (borderColor != null && borderColor.InnerText.Length > 0)
            {
                this.BorderColor = System.Drawing.Color.FromName(borderColor.InnerText);
            }

            XmlNode borderWidth = xmlData.SelectSingleNode("//COE:BorderWidth", manager);
            if (borderWidth != null && borderWidth.InnerText.Length > 0)
            {
                this.BorderWidth = new Unit(borderWidth.InnerText);
            }

            XmlNode topBorderStyle = xmlData.SelectSingleNode("//COE:TopBorderStyle", manager);
            if (topBorderStyle != null && topBorderStyle.InnerText.Length > 0)
            {
                switch (topBorderStyle.InnerText.ToLower())
                {
                    case "dashed":
                        this.BorderDetails.StyleTop = BorderStyle.Dashed;
                        break;
                    case "solid":
                        this.BorderDetails.StyleTop = BorderStyle.Solid;
                        break;
                    case "double":
                        this.BorderDetails.StyleTop = BorderStyle.Double;
                        break;
                    case "groove":
                        this.BorderDetails.StyleTop = BorderStyle.Groove;
                        break;
                    case "inset":
                        this.BorderDetails.StyleTop = BorderStyle.Inset;
                        break;
                    case "none":
                        this.BorderDetails.StyleTop = BorderStyle.None;
                        break;
                    case "outset":
                        this.BorderDetails.StyleTop = BorderStyle.Outset;
                        break;
                    case "ridge":
                        this.BorderDetails.StyleTop = BorderStyle.Ridge;
                        break;

                    default:
                        this.BorderDetails.StyleTop = BorderStyle.Solid;
                        break;
                }
            }

            XmlNode topBorderColor = xmlData.SelectSingleNode("//COE:TopBorderColor", manager);
            if (topBorderColor != null && topBorderColor.InnerText.Length > 0)
            {
                this.BorderDetails.ColorTop = System.Drawing.Color.FromName(topBorderColor.InnerText);
            }

            XmlNode topBorderWidth = xmlData.SelectSingleNode("//COE:TopBorderWidth", manager);
            if (topBorderWidth != null && topBorderWidth.InnerText.Length > 0)
            {
                this.BorderDetails.WidthTop = new Unit(topBorderWidth.InnerText);
            }

            XmlNode bottomBorderStyle = xmlData.SelectSingleNode("//COE:BottomBorderStyle", manager);
            if (bottomBorderStyle != null && bottomBorderStyle.InnerText.Length > 0)
            {
                switch (bottomBorderStyle.InnerText.ToLower())
                {
                    case "dashed":
                        this.BorderDetails.StyleBottom = BorderStyle.Dashed;
                        break;
                    case "solid":
                        this.BorderDetails.StyleBottom = BorderStyle.Solid;
                        break;
                    case "double":
                        this.BorderDetails.StyleBottom = BorderStyle.Double;
                        break;
                    case "groove":
                        this.BorderDetails.StyleBottom = BorderStyle.Groove;
                        break;
                    case "inset":
                        this.BorderDetails.StyleBottom = BorderStyle.Inset;
                        break;
                    case "none":
                        this.BorderDetails.StyleBottom = BorderStyle.None;
                        break;
                    case "outset":
                        this.BorderDetails.StyleBottom = BorderStyle.Outset;
                        break;
                    case "ridge":
                        this.BorderDetails.StyleBottom = BorderStyle.Ridge;
                        break;
                    default:
                        this.BorderDetails.StyleBottom = BorderStyle.Solid;
                        break;
                }
            }

            XmlNode bottomBorderColor = xmlData.SelectSingleNode("//COE:BottomBorderColor", manager);
            if (bottomBorderColor != null && bottomBorderColor.InnerText.Length > 0)
            {
                this.BorderDetails.ColorBottom = System.Drawing.Color.FromName(bottomBorderColor.InnerText);
            }

            XmlNode bottomBorderWidth = xmlData.SelectSingleNode("//COE:BottomBorderWidth", manager);
            if (bottomBorderWidth != null && bottomBorderWidth.InnerText.Length > 0)
            {
                this.BorderDetails.WidthBottom = new Unit(bottomBorderWidth.InnerText);
            }

            XmlNode rightBorderStyle = xmlData.SelectSingleNode("//COE:RightBorderStyle", manager);
            if (rightBorderStyle != null && rightBorderStyle.InnerText.Length > 0)
            {
                switch (rightBorderStyle.InnerText.ToLower())
                {
                    case "dashed":
                        this.BorderDetails.StyleRight = BorderStyle.Dashed;
                        break;
                    case "solid":
                        this.BorderDetails.StyleRight = BorderStyle.Solid;
                        break;
                    case "double":
                        this.BorderDetails.StyleRight = BorderStyle.Double;
                        break;
                    case "groove":
                        this.BorderDetails.StyleRight = BorderStyle.Groove;
                        break;
                    case "inset":
                        this.BorderDetails.StyleRight = BorderStyle.Inset;
                        break;
                    case "none":
                        this.BorderDetails.StyleRight = BorderStyle.None;
                        break;
                    case "outset":
                        this.BorderDetails.StyleRight = BorderStyle.Outset;
                        break;
                    case "ridge":
                        this.BorderDetails.StyleRight = BorderStyle.Ridge;
                        break;
                    default:
                        this.BorderDetails.StyleRight = BorderStyle.Solid;
                        break;
                }
            }

            XmlNode rightBorderColor = xmlData.SelectSingleNode("//COE:RightBorderColor", manager);
            if (rightBorderColor != null && rightBorderColor.InnerText.Length > 0)
            {
                this.BorderDetails.ColorRight = System.Drawing.Color.FromName(rightBorderColor.InnerText);
            }

            XmlNode rightBorderWidth = xmlData.SelectSingleNode("//COE:RightBorderWidth", manager);
            if (rightBorderWidth != null && rightBorderWidth.InnerText.Length > 0)
            {
                this.BorderDetails.WidthRight = new Unit(rightBorderWidth.InnerText);
            }

            XmlNode leftBorderStyle = xmlData.SelectSingleNode("//COE:LeftBorderStyle", manager);
            if (leftBorderStyle != null && leftBorderStyle.InnerText.Length > 0)
            {
                switch (leftBorderStyle.InnerText.ToLower())
                {
                    case "dashed":
                        this.BorderDetails.StyleLeft = BorderStyle.Dashed;
                        break;
                    case "solid":
                        this.BorderDetails.StyleLeft = BorderStyle.Solid;
                        break;
                    case "double":
                        this.BorderDetails.StyleLeft = BorderStyle.Double;
                        break;
                    case "groove":
                        this.BorderDetails.StyleLeft = BorderStyle.Groove;
                        break;
                    case "inset":
                        this.BorderDetails.StyleLeft = BorderStyle.Inset;
                        break;
                    case "none":
                        this.BorderDetails.StyleLeft = BorderStyle.None;
                        break;
                    case "outset":
                        this.BorderDetails.StyleLeft = BorderStyle.Outset;
                        break;
                    case "ridge":
                        this.BorderDetails.StyleLeft = BorderStyle.Ridge;
                        break;

                    default:
                        this.BorderDetails.StyleLeft = BorderStyle.Solid;
                        break;
                }
            }

            XmlNode leftBorderColor = xmlData.SelectSingleNode("//COE:LeftBorderColor", manager);
            if (leftBorderColor != null && leftBorderColor.InnerText.Length > 0)
            {
                this.BorderDetails.ColorLeft = System.Drawing.Color.FromName(leftBorderColor.InnerText);
            }

            XmlNode leftBorderWidth = xmlData.SelectSingleNode("//COE:LeftBorderWidth", manager);
            if (leftBorderWidth != null && leftBorderWidth.InnerText.Length > 0)
            {
                this.BorderDetails.WidthLeft = new Unit(leftBorderWidth.InnerText);
            }

            XmlNode backColor = xmlData.SelectSingleNode("//COE:BackColor", manager);
            if (backColor != null && backColor.InnerText.Length > 0)
            {
                this.BackColor = System.Drawing.Color.FromName(backColor.InnerText);
            }

            XmlNode selectedImage = xmlData.SelectSingleNode("//COE:SelectedImage", manager);
            if (selectedImage != null && selectedImage.InnerText.Length > 0)
            {
                this.SelectedTabStyle.BackgroundImage = selectedImage.InnerText;
            }

            XmlNode selectedCssClass = xmlData.SelectSingleNode("//COE:SelectedCssClass", manager);
            if (selectedCssClass != null && selectedCssClass.InnerText.Length > 0)
            {
                this.SelectedTabStyle.CssClass = selectedCssClass.InnerText;
            }

            XmlNode disabledImage = xmlData.SelectSingleNode("//COE:DisabledImage", manager);
            if (disabledImage != null && disabledImage.InnerText.Length > 0)
            {
                this.DisabledTabStyle.BackgroundImage = disabledImage.InnerText;
            }

            XmlNode disabledCssClass = xmlData.SelectSingleNode("//COE:DisabledCssClass", manager);
            if (disabledCssClass != null && disabledCssClass.InnerText.Length > 0)
            {
                this.DisabledTabStyle.CssClass = disabledCssClass.InnerText;
                this.DefaultTabStyle.CssClass = disabledCssClass.InnerText;
            }

            XmlNode roundedSelectedImage = xmlData.SelectSingleNode("//COE:RoundedSelectedImage", manager);
            if (roundedSelectedImage != null && roundedSelectedImage.InnerText.Length > 0)
            {
                this.RoundedImage.SelectedImage = roundedSelectedImage.InnerText;
                this.RoundedImage.RightSideWidth = 5;
                this.RoundedImage.LeftSideWidth = 5;
                this.RoundedImage.FillStyle = RoundedImageStyle.LeftMergedWithCenter;
            }

            XmlNode roundedDisabledImaged = xmlData.SelectSingleNode("//COE:RoundedDisabledImage", manager);
            if (roundedDisabledImaged != null && roundedDisabledImaged.InnerText.Length > 0)
            {
                this.RoundedImage.NormalImage = roundedDisabledImaged.InnerText;
            }

            XmlNode displayMode = xmlData.SelectSingleNode("//COE:DisplayMode", manager);
            if (displayMode != null && displayMode.InnerText.Length > 0)
            {
                switch (displayMode.InnerText.ToLower())
                {
                    case "multirow":
                        this.DisplayMode = DisplayMode.MultiRow;
                        break;
                    case "scrollable":
                        this.DisplayMode = DisplayMode.Scrollable;
                        break;
                    case "singlerow":
                        this.DisplayMode = DisplayMode.SingleRow;
                        break;
                    default:
                        this.DisplayMode = DisplayMode.SingleRow;
                        break;
                }
            }

            XmlNode spaceOnLeft = xmlData.SelectSingleNode("//COE:SpaceOnLeft", manager);
            if (spaceOnLeft != null && spaceOnLeft.InnerText.Length > 0)
            {
                this.SpaceOnLeft = int.Parse(spaceOnLeft.InnerText);
            }

            XmlNode threeDEffect = xmlData.SelectSingleNode("//COE:ThreeDEffect", manager);
            if (threeDEffect != null && threeDEffect.InnerText.Length > 0)
            {
                this.ThreeDEffect = bool.Parse(threeDEffect.InnerText);
            }

            XmlNode spaceOnRight = xmlData.SelectSingleNode("//COE:SpaceOnRight", manager);
            if (spaceOnRight != null && spaceOnRight.InnerText.Length > 0)   //Coverity Fix CID 20249 ASV
            {
                this.SpaceOnRight = int.Parse(spaceOnRight.InnerText);
            }

            int counter = 0;
            string errorMsg;
            foreach (XmlNode tab in xmlData.SelectNodes("//COE:tabs/COE:tab", manager))
            {
                Infragistics.WebUI.UltraWebTab.Tab tabObject = new Infragistics.WebUI.UltraWebTab.Tab();
                tabObject.Text = tab.Attributes["name"].Value;

                if (tab.Attributes["tooltip"] != null)
                {
                    tabObject.Tooltip = tab.Attributes["tooltip"].Value;
                }

                this.Tabs.Add(tabObject);
                foreach (XmlNode control in tab.SelectNodes("./COE:controls/COE:control", manager))
                {
                    ICOEGenerableControl formElement = COEFormGenerator.GetCOEGenerableControl(control.SelectSingleNode("./COE:formElement", manager).OuterXml, out errorMsg);
                    FormGroup.FormElement messagingType = FormGroup.FormElement.GetFormElement(control.SelectSingleNode("./COE:formElement", manager).OuterXml);
                    if (!string.IsNullOrEmpty(messagingType.DefaultValue))
                        formElement.PutData(messagingType.DefaultValue);

                    formElements.Add(formElement, messagingType);

                    this.Tabs.GetTab(counter).ContentPane.Controls.Add((Control)formElement);
                }

                this.Tabs.GetTab(counter).ContentPane.Scrollable = OverflowType.Auto;

                counter = counter + 1;
            }

            _staticCounter = counter;

            XmlNode dynTab = xmlData.SelectSingleNode("//COE:dynamicTab", manager);
            if (dynTab != null && dynTab.InnerText.Length > 0)
            {
                if (dynTab.Attributes["dataMember"] != null && dynTab.Attributes["dataMember"].Value.Length > 0)
                    _tabsDataMember = dynTab.Attributes["dataMember"].Value;

                if (dynTab.Attributes["dataValue"] != null && dynTab.Attributes["dataValue"].Value.Length > 0)
                    _tabsDataValue = dynTab.Attributes["dataValue"].Value;

                Infragistics.WebUI.UltraWebTab.Tab tabObject = new Infragistics.WebUI.UltraWebTab.Tab();
                tabObject.Text = dynTab.Attributes["name"].Value;

                if (dynTab.Attributes["tooltip"] != null)
                {
                    tabObject.Tooltip = dynTab.Attributes["tooltip"].Value;
                }

                this.Tabs.Add(tabObject);
                foreach (XmlNode control in dynTab.SelectNodes("./COE:controls/COE:control", manager))
                {
                    ICOEGenerableControl formElement = COEFormGenerator.GetCOEGenerableControl(control.SelectSingleNode("./COE:formElement", manager).OuterXml, out errorMsg);
                    ((Control)formElement).ID = ((Control)formElement).ID + "T_T0";
                    ICOEGenerableControl dynamicFormElement = COEFormGenerator.GetCOEGenerableControl(control.SelectSingleNode("./COE:formElement", manager).OuterXml, out errorMsg);
                    ((Control)dynamicFormElement).ID = ((Control)dynamicFormElement).ID + "T_T0";

                    FormGroup.FormElement messagingType = FormGroup.FormElement.GetFormElement(control.SelectSingleNode("./COE:formElement", manager).OuterXml);
                    messagingType.Id = messagingType.Id + "T_T0";
                    FormGroup.FormElement dynamicMessagingType = FormGroup.FormElement.GetFormElement(control.SelectSingleNode("./COE:formElement", manager).OuterXml);
                    dynamicMessagingType.Id = dynamicMessagingType.Id + "T_T0";

                    if (!string.IsNullOrEmpty(messagingType.DefaultValue))
                        formElement.PutData(messagingType.DefaultValue);

                    formElements.Add(formElement, messagingType);
                    dynamicFormElements.Add(dynamicFormElement, dynamicMessagingType);

                    this.Tabs.GetTab(counter).ContentPane.Controls.Add((Control)formElement);
                }

                this.Tabs.GetTab(counter).ContentPane.Scrollable = OverflowType.Auto;

                counter = counter + 1;
            }

            _coeLog.LogEnd(methodSignature);
        }

        #endregion

        #region LyfeCycle overriden methods

        // Tried to clone tabs from OnInit and Render events but it doesn't work
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        #endregion

    }
}