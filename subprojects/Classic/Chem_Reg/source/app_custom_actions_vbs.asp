<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
custom_action = request("dataaction2")
dbkey = request("dbname")
formgroup = request("formgroup")
	Select Case UCase(custom_action)
		Case "APPROVE_ALL"
			theIDS = GetIDFromRS(dbkey, formgroup, "Base_RS")
			theReturn= doApproveAll(dbkey, formgroup,theIDS)
			showmessagedialog(theReturn)
			Session("ReturnLocation" & dbkey & formgroup)=Request("CurrentLocation") 
			redirectpath = Session("ReturnLocation" & dbkey & formgroup) & "&done=true"
			DoRedirect dbkey, redirectpath
			
		Case "APPROVE_SINGLE"
			unique_id = request("unique_id")
			theReturn = doApproveAll(dbkey, formgroup,unique_id)
			showmessagedialog("Record Approved")
			Session("ReturnLocation" & dbkey & formgroup)=Request("CurrentLocation") 
			redirectpath = Session("ReturnLocation" & dbkey & formgroup) & "&done=true"
			DoRedirect dbkey, redirectpath
		
		'SYAN added on 12/8/2004 to fix CSBR-49587
		Case "PRE_REGISTER_APPROVE_ALL"
			theIDS = GetIDFromRS(dbkey, formgroup, "Base_RS")
			theReturn= doPreRegisterApprove(dbkey, formgroup,theIDS)
			showmessagedialog(theReturn)
			Session("ReturnLocation" & dbkey & formgroup)=Request("CurrentLocation") 
			redirectpath = Session("ReturnLocation" & dbkey & formgroup) & "&approveDone=true"
			DoRedirect dbkey, redirectpath
			
		Case "PRE_REGISTER_APPROVE_SINGLE"
			unique_id = request("unique_id")
			theReturn = doPreRegisterApprove(dbkey, formgroup,unique_id)
			showmessagedialog("Record Approved")
			Session("ReturnLocation" & dbkey & formgroup)=Request("CurrentLocation") 
			redirectpath = Session("ReturnLocation" & dbkey & formgroup) & "&approveDone=true"
			DoRedirect dbkey, redirectpath
		'End of SYAN modification
		
		Case "ADD_BATCH_PROJECT"
			
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Adding Record...")
			end if
			
			theReturn = DoAddBatchProject(dbkey, formgroup)
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
 			'DoRedirect dbkey, Session("PreAddReturnLocation" & dbkey)
 			ReturnEditResults dbkey, formgroup, "add_record", dbkey 

 			
		Case "UPDATE_BATCH_PROJECT"
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Updating Record...")
			end if
			batch_project_id =request("batch_project_id")
			theReturn = doUpdateBatchProject(dbkey, formgroup,batch_project_id)
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
			ReturnEditResults dbkey, formgroup, "update_record", dbkey 

		Case "DELETE_BATCH_PROJECT"
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Deleting Record...")
			end if
			batch_project_id = request("batch_project_id")
			theReturn = DoDeleteBatchProject(dbkey, formgroup,batch_project_id)
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
			ReturnEditResults dbkey, formgroup, "delete_record", dbkey 
			
		Case "DELETE_PROJECT"
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Deleting Record...")
			end if
			project_id = request("project_id")
			theReturn = DoDeleteProject(dbkey, formgroup,project_id)
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
			ReturnEditResults dbkey, formgroup, "delete_record", dbkey 
			
		Case "DELETE_COMPOUND_TYPE"
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Deleting Record...")
			end if
			compound_type_id = request("compound_type_id")
			theReturn = DoDeleteCompoundType(dbkey, formgroup,compound_type_id)
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
			ReturnEditResults dbkey, formgroup, "delete_record", dbkey 
			
		Case "DELETE_NOTEBOOK"
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Deleting Record...")
			end if
			notebook_id = request("notebook_id")
			theReturn = DoDeleteNotebook(dbkey, formgroup,notebook_id)
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
			ReturnEditResults dbkey, formgroup, "delete_record", dbkey 
			
		Case "DELETE_SEQUENCE"
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Deleting Record...")
			end if
			sequence_id = request("sequence_id")
			theReturn = DoDeleteSequence(dbkey, formgroup,sequence_id)
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				FlushMessageToClient("<br><br>" & theReturn)
			end if
			ReturnEditResults dbkey, formgroup, "delete_record", dbkey 
			
		Case "DELETE_PERSON"
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Deleting Record...")
			end if
			person_id = request("person_id")
			theReturn = DoDeletePerson(dbkey, formgroup,person_id)
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
			ReturnEditResults dbkey, formgroup, "delete_record", dbkey 
			
		Case "DELETE_SITE"
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Deleting Record...")
			end if
			site_id = request("site_id")
			theReturn = DoDeleteSite(dbkey, formgroup,site_id)
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
			ReturnEditResults dbkey, formgroup, "delete_record", dbkey 
		
		Case "DELETE_SALT"
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Deleting Record...")
			end if
			salt_id = request("salt_id")
			theReturn = DoDeleteSalt(dbkey, formgroup,salt_id)
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
			ReturnEditResults dbkey, formgroup, "delete_record", dbkey 
 		
 		Case "DELETE_SOLVATE"
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Deleting Record...")
			end if
			solvate_id = request("solvate_id")
			theReturn = DoDeleteSolvate(dbkey, formgroup,solvate_id)
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
			ReturnEditResults dbkey, formgroup, "delete_record", dbkey 
			
 		Case "ADD_UTILIZATION"
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Adding Record...")
			end if
			theReturn = DoAddUtilization(dbkey, formgroup)
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
			ReturnEditResults dbkey, formgroup, "add_record", dbkey 
 			
		Case "UPDATE_UTILIZATION"
			utilization_id = request("utilization_id")
			theReturn = DoUpdateUtilization(dbkey, formgroup,utilization_id)
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
			ReturnEditResults dbkey, formgroup, "update_record", dbkey 

			
		Case "DELETE_UTILIZATION"
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Deleting Record...")
			end if
			utilization_id = request("utilization_id")
			theReturn = DoDeleteUtilization(dbkey, formgroup,utilization_id)
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
 			ReturnEditResults dbkey, formgroup, "delete_record", dbkey 
 		
 		Case "DELETE_BATCH"
 			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Deleting Record...")
			end if
			batch_id = request("batch_id")
			theReturn = DoDeleteBatch(dbkey, formgroup,batch_id)
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
 			ReturnEditResults dbkey, formgroup, "delete_record", dbkey 
 		
 		Case "UPDATE_ANALYTICS"
 			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Updating Record...")
			end if
			batch_id = request("batch_id")
			theReturn = DoUpdateAnaltyics(dbkey, formgroup,batch_id)
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
 			ReturnEditResults dbkey, formgroup, "update_record", dbkey 
 		
 		Case "DELETE_ANALYTICS"
 		
 			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Deleting Record...")
			end if
			batch_id = request("batch_id")
			theReturn = DoDeleteAnaltyicsMaster(dbkey, formgroup,batch_id)
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
 			ReturnEditResults dbkey, formgroup, "delete_record", dbkey 
 		
 		Case "DELETE_EXPERIMENT"
 			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Deleting Record...")
			end if
			experiment_id = request("experiment_id")
			theReturn = DoDeleteExperimentMaster(dbkey, formgroup,experiment_id)
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
 			ReturnEditResults dbkey, formgroup, "delete_record", dbkey 


		Case "DELETE_REG"
			if CBool(Application("PROJECT_LEVEL_ROW_SECURITY")) = true then
					Session("storeSessionUser" & dbkey) = Session("UserName" & dbkey)
					Session("storeSessionPWD" & dbkey) = Session("UserID" & dbkey)
					Session("UserName" & dbkey) = Application("REG_USERNAME")
					Session("UserID" & dbkey) =  Application("REG_PWD")
			end if
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Deleting Record...")
			end if
			'SetSessionVars dbkey, formgroup, "add_record"
			showErrors = False
			reg_id=Request("uniqueid")
			showErrors = DoDeleteRegMaster(dbkey, formgroup, reg_id) 'this calls doDeleteRecord as before
			if showErrors = True then
				theReturn = Session("errors_found" & dbkey & formgroup)
			else
				theReturn = "Record deleted"
			end if
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
			
			if CBool(Application("PROJECT_LEVEL_ROW_SECURITY")) = true then
				Session("UserName" & dbkey) = Session("storeSessionUser" & dbkey)
				Session("UserID" & dbkey) = Session("storeSessionPWD" & dbkey)
				UpdateRLSBaseTableRecordCount dbkey, formgroup
			end if
			
 			DoRedirect dbkey, Session("CurrentLocation" & dbkey & formgroup)
 			
		Case "UPDATE_REG"
			
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Updating Record...")
			end if
			RelFields = Request("RelationalSearchFields")
			StrucFields= Request("ExactSearchFields")
			table_names = Request("ROW_ID_TABLE_NAMES")
			showErrors = DoUpdateRegMaster(dbkey, formgroup, RelFields, StrucFields, table_names) 
			if Not Session("errors_found" & dbkey & formgroup) <> "" then
				showErrors = False
			end if
			if showErrors = True then
				theReturn = Session("errors_found" & dbkey & formgroup)
			else
				theReturn = "Registry updated successfully"
			end if
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
 			ReturnEditResults dbkey, formgroup, "update_record", dbkey 
		
		Case "ADD_TEMP"
			if CBool(Application("PROJECT_LEVEL_ROW_SECURITY")) = true then
					Session("storeSessionUser" & dbkey) = Session("UserName" & dbkey)
					Session("storeSessionPWD" & dbkey) = Session("UserID" & dbkey)
					Session("UserName" & dbkey) = Application("REG_USERNAME")
					Session("UserID" & dbkey) =  Application("REG_PWD")
			end if
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Adding Record...")
			end if
			SetSessionVars dbkey, formgroup, "add_record"
			GetSearchData dbkey, formgroup
			AddOrder = Request("Add_Order")
			table_names=Session("strWhereSubforms" & dbkey & formgroup)
			field_names=Session("SearchData" & "Relational" & dbkey & formgroup)
			showErrors = False
			Session("Current_Reg_Number")=""
			Session("Reagent_is_Duplicate")=""
			commit_type = UCase(Request.querystring("commit_type"))
			theID = DoRegTemp(dbkey, formgroup, commit_type)
			if not isEmpty(theID) then
				Select case UCase((commit_type))
					Case "ADD_IDENTIFIERS"
						identLocation = CBool(Application("Identifiers_To_Temp"))
							if identLocation = True then
								theReturn = "Identifiers were added to temporary table for later review."
							else
								theReturn = "Identifiers were added to the permanent registry tables."
							End if
					Case "BATCH_COMMIT"
						batchCommitLocation = CBool(Application("Batches_To_Temp"))
							if batchCommitLocation = True then
								theReturn = "A batch was added to the temporary table for later review."
							else
								theReturn = "A batch was added to the permanent registry tables."
							End if
					
					Case "FULL_COMMIT"
						if UCase(formgroup)="ADD_REAGENT_CTRBT_FORM_GROUP" AND NOT CBool(Application("Reagents_To_Temp")) then
							
								if Session("Reagent_is_Duplicate") = True then
									theReturn = "This compound is a duplicate and was added to the temporary table for review."
								else
									theReturn = "The compound was registered. Registration Number: " & Session("Current_Reg_Number")
								end if
							
						else
							theReturn = "Your record was added to the temporary table"
						end if
					Case "ADD_SALT"
						theReturn = "A salt was added to temporary table for later review."
					Case Else
						theReturn = "Your record was added to the temporary table"
				End Select
			else
				theReturn = "Errors occurred. Record not added successfully"
			end if
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				showmessagedialog(theReturn)
				
			end if
			
			if request("button_override") <> "" and Not instr(Session("CurrentLocation" & dbkey & formgroup), "button_override")> 0 then
				Session("CurrentLocation" & dbkey & formgroup)= Session("CurrentLocation" & dbkey & formgroup) & "&button_override=" & request("button_override") & "&preserve_items=button_override"
			end if
			if CBool(Application("PROJECT_LEVEL_ROW_SECURITY")) = true then
				Session("UserName" & dbkey) = Session("storeSessionUser" & dbkey)
				Session("UserID" & dbkey) = Session("storeSessionPWD" & dbkey)
				UpdateRLSTableRecordCount dbkey, formgroup, "temporary_structures"
			end if
			
			doRedirect dbkey, MakeQueryStringBeDifferent(Session("CurrentLocation" & dbkey & formgroup))
 			'ReturnInputForm dbkey, formgroup, "add_record" 
		
'called by register button	
		Case "REGISTER"	
			if CBool(Application("PROJECT_LEVEL_ROW_SECURITY")) = true then
					Session("storeSessionUser" & dbkey) = Session("UserName" & dbkey)
					Session("storeSessionPWD" & dbkey) = Session("UserID" & dbkey)
					Session("UserName" & dbkey) = Application("REG_USERNAME")
					Session("UserID" & dbkey) =  Application("REG_PWD")
			end if
			on error goto 0
			
			if not UCase(request("commit_type")) = "DELETE_RECORD" then
				GetSearchData dbkey, formgroup
			
			if Err.number <> 0 then
				Response.Write "Error after GetSearchData: " & Err.number & " Desc: " & Err.Description & "<br><br>"
				Response.End
			end if
			end if
			'code for committing something from the temp table. the first part is the standard cows search
			'returns the reg_number and the type of commit
			tempTableUniqueID= Request.QueryString("uniqueid")	'this is temp_compound_id
			
			'dup_action= Request.QueryString("duplicate_action")
			'duplicate_ids = Request.QueryString("duplicate_ids")
			'tempTableUniqueID= Request("Temporary_Structures.Temp_Compound_ID")
			'FlushMessageToClient("Registering...")
			commit_result = DoRegCommit(dbkey, formgroup, "single_mode", tempTableUniqueID, dup_action,duplicate_ids,"")
			
			if Err.number <> 0 and Not err.number=13 then
				Response.Write "<FONT FACE=""Arial"" SIZE=""2"" COLOR=""Red"">Error after DoRegCommit: " & Err.number & " Desc: " & Err.Description & "</FONT><BR>"
				Err.Clear
				
				' Ignore it and carry on...
				Response.End
			end if
			
			if inStr(UCase(commit_result), "DUPLICATE")> 0 or inStr(UCase(commit_result), "ERROR")> 0 then
				Session("DupTrackingStr" & dbkey) = Session("DupTrackingStr" & dbkey) & ";" & commit_result
				Session("CurrentLocation" & dbkey & formgroup)=Session("ReturnLocation" & dbkey & formgroup)
				redirectpath= Session("CurrentLocation" & dbkey & formgroup) 
			else
				if Not inStr(UCase(commit_result), "SKIP")> 0 then
					Session("RegTrackingStr" & dbkey) = Session("RegTrackingStr" & dbkey) & ";" & commit_result
				end if
				Session("CurrentLocation" & dbkey & formgroup)=Session("ReturnLocation" & dbkey & formgroup)
				redirectpath= Session("CurrentLocation" & dbkey & formgroup) & "&done=true"

			end if
			if CBool(Application("PROJECT_LEVEL_ROW_SECURITY")) = true then
				Session("UserName" & dbkey) = Session("storeSessionUser" & dbkey)
				Session("UserID" & dbkey) = Session("storeSessionPWD" & dbkey)
				UpdateRLSBaseTableRecordCount dbkey, formgroup
			end if
			
			'FlushMessageToClient("Registration complete...")
			DoRedirect dbkey,  redirectpath
			'called by register button	
			
		Case "REGISTER_ALL"
			if CBool(Application("PROJECT_LEVEL_ROW_SECURITY")) = true then
				
					Session("storeSessionUser" & dbkey) = Session("UserName" & dbkey)
					Session("storeSessionPWD" & dbkey) = Session("UserID" & dbkey)
					Session("UserName" & dbkey) = Application("REG_USERNAME")
					Session("UserID" & dbkey) =  Application("REG_PWD")
			end if
			
			theReturn = DoRegisterAll(dbkey, formgroup,request("duplicate_processing"),request("temp_sort_by"),request("temp_sort_by_direction"))
			showmessagedialog(theReturn)
			Session("ReturnLocation" & dbkey & formgroup)=Request("CurrentLocation") 
			redirectpath = Session("ReturnLocation" & dbkey & formgroup) & "&done=true"
			if CBool(Application("PROJECT_LEVEL_ROW_SECURITY")) = true then
				Session("UserName" & dbkey) = Session("storeSessionUser" & dbkey)
				Session("UserID" & dbkey) = Session("storeSessionPWD" & dbkey)
				UpdateRLSBaseTableRecordCount dbkey, formgroup
			end if
			DoRedirect dbkey, redirectpath
			
		Case "ADD_PEOPLE_PROJECT"
			Session("errors_found" & dbkey & formgroup)=""
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Adding Record...")
			end if
			new_people_project_users = request("current_users_hidden")
			theReturn = DoAddPeopleProject(dbkey, formgroup,new_people_project_users)
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
 			'DoRedirect dbkey, Session("PreAddReturnLocation" & dbkey)
 			ReturnEditResults dbkey, formgroup, "add_record", dbkey 

 			
		Case "UPDATE_PEOPLE_PROJECT"
			Session("errors_found" & dbkey & formgroup)=""
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Updating Record...")
			end if
			project_id =request("project_id")
			new_people_project_users = request("current_users_hidden")
			theReturn = doUpdatePeopleProject(dbkey, formgroup,project_id, new_people_project_users)
			
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
			ReturnEditResults dbkey, formgroup, "update_record", dbkey 

		Case "DELETE_PEOPLE_PROJECT"
			Session("errors_found" & dbkey & formgroup)=""
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Deleting Record...")
			end if
			project_id =request("project_id")
			theReturn = DoDeletePeopleProject(dbkey, formgroup,project_id)
			if inStr(UCase(theReturn), "ERROR")> 0 or Session("errors_found" & dbkey & formgroup)<> "" then
				showmessagedialog(theReturn)
			else
				if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
					showmessagedialog(theReturn)
				else
					FlushMessageToClient("<br><br>" & theReturn)
				end if
			end if
			
			ReturnEditResults dbkey, formgroup, "delete_record", dbkey 
		Case "MANAGE_REG_TABLES_LIST"
			
			ReturnResults "reg", "manage_reg_tables_form_group", "get_sql_string", "" 
		
		Case "MANAGE_ANALYTICS_TABLES_LIST"
			
			ReturnResults "reg", "manage_analytics_tables_form_group", "get_sql_string", "" 
		Case "DELETE_RESULT_TYPE_MA"
		
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Deleting Record...")
			end if
			'SetSessionVars dbkey, formgroup, "add_record"
			showErrors = False
			delete_order = Request("table_delete_order")
			table_names = Request("ROW_ID_TABLE_NAMES")
			showErrors = DoDeleteRecordMaster(dbkey, formgroup, table_names, delete_order) 'this calls doDeleteRecord as before
			If LCase(Request("no_gui")) = "true" then
				if showErrors then
					Response.Clear
					Response.ContentType = "text/xml"
					Response.Write "<ERROR>search_funcs_vbs.asp:DoAddRecord:" & Session("errors_found" & dbkey & formgroup) &"</ERROR>"
				Else
					Response.Clear
					Response.ContentType = "text/xml"
					Response.Write "<RECORDDELETED></RECORDDELETED>"
				End if		
			Else
				if showErrors = True then
					theReturn =Session("errors_found" & dbkey & formgroup)
				else
					theReturn ="Record deleted"
				end if
				if inStr(theReturn, "error")> 0 then
					showmessagedialog(theReturn)
				else
					if CBool(Application("ALWAYS_DISPLAY_ALERTS")) = TRUE then
						showmessagedialog(theReturn)
					else
						FlushMessageToClient("<br>" & theReturn)
					end if
				end if
				ReturnEditResults dbkey, formgroup, strDataAction, dbkey 
			End if
	'Start Givaudan EVAL customization
		case "DO_ADD_EVAL_DATA"
			batchid = Request("batch_id")
			indexvalue = Request("indexvalue")
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Adding record...")
			end if
			
			SetSessionVars dbkey, formgroup, "add_record"
		
			GetSearchData dbkey, formgroup
			AddOrder = "EVSCREEN"
			showErrors = False
			table_names="EVSCREEN"
			field_names=Session("SearchData" & "Relational" & dbkey & formgroup)
			Session("StoreBase_RS" & dbkey)=Session("Base_RS" & dbkey)
			
			isOK= DoAddRecord(dbkey, formgroup, table_names, field_names, "")
			Session("Base_RS" & dbkey & formgroup)=Session("StoreBase_RS" & dbkey & formgroup)
			if isOK = false then
				showErrors = True
			end if
			if showErrors = True then
				ShowMessageDialog(Session("errors_found" & dbkey & formgroup))
			else
				ShowMessageDialog("Record added")
			end if
			'returnlocation = request("return_location")
			if not session("override_return_location") = true then
				returnlocation = "/chem_reg/evscreen/eval_data_form.asp?formgroup=evscreen_form_group&dbname=reg&formmode=eval_data&indexvalue=" & indexvalue & "&fromForm=add_eval&alt_unique_id=" & batchid
			else
				returnlocation = "/chem_reg/evscreen/eval_data_form.asp?formgroup=evscreen_form_group&dbname=reg&formmode=eval_data&indexvalue=0&alt_unique_id=" & batchid
			end if
			doRedirect dbkey, returnlocation
			
		
		case "DO_EDIT_EVAL_DATA"
			If CBool(Application("USE_ANIMATED_GIF"))=true then
				FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			else
				FlushMessageToClient("Updating record...")
			end if
			
			
			'SetSessionVars dbkey, formgroup, "update_record"
			id = request("EVSCREEN.ID")
			batch_id = request("batch_id")
			RelFields = Request("RelationalSearchFields")
			showErrors = False
			Session("errors_found" & dbkey & formgroup)=""
			temp = "EVSCREEN"
			table_name_array = split(temp, ",", -1)
			StrucFields= ""
			thecounteri=0
			Session("counti")= UBound(table_name_array)
			for thecounteri = 0 to CLng(Session("counti"))
				table_name = table_name_array(thecounteri)	
				tables_row_ids = request(table_name & "_ROW_IDS")
				
				tables_row_ids_array = split(tables_row_ids, ",", -1)
				thecounterj=0
				Session("countj")= UBound(tables_row_ids_array)
				for thecounterj = 0 to CLng(Session("countj"))
					row_id = tables_row_ids_array(thecounterj)
					'get all submitted data fields for this table and row_id
					
					isOK = DoUpdateRecord(dbkey, formgroup, "",RelFields, table_name, row_id)
					if isOK = False then
						showErrors = True
					end if
				next 'row_id thecounter for table i
			next 'table i
			
			if Not Session("errors_found" & dbkey & formgroup) <> "" then
				showErrors = False
			end if
			if showErrors = True then
				ShowMessageDialog(Session("errors_found" & dbkey & formgroup))
			else
				ShowMessageDialog("Record updated")
			end if
			
			returnlocation = request("return_location")
			returnlocation = replace(returnlocation,"edit_record", "eval_data")
			
			doRedirect dbkey, returnlocation
		
		case "DO_DELETE_EVAL_DATA"
			row_id = Request("evscreen.id")
			table_name="EVSCREEN"
			strWhere= ""
												
			isOK = DoDeleteRecord(dbkey, formgroup, row_id,table_name, strWhere)	
						
			if showErrors = True then
				ShowMessageDialog(Session("errors_found" & dbkey & formgroup))
			else
				ShowMessageDialog("Record Deleted")
			end if
			returnlocation = request("return_location")
			returnlocation = replace(returnlocation,"edit_record", "eval_delete")
			
			doRedirect dbkey, returnlocation
			'doRedirect dbkey, Session("currentlocation" & dbkey & formgroup)
		

		'End Givaudan EVAL customization
		
		'SYAN added on 10/19/2004 to fix CSBR-46772
		case "DELETE_MARKED_TEMP"
			stop
			if IsEmpty(Session("MarkedHits" & dbkey & formgroup)) then
				ShowMessageDialog("No Records Marked.")
			else
				isOK = DeleteMarkedTemp(dbkey, formgroup)
			
			
				if isOK <> true then
					showErrors = True
				end if
			
				if showErrors = True then
					ShowMessageDialog(isOK)
				else
					ShowMessageDialog("Records Deleted")
				end if
			end if
			
			Session("ReturnLocation" & dbkey & formgroup)=Request("CurrentLocation") 
			
			redirectpath = Session("ReturnLocation" & dbkey & formgroup)
			DoRedirect dbkey, redirectpath
			
		case "DELETE_MARKED_PERM"
			stop
			if IsEmpty(Session("MarkedHits" & dbkey & formgroup)) then
				ShowMessageDialog("No Records Marked.")
			else
				isOK = DeleteMarkedPerm(dbkey, formgroup)
			
			
				if isOK <> true then
					showErrors = True
				end if
			
				if showErrors = True then
					ShowMessageDialog(isOK)
				else
					ShowMessageDialog("Records Deleted")
				end if
			end if
			
			Session("ReturnLocation" & dbkey & formgroup)=Request("CurrentLocation") 
			
			redirectpath = Session("ReturnLocation" & dbkey & formgroup)
			DoRedirect dbkey, redirectpath
		'End of SYAN modification
		
		case "REAPPLY_CHEMSCRIPT_RULES"
			'stop
			if IsEmpty(Session("MarkedHits" & dbkey & formgroup)) then
				ShowMessageDialog("No Records Marked.")
			else
				ReApplyChemScriptRules dbkey, formgroup
			
				ShowMessageDialog("ChemScript Rules Re-applied")
			end if
			
			Session("ReturnLocation" & dbkey & formgroup)=Request("CurrentLocation") 
			
			redirectpath = Session("ReturnLocation" & dbkey & formgroup)
			DoRedirect dbkey, redirectpath


		case "REAPPLY_CHEMSCRIPT_RULES_SINGLE"
			stop
			tempID = request("tempID")

			ReApplyChemScriptRulesSingle dbkey, formgroup, tempID
			
			ShowMessageDialog("ChemScript Rules Re-applied")
			
			Session("ReturnLocation" & dbkey & formgroup)=Request("CurrentLocation") 
			
			redirectpath = Session("ReturnLocation" & dbkey & formgroup)
			DoRedirect dbkey, redirectpath
end select

		
%>

