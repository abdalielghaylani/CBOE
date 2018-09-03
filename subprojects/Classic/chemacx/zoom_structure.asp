<%@ LANGUAGE=VBScript  %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<%BaseID = request.querystring("baseid")
dbkey = Request.QueryString("dbname")
initial_CDP_file = Application("TempFileDirectoryHTTP" & dbkey) & "mt.cdx"
 
fullSrucFieldName = request.QueryString("fullSrucFieldName")
structDataObjName = request.QueryString("structDataObjName")
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
        <style>
        * {
            box-sizing: border-box;
        }

        .copyContainer {
            position: relative;
            width: 100%;
            height: 100%;
        }

        .copyOverlay {
            position: absolute;
            bottom: 140px;
            left: 140px;
            background: rgba(0, 0, 0, 0.20); /* Black see-through */
            width: 30px;
            height: 30px;
            transition: .5s ease;
            opacity: 0;
            color: white;
            font-size: 10px;
            padding: 5px;
            text-align: center;
        }

        .copyContainer:hover .copyOverlay {
            opacity: 1;
        }
    </style>
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
        <%if detectModernBrowser = true then%>
        <script language="javascript" src="<%=Application("CDJSUrl")%>"></script>
        <!--#INCLUDE FILE="source/app_js.js"-->
        <%end if%>
	</head>
	<body bgcolor="White">
		<center>
        <%if detectModernBrowser = true then
        TempCdxPath = Application("TempFileDirectoryHTTP" & "chemacx")
        %>
        <div style="display: none">
            <script language="JavaScript">cd_insertObject("chemical/x-cdx", "100", "100", "mycdx", "<%=TempCdxPath%>mt.cdx", "False", "true", "", "true", <%=ISISDraw%>)</script>
        </div>
        <%end if%>
		<table bgcolor="#ffffff" cellspacing="1" border="0" cellpadding="4">
			<tr>
				<td>
<% 
	if detectModernBrowser = true then
        gifWidth = 300
        gifHeight = 300
		zoomGifDir = Application("TempFileDirectory" & dbkey) & FileRootName & "_" & gifWidth & "x" & gifHeight & ".gif"
		zoomGifPath = Application("TempFileDirectoryHTTP" & dbkey) & FileRootName & "_" & gifWidth & "x" & gifHeight & ".gif"
		Set checkFile = Server.CreateObject("Scripting.FileSystemObject")
		If NOT checkFile.FileExists(zoomGifDir) then
			ConvertCDXtoGIF dbkey, TableName, FieldName, BaseID, gifWidth, gifHeight	
		End if
		Set checkFile = nothing	
        embed_tag_string = embed_tag_string & "<div class=""copyContainer""><IMG SRC=""" & zoomGifPath & """" & "border=0>"
        embed_tag_string = embed_tag_string &  "<div class=""copyOverlay""><A HREF =""#"" onclick=""doStructureCopy('" & structDataObjName & "', true); return false;""><img width=""20"" size=""20"" src=""/ChemInv/graphics/copy-icon.png"" /></a></div></div>"
        Response.Write embed_tag_string				
Else%>
					<script language="javascript">cd_insertObjectStr("<embed src='<%=initial_CDP_file%>' border='0' width='500' height='400' id='1' name='CDX' viewonly='true' type='chemical/x-cdx' dataurl='<%=Application("ActionForm" & dbkey)%>?dbname=chemacx&amp;formgroup=base_form_group&amp;dataaction=get_structure&amp;Table=<%=TableName%>&amp;Field=<%=FieldName%>&amp;DisplayType=cdx&amp;StrucID=<%=BaseID%>'>");</script> 
<%End if %>		
				</td>
			</tr>
		</table>
		<br>
		<a HREF="javascript:window.close();" onMouseOver="window.status=&quot;Zoom structure&quot;; return true;"><br><img SRC="<%=Application("NavButtonGifPath")%>close.gif" BORDER="0"></a>
		</center>
	</body>
</html>