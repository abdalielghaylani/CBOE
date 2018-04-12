<%@LANGUAGE = "VBScript"%>
<html>

<head>
	<title>Delete confirm</title>
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
	<!-- INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
	<!--#INCLUDE FILE="../source/secure_nav.asp"-->
	<!--#INCLUDE FILE="../source/app_js.js"-->
	<!--#INCLUDE FILE="../source/app_vbs.asp"-->
</head>

<body <%=Application("BODY_BACKGROUND")%>>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<!-- end the COWS input form -->
</form>


<%
deltype = Request.QueryString( "deltype" )
dbkey = Request.QueryString( "dbname" )
formgroup = Request.QueryString( "formgroup" )
keyPrimary = Session( "PrimaryKey" & dbkey )

'Response.Write( "<br>deltype = '" & deltype & "'" )
'Response.Write( "<br>dbkey = '" & dbkey & "'" )
'Response.Write( "<br>formgroup = '" & formgroup & "'" )
'Response.Write( "<br>keyPrimary = '" & keyPrimary & "'<br>" )


select case  deltype
	case "parent"
		sConfirmMessage = "Deleting a parent compound also deletes the experiments " & _
			"performed on that parent as well as the degradant compounds produced " & _
			"by those experiments.<p>Do you really want to delete this parent compound?"
		sDeletionCall = "DeleteParent.asp?dbname=" & dbkey & "&formgroup=" & formgroup & "&keyprimary=" & keyPrimary
		sReturnToAfterCancel = Session( "ReturnToParentDetails" & dbkey )
	' end case "parent"

	case "experiment"
		sConfirmMessage = "Deleting a degradation experiment deletes all the " & _
			"degradant compounds produced by that experiment as well.<p>Do you " & _
			"really want to delete this experiment?"
		sDeletionCall = "DeleteExperiment.asp?dbname=" & dbkey & "&formgroup=" & formgroup & "&keyprimary=" & keyPrimary
		sReturnToAfterCancel = Session( "ReturnToExperimentDetails" & dbkey )
	' end case "experiment"

	case "degradant"
		sConfirmMessage = "Are you sure you want to delete this degradant compound?"
		sDeletionCall = "DeleteDegradant.asp?dbname=" & dbkey & "&formgroup=" & formgroup & "&keyprimary=" & keyPrimary
		sReturnToAfterCancel = Session( "ReturnToExperimentDetails" & dbkey )
	' end case "degradant"

	case "mechanism"
		sConfirmMessage = "Are you sure you want to delete this mechanism?"
		sDeletionCall = "DeleteMechanism.asp?dbname=" & dbkey & "&formgroup=" & formgroup & "&keyprimary=" & keyPrimary
		sReturnToAfterCancel = Session( "ReturnToExperimentDetails" & dbkey )
	' end case "mechanism"

	case "condition"
		' The page from which you can delete a condition can have multiple conditions
		' which you may select to delete.  This means that the primary key can not be
		' tucked into a session variable, but rather must be taken from the query string.
		keyPrimary = Request.QueryString( "keyPrimary" )

		' Open a connection for the current conditions list.
		Set connDB = GetNewConnection( dbkey, formgroup, "base_connection" )
		if 0 <> err.number then
			' The connection couldn't be opened.
			Set connDB = nothing
			connDB = ""

			' Redirect to an error dialog.
			Response.Redirect( "db-not-open-error.html" )
		end if

		' Successfully opened the connection to the database.

		' Make a record set for the degradation experiment condition.
		Dim	rsDegCond
		Set rsDegCond = Server.CreateObject( "ADODB.Recordset" )
		sSQL = "select * from DRUGDEG_CONDS where DEG_COND_KEY = " & keyPrimary
		rsDegCond.Open sSQL, connDB

		' Make a message for confirming the deletion.
		sConfirmMessage = "Are you sure you want to delete this condition, &quot;" & _
			rsDegCond.Fields( "DEG_COND_TEXT" ) & "&quot;?"
'		sConfirmMessage = "Are you sure you want to delete this condition?"

		' Close the recordset and database connection.
		rsDegCond.Close
		connDB.Close

		' Set up the call for if the user confirms the deletion.
		sDeletionCall = "DeleteCondition.asp?dbname=" & dbkey & "&formgroup=" & formgroup & "&keyprimary=" & keyPrimary

		' Set up the place to which to go after the user finishes, delete or not.
		sReturnToAfterCancel = Session( "ReturnToConditionListAdmin" & dbkey )
	' end case "condition"

	case "fgroup"
	' The page from which you can delete a condition can have multiple conditions
		' which you may select to delete.  This means that the primary key can not be
		' tucked into a session variable, but rather must be taken from the query string.
		keyPrimary = Request.QueryString( "keyPrimary" )

		' Open a connection for the current conditions list.
		Set connDB = GetNewConnection( dbkey, formgroup, "base_connection" )
		if 0 <> err.number then
			' The connection couldn't be opened.
			Set connDB = nothing
			connDB = ""

			' Redirect to an error dialog.
			Response.Redirect( "db-not-open-error.html" )
		end if

		' Successfully opened the connection to the database.

		' Make a record set for the degradation experiment condition.
		Dim	rsFGroups
		Set rsFGroups = Server.CreateObject( "ADODB.Recordset" )
		sSQL = "select * from DrugDeg_FGroups where DEG_FGROUP_KEY = " & keyPrimary
		rsFGroups.Open sSQL, connDB

		' Make a message for confirming the deletion.
		sConfirmMessage = "Are you sure you want to delete this functional group, &quot;" & _
			rsFGroups.Fields( "DEG_FGROUP_TEXT" ) & "&quot;?"
'		sConfirmMessage = "Are you sure you want to delete this condition?"

		' Close the recordset and database connection.
		rsFGroups.Close
		connDB.Close

		' Set up the call for if the user confirms the deletion.
		sDeletionCall = "DeleteFunctionalGroup.asp?dbname=" & dbkey & "&formgroup=" & formgroup & "&keyprimary=" & keyPrimary

		' Set up the place to which to go after the user finishes, delete or not.
		sReturnToAfterCancel = Session( "ReturnToFunctionalGroupListAdmin" & dbkey )
	' end case "condition"
	case "doclink"
		sConfirmMessage = "Are you sure you want to delete this document link?"
		keyPrimary = Request.QueryString( "link_key" )
		sDeletionCall = "DeleteDocumentLink.asp?dbname=" & dbkey & "&formgroup=" & formgroup & "&keyprimary=" & keyPrimary
		sReturnToAfterCancel = Session( "ReturnToParentDetails" & dbkey )
	' end case "doclink"

	case else
	' end default case

end select
%>

<%=sConfirmMessage%>
<p>
<a href="<%=sDeletionCall%>"><img SRC="/<%=Application( "appkey" )%>/graphics/Button_Delete.gif" border="0"></a>
&nbsp; &nbsp; &nbsp; &nbsp;
<a href="<%=sReturnToAfterCancel%>"><img SRC="/<%=Application( "appkey" )%>/graphics/Button_Cancel.gif" border="0"></a>

</body>

</html>
