<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug

bDebugPrint = false
bWriteError = False
strError = "Error:CreateLink<BR>"

FK_value = Request("FK_value")
FK_name = Request("FK_name")
Table_name = Request("Table_name")
URLhref = Request("URLhref")
URLType = Request("URLType")
Image_source = Request("Image_source")
Link_text = Request("Link_text")


' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/CreateLink.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(FK_value) then
	strError = strError & "FK Value is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(FK_name) then
	strError = strError & "FK Name is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(Table_Name) then
	strError = strError & "Table Name is a required parameter<BR>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Links.CreateLink", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9			
Cmd.Parameters.Append Cmd.CreateParameter("PFK_VALUE", 200, adParamInput, 30, FK_Value) 
Cmd.Parameters.Append Cmd.CreateParameter("PFK_NAME", 200, adParamInput, 30, FK_Name) 
Cmd.Parameters.Append Cmd.CreateParameter("PTABLE_NAME", 200, adParamInput, 30, Table_Name) 
Cmd.Parameters.Append Cmd.CreateParameter("PURL", 200, adParamInput, 2000, URLhref) 
Cmd.Parameters.Append Cmd.CreateParameter("PLINKTEXT", 200, adParamInput, 2000, Link_text) 
Cmd.Parameters.Append Cmd.CreateParameter("PIMAGESOURCE", 200, adParamInput, 2000, Image_source) 
Cmd.Parameters.Append Cmd.CreateParameter("PURLTYPE", 200, adParamInput, 50, URLType) 


if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Links.CreateLink")
End if

' Return code
Response.Write Cmd.Parameters("RETURN_VALUE")

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
