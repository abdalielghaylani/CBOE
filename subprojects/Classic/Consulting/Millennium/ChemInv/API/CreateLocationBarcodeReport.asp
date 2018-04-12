<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->

<%
Dim strError
Dim bWriteError
Dim PrintDebug
Dim SortbyFieldName
Dim SortDirectionTxt
Dim SortDirection
Dim InvSchema

bDebugPrint = False
bWriteError = False
strError = "Error:CreateLocationReport<BR>"

'RPT paths
RPTPath = Application("RPT_PATH")
ReportQueuePath = RPTPath & "reportqueue.mdb"
ReportArchiveDBPath =  RPTPath & "reportqueuearchive.mdb"	
ReportDBPath = Application("ReportDBPath")
ReportsHTTPPath = Application("ReportsHTTPPath") 

'Required Paramenters
LocationID= Request("LocationID")
ReportName = Request("ReportName")
ShowInList = Request("ShowInList")
InvSchema = Application("CHEMINV_USERNAME")

'Optional parameters
ReportFormat = Request("ReportFormat")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/apiCreateLocationBarcodeReport.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(LocationID) then
	strError = strError & "LocationID is a required parameter.<BR />"
	bWriteError = True
End if
If IsEmpty(ReportName) then
	strError = strError & "ReportName is a required parameter.<BR />"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

'Check optional parameters
if ReportFormat = "" then
	ReportFormat = "SNP"
End if

SQL = " select inv_locations.location_id, inv_locations.parent_id, inv_locations.description, inv_location_types.location_type_name, inv_locations.location_name, inv_locations.location_description, inv_locations.location_barcode as barcode, "
SQL = SQL & InvSchema & ".GUIUTILS.GETLOCATIONPATH(inv_locations.location_id) as location_path, "
SQL = SQL & " inv_locations.owner_id_fk, inv_locations.creator, inv_address.contact_name, inv_address.address1, inv_address.address2, inv_address.address3, inv_address.address4, inv_address.city, inv_states.state_name, inv_states.state_abbreviation, "
SQL = SQL & " inv_country.country_name, inv_address.zip, inv_address.fax, inv_address.phone, inv_address.email "
SQL = SQL & " from inv_locations, inv_location_types, inv_address, inv_states, inv_country "
SQL = SQL & " where inv_locations.location_id = " & LocationID
SQL = SQL & " and inv_locations.location_type_id_fk = inv_location_types.location_type_id "
SQL = SQL & " and inv_locations.address_id_fk = inv_address.address_id(+) "
SQL = SQL & " and inv_address.state_id_fk = inv_states.state_id(+) "
SQL = SQL & " and inv_address.country_id_fk = inv_country.country_id(+) "

QueryText = SQL
QueryName = "qryLocationBarcodeReport"

'Response.Write SQL
'Response.End

if bDebugPrint then
	'Debugging section
	Response.write("QueuePath   : " & ReportQueuePath & "<br>")
	Response.write("DatabasePath: " & ReportDbPath & "<br>")
	Response.write("ReportName  : " & ReportName & "<br>")
	Response.write("ReportDirectory  : " & ReportDirectory & "<br>")
	Response.write("QueryName  : " & QueryName & "<br>")
	Response.write("QueryText  : " & QueryText & "<br>")
	Response.write("ReportFormat: " & ReportFormat & "<br>")
Else
	'Create RPT
	Set ReportQ = Server.CreateObject("ReportQ.CReportQ")
	ReportFileName = ReportQ.MakeReport(ReportQueuePath, ReportDBPath, ReportName, QueryName, QueryText, ReportFormat)
	Response.ContentType = "text/html"
	If InStr(1,ReportFileName,"Report Error") = 0 Then	
		Response.Write ReportsHTTPPath & ReportFileName
	Else 
		Response.Write ReportFileName
	End if
End if
%>
