using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.UltraWebToolbar;

namespace Forms
{
	public partial class AppointmentAdd : System.Web.UI.Page
	{
		protected Forms.ComboBox combobox1;
	
		protected void Page_Load(object sender, System.EventArgs e)
		{						
			//Infragistics.WebUI.WebDialog.WebDialogManager wdm = new Infragistics.WebUI.WebDialog.WebDialogManager();
			this.UltraWebTab1.Tabs.GetTab(0).ContentPane.UserControlUrl = "AppointmentAdd.ascx";
			this.UltraWebTab1.CssClass = "Fonts BackgroundTab";

			this.UltraWebToolbar2.Items.FromKeyButton("Save").Images.DefaultImage.AlternateText = "Save and Close"; 
			this.UltraWebToolbar2.Items.FromKeyButton("Print").Images.DefaultImage.AlternateText = "Print"; 
			this.UltraWebToolbar2.Items.FromKeyButton("Recurrence").Images.DefaultImage.AlternateText = "Recurrence"; 
			this.UltraWebToolbar2.Items.FromKeyButton("Delete").Images.DefaultImage.AlternateText = "Delete"; 
			this.UltraWebToolbar2.Items.FromKeyButtonGroup("Importance").Buttons[0].Images.DefaultImage.AlternateText = "High";
			this.UltraWebToolbar2.Items.FromKeyButtonGroup("Importance").Buttons[1].Images.DefaultImage.AlternateText = "Low";
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion
	}
}
