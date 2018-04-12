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
strError = "Error:CreatePhysPlateType<BR>"

pRowCount = Request("pRowCount")
pColCount = Request("pColCount")
pRowPrefix= Request("pRowPrefix")
pColPrefix= Request("pColPrefix")
pRowUseLetters= Request("pRowUseLetters")
pColUseLetters= Request("pColUseLetters")
pNameSeparator= Request("pNameSeparator")
pNumberStartCorner= Request("pNumberStartCorner")
pNumberDirection= Request("pNumberDirection")
pPhysPlateName =  Request("pPhysPlateName")
pIsPreBarcoded = Request("pIsPreBarcoded")
pSupplierID = Request("SupplierID")
pCapacityUnitID = Request("pCapacityUnitID")
pWellCapacity = Request("pWellCapacity")
pZeroPaddingCount = Request("pZeroPaddingCount")
pCellNaming = Request("p_cell_naming")
if pCellNaming ="" then pCellNaming = 0
' Redirect to help page izf no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/CreatePhysPlateType.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(pPhysPlateName) then
	strError = strError & "Name is a required parameter.<BR>"
	bWriteError = True
End if
If IsEmpty(pRowCount) then
	strError = strError & "RowCount is a required parameter.<BR>"
	bWriteError = True
End if
If IsEmpty(pColCount) then
	strError = strError & "ColCount is a required parameter.<BR>"
	bWriteError = True
End if
If IsEmpty(pWellCapacity) then
	strError = strError & "WellCapacity is a required parameter.<BR>"
	bWriteError = True
End if
If IsEmpty(pCapacityUnitID) then
	strError = strError & "CapacityUnitID is a required parameter.<BR>"
	bWriteError = True
End if
If IsEmpty(pZeroPaddingCount) then
	strError = strError & "ZeroPaddingCount is a required parameter.<BR>"
	bWriteError = True
End If
' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".PLATESETTINGS.CreatePhysPlateType", adCmdStoredProc)

Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("pPhysPlateName", 200, 1, 4000, pPhysPlateName)
Cmd.Parameters.Append Cmd.CreateParameter("pRowCount",131, 1, 0, pRowCount)
Cmd.Parameters("pRowCount").Precision = 6
Cmd.Parameters.Append Cmd.CreateParameter("pColCount",131, 1, 0, pColCount)
Cmd.Parameters("pColCount").Precision = 6
Cmd.Parameters.Append Cmd.CreateParameter("pRowPrefix",200, 1, 50, pRowPrefix)
Cmd.Parameters.Append Cmd.CreateParameter("pColPrefix",200, 1, 50, pColPrefix)
Cmd.Parameters.Append Cmd.CreateParameter("pRowUseLetters",131, 1, 0, pRowUseLetters)
Cmd.Parameters("pRowUseLetters").Precision = 1
Cmd.Parameters.Append Cmd.CreateParameter("pColUseLetters",131, 1, 0, pColUseLetters)
Cmd.Parameters("pColUseLetters").Precision = 1
Cmd.Parameters.Append Cmd.CreateParameter("pNameSeparator",200, 1, 1, pNameSeparator)
Cmd.Parameters.Append Cmd.CreateParameter("pNumberStartCorner",131, 1, 0, pNumberStartCorner)
Cmd.Parameters("pNumberStartCorner").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("pNumberDirection",131, 1, 0, pNumberDirection)
Cmd.Parameters("pNumberDirection").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("pZeroPaddingCount", 131, 1, 0, pZeroPaddingCount)
Cmd.Parameters.Append Cmd.CreateParameter("pSupplierID",131, 1, 0, pSupplierID)
Cmd.Parameters("pSupplierID").Precision = 4
Cmd.Parameters.Append Cmd.CreateParameter("pIsPreBarcoded",200, 1, 1, pIsPreBarcoded)
Cmd.Parameters.Append Cmd.CreateParameter("pWellCapacity",5, 1, 0, pWellCapacity)
Cmd.Parameters.Append Cmd.CreateParameter("pCapacityUnitID",131, 1, 0, pCapacityUnitID)
Cmd.Parameters("pCapacityUnitID").Precision = 4
Cmd.Parameters.Append Cmd.CreateParameter("pCellNaming",200, 1, 1, pCellNaming)

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".PLATESETTINGS.CreatePhysPlateType")
End if

' Return the newly created LocationID
Response.Write Cmd.Parameters("RETURN_VALUE")

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
