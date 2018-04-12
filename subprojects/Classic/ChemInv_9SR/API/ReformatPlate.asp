<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug

Response.Expires = -1

bDebugPrint = FALSE
bWriteError = FALSE
strError = "Error:ReformatPlate<BR>"

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/ReformatPlate.htm"
	Response.end
End if

'Required Paramenters
SourcePlateIDList = Request("SourcePlateIDList")
ReformatMapID = Request("xmldoc_id")
BarcodeList = Request("BarcodeList")
LocationID = Request("LocationID")
PlateTypeID = Request("PlateTypeID")
Amt = Request("Amt")
AmtUnitID = Request("AmtUnitID")
SolventID = Request("SolventID")
SolventVolume = Request("SolventVolume")
SolventVolumeUnitID = Request("SolventVolumeUnitID")
NumTargetPlates = Request("NumTargetPlates")

' Check for required parameters
If IsEmpty(SourcePlateIDList) then
	strError = strError & "SourcePlateIDList is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(ReformatMapID) then
	strError = strError & "ReformatMapID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(BarcodeList) then
	strError = strError & "BarcodeList is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(LocationID) then
	strError = strError & "LocationID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(PlateTypeID) then
	strError = strError & "PlateTypeID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(Amt) then
	strError = strError & "Amt is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(AmtUnitID) then
	strError = strError & "AmtUnitID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(SolventID) then
	strError = strError & "SolventID is a required parameter<BR>"
	bWriteError = True
End if
'If IsEmpty(SolventVolume) then
'	strError = strError & "SolventVolume is a required parameter<BR>"
'	bWriteError = True
'End if
If IsEmpty(SolventVolumeUnitID) then
	strError = strError & "SolventVolumeUnitID is a required parameter<BR>"
	bWriteError = True
End if

'-- set up default values
if SolventVolume = "" then SolventVolume = null 

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

if instr(ReformatMapID, ",") then
	'this is a multiple daughtering
	arrReformatMapIDs = split(ReformatMapID, ",")
	arrPlateIDs = split(SourcePlateIDList, ",")
	arrBarcodeList = split(BarcodeList,",")
	for i = 0 to ubound(arrReformatMapIDs)
		plateBarcodeList = "" 
		start = i*NumTargetPlates
		for j = start to (start + (NumTargetPlates -1))
			plateBarcodeList = plateBarcodeList & arrBarcodeList(j) & ","
		next
		outTemp = callReformatPlates(arrPlateIDs(i), arrReformatMapIDs(i), plateBarcodeList, PlateTypeID, Amt, AmtUnitID, SolventID, SolventVolume, SolventVolumeUnitID, LocationID)
		out = out & outTemp & ","
	next
	out = left(out,len(out)-1)
else
	out = callReformatPlates(SourcePlateIDList, ReformatMapID, BarcodeList, PlateTypeID, Amt, AmtUnitID, SolventID, SolventVolume, SolventVolumeUnitID, LocationID)
end if

Conn.Close

Set Cmd = Nothing
Set Conn = Nothing

Response.ContentType = "Text/Plain"
Response.Write out
Response.end

function callReformatPlates(SourcePlateIDList, ReformatMapID, BarcodeList, PlateTypeID, Amt, AmtUnitID, SolventID, SolventVolume, SolventVolumeUnitID, LocationID)

Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Reformat.REFORMATPLATES", adCmdStoredProc)

Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 200, adParamReturnValue, 4000, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("PSOURCEPLATEIDLIST", 200, 1, 2000, SourcePlateIDList) 	
Cmd.Parameters.Append Cmd.CreateParameter("PREFORMATMAPID", 131, 1, 0, ReformatMapID) 	
Cmd.Parameters.Append Cmd.CreateParameter("PBARCODELIST", 200, 1, 2000, BarcodeList)
Cmd.Parameters.Append Cmd.CreateParameter("PPLATETYPEID", 131, 1, 0, PlateTypeID) 	
Cmd.Parameters.Append Cmd.CreateParameter("PAMT", 5, 1, 0, Amt)
Cmd.Parameters.Append Cmd.CreateParameter("PAMTUNITID", 131, 1, 0, AmtUnitID)
Cmd.Parameters.Append Cmd.CreateParameter("PSOLVENTID", 131, 1, 0, SolventID)
Cmd.Parameters.Append Cmd.CreateParameter("PSOLVENTVOLUME", 5, 1,	0, SolventVolume)
Cmd.Parameters.Append Cmd.CreateParameter("PSOLVENTVOLUMEUNITID", 131, 1, 0, SolventVolumeUnitID)
Cmd.Parameters.Append Cmd.CreateParameter("PLOCATIONID", 131, 1, 0, LocationID)

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End	
Else
	'Conn.Execute("Alter session set sql_trace = true")
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".REFORMATPLATES")
	'Conn.Execute("Alter session set sql_trace = false")
End if

' Return the newly created PlateIDs
theReturn = Cstr(Cmd.Parameters("RETURN_VALUE"))

callReformatPlates = theReturn
end function
</SCRIPT>
