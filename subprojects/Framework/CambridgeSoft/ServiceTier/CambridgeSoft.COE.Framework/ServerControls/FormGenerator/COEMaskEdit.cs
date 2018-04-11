using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Xml;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    [ToolboxData("<{0}:COEMaskEdit runat=server></{0}:COEMaskEdit>")]
    public class COEMaskEdit : Infragistics.WebUI.WebDataInput.WebMaskEdit, ICOEGenerableControl, ICOELabelable
    {

        #region Variables
        
        private string _defaultValue = string.Empty;
        private Label _lit = new Label();
        
        #endregion

        #region ICOEGenerableControl Members

        public object GetData()
        {
            return this.Text;
        }

        public void PutData(object data)
        {
            this.Text = data.ToString();
        }

        public void LoadFromXml(string xmlDataAsString)
        {
            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(xmlDataAsString);

            XmlNamespaceManager manager = new XmlNamespaceManager(xmlData.NameTable);
            manager.AddNamespace("COE", xmlData.DocumentElement.NamespaceURI);

            XmlNode width = xmlData.SelectSingleNode("//COE:Width", manager);
            if (width != null && width.InnerText.Length > 0)
                this.Width = new Unit(width.InnerText);
          
            XmlNode height = xmlData.SelectSingleNode("//COE:Height", manager);
            if (height != null && height.InnerText.Length > 0)
                this.Height = new Unit(height.InnerText);
          
            XmlNode cssClass = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if (cssClass != null && cssClass.InnerText.Length > 0)
                this.CssClass = cssClass.InnerText;

            XmlNode cssLabelClass = xmlData.SelectSingleNode("//COE:CSSLabelClass", manager);
            if (cssLabelClass != null && cssLabelClass.InnerText.Length > 0)
                _lit.CssClass = cssLabelClass.InnerText;

            XmlNode inputMask = xmlData.SelectSingleNode("//COE:InputMask", manager);
            if (inputMask != null && inputMask.InnerText.Length > 0)
                this.InputMask = inputMask.InnerText;

            XmlNode toolTip = xmlData.SelectSingleNode("//COE:ToolTip", manager);
            if (toolTip != null && toolTip.InnerText.Length > 0)
                this.ToolTip = toolTip.InnerText;

            XmlNode display = xmlData.SelectSingleNode("//COE:ReadOnly", manager);
            if (display != null && display.InnerText.Length > 0)
                this.ReadOnly = ((((string)display.InnerText).Trim()).ToLower()).Equals("true") ? true : false;
          
        }

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

        #region ICOELabelable Members

        public string Label
        {
            get
            {
                if (ViewState[Constants.Label_VS] != null)
                    return (string)ViewState[Constants.Label_VS];
                else
                    return string.Empty;
            }
            set
            {
                ViewState[Constants.Label_VS] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Control's label CSS styles attributes.
        /// </summary>
        public Dictionary<string, string> LabelStyles
        {
            get
            {
                if (ViewState[Constants.LabelStyles_VS] != null)
                    return (Dictionary<string, string>)ViewState[Constants.LabelStyles_VS];
                else
                    return null;
            }
            set
            {
                ViewState[Constants.LabelStyles_VS] = value;
            }
        }


        #endregion

        #region Control life cycle

        protected override void Render(HtmlTextWriter output)
        {
            if (!string.IsNullOrEmpty(this.Label))
            {
                _lit.Text = this.Label;
                if (this.LabelStyles != null)
                {
                    IEnumerator styles = this.LabelStyles.Keys.GetEnumerator();
                    while (styles.MoveNext())
                    {
                        string key = (string)styles.Current;
                        _lit.Style.Add(key, (string)this.LabelStyles[key]);
                    }
                }
                _lit.Style.Add(HtmlTextWriterStyle.Display, "block");
                _lit.RenderControl(output);
                this.Style.Add(HtmlTextWriterStyle.Display, "block");
            }
            base.Render(output);
        }

        #endregion
    }
}
