

<HTML>
<HEAD>

		<script language="javascript">
			function launchExtLinkWindow(exturl){
			window.open(exturl, 'extlinkwindow', 'toolbar=no,location=no,scrollbars=yes,width=800,height=600')
			}
		</script>

<!--#INCLUDE FILE="../source/app_vbs.asp"-->
</HEAD>

<BODY>

<!--#INCLUDE VIRTUAL="/docmanager/docmanager/externallinks/externallinksfunctions.asp"-->


<%
'Response.Write "xx" & Application("PrivTableList") & "<br><br>"
'for each test in Session.Contents
'	on error resume next
'	Response.Write test & "=" & Session(test) & "<br><br>"
'next

'Response.end

'for debugging purposes

function replacePlaceHolders(ByVal rInputString, ByVal ReplaceString, ByVal rReplaceVal)
	
		replacePlaceHolders = Replace(rInputString, ReplaceString, rReplaceVal)
	
end function		

'stop
curappname = ""
'Response.Write "<form name=""geturl"">"
'Response.Write "<select>"
arrLinkTypeList = split(Application("SHOW_LINKS_LIST"),",")
totalCount = UBound(arrLinkTypeList)
For i = 0 to totalcount
	currLinkType = UCASE(arrLinkTypeList(i))

	minlinkpriv = Application(currLinkType & "_MINPRIV")
	linkapp = Application(currLinkType & "_LINK_APP")
	linksrc = Application(currLinkType & "_LINK_SRC")
	linktext = Application(currLinkType & "_LINK_TEXT")
	linktitle = Application(currLinkType & "_LINK_TITLE")
	
	Call GetExtLinksForDocMgrRS("typelinkidsearch", currLinkType, Request("unique_id"))

	'SYAN modified on 12/3/2004 to fix CSBR-49503
	if currLinkType = "DRUGDEGPARENTID" then
		headerText = "Links to Drug Deg Parents"
	'jhs added this for drugdeg installations....
	'this is not in a service release anywhere
	elseif currLinkType = "DRUGDEGEXPTID" then
		headerText = "Links to Drug Deg Experiments"	
	elseif currLinkType = "CHEMINVCONTAINERID" then
		headerText = "Links to Inventory Manager Containers"
	elseif currLinkType = "CHEMINVCOMPOUNDID" then
		headerText = "Links to Inventory Manager Compounds"
	else
		headerText = "Links to " & linkapp
	end if

	Response.write "<b>" & headerText & "</b><br>"
	'End of SYAN modification
	
	'SYAN modified on 12/3/2004 to fix CSBR-49503
	if getDocumentInfoRS.BOF and getDocumentInfoRS.EOF then
		if currLinkType = "DRUGDEGPARENTID" then
			displayText = "any DrugDeg parent record"
		elseif currLinkType = "DRUGDEGEXPTID" then
			displayText = "any DrugDeg Experiment record"			
		elseif currLinkType = "CHEMINVCONTAINERID" then
			displayText = "any Inventory container"
		elseif currLinkType = "CHEMINVCOMPOUNDID" then
			displayText = "any Inventory compound"
		elseif currLinkType = "CHEMREGREGNUMBER" then
			displayText = "any Registration compound"
		else
			displayText = currLinkType
		end if
		Response.Write "This document is not linked to " & displayText & "." & "<br>"
	'End of SYAN modification
	else
		ii=0
		'Get the results and display them appropriately		

			'jhs add a var to track whether failure was displayed for special reglookup
			showederrormessage = false
			
		while not getDocumentInfoRS.EOF
					
			if ii=0 then
				if i <> 0 then
					Response.Write "<br>"
				end if
			end if
			'stop
			currentLinkID = getDocumentInfoRS("LINKID").value
			
			'SYAN added on 2/2//2006 to fix CSBR-64105
			if currLinkType = "CHEMREGREGNUMBER" and Application("CHEMREGREGNUMBER_SOURCE_LOOKUP") then
				Set connRegNumber = Server.CreateObject("ADODB.Connection")
				connRegNumber.Open(connStr)
	
				Set checkRegNumberExists = Server.CreateObject("ADODB.Recordset")
				on error resume next
				Set checkRegNumberExists = connRegNumber.Execute("Select count(*) as matches From REGDB.REG_NUMBERS WHERE ROOT_NUMBER = '" & currentLinkID & "'")
				
				if err.number = "" then
					
					regNumberExists = cint(checkRegNumberExists("matches"))
						
					if regNumberExists >= 1 then
						curlinktext = replacePlaceHolders(linktext,"<LINKID>",currentLinkID)
						curlinksrc = replacePlaceHolders(linksrc,"<LINKID>",currentLinkID)
						curlinktitle = replacePlaceHolders(linktitle,"<LINKID>",currentLinkID)
						'Response.Write "xx" & Session(minlinkpriv & "docmanager")
						If Session(minlinkpriv & "DocManager") then
							Response.Write "<a title=""" & curlinktitle & """ href=""#"" onclick=""launchExtLinkWindow('" & curlinksrc & "')"">" & curlinktext & "</a>" & "<br>"
						else
							Response.Write "<a title=""You do not have the appropriate privileges to view " & curlinktext & " on the " & linkapp & " application."" href=""#"" onclick=""alert('You do not have the appropriate privileges to view " & curlinktext & " on the " & linkapp & " application. Please ask the administrators to grant you appropriate privileges and log back in to try again.');return false;"">" & curlinktext & "</a>" & "<br>"
						end if
					end if
						
					checkRegNumberExists.Close	
					connRegNumber.Close
				else
					if not showederrormessage then
						Response.Write "You do not have access to the registration system. The external links could not be displayed"
						showederrormessage = true
					end if
				end if
			else
				curlinktext = replacePlaceHolders(linktext,"<LINKID>",currentLinkID)
				curlinksrc = replacePlaceHolders(linksrc,"<LINKID>",currentLinkID)
				curlinktitle = replacePlaceHolders(linktitle,"<LINKID>",currentLinkID)
				'Response.Write "xx" & Session(minlinkpriv & "docmanager")
				If Session(minlinkpriv & "DocManager") then
					Response.Write "<a title=""" & curlinktitle & """ href=""#"" onclick=""launchExtLinkWindow('" & curlinksrc & "')"">" & curlinktext & "</a>" & "<br>"
				else
					Response.Write "<a title=""You do not have the appropriate privileges to view " & curlinktext & " on the " & linkapp & " application."" href=""#"" onclick=""alert('You do not have the appropriate privileges to view " & curlinktext & " on the " & linkapp & " application. Please ask the administrators to grant you appropriate privileges and log back in to try again.');return false;"">" & curlinktext & "</a>" & "<br>"

				end if
						
				'Response.Write "<a title=""" & curlinktitle & """ href=""#"" onclick=""launchExtLinkWindow('" & curlinksrc & "')"">" & curlinktext & "</a>" & "<br>"
						
				'Response.Write "<option value=" & linkid & ">" & curlinktext & "</option>"
			end if
			'End of SYAN modification	
			

			getDocumentInfoRS.MoveNext
			ii=ii+1
		wend
	End if
		
	Response.Write "<br>"
		
	Call CloseExtLinksRS
Next
	
'Response.Write "</select>"
'Response.Write "</form>"
%>
</BODY>
</HTML>
