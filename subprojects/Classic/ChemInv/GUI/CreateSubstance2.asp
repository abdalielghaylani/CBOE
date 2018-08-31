<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<html>
<head>

    <title><%=Application("appTitle")%> -- Create or Edit an Inventory Substance</title>
    <script language="javascript" src="/cheminv/Choosecss.js"></script>
    <script language="javascript" src="CalculateFromPlugin.js"></script>
    <script language="javascript" src="/cheminv/utils.js"></script>
    <script language="JavaScript">
        var cd_plugin_threshold= <%=Application("CD_PLUGIN_THRESHOLD")%>;
        focus();
    </script>
    <script language="JavaScript" src="/cfserverasp/source/chemdraw.js"></script>
    <%if detectModernBrowser = true then%>
    <SCRIPT LANGUAGE="javascript" src="<%=Application("CDJSUrl")%>"></SCRIPT>
    <%end if %>
    <script>cd_includeWrapperFile("/cfserverasp/source/")</script>
    <script language="javascript">
        
        function BackOrClose(){	
            if (document.location.href.indexOf('action=showconflicts') == -1 & history.length > 0){
                var urlBack = document.referrer.replace('&conflictBack=true','');
				if(document.referrer.indexOf("?") > -1) {
					urlBack += '&';
				}
				else {
					urlBack += '?';
				}
				urlBack += 'conflictBack=true';
				if(document.referrer.indexOf("&cddEditMode=true") == -1) {
				    urlBack += '&cddEditMode=true';
				}
				window.location.href = urlBack;
            }
            else{
                opener.close();
                window.close();
            }
		
        }
	
        function OKButtonClick()
        {
            <% if Session("GUIReturnURL") <> "" then %>
                window.location.href="<%=Session("GUIReturnURL")%>";
            <%else%>
                top.close();
            return false;
            <%End if%>
            }
	
        function CloseAndReload()
        {
            opener.location.reload( true );
            self.close();
        }

    </script>
<!--#INCLUDE FILE="../source/app_js.js"-->
</head>
<%if Request("ManageMode") = "2" then %>
<body onunload="CloseAndReload();">
<%else %>
<body>
<%end if %>
    <form name="form1" action="POST">
            <!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
            <!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
            <!--#INCLUDE VIRTUAL = "/cfserverasp/source/csdo_utils_vbs.asp"-->
            <!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
            <!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
            <!--#INCLUDE VIRTUAL = "/cheminv/gui/compound_utils_vbs.asp"-->
            <%
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
Dim ManageMode
Dim LocationTypeID

bSameWindow = false
bDuplicatesFound = false
bIsEdit = false
bDebugPrint = false
bWriteError = False

strError = "Error:CreateSubstance2<BR>"
action = Request("action")
CompoundID = Request("CompoundID")
ResolveConflictsLater = Request("ResolveConflictsLater")
if Application("ENABLE_OWNERSHIP")="TRUE" then
LocationTypeID=Request("LocationTypeID")
end if
bResolveConflictsLater = false
bShowConflicts = false
If ResolveConflictsLater = "1" then bResolveConflictsLater = true

if action = "showconflicts" then session("GUIReturnURL") = ""
ManageMode = Request("ManageMode")
if ManageMode="" then ManageMode=0 
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
Session("tmp_inv_compounds.Substance_Name") = SubstanceName
Session("tmp_inv_compounds.Structure") = Structure
Session("tmp_inv_compounds.CAS") = CAS
Session("tmp_inv_compounds.ACX_ID") = ACX_ID
Session("tmp_inv_compounds.Density") = Density
Session("tmp_inv_Compounds.ALT_ID_1") = ALT_ID_1
Session("tmp_inv_Compounds.ALT_ID_2") = ALT_ID_2
Session("tmp_inv_Compounds.ALT_ID_3") = ALT_ID_3
Session("tmp_inv_Compounds.ALT_ID_4") = ALT_ID_4
Session("tmp_inv_Compounds.ALT_ID_5")  = ALT_ID_5

For each key in unique_alt_ids_dict
	cKey = Replace(Key,"inv_compounds.","")
	uniqueAltIDList =  uniqueAltIDList & cKey & ","
Next
if Len(uniqueAltIDList) > 0 then uniqueAltIDList = left(uniqueAltIDList, len(uniqueAltIDList)-1)

' WJC Millennium  get custom field values from form
for each key in custom_fields_info_dict
    if CustomField_GetCategory(key) = Session("cfTab") and CustomField_GetReadonly(key) = "0" then
        if Request("cc" & key) <> "" or Request.Form("cc" & key) <> "" then
            custom_fields_value_dict(key) = "Yes"
        else
            custom_fields_value_dict(key) = "No"
        end if
    end if
next

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
' WJC Millennium - create/update custom fields
'stop
'CSBR-160188, 160299: Only do this when its Compound Edit
if bIsEdit then
	GetInvConnection()
	' CSBR-158486: Allowing only to delete the custom fields to which the user has access to
	if Session("INV_SUBSTANCE_SAFETY_DATA" & "ChemInv") = false then
		sql = "DELETE FROM CHEMINVDB2.INV_CUSTOM_CPD_FIELD_VALUES WHERE CUSTOM_CPD_FIELD_VALUE_ID IN " & _
		" (SELECT CUSTOM_CPD_FIELD_VALUE_ID FROM CHEMINVDB2.INV_CUSTOM_CPD_FIELD_VALUES, CHEMINVDB2.INV_CUSTOM_FIELDS, CHEMINVDB2.INV_CUSTOM_FIELD_GROUPS, CHEMINVDB2.INV_CUSTOM_FIELD_GROUP_TYPES " & _
		" WHERE CUSTOM_FIELD_GROUP_TYPE_NAME <> 'Safety Data' AND CUSTOM_FIELD_GROUP_TYPE_ID = CUSTOM_FIELD_GROUP_TYPE_ID_FK " & _
		" AND CUSTOM_FIELD_GROUP_ID = CUSTOM_FIELD_GROUP_ID_FK AND CUSTOM_FIELD_ID = CUSTOM_FIELD_ID_FK AND COMPOUND_ID_FK = ?)"
	else
		Sql = "DELETE FROM inv_custom_cpd_field_values WHERE compound_id_fk = ?"
	end if
	Sql = replace(SQL, "inv_", "cheminvdb2.inv_")
	Set cmd = Server.CreateObject("ADODB.Command")
	cmd.ActiveConnection = Conn
	cmd.CommandType = adCmdText
	cmd.CommandText = Sql
	cmd.Parameters.Append cmd.CreateParameter("compound_id_fk", adNumeric, adParamInput, 0, iif(bIsEdit, CompoundID, oCompoundID))
	cmd.Execute
	Set cmd = nothing
	for each key in custom_fields_value_dict
		if custom_fields_value_dict.item(key) <> CustomField_GetDefault(key) then
	' CSBR-158486: Allowing only to delete the custom fields to which the user has access to
			if not (Session("INV_SUBSTANCE_SAFETY_DATA" & "ChemInv") = false AND CustomField_GetCustomFieldGroupTypeName(key) = "Safety Data") then
				strPicklistName = custom_fields_value_dict.item(key)
				CustomFieldID = CustomField_GetKeyId(key)
				PicklistTypeID = CustomField_GetPicklistId(key)
				' Get picklist_id for the current value
				Sql = "SELECT picklist_id FROM inv_picklists WHERE picklist_display = ?"
				Sql = replace(Sql, "inv_", "cheminvdb2.inv_")
				Set cmd = Server.CreateObject("ADODB.Command")
				cmd.ActiveConnection = Conn
				cmd.CommandType = adCmdText
				cmd.CommandText = Sql
				cmd.Parameters.Append cmd.CreateParameter("picklist_name", adLongVarchar, adParamInput, Len(strPicklistName) + 1, strPicklistName)
				dim RS
				Set RS = cmd.Execute
				Set cmd = nothing
				PicklistID = RS("picklist_id").value
				Set RS = nothing
				' Save custom setting
				Sql = "INSERT INTO inv_custom_cpd_field_values (custom_cpd_field_value_id, picklist_id_fk, custom_field_id_fk, compound_id_fk) VALUES (Null, ?, ?, ?)"
				Sql = replace(SQL, "inv_", "cheminvdb2.inv_")
				Set cmd = Server.CreateObject("ADODB.Command")
				cmd.ActiveConnection = Conn
				cmd.CommandType = adCmdText
				cmd.CommandText = Sql
				cmd.Parameters.Append cmd.CreateParameter("picklist_id_fk", adNumeric, adParamInput, 0, PicklistID)
				cmd.Parameters.Append cmd.CreateParameter("custom_field_id_fk", adNumeric, adParamInput, 0, CustomFieldID)
				cmd.Parameters.Append cmd.CreateParameter("compound_id_fk", adNumeric, adParamInput, 0, iif(bIsEdit, CompoundID, oCompoundID))
				cmd.Execute
				Set cmd = nothing
			end if
		end if
	Next	     
	For each key in alt_ids_dict
		if left(alt_ids_dict.item(key),1) = "*" then
			PicklistID = Request(key)
	'        if PicklistID <> AltField_GetDefault(key) then ' WJC now returns "" when value is the default
			if PicklistID <> "" then
				AltFieldID = AltField_GetKeyId(key)
				' Save custom setting
				Sql = "INSERT INTO inv_custom_cpd_field_values (custom_cpd_field_value_id, picklist_id_fk, custom_field_id_fk, compound_id_fk) VALUES (Null, ?, ?, ?)"
				Sql = replace(SQL, "inv_", "cheminvdb2.inv_")
				Set cmd = Server.CreateObject("ADODB.Command")
				cmd.ActiveConnection = Conn
				cmd.CommandType = adCmdText
				cmd.CommandText = Sql
				cmd.Parameters.Append cmd.CreateParameter("picklist_id_fk", adNumeric, adParamInput, 0, PicklistID)
				cmd.Parameters.Append cmd.CreateParameter("custom_field_id_fk", adNumeric, adParamInput, 0, AltFieldID)
				cmd.Parameters.Append cmd.CreateParameter("compound_id_fk", adNumeric, adParamInput, 0, iif(bIsEdit, CompoundID, oCompoundID))
				cmd.Execute
				Set cmd = nothing
			end if
		end if
	Next
	Conn.Close
end if
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
            if not Session("bManageMode") then  ' WJC Otherwise we get Close and OK buttons
                Response.Write( "<br /><br /><div align=""center"">" )
                if( ManageMode = 2 ) then
                    ' ManageMode = 2 was developed for the "Manage Substance" link when viewing a container's substance.
                    ' In this case, we need to reload the parent window so any modifications are visible.
                    Response.Write( CloseButton( "Close this window", "CloseAndReload();return false;" ) )
			    else
			        Response.Write( GetCloseButton )
			    end if
			    Response.Write( "</div>" )
			end if
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
	Response.Write "<center><table border=0 bordercolor=666666 cellpadding=1 cellspacing=0 width=""80%""><tr><td align=right><a class=MenuLink href=""#"" onclick=""document.repostForm.action = 'CreateSubstance2.asp?action=" & action & "';document.repostForm.ResolveConflictsLater.value=0;document.repostForm.submit();"">Refresh</a></td></tr><tr><td align=center><span class=""GuiFeedback"">Your substance conflicts with " & art & "existing " & Application("appTitle") & " substance" & plural & ".</td></tr></span></center>"
	Response.Write "<tr><td><font size=1>You may:<BR> <ul><li>Select an existing substance</li><li>Make the appropriate edits to resolve the conflict</li><span id=""createDuplicateText"" style=""display:" & duplicateTextDisplay & ";""><li>Create a duplicate substance and resolve the conflict at a later time</li></span></ul></font></td></tr></table>"
	cc = 0
	For each key in Duplicates_dict
		'Response.Write "Duplicate " & Duplicates_dict.Item(key) & " found at " & key & "<BR>"
		if len(Key) > 0  and Key <> "0" and Key <> Cstr(CompoundID) then
			cc = cc + 1
			if Duplicates_dict.Count > 1 then snum = cc
			GetSubstanceAttributesFromDb(Key)
			DisplaySubstance "", "Existing Substance " & snum, true ,bShowCreateDuplicate, bShowEdit, false, bShowConflicts,  inLineMarker & dBStructure
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
		Case else
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
			bShowEdit = true
			bShowEditExisting = false
			bShowConflicts = false
	End Select		
DisplaySubstance "", "Conflicting Substance" , bShowSelect ,bShowCreateDuplicate, bShowEdit, bShowEditExisting, bShowConflicts, inLineMarker & dBStructure
if bShowCreateDuplicate then
	Response.Write "<script language=""javascript"">document.all.createDuplicateText.style.display = 'block';</script>"
end if
            %>
            <center>
		<table width="80%">
		<tr>
			<td colspan="2" align="right"> 
				<a HREF="#" onclick="OKButtonClick()"><img SRC="../graphics/ok_dialog_btn.gif" border="0" alt="Ok" WIDTH="61" HEIGHT="21"></a>
				<a HREF="#" onclick="BackOrClose(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" alt="Cancel" WIDTH="61" HEIGHT="21"></a>
			</td>
		</tr>
		</table>
	</table>
	</center>
            <%End if%>
        </form>
        <form name="repostForm" action="CreateSubstance2.asp" method="POST">
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
