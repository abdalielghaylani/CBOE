<SCRIPT LANGUAGE=vbscript RUNAT=Server>
'----------------------------------------------------------------------------'
'Name: TagCSDOHitlistID(hitlistID, UserName, formgroup, dbname)
'Purpose: Update CSDOHitListID table with user_id, formgroup and time stamp
'Parameter description: hitlistID as String, UserName as String, formgroup as String, dbname as String
'Return Values: none
'Comments:  Uses functions from manage_user_settings to get connection and SQLsyntax
'----------------------------------------------------------------------------'
Sub TagCSDOHitlistID(hitlistID, formgroup, dbname)
	Dim lRecsAffected
	Dim slq
	Dim tableName
	
	currentRDBMS = GetUserSettingsSQLSyntax(dbname, "base_form_group")
	UserName = Session("USER_SETTINGS_ID" & dbkey)
	Select Case(currentRDBMS)
		Case "ORACLE"
			tableName = Application(dbname & "_USERNAME") & ".CSDOHITLISTID"  
		Case "SQLSERVER", "ACCESS"
			tableName = "CSDOHITLISTID"  
	End select
	sql =	"UPDATE " & tableName &_
			" SET USER_ID=?" & _
			" ,FORMGROUP=?" &_
			" ,NUMBER_HITS=?" &_
			" ,IS_PUBLIC='0'" &_
			" ,DATE_CREATED=?" &_
			" WHERE ID=?"

	GetUserSettingsConnection dbname, "base_form_group", "base_connection"
	Set Cmd = Server.CreateObject("ADODB.COMMAND")
	
	Cmd.ActiveConnection = UserSettingConn
	Cmd.CommandType = adCmdText
	Cmd.CommandText = sql
	Cmd.Parameters.Append Cmd.CreateParameter("pUserName", 200, 1,Len(UserName) + 1, Trim(UCase(UserName))) 
	Cmd.Parameters.Append Cmd.CreateParameter("pFormgroup", 200, 1,Len(formgroup) + 1,Trim(Ucase(formgroup))) 
	Cmd.Parameters.Append Cmd.CreateParameter("pNumberHits", 5, 1,0,Session("hitlistrecordcount" & dbkey & formgroup)) 
	Cmd.Parameters.Append Cmd.CreateParameter("pDateCreated", adDBTimeStamp, 1,30, now()) 
	Cmd.Parameters.Append Cmd.CreateParameter("pID", 5, 1, 0, hitlistID) 
	
	Cmd.Execute
	
	Session("TooManyHitsToDisplay" & dbkey & formgroup) =  Session("hitlistrecordcount" & dbkey & formgroup) > Application("TooManyHitsMaximumRetrievable")
	
	Session("HitListHistoryExists" & dbname & formgroup) = AddItemToDelimitedList(Session("HitListHistoryExists" & dbname & formgroup), Session("HitListID" & dbname & formgroup), ",")
	' Remember the first search for restore last case
	if Session("LastHitListID" & dbkey & formgroup) = "" then
		Session("LastHitListID" & dbkey & formgroup) = Session("HitListID" & dbkey & formgroup)
		Session("LastHitListRecordcount" & dbkey & formgroup) = Session("hitlistRecordCount" & dbkey & formgroup)
	End if
End sub

Function getSessionInfoFromCSDOHitlist(dbkey, formgroup, user_id)
	Dim hitlistIDTableName
	
	hitListIDTableName = GetFullTableName(dbkey, formgroup,"CSDOHITLISTID")
	sql = "SELECT NAME, DESCRIPTION, ID, USER_ID, FORMGROUP, NUMBER_HITS, IS_PUBLIC, DATE_CREATED" &_
		" FROM " & hitlistIDTableName  &_
		" WHERE " & hitlistIDTableName & ".USER_ID='"& Ucase(user_id) & "'" &_
		" AND " & hitlistIDTableName & ".FORMGROUP='"& Ucase(formgroup) & "'" &_
		" AND " & hitlistIDTableName & ".NAME='TEMP'" &_
		" ORDER BY DATE_CREATED DESC"
		
		getSessionInfoFromCSDOHitlist=sql
End Function

Function getSavedHitlistsSQL(dbkey, formgroup, user_id, showPublic)
	Dim hitlistIDTableName
	
	hitListIDTableName = GetFullTableName(dbkey, formgroup,"USERHITLISTID")

	sql = "SELECT NAME, DESCRIPTION, ID, USER_ID, FORMGROUP, NUMBER_HITS, IS_PUBLIC, DATE_CREATED" &_
		" FROM " & hitListIDTableName  &_
		" WHERE " & hitListIDTableName & ".USER_ID='" & Ucase(user_id) & "'"  &_
		" AND " & hitListIDTableName & ".FORMGROUP='"& Ucase(formgroup) & "'" &_
		" AND " & hitListIDTableName & ".NAME<>'TEMP'" &_
		" AND " & hitListIDTableName & ".NAME<>'USERMARKEDHITS'"
		if showPublic then
			sql = sql & " OR " & hitListIDTableName & ".IS_PUBLIC='1'"
			maybePublic = "IS_PUBLIC," 		 
		end if
	sql = sql & " ORDER BY " & maybePublic & "NAME"  
	getSavedHitlistsSQL = sql
End Function

Function CreateNewHitList(hitlistID1, hitlistID2, listType, operation, dbname, formgroup, user_id)
	Dim NewhitlistID
	Dim hitListTableName
	Dim hitListIDTableName
	Dim numRecsInserted_temp
	Dim bFlipParams
	Dim ADOProvider

	bFlipParams = false
	numRecsInserted_temp = 0
	hitListTableName = GetFullTableName(dbkey, formgroup,"CSDOHITLIST")
	hitListTableName1 = GetFullTableName(dbkey, formgroup,"CSDOHITLIST")
	hitListTableName2 = GetFullTableName(dbkey, formgroup, Ucase(listType) & "HITLIST")
	
	hitListIDTableName = GetFullTableName(dbkey, formgroup,"CSDOHITLISTID")
	currentRDBMS = GetUserSettingsSQLSyntax(dbname, "base_form_group")
	GetUserSettingsConnection dbname, "base_form_group", "base_connection"
	ADOProvider = UserSettingConn.Provider
	NewHitlistID = GetNewHitlistID(dbkey, formgroup, user_id, "TEMP", "CSDO")
	'May need to translate rowid into basetable pk
	basetable = GetBaseTable2(dbkey, formgroup, "basetable")
	baseTablePK = GetTableVal(dbkey, basetable, kPrimaryKey)
	
	Select Case UCase(operation)
		Case "UNION"
			Select Case(currentRDBMS)
				Case "ORACLE"
					sql =	"INSERT INTO " & hitListTableName & " (hitlistID, ID) " &_
							"SELECT '" & NewhitlistID & "' AS hitlistID, L1.ID FROM " & hitListTableName1 & " L1 " &_
							"WHERE L1.hitlistID = ? " &_
							"UNION "
					if NOT (Ucase(listType) = "USER" AND UseCartridge(dbkey, formgroup))  then		
						sql = sql &	"SELECT '" & NewhitlistID & "' AS hitlistID, L2.ID FROM " & hitListTableName2 & " L2 " &_
							"WHERE L2.hitlistID = ? " 
					else		
						sql= sql & 	"SELECT '" & NewhitlistID & "' AS hitlistID, b.rowid AS ID FROM " & baseTable & " b, " & hitListTableName2 & " L2 " &_
							"WHERE b." & baseTablePK & "= L2.ID AND L2.hitlistID = ? "
					end if
				Case "SQLSERVER"
					sql =	"INSERT INTO " & hitListTableName & " (hitlistID, ID) " &_
							"SELECT '" & NewhitlistID & "' AS hitlistID, L1.ID FROM " & hitListTableName1 & " L1 " &_
							"WHERE L1.hitlistID = ? " &_
							"UNION " &_
							"SELECT '" & NewhitlistID & "' AS hitlistID, L2.ID FROM " & hitListTableName2 & " L2 " &_
							"WHERE L2.hitlistID = ? " 
				Case "ACCESS"
					' Using insert into selecting from a UNION query did not work in access
					' So we insert the first list
					sql =	"INSERT INTO " & hitListTableName & " (hitlistID, ID) " &_
							"SELECT '" & NewhitlistID & "', ID FROM " & hitListTableName1 & " WHERE hitlistid= ? "
			
					Set Cmd = Server.CreateObject("ADODB.COMMAND")
	
					Cmd.ActiveConnection = UserSettingConn
					Cmd.CommandType = adCmdText
					Cmd.CommandText = sql
	
					Cmd.Parameters.Append Cmd.CreateParameter("pID1", 5, 1, 0, hitlistID1) 

					Cmd.Execute numRecsInserted_temp		
			
					' and then insert the second one minus the first one		
					sql =  	"INSERT INTO " & hitListTableName & " (hitlistID, ID) " &_
							"SELECT '" & NewhitlistID & "', L2.ID FROM " & hitListTableName2 & " L2 " &_
							"WHERE NOT EXISTS (" &_
							"SELECT 1 FROM " & hitListTableName1 & " L1 WHERE L1.hitlistID =? AND L1.ID=L2.ID)" &_
							"AND L2.hitlistID = ?"	
			End select
		Case "SUBTRACT"
			Select Case(currentRDBMS)
				Case "ORACLE"
					sql =	"INSERT INTO " & hitListTableName & " (hitlistID, ID) " &_
							"SELECT '" & NewhitlistID & "' AS hitlistID, L1.ID FROM " & hitListTableName1 & " L1 " &_
							"WHERE L1.hitlistID = ? " &_
							"MINUS "
							
					if NOT (Ucase(listType) = "USER" AND UseCartridge(dbkey, formgroup))then		
						sql= sql & "SELECT '" & NewhitlistID & "' AS hitlistID, L2.ID FROM " & hitListTableName2 & " L2 " &_
							"WHERE L2.hitlistID = ? " 
					else		
						sql= sql & 	"SELECT '" & NewhitlistID & "' AS hitlistID, b.rowid AS ID FROM " & baseTable & " b, " & hitListTableName2 & " L2 " &_
							"WHERE b." & baseTablePK & "= L2.ID AND L2.hitlistID = ? "
					end if
							 
				Case "ACCESS", "SQLSERVER"
					sql =	"INSERT INTO " & hitListTableName & " (hitlistID, ID) " &_
							"SELECT '" & NewhitlistID & "' AS hitlistID , L1.ID AS ID FROM " & hitListTableName1 & " L1 " &_
							"WHERE L1.hitlistID = ? " &_
							"AND NOT EXISTS (SELECT 1 FROM " & hitListTableName2 & " L2 WHERE L2.hitlistID =? AND L2.ID=L1.ID)"	
				
				'for some reason the JOLT provider is flipping the order of the parameters for this statement
				'we need to detect this case and fix below
				if (ADOProvider = "Microsoft.Jet.OLEDB.4.0") then bFlipParams = true
			End select	
		Case "INTERSECT"
			Select Case(currentRDBMS)
				Case "ORACLE"
					sql =	"INSERT INTO " & hitListTableName & " (hitlistID, ID) " &_
							"SELECT '" & NewhitlistID & "' AS hitlistID, L1.ID FROM " & hitListTableName1 & " L1 " &_
							"WHERE L1.hitlistID = ? " &_
							"INTERSECT "
							
					if NOT (Ucase(listType) = "USER" AND UseCartridge(dbkey, formgroup)) then		
						sql= sql & "SELECT '" & NewhitlistID & "' AS hitlistID, L2.ID FROM " & hitListTableName2 & " L2 " &_
							"WHERE L2.hitlistID = ? " 
					else		
						sql= sql & 	"SELECT '" & NewhitlistID & "' AS hitlistID, b.rowid AS ID FROM " & baseTable & " b, " & hitListTableName2 & " L2 " &_
							"WHERE b." & baseTablePK & "= L2.ID AND L2.hitlistID = ? "
					end if
				Case "ACCESS", "SQLSERVER"
					sql =	"INSERT INTO " & hitListTableName & " (hitlistID, ID) " &_
							"SELECT '" & NewhitlistID & "', L1.ID FROM " & hitListTableName1 & " L1, " & hitListTableName2 & " L2 " &_
							"WHERE L1.ID = L2.ID " &_
							"AND L1.hitlistid = ? " &_
							"AND L2.hitlistid = ?"
			End select
	End Select
	
	
	Set Cmd = Server.CreateObject("ADODB.COMMAND")
	
	Cmd.ActiveConnection = UserSettingConn
	Cmd.CommandType = adCmdText
	Cmd.CommandText = sql
	
	if bFlipParams then
		Cmd.Parameters.Append Cmd.CreateParameter(, 5, 1, 0, hitlistID2) 
		Cmd.Parameters.Append Cmd.CreateParameter(, 5, 1, 0, hitlistID1)
	else
		Cmd.Parameters.Append Cmd.CreateParameter(, 5, 1, 0, hitlistID1) 
		Cmd.Parameters.Append Cmd.CreateParameter(, 5, 1, 0, hitlistID2)
	End if
	bDebugHitlistSQL = true
	if bDebugHitlistSQL then
		logAction "Hitlist Operation: " & operation & " SQL: " & sql & vblf & "Params: " & hitlistId1 & " and " & hitlistid2
	end	if
	Cmd.Execute numRecsInserted
	
	Session("hitlistrecordcount" & dbkey & formgroup)= numRecsInserted + numRecsInserted_temp
	Cmd.CommandText =	"UPDATE " & hitListIDTableName & " SET NUMBER_HITS=" & numRecsInserted + numRecsInserted_temp &_
						" WHERE ID=" & NewHitlistID
	Cmd.Execute
	CreateNewHitList = NewHitlistID					
End Function

'****************************************************************************************
'*	NAME: AddSingleHitToHitlist(dbkey, formgroup, hitlistID, marked_hit)
'*	PURPOSE: Adds a single value to a hitlist                    
'*	INPUT: dbkey and formgroup, hitlistID and ID of value to check
'*  RETURNS: boolean                  			
'****************************************************************************************
Function AddSingleHitToHitlist(dbkey, formgroup, hitlistID, marked_hit, tableType)
	Dim sql
	Dim hitListIDTableName
	Dim numRecsInserted
	GetUserSettingsConnection dbkey, formgroup, "base_connection"
	if CheckHitValueExists(dbkey, formgroup, hitlistID, marked_hit, "USER") then
		 AddSingleHitToHitlist = false
		' EXIT POINT
	End if
	hitListTableName = GetFullTableName(dbkey, formgroup, tableType & "HITLIST")
	
	sql =	"INSERT INTO " & hitlistTableName &_ 
			" (HITLISTID, ID) VALUES (? , ?)"
	
	Set Cmd = Server.CreateObject("ADODB.COMMAND")
	Cmd.ActiveConnection = UserSettingConn
	Cmd.CommandType = adCmdText
	Cmd.CommandText = sql
	
	Cmd.Parameters.Append Cmd.CreateParameter("phitlistID", 5, 1, 0, hitlistID) 
	Cmd.Parameters.Append Cmd.CreateParameter("pID", 5, 1, 0, marked_hit)
	
	Cmd.Execute numRecsInserted
	
	if IsEmpty(numRecsInserted) OR numRecsInserted = 0 then
		AddSingleHitToHitlist = false
	Else
		AddSingleHitToHitlist = true
	End if
End function

'****************************************************************************************
'*	NAME: CheckHitValueExists(dbkey, formgroup, hitlistID, ID)
'*	PURPOSE: Checks if a given value has already been saved to a hitlist                    
'*	INPUT: dbkey and formgroup, hitlistID and ID of value to check
'*  RETURNS: boolean                  			
'****************************************************************************************
Function CheckHitValueExists(dbkey, formgroup, hitlistID, ID, tableType)
	Dim hitListIDTableName
	GetUserSettingsConnection dbkey, formgroup, "base_connection"
	hitListTableName = GetFullTableName(dbkey, formgroup,tableType & "HITLIST")
	
	sql =	"SELECT HITLISTID FROM " & hitlistTableName &_
			" WHERE HITLISTID = ? AND ID = ?"
	
	Set Cmd = Server.CreateObject("ADODB.COMMAND")
	Cmd.ActiveConnection = UserSettingConn
	Cmd.CommandType = adCmdText
	Cmd.CommandText = sql
	
	Cmd.Parameters.Append Cmd.CreateParameter("phitlistID", 5, 1, 0, hitlistID) 
	Cmd.Parameters.Append Cmd.CreateParameter("pID", 5, 1, 0, ID)
	
	Set RS = Cmd.Execute
	
	if RS.BOF AND RS.EOF then
		CheckHitValueExists =  false
	Else
		CheckHitValueExists = true
	End if
End function


'****************************************************************************************
'*	NAME: GetNewHitlistID(dbkey, formgroup, user_id, hitlistName)
'*	PURPOSE: Creates a new hitlist by inserting into CSDOHitlistID table                    
'*	INPUT: dbkey and formgroup, user_id and list name
'*  RETURNS: hitlistID value for the newly created list                  			
'****************************************************************************************
Function GetNewHitlistID(dbkey, formgroup, user_id, hitlistName, tableType)
	Dim RS
	Dim hitListIDTableName
	GetUserSettingsConnection dbkey, formgroup, "base_connection"
	hitListIDTableName = GetFullTableName(dbkey, formgroup, tableType & "HITLISTID")
	currentRDBMS = GetUserSettingsSQLSyntax(dbkey, "base_form_group")
	Set RS = Server.CreateObject("ADODB.RECORDSET")
	if UCase(currentRDBMS) = "ORACLE" then
		sql = "select " & tableType & "HITLISTID_SEQ.NEXTVAL as HITLISTID from Dual"
		RS.Open sql, UserSettingConn, adOpenKeyset,  adLockOptimistic, adCmdTxt
		hitlist_id =RS("HITLISTID")
		RS.Close
	end if
	RS.Open hitListIDTableName, UserSettingConn, adOpenKeyset,  adLockOptimistic, adCmdTable 
	if Not (RS.EOF and RS.BOF) then
		RS.MoveLast
	end if
		RS.AddNew
		RS("NAME").Value = hitlistName
		RS("FORMGROUP").Value = Ucase(formgroup)
		RS("DATE_CREATED").Value = Now()
		RS("USER_ID").Value = Ucase(user_id)
		RS("IS_PUBLIC").Value = "0"
	if Ucase(GetUserSettingsSQLSyntax(dbkey, formgroup)) = "ORACLE" then
		RS("ID") = hitlist_id
		RS.Update
	else
		RS.MoveLast
		hitlist_id=RS("ID")
	end if
	GetNewHitlistID = hitlist_id
End function

'****************************************************************************************
'*	PURPOSE: Sets a cookie for non-login applications                         
'*	INPUT: dbkey and formgroup	                    			
'*	OUTPUT: the value to use for stamping the cookie					
'****************************************************************************************

Function getUserSettingCookieOracle(dbkey, formgroup)
	'if OverrideManageHits = true then exit function
	Dim RS
	Dim the_new_cookie
	GetUserSettingsConnection dbkey, formgroup, "base_connection"
	on error resume next
	Set RS = Server.CreateObject("ADODB.RECORDSET")
	sql = "select USER_ID_SEQ.NEXTVAL as SEQ from Dual"
	RS.Open sql, UserSettingConn, adOpenKeyset,  adLockOptimistic, adCmdTxt
	the_new_cookie=RS("SEQ")
	RS.Close
	getUserSettingCookieOracle=the_new_cookie
end Function 

'****************************************************************************************
'*	NAME: GetHitlistAsString(dbkey, formgroup, hitlistID, tableType)
'*	PURPOSE: Gets a hitlist as a comma delimeted string                    
'*	INPUT: dbkey and formgroup, hitlistID
'*  RETURNS: comma delimited list of hitlist values.  Empty string for nonexisting hitlist                  			
'****************************************************************************************
Function GetHitlistAsString(dbkey, formgroup, hitlistID, tableType)
	Dim hitListIDTableName
	Dim RS
	
	hitListTableName = GetFullTableName(dbkey, formgroup, tableType & "HITLIST")
	GetUserSettingsConnection dbkey, formgroup, "base_connection"
	sql =	"SELECT ID FROM " & hitlistTableName &_
			" WHERE HITLISTID = ?"
	
	Set Cmd = Server.CreateObject("ADODB.COMMAND")
	Cmd.ActiveConnection = UserSettingConn
	Cmd.CommandType = adCmdText
	Cmd.CommandText = sql
	
	Cmd.Parameters.Append Cmd.CreateParameter("phitlistID", 5, 1, 0, hitlistID) 
	
	Set RS = Cmd.Execute
	
	if RS.BOF AND RS.EOF then
		GetHitlistASString = ""
	Else
		strTemp = RS.GetString(2,,,",","")
		GetHitlistASString = Left(strTemp,Len(strTemp)-1)
	End if
End function


Function GetFullTableName(dbkey, formgroup, tableName)
	Dim DB_SCHEMA
	
	DB_SCHEMA = getCoreSchemaPrefix(dbkey, formgroup)
	If DB_SCHEMA <> "" then
		DB_SCHEMA = DB_SCHEMA & "." 
	else
		DB_SCHEMA = ""
	end if
	
	GetFullTableName = DB_SCHEMA & tableName 
End function

Function doSaveHitlist(dbkey,  formgroup, hitlistID, hitlistName, user_id, description,public_flag)
	Dim hitListIDTableName
	Dim thereturn
	Dim RS
	Dim newUserListID

	currentRDBMS = GetUserSettingsSQLSyntax(dbkey, "base_form_group")	
	UserListIDTableName = GetFullTableName(dbkey, formgroup,"USERHITLISTID")
	UserHitsTableName = GetFullTableName(dbkey, formgroup,"USERHITLIST")
	CSDOListIDTableName = GetFullTableName(dbkey, formgroup,"CSDOHITLISTID")
	CSDOHitsTableName = GetFullTableName(dbkey, formgroup,"CSDOHITLIST")
	
	if isEmpty(public_flag) then public_flag = "0"
	GetUserSettingsConnection dbkey, "base_form_group", "base_connection"
	Set Cmd = Server.CreateObject("ADODB.COMMAND")
	Cmd.ActiveConnection = UserSettingConn
	Cmd.CommandType = adCmdText
	on error resume next
	if Not public_flag <> "" then public_flag="0"
	

	Select Case(currentRDBMS)
		Case "ORACLE"
			'support applications that limit display recordsets. The real hitlist recordcount is stored in Session("Lasthitlistrecordcount" & dbkey & formgroup) the display recordcount
			'is reset to match the max number stored in Application("TooManyHitsMaximumRetrievable")
			
			' Get a unique id for the userhilistid table
			sql = "SELECT USERHITLISTID_SEQ.NextVal AS ID FROM Dual"
			Cmd.CommandText = sql
			Set RS= Cmd.Execute
			newUserListID = RS("ID")
			' Insert into userhitlistid table
			sql =	"INSERT INTO " & UserListIDTableName &_
					"(ID,TEMPID,NAME,DESCRIPTION,USER_ID,FORMGROUP,NUMBER_HITS,DATE_CREATED,IS_PUBLIC) VALUES " &_
					"(?,?,?,?,?,?,?,?,?) " 
			Cmd.CommandText = sql
			Cmd.Parameters.Append Cmd.CreateParameter("phitlistID", 5, 1, 0, newUserListID)
			Cmd.Parameters.Append Cmd.CreateParameter("ptempID", 5, 1, 0, hitlistID)
			Cmd.Parameters.Append Cmd.CreateParameter("phitlistName", 200, 1, 250, Trim(hitlistName))
			Cmd.Parameters.Append Cmd.CreateParameter("pDescription", 200, 1, 250, Trim(description))
			Cmd.Parameters.Append Cmd.CreateParameter("pUsername", 200, 1, 250, UCase(user_id))
			Cmd.Parameters.Append Cmd.CreateParameter("pFormgroup", 200, 1,Len(formgroup) + 1,Trim(Ucase(formgroup))) 
			Cmd.Parameters.Append Cmd.CreateParameter("pNumberHits", 5, 1,0,Session("hitlistrecordcount" & dbkey & formgroup)) 
			Cmd.Parameters.Append Cmd.CreateParameter("pDateCreated", 135, 1,30, now()) 
			Cmd.Parameters.Append Cmd.CreateParameter("pisPublic", 200, 1, 1, public_flag)

			Cmd.Execute numRecsInserted
			
			if UseCartridge(dbkey, formgroup) then 
				'Need to translate rowid into basetable pk
				basetable = GetBaseTable2(dbkey, formgroup, "basetable")
				baseTablePK = GetTableVal(dbkey, basetable, kPrimaryKey)
				sql =	"INSERT INTO " & UserHitsTableName &_
						"  ( HITLISTID, ID ) " &_
						" SELECT '" & newUserListID & "' AS HITLISTID, b." & baseTablePK & " AS ID" &_
						" FROM " & CSDOHitsTableName & " c, " & baseTable & " b"&_
						" WHERE c.id = b.rowid" &_ 
						" AND " &  "c.hitlistID = ?"
			else
				sql =	"INSERT INTO " & UserHitsTableName &_
						"  ( HITLISTID, ID ) " &_
						" SELECT '" & newUserListID & "' AS HITLISTID, ID FROM " & CSDOHitsTableName &_
						" WHERE " & CSDOHitsTableName & ".hitlistID = ?"
			
			End if			
		Case "SQLSERVER","ACCESS"
		
			' Insert into userhitlistid table
			sql =	"INSERT INTO " & UserListIDTableName &_
					"(TEMPID,NAME,DESCRIPTION,USER_ID,FORMGROUP,NUMBER_HITS,DATE_CREATED,IS_PUBLIC) VALUES " &_
					"(?,?,?,?,?,?,?,?) " 
			Cmd.CommandText = sql
			Cmd.Parameters.Append Cmd.CreateParameter("ptempID", 5, 1, 0, hitlistID)
			Cmd.Parameters.Append Cmd.CreateParameter("phitlistName", 200, 1, 250, Trim(hitlistName))
			Cmd.Parameters.Append Cmd.CreateParameter("pDescription", 200, 1, 250, Trim(description))
			Cmd.Parameters.Append Cmd.CreateParameter("pUsername", 200, 1, 250, UCase(user_id))
			Cmd.Parameters.Append Cmd.CreateParameter("pFormgroup", 200, 1,Len(formgroup) + 1,Trim(Ucase(formgroup))) 
			Cmd.Parameters.Append Cmd.CreateParameter("pNumberHits", 5, 1,0,Session("hitlistrecordcount" & dbkey & formgroup)) 
			Cmd.Parameters.Append Cmd.CreateParameter("pDateCreated", 135, 1,30, now()) 
			Cmd.Parameters.Append Cmd.CreateParameter("pisPublic", 200, 1, 1, public_flag)

			Cmd.Execute numRecsInserted
			' Get a unique id for the userhilistid table
			sql = "SELECT Max(ID) AS theResult FROM " & UserListIDTableName
			Cmd.CommandText = sql
			Set RS= Cmd.Execute
			newUserListID = RS("theResult")
			
			sql =	"INSERT INTO " & UserHitsTableName &_
			"  ( HITLISTID, ID ) " &_
			" SELECT '" & newUserListID & "' AS HITLISTID, ID FROM " & CSDOHitsTableName &_
			" WHERE " & CSDOHitsTableName & ".hitlistID = ?"
	End select
	
	Set Cmd = nothing
	
	' Copy hits to userhits table
	Set Cmd = Server.CreateObject("ADODB.COMMAND")
	Cmd.ActiveConnection = UserSettingConn
	Cmd.CommandType = adCmdText
	Cmd.CommandText = sql
	Cmd.Parameters.Append Cmd.CreateParameter("phitlistID", 5, 1, 0, hitlistID)
	Trace "doSaveHitlist SQL= " & sql, 9 		
	t0 = timer
	Cmd.Execute numRecsInserted
	
	Trace "Time to save hitlist:(" & numRecsInserted & ")hits: " & timer - t0, 8
	if err.number <> 0 then
		theReturn = "Error in doSaveHitlist for" & HitlistName & ": " & err.number & err.Description
		err.clear()
	else
		theReturn = ""
		Session("HitListExists" & dbkey & formgroup) = addItemToDelimitedList(Session("HitListExists" & dbkey & formgroup), newUserListID, ",")
	end if
	
	
	'LJB 4/12/2004 Update userhitlist table with actual number of hits rather then depending on Session("hitlistrecordcount" & dbkey & formgroup)
	Set Cmd = Server.CreateObject("ADODB.COMMAND")
	Cmd.ActiveConnection = UserSettingConn
	Cmd.CommandType = adCmdText
	
	sql =	"UPDATE " & UserListIDTableName & " SET " &_
			"NUMBER_HITS= ?" &_
			"WHERE ID= ? "
			
	Cmd.CommandText = sql
	Cmd.Parameters.Append Cmd.CreateParameter("pNumHits", 5, 1, 0, numRecsInserted)
	Cmd.Parameters.Append Cmd.CreateParameter("phitlistID", 5, 1, 0, newUserListID)
	Cmd.Execute 
	on error goto 0		
	doSaveHitlist = theReturn
End function

Function doUpdateHitlistIDTable(dbkey, formgroup, hitlistID, hitlistName, user_ID, description,public_flag)
	Dim hitListIDTableName
	Dim thereturn
		
	hitListIDTableName = GetFullTableName(dbkey, formgroup,"USERHITLISTID")
	GetUserSettingsConnection dbkey, "base_form_group", "base_connection"
	
	sql =	"UPDATE " & hitListIDTableName & " SET " &_
			"NAME= ? , " &_
			"DESCRIPTION= ? , " &_
			"USER_ID= ? , " &_
			"IS_PUBLIC= ? " &_ 
			"WHERE ID= ? "

	Set Cmd = Server.CreateObject("ADODB.COMMAND")
	Cmd.ActiveConnection = UserSettingConn
	Cmd.CommandType = adCmdText
	Cmd.CommandText = sql
	 
	Cmd.Parameters.Append Cmd.CreateParameter("phitlistName", 200, 1, 250, Trim(hitlistName))
	Cmd.Parameters.Append Cmd.CreateParameter("pDescription", 200, 1, 250, Trim(description))
	Cmd.Parameters.Append Cmd.CreateParameter("pUsername", 200, 1, 250, UCase(user_id))
	Cmd.Parameters.Append Cmd.CreateParameter("pisPublic", 200, 1, 1, public_flag)
	Cmd.Parameters.Append Cmd.CreateParameter("phitlistID", 5, 1, 0, hitlistID)
	
	Cmd.Execute numRecsInserted
		
	if err.number <> 0 then
		theReturn = "Error in doUpdateHitlistIDTable for" & HitlistName & ": " & err.number & err.Description
		err.clear()
	else
		theReturn = ""
		Session("HitListExists" & dbkey & formgroup) = addItemToDelimitedList(Session("HitListExists" & dbkey & formgroup), hitlistID, ",")
	end if
	doUpdateHitlistIDTable = theReturn		
End function



Function doDeleteHitlist(dbkey, formgroup, hitlistid)
	Dim Cmd
	Dim sql
	Dim ADOProvider

	GetUserSettingsConnection dbkey, "base_form_group", "base_connection"
	hitListIDTableName = GetFullTableName(dbkey, formgroup,"USERHITLISTID")
	
	' NOTE values in USERHitlist are not deleted.  They need to be cleaned up periodically by an schedulled process.
	sql ="DELETE FROM " & hitListIDTableName  &_
		 " WHERE ID = ?" 
	
	Set Cmd = Server.CreateObject("ADODB.COMMAND")
	Cmd.ActiveConnection = UserSettingConn
	Cmd.CommandText = sql
	
	Cmd.Parameters.Append Cmd.CreateParameter("phitlistid", 5, 1,0, hitlistID) 
	Cmd.Execute 
	
	Set Cmd = Nothing
	if err.number <> 0 then
		theReturn = "Error in doDeleteHitlist for id= " & hitlistid & ": " & err.number & err.Description
		err.clear()
	else
		theReturn = ""
	end if
	doDeleteHitlist = theReturn		
End function


'****************************************************************************************
'*	NAME: GetUserHitlists(dbkey, formgroup, user_id, bGetHistoryItems)
'*	PURPOSE: Gets a list of hitlistIDs associated with a user as a comma delimeted string                    
'*	INPUT: dbkey and formgroup,user_id, bGetHistoryItems (set to false to return only saved
'*		   items OR set to true to return only History items)
'*  RETURNS: comma delimited list of hitlistID values.  Empty string if no ids exist                  			
'****************************************************************************************
Function GetUserHitlists(dbkey, formgroup, user_id, bGetHistoryItems)
	Dim hitlistIDTableName
	Dim Cmd
	Dim sql
	Dim RS
	Dim NameOperator
	
	if bGetHistoryItems then
		hitListIDTableName = GetFullTableName(dbkey, formgroup,"CSDOHITLISTID")
	else
		hitListIDTableName = GetFullTableName(dbkey, formgroup,"USERHITLISTID")
	End if
	
	GetUserSettingsConnection dbkey, "base_form_group", "base_connection"

	Set Cmd = Server.CreateObject("ADODB.COMMAND")
	Cmd.ActiveConnection = UserSettingConn
	Cmd.CommandType = adCmdText
	
	sql = "SELECT ID " &_
		" FROM " & hitListIDTableName  &_
		" WHERE " & hitListIDTableName & ".USER_ID= ?" &_
		" AND " & hitListIDTableName & ".FORMGROUP= ?" 
	
	if NOT bGetHistoryItems then
		sql = sql & " AND " & hitListIDTableName & ".NAME<>'USERMARKEDHITS'"
		sql = sql & " OR " & hitListIDTableName & ".IS_PUBLIC='1'"
	end if
	
	Cmd.CommandText = sql
	Cmd.Parameters.Append Cmd.CreateParameter("pUserID", 200, 1, Len(user_id)+1 , Trim(UCase(user_id)))
	Cmd.Parameters.Append Cmd.CreateParameter("pformgroup", 200, 1,Len(formgroup)+1, UCase(formgroup)) 
	on error resume next
	Set RS = Cmd.Execute 
	if err.number <> 0 then
		response.Write "ERROR: hitlist tables for this application are missing. Please reload the application so that these tables can be automatically created."
		response.end
		on error goto 0
	end if
	if (RS.EOF AND RS.BOF) then
		GetUserHitlists = ""	
	Else
		strTemp = RS.GetString(2,,,",","")
		GetUserHitlists = Left(strTemp, Len(strTemp)-1)
	End if
End Function

Sub rememberLastHitlist(dbkey, formgroup)
	if Session("HitListID" & dbkey & formgroup) = "" then Session("HitListID" & dbkey & formgroup) = "0"
	if Session("hitlistRecordCount" & dbkey & formgroup) = "" then Session("hitlistRecordCount" & dbkey & formgroup) = "0"
	if CLng(Session("HitListID" & dbkey & formgroup)) > 0 AND Clng(Session("hitlistRecordCount" & dbkey & formgroup)) > 0 then
		Session("LastHitListID" & dbkey & formgroup) = Session("HitListID" & dbkey & formgroup)
		Session("LastHitListRecordcount" & dbkey & formgroup) = Session("hitlistRecordCount" & dbkey & formgroup)
	End if
End Sub


'****************************************************************************************
'*	NAME: GetCSDOHitlistIDFromUserHitlistID(userHitlistID)
'*	PURPOSE: Used during restore_hitlist to make sure that the list being restored is 
'*			in the CSDOHitlist table.  The function tries to determine if the required list
'*			is still in CSDO table.  If it is it returns its hitlistID.  If it isn't it copies
'*			it there and returns the resulting hitlistID value                   
'*	INPUT: userHitlistID value from UserHitlistID table
'*  RETURNS: hitlistID value from CSDOHitlist table                  			
'****************************************************************************************
Function GetCSDOHitlistIDFromUserHitlistID(dbkey, formgroup, userHitlistID)
	Dim tempHitlistID,markedHitsID
	' if the id is for marked hits then always copyd the data to csdohitlist from userhitlist
	if not Session("markedHitsListID" & dbkey & formgroup) = "" then
		on error resume next
		markedHitsID = Clng(Session("markedHitsListID" & dbkey & formgroup))
		if err.number <> 0 then
			markedHitsID = 0
		end if
	else
		markedHitsID = 0
	end if
	if userHitListID = markedHitsID then
			GetCSDOHitlistIDFromUserHitlistID = CopyUserHitlistToCSDOHitlist(dbkey, formgroup, userHitlistID)
	else
		tempHitlistID = CheckTempHitlistStillExists(dbkey, formgroup,userHitlistID)
		if tempHitlistID > 0 then
			GetCSDOHitlistIDFromUserHitlistID = tempHitlistID		 			
		else
			GetCSDOHitlistIDFromUserHitlistID = CopyUserHitlistToCSDOHitlist(dbkey, formgroup, userHitlistID)
		End if
	end if
end function

'****************************************************************************************
'*	NAME: CheckTempHitlistStillExists(dbkey, formgroup, userHitlistID)
'*	PURPOSE: The function tries to determine a copy of the user hitlist still exists
'*			 in the csdohitlist table
'*	INPUT: userHitlistID value from UserHitlistID table
'*  RETURNS: hitlistID value from CSDOHitlist table or Zero if list not found                  			
'****************************************************************************************
Function CheckTempHitlistStillExists(dbkey, formgroup, userHitlistID)
	
	GetUserSettingsConnection dbkey, formgroup, "base_connection"
	
	currentRDBMS = GetUserSettingsSQLSyntax(dbkey, formgroup)	
	UserListIDTableName = GetFullTableName(dbkey, formgroup,"USERHITLISTID")
	UserHitsTableName = GetFullTableName(dbkey, formgroup,"USERHITLIST")
	CSDOListIDTableName = GetFullTableName(dbkey, formgroup,"CSDOHITLISTID")
	CSDOHitsTableName = GetFullTableName(dbkey, formgroup,"CSDOHITLIST")
	
	Set Cmd = Server.CreateObject("ADODB.COMMAND")
	Cmd.ActiveConnection = UserSettingConn
	Cmd.CommandType = adCmdText
	
	' Check that tempID hitlist has the expected number of hits
	sql =	"SELECT u.tempID, u.number_hits, " &_
			"		(SELECT Count(ID) FROM " &  CSDOHitsTableName & " WHERE hitlistID = u.tempID) AS tempHits " &_ 
			"FROM " & UserListIDTableName & " u WHERE u.ID = ?"
	
	Cmd.CommandText = sql
	Cmd.Parameters.Append Cmd.CreateParameter("pHitlistID", 5, 1,0, userHitlistID) 
	
	Set RS = Cmd.Execute 
	
	if (RS.EOF AND RS.BOF) then
		'raise error	
	Else
		if Clng(RS("number_hits").value) = Clng(RS("tempHits").value) then
			CheckTempHitlistStillExists = Clng(RS("tempID").value)
		Else
			CheckTempHitlistStillExists = 0
		End if
	End if
	Set cmd = nothing	
end function

'****************************************************************************************
'*	NAME: CopyUserHitlistToCSDOHitlist(dbkey,  formgroup, userHitlistID)
'*	PURPOSE: Copies a hitlist from user table to csdo table and returns
'*			the newly generated hitlistID value                   
'*	INPUT: userHitlistID value from UserHitlistID table
'*  RETURNS: newly generated hitlistID value from CSDOHitlist table                  			
'****************************************************************************************
Function CopyUserHitlistToCSDOHitlist(dbkey,  formgroup, userHitlistID)
	Dim hitListIDTableName
	Dim thereturn
	Dim RS
	Dim newUserListID
	
	currentRDBMS = GetUserSettingsSQLSyntax(dbkey, "base_form_group")	
	UserListIDTableName = GetFullTableName(dbkey, formgroup,"USERHITLISTID")
	UserHitsTableName = GetFullTableName(dbkey, formgroup,"USERHITLIST")
	CSDOListIDTableName = GetFullTableName(dbkey, formgroup,"CSDOHITLISTID")
	CSDOHitsTableName = GetFullTableName(dbkey, formgroup,"CSDOHITLIST")
	
	
	GetUserSettingsConnection dbkey, "base_form_group", "base_connection"
	Set Cmd = Server.CreateObject("ADODB.COMMAND")
	Cmd.ActiveConnection = UserSettingConn
	Cmd.CommandType = adCmdText
	on error resume next
	Select Case(currentRDBMS)
		Case "ORACLE"
			' Get a unique id for the csdohilistid table
			sql = "SELECT CSDOHITLISTID_SEQ.NextVal AS ID FROM Dual"
			Cmd.CommandText = sql
			Set RS= Cmd.Execute
			newCSDOhitListID = RS("ID")
			
			' copy list data into csdohitlistid table
			sql =	"INSERT INTO " & CSDOListIDTableName &_
					" (ID,NAME,USER_ID,FORMGROUP,NUMBER_HITS,DATE_CREATED) " &_
					"SELECT " & newCSDOhitListID & ", NAME, USER_ID, FORMGROUP,NUMBER_HITS,DATE_CREATED " &_ 
					"FROM " & UserListIDTableName & " WHERE ID=?"
						
			Cmd.CommandText = sql
			Cmd.Parameters.Append Cmd.CreateParameter("phitlistID", 5, 1, 0, userHitlistID)
			Cmd.Execute numRecsInserted
			
			'Need to translate rowid into basetable pk
			basetable = GetBaseTable2(dbkey, formgroup, "basetable")
			baseTablePK = GetTableVal(dbkey, basetable, kPrimaryKey)
			
			if UseCartridge(dbkey, formgroup) then 
				sql =	"INSERT INTO " & CSDOHitsTableName &_
						"  ( HITLISTID, ID ) " &_
						" SELECT '" & newCSDOhitListID & "' AS HITLISTID, b.rowid AS ID" &_
						" FROM " & UserHitsTableName & " u, " & baseTable & " b"&_
						" WHERE u.id = b." & baseTablePK &_ 
						" AND " &  "u.hitlistID = ?"
			else
				sql =	"INSERT INTO " & CSDOHitsTableName &_
						"  ( HITLISTID, ID ) " &_
						" SELECT " & newCSDOhitListID & " AS HITLISTID, ID FROM " & UserHitsTableName &_
						" WHERE " & UserHitsTableName & ".hitlistID = ?"
			End if			
		Case "SQLSERVER","ACCESS"
			' Insert into csdohitlistid table
			sql =	"INSERT INTO " & CSDOListIDTableName &_
					"(NAME,USER_ID,FORMGROUP,NUMBER_HITS,DATE_CREATED) " &_
					"SELECT NAME, USER_ID, FORMGROUP,NUMBER_HITS,DATE_CREATED " &_ 
					"FROM " & UserListIDTableName & " WHERE ID=?"
						
			Cmd.CommandText = sql
			Cmd.Parameters.Append Cmd.CreateParameter("phitlistID", 5, 1, 0, userHitlistID)
			Cmd.Execute numRecsInserted
			' Get a unique id for the csdohitistid table
			sql = "SELECT Max(ID) AS theResult FROM " & CSDOListIDTableName
			Cmd.CommandText = sql
			Set RS= Cmd.Execute
			newCSDOhitListID = RS("theResult")
			
			sql =	"INSERT INTO " & CSDOHitsTableName &_
			"  ( HITLISTID, ID ) " &_
			" SELECT " & newCSDOhitListID & " AS HITLISTID, ID FROM " & UserHitsTableName &_
			" WHERE " & UserHitsTableName & ".hitlistID = ?"
			
	End select
	
	Set Cmd = nothing
	
	' Copy hits to csdohits table
	Set Cmd = Server.CreateObject("ADODB.COMMAND")
	Cmd.ActiveConnection = UserSettingConn
	Cmd.CommandType = adCmdText
	
	
	Cmd.CommandText = sql
	Cmd.Parameters.Append Cmd.CreateParameter("phitlistID", 5, 1, 0, userHitlistID)
	Cmd.Execute numRecsInserted
	
	Set Cmd = nothing
	
	Set Cmd = Server.CreateObject("ADODB.COMMAND")
	Cmd.ActiveConnection = UserSettingConn
	Cmd.CommandType = adCmdText
	
	' Backfill the new tempID into user hitlistID table
	sql = "UPDATE " & UserListIDTableName & " SET tempID = ? WHERE id = ?"
	Cmd.Parameters.Append Cmd.CreateParameter("ptemphitlistID", 5, 1, 0, newCSDOhitListID)  
	Cmd.Parameters.Append Cmd.CreateParameter("puserhitlistID", 5, 1, 0, userHitlistID)
	Cmd.CommandText = sql
	Cmd.Execute numRecsUpdated
	
	if err.number <> 0 then
		theReturn = "Error in CopyUserHitlistToCSDOHitlist for hitlist id= " & userHitlistID & ": " & err.number & err.Description
		err.clear()
	else
		theReturn = ""
		Session("HitListExists" & dbkey & formgroup) = addItemToDelimitedList(Session("HitListExists" & dbkey & formgroup), newCSDOhitListID, ",")
	end if
	on error goto 0		
	CopyUserHitlistToCSDOHitlist = newCSDOhitListID
End function

</SCRIPT>
