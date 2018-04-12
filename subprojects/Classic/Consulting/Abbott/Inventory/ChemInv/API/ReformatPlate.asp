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
'BarcodeDescIDList = Request("BarcodeDescIDList")
BarcodeList = Request("BarcodeList")
PlateTypeIDList = Request("PlateTypeIDList")
AmtList = Request("AmtList")
AmtUnitIDList = Request("AmtUnitIDList")
AmtType = Request("AmtType")
SolventIDList = Request("SolventIDList")
SolventVolumeList = Request("SolventVolumeList")
SolventVolumeUnitIDList = Request("SolventVolumeUnitIDList")
LocationIDList = Request("LocationIDList")
NumTargetPlates = Request("NumTargetPlates")
TargetVolumeList = Request("TargetVolumeList")
TargetVolumeUnitIDList = Request("TargetVolumeUnitIDList")

' Check for required parameters
If IsEmpty(SourcePlateIDList) then
	strError = strError & "SourcePlateIDList is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(ReformatMapID) then
	strError = strError & "ReformatMapID is a required parameter<BR>"
	bWriteError = True
End if
'If IsEmpty(BarcodeDescIDList) then
'	strError = strError & "BarcodeDescIDList is a required parameter<BR>"
'	bWriteError = True
'End if
If IsEmpty(BarcodeList) then
	strError = strError & "BarcodeList is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(PlateTypeIDList) then
	strError = strError & "PlateTypeIDList is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(AmtList) then
	strError = strError & "AmtList is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(AmtUnitIDList) then
	strError = strError & "AmtUnitIDList is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(AmtType) then
	strError = strError & "AmtType is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(SolventIDList) then
	strError = strError & "SolventIDList is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(SolventVolumeList) then
	strError = strError & "SolventVolumeList is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(SolventVolumeUnitIDList) then
	strError = strError & "SolventVolumeUnitIDList is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(LocationIDList) then
	strError = strError & "LocationIDList is a required parameter<BR>"
	bWriteError = True
End if
If AmtType = "TargetConcentration" and IsEmpty(TargetVolumeList) then
	strError = strError & "TargetVolumeList is a required parameter<BR>"
	bWriteError = True
elseif AmtType <> "TargetConcentration" and IsEmpty(TargetVolumeList) then
	TargetVolumeList = null
end if
If AmtType = "TargetConcentration" and IsEmpty(TargetVolumeUnitIDList) then
	strError = strError & "TargetVolumeUnitIDList is a required parameter<BR>"
	bWriteError = True
elseif AmtType <> "TargetConcentration" and IsEmpty(TargetVolumeUnitIDList) then
	TargetVolumeUnitIDList = null
end if


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
		'plateBarcodeList = trim(left(plateBarcodeList,len(plateBarcodeList)-1))
		'Response.Write plateBarcodeList & "<BR>"
		'outTemp = callReformatPlates(arrPlateIDs(i), arrReformatMapIDs(i), BarcodeDescIDList, PlateTypeIDList, AmtList, AmtUnitIDList, AmtType, SolventIDList, SolventVolumeList, SolventVolumeUnitIDList, LocationIDList, NumTargetPlates, TargetVolumeList, TargetVolumeUnitIDList)
		outTemp = callReformatPlates(arrPlateIDs(i), arrReformatMapIDs(i), plateBarcodeList, PlateTypeIDList, AmtList, AmtUnitIDList, AmtType, SolventIDList, SolventVolumeList, SolventVolumeUnitIDList, LocationIDList, NumTargetPlates, TargetVolumeList, TargetVolumeUnitIDList)
		out = out & outTemp & ","
	next
	'Response.end
	out = left(out,len(out)-1)
else
	'out = callReformatPlates(SourcePlateIDList, ReformatMapID, BarcodeDescIDList, PlateTypeIDList, AmtList, AmtUnitIDList, AmtType, SolventIDList, SolventVolumeList, SolventVolumeUnitIDList, LocationIDList, NumTargetPlates, TargetVolumeList, TargetVolumeUnitIDList)
	out = callReformatPlates(SourcePlateIDList, ReformatMapID, BarcodeList, PlateTypeIDList, AmtList, AmtUnitIDList, AmtType, SolventIDList, SolventVolumeList, SolventVolumeUnitIDList, LocationIDList, NumTargetPlates, TargetVolumeList, TargetVolumeUnitIDList)
end if
'NewIDs =  Cmd.Parameters("PNEWIDS")
Conn.Close

Set Cmd = Nothing
Set Conn = Nothing

Response.ContentType = "Text/Plain"
Response.Write out
Response.end

function callReformatPlates(SourcePlateIDList, ReformatMapID, BarcodeList, PlateTypeIDList, AmtList, AmtUnitIDList, AmtType, SolventIDList, SolventVolumeList, SolventVolumeUnitIDList, LocationIDList, NumTargetPlates, TargetVolumeList, TargetVolumeUnitIDList)

Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Reformat.REFORMATPLATES", adCmdStoredProc)

Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 200, adParamReturnValue, 4000, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("PSOURCEPLATEIDLIST", 200, 1, 2000, SourcePlateIDList) 	
Cmd.Parameters.Append Cmd.CreateParameter("PREFORMATMAPID", 131, 1, 0, ReformatMapID) 	
Cmd.Parameters.Append Cmd.CreateParameter("PBARCODELIST", 200, 1, 2000, BarcodeList)
Cmd.Parameters.Append Cmd.CreateParameter("PPLATETYPEIDLIST", 200, 1, 2000, PlateTypeIDList) 	
Cmd.Parameters.Append Cmd.CreateParameter("PAMTLIST", 200, 1, 2000, AmtList)
Cmd.Parameters.Append Cmd.CreateParameter("PAMTUNITIDLIST", 200, 1, 2000, AmtUnitIDList)
Cmd.Parameters.Append Cmd.CreateParameter("PAMTTYPE", 200, 1, 2000, AmtType)
Cmd.Parameters.Append Cmd.CreateParameter("PSOLVENTIDLIST", 200, 1, 2000, SolventIDList)
Cmd.Parameters.Append Cmd.CreateParameter("PSOLVENTVOLUMELIST", 200, 1, 2000, SolventVolumeList)
Cmd.Parameters.Append Cmd.CreateParameter("PSOLVENTVOLUMEUNITIDLIST", 200, 1, 2000, SolventVolumeUnitIDList)
Cmd.Parameters.Append Cmd.CreateParameter("PLOCATIONIDLIST", 200, 1, 2000, LocationIDList)
Cmd.Parameters.Append Cmd.CreateParameter("PNUMTARGETPLATES", 5, 1, 0, NumTargetPlates) 	
Cmd.Parameters.Append Cmd.CreateParameter("PTARGETVOLUMELIST", 200, 1, 2000, TargetVolumeList) 	
Cmd.Parameters.Append Cmd.CreateParameter("PTARGETVOLUMEUNITIDLIST", 200, 1, 2000, TargetVolumeUnitIDList) 	

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
