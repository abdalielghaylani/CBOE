using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Xml;
using System.Web.UI.HtmlControls;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// This class implements a LinkButton control that may be used inside a <see cref="COEFormGenerator"/>.
    /// </para>
    /// <para>
    /// The COELinkButton class accepts every LinkButton property to be set, but as ICOEGenerable control it also provides
    /// GetData(), PutData(), DefaultValue and LoadFromXml() Methods.
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>defaultValue: What is the default text?</item>
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
    ///   &lt;formElement&gt;
    ///       &lt;defaultValue&gt;Resolve This Compound&lt;/defaultValue&gt;
    ///       &lt;bindingExpression/&gt;
    ///       &lt;serverEvents&gt;
    ///         &lt;event eventName="Click" handlerName="ResolveLinkButton_Click"/&gt;
    ///       &lt;/serverEvents&gt;
    ///       &lt;configInfo&gt;
    ///         &lt;fieldConfig&gt;
    ///         &lt;CSSClass&gt;COELabelView&lt;/CSSClass&gt;
    ///         &lt;ID&gt;ResolveLinkButton&lt;/ID&gt;
    ///         &lt;/fieldConfig&gt;
    ///       &lt;/configInfo&gt;
    ///       &lt;displayInfo&gt;
    ///         &lt;top&gt;325px&lt;/top&gt;
    ///         &lt;left&gt;100px&lt;/left&gt;
    ///         &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELinkButton&lt;/type&gt;
    ///       &lt;/displayInfo&gt;
    ///     &lt;/formElement&gt;
    ///   &lt;/formElements&gt;
    /// </code>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// In this implementation "Default Value", GetData and PutData Methods reffer to the Text property.
    /// </para>
    /// </summary>
    [ToolboxData("<{0}:COEComplexLinkButton runat=server></{0}:COEComplexLinkButton>")]
    public class COEComplexLink : HyperLink, ICOEGenerableControl
    {
        #region Variables
        private string _textParamName = String.Empty;
        private string _urlParamName = String.Empty;
        private string _paramsSeparator = "|"; //This can be read by configuration 
        private string _keySeparator = "=";
        private string _defaultValue = string.Empty;

        #endregion

        #region ICOEGenerableControl
        /// <summary>
        /// <para>Gets the HRef of the link.</para>
        /// </summary>
        /// <returns>A string with the link's HRef</returns>
        public object GetData()
        {
            return this.NavigateUrl;
        }

        /// <summary>
        /// <para>Sets the link's HRef and text</para>
        /// </summary>
        /// <param name="data">A string with a concat of the text and url to display</param>
        public void PutData(object data)
        {
            if (!string.IsNullOrEmpty(data.ToString()))
            {
                this.Text = this.ToolTip = this.FindParam(data.ToString(), _textParamName);
                this.NavigateUrl = this.FindParam(data.ToString(), _urlParamName);
            }
        }

        /// <summary>Loads its specific configuration from an xml in the form:
        /// <code lang="Xml">
        ///   &lt;fieldConfig&gt;
        ///   &lt;CSSClass&gt;COELabelView&lt;/CSSClass&gt;
        ///   &lt;ID&gt;ResolveLinkButton&lt;/ID&gt;
        ///   &lt;/fieldConfig&gt;
        /// </code>
        /// </summary>
        /// <param name="xmlDataAsString">The configInfo xml snippet</param>
        public void LoadFromXml(string xmlDataAsString)
        {
            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(xmlDataAsString);

            XmlNamespaceManager manager = new XmlNamespaceManager(xmlData.NameTable);
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

            XmlNode cssClass = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if (cssClass != null && !string.IsNullOrEmpty(cssClass.InnerText))
                this.Attributes["class"] = cssClass.InnerText;

            XmlNode target = xmlData.SelectSingleNode("//COE:Target", manager);
            if (target != null && !string.IsNullOrEmpty(target.InnerText))
                this.Target = target.InnerText;

            //In case there is not test to display, show this default text.
            XmlNode defaultText = xmlData.SelectSingleNode("//COE:DefaultTextToDisplay", manager);
            if (defaultText != null && !string.IsNullOrEmpty(defaultText.InnerText))
                this.Text = this.ToolTip = defaultText.InnerText;

            //In case there is no URL to display, set this default URL
            XmlNode defaultURL = xmlData.SelectSingleNode("//COE:DefaultURLToDisplay", manager);
            if (defaultURL != null && !string.IsNullOrEmpty(defaultURL.InnerText))
                this.NavigateUrl = defaultURL.InnerText;

            //Name of the param that contains the text to display
            XmlNode textParamName = xmlData.SelectSingleNode("//COE:TextParamName", manager);
            if (textParamName != null && !string.IsNullOrEmpty(textParamName.InnerText))
                this._textParamName= textParamName.InnerText;

            //Name of the param that contains the url to set
            XmlNode URLParamName = xmlData.SelectSingleNode("//COE:URLParamName", manager);
            if (URLParamName != null && !string.IsNullOrEmpty(URLParamName.InnerText))
                this._urlParamName = URLParamName.InnerText;
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

        #region Methods

        /// <summary>
        /// Find a param given a logn string.
        /// </summary>
        /// <param name="sourceText">Full string to search for the param</param>
        /// <param name="paramName">Param name to search</param>
        /// <returns></returns>
        private string FindParam(string sourceText, string paramName)
        {
            string retVal = String.Empty;
            if (!string.IsNullOrEmpty(sourceText) && !string.IsNullOrEmpty(paramName))
            {
                if (sourceText.Contains(paramName))
                {
                    string[] keys = sourceText.Split(_paramsSeparator.ToCharArray());
                    foreach (string key in keys)
                    {
                        if (key.Contains(paramName))
                        {
                            retVal = key.Substring(key.IndexOf(_keySeparator) + 1);
                            break;
                        }
                    }
                }
            }
            return retVal;
        }
        #endregion
    }
}
