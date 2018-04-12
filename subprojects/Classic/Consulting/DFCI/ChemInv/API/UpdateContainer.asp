<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim ContainerID
Dim UOMID
Dim ContainerTypeID
Dim CompoundID
Dim QtyRemaining
Dim QtyMax
Dim MinStockQty
Dim MaxStockQty
Dim ExpDate
Dim ContainerName
Dim ContainerDesc
Dim TareWeight
Dim UOWID
Dim Purity
Dim UOPID
Dim Concentration
Dim UOCID
Dim Grade
Dim Solvent
Dim Comments

Dim Conn
Dim RS

Dim strError
Dim bWriteError
Dim PrintDebug

bDebugPrint = false
bWriteError = False
strError = "Error:UpdateContainer<BR>"

ContainerID = Request("ContainerID")
UOMID = Request("UOMID")
QtyMax = Request("QtyMax")
ContainerTypeID = Request("ContainerTypeID")
CompoundID = Request("CompoundID")
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

' Redirect to help page if no parameters are passed
if Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/UpdateContainer.htm"
	Response.end
End if

' Check for required parameters
if IsEmpty(ContainerID) OR ContainerID = "" then
	strError = strError & "ContainerID is a required parameter<BR>"
	bWriteError = True
End if
If NOT IsEmpty(ExpDate) AND NOT IsDate(ExpDate) then
	strError = strError & "ExpDate could not be interpreted as a valid date<BR>"
	bWriteError = True
End if
' Respond with Error
if bWriteError then
	Response.Write strError
	Response.end
End if

Call GetInvConnection()

' Update the Containers table
UpdateContainerSQL = "UPDATE inv_containers SET "
maybeComma = ""
if NOT IsEmpty(UOMID) then
	if UOMID = "" then UOMID = "NULL"
	UpdateContainerSQL = UpdateContainerSQL & maybeComma & "inv_containers.UNIT_OF_MEAS_ID_FK= " & UOMID
	maybeComma = ", "
End if
if NOT IsEmpty(ContainerName) then
	UpdateContainerSQL = UpdateContainerSQL & maybeComma & "inv_containers.Container_Name= '" & ContainerName & "'"
	maybeComma = ", "
End if
if NOT IsEmpty(ExpDate) then
	ExpDate = GetOracleDateString(CDate(ExpDate))
	UpdateContainerSQL = UpdateContainerSQL & maybeComma & "inv_containers.Date_Expires= " & ExpDate
	maybeComma = ", "
End if
if NOT IsEmpty(ContainerDesc) then
	UpdateContainerSQL = UpdateContainerSQL & maybeComma & "inv_containers.Container_Description= '" & ContainerDesc & "'"
	maybeComma = ", "
End if
if NOT IsEmpty(ContainerTypeID) then
	if ContainerTypeID = "" then ContainerTypeID = "NULL"
	UpdateContainerSQL = UpdateContainerSQL & maybeComma & "inv_containers.Container_Type_ID_FK= " & ContainerTypeID
	maybeComma = ", "
End if
if NOT IsEmpty(CompoundID) then
	if CompoundID = "" then CompoundID = "NULL"
	UpdateContainerSQL = UpdateContainerSQL & maybeComma & "inv_containers.Compound_ID_FK= " & CompoundID
	maybeComma = ", "
End if
if NOT IsEmpty(QtyMax) then
	if QtyMax = "" then QtyMax = "NULL"
	UpdateContainerSQL = UpdateContainerSQL & maybeComma & "inv_containers.Qty_Max= " & QtyMax
	maybeComma = ", "
End if
if NOT IsEmpty(QtyRemaining) then
	if QtyRemaining = "" then QtyRemaining = "NULL"
	UpdateContainerSQL = UpdateContainerSQL & maybeComma & "inv_containers.Qty_Remaining= " & QtyRemaining
	maybeComma = ", "
End if
if NOT IsEmpty(MinStockQty) then
	if MinStockQty = "" then MinStockQty = "NULL"
	UpdateContainerSQL = UpdateContainerSQL & maybeComma & "inv_containers.Qty_MinStock= " & MinStockQty
	maybeComma = ", "
End if
if NOT IsEmpty(MaxStockQty) then
	if MaxStockQty = "" then MaxStockQty = "NULL"
	UpdateContainerSQL = UpdateContainerSQL & maybeComma & "inv_containers.Qty_MaxStock= " & MaxStockQty
	maybeComma = ", "
End if
if NOT IsEmpty(TareWeight) then
	if TareWeight = "" then TareWeight = "NULL"
	UpdateContainerSQL = UpdateContainerSQL & maybeComma & "inv_containers.Tare_Weight= " & TareWeight
	maybeComma = ", "
End if
if NOT IsEmpty(Purity) then
	if Purity = "" then Purity = "NULL"
	UpdateContainerSQL = UpdateContainerSQL & maybeComma & "inv_containers.Purity= " & Purity
	maybeComma = ", "
End if
if NOT IsEmpty(Concentration) then
	if Concentration = "" then Concentration = "NULL"
	UpdateContainerSQL = UpdateContainerSQL & maybeComma & "inv_containers.Concentration= " & Concentration
	maybeComma = ", "
End if
if NOT IsEmpty(Grade) then
	UpdateContainerSQL = UpdateContainerSQL & maybeComma & "inv_containers.Grade= '" & Grade & "'"
	maybeComma = ", "
End if
if NOT IsEmpty(Solvent) then
	UpdateContainerSQL = UpdateContainerSQL & maybeComma & "inv_containers.Solvent= '" & Solvent & "'"
	maybeComma = ", "
End if
if NOT IsEmpty(Comments) then
	UpdateContainerSQL = UpdateContainerSQL & maybeComma & "inv_containers.Container_Comments='" & Comments & "'"
	maybeComma = ", "
End if
if NOT IsEmpty(UOWID) then
	UpdateContainerSQL = UpdateContainerSQL & maybeComma & "inv_containers.Unit_of_WGHT_ID_FK= " & UOWID
	maybeComma = ", "
End if
if NOT IsEmpty(UOCID) then
	UpdateContainerSQL = UpdateContainerSQL & maybeComma & "inv_containers.Unit_of_CONC_ID_FK= " & UOCID
	maybeComma = ", "
End if
if NOT IsEmpty(UOPID) then
	UpdateContainerSQL = UpdateContainerSQL & maybeComma & "inv_Containers.Unit_of_Purity_ID_FK= " & UOPID
	maybeComma = ", "
End if

UpdateContainerSQL = UpdateContainerSQL & " WHERE inv_containers.Container_ID=" & ContainerID
if bDebugPrint then
	Response.Write UpdateContainerSQL  
Else
	Conn.Execute UpdateContainerSQL, lRecsAffected
End if

Conn.Close
Set Conn = Nothing

' Return success code
Response.Write lRecsAffected
</SCRIPT>
