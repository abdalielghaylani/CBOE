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
strError = "Error:CreateReformatMap<BR>"

reformatMapName = Request("iReformatMapName")
reformatMapType = Request("reformatMapType")
sourcePlateFormatIDTemp = Request("sourcePlateFormatID")
targetPlateFormatID = Request("targetPlateFormatID")
position1 = request("Position1")
position2 = request("Position2")
position3 = request("Position3")
position4 = request("Position4")

'-- get the source plate format id and the size of the source plate
arrSourcePlateFormatID = split(sourcePlateFormatIDTemp,":")
sourcePlateFormatID = arrSourcePlateFormatID(0)
sourceSize = cint(arrSourcePlateFormatID(1))

'Response.Write position1 & ":" & position2 & ":" & position3 & ":" & position4
'Response.End

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/CreateReformatMap.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(reformatMapName) then
	strError = strError & "ReformatMapName is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(reformatMapType) then
	strError = strError & "ReformatMapType is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(sourcePlateFormatID) then
	strError = strError & "SourcePlateFormatID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(targetPlateFormatID) then
	strError = strError & "TargetPlateFormatID is a required parameter.<BR>"
	bWriteError = True
End If

Call GetInvConnection()

' get number of rows and columns for the specified source format
SQL = "SELECT row_count, col_count FROM inv_vw_plate_format WHERE plate_format_id = ?"
Set Cmd = GetCommand(Conn, SQL, adCmdText)
Cmd.Parameters.Append Cmd.CreateParameter("PlateFormat", 5, 1, 0, sourcePlateFormatID)
Set RS = Cmd.Execute
sourceRows = cint(RS("row_count"))
sourceCols = cint(RS("col_count"))

' get number of rows and columns for the specified target format
SQL = "SELECT row_count, col_count FROM inv_vw_plate_format WHERE plate_format_id = ?"
Set Cmd = GetCommand(Conn, SQL, adCmdText)
Cmd.Parameters.Append Cmd.CreateParameter("PlateFormat", 5, 1, 0, targetPlateFormatID)
Set RS = Cmd.Execute
targetRows = cint(RS("row_count"))
targetCols = cint(RS("col_count"))

'-- get starting grid_format_id
sqlGridPositionID = "Select min(grid_position_id) AS grid_position_id FROM inv_vw_well_format where plate_format_id_fk = " & targetPlateFormatID
Set rsGridPositionID = Conn.Execute(sqlGridPositionID)
startingGridPositionID = rsGridPositionID("grid_position_id")
rsGridPositionID.Close()
Set rsGridPositiontID = Nothing
gridPositionID = cint(startingGridPositionID)

Set oMap = server.CreateObject("MSXML2.DomDocument.4.0")
'-- create reformat map

'-- create base map 
set doc = oMap.createElement("REFORMAT_MAP")
oMap.documentElement = doc
for i = 1 to 4
	set source = oMap.createElement("SOURCE_PLATE")
	source.setAttribute "PLATENUM",i
	source.setAttribute "ROWS", sourceRows
	source.setAttribute "COLS", sourceCols
	source.setAttribute "PLATE_FORMAT_ID_FK", sourcePlateFormatID
	doc.appendChild(source)
next
set target = oMap.createElement("TARGET_PLATE")
target.setAttribute "PLATENUM", 1
target.setAttribute "ROWS", targetRows
target.setAttribute "COLS", targetCols
target.setAttribute "PLATE_FORMAT_ID_FK", targetPlateFormatID
doc.appendChild(target)
for i = 1 to targetRows
	set row = oMap.createElement("ROW")
	row.setAttribute "ID", i
	for j = 1 to targetCols
		set col = oMap.createElement("COL")
		col.setAttribute "ID", j
		col.setAttribute "GRID_POSITION_ID_FK", gridPositionID
		set source = oMap.createElement("SOURCE")
		source.setAttribute "PLATENUM",j
		source.setAttribute "ROWID", j
		source.setAttribute "COLID", j
		col.appendChild(source)
		row.appendChild(col)
		gridPositionID = gridPositionID + 1
	next
	target.appendChild(row)
next

'Response.Write oMap.xml
'Response.End

'-- create stamped 
if reformatMapType = "1" then
	for i = 1 to targetRows/2
		for j = 1 to targetCols/2
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@PLATENUM")
			currNode.text = 1
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@ROWID")
			currNode.text = i
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@COLID")
			currNode.text = j
			'Response.Write currNode.text & "<BR>"
		next
	next
	for i = 1 to targetRows/2
		for j = (targetCols/2+1) to targetCols
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@PLATENUM")
			currNode.text = 2
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@ROWID")
			currNode.text = i
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@COLID")
			currNode.text = j - targetCols/2
			'Response.Write currNode.text & "<BR>"
		next
	next
	for i = (targetRows/2+1) to targetRows
		for j = 1 to targetCols/2
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@PLATENUM")
			currNode.text = 3
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@ROWID")
			currNode.text = i - targetRows/2
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@COLID")
			currNode.text = j
			'Response.Write currNode.text & "<BR>"
		next
	next
	for i = (targetRows/2+1) to targetRows
		for j = (targetCols/2+1) to targetCols
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@PLATENUM")
			currNode.text = 4
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@ROWID")
			currNode.text = i - targetRows/2
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@COLID")
			currNode.text = j - targetCols/2
			'Response.Write currNode.text & "<BR>"
		next
	next
'-- create interleaved plates
elseif reformatMapType = "2" then 
	size = sourceSize - 1
	'create source plates
	redim arrPlate1(size,2)
	redim arrPlate2(size,2)
	redim arrPlate3(size,2)
	redim arrPlate4(size,2)
	'fill source plates
	col = 1
	for x = 1 to 4
		for i = 0 to size
			if col > sourceCols then col = 1
			execute("arrPlate" & x & "(i,0) = i")
			execute("arrPlate" & x & "(i,1) = (i \ 12) + 1")
			execute("arrPlate" & x & "(i,2) = col")
			col = col + 1
		next 
	next
	n=0
	for i = 1 to targetRows step 2
		for j = 1 to targetCols step 2
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@PLATENUM")
			currNode.text = position1
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@ROWID")
			execute("currNode.text = arrPlate" & position1 & "(n,1)")
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@COLID")
			execute("currNode.text = arrPlate" & position1 & "(n,2)")
			'currNode.text = arrPlate1(n,2)
			'Response.Write currNode.text & "<BR>"
			'Response.Write n & ": i=" & i & " j=" & j & "<BR>"
			'Response.Write arrPlate1(n,0) & ":" & arrPlate1(n,1) & ":" & arrPlate1(n,2) & "<BR>"
			n = n+1
		next
	next
	n = 0
	for i = 1 to targetRows step 2
		for j = 2 to targetCols step 2
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@PLATENUM")
			currNode.text = position2
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@ROWID")
			execute("currNode.text = arrPlate" & position2 & "(n,1)")
			'currNode.text = arrPlate3(n,1)
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@COLID")
			execute("currNode.text = arrPlate" & position2 & "(n,2)")
			'currNode.text = arrPlate3(n,2)
			'Response.Write currNode.text & "<BR>"
			'Response.Write n & ": i=" & i & " j=" & j & "<BR>"
			'Response.Write arrPlate1(n,0) & ":" & arrPlate1(n,1) & ":" & arrPlate1(n,2) & "<BR>"
			n = n+1
		next
	next
	n = 0
	for i = 2 to targetRows step 2
		for j = 1 to targetCols step 2
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@PLATENUM")
			currNode.text = position3
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@ROWID")
			execute("currNode.text = arrPlate" & position3 & "(n,1)")
			'currNode.text = arrPlate2(n,1)
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@COLID")
			execute("currNode.text = arrPlate" & position3 & "(n,2)")
			'currNode.text = arrPlate2(n,2)
			'Response.Write currNode.text & "<BR>"
			'Response.Write n & ": i=" & i & " j=" & j & "<BR>"
			'Response.Write arrPlate1(n,0) & ":" & arrPlate1(n,1) & ":" & arrPlate1(n,2) & "<BR>"
			n = n+1
		next
	next
	n = 0
	for i = 2 to targetRows step 2
		for j = 2 to targetCols step 2
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@PLATENUM")
			currNode.text = position4
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@ROWID")
			execute("currNode.text = arrPlate" & position4 & "(n,1)")
			'currNode.text = arrPlate4(n,1)
			Set currNode = oMap.selectSingleNode("/REFORMAT_MAP/TARGET_PLATE/ROW[@ID='" & i & "']/COL[@ID='" & j & "']/SOURCE/@COLID")
			execute("currNode.text = arrPlate" & position4 & "(n,2)")
			'currNode.text = arrPlate4(n,2)
			'Response.Write currNode.text & "<BR>"
			'Response.Write n & ": i=" & i & " j=" & j & "<BR>"
			'Response.Write arrPlate1(n,0) & ":" & arrPlate1(n,1) & ":" & arrPlate1(n,2) & "<BR>"
			n = n+1
		next
	next
	'httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/CreateDataMap.asp", "ChemInv", FormData)
end if

'Response.Write oMap.xml
'Response.End

Set Cmd = Server.CreateObject("ADODB.Command")
Cmd.ActiveConnection = Conn
Cmd.CommandType = adCmdStoredProc
Cmd.CommandText = Application("CHEMINV_USERNAME") & ".Reformat.InsertReformatMap"

Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",200, adParamReturnValue, 200, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("pMapXML", AdLongVarChar, 1, len(oMap.xml) + 1, oMap.xml)
Cmd.Parameters.Append Cmd.CreateParameter("pName",200, 1, 255, reformatMapName)
Cmd.Properties("SPPrmsLOB") = TRUE
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Reformat.InsertReformatMap")
	returnValue = Cmd.Parameters("RETURN_VALUE")
End if
Cmd.Properties("SPPrmsLOB") = FALSE

Response.Write returnValue
oMap.save("c:\test.xml")
'Response.End

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
