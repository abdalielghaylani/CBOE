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
Dim ALT_ID_ClassDict
aPersist = Array("inv_compounds.Substance_Name", "inv_compounds.Structure","inv_compounds.CAS","inv_compounds.ACX_ID","inv_compounds.Density","inv_compounds.ALT_ID_1","inv_compounds.ALT_ID_2","inv_compounds.ALT_ID_3","inv_compounds.ALT_ID_4","inv_compounds.ALT_ID_5","inv_compounds.NDC_Investigational","inv_compounds.Package_Size")
if Request("TB") = "" then
    for nPersist = LBound(aPersist) to UBound(aPersist)
        strPersist = aPersist(nPersist)
        Session("tmp_"&strPersist) = Request(strPersist)
    next
end if

'
Dim Conn
Dim Cmd
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
Caption = "Enter drug attributes:"
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
        Session("tmp_inv_compounds.NDC_Investigational") = dBNDCInvestigational
        Session("tmp_inv_compounds.Package_Size") = dBPackageSize
    end if
    bIsEdit = true
    Caption = "Edit drug attributes:"
    action = "edit"
Else
    if Request("TB") = "" then
        Call CustomFieldsValue_CompoundGet(0)
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
        Session("tmp_inv_compounds.NDC_Investigational") = dBNDCInvestigational
        Session("tmp_inv_compounds.Package_Size") = dBPackageSize
    end if
End If
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
dBNDCInvestigational = Session("tmp_inv_compounds.NDC_Investigational")
dBPackageSize = Session("tmp_inv_compounds.Package_Size")
if Request("TB") <> "" then
    ' WJC Millennium get custom field values from form
    for each key in custom_fields_info_dict
        if CustomField_GetCategory(key) = Session("cfTab") and CustomField_GetReadonly(key) = "0" then
            if Request("cc" & key) <> "" then
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

' DFCI custom field classes
set ALT_ID_ClassDict = Server.CreateObject("Scripting.Dictionary")

' PCN:
ALT_ID_ClassDict.Add "inv_compounds.ALT_ID_2", ""

' Protocol ID:
ALT_ID_ClassDict.Add "inv_compounds.ALT_ID_3", "investigational_required"

' GCN:
ALT_ID_ClassDict.Add "inv_compounds.ALT_ID_4", ""

%>
<html>
<head>
<style>
SPAN.commercial_required
{
    font-size: 12px;
    color: red;
    font-family: verdana, arial, helvetica, sans-serif;
}
SPAN.investigational_required
{
    font-size: 12px;
    color: red;
    font-family: verdana, arial, helvetica, sans-serif;
}
</style>
<meta NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script LANGUAGE="javascript">
<!--
	var cd_plugin_threshold= <%=Application("CD_PLUGIN_THRESHOLD")%>;
	var blankb64 = "VmpDRDAxMDAEAwIBAAAAAAAAAAAAAAAAAAAAAAMAEAAAAENoZW1EcmF3IDYuMC4xCAAMAAAAbXl0ZXN0LmNkeAADMgAIAP///////wAAAAAAAP//AAAAAP////8AAAAA//8AAAAA/////wAAAAD/////AAD//wEJCAAAAFkAAAAEAAIJCAAAAKcCAAAXAgIIEAAAAAAAAAAAAAAAAAAAAAAAAwgEAAAAeAAECAIAeAAFCAQAAJoVAAYIBAAAAAQABwgEAAAAAQAICAQAAAACAAkIBAAAswIACggIAAMAYAC0AAMACwgIAAQAAADwAAMADQgAAAAIeAAAAwAAAAEAAQAAAAAACwARAAAAAAALABEDZQf4BSgAAgAAAAEAAQAAAAAACwARAAEAZABkAAAAAQABAQEABwABJw8AAQABAAAAAAAAAAAAAAAAAAIAGQGQAAAAAAJAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAEAhAAAAD+/wAA/v8AAAIAAAACAAABJAAAAAIAAwDkBAUAQXJpYWwEAOQEDwBUaW1lcyBOZXcgUm9tYW4BgAEAAAAEAhAAAAD+/wAA/v8AAAIAAAACAA8IAgABABAIAgABABYIBAAAACQAGAgEAAAAJAAAAAAA";
	var sDrugType = "Commercial";
	var sDrugTypeID = "1000";

	window.focus();
	function postToSelf(strTab){
		document.form1.action = "/cheminv/gui/CreateOrEditSubstance.asp?ManageMode=<%=ManageMode%>&action=<%=action%>&CompoundID=<%=CompoundID%>&TB=" + strTab;
		document.form1.submit();
	}
	
	function getStrucData(){
		
		var b64 = cd_getData("mycdx", "chemical/x-cdx");
		//alert(b64)
		//if (b64.length == 0){
			b64 = blankb64;
		//	document.form1.isEmptyStruc.value= "1";
		//}
		
		document.form1["inv_compounds.Structure"].value = b64;
		
		//alert(b64)
		document.form1.action = "/cheminv/gui/CreateSubstance2.asp?action=<%=action%>";
		ValidateSubstance();
	}
	
	function ValidateSubstance()
	{		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		
		// SubstanceName is required		
		var SN = document.form1["inv_compounds.Substance_Name"].value;
		if (trim(SN).length == 0)
		{
			errmsg = errmsg + "- Drug Name is required.\r";
			bWriteError = true;
		}
		
		var ABC = trim(document.form1["inv_compounds.ACX_ID"].value);
		if( ABC.length > 0 )
		{
		     if( ABC.length != 7 )
		     {
		        errmsg = errmsg + "- ABC Number must be 7 digits.\r";
			    bWriteError = true;			
		     }
		     else 
		     {
		        if( isNaN(parseInt(ABC)) )
		        {
		            errmsg = errmsg + "- ABC Number must be numeric.\r";
			        bWriteError = true;
			    }
		     }
		}
			
		if( (sDrugTypeID == "1000") || (sDrugTypeID == "1002") || (sDrugTypeID == "1003") )
        {
            var NDC = trim(document.form1["inv_compounds.CAS"].value);
		    if( NDC.length == 0 )
		    {
		        errmsg = errmsg + "- NDC is required for " + sDrugType + " drugs.\r";
			    bWriteError = true;			
		    }
		    else
		    {
		        if( NDC.length != 11 )
		        {
		            errmsg = errmsg + "- NDC number must be 11 digits.\r";
			        bWriteError = true;
		        }
		        if( isNaN(parseInt(NDC)) )
		        {
		            errmsg = errmsg + "- NDC number must be numeric.\r";
			        bWriteError = true;
		        }
		    }
        }
        else if( sDrugTypeID == "1001" )
        {
          /*  if( document.form1["inv_compounds.NDC_INVESTIGATIONAL"].value.length == 0 )
            {            
		        errmsg = errmsg + "- NDC for Investigational is required for Investigational drugs.\r";
		        bWriteError = true;
		    } */
            
            if( document.form1["inv_compounds.ALT_ID_3"].value.length == 0 )
            {            
		        errmsg = errmsg + "- Protocol ID is required for Investigational drugs.\r";
		        bWriteError = true;
		    }
        }
        
        var BDPP = trim(document.form1["inv_compounds.ALT_ID_1"].value);
		if( BDPP.length == 0 )
		{
		    errmsg = errmsg + "- BDPP is required.\r";
		    bWriteError = true;
		}
	//	else
	//	{
	//	    if( BDPP.length != 5 )
	//       {
	//            errmsg = errmsg + "- BDPP must be 5 digits.\r";
	//	        bWriteError = true;
	//        }
	//    if( isNaN(parseInt(BDPP)) )
	//    {
	//        errmsg = errmsg + "- BDPP must be numeric.\r";
	//	        bWriteError = true;
	//   }
	//	}
        
		// Report problems
		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.submit();
		}
	}
	
	// DFCI custom functionality... change which fields are required based on the drug type
	function OnChangePicklist(objSelect, strLabel)
	{	    
	var iSelected = objSelect.value;
	    if( strLabel == "Drug Type:" )
	    {
	        for( i = 0; i < objSelect.options.length; i++ )
	        {
	            if( objSelect.options(i).value == iSelected )
	            {
	                sDrugType = objSelect.options(i).text;
			sDrugTypeID = objSelect.options(i).value;
	            }
	        }      
	        SwitchRequiredFields();
	    }

	}
//need to add an on load switch so that the proper fields are activated initially
	function onLoadSwitch() {
	OnChangePicklist(document.form1["inv_compounds.ALT_ID_5"],"Drug Type:");
}
	
	function SwitchRequiredFields()
	{
	    if( (sDrugTypeID == "1000") || (sDrugTypeID == "1002") || (sDrugTypeID == "1003") )
        {
            AlterCSS('SPAN.commercial_required','color','red');
            AlterCSS('SPAN.investigational_required','color','black');
        }
        else if( sDrugTypeID == "1001" )
        {            
            AlterCSS('SPAN.commercial_required','color','black');
            AlterCSS('SPAN.investigational_required','color','red');
        }
	}
	SwitchRequiredFields();
//-->
</script>
<% if Session("isCDP") = "TRUE" then %>
<script language="JavaScript" src="/cfserverasp/source/chemdraw.js"></script>
<script>cd_includeWrapperFile("/cfserverasp/source/")</script>
<% end if %>
<title>Create or Edit a substance in <%=Application("appTitle")%></title>
</head>
<body onload="onLoadSwitch();">
<br><br><br>
<center>
<form name="form1" action method="POST">
	<input type="hidden" name="CompoundID" value="<%=CompoundID%>">
	<input type="hidden" name="ManageMode" value="<%=ManageMode%>">
	<input type="hidden" name="isEmptyStruc" value="1">
	<input type="hidden" name="inv_compounds.Density" value="<%=dbDensity%>">
	<table border="0" width="500" cellpadding="0" cellspacing="0">
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
				<table border="0" cellspacing="0" cellpadding="0" width="100%">
					<tr>
						<td align="right" nowrap>
							<span title="(eg. Acetonitrile)" class="required">Drug Name:</span>
						</td>
						<td align="left">
							<input type="text" name="inv_compounds.Substance_Name" value="<%=dBSubstanceName%>" size="60"> 
							<input type="hidden" name="<%=FNP%>Substance_Name_orig" value="<%=dBSubstanceName%>">
						</td>
					</tr>
				</table>
				<br>
			</td>
		  </tr>
		  <tr>
		  	<td rowspan="2">
		  		<img src="../graphics/pixel.gif" width="60" height="1" alt border="0">
		  	</td>
			<td width="300" valign="top">
				<table border="1" cellpadding="0" cellspacing="0" width="280" height="280">
					<tr>
						<td valign="top">
						<% if Session("isCDP") = "TRUE" then %>
							<input type="hidden" name="inv_compounds.Structure" value>
							<input type="hidden" name="inv_compounds.Structure.sstype" value="1">
							<%if bIsEdit then%>
								<input type="hidden" name="<%=FNP%>BASE64_CDX<%=CompoundID%>_orig" value="<%=InLineCdx%>">
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
							end if%>
							<script language="JavaScript">cd_insertObject("chemical/x-cdx", "280", "280", "mycdx", "<%=TempCdxPath%>mt.cdx", "False", "true", escape(document.all.inline.value),  "true", <%=ISISDraw%>)</script>
						</td>
					</tr>
				</table>		
			</td>
			<td valign="top">
				<table border="0">					
					<tr>
			        	<td>
							<span title="(eg. 50-50-0)" class="commercial_required">NDC:</span><br>
							<input type="text" name="inv_compounds.CAS" value="<%=dBCAS%>" SIZE="30" maxlength="11">
							<%if bIsEdit then %>
								<input type="hidden" name="<%=FNP%>CAS_orig" value="<%=dBCAS%>"> 
							<%end if%>
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="(eg. 50-50-0)">NDC for Investigational:</span><br>
							<input type="text" name="inv_compounds.NDC_INVESTIGATIONAL" value="<%=dBNDCInvestigational%>" SIZE="30" maxlength="11">
							<%if bIsEdit then %>
								<input type="hidden" name="<%=FNP%>NDC_INVESTIGATIONAL_orig" value="<%=dBNDCInvestigational%>"> 
							<%end if%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="(e.g. X1001545-9)">ABC Number:<br>
							<input type="text" name="inv_compounds.ACX_ID" value="<%=dBACX_ID%>" SIZE="30" value maxlength="15">
							<%if bIsEdit then %>
								<input type="hidden" name="<%=FNP%>ACX_ID_orig" value="<%=dBACX_ID%>"> 
							<%end if%>
						</td>
					</tr>
					<tr>
			        	<td>
							<span title="">Package Size:</span><br>
<%=ShowSelectBox3("inv_compounds.PACKAGE_SIZE", dbPackageSize, "SELECT SIZE_FK AS Value,  SIZE_FK AS DisplayText FROM CHEMACXDB.PACKAGESIZECONVERSION ORDER BY SIZE_FK ASC", 20, RepeatString(18, "&nbsp;"), "", "")%>
			
							<!--<input type="text" name="inv_compounds.PACKAGE_SIZE" value="<%=dBPackageSize%>" SIZE="30" maxlength="15"> -->

							<%if bIsEdit then %>
								<input type="hidden" name="<%=FNP%>PACKAGE_SIZE_orig" value="<%=dBPackageSize%>"> 
							<%end if%>
						</td>
		      		</tr>
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
						    response.Write ShowCustomPicklist(Key,"javascript:OnChangePicklist(this,'" & strLabel & "');")
						else
						    if ALT_ID_ClassDict.Exists(Key) then
						        Response.Write "<span class=""" & ALT_ID_ClassDict(Key) & """>"
						    else
						        if req_alt_ids_dict.Exists(Key) then
							        Response.Write "<span class=""required"">"
						        Else
    							    Response.Write "<span title="""">"
	    					    end if
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
        if Session("cfTab") = "" then Session("cfTab") = tabs_dict.Items()(0)
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
	        Set Tabx = TabView.Tabs.Add(key,key,"","","")
            if Tabx.Key = Session("cfTab") then
    	        Tabx.DHTML = "onclick=""return false;"""
            else
    	        Tabx.DHTML = "onclick=""postToSelf('" & Tabx.Key & "'); return false;"""
    	    end if
	        Tabx.DHTML = Tabx.DHTML & " onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"
	        Tabx.DHTML = Tabx.DHTML & " onmouseout=javascript:this.style.cursor='default'"
        Next
    	
	    ' Write out the HTML for the tabs
	    TabView.Show
        Response.Write vblf
    	
	    ' Clean up the TabView object
	    TabView.Unload
	    Set TabView = Nothing
        Response.Write " </td>" & vblf
        Response.Write "</tr>" & vblf
        
        ' Populate the tab
        nCol = 0
        for each key in custom_fields_info_dict
            if CustomField_GetCategory(key) = Session("cfTab") then
                if nCol = 0 then Response.Write "<tr>" & vblf & "<td>&nbsp;</td>" & vblf
                Response.Write "<td>" & vblf
				Response.write "<input type='checkbox'"
	            if custom_fields_value_dict.item(key) = "Yes" then
				    Response.write " checked"
				end if
	            if CustomField_GetReadonly(key) <> "0" then
				    Response.write " disabled"
				end if
				Response.write " name=""" & "cc" & key & """ size=60 value=""" & "Yes" & """>"
				if bIsEdit then 
					Response.write "<input type=hidden name=""" & FNP & "cc" & key & "_orig"" value=""" & "Yes" & """>"
				End if
	            Response.Write "<span>" & key & "</span>"
                Response.Write "</td>" & vblf
                if nCol = 1 then Response.Write "</tr>" & vblf
                nCol = (nCol + 1) Mod 2
            end if
        next
        if nCol = 1 then Response.Write "</tr>" & vblf
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

