<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim bDebugPrint

bDebugPrint = false
bWriteError = false
strError = "Error:CreateDataMap<BR>"

dataMapTypeID = Request("MapTypeID")
dataMapName = Request("TemplateName")
dataMapComments = Request("Remarks")
if Request("NumHeaderLines") <> "" then
	numHeaderRows = Request("NumHeaderLines")
else
	numHeaderRows = 0
end if
numColumns = Request("NumDataCol")
columnDelimiter = Request("DataColDel")
dataMapFieldList = Request("dataMapFieldList")
dataMapColumnList = Request("dataMapColumnList")
useWellCoordinates = Request("UseWellCoordinates")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/CreateDataMap.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(dataMapName) then
	strError = strError & "Data Map Name is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(dataMapTypeID) then
	strError = strError & "Data Map Type is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(dataMapComments) then
	strError = strError & "Comments is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(numColumns) then
	strError = strError & "Number of Columns is a required parameter.<BR>"
	bWriteError = True
End If
If IsEmpty(columnDelimiter) then
	strError = strError & "Column Delimiter is a required parameter.<BR>"
	bWriteError = True
End If
'Response.Write(dataMapName & "<br>" & dataMapTypeID & "<br>"& dataMapComments & "<br>Header Rows:[" & numHeaderRows & "]<br>" & numColumns & "<br>" & columnDelimiter & "<br>"& dataMapFieldList & "<br>" & dataMapColumnList)
'Response.end
' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".DATAMAPS.CreateDataMap", adCmdStoredProc)

Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 16
Cmd.Parameters.Append Cmd.CreateParameter("pName",200 , 1, 50, dataMapName)
Cmd.Parameters.Append Cmd.CreateParameter("pTypeID",131, 1, 0, dataMapTypeID)
Cmd.Parameters.Append Cmd.CreateParameter("pComments",200, 1, 2000, dataMapComments)
Cmd.Parameters.Append Cmd.CreateParameter("pNumHeaderRows",131, 1, 0, numHeaderRows)
Cmd.Parameters.Append Cmd.CreateParameter("pNumColumns",131, 1, 0, numColumns)
Cmd.Parameters.Append Cmd.CreateParameter("pColumnDelimiter",200, 1, 3, columnDelimiter)
Cmd.Parameters.Append Cmd.CreateParameter("pUseWellCoordintes",131, 1, 0, useWellCoordinates)
Cmd.Parameters.Append Cmd.CreateParameter("pDataMapFieldList",200, 1, 2000, dataMapFieldList)
Cmd.Parameters.Append Cmd.CreateParameter("pDataMapColumnList", 200, 1, 2000, dataMapColumnList)
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".DATAMAPS.CreateDataMap")
End if

' Return the newly created LocationID
Response.Write Cmd.Parameters("RETURN_VALUE")

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
