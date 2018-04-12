<%Option Explicit%>
<%
dim dbkey

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
						Batch Load Document
					</i></font>
			</td>
		</tr>
	</table>
	
	<table width="760" border="0" cellpadding="0" cellspacing="0">
		<tr><td width="100"></td>
			<td width="660">
				
				<table width="660">
					<tr><td colspan="2" align="left">
							<a href="/<%=Application("appkey")%>/inputtoggle.asp?formgroup=base_form_group&amp;dbname=<%=dbkey%>" target="_top"><img src="/<%=Application("appkey")%>/graphics/searchdoc_btn.gif" border="0"></a>
							<a href="/<%=Application("AppKey")%>/docmanager/src/locateDocs.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/submit_btn.gif" border="0"></a>						
							<a href="/<%=Application("appkey")%>/docmanager/BatchLoadConfigForm.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/batchSubmission_btn.gif" border="0"></a>
							<a href="/<%=Application("appkey")%>/docmanager/RecentActivitiesFrameset.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/recent_activities_btn.gif" border="0"></a>
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

</body>
</html>
