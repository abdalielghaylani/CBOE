<%@ LANGUAGE=VBScript %>
<%
dbkey = Request("dbname")
appkey = Application("appkey")
dataaction = Request("dataaction")
formgroup = Request("formgroup")
formmode = Request("formmode")
returnaction = Request("returnaction")
Session("returnaction") = returnaction
Showbanner = Request.QueryString("showbanner")
bannerRows = 55
if IsEmpty(Showbanner) then 
	Showbanner = "True"
	Session("IsPopUP") = false
Elseif NOT Showbanner then
	'-- get rid of empty space when banner not shown
	bannerRows = 1
	Session("IsPopUP") = true
End if	

if Application("DBLoaded" & dbkey) = True then
	userinfopath = "/" & appkey & "/user_info.asp?dbname=" & dbkey & "&formgroup=" & formgroup
	if request.Querystring("dataaction")= "query_string" then
		Session("CurrentLocation" & dbkey & formgroup) = ""
		path= "/" & appkey  & "/" &  dbkey & "/" & dbkey & "_action.asp?" & request.querystring
	else
		if formmode = "" then
			thearray = Application(formgroup & dbkey)
			CONST kInputFormMode = 3
			formmode =thearray(kInputFormMode)
		End if
		path = "/" & appkey  & "/" &  dbkey & "/" & dbkey & "_action.asp" & "?formgroup=" & formgroup & "&dataaction=db&formmode=" & formmode & "&dbname=" & dbkey
	end if
	if (Not Session("CurrentLocation" & dbkey & formgroup) <> "")  or Request("ClearCurrentLocation") = "1" then
		Session("CurrentLocation" & dbkey & formgroup)=path
	end if
%>
<html>
<head>
<title><%=Application("appTitle")%></title>
<script language="javascript">
</script>
</head>
<frameset BORDER="0" FRAMEBORDER="0" FRAMESPACING="0" rows="<%=bannerRows%>,*">
		<frame BORDER="0" MARGINWIDTH="1" MARGINHEIGHT="1" SRC="<%=Application("AppPathHTTP")%>/<%=Application("appkey")%>/bannerframe.asp?showbanner=<%=Showbanner%>&formgroup=<%=formgroup%>" NAME="bannerFrame" scrolling="auto">
			<frameset BORDER="0" FRAMEBORDER="0" FRAMESPACING="0" rows="110,*">
				<frameset border="0" frameborder="0" framespacing="0" cols="580,*">
			  		<frame BORDER="0" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="no" SRC="loading.html" NAME="navbar">	
					<frame BORDER="0" MARGINWIDTH="1" MARGINHEIGHT="1" SRC="loading.html" NAME="userinfo" scrolling="no">	
				</frameset>
				<frame scrolling="auto" BORDER="0" MARGINWIDTH="1" MARGINHEIGHT="1" SRC="<%=Session("CurrentLocation" & dbkey & formgroup)%>" NAME="main">			
			</frameset>
 <noframes>
 <body>
 	This page requires frames
 </body>
</noframes>
</frameset>
</html>
<%
else 'db not loaded
	Response.Write "The web database requested did not initialize.<br>"
	Response.Write "For details, see the log file at <a href=" & Application("AppPathHTTP") & "/logfiles/" & dbkey & "log.html" & ">" & Application("AppPathHTTP") & "/logfiles/" & dbkey & "log.html</a>"
end if
%>
