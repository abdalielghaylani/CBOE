namespace Forms
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for Appointment1.
	/// </summary>
	public partial class AppointmentAddUC : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Label labelEndTime;
		protected Forms.ComboBox combobox1;
		protected Forms.ComboBox Combobox2;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			this.wdcStartTime.Attributes.CssStyle["height"]=" ";
			this.wdcStartTime.EditStyle.CustomRules="height:;";
			this.wdcEndTime.Attributes.CssStyle["height"]=" ";
			this.wdcEndTime.EditStyle.CustomRules="height:;";
            this.LocalizeStrings();
        }

        #region LocalizeStrings

        private void LocalizeStrings()
        {
            this.statusText.InnerHtml = Resources.strings.AppointmentDialog_RecurrenceStatus;
            this.SubjectLabel.InnerHtml = Resources.strings.AppointmentDialog_Subject;
            this.LocationLabel.InnerHtml = Resources.strings.AppointmentDialog_Location;
            this.recurrenceDescriptionText.InnerHtml = Resources.strings.AppointmentDialog_RecurrenceDescriptionText;
            this.StartTimeLabel.InnerHtml = Resources.strings.AppointmentDialog_StartTime;
            this.startdateLabel508.InnerHtml = Resources.strings.AppointmentDialog_StartDate_Section508;
            this.startTimeLabel508.InnerHtml = Resources.strings.AppointmentDialog_StartTime_Section508;
            this.AllDayEventLabel.InnerHtml = Resources.strings.AppointmentDialog_AllDayEvent;
            this.EndTimeLabel.InnerHtml = Resources.strings.AppointmentDialog_EndTime;
            this.endTimeLabel508.InnerHtml = Resources.strings.AppointmentDialog_EndTime_Section508;
            this.endDateLabel508.InnerHtml = Resources.strings.AppointmentDialog_EndDate_Section508;
            this.ReminderLabel.InnerHtml = Resources.strings.AppointmentDialog_Reminder;
            this.ReminderIntervalSection508.InnerHtml = Resources.strings.AppointmentDialog_ReminderInterval_Section508;
            this.ri_eightHours.InnerHtml = Resources.strings.AppointmentDialog_ReminderIntervalOption_EightHours;
            this.ri_fifteenMin.InnerHtml = Resources.strings.AppointmentDialog_ReminderIntervalOption_FifteenMinutes;
            this.ri_fiveMin.InnerHtml = Resources.strings.AppointmentDialog_ReminderIntervalOption_FiveMinutes;
            this.ri_fourHours.InnerHtml = Resources.strings.AppointmentDialog_ReminderIntervalOption_FourHours;
            this.ri_halfDay.InnerHtml = Resources.strings.AppointmentDialog_ReminderIntervalOption_HalfDay;
            this.ri_oneDay.InnerHtml = Resources.strings.AppointmentDialog_ReminderIntervalOption_OneDay;
            this.ri_oneHour.InnerHtml = Resources.strings.AppointmentDialog_ReminderIntervalOption_OneHour;
            this.ri_tenMin.InnerHtml = Resources.strings.AppointmentDialog_ReminderIntervalOption_TenMinutes;
            this.ri_thirtyMin.InnerHtml = Resources.strings.AppointmentDialog_ReminderIntervalOption_ThirtyMinutes;
            this.ri_twoDays.InnerHtml = Resources.strings.AppointmentDialog_ReminderIntervalOption_TowDays;
            this.ri_twoHours.InnerHtml = Resources.strings.AppointmentDialog_ReminderIntervalOption_TwoHours;
            this.ri_zeroMin.InnerHtml = Resources.strings.AppointmentDialog_ReminderIntervalOption_ZeroMinutes;
            this.ShowTimeAsLabel.Text = Resources.strings.AppointmentDialog_ShowTimeAs;
            this.showTimeAsLabelSection508.InnerHtml = Resources.strings.AppointmentDialog_ShowTimeAs_Section508;
            this.showTimeAs_Free.InnerHtml = Resources.strings.AppointmentDialog_ShowTimeAs_Free;
            this.showTimeAs_Busy.InnerHtml = Resources.strings.AppointmentDialog_ShowTimeAs_Busy;
            this.showTimeAs_OutofOffice.InnerHtml = Resources.strings.AppointmentDialog_ShowTimeAs_OutofOffice;
            this.showTimeAs_Tentative.InnerHtml = Resources.strings.AppointmentDialog_ShowTimeAs_Tentative;
            this.descriptionSection508.InnerHtml = Resources.strings.AppointmentDialog_Description_Section508;

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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
	}
}
