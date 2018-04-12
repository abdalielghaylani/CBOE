<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim httpResponse
Dim ServerName
Dim Credentials
Dim FormData
Dim bDebugPrint

bDebugPrint = false

ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
FormData = Request.Form & Credentials

'ShowFormVars(true)

'Required Paramenters
multiselect = Request("multiselect")
'dataMode = Request("dataMode")
PlateID = Request("PlateID")
SolventID = Request("SolventID")
SolventVolume = Request("SolventVolume")
SolventVolumeUnitID = Request("SolventVolumeUnitID")
Concentration = Request("Concentration")
ConcentrationUnitID = Request("ConcentrationUnitID")
bSolvate = false
XMLDoc_ID = request("XMLDoc_ID")
reformatAction = Request("reformatAction")
numDaughterPlates = Request("numDaughterPlates")
stepCount = Request("stepCount")
stepValue = Request("stepValue")
pageMode = Request("pageMode")
locationID = Request("locationID")

numSourcePlates = Request("numSourcePlates")
PlateIDList = " "
SolventIDList = " "
SolventVolumeList = " "
SolventVolumeUnitIDList = " "
ConcentrationList = " "
ConcentrationUnitIDList = " "
for i = 0 to numSourcePlates-1
	if eval("Request(""solvate" & i & """) = ""yes""") then
		if not bSolvate then bSolvate = true
		execute("PlateIDList = PlateIDList & Request(""PlateID" & i & """) & "",""")						
		execute("SolventIDList = SolventIDList & Request(""SolventID" & i & """) & "",""")						
		execute("SolventVolumeList = SolventVolumeList & Request(""SolventVolumeAdded" & i & """) & "",""")						
		execute("SolventVolumeUnitIDList = SolventVolumeUnitIDList & Request(""SolventVolumeAddedUnitID" & i & """) & "",""")						
		execute("ConcentrationList = ConcentrationList & Request(""Concentration" & i & """) & "",""")						
		execute("ConcentrationUnitIDList = ConcentrationUnitIDList & Request(""ConcentrationUnitID" & i & """) & "",""")						
	end if
next
PlateIDList = trim(left(PlateIDList,len(PlateIDList)-1))
SolventIDList = trim(left(SolventIDList,len(SolventIDList)-1))
SolventVolumeList = trim(left(SolventVolumeList,len(SolventVolumeList)-1))
SolventVolumeUnitIDList = trim(left(SolventVolumeUnitIDList,len(SolventVolumeUnitIDList)-1))
ConcentrationList = trim(left(ConcentrationList,len(ConcentrationList)-1))
ConcentrationUnitIDList = trim(left(ConcentrationUnitIDList,len(ConcentrationUnitIDList)-1))

QSData = "PlateIDList=" & PlateIDList
QSData = QSData & "&SolventIDList=" & SolventIDList
QSData = QSData & "&SolventVolumeList=" & SolventVolumeList
QSData = QSData & "&SolventVolumeUnitIDList=" & SolventVolumeUnitIDList
QSData = QSData & "&ConcentrationList=" & server.URLEncode(ConcentrationList)
QSData = QSData & "&ConcentrationUnitIDList=" & ConcentrationUnitIDList
QueryString = QSData & Credentials

'Response.Write ConcentrationList
'Response.Write querystring
'testList = ConcentrationList & "," & ConcentrationList
'test = CleanseSciNotation(testList)
'Response.Write testList & "=testList<BR>"
'Response.Write test & "=test"
'Response.End
'Response.Write Request.Form & "<BR>"
'Response.Write QueryString
'Response.End
if bSolvate then
	httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/SolvatePlate.asp", "ChemInv", QueryString)
	out = trim(httpResponse)
else
	out = 1
end if
'Response.Write out
'Response.End
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Solvate a Plate</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
	window.focus();
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff width=90%>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE align=center>
<%
			If isNumeric(out) then
				if Clng(out) > 0 then
					containerList = out
					if pageMode = "reformat" then
						theAction = "GotoDaughterPlate"
					else
						theAction = "Exit"
					end if
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
				Case "GotoDaughterPlate"
					QueryString = "multiSelect=" & multiSelect
					QueryString = QueryString & "&dataMode=" & dataMode
					QueryString = QueryString & "&XMLDoc_ID=" & XMLDoc_ID
					QueryString = QueryString & "&PlateID=" & PlateID
					QueryString = QueryString & "&reformatAction=" & reformatAction
					QueryString = QueryString & "&numDaughterPlates=" & numDaughterPlates
					QueryString = QueryString & "&numSourcePlates=" & numSourcePlates
					QueryString = QueryString & "&stepCount=" & stepCount
					QueryString = QueryString & "&stepValue=" & stepValue
					QueryString = QueryString & "&" & QSData
					Response.write "<Script language=javascript>window.location = 'ReformatPlate_TargetCriteria.asp?" & QueryString & "';</script>"		
				Case "Exit"
					Session("bMultiSelect") = false
					plate_multiSelect_dict.RemoveAll()
					plateID = iif(isEmpty(Session("plPlate_ID")),"0",Session("plPlate_ID"))
					Response.Write "<center><SPAN class=""GuiFeedback"">Plates diluted.</SPAN></center>"
					'Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNode(0, " & Session("CurrentLocationID") & ", 0, '" & Session("TreeViewOpenNodes1") & "',0,1); opener.focus(); window.close();</SCRIPT>" 
					'Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNode(0, " & locationID & ", 0, '" & Session("TreeViewOpenNodes1") & "',0,1); opener.focus(); window.close();</SCRIPT>" 
					Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNode(0, " & locationID & ", 0, '" & Session("TreeViewOpenNodes1") & "'," & plateID & ",1); opener.focus(); window.close();</SCRIPT>" 
					'Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNode(0, " & locationID & ", 0, '" & Session("TreeViewOpenNodes1") & "',0,1);</SCRIPT>" 
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
