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
    /// Summary description for Recurrence.
    /// </summary>
    public partial class Recurrence : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.RadioButton RadioButton1;
        protected System.Web.UI.WebControls.RadioButtonList RadioButtonList1;

        private void Page_Load(object sender, System.EventArgs e)
        {
			this.Title = Resources.strings.RecurrenceDialogTitle;
            this.wdcEndTime.AllowNull = this.wdcStartTime.AllowNull = this.wdcEndRecurrence.AllowNull = false;
            this.wdcStartTime.Attributes.CssStyle["height"] = " ";
            this.wdcStartTime.EditStyle.CustomRules = "height:;";
            this.wdcEndTime.Attributes.CssStyle["height"] = " ";
            this.wdcEndTime.EditStyle.CustomRules = "height:;";
            this.wdcEndRecurrence.Attributes.CssStyle["height"] = " ";
            this.wdcEndRecurrence.EditStyle.CustomRules = "height:;";
            this.LocalizeStrings();
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "RecurrenceDialogScriptArray", this.GetJavascriptArray());
        }

        #region GetJavascriptArray

        private string GetJavascriptArray()
        {
            string array = "<script type='text/javascript'> \r\n var RecurrenceDialogStrings = [" +
                                "'" + Resources.strings.JS_RecurrenceDialog_InvalidPattern + "'," +
                                "'" + Resources.strings.JS_RecurrenceDialog_LooseDataConfirmation + "'" +
                            "]; \r\n </script>";

            return array;
        }

        #endregion

        #region LocalizeStrings
        private void LocalizeStrings()
        {
            this.AppointmentTimeLegend.InnerHtml = Resources.strings.RecurrenceDialog_AppointmentTimeLegend;
            this.StartLabel.InnerHtml = this.StartLabel2.InnerHtml = Resources.strings.RecurrenceDialog_Start_Label;
            this.EndDateLabel.InnerHtml = Resources.strings.RecurrenceDialog_EndDateLabel;
            this.EndTimeLabel.InnerHtml = Resources.strings.RecurrenceDialog_EndTimeLabel;
            this.RecurrencePatternLegend.InnerHtml = Resources.strings.RecurrenceDialog_RecurrencePatterLegend;
            this.radDaily.Value = this.radDailySection508.InnerHtml = Resources.strings.RecurrenceDialog_Daily;
            this.radWeekly.Value = this.radWeeklySection508.InnerHtml = Resources.strings.RecurrenceDialog_Weekly;
            this.radMonthly.Value = this.radMonthlySection508.InnerHtml = Resources.strings.RecurrenceDialog_Monthly;
            this.radYearly.Value = this.radYearlySection508.InnerHtml = Resources.strings.RecurrenceDialog_Yearly;
            this.EveryLabel.InnerHtml = this.yearlyEveryLabel.InnerHtml = this.radDayOf.Value = Resources.strings.RecurrenceDialog_Every;
            this.DaysLabel.InnerHtml = Resources.strings.RecurrenceDialog_Days;
            this.EveryWeekDayLabel.InnerHtml = Resources.strings.RecurrenceDialog_EveryWeekDay;
            this.RecursEveryLabel.InnerHtml = Resources.strings.RecurrenceDialog_RecursEvery;
            this.WeeksOnLabel.InnerHtml = Resources.strings.RecurrenceDialog_WeeksOn;
            this.monthlySundayOption.Attributes["title"] = this.yearlySundayOption.Attributes["title"] = this.cbSundaySection508.InnerHtml = this.monthlySundayOption.InnerHtml = this.yearlySundayOption.InnerHtml = Resources.strings.RecurrenceDialog_Sunday;
            this.monthlyMondayOption.Attributes["title"] = this.yearlyMondayOption.Attributes["title"] = this.cbMondaySection508.InnerHtml = this.monthlyMondayOption.InnerHtml = this.yearlyMondayOption.InnerHtml = Resources.strings.RecurrenceDialog_Monday;
            this.monthlyTuesdayOption.Attributes["title"] = this.yearlyTuesdayOption.Attributes["title"] = this.cbTuesdaySection508.InnerHtml = this.monthlyTuesdayOption.InnerHtml = this.yearlyTuesdayOption.InnerHtml = Resources.strings.RecurrenceDialog_Tuesday;
            this.monthlyWednesdayOption.Attributes["title"] = this.yearlyWednesdayOption.Attributes["title"] = this.cbWednesdaySection508.InnerHtml = this.monthlyWednesdayOption.InnerHtml = this.yearlyWednesdayOption.InnerHtml = Resources.strings.RecurrenceDialog_Wednesday;
            this.monthlyThursdayOption.Attributes["title"] = this.yearlyThursdayOption.Attributes["title"] = this.cbThursdaySection508.InnerHtml = this.monthlyThursdayOption.InnerHtml = this.yearlyThursdayOption.InnerHtml = Resources.strings.RecurrenceDialog_Thursday;
            this.monthlyFridayOption.Attributes["title"] = this.yearlyFridayOption.Attributes["title"] = this.cbFridaySection508.InnerHtml = this.monthlyFridayOption.InnerHtml = this.yearlyFridayOption.InnerHtml = Resources.strings.RecurrenceDialog_Friday;
            this.monthlySaturdayOption.Attributes["title"] = this.yearlySaturdayOption.Attributes["title"] = this.cbSaturdaySection508.InnerHtml = this.monthlySaturdayOption.InnerHtml = this.yearlySaturdayOption.InnerHtml = Resources.strings.RecurrenceDialog_Saturday;
            this.DayLabel.InnerHtml = Resources.strings.RecurrenceDialog_Day;
            this.radXOf.Value = this.TheLabel.InnerHtml = Resources.strings.RecurrenceDialog_The;
            this.monthlyFirstOption.Attributes["title"] = this.yearlyFirstOption.Attributes["title"] = this.monthlyFirstOption.InnerHtml = this.yearlyFirstOption.InnerHtml = Resources.strings.RecurrenceDialog_First;
            this.monthlySecondOption.Attributes["title"] = this.yearlySecondOption.Attributes["title"] = this.monthlySecondOption.InnerHtml = this.yearlySecondOption.InnerHtml = Resources.strings.RecurrenceDialog_Second;
            this.monthlyThirdOption.Attributes["title"] = this.yearlyThirdOption.Attributes["title"] = this.monthlyThirdOption.InnerHtml = this.yearlyThirdOption.InnerHtml = Resources.strings.RecurrenceDialog_Third;
            this.monthlyFourthOption.Attributes["title"] = this.yearlyFourthOption.Attributes["title"] = this.monthlyFourthOption.InnerHtml = this.yearlyFourthOption.InnerHtml = Resources.strings.RecurrenceDialog_Fourth;
            this.monthlyLastOption.Attributes["title"] = this.yearlyLastOption.Attributes["title"] = this.monthlyLastOption.InnerHtml = this.yearlyLastOption.InnerHtml = Resources.strings.RecurrenceDialog_Last;
            this.monthlyDayOption.Attributes["title"] = this.yearlyDayOption.Attributes["title"] = this.monthlyDayOption.InnerHtml = this.yearlyDayOption.InnerHtml = Resources.strings.RecurrenceDialog_day_lowercase;
            this.monthlyWeekDayOption.Attributes["title"] = this.yearlyWeekdayOption.Attributes["title"] = this.monthlyWeekDayOption.InnerHtml = this.yearlyWeekdayOption.InnerHtml = Resources.strings.RecurrenceDialog_Weekday;
            this.monthlyWeekEndDayOption.Attributes["title"] = this.yearlyWeekendDayOption.Attributes["title"] = this.monthlyWeekEndDayOption.InnerHtml = this.yearlyWeekendDayOption.InnerHtml = Resources.strings.RecurrenceDialog_WeekendDay;
            this.OfEveryLabel.InnerHtml = Resources.strings.RecurrenceDialog_OfEvery;
            this.MonthsLabel.InnerHtml = Resources.strings.RecurrenceDialog_Months;
            this.yearlyJanuaryOption.Attributes["title"] = this.yearlyJanuaryOption2.Attributes["title"] = this.yearlyJanuaryOption.InnerHtml = this.yearlyJanuaryOption2.InnerHtml = Resources.strings.RecurrenceDialog_January;
            this.yearlyFebruaryOption.Attributes["title"] = this.yearlyFebruaryOption2.Attributes["title"] = this.yearlyFebruaryOption.InnerHtml = this.yearlyFebruaryOption2.InnerHtml = Resources.strings.RecurrenceDialog_February;
            this.yearlyMarchOption.Attributes["title"] = this.yearlyMarchOption2.Attributes["title"] = this.yearlyMarchOption.InnerHtml = this.yearlyMarchOption2.InnerHtml = Resources.strings.RecurrenceDialog_March;
            this.yearlyAprilOption.Attributes["title"] = this.yearlyAprilOption2.Attributes["title"] = this.yearlyAprilOption.InnerHtml = this.yearlyAprilOption2.InnerHtml = Resources.strings.RecurrenceDialog_April;
            this.yearlyMayOption.Attributes["title"] = this.yearlyMayOption2.Attributes["title"] = this.yearlyMayOption.InnerHtml = this.yearlyMayOption2.InnerHtml = Resources.strings.RecurrenceDialog_May;
            this.yearlyJuneOption.Attributes["title"] = this.yearlyJuneOption2.Attributes["title"] = this.yearlyJuneOption.InnerHtml = this.yearlyJuneOption2.InnerHtml = Resources.strings.RecurrenceDialog_June;
            this.yearlyJulyOption.Attributes["title"] = this.yearlyJulyOption2.Attributes["title"] = this.yearlyJulyOption.InnerHtml = this.yearlyJulyOption2.InnerHtml = Resources.strings.RecurrenceDialog_July;
            this.yearlyAugustOption.Attributes["title"] = this.yearlyAugustOption2.Attributes["title"] = this.yearlyAugustOption.InnerHtml = this.yearlyAugustOption2.InnerHtml = Resources.strings.RecurrenceDialog_August;
            this.yearlySeptemberOption.Attributes["title"] = this.yearlySeptemberOption2.Attributes["title"] = this.yearlySeptemberOption.InnerHtml = this.yearlySeptemberOption2.InnerHtml = Resources.strings.RecurrenceDialog_September;
            this.yearlyOctoberOption.Attributes["title"] = this.yearlyOctoberOption2.Attributes["title"] = this.yearlyOctoberOption.InnerHtml = this.yearlyOctoberOption2.InnerHtml = Resources.strings.RecurrenceDialog_October;
            this.yearlyNovemberOption.Attributes["title"] = this.yearlyNovemberOption2.Attributes["title"] = this.yearlyNovemberOption.InnerHtml = this.yearlyNovemberOption2.InnerHtml = Resources.strings.RecurrenceDialog_November;
            this.yearlyDecemberOption.Attributes["title"] = this.yearlyDecemberOption2.Attributes["title"] = this.yearlyDecemberOption.InnerHtml = this.yearlyDecemberOption2.InnerHtml = Resources.strings.RecurrenceDialog_December;
            this.RangeOfRecurrenceLegend.InnerHtml = Resources.strings.RecurrenceDialog_RangeofRecurrence;
            this.NoEndDateLabel.InnerHtml = Resources.strings.RecurrenceDialog_NoEndDate;
            this.EndAfterLabel.InnerHtml = Resources.strings.RecurrenceDialog_EndAfter;
            this.OccurrencesLabel.InnerHtml = Resources.strings.RecurrenceDialog_Occurrences;
            this.EndByLabel.InnerHtml = Resources.strings.RecurrenceDialog_EndBy;
            this.buttonOK.InnerHtml = Resources.strings.RecurrenceDialog_OkButton;
            this.buttonCancel.InnerHtml = Resources.strings.RecurrenceDialog_CancelButton;
            this.buttonRemoveRecurrence.InnerHtml = Resources.strings.RecurrenceDialog_RemoveRecurrenceButton;
            this.Section508_1.InnerHtml = Resources.strings.RecurrenceDialog_Section508_1;
            this.Section508_2.InnerHtml = Resources.strings.RecurrenceDialog_Section508_2;
            this.Section508_3.InnerHtml = Resources.strings.RecurrenceDialog_Section508_3;
            this.Section508_4.InnerHtml = Resources.strings.RecurrenceDialog_Section508_4;
            this.Section508_5.InnerHtml = Resources.strings.RecurrenceDialog_Section508_5;
            this.Section508_6.InnerHtml = Resources.strings.RecurrenceDialog_Section508_6;
            this.Section508_7.InnerHtml = Resources.strings.RecurrenceDialog_Section508_7;
            this.Section508_8.InnerHtml = Resources.strings.RecurrenceDialog_Section508_8;
            this.Section508_9.InnerHtml = Resources.strings.RecurrenceDialog_Section508_9;
            this.Section508_10.InnerHtml = Resources.strings.RecurrenceDialog_Section508_10;
            this.Section508_11.InnerHtml = Resources.strings.RecurrenceDialog_Section508_11;
            this.Section508_12.InnerHtml = Resources.strings.RecurrenceDialog_Section508_12;
            this.Section508_13.InnerHtml = Resources.strings.RecurrenceDialog_Section508_13;
            this.Section508_14.InnerHtml = Resources.strings.RecurrenceDialog_Section508_14;
            this.Section508_15.InnerHtml = Resources.strings.RecurrenceDialog_Section508_15;
            this.Section508_16.InnerHtml = Resources.strings.RecurrenceDialog_Section508_16;
            this.Section508_17.InnerHtml = Resources.strings.RecurrenceDialog_Section508_17;
            this.Section508_18.InnerHtml = Resources.strings.RecurrenceDialog_Section508_18;
            this.Section508_19.InnerHtml = Resources.strings.RecurrenceDialog_Section508_19;
            this.Section508_20.InnerHtml = Resources.strings.RecurrenceDialog_Section508_20;
            this.Section508_21.InnerHtml = Resources.strings.RecurrenceDialog_Section508_21;
            this.Section508_22.InnerHtml = Resources.strings.RecurrenceDialog_Section508_22;
            this.Section508_23.InnerHtml = Resources.strings.RecurrenceDialog_Section508_23;
            this.Section508_24.InnerHtml = Resources.strings.RecurrenceDialog_Section508_24;
            this.Section508_25.InnerHtml = Resources.strings.RecurrenceDialog_Section508_25;
            this.Section508_26.InnerHtml = Resources.strings.RecurrenceDialog_Section508_26;


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
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion
    }
}
