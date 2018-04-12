<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->

<%
Dim Conn
LocationID = Request("LocationID")
if isEmpty(LocationID) then  LocationID = Session("CurrentLocationID")
if isEmpty(LocationID) then LocationID = "0"

LocationType = Request("LocationType")
if LocationType = "rack" then
	LocationText = "Rack"
	ParentID = 0
else
	LocationText = "Location"
end if

if LocationID > 0 then
	LocationInGrid = ValidateLocationInGrid(LocationID)
	if LocationInGrid <> "" then
		arrLocationInGrid = split(LocationInGrid,"::")
		bIsRack = arrLocationInGrid(5)
	end if
end if

SourceLocationID = LocationID
str = Session("CurrentLocationName")
start = InStrRev(str,"\")
SourceLocationName = Mid(str, start+1, Len(str)-start) 


%>

<html>
<head>
<title><%=Application("appTitle")%> -- Move Inventory <%=LocationText%></title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
	var DialogWindow;

	function ValidateMove(){
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		
		// LocationID is required
		if (document.form1.LocationID.value.length == 0) {
			errmsg = errmsg + "- Location to move is required is required.\r";
			bWriteError = true;
		}
		// Destination is required
		if (document.form1.ParentID.value.length == 0) {
			errmsg = errmsg + "- Destination is required.\r";
			bWriteError = true;
		}
		// Destination cannot be assign to a rack parent
		if (ValidateLocationIDType(document.form1.ParentID.value) == 1) {
			errmsg = errmsg + "- Destination can not be a Rack. If you would like to move a Rack \rinto another Rack, please edit the existing Rack.\r";
			bWriteError = true;
		}
		
		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.target = window.name;
			document.form1.submit();
		}
	}
-->
</script>
</head>

<body>
<center>
<form name="form1" xaction="echo.asp" action="MoveLocation_action.asp" method="POST" target="this.window">

<% 
'-- If this location is in a Grid and is not a Rack display error
if LocationInGrid <> "" and bIsRack <> "1" then 
%>
<br /><br />
<span class="GuiFeedback">You cannot move or delete a location storage within a grid format.</span><br><br>
<%=GetCancelButton()%>
<% else %>
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Move an inventory <%=lCase(LocationText)%> and all of its contents.</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required"><%=LocationText%> to move: <%=GetBarcodeIcon()%></span>
		</td>
		<td>
			<%ShowLocationPicker "document.form1", "LocationID", "lpLocationBarCode_1", "lpLocationName_1", 10, 30, false%> 
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required" title="LocationID of the destination">Destination Location: <%=GetBarcodeIcon()%></span>
		</td>
		<td>
			<%ShowLocationPicker "document.form1", "ParentID", "lpLocationBarCode_2", "lpLocationName_2", 10, 30, false%> 
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<%=GetCancelButton()%>&nbsp;<a HREF="#" onclick="ValidateMove(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>	
</table>	
<% end if %>
</form>
</center>
</body>
</html>
