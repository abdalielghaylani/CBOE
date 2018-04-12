<%
Option Explicit

%>

<html>
<head>
<title>Log in</title>

</head>

<body background="<%=Application("UserWindowBackground")%>">
<form method="post" action="submitDocs.asp">
<table width="660" border="0" cellpadding="0" cellspacing="0">
	<tr><td width="100"></td>
		<td>
			<table border="0" cellspacing="0" cellpadding="0">
				<tr>
					<td><img src="/DocMgr/gifs/banner.gif"></td>
				</tr>
			</table>

			<table>
				<tr><td valign="top"><table>
							<tr>
								<td>User name:
									<br>
									<input type="text" size="14" name="userName"></td>
							</tr>
							<tr>
								<td>Password:
									<br>
									<input type="text" size="14" name="password"></td>
							</tr>
							<tr>
								<td><input type="submit" value="Log in"></td>
							</tr>
						</table>
					</td>
					
					<td><table>
							<tr><td><font size="4">Welcome to document manager!</font>
									<p>
									If you have an account already, please login to access your private document 
									management account. If you do not have an account yet, please 
									<a href="mailto:syan@camsoft.com">contact administrator</a>. 
									<p>
									<font size="4">With document manager, you can</font>
									<p>
									<li>View and search the shared documents at any place with Internet connection.
										<br>Simply log in to the site. With previlige granted, you can either 
											search the documents by chemistry structure, or by free text.
									<p>
									<li>Share your documents with others having previlige of viewing the documents.
										<br>
										Submit your documents via the web interface after log in. The document
										submitted will be securely stored in database, indexed by both structure and 
										text for other user to search and view.
									<p>
									<li>Remove documents that are no longer shared.
									<p>
									<li>Manage users and groups with administration previlige. Adding and remove 
										users, grant and revoke previliges all via web.
								</td>
								
							</tr>			
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
</form>
</body>
</html>
