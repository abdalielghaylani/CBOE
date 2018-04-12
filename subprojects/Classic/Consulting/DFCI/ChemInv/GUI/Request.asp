<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS

action = lcase(Request("action"))
RequestID = Request("RequestID")
ContainerID = Request("ContainerID")
ContainerName = Request("ContainerName")
LocationID = Session("CurrentLocationID")
cLocationID = Request("LocationID")
if cLocationID = "" then cLocationID = 0
dateFormatString = Application("DATE_FORMAT_STRING")
Select Case action
	Case "create"
		Caption = "Request delivery for this container."
		'DateRequired = Month(Now()) & "/" & Day(Now()) & "/" & Year(Now())
		DateRequired = Today
		QtyRequired = Request("QtyRequired")
		LocationID = Session("DefaultLocation")
		UserID = Ucase(Session("UserNameChemInv"))
	Case "edit"
		Caption = "Edit this request"
		Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GetRequest(?,?)}", adCmdText)	
		Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTID",131, 1, 0, RequestID)
		Cmd.Parameters("PREQUESTID").Precision = 9	
		Cmd.Parameters.Append Cmd.CreateParameter("PDATEFORMAT",200,1,30, dateFormatString)		
		Cmd.Properties ("PLSQLRSet") = TRUE  
		Set RS = Cmd.Execute
		Cmd.Properties ("PLSQLRSet") = FALSE
		If (RS.EOF AND RS.BOF) then
			Response.Write ("<table><TR><TD align=center colspan=6><span class=""GUIFeedback"">No requests found for this container</Span></TD></tr></table>")
			Response.End 
		Else
			UserID = RS("RUserID")
			QtyRequired = RS("Qty_Required")
			'ConvertDateToStr(date_format, dateObj)
			DateRequired = ConvertDateToStr(Application("DATE_FORMAT"),RS("Date_Required"))
			LocationID = RS("delivery_Location_ID_FK")
			comments = RS("request_comments")
		End if	
	Case "delete"
			Caption = "Are you sure you want to delete this request?"
	Case "undodelivery"
			Caption = "This request has been marked as delivered.<BR>Are you sure you want to undo the delivery?"		
End select
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Request an Inventory Container</title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	function ValidateRequest(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		//LocationID is required
		//document.form1.LocationID.value = document.form1.lpLocationID.value;
		<%if action <> "delete" AND action <> "undodelivery" then%>
		if (document.form1.LocationID.value.length == 0) {
			errmsg = errmsg + "- Location ID is required.\r";
			bWriteError = true;
		}
		else{
			// LocationID must be a psositve number
			if (!isPositiveNumber(document.form1.LocationID.value)){
				errmsg = errmsg + "- Location ID must be a positive number.\r";
				bWriteError = true;
			}
		}
		// Quantity reserved is required
		if (document.form1.QtyRequired){
			if (document.form1.QtyRequired.value.length == 0) {
				errmsg = errmsg + "- Quantity is required.\r";
				bWriteError = true;
			}
		
			// Quantity required if present must be a number
			if (!isNumber(document.form1.QtyRequired.value)){
				errmsg = errmsg + "- Quantity required must be a number.\r";
				bWriteError = true;
			}
		}
		// Date required must be a date
		if (document.form1.DateRequired.value.length > 0 && !isDate(document.form1.DateRequired.value)){
			errmsg = errmsg + "- Date Required must be in " + dateFormatString + " format.\r";
			bWriteError = true;
		}
					
		<%end if%>
		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.submit();
		}
	}
-->
</script>
</head>
<body>
<center>
<form name="form1" xaction="echo.asp" action="request_action.asp?action=<%=action%>" method="POST">
<input Type="hidden" name="ContainerID" value="<%=ContainerID%>">
<input Type="hidden" name="cLocationID" value="<%=cLocationID%>">
<input Type="hidden" name="RequestID" value="<%=RequestID%>">
<INPUT TYPE="hidden" NAME="RequestTypeID" VALUE="1">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><%=Caption%></span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Container:
		</td>
		<td>
			<input TYPE="tetx" SIZE="44" Maxlength="50" VALUE="<%=ContainerName%>" onfocus="blur()" disabled id="Text1" name="Text1">
		</td>
	</tr>
	<%if action <> "delete" AND action <> "undodelivery" then%>
	<tr>
		<td align="right" nowrap>
			<span class="required" title="Location where the container should be delivered to">Delivery Location: <%=GetBarcodeIcon()%></span>
		</td>
		<td>
			<%ShowLocationPicker "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 30, false%> 
		</td>
	</tr>
	<tr>
		<%=ShowPickList("UserID:", "UserID", UserID, "SELECT User_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC")%>
	</tr>
	<tr>
		<td align="right">
			<span class="required">Amount Required (<%=Request("UOMAbv")%>):</span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="10" Maxlength="50" NAME="QtyRequired" value="<%=QtyRequired%>">
		</td>
	</tr>
	<tr>
		<td align="right" valign="top" nowrap width="150">
			Date Required:
		</td>
		<td>
			<%call ShowInputField("", "", "DateRequired:form1:" & DateRequired , "DATE_PICKER:TEXT", "15")%>
			<!--<input type="text" name="DateRequired" size="15" value="<%=DateRequired%>"><a href onclick="return PopUpDate(&quot;DateRequired&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->
		</td>
	</tr>
	<tr height="150">
		<td align="right" valign="top" nowrap>
			Comments:
		</td>
		<td valign="top">
			<textarea rows="5" cols="30" name="RequestComments" wrap="hard"><%=Comments%></textarea>
		</td>
	</tr>
	<%end if%>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateRequest(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" alt WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>		
</table>	
</form>
</center>
</body>
</html>
