<%@ Language=VBScript %>
<%Response.Expires=0%>
<html>

<head>
	<title>Helper.asp</title>

	<script language= "javascript">
		theWindow = <%=Application("mainwindow")%>
		formmode = "<%=formmode%>"
		if ( "" == formmode ){
			formmode = theWindow.formmode
		}
	</script>
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<%
on error resume next
dbkey = Request.QueryString("dbname")

formmode= Request.QueryString("formmode")
formgroup=Request.QueryString("formgroup")
action = Request.QueryString("action")
if dbkey = "" then 
	dbkey = "reg"
end if

Select Case UCase(action)
	Case "GET_CHEMIST"
		theID = request.querystring("unique_id")
		rowid= Request.querystring("rowid")
		if theID <> "" then
			set DataConn = GetNewConnection(dbkey, formgroup,"base_connection")
			sql = "Select * from People Where Upper(user_code) ='" & Trim(UCase(theID)) & "'"
			set rs = DataConn.Execute(sql)
			if not (rs.eof and rs.bof)then
				theValue = rs("First_Name") & " " & rs("Middle_Name")& " " & rs("Last_Name")
				person_id = rs("person_id")
				closers(rs)

				if not formgroup = "base_form_group" then%>
					<script language="javascript">
						if (formmode.toLowerCase() == "edit_record") {
							top.frames['main'].document.cows_input_form.elements['UID.<%=rowid%>:Temporary_Structures.Scientist_ID'].value = "<%=person_id%>"
							top.frames['main'].document.cows_input_form.elements['Temporary_Structures.Chemist_Name'].value = "<%=theValue%>"
							theWindow.UpdateRelUpdateFields("UID.<%=rowid%>:Temporary_Structures.Scientist_ID")
							theWindow.UpdateTable_Row_IDS("UID.<%=rowid%>:Temporary_Structures.Scientist_ID")
							theWindow.UpdateTable_Names("UID.<%=rowid%>:Temporary_Structures.Scientist_ID")
						}
						else {
							if  ((formmode.toLowerCase()== "search") || (formmode.toLowerCase()== "add_record") || (formmode.toLowerCase()== "add_compounds") || (formmode.toLowerCase()== "add_experiments")) {
								top.frames['main'].document.cows_input_form.elements['Temporary_Structures.Scientist_ID'].value = "<%=person_id%>"
								top.frames['main'].document.cows_input_form.elements['Temporary_Structures.Chemist_Name'].value = "<%=theValue%>"
							}
						}
					</script>
				<%else%>
					<script language="javascript">
						if (formmode.toLowerCase() == "edit_record"){
							top.frames['main'].document.cows_input_form.elements['UID.<%=rowid%>:Batches.Scientist_ID'].value = "<%=person_id%>"
							top.frames['main'].document.cows_input_form.elements['Batches.Chemist_Name'].value = "<%=theValue%>"
							theWindow.UpdateRelUpdateFields("UID.<%=rowid%>:Batches.Scientist_ID")
							theWindow.UpdateTable_Row_IDS("UID.<%=rowid%>:Batches.Scientist_ID")
							theWindow.UpdateTable_Names("UID.<%=rowid%>:Batches.Scientist_ID")
						}
						else {
							if  ((formmode.toLowerCase()== "search") || (formmode.toLowerCase()== "add_record") || (formmode.toLowerCase()== "add_compounds") || (formmode.toLowerCase()== "add_experiments")){
								top.frames['main'].document.cows_input_form.elements['Batches.Scientist_ID'].value = "<%=person_id%>"
								top.frames['main'].document.cows_input_form.elements['Batches.Chemist_Name'].value = "<%=theValue%>"
							}
						}
					</script>
				<%end if%>
			<%else
				if not formgroup = "base_form_group" then%>
					<script language="javascript">
						if (formmode.toLowerCase() == "edit_record"){
							top.frames['main'].document.cows_input_form.elements['UID.<%=rowid%>:Temporary_Structures.Scientist_ID'].value = ""
							top.frames['main'].document.cows_input_form.elements['Temporary_Structures.Chemist_Code'].value = ""
							top.frames['main'].document.cows_input_form.elements['Temporary_Structures.Chemist_Name'].value = ""
							alert("invalid chemist code")
						}
						else {
							if  ((formmode.toLowerCase()== "search") || (formmode.toLowerCase()== "add_record") || (formmode.toLowerCase()== "add_compounds") || (formmode.toLowerCase()== "add_experiments")){
								top.frames['main'].document.cows_input_form.elements['UID.<%=rowid%>:Temporary_Structures.Scientist_ID'].value = ""
								top.frames['main'].document.cows_input_form.elements['Temporary_Structures.Scientist_Code'].value = ""
								top.frames['main'].document.cows_input_form.elements['Temporary_Structures.Scientist_Name'].value = ""
								alert("invalid chemist code")
							}
						}
					</script>
				<%else%>
					<script language="javascript">
						if (formmode.toLowerCase() == "edit_record"){
							top.frames['main'].document.cows_input_form.elements['UID.<%=rowid%>:Batches.Scientist_ID'].value = ""
							top.frames['main'].document.cows_input_form.elements['Batches.Chemist_Code'].value = ""
							top.frames['main'].document.cows_input_form.elements['Batches.Chemist_Name'].value = ""
							alert("invalid chemist code")
						}
						else{
							if  ((formmode.toLowerCase()== "search") || (formmode.toLowerCase()== "add_record") || (formmode.toLowerCase()== "add_compounds") || (formmode.toLowerCase()== "add_experiments")){
								top.frames['main'].document.cows_input_form.elements['Batches.Scientist_ID'].value = ""
								top.frames['main'].document.cows_input_form.elements['Batches.Chemist_Code'].value = ""
								top.frames['main'].document.cows_input_form.elements['Batches.Chemist_Name'].value = ""
								alert("invalid chemist code")
							}
						}
					</script>
				<%end if
			end if
			closeconn(dataconn)
		end if
	' end case "GET_CHEMIST"

	Case "GET_PRODUCER"
		theID = request.querystring("unique_id")
		rowid = request.querystring("rowid")
		if theID <> "" then
			set DataConn = GetNewConnection(dbkey, formgroup,"base_connection")
			sql = "Select * from People Where Upper(user_code) ='" & Trim(UCase(theID)) & "'"
			set rs = DataConn.Execute(sql)
			if not (rs.eof and rs.bof)then
				theValue = rs("First_Name") & " " & rs("Middle_Name")& " " & rs("Last_Name")
				person_id = rs("person_id")
				closers(rs)
				if not formgroup = "base_form_group" then%>
					<script language="javascript">
						if (formmode.toLowerCase() == "edit_record"){
							top.frames['main'].document.cows_input_form.elements['UID.<%=rowid%>:Temporary_Structures.Producer_ID'].value = "<%=person_id%>"
							top.frames['main'].document.cows_input_form.elements['Temporary_Structures.Producer_Name'].value = "<%=theValue%>"
						}
						else {
							if ( (formmode.toLowerCase()== "search") || (formmode.toLowerCase()== "add_record") || (formmode.toLowerCase()== "add_compounds") || (formmode.toLowerCase()== "add_experiments")){
								top.frames['main'].document.cows_input_form.elements['Temporary_Structures.Producer_ID'].value = "<%=person_id%>"
								top.frames['main'].document.cows_input_form.elements['Temporary_Structures.Producer_Name'].value = "<%=theValue%>"
							}
						}
					</script>
				<%else%>
					<script language="javascript">
						if (formmode.toLowerCase() == "edit_record"){
							top.frames['main'].document.cows_input_form.elements['UID.<%=rowid%>:Batches.Producer_ID'].value = "<%=person_id%>"
							top.frames['main'].document.cows_input_form.elements['Batches.Producer_Name'].value = "<%=theValue%>"
						}
						else {
							if  ((formmode.toLowerCase()== "search") || (formmode.toLowerCase()== "add_record") || (formmode.toLowerCase()== "add_compounds") || (formmode.toLowerCase()== "add_experiments")){
								top.frames['main'].document.cows_input_form.elements['Batches.Producer_ID'].value = "<%=person_id%>"
								top.frames['main'].document.cows_input_form.elements['Batches.Producer_Name'].value = "<%=theValue%>"
							}
						}
					</script>
				<%end if%>
			<%else
				if not formgroup = "base_form_group" then%>
					<script language="javascript">
						if (formmode.toLowerCase() == "edit_record"){
							top.frames['main'].document.cows_input_form.elements['UID.<%=rowid%>:Temporary_Structures.Producer_ID'].value = ""
							top.frames['main'].document.cows_input_form.elements['Temporary_Structures.Producer_Code'].value = ""
							top.frames['main'].document.cows_input_form.elements['Temporary_Structures.Producer_Name'].value = ""
							alert("invalid chemist code")
						}
						else{
							if  ((formmode.toLowerCase()== "search") || (formmode.toLowerCase()== "add_record") || (formmode.toLowerCase()== "add_compounds") || (formmode.toLowerCase()== "add_experiments")){
								top.frames['main'].document.cows_input_form.elements['Temporary_Structures.Producer_ID'].value = ""
								top.frames['main'].document.cows_input_form.elements['Temporary_Structures.Producer_Code'].value = ""
								top.frames['main'].document.cows_input_form.elements['Temporary_Structures.Producer_Name'].value = ""
								alert("invalid chemist code")
							}
						}
					</script>
				<%else%>
					<script language="javascript">
						if (formmode.toLowerCase() == "edit_record"){
							top.frames['main'].document.cows_input_form.elements['UID.<%=rowid%>:Batches.Producer_ID'].value = ""
							top.frames['main'].document.cows_input_form.elements['Batches.Producer_Code'].value = ""
							top.frames['main'].document.cows_input_form.elements['Batches.Producer_Name'].value = ""
							alert("invalid chemist code")
						}
						else{
							if  ((formmode.toLowerCase()== "search") || (formmode.toLowerCase()== "add_record") || (formmode.toLowerCase()== "add_compounds") || (formmode.toLowerCase()== "add_experiments")){
								top.frames['main'].document.cows_input_form.elements['Batches.Producer_ID'].value = ""
								top.frames['main'].document.cows_input_form.elements['Batches.Producer_Code'].value = ""
								top.frames['main'].document.cows_input_form.elements['Batches.Producer_Name'].value = ""
								alert("invalid chemist code")
							}
						}
					</script>
				<%end if
			end if
			closeconn(dataconn)

			if not formgroup = "base_form_group" then%>
				<script language="javascript">
					if(formmode.toUpperCase() ==  "EDIT_RECORD"){
						theWindow.UpdateRelUpdateFields("UID.<%=rowid%>:Temporary_Structures.Producer_ID")
						theWindow.UpdateTable_Row_IDS("UID.<%=rowid%>:Temporary_Structures.Producer_ID")
						theWindow.UpdateTable_Names("UID.<%=rowid%>:Temporary_Structures.Producer_ID")
					}
				</script>
			<%else%>
				<script language="javascript">
					if(formmode.toUpperCase() ==  "EDIT_RECORD"){
						theWindow.UpdateRelUpdateFields("UID.<%=rowid%>:Batches.Producer_ID")
						theWindow.UpdateTable_Row_IDS("UID.<%=rowid%>:Batches.Producer_ID")
						theWindow.UpdateTable_Names("UID.<%=rowid%>:Batches.Producer_ID")
					}
				</script>
			<%end if
		end if
	' end case "GET_PRODUCER"

	Case Else

end select
%>
</head>

<body  <%=Application("BODY_BACKGROUND")%>  bgProperties = "fixed" >
<p>&nbsp;</p>

</body>

</html>