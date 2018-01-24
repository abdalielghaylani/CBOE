<%@ Page UICulture="auto" Culture="auto" language="c#" CodeFile="ModifyRecurrenceDialog.aspx.cs" AutoEventWireup="false" Inherits="Forms.ModifyRecurrenceDialog" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML style="margin:0px; padding:0px;">
	<HEAD id="HEAD1" runat="server">
		<title>Modify Recurrence Dialog</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="./Styles/ModifyRecurrenceDialog.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body style="margin:0px; padding:0px;" onload="ModifyDialogLoad()">
		<form id="ModifyRecurrenceDialogForm" method="post" runat="server" style="margin:0px; padding:0px;">
			<script language="javascript">

				var openerDoc = null;
				var appointment = null;
				function ModifyDialogLoad()
				{
					if (window.dialogArguments) // Modal Dialog (IE)
					{
						openerDoc = window.dialogArguments;
						appointment = openerDoc._appt;
					}
					else if (window.opener != null)// Modeless Dialog (Firefox)
					{
						openerDoc = window.opener.document;
						appointment = openerDoc._appt;
					}
				}

				function okClicked()
				{
					var radioButtons = document.getElementsByName("modifyseries");
					if (radioButtons[0].checked)
						appointment.getRecurrence().tempEditType = 2;
					else
						appointment.getRecurrence().tempEditType = 1;

					try
					{
						if (!window.dialogArguments)
							openerDoc._currentDialog._fieldValues.WebScheduleInfo._modifyRecurrenceDialogClosing();
					} catch (e) { };

					window.close();
				}

				function cancelClicked()
				{
					openerDoc._cancelDlg = true;
					window.close();
				}
		
			</script>
			<table style="width:100%;" cellspacing="0" class="Fonts">
				<tr>
					<td><img src="./images/warning.gif" /></td>
					<td colspan="3">
					<br />
						<span id="message" runat="server"></span>
					</td>
				</tr>
				<tr>
					<td colspan="4">&nbsp;</td>
				</tr>
				<tr>
					<td>&nbsp;</td>
					<td colspan="3">
					<input type="radio" id="radOccurence" name="modifyseries" checked>
					<label id="openOccurrence" runat="server" for="radOccurence">Open this occurrence.</label>
					</td>
				</tr>
				<tr>
					<td colspan="4">&nbsp;</td>
				</tr>
				<tr>
					<td>&nbsp;</td>
					<td colspan="3">
					    <input type="radio" id="radSeries" name="modifyseries">
					    <label id="openSeries" runat="server" for="radSeries">Open the series.</label>
					</td>
				</tr>
				<tr>
					<td colspan="4">&nbsp;</td>
				</tr>
				<tr>
					<td>&nbsp;</td>
					<td><button class="Fonts" id="okButton" runat="server" style="width:90px" onclick="okClicked()">OK</button></td>
					<td>&nbsp;</td>
					<td><button class="Fonts" id="cancelButton" runat="server" style="width:90px" onclick="cancelClicked()">Cancel</button></td>
					<td>&nbsp;</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
