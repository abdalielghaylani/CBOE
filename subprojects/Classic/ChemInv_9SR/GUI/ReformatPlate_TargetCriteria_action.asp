<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->

<html>
<head>
<title><%=Application("appTitle")%> -- Reformat Plates</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
	window.focus();
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<%
Response.Write "<DIV ALIGN=""center""><img src=""/cfserverasp/source/graphics/processing_Ybvl_Ysh_grey.gif"" WIDTH=""130"" HEIGHT=""100"" BORDER=""""></DIV>"
Response.Flush

Dim httpResponse
Dim ServerName
Dim Credentials
Dim FormData
Dim bDebugPrint
Dim Conn
Dim Cmd

bDebugPrint = false

ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
FormData = Request.Form & Credentials

'Required Parameters
multiSelect = Request("multiSelect")
dataMode = Request("dataMode")
numSourcePlates = Request("numSourcePlates")
numTargetPlates = Request("numTargetPlates")
numTotalPlates = request("numTotalPlates")
SourcePlateIDList = Request("PlateID")
XMLDoc_ID = Request("XMLDoc_ID")
'AmtType = Request("AmtType")
ReformatAction = Request("ReformatAction")

'optional
DrySourcePlates = Request("DrySourcePlates")

'if AmtType = "SourceVolumeTaken" then
'	dataField = "SourceVolTaken"
'	unitIDField = "SourceVolTakenUnitID"
'elseif AmtType = "SourceAmountTaken" then
'	dataField = "SourceAmtTaken"
'	unitIDField = "SourceAmountUnitID"
'end if

'build barcode list
AutoGen = Request("AutoGen_r")
arrSourcePlateIDs = plate_multiSelect_dict.Keys

barcodeStart = Request("Barcode_Start")
barcodePrefix = Request("Barcode_Prefix")
'-- convert the barcode start to an int
if AutoGen = "2" then
	if isEmpty(barcodeStart) or barcodeStart="" then 
		barcodeStart = 0
	else
		barcodeStart = clng(barcodeStart)
	end if
end if
for i = 0 to (numTotalPlates) -1
	'manually enter 
	if AutoGen = "0" then
		execute("BarcodeList = BarcodeList & Request(""Plate_Barcode" & i & """) & "",""")
	'use barcode desc
	elseif AutoGen = "1" then
		execute("BarcodeList = BarcodeList & Request(""BarcodeDescID0"") & "":auto,""")
	'barcode starts with
	elseif AutoGen = "2" then
		execute("BarcodeList = BarcodeList & barcodePrefix & (barcodeStart+i) & "",""")
	end if
next

BarcodeList = trim(left(BarcodeList,len(BarcodeList)-1))

'Response.Write Request.Form
'Response.End

PlateTypeID = Request("PlateTypeID")
Amt = Request("SourceAmtTaken")
AmtUnitID = Request("SourceAmtTakenUnitID")
SolventID = Request("SolventID")
SolventVolume = Request("SolventVolume")
SolventVolumeUnitID = Request("SolventVolumeUnitID")
LocationID = Request("LocationID")

vars = ""
vars = vars &  SourcePlateIDList & "=SourcePlateIDList<BR>"
vars = vars &  XMLDoc_ID & "=XMLDoc_ID<BR>"
vars = vars &  BarcodeList & "=BarcodeList<BR>"
vars = vars &  LocationIDList & "=LocationIDList<BR>"
vars = vars &  PlateTypeIDList & "=PlateTypeIDList<BR>"
vars = vars &  AmtList & "=AmtList<BR>"
vars = vars &  AmtUnitIDList & "=AmtUnitIDList<BR>"
'vars = vars &  AmtType & "=AmtType<BR>"
vars = vars &  SolventIDList & "=SolventIDList<BR>"
vars = vars &  SolventVolumeList & "=SolventVolumeList<BR>"
vars = vars &  SolventVolumeUnitIDList & "=SolventVolumeUnitIDList<BR>"
vars = vars &  NumTargetPlates & "=NumTargetPlates<BR>"
'Response.Write vars
'Response.End

QueryString = "SourcePlateIDList=" & SourcePlateIDList
QueryString = QueryString & "&xmldoc_id=" & XMLDoc_ID 
QueryString = QueryString & "&BarcodeList=" & BarcodeList
QueryString = QueryString & "&LocationID=" & LocationID
QueryString = QueryString & "&PlateTypeID=" & PlateTypeID
QueryString = QueryString & "&Amt=" & Amt
QueryString = QueryString & "&AmtUnitID=" & AmtUnitID
QueryString = QueryString & "&SolventID=" & SolventID
QueryString = QueryString & "&SolventVolume=" & SolventVolume
QueryString = QueryString & "&SolventVolumeUnitID=" & SolventVolumeUnitID
QueryString = QueryString & "&NumTargetPlates=" & NumTargetPlates
QueryString = QueryString & Credentials
'Response.Write QueryString
'Response.End
'-- 10 minute receive timeout, 1 minute send timeout
sendTimeout = 60 *1000
receiveTimeout = 10 * 60 * 1000
httpResponse = CShttpRequest3("POST", ServerName, "/cheminv/api/ReformatPlate.asp", "ChemInv", QueryString, sendTimeout, receiveTimeout)
out = trim(httpResponse)

'Response.Write out
'Response.End
arrOut = split(out, ",")

'dry plates if necessary
if DrySourcePlates = "true" then
	' Set up an ADO command
	Call GetInvCommand(Application("CHEMINV_USERNAME") & ".REFORMAT.DRYPLATE", adCmdStoredProc)

	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 200, adParamReturnValue, 4000, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PPLATEIDLIST", 200, 1, 2000, SourcePlateIDList) 	

	if bDebugPrint then
		For each p in Cmd.Parameters
			Response.Write p.name & " = " & p.value & "<BR>"
		Next
		Response.End	
	Else
		Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".REFORMAT.DRYPLATE")
	End if

	out2 = Cstr(Cmd.Parameters("RETURN_VALUE"))
	Conn.Close

	Set Cmd = Nothing
	Set Conn = Nothing

end if
'Response.Write out
'Response.End
'Response.Write out2
'Response.End


'increment plate count
currCount = Application("inv_platesRecordCount" & Application("appkey"))
Application.Lock
	Application("inv_platesRecordCount" & Application("appkey")) = cInt(currCount) + cInt(NumTargetPlates)
Application.UnLock

%>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff width=90%>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE align=center>
<%
			If isNumeric(arrOut(0)) then
				if Clng(arrOut(0)) > 0 then
					containerList = out
					theAction = "Exit"
				Else
					theAction = "WriteAPIError"
				End if
			Elseif InStr(Left(out,18),"|") then
				containerList = out
				out = left(out,InStr(out,"|")-1)
				theAction = "SelectContainer"	
			Else
				theAction = "WriteOtherError"
			End if
			'Response.Write("<br>action:"&theAction)
			Select Case theAction
				Case "Exit"
					Session("bMultiSelect") = false
					plate_multiSelect_dict.RemoveAll()
					Response.Write "<center><SPAN class=""GuiFeedback"">Reformatting complete.</SPAN></center>"
					Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNode(0, " & Session("CurrentLocationID") & ", 0, '" & Session("TreeViewOpenNodes1") & "'," & arrOut(0) & ",1); opener.focus(); window.close();</SCRIPT>" 
				Case "WriteAPIError"
					Response.Write "<P><CODE>ChemInv API Error: " & Application(out) & "</CODE></P>"
					Response.Write "<SPAN class=""GuiFeedback""><center>Source Plates could not be solvated.</center></SPAN>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				Case "WriteOtherError"
					Response.Write "<P><CODE>Oracle Error: " & out & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.end
			End Select
%>
		</TD>
	</TR>
</TABLE>
</Body>			
