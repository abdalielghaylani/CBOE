<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->

<%
Dim Conn
Dim Cmd
Dim RS

StatusCertified = Application("StatusCertified")
ContainerID = Session("ContainerID")
ContainerName = Session("ContainerName")
Purity = Session("Purity")
UOPAbv = Session("UOPAbv")
StorageConditions = Session("StorageConditions")
HandlingProcedures = Session("HandlingProcedures")

GetInvConnection()
SQL = "SELECT container_status_name FROM inv_container_status WHERE container_status_id = ?" 
Set Cmd = GetCommand(Conn, SQL, adCmdText)
Cmd.Parameters.Append Cmd.CreateParameter("StatusID", adNumeric, 1, 0, cInt(StatusCertified))
Set RS = Cmd.Execute
StatusName = RS("container_status_name")

dbkey = Application("appkey")
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Certify an Inventory Container</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();

	function ValidateCertify(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		// Interval is required
		if (document.form1.Interval.value.length == 0) {
			errmsg = errmsg + "- Recertification Interval is required.\r";
			bWriteError = true;
		}
		else{
			// Interval must be a positve number
			if (document.form1.Interval.value.length > 0 && !isPositiveNumber(document.form1.Interval.value)){
				errmsg = errmsg + "- Interval must be a positive number.\r";
				bWriteError = true;
			}
		}

		// Purity if given must be a positve number
		if (document.form1.Purity.value.length > 0 && !isPositiveNumber(document.form1.Purity.value)){
			errmsg = errmsg + "- Purity must be a positive number.\r";
			bWriteError = true;
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
<form name="form1" xaction="echo.asp" action="Certify_action.asp" method="POST">
<input Type="hidden" name="ContainerID" value="<%=ContainerID%>">
<input type="hidden" name="StatusIDFK" value="<%=StatusCertified%>">
<table border="0">
	<tr>
		<td colspan="2" align="center">
			<span class="GuiFeedback">Certify an inventory container.</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>Container:</td>
		<td>
			<input TYPE="text" SIZE="25" Maxlength="50" VALUE="<%=ContainerName%>" onfocus="blur()" disabled id="Text1" name="Text1">
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>Container Status:</td>
		<td>			
			<input TYPE="text" SIZE="25" Maxlength="50" VALUE="<%=StatusName%>" onfocus="blur()" disabled id="Text2" name="Text2">
		</td>
	</tr>
	<tr>
		<td align="right" nowrap><span class="required">Recertification Interval (months):</span></td>
		<td>			
			<input TYPE="text" SIZE="25" Maxlength="50" VALUE="" name="Interval">
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Purity (<%=UOPAbv%>):
		</td>
		<td>			
			<input TYPE="text" SIZE="25" Maxlength="50" VALUE="<%=Purity%>" name="Purity">
		</td>
	</tr>
	<tr height="150">
		<td align="right" valign="top" nowrap>
			Storage Conditions:
		</td>
		<td valign="top">
			<textarea rows="8" cols="30" name="StorageConditions" wrap="hard"><%=StorageConditions%></textarea>
		</td>
	</tr>
	<tr height="150">
		<td align="right" valign="top" nowrap>
			Handling Procedures:
		</td>
		<td valign="top">
			<textarea rows="8" cols="30" name="HandlingProcedures" wrap="hard"><%=HandlingProcedures%></textarea>
		</td>
	</tr>
	
<!--
	<tr>
		<td align="right" nowrap>Expiration Date:</td>
		<td>
			<input TYPE="text" SIZE="25" Maxlength="50" VALUE="" onfocus="blur()" disabled name="ExpirationDateText">
		</td>
	</tr>
-->
	<tr>
		<td colspan="2" align="right">	
							<%IF CBool(Application("SHOW_DOCMANAGER_LINK")) then%>
								<%If Session("SEARCH_DOCS" & dbkey) then%>
								<a class="MenuLink" href="Manage%20Documents%20for%20this%20container" onclick="OpenDialog('/cheminv/gui/manageDocuments.asp?FK_value=<%=ContainerID%>&FK_name=CONTAINER%20ID&Table_Name=INV_CONTAINERS&LINK_TYPE=CHEMINVCONTAINERID', 'Documents_Window', 2); return false;" title="Manage documents associated to this compound">Manage Documents</a>
								<%else%>
								<a class="MenuLink" href="Manage%20Documents%20for%20this%20container" onclick="alert('You do not have the appropriate privileges to add or view documents. Please ask the administrators to grant you DOCMGR_EXTERNAL role and log back in to try again.'); return false;">Manage Documents</a>
								<%end if%>
								<br>
							<%end if%>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="opener.focus(); window.close();  return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateCertify(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" alt="Check out/in container" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>		
</table>	
</form>
</center>
</body>
</html>