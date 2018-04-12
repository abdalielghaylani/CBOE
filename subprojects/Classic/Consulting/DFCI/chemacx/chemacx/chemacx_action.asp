<!--#INCLUDE FILE = "LogSearches.asp"-->
<!--#INCLUDE FILE = "../IncrementAnonymousSearchCounter.asp"-->
<!--#INCLUDE VIRTUAL = "/chemacx/export/excel_export_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/chemacx/export/app_specific_shape_statement.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/form_action_vbs.asp"-->
<!--#INCLUDE FILE = "../source/app_vbs.asp"-->

<%
	'Subscription handling and redirecting for public site.
	if Request("Authenticate") then
		' CSWebUser Authentication
		Serviceid = 10
		Session("ShowContent") = Session("okOnService_10")

		if not Session("ShowContent") then
			Response.Clear
			Session.Abandon
		%>
			<form target="_top" name=relay action="https://accounts.cambridgesoft.com/login.cfm?serviceid=10&promptsid=true" method="POST">		
				<input type="Hidden" name="ReturnURL" value="http://<%=Request.ServerVariables("SERVER_NAME")%>/chemacx/default.asp">
				<input type="Hidden" name="ReturnType" value="GET">
		<%
			For each fld in Request.QueryString
				response.write "<input type=""Hidden"" name=""" & fld & """ value=""" & Request.QueryString(fld) & """ >" 
			Next
		%>		
			</form>
			<script language="javascript">
				document.relay.submit();
			</script>
		<%
		Response.end
		end if
	end if
%>

<%custom_action = request("dataaction2")

Select Case UCase(custom_action)
	'LJB 6/9/2005 Export Feature Modified to Include Export To Excel. Eventually to be moved to core
	Case "EXPORT_HITS"
	
			If CBool(Application("USE_ANIMATED_GIF"))=true then
			FlushImageToClient(Application("ANIMATED_GIF_PATH"))
			FlushMessageToClient("this process may take several minutes...")
		else
			FlushMessageToClient("exporting...")
			FlushMessageToClient("this process may take several minutes...")
		end if
		Session("filepath") = ""
		ExportInfo = GetFormGroupVal(dbkey, formgroup,kSDFileFields)
		if INSTR(UCase(ExportInfo), "ALT_FORMGROUP:")> 0 then
			ExportInfo_temp=split(exportinfo, ":", -1)
			formgroup =ExportInfo_temp(1)
		end if
		'pick up export information from session variables

		ExportType = request("exporttype")
		MaxExportNumber = request("MaxExportNumber")
		fieldstoexport = session("all_export_fields")
		
		if trim(request("export_structure_data")) = "CS_OUTPUT_STRUC_DATA" then
			bStructOut = true
		else
			bStructOut = false
		end if
		OutputType = request("File_Ouput_Type")
		FileFormatType = request("File_Format_Type")
		
		Select Case UCase(FileFormatType)
			case "SDFILE"
				Select Case UCase(OutputType)
					Case "NESTED"
						if ExportType = "VIEW" then
							Session("filepath")= DoSDFileExport(dbkey, formgroup, fieldstoexport,ExportType,MaxExportNumber,bStructOut)
						else
							Session("filepath")= ExportSDFile(dbkey, formgroup, fieldstoexport,ExportType,MaxExportNumber,bStructOut)
						end if
					Case "FLAT"
						Session("filepath")= DoSDFileExport(dbkey, formgroup, fieldstoexport,ExportType,MaxExportNumber,bStructOut)
				
				End Select
			case "EXCEL"
			
				Select Case UCase(OutputType)
					Case "FLAT"
				
							Session("filepath")= ExportFlatExcel(dbkey, formgroup, fieldstoexport,ExportType,MaxExportNumber,bStructOut)
					Case "NESTED"
					
							Session("filepath")= ExportNestedExcel(dbkey, formgroup, fieldstoexport,ExportType,MaxExportNumber,bStructOut)
					
				End Select
		end Select
		
		'fieldstoexport = Split(SDFileFields, ",", -1) ' go through all fields and see which ones are checked for output
		redirectpath = "/" & Application("AppKey") & "/export_hits.asp?formmode=" & formmode & "&dbname=" & dbkey & "&formgroup=" & formgroup & "&export=complete"
		%>
		<script language = "javascript">
		document.location.href = "<%=redirectpath%>"
		</script>
		<%
End Select%>

