<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->

<%
Dim Conn
Dim Cmd
Dim RS
bDebugPrint = False

'-- Figure out how many samples to create from each container in the batch
'ShowFormVars true

'-- Create the samples from that batch
RequestID = Request("RequestID")
BatchContainerIDs = Request("BatchContainerIDs")
UOMAbv = Request("UOMAbv")
QtyList = ""
for i = 1 to Request("NumContainersDisplay")
	if i = 1 then 
		QtyList = QtyList & Request("Sample"&i)
	else
		QtyList = QtyList & "," & Request("Sample"&i)
	end if
next

'-- Get Request Info - always look it up b/c it can be edited from a link on this page
GetInvConnection()
'Response.Write(RequestID & "<br>")
'Response.Write(QtyList & "<br>")
'Response.Write(BatchContainerIDs & "<br>")
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".REQUESTS.GETSAMPLESPERCONTAINER", adCmdStoredProc)	
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", advarchar, adParamReturnValue, 500, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTID",131, 1, 0, RequestID)
Cmd.Parameters("PREQUESTID").Precision = 9	
Cmd.Parameters.Append Cmd.CreateParameter("PQTYLIST",advarchar, 1, 500, QtyList)
Cmd.Parameters.Append Cmd.CreateParameter("PBATCHCONTAINERIDS",advarchar, 1, 500, BatchContainerIDs)
Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".REQUESTS.GETSAMPLESPERCONTAINER")
distribution = Cmd.Parameters("RETURN_VALUE")
'Response.Write("@@" & distribution & "@@@")
'Response.End
if distribution = "-103" then
	errmsg = "Cannot create samples from batch with containers containing mixed unit of measures. <br />Please update the containers in the batch to have matching unit of measures."
end if

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Create Samples from Batch</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
</head>
<body>
<center>
<% if distribution = "-103" then %>
	<span class="GuiFeedback"><%=errmsg%></span><br /><br />
	<a href="#" onclick="window.close(); return false;"><img src="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
<% else %>
<form name="form1" action="CreateSamplesFromBatch_action.asp" method="POST">
<%FormToHiddenFields%>
<input type="hidden" name="distribution" VALUE="<%=distribution%>">
<input type="hidden" name="mode" VALUE="CreateSamplesFromBatch">
<table border="0">
	<tr>
		<td colspan="2" align="center">
			<span class="GuiFeedback">Preview</span>
		</td>
	</tr>
	<tr><td colspan="2">
	<table border="1" cellspacing="0" cellpadding="1">
	<tr>
		<th>Batch Container</th>
		<th>Qty Remaining</th>
		<th>Samples Created</th>
	</tr>
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
	theRow = "<tr>"
	theRow = theRow & "<td rowspan=""" & rowspan & """ valign=""top"">" & currContainerBarcode & "</TD>"
	theRow = theRow & "<td rowspan=""" & rowspan & """ valign=""top"">" & currQtyRemaining & "(" & currUOMAbv & ")</TD>"
	if currNumSamples > 0 then
		for j = 1 to currNumSamples
			if j>1 then theRow = theRow & "</tr><tr>"
			currQty = eval("Request(""Sample" & currSampleIndex & """)")
			theRow = theRow & "<td>" & currQty & " (" & UOMAbv & ")</td>"
			currSampleIndex = currSampleIndex + 1
		next
	else
		theRow = theRow & "<td>None</td>"
	end if
	'theRow = theRow & "<td rowspan=""" & rowspan & """ valign=""top"">" & currQtyRemaining & "</TD>"
	'theRow = theRow & "<td rowspan=""" & rowspan & """ valign=""top"">" & currContainerID & "</TD>"
	theRow = theRow & "</tr>" & vbcrlf
	Response.Write theRow
	'Response.Write currContainerID & "<br>"
	'Response.Write currNumSamples & "<br>"
next
'Response.End
%>	
	</table>
	</td></tr>

	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="history.go(-1); return false;"><img SRC="/cheminv/graphics/sq_btn/btn_back_61.gif" border="0"></a><a HREF="#" onclick="document.form1.submit(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>	
	
</table>	
</form>
<% end if %>
</center>
</body>
</html>
<%
	Conn.Close
	Set Cmd = Nothing
	Set Conn = Nothing



sub FormToHiddenFields()
	for each key in Request.Form
		Response.Write "<input type=""hidden"" name=""" & key & """ value=""" & Request.Form(key) & """>" & vbcrlf
	next

end sub
%>