<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim strError
Dim bWriteError
Dim bDebugPrint
Dim bXMLDebug

bDebugPrint = False
bXMLDebug = True
bWriteError = False
strError = "Error:CreateSubstanceXML<BR>"
InvServerName = Application("InvServerName")

Set oCompoundXML = Server.CreateObject("MSXML2.DOMDocument.4.0")
oCompoundXML.load(request)
If bXMLDebug then oCompoundXML.save(Server.MapPath("/" & Application("AppKey") & "/config/xml_Templates/debug01_CreateSubstanceXML.xml")) 
'If bXMLDebug then oCompoundXML.save(Server.MapPath("/" & Application("AppKey") & "/config/xml_Templates/debug" & replace(replace(time,":","")," ","") & "_CreatePlateXML.xml"))
'Response.End

' Redirect to help page if no xmldoc is passed
If Len(oCompoundXML.xml) = 0 then
	Response.Redirect "/cheminv/help/admin/api/CreateSubstanceXML.htm"
	Response.end
End if

'get api variables from the xmldoc
Set oCompoundsElement = oCompoundXML.documentElement
CSUserName = oCompoundsElement.getAttribute("CSUSERNAME")
CSUserID = oCompoundsElement.getAttribute("CSUSERID")

' Check for required parameters
If IsEmpty(CSUserName) or IsNull(CSUserName) then
	strError = strError & "CSUserName is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(CSUserID) or IsNull(CSUserID) then
	strError = strError & "CSUserID is a required parameter<BR>"
	bWriteError = True
End if

If bWriteError then
	Response.Write strError
	Response.End
end if

'Register compounds or add to inv_compounds
oCompoundXML.setProperty "SelectionLanguage","XPath"
Set nlCompounds = oCompoundXML.selectNodes("//COMPOUND")
'if bDebugPrint then Response.Write nlCompounds.length & "=NumCompounds<BR>"

Set oResultXML = server.CreateObject("MSXML2.DOMDocument")
Set oRootNode = oResultXML.createElement("CREATE_SUBSTANCE_RESULTS")

Set oCmpdResultXML = server.CreateObject("MSXML2.DOMDocument")
	Credentials = "&CSUserName=" & CSUserName & "&CSUSerID=" & CSUSerID
	For Each oNode In nlCompounds
		Set oParentNode = oNode.parentNode
		SubstanceName = oNode.GetAttribute("SUBSTANCE_NAME")
		Structure = oNode.GetAttribute("BASE64_CDX")
		CAS = oNode.GetAttribute("CAS")
		ACX_ID = oNode.GetAttribute("ACX_ID")
		ALT_ID_1 = oNode.GetAttribute("ALT_ID_1")
		ALT_ID_2 = oNode.GetAttribute("ALT_ID_2")
		ALT_ID_3 = oNode.GetAttribute("ALT_ID_3")
		ALT_ID_4 = oNode.GetAttribute("ALT_ID_4")
		ALT_ID_5 = oNode.GetAttribute("ALT_ID_5")

		QueryString = "inv_compounds.Substance_Name=" & SubstanceName
		QueryString = QueryString & "&RegisterIfConflicts=true"
		QueryString = QueryString & "&ResolveConflictsLater=1"
		QueryString = QueryString & "&inv_compounds.CAS=" & CAS
		QueryString = QueryString & "&inv_compounds.ACX_ID=" & ACX_ID
		QueryString = QueryString & "&inv_compounds.ALT_ID_1=" & ALT_ID_1
		QueryString = QueryString & "&inv_compounds.ALT_ID_2=" & ALT_ID_2
		QueryString = QueryString & "&inv_compounds.ALT_ID_3=" & ALT_ID_3
		QueryString = QueryString & "&inv_compounds.ALT_ID_4=" & ALT_ID_4
		QueryString = QueryString & "&inv_compounds.ALT_ID_5=" & ALT_ID_5
		'QueryString = QueryString & "&inv_compounds.Structure=" & Server.URLEncode(Structure)
		QueryString = QueryString & "&inv_compounds.Structure=" & Structure
		QueryString = QueryString & Credentials
		logaction(QueryString)
		if bDebugPrint then 
			Response.Write QueryString & "<BR>"
			Response.End
		end if
		httpResponse = CShttpRequest2("POST", InvServerName, "/cheminv/api/CreateSubstance.asp", "ChemInv", QueryString)
		out2 = httpResponse
		logaction(out2)
		if bDebugPrint then Response.Write out2 & "=CreateSubstance Return Value<BR>" 
		oCmpdResultXML.loadXML(out2)
		oCmpdResultXML.setProperty "SelectionLanguage","XPath"
		'substance already exists
		Set nlExistingSubstances = oCmpdResultXML.selectNodes("//EXISTINGSUBSTANCE")
		if nlExistingSubstances.Length > 0 then Set nlToCheck = nlExistingSubstances
		'substance created
		Set nlNewSubstances = oCmpdResultXML.selectNodes("//NEWSUBSTANCE")
		if nlNewSubstances.Length > 0 then Set nlToCheck = nlNewSubstances
		'duplicate substance
		Set nlDuplicateSubstances = oCmpdResultXML.selectNodes("//DUPLICATESUBSTANCE")
		if nlDuplicateSubstances.Length > 0 then Set nlToCheck = nlDuplicateSubstances
		'conflicting substances
		Set nlConflictingSubstances = oCmpdResultXML.selectNodes("//CONFLICTINGSUBSTANCES")
		if nlConflictingSubstances.Length > 0 then Set nlToCheck = oCmpdResultXML.selectNodes("//CONFLICTINGSUBSTANCES/CONFLICT")
		'RCNode means Registered Compound Node
		if isObject(nlToCheck) then
			For Each oRCNode in nlToCheck
				oRootNode.appendChild(oRCNode)				
				CompoundID = oRCNode.GetAttribute("CompoundID")
			Next
		end if
		Set oCompoundIDNode = oCompoundXML.createAttribute("COMPOUND_ID_FK")
		oCompoundIDNode.Value = CompoundID
		oParentNode.SetAttributeNode(oCompoundIDNode)
	Next



If bXMLDebug then oCompoundXML.save(Server.MapPath("/" & Application("AppKey") & "/config/xml_Templates/debug02_CreatePlateXML.xml")) 
'oRootNode.appendChild(oSubstanceResultsNode)
oResultXML.appendChild(oRootNode)
'Response.Write trim(Cmd.Parameters("RETURN_VALUE"))
'Return an XML doc with the substance registration info
'Response.Write Cmd.Parameters("RETURN_VALUE")
Response.Write oResultXML.xml

'Clean up
Set oCompoundXML = nothing
Set oResultXML = nothing
set oCmpdResultXML = nothing
Set nlCompounds = nothing
Set nlExistingSubstances = nothing
Set nlNewSubstances = nothing
Set nlDuplicateSubstances = nothing
Set nlToCheck = nothing

Response.End


Sub SetSolventID(solventName)

End Sub


'-------------------------------------------------------------------------------
' Name: LogAction(inputstr)
' Type: Sub
' Purpose:  writes imformation to a output file 
' Inputs:   inputstr  as string - variable to output
' Returns:	none
' Comments: writes informtion to /inetput/cfwlog.txt file
'-------------------------------------------------------------------------------
Sub LogAction(ByVal inputstr)
		on error resume next
		filepath = Application("ServerDrive") & "\" & Application("ServerRoot") & "\cfwlog.txt"
		Set fs = Server.CreateObject("Scripting.FileSystemObject")
		Set a = fs.OpenTextFile(filepath, 8, True)  
		a.WriteLine Now & ": " & inputstr
		a.WriteLine " "
		a.close
End Sub



</SCRIPT>
