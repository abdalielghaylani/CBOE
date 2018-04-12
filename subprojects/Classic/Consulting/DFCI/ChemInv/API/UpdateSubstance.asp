<%@ Language=VBScript%>
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

bDebugPrint = false
bWriteError = false
strError = "Error:UpdateSubstance<BR>"

ResolveConflictsLater = Request("ResolveConflictsLater")
bResolveConflictsLater = false
If ResolveConflictsLater = "1" then bResolveConflictsLater = true

EditCompoundID = Request("CompoundID")
SubstanceName = Request("SubstanceName")
if substanceName = "" then SubstanceName = Request("inv_compounds.Substance_Name")
Structure = Request("Structure")
if structure = ""  then Structure = Request("inv_compounds.structure")
CAS = Request("CAS")
ACX_ID = Request("ACX_ID")
ALT_ID_1 = Request("ALT_ID_1")
ALT_ID_2 = Request("ALT_ID_2")
ALT_ID_3 = Request("ALT_ID_3")
ALT_ID_4 = Request("ALT_ID_4")
ALT_ID_5 = Request("ALT_ID_5") 

For each key in unique_alt_ids_dict
	cKey = Replace(Key,"inv_compounds.","")
	uniqueAltIDList =  uniqueAltIDList & cKey & ","
Next
if Len(uniqueAltIDList) > 0 then uniqueAltIDList = left(uniqueAltIDList, len(uniqueAltIDList)-1)


' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/UpdateSubstance.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(EditCompoundID) then
	strError = strError & "CompoundID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(SubstanceName) then
	strError = strError & "SubstanceName is a required parameter<BR>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if
%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/createsubstanceOraSP.asp"-->	
<%
'Response.end
if oIsExistingCompound then 
	' Compound determined to alredy exist
	Response.Clear
	Response.ContentType = "text/xml"
	response.write "<EXISTINGSUBSTANCE CompoundID=""" & oCompoundID & """/>"	
	Response.end
	' EXIT POINT <=======
End if	

If ((Not bDuplicatesFound) OR (bResolveConflictsLater)) AND (NOT bIsTest) then
	'A compound should have been updated
		cID = oCompoundID
		if bDuplicatesFound AND bResolveConflictsLater then
			xmlText = "DUPLICATESUBSTANCE"
		else
			xmlText = "EXISITNGSUBSTANCE"
		end if
		Response.Clear
		Response.ContentType = "text/xml"
		response.write "<" & xmlText & " CompoundID=""" & cID &  """/>"	
		Response.end
Else
	if bDuplicatesFound then
		'A compound was not updated because of duplicates found
		
		dim xmlDoc
		set xmlDoc = Server.CreateObject("Msxml2.DOMDocument")
		Set RootElm = xmlDoc.createElement("CONFLICTINGSUBSTANCES")
		xmlDoc.appendChild(RootElm)
		
		For each Key in Duplicates_dict
			theCompoundID = Key
			theConflict_arr = Split(Duplicates_dict.Item(Key),",") 
			For i = 0 to Ubound(theConflict_arr)
				if key <> "0" then Call CreateNode(xmlDoc, "CONFLICT", RootElm,theConflict_arr(i), "CompoundID", theCompoundID)
			Next
		Next	
		Response.Clear
		Response.ContentType = "text/xml"
		response.write xmlDoc.xml
		Response.end
	Else
		'A compound was not updated because this was a test run	
		Response.Clear
		Response.ContentType = "text/xml"
		response.write "<EXISTINGSUBSTANCE CompoundID=""""/>"	
		Response.end
	End if
End if
%>