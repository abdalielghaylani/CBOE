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

namespace Forms
{
	/// <summary>
	/// Summary description for Reminder.
	/// </summary>
	public partial class Reminder : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label Label2;
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
            this.LocalizeStrings();
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "ReminderDialogScriptArray", this.GetJavascriptArray());
        }

        #region GetJavascriptArray

        private string GetJavascriptArray()
        {
            string array = "<script type='text/javascript'> \r\n var ReminderDialogStrings = [" +                                
                                "'" + Resources.strings.OneWeek + "'," +                            //0
                                "'" + Resources.strings.Weeks + "'," +                              //1
                                "'" + Resources.strings.OneDay + "'," +                             //2
                                "'" + Resources.strings.Days + "'," +                               //3
                                "'" + Resources.strings.OneHour + "'," +                            //4
                                "'" + Resources.strings.Hours + "'," +                              //5
                                "'" + Resources.strings.OneMinute + "'," +                          //6
                                "'" + Resources.strings.Minutes + "'," +                            //7
                                "'" + Resources.strings.OneMinuteOrLess + "'," +                    //8
                                "'" + Resources.strings.ReminderDialog_NoSubject + "'," +           //9
                                "'" + Resources.strings.ReminderDialog_LocationLabel + "'," +       //10
                                "'" + Resources.strings.JS_ReminderDialog_Overdue + "'," +          //11
                                "'" + Resources.strings.JS_ReminderDialog_Now + "'" +               //12
                            "]; \r\n </script>";

            return array;
        }

        #endregion
        #region LocalizeStrings

        private void LocalizeStrings()
        {
			this.Title = Resources.strings.ReminderDialogTitle;
            this.Subject.InnerHtml = Resources.strings.ReminderDialog_NoSubject;
            this.LocationLabel.InnerHtml = Resources.strings.ReminderDialog_LocationLabel;
            this.TimeLabel.InnerHtml = Resources.strings.ReminderDialog_TimeLabel;
            this.SubjectHeaderLabel.InnerHtml = Resources.strings.ReminderDialog_SubjectHeader;
            this.DueInHeaderLable.InnerHtml = Resources.strings.ReminderDialog_DueInHeader;
            this.DismissAll.Value = Resources.strings.ReminderDialog_DimissAllButton;
            this.ClickSnoozeLabel.InnerHtml = Resources.strings.ReminderDialog_ClickSnoozeLabel;
            this.OpenItem.Value = Resources.strings.ReminderDialog_OpenItemButton;
            this.Dismiss.Value = Resources.strings.ReminderDialog_DismissButton;
            this.Snooze.Value = Resources.strings.ReminderDialog_SnoozeButton;
            this.CloseButton.Value = Resources.strings.ReminderDialog_CloseButton;
            this.fiveMinutesOption.InnerHtml = "5 " + Resources.strings.Minutes;
            this.tenMinutesOption.InnerHtml = "10 " + Resources.strings.Minutes;
            this.fifteenMinutesOption.InnerHtml = "15 " + Resources.strings.Minutes;
            this.oneHourOption.InnerHtml = "1 " + Resources.strings.OneHour;
            this.twoHoursOption.InnerHtml = "2 " + Resources.strings.Hours;
            this.fourHoursOption.InnerHtml = "4 " + Resources.strings.Hours;
            this.eightHoursOption.InnerHtml = "8 " + Resources.strings.Hours;
            this.oneDayOption.InnerHtml = "1 " + Resources.strings.OneDay;
            this.twoDaysOption.InnerHtml = "2 " + Resources.strings.Days;
            this.threeDaysOption.InnerHtml = "3 " + Resources.strings.Days;
            this.oneWeekOption.InnerHtml = "1 " + Resources.strings.OneWeek;
            this.twoWeeksOption.InnerHtml = "2 " + Resources.strings.Weeks;
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

