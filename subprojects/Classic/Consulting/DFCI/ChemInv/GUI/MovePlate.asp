<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim Cmd
Set myDict = plate_multiSelect_dict
if Lcase(Request("multiSelect")) = "true" then 
	plateCount =  plate_multiSelect_dict.count
	if plateCount = 0 then
		action = "noPlates"
	else
		PlateID = DictionaryToList(myDict)
		Barcode =  myDict.count & " plates will be moved"
	end if
Else
	PlateID = Session("plPlate_ID")
	Barcode = Session("plPlate_Barcode")
End if

'LocationID = Request.QueryString("LocationID")
multiSelect = lcase(Request("multiSelect"))

if action <> "noPlates" then
	Call GetInvConnection()
	SQL = "SELECT DISTINCT location_name, location_id "
	SQL = SQL & "FROM inv_locations, inv_plates "
	SQL = SQL & "WHERE location_id_fk = location_id"
	SQL = SQL & " AND plate_id in (" & PlateID & ") "
	Set Cmd = GetCommand(Conn, SQL, &H0001)
	'Cmd.Parameters.Append Cmd.CreateParameter("PlateIDs", 200, 1, 2000, targetPlateFormats)
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
			PlateLocationID = rs("location_id")
		END IF
	END IF
end if

'Response.Write locationid & "test"

if Request("LocationID") <> "" then
	LocationID = Request("LocationID")
else
	LocationID = PlateLocationID
end if
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Move an Inventory Plate</title>
<script type="text/javascript" language="javascript" src="/cheminv/Choosecss.js"></script>
<script type="text/javascript" language="javascript" src="/cheminv/utils.js"></script>
<script type="text/javascript" language="javascript" src="/cheminv/gui/validation.js"></script>

<script type="text/javascript" language="javascript">
<!--Hide JavaScript
	var DialogWindow;
	var PreviewURL;
	window.focus();

	var multiselect = "<%=multiselect%>";

	function ValidateMove(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		// Destination is required
		if (document.form1.Location_ID_FK.value.length == 0) {
			errmsg = errmsg + "- Destination is required.\r";
			bWriteError = true;
		}
		else{
			// Destination must be a number
			if (!isNumber(document.form1.Location_ID_FK.value)){
			errmsg = errmsg + "- Destination must be a number.\r";
			bWriteError = true;
			}
		}
		if (document.form1.Location_ID_FK.value == 0) {
			errmsg = errmsg + "- Cannot move plates to root location.\r";
			bWriteError = true;
		}
		if (!IsPlateTypeAllowed('<%=PlateID%>', document.form1.Location_ID_FK.value,'','')) {
			errmsg = errmsg + "- This location does not accept this plate type.\r";
			bWriteError = true;
		}
		<% if Lcase(Request("multiSelect")) = "true" then %>
		var numPlates = <%=myDict.count%>;
		<% else %>
		var numPlates = 1;
		<% end if%>
		
		// determine whether there are enough open rack positions
		if (document.form1.isRack.value == "1") {
		    if (AreEnoughRackPositions(numPlates,document.form1.Location_ID_FK.value) == false) {
				errmsg = errmsg + "- There are not enough open positions from the selected start position\rin the rack(s) you have selected.\r";
				bWriteError = true;
		    }
		}

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
<%if action = "noPlates" then%>
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>You must select plates to move.</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>			
		</td>
	</tr>	
</table>	
<%else%>
<form name="form1" xaction="echo.asp" action="MovePlate_action.asp" method="POST">
<input Type="hidden" name="PlateID" value="<%=PlateID%>">
<input Type="hidden" name="multiscan" value="<%=Request("multiscan")%>">
<table border="0">
	<tr>
		<td align="center" colspan="2">
			<span class="GuiFeedback">Move an inventory plate.</span><br><br>
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
		<td align="right" nowrap>
			<span class="required" title="LocationID of the destination">Destination Location:</span>
		</td>
		<td>
			&nbsp;<%=GetBarcodeIcon()%>&nbsp;<%ShowLocationPicker3 "document.form1", "Location_ID_FK", "Location_BarCode", "Location_Name", 10, 25, false, ""%> 
		</td>								 
	</tr>
	<tr height="25">
		<td colspan="2" align="center" nowrap><span id="FillGridSpan" style="display:none;">
			<input type="checkbox" name="DoFillGrid" value="true" onclick="if (this.checked){document.all.PreviewSpan.style.display='block';}else{document.all.PreviewSpan.style.display='none';}">Move plates to empty sub-locations sequentially&nbsp;&nbsp;
			<span id="PreviewSpan" style="display:none;"><a class="MenuLink" HREF="#" onclick="OpenDialog(PreviewURL, 'PreviewDiag', 1); return false">Preview</a></span>
		</span></td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>&nbsp;<a HREF="#" onclick="ValidateMove(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>	
</table>	
</form>
<%end if%>
</center>
</body>
</html>

