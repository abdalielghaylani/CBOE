<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->

<%
Dim Conn
Dim Cmd
Dim RS


ApprovedRequestIDList = Request("ApprovedRequestIDList")
DeclinedRequestIDList = Request("DeclinedRequestIDList")
action = Request("action")

arrDeclinedRequestID = split(DeclinedRequestIDList,",")
numDeclines = ubound(arrDeclinedRequestID) + 1
'Response.Write DeclinedRequestIDList
'Response.End

Call GetInvConnection()
SQL = "SELECT request_id, decline_reason, p.user_id AS RUserID FROM inv_requests r, cs_security.people p WHERE Upper(r.user_id_fk) = Upper(p.user_id) and request_id in (" & DeclinedRequestIDList & ")"
Set RS = Conn.Execute(SQL)

if action="edit" then
	defaultStyle = "block"
else
	defaultStyle = "none"
end if

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Decline Sample Request</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();	
	function ShowHide(requestNumber){
		displayValue = "none";
		var isChecked ;
		eval("isChecked = document.form1.EnterDeclineReason" + requestNumber + ".checked");
		if (isChecked) 
			displayValue = "block";
		eval("document.all.div" + requestNumber + ".style.display = '" + displayValue + "'");
	}
	

	function applyToAll(requestNumber, element) {
		if (element.checked) {
			eval("DeclineReason = document.form1.DeclineReasonList[" + requestNumber + "].value");
			numDeclines = document.all.numDeclines.value;
			for (i=0; i<numDeclines; i++) {
				if (i != requestNumber) {
					eval("document.form1.EnterDeclineReason" + i + ".checked = true");
					ShowHide(i);
					eval("document.form1.DeclineReasonList[" + i + "].value = DeclineReason");
					eval("document.form1.applyToAll" + i + ".checked = false");
					
				}
			}
		}
	}
		
-->
</script>

</head>
<body>
<center>
<form name="form1" action="ApproveRequestSample_action.asp" method="POST">
<INPUT TYPE="hidden" NAME="numDeclines" VALUE="<%=numDeclines%>">
<INPUT TYPE="hidden" NAME="ApprovedRequestIDList" VALUE="<%=ApprovedRequestIDList%>">
<INPUT TYPE="hidden" NAME="DeclinedRequestIDList" VALUE="<%=DeclinedRequestIDList%>">
<INPUT TYPE="hidden" NAME="action" VALUE="<%=action%>">

<table border="0">
	<%
	for i=0 to ubound(arrDeclinedRequestID)
	%>
	<%if action <> "edit" then%>
	<TR>
		<TD colspan="2" ALIGN="left" width="400"><b>The following requests will be declined:</b>
		<br></TD>
	</TR>
	<%end if%>
	<tr>
		<TD ALIGN="right" WIDTH="200" NOWRAP>Request ID:</TD>
		<td align="left" width="400"><input TYPE="text" CLASS="GrayedText" SIZE="10" Maxlength="50" VALUE="<%=RS("request_id")%>" READONLY></td>
	</tr>
	<tr>
		<td align="right" width="200" nowrap>Requested By:</td>
		<td align="left" width="400"><input TYPE="text" CLASS="GrayedText" SIZE="10" Maxlength="50" VALUE="<%=RS("RUserID")%>" READONLY></td>
	</tr>
	<TR><TD COLSPAN="2">
	<DIV STYLE="display:<%=defaultStatus%>;" ID="div<%=i%>">
	<TABLE BORDER="0">
		<tr>
			<td align="right" valign="top" nowrap width="200">
				<span class="required">Reason for Decline:</span>
			</td>
			<td valign="top">
				<textarea rows="5" cols="32" name="DeclineReasonList" wrap="hard"><%=Trim(RS("decline_reason"))%></textarea>
			</td>
		</tr>
		<%if numDeclines > 1 then%>
		<TR>
			<TD ALIGN="right" WIDTH="200">Apply to all:</TD>
			<TD>
            <INPUT TYPE="checkbox" NAME="applyToAll<%=i%>" ONCLICK="applyToAll(<%=i%>, this);" value="ON"></TD>
		</TR>
		<%end if%>
	</TABLE>
	</DIV>
	</TD></TR>
	<%
		RS.MoveNext
	next
	%>
	<%if action = "edit" then%>
		<tr>
			<td colspan="2" align="right"> 
				<a HREF="#" onclick="window.close();"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a><a HREF="#" onclick="document.form1.submit(); return false;">&nbsp;<img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
			</td>
		</tr>	
	<%else%>
		<tr>
			<td colspan="2" align="right"><a HREF="#" onclick="history.back();"><img SRC="/cheminv/graphics/sq_btn/btn_back_61.gif" border="0"></a><a HREF="#" onclick="document.form1.submit(); return false;"><input type="image" SRC="/cheminv/graphics/sq_btn/btn_next_61.gif" border="0"></a></td>
		</tr>	
	<%end if%>
</table>	
</form>
</center>
</body>
</html>