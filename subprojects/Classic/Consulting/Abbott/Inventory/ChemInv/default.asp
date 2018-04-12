<%@ LANGUAGE=VBScript %>
<%
dbkey = Request("dbname")
appkey = Application("appkey")
dataaction = Request("dataaction")
formgroup = Request("formgroup")
formmode = Request("formmode")
Showbanner = Request.QueryString("showbanner")
if IsEmpty(Showbanner) then 
	Showbanner = "True"
	Session("IsPopUP") = false
Elseif NOT Showbanner then
	Session("IsPopUP") = true
End if	

if Application("DBLoaded" & dbkey) = True then

	if formgroup = "batches_form_group" then
		if Session("isCDP") <> "TRUE" then
			formgroup = "batches_np_form_group"	
		end if		
	elseif formgroup = "gs_form_group" then
		if Session("isCDP") <> "TRUE" then
			formgroup = "gs_np_form_group"	
		end if		
	elseif formgroup = "global_substanceselect_form_group" then
		if Session("isCDP") <> "TRUE" then
			formgroup = "global_substanceselect_np_form_group"	
		end if
	end if

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
	if Not Session("CurrentLocation" & dbkey & formgroup) <> "" then
		Session("CurrentLocation" & dbkey & formgroup)=path
	end if
	
	userinfopath = "/" & appkey & "/user_info.asp?dbname=" & dbkey & "&formgroup=" & formgroup
	'Response.Write(userinfopath & "<br>" & path)
	'Response.End


%>
<html>
<head>
<title><%=Application("appTitle")%></title>
</head>
<frameset BORDER="0" FRAMEBORDER="0" FRAMESPACING="0" rows="55,*">
		<frame BORDER="0" MARGINWIDTH="1" MARGINHEIGHT="1" SRC="<%=Application("AppPathHTTP")%>/<%=Application("appkey")%>/bannerframe.asp?showbanner=<%=Showbanner%>&formgroup=<%=formgroup%>" NAME="bannerFrame" scrolling="auto">
		<% 
		'Response.Write("###" & Session("CurrentLocation" & dbkey & formgroup) & "##")
		'Response.End %>
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

Sub LogAction(ByVal inputstr)
		on error resume next
		filepath = Application("ServerDrive") & "\" & Application("ServerRoot") & "\cfwlog.txt"
		Set fs = Server.CreateObject("Scripting.FileSystemObject")
		Set a = fs.OpenTextFile(filepath, 8, True)  
		a.WriteLine Now & ": " & inputstr
		a.WriteLine " "
		a.close
End Sub
	

%>
