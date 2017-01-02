<%@ Page UICulture="auto" Culture="auto" language="c#" Inherits="Forms.Reminder" CodeFile="Reminder.aspx.cs" %>
<%@ Register TagPrefix="igmcl" NameSpace="Forms" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML style="HEIGHT: 100%">
	<HEAD id="HEAD1" runat="server">
		<title>Reminder</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="./Styles/ReminderDialog.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body style="overflow:hidden;" class="Background" bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" onunload="return window_onunload()" onload="return window_onload()">
		<form id="Form1" method="post" runat="server">
			<TABLE id="Table1" style="Z-INDEX: 101" height="100%" cellSpacing="1" cellPadding="1" width="100%" border="1">
				<tr>
					<td style="BORDER-TOP-STYLE: none; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none">
						<table style="MARGIN-LEFT: 4px">
							<TR>
								<td class="Fonts" style="BORDER-TOP-STYLE: none; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" colSpan="4"><span id="Image"></span>&nbsp;
									<span class="Fonts" runat="server" id="Subject" style="FONT-WEIGHT: bold">(No Subject)</span>
								</td>
							</TR>
							<tr>
								<td class="Fonts" style="BORDER-TOP-STYLE: none; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" colSpan="4"><span class="Fonts" runat="server" id="TimeLabel">Time:&nbsp;</span>
									<span class="Fonts" runat="server" id="Time"></span>
								</td>
							</tr>
							<tr>
								<td class="Fonts" style="BORDER-TOP-STYLE: none; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" colSpan="4"><span class="Fonts" runat="server" id="LocationLabel">Location:&nbsp;</span>
									<span class="Fonts" id="Location"></span>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td style="MARGIN-TOP: -10px; MARGIN-BOTTOM: -20px; BORDER-TOP-STYLE: none; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" colSpan="4" height="10">
						<table style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; MARGIN-LEFT: 4px; BORDER-LEFT: gray 1px solid; MARGIN-RIGHT: -5px; BORDER-BOTTOM: gray 1px solid" height="100%" width="99%">
							<TR>
								<td class="Fonts" style="BORDER-RIGHT: black 1px solid">&nbsp;<IMG src="./Images/blue_note.bmp">&nbsp;</td>
								<td class="Fonts" id="SubjectHeaderLabel" runat="server" style="BORDER-RIGHT: black 1px solid" width="50%" colSpan="2">Subject</td>
								<td class="Fonts" id="DueInHeaderLable" runat="server" width="50%">Due In</td>
							</TR>
						</table>
					</td>
				</tr>
				<tr height="100%">
					<td style="BORDER-TOP-STYLE: none; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" colSpan="4">
						<table style="MARGIN-TOP: -5px" height="100%" width="100%">
							<tr height="100%">
								<td colSpan="4"><igmcl:mclistbox id="MCL1" runat="server"></igmcl:mclistbox></td>
							</tr>
							<tr height="20">
								<td style="HEIGHT: 2px" width="100%" colSpan="2"><INPUT class="Fonts" runat="server" id="DismissAll" onclick="onDismissAllClicked();" type="button" value="Dismiss All"></td>
								<td style="HEIGHT: 2px"><INPUT class="Fonts" runat="server" id="OpenItem" onclick="onOpenItemClicked();" type="button" value="Open Item"></td>
								<td style="HEIGHT: 2px">
									<center><INPUT class="Fonts" runat="server" id="Dismiss" onclick="onDismissClicked();" type="button" value="Dismiss"></center>
								</td>
							</tr>
							<tr height="20" width="100%">
								<td colSpan="4">
									<DIV class="Fonts" id="ClickSnoozeLabel" runat="server" style="WIDTH: 100%; POSITION: relative; TOP: 5px">Click 
										Snooze to be reminded again in:</DIV>
								</td>
							</tr>
							<tr height="20">
								<td colSpan="3"><SELECT class="Fonts" id="ddSnooze" style="WIDTH: 100%" name="Select1">
										<OPTION id="fiveMinutesOption" runat="server" selected>5 minutes</OPTION>
										<OPTION id="tenMinutesOption" runat="server" >10 minutes</OPTION>
										<OPTION id="fifteenMinutesOption" runat="server" >15 minutes</OPTION>
										<OPTION id="oneHourOption" runat="server" >1 hour</OPTION>
										<OPTION id="twoHoursOption" runat="server" >2 hours</OPTION>
										<OPTION id="fourHoursOption" runat="server" >4 hours</OPTION>
										<OPTION id="eightHoursOption" runat="server" >8 hours</OPTION>
										<OPTION id="oneDayOption" runat="server" >1 day</OPTION>
										<OPTION id="twoDaysOption" runat="server" >2 days</OPTION>
										<OPTION id="threeDaysOption" runat="server" >3 days</OPTION>
										<OPTION id="oneWeekOption" runat="server" >1 week</OPTION>
										<OPTION id="twoWeeksOption" runat="server" >2 weeks</OPTION>
									</SELECT>
								</td>
								<td>
									<center><INPUT class="Fonts" runat="server" id="Snooze" onclick="onSnoozeClicked();" type="button" value="Snooze"></center>
								</td>
							</tr>
							<tr height="20">
								<td class="Fonts" width="100%"></td>
								<td></td>
								<td></td>
								<td>
									<center><INPUT class="Fonts" dataSrc="Close" runat="server" id="CloseButton" onclick="onCloseClicked();" type="button" value=" Close  "></center>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</TABLE>
			<script language="javascript" src="./Scripts/ig_reminderDialog.js"></script>
		</form>
		<script language="javascript" event="oncontextmenu" for="document">
			<!--
				return document_oncontextmenu()
			//-->
		</script>
	</body>
</HTML>
