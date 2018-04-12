<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/GetorderContainer_cmd.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp" -->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug
CsUserName =Request("tempCsUserName") 
CsUserID = URLDecode(CryptVBS(request("tempCsUserID"),CsUserName))

Response.Expires = -1

bDebugPrint = False
bWriteError = False
strError = "Error:OrderContainer<BR>"

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/OrderContainer.htm"
	Response.end
End if

'Required Paramenters
LocationID = Request("LocationID")
UOMID = Request("UOMID")
QtyMax = Request("QtyMax")
ContainerTypeID = Request("ContainerTypeID")
ContainerStatusID = Request("ContainerStatusID")

'Optional Parameters
Barcode = Request("Barcode")
CompoundID = Request("CompoundID")
RegID = Request("RegID")
BatchNumber = Request("BatchNumber")
ExpDate = Request("ExpDate")
MinStockQty = Request("MinStockQty")
MaxStockQty = Request("MaxStockQty")
ContainerName = Request("ContainerName")
ContainerDesc = Request("ContainerDesc")
TareWeight = Request("TareWeight")
UOWID = Request("UOWID")
Purity = Request("Purity")
UOPID = Request("UOPID")
Concentration = Request("Concentration")
UOCID = Request("UOCID")
Grade = Request("Grade")
Solvent = Request("Solvent")
Comments = Request("Comments")
SupplierID = Request("SupplierID")
SupplierCatNum = Request("SupplierCatNum")
LotNum = Request("LotNum")
DateProduced = Request("DateProduced")
DateOrdered = Request("DateOrdered")
DateReceived = Request("DateReceived")
ContainerCost =  Request("ContainerCost")
UOCostID = Request("UOCostID")
OwnerID = Request("OwnerID")
CurrentUserID = UCase(Request("tempCSUserName"))
PONumber = Request("PONumber")
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

'MWS: Added extra attributes
DueDate = Request("DueDate")
Project = Request("Project")
Job = Request("Job")
RushOrder = Request("RushOrder")
DeliveryLocationID = Request("DeliveryLocationID")
UnknownSupplierName = Request("UnknownSupplierName")
UnknownSupplierContact = Request("UnknownSupplierContact")
UnknownSupplierPhoneNumber = Request("UnknownSupplierPhoneNumber")
UnknownSupplierFAXNumber = Request("UnknownSupplierFAXNumber")

'MCD: Added for 'Order Reason'
OrderReason = Request("OrderReason")
OrderReasonOther = Request("OrderReasonOther")


' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".OrderContainer", adCmdStoredProc)
Call GetOrderContainer_cmd()

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".OrderContainer")
End if

' Return the newly created ContainerID
out = Cstr(Cmd.Parameters("RETURN_VALUE"))
NewIDs =  Cmd.Parameters("PNEWIDS")
Conn.Close
Set paramR = Nothing
Set Cmd = Nothing
Set Conn = Nothing
' Increment container count
If out > 0 then
	tempArr = split(NewIDS,"|")
	numContainers = Ubound(tempArr) + 1
	containerCount = CLng(Application("inv_Containers" & "RecordCount" & "ChemInv")) + numContainers 
	Application.Lock
		Application("inv_Containers" & "RecordCount" & "ChemInv") = containerCount
	Application.UnLock
	out= NewIDS
End if 
Response.ContentType = "Text/Plain"
Response.Write out
Response.end


'Function to Decode the encrypted string 
Function URLDecode(str) 
	str = Replace(str, "+", " ") 
    For i = 1 To Len(str) 
		sT = Mid(str, i, 1) 
        If sT = "%" Then 
			If i+2 < Len(str) Then 
				sR = sR & _ 
                Chr(CLng("&H" & Mid(str, i+1, 2))) 
                i = i+2 
            End If 
        Else 
			sR = sR & sT 
        End If 
   Next 
   URLDecode = sR 
End Function 

</SCRIPT>
