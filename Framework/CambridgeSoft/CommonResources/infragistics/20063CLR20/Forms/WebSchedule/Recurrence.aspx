<%@ Page language="c#" CodeFile="Recurrence.aspx.cs" AutoEventWireup="false" Inherits="Forms.Recurrence" %>
<%@ Register TagPrefix="igsch" Namespace="Infragistics.WebUI.WebSchedule" Assembly="Infragistics2.WebUI.WebDateChooser.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ Register TagPrefix="cmb" Namespace="Forms" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; PADDING-TOP: 0px">
	<head runat="server">
		<title>Recurrence pattern</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="./Styles/RecurrenceDialog.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="./Scripts/ig_recurrenceDialog.js"></script>
		<script language="javascript" src="./Scripts/ig_recurrenceDialogForm.js"></script>
	</head>
	<body style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; PADDING-TOP: 0px" onload="recurrenceLoad()">
		<form id="RecurrenceForm" method="post" runat="server" style="PADDING-RIGHT:10px; PADDING-LEFT:10px; PADDING-BOTTOM:10px; MARGIN:0px; PADDING-TOP:10px">
			<fieldset>
				<legend align="left" class="Fonts">
					Appointment time
				</legend>
				<div style="WIDTH:100%; HEIGHT:100%">
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
														<DIV noWrap>Start: &nbsp;</DIV>
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
														<DIV noWrap align="left">End Date: &nbsp;</DIV>
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
																			<CalendarStyle Width="100%" Height="100%" CssClass="Fonts"></CalendarStyle>
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
																	<div style="WIDTH: 15px"><BUTTON style="WIDTH: 15px; HEIGHT: 20px" onclick="DropDown_Cal2()" type="button" tabindex="-1"><IMG src="./Images/downarrow.gif"></BUTTON>
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
														<DIV noWrap align="left">End Time: &nbsp;</DIV>
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
				<legend align="left" class="Fonts">
					Recurrence pattern
				</legend>
				<div style="WIDTH:100%; HEIGHT:100%">
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
											<input id="radDaily" type="radio" value="Daily" name="radioList" onchange="firefoxRadioChanged(this)" onpropertychange="radioChanged(this)" onkeyup="backward_focus('ddApptEndTime_inputbox',event)" tabindex="5" /><label for="radDaily">Daily</label>
										</td>
									</tr>
									<tr>
										<td>
											<input id="radWeekly" type="radio" value="Weekly" name="radioList" onchange="firefoxRadioChanged(this)" onpropertychange="radioChanged(this)" onkeyup="backward_focus('ddApptEndTime_inputbox',event)" checked="checked" tabindex="6" /><label for="radWeekly">Weekly</label>
										</td>
									</tr>
									<tr>
										<td>
											<input id="radMonthly" type="radio" value="Monthly" name="radioList" onchange="firefoxRadioChanged(this)" onpropertychange="radioChanged(this)" onkeyup="backward_focus('ddApptEndTime_inputbox',event)" tabindex="7" /><label for="radMonthly">Monthly</label>
										</td>
									</tr>
									<tr>
										<td>
											<input id="radYearly" type="radio" value="Yearly" name="radioList" onchange="firefoxRadioChanged(this)"  onpropertychange="radioChanged(this)" onkeyup="backward_focus('ddApptEndTime_inputbox',event)" tabindex="8" /><label for="radYearly">Yearly</label>
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
												<nobr>Every <input class="Fonts" onmousedown="elem_focus(this)" style="WIDTH:37px" type="text" id="inputEveryXDays" tabindex="121" /> day(s)</nobr>
											</td>
										</tr>
										<tr>
											<td nowrap>
												<input id="radEveryWeekDay" type="radio" value="Every WeekDay" name="dailyRadioList" onpropertychange="" onkeyup="backward_focus('radioGroupLink',event)" checked="checked" tabindex="131" />
												<nobr>Every Weekday</nobr>
											</td>
										</tr>
									</table>
								</div>
								<div id="radWeekly_div" style="WIDTH:100%">
									<table border="0" cellpadding="0" cellspacing="0" class="Fonts" style="WIDTH:100%">
										<tr>
											<td>
											    <a tabindex='102' onfocus="document.getElementById('inputRecursOn').focus()"></a>
												<nobr>&nbsp; Recurs every <input class="Fonts" style="WIDTH:37px" value="1" type="text" id="inputRecursOn" onkeyup="backward_focus('radWeekly',event)" tabindex="112" /> week(s) on</nobr>
											</td>
										</tr>
										<tr>
											<td style="WIDTH:100%">
												<table cellpadding="0" cellspacing="0" class="Fonts" style="WIDTH:100%">
													<tr>
														<td align="left" nowrap>
															<input val="1" id="cbSunday" type="checkbox" value="Sunday" name="dayOfWeekCb" onpropertychange="" tabindex="119" />
															<label for="cbSunday">Sunday</label>
														</td>
														<td align="left" nowrap>
															<input val="2" id="cbMonday" type="checkbox" value="Monday" name="dayOfWeekCb" onpropertychange="" tabindex="122" />
															<label for="cbMonday">Monday</label>
														</td>
														<td align="left" nowrap>
															<input val="4" id="cbTuesday" type="checkbox" value="Tuesday" name="dayOfWeekCb" onpropertychange="" tabindex="132" />
															<label for="cbTuesday">Tuesday</label>
														</td>
														<td align="left" nowrap>
															<input val="8" id="cbWednesday" type="checkbox" value="Wednesday" name="dayOfWeekCb" onpropertychange="" tabindex="142" />
															<label for="cbWednesday">Wednesday</label>
														</td>
														<td align="right" nowrap>
															&nbsp;
														</td>
													</tr>
													<tr>
														<td align="left" nowrap>
															<input val="16" id="cbThursday" type="checkbox" value="Thursday" name="dayOfWeekCb" onpropertychange="" tabindex="152" />
															<label for="cbThursday">Thursday</label>
														</td>
														<td align="left" nowrap>
															<input val="32" id="cbFriday" type="checkbox" value="Friday" name="dayOfWeekCb" onpropertychange="" tabindex="162" />
															<label for="cbFriday">Friday</label>
														</td>
														<td align="left" nowrap>
															<input val="64" id="cbSaturday" type="checkbox" value="Saturday" name="dayOfWeekCb" onpropertychange="" tabindex="172" />
															<label for="cbSaturday">Saturday</label>
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
                                                    <input id="radDayOf" type="radio" name="monthlyRadioList" checked="checked" tabindex='113' value="Every" onpropertychange="" onkeyup="backward_focus('radioGroupLink',event)" />
                                            </td>
                                            <td><nobr>Day <input class="Fonts" onmousedown="elem_focus(this)" title="" style="WIDTH:37px" type="text" id="input1DayOf" tabindex='123' /> of every <input class="Fonts" onmousedown="elem_focus(this)" style="WIDTH:37px" type="text" id="input2DayOf" title="" tabindex='133' /> month(s)</nobr></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                    <input id="radXOf" type="radio" name="monthlyRadioList" tabindex='143' value="The" onpropertychange="" onkeyup="backward_focus('radioGroupLink',event)" />
                                            </td>
                                            <td>
                                                <nobr>The 
                                                    <select id="select1XOf" class="Fonts" style="WIDTH:58px" tabindex='153' onmousedown="elem_focus(this)">
		                                                <option title="first" selected value="1">first</option>
		                                                <option title="second" value="2">second</option>
		                                                <option title="third" value="3">third</option>
		                                                <option title="fourth" value="4">fourth</option>
		                                                <option title="last" value="5">last</option>
	                                                </select>
	                                                <select id="select2XOf" class="Fonts" style="WIDTH:90px" tabindex='163' onmousedown="elem_focus(this)" >
		                                                <option title="day" selected val="127">day</option>
		                                                <option title="weekday" val="62">weekday</option>
		                                                <option title="weekend day" val="65">weekend day</option>
		                                                <option title="Sunday" value="0" val="1">Sunday</option>
		                                                <option title="Monday" value="1" val="2">Monday</option>
		                                                <option title="Tuesday" value="2" val="4">Tuesday</option>
		                                                <option title="Wednesday" value="3" val="8">Wednesday</option>
		                                                <option title="Thursday" value="4" val="16">Thursday</option>
		                                                <option title="Friday" value="5" val="32">Friday</option>
		                                                <option title="Saturday" value="6" val="64">Saturday</option>
	                                                </select> 
	                                                of every 
	                                                <input class="Fonts" style="WIDTH:37px" type="text" id="inputXOf" tabindex='173' onmousedown="elem_focus(this)" />
	                                                month(s)
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
												<input id="radDateOf" type="radio" value="Every" name="yearlyRadioList" onpropertychange="" onkeyup="backward_focus('radioGroupLink',event)" checked="checked" tabindex="114" />
												<nobr>Every 
												<select id="select1DateOf" onmousedown="elem_focus(this)"  class="Fonts" style="WIDTH:80px" tabindex="124" />
													<option selected="selected" value="1" title="January">January</option>
													<option value="2" title="February">February</option>
													<option value="3" title="March">March</option>
													<option value="4" title="April">April</option>
													<option value="5" title="May">May</option>
													<option value="6" title="June">June</option>
													<option value="7" title="July">July</option>
													<option value="8" title="August">August</option>
													<option value="9" title="September">September</option>
													<option value="10" title="October">October</option>
													<option value="11" title="November">November</option>
													<option value="12" title="December">December</option>
												</select>
												<input onmousedown="elem_focus(this)" class="Fonts" style="WIDTH:37px" type="text" id="inputDateOf" tabindex="134" />
												</nobr>
											</td>
										</tr>
										<tr>
											<td nowrap>
												<input id="radXOfX" type="radio" value="Every" name="yearlyRadioList" onpropertychange="" onkeyup="backward_focus('radioGroupLink',event)" tabindex="144" />
												<nobr>The 
												<select id="select1XOfX" onmousedown="elem_focus(this)" class="Fonts" style="WIDTH:58px" tabindex="154">
													<option selected="selected" value="1" title="first">first</option>
													<option value="2" title="second">second</option>
													<option value="3" title="third">third</option>
													<option value="4" title="fourth">fourth</option>
													<option value="5" title="last">last</option>
												</select>
												<select id="select2XOfX" onmousedown="elem_focus(this)" class="Fonts" style="WIDTH:90px" tabindex="164">
													<option selected="selected" val="127" title="day">day</option>
													<option val="62" title="weekday">weekday</option>
													<option val="65" title="weekend day">weekend day</option>
													<option value="0" val="1" title="Sunday">Sunday</option>
													<option value="1" val="2" title="Monday">Monday</option>
													<option value="2" val="4" title="Tuesday">Tuesday</option>
													<option value="3" val="8" title="Wednesday">Wednesday</option>
													<option value="4" val="16" title="Thursday">Thursday</option>
													<option value="5" val="32" title="Friday">Friday</option>
													<option value="6" val="64" title="Saturday">Saturday</option>
												</select>
												 of 
												<select id="select3XOfX" onmousedown="elem_focus(this)" class="Fonts" style="WIDTH:80px" tabindex="174">
													<option selected="selected" value="1" title="January">January</option>
													<option value="2" title="February">February</option>
													<option value="3" title="March">March</option>
													<option value="4" title="April">April</option>
													<option value="5" title="May">May</option>
													<option value="6" title="June">June</option>
													<option value="7" title="July">July</option>
													<option value="8" title="August">August</option>
													<option value="9" title="September">September</option>
													<option value="10" title="October">October</option>
													<option value="11" title="November">November</option>
													<option value="12" title="December">December</option>
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
				<legend align="left" class="Fonts">
					Range of recurrence
				</legend>
				<div style="WIDTH:100%; HEIGHT:100%">
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
											<DIV class="Fonts" noWrap>&nbsp; Start: &nbsp;</DIV>
										</td>
										<td>
											<igsch:webdatechooser tabIndex="300" id="wdcStartTime" style="DISPLAY: inline" width="110px" runat="server" >
												<EditStyle CssClass="Fonts">
													<Padding Bottom="0px" Top="0px" Right="0px"></Padding>
													<Margin Bottom="0px" Top="0px" Right="0px"></Margin>
												</EditStyle>
												<ClientSideEvents ValueChanged="wdcStartRecTime_ValueChanged"></ClientSideEvents>
												<CalendarLayout MaxDate="" ShowYearDropDown="False" ShowMonthDropDown="False" ShowFooter="False">
													<CalendarStyle Width="100%" Height="100%" CssClass="Fonts"></CalendarStyle>
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
												<BUTTON style="WIDTH: 15px; HEIGHT: 20px" onclick="DropDown_Cal1()" type="button" tabindex="-1"><IMG src="./Images/downarrow.gif">
												</BUTTON>
											</div>
										</td>
									</tr>
								</table>
							</td>
							<td align="left" nowrap>
							    <a tabindex="302" onfocus="forward_focus('radNoEndDate','radEndAfter','radEndBy')"></a>
								<input id="radNoEndDate" type="radio" name="rangeRadioList" onpropertychange="" onkeyup="backward_focus('wdcStartTime_input',event)" checked="checked" tabindex="303">
								<label for="radNoEndDate">No end date</label>
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
									End after: &nbsp;<input class="Fonts" onmousedown="elem_focus(this)" style="WIDTH:37px" type="text" value="10" id="inputEndAfter" tabindex="305" />
								 occurrences</nobr>
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
												End by: &nbsp;</nobr>
										</td>
										<td nowrap>
											<igsch:webdatechooser tabIndex="306" id="wdcEndRecurrence" style="DISPLAY: inline" width="110px" runat="server">
												<EditStyle CssClass="Fonts">
													<Padding Bottom="0px" Top="0px" Right="0px"></Padding>
													<Margin Bottom="0px" Top="0px" Right="0px"></Margin>
												</EditStyle>
												<ClientSideEvents ValueChanged="wdcEndRecTime2_ValueChanged"></ClientSideEvents>
												<CalendarLayout MaxDate="" ShowYearDropDown="False" ShowMonthDropDown="False" ShowFooter="False">
													<CalendarStyle Width="100%" Height="100%" CssClass="Fonts"></CalendarStyle>
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
											<div style="WIDTH: 15px"><BUTTON style="WIDTH: 15px; HEIGHT: 20px" onclick="DropDown_Cal3()" type="button" tabindex="-1"><IMG src="./Images/downarrow.gif"></BUTTON>
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
						<button style="WIDTH:75px; height:22px;" class="Fonts" id="buttonOK" onclick="okClicked()" type="button" tabindex="308">OK</button> &nbsp;
						<button style="WIDTH:75px;  height:22px;" class="Fonts" id="buttonCancel" onclick="cancelClicked()" type="button" tabindex="309">Cancel</button> &nbsp;
						<button style="WIDTH:115px;  height:22px;" class="Fonts" id="buttonRemoveRecurrence" onclick="removeRecurrenceClicked()" type="button" tabindex="310">Remove Recurrence</button>
					</td>
					<td align="right">
						&nbsp;
					</td>
				</tr>
			</table>
			<div style="position: absolute; top: 32000; left: 32000; height: 1px; width: 1px; overflow: hidden;">
                <label for="input1DayOf">Editable field to enter day number of calendar month</label>
                <label for="input2DayOf">Editable field to enter number of months to skip between occurrences</label>
                <label for="select1XOf">Select the week of the month</label>
                <label for="select2XOf">Select the day or days of the week</label>
                <label for="inputXOf">Editable field to enter number of months to skip between occurrences</label>
                <label for="radDayOf">Appointment recurs on the same calendar date of every one or more months</label>
                <label for="radXOf">Appointment recurs on the same day or days of the specified week in every one or more months</label>
                <label for="ddApptStartTime_inputbox">Select the start time of your recurring appointment</label>
                <label for="wdcEndTime_input">Select the end date of your recurring appointment if it is different from the day each occurrence will start</label>
                <label for="ddApptEndTime_inputbox">Select the end time of your recurring appointment</label>
                <label for="radEveryXDays">Appointment recurs periodically once every several days</label>
                <label for="radEveryWeekDay">Appointment recurs on every weekday</label>
                <label for="inputEveryXDays">Editable field to enter number of days between each occurrence</label>
                <label for="inputRecursOn">Editable field to enter how frequently as a number of weeks the appointment recurs on a certain day or days of the week</label>
                <label for="radDateOf">Appointment recurs on the same calendar date every year</label>
                <label for="select1DateOf">Select the anniversary month</label>
                <label for="inputDateOf">Editable field to enter the date in the anniversary month</label>
                <label for="radXOfX">Appointment recurs on the same day and week in a specific month every year</label>
                <label for="select1XOfX">Select the week of the month the occurrence takes place</label>
                <label for="select2XOfX">Select the day of the week the occurrence takes place</label>
                <label for="select3XOfX">Select the month of the year the occurrence takes place every year</label>
                <label for="wdcStartTime_input">Select the start date of your recurrence, also the date on which the first appointment occurs</label>
                <label for="radEndAfter">End after maximum number of occurrences</label>
                <label for="inputEndAfter">Editable field to enter the maximum number of occurrences</label>
                <label for="radEndBy">End by a specific date</label>
                <label for="wdcEndRecurrence_input">Select the end date when the recurrence will stop</label>
            </div>
		</form>
	</body>
</HTML>
