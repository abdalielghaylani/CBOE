<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<html>
<head>
<title><%=Application("appTitle")%> -- Reconcile Location</title>
<script LANGUAGE="javascript">
window.focus()
var DialogWindow;
function ReconcileLocation(){
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		// Destination is required
		if (document.form1.LocationID.value.length == 0) {
			errmsg = errmsg + "- Destination is required.\r";
			bWriteError = true;
		}
		
		if (bWriteError){
			alert(errmsg);
		}
		else{
			top.document.location.href = "/cheminv/gui/reconcileLocation_frset.asp?LocationID=" + document.form1.LocationID.value;
		}
	}
</script>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
</head>
<%
QS = Request.QueryString

if Request("LocationID") = "" then%>
	<body>
	<form name="form1">
	<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Reconcile an Inventory Location.</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required" title="Scan or pick location to reconcile">Location ID: <%=GetBarcodeIcon()%></span>
		</td>
		<td>
			<%ShowLocationPicker "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 31, false%> 
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ReconcileLocation(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	
</form>
<%else%>
<frameset rows="300,*">
		<frame name="TabFrame" src="reconcile.asp?clear=1&amp;<%=QS%>" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="auto">
		<frame name="ListFrame" src="/cheminv/cheminv/BuildList.asp?reconcile=1&amp;<%=QS%>" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="auto">

</frameset>
<%end if%>
</html>
