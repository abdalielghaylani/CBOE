<%
Dim Conn
Dim RS
Session("isSearchMode") = False
dbkey = "ChemInv"
' Store the ASPSessionID cookie identifier.
' Will be used by cs_security logoff page to terminate all sessions
if NOT Session("bASPcookieIDStored") then
	' The only way to get at the ASPSession cookie is by parsing the cookie header
	strCookieHeader = Request.ServerVariables("HTTP_COOKIE")
	start = InstrRev(strCookieHeader, "ASPSESSIONID", -1, 1) + 12
	if start > 12 then
		equalsign = InStr(start, strCookieHeader, "=")
		ASPcookieID= Mid(strCookieHeader, start , equalsign - start)
		CowsASPIDs = Request.Cookies("COWSASPIDS")
		' Store all COWS related ASPCookieIDs in a separate cookie
		if NOT inStr(1, CowsASPIDs, ASPcookieID) > 0 then
			if CowsASPIDs = "" then
				CowsASPIDs = ASPcookieID
			Else
				CowsASPIDs = CowsASPIDs & "," & ASPcookieID
			End if	
			Response.Cookies("COWSASPIDS") = CowsASPIDs
			Response.Cookies("COWSASPIDS").Path = "/"
		End if	 
	end if
	Session("bASPcookieIDStored") = true
end if
%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>Banner Frame</title>
		<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
		<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
		<script language="JavaScript">
		MainFrame = <%=Application("mainwindow")%>;
		function ChangeLocation(){
			document.LocationBar.submit();
		}
		</script>
	</head>
	<body bgcolor="#FFFFFF">
	<form name="LocationBar" METHOD="post" Action="ChangeLocation.asp" Target="HiddenFrame">
		<table border="0" cellspacing="0" cellpadding="2" width="700" align="left">
			<tr>
				<td valign="top"><a href="/cs_security/home.asp"><img src="<%=Application("NavButtonGifPath")%>cheminventory_banner.gif" border="0"></a></td>
				<td align="right" valign="top" nowrap>
					<%if Application("UseCustomBrowseBannerFrameLinks") then%>
						<!--#INCLUDE VIRTUAL = "/cheminv/custom/cheminv/browsebanner_frame_links.asp"-->
					<%else%>
						<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/browsebanner_frame_links.asp"-->
					<%end if%>
				</td>
			</tr>
			<tr>
				<td colspan="2" nowrap bgcolor="cococo">
				Location:<input TYPE="Text" NAME="LocationBox" size="50" onfocus="blur()">&nbsp;<!---<a Class="MenuLink" href="#" onclick="ChangeLocation(); return false;">Change Location</a>&nbsp;-->
				</td>
			</tr>
		</table>
	</form>
	<script language="JavaScript">
<!--Hide JavaScript			
		   if (navigator.appName != "Netscape"){
			  if (document.LocationBar.LocationBox){
				document.LocationBar.LocationBox.size = document.LocationBar.LocationBox.size * 2
			  }
		   }
//-->
		</script>
	</body>
</html>

