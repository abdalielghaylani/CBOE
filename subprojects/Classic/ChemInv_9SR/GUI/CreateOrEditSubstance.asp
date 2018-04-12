<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/csdo_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/compound_utils_vbs.asp"-->
<%
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
Caption = "Enter substance attributes:"
if CompoundID > 0 then
	FNP = "UID." & CompoundID & ":" & "inv_compounds."
	Call GetSubstanceAttributesFromDb(CompoundID)
		
	bIsEdit = true
	Caption = "Edit substance attributes:"
	action = "edit"
Else
	dbSubstanceName = Request("inv_compounds.Substance_Name")
	dbStructure = Request("inv_compounds.Structure")
	dbCAS = Request("inv_compounds.CAS")
	dbACX_ID = Request("inv_compounds.ACX_ID")
	dbDensity = Request("inv_compounds.Density")
	dbALT_ID_1 = Request("inv_compounds.ALT_ID_1")
	dbALT_ID_2 = Request("inv_compounds.ALT_ID_2")
	dbALT_ID_3 = Request("inv_compounds.ALT_ID_3")
	dbALT_ID_4 = Request("inv_compounds.ALT_ID_4")
	dbALT_ID_5 = Request("inv_compounds.ALT_ID_5")
End If
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
<script LANGUAGE="javascript">
<!--
	var cd_plugin_threshold= <%=Application("CD_PLUGIN_THRESHOLD")%>;
	var blankb64 = "VmpDRDAxMDAEAwIBAAAAAAAAAAAAAAAAAAAAAAMAEAAAAENoZW1EcmF3IDYuMC4xCAAMAAAAbXl0ZXN0LmNkeAADMgAIAP///////wAAAAAAAP//AAAAAP////8AAAAA//8AAAAA/////wAAAAD/////AAD//wEJCAAAAFkAAAAEAAIJCAAAAKcCAAAXAgIIEAAAAAAAAAAAAAAAAAAAAAAAAwgEAAAAeAAECAIAeAAFCAQAAJoVAAYIBAAAAAQABwgEAAAAAQAICAQAAAACAAkIBAAAswIACggIAAMAYAC0AAMACwgIAAQAAADwAAMADQgAAAAIeAAAAwAAAAEAAQAAAAAACwARAAAAAAALABEDZQf4BSgAAgAAAAEAAQAAAAAACwARAAEAZABkAAAAAQABAQEABwABJw8AAQABAAAAAAAAAAAAAAAAAAIAGQGQAAAAAAJAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAEAhAAAAD+/wAA/v8AAAIAAAACAAABJAAAAAIAAwDkBAUAQXJpYWwEAOQEDwBUaW1lcyBOZXcgUm9tYW4BgAEAAAAEAhAAAAD+/wAA/v8AAAIAAAACAA8IAgABABAIAgABABYIBAAAACQAGAgEAAAAJAAAAAAA";

	window.focus();
	function getStrucData(){
		
		var b64 = cd_getData("mycdx", "chemical/x-cdx");
		//alert(b64)
		if (b64.length == 0){
			b64 = blankb64;
			document.form1.isEmptyStruc.value= "1";
		}
		
		document.form1["inv_compounds.Structure"].value = b64;
		
		//alert(b64)
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
<script language="JavaScript" src="/cfserverasp/source/chemdraw.js"></script>
<script>cd_includeWrapperFile("/cfserverasp/source/")</script>
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
							<span title="(eg. Acetonitrile)" class="required">Substance Name:</span>
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
				<table border="1" cellpadding="0" cellspacing="0">
					<tr>
						<td valign="top">
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
							%>
							<script language="JavaScript">cd_insertObject("chemical/x-cdx", "280", "280", "mycdx", "<%=TempCdxPath%>mt.cdx", "False", "true", escape(document.all.inline.value),  "true", <%=ISISDraw%>)</script>
						</td>
					</tr>
				</table>		
			</td>
			<td valign="top">
				<table border="0">
					
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
					<%
					
					For each key in alt_ids_dict
						Response.Write "<TR><td>" & vblf
						if req_alt_ids_dict.Exists(Key) then
							requiredFields =  requiredFields & "," & Key & ";0:" & alt_ids_dict.Item(Key)
							Response.Write "<span class=""required"">"
						Else
							Response.Write "<span title="""">"
						end if
						Response.Write alt_ids_dict.Item(Key) & "</span><br>"
						cKey = Replace(Key, "inv_compounds.","")
						
						Response.write "<input type=text name=""" & Key & """ size=30 value=""" & Eval("dB" & cKey) & """>"
						if bIsEdit then 
							Response.write "<input type=hidden name=""" & FNP & ckey & "_orig"" value=""" & Eval("dB" & cKey) & """>"
						End if
						Response.Write "</td></TR>" & vblf 
					Next 
					%>
		    	</table>
			</td>
		  </tr>
		  <tr>
			<td align="right" colspan="2">
				<%=GetCancelButton()& vblf%>
				<a HREF="Create%20a%20new%20substance" onclick="getStrucData(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" alt="Create a new substance for this container" WIDTH="61" HEIGHT="21"></a>
			</td>
		</tr>
		</table>
	
	<input type="hidden" name="return_location" value>
	<input type="hidden" name="no_gui" value="true">
	
</form>
</center>
</body>
</html>

