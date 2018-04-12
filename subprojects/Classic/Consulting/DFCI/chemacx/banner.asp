<MAP NAME="quicklinks">
	<AREA SHAPE="RECT" COORDS="0,0,125,16" HREF="http://www.chemstore.com" target="_top" Alt="Chemicals, Supplies and Software">
	<AREA SHAPE="RECT" COORDS="126,0,250,16" HREF="http://www.chemfinder.com" target="_top"  Alt="Information and Internet Searching">
	<AREA SHAPE="RECT" COORDS="0,17,125,32" HREF="http://www.chemnews.com" target="_top"  Alt="News, Reviews and Roundups">
	<AREA SHAPE="RECT" COORDS="126,17,250,32" HREF="http://www.chemclub.com" target="_top"  Alt="World Chemistry Community">
	<AREA SHAPE="RECT" COORDS="0,33,250,48" HREF="http://www.cambridgesoft.com" target="_top"  Alt="Life Science Enterprise Solutions">
</MAP>

<script language="JavaScript">
function ACXLogin() { document.location.href = "/chemacx/logoff.asp" }
</script>

<TABLE border="0" valign="top" width="600" cellpadding="2">
<tr>
<td valign="top" width="350">
<% 
if not Session("IsNet") then
	response.write "<a href=""/"" target=""_top""><img src=""" & Application("NavButtonGifPath") & "logo_chemacxproblue_250.gif"" width=""250"" height=""48"" alt="""" border=""0""></a>"
else
	response.write "<a href=""/"" target=""_top""><img src=""" & Application("NavButtonGifPath") & "logo_chemacxnetblue_250.gif"" width=""250"" height=""48"" alt="""" border=""0""></a>"
end if
%>	
</td>
<td align="right">
<% 
if not Session("IsNet") then
	response.write "<a href=""login"" target=""_top"" onclick=""ACXLogin(); return false""><img src=""" & Application("NavButtonGifPath") & "icon_logout_65.gif"" width=""65"" height=""15"" alt="""" border=""0""></a>"
else
	response.write "<a href=""login"" target=""_top"" onclick=""ACXLogin(); return false""><img src=""" & Application("NavButtonGifPath") & "icon_login_65.gif"" width=""65"" height=""15"" alt="""" border=""0""></a>"
end if
%>	
</td>
<td width="250">
	<img src="http://images.cambridgesoft.com/common/sitenav_topblock_250.gif" border="0" usemap="#quicklinks">
</td>					
</tr>
</table>
