using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;
using System.Text;
namespace Forms
{
	/// <summary>
	/// Summary description for ComboBox.
	/// </summary>
	[ToolboxData("<{0}:ComboBox runat=server></{0}:ComboBox>")]
	public class ComboBox : System.Web.UI.WebControls.WebControl
	{

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			Infragistics.WebUI.Shared.Util.ClientScript.RegisterCommonScriptResource(Page, "/ig_common/" + Infragistics.WebUI.Shared.AssemblyVersion.Build + "/scripts/ig_shared.js");

			Page.RegisterClientScriptBlock("ig_comboBox.js", "<script type=\"text/javascript\" src=\"./Scripts/ig_comboBox.js\"></script>");

			#region Render JavaScript

			StringBuilder sbjs = new StringBuilder();
			sbjs.Append("<script language=javascript><!--\n");
			sbjs.Append("try{var o" + this.ClientID + " = ig_CreateComboBox(\"" + this.ClientID + "\");}catch(e){status=\"Error initializing the comboBox.\";}\n");
			sbjs.Append("--></script>\n");
			Page.RegisterStartupScript(this.ClientID + "_script", sbjs.ToString());

			#endregion
		}

		protected override void Render(HtmlTextWriter output)
		{
			output.AddAttribute(System.Web.UI.HtmlTextWriterAttribute.Cellpadding, "0");
			output.AddAttribute(System.Web.UI.HtmlTextWriterAttribute.Cellspacing, "0");
			output.AddAttribute(System.Web.UI.HtmlTextWriterAttribute.Id, this.ClientID);
			output.AddAttribute("ig_mark", "");
			output.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Table);
			output.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Tr);
			output.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Td);
			output.AddAttribute(System.Web.UI.HtmlTextWriterAttribute.Id, this.ClientID + "_inputbox");
			output.AddAttribute(System.Web.UI.HtmlTextWriterAttribute.Class, "Fonts");
			output.AddAttribute(System.Web.UI.HtmlTextWriterAttribute.Tabindex, this.TabIndex.ToString());
			if (this.Width != Unit.Empty)
				output.AddStyleAttribute("width", this.Width.ToString());
			output.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Input);
			output.RenderEndTag();			// Input
			output.RenderEndTag();			// Td
			output.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Td);
			output.WriteLine("<BUTTON style='WIDTH: 15px; HEIGHT: 20px' onfocus='blur()' type='button' ID='" + this.ClientID + "_button'>");
			output.Write("<img src = './Images/downarrow.gif'>");
			output.WriteLine("</BUTTON>");
			output.RenderEndTag();			// TD
			output.RenderEndTag();			// Tr
			output.RenderEndTag();			// Table

		}
	}



}