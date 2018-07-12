<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Set myDict = plate_multiSelect_dict
if Lcase(Request("multiSelect")) = "true" then 
	if myDict.count > 1 then 
		pluralize = "s"
		pluralize2 = "these plates"
	else
		pluralize = ""
		pluralize2 = "this plate"
	end if
	PlateID = DictionaryToList(myDict)
    DisplayText = myDict.count & " plate" & pluralize & " will be deleted"
    DisplaySpan = "block"
    If Len(Trim(PlateID)) > 2000 OR myDict.count > 300 then
	    DisplayText = "Please select 300 or less plates."
        DisplaySpan = "none"
    End if
	
Else
	PlateID = Session("plPlate_ID")
	PlateName = Session("plPlate_Name")
	pluralize2 = Session("plPlate_Barcode")
	DisplayText = Session("plPlate_Barcode")
End if

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Delete an Inventory Plate</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
-->
</script>
</head>
<body>
<center>
<form name="form1" xaction="echo.asp" action="DeletePlate_action.asp" method="POST">
<input Type="hidden" name="PlateID" value="<%=PlateID%>">
<input Type="hidden" name="multiscan" value="<%=Request("multiscan")%>">

<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>Are you sure you want to move <%=pluralize2%> to the trash?</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Plate to delete:
		</td>
		<td>
			<input TYPE="text" SIZE="60" Maxlength="50" onfocus="blur()" VALUE="<%=DisplayText%>" disabled id="text1" name="text1">
		</td>
	<tr>
		<td colspan="2" align="right"> 
			<table align="right"><tr><td>
	<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
</td><td>
	<a HREF="#" id="spanSubmit" style="display:<%=DisplaySpan%>" onclick="return document.forms.form1.submit()"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
	</td></tr></table>
		</td>
	</tr>	
</table>	
</form>
</center>
</body>
</html>
