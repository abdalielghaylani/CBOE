<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim Conn

LocationID = Request("LocationID")
if isEmpty(LocationID) then  LocationID = Session("CurrentLocationID")
if isEmpty(LocationID) then LocationID = "0"
str = Session("CurrentLocationName")
start = InStrRev(str,"\")
LocationName = Mid(str, start+1, Len(str)-start) 
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Delete an Inventory Location</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
	
	function ValidateDelete(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		
		// LocationID is required
		if (document.form1.LocationID.value.length == 0) {
			errmsg = errmsg + "- Location to delete is required is required.\r";
			bWriteError = true;
		}
		if (bWriteError){
			alert(errmsg);
			return false;
		}
		var bcontinue = true;
		if (document.form1.Recursively.checked){
			bcontinue = confirm("You are about to delete this location and all related sublocations and containers. This may include:\r-All Locations below this one.\r-All Locations on the same location grid as this one.\r-All containers below all those locations.\rDo you really want to continue?");
		}
		if (bcontinue) document.form1.submit();
	}
	
-->
</script>
</head>
<body>

<center>
<form name="form1" action="DeleteLocation_action.asp" method="POST">


<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>Are you sure you want to permanently delete this Location?</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required">Location to delete: <%=GetBarcodeIcon()%></span>
		</td>
		<td>
			<%ShowLocationPicker "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 30, false%> 
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			&nbsp;
		</td>
		<td>
			<input type="checkbox" name="Recursively" value="1">
			Recursively delete related locations and containers
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<%=GetCancelButton()%><a HREF="#" onclick="ValidateDelete(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	
</form>
</center>
</body>
</html>