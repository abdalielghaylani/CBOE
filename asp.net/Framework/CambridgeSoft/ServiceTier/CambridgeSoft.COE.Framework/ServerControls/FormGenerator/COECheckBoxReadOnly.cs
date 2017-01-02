using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Xml;
using System.ComponentModel;
using System.Web.UI.WebControls;
[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// This is the read only version of <see cref="COECheckBox"/>.
    /// </para>
    /// <para>
    /// The COECheckBoxReadOnly class accepts every CheckBox property to be set, but as ICOEGenerable control it also provides
    /// GetData(), PutData(), DefaultValue and LoadFromXml() Methods.
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>defaultValue: Would be checked by default?</item>
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
    ///     &lt;formElement&gt;
    ///       &lt;defaultValue&gt;true&lt;/defaultValue&gt;
    ///       &lt;bindingExpression&gt;Compound.BaseFragment.Structure.UseNormalizedStructure&lt;/bindingExpression&gt;
    ///       &lt;label&gt;Use normalized structure&lt;/label&gt;
    ///       &lt;configInfo&gt;
    ///         &lt;fieldConfig&gt;
    ///         &lt;CSSClass&gt;COELabelView&lt;/CSSClass&gt;
    ///         &lt;ID&gt;UseNormalizedStructureCheck&lt;/ID&gt;
    ///         &lt;/fieldConfig&gt;
    ///       &lt;/configInfo&gt;
    ///       &lt;displayInfo&gt;
    ///         &lt;top&gt;310px&lt;/top&gt;
    ///         &lt;left&gt;100px&lt;/left&gt;
    ///         &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COECheckBoxReadOnly&lt;/type&gt;
    ///       &lt;/displayInfo&gt;
    ///     &lt;/formElement&gt;
    /// </code>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// In this implementation "Default Value", GetData and PutData Methods reffer to the Checked property.
    /// </para>
    /// </summary>
    [ToolboxData("<{0}:COECheckBoxReadOnly runat=server></{0}:COECheckBoxReadOnly>")]
    public class COECheckBoxReadOnly : COECheckBox
    {
        
        protected override void OnPreRender(EventArgs e)
        {
            string str = "<script language='javascript' type='text/javascript'>"
                   + "function noChange(item)"
                   + "{"
                   + "var checkbox=document.getElementById(item.id);"
                   + "checkbox.checked=! checkbox.checked;"
                   + "}"
                   + "</script>";
            Page.RegisterClientScriptBlock("CheckboxDisplayOnly", str);
            this.Attributes.Add("onclick", "noChange(this)");
            base.OnPreRender(e);
        }

    }
}
