<%@ Language=VBScript%>
<%session.Abandon%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/xml_utils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/compound_utils_vbs.asp"-->
<%
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim bDebugPrint
Dim Structure
Dim CAS
Dim ACX_ID
Dim SubstanceName
Dim ALT_ID_1
Dim ALT_ID_2
Dim ALT_ID_3
Dim ALT_ID_4
Dim ALT_ID_5
Dim dupMolID
Dim dupID
Dim bDuplicateStructure
Dim bSubstanceCreated
Dim bDuplicatesFound
Dim bIsEdit

bDuplicatesFound = false
bDebugPrint = FALSE
bWriteError = False
strError = "Error:CreateSubstance<BR>"

'DumpRequest()

ResolveConflictsLater = Request("ResolveConflictsLater")
bResolveConflictsLater = false
If ResolveConflictsLater = "1" then bResolveConflictsLater = true	
RegisterIfConflicts = Request("RegisterIfConflicts")
bRegisterIfConflicts = false
if RegisterIfConflicts = "true" then bRegisterIfConflicts = true

action = Request("action")
bIstest = false
If action = "test" then bIstest = true

EditCompoundID = Null
SubstanceName = Request("inv_compounds.Substance_Name")
Structure = Request("inv_compounds.Structure")
CAS = Request("inv_compounds.CAS")
ACX_ID = Request("inv_compounds.ACX_ID")
Density = Request("inv_compounds.Density")
if Density = "" then Density = 1
cLogP = Request("inv_compounds.CLOGP")
if cLogP = "" then cLogP = null
Rotatable_Bonds = Request("inv_compounds.Rotatable_Bonds")
if Rotatable_Bonds = "" then Rotatable_Bonds = null
Tot_Pol_Surf_Area = Request("inv_compounds.Tot_Pol_Surf_Area")
if Tot_Pol_Surf_Area = "" then Tot_Pol_Surf_Area = null
HBond_Acceptors = Request("inv_compounds.HBond_Acceptors")
if HBond_Acceptors = "" then HBond_Acceptors = null
HBond_Donors = Request("inv_compounds.HBond_Donors")
if HBond_Donors = "" then HBond_Donors = null
'Response.Write Density & ":" & cLogP & ":" & Rotatable_Bonds & ":" & Tot_Pol_Surf_Area & ":" & HBond_Acceptors & ":" & HBond_Donors & "<BR>"
'Response.End
ALT_ID_1 = Request("inv_Compounds.ALT_ID_1")
ALT_ID_2 = Request("inv_Compounds.ALT_ID_2")
ALT_ID_3 = Request("inv_Compounds.ALT_ID_3")
ALT_ID_4 = Request("inv_Compounds.ALT_ID_4")
ALT_ID_5 = Request("inv_Compounds.ALT_ID_5") 

if Density = "" or isEmpty(Density) then Density = 1

For each key in unique_alt_ids_dict
	cKey = Replace(Key,"inv_compounds.","")
	uniqueAltIDList =  uniqueAltIDList & cKey & ","
Next
if Len(uniqueAltIDList) > 0 then uniqueAltIDList = left(uniqueAltIDList, len(uniqueAltIDList)-1)

CompoundID = 0

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/api/help/CreateSubstance.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(SubstanceName) OR SubstanceName = "" then
	strError = strError & "SubstanceName is a required parameter<BR>"
	Response.Write strError
	Response.End
End if
	
conflictingFields = "&inv_compounds.Conflicting_Fields="
%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/createsubstanceOraSP.asp"-->
<%

if oIsExistingCompound then 
	' Compound determined to alredy exist
	Response.Clear
	Response.ContentType = "text/xml"
	response.write "<EXISTINGSUBSTANCE CompoundID=""" & oCompoundID & """/>"	
	Response.end
	' EXIT POINT <=======
End if	

If ((Not bDuplicatesFound) OR (bResolveConflictsLater) or (bRegisterIfConflicts)) AND (NOT bIsTest)  then
	'A compound should have been created
	If oCompoundID > 0 then
		cID = oCompoundID
		if bDuplicatesFound AND bResolveConflictsLater then
			xmlText = "DUPLICATESUBSTANCE"
		else
			xmlText = "NEWSUBSTANCE"
		end if
		Response.Clear
		Response.ContentType = "text/xml"
		response.write "<" & xmlText & " CompoundID=""" & cID &  """/>"	
		' Increment substance count
		If isEmpty(Application("inv_CompoundsRecordCountChemInv")) then
			Application.Lock
				Application("inv_Compounds" & "RecordCount" & "ChemInv") = GetSubstanceCount()
			Application.UnLock
		end if
		substanceCount = CLng(Application("inv_Compounds" & "RecordCount" & "ChemInv")) + 1
		Application.Lock
			Application("inv_Compounds" & "RecordCount" & "ChemInv") = substanceCount
		Application.UnLock
		Response.end
	Else
		' Something went wrong with compound insert
		strError = strError & "Error Adding Compound<BR>"
		Response.Write strError
		Response.End
	End if
Else
	 
	if bDuplicatesFound and not((bResolveConflictsLater) or (bRegisterIfConflicts)) then
		'A compound was not created because of duplicates found
		
		dim xmlDoc
		set xmlDoc = Server.CreateObject("Msxml2.DOMDocument")
		Set RootElm = xmlDoc.createElement("CONFLICTINGSUBSTANCES")
		xmlDoc.appendChild(RootElm)
		
		For each Key in Duplicates_dict
			theCompoundID = Key
			theConflict_arr = Split(Duplicates_dict.Item(Key),",") 
			For i = 0 to Ubound(theConflict_arr)
				if key <> "0" then Call CreateNode(xmlDoc, "CONFLICT", RootElm,theConflict_arr(i), "CompoundID", theCompoundID)
				'Response.Write "CONFLICT" & theConflict_arr(i) & "CompoundID" & theCompoundID & "::"
			Next
		Next	
		Response.Clear
		Response.ContentType = "text/xml"
		response.write xmlDoc.xml
		Response.end
	Else
		'A compound was not created because this was a test run	
		Response.Clear
		Response.ContentType = "text/xml"
		response.write "<NEWSUBSTANCE CompoundID=""""/>"	
		Response.end
	End if
End if

Function GetSubstanceCount()
	dim RS

	GetInvConnection()
	sql = "SELECT count(*) as count from inv_compounds"
	Set RS = Conn.Execute(sql)
	theReturn = RS("count")
	Conn.Close
	Set Conn = nothing
	Set RS = nothing
	GetSubstanceCount = theReturn
end function

%>
	
