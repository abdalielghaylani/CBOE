
<html>
<head>
<title>Document Manager - Search Help</title>
</head>
<body background="<%=Application("UserWindowBackground")%>">

<table width="660" border="0" cellpadding="0" cellspacing="0">
	<tr><td width="100"></td>
		<td width="560">
			<table border="0">
				<tr>
					<td colspan="2"><img src="/docmanager/docmanager/gifs/banner.gif"></td>
				</tr>
			</table><table width="560">
				<tr><td colspan="2" align="left">
						<a href="/<%=Application("appkey")%>/inputtoggle.asp?formgroup=base_form_group&amp;dbname=<%=dbkey%>"><img src="/<%=Application("appkey")%>/graphics/searchdoc_btn.gif" border="0"></a>
						<a href="/docmanager/docmanager/mainpage.asp"><img src="/<%=Application("appkey")%>/graphics/mainmenu.gif" border="0"></a>
					</td>
				</tr>
			</table>
			
			<table>
				<tr>
					<td align="right" width="450"><font color="#000099">You are logged in as <font color="#990000"><%=Session("UserName" & Application("appkey"))%></font></font></td>
					<td align="center"><a href="/<%=Application("appkey")%>/logoff.asp"><img src="/<%=Application("appkey")%>/graphics/log_off_big_btn.gif" border="0"></a></td>
				</tr>
			</table>
				
			
			<br>
			<br>
			
			<table>
				<tr><td><h3 align="left"><b><font size="3">FULL-TEXT SEARCH</font></b></h3>
						<p>Full-Text search allows you complete control over your search 
							using a variety of operators including:
						<p><li>Exact Phrase - Match exact word or phrase
						<br><i>&nbsp;&nbsp;&nbsp;&nbsp;Examples:</i>
						<p><li>Boolean - AND, OR, NOT
						<br>&nbsp;&nbsp;&nbsp;&nbsp;<i>Examples: Oracle AND internet</i>
						<p><li>Wildcard -Match partial word
						<br>&nbsp;&nbsp;&nbsp;&nbsp;<i>Examples: oracl*</i>
						<p><li>Common web query syntax are also supported:
						<p>&nbsp;&nbsp;&nbsp;&nbsp;&quot;x y z&quot; means phrase
							<br>
							&nbsp;&nbsp;&nbsp;&nbsp;-x means x must be absent
						
						<h4>ABOUT SEARCH</h4>
						<p>Syntax: ABOUT(x)
						<br><i>Examples: </i>
						<p>ABOUT search complements full-text search and is useful 
							as you do not have to conform to any specific query fromat or syntax.
						<p>ABOUT query takes the guessing out of text retrieval. It find all 
							the documents related to the query terms in meaning, in addition to 
							those which match query terms letter-by-letter. At the same time, 
							ABOUT is capable of filtering out the majority of irrelevant 
							documents.
						<p>For example, if the query term is baseball, most probably 
							the user does not want to see a document which merely says &quot;Person X 
							was wearing a baseball hat&quot;. ABOUT lowers the ranking of such 
							documents by determining that they lack additional evidence for 
							meanings related to baseball. 
						<p>By eliminating the need to guess exact terms for queries and 
							by filtering out irrelevant text, ABOUT query improves the 
							productivity of the end user in finding relevant texts.
						
					</td>
				</tr>
			</table></td>
	</tr>
	
</table>


</body>
</html>
