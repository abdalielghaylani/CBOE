<%'Option Explicit%>

<%'SYAN modified on 1/16/2006 to fix CSBR-54770%>
<!--#INCLUDE VIRTUAL="/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<%'End of SYAN modification%>

<%
dim dbkey
dim batchload_username, batchload_password, loadFrom, indexOn, loadTime, instant, midNight, morning, noon, evening, cancelInstant, instantExecuteTime, cnn, cmd
dim output
dim fso
dim scheduleStatus

StoreASPSessionID()

dbkey="docmanager"

loadFrom = request("loadFrom")
indexOn = request("indexOn")

instant = Request("instant")
midNight = Request("midNight")
morning = Request("morning")
noon = Request("noon")
evening = Request("evening")

cancelInstant = Request("cancelInstant")
instantExecuteTime = Request("instantExecuteTime")

'batchload_username = Request("batchload_username")
'batchload_password = Request("batchload_password")

batchload_username = Application("BATCH_LOAD_USERNAME")
batchload_password = Application("BATCH_LOAD_PWD")

  'stop
if loadFrom <> "" then
	Set fso = CreateObject("Scripting.FileSystemObject")
	if not fso.FolderExists(loadFrom) then
		output = "<font color=""red"">The 'Load From' location you specified is not a valid location. Please correct it and retry.</font>"
	else
		CleanUpJobs
		
		if loadFrom <> "" then
			WriteConfigEntry "LOAD_FROM", loadFrom
		end if
		
		if instant = "ON" then
			scheduleStatus = ScheduleJob("FileBatchLoad_Instant", "once", 5, "", batchload_username, batchload_password) '5 minutes later
			loadTime = loadTime & DateAdd("n", 5, Now())
      elseif UCase(cancelInstant) = "FALSE" then
      'Since all jobs are cleared first, need to reschedule the instant task.
      scheduleStatus = ScheduleJob("FileBatchLoad_Instant", "once", 0, instantExecuteTime, batchload_username, batchload_password) 'whatever was scheduled before
      loadTime = loadTime & "," & instantExecuteTime
      end if

      if midNight = "ON" then 'has scheduled task
      scheduleStatus = ScheduleJob("FileBatchLoad_Midnight", "daily", 0, "00", batchload_username, batchload_password)
      loadTime = loadTime & "," & "00:00"
      end if

      if morning = "ON" then 'has scheduled task
      scheduleStatus = ScheduleJob("FileBatchLoad_Morning", "daily", 0, "06", batchload_username, batchload_password)
      loadTime = loadTime & "," & "6:00"
      end if

      if noon = "ON" then 'has scheduled task
      scheduleStatus = ScheduleJob("FileBatchLoad_Noon", "daily", 0, "12", batchload_username, batchload_password)
      loadTime = loadTime & "," & "12:00"
      end if

      if evening = "ON" then 'has scheduled task
      scheduleStatus = ScheduleJob("FileBatchLoad_Evening", "daily", 0, "20", batchload_username, batchload_password)
      loadTime = loadTime & "," & "20:00"
		end if
				
		
		WriteConfigEntry "LOAD_TIME", loadTime
		
		if scheduleStatus = "" or Instr(scheduleStatus, "ERROR:") > 0 then
			output = "<font color=red>No batch submission is scheduled. " & scheduleStatus & "<br>Please try again.</font>"
		else
			output = "Your configuration of batch submission has been saved."
		end if
	end if
	
end if

if indexOn = "ON" then
	ToggleIndex "ON"
	Application("indexOn") = true

else
	ToggleIndex "OFF"
	Application("indexOn") = false
end if


Sub ToggleIndex(flag)
	Set cnn = Server.CreateObject("ADODB.Connection")
	with cnn
		.ConnectionString = Application("cnnStr")
	end with
	cnn.Open

	set cmd = Server.CreateObject("ADODB.Command")

	cmd.ActiveConnection = cnn	
	
	cmd.CommandType = 4 'Stored Procedure
	
	if flag = "ON" then
		cmd.CommandText = "CTX_SCHEDULE_DOCMANAGER.STARTUP ('INDEX_DOCMGR_DOCUMENTS', 'SYNC', 1)"
		on error resume next
		cmd.Execute()
		
		cmd.CommandText = "CTX_SCHEDULE_DOCMANAGER.STARTUP ('INDEX_DOCMGR_DOCUMENTS', 'OPTIMIZE FAST', 60)"
		on error resume next
		cmd.Execute()
	elseif flag = "OFF" then
		cmd.CommandText = "CTX_SCHEDULE_DOCMANAGER.STOP ('INDEX_DOCMGR_DOCUMENTS', 'OPTIMIZE FAST')"
		on error resume next
		cmd.Execute()
	end if
	
	cnn.Close
	Set cnn = nothing
	set cmd = nothing
End Sub 

function ReadConfigEntry(key)
	dim fso, f, lineText, lineNumber, i, replaceString, retVal
	
	retVal = ""
	
	Set fso = CreateObject("Scripting.FileSystemObject")
	Set f = fso.OpenTextFile(Server.MapPath("/docmanager/FileBatchLoad/config.ini"), 1) 'ForReading
	
	' Get file content into an array:
	Dim contents
	contents = Split(f.ReadAll, vbCrLf)
	f.close
	
	For i = LBound(contents) to Ubound(contents)
		lineText = contents(i)
		if instr(lineText, key & "=") = 1 then
			retVal = Right(lineText, Len(lineText) - Len(key & "="))
			exit for
		end if
	Next
	
	ReadConfigEntry= retVal
 end function

'Replace the existing entry with the new value
function WriteConfigEntry(key, value)
	dim f, s, lineNumber, i, replaceString
	Set f = fso.OpenTextFile(Server.MapPath("/docmanager/FileBatchLoad/config.ini"), 1) 'ForReading
	
	' Get file content into an array:
	Dim contents
	contents = Split(f.ReadAll, vbCrLf)
	
	f.close
	
	For i = LBound(contents) to Ubound(contents)
		if instr(contents(i), key & "=") = 1 then
			lineNumber = i
			exit for
		end if
	Next
	
	replaceString = key & "=" & value
	contents(lineNumber) = replaceString

	Set f = fso.OpenTextFile(Server.MapPath("/docmanager/FileBatchLoad/config.ini"), 2) 'ForWriting
	f.Write Join(contents, vbCrLf)
	f.Close
end function

'Append the existing entry with the new value
function AppendConfigEntry(key, value)
	dim f, s, lineNumber, i, replaceString, keyValueArr, oldValue, newValue
	
	Set f = fso.OpenTextFile(Server.MapPath("/docmanager/FileBatchLoad/config.ini"), 1) 'ForReading
	
	' Get file content into an array:
	Dim contents
	contents = Split(f.ReadAll, vbCrLf)
	
	f.close
	
	For i = LBound(contents) to Ubound(contents)
		if instr(contents(i), key & "=") = 1 then
			lineNumber = i
			exit for
		end if
	Next
	
	keyValueArr = split(contents(lineNumber), "=")
	oldValue = keyValueArr(1)
	if oldValue <> "" then
		newValue = oldValue & "," & value	
	else
		newValue = value
	end if
	
	replaceString = key & "=" & newValue
    contents(lineNumber) = replaceString

    Set f = fso.OpenTextFile(Server.MapPath("/docmanager/FileBatchLoad/config.ini"), 2) 'ForWriting
    f.Write Join(contents, vbCrLf)
    f.Close

    end function

    function CleanUpJobs
    'stop
    Dim taskNames
    dim scheduler
    dim idArr, i

    set scheduler = Server.CreateObject("Scheduler.TaskScheduler")

    taskNames = ReadConfigEntry("TASK_NAMES")

    if taskNames <> "" then
    idArr = split(taskNames, ",")
    for i = Lbound(idArr) to UBound(idArr)
    if idArr(i) <> "" then
      on error resume next
      scheduler.DeleteTask(idArr(i))
      end if
      next
      end if

      WriteConfigEntry "TASK_NAMES", ""

      set scheduler = nothing

      End function

      function ScheduleJob(taskName, frequency, interval, scheduleHour, username, password)
      dim scheduler
      dim taskID

      set scheduler = Server.CreateObject("Scheduler.TaskScheduler")

      if frequency = "once" then
      on error resume next
      taskName = scheduler.ScheduleTask (taskName, Server.MapPath("/docmanager/filebatchload/FileBatchLoad.exe"), "once", CInt(interval), scheduleHour, username, password)
      'taskName = scheduler.ScheduleTask (taskName, Server.MapPath("/docmanager/filebatchload/FileBatchLoad.exe"), "once", CInt(interval), scheduleHour, "camsoft_admin", "cambridgesoft")
      'taskName = scheduler.ScheduleTask (taskName, Server.MapPath("/docmanager/filebatchload/FileBatchLoad.exe"), "once", CInt(interval), scheduleHour, "system", "")
      elseif frequency = "daily" then
      on error resume next
      taskName = scheduler.ScheduleTask (taskName, Server.MapPath("/docmanager/filebatchload/FileBatchLoad.exe"), "daily", CInt(interval), scheduleHour, username, password)
      'taskName = scheduler.ScheduleTask (taskName, Server.MapPath("/docmanager/filebatchload/FileBatchLoad.exe"), "daily", CInt(interval), scheduleHour, "camsoft_admin", "cambridgesoft")
      'taskName = scheduler.ScheduleTask (taskName, Server.MapPath("/docmanager/filebatchload/FileBatchLoad.exe"), "daily", CInt(interval), scheduleHour, "camsoft_admin", "")
      end if

      AppendConfigEntry "TASK_NAMES", taskName

      set scheduler = nothing

      ScheduleJob = taskName
      end function

      %>
      <html>
<head>
<title>Document submission</title>


</head>

<body background="<%=Application("UserWindowBackground")%>">

	<table border="0" bordercolor="fuchsia" cellspacing="0" cellpadding="0">
		<!-- The table for the banner. -->
		<tr>

			<td valign="top" width="300">
				<img src="/docmanager/docmanager/gifs/banner.gif" border="0">
			</td>

			<td>
					<font face="Arial" color="#0099FF" size="4"><i>
						Batch Load Document
					</i></font>
			</td>
		</tr>
	</table>
	
	<table width="7000">
		<tr><td width="100"></td>
		
			<td colspan="2" align="left">
				<a href="/<%=Application("appkey")%>/inputtoggle.asp?formgroup=base_form_group&amp;dbname=<%=dbkey%>" target="_top"><img src="/<%=Application("appkey")%>/graphics/searchdoc_btn.gif" border="0"></a>
				<a href="/<%=Application("AppKey")%>/docmanager/src/locateDocs.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/submit_btn.gif" border="0"></a>						
				<a href="/<%=Application("appkey")%>/docmanager/BatchLoadConfigForm.asp"><img src="/<%=Application("appkey")%>/graphics/batchSubmission_btn.gif" border="0"></a>
				<a href="/<%=Application("appkey")%>/docmanager/RecentActivitiesFrameset.asp"><img src="/<%=Application("appkey")%>/graphics/recent_activities_btn.gif" border="0"></a>
				<a href="/docmanager/docmanager/mainpage.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/mainmenu.gif" border="0"></a>
				<a href="/cs_security/home.asp"><img src="/<%=Application("appkey")%>/graphics/home_oval_btn.gif" border="0"></a>
				<a href="/<%=Application("appkey")%>/logoff.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/log_off_big_btn.gif" border="0"></a>
			</td>
		</tr>
				
		<tr><td width="100"></td>
			<td>
				<a href="/CBOEHelp/CBOEContextHelp/Doc Manager Webhelp/Default.htm" target="new"><img src="/docmanager/graphics/help_btn.gif" border="0"></a>
				<a href="#" onclick="window.open('/docmanager/about.asp', 'about', 'width=560,height=450,status=no,resizable=yes')"><img src="/cfserverasp/source/graphics/navbuttons/about_btn.gif" border="0"></a>
			</td>
		</tr>
	</table>
	
	<table width="660" border="0" cellpadding="0" cellspacing="0">
		<tr><td width="100"></td>
			<td><table border="0" width="560" cellspacing="0" cellpadding="0">
					
					<tr>
						<td align="right" width="450"><font color="#000099">You are logged in as <font color="#990000"><%=Session("UserName" & Application("appkey"))%></font></font></td>
					</tr>
					
					<tr><td height="20"></td></tr>
				</table>

				<table cellspacing="0" cellpadding="0">
					<tr>
						<td><font size="3"><%=output%></font></td>
					</tr>
				</table>
			</td>
		</tr>
	</table>

	<table width="660">
		<tr><td width="100">
			</td>
				
			<!--td><input type="submit" name="viewBtn" value="View Logs of Recent Submissions">
			</td-->
				
			<td align="center"><a href="ViewLogsFrameset.asp"><img src="/<%=Application("appkey")%>/graphics/viewlogs_btn.gif" border="0"></td>
		</tr>
		
	</table>

</body>
</html>

<%
%>
