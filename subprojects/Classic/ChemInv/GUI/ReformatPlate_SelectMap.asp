<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<%
Dim Conn
Dim Cmd
Dim RS

bDebugPrint = false
'-- get QS values
multiSelect = lcase(Request("multiSelect"))
reformatAction = lcase(Request("reformatAction"))

'-- set values to be uesd in GUI
instructionText = "Reformat inventory plates."
stepCount = 4
stepValue = 1

Call GetInvConnection()

if multiSelect = "true" then 
	Set myDict = plate_multiSelect_dict
	plateCount =  plate_multiSelect_dict.count
	if plateCount = 0 then
		action = "noPlates"
	else
		plateID = DictionaryToList(myDict)
		Barcode =  myDict.count & " plates selected."
	end if
Else
	plateID = Session("plPlate_ID")
	Barcode = Session("plBarcode")
End if
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Reformat Plates</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	function Validate(){
		var bWriteError = false;

		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.all.form1.action = "ReformatPlate_SelectOrder.asp";
			document.form1.submit();
		}
	}
-->
</script>
</head>
<body>
<center>
<%if action = "noPlates" then%>
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>You must select plates to reformat.</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>			
		</td>
	</tr>	
</table>	
<%else%>

<form name="form1" method="POST">
<INPUT TYPE="hidden" NAME="multiselect" VALUE="<%=multiselect%>">
<input Type="hidden" name="plateID" value="<%=plateID%>">
<INPUT TYPE="hidden" NAME="reformatAction" VALUE="<%=reformatAction%>">
<INPUT TYPE="hidden" NAME="stepCount" VALUE="<%=stepCount%>">
<INPUT TYPE="hidden" NAME="stepValue" VALUE="<%=stepValue%>">
<table border="0" width="100%">
	<tr>
		<td colspan="2" align="center">
			<span class="GuiFeedback">
			<%if len(errText) > 0  then
				Response.Write errText
			else 
				Response.Write instructionText
			end if
			%>
			</span><br><br>
			<table bgcolor="#e1e1e1" width="100%">
				<tr><td align="center">Step <strong><%=stepValue%></strong> of <%=stepCount%></td></tr>
			</table>	
		</td>
	</tr>
</table>
<table border="0">
<%
	'-- get valid maps for selected plates, ie, finds maps with the source plates of the selected plate formats
	Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Reformat.GETVALIDREFORMATMAPS", adCmdStoredProc)
	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 200, adParamReturnValue, 4000, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PSOURCEPLATEIDS", 200, 1, 2000, PlateID)
	if bDebugPrint then Call DebugCommand(Cmd, true)
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Reformat.GETVALIDREFORMATMAPS")
	validMaps = trim(Cstr(Cmd.Parameters("RETURN_VALUE")))
	'Response.Write validMaps & "=validMaps<BR>"
	bValid=false
	if instr(validMaps, ",") then 
		bValid = true
	elseif cint(validMaps) > 0 then
		bValid = true
	end if
	'if cInt(validMaps) < 0 then
	if not bValid then
%>
	<TR>
		<TD COLSPAN="2">There are no valid reformat maps for the selected plates.</TD>
	</TR>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="opener.focus(); window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
<%	
	else
%>	
	<tr>
		<td align="right" nowrap>Plates to reformat:</td>
		<td>
			<input TYPE="text" SIZE="33" onfocus="blur()" VALUE="<%=Barcode%>" disabled>
		</td>
	</tr>
	<tr height="25">
		<%=ShowPickList("<span class=""required"" title=""Select a Reformat Map"">Select a Reformat Map:</span>", "XMLDoc_ID", xmldoc_id, "SELECT XMLDOC_ID AS Value, NAME AS DisplayText FROM inv_xmldocs WHERE xmldoc_type_id_FK IN (1) AND xmldoc_id in (" & validMaps & ") ORDER BY lower(DisplayText) ASC")%>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="opener.focus(); window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>&nbsp;&nbsp;&nbsp;&nbsp;<a HREF="#" onclick="Validate(); return false;"><img SRC="../graphics/next_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
<%
	end if
%>
</table>	
</form>
<%end if%>
</center>
</body>
</html>
<%
Conn.Close
Set Cmd = Nothing
Set Conn = Nothing
%>
