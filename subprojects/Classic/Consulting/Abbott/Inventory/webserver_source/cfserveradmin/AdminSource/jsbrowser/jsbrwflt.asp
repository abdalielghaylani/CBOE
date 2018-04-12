<%@ LANGUAGE=VBScript %>
<% Response.Expires = 0 %>

<% 
Const L_FILENAME_TEXT = "File name:"
Const L_OPEN_TEXT = "OK"
Const L_CANCEL_TEXT = "Cancel"

 %>

<HTML>
<HEAD>
	<TITLE></TITLE>
</HEAD>

<BODY BGCOLOR="#CCCCCC">
<FORM NAME="userform"><FONT FACE="Helv" SIZE = 1>	
		<TABLE WIDTH = 100% >
		<TR>
			<TD>
				<FONT FACE="Helv" SIZE = 1>	
					<%= L_FILENAME_TEXT %>
				</FONT>
			</TD>
			<TD>
				 <INPUT NAME="currentFile" SIZE = 45>
			</TD>
			<TD ALIGN="right">
				
				<INPUT TYPE="Button" VALUE="&nbsp;<%= "  " & L_OPEN_TEXT & "   " %>&nbsp;&nbsp;" onClick="parent.head.listFunc.setPath();">
			</TD>
		</TR>
		<TR>
			<TD COLSPAN = 2>
			<TD ALIGN="right">
				<INPUT TYPE="Button" VALUE="<%= L_CANCEL_TEXT %>" onClick = "top.location.href = 'JSBrwCl.asp';">
			</TD>
		</TR>
		</TABLE>
</FORM>


</BODY>
</HTML>
