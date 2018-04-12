<%@ Language=VBScript %>

<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()
Response.Buffer = true%>
<!--#INCLUDE VIRTUAL = "/biosar_browser/reports/ReportUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/biosar_browser/reports/reports_app_js.js"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->


<%
Dim action
action = request("action")
dbkey = "biosar_browser"
formgroup = request("formgroup")
Getbiosar_browserSettingsConnection
profile_id = request("profile_id")
profile_name = request("profile_name")

action = request("action")
Dim ShowButtons
if UCase(action) = "EDIT" then
	title_text= "<h3>Change settings and click ok to update profile</h3>"
	submit_action = "update_profile"
	
	profile_id = request("profile_id")
	ShowButtons = true
end if
if UCase(action) = "NEW" then
	title_text= "<h3>Enter a name, description, change settings and click ok to create profile</h3>"
	submit_action = "new_profile"
	profile_id = "1"
	profile_id = request("profile_id")
	ShowButtons = true
end if 
if UCase(action) = "DUPLICATE" then
	title_text= "<h3>Enter a name, description, change settings and click ok to create profile</h3>"
	submit_action = "new_profile"
	ShowButtons = true
end if 
if UCase(action) = "VIEW" then
	ShowButtons = false
end if
if UCase(action) = "DELETE" then
	ShowButtons = false
end if

if UCase(action) = "DELETE_LOGO" then
	HideCancel = true
else
	HideCancel = false
end if



%>
<html>
<head>

<title> BioSAR Browser Report Profile</title>
</body>
<form action="/biosar_browser/reports/report_profile_action.asp?action=<%=submit_action%>&profile_id=<%=profile_id%>"  name=form1 method=post>
<table border="0" width="398" ID="Table2">
    <tr>
    <% if ShowButtons = true then%>
    <td><%=title_text%></td></tr><TR>
      <td width="300"><a href="javascript:doProfileSubmit(&quot;biosar_browser&quot;,&quot;<%=formgroup%>&quot;)"><img border="0"
      src="<%=Application("NavButtonGifPath") & "OK.gif"%>" width="61" height="21" alt="save query"></a>
    <%end if
    if HideCancel = false then%>
      <a href="javascript:window.close()"><img border="0"
      src="<%=Application("NavButtonGifPath") & "Cancel.gif"%>" width="61" height="21"
      alt="close window without saving"></a> </td>
    </tr>
    <%end if%>
  </table>
<table>
<%Select Case UCase(action)
Case "DELETE"
	Set Cmd = Server.CreateObject("ADODB.COMMAND")
	Set RS = Server.CreateObject("ADODB.RECORDSET")
	Cmd.ActiveConnection = RptConn
	Cmd.CommandType=adCmdText
	Cmd.CommandText = "delete from rptprofilesettings where profile_id=?"
	Cmd.Parameters.Append Cmd.CreateParameter("pProfile_name", 5, 1, 0, profile_id) 
	Cmd.Execute
	Cmd.CommandText = "delete from rptprofiles where profile_id=?"
	Cmd.Parameters.Append Cmd.CreateParameter("pProfile_name", 5, 1, 0, profile_id) 
	Cmd.Execute%>
	<script language="javascript">
	doCreateReport('biosar_browser','<%=formgroup%>')
	window.close()
	</script><%
	
	
Case "EDIT", "VIEW", "NEW","DUPLICATE"
	Set Cmd = Server.CreateObject("ADODB.COMMAND")
	Set RS = Server.CreateObject("ADODB.RECORDSET")
	Cmd.ActiveConnection = RptConn
	
	sql = "SELECT RptProfiles.Profile_name,RptProfiles.owner,RptProfiles.is_public,RptProfiles.BoilerPlateLine1,RptProfiles.BoilerPlateLine2,RptProfiles.BoilerPlateLine3,RptProfiles.BoilerPlateLine4,RptProfiles.BoilerPlateLine5,RptProfiles.Description as Profile_description, RptProfileSettings.Profile_Field_Name, RptProfileSettings.Value as SettingValue, RptProfile_RptSettingLkup.Profile_Field_Description as Field_Description, RptSettingLkup.Lkup_name,RptSettingLkup.Description as Setting_Description" &_
			" FROM RptProfiles, RptProfileSettings, RptProfile_RptSettingLkup, RptSettingLkup " &_
			" WHERE RptProfiles.Profile_id=RptProfileSettings.Profile_ID " &_
			" AND RptProfileSettings.Profile_Field_Name=RptProfile_RptSettingLkup.Profile_Field_Name " &_
			" AND RptProfile_RptSettingLkup.Lkup_Name=RptSettingLkup.Lkup_Name " &_
			" AND RptProfileSettings.Value=RptSettingLkup.Value  " &_
			" AND RptProfile_RptSettingLkup.Hide_From_GUI = 0 " &_
			" AND RptProfiles.Profile_id=? " &_
			" ORDER BY rptprofilesettings.id"
		

	Cmd.CommandText = sql
	Cmd.CommandType=adCmdText
	Cmd.Parameters.Append Cmd.CreateParameter("pProfile_name", 5, 1, 0, profile_id) 
	Set RS = Cmd.Execute
	'response.Write "<table>"
	if Not (RS.BOF and RS.EOF) then
		'response.Write "<tr><td><b>" & "Profile Description" & "</b></td><td> " & RS("Profile_description") & "</td></tr>"
		'response.Write "<tr><td><b>" & "Owner" & "</b></td><td> " & RS("owner") & "</td></tr>"
		'response.Write "<tr><td><b>" & "Public" & "</b></td><td>" & RS("is_public") & "</td></tr>"
		Set myDict = Server.CreateObject("Scripting.Dictionary")
		if UCase(action) = "NEW" or UCase(action) = "DUPLICATE" then
			profile_description = ""
			profile_owner = Session("username" & "biosar_browser")
			profile_name = ""
			profile_is_public = 1
		else
			profile_description = RS("Profile_description")
			profile_owner = RS("owner")
			profile_name = RS("Profile_Name")
			profile_is_public = RS("is_public")
		end if
		if UCase(action) = "VIEW" then
			if profile_is_public = "0" then
				profile_is_public = "No"
			else
				profile_is_public = "Yes"
			end if
		end if
		
		Do While Not RS.EOF
			
			setting_current_value = RS("SettingValue")
			if UCase(action) = "VIEW" then
				setting_current_value = RS("Setting_Description")
			end if
			setting_name = RS("Profile_Field_Name")
		
			lookup_name = RS("lkup_name")
			description = RS("Field_Description") 
			BoilerPlateLine1 = RS("BoilerPlateLine1") 
		
			BoilerPlateLine2 = RS("BoilerPlateLine2") 
			BoilerPlateLine3 = RS("BoilerPlateLine3") 
			BoilerPlateLine4 = RS("BoilerPlateLine4") 
			BoilerPlateLine5 = RS("BoilerPlateLine5") 
			'response.Write setting_name
			if not myDict.Exists(UCase(CStr(setting_name))) then
				myDict.Add UCase(CStr(setting_name)), description & "|" & lookup_name & "|" & setting_current_value
				'response.Write "<tr><td><b>" & RS("Field_Description") & "</b></td><td> " 
				'BuildInputBox setting_name,setting_current_value,lookup_name
				'response.Write "</td></tr>"
			end if
			RS.MoveNext
		loop
		RS.Close
	end if%>
	<table ><tr><td>
	<table>
	<input type = "hidden" name="currentlocation" value = "<%=request.querystring%>")
	<%if profile_name = "default" then%>
		<tr><th align = right width = "200">Profile Name</th><td width=250 align = left><%=profile_name%><input type="hidden" name="profile_name" value = "<%=profile_name%>"</td></tr>

	<%else%>
		<tr><th align = right width = "200">Profile Name</th><td width=250 align = left><%=createInputField("profile_name",profile_name, 50)%></td></tr>
	<%end if%>
	<tr><th align = right width = "200">Description</th><td width=250 align = left><%=createInputField("profile_description",profile_description, 50)%></td></tr>
	<tr><th align = right width = "200">Owner</th><td width=250 align = left><%=getFieldValue("owner",profile_owner )%></td></tr>
	<%if profile_name = "default" then%>
		<tr><th align = right width = "200">Public</th><td width=250 align = left>Yes</td></tr>
	<%else%>
		<tr><th align = right width = "200">Public</th><td width=250 align = left><%=createLookup("is_public",profile_is_public,"publicflag" )%></td></tr>
	<%end if%>
	
	</table>
	</td></tr></table>
	<table><tr><td>
		<table>
		<tr><th align = right width = "200"><%=getSettingData( MyDict,"right_margin", "DESCRIPTION")%></th><td width=80 align = right><%=createLookupFromDict(myDict,"right_margin")%></td></tr>
		<tr><th align = right width = "200"><%=getSettingData( MyDict,"left_margin", "DESCRIPTION")%></th><td width=80 align = right><%=createLookupFromDict(myDict,"left_margin")%></td></tr>
		<tr><th align = right width = "200"><%=getSettingData( MyDict,"top_margin", "DESCRIPTION")%></th><td width=80 align = right><%=createLookupFromDict(myDict,"top_margin")%></td></tr>
		<tr><th align = right width = "200"><%=getSettingData( MyDict,"bottom_margin", "DESCRIPTION")%></th><td width=80 align = right><%=createLookupFromDict(myDict,"bottom_margin")%></td></tr>
		</table>
		</td><td>
		<table>
		<tr><th align = right width = "200"><%=getSettingData( MyDict,"header_color", "DESCRIPTION")%></th><td width=80 align = right><%=createLookupFromDict(myDict,"header_color")%></td></tr>
		<tr><th align = right width = "200"><%=getSettingData( MyDict,"footer_color", "DESCRIPTION")%></th><td width=80 align = right><%=createLookupFromDict(myDict,"footer_color")%></td></tr>
		<tr><th align = right width = "200"><%=getSettingData( MyDict,"detail_color", "DESCRIPTION")%></th><td width=80 align = right><%=createLookupFromDict(myDict,"detail_color")%></td></tr>
	</table>
	</td></tr></table>
	<table ><tr><td>
	<table><tr><td ></td>
	<tr><th align = right width = "200">Logo</th><td width=80 align = right><%=createLookupFromDict(myDict,"rpt_logo")%>
	<%if (UCase(action) = "EDIT" or UCase(action) = "DUPLICATE" or UCase(action) = "NEW") then%><script language="javascript">getLogoLinks()</script></td></tr><%end if%>
	<tr><th align = right width = "200">Header Text Line 1</th><td width=300 align = right><%=createInputField("BoilerPlateLine1",BoilerPlateLine1, 100)%></td></tr>
	<tr><th align = right width = "200">Header Text Line 2</th><td width=300 align = right><%=createInputField("BoilerPlateLine2",BoilerPlateLine2, 100)%></td></tr>
	<tr><th align = right width = "200">Header Text Line 3</th><td width=300 align = right><%=createInputField("BoilerPlateLine3",BoilerPlateLine3, 100)%></td></tr>
	<tr><th align = right width = "200">Header Text Line 4</th><td width=300 align = right><%=createInputField("BoilerPlateLine4",BoilerPlateLine4, 100)%></td></tr>
	<tr><th align = right width = "200">Header Text Line 5</th><td width=300 align = right><%=createInputField("BoilerPlateLine5",BoilerPlateLine5, 100)%></td></tr>
	</table>
	</td></tr></table>
	<table ><tr><td>
	<table>
	<tr><td width = "150"></td><th align=right>Font</th><th align=right>Size</th><th align=right>Weight</th><th align=right>Color</th><th align=right>Underline</th><th align=right>Italic</th><th align=right>Background Color</th></td></tr>
	<tr></tr><th align = right width = "200">Report Name</th><td width=80 align = right><%=createLookupFromDict(myDict,"Report_Name_FontName")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"Report_Name_FontSize")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"Report_Name_FontWeight")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"Report_Name_FontColor")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"Report_Name_FontUL")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"Report_Name_FontItalic")%></td><td width=100 align = right><%=createLookupFromDict(myDict,"Report_Name_BackColor")%></td></tr>
	<tr></tr><th align = right width = "200">Report Footer</th><td width=80 align = right><%=createLookupFromDict(myDict,"Report_Footer_FontName")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"Report_Footer_FontSize")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"Report_Footer_FontWeight")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"Report_Footer_FontColor")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"Report_Footer_FontUL")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"Report_Footer_FontItalic")%></td><td width=100 align = right><%=createLookupFromDict(myDict,"Report_Footer_BackColor")%></td></tr>
	<tr></tr><th align = right width = "200">Base Table Name</th><td width=80 align = right><%=createLookupFromDict(myDict,"BT_Name_FontName")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BT_Name_FontSize")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BT_Name_FontWeight")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BT_Name_FontColor")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BT_Name_FontUL")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BT_Name_FontItalic")%></td><td width=100 align = right><%=createLookupFromDict(myDict,"BT_Name_BackColor")%></td></tr>
	<tr></tr><th align = right width = "200">Base Table Field Names</th><td width=80 align = right><%=createLookupFromDict(myDict,"BT_Fields_FontName")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BT_Fields_FontSize")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BT_Fields_FontWeight")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BT_Fields_FontColor")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BT_Fields_FontUL")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BT_Fields_FontItalic")%></td><td width=100 align = right><%=createLookupFromDict(myDict,"BT_Fields_BackColor")%></td></tr>
	<tr></tr><th align = right width = "200">Base Table Data</th><td width=80 align = right><%=createLookupFromDict(myDict,"BT_Data_FontName")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BT_Data_FontSize")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BT_Data_FontWeight")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BT_Data_FontColor")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BT_Data_FontUL")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BT_Data_FontItalic")%></td><td width=100 align = right><%=createLookupFromDict(myDict,"BT_Data_BackColor")%></td></tr>
	<tr></tr><th align = right width = "200">Child Table Name</th><td width=80 align = right><%=createLookupFromDict(myDict,"CT_Name_FontName")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"CT_Name_FontSize")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"CT_Name_FontWeight")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"CT_Name_FontColor")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"CT_Name_FontUL")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"CT_Name_FontItalic")%></td><td width=100 align = right><%=createLookupFromDict(myDict,"CT_Name_BackColor")%></td></tr>
	<tr></tr><th align = right width = "200">Child Table Field Names</th><td width=80 align = right><%=createLookupFromDict(myDict,"CT_Fields_FontName")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"CT_Fields_FontSize")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"CT_Fields_FontWeight")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"CT_Fields_FontColor")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"CT_Fields_FontUL")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"CT_Fields_FontItalic")%></td><td width=100 align = right><%=createLookupFromDict(myDict,"CT_Fields_BackColor")%></td></tr>
	<tr></tr><th align = right width = "200">Child Table Data</th><td width=80 align = right><%=createLookupFromDict(myDict,"CT_Data_FontName")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"CT_Data_FontSize")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"CT_Data_FontWeight")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"CT_Data_FontColor")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"CT_Data_FontUL")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"CT_Data_FontItalic")%></td><td width=100 align = right><%=createLookupFromDict(myDict,"CT_Data_BackColor")%></td></tr>
	<tr></tr><th align = right width = "200">Header Text Line 1</th><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L1_FontName")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L1_FontSize")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L1_FontWeight")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L1_FontColor")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L1_FontUL")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L1_FontItalic")%></td><td width=100 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L1_BackColor")%></td></tr>
	<tr></tr><th align = right width = "200">Header Text Line 2</th><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L2_FontName")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L2_FontSize")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L2_FontWeight")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L2_FontColor")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L2_FontUL")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L2_FontItalic")%></td><td width=100 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L2_BackColor")%></td></tr>
	<tr></tr><th align = right width = "200">Header Text Line 3</th><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L3_FontName")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L3_FontSize")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L3_FontWeight")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L3_FontColor")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L3_FontUL")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L3_FontItalic")%></td><td width=100 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L3_BackColor")%></td></tr>
	<tr></tr><th align = right width = "200">Header Text Line 4</th><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L4_FontName")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L4_FontSize")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L4_FontWeight")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L4_FontColor")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L4_FontUL")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L4_FontItalic")%></td><td width=100 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L4_BackColor")%></td></tr>
	<tr></tr><th align = right width = "200">Header Text Line 5</th><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L5_FontName")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L5_FontSize")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L5_FontWeight")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L5_FontColor")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L5_FontUL")%></td><td width=80 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L5_FontItalic")%></td><td width=100 align = right><%=createLookupFromDict(myDict,"BoilerPlate_L5_BackColor")%></td></tr>

	</table>
	</td></tr></table>
	<input type = "hidden" name = "header_template" value="generic_on_open_template">
	</form>
	
	<%Case "UPDATE_PROFILE"
		
		UpdateSettings(request("profile_id"))
		
	Case "NEW_PROFILE"
		CreateNewProfile()
		
	Case "ADD_LOGO"
	
	%><script language="javascript">
	window.reload()
	</script><%
	Case "DELETE_LOGO"
		logoname=request("logo_name")
		lastlocation = request("currentlocation")
		Set Cmd = Server.CreateObject("ADODB.COMMAND")
		Set RS = Server.CreateObject("ADODB.RECORDSET")
		Cmd.ActiveConnection = RptConn
		Cmd.CommandType=adCmdText
		sql = "select * from  RptProfileSettings where value = '" & logoname & "'"
		Cmd.CommandText = sql
		RS.Open cmd
		if Not (RS.EOF and RS.BOF) then
			bUsed = true
			RS.Close
		else
			bUsed = false
		end if
		if bUsed = false then
			sql = "delete from RptSettingLkup where lkup_name = 'rptlogo' and value ='" & logoname & "'"
			Cmd.CommandText = sql
			Cmd.execute
		%><script language="javascript">
			document.location.href="/biosar_browser/reports/report_profile.asp?<%=lastlocation%>"
		</script><%	
		else
		%><script language="javascript">
			alert("logo is used by one or more profiles and cannot be deleted")
			document.location.href="/biosar_browser/reports/report_profile.asp?<%=lastlocation%>"
		</script><%	
		end if
	End Select

Sub UpdateSettings(profile_id)

	Set RS = Server.CreateObject("adodb.recordset")
	Set Cmd = Server.CreateObject("ADODB.COMMAND")
	Cmd.ActiveConnection = RptConn
	Cmd.CommandType=adCmdText
	
	RS.Open "select * from rptprofiles where profile_id=" & profile_id, RptConn, adOpenKeyset,  adLockOptimistic, adCmdText 
	
	
	RS("profile_name") = request("Profile_name")
	RS("description") = request("profile_description")
	RS("is_public") = request("is_public")
	RS("BoilerPlateLine1") = request("BoilerPlateLine1")
	
	RS("BoilerPlateLine2") = request("BoilerPlateLine2")
	RS("BoilerPlateLine3") = request("BoilerPlateLine3")
	RS("BoilerPlateLine4") = request("BoilerPlateLine4")
	RS("BoilerPlateLine5") = request("BoilerPlateLine5")
	RS.Update
	RS.Close
	on error resume next
	Set RS2 = Server.CreateObject("adodb.recordset")
	Set Cmd2 = Server.CreateObject("ADODB.COMMAND")
	Cmd2.ActiveConnection = RptConn
	Cmd2.CommandType=adCmdText
	Cmd2.CommandText = "select profile_field_name from RptProfile_RptSettingLkup"
	Set RS2 = Cmd2.Execute
	
	if Not (RS2.EOF and RS2.BOF) then
	
		Do While Not RS2.EOF
			field_name = RS2("profile_field_name") 
			
			if (not UCase(field_name) = "PROFILE_NAME") and (not  UCase(field_name) = "DESCRIPTION") then
				field_value = request(field_name)
				Cmd.CommandText = "update  RptProfileSettings set [value]='" & field_value & "' where profile_id=" & profile_id & "AND  profile_field_name='" & field_name & "'"
				Cmd.Execute
			end if
			RS2.MoveNext
		Loop
	end if
	RS2.close
End Sub

Sub CreateNewProfile()

	Set RS = Server.CreateObject("adodb.recordset")
	Set Cmd = Server.CreateObject("ADODB.COMMAND")
	Cmd.ActiveConnection = RptConn
	Cmd.CommandType=adCmdText
	
	RS.Open "rptprofiles", RptConn, adOpenKeyset,  adLockOptimistic, adCmdTable 
	
	RS.AddNew
	RS("profile_name") = request("Profile_name")
	RS("description") = request("profile_description")
	RS("owner") = Session("UserName" & "biosar_browser")
	RS("is_public") = request("is_public")
	RS("BoilerPlateLine1") = request("BoilerPlateLine1")
	RS("BoilerPlateLine2") = request("BoilerPlateLine2")
	RS("BoilerPlateLine3") = request("BoilerPlateLine3")
	RS("BoilerPlateLine4") = request("BoilerPlateLine4")
	RS("BoilerPlateLine5") = request("BoilerPlateLine5")
	RS.Update
	profile_id = RS("Profile_ID")
	RS.Close
	on error resume next
	Set RS2 = Server.CreateObject("adodb.recordset")
	Set Cmd2 = Server.CreateObject("ADODB.COMMAND")
	Cmd2.ActiveConnection = RptConn
	Cmd2.CommandType=adCmdText
	Cmd2.CommandText = "select profile_field_name from RptProfile_RptSettingLkup"
	Set RS2 = Cmd2.Execute
	if Not (RS2.EOF and RS2.BOF) then
	
		Do While Not RS2.EOF
			field_name = RS2("profile_field_name") 
			if (not UCase(field_name) = "PROFILE_NAME") and (not  UCase(field_name) = "DESCRIPTION") then
				field_value = request(field_name)
				
				Cmd.CommandText = "insert into RptProfileSettings(Profile_id,profile_field_name,[value])values('" & profile_id & "','" & field_name & "','" & field_value & "')"
				Cmd.Execute
			end if
			RS2.MoveNext
		Loop
	end if
	
	RS2.close
End Sub
Function getSettingData(byRef MyDict,setting_name, setting_type)

	value = myDict.Item(UCase(CStr(setting_name)))

	if value <> "" then
	temp = split(value, "|", -1)
	on error resume next
	Select Case UCase(setting_type)
	Case "DESCRIPTION"
		theReturn = temp(0)
	Case "LOOKUP_NAME"
		theReturn = temp(1)
	Case "CURRENT_VALUE"
		theReturn = temp(2)
	End Select
	
	end if
	getSettingData = theReturn
End Function

Function createInputFieldFromDict(byRef MyDict,setting_name, setting_type, width)
	if UCase(action) = "VIEW" then
		theReturn = theValue
	else
		thevalue=getSettingData(MyDict,setting_name,setting_type)
		theReturn = "<input type=text name=" & setting_name  & " size=" & width & " value=""" & thevalue & """>"
	end if
	createInputFieldFromDict = theReturn
End Function

Function createLookup(setting_name,current_value,lookup_value)
	if UCase(action) = "VIEW" then
		theReturn=current_value
	else
		theReturn=ShowSelectBox(setting_name,current_value, "SELECT Description AS DisplayText,Value  as [value] FROM RptSettingLkup Where lkup_name = '" & lookup_value & "'")
	end if
	createLookup = theReturn
End function

Function createLookupFromDict(byRef MyDict,setting_name)
on error resume next
	current_value=getSettingData(MyDict,setting_name,"CURRENT_VALUE")
	if UCase(action) = "VIEW" then
		theReturn = current_value
	else
	lookup_value=getSettingData(MyDict,setting_name,"LOOKUP_NAME")
	
	theReturn=ShowSelectBox(setting_name,current_value, "SELECT Description AS DisplayText,Value  as [value] FROM RptSettingLkup Where lkup_name = '" & lookup_value & "'")
	end if
	createLookupFromDict = theReturn
	
End Function

Function createInputField(setting_name, current_value, width)
	if UCase(action) = "VIEW" then
		theReturn = current_value
	else
		theReturn = "<input type=text name=" & setting_name & " size=" & width & " value=""" & current_value & """>"
	end if
	createInputField = theReturn
End Function

Function getFieldValue(setting_name, current_value)
		theReturn = current_value
		getFieldValue = theReturn
End Function


%>
</table>
</form>
</html>
</body>
