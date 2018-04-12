<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->

<%
Dim Conn
Dim Cmd
Dim RS

kStatusAvailable = 1
kStatusInUse = 9

action= lCase(Request("action"))

Set myDict = multiSelect_dict
if Lcase(Request("multiSelect")) = "true" then 
	ContainerID = DictionaryToList(myDict)
	FirstContainerID = GetFirstInList(ContainerID,",")
	ContainerName =  myDict.count & " containers will be checked " & action
Else
	ContainerID = Session("ContainerID")
	FirstContainerID = ContainerID
	ContainerName = Session("ContainerName")
End if

Select case Lcase(action)
	case "in"
		sql = "SELECT inv_containers.Def_Location_ID_FK, inv_Locations.Location_Name AS DefLocationName, inv_locations.Owner_ID_FK AS LocationOwner, inv_Containers.Owner_ID_FK AS OwnerID FROM inv_Containers, inv_Locations WHERE inv_containers.Def_Location_ID_FK = inv_Locations.Location_ID AND inv_containers.Container_ID=" & FirstContainerID
		'Response.Write sql
		'Response.End
		Call GetInvCommand(sql, 1)
		Set RS = Cmd.Execute
		LocationID = RS("Def_Location_ID_FK")
		
		If IsNull(LocationID) then LocationID = Request.QueryString("LocationID")
		DefaultLocationID = LocationID
		'DJP: changed to current user
		CurrentUserID = Ucase(Session("UserNameChemInv"))
		'CurrentUserID = RS("LocationOwner").value
		if CurrentUserID = "" then CurrentUserID = "Unknown"
		DestinationLocationName = RS("DefLocationName")
		OwnerID = RS("OwnerID")
		ContainerStautusID = kStatusAvailable
		RS.Close
		Conn.Close
		Set RS = Nothing
		Set Conn = Nothing
		MakeDefault = "1"
	case "out"
		LocationID = Session("DefaultLocation")
		CurrentUserID = Ucase(Session("UserNameChemInv"))
		OwnerID = NULL
		'DefaultLocationID = NULL
		DefaultLocationID = Session("CurrentLocationID")
		ContainerStatusID = kStatusInUse
end select	
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Check out/in an Inventory Container</title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();

	var cd_plugin_threshold= <%=Application("CD_PLUGIN_THRESHOLD")%>;
    var CD_AUTODOWNLOAD_PLUGIN = "<%=APPLICATION("CD_PLUGIN_DOWNLOAD_PATH")%>";

	function ValidateCheckout(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		//LocationID is required
		//document.form1.LocationID.value = document.form1.lpLocationID.value;
		if (document.form1.LocationID.value.length == 0) {
			errmsg = errmsg + "- Location ID is required.\r";
			bWriteError = true;
		}
		else{
			// LocationID must be a psositve number
			if (!isPositiveNumber(document.form1.LocationID.value)){
				errmsg = errmsg + "- Location ID must be a positive number.\r";
				bWriteError = true;
			}
		}

		//Custom Check In field validation
		<%if action = "in" and Application("ShowCheckInDetails") then
			For each Key in custom_checkin_fields_dict
				If InStr(Key, "DATE") then%>
					if (document.form1.i<%=Key%>.value.length > 0 && !isDate(document.form1.i<%=Key%>.value)){
						errmsg = errmsg + "- <%=custom_checkin_fields_dict.Item(Key)%> must be in " + dateFormatString + " format.\r";
						bWriteError = true;
					}
				<%end if%>
			<%next%>
		
			//Validate requried custom fields
			<% For each Key in req_custom_checkin_fields_dict%>
				if (document.form1.i<%=Key%>.value.length == 0) {
					errmsg = errmsg + "- <%=req_custom_checkin_fields_dict.Item(Key)%> is required.\r";
					bWriteError = true;
				}
			<%Next%>
		<%end if%>
			
		if (bWriteError){
			alert(errmsg);
		}
		else{
			<%if action = "in" then%>
				document.form1.DefaultLocationID.value = document.form1.LocationID.value;
			<%end if%>
			document.form1.submit();
		}
	}
-->
</script>
</head>
<body>
<center>
<form name="form1" xaction="echo.asp" action="CheckOut_action.asp" method="POST">
<input Type="hidden" name="ContainerID" value="<%=ContainerID%>">
<input Type="hidden" name="DefaultLocationID" value="<%=DefaultLocationID%>">
<input Type="hidden" name="multiscan" value="<%=Request("multiscan")%>">
<INPUT TYPE="hidden" NAME="action" VALUE="<%=action%>">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Check <%=Lcase(action)%> an inventory container.</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Container:
		</td>
		<td>
			<input TYPE="tetx" SIZE="44" Maxlength="50" VALUE="<%=ContainerName%>" onfocus="blur()" disabled id="Text1" name="Text1">
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required" title="Destination Location">Destination Location: <%=GetBarcodeIcon()%></span>
		</td>
		<td>
			<%ShowLocationPicker "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 30, false%> 
		</td>
	</tr>
	<tr>
		<td align="right">
			<span class="required">Current User:</span>
		</td>
		<td>
		<%=ShowSelectBox2("CurrentUserID", Ucase(CurrentUserID), "SELECT Upper(User_ID) AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC", 30,"","")%>
		</td>
	</tr>
	<%
	if action = "in" and Application("ShowCheckInDetails") then
		For each key in custom_checkin_fields_dict
			Response.Write "<tr height=""25"">" & vblf
			if ucase(Key) ="USER_ID_FK" then
				Response.Write "<td align=""right"" valign=""top"" nowrap width=""150"">"
				if req_custom_checkin_fields_dict.Exists(Key) then Response.Write "<span class=""required"">"
				Response.Write custom_checkin_fields_dict.Item(key)
				if req_custom_checkin_fields_dict.Exists(Key) then Response.Write "</span>"
				Response.Write ":</td>"
				Response.Write "<td>"
				Response.Write ShowSelectBox2("USER_ID_FK", Ucase(CurrentUserID), "SELECT Upper(User_ID) AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC", 30,"","")
				Response.Write "</td>"
			elseif inStr(uCase(Key), "DATE_") then
				Response.Write "<td align=""right"" valign=""top"" nowrap width=""150"">" 
				if req_custom_checkin_fields_dict.Exists(Key) then Response.Write "<span class=""required"">"
				Response.Write custom_checkin_fields_dict.Item(key)
				if req_custom_checkin_fields_dict.Exists(Key) then Response.Write "</span>"
				Response.Write ":</td>"
				Response.Write "<td>"
				call ShowInputField("", "", "i" & Key & ":form1:" & eval(Key) , "DATE_PICKER:TEXT", "15")
				Response.Write "</td>"
			
				'str = ShowInputBox(custom_fields_dict.Item(key), Key, 25, "", False, req_custom_fields_dict.Exists(Key))
				'Response.write Left(str,len(str)-5)
				'Response.Write "<a href onclick=""return PopUpDate(&quot;i" & Key & "&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)""><img src=""/cfserverasp/source/graphics/navbuttons/icon_mview.gif"" height=""16"" width=""16"" border=""0""></a>"
				'Response.Write "</td>" & vblf
			Else
				Response.write ShowInputBox(custom_checkin_fields_dict.Item(key) & ":", Key, 15, "", False, req_custom_checkin_fields_dict.Exists(Key)) & vblf
			end if
			Response.Write "</TR>" 
		Next 
	
	end if
	%>
	<%'if action = "in" then %>
<!--
	<tr>
		<td align="right">
			Owner:
		</td>
		<td>
		<%=ShowSelectBox2("OwnerID", OwnerID, "SELECT owner_ID AS Value, description AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_owners ORDER BY lower(description) ASC", 30, RepeatString(43, "&nbsp;"), "")%>
		</td>
	</tr>
-->
	<%'end if%>
	<tr>
		<td align="right" nowrap>
			<span title="Container Status">Container Status:</span>
		</td>
		<td>
			<%=ShowSelectBox2("ContainerStatusID", ContainerStatusID, "SELECT Container_Status_ID AS Value, Container_Status_Name AS DisplayText FROM "  & Application("CHEMINV_USERNAME") &  ".inv_container_status ORDER BY lower(Container_Status_Name) ASC", 27, "", "")%> 
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateCheckout(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" alt="Check out/in container" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>		
</table>	
</form>
</center>
</body>
</html>
