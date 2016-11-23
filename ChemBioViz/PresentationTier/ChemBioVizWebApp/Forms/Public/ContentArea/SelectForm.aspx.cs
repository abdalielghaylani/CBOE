using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COEFormService;
using System.Reflection;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common;

public partial class Forms_ContentArea_SelectForm : GUIShellPage
{
	#region Properties

	/// <summary>
	/// Current DataViewBOList. Use this for easier access
	/// </summary>
	private COEFormBOList FormBOList
	{
		get
		{
			return GetFormBOList();
		}
	}

	private void ShowFormBOList()
	{
		
		if (FormBOList.Count > 0)
		{
			string searchPath = "~/Forms/Search/ContentArea/ChemBioVizSearch.aspx?AllowFullScan=true&FormGroupId=";
			foreach (COEFormBO coeForm in FormBOList)
			{
				HyperLink formLink = new HyperLink();
				formLink.Text = coeForm.Name;
				formLink.NavigateUrl = searchPath + coeForm.ID.ToString();

				LiteralControl descLiteral = new LiteralControl(" - " + coeForm.Description);
				LiteralControl brLiteral = new LiteralControl("<br />");

				FormListPH.Controls.Add(formLink);
				FormListPH.Controls.Add(descLiteral);
				FormListPH.Controls.Add(brLiteral);
			
			}
		}
		
	}

    private string ApplicationName {
        get {
            if(!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AppName"]))
                return ConfigurationManager.AppSettings["AppName"];

            return null;
        }
    }

	private COEFormBOList GetFormBOList()
	{
		Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
		COEFormBOList FormBOList = COEFormBOList.GetCOEFormBOList(COEUser.Name, null, this.ApplicationName, 1, true);
		Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
		return FormBOList;
	}

	#endregion

	#region Events Handlers

	protected void Page_Load(object sender, EventArgs e)
	{
		Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
		if (!Page.IsPostBack)
		{
			this.SetControlsAttributtes();
		}
	
		//JHS 8/14/2008 - Yes I have no idea what I am doing
		//ShowFormBOList();

		//JHS 10/14/2008
		//change to grid view instead of custom code for now
        COEFormBOList formList = GetFormBOList();

        if (formList.Count > 0)
        {
            GridView1.DataSource = formList;
            GridView1.DataBind();
        }
        else
        {
            LiteralControl textLiteral = new LiteralControl("<br /> No web forms have been loaded to the database.  Please use config loader to add new forms.");
            FormListPH.Controls.Add(textLiteral);
        }


		Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
	}

	protected override void OnInit(EventArgs e)
	{
		Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.Master.ShowLeftPanel = false;
		base.OnInit(e);
		Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
	}
	#endregion

	#region GUIShell Methods

	/// <summary>
	/// This method sets all the controls attributtes as Text, etc...
	/// </summary>
	protected override void SetControlsAttributtes()
	{
		Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
		this.Master.SetPageTitle("ChemBioViz: Select a Search Form");
		Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
	}
	#endregion
}
