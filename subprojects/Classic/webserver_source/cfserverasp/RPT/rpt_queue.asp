<% 
'Shows the report queue
'NOTE: This example shows the most recent 20 entries

	TheQueuePath = "F:\InetPub\Scripts\reportqueue.mdb"
      TheExample = Request.QueryString("Example")

	set ReportQ = Server.CreateObject("ReportQ.CReportQ")

      Select Case TheExample
	Case 8
         QueueSQL = ReportQ.WriteQueueHTML(TheQueuePath,200)
      Case 9
	   Result = ReportQ.ArchiveQueue(TheQueuePath)
         if Result = 1 then 
            QueueSQL = "Queue has been Archived" 
         else
            QueueSQL = "Queue Archive operation encountered an error" 
         end if 
      end select
	set ReportQ = Nothing
%>
	<html><head><title>RPT Software - Report Queue</title></head>
	<font size="6" color="#000080"><strong>Report Server - Queue</strong>
	</font><br><hr>
	<%=QueueSQL%>	
	</BODY></HTML>

