<%'Option Explicit%>

<%'SYAN modified on 1/16/2006 to fix CSBR-54770%>
<!--#INCLUDE VIRTUAL="/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<%'End of SYAN modification%>

<%
dim dbkey
dim submissionTime, sortBy, days

StoreASPSessionID()

submissionTime = request("submissionTime")
sortBy = request("sortBy")
days = request("days")
'JHS 1/14/2008 -add a dbkey so session vars work
dbkey = "docmanager"
%>

<html>
<head>
<title>View Logs</title>


</head>

<body background="<%=Application("UserWindowBackground")%>">

	<table border="0" bordercolor="fuchsia" cellspacing="0" cellpadding="0">
		<!-- The table for the banner. -->
		<tr>

			<td valign="top" width="300">
				<img src="/docmanager/docmanager/gifs/banner.gif" border="0">
			</td>

			<td>
					<font face="Arial" color="#0099FF" size="4"><i>
						View Recent Activities
					</i></font>
			</td>
		</tr>
	</table>
	
	<table width="800" border="0" cellpadding="0" cellspacing="0">
		<tr><td width="100"></td>
			<td width="700">
				
				<table width="700">
					<tr><td colspan="2" align="left">
							<a href="/<%=Application("appkey")%>/inputtoggle.asp?formgroup=base_form_group&amp;dbname=<%=dbkey%>" target="_top"><img src="/<%=Application("appkey")%>/graphics/searchdoc_btn.gif" border="0"></a>
							<%if Session("Submit_docs" & dbkey) then%>
							<a href="/<%=Application("AppKey")%>/docmanager/src/locateDocs.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/submit_btn.gif" border="0"></a>						
							<%end if%>
							<%if session("BATCHLOAD_DOCS" & dbkey) then%>
								<a href="/<%=Application("appkey")%>/docmanager/BatchLoadConfigForm.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/batchSubmission_btn.gif" border="0"></a>
							<%end if%>
							<a href="/docmanager/docmanager/mainpage.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/mainmenu.gif" border="0"></a>
							<a href="/cs_security/home.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/home_oval_btn.gif" border="0"></a>
							<a href="/<%=Application("appkey")%>/logoff.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/log_off_big_btn.gif" border="0"></a>
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
					</tr>
				</table>
			</td>
		</tr>
	</table>

	<form name="RecentActivitiesForm" method="post" action="RecentActivitiesFrameset.asp" target="_top">
		<table>
			<tr>
				<td width="100"></td>
				<td>
					<table border="1" bgcolor="#cccccc">
						<tr>
														
							<td valign="top"><table>
									<tr><td>
										Documents Submitted
										</td>
									</tr>
									
									<tr>	
										<td>
											<input type="radio" name="submissionTime" value="today" <%if submissionTime="today" or submissionTime="" then%>checked<%end if%>>Today
										</td>
									</tr>
									
									<tr>
										<td>
											<input type="radio" name="submissionTime" value="lastDays" <%if submissionTime = "lastDays" then%>checked<%end if%>>
											In the last <input type="text" name="days" size="2" value="<%if days = "" then%><%="2"%><%else%><%=days%><%end if%>"> days
										</td>
									</tr>
								</table>
							</td>

							<td width="40">&nbsp;</td>

							<td><table>
									<tr><td>Sort by</td>
									</tr>

									<tr>	
										<td>
											<input type="radio" name="sortBy" value="time" <%if sortBy="time" or sortBy="" then%>checked<%end if%>>Submit Time
										</td>
									</tr>
									
									<tr>	
										<td>
											<input type="radio" name="sortBy" value="submitter" <%if sortBy="submitter" then%>checked<%end if%>>Submitter
										</td>
									</tr>

									<tr>	
										<td>
											<input type="radio" name="sortBy" value="title" <%if sortBy="title" then%>checked<%end if%>>Title
										</td>
									</tr>

									<tr>	
										<td>
											<input type="radio" name="sortBy" value="type" <%if sortBy="type" then%>checked<%end if%>>File Type
										</td>
									</tr>

								</table>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>

		<table width="400">
			<tr><td width="100"></td>
				<td align="right"><a href="javascript:this.document.forms[0].submit();">Show Activities</a>
				</td>
			</tr>
		</table>
	</form>
</body>
</html>
