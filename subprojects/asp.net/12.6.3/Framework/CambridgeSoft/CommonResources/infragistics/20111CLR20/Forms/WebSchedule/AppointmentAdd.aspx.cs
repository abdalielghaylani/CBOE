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
			this.Title = Resources.strings.AppointmentTitle;
			this.UltraWebTab1.Tabs.GetTab(0).ContentPane.UserControlUrl = "AppointmentAdd.ascx";
			this.UltraWebTab1.CssClass = "Fonts BackgroundTab";
            this.LocalizeStrings();
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "AppointmentDialogScriptArray", this.GetJavascriptArray());
        }

        #region GetJavascriptArray

        private string GetJavascriptArray()
        {
            string array = "<script type='text/javascript'> \r\n var AppointmentDialogStrings = [" +
                                "'" + Resources.strings.JS_AppointmentDialog_Error_CantFindDateChooser + "'," +             //0
                                "'" + Resources.strings.JS_AppointmentDialog_RecurrenceDescriptionLabel + "'," +            //1
                                "'" + Resources.strings.JS_AppointmentDialog_Error_OccurrenceBeforeStartDate + "'," +       //2
                                "'" + Resources.strings.JS_AppointmentDialog_Error_OccurrenceSkipPrevOccurrence + "'," +    //3
                                "'" + Resources.strings.JS_AppointmentDialog_Error_OccurrenceAfterEndDay + "'," +           //4
                                "'" + Resources.strings.JS_AppointmentDialog_Error_OccurrenceSkipNextOccurrence + "'," +    //5
                                "'" + Resources.strings.JS_AppointmentDialog_ReccurrenceStatus + "'," +                     //6
                                "'" + Resources.strings.OneWeek + "'," +                                                    //7 
                                "'" + Resources.strings.Weeks + "'," +                                                      //8
                                "'" + Resources.strings.OneDay + "'," +                                                     //9
                                "'" + Resources.strings.Days + "'," +                                                       //10
                                "'" + Resources.strings.OneHour + "'," +                                                    //11
                                "'" + Resources.strings.Hours + "'," +                                                      //12
                                "'" + Resources.strings.OneMinute + "'," +                                                  //13
                                "'" + Resources.strings.Minutes + "'," +                                                    //14
                                "'" + Resources.strings.OneMinuteOrLess + "'" +                                             //15
                            "]; \r\n </script>";
                            
            return array;
        }

        #endregion

        #region LocalizeStrings()

        private void LocalizeStrings()
        {
            TBarButton saveButton =    this.UltraWebToolbar2.Items.FromKeyButton("Save");
            saveButton.Text = "<NOBR><img style='margin:0;' igimg='1' src= './Images/save.gif' align ='AbsMiddle' />&nbsp; " + Resources.strings.AppointmentDialog_Toolbar_SaveAndClose_Text + " &nbsp;</NOBR>";
            saveButton.ToolTip = Resources.strings.AppointmentDialog_Toolbar_SaveAndClose_Tooltip;
            saveButton.Images.DefaultImage.AlternateText = Resources.strings.AppointmentDialog_Toolbar_SaveAndClose_AltText;

            TBarButton printButton =   this.UltraWebToolbar2.Items.FromKeyButton("Print");
            printButton.ToolTip = Resources.strings.AppointmentDialog_Toolbar_Print_Tooltip;
            printButton.Images.DefaultImage.AlternateText = Resources.strings.AppointmentDialog_Toolbar_Print_AltText;

            TBarButton recurrenceButton =  this.UltraWebToolbar2.Items.FromKeyButton("Recurrence");
            recurrenceButton.Text = "<NOBR><img style='margin:0;' igimg='1' src= './Images/recur.gif' align ='AbsMiddle'/>&nbsp; " + Resources.strings.AppointmentDialog_Toolbar_Recurrence_Text + " &nbsp;</NOBR>";
            recurrenceButton.ToolTip = Resources.strings.AppointmentDialog_Toolbar_Recurrence_Tooltip;
            recurrenceButton.Images.DefaultImage.AlternateText = Resources.strings.AppointmentDialog_Toolbar_Recurrence_AltText;

            TBarButton deleteButton =  this.UltraWebToolbar2.Items.FromKeyButton("Delete");
            deleteButton.ToolTip = Resources.strings.AppointmentDialog_Toolbar_Delete_Tooltip;
            deleteButton.Images.DefaultImage.AlternateText = Resources.strings.AppointmentDialog_Toolbar_Delete_AltText;

            TBarButton highButton =    this.UltraWebToolbar2.Items.FromKeyButtonGroup("Importance").Buttons[0];
            highButton.ToolTip = Resources.strings.AppointmentDialog_Toolbar_HightPriority_Tooltip;
            highButton.Images.DefaultImage.AlternateText = Resources.strings.AppointmentDialog_Toolbar_HightPriority_AltText;

            TBarButton lowButton =     this.UltraWebToolbar2.Items.FromKeyButtonGroup("Importance").Buttons[1];
            lowButton.ToolTip = Resources.strings.AppointmentDialog_Toolbar_LowPriority_Tooltip;
            lowButton.Images.DefaultImage.AlternateText = Resources.strings.AppointmentDialog_Toolbar_LowPriority_AltText;

            this.UltraWebTab1.Tabs.GetTab(0).Text = Resources.strings.AppointmentDialog_AppointmentTab_Text;
        }

        #endregion

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
