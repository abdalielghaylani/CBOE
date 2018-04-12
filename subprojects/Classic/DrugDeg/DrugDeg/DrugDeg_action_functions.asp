<%'Copyright 1998-2001 CambridgeSoft Corporation All Rights Reserved%>
<%
' These are functions private to DrugDeg_action.asp.

'#####
' Validating records to be added within DrugDeg_action.asp.
'#####




Function  SaltCodeIsValid( ByVal sSaltCode )
	' Trim off the spaces.
	sTrimmed = Trim( sSaltCode )

	' Check the size of the trimmed salt code.
	if 2 = Len( sTrimmed ) then
		' Valid codes are two characters long.
		bSaltCodeIsValid = true
	else
		' Not two characters long, so not valid.  Tell the user.
		bSaltCodeIsValid = false
		sAlert = "Salt codes must be two characters long.  For single digit codes, use " & _
			"a zero and then the digit ('05' rather than '5')."
		ShowMessageInAlertDialog( sAlert )
	end if

	SaltCodeIsValid = bSaltCodeIsValid
End function


Function  SaltNameIsValid( ByVal sSaltName )
	' Make sure there are non-space characters in the name.
	sTrimmed = Trim( sSaltName )
	if 0 < Len( sTrimmed ) then
		' There are characters left after we trim off the leading and trailing spaces.
		bSaltNameIsValid = true
	else
		' No characters left after trimming off the leading and trailing spaces, so not valid.
		' Tell the user.
		bSaltNameIsValid = false
		sAlert = "Salt names must have some characters which are not spaces."
		ShowMessageInAlertDialog( sAlert )
	end if

	SaltNameIsValid = bSaltNameIsValid
End function


Function  SaltCodeIsAlreadyInDatabase( ByVal sSaltCode, ByVal dbkey, ByVal formgroup )
	' Open a connection to the database.
	Dim	connDB
	Set connDB = Server.CreateObject( "ADODB.Connection" )
	Set connDB = GetConnection( dbkey, formgroup, "DRUGDEG_SALTS" )

	' Fill a recordset with all the database salts that have the given salt code.
	Dim	rsSalts
	Set rsSalts = Server.CreateObject( "ADODB.Recordset" )
	sSQL = "Select * from DRUGDEG_SALTS where SALT_CODE = '" & sSaltCode & "'"
	rsSalts.Open sSQL, connDB
	if not rsSalts.BOF or not rsSalts.EOF then
		' There is a salt with the same salt code already in the database.
		bSaltCodeIsAlreadyInDatabase = true
	else
		' The code is not already in the database.
		bSaltCodeIsAlreadyInDatabase = false
	end if

	' Close the recordset of salts and the database connection.
	rsSalts.Close
	connDB.Close

	SaltCodeIsAlreadyInDatabase = bSaltCodeIsAlreadyInDatabase
End function


Function  SaltNameIsAlreadyInDatabase( ByVal sSaltName, ByVal dbkey, ByVal formgroup )
	' Open a connection to the database.
	Dim	connDB
	Set connDB = Server.CreateObject( "ADODB.Connection" )
	Set connDB = GetConnection( dbkey, formgroup, "DRUGDEG_SALTS" )

	' Fill a recordset with all the database salts that have the given salt name.
	Dim	rsSalts
	Set rsSalts = Server.CreateObject( "ADODB.Recordset" )
	sSQL = "Select * from DRUGDEG_SALTS where SALT_NAME = '" & sSaltName & "'"
	rsSalts.Open sSQL, connDB
	if not rsSalts.BOF or not rsSalts.EOF then
		' There is a salt with the same salt name already in the database.
		bSaltNameIsAlreadyInDatabase = true
	else
		' The name is not already in the database.
		bSaltNameIsAlreadyInDatabase = false
	end if

	' Close the recordset of salts and the database connection.
	rsSalts.Close
	connDB.Close

	SaltNameIsAlreadyInDatabase = bSaltNameIsAlreadyInDatabase
End function


Function  CondTextIsAlreadyInDatabase( ByVal sCondText, ByVal dbkey, ByVal formgroup )
	' Open a connection to the database.
	Dim	connDB
	Set connDB = Server.CreateObject( "ADODB.Connection" )
	Set connDB = GetConnection( dbkey, formgroup, "DRUGDEG_CONDS" )

	' Fill a recordset with all the degradation conditions in the database that have the
	' user-entered degradation text.
	Dim	rsConds
	Set rsConds = Server.CreateObject( "ADODB.Recordset" )
	sSQL = "Select * from DRUGDEG_CONDS where DEG_COND_TEXT = '" & sCondText & "'"
	rsConds.Open sSQL, connDB
	if not rsConds.BOF or not rsConds.EOF then
		' There is a database entry with the same degradation condition text.
		bCondTextIsAlreadyInDatabase = true
	else
		' The degradation condition text is not already in the database.
		bCondTextIsAlreadyInDatabase = false
	end if

	' Close the recordset of degradation conditions and the database connection.
	rsConds.Close
	connDB.Close

	CondTextIsAlreadyInDatabase = bCondTextIsAlreadyInDatabase
End function

Function  FgroupTextIsAlreadyInDatabase( ByVal sFgroupText, ByVal dbkey, ByVal formgroup )
	' Open a connection to the database.
	Dim	connDB
	Set connDB = Server.CreateObject( "ADODB.Connection" )
	Set connDB = GetConnection( dbkey, formgroup, "DRUGDEG_FGROUPS" )

	' Fill a recordset with all the degradation conditions in the database that have the
	' user-entered degradation text.
	Dim	rsFgroups
	Set rsFgroups = Server.CreateObject( "ADODB.Recordset" )
	sSQL = "Select * from DRUGDEG_FGROUPS where DEG_FGROUP_TEXT = '" & sFgroupText & "'"
	rsFgroups.Open sSQL, connDB
	if not rsFgroups.BOF or not rsFgroups.EOF then
		' There is a database entry with the same degradation condition text.
		bFGroupTextIsAlreadyInDatabase = true
	else
		' The degradation condition text is not already in the database.
		bFGroupTextIsAlreadyInDatabase = false
	end if

	' Close the recordset of degradation conditions and the database connection.
	rsFgroups.Close
	connDB.Close

	FgroupTextIsAlreadyInDatabase = bFGroupTextIsAlreadyInDatabase
End function

Function  StatusTextIsAlreadyInDatabase( ByVal sStatusText, ByVal dbkey, ByVal formgroup )
	' Open a connection to the database.
	Dim	connDB
	Set connDB = Server.CreateObject( "ADODB.Connection" )
	Set connDB = GetConnection( dbkey, formgroup, "DRUGDEG_STATUSES" )

	' Fill a recordset with all the statuses in the database that have the
	' user-entered status text.
	Dim	rsStatus
	Set rsStatus = Server.CreateObject( "ADODB.Recordset" )
	sSQL = "Select * from DRUGDEG_STATUSES where STATUS_TEXT = '" & sStatusText & "'"
	rsStatus.Open sSQL, connDB
	if not rsStatus.BOF or not rsStatus.EOF then
		' There is a database entry with the same status text.
		bStatusTextIsAlreadyInDatabase = true
	else
		' The status text is not already in the database.
		bStatusTextIsAlreadyInDatabase = false
	end if

	' Close the recordset of statuses and the database connection.
	rsStatus.Close
	connDB.Close

	StatusTextIsAlreadyInDatabase = bStatusTextIsAlreadyInDatabase
End function

'#####
' Modifying records from within DrugDeg_action.asp.
'#####

Sub ChangeNameOfDatabaseSalt( ByVal sSaltKey, ByVal sNewName, ByVal dbkey, ByVal formgroup )
	' Open a read-write connection to the database.
	Dim	connDB
	Set connDB = Server.CreateObject( "ADODB.Connection" )
	connDB.Mode = adModeReadWrite
	Set connDB = GetConnection( dbkey, formgroup, "DRUGDEG_SALTS" )

	' Fill a recordset for the salt with the desired key.
	Dim	rsSalt
	Set rsSalt = Server.CreateObject( "ADODB.Recordset" )
	sSQL = "Select * from DRUGDEG_SALTS where SALT_KEY = '" & sSaltKey & "'"
	rsSalt.Open sSQL, connDB
	if not rsSalt.BOF or not rsSalt.EOF then
		' The salt is indeed in the database.
		' Get the old name.
		sOldName = rsSalt.Fields( "SALT_NAME" )

		' Change the name.
		sSQL = "update DRUGDEG_SALTS set SALT_NAME = '" & sNewName & "' where SALT_KEY = '" & sSaltKey & "'"
		connDB.Execute sSQL
		' The following is for Oracle databases.
		connDB.Execute "commit"

		' Tell the user what just happened.
		sAlert = "The name of the salt has been changed from '" & sOldName & "' to '" & sNewName & "'."
		ShowMessageInAlertDialog( sAlert )
	end if

	' Close the salt recordset and the database connection.
	rsSalt.Close
	connDB.Close
End sub


Sub ChangeNameOfDatabaseCondition( ByVal sCondKey, ByVal sNewText, ByVal dbkey, ByVal formgroup )
	' Open a read-write connection to the database.
	Dim	connDB
	Set connDB = Server.CreateObject( "ADODB.Connection" )
	connDB.Mode = adModeReadWrite
	Set connDB = GetConnection( dbkey, formgroup, "DRUGDEG_CONDS" )

	' Fill a recordset with the record which has the input degradation condition key.
	Dim	rsCond
	Set rsCond = Server.CreateObject( "ADODB.Recordset" )
	sSQL = "Select * from DRUGDEG_CONDS where DEG_COND_KEY = '" & sCondKey & "'"
	rsCond.Open sSQL, connDB
	if not rsCond.BOF or not rsCond.EOF then
		' The condition is indeed in the database.
		' Get the old text.
		sOldText = rsCond.Fields( "DEG_COND_TEXT" )

		sSQL = "update DRUGDEG_CONDS set DEG_COND_TEXT = '" & sNewText & "' where DEG_COND_KEY = '" & sCondKey & "'"
		connDB.Execute sSQL
		' The following is for Oracle databases.
		connDB.Execute "commit"

		' Tell the user what just happened.
		sAlert = "The text of the degradation condition has been changed from '" & sOldText & "' to '" & sNewText & "'."
		ShowMessageInAlertDialog( sAlert )
	end if

	' Close the degradation condition recordset and the database connection.
	rsCond.Close
	connDB.Close
End sub

Sub ChangeNameOfDatabaseFGroup( ByVal sDegFGroupKey, ByVal sNewText, ByVal dbkey, ByVal formgroup )
	' Open a read-write connection to the database.
	Dim	connDB
	Set connDB = Server.CreateObject( "ADODB.Connection" )
	connDB.Mode = adModeReadWrite

	Set connDB = GetConnection( dbkey, formgroup, "DRUGDEG_FGROUPS" )

	' Fill a recordset with the record which has the input degradation condition key.
	Dim	rsFGroup
	Set rsFGroup = Server.CreateObject( "ADODB.Recordset" )
	sSQL = "Select * from DRUGDEG_FGROUPS where DEG_FGROUP_KEY = '" & sDegFGroupKey & "'"

	rsFGroup.Open sSQL, connDB
	if not rsFGroup.BOF or not rsFGroup.EOF then
		' The functional group is indeed in the database.
		' Get the old text.
		sOldText = rsFGroup.Fields( "DEG_FGROUP_TEXT" )

		sSQL = "update DRUGDEG_FGROUPS set DEG_FGROUP_TEXT = '" & sNewText & "' where DEG_FGROUP_KEY = '" & sDegFGroupKey & "'"
		connDB.Execute sSQL
		' The following is for Oracle databases.
		connDB.Execute "commit"

		' Tell the user what just happened.
		sAlert = "The text of the functional group has been changed from '" & sOldText & "' to '" & sNewText & "'."
		ShowMessageInAlertDialog( sAlert )
	end if

	' Close the functional group recordset and the database connection.
	rsFGroup.Close
	connDB.Close
End sub

Sub ChangeNameOfDatabaseStatus( ByVal sStatusKey, ByVal sNewText, ByVal dbkey, ByVal formgroup )
	' Open a read-write connection to the database.
	Dim	connDB
	Set connDB = Server.CreateObject( "ADODB.Connection" )
	connDB.Mode = adModeReadWrite
	Set connDB = GetConnection( dbkey, formgroup, "DRUGDEG_STATUSES" )

	' Fill a recordset with the record which has the input degradation condition key.
	Dim	rsStatus
	Set rsStatus = Server.CreateObject( "ADODB.Recordset" )
	sSQL = "Select * from DRUGDEG_STATUSES where STATUS_KEY = '" & sStatusKey & "'"
	rsStatus.Open sSQL, connDB
	if not rsStatus.BOF or not rsStatus.EOF then
		' The condition is indeed in the database.
		' Get the old text.
		sOldText = rsStatus.Fields( "STATUS_TEXT" )

		sSQL = "update DRUGDEG_STATUSES set STATUS_TEXT = '" & sNewText & "' where STATUS_KEY = '" & sStatusKey & "'"
		connDB.Execute sSQL
		' The following is for Oracle databases.
		connDB.Execute "commit"

		' Tell the user what just happened.
		sAlert = "The text of the status has been changed from '" & sOldText & "' to '" & sNewText & "'."
		ShowMessageInAlertDialog( sAlert )
	end if

	' Close the degradation condition recordset and the database connection.
	rsStatus.Close
	connDB.Close
End sub

function getDefaultStatusId
	Dim	connDB
	Set connDB = Server.CreateObject( "ADODB.Connection" )
	connDB.Mode = adModeReadWrite
	Set connDB = GetConnection( dbkey, formgroup, "DRUGDEG_STATUSES" )

	' Fill a recordset with the record which has the input degradation condition key.
	Dim	rsStatus
	Set rsStatus = Server.CreateObject( "ADODB.Recordset" )
	sSQL = "Select STATUS_KEY from DRUGDEG_STATUSES order by STATUS_KEY asc"
	rsStatus.Open sSQL, connDB
	if not rsStatus.eof then
		rsStatus.movefirst
		getDefaultStatusId = rsStatus.Fields("STATUS_KEY")
	else
		getDefaultStatusId = 0
	end if
end function

%>