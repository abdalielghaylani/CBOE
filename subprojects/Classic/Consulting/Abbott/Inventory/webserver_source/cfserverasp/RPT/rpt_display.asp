<% 
Response.Buffer = False
'---------------------------------------------------------------------------
'Please change the following lines to the correct paths
'Note: All paths are assuming you are on the server (no mapped drives etc...)

   TheQueuePath = "C:\inetpub\wwwroot\chemoffice\webserver_source\RPT\reportqueue.mdb"
   ExampleDatabasePath = "C:\inetpub\wwwroot\chemoffice\webserver_source\RPT\rpt_example.mdb"
   ArchiveDatabasePath = "C:\inetpub\wwwroot\chemoffice\webserver_source\RPT\reportqueuearchive.mdb"
	
   TheReportFormat = "PDF"

   ReportDirectory = "/cfserverasp/RPT/Reports/"
'---------------------------------------------------------------------------

'Get the querystring paramter to determine which example to run
   TheExample = Request.QueryString("Example")
%>
<head>
<meta name="GENERATOR" content="Microsoft FrontPage 3.0">
<meta http-equiv="Pragma" content="no-cache">
<title>RPT Software Report Display</title></head>
<body bgcolor="#FFFFFF" text="#000000" link="#000080" vlink="#000080" alink="#0000FF"
topmargin="0" leftmargin="5"><p align="center"><br><br>
Please wait as your requested Example Report is processed....</p>
<p align="center"><font size="4" color="#000080">&nbsp;</font><img src="ball.gif"
width="105" height="45" alt="Please wait."><br></p>
<% 

'Create the object so we can use the ReportQ to submit report requests
   set ReportQ = Server.CreateObject("ReportQ.CReportQ")

'Check how many reports are sitting in the reportQ that are waiting to be created
'If there are too many reports waiting return a message to the user that
'things are too busy right now and to come back later.  I am using 5 in this
'example, however this number could be anything you want or you could
'just skip checking the queuesize and just create the report (to save a second or so). 
   QueueSize = ReportQ.GetQueueSize(TheQueuePath)
   if (QueueSize > 5) then
%>
   <p align="center"><font size="4" color="#000080"><strong><br><br><br>
   Sorry - The report server is too busy to create your report.<br>
   Please click the back button and try again after a few minutes.</strong></font></p>

<%
   else
      Select Case TheExample
	Case 1
         TheDatabasePath = ExampleDatabasePath
	   TheReportName = "rptExample"
         TheQueryName = ""
         TheQueryText = ""
	Case 2
         TheDatabasePath = ExampleDatabasePath
         TheReportName = "rptExample"
         TheQueryName = ""
         TheQueryText = "WHERE Left(ProductName,1) = ""C"""
	Case 3
         TheDatabasePath = ExampleDatabasePath
         TheReportName = "rptExample"
         TheQueryName = ""
         TheQueryText = "WHERE Left(ProductName,1) = """ & Request("cboProduct") & """"
         TheReportFormat = Request("cboReportFormat")
	Case 4
         TheDatabasePath = ExampleDatabasePath
         TheReportName = "rptDynamicExample"
         TheQueryName = "qryDynamicExample"
         TheQueryText = "SELECT tblExample.* FROM tblExample WHERE (((Left([ProductName],1))='C' Or (Left([ProductName],1))='G' Or (Left([ProductName],1))='N'));"
	Case 5
         TheDatabasePath = ExampleDatabasePath
         TheReportName = "rptDynamicExample"
         TheQueryName = "qryDynamicExample"
         TheQueryText = "SELECT tblExample.* FROM tblExample WHERE (Left([ProductName],1)='" & Request("cboProduct") & "');"
         TheReportFormat = Request("cboReportFormat")
	Case 6
         TheDatabasePath = ExampleDatabasePath
         TheReportName = "rptMult"
         TheQueryName = "qryMult1|qryMult2|qryMult3"
         TheQueryText = "SELECT tblExample.* FROM tblExample WHERE (((Left([ProductName],1))='T'));|SELECT tblExample.* FROM tblExample WHERE (((Left([ProductName],1))='N'));|SELECT tblExample.* FROM tblExample WHERE (((Left([ProductName],1))='C'));"
	Case 7
         TheDatabasePath = ArchiveDatabasePath
         TheReportName = "rptUsage"
         TheQueryName = ""
         TheQueryText = ""


	end select

'Debugging section
Response.write("--------- Debugging ------------------------<br>")
Response.write("The following values are being passed in to the MakeReport method:<br>")
Response.write("QueuePath   : " & TheQueuePath & "<br>")
Response.write("DatabasePath: " & TheDatabasePath & "<br>")
Response.write("ReportName  : " & TheReportName & "<br>")
Response.write("QueryName  : " & TheQueryName & "<br>")
Response.write("QueryText  : " & TheQueryText & "<br>")
Response.write("ReportFormat: " & TheReportFormat & "<br>")
Response.write("--------------------------------------------<br>")
 
'This is the main method to create a report.  Note: You can also pass in up to 10 optional parameters
'to store information in the reportqueue table.  QueueSize is a good example of something you might want to track,
'any kind of information on who the user are other good examples.  All of this information can then be used
'to create reports showing which reports where created, by who, when, did errors occur, did the user wait   
      TheReportFile = ReportQ.MakeReport(TheQueuePath, TheDatabasePath, TheReportName, TheQueryName, TheQueryText, TheReportFormat)

      If Left(TheReportFile,12) <> "Report Error" Then
         sTemp = ReportDirectory & TheReportFile
'Now redirect the file to the user
'You could also return a link to the report file 
'as an alternative approach
%>
         <script Language="JavaScript">
         <!---
            location.replace("<%=sTemp %>")
         //-->
         </script>

<%
      Else
%>

<p align="center"><font size="4" color="#000080"><strong>
The following error occured while attempting to produce your report:<br>
</p><%=TheReportFile%></strong></font>
<%
      End If
   end if
   set ReportQ = Nothing
%>
</body>
</html>


