<%@ LANGUAGE=VBScript  %>
<%
Response.Expires = -1
' Receive parameters via QueryString
StrucID = request.querystring("StrucID")
gifWidth = request.QueryString("gifWidth")
gifHeight = request.QueryString("gifHeight")

dbkey = "ChemACX"
initial_CDP_file = Application("TempFileDirectoryHTTP" & dbkey) & "mt.cdx"
TableName= "Substance"
FieldName = "Structure"
FileRootName = TableName & FieldName & "_" & StrucID 

cdxPath = Application("TempFileDirectory" & dbkey) & FileRootName & ".cdx"
%>

<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>
	<head>
	<title>Display ChemACX Structure</title>
		<script language="JavaScript" src="/cfserverasp/source/chemdraw.js"></script>
		<script>cd_includeWrapperFile("/cfserverasp/source/")</script>
	</head>
	<body bgcolor="White">
		<center>
		<table bgcolor="#ffffff" cellspacing="1" border="0" cellpadding="4">
			<tr>
				<td>
<% 
	if len(gifWidth)>0 then
		zoomGifDir = Application("TempFileDirectory" & dbkey) & FileRootName & "_" & gifWidth & "x" & gifHeight & "zoom.gif"
		zoomGifPath = Application("TempFileDirectoryHTTP" & dbkey) & FileRootName & "_" & gifWidth & "x" & gifHeight & "zoom.gif"
		Set checkFile = Server.CreateObject("Scripting.FileSystemObject")
		If NOT checkFile.FileExists(zoomGifDir) then
			Set myGifConverter = Server.CreateObject("CDXLibCOM.Converter")
			myGifConverter.MakeImage "image/gif", cdxPath, zoomGifDir, 72, gifWidth, gifHeight
			Set myGifConverter = nothing
		End if
		Set checkFile = nothing				
%>	
					<img SRC="<%=zoomGifPath%>" Border="0">
<%Else%>
					<script language="javascript">cd_insertObjectStr("<embed src='<%=initial_CDP_file%>' border='0' width='500' height='400' id='1' name='CDX' viewonly='true' type='chemical/x-cdx' dataurl='<%=Application("ActionForm" & dbkey)%>?dbname=chemacx&amp;formgroup=base_form_group&amp;dataaction=get_structure&amp;Table=<%=TableName%>&amp;Field=<%=FieldName%>&amp;DisplayType=cdx&amp;StrucID=<%=StrucID%>'>");</script> 
<%End if %>		
				</td>
			</tr>
		</table>
		<br>
		<a HREF="javascript:window.close();" onMouseOver="window.status=&quot;Zoom structure&quot;; return true;"><br><img SRC="<%=Application("NavButtonGifPath")%>close_btn.gif" BORDER="0"></a>
		</center>
	</body>
</html>