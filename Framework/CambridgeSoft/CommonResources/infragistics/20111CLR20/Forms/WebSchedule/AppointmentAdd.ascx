 <%@ Register TagPrefix="cmb" NameSpace="Forms" %>
<%@ Register TagPrefix="igsch" Namespace="Infragistics.WebUI.WebSchedule" Assembly="Infragistics2.WebUI.WebDateChooser.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ Control Language="c#" Inherits="Forms.AppointmentAddUC" CodeFile="AppointmentAdd.ascx.cs" %>
<TABLE class="BackgroundTab" id="AddAppointmentTable" style="WIDTH: 100%; BORDER-COLLAPSE: collapse; HEIGHT: 100%" cellSpacing="0" cellPadding="0">
	<tr >
		<td style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px" vAlign="top">
			<TABLE id="statusBar" class="fonts" style="display:none;BORDER-RIGHT: black 1px solid; BORDER-TOP: black 1px solid; BACKGROUND: #ffffe1; BORDER-LEFT: black 1px solid; BORDER-BOTTOM: black 1px solid" cellSpacing="0" cellPadding="1" width="100%">
				<TR style="HEIGHT: 1.1em" width="100%">
					<TD vAlign="top" noWrap align="middle" width="1%"><IMG src="./images/info.gif">
					</TD>
					<TD id="statusText" width="99%" runat="server">This is a recurring appointment.
					</TD>
				</TR>
			</TABLE>
		</td>
	</tr>
	<TR>
		<TD style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px; PADDING-TOP: 5px" vAlign="top">
			<TABLE cellSpacing="0" cols="2" cellPadding="0" width="100%">
				<TR width="100%">
					<TD vAlign="top" noWrap><label class="Fonts" runat="server" id="SubjectLabel" for="tbSubject">Subject:&nbsp;</label></TD>
					<td style="WIDTH: 100%"><INPUT class="Fonts" id="tbSubject" style="WIDTH: 100%" tabIndex="1" type="text">
					</td>
				</TR>
				<TR width="100%">
					<TD vAlign="top" noWrap><label class="Fonts" runat="server" id="LocationLabel" for="tbLocation">Location:</label>
					</TD>
					<td style="WIDTH: 100%" align="left"><INPUT class="Fonts" id="tbLocation" style="WIDTH: 100%" tabIndex="2" type="text">
					</td>
				</TR>
			</TABLE>
		</TD>
	</TR>
	<tr style="FONT-SIZE: 1pt">
		<td style="WIDTH: 100%">
			<hr style="COLOR: #6a8ccb; HEIGHT: 1px">
		</td>
	</tr>
	<TR width="100%">
		<TD style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px">
			<table id="recurrenceTable" style="DISPLAY: none; WIDTH: 100%" cellSpacing="0" cellPadding="0">
				<tr>
					<td><SPAN class="Fonts" runat="server" id="recurrenceDescriptionText">Recurrence: This is a recurring appointment.</SPAN>
					</td>
				</tr>
			</table>
			<TABLE id="durationTable" style="OVERFLOW: hidden" cellSpacing="0" cellPadding="0" width="100%">
				<TR style="WIDTH: 100%; HEIGHT: 22px">
					<TD style="WIDTH: 55px" noWrap>
						<DIV class="Fonts" id="StartTimeLabel" runat="server" noWrap>Start Time:</DIV>
						<label style="display:none;" id="startdateLabel508" runat="server">Start Date</label>						
					</TD>
					<TD style="WIDTH: 110px" noWrap><span style="WIDTH: 110px"><igsch:webdatechooser id="wdcStartTime" style="DISPLAY: inline" tabIndex="3" width="100%" runat="server" EnableKeyboardNavigation="true">
								<EditStyle CssClass="Fonts">
									<Padding Bottom="0px" Top="0px" Right="0px"></Padding>
									<Margin Bottom="0px" Top="0px" Right="0px"></Margin>
								</EditStyle>
								<ClientSideEvents ValueChanged="wdcStartTime_ValueChanged"></ClientSideEvents>
								<CalendarLayout MaxDate="" ShowYearDropDown="False" ShowMonthDropDown="False" ShowFooter="False">
									<CalendarStyle Height="100%" CssClass="Fonts"></CalendarStyle>
									<TitleStyle BackColor="#C3DAF9"></TitleStyle>
								</CalendarLayout>
								<DropButton ImageUrl2="./images/clearpixel.gif" ImageUrl1="./images/clearpixel.gif">
									<Style>

<Padding Bottom="0px" Left="0px" Top="0px" Right="0px">
</Padding>

<Margin Bottom="0px" Left="0px" Top="0px" Right="0px">
</Margin>

									</Style>
								</DropButton>
							</igsch:webdatechooser></span></TD>
					<td width="15">						
						<div style="WIDTH: 15px"><BUTTON style="padding:0px; width: 15px; height:20px" onclick="DropDown_Cal1()" tabIndex="-1" type="button"><IMG src="./Images/downarrow.gif"></BUTTON>
						</div>
					</td>
					<TD id="startTime" style="WIDTH: 100px"><label style="display:none;" id="startTimeLabel508" runat="server">Start Time</label><cmb:combobox id="ddStartTime" tabIndex="4" runat="server"></cmb:combobox></TD>
					<TD style="WIDTH: 15px"><input id="cbAllDayEvent" onclick="cbAllDayEvent_Clicked()" tabIndex="5" type="checkbox">
					</TD>
					<td style="WIDTH: 100%" noWrap>
						<label style="display:inline;" class="Fonts" id="AllDayEventLabel" for="cbAllDayEvent" runat="server">All Day Event</label>
					</td>
				</TR>
				<TR style="WIDTH: 100%; HEIGHT: 20px">
					<TD style="WIDTH: 55px" noWrap>
						<DIV class="Fonts" id="EndTimeLabel" runat="server" noWrap>End Time:</DIV>
						<label style="display:none;" id="endDateLabel508" runat="server">End Date</label>
					</TD>
					<TD style="WIDTH: 110px" noWrap><span style="WIDTH: 110px"><igsch:webdatechooser id="wdcEndTime" style="DISPLAY: inline" tabIndex="6" width="100%" runat="server" EnableKeyboardNavigation="true">
								<EditStyle CssClass="Fonts">
									<Padding Bottom="0px" Top="0px" Right="0px"></Padding>
									<Margin Bottom="0px" Top="0px" Right="0px"></Margin>
								</EditStyle>
								<ClientSideEvents ValueChanged="wdcEndTime_ValueChanged"></ClientSideEvents>
								<CalendarLayout MaxDate="" ShowYearDropDown="False" ShowMonthDropDown="False" ShowFooter="False">
									<CalendarStyle  Height="100%" CssClass="Fonts"></CalendarStyle>
									<TitleStyle BackColor="#C3DAF9"></TitleStyle>
								</CalendarLayout>
								<DropButton ImageUrl2="./images/clearpixel.gif" ImageUrl1="./images/clearpixel.gif">
									<Style>

<Padding Bottom="0px" Left="0px" Top="0px" Right="0px">
</Padding>

<Margin Bottom="0px" Left="0px" Top="0px" Right="0px">
</Margin>

									</Style>
								</DropButton>
							</igsch:webdatechooser></span></TD>
					<td width="15">
						<div style="WIDTH: 15px"><BUTTON style="WIDTH: 15px; HEIGHT: 20px; padding:0px;" onclick="DropDown_Cal2()" type="button"><IMG src="./Images/downarrow.gif"/></BUTTON>
						</div>
					</td>
					<TD id="endTime" style="WIDTH: 100px" noWrap><label style="display:none;" id="endTimeLabel508" runat="server">End Time</label><cmb:combobox id="ddEndTime" tabIndex="7" runat="server"></cmb:combobox></TD>
				</TR>
			</TABLE>
		</TD>
	</TR>
	<tr style="FONT-SIZE: 1pt">
		<td style="WIDTH: 100%">
			<hr style="COLOR: #6a8ccb; HEIGHT: 1px">
		</td>
	</tr>
	<TR>
		<TD style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px" vAlign="top">
			<TABLE id="Table2" style="PADDING-RIGHT: 5px; FONT-SIZE: 100%; WIDTH: 100%; POSITION: relative">
				<TR id="rem_busy" style="PADDING-RIGHT: 5px; WIDTH: 100%; POSITION: relative">
					<TD noWrap><INPUT id="cbReminder" onclick="cbReminder_Clicked()" tabIndex="8" type="checkbox" name="Checkbox1">
					<label style="display:inline;" class="Fonts" id="ReminderLabel" for="cbReminder" runat="server">Reminder</label></TD>
					<TD><label style="display:none;" for="ddReminder" id="ReminderIntervalSection508" runat="server">Reminder Interval</label><SELECT class="Fonts" id="ddReminder" tabIndex="9">
							<OPTION selected runat="server" id="ri_zeroMin">0 minutes</OPTION>
							<OPTION runat="server" id="ri_fiveMin">5 minutes</OPTION>
							<OPTION runat="server" id="ri_tenMin">10 minutes</OPTION>
							<OPTION runat="server" id="ri_fifteenMin">15 minutes</OPTION>
							<OPTION runat="server" id="ri_thirtyMin">30 minutes</OPTION>
							<OPTION runat="server" id="ri_oneHour">1 hour</OPTION>
							<OPTION runat="server" id="ri_twoHours">2 hours</OPTION>
							<OPTION runat="server" id="ri_fourHours">4 hours</OPTION>
							<OPTION runat="server" id="ri_eightHours">8 hours</OPTION>
							<OPTION runat="server" id="ri_halfDay">0.5 days</OPTION>
							<OPTION runat="server" id="ri_oneDay">1 day</OPTION>
							<OPTION runat="server" id="ri_twoDays">2 days</OPTION>
						</SELECT>
					</TD>
					<TD noWrap><label style="display:none;" for="ddShowTimeAs" id="showTimeAsLabelSection508" runat="server">Show Time As</label><asp:label id="ShowTimeAsLabel" runat="server" CssClass="Fonts">Show time as:</asp:label>&nbsp;
					</TD>
					<TD width="100%"><SELECT class="Fonts" id="ddShowTimeAs" tabIndex="10" name="Select1">
							<OPTION selected runat="server" id="showTimeAs_Free">Free</OPTION>
							<OPTION runat="server" id="showTimeAs_Tentative">Tentative</OPTION>
							<OPTION runat="server" id="showTimeAs_OutofOffice">Out of Office</OPTION>
							<OPTION runat="server" id="showTimeAs_Busy">Busy</OPTION>
						</SELECT>
					</TD>
				</TR>
			</TABLE>
		</TD>
	</TR>
	<TR id="TAB1" vAlign="top" height="100%">
		<TD style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px; PADDING-BOTTOM: 5px">
			<DIV id="prnMsgBody" style="DISPLAY: none"></DIV>
			<label style="display:none;" for="txtMsgBody" runat="server" id="descriptionSection508">Description</label>
			<TEXTAREA class="Fonts" id="txtMsgBody" style="resize:none; WIDTH: 100%; HEIGHT: 100%" tabIndex="11" name="txtMsgBody" rows="22" cols="96" onchange="this.dirty='1'" dirty="0"></TEXTAREA>
		</TD>
	</TR>
</TABLE>
