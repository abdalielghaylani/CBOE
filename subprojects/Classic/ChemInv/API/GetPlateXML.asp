<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug

if isEmpty(Request("bDebugPrint")) then 
	bDebugPrint = False
else
	bDebugPrint = cbool(request("bDebugPrint"))
end if
bWriteError = False
strError = "Error:GetPlateXML<BR>"
PlateIDs = Request("PlateIDs")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/GetPlateXML.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(PlateIDs) then
	strError = strError & "PlateIDs is a required parameter<BR>"
	bWriteError = True
End if

If bWriteError then
	Response.Write strError
	Response.End
end if

' Set up an ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Reformat.GetPlateXML", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", AdLongVarChar, adParamReturnValue, 300000, NULL)
Cmd.Properties("SPPrmsLOB") = TRUE
Cmd.Parameters.Append Cmd.CreateParameter("pPlateIDs", 200 , 1, 1000,"1004")
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Reformat.GetPlateXML")
End if

'transform PlateXML into canonical form
Set oPlateXML = server.CreateObject("MSXML2.FreeThreadedDOMDocument")
Set oCanonicalPlateXML = server.CreateObject("MSXML2.FreeThreadedDOMDocument")
oPlateXML.loadXML(Cmd.Parameters("RETURN_VALUE"))

if Not isObject(Application("oGetPlateXMLTransform")) then
	Application.Lock
	Set oGetPlateXMLTransform = Server.CreateObject("MSXML2.FreeThreadedDOMDocument")
	oGetPlateXMLTransform.load(Server.MapPath("/cheminv/config/xml_templates/GetPlateXML.xsl"))
	Set Application("oGetPlateXMLTransform") = oGetPlateXMLTransform
	Set oGetPlateXMLTransform = nothing
	Application.UnLock
end if
	
Set oGetPlateXMLTransform = Application("oGetPlateXMLTransform") 
oPlateXML.transformNodeToObject oGetPlateXMLTransform, oCanonicalPlateXML
Response.ContentType = "Text/Plain"
Response.Write oCanonicalPlateXML.xml

'Clean up
Cmd.Properties("SPPrmsLOB") = FALSE
Conn.Close
Set oPlateXML = nothing
Set oCanonicalPlateXML = nothing
set oGetPlateXMLTransfrom = nothing
Set Conn = Nothing
Set Cmd = Nothing

Response.End
</SCRIPT>
