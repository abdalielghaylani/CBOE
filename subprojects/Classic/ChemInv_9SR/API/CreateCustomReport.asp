<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
'Response.end
Dim strError
Dim bWriteError
Dim PrintDebug
Dim SortbyFieldName
Dim SortDirectionTxt
Dim SortDirection
Dim Cmd
Dim Conn
dbkey = "ChemInv"

bDebugPrint = false
bWriteError = False
strError = "Error:CreateCustomReport<BR>"

'RPT paths
RPTPath = Application("RPT_PATH")
ReportQueuePath = RPTPath & "reportqueue.mdb"
ReportArchiveDBPath =  RPTPath & "reportqueuearchive.mdb"	
ReportDBPath = Application("ReportDBPath")
ReportsHTTPPath = Application("ReportsHTTPPath") 

'Required Paramenters
ReportName = Request("ReportName")
QueryName = Request("QueryName")
Report_ID = Request("Report_ID")
'Report_ID = FormatNumber(Report_ID_string)
CSUserName = Request("CSUserName")
'Optional parameters
ReportFormat = Request("ReportFormat")
userfilter = Request("userfilter")


' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/apiCreateCustomReport.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(Report_ID) then
	strError = strError & "Report ID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(ReportName) then
	strError = strError & "ReportName is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(QueryName) then
	strError = strError & "QueryName is a required parameter<BR>"
	bWriteError = True
End if

' Get additional report parameters and validate
ValuePairs=""
	for each field in Request.form
	   	if InStr(field, "field_") > 0 then
	   	    position = InStr(field, "field_")
	   		value = Request(field)
	   		    shift_string = position + 5
	    		name = right(field,(len(field)-shift_string))
	    		paramtype = GetListFromSQL("SELECT ParamType FROM " &  Application("CHEMINV_USERNAME") & ".inv_ReportParams WHERE ParamName='" & name & "' AND Report_ID=" & Report_ID)
	    		isrequired = GetListFromSQL("SELECT isrequired FROM " &  Application("CHEMINV_USERNAME") & ".inv_ReportParams WHERE ParamName='" & name & "' AND Report_ID=" & Report_ID)
	    	    If isrequired = "1" AND (IsEmpty(value) OR value = "") then
					strError = strError & name & " is a required parameter<BR>"
					bWriteError = True
				End if
	   		if value <> "" then

	    		' validate parameter and add to ValuePairs
	    		if Rtrim(paramtype) = "num" then
	    			if isNumeric(value) then
	    				ValuePairs = ValuePairs & " AND " & name & "=" & value
	    			else
	    				strError = strError & name & " must be a number<BR>"
	    				bWriteError = True
	    			end if
	    		elseif Rtrim(paramtype) = "location" then
	    				ValuePairs = ValuePairs & " AND " & name & " IN (SELECT Location_ID from inv_Locations CONNECT BY prior Location_ID = Parent_ID START WITH Location_ID = " & value & ")"
	    		elseif Rtrim(paramtype) = "start_date" then
	    			if isDate(value) then
	    			    name=right(name,(len(name)-2))
	    				ValuePairs = ValuePairs & " AND " & name & " > " & GetOracleDateString(value)
	    			else
	    					strError = strError & name & " must be a date<BR>"
	    					bWriteError = True
	    			end if
	    		elseif Rtrim(paramtype) = "end_date" then
	    			if isDate(value) then
	    			    name=right(name,(len(name)-2))
    					ValuePairs = ValuePairs & " AND " & name & " < " & GetOracleDateString(value) 
	    			else
	    					strError = strError & name & " must be a date<BR>"
	    					bWriteError = True
	    			end if
	    		elseif Rtrim(paramtype) = "user_name" then
    				ValuePairs = ValuePairs & " AND " & name & "='" & UCASE(value) & "'"
	    		elseif Rtrim(paramtype) = "text" then
    				ValuePairs = ValuePairs & " AND " & name & "='" & value & "'"
	    		end if
			end if
	   	end if
    next

'add credential requirement for Requests Report (ID 12)
if Report_ID=12 AND userfilter="1" then
	ValuePairs = ValuePairs & " AND r.user_id_fk='" & UCASE(CSUserName) & "'"
end if
  
If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

'Check optional parameters
If ReportFormat = "" then ReportFormat = Application("DefaultReportFormat")

if bDebugPrint then
	'Debugging section
	Response.write("QueuePath   : " & ReportQueuePath & "<br>")
	Response.write("DatabasePath: " & ReportDbPath & "<br>")
	Response.write("ReportName  : " & ReportName & "<br>")
	Response.write("ReportDirectory  : " & ReportDirectory & "<br>")
	Response.write("QueryName  : " & QueryName & "<br>")
	Response.write("QueryText  : " & QueryText & "<br>")
	Response.write("ReportFormat: " & ReportFormat & "<br>")
	Response.write("ValuePairs: " & ValuePairs & "<br>")
		Response.write("Report_ID: " & Report_ID & "<br>")
response.end

Else
'build QueryText
	GetInvConnection()
	Set RS=Conn.Execute("SELECT ReportSQL FROM " &  Application("CHEMINV_USERNAME") & ".inv_Reports WHERE ID=" & Report_ID)
	'QueryText = GetListFromSQL("SELECT ReportSQL FROM " &  Application("CHEMINV_USERNAME") & ".inv_Reports WHERE ID=1")
'Response.write QueryText
'Response.end
QueryText=RS("ReportSQL")
	if ValuePairs <> "" then
		QueryText = QueryText & ValuePairs
	end if


	'Create RPT
	Set ReportQ = Server.CreateObject("ReportQ.CReportQ")
	'Response.end
	if QueryName <> "" AND QueryText <> "" then
		ReportFileName = ReportQ.MakeReport(ReportQueuePath, ReportDBPath, ReportName, QueryName, QueryText, ReportFormat)
	else	
		ReportFileName = ReportQ.MakeReport(ReportQueuePath, ReportDBPath, ReportName, "", "", ReportFormat)
	end if
	Response.ContentType = "text/html"
	If InStr(1,ReportFileName,"Report Error") = 0 Then	
		Response.Write ReportsHTTPPath & ReportFileName
	Else 
		Response.Write ReportFileName
	End if
End if
%>