<%@ Language=VBScript %>
<HTML>
<HEAD>
<META NAME="GENERATOR" Content="Microsoft FrontPage 5.0">
</HEAD>
<BODY>


<%

	
		on error resume next
		Response.Buffer = True
		if detectNS6 = true then
			response.Write "."
		end if	
		Response.Flush
		Response.Write String(255," ")
		'!DGB! 10/17/01
		'Path of gif should point to /source/graphics not to application navbuttons path.
		'Unless there is an explicit override from the ini file
		Response.Write "<BR><BR><BR><TABLE Width=""600""  border=""0""><tr><td valign=middle align=center><IMG SRC=""" & Application("ANIMATED_GIF_PATH") & """></td></tr></TABLE>"

		Response.Buffer = True
		Response.Flush
		Response.Write String(255," ")
		
		Response.Write "<TABLE Width=""600""  border=""0""><tr><td valign=top align=center>" & "A layout for the requested report was not found as is being generated. This will take several minutes in addition to the report generation. Please be patient." & "</td></tr></TABLE>"
		
		Response.Flush
	
	
%>


</BODY>
</HTML>