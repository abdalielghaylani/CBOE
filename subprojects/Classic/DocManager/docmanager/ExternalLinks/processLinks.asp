<!--#INCLUDE File = "ExternalLinksFunctions.asp"-->
<%
'Response.Write connstr
rmaction = Request("maction")

'Check for existence of fields for working with links

If Request("extLinkID") <> "" then
	rextLinkID  = Request("extLinkID")
Elseif Session("extLinkID") <> "" then
	rextLinkID  = Session("extLinkID")
Else
	rextLinkID = ""
End If

If Request("extAppName") <> "" then
	rextAppName  = Request("extAppName")
Elseif Session("extAppName") <> "" then
	rextAppName  = Session("extAppName")
Else
	rextAppName = ""
End If

If Request("LinkType") <> "" then
	rLinkType  = Request("LinkType")
Elseif Session("LinkType") <> "" then
	rLinkType  = Session("LinkType")
Else
	rLinkType = ""
End If

If Request("LinkFieldName") <> "" then
	rLinkFieldName  = Request("LinkFieldName")
Elseif Session("LinkFieldName") <> "" then
	rLinkFieldName  = Session("LinkFieldName")
Else
	rLinkFieldName = ""
End If

If Request("docid") <> "" then
	rdocid  = Request("docid")
Else
	rdocid = ""
End If



If not Session("useReload") then
	reloadmethod = "window.opener.location = window.opener.location"
else
	reloadmethod = "window.opener.location.reload(true)"
end if

If rmaction = "Add" then
	'Response.Write rmaction
	
	'Adding Links requires all of the fields passed
	If rextLinkID = "" or rextAppName = "" or rLinkType = "" or rLinkFieldName = "" or rdocid = "" then
		Response.Write "One of the following values was not supplied."
		Response.Write "<br>"
		Response.Write "rextLinkID= " & rextLinkID
		Response.Write "<br>"
		Response.Write "rextAppName= " & rextAppName
		Response.Write "<br>"
		Response.Write "rLinkType= " & rLinkType
		Response.Write "<br>"
		Response.Write "rLinkFieldName= " & rLinkFieldName
		Response.Write "<br>"
		Response.Write "rdocid= " & rdocid
		
	Else	

		addLinkReturn = addLinkToTable(rextLinkID, rextAppName, rLinkType, rLinkFieldName, rdocid)
		'Response.Write addLinkReturn
		'Response.End
		
		If addLinkReturn = "DOCUMENTEXISTS" then	
		Call ShowBasicHeader("Document Link Already Exists", true)
		%>

				The link you attempted to add already exists.
				<br><br>
				<a href="/docmanager/default.asp?formgroup=base_form_group&dbname=docmanager&extAppName=<%=rextAppName%>&LinkType=<%=rLinkType%>&linkfieldname=<%=rLinkFieldName%>&showselect=true&extlinkid=<%=rextlinkid%>">Add Another Document</a>
				<br><br>
				<a href="processLinks.asp?maction=close">Close Window</a>

		<%
			Call ShowBasicFooter
			Response.End
		elseif addLinkReturn = "DOCUMENTADDED" then
		
			Session("showselect") = false
			Session("extAppName") = ""
			Session("LinkType") = ""
			Session("extLinkID") = ""
			Session("LinkFieldName") = ""
			Session("useReload") = Empty
			%>
			<HTML>
			<HEAD>
			<META NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
			<TITLE></TITLE>
			<script language="javascript">
				function closerefresh(){
						//window.opener.location.reload(true);
					<%=reloadmethod%>;
					
					var agt = navigator.userAgent.toLowerCase();
					var is_ie6 = (agt.indexOf("msie 6.") != -1);
					var is_ie7 = (agt.indexOf("msie 7.") != -1);
					if (is_ie6)
					{
						var oMe = window.self;
						oMe.opener = window.self;
						oMe.close();
					}
					else
					{
						window.open('','_self');
						window.close();
					}
				}
			</script>
			</HEAD>
			<BODY onload="closerefresh()">


			</BODY>
			</HTML>
		<%	'SYAN added on 10/25/2004 to fix CSBR-48313
			Session("doc_uid") = ""
			'End of SYAN modification

			Response.End
		else
		
		end if
	
	End If

elseif rmaction="deleteconfirm" then


if  Request("useReload") = "false" then
	Session("useReload") = false
else
	Session("useReload") = true
end if	

%>
			<HTML>
			<HEAD>
			<META NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
			<TITLE>Delete Confirmation</TITLE>		
			</HEAD>
			<BODY>
				<%
					
				Response.Write "<font face=""Verdana, Arial,Helvetica"" size = ""+1""><b>Delete Confirmation</b></font>" & "<br><br>"
				Response.Write "<font face=""Verdana, Arial,Helvetica"" size = ""-1"">Do you really want to delete the document link between Document ID " & Request.Querystring("docid") & " and " & Request.Querystring("extAppName") & " " & Request.Querystring("extLinkID") & "?</font>"
				
				Response.Write "<br><br>"
				urlfordelete = "/docmanager/docmanager/externallinks/processLinks.asp?maction=delete&docid="  & Request.QueryString("docid") & "&LinkType=" & Request.QueryString("LinkType") & "&extLinkID=" & Request.QueryString("extLinkID") & "&extAppName=" & Request.QueryString("extAppName")
				
				Response.Write "<a href=""" & urlfordelete & """><img src=""/docmanager/graphics/delete_btn.gif"" border=""0""></a>"
				Response.Write "&nbsp;&nbsp;"
				Response.Write "<a href=""#"" onclick=""window.close()""><img src=""/docmanager/graphics/cancel_edit_btn.gif"" border=""0""></a>"
				
				%>

			</BODY>
			</HTML>


<%
elseif rmaction="delete" then

	If rextLinkID = "" or rLinkType = "" or rdocid = "" then
		Response.Write "One of the following values was not supplied."
		Response.Write "<br>"
		Response.Write "rextLinkID= " & rextLinkID
		Response.Write "<br>"
		Response.Write "rextAppName= " & rextAppName
		Response.Write "<br>"
		Response.Write "rLinkType= " & rLinkType
		Response.Write "<br>"
		Response.Write "rdocid= " & rdocid
	End If
	

	DeleteLinkReturn = DeleteLinkFromTable(rextLinkID, rextAppName, rLinkType, rdocid)
	
			Session("showselect") = false
			Session("extAppName") = ""
			Session("LinkType") = ""
			Session("extLinkID") = ""
			Session("LinkFieldName") = ""
			Session("useReload") = Empty
		
			%>
			<HTML>
			<HEAD>
			<META NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
			<TITLE></TITLE>
			<script language="javascript">
				function closerefresh(){
						//window.opener.location.reload(true);
					<%=reloadmethod%>;
					var agt = navigator.userAgent.toLowerCase();
					var is_ie6 = (agt.indexOf("msie 6.") != -1);
					var is_ie7 = (agt.indexOf("msie 7.") != -1);
					if (is_ie6)
					{
						var oMe = window.self;
						oMe.opener = window.self;
						oMe.close();
					}
					else
					{
						window.open('','_self');
						window.close();
					}
				}
			</script>
			</HEAD>
			<BODY onload="closerefresh()">


			</BODY>
			</HTML>
<%
elseif rmaction="close" then

	Session("showselect") = false
	Session("extAppName") = ""
	Session("LinkType") = ""
	Session("extLinkID") = ""
	Session("LinkFieldName") = ""
	Session("useReload") = Empty
	%>
		<HTML>
		<HEAD>
		<META NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
		<TITLE></TITLE>
		<script language="javascript">
			function closerefresh(){
						//window.opener.location.reload(true);
					<%=reloadmethod%>;	
					var agt = navigator.userAgent.toLowerCase();
					var is_ie6 = (agt.indexOf("msie 6.") != -1);
					var is_ie7 = (agt.indexOf("msie 7.") != -1);
					if (is_ie6)
					{
						var oMe = window.self;
						oMe.opener = window.self;
						oMe.close();
					}
					else
					{
						window.open('','_self');
						window.close();
					}
				}
		</script>
		</HEAD>
		<BODY onload="closerefresh()">


		</BODY>
		</HTML>
	<%	
End If

%>
