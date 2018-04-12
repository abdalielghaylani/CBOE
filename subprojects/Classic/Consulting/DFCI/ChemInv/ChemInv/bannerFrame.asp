<%
formgroup = Request.QueryString("formgroup")
dbkey="ChemInv"
%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>Banner Frame</title>
		<script language="javascript" type="text/javascript" src="/cheminv/Choosecss.js"></script>
		<script language="javascript" type="text/javascript" src="/cheminv/utils.js"></script>
		<script language="javascript" type="text/javascript">
        <!--Hide JavaScript			
		MainFrame = <%=Application("mainwindow")%>

		function ChangeLocation(){
			document.LocationBar.submit();
		}
		
        //-->
		</script>
	</head>
	<body>
	<form name="LocationBar" METHOD="post" Action="ChangeLocation.asp" Target="HiddenFrame">
		<table border="0" cellspacing="0" cellpadding="2" width="600" align="left">
			<% if CBool(Request.QueryString("ShowBanner")) then%>
			<tr>
				<td valign="top"><a href="/cs_security/home.asp"><img src="<%=Application("NavButtonGifPath")%>cheminventory_banner.gif" border="0"></a></td>
				<td align="right" valign="top" nowrap>
						<%if Application("UseCustomBannerFrameLinks") then%>
							<!--#INCLUDE VIRTUAL = "/cheminv/custom/cheminv/banner_frame_links.asp"-->
						<%else%>
							<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/banner_frame_links.asp"-->
						<%end if%>
						<br><br>
						<%If Application("Admin_required") then%>
							Current login:&nbsp;<%=Ucase(Session("UserNameChemInv"))%>
						<%End if%>
				</td>
			</tr>
			<% end if%>
		</table>
	</form>
	</body>
</html>

