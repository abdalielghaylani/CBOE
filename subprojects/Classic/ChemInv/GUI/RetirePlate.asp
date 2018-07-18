<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetLocationAuthorizedAttribute.asp" -->
<% end if %>
<%
Dim Conn
Dim Cmd
Dim RS
Set myDict = plate_multiSelect_dict
if Lcase(Request("multiSelect")) = "true" then 
	Plate_ID = DictionaryToList(myDict)
	NumPlates=myDict.count
	Barcode =  NumPlates & " plates will be retired"
Else
	Plate_ID = Session("plPlate_ID")
	Barcode = Session("plPlate_Barcode")
End if
LocationID = Application("plDefRetireLocationID")
Qty_Remaining = Session("plQty_Remaining")
Status_ID_FK = Application("plDefRetireStatusID")

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Retire an Inventory Plate</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	function ValidateMove(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		// Destination is required
		if (document.form1.Location_ID_FK.value.length == 0) {
			errmsg = errmsg + "- Destination is required.\r";
			bWriteError = true;
		}
   		<%if Application("ENABLE_OWNERSHIP")="TRUE" then%>
		     if(GetAuthorizedLocation(document.form1.Location_ID_FK.value,document.getElementById("tempCsUserName").value,document.getElementById("tempCsUserID").value)==0)
            {
                errmsg = errmsg + "- Not athorized for this location.\r";
			    alert(errmsg);
			    return;
            }
		<%end if%>
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
<%if Cint(NumPlates)>300 then%>
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>Cannot retire more than 300 plates.</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>			
		</td>
	</tr>	
</table>
<%else %>	
<form name="form1" xaction="echo.asp" action="RetirePlate_action.asp" method="POST">
<input Type="hidden" name="Plate_ID" value="<%=Plate_ID%>">
<input Type="hidden" name="Qty_Remaining" value="<%=Qty_Remaining%>">
<input Type="hidden" name="tempQty_Remaining" value="<%=Qty_Remaining%>">
<input Type="hidden" name="multiscan" value="<%=Request("multiscan")%>">
<input TYPE="hidden" NAME="tempCsUserName" id="tempCsUserName" Value="<%=Session("UserName" & "cheminv")%>" >
<input type="hidden" name="tempCsUserID" id="tempCsUserID" value="<%=Server.URLEncode(CryptVBS(Session("UserID" & "cheminv"), "ChemInv\API\GetBatchInfo.asp"))%>" />
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Retire an inventory plate.</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Plate to retire:
		</td>
		<td>
			<input TYPE="tetx" SIZE="44" onfocus="blur()" VALUE="<%=Barcode%>" disabled>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required" title="LocationID of the destination">Destination Location: <%=GetBarcodeIcon()%></span>
		</td>
		<td>
			<%ShowLocationPicker "document.form1", "Location_ID_FK", "lpLocationBarCode", "lpLocationName", 10, 30, false%> 
		</td>
	</tr>
	<tr>
		<%=ShowPickList("Plate status:", "Status_ID_FK", Status_ID_FK,"SELECT Enum_ID AS Value, Enum_Value AS DisplayText FROM inv_enumeration, inv_enumeration_set WHERE Eset_name = 'Plate Status' and eset_id_fk = eset_id ORDER BY lower(DisplayText) ASC")%>
	</tr>
	<tr>
		<td align"right" nowrap></td><td><input type="checkbox" name="zeroQty_cb" value="1" onclick="this.checked ? document.form1.Qty_Remaining.value = '' :  document.form1.Qty_Remaining.value = document.form1.tempQty_Remaining.value">Set the quantity remaining to zero</td>	
		<script LANGUAGE="javascript">document.form1.zeroQty_cb.click();</script>

	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateMove(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	
</form>
<%end if %>
</center>
</body>
</html>
