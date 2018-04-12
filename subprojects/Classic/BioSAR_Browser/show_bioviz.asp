<%@ LANGUAGE=VBScript  %>
<%
	

%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/manage_user_settings_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_vbs.asp"-->
<%
	'on error resume next
	
	
	dbkey = request("dbname")
	formgroup = request("formgroup")
'	hitlistname = request("hitlistname")

	if Session("hitlistid" & dbkey & formgroup) ="" or Session("hitlistid" & dbkey & formgroup) ="0" then
		Response.Write "<BR><Br>Your hitlist has expired.<BR> You will need to repeat your query in order to be able to export it."
		Response.Write "<BR><BR><center><input type=image src=""graphics\close.gif"" onclick=""window.close()""></center>"
		Response.End
	End if

'if hitlistname <> ""	then
	currentLogin = UCase(Session("UserName" & dbkey))
	bname = currentLogin & "_" & formgroup
	xmlPath ="/CFWTemp/biosar_browser/biosar_browsertemp/sessiondir/" & session.sessionid & "/" & bname & ".bsbxml"

	if Not isObject(Session("TemplateDom")) then 'if it is an object then don't recreate. Only wiped when edittin formgroup or creteing  new search
		Set Session("TemplateDom") = Server.CreateObject("MSXML2.FreeThreadedDOMDocument.4.0")
	end if	
	Set displayDom = Session("TemplateDom")
	
	' DGB Save the hitlist permanently for BIOViZ
	' Save the csdohitlist to userhitlist if we have not already done this before
	' JHS 4/17/2007 - Type mismatch.  So setting them both to CLng
	if Clng(Session("hitlistid" & dbkey & formgroup)) <> CLng(Session("LastSavedHitlistid" & dbkey & formgroup)) then
		Session("SavedHitlistID") = BSBdoSaveHitlist(dbkey,  formgroup, Session("hitlistid" & dbkey & formgroup), "X BioBiz_" & Now() , Session("UserName" & dbKey), "Auto saved during BioBiz display",false)	
		Session("LastSavedHitlistid" & dbkey & formgroup) = Session("hitlistid" & dbkey & formgroup)
	end if
	' Check for error during save	
	if NOT isNumeric(Session("SavedHitlistID")) then
		Response.Write "Error while trying to save hitlist to permanent table"
		Response.end	
	end if
	displayDom.loadXML(Session("chemfinderTemplate"))	
	
	'Inject the userhitlistid into the xml
	Set node = displayDom.SelectSingleNode("//HITLISTID_INFO")
	AddAttributeToElement displayDom, node, "USERHITLISTID", Session("SavedHitlistID")
	' Writeout the file
	displayDom.save(Server.MapPath(xmlPath))
	'Response.write Session("chemfinderTemplate")
	'Response.end

	 'serve the xml file
	 Set objStream = Server.CreateObject("ADODB.Stream")
	 objStream.Open
	 objStream.Type = 1 'adTypeBinary
	 objStream.LoadFromFile Server.MapPath(xmlPath)
	 
	 response.AddHeader "content-disposition", "attachment; filename=" & bname & ".bsbxml"
	 response.contentType = "application/vnd.chemfinder"
	 
	 Response.BinaryWrite objStream.Read
	 Response.Flush
	 
	 objStream.Close
	 Set objStream = Nothing
'else
%>

<!---<html>
	<title>Export hitlist to BioViz</title>
	<body>
		<form name="form1" method="post">
			<input type="hidden" name="dbkey" value="<%=dbkey%>">
			<input type="hidden" name="formgroup" value="<%=formgroup%>">
			Enter a name for the hitlist: <input type="text" name="hitlistname" value="" size="50">
			<input type="submit" value="submit">
		</form>	
	</body>
</html>--->
	
<%
'end if

'/////////////////////////////////////////////////

Function BSBdoSaveHitlist(dbkey,  formgroup, hitlistID, hitlistName, user_id, description,public_flag)
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
	

	'support applications that limit display recordsets. The real hitlist recordcount is stored in Session("Lasthitlistrecordcount" & dbkey & formgroup) the display recordcount
	'is reset to match the max number stored in Application("TooManyHitsMaximumRetrievable")
			
	' Get a unique id for the userhitlistid table
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
			
		'Need to translate rowid into basetable pk
		basetable = GetBaseTable2(dbkey, formgroup, "basetable")
		baseTablePK = GetTableVal(dbkey, basetable, kPrimaryKey)
		sql =	"INSERT INTO " & UserHitsTableName &_
				"  ( HITLISTID, ID ) " &_
				" SELECT '" & newUserListID & "' AS HITLISTID, b." & baseTablePK & " AS ID" &_
				" FROM " & CSDOHitsTableName & " c, " & baseTable & " b"&_
				" WHERE c.id = b.rowid" &_ 
				" AND " &  "c.hitlistID = ?"
	
	Set Cmd = nothing
	
	' Copy hits to userhits table
	Set Cmd = Server.CreateObject("ADODB.COMMAND")
	Cmd.ActiveConnection = UserSettingConn
	Cmd.CommandType = adCmdText
	Cmd.CommandText = sql
	Cmd.Parameters.Append Cmd.CreateParameter("phitlistID", 5, 1, 0, hitlistID)
	Trace "BSBdoSaveHitlist SQL= " & sql, 9 		
	t0 = timer
	Cmd.Execute numRecsInserted
	
	Trace "BSB Time to save hitlist:(" & numRecsInserted & ")hits: " & timer - t0, 8
	if err.number <> 0 then
		theReturn = "Error in BSBdoSaveHitlist for" & HitlistName & ": " & err.number & err.Description
		err.clear()
	else
		theReturn = cLng(newUserListID)
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
	BSBdoSaveHitlist = theReturn
End function

Sub AddAttributeToElement(byRef theDOM,byRef theElement, byVal sAttrName, byval sAttrValue)
	Dim attr
	if Len(sAttrValue) >0  then
		Set attr = theDOM.CreateAttribute(sAttrName)
		theElement.setAttributeNode(attr)
		theElement.setAttribute sAttrName, sAttrValue
	End if
	Set attr = nothing
End Sub
  
%>

