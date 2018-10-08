<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<html>
<head>

<title><%=Application("appTitle")%> -- Create or Edit an Inventory Substance</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="CalculateFromPlugin.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
	var cd_plugin_threshold= <%=Application("CD_PLUGIN_THRESHOLD")%>;
	focus();
</script>
<script language="JavaScript" src="/cfserverasp/source/chemdraw.js"></script>
<script>cd_includeWrapperFile("/cfserverasp/source/")</script>
<script LANGUAGE="javascript">

	function BackOrClose(){
		if (history.length > 0){
			history.back()
		}
		else{
			opener.close();
			window.close();
		}
		
	}

</script>
</head>
<body>
<form name="form1" action="POST">
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/csdo_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/compound_utils_vbs.asp"-->
<%

' DGB force the use of plugin when coming from ELN
Session("isCDP") = "TRUE"

Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug
Dim Structure
Dim CAS
Dim ACX_ID
Dim SubstanceName
Dim dupMolID
Dim dupID
Dim bDuplicateStructure
Dim bSubstanceCreated
Dim bDuplicatesFound
Dim bIsEdit
Dim bSameWindow

'BD 5/07/07 added variable so that when DisplaySubstance function of compound_util_vbs.asp is called we know which asp page its called from. 
Dim isCreateSubstance3
isCreateSubstance3= true


bSameWindow = false
bDuplicatesFound = false
bIsEdit = false
bDebugPrint = false
bWriteError = False


strError = "Error:CreateSubstance2<BR>"
action = Request("action")
CompoundID = Request("CompoundID")
ResolveConflictsLater = Request("ResolveConflictsLater")

bResolveConflictsLater = false
bShowConflicts = false
If ResolveConflictsLater = "1" then bResolveConflictsLater = true

if action = "showconflicts" then session("GUIReturnURL") = ""
ManageMode = Request("ManageMode")
if ManageMode = "1" then 
	Session("GUIReturnURL") = "/cheminv/inputtoggle.asp?formgroup=substances_form_group&dbname=cheminv&GotoCurrentLocation=true"
Else
	Session("GUIReturnURL") = ""
End if

EditCompoundID = Null
RegisterIfConflicts = Request("RegisterIfConflicts")
if RegisterIfConflicts = "" then RegisterIfConflicts = "false"
SubstanceName = Request("inv_compounds.Substance_Name")
Structure = Request("inv_compounds.Structure")
CAS = Request("inv_compounds.CAS")
ACX_ID = Request("inv_compounds.ACX_ID")
Density = Request("inv_compounds.Density")
if Density = "" then Density = 1
ALT_ID_1 = Request("inv_Compounds.ALT_ID_1")
ALT_ID_2 = Request("inv_Compounds.ALT_ID_2")
ALT_ID_3 = Request("inv_Compounds.ALT_ID_3")
ALT_ID_4 = Request("inv_Compounds.ALT_ID_4")
ALT_ID_5 = Request("inv_Compounds.ALT_ID_5") 

For each key in unique_alt_ids_dict
	cKey = Replace(Key,"inv_compounds.","")
	uniqueAltIDList =  uniqueAltIDList & cKey & ","
Next
if Len(uniqueAltIDList) > 0 then uniqueAltIDList = left(uniqueAltIDList, len(uniqueAltIDList)-1)

Select Case lcase(action)
	Case "edit"
		CompoundID = Clng(CompoundID)
		EditCompoundID = CompoundID
		bIsEdit = true
		bShowSelect = false
		bShowCreateDuplicate = false
		bShowEdit = false
		bShowEditExisting = true
		bShowConflicts = false	
	Case "showconflicts"
		CompoundID = Clng(CompoundID)
		GetSubstanceAttributesFromDb(CompoundID)
		bShowConflicts = true
		dBCompoundID = CompoundID
		Structure = dbStructure
		
		SubstanceName = dbSubstanceName
		RegisterIfConflicts = dbRegisterIfConflicts
		CAS = dBCAS 
		ACX_ID = dBACX_ID 
		Density = dBDensity
		ALT_ID_1 = dBALT_ID_1 
		ALT_ID_2 = dBALT_ID_2
		ALT_ID_3 = dBALT_ID_3
		ALT_ID_4 = dBALT_ID_4
		ALT_ID_5 = dBALT_ID_5
		bShowSelect = false
		bShowCreateDuplicate = false
		bShowEdit = false
		bShowEditExisting = true
		bShowConflicts = false
	Case Else
		CompoundID = 0
		bShowSelect = false
		bShowCreateDuplicate = false
		bShowEdit = false
		bShowEditExisting = true
		bShowConflicts = false		
end Select


TempCdxPath = Application("AppTempDirPathHTTP")& "/" & Application("appkey")& "Temp/"
inLineCdx = TempCdxPath & "nostructure.cdx"
inLineMarker = "data:chemical/x-cdx;base64,"

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/api/help/CreateSubstance2.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
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
If ((Not bDuplicatesFound) OR (bResolveConflictsLater)) AND (NOT bShowConflicts) then
	If bIsEdit then
			if bResolveConflictsLater then
				headerTxt = "Duplicate Substance has been Updated"
			else
				headerTxt = "Existing Substance has been Updated"
			end if
			bSubstanceCreated = True
			GetSubstanceAttributesFromDb(CompoundID)
			DisplaySubstance "", headerTxt, bShowSelect, bShowCreateDuplicate, false, false, bShowConflicts, inLineMarker & dBStructure
			Response.Write("<br /><br /><div align=""center""><a href=""#"" onclick=""opener.close(); window.close();""><img src=""../graphics/close_dialog_btn.gif"" border=""0"" alt=""Close"" width=""61"" height=""21""></a></div>")
	Else 'Add record	
		if oCompoundID = 0 AND oIsDuplicateCompound then	
			cID = NewCompoundID 
			headerTxt = "Duplicate Structure Found"
			thExisting = "Existing Substance"
			bSubstanceCreated = false
			bDuplicateStructure = true
		Elseif oCompoundID > 0 then
			cID = oCompoundID
			if bDuplicatesFound AND bResolveConflictsLater then
				headerTxt = "Duplicate Substance has been created"
				xmlText = "DUPLICATESUBSTANCE"
			else
				headerTxt = "New Substance has been created"
				xmlText = "NEWSUBSTANCE"
			end if
			bSubstanceCreated = True
			GetSubstanceAttributesFromDb(cID)
			DisplaySubstance "", headerTxt, true ,bShowCreateDuplicate, false, false, bShowConflicts, inLineMarker & dBStructure
		End if
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
	End if
	if Session("bManageMode") then
		Response.Write "<center>"
		Response.write "<br /><a class=""menuLink"" href=""CreateOrEditSubstance.asp?ManageMode=1"">Add additional substance</a><br /><br />"
		Response.Write GetOkButton()
		Response.Write "</center>"
	End if
Else
	if Duplicates_dict.Count > 1 then 
		plural = "s"
	else
		art = "an "
	End if	
	duplicateTextDisplay = "none"
	if bShowCreateDuplicate then duplicateTextDisplay = "block"
'	Response.Write "<center><table border=0 bordercolor=666666 cellpadding=1 cellspacing=0 width=""80%""><tr><td align=right><a class=MenuLink href=""#"" onclick=""document.repostForm.ResolveConflictsLater.value=0;document.repostForm.submit();"">Refresh</a></td></tr><tr><td align=center><span class=""GuiFeedback"">Your substance conflicts with " & art & "existing " & Application("appTitle") & " substance" & plural & ".</td></tr></span></center>"
	Response.Write "<tr><td><font size=1>You may:<BR> <ul><li>Select an existing substance</li><span id=""createDuplicateText"" style=""display:" & duplicateTextDisplay & ";""><li>Create a duplicate substance and resolve the conflict at a later time</li></span></ul></font></td></tr></table>"
	cc = 0
	For each key in Duplicates_dict
		'Response.Write "Duplicate " & Duplicates_dict.Item(key) & " found at " & key & "<BR>"
		if len(Key) > 0  and Key <> "0" and Key <> Cstr(CompoundID) then
			cc = cc + 1
			if Duplicates_dict.Count > 1 then snum = cc
			GetSubstanceAttributesFromDb(Key)
			DisplaySubstance "", "Existing Substance " & snum, true ,bShowCreateDuplicate, bShowEdit, false, bShowConflicts,  inLineMarker & dbStructure
		end if
	Next
End if	

if NOT bSubstanceCreated then
	Select Case lcase(action)
		Case "showconflicts"
			bShowSelect = false
			bShowCreateDuplicate = false
			bShowEdit = false
			bShowEditExisting = true
			bShowConflicts = false
			bSameWindow = true
			GetSubstanceAttributesFromDb(CompoundID)
		Case "edit"
			dbStructure = Structure
			dBCompoundID = CompoundID
			dBCAS = CAS
			dBACX_ID = ACX_ID
			dBDensity = Density
			dBALT_ID_1 = ALT_ID_1
			dBALT_ID_2 = ALT_ID_2
			dBALT_ID_3 = ALT_ID_3
			dBALT_ID_4 = ALT_ID_4
			dBALT_ID_5 = ALT_ID_5
			dBSubstanceName = SubstanceName
			bShowSelect = false
			bShowCreateDuplicate = false
			bShowEdit = true
			bShowEditExisting = false
			bShowConflicts = false
		Case Else
			dbStructure = Structure
			dbRegisterIfConflicts = "false"
			dBCompoundID = 0
			dBCAS = CAS
			dBACX_ID = ACX_ID
			dBDensity = Density
			dBALT_ID_1 = ALT_ID_1
			dBALT_ID_2 = ALT_ID_2
			dBALT_ID_3 = ALT_ID_3
			dBALT_ID_4 = ALT_ID_4
			dBALT_ID_5 = ALT_ID_5
			dBSubstanceName = SubstanceName
			bShowSelect = false
			bShowCreateDuplicate = true
			bShowEdit = false
			bShowEditExisting = false
			bShowConflicts = false			
	End Select		
DisplaySubstance "", "Conflicting Substance" , bShowSelect ,bShowCreateDuplicate, bShowEdit, bShowEditExisting, bShowConflicts, inLineMarker & dbStructure
if bShowCreateDuplicate then
	Response.Write "<script language=""javascript"">document.all.createDuplicateText.style.display = 'block';</script>"
end if
%>
	<center>
		<table width="80%">
		<tr>
			<td colspan="2" align="right"> 
			</td>
		</tr>
		</table>
	</table>
	</center>
<%End if%>
	</form>
	<form name="repostForm" action="CreateSubstance3.asp" method="POST">
	<input type="hidden" name="ResolveConflictsLater" value="1">
	<input type="hidden" name="Density" value="<%=Density%>">
		<%
		for each fld in Request.Form
			if fld <> "action" then
				Response.Write "<input type=hidden name=""" & fld & """ value=""" & Request(fld) & """>" & vblf
			end if
		next
		for each fld in Request.QueryString
			if fld <> "action" then
				Response.Write "<input type=hidden name=""" & fld & """ value=""" & Request(fld) & """>" & vblf
			end if
		next
		%>
	</form>
</body>

<%
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
