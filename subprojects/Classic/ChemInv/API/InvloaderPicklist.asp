<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<% 
    pStr = "private, no-cache, must-revalidate" 
    Response.ExpiresAbsolute = #2000-01-01# 
    Response.AddHeader "pragma", "no-cache" 
    Response.AddHeader "cache-control", pStr 
%><%
Dim Conn
Dim RS
Dim ConStr
Dim strError
Dim bWriteError
Dim bDebugPrint
Dim CSUserName
Dim CSUserID

bDebugPrint = False
bWriteError = False
strError = "Error:InvLoaderPicklist<BR>"
Picklist = Request("PicklistName")
Params = Request("Params")

CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")


' Check for required parameters
If IsEmpty(Picklist) then
	strError = strError & "PicklistName is a required parameter<BR>"
	Response.Write strError
	Response.end
End if

'This XML load should be moved where it is done less often

sURL = Server.MapPath("\cheminv\config\invloader.xml")
Set oFieldXML = CreateObject("MSXML2.DOMDocument")
oFieldXML.async = False
oFieldXML.load sURL

If Len(oFieldXML.xml) = 0 then
	strError = strError & "Could not load invloader.xml<BR>"
	Response.Write strError
	Response.end
End if

Set oNode = oFieldXML.SelectSingleNode("//picklists/picklist[@name='" & PickList & "']")
If oNode is nothing then
	strError = strError & "Could not find picklist with name '" & PickList & "' in invloader.xml<BR>"
	Response.Write strError
	Response.end
End if

strSQL = oNode.text

GetInvConnection()
Set rs = Server.CreateObject("ADODB.Recordset")
		
' Open Recordset and save to a stream.	
rs.CursorLocation = adUseClient		
rs.Open strSql, Conn, adOpenForwardOnly, adLockReadOnly
     	
Response.ContentType = "text/xml"
rs.Save Response, adPersistXML		
		
' Cleanup:
rs.Close
Set rs = Nothing

%>
