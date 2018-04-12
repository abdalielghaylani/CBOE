<%@ Language=VBScript %>
<HTML>
<HEAD>
<META NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
</HEAD>
<BODY <%=Application("BODY_BACKGROUND")%>>

<P><font face="Arial" size="3" color="Red">
You have been automatically logged out of the system<br>
due to a period of inactivity, please sign in again<br>
on the main screen.</font></P>
<P>
<a href="javascript:location.replace('/<%=Application("AppKey")%>/login.asp')">Log In</a>
</P>
</BODY>
</HTML>
