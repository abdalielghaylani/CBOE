<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/compound_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/xml_source/RS2HTML.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetWellAttributes.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS
call clearWellRegAttributes
dbkey = "ChemInv"
wellID = Request("WellID")
refresh = Request("refresh")
if refresh = "True" then
	Session("sWellTab") = ""
end if
Session("wIsEdit")=False
if wellID = Session("SelectWell") then
	Session("lastSelectedWell") = Session("SelectWell")
	Session("SelectWell") = "0"
end if
%>
<html>
<head>
<style>
	A {Text-Decoration: none;}
	.TabView {color:#000000; font-size:8pt; font-family: verdana}
	A.TabView:LINK {Text-Decoration: none; color:#000000; font-size:8pt; font-family: verdana}
	A.TabView:VISITED {Text-Decoration: none; color:#000000; font-size:8pt; font-family: verdana}
	A.TabView:HOVER {Text-Decoration: underline; color:#4682b4; font-size:8pt; font-family: verdana}
</style>
<title><%=Application("appTitle")%> -- View a Plate Well</title>

<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<SCRIPT LANGUAGE="JavaScript" SRC="/cheminv/utils.js"></SCRIPT>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script language="JavaScript" src= "/cfserverasp/source/chemdraw.js"></script>
<script>cd_includeWrapperFile("/cfserverasp/source/")</script>
<%if detectModernBrowser = true then%>
<SCRIPT LANGUAGE="javascript" src="<%=Application("CDJSUrl")%>"></SCRIPT>
<%end if %>
<SCRIPT LANGUAGE="JavaScript">
function ParentWellLink(LocationID, TreeViewOpenNodes1, PlateID, WellID) {
	//alert(event.ctrlKey);
	if (event.ctrlKey) {
		SelectLocationNode2(0, LocationID, 0,TreeViewOpenNodes1, PlateID, 1, WellID);
		window.close();
	}
	else {
		OpenDialog('/cheminv/gui/ViewWell.asp?wellID=' + WellID + '&refresh=true', 'ParentDiag'+WellID, 5);
	}
}
</SCRIPT>
<!--#INCLUDE FILE="../source/app_js.js"-->
</head>
<body>
<table border="0" cellspacing="0" cellpadding="2" width="100%" align="left">
	<tr>
		<td align="right" valign="top" nowrap>
			<%if Application("UseCustomTabFrameLinks") then%>
				<!--#INCLUDE VIRTUAL = "/cheminv/custom/gui/well_tab_frame_links.asp"-->
			<%else%>
				<!--#INCLUDE VIRTUAL = "/cheminv/gui/well_tab_frame_links.asp"-->
			<%end if%>
		</td>
	</tr>
</table>
<BR clear=all>
<form name="form1" action="echo.asp" method="POST">
<input type="hidden" name="PlateID" value="<%=wellID%>">
<!--#INCLUDE VIRTUAL = "/cheminv/gui/WellSQL.asp"-->
<%
	Call GetInvConnection()
	countSQL = "SELECT count(*) as CompoundCount FROM inv_well_compounds WHERE well_id_fk = ?"
	Set Cmd = GetCommand(Conn, countSQL, adCmdText)
	Cmd.Parameters.Append Cmd.CreateParameter("WellID", 5, 1, 0, wellID)
	Set RS = Cmd.Execute
	if cint(RS("CompoundCount")) > 1 then
		SQL = SQL & " WELL_ID = " & wellID
	else
		SQL = FlatWellSQL & " WELL_ID = " & wellID
	end if
	'Response.Write SQL
	'Response.End
	Set rsWell = server.CreateObject("ADODB.recordset")
	rsWell.CursorLocation = adUseClient
	rsWell.Open SQL,Conn,2,3

	ParentWellIDs = rsWell("PARENT_WELL_ID")
	ParentWellLabels = rsWell("PARENT_WELL_LABELS")
	ParentPlateLocationIDs = rsWell("PARENT_PLATE_LOCATION_ID_FK")
	ParentPlateIDs = rsWell("PARENT_PLATE_ID_FK")
	ParentCount = rsWell("ParentCount")
	CompoundCount = rsWell("CompoundCount")

	'Set oTemplate = Server.CreateObject("MSXML2.FreeThreadedDOMDocument")
	Set oTemplate = Server.CreateObject("MSXML2.FreeThreadedDOMDocument.4.0")
	if (cint(ParentCount) > 1 or (cint(CompoundCount)>1 and cint(ParentCount) = 1)) then
		oTemplate.load(Server.MapPath("/" & Application("AppKey") & "/config/xml_Templates/ViewMixtureWell_Summary.xml"))
		Set mainTable = oTemplate.selectSingleNode("/DOCUMENT/DISPLAY/TABLE_ELEMENT/TABLE_ELEMENT")
	else
		oTemplate.load(Server.MapPath("/" & Application("AppKey") & "/config/xml_Templates/ViewWell_Summary.xml"))
		Set mainTable = oTemplate.selectSingleNode("/DOCUMENT/DISPLAY/TABLE_ELEMENT")
		'if there is no plugin generate a gif for the structure
		if Session("isCDP") <> "TRUE" or detectModernBrowser = true  then
			Set currentUserNode = oTemplate.selectSingleNode("/DOCUMENT/DISPLAY/FIELD[1]/@IS_STRUCTURE")
			currentUserNode.text = "0"
            Set currentUserNode = oTemplate.selectSingleNode("/DOCUMENT/DISPLAY/TABLE_ELEMENT/FIELD[@VALUE_COLUMNS='REG_BATCH_ID,REG_ID_FK,REG_NUMBER']")
            currentUserNode.text = Replace(currentUserNode.text, "reg_number=#REG_NUMBER#", "reg_number=#REG_ID_FK#")
			Set currentUserNode = oTemplate.selectSingleNode("/DOCUMENT/DISPLAY/FIELD[1]")
			SessionDir = Application("TempFileDirectory" & "ChemInv") & "Sessiondir"  & "\" & Session.sessionid & "\"
			filePath = SessionDir & "structure" & "_" & 160 & "x" & 140 & ".gif"
			SessionURLDir = Application("TempFileDirectoryHTTP" & "ChemInv") & "Sessiondir"  & "/" & Session.sessionid & "/"
			fileURL = SessionURLDir & "structure" & "_" & 160 & "x" & 140 & ".gif"
			ConvertCDXtoGif_Inv filePath, rsWell("base64_cdx"), 160, 140
            structureNode = "<div style=""display:none;""><input type=""hidden"" id=""inline_0"" name=""inline_0"" value=""" & rsWell("base64_cdx") &""">"
		    structureNode = structureNode & "<" & "script language=""JavaScript"">"
		    structureNode = structureNode & "    cd_insertObject(""chemical/x-cdx"", ""185"", ""130"", ""CD_0"", """ & Application("TempFileDirectoryHTTP" & "ChemInv")  & "mt.cdx"", ""true"", ""true"", escape(document.all.inline_0.value),  ""true""" & ")"
		    structureNode = structureNode & "</" & "script></div>"
			currentUserNode.text = structureNode & "<div class=""copyContainer"" style=""width: 160px; height: 140px;""><img src=""" & fileURL & """ width=""160"" height=""140"" border=""1""><div class=""copyOverlay""><A HREF =""#"" onclick=""doStructureCopyIndividual(); return false;""><img width=""20"" size=""20"" src=""/ChemInv/graphics/copy-icon.png"" /></a></div>"
		end if
	end if
	'Response.Write mainTable.xml & "=xml<BR>"
	'Set newNode = oTemplate.createNode(1, "FIELD", "")
	'Response.End

	For each key in custom_well_fields_dict
		Set newNode = CreateFieldNode(oTemplate, ucase(key), "GrayedText", null, null, null, custom_well_fields_dict.Item(key) & ":", "RightAlign", 1, null, null, 1, "#" & ucase(key) & "#")
		Set currNode = mainTable.insertBefore(newNode,null)
	Next

	'otemplate.save ("c:\test.xml")
	'Response.End
	HTML = RS2HTML(rsWell,oTemplate,null,null,null,null,null,null,null)

	'create parent well links and insert structures for mixture wells
	if (cint(ParentCount) > 1 or (cint(CompoundCount)>1 and cint(ParentCount)= 1)) then
		arrParentWellIDs = split(ParentWellIDs,",")
		arrParentPlateIDs = split(ParentPlateIDs,",")
		arrParentWellLabels = split(ParentWellLabels,",")
		arrParentLocationIDs = split(ParentPlateLocationIDs,",")
		for i = 0 to ubound(arrParentWellIDs)
			link = "<A CLASS=""MenuLink"" HREF=""#"" TITLE=""Parent Well"" ONCLICK=""ParentWellLink(" & arrParentLocationIDs(i) & ",'" & TreeViewOpenNodes1 & "'," & arrParentPlateIDs(i) & "," & arrParentWellIDs(i) & ");return false;"" onmouseover=""javascript:this.style.cursor='hand';"" onmouseout=""javascript:this.style.cursor='default';"">" & arrParentWellLabels(i) & "</a>"
			parentLinks = parentLinks & "<span id=""Parent Well:"" title="""">" & link & "</span>&nbsp;<BR>"
		next
		HTML = replace(HTML,"PARENTWELLREPLACEMENT",parentLinks)

		'"DECODE(wc.Reg_ID_FK, NULL, cpd.Substance_Name, (SELECT alt_ids.identifier AS REG_SUBSTANCE_NAME FROM alt_ids WHERE identifier_type = 0 AND reg_internal_id = wc.reg_id_fk AND rownum = 1)) as Substance_Name, " & _
		compoundSQL =  "SELECT DECODE(cpd.Reg_ID_FK, NULL, cpd.BASE64_CDX, (SELECT BASE64_CDX AS REG_BASE64_CDX FROM cheminvdb2.inv_vw_reg_structures ivrs WHERE ivrs.regid = cpd.reg_id_fk )) AS Base64_CDX," & _
						"DECODE(cpd.Reg_ID_FK, NULL, cpd.Substance_Name, (SELECT RegName FROM inv_vw_reg_batches WHERE regid = cpd.reg_id_fk and batchnumber=cpd.batch_number_fk)) as Substance_Name, " & _
						"wc.COMPOUND_ID_FK "
						if Application("RegServerName") <> "NULL" then
							compoundSQL = compoundSQL & ", DECODE(cpd.Reg_ID_FK, NULL, NULL, (SELECT RegBatchID FROM inv_vw_reg_batches WHERE inv_vw_reg_batches.regid = cpd.reg_id_fk and batchnumber=cpd.batch_number_fk)) AS Reg_Batch_ID "
							compoundSQL = compoundSQL & ", DECODE(cpd.Reg_ID_FK, NULL, NULL, (SELECT RegNumber FROM inv_vw_reg_batches WHERE inv_vw_reg_batches.regid = cpd.reg_id_fk and batchnumber=cpd.batch_number_fk)) AS Reg_Number "
						End if
						compoundSQL = compoundSQL & "FROM inv_well_compounds wc, inv_compounds cpd where compound_id_fk = compound_id(+) AND well_id_fk = " & wellID
						'in (" & ParentWellIDs & ")"
		'Response.Write compoundSQL
		'Response.End
		Set rsMixtureCompounds = Conn.execute(compoundSQL)
		i = 0
		count = 1
		TempcdxPath = Application("AppTempDirPathHTTP")& "/" & Application("appkey")& "Temp/mt.cdx"
		structureHTML = "<tr>"
		substanceNameHTML = "<tr>"
		IDHTML = "<tr>"
		for i = 0 to (cint(CompoundCount)-1)
			if count > 5 then
				substanceNameHTML = substanceNameHTML & "</tr>"
				IDHTML = IDHTML & "</tr>"
				structureHTML = structureHTML & "</tr>" & substanceNameHTML & IDHTML & "<tr>"
				substanceNameHTML = "<tr>"
				IDHTML = "<tr>"
				count = 1
			end if
			if session("DrawPref") = "ISIS" then
				ISISDraw = """True"""
			else
				ISISDraw = """False"""
			end if
 			if not rsMixtureCompounds.EOF then
 				compoundID = rsMixtureCompounds("compound_id_fk")
 				If Application("RegServerName") <> "NULL" then
 					regBatchID = rsMixtureCompounds("reg_batch_id")
 					regNumber = rsMixtureCompounds("reg_number")
                    if detectModernBrowser = true then
                        regIdParameter = rsMixtureCompounds("Reg_ID_FK")
                    else
                        regIdParameter = regNumber
                    end if
 				else
 					regBatchID = null
 					regNumber = null
 				end if
				'if there is no plugin generate a gif for the structure
				if Session("isCDP") <> "TRUE" AND detectModernBrowser = true then
				'if true then
					SessionDir = Application("TempFileDirectoryHTTP" & "ChemInv") & "Sessiondir"  & "/" & Session.sessionid & "/"
					filePath = SessionDir & "structure" & i & "_" & 160 & "x" & 140 & ".gif"
					ConvertCDXtoGif_Inv filePath, rsMixtureCompounds("base64_cdx"), 160, 140
					structureHTML = structureHTML & "<td COLSPAN=""2""><table cellpadding=""1"" cellspacing=""1"" BORDER=""1""><tr><td>"
                    structureHTML = structureHTML & "<div style=""display:none;""><input type=""hidden"" id=""inline_" & compoundID & """ name=""inline_" & compoundID & """ value=""" & rsMixtureCompounds("base64_cdx") &""">"
		            structureHTML = structureHTML & "<" & "script language=""JavaScript"">"
		            structureHTML = structureHTML & "    cd_insertObject(""chemical/x-cdx"", ""185"", ""130"", ""CD_" & dbCompoundID & """, """ & Application("TempFileDirectoryHTTP" & "ChemInv")  & "mt.cdx"", ""true"", ""true"", escape(document.all.inline_" & dbCompoundID & ".value),  ""true""" & ")"
		            structureHTML = structureHTML & "</" & "script></div>"
					structureHTML = structureHTML & "<div class=""copyContainer"" style=""width: 160px; height: 140px;""><img src=""" & filePath & """ width=""160"" height=""140"" border=""1""><div class=""copyOverlay""><A HREF =""#"" onclick=""doStructureCopyIndividual(); return false;""><img width=""20"" size=""20"" src=""/ChemInv/graphics/copy-icon.png"" /></a></div>" & "</td></tr></table></td>"
				else
					structureHTML = structureHTML & "<td COLSPAN=""2""><table cellpadding=""1"" cellspacing=""1"" BORDER=""1""><tr><td><input type=""hidden"" name=""structure" & i & """ value=""data:chemical/x-cdx;base64," & rsMixtureCompounds("base64_cdx") & """>"
					structureHTML = structureHTML & "<script language=""javascript"">cd_insertObject(""chemical/x-cdx"",150,130,""mycdx"",""" & TempcdxPath & """, true, true, dataURL=document.form1.structure" & i & ".value," & ISISDraw & ");</script></td></tr></table></td>"
				end if
				substanceNameHTML = substanceNameHTML & "<td COLSPAN=""2"" CLASS=""CenteredBoldItalic"">"  & TruncateInSpan(rsMixtureCompounds("Substance_Name"),18,"substance_name"&i) & "</td>"
				if not isNull(compoundID) then
					IDHTML = IDHTML & "<td CLASS=""LeftAlign"" WIDTH="""" HEIGHT="""">Compound ID:</td><td CLASS=""GrayedTextLeft"" WIDTH=""60"">" & compoundID & "</td>"
				elseif not isNull(regBatchID) then                    
					IDHTML = IDHTML & "<td CLASS=""LeftAlign"" WIDTH="""" HEIGHT="""">Reg Batch ID:</td><td CLASS=""GrayedTextLeft"" WIDTH=""60""><a class=""MenuLink"" href=""ViewRegDetails.asp?reg_number=" & regIdParameter & """ target=""_blank"">" & regBatchID & "</a></td>"
				else
					IDHTML = IDHTML & "<td CLASS=""LeftAlign"" WIDTH="""" HEIGHT=""""></td><td CLASS=""GrayedText""></td>"
				end if
				rsMixtureCompounds.movenext
			else
				structureHTML = structureHTML & "<td COLSPAN=""2""><table cellpadding=""1"" cellspacing=""1"" BORDER=""1""><tr><td><input type=""hidden"" name=""structure" & i & """ value="""">"
				structureHTML = structureHTML & "<script language=""javascript"">cd_insertObject(""chemical/x-cdx"",150,130,""mycdx"",""" & TempcdxPath & """, true, true, dataURL=document.form1.structure" & i & ".value," & ISISDraw & ");</script></td></tr></table></td>"
				substanceNameHTML = substanceNameHTML & "<td COLSPAN=""2""></td>"
				IDHTML = IDHTML & "<td COLSPAN=""2""></td>"
			end if
			count = count + 1
		next
		substanceNameHTML = substanceNameHTML & "</tr>"
		IDHTML = IDHTML & "</tr>"
		structureHTML = "<table cellpadding=""0"" cellspacing=""1"" BORDER=""0"">" & substanceNameHTML & structureHTML & "</tr>" & IDHTML & "</table>"
		HTML = replace(HTML,"STRUCTUREREPLACEMENT",structureHTML)
		HTML = replace(HTML,"SUBSTANCENAMEREPLACEMENT",substanceNames)

	end if

	Response.Write HTML
%>
	</form>
</body>
</html>
