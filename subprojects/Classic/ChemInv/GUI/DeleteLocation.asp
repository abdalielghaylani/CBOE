<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim Conn

LocationID = Request("LocationID")
if isEmpty(LocationID) then  LocationID = Session("CurrentLocationID")
if isEmpty(LocationID) then LocationID = "0"
LocationText = "Location"

LocationType = Request("LocationType")
if LocationType = "rack" then
	RecursiveDelete = " checked"
else
	RecursiveDelete = ""
end if

'-- Validate location and prevent from deletion if contained in grid element
if LocationID > 0 then
	LocationInGrid = ValidateLocationInGrid(LocationID)
	if LocationInGrid <> "" then
		arrLocationInGrid = split(LocationInGrid,"::")
		bIsRack = arrLocationInGrid(5)
	end if
end if

Call GetInvConnection()
SQL="SELECT LOCATION_ID FROM " &  Application("CHEMINV_USERNAME") & ".inv_Locations WHERE LOCATION_TYPE_ID_FK=27" 
Dim racks
Set RS= Conn.Execute(SQL)
IF Not RS.EOF  THEN
    RS.MoveFirst
    While NOT RS.EOF
        racks=racks & "," & RS("LOCATION_ID")
        RS.movenext
    Wend
End if

str = Session("CurrentLocationName")
start = InStrRev(str,"\")
LocationName = Mid(str, start+1, Len(str)-start) 
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Delete Inventory <%=LocationText%></title>
<script language="javascript" type="text/javascript" src="/cheminv/Choosecss.js"></script>
<script language="javascript" type="text/javascript" src="/cheminv/gui/validation.js"></script>
<script language="javascript" type="text/javascript" src="/cheminv/utils.js"></script>
<script language="javascript" type="text/javascript">
<!--Hide JavaScript
	window.focus();
	
	function ValidateDelete(){
		var racks="<%=racks%>";
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		
		// LocationID is required
		if (document.form1.LocationID.value.length == 0) {
			errmsg = errmsg + "- <%=LocationText%> to delete is required is required.\r";
			bWriteError = true;
		}
		else{
		    if(IsChildGridLocation(document.form1.LocationID.value)==1) {
		        errmsg = errmsg + " - Cannot delete a location within a grid format.\r";
		        bWriteError = true;
		    }
		}
		if (bWriteError){
			alert(errmsg);
			return false;
		}
		var bcontinue = true;
		if (document.form1.Recursively.checked){
			bcontinue = confirm("You are about to delete this <%=LocationText%> and all related sublocations and containers. This may include:\r-All Locations below this one.\r-All Locations on the same location grid as this one.\r-All containers below all those locations.\rDo you really want to continue?");
		}
		if (bcontinue) document.form1.submit();
	}
	
-->
</script>
</head>
<body>

<center>
<form name="form1" action="DeleteLocation_action.asp" method="POST">

<%
if Application("ENABLE_OWNERSHIP")="TRUE" then
isAuthorised=cint(isAuthorisedLocation(LocationID))
if isAuthorised = 0 then%>
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>You are not authorized to delete this location.</center></span><br><br>
		</td> 
	</tr>	
</table>	
<% Response.End

elseif Session("Inv_Manage_Locations")=False and isPublicLocation(LocationID)="1" Then %>
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>You are not authorized to delete this location.</center></span><br><br>
		</td> 
	</tr>	
</table>	
<% 
Response.End
end if %>

<% end if 
'-- If this location is in a Grid location display error
isaGridElement = cint(isGridElement(LocationID))
if isaGridElement > 0 then %>

	<br /><br />
	<span class="GuiFeedback">You cannot move or delete a location storage within a grid format.</span><br><br>
	<%=GetOkButton()%>

<% else %>

<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>Are you sure you want to permanently delete this <%=LocationText%>?</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required"><%=LocationText%> to delete: <%=GetBarcodeIcon()%></span>
		</td>
		<td>
			<%ShowLocationPicker6 "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 30, false,false%> 
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			&nbsp;
		</td>
		<td>
			<input type="checkbox" name="Recursively" value="1" <%=RecursiveDelete%>>
			Recursively delete related Locations, Plates, and Containers
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<%=GetCancelButton()%>&nbsp;<a HREF="#" onclick="ValidateDelete(); return false;"><img src="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>	
</table>	
<% end if %>

</form>
</center>
</body>
</html>
