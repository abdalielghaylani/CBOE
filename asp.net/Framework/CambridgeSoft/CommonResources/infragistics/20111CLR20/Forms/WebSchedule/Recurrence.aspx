<%@ Page UICulture="auto" Culture="auto" language="c#" CodeFile="Recurrence.aspx.cs" AutoEventWireup="false" Inherits="Forms.Recurrence" %>
<%@ Register TagPrefix="igsch" Namespace="Infragistics.WebUI.WebSchedule" Assembly="Infragistics2.WebUI.WebDateChooser.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ Register TagPrefix="cmb" Namespace="Forms" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; PADDING-TOP: 0px;">
	<head id="Head1" runat="server">
		<title>Recurrence pattern</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="./Styles/RecurrenceDialog.css" type="text/css" rel="stylesheet">		
	</head>
	<body style="overflow:hidden; PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; PADDING-TOP: 0px" onload="recurrenceLoad()">
		<form id="RecurrenceForm" method="post" runat="server" style="PADDING-RIGHT:10px; PADDING-LEFT:10px; PADDING-BOTTOM:10px; MARGIN:0px; PADDING-TOP:10px">
			<fieldset >
				<legend id="AppointmentTimeLegend" runat="server" align="left" class="Fonts">
					Appointment time
				</legend>
				<div style="WIDTH:100%; HEIGHT:20px">
					<table style="WIDTH:100%" cellpadding="0" cellspacing="0">
						<tr>
							<td style="WIDTH:100%">
								<TABLE style="OVERFLOW: hidden" cellSpacing="0" cellPadding="0" width="100%">
									<tr style="FONT-SIZE:3pt">
										<td>
											&nbsp;
										</td>
									</tr>
									<TR>
										<td nowrap>
											&nbsp;
										</td>
										<TD>
											<table class="Fonts" cellpadding="0" cellspacing="0">
												<tr>
													<td>
														<DIV noWrap id="StartLabel" runat="server">Start: </DIV>
													</td>
													<td>
														<cmb:combobox id="ddApptStartTime" runat="server" TabIndex="1"></cmb:combobox>
													</td>
												</tr>
											</table>
										</TD>
										<td nowrap>
											&nbsp; &nbsp;
										</td>
										<TD align="right">
											<table class="Fonts" cellpadding="0" cellspacing="0">
												<tr>
													<td>
														<DIV id="EndDateLabel" runat="server" align="left">End Date:</DIV>
													</td>
													<td>
														<table cellpadding="0" cellspacing="0" class="Fonts">
															<tr>
																<td nowrap>
																	<igsch:webdatechooser tabIndex="2" id="wdcEndTime" style="DISPLAY: inline" width="90px" runat="server">
																		<EditStyle CssClass="Fonts">
																			<Padding Bottom="0px" Top="0px" Right="0px"></Padding>
																			<Margin Bottom="0px" Top="0px" Right="0px"></Margin>
																		</EditStyle>
																		<ClientSideEvents ValueChanged="wdcEndRecTime1_ValueChanged"></ClientSideEvents>
																		<CalendarLayout MaxDate="" ShowYearDropDown="False" ShowMonthDropDown="False" ShowFooter="False">
																			<CalendarStyle Height="100%" CssClass="Fonts"></CalendarStyle>
																			<TitleStyle BackColor="#C3DAF9"></TitleStyle>
																		</CalendarLayout>
																		<DropButton ImageUrl2="./images/clearpixel.gif" ImageUrl1="./images/clearpixel.gif">
																			<Style>
																				<Padding Bottom="0px" Left="0px" Top="0px" Right="0px"></Padding>
																				<Margin Bottom="0px" Left="0px" Top="0px" Right="0px"></Margin>
																			</Style>
																		</DropButton>
																	</igsch:webdatechooser>
																</td>
																<td width="15">
																	<div style="WIDTH: 15px"><BUTTON style="padding:0px; WIDTH: 15px; HEIGHT: 20px" onclick="DropDown_Cal2()" type="button" tabindex="-1"><IMG src="./Images/downarrow.gif"></BUTTON>
																	</div>
																</td>
															</tr>
														</table>
													</td>
												</tr>
											</table>
										</TD>
										<td nowrap>
											&nbsp; &nbsp;
										</td>
										<TD align="right">
											<table class="Fonts" cellpadding="0" cellspacing="0">
												<tr>
													<td>
														<DIV id="EndTimeLabel" runat="server" align="left">End Time:</DIV>
													</td>
													<td>
														<cmb:combobox id="ddApptEndTime" runat="server" TabIndex="3"></cmb:combobox>
													</td>
												</tr>
											</table>
										</TD>
										<td nowrap>
											&nbsp; &nbsp;
										</td>
									</TR>
								</TABLE>
							</td>
						</tr>
					</table>
				</div>
			</fieldset>
			<fieldset style="padding-top:5px">
				<legend id="RecurrencePatternLegend" runat="server" align="left" class="Fonts">
					Recurrence pattern
				</legend>
				<div style="WIDTH:100%; HEIGHT:75px">
					<table cellpadding="0" cellspacing="0">
						<tr style="FONT-SIZE:3pt">
							<td>
								&nbsp;
							</td>
						</tr>
						<tr>
							<td style="BORDER-RIGHT:black 1px solid; WIDTH:120px" nowrap>
								<table style="WIDTH:100%" id="radioList" border="0" cellpadding="0" cellspacing="0" class="Fonts">
									<tr>
										<td>
										    <a id="radioGroupLink" tabindex="4" onfocus="forward_focus('radDaily','radWeekly','radMonthly','radYearly')" ></a>
											<input id="radDaily" runat="server" type="radio" value="Daily" name="radioList" onchange="firefoxRadioChanged(this)" onpropertychange="radioChanged(this)" onkeyup="backward_focus('ddApptEndTime_inputbox',event)" tabindex="5" /><label runat="server" id="radDailySection508" for="radDaily">Daily</label>
										</td>
									</tr>
									<tr>
										<td>
											<input id="radWeekly" runat="server" type="radio" value="Weekly" name="radioList" onchange="firefoxRadioChanged(this)" onpropertychange="radioChanged(this)" onkeyup="backward_focus('ddApptEndTime_inputbox',event)" checked="true" tabindex="6" /><label runat="server"  id="radWeeklySection508" for="radWeekly">Weekly</label>
										</td>
									</tr>
									<tr>
										<td>
											<input id="radMonthly" runat="server" type="radio" value="Monthly" name="radioList" onchange="firefoxRadioChanged(this)" onpropertychange="radioChanged(this)" onkeyup="backward_focus('ddApptEndTime_inputbox',event)" tabindex="7" /><label runat="server" id="radMonthlySection508" for="radMonthly">Monthly</label>
										</td>
									</tr>
									<tr>
										<td>
											<input id="radYearly" runat="server" type="radio" value="Yearly" name="radioList" onchange="firefoxRadioChanged(this)"  onpropertychange="radioChanged(this)" onkeyup="backward_focus('ddApptEndTime_inputbox',event)" tabindex="8" /><label runat="server" id="radYearlySection508" for="radYearly">Yearly</label>
										</td>
									</tr>
								</table>
							</td>
							<td style="BORDER-LEFT:white 1px solid" valign="top">
								<div id="radDaily_div" style="DISPLAY:none">
									<table id="dailyRadioList" border="0" cellpadding="0" cellspacing="0" class="Fonts">
										<tr>
											<td nowrap>
											    <a tabindex='103' onfocus="forward_focus('radEveryXDays','radEveryWeekDay')"></a>
												<input id="radEveryXDays" type="radio" value="Every" name="dailyRadioList" onpropertychange="" onkeyup="backward_focus('radioGroupLink',event)" tabindex="111" />
												<nobr><span id="EveryLabel" runat="server">Every</span> <input class="Fonts" onmousedown="elem_focus(this)" style="WIDTH:37px" type="text" id="inputEveryXDays" tabindex="121" /> <span id="DaysLabel" runat="server">day(s)</span></nobr>
											</td>
										</tr>
										<tr>
											<td nowrap>
												<input id="radEveryWeekDay" type="radio" value="Every WeekDay" name="dailyRadioList" onpropertychange="" onkeyup="backward_focus('radioGroupLink',event)" checked="checked" tabindex="131" />
												<nobr><span id="EveryWeekDayLabel" runat="server">Every Weekday</span></nobr>
											</td>
										</tr>
									</table>
								</div>
								<div id="radWeekly_div" style="WIDTH:100%">
									<table border="0" cellpadding="0" cellspacing="0" class="Fonts" style="WIDTH:100%">
										<tr>
											<td>
											    <a tabindex='102' onfocus="document.getElementById('inputRecursOn').focus()"></a>
												<nobr>&nbsp;<span id="RecursEveryLabel" runat="server">Recurs every </span><input class="Fonts" style="WIDTH:37px" value="1" type="text" id="inputRecursOn" onkeyup="backward_focus('radWeekly',event)" tabindex="112" /><span id="WeeksOnLabel" runat="server"> week(s) on</span></nobr>
											</td>
										</tr>
										<tr>
											<td style="WIDTH:100%">
												<table cellpadding="0" cellspacing="0" class="Fonts" style="WIDTH:100%">
													<tr>
														<td align="left" nowrap>
															<input val="1" id="cbSunday" type="checkbox" name="dayOfWeekCb" onpropertychange="" tabindex="119" />
															<label runat="server" id="cbSundaySection508" for="cbSunday">Sunday</label>
														</td>
														<td align="left" nowrap>
															<input val="2" id="cbMonday" type="checkbox" name="dayOfWeekCb" onpropertychange="" tabindex="122" />
															<label runat="server" id="cbMondaySection508" for="cbMonday">Monday</label>
														</td>
														<td align="left" nowrap>
															<input val="4" id="cbTuesday" type="checkbox" name="dayOfWeekCb" onpropertychange="" tabindex="132" />
															<label runat="server" id="cbTuesdaySection508" for="cbTuesday">Tuesday</label>
														</td>
														<td align="left" nowrap>
															<input val="8" id="cbWednesday" type="checkbox" name="dayOfWeekCb" onpropertychange="" tabindex="142" />
															<label runat="server" id="cbWednesdaySection508" for="cbWednesday">Wednesday</label>
														</td>
														<td align="right" nowrap>
															&nbsp;
														</td>
													</tr>
													<tr>
														<td align="left" nowrap>
															<input val="16" id="cbThursday" type="checkbox" name="dayOfWeekCb" onpropertychange="" tabindex="152" />
															<label runat="server" id="cbThursdaySection508" for="cbThursday">Thursday</label>
														</td>
														<td align="left" nowrap>
															<input val="32" id="cbFriday" type="checkbox" name="dayOfWeekCb" onpropertychange="" tabindex="162" />
															<label runat="server" id="cbFridaySection508" for="cbFriday">Friday</label>
														</td>
														<td align="left" nowrap>
															<input val="64" id="cbSaturday" type="checkbox" name="dayOfWeekCb" onpropertychange="" tabindex="172" />
															<label runat="server" id="cbSaturdaySection508" for="cbSaturday">Saturday</label>
														</td>
														<td align="left" nowrap>
															&nbsp;
														</td>
														<td align="right" nowrap>
															&nbsp;
														</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
								</div>
								<div id="radMonthly_div" style="DISPLAY:none">
                                    <table id="monthlyRadioList" border="0" cellspacing="0" cellpadding="0" class="Fonts" summary="Characterize the monthly recurrence pattern for your recurring appointment." >
                                        <tr>
                                            <td>
                                                    <a tabindex='103' onfocus="forward_focus('radDayOf','radXOf')"></a>
                                                    <input id="radDayOf" runat="server" type="radio" name="monthlyRadioList" checked="true" tabindex='113' value="Every" onpropertychange="" onkeyup="backward_focus('radioGroupLink',event)" />
                                            </td>
                                            <td><nobr><span id="DayLabel" runat="server">Day</span> <input class="Fonts" onmousedown="elem_focus(this)" title="" style="WIDTH:37px" type="text" id="input1DayOf" tabindex='123' /> of every <input class="Fonts" onmousedown="elem_focus(this)" style="WIDTH:37px" type="text" id="input2DayOf" title="" tabindex='133' /> month(s)</nobr></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                    <input id="radXOf" type="radio" runat="server" name="monthlyRadioList" tabindex='143' value="The" onpropertychange="" onkeyup="backward_focus('radioGroupLink',event)" />
                                            </td>
                                            <td>
                                                <nobr><span id="TheLabel" runat="server">The</span> 
                                                    <select id="select1XOf" class="Fonts" style="WIDTH:58px" tabindex='153' onmousedown="elem_focus(this)">
		                                                <option id="monthlyFirstOption" runat="server" title="first" selected value="1">first</option>
		                                                <option id="monthlySecondOption" runat="server" title="second" value="2">second</option>
		                                                <option id="monthlyThirdOption" runat="server" title="third" value="3">third</option>
		                                                <option id="monthlyFourthOption" runat="server" title="fourth" value="4">fourth</option>
		                                                <option id="monthlyLastOption" runat="server" title="last" value="5">last</option>
	                                                </select>
	                                                <select id="select2XOf" class="Fonts" style="WIDTH:90px" tabindex='163' onmousedown="elem_focus(this)" >
		                                                <option id="monthlyDayOption" runat="server" title="day" selected val="127">day</option>
		                                                <option id="monthlyWeekDayOption" runat="server" title="weekday" val="62">weekday</option>
		                                                <option id="monthlyWeekEndDayOption" runat="server" title="weekend day" val="65">weekend day</option>
		                                                <option id="monthlySundayOption" runat="server" title="Sunday" value="0" val="1">Sunday</option>
		                                                <option id="monthlyMondayOption" runat="server" title="Monday" value="1" val="2">Monday</option>
		                                                <option id="monthlyTuesdayOption" runat="server" title="Tuesday" value="2" val="4">Tuesday</option>
		                                                <option id="monthlyWednesdayOption" runat="server" title="Wednesday" value="3" val="8">Wednesday</option>
		                                                <option id="monthlyThursdayOption" runat="server" title="Thursday" value="4" val="16">Thursday</option>
		                                                <option id="monthlyFridayOption" runat="server" title="Friday" value="5" val="32">Friday</option>
		                                                <option id="monthlySaturdayOption" runat="server" title="Saturday" value="6" val="64">Saturday</option>
	                                                </select> 
	                                                <span id="OfEveryLabel" runat="server">of every </span>
	                                                <input class="Fonts" style="WIDTH:37px" type="text" id="inputXOf" tabindex='173' onmousedown="elem_focus(this)" />
	                                                <span id="MonthsLabel" runat="server">month(s)</span>
                                                </nobr>
                                            </td>
                                        </tr>
                                    </table>
  								</div>
								<div id="radYearly_div" style="DISPLAY:none">
									<table id="yearlyRadioList" border="0" cellpadding="0" cellspacing="0" class="Fonts">
										<tr>
											<td>
                                                <a tabindex='104' onfocus="forward_focus('radDateOf','radXOfX')"></a>
												<input id="radDateOf" runat="server" type="radio" value="Every" name="yearlyRadioList" onpropertychange="" onkeyup="backward_focus('radioGroupLink',event)" checked="true" tabindex="114" />
												<nobr><span id="yearlyEveryLabel" runat="server">Every</span>
												<select id="select1DateOf" onmousedown="elem_focus(this)"  class="Fonts" style="WIDTH:80px" tabindex="124" />
													<option id="yearlyJanuaryOption" runat="server" selected="selected" value="1" title="January">January</option>
													<option id="yearlyFebruaryOption" runat="server" value="2" title="February">February</option>
													<option id="yearlyMarchOption" runat="server" value="3" title="March">March</option>
													<option id="yearlyAprilOption" runat="server" value="4" title="April">April</option>
													<option id="yearlyMayOption" runat="server" value="5" title="May">May</option>
													<option id="yearlyJuneOption" runat="server" value="6" title="June">June</option>
													<option id="yearlyJulyOption" runat="server" value="7" title="July">July</option>
													<option id="yearlyAugustOption" runat="server" value="8" title="August">August</option>
													<option id="yearlySeptemberOption" runat="server" value="9" title="September">September</option>
													<option id="yearlyOctoberOption" runat="server" value="10" title="October">October</option>
													<option id="yearlyNovemberOption" runat="server" value="11" title="November">November</option>
													<option id="yearlyDecemberOption" runat="server" value="12" title="December">December</option>
												</select>
												<input onmousedown="elem_focus(this)" class="Fonts" style="WIDTH:37px" type="text" id="inputDateOf" tabindex="134" />
												</nobr>
											</td>
										</tr>
										<tr>
											<td nowrap>
												<input id="radXOfX" type="radio" value="Every" name="yearlyRadioList" onpropertychange="" onkeyup="backward_focus('radioGroupLink',event)" tabindex="144" />
												<nobr><span id="TheLabel2" runat="server">The</span> 
												<select id="select1XOfX" onmousedown="elem_focus(this)" class="Fonts" style="WIDTH:58px" tabindex="154">
													<option id="yearlyFirstOption" runat="server" selected="selected" value="1" title="first">first</option>
													<option id="yearlySecondOption" runat="server" value="2" title="second">second</option>
													<option id="yearlyThirdOption" runat="server" value="3" title="third">third</option>
													<option id="yearlyFourthOption" runat="server" value="4" title="fourth">fourth</option>
													<option id="yearlyLastOption" runat="server" value="5" title="last">last</option>
												</select>
												<select id="select2XOfX" onmousedown="elem_focus(this)" class="Fonts" style="WIDTH:90px" tabindex="164">
													<option id="yearlyDayOption" runat="server" selected="selected" val="127" title="day">day</option>
													<option id="yearlyWeekdayOption" runat="server" val="62" title="weekday">weekday</option>
													<option id="yearlyWeekendDayOption" runat="server" val="65" title="weekend day">weekend day</option>
													<option id="yearlySundayOption" runat="server" value="0" val="1" title="Sunday">Sunday</option>
													<option id="yearlyMondayOption" runat="server" value="1" val="2" title="Monday">Monday</option>
													<option id="yearlyTuesdayOption" runat="server" value="2" val="4" title="Tuesday">Tuesday</option>
													<option id="yearlyWednesdayOption" runat="server" value="3" val="8" title="Wednesday">Wednesday</option>
													<option id="yearlyThursdayOption" runat="server" value="4" val="16" title="Thursday">Thursday</option>
													<option id="yearlyFridayOption" runat="server" value="5" val="32" title="Friday">Friday</option>
													<option id="yearlySaturdayOption" runat="server" value="6" val="64" title="Saturday">Saturday</option>
												</select>
												 of 
												<select id="select3XOfX" onmousedown="elem_focus(this)" class="Fonts" style="WIDTH:80px" tabindex="174">
													<option id="yearlyJanuaryOption2" runat="server" selected="selected" value="1" title="January">January</option>
													<option id="yearlyFebruaryOption2" runat="server" value="2" title="February">February</option>
													<option id="yearlyMarchOption2" runat="server" value="3" title="March">March</option>
													<option id="yearlyAprilOption2" runat="server" value="4" title="April">April</option>
													<option id="yearlyMayOption2" runat="server" value="5" title="May">May</option>
													<option id="yearlyJuneOption2" runat="server" value="6" title="June">June</option>
													<option id="yearlyJulyOption2" runat="server" value="7" title="July">July</option>
													<option id="yearlyAugustOption2" runat="server" value="8" title="August">August</option>
													<option id="yearlySeptemberOption2" runat="server" value="9" title="September">September</option>
													<option id="yearlyOctoberOption2" runat="server" value="10" title="October">October</option>
													<option id="yearlyNovemberOption2" runat="server" value="11" title="November">November</option>
													<option id="yearlyDecemberOption2" runat="server" value="12" title="December">December</option>
												</select>
												</nobr>
											</td>
										</tr>
									</table>
								</div>
							</td>
						</tr>
					</table>
				</div>
			</fieldset>
			<fieldset style="padding-top:5px">
				<legend runat="server" id="RangeOfRecurrenceLegend" align="left" class="Fonts">
					Range of recurrence
				</legend>
				<div style="WIDTH:100%; HEIGHT:75px">
					<table id="rangeRadioList" cellpadding="0" cellspacing="0" style="WIDTH:100%" class="Fonts">
						<tr style="FONT-SIZE:3pt">
							<td>
								&nbsp;
							</td>
						</tr>
						<tr>
							<td nowrap>
								<table cellpadding="0" cellspacing="0">
									<tr>
										<td>
											<DIV class="Fonts" id="StartLabel2" runat="server" noWrap>Start:</DIV>
										</td>
										<td>
											<igsch:webdatechooser tabIndex="300" id="wdcStartTime" style="DISPLAY: inline" width="110px" runat="server" >
												<EditStyle CssClass="Fonts">
													<Padding Bottom="0px" Top="0px" Right="0px"></Padding>
													<Margin Bottom="0px" Top="0px" Right="0px"></Margin>
												</EditStyle>
												<ClientSideEvents ValueChanged="wdcStartRecTime_ValueChanged"></ClientSideEvents>
												<CalendarLayout MaxDate="" ShowYearDropDown="False" ShowMonthDropDown="False" ShowFooter="False">
													<CalendarStyle Height="100%" CssClass="Fonts"></CalendarStyle>
													<TitleStyle BackColor="#C3DAF9"></TitleStyle>
												</CalendarLayout>
												<DropButton ImageUrl2="./images/clearpixel.gif" ImageUrl1="./images/clearpixel.gif">
													<Style>
														<Padding Bottom="0px" Left="0px" Top="0px" Right="0px"></Padding>
														<Margin Bottom="0px" Left="0px" Top="0px" Right="0px"></Margin>
													</Style>
												</DropButton>
											</igsch:webdatechooser>
										</td>
										<td>
											<div style="WIDTH: 15px">
												<BUTTON style="padding:0px; WIDTH: 15px; HEIGHT: 20px" onclick="DropDown_Cal1()" type="button" tabindex="-1"><IMG src="./Images/downarrow.gif">
												</BUTTON>
											</div>
										</td>
									</tr>
								</table>
							</td>
							<td align="left" nowrap>
							    <a tabindex="302" onfocus="forward_focus('radNoEndDate','radEndAfter','radEndBy')"></a>
								<input id="radNoEndDate" type="radio" name="rangeRadioList" onpropertychange="" onkeyup="backward_focus('wdcStartTime_input',event)" checked="checked" tabindex="303">
								<label for="radNoEndDate" runat="server" id="NoEndDateLabel">No end date</label>
							</td>
						</tr>
						<tr>
							<td colspan="2" style="font-size:3pt;">
								&nbsp;
							</td>
						</tr>
						<tr>
							<td>
								&nbsp;
							</td>
							<td align="left">
								<input id="radEndAfter" type="radio" name="rangeRadioList" onpropertychange="" onkeyup="backward_focus('wdcStartTime_input',event)" tabindex="304"><nobr>
									<span id="EndAfterLabel" runat="server">End after:</span> &nbsp;<input class="Fonts" onmousedown="elem_focus(this)" style="WIDTH:37px" type="text" value="10" id="inputEndAfter" tabindex="305" />
								 <span runat="server" id="OccurrencesLabel">occurrences</span></nobr>
							</td>
						</tr>
						<tr>
							<td colspan="2" style="font-size:3pt;">
								&nbsp;
							</td>
						</tr>
						<tr>
							<td>
								&nbsp;
							</td>
							<td align="left">
								<table cellpadding="0" cellspacing="0" class="Fonts">
									<tr>
										<td nowrap>
											<input id="radEndBy" type="radio" name="rangeRadioList" onpropertychange="" onkeyup="backward_focus('wdcEndTime_input',event)" tabindex="306" /><nobr>
												<span id="EndByLabel" runat="server">End by:</span></nobr>
										</td>
										<td nowrap>
											<igsch:webdatechooser tabIndex="306" id="wdcEndRecurrence" style="DISPLAY: inline" width="110px" runat="server">
												<EditStyle CssClass="Fonts">
													<Padding Bottom="0px" Top="0px" Right="0px"></Padding>
													<Margin Bottom="0px" Top="0px" Right="0px"></Margin>
												</EditStyle>
												<ClientSideEvents ValueChanged="wdcEndRecTime2_ValueChanged"></ClientSideEvents>
												<CalendarLayout MaxDate="" ShowYearDropDown="False" ShowMonthDropDown="False" ShowFooter="False">
													<CalendarStyle Height="100%" CssClass="Fonts"></CalendarStyle>
													<TitleStyle BackColor="#C3DAF9"></TitleStyle>
												</CalendarLayout>
												<DropButton ImageUrl2="./images/clearpixel.gif" ImageUrl1="./images/clearpixel.gif">
													<Style>
														<Padding Bottom="0px" Left="0px" Top="0px" Right="0px"></Padding>
														<Margin Bottom="0px" Left="0px" Top="0px" Right="0px"></Margin>
													</Style>
												</DropButton>
											</igsch:webdatechooser>
										</td>
										<td width="15">
											<div style="WIDTH: 15px"><BUTTON style=" padding:0px; WIDTH: 15px; HEIGHT: 20px" onclick="DropDown_Cal3()" type="button" tabindex="-1"><IMG src="./Images/downarrow.gif"></BUTTON>
											</div>
										</td>
									</tr>
								</table>
							</td>
						</tr>
						<tr>
							<td colspan="2" style="font-size:3pt;">
								&nbsp;
							</td>
						</tr>
					</table>
				</div>
			</fieldset>
			<table style="WIDTH:100%;">
				<tr style="FONT-SIZE:3pt">
					<td>
						&nbsp;
					</td>
				</tr>
				<tr>
					<td align="left">
						&nbsp;
					</td>
					<td align="middle">
						<button style="width:75px; height:22px;" runat="server" class="Fonts" id="buttonOK" onclick="okClicked()" type="button" tabindex="308">OK</button> &nbsp;
						<button style="width:75px;  height:22px;" runat="server" class="Fonts" id="buttonCancel" onclick="cancelClicked()" type="button" tabindex="309">Cancel</button> &nbsp;
						<button style=" height:22px;" runat="server" class="Fonts" id="buttonRemoveRecurrence" onclick="removeRecurrenceClicked()" type="button" tabindex="310">Remove Recurrence</button>&nbsp;
					</td>
					<td align="right">
						&nbsp;
					</td>
				</tr>
			</table>
			<div style="position: absolute; top: 32000; left: 32000; height: 1px; width: 1px; overflow: hidden;">
                <label id="Section508_1" runat="server" for="input1DayOf">Editable field to enter day number of calendar month</label>
                <label id="Section508_2" runat="server" for="input2DayOf">Editable field to enter number of months to skip between occurrences</label>
                <label id="Section508_3" runat="server" for="select1XOf">Select the week of the month</label>
                <label id="Section508_4" runat="server" for="select2XOf">Select the day or days of the week</label>
                <label id="Section508_5" runat="server" for="inputXOf">Editable field to enter number of months to skip between occurrences</label>
                <label id="Section508_6" runat="server" for="radDayOf">Appointment recurs on the same calendar date of every one or more months</label>
                <label id="Section508_7" runat="server" for="radXOf">Appointment recurs on the same day or days of the specified week in every one or more months</label>
                <label id="Section508_8" runat="server" for="ddApptStartTime_inputbox">Select the start time of your recurring appointment</label>
                <label id="Section508_9" runat="server" for="wdcEndTime_input">Select the end date of your recurring appointment if it is different from the day each occurrence will start</label>
                <label id="Section508_10" runat="server" for="ddApptEndTime_inputbox">Select the end time of your recurring appointment</label>
                <label id="Section508_11" runat="server" for="radEveryXDays">Appointment recurs periodically once every several days</label>
                <label id="Section508_12" runat="server" for="radEveryWeekDay">Appointment recurs on every weekday</label>
                <label id="Section508_13" runat="server" for="inputEveryXDays">Editable field to enter number of days between each occurrence</label>
                <label id="Section508_14" runat="server" for="inputRecursOn">Editable field to enter how frequently as a number of weeks the appointment recurs on a certain day or days of the week</label>
                <label id="Section508_15" runat="server" for="radDateOf">Appointment recurs on the same calendar date every year</label>
                <label id="Section508_16" runat="server" for="select1DateOf">Select the anniversary month</label>
                <label id="Section508_17" runat="server" for="inputDateOf">Editable field to enter the date in the anniversary month</label>
                <label id="Section508_18" runat="server" for="radXOfX">Appointment recurs on the same day and week in a specific month every year</label>
                <label id="Section508_19" runat="server" for="select1XOfX">Select the week of the month the occurrence takes place</label>
                <label id="Section508_20" runat="server" for="select2XOfX">Select the day of the week the occurrence takes place</label>
                <label id="Section508_21" runat="server" for="select3XOfX">Select the month of the year the occurrence takes place every year</label>
                <label id="Section508_22" runat="server" for="wdcStartTime_input">Select the start date of your recurrence, also the date on which the first appointment occurs</label>
                <label id="Section508_23" runat="server" for="radEndAfter">End after maximum number of occurrences</label>
                <label id="Section508_24" runat="server" for="inputEndAfter">Editable field to enter the maximum number of occurrences</label>
                <label id="Section508_25" runat="server" for="radEndBy">End by a specific date</label>
                <label id="Section508_26" runat="server" for="wdcEndRecurrence_input">Select the end date when the recurrence will stop</label>
            </div>
		</form>
		<script language="javascript" src="./Scripts/ig_recurrenceDialog.js"></script>
		<script language="javascript" src="./Scripts/ig_recurrenceDialogForm.js"></script>
	</body>
</HTML>
