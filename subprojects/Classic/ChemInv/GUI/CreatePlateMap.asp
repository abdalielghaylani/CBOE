<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetLocationAuthorizedAttribute.asp" -->
<% end if %>
<%
Dim Conn
Set myDict = plate_multiSelect_dict
if Lcase(Request("multiSelect")) = "true" then 
	Instructions = "Create plate maps based on these plates."
	PlateID = DictionaryToList(myDict)
	Barcode =  myDict.count & " plates will be moved"
Else
	Instructions = "Create a plate map based on this plate."
	PlateID = Session("plPlate_ID")
	Barcode = Session("plPlate_Barcode")
End if

LocationID = Request.QueryString("LocationID")
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Create a Plate Map</title>
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
			errmsg = errmsg + "- Plate Map Location is required.\r";
			bWriteError = true;
		}
		else{
			// Destination must be a number
			if (!isNumber(document.form1.Location_ID_FK.value)){
			errmsg = errmsg + "- Plate Map Location must be a number.\r";
			bWriteError = true;
			}
		}
   		<%if Application("ENABLE_OWNERSHIP")="TRUE" then%>
		     if(GetAuthorizedLocation(document.form1.Location_ID_FK.value,document.getElementById("tempCsUserName").value,document.getElementById("tempCsUserID").value)==0)
            {
                errmsg = errmsg + "- Not athorized for this location.\r";
			    alert(errmsg);
			    return;
            }
    	<%end if%>	       
		// Destination must be plate map location.
		if (!bWriteError){
		    if (!(IsPlateTypeAllowed(0,document.form1.Location_ID_FK.value,0,1) == 1)){
			    errmsg = errmsg + "- This is not a valid plate map location.\r";
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
<form name="form1" xaction="echo.asp" action="CreatePlateMap_action.asp" method="POST">
<input Type="hidden" name="PlateID" value="<%=PlateID%>">
<input Type="hidden" name="multiscan" value="<%=Request("multiscan")%>">
<input TYPE="hidden" NAME="tempCsUserName" id="tempCsUserName" Value="<%=Session("UserName" & "cheminv")%>" >
<input type="hidden" name="tempCsUserID" id="tempCsUserID" value="<%=Server.URLEncode(CryptVBS(Session("UserID" & "cheminv"), "ChemInv\API\GetBatchInfo.asp"))%>" />
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><%=Instructions%></span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Plate map source:
		</td>
		<td>
			<input TYPE="text" SIZE="33" onfocus="blur()" VALUE="<%=Barcode%>" disabled>
		</td>
	</tr>
	<tr height="25">
		<td align="right" nowrap>
			<span class="required" title="LocationID of the destination">Plate Map Location:</span>
		</td>
		<td>
			&nbsp;<%=GetBarcodeIcon()%>&nbsp;<%ShowPlateMapLocationPicker "document.form1", "Location_ID_FK", "Location_BarCode", "Location_Name", 10, 33, false%> 
		</td>
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
