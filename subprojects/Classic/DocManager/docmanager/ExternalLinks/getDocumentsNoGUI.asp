<%'@ EnableSessionState=False%>
<!--#INCLUDE File = "ExternalLinksFunctions.asp"-->
<!--#INCLUDE VIRTUAL="/docmanager/docmanager/src/datetimefunc.asp"-->
<%
Response.Expires = -1
'Response.Write "xx" & Request.Form
'Response.end

If connStr = "" then
		Response.ContentType = "text/xml"
		Response.Write "<externalinks></externalinks>"
		response.end
end if
'*******************check for any passed variables********************

'Required Variables in combination
'Allowed Combinations
'rextAppName & rextLinkID
'rLinkType & rextLinkID
'rextAppName & rLinkType & rextLinkID
rextAppName = Request("extAppName")
rLinkType = Request("LinkType")
rextLinkID = Request("extLinkID")



'Optional Variables with default values
rReturnType = Request("ReturnType") 'list, html, xml
rUseHeaders = Request("UseHeaders")
rShowDelete = Request("ShowDelete")
rShowLogo =  Request("showlogo")
rShowSubmitter = Request("ShowSubmitter")
rShowSubmitDate = Request("ShowSubmitDate")



'Set defaults for parameters that are not required
If len(trim(rReturnType)) = 0 then
	rReturnType = "html"
End If

If lcase(rUseHeaders) = "true" or rUseHeaders = true then
	rUseHeaders = true
else
	rUseHeaders = false
End If

If lcase(rShowDelete) = "true" or rShowDelete = true then
	rShowDelete = true
Else
	rShowDelete = false
End if


If lcase(rShowLogo) = "true" or rShowLogo = true then
	rShowLogo = true
Else
	rShowLogo = false
End if

If lcase(rShowSubmitter) = "true" or rShowSubmitter = true then
	rShowSubmitter = true
Else
	rShowSubmitter = false
End if

If lcase(rShowSubmitDate) = "true" or rShowSubmitDate = true then
	rShowSubmitDate = true
Else
	rShowSubmitDate = false
End if

'**************Begin Searching********************

'Test for a the kind of search that must be performed
searchtype = GetSearchType(rextAppName, rLinkType, rextLinkID)

Select Case searchtype
	
	'If an illegal call was made send appropriate illegal call response.
	Case "illegalcall" 
		
		'Return a simple error for list
		If rReturnType = "list" then
		
			Response.Write "-2"
		
		'Return an html error message using header if appropriate	
		Elseif rReturnType = "html" then	
			
			If rUseHeaders then
				Call ShowBasicHeader("Processing error", rShowLogo)
			End If
			
			Response.Write "You have made an illegal call to this page."
			Response.Write "<br><br>"
			Response.Write "This page requires one of the following combinations." 
			Response.Write "<br><br>"
			Response.Write "extAppName &amp; extLinkID"
			Response.Write "<br><br>"
			Response.Write "LinkType &amp; extLinkID"
			Response.Write "<br><br>"
			Response.Write "extAppName &amp; LinkType &amp; extLinkID"
			
			Response.Write "You sent"
			Response.Write "<br><br>"
			Response.Write "appname=" & Request("extAppName")
			Response.Write "<br><br>"
			Response.Write "LinkType=" & Request("LinkType")
			Response.Write "<br><br>"
			Response.Write  "extLinkID=" & Request("extLinkID")
			
			If rUseHeaders then
				Call ShowBasicFooter
			End If
		
		'Return a simple xml message
		Elseif rReturnType = "xml" then
		
			Response.Write "<error>"
			Response.Write "<message>You have made an illegal call</message>"
			Response.Write "</error>"
		End If
		
		Response.End

		
	Case Else
		
		'Response.Write searchtype

		
		Call GetExtLinksRS(searchtype, rextAppName, rLinkType, rextLinkID)
		
		'Get the results and display them appropriately
		If rReturnType = "list" then
		
			while not getDocumentInfoRS.EOF
				returnstring =  getDocumentInfoRS("DOCID").value
				returnstring = returnstring &  ","	
				getDocumentInfoRS.MoveNext
			wend
		
			returnstring = mid(returnstring,1,len(returnstring)-1)
			If len(trim(returnstring)) = 0 then
				returnstring = 0
			End If
			response.write returnstring
		
		elseif rReturnType = "html" then
			
			If rUseHeaders then
				Call ShowBasicHeader("External Links", rShowLogo)
			End If
			
			Response.Write "<scr" & "ipt language=""javascript"">" & chr(13)
			Response.Write "	function DeleteDocManagerLink(rdocid, rlinktype, rlinkid, rappname){" & chr(13)
			Response.Write "	var urltopass = '/docmanager/docmanager/externallinks/processLinks.asp?maction=deleteconfirm&docid=' + rdocid + '&LinkType=' + rlinktype + '&extLinkID='+ rlinkid + '&extAppName=' + rappname" & chr(13)
			Response.Write "	window.open( urltopass, 'docmgrwindow', 'toolbar=no,location=no,scrollbars=yes,width=400,height=300')" & chr(13)
			Response.Write "		}" & chr(13)
			Response.Write "</scr" & "ipt>"
			
			Response.Write "<table cellpadding=3 cellspacing=0 border=1>"
			Response.Write "<tr>"
			Response.Write "<td><b>Document Title</b></td>"
			Response.Write "<td><b>Document Name</b></td>"
			If rShowSubmitter then
				Response.Write "<td><b>Link Submitted by</b></td>"
			End If
			
			If rShowSubmitDate then
				Response.Write "<td><b>Link Submitted on</b></td>"
			End If
			

			If rShowDelete then
				Response.Write "<td><b>Delete Link</b></td>"
			End If
			
			Response.Write "</tr>"
			
			i=0
			while not getDocumentInfoRS.EOF
				Response.Write "<tr>"
				Response.Write "<td>" & getDocumentInfoRS("TITLE").value & "</td>"
				linktodoc = "/docmanager/default.asp?formgroup=base_form_group&dbname=docmanager&formmode_override=edit&dataaction=query_string&field_type=integer&full_field_name=docmgr_documents.docid&field_value=" & getDocumentInfoRS("DOCID").value
				
				Response.Write "<td><a target=""_blank"" href=""" & linktodoc & """>" & getDocumentInfoRS("DOCNAME").value & "</a></td>"
				
				If rShowSubmitter then
					Response.Write "<td>" & getDocumentInfoRS("SUBMITTER").value & "</td>"
				End If
			
				If rShowSubmitDate then
					'return a correctly formatted date
					dt=fmtDateTime(CDATE(getDocumentInfoRS("DATE_SUBMITTED")),Application("DATE_FORMAT_DISPLAY")  & " hh:mm:ss")
					Response.Write "<td>" & dt & "</td>"
				End If
			
				If rShowDelete then
					'stop
					'delete_btn_url = "http://" & Request.ServerVariables("REMOTE_HOST") & Application("AppPathHTTP") & "/graphics/delete_btn.gif"
					delete_btn_url = Application("AppPathHTTP") & "/graphics/delete_btn.gif"
					Response.Write "<td><a href=""#"" onclick=""DeleteDocManagerLink('" & getDocumentInfoRS("DOCID").value & "','" & getDocumentInfoRS("LINKTYPE").value & "','" & getDocumentInfoRS("LINKID").value & "','" & getDocumentInfoRS("APPNAME").value & "')""><img src=""" & delete_btn_url & """ border=""0""></a></td>"
				End If
				
				Response.Write "</tr>"
				
				i= i + 1
				getDocumentInfoRS.movenext
			wend
			
			
			Response.write "</table>"
			
			If i = 0 then
				Response.Write "<span style=""color:red""><b>No document links currently exist.</b></span>"
			End if
			
			If rUseHeaders then
				Call ShowBasicFooter
			End If
			
		
		
		elseif rReturnType = "xml" then
			
			Response.ContentType = "text/xml"
			Response.Write "<externallinks>"
			
			while not getDocumentInfoRS.EOF
				Response.Write "<externallink>"
				Response.Write "<doctitle>" & getDocumentInfoRS("TITLE").value & "</doctitle>"
				
				linktodoc = "/docmanager/default.asp?formgroup=base_form_group&amp;dbname=docmanager&amp;formmode_override=edit&amp;dataaction=query_string&amp;field_type=integer&amp;full_field_name=docmgr_documents.docid&amp;field_value=" & getDocumentInfoRS("DOCID").value
				Response.Write "<doclink>" & linktodoc & "</doclink>"

				Response.Write "<docname>" & getDocumentInfoRS("DOCNAME").value & "</docname>"
				
				Response.Write "<submitter>" & getDocumentInfoRS("SUBMITTER").value & "</submitter>"
				
				'JHS add date formatting to the response
				dt=fmtDateTime(CDATE(getDocumentInfoRS("DATE_SUBMITTED")),Application("DATE_FORMAT_DISPLAY")  & " hh:mm:ss")
				Response.Write "<datesubmitted>" & dt & "</datesubmitted>"
			
				
				delete_url =  "/docmanager/docmanager/externallinks/processLinks.asp?maction=deleteconfirm&amp;docid=" & getDocumentInfoRS("DOCID").value  & "&amp;LinkType=" & getDocumentInfoRS("LINKTYPE").value & "&amp;extLinkID=" & getDocumentInfoRS("LINKID").value & "&amp;extAppName=" & getDocumentInfoRS("APPNAME").value
				Response.Write "<deleteurl>" & delete_url & "</deleteurl>"

				Response.Write "</externallink>"
				
				getDocumentInfoRS.movenext
			wend
			
			
			
			
			Response.write "</externallinks>"
			

		
		end if
		
		Call CloseExtLinksRS
		
		If Request("csusername") <> "" or Request("ticket") <> "" then
			Session.Abandon
		End If
		
		Response.end
End Select






%>