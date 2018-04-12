<%'SYAN modified on 1/16/2006 to fix CSBR-54770%>
<!--#INCLUDE VIRTUAL="/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL="/docmanager/docmanager/src/datetimefunc.asp"-->
<%'End of SYAN modification%>

<%

dim dbkey
dim loadTime, midNight, morning, noon, evening, showInstantCheckBox, instantExecuteTime
dim timeArr, i
dbkey="docmanager"

StoreASPSessionID()

loadTime = ReadConfigEntry("LOAD_TIME")

if loadTime <> "NULL" and loadTime <> "" then
	timeArr = Split(loadTime, ",")
	
	for i = Lbound(timeArr) to UBound(timeArr)
		select case timeArr(i)
		case "00:00" 
			midNight = true
		case "6:00"
			morning = true
		case "12:00"
			noon = true
		case "20:00" 
			evening = true
		case else
			if IsDate(timeArr(i)) then
				if DateDiff("s", Now(), timeArr(i)) < 0 then 'the task has already been executed
					showInstantCheckBox = true
				else 'the task has not been executed yet
					showInstantCheckBox = false
					instantExecuteTime = timeArr(i)
				end if	
			end if
		end select		
	Next
end if

if IsEmpty(showInstantCheckBox) then
	showInstantCheckBox = true	
end if

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

%>
<html>
<head>
<title>Document submission</title>

<script language="javascript">
	function CheckFieldsAndSubmit(){
		if ((this.document.configForm.loadFrom.value) == ''){
			alert('The \'Load from\' field can not be empty.');
		}
		else {
			this.document.forms[0].submit();
		}
	}
</script>
<!-- CBOE-1823 added code to display Document Manager help on F1 click. Debu 05SEP13 -->
 <script language="javascript" type="text/javascript">
     function onkeydown_handler() {
         switch (event.keyCode) {
             case 112: // 'F1'
                 document.onhelp = function () { return (false); }
                 window.onhelp = function () { return (false); }
                 event.returnValue = false;
                 event.keyCode = 0;
                 window.open('../../../../CBOEHelp/CBOEContextHelp/Doc%20Manager%20Webhelp/Default.htm');
                 return false;
                 break;
         }
     }
     document.attachEvent("onkeydown", onkeydown_handler);
    </script>
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

<table width="660" border="0" cellpadding="0" cellspacing="0">
	<tr><td width="100"></td>
		<td width="560">
			
			<table width="560">
				<tr><td colspan="2" align="left">
						<a href="/<%=Application("appkey")%>/inputtoggle.asp?formgroup=base_form_group&amp;dbname=<%=dbkey%>" target="_top"><img src="/<%=Application("appkey")%>/graphics/searchdoc_btn.gif" border="0"></a>
						<%if session("SUBMIT_DOCS" & dbkey) then%>
						<a href="/<%=Application("AppKey")%>/docmanager/src/locateDocs.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/submit_btn.gif" border="0"></a>						
						<%end if%>
						<%if session("VIEW_HISTORY" & dbkey) then%>
						<a href="/<%=Application("appkey")%>/docmanager/RecentActivitiesFrameset.asp"><img src="/<%=Application("appkey")%>/graphics/recent_activities_btn.gif" border="0"></a>
						<%end if %>
						<a href="/docmanager/docmanager/mainpage.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/mainmenu.gif" border="0"></a>
						<a href="/cs_security/home.asp"><img src="/<%=Application("appkey")%>/graphics/home_oval_btn.gif" border="0"></a>
						<a href="/<%=Application("appkey")%>/logoff.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/log_off_big_btn.gif" border="0"></a>
					</td>
				</tr>
				
				<tr><td>
						<a href="/CBOEHelp/CBOEContextHelp/Doc Manager Webhelp/Default.htm" target="new"><img src="/docmanager/graphics/help_btn.gif" border="0"></a>
						<a href="#" onclick="window.open('/docmanager/about.asp', 'about', 'width=560,height=450,status=no,resizable=yes')"><img src="/cfserverasp/source/graphics/navbuttons/about_btn.gif" border="0"></a>
					</td>
				</tr>
			</table>
			
			<table>
				<tr>
					<td align="right" width="450"><font color="#000099">You are logged in as <font color="#990000"><%=Session("UserName" & Application("appkey"))%></font></font></td>
				</tr>
			</table>
		</td>
	</tr>
</table>
	
<table width="660" border="0" cellpadding="0" cellspacing="0" border="0">
	<tr><td width="100"></td>
		<td><table border="0" width="560" cellspacing="0" cellpadding="0">
				
				<tr><td height="20"></td></tr>
			</table>

			<table cellspacing="0" cellpadding="0">
				<tr>
					<td><font size="3">Configure the system to batch submit documents by specifying following parameters:</font></td>
				</tr>
			</table>
		</td>
	</tr>
</table>

<form name="configForm" method="post" action="ConfigBatchLoad.asp" target="_self">
<table width="660" border="0">
	<tr>
		<td width="100" nowrap>&nbsp;</td>
		
		<td colspan="2"><font size="2">This page should only be edited by application administrator. The information
						on this page is global, it affects all users.
						<br>
						<br>
						</font>
		</td>
	</tr>

	<tr><td width="100">&nbsp;</td>
		
		<td valign="top" nowrap>Load from:
		</td>
		
		<td>
			<table>
				<tr><td><input type="text" name="loadFrom" value="<%=ReadConfigEntry("LOAD_FROM")%>" size="70">
					</td>
				</tr>
				
				<tr><td><font size="2">The path where the to-be-loaded files are. Note this is the location on the server.
						</font>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	
	<tr><td colspan="3" height="20"></td></tr>

  <!--tr>
    <td width="100">&nbsp;</td>
    <td>Load As: </td>
    <td>
    <input type="text" name="batchload_username" value="camsoft_admin"></input>
    </td>
  </tr>

  <tr>
    <td width="100">&nbsp;</td>
    <td>Password: </td>
    <td><input type="password" name="batchload_password" value=""></td>
  </tr-->

  <tr>
    <td colspan="3" height="20"></td>
  </tr>
	<tr><td width="100">&nbsp;</td>
		
		<td valign="top" nowrap>When to load:
		</td>
		
		<td><%if showInstantCheckBox = true then%>
				<input type="checkbox" name="instant" value="ON">Run once in five minutes
			<%else%>
				<%
				if Application("DATE_FORMAT") = 9 then
					instantExecuteTimeDisp = fmtDateTime(CDATE(instantExecuteTime),"dd/mm/yyyy  hh:mm:ss")
				elseif Application("DATE_FORMAT") = 10 then
					instantExecuteTimeDisp = fmtDateTime(CDATE(instantExecuteTime),"yyyy/mm/dd  hh:mm:ss")
				else
					instantExecuteTimeDisp = instantExecuteTime
				end if
				%>
				<font color="blue">The batch load will be executed at <%=instantExecuteTimeDisp%>.</font>
				<input type="hidden" name="cancelInstant" value="false">
				<input type="hidden" name="instantExecuteTime" value="<%=instantExecuteTime%>">
				<a href="javascript:this.document.forms['configForm'].cancelInstant.value='true';CheckFieldsAndSubmit();">Cancel It</a>
			<%end if%>
			<br><br>
			And/Or Everyday at:
			<br><br>
			<input type="checkbox" name="midNight" value="ON" <%if midNight = true then %>checked<%end if%>>Mid-night
			<br>
			<input type="checkbox" name="morning" value="ON" <%if morning = true then %>checked<%end if%>>6:00 AM
			<br>
			<input type="checkbox" name="noon" value="ON" <%if noon = true then %>checked<%end if%>>Noon
			<br>
			<input type="checkbox" name="evening" value="ON" <%if evening = true then %>checked<%end if%>>8:00 PM
		</td>
	</tr>
	
	<tr><td colspan="3" height="10"></td></tr>

	<tr>
		<td width="100">&nbsp;</td>
		
		<td colspan="2"><font size="2">Note: Batch submission will significantly decrease the server performance 
			for normal users. We recommend the submission to be executed at low traffic times
			and run no more than twice a day.</font>
		</td>
	</tr>

	<tr><td colspan="3" height="20"></td></tr>

	<tr><td width="100">&nbsp;</td>
		
		<td valign="top" nowrap>Indexing:
		</td>
		
		<td><input type="checkbox" name="indexOn" value="ON" <%if Application("indexOn") = true or Application("indexOn") = "" then%>checked<%end if%>>Index automatically
		</td>
	</tr>
	
	<tr><td colspan="3" height="10"></td></tr>

	<tr>
		<td width="100">&nbsp;</td>
		
		<td colspan="2"><font size="2">Note: Turn full-text index off will significantly improve the loading performance.
		If you turn it off, you need to come back to this page and turn it on after the loading finishes. Click ok after turn indexing on. 
		Otherwise the documents will not be text searchable.</font>
		</td>
	</tr>

	<tr><td colspan="3" height="10"></td></tr>

	<tr><td width="100">&nbsp;</td>
				
		<td colspan="2" align="center">
			<a href="javascript:CheckFieldsAndSubmit();"><img src="/<%=Application("appkey")%>/graphics/ok_dialog_btn.gif" border="0"><a href="mainpage.asp"><img src="/<%=Application("appkey")%>/graphics/cancel_dialog_btn.gif" border="0"></td>
	</tr>
</table>
</form>

<table width="660">
	<tr><td width="100">&nbsp;</td>
						
		<td align="center"><a href="ViewLogsFrameset.asp"><img src="/<%=Application("appkey")%>/graphics/viewlogs_btn.gif" border="0"></td>
	</tr>
	
</table>


</body>
</html>
