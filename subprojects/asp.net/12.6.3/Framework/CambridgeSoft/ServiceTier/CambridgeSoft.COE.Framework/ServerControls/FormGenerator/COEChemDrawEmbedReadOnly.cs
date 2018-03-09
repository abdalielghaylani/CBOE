using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Controls;
using CambridgeSoft.COE.Framework.Controls.ChemDraw;
using System.Xml;
[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// This is the read only version of <see cref="COEChemDraw"/> class.
    /// </para>
    /// <para>
    /// The COEChemDrawEmbedReadOnly class accepts every <see cref="COEChemDraw"/> property to be set, but as ICOEGenerable control it also provides
    /// GetData(), PutData(), DefaultValue and LoadFromXml() Methods.
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>defaultValue: What is the default base64?</item>
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
    /// &lt;formElement&gt;
    ///     &lt;defaultValue/&gt;
    ///     &lt;bindingExpression&gt;Compound.BaseFragment.Structure.Value&lt;/bindingExpression&gt;
    ///     &lt;configInfo&gt;
    ///         &lt;fieldConfig&gt;
    ///         &lt;CSSClass&gt;COEChemDrawView&lt;/CSSClass&gt;
    ///         &lt;ID&gt;BaseFragmentStructure&lt;/ID&gt;
    ///         &lt;Height&gt;300px&lt;/Height&gt;
    ///         &lt;Width&gt;300px&lt;/Width&gt;
    ///         &lt;/fieldConfig&gt;
    ///     &lt;/configInfo&gt;
    ///     &lt;displayInfo&gt;
    ///         &lt;height&gt;300px&lt;/height&gt;
    ///         &lt;width&gt;300px&lt;/width&gt;
    ///         &lt;top&gt;12px&lt;/top&gt;
    ///         &lt;left&gt;15px&lt;/left&gt;
    ///         &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEChemDrawEmbedReadOnly&lt;/type&gt;
    ///     &lt;/displayInfo&gt;
    /// &lt;/formElement>
    /// </code>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// In this implementation "Default Value", GetData and PutData Methods reffer to its base64 representation.
    /// </para>
    /// </summary>
    [ToolboxData("<{0}:COEChemDrawEmbedReadOnly runat=server></{0}:COEChemDrawEmbedReadOnly>")]
    public class COEChemDrawEmbedReadOnly : COEChemDraw
    {
        #region COEGenerableControl Members
        /// <summary>Loads its specific configuration from an xml in the form:
        /// <code lang="Xml">
        ///     &lt;configInfo&gt;
        ///         &lt;fieldConfig&gt;
        ///         &lt;CSSClass&gt;COEChemDraw&lt;/CSSClass&gt;
        ///         &lt;ID&gt;BaseFragmentStructure&lt;/ID&gt;
        ///         &lt;Height&gt;300px&lt;/Height&gt;
        ///         &lt;Width&gt;300px&lt;/Width&gt;
        ///         &lt;/fieldConfig&gt;
        ///     &lt;/configInfo&gt;
        /// </code>
        /// </summary>
        /// <param name="xmlDataAsString">The configInfo xml snippet</param>
        public override void LoadFromXml(string xmlDataAsString)
        {
            /*
             * <fieldConfig>
             *  <Style>border-color:blue;</Style>
             *  <Height></Height>
             *  <Width></Width>
             *  <ID>32</ID>
             * </fieldConfig>
             */
            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(xmlDataAsString);
            XmlNode viewOnlyNode = xmlData.CreateNode(XmlNodeType.Element, xmlData.DocumentElement.Prefix,"ViewOnly", xmlData.DocumentElement.NamespaceURI);
            viewOnlyNode.InnerText = "true";
            
            if(xmlData.DocumentElement.FirstChild == null)
                xmlData.DocumentElement.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlData.DocumentElement.Prefix, "fieldConfig", xmlData.DocumentElement.NamespaceURI));

            xmlData.DocumentElement.FirstChild.AppendChild(viewOnlyNode);
            base.LoadFromXml(xmlData.OuterXml);
        }
        #endregion
    }
}
