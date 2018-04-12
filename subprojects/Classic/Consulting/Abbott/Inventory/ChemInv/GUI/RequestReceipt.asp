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

RequestID = Request("RequestID")
QtySelected = Request("QtySelected")
ContainerIDList = Request("ContainerIDList")
ContainerGridPositionIDList = Request("ContainerGridIDList")
RackIDList = Request("RackIDList")

UOMAbbrv = Request("UOMAbbrv")
if not IsEmpty(Application("DEFAULT_SAMPLE_REQUEST_CONC")) then
	arrUOM = split(Application("DEFAULT_SAMPLE_REQUEST_CONC"),"=")
	UOMAbbrv = arrUOM(1)
end if

LocationID = Session("CurrentLocationID")
cLocationID = Request("LocationID")
if cLocationID = "" then cLocationID = 0
dateFormatString = Application("DATE_FORMAT_STRING")

if Request("printworksheet") = "true" then
	caption = "Request Worksheet"
else
	caption = "Request Receipt"
end if

'-- Retreive general request information
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GetBatchRequest(?,?)}", adCmdText)	
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
	DateRequired = ConvertDateToStr(Application("DATE_FORMAT"),RS("Date_Required"))
	DateDelivered = ConvertDateToStr(Application("DATE_FORMAT"),RS("Date_Delivered"))
	LocationID = RS("delivery_Location_ID_FK")
	comments = RS("request_comments")
	QtyRequired = RS("qty_required")
	BatchID = RS("batch_id_fk")
	RUserID = RS("RUserID")
	DUserID = RS("DUserID")
	Field_1 = RS("field_1")
	Field_2 = RS("field_2")
	Field_3 = RS("field_3")
	Field_4 = RS("field_5")
	Field_5 = RS("field_1")
	Date_1 = RS("date_1")
	Date_2 = RS("date_2")
End if	

'-- Retreive Custom Request fields
reqHtml = ""
For each Key in custom_createrequest_fields_dict
	reqHtml = reqHtml & "<tr><td align=""right"">" & custom_createrequest_fields_dict.item(key) & "</td>" & vbcrlf
	execute("TempValue = " & Key)
	reqHtml = reqHtml & "<td bgcolor=""#d3d3d3"">" & TempValue & "</td></tr>" & vbcrlf
Next	
For each Key in custom_fulfillrequest_fields_dict
	'if Key = "FIELD_3" then
	reqHtml = reqHtml & "<tr><td align=""right"">" & custom_fulfillrequest_fields_dict.item(key) & "</td>" & vbcrlf
	execute("TempValue = " & Key)
	reqHtml = reqHtml & "<td bgcolor=""#d3d3d3"">" & TempValue & "</td></tr>" & vbcrlf
	'end if
Next	

'-- Retreive Batch information from view
dbkey = "ChemInv"
GetInvConnection()
sql = "SELECT * FROM inv_vw_reg_batches vb, inv_container_batches b "
sql = sql & "WHERE b.batch_field_1 = vb.RegID and b.batch_field_2 = vb.BatchNumber "
sql = sql & "And b.batch_id = "  & BatchID
Set RS = Conn.Execute(sql)
regHtml = ""
if Not(RS.BOF and RS.EOF) then
	regHtml = regHtml & "<tr><td colspan=""2"" align=""center""><em><b>"
	if RS("RegName") <> "" then
		regHtml = regHtml & RS("RegName")
	else
		regHtml = regHtml & "No Substance Name"
	end if
	regHtml = regHtml & "</b></em></td></tr>"
	for each key in reg_fields_dict
		if key <> "BASE64_CDX" and key <> "REGNAME" then
			regHtml = regHtml & "<tr><td align=""right"">" & reg_fields_dict.item(key) & ":</td><td bgcolor=""#d3d3d3"">" & RS(key) & "</td></tr>"
		end if
	next
end if
RS.Close()
Set RS = Nothing


%>

<html>
<head>
<title><%=Application("appTitle")%> -- Request an Inventory Container</title>
<script language="javascript" src="/cheminv/choosecss.js"></script>
<script language="javascript" src="/cheminv/utils.js"></script>
<script language="javascript" src="/cheminv/gui/validation.js"></script>
<script language="javascript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
-->
</script>
<style>
table.receipt
{
    font-size: 9px;
    font-family: verdana, arial, helvetica, sans-serif
}


</style>
</head>
<body>
<center>
<form name="form1" action="" method="post">


<%
'Response.Write(RackIDList & "<br>Container List: " & ContainerIDList & "<br><br>")
if Request("printworksheet") = "true" then
	Response.Write("<div align=""center""><table bgcolor=""#d3d3d3"" width=""75%""><tr><td align=""center""><span class=""GuiFeedback"">" & caption & "</span></td></tr></table><br />")
	Response.write(DisplaySimpleMovedRack(RackIDList,ContainerIDList,ContainerGridPositionIDList))
	Response.Write("<a href=""javascript:window.print()""><img src=""/cheminv/graphics/sq_btn/print_btn.gif"" border=""0""></a>")
	Response.Write("&nbsp;<a href=""javascript:window.close()""><img src=""/cheminv/graphics/sq_btn/close_dialog_btn.gif"" border=""0""></a>")
	Response.Write("</div>")
else
%>

<table border="0" cellpadding="0" cellspacing="0" class="receipt" width="400" style="border-right:1px dashed #999999;border-bottom:1px dashed #999999;border-left:1px dashed #999999;border-top:1px dashed #999999;">
<tr><td colspan="2" align="center" bgcolor="#cccccc"  style="border-bottom:1px solid #000000;"><b><%=caption%></b></td></tr>
<tr><td valign="top">
	<table border="0" class="receipt">
		<tr><td align="right">Request ID:</td><td bgcolor="#d3d3d3"><%=RequestID%></td></tr>
		<tr><td align="right">Date Required:</td><td bgcolor="#d3d3d3"><%=DateRequired%></td></tr>
		<tr><td align="right">Date Delivered:</td><td bgcolor="#d3d3d3"><%=DateDelivered%></td></tr>
		<tr><td align="right">Requested By:</td><td bgcolor="#d3d3d3"><%=RUserID%></td></tr>
		<tr><td align="right">Requested For:</td><td bgcolor="#d3d3d3"><%=DUserID%></td></tr>
		<tr><td align="right">Amount Requested:</td><td bgcolor="#d3d3d3"><%=FormatNumber(QtyRequired,2)%>&nbsp;<%=UOMAbbrv%></td></tr>
		<tr><td align="right">Amount Delivered:</td><td bgcolor="#d3d3d3"><%=FormatNumber(QtySelected,2)%>&nbsp;<%=UOMAbbrv%></td></tr>
		<%=reqHtml%>
		<tr><td align="right" valign="top">Comments:</td><td bgcolor="#d3d3d3"><%=comments%></td></tr>
		<tr><td colspan="2">&nbsp;</td></tr>
		<!-- regHtml -->
		<tr><td colspan="2">&nbsp;</td></tr>
	</table>	
</td><td valign="top">
	<table border="0" class="receipt">
		<%=regHtml%>
	</table>	
</td></tr>
</table>
<%
	Response.Write "<a href=""javascript:window.print()""><img src=""/cheminv/graphics/sq_btn/print_btn.gif"" border=""0""></a>"
	Response.Write "&nbsp;<a href=""javascript:window.close()""><img src=""/cheminv/graphics/sq_btn/close_dialog_btn.gif"" border=""0""></a>"
%>

<%
end if %>

</form>
</center>
</body>
</html>

