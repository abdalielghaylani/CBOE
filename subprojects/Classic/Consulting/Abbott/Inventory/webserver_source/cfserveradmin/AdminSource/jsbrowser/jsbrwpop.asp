<%@ LANGUAGE=VBScript %>
<% Option Explicit %>
<% Response.Expires = 0 %>

<% 


Const L_TITLE_TEXT="ChemFinder Files On Server File System"

Dim pg

pg=Request.QueryString("pg")
%>

<HTML>
<HEAD>
<TITLE><%= L_TITLE_TEXT %></TITLE>
</HEAD>
<% if Instr(Request.ServerVariables("HTTP_USER_AGENT"),"MSIE") then %>
<FRAMESET ROWS="100%" COLS="100%"  BORDER=NO FRAMESPACING=1 FRAMEBORDER=0>
	<FRAME SRC="<%= pg %>" NAME="main" SCROLLING=NO FRAMESPACING=0 BORDER=NO  MARGINHEIGHT=5 MARGINWIDTH=5>
</FRAMESET>
<% else %>
<FRAMESET ROWS="*,0" COLS="100%" BORDER=NO FRAMESPACING=1 FRAMEBORDER=0>
	<FRAME SRC="<%= pg %>" NAME="main" SCROLLING=NO FRAMESPACING=0 BORDER=NO MARGINHEIGHT=5 MARGINWIDTH=5>
</FRAMESET>
<% end if %>
</HTML>
