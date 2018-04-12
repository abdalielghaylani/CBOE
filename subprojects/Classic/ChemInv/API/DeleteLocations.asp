<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim LocationList
Dim LocationSQL
Dim CountChildrenSQL
Dim LocInList
Dim LocRangeSQL
Dim tempArr
Dim RangeArr
Dim i
Dim DeletePathSQL
Dim DeleteLocationsSQL
Dim Conn
Dim RS
Dim ConStr
Dim strError
Dim bWriteError
Dim PrintDebug

bDebugPrint = False
bWriteError = False
strError = "Error:DeleteLocations<BR>"
LocationList = Request("LocationList")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/DeleteLocations.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(LocationList) then
	strError = strError & "LocationList is a required parameter<BR>"
	bWriteError = True
Else
	' Parse LocationList
	tempArr = Split(LocationList,",")
	LocInList = " Location_ID IN (" 
	For i = 0 to Ubound(tempArr)
		' Check for ranges
		RangeArr = Split(tempArr(i),"-")
		if Ubound(RangeArr) > 0 then
			if CLng(RangeArr(0)) < CLng(RangeArr(1)) then
				Low = RangeArr(0)
				high = RangeArr(1)
			Else
				Low = RangeArr(1)
				High = RangeArr(0)
			End if
			LocRangeSQL = LocRangeSQL & " OR (Location_ID >= " & Low & " AND Location_ID <= " & High & ")"
		Else
			LocInList = LocInList & tempArr(i) & ","
		End if
		Next
		If InStr(LocInList, ",") then 
			LocInList = Left(LocInList, Len(LocInList)-1) & ")"
		Else
			' There is no IN clause, so remove the leading OR from LocRangeSQL
			LocInList = ""
			LocRangeSQL = Right(LocRangeSQL, Len(LocRangeSQL)-3)
		End if
		LocationSQL = LocInList & LocRangeSQL
End if

if NOT bWriteError then
	Call GetInvConnection()

	' Locations must be empty before deleting
	CountChildrenSQL = "SELECT Count(Container_ID)AS ContainerCount FROM inv_containers WHERE " & LocationSQL 
	if bDebugPrint then
		Response.Write CountChildrenSQL & "<BR>"
	Else
		Set RS = Conn.Execute(CountChildrenSQL)
		if CLng(RS("ContainerCount")) > 0 then
			strError = strError & "At least one location in the location list is not empty"
			bWriteError = True
		End if
		RS.Close
		Set RS= Nothing
	End if
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

' Delete from Locations
DeleteLocationsSQL= "DELETE FROM inv_locations WHERE" & LocationSQL
if bDebugPrint then
	Response.Write DeleteLocationsSQL & "<BR>"
Else
	Conn.Execute DeleteLocationsSQL, lRecsAffected
End if

Conn.Close
Set Conn = Nothing

' Return success	
Response.Write lRecsAffected
</SCRIPT>
