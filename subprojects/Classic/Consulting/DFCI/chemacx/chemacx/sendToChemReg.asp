<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/chemacx/api/apiutils.asp"-->
<!--#INCLUDE VIRTUAL = "/chemacx/api/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<%
CS_SEC_UserName = Session("CSSUserName" & "chemacx")
if (CS_SEC_UserName <> "") then
	CS_SEC_UserID = Session("CSSUserID" & "chemacx")
	
	
Else
	Response.Write "Error:  No Credentials available to log you into the registration system.<br>  ChemACX must be configured to use CS Security and you must be logged with valid credentials."
	Response.end
End if
'We add the credentials required to login into ChemACX via cs_security
Credentials = "CSUserName=" & CS_SEC_UserName & "&CSUSerID=" & CS_SEC_UserID

CAS = Request("CAS")

if lcase(Request("action")) = "send" then
	StrucData = Request("StrucData")
	ChemicalName = Request("ChemicalName")
	 
	'This are the credentials expected by chem_reg HTTP API
	Credentials = Credentials & "&User_id=" & CS_SEC_UserName & "&User_pwd=" & CS_SEC_UserID


	'Try to authenticate against reg:
	FormData = Credentials & "&Reg_method=db_info&reg_parameter=authenticate"
	httpResponse = CShttpRequest2("POST", Application("RegServer"), "chem_reg/reg/reg_post_action.asp", "ChemACX", FormData)
	
	If  httpResponse <> "user authenticated" then
		Response.Write ucase(CS_SEC_UserName) & " does not have privileges to access the Registration System"
		Response.end
	End if

	FormData = Credentials & "&Reg_method=Reg_Temp&reg_parameter=add_compound&Temporary_Structures.Structure=" & Server.UrlEncode(strucData) & "&Temporary_Structures.CAS_Number=" & CAS & "&Temporary_Structures.Chemical_Name=" & ChemicalName
	
	httpResponse = CShttpRequest2("POST", Application("RegServer"), "chem_reg/reg/reg_post_action.asp", "ChemACX", FormData)
	'Response.Write httpResponse
	'Response.end
	If isNumeric(httpResponse) then
		Response.Write "<script language=javascript>window.open('/chem_reg/reg/reg_post_action.asp?reg_method=edit_temp&user_id=" & CS_SEC_UserName & "&user_pwd=" & CS_SEC_UserID & "&temp_id=" & httpResponse & "');" & vblf
		Response.Write "window.close();" & vblf
		Response.Write "</script>" & vblf
	Else
		Response.Write "Error while sending data to the Registration system:<BR><BR><BR>" & httpResponse
		Response.end
	End if
Else
	StrucID = Request("CsNum")

	DisplayType =  "rawbase64cdx"
	strucdata = CShttpRequest2("POST", Request.ServerVariables("HTTP_HOST"), "ChemACX/chemacx/chemacx_action.asp?dbname=chemacx&dataaction=get_structure2&formgroup=base_form_group&Table=substance&Field=structure&DisplayType=" & DisplayType & "&StrucID=" & StrucID, "ChemACX", Credentials)
	mtCdxPath = Application("TempFileDirectoryHTTP" & "ChemACX") & "mt.cdx"
End if
%>
<!--#INCLUDE VIRTUAL = "/chemacx/chemacx/getSynRS.asp"-->
<html>
<head>
<title>ChemACX --Register a compound</title>
<meta NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
	<script language="JavaScript" src="/cfserverasp/source/chemdraw.js"></script>
	<script>cd_includeWrapperFile("/cfserverasp/source/")</script>
	<script LANGUAGE="javascript" src="/chemacx/Choosecss.js"></script>
</head>
<body>
<center>
<span class="GuiFeedback">Are you sure you want to send this compound to the Registry?<br><br><br></span>
<form name="form1" action="sendToChemReg.asp?action=send&csnum=<%=Request("CsNum")%>" method="POST">
<table border="0">
	<tr>
		<td>
			<table border="1" width="350">
				<tr>	
					<td colspan="2">										
						<input type="hidden" name="strucData" value="<%=strucdata%>"> 
						<script language="JavaScript">
							cd_insertObject("chemical/x-cdx", "285", "230", "CD_1", "<%=mtCdxPath%>", "true", "true", escape('data:chemical/x-cdx;base64,' + document.form1.strucData.value),  "true"); 
						</script>
					</td>
				</tr>
			</table>
		</td>
	</tr>	
	<tr>
		<td>
			<table>				
				<tr>
					<td align="right">CAS:</td>
					<td><input size="12" type="text" name="CAS" value="<%=CAS%>"></td>
				</tr>
				<tr>
					<td align="right">Chemical Name:</td>
					<td>
						<select name="ChemicalName" size="1">
						<%
						if NOT (SynRS.EOF AND SynRS.BOF) then
							While Not SynRS.EOF
							Name = SynRS("Name")
							Response.Write "<option value=""" & Name & """>" & Name
							SynRS.MoveNext
							Wend
						else
							synList = "No names found"
						end if			
						%>
						</select>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>	
		<td align="right"> 
			<a HREF="#" onclick="window.close(); return false"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><input type="image" SRC="../graphics/ok_dialog_btn.gif" border="0" alt="Register compound" WIDTH="61" HEIGHT="21">
		</td>
	</tr>	
</table>
</form>
</center>
</body>
</html>
