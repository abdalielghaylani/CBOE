<!--#INCLUDE VIRTUAL = "/cfserverasp/source/form_action_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/DrugDeg/source/app_vbs.asp"-->

<!--#INCLUDE FILE = "DrugDeg_action_functions.asp"-->
<%custom_action = request( "dataaction2" )%>

<%
'ShowMessageDialog( "Entering DrugDeg_action.asp" )

' I want to limit case differences, so I'll create a lower-case version of the formgroup.
sFormGroup_LowerCase = LCase( formgroup )
'logaction ( " does this have a custom action: " & custom_action )
'logaction ( "sFormGroup_LowerCase " & sFormGroup_LowerCase)
Select Case UCase( custom_action )
	Case "UPDATE_DRUGDEG"
		select case sFormGroup_LowerCase
			case "adddegradant_form_group"
				if session("wizard") = "wiz_edit_deg_1" then
					session("wizard") = "wiz_edit_deg_2"
				end if
				Set DataConn = GetConnection( dbkey, formgroup, "DRUGDEG_PARENTS" )
				new_deg_fgroups = request("current_fgroups_hidden")
				theReturn = DoAddDegFGroup(dbkey, formgroup, DataConn, new_deg_fgroups, Session( "PrimaryKey" & dbkey ))
							
				'Calculate MW difference if there is no r-group in the parent and degradant then update the record
				theReturn = DoUpdateMWDifference(dbkey, formgroup, DataConn, Session( "PrimaryKey" & dbkey ))
				
				Session( "Base_RS" & dbkey & formgroup ) = StoredRS

				
				if inStr( Session( "ReturnToExperimentDetails" & dbkey ), "dbname" ) <= 0 then
					' something strange happened, rebuild the url
					Session( "ReturnToExperimentDetails" & dbkey ) = "/" & Application("Appkey") & "/drugdeg/Experiment_details_form.asp?dbname="& dbkey &"&formgroup=AddExperiment_form_group&formmode=edit&formgroupflag_override=Experiment_details&keyexpt=" & request("DEG_EXPT_FK")
				end if

				if inStr( Session( "ReturnToExperimentDetails" & dbkey ), "keyexpt" ) <= 0 then
					' "keyexpt" wasn't in the query string.  Put it in.
					Session( "ReturnToExperimentDetails" & dbkey ) = Session( "ReturnToExperimentDetails" & dbkey ) & "&keyexpt=" & request("DEG_EXPT_FK")
				end if

				' Send control over to the experiment details display.
				'logaction( "current " & Session( "CurrentLocation" & dbkey & formgroup ) )
				'logaction( "return to exp." & Session( "ReturnToExperimentDetails" & dbkey ) )
				if Session( "ReturnToExperimentDetails" & dbkey ) = "" then
					Session( "ReturnToExperimentDetails" & dbkey ) = "/" & Application("Appkey") & "/drugdeg/Experiment_details_form.asp?dbname="& dbkey &"&formgroup=AddExperiment_form_group&formmode=edit&formgroupflag_override=Experiment_details&keyexpt=" & request("DEG_EXPT_FK")
				end if
				' Make sure the page reloads when you get to the experiment details.
				sNewPath = MakeQueryStringBeDifferent( Session( "ReturnToExperimentDetails" & dbkey ) )
				Session( "ReturnToExperimentDetails" & dbkey ) = sNewPath

							
							
				
							
				'logaction("return to experiment details" & Session("ReturnToExperimentDetails" & dbkey))
				DoRedirect dbkey, Session( "ReturnToExperimentDetails" & dbkey )
		end select
	Case "SEND_EMAIL"
		'the record was already updated in form_Action_vbs but we need to send an email if the status has changed.
		select case sFormGroup_LowerCase
			case "base_form_group"
				set r = Request.Form
				'logaction ("addparent_form_group")
				'logaction ("old_Status " & request("old_Status"))
				
				sStatus = request("UID." & Session("Primarykey" &dbkey) & ":DRUGDEG_PARENTS.STATUS")
				
				'logaction ("new status " & sStatus)
				if request("old_Status") <> sStatus then
				'send email
					bSendStatusMail = SendStatusMail ( sStatus,  sFormGroup_LowerCase )
					'logaction ("send email")
				end if
			case "addexperiment_form_group"
				sStatus = request("UID." & Session("Primarykey" &dbkey) & ":DRUGDEG_EXPTS.STATUS")
				if request("old_Status") <> sStatus then
					'logaction ("send email")
					bSendStatusMail = SendStatusMail ( sStatus, sFormGroup_LowerCase )
				end if
		end select
			
	Case "ADD_DRUGDEG"
	
		' We start out with the presumption that the new record _should_ be added.
		bAddNewRecord = true

		' If we are adding a parent compound we need to grab the salt string now.  COWS doesn't
		' seem to handle checkboxes well, so I will have to add the salt string myself.
		if "addparent_form_group" = sFormGroup_LowerCase then
			sSalts = Request.Form( "DRUGDEG_PARENTS.SALT" )
			
			' Response.Write( "Salts from form = &quot;" & sSalts & "&quot;<br>" )

			' Protect any apostrophes in the text.
			sSalts = CleanString_ProtectApostrophes( sSalts )
			
		end if


		' First let's determine whether we are actually to add the new record.
		select case sFormGroup_LowerCase
			case  "addparent_form_group"
				' Validate the new parent.

					bAddNewRecord = true

			case  "salt_form_group"
				sSaltOperation = Request.Form( "Operation" )
				if "ADD" = sSaltOperation then
					' The user is trying to add a new salt.  Get the code and name given
					' for the new salt.
					sSaltCode = Request.Form( "DRUGDEG_SALTS.SALT_CODE" )
					sSaltName = Request.Form( "DRUGDEG_SALTS.SALT_NAME" )

					' Protect apostrophes in the salt name.
					sSaltName = CleanString_ProtectApostrophes( sSaltName )

					if true <> SaltCodeIsValid( sSaltCode ) then
						' The salt code is not valid.  We will not be adding a new salt record
						' and we did not modify an existing one.
						bAddNewRecord = false
						bModifiedRecord = false
					elseif SaltCodeIsAlreadyInDatabase( sSaltCode, dbkey, formgroup ) then
						' The salt code is already in the database.  We will not be adding a
						' new salt record and we did not modify an existing one.
						bAddNewRecord = false
						bModifiedRecord = false
						' Tell the user about this problem.
						sAlert = "There is already a salt with that code (" & sSaltCode & ").  " & _
							"No new salt shall be added to the database."
						ShowMessageInAlertDialog( sAlert )
					elseif not SaltNameIsValid( sSaltName ) then
						' The salt code is not valid.  We will not be adding a new salt record
						' and we did not modify an existing one.
						bAddNewRecord = false
						bModifiedRecord = false
					elseif SaltNameIsAlreadyInDatabase( sSaltName, dbkey, formgroup ) then
						' The salt name is already in the database.  We will not be adding a
						' new salt record and we did not modify an existing one.
						bAddNewRecord = false
						bModifiedRecord = false
						' Tell the user about this problem.
						sAlert = "There is already a salt with that name (" & sSaltName & ").  " & _
							"No new salt shall be added to the database."
						ShowMessageInAlertDialog( sAlert )
					else
						' The code and name are valid and the code is a new one, so we will
						' be adding a new record and not modifying an existing one.
						bAddNewRecord = true
						bModifiedRecord = false
					end if  ' several validity checks
				elseif "MODIFY" = sSaltOperation then
					' We are modifying a salt name.  Get the salt's key and the new name.
					sSaltKey = Request.Form( "SaltKey" )
					sNewSaltName = Request.Form( "DRUGDEG_SALTS.SALT_NAME" )

					' Save an unmodified form of the salt name for output purposes, then
					' protect apostrophes in the salt name.
					sNewSaltName_Input = sNewSaltName
					sNewSaltName = CleanString_ProtectApostrophes( sNewSaltName )

					if SaltNameIsAlreadyInDatabase( sNewSaltName, dbkey, formgroup ) then
						' The salt name is already in the database.  We shall not have two salts
						' with the same name.  Do not modify a record, do not add a record.
						bAddNewRecord = false
						bModifiedRecord = false

						' Tell the user about this problem.
						sAlert = "There is already a salt with that name (" & _
							sNewSaltName_Input & ").  No two salts may have the same name."
						ShowMessageInAlertDialog( sAlert )
					else
						' Change the name of that salt.
						ChangeNameOfDatabaseSalt sSaltKey, sNewSaltName, dbkey, formgroup

						' We aren't adding a new record to the database.
						bAddNewRecord = false
						' We _did_ modify an existing record.
						bModifiedRecord = true

						' Clear the field for the name.
%><script language="javascript">
	//clearAllFields()
</script><%
					end if  ' if the name is already in the database ...
				end if  ' sSaltOperation
			' end of case  "salt_form_group"

			case  "condition_form_group"
				sCondText = Request.Form( "DRUGDEG_CONDS.DEG_COND_TEXT" )

				' Save an unmodified form of the condition text for output purposes, then
				' protect apostrophes in the condition text.
				sCondText_Input = sCondText
				sCondText = CleanString_ProtectApostrophes( sCondText )

				' Find out what operation we are performing here.
				sCondOperation = Request.Form( "Operation" )

				if "ADD" = sCondOperation then
					' The user is trying to add a new degradation condition.  If the
					' degradation condition text is not already in the database, we
					' add a new record.
					if true = CondTextIsAlreadyInDatabase( sCondText, dbkey, formgroup ) then
						' The text entered is already in the database.  We are not going
						' to add a new record for that.
						bAddNewRecord = false
						' Tell the user about the problem.
						sAlert = "The degradation condition, '" & sCondText_Input & _
							"', is already in the database so it shall not be added."
						ShowMessageInAlertDialog( sAlert )
					else
						' The text entered is not already in the database so add it.
						bAddNewRecord = true
					end if
					' In any event we aren't modifying a record.
					bModifiedRecord = false
				elseif "MODIFY" = sCondOperation then
					' We are modifying the text of an existing degradation condition.
					' Get the key for the condition.
					sCondKey = Request.Form( "DegCondKey" )

					if true = CondTextIsAlreadyInDatabase( sCondText, dbkey, formgroup ) then
						' The text entered is already in the database.  We do not allow two
						' conditions with the same description text, so no modifying the
						' record and no adding a new record.
						bAddNewRecord = false
						bModifiedRecord = false

						' Tell the user about the problem.
						sAlert = "The degradation condition, '" & sCondText_Input & _
							"', is already in the database.  No two condition descriptions may be the same."
						ShowMessageInAlertDialog( sAlert )
					else
						' Change the name of that condition to the entered name.
						ChangeNameOfDatabaseCondition sCondKey, sCondText, dbkey, formgroup

						' We aren't adding a new record to the database.
						bAddNewRecord = false
						' We _did_ modify an existing record.
						bModifiedRecord = true

						' Clear the field for the name.
%><script language="javascript">
	//clearAllFields()
</script><%
					end if  ' if the condition text is already in the database ...
				end if  ' sCondOperation
				
				
				
			case  "status_form_group"
				sStatusText = Request.Form( "DRUGDEG_STATUSES.STATUS_TEXT" )

				' Save an unmodified form of the condition text for output purposes, then
				' protect apostrophes in the condition text.
				sStatusText_Input = sStatusText
				sStatusText = CleanString_ProtectApostrophes( sStatusText )

				' Find out what operation we are performing here.
				sStatusOperation = Request.Form( "Operation" )

				if "ADD" = sStatusOperation then
					' The user is trying to add a new degradation condition.  If the
					' degradation condition text is not already in the database, we
					' add a new record.
					if true = StatusTextIsAlreadyInDatabase( sStatusText, dbkey, formgroup ) then
						' The text entered is already in the database.  We are not going
						' to add a new record for that.
						bAddNewRecord = false
						' Tell the user about the problem.
						sAlert = "The status , '" & sStatusText_Input & _
							"', is already in the database so it shall not be added."
						ShowMessageInAlertDialog( sAlert )
					else
						' The text entered is not already in the database so add it.
						bAddNewRecord = true
					end if
					' In any event we aren't modifying a record.
					bModifiedRecord = false
				elseif "MODIFY" = sStatusOperation then
					' We are modifying the text of an existing degradation condition.
					' Get the key for the condition.
					sStatusKey = Request.Form( "StatusKey" )

					if true = StatusTextIsAlreadyInDatabase( sStatusText, dbkey, formgroup ) then
						' The text entered is already in the database.  We do not allow two
						' conditions with the same description text, so no modifying the
						' record and no adding a new record.
						bAddNewRecord = false
						bModifiedRecord = false

						' Tell the user about the problem.
						sAlert = "The status, '" & sStatusText_Input & _
							"', is already in the database.  No two statuses may be the same."
						ShowMessageInAlertDialog( sAlert )
					else
						' Change the name of that condition to the entered name.
						ChangeNameOfDatabaseStatus sStatusKey, sStatusText, dbkey, formgroup

						' We aren't adding a new record to the database.
						bAddNewRecord = false
						' We _did_ modify an existing record.
						bModifiedRecord = true

						' Clear the field for the name.
%><script language="javascript">
	//clearAllFields()
</script><%
					end if  ' if the status text is already in the database ...
				end if  ' sStatusOperation
			' end of case  "status_form_group"
		end select
		
		' Set up some variables we'll be needing, add or not.
		SetSessionVars dbkey, formgroup, "add_record"
		GetSearchData dbkey, formgroup
		AddOrder = Request( "Add_Order" )
		showErrors = False
		'table_names = Session( "strWhereSubforms" & dbkey ) & ",DRUGDEG_BASE64"
		table_names = Session( "strWhereSubforms" & dbkey )
		'logaction("table_names " & table_names)
		field_names = Session( "SearchData" & "Relational" & dbkey & formgroup )
		'logaction("SearchData" & "Relational" & dbkey & formgroup)
		'can't figure out why the SALT file is never seen in the submitted fields.
		if "addparent_form_group" = sFormGroup_LowerCase then
			if field_names <> "" then
				if Not 0 < inStr( field_names, "DRUGDEG_PARENTS.SALT;0" ) then
					field_names = field_names & "," & "DRUGDEG_PARENTS.SALT;0"
				end if
				if Not 0 < inStr( field_names, "DRUGDEG_PARENTS.STATUS;0" ) then
					field_names = field_names & "," & "DRUGDEG_PARENTS.STATUS;0"
				end if
			else 
				field_names = "DRUGDEG_PARENTS.SALT;0,DRUGDEG_PARENTS.STATUS;0"
			end if
		end if
		
		if "adddegradant_form_group" = sFormGroup_LowerCase then
			if Not 0 < inStr( field_names, "DRUGDEG_DEGS.DEG_EXPT_FK;1" ) then
				field_names = field_names & "," & "DRUGDEG_DEGS.DEG_EXPT_FK;1"
			end if
		end if
		
		if "addexperiment_form_group" = sFormGroup_LowerCase then
			if Not 0 < inStr( field_names, "DRUGDEG_EXPTS.STATUS;0" ) then
				field_names = field_names & "," & "DRUGDEG_EXPTS.STATUS;0"
			end if
		end if
		
		if "" <> AddOrder then
			table_names = AddOrder
			AddType = "CASCADE"
		else
			AddType = ""
		end if
		StoredRS = Session( "Base_RS" & dbkey & formgroup )

	
		if true = bAddNewRecord then
			' Add the record.
			'stop
			
			'we only need to check if we are a parent
			'duplicates are allowed for mechanisms and degradants
			if sFormGroup_LowerCase = "addparent_form_group" then
			
				'this only tells you if it is in the base64 table
				isdup  = DoCartridgeDuplicateSearch(dbkey, formgroup, "DRUGDEG_BASE64", Request("DRUGDEG_BASE64.Structure"), "", "")
			
				'if there is a list we want to create an array to loop through....
				if isdup <> "" then
				
					'unfortunately since dups are allowed for degradants we could have a list
					isduparr = split(isdup,",")
				
					'now we will actually set isdup="" to reset back
					isdup = ""
					for duparr=0 to ubound(isduparr)
						'so now see if it is a parent compound via a join
						
							ispardupSQL = "select count(*) as thecount from drugdeg_base64 inner join  drugdeg_parents  on drugdeg_base64.mol_id =  drugdeg_parents.mol_id where drugdeg_base64.base64_id = '" & isduparr(duparr) & "'"
							Set connChem = GetConnection( dbkey, formgroup, "DRUGDEG_PARENTS" )
							Set rsDupcheck = Server.CreateObject( "ADODB.Recordset" )
							Set rsDupcheck = connChem.execute(ispardupsql)
							
							' well if the count is not 0 then it was there so it is a dup
							'pass the dup value
							if rsDupcheck("thecount") = "1" then
								isdup = isduparr(duparr)
							'if there is no dup then set it to ""
							else
								isdup = isdup
							end if
					next
				end if
				

			else
				isdup = ""
			end if
			
				
			Set conChem = Nothing
			'stop
			if isdup <> "" then
				isOK = False
				'bisdup = False
				ShowMessageDialog( "Errors, DrugDeg_action.asp: ' A duplicate structure was found: " & isdup & "'" )
				ReturnInputForm dbkey, formgroup, "add_record"
			else 
				'bisdup = true
				isOK = true	
			end if
			
			
			if isok then
				isOK = DoAddRecord( dbkey, formgroup, table_names, field_names, AddType )	
			end if
			
			if isOK = false and isdup <> "" then
				showErrors = True
			end if
			'stop
			if showErrors = True then
				loc = InStr(Session( "errors_found" & dbkey & formgroup ),"Multiple-step")
				if loc = 0 then
					if "addmechanism_form_group" = sFormGroup_LowerCase then
						loc = InStr(Session( "errors_found" & dbkey & formgroup ),"Item cannot be found")
						if loc = 0 then
							ShowMessageDialog( "Errors, DrugDeg_action.asp: '" & Session( "errors_found" & dbkey & formgroup ) & "'" )
							ReturnInputForm dbkey, formgroup, "add_record"
						end if
					else
						ShowMessageDialog( "Errors, DrugDeg_action.asp: '" & Session( "errors_found" & dbkey & formgroup ) & "'" )
						ReturnInputForm dbkey, formgroup, "add_record"
					end if
				else
					showerrors = false
					isOK = true
					ShowMessageDialog( "Record added" )
				end if
				
			else
				ShowMessageDialog( "Record added" )
			end if
			
			if "addparent_form_group" = sFormGroup_LowerCase then
				'update to the default status
				statusId  = getDefaultStatusId
				Set connChem = GetConnection( dbkey, formgroup, "DRUGDEG_PARENTS" )
				Set rsParents = Server.CreateObject( "ADODB.Recordset" )
				sSQL = "Select PARENT_CMPD_KEY from DRUGDEG_PARENTS order by PARENT_CMPD_KEY desc"
				rsParents.Open sSQL, connChem
				rsParents.MoveFirst
				keyNewParent = rsParents.Fields( "PARENT_CMPD_KEY" )
				rsParents.Close
				
				if "" <> sSalts then
					' We added a parent compound with a salt string.  We need to add the salt
					' string to the newly-added record, since COWS doesn't yet handle
					' checkboxes well.

					' To get the newly-added parent, we get all the parents, order them by their
					' primary keys, and grab the one with the highest primary key.  Those keys are
					' unique and ever-increasing, so the newest parent has the highest key.
					

					' Add the salt string to the newly-added parent.
					
					if sSalts <> "" then
						sSQL = "Update DRUGDEG_PARENTS set SALT = '" & sSalts & "' where PARENT_CMPD_KEY = '" & keyNewParent & "'"
					end if
					'logaction ("update parent sql: " & sSQL)
					connChem.Execute sSQL

					' The following is for Oracle databases.
					connChem.Execute "commit"

				end if
					
					
				sSQL = "Update DRUGDEG_PARENTS set submitted_by = '" & Session( "UserName" & dbkey )  &"' , status = " & statusId & " where PARENT_CMPD_KEY = '" & keyNewParent & "'"
				'sSQL = "Update DRUGDEG_PARENTS set submitted_by = '" & Session("UserID" & dbkey) &"' , status = " & statusId & " where PARENT_CMPD_KEY = '" & keyNewParent & "'"
				connChem.Execute sSQL
				
				' The following is for Oracle databases.
				connChem.Execute "commit"
				
				connChem.close
				Session( "PrimaryKey" & dbkey ) = keyNewParent
				'send email
				bSendStatusMail = SendStatusMail ( statusId, sFormGroup_LowerCase )
				
				'#LJB added redirect so that you return to the add parent page this session variable 
				'is set in the addParent_input_form.asp
				'DoRedirect dbkey, Session( "ReturnToCurrentPage" & dbkey ) 
				'formmode=search&formgroup=base_form_group&dataaction=search&dbname=DRUGDEG
				
				'MRE redirect to the parent details page. go through hiddensearch.asp to search by id
				DoRedirect dbkey, "/" & Application( "appkey" ) & "/drugdeg/hiddenSearch.asp?formmode=search&formgroup=base_form_group&dataaction=search&dbname=DRUGDEG&PrimaryKey=" & keyNewParent
				
			elseif "addexperiment_form_group" = sFormGroup_LowerCase then
				'update to the default status
				statusId  = getDefaultStatusId
				Set connChem = GetConnection( dbkey, formgroup, "DRUGDEG_PARENTS" )
				Set rsExperiments = Server.CreateObject( "ADODB.Recordset" )
				sSQL = "Select EXPT_KEY from DRUGDEG_EXPTS order by EXPT_KEY desc"
				rsExperiments.Open sSQL, connChem
				rsExperiments.MoveFirst
				keyNewExpt = rsExperiments.Fields( "EXPT_KEY" )
				rsExperiments.Close

				sSQL = "Update DRUGDEG_EXPTS set submitted_by = '" & Session( "UserName" & dbkey )  &"' , status = " & statusId & " where EXPT_KEY = '" & keyNewExpt & "'"
				'sSQL = "Update DRUGDEG_EXPTS set submitted_by = '" & Session("UserID" & dbkey) &"' , status = " & statusId & " where EXPT_KEY = '" & keyNewExpt & "'"
				connChem.Execute sSQL
				connChem.close
				
				Session( "PrimaryKey" & dbkey ) = keyNewExpt
				'send email
				bSendStatusMail = SendStatusMail ( statusId, sFormGroup_LowerCase )
				
			elseif "adddegradant_form_group" = sFormGroup_LowerCase then
				'LJB 11/17 we need to add the structure separately to avoid the need to Alias the drugdeg_base64 table. The id is stored in  Session("Add_Record_New_ID" & dbkey & formgroup)
				
				Set DataConn = GetConnection( dbkey, formgroup, "DRUGDEG_PARENTS" )
				Set CommitRS = Server.CreateObject( "ADODB.Recordset" )
				'get the molid from the commit to the drugdeg_degs table
				
				sSQL = "select mol_id, DEG_EXPT_FK from drugdeg_degs where DEG_CMPD_KEY=" & Session("Add_Record_New_ID" & dbkey & formgroup)
				'logaction("sSQL " & sSQL)
				CommitRS.Open sSQL, DataConn
				CommitRS.MoveFirst
				drugdegs_deg_molid = CommitRS.Fields( "MOL_ID" )
				drugdeg_expt_id = CommitRS.Fields( "DEG_EXPT_FK" )
				CommitRS.Close
				
				'create a new record in the DRUGDEG_BASE64 table
				CommitRS.Open "DRUGDEG_BASE64", DataConn, adOpenKeyset,  adLockOptimistic, adCmdTable 'get the right record with mol_id
				CommitRS.AddNew	
				'add the molid and commit
				CommitRS("MOL_ID") = drugdegs_deg_molid
				CommitRS.Update
				'now add the structure to the base64 table
				'it doesn't appear that anything is being done with duplicates so why bother doing the duplicate check??
				
				success = DoCartridgeAddStructure(dbkey, formgroup, Session("SearchData" & "Exact" & dbkey & formgroup), drugdegs_deg_molid,"",DataConn,CommitRS)
				CloseRS(CommitRS)

				'add the function groups
				new_deg_fgroups = request("current_fgroups_hidden")
				theReturn = DoAddDegFGroup(dbkey, formgroup, DataConn, new_deg_fgroups, Session("Add_Record_New_ID" & dbkey & formgroup))
				
				'Calculate MW difference if there is no r-group in the parent and degradant
				theReturn = DoUpdateMWDifference(dbkey, formgroup, DataConn, Session("Add_Record_New_ID" & dbkey & formgroup))
				
				'jhs add
				if request("drugdeg_degs.compound_number") <> "" then
				sSQL = "Update DRUGDEG_DEGS set compound_number = '" & request("drugdeg_degs.compound_number") & _
						"' where DEG_CMPD_KEY = " & Session("Add_Record_New_ID" & dbkey & formgroup) & ""
				DataConn.Execute sSQL			
				end if
				

			elseif "addmechanism_form_group" = sFormGroup_LowerCase then
				'MRE 11/17 we need to add the structure separately to avoid the need to Alias the drugdeg_base64 table. The id is stored in  Session("Add_Record_New_ID" & dbkey & formgroup)
				
				Set DataConn = GetConnection( dbkey, formgroup, "DRUGDEG_PARENTS" )
				Set CommitRS = Server.CreateObject( "ADODB.Recordset" )
				'get the molid from the commit to the drugdeg_degs table
				
				sSQL = "select mol_id from drugdeg_mechs where MECH_KEY=" & Session("Add_Record_New_ID" & dbkey & formgroup)
				CommitRS.Open sSQL, DataConn
				CommitRS.MoveFirst
				drugdegs_mech_molid = CommitRS.Fields( "MOL_ID" )
				CommitRS.Close
				
				'create a new record in the DRUGDEG_BASE64 table
				CommitRS.Open "DRUGDEG_BASE64", DataConn, adOpenKeyset,  adLockOptimistic, adCmdTable 'get the right record with mol_id
				CommitRS.AddNew	
				'add the molid and commit
				CommitRS("MOL_ID") = drugdegs_mech_molid
				CommitRS.Update
				'now add the structure to the base64 table
				'it doesn't appear that anything is being done with duplicates so why bother doing the duplicate check??
				
				success = DoCartridgeAddStructure(dbkey, formgroup, Session("SearchData" & "Exact" & dbkey & formgroup), drugdegs_mech_molid,"",DataConn,CommitRS)
				CloseRS(CommitRS)

			end if  ' if we added a parent compound, ...
		end if

		' Determine where to go after adding or modifying a record, then redirect to there.
	
		table_names_test = replace(UCase(table_names), ",DRUGDEG_BASE64","")
		'logaction ("****table_names " & table_names)

		if request("sendemail") = "SEND_EMAIL" then
			'the record was already updated in form_Action_vbs but we need to send an email if the status has changed.
			if Application("DRUGDEG_EMAIL") <> "" then
				select case sFormGroup_LowerCase
					case "base_form_group"
						set r = Request.Form
						
						sStatus = request("UID." & Session("Primarykey" &dbkey) & ":DRUGDEG_PARENTS.STATUS")
										
						if request("old_Status") <> sStatus then
						'send email
							bSendStatusMail = SendStatusMail ( sStatus,  sFormGroup_LowerCase )
						end if
					case "addexperiment_form_group"
						sStatus = request("UID." & Session("Primarykey" &dbkey) & ":DRUGDEG_EXPTS.STATUS")
						if request("old_Status") <> sStatus then
							bSendStatusMail = SendStatusMail ( sStatus, sFormGroup_LowerCase )
						end if
				end select
			end if
		end if

		Select case UCase( table_names_test )
			case  "DRUGDEG_MECHS"
				' We added a degradation mechanism, so we are going to return to the
				' experiment details display from which we added the mechanism.

				Session( "Base_RS" & dbkey & formgroup ) = StoredRS

				if inStr( Session( "ReturnToExperimentDetails" & dbkey ), "keyexpt" ) <= 0 then
					' "keyexpt" wasn't in the query string.  Put it in.
					Session( "ReturnToExperimentDetails" & dbkey ) = Session( "ReturnToExperimentDetails" & dbkey ) & "&keyexpt=" & Request.Form( "DEG_EXPT_FK" )
				end if

				' Make sure the page reloads when you get to the experiment details.
				sNewPath = MakeQueryStringBeDifferent( Session( "ReturnToExperimentDetails" & dbkey ) )
				Session( "ReturnToExperimentDetails" & dbkey ) = sNewPath

				' Force a reload upon return.
				Session( "reload_list" ) = true
				Session( "reload_form" ) = true
				Session( "formmode_loaded" ) = "edit"

				' Send control over to the experiment details display.
				Session( "CurrentLocation" & dbkey & formgroup ) = Session( "ReturnToExperimentDetails" & dbkey )
				DoRedirect dbkey, Session( "ReturnToExperimentDetails" & dbkey )	
			' end of DRUGDEG_MECHS case

			case  "DRUGDEG_DOCMGR_LINKS"
				' We added a link from a parent compound to a document, so we are going to
				' return to the parent compound details display from which we added the link.

				Session( "Base_RS" & dbkey & formgroup ) = StoredRS

				if inStr( Session( "ReturnToParentDetails" & dbkey ), "keyparent" ) <= 0 then
					' "keyparent" wasn't in the query string.  Put it in.
					Session( "ReturnToParentDetails" & dbkey )= Session( "ReturnToParentDetails" & dbkey ) & "&keyparent=" & Request.Form( "PARENT_CMPD_FK" )
				end if

				' Make sure the page reloads when you get to the parent details.
				sNewPath = MakeQueryStringBeDifferent( Session( "ReturnToParentDetails" & dbkey ) )
				Session( "ReturnToParentDetails" & dbkey ) = sNewPath

				' Force a reload upon return.
				Session( "reload_list" ) = true
				Session( "reload_form" ) = true
				Session( "formmode_loaded" ) = "edit"

				' Send control over to the parent details display.
				Session( "CurrentLocation" & dbkey & formgroup ) = Session( "ReturnToParentDetails" & dbkey )
				DoRedirect dbkey, Session( "ReturnToParentDetails" & dbkey )				
			' end of DRUGDEG_DOCMGR_LINKS case

			case  "DRUGDEG_DEGS"
				' We added a degradant compound, so we are going to return to the
				' experiment details display from which we added the degradant.

				Session( "Base_RS" & dbkey & formgroup ) = StoredRS

				if inStr( Session( "ReturnToExperimentDetails" & dbkey ), "keyexpt" ) <= 0 then
					' "keyexpt" wasn't in the query string.  Put it in.
					Session( "ReturnToExperimentDetails" & dbkey ) = Session( "ReturnToExperimentDetails" & dbkey ) & "&keyexpt=" & drugdeg_expt_id
				end if

				' Make sure the page reloads when you get to the experiment details.
				sNewPath = MakeQueryStringBeDifferent( Session( "ReturnToExperimentDetails" & dbkey ) )
				Session( "ReturnToExperimentDetails" & dbkey ) = sNewPath

				
				
				' Send control over to the experiment details display.
				'logaction( "current" & Session( "CurrentLocation" & dbkey & formgroup ) )
				'logaction( "return to exp." & Session( "ReturnToExperimentDetails" & dbkey ) )
				if Session( "ReturnToExperimentDetails" & dbkey ) = "" then
					Session( "ReturnToExperimentDetails" & dbkey ) = "/" & Application("Appkey") & "/drugdeg/Experiment_details_form.asp?dbname="& dbkey &"&formgroup=AddExperiment_form_group&formmode=edit&formgroupflag_override=Experiment_details&keyexpt=" & drugdeg_expt_id
				end if
				if session("wizard") = "wiz_add_deg_3" or session("wizard") = "wiz_add_deg_4" then
					if session("wizard") = "wiz_add_deg_4" then
						'change back to step 3 because we are just adding another degradant to the same experiment.
						session("wizard") = "wiz_add_deg_3"
					end if
					
					Session("ReturnToDegradantPage" & dbkey) = "/" & Application("Appkey") & "/drugdeg/AddDegradant_input_form.asp?dbname="& dbkey &"&formgroup=AddDegradant_form_group&formmode=add_degradant&parent_rgroup=False&formgroupflag_override=add_compounds&keyexpt=" & drugdeg_expt_id
					'logaction("here" & Session("ReturnToDegradantPage" & dbkey))
					DoRedirect dbkey, Session("ReturnToDegradantPage" & dbkey)
				elseif session("wizard") = "wiz_edit_deg_1" then
					'logaction("return to experiment details" & Session("ReturnToExperimentDetails" & dbkey))
					DoRedirect dbkey, Session( "ReturnToExperimentDetails" & dbkey )
				else
					' Force a reload upon return.
					Session( "reload_list" ) = true
					Session( "reload_form" ) = true
					Session( "formmode_loaded" ) = "edit"
					Session( "CurrentLocation" & dbkey & formgroup ) = Session( "ReturnToExperimentDetails" & dbkey )
					'logaction ("there" & Session( "ReturnToExperimentDetails" & dbkey )	)
					DoRedirect dbkey, Session( "ReturnToExperimentDetails" & dbkey )
				end if
				
			' end of DRUGDEG_DEGS case

			case  "DRUGDEG_EXPTS"
				' We added an experiment, so we are going to return to the
				' parent details display from which we added the experiment.

				Session( "Base_RS" & dbkey & formgroup ) = StoredRS

				if inStr( Session( "ReturnToParentDetails" & dbkey ), "keyparent" ) <= 0 then
					' "keyparent" wasn't in the query string.  Put it in.
					Session( "ReturnToParentDetails" & dbkey )= Session( "ReturnToParentDetails" & dbkey ) & "&keyparent=" & Request.Form( "PARENT_CMPD_FK" )
				end if

				' Make sure the page reloads when you get to the parent details.
				sNewPath = MakeQueryStringBeDifferent( Session( "ReturnToParentDetails" & dbkey ) )
				Session( "ReturnToParentDetails" & dbkey ) = sNewPath

				' Force a reload upon return.
				Session( "reload_list" ) = true
				Session( "reload_form" ) = true
				Session( "formmode_loaded" ) = "edit"

				' Send control over to the parent details display.
				Session( "CurrentLocation" & dbkey & formgroup ) = Session( "ReturnToParentDetails" & dbkey )
				'logaction ("expts " & Session( "ReturnToParentDetails" & dbkey ) )
				if session("wizard") = "wiz_add_exp_2" then
					Session("ReturnToExpirementDetails" & dbkey) = "/drugdeg/drugdeg/Experiment_details_form.asp?dbname="&dbkey&"&formgroup=AddExperiment_form_group&formmode=edit&formgroupflag_override=Experiment_details&keyexpt=" &keyNewExpt
					DoRedirect dbkey, Session("ReturnToExpirementDetails" & dbkey)
				elseif Session("wizard") = "wiz_add_deg_2" then
					Session("wizard") = "wiz_add_exp_3"
					Session("ReturnToAddDegradant" & dbkey) = "/drugdeg/drugdeg/AddDegradant_input_form.asp?dbname="&dbkey&"&formgroup=AddDegradant_form_group&formmode=add_degradant&parent_rgroup=False&formgroupflag_override=add_compounds&keyexpt=" &keyNewExpt
					DoRedirect dbkey, Session("ReturnToAddDegradant" & dbkey)
				elseif Session("wizard") = "wiz_add_par_3" then
					'added a parent and now we added an expirement
					Session("wizard") = "wiz_add_par_4"
					Session("ReturnToAddDegradant" & dbkey) = "/drugdeg/drugdeg/AddDegradant_input_form.asp?dbname="&dbkey&"&formgroup=AddDegradant_form_group&formmode=add_degradant&parent_rgroup=False&formgroupflag_override=add_compounds&keyexpt=" &keyNewExpt
					'go back to parent
					DoRedirect dbkey, Session( "ReturnToParentDetails" & dbkey )
				else
					DoRedirect dbkey, Session( "ReturnToParentDetails" & dbkey )				
				end if
				
			' end of DRUGDEG_EXPTS case

			case  "DRUGDEG_PARENTS"
				if true = bAddNewRecord then
					' We added a parent.  I think COWS will by default go to the list view
					' for the formgroup in the ini file.  I _know_ that if you click the "New
					' Query" button for a list view you will go to the input view for the
					' formgroup.  That means that after you add a parent you will see the
					' parent list view, and if you click the "New Query" button you will go
					' to the Add Parent page, NOT the parent query page.  I will avoid
					' all that by going to the main page after adding a parent.
					sReturnLoc = "/" & Application( "AppKey" ) & "/" & dbkey & "/mainpage.asp?dbname=" & dbkey

					' Add a tag to the query string to indicate its origin.
					sNewPath = URLWithMadeInArgument( sReturnLoc, "DDAction" )
					sReturnLoc = sNewPath

					' Make sure the main page reloads when you get there.
					sNewPath = MakeQueryStringBeDifferent( sReturnLoc )
					sReturnLoc = sNewPath

					DoRedirect dbkey, sReturnLoc
				else
					' The new parent is not to be added for some reason.

					' We will return to the Add Parent display, but first we must tack a bit onto
					' the query string to make the data fields reload.
					sReturnLoc = Session( "ReturnToAddParent" & dbkey ) & "&special=edit_query"

					' Now we must fill a variable with the selected salt list, because COWS
					' doesn't deal with checkboxes well.  The list will be used for reloading
					' the set of salt checkboxes in the Add Parent display.
					Session( "SelectedSalts" & dbkey ) = sSalts

					' Return to the Add Parent page to give the user a chance to try again.
					DoRedirect dbkey, sReturnLoc
				end if
			' end of DRUGDEG_PARENTS case

			case  "DRUGDEG_CONDS"
				if bAddNewRecord then
					' We added a degradation experiment condition.  We may need to add
					' a number of conditions, so we are going to return to the display
					' for adding conditions.
					Session( "Base_RS" & dbkey & formgroup ) = StoredRS

					' Make sure the page reloads when you get back to it.
					sNewPath = MakeQueryStringBeDifferent( Session( "ReturnToConditionListAdmin" & dbkey ) )
					Session( "ReturnToConditionListAdmin" & dbkey ) = sNewPath

					Session( "CurrentLocation" & dbkey & formgroup ) = Session( "ReturnToConditionListAdmin" & dbkey )
					DoRedirect dbkey, Session( "ReturnToConditionListAdmin" & dbkey )	
				elseif bModifiedRecord then
					' We modified a degradation experiment condition.  We may need to add
					' a number of conditions, so we are going to return to the display
					' for adding conditions.
					Session( "Base_RS" & dbkey & formgroup ) = StoredRS

					' Make sure the page reloads when you get back to it.
					sNewPath = MakeQueryStringBeDifferent( Session( "ReturnToConditionListAdmin" & dbkey ) )
					Session( "ReturnToConditionListAdmin" & dbkey ) = sNewPath

					Session( "CurrentLocation" & dbkey & formgroup ) = Session( "ReturnToConditionListAdmin" & dbkey )
					DoRedirect dbkey, Session( "ReturnToConditionListAdmin" & dbkey )	
				else
					' We did not add a degradation experiment condition.  We may need to
					' add a number of conditions, so we are going to return to the display
					' for adding conditions, but first we must tack a bit onto the query
					' string to make the data fields reload.
					sReturnLoc = Session( "ReturnToConditionListAdmin" & dbkey )

					' Return to the degradation conditions page to give the user a chance
					' to try again.
					DoRedirect dbkey, sReturnLoc
				end if
			' end of DRUGDEG_CONDS case
			case  "DRUGDEG_FGROUPS"
				if bAddNewRecord then
					' We added a degradation functional group.  We may need to add
					' a number of groups, so we are going to return to the display
					' for adding groups.
					Session( "Base_RS" & dbkey & formgroup ) = StoredRS

					' Make sure the page reloads when you get back to it.
					sNewPath = MakeQueryStringBeDifferent( Session( "ReturnToFunctionalGroupListAdmin" & dbkey ) )
					Session( "ReturnToFunctionalGroupListAdmin" & dbkey ) = sNewPath

					Session( "CurrentLocation" & dbkey & formgroup ) = Session( "ReturnToFunctionalGroupListAdmin" & dbkey )
					DoRedirect dbkey, Session( "ReturnToFunctionalGroupListAdmin" & dbkey )	
				elseif bModifiedRecord then
					' We modified a degradation functional group.  We may need to add
					' a number of groups, so we are going to return to the display
					' for adding groups.
					Session( "Base_RS" & dbkey & formgroup ) = StoredRS

					' Make sure the page reloads when you get back to it.
					sNewPath = MakeQueryStringBeDifferent( Session( "ReturnToFunctionalGroupListAdmin" & dbkey ) )
					Session( "ReturnToFunctionalGroupListAdmin" & dbkey ) = sNewPath

					Session( "CurrentLocation" & dbkey & formgroup ) = Session( "ReturnToFunctionalGroupListAdmin" & dbkey )
					DoRedirect dbkey, Session( "ReturnToFunctionalGroupListAdmin" & dbkey )	
				else
					' We did not add a degradation functional group.  We may need to
					' add a number of groups, so we are going to return to the display
					' for adding groups, but first we must tack a bit onto the query
					' string to make the data fields reload.
					sReturnLoc = Session( "ReturnToFunctionalGroupListAdmin" & dbkey )

					' Return to the functional groups page to give the user a chance
					' to try again.
					DoRedirect dbkey, sReturnLoc
				end if
			' end of DRUGDEG_CONDS case

			case  "DRUGDEG_STATUSES"
				if bAddNewRecord then
					' We added a degradation experiment condition.  We may need to add
					' a number of conditions, so we are going to return to the display
					' for adding conditions.
					Session( "Base_RS" & dbkey & formgroup ) = StoredRS

					' Make sure the page reloads when you get back to it.
					sNewPath = MakeQueryStringBeDifferent( Session( "ReturnToStatusListAdmin" & dbkey ) )
					Session( "ReturnToStatusListAdmin" & dbkey ) = sNewPath

					Session( "CurrentLocation" & dbkey & formgroup ) = Session( "ReturnToStatusListAdmin" & dbkey )
					DoRedirect dbkey, Session( "ReturnToStatusListAdmin" & dbkey )	
				elseif bModifiedRecord then
					' We modified a degradation experiment condition.  We may need to add
					' a number of conditions, so we are going to return to the display
					' for adding conditions.
					Session( "Base_RS" & dbkey & formgroup ) = StoredRS

					' Make sure the page reloads when you get back to it.
					sNewPath = MakeQueryStringBeDifferent( Session( "ReturnToStatusListAdmin" & dbkey ) )
					Session( "ReturnToStatusListAdmin" & dbkey ) = sNewPath

					Session( "CurrentLocation" & dbkey & formgroup ) = Session( "ReturnToStatusListAdmin" & dbkey )
					DoRedirect dbkey, Session( "ReturnToStatusListAdmin" & dbkey )	
				else
					' We did not add a degradation experiment condition.  We may need to
					' add a number of conditions, so we are going to return to the display
					' for adding conditions, but first we must tack a bit onto the query
					' string to make the data fields reload.
					sReturnLoc = Session( "ReturnToStatusListAdmin" & dbkey )

					' Return to the degradation conditions page to give the user a chance
					' to try again.
					DoRedirect dbkey, sReturnLoc
				end if
			' end of DRUGDEG_STATUSES case
			case  "DRUGDEG_SALTS"
				if bAddNewRecord or bModifiedRecord then
					' We added a parent salt or modified an existing one.  We may need to add
					' or change a number of such salts return to the display for adding them
					' and do not reload the data entry fields.

					Session( "Base_RS" & dbkey & formgroup ) = StoredRS

					' Make sure the page reloads when you get back to it.
					sNewPath = MakeQueryStringBeDifferent( Session( "ReturnToSaltListAdmin" & dbkey ) )
					sReturnLoc = Session( "ReturnToSaltListAdmin" & dbkey )

					Session( "CurrentLocation" & dbkey & formgroup ) = Session( "ReturnToSaltListAdmin" & dbkey )
					DoRedirect dbkey, Session( "ReturnToSaltListAdmin" & dbkey )
				else
					' We are not adding a salt or are we modifying an existing one.  We will
					' return to the Add Salt display, but first we must tack a bit onto the
					' query string to make the data fields reload.
					sReturnLoc = Session( "ReturnToSaltListAdmin" & dbkey ) & "&special=edit_query"

					' Return to the Add Salt page to give the user a chance to try again.
					DoRedirect dbkey, sReturnLoc
				end if
			' end of DRUGDEG_SALTS case

			case ELSE
				Response.Write "undefined table case"
				Response.Write table_names
			' end of default case

		end select	' table_names

	' end of ADD_DRUGDEG case

End Select
%>

