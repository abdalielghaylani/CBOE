<%@ EnableSessionState=True Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim RS
Dim ConStr
Dim strError
Dim bWriteError
Dim PrintDebug

Response.Expires = -1

bDebugPrint = False
bWriteError = False
strError = "Error:ValidateUniqueName<BR>"
NameType = Request("NameType")
Name = Request("Name")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
    Response.Redirect "/cheminv/help/admin/api/ValidateUniqueName.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(NameType) then
	strError = strError & "NameType is required<br>"
	bWriteError = True
End if
If IsEmpty(Name) then
	strError = strError & "Name is required<br>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

if NameType = "organization" then
	strSQL = "select count(org_name) as count from inv_org_unit where org_name = '" & Name & "'"
elseif NameType = "gridformat" then
	strSQL = "select count(name) as count from inv_grid_format where name = '" & Name & "'"
end if
if bdebugPrint then
	Response.Write strSQL
	Response.end
else    
	Response.write GetListFromSQLRS(strSQL)
end if
%>