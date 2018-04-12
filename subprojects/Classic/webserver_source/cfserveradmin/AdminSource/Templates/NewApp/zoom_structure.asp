<%@ LANGUAGE=VBScript  %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<%BaseID = request.querystring("baseid")
dbkey = Request.QueryString("dbname")
initial_CDP_file = Application("TempFileDirectoryHTTP" & dbkey) & "mt.cdx"
 
fullSrucFieldName = request.QueryString("fullSrucFieldName")
' DGB make this page work with standard zoom button which passes the fieldname and baseid together
if fullStrucFieldName = "" then 
	fullSrucFieldName = request.QueryString("fieldname")
	last = len(fullSrucFieldName)
	for i = last to 1 step -1 
		If not isNumeric(Mid(fullSrucFieldName, i, 1)) then 
			exit for
		End if	 
	next
	baseID = Right(fullSrucFieldName, last - i)	
	fullSrucFieldName = left(fullSrucFieldName, i)
End if	
tempArr = Split(fullSrucFieldName,".")
TableName = tempArr(0)
FieldName = tempArr(1)
FileRootName = TableName & FieldName & "_" & BaseID 

gifWidth = request.QueryString("gifWidth")
gifHeight = request.QueryString("gifHeight")
cdxPath = Application("TempFileDirectory" & dbkey) & FileRootName & ".cdx"

%>

<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>
	<head>
	<title>zoom structure</title>
		<script language="javascript">
			function getButtons() {
				outputval = ""
				var buttonGifPath = theOpener.button_gif_path 
				closebutton = buttonGifPath + "close.gif"
				outputval = '<A HREF = "javascript:window.close();" onMouseOver="window.status=&quot;' + closehelpstr + '&quot;; return true;"><IMG SRC="' +  closebutton + '" BORDER="0"></A>'
				document.write (outputval)
			
			}
		</script>
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
		zoomGifDir = Application("TempFileDirectory" & dbkey) & FileRootName & "_" & gifWidth & "x" & gifHeight & ".gif"
		zoomGifPath = Application("TempFileDirectoryHTTP" & dbkey) & FileRootName & "_" & gifWidth & "x" & gifHeight & ".gif"
		Set checkFile = Server.CreateObject("Scripting.FileSystemObject")
		If NOT checkFile.FileExists(zoomGifDir) then
			ConvertCDXtoGIF dbkey, TableName, FieldName, BaseID, gifWidth, gifHeight	
		End if
		Set checkFile = nothing				
%>	
					<img SRC="<%=zoomGifPath%>" Border="0">
<%Else%>
					<script language="javascript">cd_insertObjectStr("<embed src='<%=initial_CDP_file%>' border='0' width='500' height='400' id='1' name='CDX' viewonly='true' type='chemical/x-cdx' dataurl='<%=Application("ActionForm" & dbkey)%>?dbname=<%=dbkey%>&amp;formgroup=base_form_group&amp;dataaction=get_structure&amp;Table=<%=TableName%>&amp;Field=<%=FieldName%>&amp;DisplayType=cdx&amp;StrucID=<%=BaseID%>'>");</script> 
<%End if %>		
				</td>
			</tr>
		</table>
		<br>
		<a HREF="javascript:window.close();" onMouseOver="window.status=&quot;Zoom structure&quot;; return true;"><br><img SRC="<%=Application("NavButtonGifPath")%>close.gif" BORDER="0"></a>
		</center>
	</body>
</html>