<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetLocationAuthorizedAttribute.asp" -->
<% end if %>
<%

'Response.Write(Request.Form & "<br><br>")
'Response.Write(Request.QueryString & "<br><br>")

Dim Conn
Set myDict = multiSelect_dict
if Lcase(Request("multiSelect")) = "true" then 
	ContainerID = DictionaryToList(myDict)
	ContainerName =  myDict.count & " containers will be moved"
Else
	ContainerID = Session("ContainerID")
	ContainerName = Session("ContainerName")
End if

if Request("LocationID") <> "" then
	if Request("GetSessionLocationID") = "" then 
		LocationID = Request("LocationID")
		Session("CurrentLocationID") = LocationID
	else
		LocationID = Session("CurrentLocationID")
	end if 
else
	LocationID = Session("CurrentLocationID")
end if
if isBlank(LocationID) then LocationID = 0
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Move Inventory Container</title>
<script type="text/javascript" language="javascript" src="/cheminv/choosecss.js"></script>
<script type="text/javascript" language="javascript" src="/cheminv/utils.js"></script>
<script type="text/javascript" language="javascript" src="/cheminv/gui/validation.js"></script>

<style>
	.singleRackElement {
		display:none;
	}
	.suggestRackList {
		display:none;
	}
	.locationRackList {
		display:none;
	}
	.rackLabel {color:#000;}
</style>

<script type="text/javascript" language="javascript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	function ValidateMove(){		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		// Destination is required
		if (document.form1.LocationID.value.length == 0) {
			errmsg = errmsg + "- Destination is required.\r";
			bWriteError = true;
		}
		else{
			// Destination must be a number
			if (!isNumber(document.form1.LocationID.value)){
			errmsg = errmsg + "- Destination must be a number.\r";
			bWriteError = true;
			}
		}
   		<%if Application("ENABLE_OWNERSHIP")="TRUE" then%>				
	     if(GetAuthorizedLocation(document.form1.LocationID.value,document.getElementById("tempCsUserName").value,document.getElementById("tempCsUserID").value)==0)
            {
                errmsg = errmsg + "- Not authorized for this location.\r";
			    alert(errmsg);
			    return;
            }
       <%end if%>  
        
		if (document.form1.LocationID.value == 0) {
			errmsg = errmsg + "- Cannot move containers to root location.\r";
			bWriteError = true;
		}


		// Validation for Rack assignment
		<% if Lcase(Request("multiSelect")) = "true" then %>
		var numContainers = <%=myDict.count%>;
		<% else %>
		var numContainers = 1;
		<% end if%>
		
		// determine whether there are enough open rack positions
		if (document.form1.isRack.value == "1") {
		    if (AreEnoughRackPositions(numContainers,document.form1.LocationID.value) == false) {
				errmsg = errmsg + "- There are not enough open positions from the selected start position\rin the rack(s) you have selected.\r";
				bWriteError = true;
		    }
		}
		//alert(AreEnoughRackPositions(numContainers,document.form1.LocationID.value));
		//alert(numContainers + "::" + document.form1.LocationID.value);
		//alert("'" + document.form1.isRack.value + "'");
		//alert(document.form1.isRack.value == "1")
		//bWriteError = true;
	
		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.submit();
		}
	}

	function validateLocation(LocationID){
		CurrLocationID = 0;
		CurrLocationID = <%=LocationID%>;
		if (LocationID != CurrLocationID){
			<% 
			if Lcase(Request("multiSelect")) = "true" then 
				multiSelect = "?multiSelect=true&" 
			else
				multiSelect = "?" 
			end if
			%>
			document.form1.action = "MoveContainer.asp<%=multiSelect%>LocationID="+LocationID;
			document.form1.submit();
		}
	}


-->
</script>
</head>
<body>
<center>
<form name="form1" xaction="echo.asp" action="MoveContainer_action.asp" method="POST">
<%if len(ContainerID) = 0 then%>
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>You must select containers to move.</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	
<%else%>
<input type="hidden" name="ContainerID" value="<%=ContainerID%>">
<input type="hidden" name="GetSessionLocationID" value>
<input type="hidden" name="multiscan" value="<%=Request("multiscan")%>">
<input type="hidden" name="tempCSUserName" value="<%=Session("UserName" & "cheminv")%>" />
<input type="hidden" name="tempCsUserID" value="<%=Server.URLEncode(CryptVBS(Session("UserID" & "cheminv"),"ChemInv\API\GetBatchInfo.asp"))%>" />
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Move an inventory container.</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Container to move:
		</td>
		<td>
			<input type="tetx" size="44" onfocus="blur()" value="<%=ContainerName%>" disabled>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required" title="LocationID of the destination">Destination Location:</span>
		</td>
		<td>
			<!--<%ShowLocationPicker "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 31, false%> -->
			&nbsp;<%=GetBarcodeIcon()%>&nbsp;<%ShowLocationPicker3 "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 20, false,""%> 
			<%'ShowLocationPicker3 "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 20, false,"validateLocation(document.form1.LocationID.value)"%> 
		</td>
	</tr>
   	<tr>
		<td colspan="2" align="right"> 
			<a href="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img src="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>&nbsp;<a href="#" onclick="ValidateMove(); return false;"><img src="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>	
</table>	
<%end if%>
</form>

</center>
</body>
</html>
