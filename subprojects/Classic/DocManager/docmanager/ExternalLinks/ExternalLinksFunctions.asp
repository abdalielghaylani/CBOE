<%
'***********************************inline settings***********************************

Dim connStr
Dim getDocumentInfoRS
Dim connExtDB

' Connection String
connection_array = Application("base_connection" & Application("appkey"))

If Request("csusername") <> "" then
	connStr =  connection_array(0) & "="  & connection_array(1) & ";User ID=" & Request("csusername") & ";Password=" & Request("csuserid")
Elseif Request("ticket") <> "" then
	'this is just for dev testing...this if else can be removed
	if Request("ticket")="E213B78CE0CC3615849C1639EDD019CA41917D494E7582ACFEABC1A1984F405C2A512E630F623B238852A011875FF898C8B2FC869C26B5969F906B1AB817E715F9DE00E5CE3042DDD48E6BD6F592A6AD66FA6B9BAD5F408BFD15D0CCAD244A75" then
		connStr =  "File Name=" & Server.MapPath("/docmanager/config/docmanager.udl") & ";User ID=" & Application("DOCMANAGER_USERNAME") & ";Password=" & Application("DOCMANAGER_PWD")
	else
			set SSOobj = Server.CreateObject("CambridgeSoft.COE.Security.Services.SingleSignOnCom")
			isValidTicket = SSOobj.ValidateTicket(Request("ticket"))
			set SSOobj = Nothing
			if isValidTicket then 
				connStr =  "File Name=" & Server.MapPath("/docmanager/config/docmanager.udl") & ";User ID=" & Application("DOCMANAGER_USERNAME") & ";Password=" & Application("DOCMANAGER_PWD")
			else
				connStr =""
			end if
	end if
Else
	connStr =  connection_array(0) & "="  & connection_array(1) & ";User ID=" & Session("Username" & "docmanager") & ";Password=" & Session("userid" & "docmanager")
End If
DocManagerLogoPath = "/docmanager/docmanager/gifs/banner.gif"


'*******************************

function addLinkToTable(ByVal rextLinkID, ByVal rextAppName, ByVal rLinkType, ByVal rLinkFieldName, ByVal rdocid)
	
	sSQL = "Insert Into DOCMGR.DOCMGR_EXTERNAL_LINKS (APPNAME, LINKTYPE,LINKID,DOCID,LINKFIELDNAME,SUBMITTER,DATE_SUBMITTED) VALUES ('" & rextAppName & "', '" & rLinkType & "', '"  & rextLinkID & "', " & rdocid & ", '" & rLinkFieldName & "', USER, SYSDATE)" 
	
	'Response.Write sSQL

	Set connExtDB = Server.CreateObject("ADODB.Connection")
	connExtDB.Open(connStr)
	
	Set checkAlreadyExists = Server.CreateObject("ADODB.Recordset")
	Set checkAlreadyExists = connextdb.Execute("Select count(DOCID) as matches From DOCMGR.DOCMGR_EXTERNAL_LINKS WHERE APPNAME = '" & rextAppName & "' AND LINKTYPE = '" & rLinkType & "' AND LINKID = '" & rextLinkID & "' AND DOCID = " & rdocid & " AND LINKFIELDNAME = '" & rLinkFieldName & "'")
		
		'Response.Write checkAlreadyExists("matches") & "<br>"
		
		checkstr = cint(checkAlreadyExists("matches"))
		
		'Response.Write checkstr & "<br>"
		
		if checkstr >= 1 then
			'Response.Write "in true" & "<br>"
			documentExists = true
			returnmessage = "DOCUMENTEXISTS"
		else
			'Response.Write "false" & "<br>"
			documentExists = false
		end if
		'Response.Write documentExists  & "<br>"
		'Response.end
		if not documentExists then
		'Response.Write "in if" & "<br>"
			connExtDB.Execute(sSQL)
			returnmessage = "DOCUMENTADDED"
		end if
		
		'Response.Write returnmessage
		
	connExtDB.Close
	
	addLinkToTable = returnmessage
	
end function


function DeleteLinkFromTable(ByVal rextLinkID, ByVal rextAppName, ByVal rLinkType, ByVal rdocid)
	
	'sSQL = "Insert Into DOCMGR.DOCMGR_EXTERNAL_LINKS (APPNAME, LINKTYPE,LINKID,DOCID,LINKFIELDNAME,SUBMITTER,DATE_SUBMITTED) VALUES ('" & rextAppName & "', '" & rLinkType & "', '"  & rextLinkID & "', " & rdocid & ", '" & rLinkFieldName & "', USER, SYSDATE)" 
	sSQL = "Delete from DOCMGR.DOCMGR_EXTERNAL_LINKS WHERE APPNAME = '" & rextAppName & "' AND LINKTYPE = '" & rLinkType & "' AND LINKID = '" & rextLinkID & "' AND DOCID = " & rdocid & ""
	'Response.Write sSQL

	Set connExtDB = Server.CreateObject("ADODB.Connection")
	connExtDB.Open(connStr)

	connExtDB.Execute(sSQL)
	returnmessage = "DOCUMENTDELETED"
		
	connExtDB.Close
	
	DeleteLinkFromTable = returnmessage
	
end function

function GetSearchType(ByVal rAppName, ByVal rLinkType, ByVal rLinkID)
	
	rAppName = trim(rAppName)
	rLinkType = trim(rLinkType)
	rLinkID = trim(rLinkID)
	
	If rAppName = "" and rLinkType = "" and rLinkID = "" then
		
		searchtype = "illegalcall"
		
	elseif rAppName = "" and rLinkType = "" and rLinkID <> "" then
		
		searchtype = "illegalcall"
		
	elseif rAppName <> "" and rLinkType = "" and rLinkID = "" then
		
		searchtype = "alldocsforapp"
		
	elseif rAppName <> "" and rLinkType = "" and rLinkID <> "" then
		
		searchtype = "applinkidsearch"
		
	elseif rAppName = "" and rLinkType <> "" and rLinkID <> "" then
		
		searchtype = "typelinkidsearch"
	
	elseif rAppName <> "" and rLinkType <> "" and rLinkID <> "" then
	
		searchtype = "typelinkidsearch"
	
	End If
	
	GetSearchType = searchtype
	
end function


sub ShowBasicHeader(ByVal rTitle, ByVal bUseLogo)

	If rTitle <> "" then
		rTitle = rTitle
	Else
		rTitle = "ChemMSDX"
	End If
					
	Response.Write "<HTML>"
	Response.Write "<HEAD>"
	Response.Write "<TITLE>" + rTitle + "</TITLE>"
	Response.Write "</HEAD>"
	Response.Write "<BODY leftmargin=3 topmargin=3>"

	If bUseLogo then
		Response.write "<img src=""" + DocManagerLogoPath + """ border=""0"">"
		Response.Write "<br clear=""all""><br clear=""all"">"
	End If
	
end Sub

sub ShowBasicFooter()
	Response.Write "<br><br>"
	Response.Write "</BODY>"
	Response.Write "</HTML>"
end sub

Sub GetExtLinksRS(ByVal rsearchtype, ByVal rextAppName, ByVal rLinkType, ByVal rextLinkID)
	
	sql = "Select DISTINCT DOCMGR.DOCMGR_EXTERNAL_LINKS.APPNAME, DOCMGR.DOCMGR_EXTERNAL_LINKS.LINKTYPE, "
	sql = sql & "DOCMGR.DOCMGR_EXTERNAL_LINKS.LINKID, DOCMGR.DOCMGR_EXTERNAL_LINKS.DOCID, DOCMGR.DOCMGR_EXTERNAL_LINKS.LINKFIELDNAME," 
	sql = sql & "DOCMGR.DOCMGR_EXTERNAL_LINKS.SUBMITTER, DOCMGR.DOCMGR_EXTERNAL_LINKS.DATE_SUBMITTED, DOCMGR.DOCMGR_DOCUMENTS.DOCNAME, DOCMGR.DOCMGR_DOCUMENTS.TITLE"
	sql = sql & " From DOCMGR.DOCMGR_EXTERNAL_LINKS, DOCMGR.DOCMGR_DOCUMENTS"
	joinclause = "DOCMGR.DOCMGR_DOCUMENTS.DOCID = DOCMGR.DOCMGR_EXTERNAL_LINKS.DOCID"
	whereclause = GetWhereClause(rsearchtype, rextAppName, rLinkType, rextLinkID)
	orderbyclause = "ORDER BY upper(APPNAME), upper(LINKTYPE)" 
	
	sql = sql & " WHERE " & joinclause & " AND " & whereclause & " " & orderbyclause

	Set connExtDB = Server.CreateObject("ADODB.Connection")
	connExtDB.Open(connStr)
	
	Set getDocumentInfoRS = Server.CreateObject("ADODB.Recordset")
	Set getDocumentInfoRS = connExtDB.Execute(sql)
	
	
end sub


Sub GetExtLinksForDocMgrRS(ByVal rsearchtype, ByVal rLinkType, ByVal rextLinkID)
	
	sql = "Select DOCMGR.DOCMGR_EXTERNAL_LINKS.APPNAME, DOCMGR.DOCMGR_EXTERNAL_LINKS.LINKTYPE, "
	sql = sql & "DOCMGR.DOCMGR_EXTERNAL_LINKS.LINKID, DOCMGR.DOCMGR_EXTERNAL_LINKS.DOCID, DOCMGR.DOCMGR_EXTERNAL_LINKS.LINKFIELDNAME," 
	sql = sql & "DOCMGR.DOCMGR_EXTERNAL_LINKS.SUBMITTER, DOCMGR.DOCMGR_EXTERNAL_LINKS.DATE_SUBMITTED, DOCMGR.DOCMGR_DOCUMENTS.DOCNAME, DOCMGR.DOCMGR_DOCUMENTS.TITLE"
	sql = sql & " From DOCMGR.DOCMGR_EXTERNAL_LINKS, DOCMGR.DOCMGR_DOCUMENTS"
	joinclause = "DOCMGR.DOCMGR_DOCUMENTS.DOCID = DOCMGR.DOCMGR_EXTERNAL_LINKS.DOCID"
	whereclause = "UPPER(DOCMGR.DOCMGR_EXTERNAL_LINKS.LINKTYPE) = '" & rLinkType & "' AND DOCMGR.DOCMGR_EXTERNAL_LINKS.DOCID = " & rextLinkID & "" 
	orderbyclause = "ORDER BY upper(APPNAME), upper(LINKTYPE)" 
	
	sql = sql & " WHERE " & joinclause & " AND " & whereclause & " " & orderbyclause
	Set connExtDB = Server.CreateObject("ADODB.Connection")
	connExtDB.Open(connStr)
	
	Set getDocumentInfoRS = Server.CreateObject("ADODB.Recordset")
	Set getDocumentInfoRS = connExtDB.Execute(sql)
	
	
end sub


Sub closeExtLinksRS()
	Set getDocumentInfoRS = Nothing	
	connExtDB.close
	Set connExtDB = Nothing	
	
End Sub

function GetWhereClause(ByVal rsearchtype, ByVal rextAppName, ByVal rLinkType, ByVal rextLinkID)

	if rsearchtype = "alldocsforapp" then
	
		whereclause = "APPNAME = '" & rextAppName & "'"
	
	elseif rsearchtype = "applinkidsearch" then
		
		'whereclause = "APPNAME = '" & rextAppName & "' AND LINKID = '" & rextLinkID & "'" 
		whereclause = "APPNAME = '" & rextAppName & "' AND LINKID like '" & rextLinkID & "%'" 
	elseif rsearchtype = "typelinkidsearch" then
		
		'whereclause = "LINKTYPE = '" & rLinkType & "' AND LINKID = '" & rextLinkID & "'" 
		whereclause = "LINKTYPE = '" & rLinkType & "' AND LINKID like '" & rextLinkID & "%'" 
		
	end if
		
	GetWhereClause = whereclause

end function


%>

