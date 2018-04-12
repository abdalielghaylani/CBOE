<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<%
Dim Conn
Dim Cmd
Set myDict = plate_multiSelect_dict
if Lcase(Request("multiSelect")) = "true" then 
	PlateID = DictionaryToList(myDict)
	Barcode =  myDict.count & " plates will be moved"
Else
	PlateID = Session("plPlate_ID")
	Barcode = Session("plPlate_Barcode")
End if

DestinationLocationID = Request.QueryString("DestinationLocationID")

'Response.Write PlateID
'Response.End
Call GetInvConnection()
SQL = "SELECT DISTINCT location_name "
SQL = SQL & "FROM inv_locations, inv_plates "
SQL = SQL & "WHERE location_id_fk = location_id "
SQL = SQL & " AND plate_id in (" & PlateID & ")"
Set Cmd = GetCommand(Conn, SQL, adCmdText)
'Cmd.Parameters.Append Cmd.CreateParameter("PlateID", 200, 1, 2000, PlateID)
Set RS = Server.CreateObject("ADODB.recordset")
Set RS = Cmd.Execute

IF NOT RS.EOF THEN
	firstLocation = RS("location_name")
	RS.MoveNext
	IF NOT RS.EOF THEN
		sourceLocation = "Multiple source locations"
	ELSE
		RS.MoveFirst
		sourceLocation = RS("location_name")
	END IF
END IF


Dim httpResponse
Dim FormData
Dim ServerName
ServerName = Application("InvServerName")
QueryString = "Location_ID_FK=" & DestinationLocationID
QueryString = QueryString & "&PlateID=" & PlateID
QueryString = QueryString & "&DoFillGrid=true"
QueryString = QueryString & "&Preview=true"
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
QueryString = QueryString & Credentials

'Response.Write QueryString
'Response.End
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/MovePlate.asp", "ChemInv", QueryString)

	
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Preview an Inventory Plate Move</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
-->
</script>
</head>
<body>
<center>
<form name="form1" xaction="echo.asp" action="MovePlate_action.asp" method="POST">
<input Type="hidden" name="PlateID" value="<%=PlateID%>">
<input Type="hidden" name="multiscan" value="<%=Request("multiscan")%>">
<table border="0">


<%
			bShowPreview = false
			If isNumeric(httpResponse) then
				If CLng(httpResponse) < 0 then
					Response.Write "<TR><TD>"
					Response.Write "<center><table><tr><td><P><CODE>" & Application(httpResponse) & "</CODE></P></td></tr></table></center>"
					Response.Write "<center><SPAN class=""GuiFeedback"">Plate(s) could not be moved.</SPAN></center>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.Write "</TR></TD>"
				Else
					'1 location returned
					LocationID = httpResponse
					bShowPreview = true				
				End if
			Else
				If instr(httpResponse, "||")>0 then
					LocationID = replace(httpResponse,"||",",")
					bShowPreview = true				
				else
					Response.Write "<TR><TD>"
					Response.Write "<P><CODE>Oracle Error:<BR> " & httpResponse & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.end
					Response.Write "</TR></TD>"
				end if
			End if

if bShowPreview then
	'reverse the list order so that oracle returns the rs in the order that the user selected
	SQL = "SELECT plate_barcode FROM inv_plates where plate_id in (" & ReverseListOrder(PlateID,",") & ")"
	Set Cmd = GetCommand(Conn, SQL, adCmdText)
	'Cmd.Parameters.Append Cmd.CreateParameter("PlateID", 200, 1, 2000, PlateID)
	Set rsPlate = Server.CreateObject("ADODB.recordset")
	Set	rsPlate = Cmd.Execute

	SQL = "SELECT location_name FROM inv_vw_grid_location WHERE location_id in (" & LocationID &") order by sort_order"
	Set Cmd = GetCommand(Conn, SQL, adCmdText)
	Cmd.Parameters.Append Cmd.CreateParameter("LocationID", 200, 1, 2000, LocationID)
	Set rsLocation = Server.CreateObject("ADODB.recordset")
	Set rsLocation = Cmd.Execute

%>	<tr>
		<td align="center" colspan="2">
			<span class="GuiFeedback">Preview: Move an inventory plate.</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>Plate to move:</td>
		<td>
			<input TYPE="text" SIZE="33" onfocus="blur()" VALUE="<%=Barcode%>" disabled>
		</td>
	</tr>
	<tr height="25">
		<td align="right" nowrap>Source Location:</td>
		<td><input TYPE="text" SIZE="33" onfocus="blur()" VALUE="<%=sourceLocation%>" disabled></td>
	</tr>
	<tr height="25">
		<td colspan="2">
		<table border="1" cellspacing="0" cellpadding="0" width="400">
			<tr>
				<td align="center" nowrap><b>Plate ID</b></td>
				<td align="center"><b>Destination Location</b></td>
			</tr>
			<%While not rsPlate.EOF and not rsLocation.EOF%>
			<tr>
				<td><%=rsPlate("plate_barcode")%></td>
				<td><%=rsLocation("location_name")%></td>
			</tr>
			<%
				rsPlate.MoveNext
				rsLocation.MoveNext
			wend
			%>
		</table>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right">
			<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
<%end if%>	
</table>	
</form>
</center>
</body>
</html>

<%
function ReverseListOrder(list, delimiter)

arrList = split(list,delimiter)
upper = ubound(arrList)
newList = ""
for i = upper to 0 step -1
	if i > 0 then
		newList = newList & arrList(i) & delimiter
	elseif i = 0 then
		newList = newList & arrList(i)
	end if
next

ReverseListOrder = newList

end function
%>