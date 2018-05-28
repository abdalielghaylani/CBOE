<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/csdo_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/compound_utils_vbs.asp"-->
<%
' WJC Millennium convert certain Request variables to temporary Session variables
' Not action, ManageMode, CompoundID, TB
Dim aPersist
aPersist = Array("inv_compounds.Substance_Name", "inv_compounds.Structure","inv_compounds.CAS","inv_compounds.ACX_ID","inv_compounds.Density","inv_compounds.ALT_ID_1","inv_compounds.ALT_ID_2","inv_compounds.ALT_ID_3","inv_compounds.ALT_ID_4","inv_compounds.ALT_ID_5")
if Request("TB") = "" and Request("cddEditMode") = "" then
    for nPersist = LBound(aPersist) to UBound(aPersist)
        strPersist = aPersist(nPersist)
        Session("tmp_"&strPersist) = Request(strPersist)
    next
end if
'
Dim Conn
Dim Cmd
Dim cddEditMode
cddEditMode = Request("cddEditMode")
Response.Expires = -1
action = Request("action")
CompoundID = Request("CompoundID")
if CompoundID = "" then CompoundID = 0
TempCdxPath = Application("AppTempDirPathHTTP")& "/" & Application("appkey")& "Temp/"
inLineCdx = TempCdxPath & "nostructure.cdx"
inLineMarker = "data:chemical/x-cdx;base64,"
ManageMode = Request("ManageMode")
if ManageMode = "1" then 
	Session("GUIReturnURL") = "/cheminv/inputtoggle.asp?formgroup=substances_form_group&dbname=cheminv&amp;GotoCurrentLocation=true"
Else
	Session("GUIReturnURL") = ""
End if
Caption = "Enter substance attributes:"
if CompoundID > 0 then
    FNP = "UID." & CompoundID & ":" & "inv_compounds."
    if Request("TB") = "" then
        Call GetSubstanceAttributesFromDb(CompoundID)
        Session("tmp_inv_compounds.Substance_Name") = dbSubstanceName
        Session("tmp_inv_compounds.Structure") = dbStructure
        Session("tmp_inv_compounds.CAS") = dbCAS
        Session("tmp_inv_compounds.ACX_ID") = dbACX_ID
        Session("tmp_inv_compounds.Density") = dbDensity
        Session("tmp_inv_compounds.ALT_ID_1") = dbALT_ID_1
        Session("tmp_inv_compounds.ALT_ID_2") = dbALT_ID_2
        Session("tmp_inv_compounds.ALT_ID_3") = dbALT_ID_3
        Session("tmp_inv_compounds.ALT_ID_4") = dbALT_ID_4
        Session("tmp_inv_compounds.ALT_ID_5") = dbALT_ID_5
        if Application("ENABLE_OWNERSHIP")="TRUE" then
        LocationTypeID=LocationTypeIDFK
        end if
    end if
    bIsEdit = true
    Caption = "Edit substance attributes:"
    action = "edit"
Else
    if Request("TB") = "" then
        Call CustomFieldsValue_CompoundGet(0)
        dbSubstanceName = Session("tmp_inv_compounds.Substance_Name")
        dbStructure = Session("tmp_inv_compounds.Structure")
        dbCAS = Session("tmp_inv_compounds.CAS")
        dbACX_ID = Session("tmp_inv_compounds.ACX_ID")
        dbDensity = Session("tmp_inv_compounds.Density")
        dbALT_ID_1 = Session("tmp_inv_compounds.ALT_ID_1")
        dbALT_ID_2 = Session("tmp_inv_compounds.ALT_ID_2")
        dbALT_ID_3 = Session("tmp_inv_compounds.ALT_ID_3")
        dbALT_ID_4 = Session("tmp_inv_compounds.ALT_ID_4")
        dbALT_ID_5 = Session("tmp_inv_compounds.ALT_ID_5")
    end if
End If
if Request("TB") <> "" then
	' CSBR-159359: Get the values from form fields when switching around the custom field tabs
	dbSubstanceName = Request("inv_compounds.Substance_Name")
	dbStructure = Request("inv_compounds.Structure")
	dbCAS = Request("inv_compounds.CAS")
	dbACX_ID = Request("inv_compounds.ACX_ID")
	dbDensity = Request("inv_compounds.Density")
	dbALT_ID_1 = Request("inv_Compounds.ALT_ID_1")
	dbALT_ID_2 = Request("inv_Compounds.ALT_ID_2")
	dbALT_ID_3 = Request("inv_Compounds.ALT_ID_3")
	dbALT_ID_4 = Request("inv_Compounds.ALT_ID_4")
	dbALT_ID_5 = Request("inv_Compounds.ALT_ID_5")
	if Application("ENABLE_OWNERSHIP")="TRUE" then
		if trim(Request("LocationTypeID")) <> "" then LocationTypeID = Request("LocationTypeID")
	end if
    ' WJC Millennium get custom field values from form
    for each key in custom_fields_info_dict
        if CustomField_GetCategory(key) = Session("cfTab") and CustomField_GetReadonly(key) = "0" then
            if (Request("cc" & key) <> "" or Request.Form("cc" & key) <> "") then
                custom_fields_value_dict(key) = "Yes"
            else
                custom_fields_value_dict(key) = "No"
            end if
        end if
    next
end if
if dbStructure <> "" then
	inLineCdx = inLineMarker & dbStructure
'elseif ManageMode = "1" then
else
	inLineCdx = ""
end if

%>
<html>
<head>
<meta NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>

<%
if inLineCdx = "" OR cddEditMode <> "" then
%>
    <script LANGUAGE="javascript" src="<%=Application("CDJSUrl")%>"></script>
<%
end if
%>
<script LANGUAGE="javascript">
<!--
	var cd_plugin_threshold= <%=Application("CD_PLUGIN_THRESHOLD")%>;
	var blankb64 = "VmpDRDAxMDAEAwIBAAAAAAAAAAAAAAAAAAAAAAMAEAAAAENoZW1EcmF3IDYuMC4xCAAMAAAAbXl0ZXN0LmNkeAADMgAIAP///////wAAAAAAAP//AAAAAP////8AAAAA//8AAAAA/////wAAAAD/////AAD//wEJCAAAAFkAAAAEAAIJCAAAAKcCAAAXAgIIEAAAAAAAAAAAAAAAAAAAAAAAAwgEAAAAeAAECAIAeAAFCAQAAJoVAAYIBAAAAAQABwgEAAAAAQAICAQAAAACAAkIBAAAswIACggIAAMAYAC0AAMACwgIAAQAAADwAAMADQgAAAAIeAAAAwAAAAEAAQAAAAAACwARAAAAAAALABEDZQf4BSgAAgAAAAEAAQAAAAAACwARAAEAZABkAAAAAQABAQEABwABJw8AAQABAAAAAAAAAAAAAAAAAAIAGQGQAAAAAAJAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAEAhAAAAD+/wAA/v8AAAIAAAACAAABJAAAAAIAAwDkBAUAQXJpYWwEAOQEDwBUaW1lcyBOZXcgUm9tYW4BgAEAAAAEAhAAAAD+/wAA/v8AAAIAAAACAA8IAgABABAIAgABABYIBAAAACQAGAgEAAAAJAAAAAAA";
	var currentb64;
	window.focus();
	//CSBR-159359: Separate function for retrieving the base64 data; This will be called in form submit and when switching the custom field tabs
	function GetCDXData() {
	    var b64;
	    <%if detectModernBrowser = true and dbStructure <> "" and cddEditMode = "" then%>
	    b64 = currentb64.substring(currentb64.indexOf('VmpD'));
	    <%else%>
	    b64 = cd_getData("mycdx", "chemical/x-cdx");
	    <%end if%>
		if (b64.length == 0){
			b64 = blankb64;
			document.form1.isEmptyStruc.value= "1";
		}
		document.form1["inv_compounds.Structure"].value = b64;
	}

	function postToSelf(strTab){
		GetCDXData();// CSBR-159359: Call this when switching around the custom field tabs
		document.form1.action = "/cheminv/gui/CreateOrEditSubstance.asp?ManageMode=<%=ManageMode%>&action=<%=action%>&CompoundID=<%=CompoundID%>&TB=" + strTab;
		document.form1.submit();
	}
	
	function getStrucData(){
		GetCDXData();// CSBR-159359: Call this during form submit
		document.form1.action = "/cheminv/gui/CreateSubstance2.asp?action=<%=action%>";
		ValidateSubstance();
	}
	
	function ValidateSubstance(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		// SubstanceName is required
		
		var SN = document.form1["inv_compounds.Substance_Name"].value;
		if (trim(SN).length == 0) {
			errmsg = errmsg + "- SubstanceName is required.\r";
			bWriteError = true;
		}
		
		if (trim(SN).length>=255) {
			errmsg = errmsg + "- Substance Name should be less than 255 Characters.\r";
			bWriteError = true;
		}
		var CAS = document.form1["inv_compounds.CAS"];
		if (CAS.value.length > 0) {
			if ((CAS.value.toLowerCase() == "na") || (CAS.value.toLowerCase() == "n/a") || (CAS.value.toLowerCase() == "n.a.")){ 
				CAS.value = "";
			}
			else{
				if (!isCAS(CAS.value)) {
					errmsg = errmsg + "- Invalid CAS number.\r";
					bWriteError = true;
				}
			}		
		}
		
		
		<%For each key in req_alt_ids_dict
			response.write "if (document.form1[""" & key & """].value.length == 0){" & vblf 
			response.write "errmsg = errmsg + ""- " & req_alt_ids_dict.item(Key) & " is required.\r"";" & vblf
			response.write " bWriteError = true;" & vblf
			response.write "}" & vblf	
		Next%>
		// Report problems
		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.submit();
		}
	}
//-->
</script>
<% if Session("isCDP") = "TRUE" then %>
<script language="JavaScript" src="/cfserverasp/source/chemdraw.js"></script>
<script>cd_includeWrapperFile("/cfserverasp/source/")</script>
<% end if %>
<title>Create or Edit a substance in <%=Application("appTitle")%></title>
</head>
<body>
<br><br><br>
<center>
<form name="form1" action method="POST">
	<input type="hidden" name="CompoundID" value="<%=CompoundID%>">
	<input type="hidden" name="ManageMode" value="<%=ManageMode%>">
	<input type="hidden" name="isEmptyStruc">
	<input type="hidden" name="inv_compounds.Density" value="<%=dbDensity%>">
	<table border="0" width="500" cellpadding="0" cellspacing="0">
		  <tr>
			<td valign="top" align="left" colspan=2>
				<img src="<%=Application("NavButtonGifPath")%>cheminventory_banner.gif" border="0"><br><br>
			</td>
			<td align="right">
				<%If Application("Admin_required") then%>
				<br><br><font face="Arial" color="#42426f" size="1"><b>Current login:&nbsp;<%=Ucase(Session("UserNameChemInv"))%></b></font>
				<%End if%>
			</td>
		 </tr>
		  <tr>
			<td>
				&nbsp;
			</td>
			<td colspan="2">
				<span class="GuiFeedback"><%=Caption%></span><br><br>
			</td>
		  </tr>
		  <tr>
			<td>
				&nbsp;
			</td>
			<td colspan="2">
				<table border="0" cellspacing="0" cellpadding="0" width="500">
					<tr>
						<td align="left" nowrap>
							<span title="(eg. Acetonitrile)" class="required">Substance Name:</span>
						</td>
						<td align="left">
							<input type="text" name="inv_compounds.Substance_Name" value="<%=dBSubstanceName%>" size="55"> 
							<input type="hidden" name="<%=FNP%>Substance_Name_orig" value="<%=dBSubstanceName%>">
						</td>
					</tr>
				</table>
				<br>
			</td>
		  </tr>
		  <tr>
		  	<td rowspan="2">
		  		<img src="../graphics/pixel.gif" width="5" height="1" alt border="0">
		  	</td>
			<td width="300" valign="top">
				<table border="1" cellpadding="0" cellspacing="0" width="280" height="280">
					<tr>
						<td valign="top">
						<% if Session("isCDP") = "TRUE" then %>
							<input type="hidden" name="inv_compounds.Structure" value>
							<input type="hidden" name="inv_compounds.Structure.sstype" value="1">
							<%if bIsEdit then%>
								<input type="hidden" id="<%=FNP%>BASE64_CDX<%=CompoundID%>_orig" name="<%=FNP%>BASE64_CDX<%=CompoundID%>_orig" value="<%=InLineCdx%>">
			                    <%if detectModernBrowser = true and cddEditMode = "" then%><script LANGUAGE="javascript">currentb64 =  document.getElementById('<%=FNP%>BASE64_CDX<%=CompoundID%>_orig').value;</script><%end if%>
                                <input type="hidden" name="ExactSearchFields" value="<%=FNP%>BASE64_CDX<%=CompoundID%>">
								<input type="hidden" name="RelationalSearchFields" value="<%=FNP%>BASE64_CDX,<%=FNP%>Substance_Name,<%=FNP%>CAS,<%=FNP%>ACX_ID,<%=FNP%>ALT_ID_1,<%=FNP%>ALT_ID_2,<%=FNP%>ALT_ID_3,<%=FNP%>ALT_ID_4,<%=FNP%>ALT_ID_5,<%=FNP%>Conflicting_Fields">
								<input type="hidden" name="row_id_table_names" value="inv_compounds">
								<input type="hidden" name="inv_compounds_ROW_IDS" value="<%=CompoundID%>">
								
							<%else%>
								<input type="hidden" name="ExactSearchFields" value="inv_compounds.Structure">
								<input type="hidden" name="add_order" value="inv_compounds">
								<input type="hidden" name="add_record_action" value="Duplicate_Search_No_Add">	
								<input type="hidden" name="commit_type" value="full_commit">	
								<input type="hidden" name="RelationalSearchFields" value="inv_compounds.Substance_Name;0,inv_compounds.CAS;0,inv_compounds.ACX_ID;0,inv_compounds.ALT_ID_1;0,inv_compounds.ALT_ID_2;0,inv_compounds.ALT_ID_3;0,inv_compounds.ALT_ID_4;0,inv_compounds.ALT_ID_5;0,inv_compounds.Conflicting_Fields">
								<script language="JavaScript">//cd_insertObjectStr("<embed src='/CFWTEMP/cheminv/cheminvTemp/mt.cdx' width='280' height='280' id='5' name='mycdx' type='chemical/x-cdx'>");</script>
							<%end if%>
							<input type="hidden" name="inline" value="<%=InLineCdx%>">
							<%
							if session("DrawPref") = "ISIS" then
								ISISDraw = """True"""
							else
								ISISDraw = """False"""
							end if'
							end if
                            if detectModernBrowser = true and dbStructure <> "" and cddEditMode = "" then
                                SessionDir = Application("TempFileDirectory" & "ChemInv") & "Sessiondir"  & "\" & Session.sessionid & "\"
		                        filePath = SessionDir & "structure" & "_" & 520 & "x" & 300 & ".gif"	
		                        SessionURLDir = Application("TempFileDirectoryHTTP" & "ChemInv") & "Sessiondir"  & "/" & Session.sessionid & "/"
		                        fileURL = SessionURLDir & "structure" & "_" & 520 & "x" & 300 & ".gif"	
		                        ConvertCDXtoGif_Inv filePath, Mid(InLineCdx, InStr(InLineCdx, "VmpD")), 520, 300
                                Dim action
                                If CompoundID <> "0" then action = "&action=edit" end if
		                        Response.Write "<a target=""_top"" href=""/cheminv/gui/CreateOrEditSubstance.asp?ManageMode=1"& action &"&cddEditMode=true&CompoundID="+ CompoundID +"""><img src=""" & fileURL & """ width=""520"" height=""300"" border=""0""></a><br/><span style="""" class=""required"">Click on image to edit structure</span>"
                            else
                                %>
                            <script language="JavaScript">cd_insertObject("chemical/x-cdx", "520", "300", "mycdx", "<%=TempCdxPath%>mt.cdx", "False", "true", escape(document.all.inline.value),  "true", <%=ISISDraw%>)</script>
						    <%end if%>
                        </td>
					</tr>
				</table>		
			</td>
		  	<td valign="top">
				<table border="0" cellpadding="5px" style="padding-left:10px;">
					
					<tr>
			        	<td>
							<span title="(eg. 50-50-0)">CAS Registry#:</span><br>
							<input type="text" name="inv_compounds.CAS" value="<%=dBCAS%>" SIZE="30" maxlength="15">
							<%if bIsEdit then %>
								<input type="hidden" name="<%=FNP%>CAS_orig" value="<%=dBCAS%>"> 
							<%end if%>
						</td>
						</tr>
						<tr>
		      		    <td>
							<span title="(e.g. X1001545-9)">ACX Number:<br>
							<input type="text" name="inv_compounds.ACX_ID" value="<%=dBACX_ID%>" SIZE="30" value maxlength="15">
							<%if bIsEdit then %>
								<input type="hidden" name="<%=FNP%>ACX_ID_orig" value="<%=dBACX_ID%>"> 
							<%end if%>
						</td>
					</tr>
					<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
					<tr>
		                <td>
			                <span class="">Location Type:<span>
			                <%=ShowSelectBox3("LocationTypeID", LocationTypeID,"SELECT Location_Type_ID AS Value, Location_Type_Name AS DisplayText FROM inv_Location_Types ORDER BY Location_Type_Name ASC", 0, "", "","")%>
		                </td>
	                </tr>
	                <% end if %>
					<%
					
					For each key in alt_ids_dict
					    strLabel = alt_ids_dict.Item(Key) & ":"   ' WJC Millennium
					    bCustom = (left(strLabel, 1) = "*")
					    if bCustom then strLabel = mid(strLabel,2)
						Response.Write "<TR><td>" & vblf
						if req_alt_ids_dict.Exists(Key) then
							requiredFields =  requiredFields & "," & Key & ";0:" & strLabel
						end if
						cKey = Replace(Key, "inv_compounds.","")
						
						if bCustom then
						    response.Write ShowCustomPicklist(Key)
						else
						    if req_alt_ids_dict.Exists(Key) then
							    Response.Write "<span class=""required"">"
						    Else
							    Response.Write "<span title="""">"
						    end if
						    Response.Write strLabel & "</span><br>"
						    Response.write "<input type=text name=""" & Key & """ size=30 value=""" & Eval("dB" & cKey) & """>"
				        end if
						if bIsEdit then 
							Response.write "<input type=hidden name=""" & FNP & ckey & "_orig"" value=""" & Eval("dB" & cKey) & """>"
						End if
						Response.Write "</td></TR>" & vblf 
					Next 
					%>
		    	</table>
			</td>
		  </tr>
<%
    ' WJC Millennium construct create or edit table
    Set tabs_dict = CustomFieldsInfo_GetCategories()
    if (tabs_dict.Count > 0) And bIsEdit then
        ' Which tab
	'CSBR-158685: The default showing tab should also follow the configuration
        if Session("cfTab") = "" then
			for each key in tabs_dict
				if not (lcase(Application("DISPLAY_SAFETY_DATA")) = "false" AND tabs_dict(key)(1) = "Safety Data") then
					Session("cfTab") = tabs_dict(key)(0)
					exit for
				end if
			Next
		end if
        if Request("TB") <> "" then Session("cfTab") = Request("TB")
        ' Empty separator row
        Response.Write "<tr><td>&nbsp;</td></tr>" & vblf
        ' Tabs
        Response.Write "<tr>" & vblf
        Response.Write " <td>&nbsp;</td>" & vblf
        Response.Write " <td colspan='2'>" & vblf
	    ' Create TabView Object
	    Set TabView = Server.CreateObject("VASPTB.ASPTabView")

	    TabView.Class = "TabView"
	    TabView.ImagePath = "../images/tabview"
	    TabView.BackColor = "#e1e1e1"
	    TabView.SelectedBackColor = "#adadad"
	    TabView.SelectedForeColor = ""
	    TabView.SelectedBold = False
	    TabView.BodyBackground = "#ffffff"
	    TabView.TabWidth = 0
	    TabView.StartTab = Session("cfTab")
	    TabView.QueryString = ""
	    TabView.LicenseKey = "7993-3E51-698B"
    	
	    for each key in tabs_dict
	        ' Add a Tab to the TabView component.
	        ' Syntax: TABVIEW.TABS.ADD([Key],[Text],[URL],[Target],[ToolTipText])
	        ' All arguments in the Add method are optional.
	        ' If the [URL] property is empty it will refresh the same page
	        ' passing in the QueryString TB=[Key] EG: my.asp?TB=TAB1
		'CSBR-158685: Display the Safety Data based on configuration
			if not (lcase(Application("DISPLAY_SAFETY_DATA"))="false" AND tabs_dict(key)(1) = "Safety Data") then
				Set Tabx = TabView.Tabs.Add(key,key,"","","")
				if Tabx.Key = Session("cfTab") then
					Tabx.DHTML = "onclick=""return false;"""
				else
					Tabx.DHTML = "onclick=""postToSelf('" & Tabx.Key & "'); return false;"""
				end if
				Tabx.DHTML = Tabx.DHTML & " onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"
				Tabx.DHTML = Tabx.DHTML & " onmouseout=javascript:this.style.cursor='default'"
			end if
        Next
    	
	    ' Write out the HTML for the tabs
		'CSBR-158922-Show the tabview only if it has tabs to show; Note: Tabs would be still needed for non-safety data
		if TabView.Tabs.count > 0 then TabView.Show
        Response.Write vblf
    	
	    ' Clean up the TabView object
	    TabView.Unload
	    Set TabView = Nothing
        Response.Write " </td>" & vblf
        Response.Write "</tr>" & vblf
        
        ' Populate the tab
        nCol = 0
		if Instr(LCase(Session("cfTab")),"sentence")>0 then
			nCols = 1
		else
			nCols = 2
		end if
		Response.Write "<tr><td colspan=4><table>"
        for each key in custom_fields_info_dict
		'CSBR-158685: Display the Safety Data based on configuration
			if not (lcase(Application("DISPLAY_SAFETY_DATA"))="false" AND CustomField_GetCustomFieldGroupTypeName(key) = "Safety Data") then
				if CustomField_GetCategory(key) = Session("cfTab") then
					if nCol = 0 then Response.Write "<tr>" & vblf & "<td width=50>&nbsp;</td>" & vblf
					Response.Write "<td>" & vblf
					Response.write "<td width=""1""><input type='checkbox'"
					if custom_fields_value_dict.item(key) = "Yes" then
						Response.write " checked"
					end if
				' CSBR-158486: added additional condition to disable based on the privilege and CustomFieldGroupTypeName
					if CustomField_GetReadonly(key) <> "0" OR (Session("INV_SUBSTANCE_SAFETY_DATA" & "ChemInv") = false AND CustomField_GetCustomFieldGroupTypeName(key) = "Safety Data") then
						Response.write " disabled"
					end if
					Response.write " name=""" & "cc" & key & """ size=60 value=""" & "Yes" & """>"
					if bIsEdit then 
						Response.write "<input type=hidden name=""" & FNP & "cc" & key & "_orig"" value=""" & "Yes" & """>"
					End if
					Response.Write "</td><td><span>" & ShowCustomSymbol(key) & "&nbsp;" & key & "</span>"
					Response.Write "</td>" & vblf
					nCol = nCol + 1
					if nCol = nCols then 
						Response.Write "</tr>" & vblf
						nCol=0
					end if
				end if
			end if
        next
        if nCol = 1 then Response.Write "</tr>" & vblf
		Response.write "</table></td></tr>"
    end if
%>		
		  <tr>
<%     if tabs_dict.Count > 0 then %>
			<td align="right" colspan="3">
<%     else %>
			<td align="right" colspan="2">
<%     end if%>
<%     Set tabs_dict = Nothing%>
				<%=GetCancelButton()& vblf%>
				<a HREF="Create%20a%20new%20substance" onclick="getStrucData(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0" alt="Create a new substance for this container"></a>
			</td>
		</tr>
		</table>

	<input type="hidden" name="return_location" value>
	<input type="hidden" name="no_gui" value="true">
	
</form>
</center>
</body>
</html>

