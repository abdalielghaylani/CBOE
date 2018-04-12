<%@ LANGUAGE=VBScript  %>
<!--#INCLUDE VIRTUAL = "/biosar_browser/source/biosar_display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->

<%

Dim  spreadheetDebug 
	spreadheetDebug = False
	
	
	on error resume next
	dbkey = request("dbname")
	formgroup = request("formgroup")
	formmode = request("formmode")
	
'-- CSBR ID:125215
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: To read the version of CDExcel configured
'-- Date: 06/24/2010
	Dim CDExcel_V12
	CDExcel_V12 = False
	if instr(Application("CDAX_ProgID"), "12")> 0 or instr(Application("CDAX_ProgID"), "14")> 0 then
		CDExcel_V12 = True
	end if
'-- End of Change #125215#
	
	if Session("hitlistid" & dbkey & formgroup) ="" or Session("hitlistid" & dbkey & formgroup) ="0" then
		Response.Write "<BR><Br>Your hitlist has expired.<BR> You will need to repeat your query in order to be able to export it."
		Response.Write "<BR><BR><center><input type=image src=""graphics\close.gif"" onclick=""window.close()"" id=image1 name=image1></center>"
		Response.End
	End if
	
	if UCase(Application("EXCEL_INTEGRATION_FORMAT")) = "USER_OPTION" then
		if request("excelFormat") = "" then
			Response.Write "<b>Please choose how you would like to format your spreadsheet.</b>"
			Response.Write "<form method=""post"" action=""show_spreadsheet.asp?" & Request.QueryString & """>"
			Response.Write "<input type=""radio"" name=""excelFormat"" value=""Convert"" checked> <b>Child data merged in a single cell</b><br><br>"
			Response.Write "<table border=""1"" cellpadding=""2"" cellspacing=""0"">"
			Response.Write "<tr><td width=""100""><b>Parent </b></td><td width=""60""><b>Child</b></td></tr>"
			Response.Write "<tr><td>ParentValue1</td><td>ChildValue1<br>ChildValue2</td></tr>"
			Response.Write "</table><br /><br />"
			
			Response.Write "<font color=""red"">The following export formats may encounter problems with large data sets, resulting in a blank Excel spreadsheet.</font>"
			Response.Write "<br /><br />"
			
			Response.Write "<input type=""radio"" name=""excelFormat"" value=""ConvertWithParentRepetition""> <b>Child data in distinct cells (Parent Repeated)</b><br><br>"
			Response.Write "<table border=""1"" cellpadding=""2"" cellspacing=""0"">"
			Response.Write "<tr><td width=""100""><b>Parent </b></td><td width=""60""><b>Child</b></td></tr>"
			Response.Write "<tr><td>ParentValue1</td><td>ChildValue1</td></tr>"
			Response.Write "<tr><td>ParentValue1</td><td>ChildValue2</td></tr>"
			Response.Write "</table><br /><br />"
			
			Response.Write "<input type=""radio"" name=""excelFormat"" Value=""ConvertWithoutParentRepetition""> <b>Child data in distinct cells (Parent Not Repeated)</b><br><br>"
			Response.Write "<table border=""1"" cellpadding=""2"" cellspacing=""0"">"
			Response.Write "<tr><td width=""100""><b>Parent </b></td><td width=""60""><b>Child</b></td></tr>"
			Response.Write "<tr><td>ParentValue</td><td>ChildValue1</td></tr>"
			Response.Write "<tr><td>&nbsp;</td><td>ChildValue2</td></tr>"
			Response.Write "</table><br /><br />"
			
			
			Response.Write "<input type=""Submit"" value=""Create Spreadsheet"">"
			Response.Write "</form>"
			Response.End
		End if
	End if
	
	
	currentLogin = UCase(Session("UserName" & dbkey))
	Set fso = Server.CreateObject("Scripting.FileSystemObject")
	Set oData = Server.CreateObject("MSXML2.FreeThreadedDOMDocument.4.0")
	Set oTemplate = Server.CreateObject("MSXML2.FreeThreadedDOMDocument.4.0")
 
	oData.validateOnParse=true
	oData.async=false
	'get the XML resultset
	if formmode = "edit" then
				Set Session("DETAIL_RS_FULL") = buildShapeRS(Session("DETAIL_RS_FULL"), dbkey, formgroup, "detail_full", false)
				if session("allOuterSorts") <> "" then
					ApplyBaseTableSort  Session("DETAIL_RS_FULL")
				end if
			Session("DETAIL_RS_FULL").Save oData, 1
		   template = session("template_definition_" & "detail" & formgroup)
	else

		   if session("allInnerSorts") <> "" then
				Session("lastAllInnerSorts") =  session("lastAllInnerSorts")
				'if more sorting on current list is done the recordset needs to be rebuilt
				if Not (Session("allInnerSorts") = Session("lastAllInnerSorts")) then
					Set Session("LIST_RS_SORTED") = Nothing
					Session("LIST_RS_SORTED") = ""
				end if
				'this really should be done with a transform rather the rebuilding the RS, but at this time this is the easiest solution.
				Set Session("LIST_RS_SORTED") = buildShapeRS(Session("LIST_RS_SORTED"), dbkey, formgroup, "list", false)
				if session("allOuterSorts") <> "" then
					ApplyBaseTableSort  Session("LIST_RS_SORTED")
				end if
				Session("LIST_RS_SORTED").Save oData, 1
			else
				if session("allOuterSorts") <> "" then
					ApplyBaseTableSort Session("list_rs")
				end if
				Session("list_rs").Save oData, 1
			end if
		 	
		 	template = session("template_definition_" & "list" & formgroup)

		  
	end if

	 if spreadheetDebug = true then
		oData.save(server.MapPath("/CFWTemp/biosar_browser/biosar_browsertemp/sessiondir/" & session.sessionid & "/InputRS.xml"))
	end if
	
	
	'load template file into a DOM
	oTemplate.loadXML template
	if spreadheetDebug = true then
		oTemplate.save(server.MapPath("/CFWTemp/biosar_browser/biosar_browsertemp/sessiondir/" & session.sessionid & "/InputTempalte.xml"))
	end if
  
	oData.setProperty "SelectionLanguage","XPath"
	oData.setProperty "SelectionNamespaces","xmlns:s='uuid:BDC6E3F0-6DA3-11d1-A2A3-00AA00C14882' xmlns:dt='uuid:C2F41010-65B3-11d1-A29F-00AA00C14882' xmlns:rs='urn:schemas-microsoft-com:rowset'"
	
	 'change to display names for tables
 	oTemplate.setProperty "SelectionLanguage","XPath"
	oTemplate.setProperty "SelectionNamespaces","xmlns:s='uuid:BDC6E3F0-6DA3-11d1-A2A3-00AA00C14882' xmlns:dt='uuid:C2F41010-65B3-11d1-A29F-00AA00C14882' xmlns:rs='urn:schemas-microsoft-com:rowset' xmlns:z='#RowsetSchema'"
	xpath_query="//DISPLAY/TABLE_ELEMENT[string-length(@HEADER_NAME)>0]"
	set templateNodeList=oTemplate.selectNodes(xpath_query)
	set dataNodeList = oData.getElementsByTagName("s:ElementType")

   'this should loop through all the table_elements and set the display name for the table name
   'CSBR# 140967 
   'Purpose of change: To correct the looping logic in accordance to the difference in the nested table structure in the case of details template
	Dim tempName
	Dim display_table_name
   	if formmode <> "edit" then
		For l = 0 to templateNodeList.length   
			Set oTNode = templateNodeList.Item(l)
			'display_table_name = oTNode.getAttribute("HEADER_NAME")
			'table_name = oTNode.getAttribute("NAME")
			Set dataNode = dataNodeList.Item(l)
			tempName = dataNode.getAttribute("name")
		
			'now go through each of the fields in the template, pull the corresponding field in the dataset and change the display name and dt:max
			set templateChildNodeList = oTNode.getElementsByTagName("FIELD")
			for k = 0 to templateChildNodeList.Length
				Set FieldNode =  templateChildNodeList.Item(k)
				fieldname = FieldNode.getAttribute("VALUE_COLUMNS")
        '-- CSBR ID: 139740
        '-- Change Done by : Manoj Unnikrishnan
        '-- Purpose: Check the show attribute to decide whether it should be shown in Excel or not
        '-- Date: 14/04/2010
			    	dim show
			    	show = true
			    	if FieldNode.getAttribute("SHOW") = "0" and UCase(fieldname) <> "STRUCTURE" then show=false
			    	if show = true then
		'-- End of Change #139740#
					if UCase(fieldname) = "STRUCTURE"  then
				    		displayname = "STRUCTURE"
					else
						displayname = FieldNode.getAttribute("HEADER_NAME")
					end if
					fieldWidth =  FieldNode.getAttribute("WIDTH")
					xpath_query="//s:ElementType[@name='" & tempName & "']/s:AttributeType[@name='" & fieldname & "']"
					set dataNode2 = oData.selectSingleNode(xpath_query)
					Set newAtt = oData.createAttribute("rs:name")	
					newAtt.value = displayname
					dataNode2.setAttributeNode(newAtt)	
					dataNode2.setAttribute("name") = fieldname
        '-- CSBR ID: 139740
        '-- Date: 14/04/2010
    			   	end if
	  '-- End of Change #139740#
			Next
		
	 	next 
	else	    
    		Set innerTableList = templateNodeList.Item(0).selectNodes("//TABLE_ELEMENT[string-length(@HEADER_NAME)>0]")
    		'now go through each of the fields in the template, pull the corresponding field in the dataset and change the display name and dt:max
    		For l = 0 to innerTableList.length    
        		Set oTNode = innerTableList.Item(l)       
        		Set dataNode = dataNodeList.Item(l)
	    		tempName = dataNode.getAttribute("name")	
			set templateChildNodeList = oTNode.getElementsByTagName("FIELD")
			for k = 0 to templateChildNodeList.Length 
				Set FieldNode =  templateChildNodeList.Item(k)
				fieldname = FieldNode.getAttribute("VALUE_COLUMNS")
				if UCase(fieldname) = "STRUCTURE"  then
					displayname = "STRUCTURE"
				else
					if l = 0 then
						displayname = FieldNode.getAttribute("DISPLAY_NAME")
					else
						displayname = FieldNode.getAttribute("HEADER_NAME")
					end if
				end if
				fieldWidth =  FieldNode.getAttribute("WIDTH")
				xpath_query="//s:ElementType[@name='" & tempName & "']/s:AttributeType[@name='" & fieldname & "']"
				set dataNode2 = oData.selectSingleNode(xpath_query)
				Set newAtt = oData.createAttribute("rs:name")	
				newAtt.value = displayname				
				dataNode2.setAttributeNode(newAtt)	
				dataNode2.setAttribute("name") = fieldname
			Next		
	 	next 
	end if 'End of change for CSBR 140967	
   	set templateNodeList=oTemplate.getElementsByTagName("TABLE_ELEMENT")
   	set dataNodeList = oData.getElementsByTagName("s:ElementType")
	For j = 0 to templateNodeList.length
		on error resume next
		Set oTNode = templateNodeList.Item(j)
		 display_table_name = oTNode.getAttribute("HEADER_NAME")
		if display_table_name <> "" then
			Set oTNode = templateNodeList.Item(j)
			table_name = oTNode.getAttribute("NAME")
			' you need to go back one because the detail view template has a parent empty table_element before the table_element of interest.
			if decrementNode = true then
				Set dataNode = dataNodeList.Item(j-1)
			else
				Set dataNode = dataNodeList.Item(j)
			end if
			'DGB Need to remove the tables that should not be displayed
			'CSBR-61865
			if oTNode.getAttribute("SHOW") = "0" then 
				'Delete the node because we don't want to show it
				dataNode.parentNode.RemoveChild(dataNode)
			else
				' Rename the name and rs:name attributes
				tempName = dataNode.getAttribute("name")
				Set oAttribute = oData.createAttribute("rs:name")		
				oAttribute.value = display_table_name
				dataNode.setAttributeNode(oAttribute)	
				dataNode.setAttribute("name") = tempName
				'decrementNode = false '140967: Commenting out the line because if in between there is an empty Table, then every subsequent table should be considered as j-1
			end if	
		else
			decrementNode = true
		end if 
	next
 
   
	finaldoc = "/CFWTemp/biosar_browser/biosar_browsertemp/sessiondir/" & session.sessionid & "/TempTransformRS.xml"
	oData.save(server.MapPath("/CFWTemp/biosar_browser/biosar_browsertemp/sessiondir/" & session.sessionid & "/TempTransformRS.xml"))
  
  
	Set oXSL = Server.CreateObject("MSXML2.FreeThreadedDOMDocument.4.0")
	Set oFinalData = Server.CreateObject("MSXML2.FreeThreadedDOMDocument.4.0")
	XSLpath = "/biosar_browser/source/convert.xsl"
	
	
	 if UCase(Application("EXCEL_INTEGRATION_FORMAT")) = "USER_OPTION" then	
		If lcase(request("excelFormat")) <> "convert" then
			XSLpath = "/biosar_browser/source/" & request("excelFormat") & ".xsl"
		end if
	else
'-- CSBR ID:125215
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Modified the below code to read the correct version of xsl file as per version CDExcel used
'-- Date: 06/24/2010
		 if UCase(Application("EXCEL_INTEGRATION_FORMAT")) = "MERGED_CHILD" then
			if CDExcel_V12 = True then
				XSLpath = "/biosar_browser/source/convert_cdx12.xsl"
			else
				XSLpath = "/biosar_browser/source/Convert.xsl"
			end if
		 elseif  UCase(Application("EXCEL_INTEGRATION_FORMAT")) = "PARENT_REPEATED" then	
			if CDExcel_V12 = True then
				XSLpath = "/biosar_browser/source/ConvertWithParentRepetition_cdx12.xsl"
			else
				XSLpath = "/biosar_browser/source/ConvertWithParentRepetition.xsl"
			end if
		 elseif UCase(Application("EXCEL_INTEGRATION_FORMAT")) = "PARENT_NOT_REPEATED" then
			if CDExcel_V12 = True then
				XSLpath = "/biosar_browser/source/ConvertWithoutParentRepetition_cdx12.xsl"
			else
				XSLpath = "/biosar_browser/source/ConvertWithoutParentRepetition.xsl"
			end if
		 end if
'-- End of Change #125215#
	end if

	oXSL.load(Server.MapPath(XSLpath))
	oFinalData.load(Server.MapPath(finaldoc))
	'create object for transform
	 bname = currentLogin & fso.GetBaseName(fso.GetTempName)
    
   'put the transform into a variable and write it to a file. 
   'If you put this in an object and save it as XML you end up loosing the &#10; which are key to the child table data rows delineation.
   'I can't find any other way to do this.
 
	Dim myvar
	myvar = oFinalData.transformNode(oXSL)
	on error resume next
	xmlName = "/CFWTemp/biosar_browser/biosar_browsertemp/sessiondir/" & session.sessionid & "/" & bname & ".xml"
	xmlPath = server.MapPath(xmlName)
	Set fs = Server.CreateObject("Scripting.FileSystemObject")
	Set a = fs.OpenTextFile(xmlPath , 8, True, -1)  
	a.Write myvar
	a.close

if err then
	response.write "Error while saving the xml file at " & xmlPath & "<BR>"
	response.write err.source & " " & Err.description & "<BR>"
	response.end

end if 


	xlsName = "/CFWTemp/biosar_browser/biosar_browsertemp/sessiondir/" & session.sessionid & "/" & bname & ".xls"
	XlsPath = Server.MapPath(xlsName)
  
	'now let's open this in Excel
	dim ExcelApp, ExcelWB, oSheet, oCell, foundCell
	set ExcelApp = Server.createobject("Excel.Application")
	if err then
		Response.Write "Error while trying to open Excel on the server<BR>"
		Response.Write "Please ensure that Excel has been installed and configured on the server to support this feaute<BR>"
		Response.Write "Error Details: " & err.Source & " - " & err.Description
		Response.end
	end if
	ExcelApp.visible = True

	set ExcelWB = ExcelApp.Workbooks.Open(xmlPath)

	Set oSheet = ExcelWB.ActiveSheet
'-- CSBR ID:125215
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: To do auto convert of the structures when opened in Excel only in case of CDExcel 12
'-- Date: 06/24/2010
	if CDExcel_V12 = True then
		oSheet.CustomProperties.Add "cdxlautoconvert", "true"
		oSheet.CustomProperties.Add "cdxldocument", "140000000000000000000"
	end if
'-- End of Change #125215#

	ExcelWB.SaveAs XlsPath , FileFormat=xlNormal

	ExcelWB.Close
	ExcelApp.quit

	Set ExcelWB = Nothing
	Set ExcelApp = Nothing
  
	' send the xls file to the client
	Set objStream = Server.CreateObject("ADODB.Stream")
	objStream.Open
	objStream.Type = 1 'adTypeBinary
	objStream.LoadFromFile XlsPath 
  
	response.AddHeader "content-disposition", "attachment; filename=" & bname & ".xls"
	response.contentType = "application/vnd.ms-excel"
	

	on error resume next 'a hammer to stop IE from hanging with XP???
	Response.BinaryWrite objStream.Read
	Response.Flush
  
	objStream.Close
	Set objStream = Nothing

	'delete the .xml file since it was just temporary
	if Not spreadheetDebug = true then
		fso.DeleteFile(xmlPath)
		fso.DeleteFile(XlsPath)
	end if
	
	
	
	Sub  ApplyBaseTableSort(byRef RS)
	on error resume next
	basetableSort = GetLastWidgetBaseTableSort()
	if basetableSort <> "" then
		RS.Sort=basetableSort
	end if
End Sub


'LJB 5/1/2005 Function to get the fields in the sort list fo the base table when the XML widget is used to sort
function GetLastWidgetBaseTableSort()
	dim theReturn
	theReturn = ""
	if Session("allOuterSorts") <> "" then
		temp = split(Session("allOuterSorts"), "|", -1)
		'return the last sort
		theReturn =  temp(UBound(temp))
	end if
	GetLastWidgetBaseTableSort=theReturn
End Function



'LJB 5/1/2005 Function to get the fields in the sort list for a child table when the XML widget is used to sort
function GetLastWidgetChildTableSort(childTableName)'
	dim theReturn
	theReturn=""
	if Session("allInnerSorts") <> "" then
		temp = split(Session("allInnerSorts"), "|", -1) 
		'loop backwards since we are interested in the most recent sort
		for i = 0 to UBound(temp)
			temp2 = split(temp(i), ".", -1)
			test = Trim(replace(childTableName, ".", "_"))& "_RS"
			if Trim(UCase(temp2(0))) = test then
				theReturn = childTableName & "." & temp2(1) ' return the fieldname and the sort direction
				exit for
			end if
		next
	end if
	GetLastWidgetChildTableSort=theReturn
end function
%>

