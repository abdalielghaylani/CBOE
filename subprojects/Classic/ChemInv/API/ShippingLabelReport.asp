<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim strError
Dim bWriteError
Dim PrintDebug

bDebugPrint = false
bWriteError = False
strError = "Error:CreateContainerReport<BR>"

'RPT paths
RPTPath = Application("RPT_PATH")
ReportQueuePath = RPTPath & "reportqueue.mdb"
ReportArchiveDBPath =  RPTPath & "reportqueuearchive.mdb"	
ReportDBPath = Application("ReportDBPath")
ReportsHTTPPath = Application("ReportsHTTPPath") 

'Required Paramenters
OrderID= Request("OrderID")
ReportName = "rptShippingLabel1" 'Request("ReportName")



' Redirect to help page if no parameters are passed
'If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
'	Response.Redirect "/cheminv/help/admin/apiCreateContainerReport.htm"
'	Response.end
'End if

If IsEmpty(ReportName) then
	strError = strError & "ReportName is a required parameter<BR>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if


ReportFormat = "SNP"

QueryText = "select L.Location_Name,O.SHIP_TO_NAME,OC.CONTAINER_ID_FK,C.Barcode " & vblf & _
             "from inv_orders O,inv_order_containers OC,inv_locations L,inv_Containers C " & vblf & _
             "where " & vblf & _
             "OC.order_ID_FK=O.order_ID " & vblf & _ 
             "and O.DELIVERY_LOCATION_ID_FK=L.location_id " & vblf & _
             "and C.CONTAINER_ID=OC.CONTAINER_ID_FK " & vblf & _
             "and O.order_ID=" & OrderID
             
QueryName = "qryShippingLabelReport"

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
	ReportFileName = ReportQ.GenerateReport(ReportQueuePath, ReportDBPath, ReportName, QueryName, QueryText, ReportFormat, Application("REPORT_WAIT_TIMEOUT"))
	Response.ContentType = "text/html"
	If InStr(1,ReportFileName,"Report Error") = 0 Then	
		Response.Write ReportFileName
	Else 
		Response.Write ReportFileName
	End if
End if
%>

