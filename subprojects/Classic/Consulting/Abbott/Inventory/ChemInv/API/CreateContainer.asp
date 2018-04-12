<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/Getcreatecontainer_cmd.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug


Response.Expires = -1

bDebugPrint = False
bWriteError = False
strError = "Error:CreateContainer<BR>"

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/CreateContainer.htm"
	Response.end
End if

'Required Paramenters
LocationID = Request("LocationID")
UOMID = Request("UOMID")
QtyMax = Request("QtyMax")
QtyInitial = Request("QtyInitial")
ContainerTypeID = Request("ContainerTypeID")
ContainerStatusID = Request("ContainerStatusID")

'Optional Parameters
Barcode = Request("Barcode")
BarcodeDescID = Request("BarcodeDescID")
CompoundID = Request("CompoundID")
RegID = Request("RegID")
BatchNumber = Request("BatchNumber")
ExpDate = Request("ExpDate")
MinStockQty = Request("MinStockQty")
MaxStockQty = Request("MaxStockQty")
ContainerName = Request("ContainerName")
ContainerDesc = Request("ContainerDesc")
TareWeight = Request("TareWeight")
NetWeight = Request("NetWeight")
FinalWeight = Request("FinalWeight")
UOWID = Request("UOWID")
Purity = Request("Purity")
UOPID = Request("UOPID")
Concentration = Request("Concentration")
Density = Request("Density")
UOCID = Request("UOCID")
UODID = Request("UODID")
Grade = Request("Grade")
SolventIDFK = Request("SolventIDFK")
Comments = Request("Comments")
StorageConditions = Request("StorageConditions")
HandlingProcedures = Request("HandlingProcedures")
SupplierID = Request("SupplierID")
SupplierCatNum = Request("SupplierCatNum")
LotNum = Request("LotNum")
DateProduced = Request("DateProduced")
DateOrdered = Request("DateOrdered")
DateReceived = Request("DateReceived")
ContainerCost =  Request("ContainerCost")
OwnerID = Request("OwnerID")
CurrentUserID = Request("CurrentUserID")
PONumber = Request("PONumber")
POLineNumber = Request("POLineNumber")
ReqNumber = Request("ReqNumber")
NumCopies = Request("NumCopies")
Field_1 = Request("Field_1")
Field_2 = Request("Field_2")
Field_3 = Request("Field_3")
Field_4 = Request("Field_4")
Field_5 = Request("Field_5")
Field_6 = Request("Field_6")
Field_7 = Request("Field_7")
Field_8 = Request("Field_8")
Field_9 = Request("Field_9")
Field_10 = Request("Field_10")
Date_1 = Request("Date_1")
Date_2 = Request("Date_2")
Date_3 = Request("Date_3")
Date_4 = Request("Date_4")
Date_5 = Request("Date_5")


'-- Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".CreateContainer", adCmdStoredProc)
Call GetCreateContainer_cmd()

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".CreateContainer")
End if

' Return the newly created ContainerID
out = Cstr(Cmd.Parameters("RETURN_VALUE"))
NewIDs =  Cmd.Parameters("PNEWIDS")

Set paramR = Nothing
Conn.Close
Set Cmd = Nothing
Set Conn = Nothing


out2 = ""
'-- Increment container count
If out > 0 then
	tempArr = split(NewIDS,"|")
	numContainers = Ubound(tempArr) + 1
	If isEmpty(Application("inv_ContainersRecordCountChemInv")) then
		Application.Lock
			Application("inv_Containers" & "RecordCount" & "ChemInv") = GetContainerCount()
		Application.UnLock
	end if
	containerCount = CLng(Application("inv_Containers" & "RecordCount" & "ChemInv")) + numContainers 
	Application.Lock
		Application("inv_Containers" & "RecordCount" & "ChemInv") = containerCount
	Application.UnLock
	out= NewIDS
End if 
Response.ContentType = "Text/Plain"
Response.Write out

Function GetContainerCount()
	dim RS

	GetInvConnection()
	sql = "SELECT count(*) as count from inv_containers"
	Set RS = Conn.Execute(sql)
	theReturn = RS("count")
	Conn.Close
	Set Conn = nothing
	Set RS = nothing
	GetContainerCount = theReturn
end Function


</SCRIPT>
