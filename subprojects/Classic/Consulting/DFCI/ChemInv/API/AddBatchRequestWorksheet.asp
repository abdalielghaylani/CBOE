<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/fieldArrayUtils.asp" -->

<%
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug

bDebugPrint = false
bWriteError = false
strError = "Error:CancelRequest<BR>"

RequestID = Request("RequestID")
RackIDList = Request("RackIDList")
ContainerIDList = Request("ContainerIDList")
ContainerGridPositionIDList = Request("ContainerGridPositionIDList")

'-- Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/AddBatchRequestWorkSheet.htm"
	Response.end
End if

'-- Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

'-- Check for required parameters
If IsEmpty(RequestID) then
	strError = strError & "RequestID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(RackIDList) then
	strError = strError & "RackIDList is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(ContainerIDList) then
	strError = strError & "ContainerIDList is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(ContainerGridPositionIDList) then
	strError = strError & "ContainerGridPositionIDList is a required parameter<BR>"
	bWriteError = True
End if

If bWriteError then
	Response.Write strError
	Response.end
End if

'-- Build Worksheet HTML
caption = "Request Worksheet"
strHtml = ""
strHtml = strHtml & "<html><head></head><body>" & vbcrlf
strHtml = strHtml & "<script language=""javascript"" src=""/cheminv/choosecss.js""></script>" & vbcrlf
strHtml = strHtml & "<div align=""center"">"
strHtml = strHtml & "Date Created: " & Now() & "<br>" & vbcrlf
strHtml = strHtml & "<table class=""grayBackground"" width=""75%""><tr><td align=""center""><span class=""GuiFeedback"">" & caption & "</span></td></tr></table><br />" & vbcrlf
strHtml = strHtml & DisplaySimpleMovedRack(RackIDList,ContainerIDList,ContainerGridPositionIDList)
strHtml = strHtml & "<a href=""javascript:window.print()""><img src=""/cheminv/graphics/sq_btn/print_btn.gif"" border=""0""></a>"
strHtml = strHtml & "&nbsp;<a href=""javascript:window.close()""><img src=""/cheminv/graphics/sq_btn/close_dialog_btn.gif"" border=""0""></a>"
strHtml = strHtml & "</div>"
strHtml = strHtml & "</body></html>"
'Response.Write(strHtml)

'-- Define variables for insert
TableName = "INV_REQUESTS"
FieldName = "REQUEST_ID"
FieldValue = RequestID
DocType = 2
Doc = strHtml


'Response.End

'-- Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".DOCS.INSERTDOC", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9			
Cmd.Parameters.Append Cmd.CreateParameter("P_TABLENAME", 200, adParamInput, 255, TableName) 
Cmd.Parameters.Append Cmd.CreateParameter("P_FIELDNAME", 200, adParamInput, 255, FieldName) 
Cmd.Parameters.Append Cmd.CreateParameter("P_FIELDVALUE", 200, adParamInput, 255, FieldValue) 
Cmd.Parameters.Append Cmd.CreateParameter("P_DOCTYPE",adNumeric, adParamInput, 0, DocType) 
Cmd.Parameters("P_DOCTYPE").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("P_DOC", 201, adParamInput, len(Doc)+1, Doc)
Cmd.Properties("SPPrmsLOB") = TRUE

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<br>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".DOCS.INSERTDOC")
End if

Cmd.Properties("SPPrmsLOB") = FALSE

'-- Return code
Response.Write Cmd.Parameters("RETURN_VALUE")

'-- Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing

%>
