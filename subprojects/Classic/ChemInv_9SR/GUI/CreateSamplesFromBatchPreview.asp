<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->

<%
Dim Conn
Dim Cmd
Dim RS
bDebugPrint = FALSE


'figure out how many samples to create from each container in the batch
'ShowFormVars true

'create the samples from that batch
RequestID = Request("RequestID")
BatchContainerIDs = Request("BatchContainerIDs")
UOMAbv = Request("UOMAbv")
'get Request Info - always look it up b/c it can be edited from a link on this page
GetInvConnection()
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".REQUESTS.GetSamplesPerContainer", adCmdStoredProc)	
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", advarchar, adParamReturnValue, 500, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTID",131, 1, 0, RequestID)
Cmd.Parameters("PREQUESTID").Precision = 9	
Cmd.Parameters.Append Cmd.CreateParameter("PBATCHCONTAINERIDS",advarchar, 1, 500, BatchContainerIDs)
Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".REQUESTS.GetSamplesPerContainer")
distribution = Cmd.Parameters("RETURN_VALUE")
'Response.Write distribution
'Response.End

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Create Samples from Batch</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
</head>
<body>
<center>
<form name="form1" action="CreateSamplesFromBatch_action.asp"method="POST">
<%FormToHiddenFields%>
<INPUT TYPE="hidden" NAME="distribution" VALUE="<%=distribution%>">
<table border="0">
	<tr>
		<td colspan="2" align="center">
			<span class="GuiFeedback">Preview</span>
		</td>
	</tr>
	<TR><TD COLSPAN="2">
	<TABLE BORDER="1" CELLSPACING="0" CELLPADDING="1">
	<TR>
		<TH>Batch Container</TH>
		<TH>Qty Remaining</TH>
		<TH>Samples Created</TH>
	</TR>
<%
arrTemp1 = split(distribution,",")
currSampleIndex = 1
for i=0 to ubound(arrTemp1)
	arrTemp2 = split(arrTemp1(i),":")
	currContainerID = arrTemp2(0)
	currContainerBarcode = arrTemp2(1)
	currNumSamples = cint(arrTemp2(2))
	currQtyRemaining = arrTemp2(3)
	currUOMAbv = arrTemp2(4)
	rowspan = currNumSamples
	if rowspan = 0 then rowspan = 1
	theRow = "<TR>"
	theRow = theRow & "<TD ROWSPAN=""" & rowspan & """ VALIGN=""top"">" & currContainerBarcode & "</TD>"
	theRow = theRow & "<TD ROWSPAN=""" & rowspan & """ VALIGN=""top"">" & currQtyRemaining & "(" & currUOMAbv & ")</TD>"
	if currNumSamples > 0 then
		for j = 1 to currNumSamples
			if j>1 then theRow = theRow & "</TR><TR>"
			currQty = eval("Request(""Sample" & currSampleIndex & """)")
			theRow = theRow & "<TD>" & currQty & " (" & UOMAbv & ")</TD>"
			currSampleIndex = currSampleIndex + 1
		next
	else
		theRow = theRow & "<TD>None</TD>"
	end if
	'theRow = theRow & "<TD ROWSPAN=""" & rowspan & """ VALIGN=""top"">" & currQtyRemaining & "</TD>"
	'theRow = theRow & "<TD ROWSPAN=""" & rowspan & """ VALIGN=""top"">" & currContainerID & "</TD>"
	theRow = theRow & "</TR>" & vbcrlf
	Response.Write theRow
	'Response.Write currContainerID & "<BR>"
	'Response.Write currNumSamples & "<BR>"
next
'Response.End
%>	
	</TABLE>
	</TD></TR>

	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="history.go(-1); return false;"><img SRC="../graphics/btn_back_61.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="document.form1.submit(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
	
</table>	
</form>
</center>
</body>
</html>
<%
	Conn.Close
	Set Cmd = Nothing
	Set Conn = Nothing



sub FormToHiddenFields()
	for each key in Request.Form
		Response.Write "<INPUT TYPE=""hidden"" NAME=""" & key & """ VALUE=""" & Request.Form(key) & """>" & vbcrlf
	next

end sub
%>