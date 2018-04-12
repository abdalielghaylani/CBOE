<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
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

LocationID = Request.QueryString("LocationID")
multiSelect = lcase(Request("multiSelect"))

Call GetInvConnection()
SQL = "SELECT DISTINCT location_name "
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
	END IF
END IF

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Move an Inventory Plate</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	var PreviewURL;
	window.focus();

	var multiselect = "<%=multiselect%>";

	function IsGridLocation(locationID){
		//determine if this location is a grid location
		var strURL = "http://" + serverName + "/cheminv/api/IsParentGridLocation.asp?LocationID=" + locationID;	
		strURL = URLEncode(strURL);
		var httpResponse = JsHTTPGet(strURL) 
		/*
		alert(httpResponse);
		alert(strURL);
		httpResonse = 1;
		*/
		if (httpResponse == 1 && multiselect == 'true') {
			document.all.FillGridSpan.style.display = 'block';
			PreviewURL = "/Cheminv/GUI/MovePlatePreview.asp?multiselect=true" + "&DestinationLocationID=" + locationID;
			if (document.all.DoFillGrid.checked == true) {
				document.all.PreviewSpan.style.display ='block';
			}

		}
		else {
			document.all.FillGridSpan.style.display = 'none';
			PreviewURL = "/Cheminv/GUI/MovePlatePreview.asp?multiselect=true";
		}	
	}


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
			&nbsp;<%=GetBarcodeIcon()%>&nbsp;<%ShowLocationPicker3 "document.form1", "Location_ID_FK", "Location_BarCode", "Location_Name", 10, 33, false, "IsGridLocation(this.value);"%> 
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
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateMove(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	
</form>
</center>
</body>
</html>
