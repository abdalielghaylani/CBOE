<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<html>
<head>
<title><%=Application("appTitle")%> -- Move an Inventory Location</title>
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
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
LocationID = Request("LocationID")
if isEmpty(LocationID) then  LocationID = Session("CurrentLocationID")
if isEmpty(LocationID) then LocationID = "0"

SourceLocationID = LocationID
str = Session("CurrentLocationName")
start = InStrRev(str,"\")
SourceLocationName = Mid(str, start+1, Len(str)-start) 
 
%>

<body>
<center>
<form name="form1" xaction="echo.asp" action="MoveLocation_action.asp" method="POST" target="this.window">


<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Move an inventory location and all of its contents.</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required">Location to move: <%=GetBarcodeIcon()%></span>
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
			<%=GetCancelButton()%><a HREF="#" onclick="ValidateMove(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	
</form>
</center>
</body>
</html>
