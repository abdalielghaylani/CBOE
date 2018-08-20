<%@  language="VBScript" %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_vbs.asp"-->
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
            bottom: 185px;
            left: 185px;
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
        window.focus();
        function getButtons() {
            outputval = ""
            var buttonGifPath = theOpener.button_gif_path 
            closebutton = buttonGifPath + "close_btn.gif"
            outputval = '<A HREF = "javascript:window.close();" onMouseOver="window.status=&quot;' + closehelpstr + '&quot;; return true;"><IMG SRC="' +  closebutton + '" BORDER="0"></A>'
            document.write (outputval)
			
        }
    </script>
    <script language="JavaScript" src="/cfserverasp/source/chemdraw.js"></script>
    <script>cd_includeWrapperFile("/cfserverasp/source/")</script>
    <script language="javascript" src="<%=Application("CDJSUrl")%>"></script>
    <!--#INCLUDE FILE="source/app_js.js"-->
</head>
<body bgcolor="White">
    <center>
        <%if detectModernBrowser = true then%>
        <div style="display: none">
            <script language="JavaScript">cd_insertObject("chemical/x-cdx", "100", "100", "mycdx", "<%=TempCdxPath%>mt.cdx", "False", "true", "", "true", <%=ISISDraw%>)</script>
        </div>
        <%end if%>
		<table bgcolor="#ffffff" cellspacing="1" border="0" cellpadding="4">
			<tr>
				<td>
<%  
if detectModernBrowser = true then
    embed_tag_string = embed_tag_string & "<div class=""copyContainer""><IMG SRC="
    embed_tag_string = embed_tag_string &  """"  
    embed_tag_string = embed_tag_string &  Application("ActionForm" & dbkey) & "?dbname=" & dbkey & "&formgroup=gs_form_group&dataaction=get_structure&Table=" & TableName & "&Field=" & FieldName & "&DisplayType=SIZEDGIF&StrucID=" & BaseID & "&width=400&height=400"
    embed_tag_string = embed_tag_string  &  """ border=0><div class=""copyOverlay""><A HREF =""#"" onclick=""doStructureCopy('" & structDataObjName & "', true); return false;""><img width=""20"" size=""20"" src=""/ChemInv/graphics/copy-icon.png"" /></a></div></div>"
    Response.Write embed_tag_string				
Else%>
					<script language="javascript">cd_insertObjectStr("<embed src='<%=initial_CDP_file%>' border='0' width='400' height='400' id='1' name='CDX' viewonly='true' type='chemical/x-cdx' dataurl='<%=Application("ActionForm" & dbkey)%>?dbname=<%=dbkey%>&formgroup=gs_form_group&dataaction=get_structure&Table=<%=TableName%>&Field=<%=FieldName%>&DisplayType=cdx&StrucID=<%=BaseID%>'>");</script> 
<%End if %>		
				</td>
			</tr>
		</table>
		</center>
</body>
</html>
