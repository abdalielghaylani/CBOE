<%@ LANGUAGE=VBScript %>
<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
dbkey = "reg"
formgroup= "base_form_group"
if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"%>

<html>
<head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/csdo_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/chem_reg/source/app_vbs.asp"-->
</head>
<body>
<%action = request("action")
if Not action <> "" then%>
	<form action="ChemInfoUpdate_a67.asp?action=start_update" method="post">
		<%if Session("EDIT_COMPOUND_REG" & dbkey) = True then%>
			Click to begin update of MW2 and FORMULA2 fields in REGDB.COMPOUND_MOLECULE
			<p>
			<input type=submit value="Begin Update" id=submit1 name=submit1>
			<p>
			</form>
		<%else%>
			You do not have appropriate privileges for performing updates to registered compounds.
		<%end if%>

<%else
	Select Case UCase(action)

	Case "START_UPDATE"
			BeginUpdate dbkey, formgroup 
	Case "CANCEL"
			Response.Write "Update cancelled."
	End Select		
end if%>

<%

'Select all compound_molecule records where MW2 is NULL and begin update
Sub BeginUpdate(dbkey, formgroup)
	storeScriptTimeout= server.ScriptTimeout
	server.ScriptTimeout = 10000000
	on error resume next
	Set RS = Server.CreateObject("ADODB.Recordset")
	Set DataConn = GetNewConnection(dbkey, formgroup, "base_connection")
	DBMSUser_ID = Session("CurrentUser" & dbkey)
	UpdatePersonID = getValueFromTable(dbkey, formgroup,"People", "User_ID",DBMSUser_ID,"Person_ID")
	UpdateDate = Date()
		
	SQL = "SELECT * FROM COMPOUND_MOLECULE " &_
		" WHERE MW2 IS NULL OR FORMULA2 IS NULL"
	
	'Create recordset containing all records missing MW2 and FORMULA2
	RS.Open SQL, DataConn, adOpenKeyset, adLockOptimistic, adCmdText
	RecordCount = GetRecordCount(dbkey, formgroup, RS)
	if RecordCount > 0 then
			FlushMessageToClient("Update in Progress. This might take several minutes...")
			InitializeProgressBar false,"/chem_reg/config/oracle_install_scripts/ChemInfoUpdate_a67.asp?action=cancel"
			Counter = 0
			If Not (RS.EOF and RS.BOF) then
				RS.MoveFirst
				Do While NOT RS.EOF
					'Using MOL_ID from recordset, query the mst file via CSDO to get the Chemical information 
					MOL_ID = RS("MOL_ID")
					sql2 = "SELECT FORMULA,MOLWT FROM COMPOUND_MOLECULE WHERE MOL_ID=" & MOL_ID
					if Not isObject(Session("cnn")) then
						Set Session("cnn") = CSDOGetCSDOConnection(dbkey, formgroup)
					end if
					Set CSDO_RS = CSDODoCommand(sql2, dbkey, formgroup, "0", Session("cnn"))
					if not CSDO_RS.EOF then
						MOLWT = CSDO_RS.FIELDS("MOLWT")
						FORMULA = CSDO_RS.FIELDS("FORMULA")
						RS("MW2")=MOLWT
						RS("FORMULA2")= FORMULA
						RS("LAST_MOD_PERSON_ID") = UpdatePersonID
						RS("LAST_MOD_DATE") = UpdateDate
						RS.UPDATE
					else
						RS("MW2")=NULL
						RS("FORMULA2") = NULL
					end if
					If  err.number <> 0 then
						FlushMessageToClient("ERRORS: " & err.number & ":" & err.description)
						response.end
					end if
					Counter = counter + 1
					Progressbar counter, RecordCount, 1
					
					RS.MoveNext
					'response.Write "here" & counter
				Loop
				CloseRS(RS)
				CloseConn(DataConn)
				server.ScriptTimeout = storeScriptTimeout
				Set CSDO_RS= nothing
				CSDO_RS = ""
				FlushMessageToClient("Update complete.")
				Session("cnn") = ""
			End if
		else
			FlushMessageToClient("All records have MW2 and FORMULA2 populated. No update is necessary")
		end if


End sub

%>



</body>
</html>

