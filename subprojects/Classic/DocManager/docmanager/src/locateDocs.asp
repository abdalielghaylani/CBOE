<%Response.Expires = -1
if Not Session("UserValidated" & dbkey) = 1 then
	'response.redirect "../../login.asp?dbname=" & request("dbname") & "&formgroup=base_form_group&perform_validate=0"
end if
'stop
%>

<%'SYAN modified on 1/16/2006 to fix CSBR-54770%>
<!--#INCLUDE VIRTUAL="/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<%'End of SYAN modification%>

<%
'stop

StoreASPSessionID()

dbkey = "docmanager"
formgroup = "base_form_group"
%>
<html>
<head>
<title>Document submission</title>
<script language="javascript">
	function fileFormatValid() {
		var filePath = document.submitForm.file.value;
		var fileValid = true;
		var fileFormat;
		fileFormat = filePath.substr(filePath.length - 4 , 4);
		fileFormat2 = filePath.substr(filePath.length - 5 , 5);
		if (fileFormat.toLowerCase() != '.doc' 
			&& fileFormat.toLowerCase() != '.txt'
			&& fileFormat.toLowerCase() != '.xls'
			&& fileFormat.toLowerCase() != '.pdf'
			&& fileFormat.toLowerCase() != '.ppt'
			&& fileFormat2.toLowerCase() != '.docx'
			&& fileFormat2.toLowerCase() != '.xlsx'
			&& fileFormat2.toLowerCase() != '.pptx') {
			alert('Supported file formats are: *.doc *.docx *.txt *.xls *.xlsx *.ppt *pptx *.pdf');
			fileValid = false;
		}
		return fileValid;
	}
	
	function previewDoc() {
		document.submitForm.action='uploadDocs.asp?toPreview=true';
		document.submitForm.submit();
	}
	
	function submitDoc() {
		document.submitForm.action='uploadDocs.asp?toSubmit=true';
		document.submitForm.submit();
	}
</script>

<!-- CBOE-1823 added code to display Document Manager help on F1 click. Debu 05SEP13 -->
 <script language="javascript" type="text/javascript">
     function onkeydown_handler() {
         switch (event.keyCode) {
             case 112: // 'F1'
                 document.onhelp = function () { return (false); }
                 window.onhelp = function () { return (false); }
                 event.returnValue = false;
                 event.keyCode = 0;
                 window.open('../../../../CBOEHelp/CBOEContextHelp/Doc%20Manager%20Webhelp/Default.htm');
                 return false;
                 break;
         }
     }
     document.attachEvent("onkeydown", onkeydown_handler);
    </script>
</head>

<body background="<%=Application("UserWindowBackground")%>">


	<form method="post" name="submitForm" ENCTYPE="multipart/form-data">
			
			<!---JHS added 4/9/2003--->
			<table border="0" bordercolor="fuchsia" cellspacing="0" cellpadding="0">
				<!-- The table for the banner. -->
				<tr>

					<td valign="top" width="300">
						<img src="/docmanager/docmanager/gifs/banner.gif" border="0">
					</td>

					<td>
							<font face="Arial" color="#0099FF" size="4"><i>
								Add Document
							</i></font>
					</td>
				</tr>
			</table>
			<!---JHS added 4/9/2003 end--->

		<table width="660" border="0" cellpadding="0" cellspacing="0">
			<tr><td width="100"></td>
				<td width="560">
					<!---JHS commented out 4/9/2003			<table border="0">				<tr>					<td colspan="2"><img src="/docmanager/docmanager/gifs/banner.gif"></td>				</tr>			</table>--->
					
					<table width="560">
						<tr><td colspan="2" align="left">
								<a href="/<%=Application("appkey")%>/inputtoggle.asp?formgroup=base_form_group&amp;dbname=<%=dbkey%>" target="_top"><img src="/<%=Application("appkey")%>/graphics/searchdoc_btn.gif" border="0"></a>
								<%if session("BATCHLOAD_DOCS") then%>
								<a href="/<%=Application("appkey")%>/docmanager/BatchLoadConfigForm.asp"><img src="/<%=Application("appkey")%>/graphics/batchSubmission_btn.gif" border="0"></a>
								<%end if%>
								<a href="/<%=Application("appkey")%>/docmanager/RecentActivitiesFrameset.asp"><img src="/<%=Application("appkey")%>/graphics/recent_activities_btn.gif" border="0"></a>
								<a href="/cs_security/home.asp"><img src="/<%=Application("appkey")%>/graphics/home_oval_btn.gif" border="0"></a>
								<a href="/docmanager/docmanager/mainpage.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/mainmenu.gif" border="0"></a>
							</td>
						</tr>
						
						<tr><td>
								<a href="/CBOEHelp/CBOEContextHelp/Doc Manager Webhelp/Default.htm" target="new"><img src="/docmanager/graphics/help_btn.gif" border="0"></a>
								<a href="#" onclick="window.open('/docmanager/about.asp', 'about', 'width=560,height=450,status=no,resizable=yes')"><img src="/cfserverasp/source/graphics/navbuttons/about_btn.gif" border="0"></a>
							</td>
						</tr>
					</table>
					
					<table>
						<tr>
							<td align="right" width="450"><font color="#000099">You are logged in as <font color="#990000"><%=Session("UserName" & Application("appkey"))%></font></font></td>
							<td align="center"><a href="/<%=Application("appkey")%>/logoff.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/log_off_big_btn.gif" border="0"></a></td>
						</tr>
					</table>
						
					
					<br>
					<br>
					
					<!--SYAN added 10/25/2004 to fix CSBR-48313-->
					<%If Request("extLinkID") <> "" then
						rextLinkID  = Request("extLinkID")
					Elseif Session("extLinkID") <> "" then
						rextLinkID  = Session("extLinkID")
					Else
						rextLinkID = ""
					End If
					
					If Request("extAppName") <> "" then
						rextAppName  = Request("extAppName")
					Elseif Session("extAppName") <> "" then
						rextAppName  = Session("extAppName")
					Else
						rextAppName = ""
					End If

					%>
					<%if session("showselect") and session("doc_uid") <> "" then%>
						<table>
							<tr>
								<td colspan="2">
									<%=session("fileName")%> (<%= session("docSize")%> kb) was successfully submitted and indexed, you can add a link to CONTAINER_ID <%=rextLinkID%> for <%=rextAppName%>.
									<a href="/docmanager/docmanager/externalLinks/processLinks.asp?maction=Add&amp;docid=<%=request("docid")%>" target="_top"><img src="/docmanager/graphics/Button_AddDocLink.gif" border="0"></a>
								</td>
							</tr>					
						</table>
						<br>
					<%end if%>
					<!--End of SYAN modification-->
					
						<%if Session("submitFeedback") <> "" then%>
						<table border="0">
							<tr>
								<td><%=Session("submitFeedback")%></td>
							</tr>
							<tr><td height="10"></td></tr>
						</table>
						<%Session("submitFeedback") = ""%>		
						<%end if%>

						<table>
						    <tr bgcolor="#0066cc" height="30">
								<td><font color="white"><b>&nbsp;Document:</b></font>
								</td>	
								
								<td align="right">
									<input type="file" size="40" name="file" accept="image/jpeg">
								</td>
												
								<td>
									<input type="button" name="submitIt" value="Submit Now" onclick="javascript:if (fileFormatValid() == true) {submitDoc();}">
								</td>
											
								<td>
									<input type="button" name="preview" value="Add Document Details" onclick="javascript:if (fileFormatValid() == true) {previewDoc();}">
								</td>
						</table>
					
				</td>
			</tr>
		</table>
		
		<%if Application("ENABLE_REGNUMBER_ENTRY") = true and Session("SEARCH_REGDocManager") = true then%>
		<table >
		<tr bgcolor="#0066cc" height="30">	
			<td>
				<font color="white"><b>&nbsp;Registry No.:</b></font> <input type="text" name="REGNumber" size="15">
			</td>
			<td>
				<font color="white">Enter an existing registry number to create a link to the Registration System</font>
			</td>
		</tr>
		</table>
		<%end if%>
		
		<%'if CBool(Application("RLS")) = true then %>
			<%
			'sql = "SELECT PROJECT_INTERNAL_ID, PROJECT_NAME " & _
			'		"FROM REGDB.PROJECTS, REGDB.PEOPLE_PROJECT, CS_SECURITY.PEOPLE " & _
			'		"WHERE REGDB.PEOPLE_PROJECT.PROJECT_ID = REGDB.PROJECTS.PROJECT_INTERNAL_ID " & _
			'			"AND CS_SECURITY.PEOPLE.PERSON_ID = REGDB.PEOPLE_PROJECT.PERSON_ID " & _
			'			"AND CS_SECURITY.PEOPLE.USER_ID = '" & UCase(Session("UserName" & Application("appkey"))) & "'"
		'else
			'sql = "SELECT PROJECT_INTERNAL_ID, PROJECT_NAME " & _
			'		"FROM REGDB.PROJECTS"
		'end if
		
		'ConnStr = "FILE NAME=" & Application("UDLPath") & ";User ID=" & Session("UserName" & dbkey) & ";Password=" & Session("UserID" & dbkey)
		'Set cnn = Server.CreateObject("ADODB.Connection")
		'cnn.Open ConnStr
		
		'set projectsRS = Server.CreateObject("ADODB.Recordset")
		
		''SYAN modified on 2/22/2006 to fix CSBR-64263
		'on error resume next
		'projectsRS.Open sql, cnn

		'projectPriv = true
		'if InStr(err.Description, "table or view does not exist") > 0 then
		'	projectPriv = false
		'	Set projectsRS = nothing
		'end if
		
		'set prefixRS = Server.CreateObject("ADODB.Recordset")
		'sql = "SELECT * FROM REGDB.SEQUENCE"
		'on error resume next
		'prefixRS.Open sql, cnn
		''End of SYAN modification
		%>	
		<!--table>
			<tr><%'if projectPriv = true then%>
					<%'if not (projectsRS.BOF and projectsRS.EOF) then%>
						<td>
							<select name="RLSProject" size="1">
											
							<%'	projectsRS.MoveFirst
							'	while not projectsRS.EOF
							'%>
									<option value="<%'=projectsRS("PROJECT_INTERNAL_ID")%>"><%'=projectsRS("PROJECT_NAME")%></option>
									<%'projectsRS.MoveNext
								'wend
								'projectsRS.Close%>
							</select>
						</td>
					<%'end if%>
				<%'else%>
					<td>Projects: <a href="#" onclick="alert('You need to be granted a Chemistry Registration Role first to search by projects information. Please ask the administrators to grant you the role and log back in to try again.'); return false;">Insufficient Privileges</a>

					</td>
				<%'end if%>
				
				<td>&nbsp;&nbsp;&nbsp;</td>
				<td>SEP Number: 
				<%'if not (prefixRS.BOF and prefixRS.EOF) then%>
				
					<select name="prefix" size="1">
											
					<%'	prefixRS.MoveFirst
						'while not prefixRS.EOF
					%>
							<option value="<%'=prefixRS("PREFIX") & prefixRS("PREFIX_DELIMITER")%>"><%'=prefixRS("PREFIX") & prefixRS("PREFIX_DELIMITER")%></option>
							<%'prefixRS.MoveNext
						'wend
						'prefixRS.Close%>
					</select>
					<input type="text" name="numberPart" size="15">
				<%'else%>
					<input type="text" name="SEPNumber" size="15">
				<%'end if%>
				</td>

			</tr>
		</table-->
	
	</form>


<%'="here " & Len(Session("fileHexString").HexString)%>

</body>
</html>
			
