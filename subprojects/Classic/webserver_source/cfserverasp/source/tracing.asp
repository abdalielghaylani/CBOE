<%@ Language=VBScript %>
<%
Response.Expires = 0
dbkey = Request.QueryString("dbkey")
action = UCase(Request.QueryString("action"))
scope = lcase(Request.QueryString("scope"))
if scope <> "application" AND scope <> "session" then scope = "session"

level = 0
if Session("CfwTraceLevel") <> "" then 
	scope = "session"
	level = Session("CfwTraceLevel")
end if	
if Application("CfwTraceLevel") <> "" then 
	scope = "application"
	level = Application("CfwTraceLevel")
end if	
if Session("PageTimer") = "on" then
	pTimer = Session("PageTimer")
elseif Application("PageTimer") = "on" then
	pTimer = Application("PageTimer")
else
	pTimer = "off"	
end if	

Select Case action 
	Case "DELETETRACE"
		if Application("COWSRoot")<>"" then cowsroot = Application("COWSRoot") & "\"
		filepath = 	Application("ServerDrive") & "\" & Application("ServerRoot") & "\" &  Application("DocumentRoot") & "\" &  cowsroot  & dbkey & "\logfiles\" & dbkey & "_trace.html"
		on error resume next
		Set fs = Server.CreateObject("Scripting.FileSystemObject")
		fs.DeleteFile filepath, true
		if err then
			Response.Write err.Description
		else
			Response.Write "1"
		end if
		Response.End	
	Case "STARTTRACING"
		level = Request.QueryString("level")
		
		
		

		if isNumeric(level) AND NOT IsEmpty(level) then  	
			if level = 0 then level = "" 
			Session("CfwTraceLevel") = ""
			Application.Lock
				Application("CfwTraceLevel") = ""
			Application.UnLock	
			if scope = "session" then 
				Session("CfwTraceLevel") = level
			elseif scope = "application" then
				Application.Lock
					Application("CfwTraceLevel") = level
				Application.UnLock
			End if
			Response.Write "1"
		else
			Response.Write "Invalid trace level value. Use numeric value."
		end if
		Response.End
		
	Case "STOPTRACING"
		Session("CfwTraceLevel") = ""
		Application.Lock
			Application("CfwTraceLevel") = ""
		Application.UnLock	
		Response.Write "1"
		Response.End
	Case "SETPAGETIMER"
		state = lcase(Request.QueryString("state"))
		if state <> "on" and state <> "off" then state = "off"
		if scope = "session" then
			Application.Lock
				Application("PageTimer") = "off"
			Application.UnLock 
			Session("PageTimer") = state
		elseif scope = "application" then
			Session("PageTimer") = "off"
			Application.Lock
				Application("PageTimer") = state
			Application.UnLock
		End if
		Response.Write "1"
		Response.End	
End Select	
%>
<!-- #include virtual="/cs_security/variables.asp" -->
<!-- #include virtual="/cs_security/functions.asp" -->

<%
PAGE_URL = "ChemOffice Enterprise -- Application Tracing"
PAGE_COLOR = color_blue
PAGE_APP = "header_" & color_blue & "_coent.gif"
TOP_NAV = "<a href=""/cfserveradmin/AdminSource/webeditor.asp""><b>Administrative Tools</b></a>&nbsp;|&nbsp;<a href=""http://www.cambridgesoft.com/services/topics.cfm?FID=6"" target=""_blank""><b>Technical Notes</b></a>"
%>

<!-- #include virtual="/cs_security/header.asp" -->
<SCRIPT LANGUAGE=javascript>
/*CSBR# 139460 
 Purpose: To change servername to include portnumber information
*/
	var serverName = window.location.host; //End of Change	
	var pageURL = window.location.href;
	var res_HTTP = pageURL.substr(0,5);
	function DeleteTrace(dbkey){
		var bContinue = true;
		bcontinue = confirm('Are you sure you want to delete the ' + dbkey + ' trace file?');
		if (bcontinue){ 
			if(res_HTTP == 'https'){
			var strURL = "https://" + serverName + "/" + dbkey + "/user_info.asp?manageTracing=1&action=deleteTrace&dbKey=" + dbkey;
			}
			else{
			var strURL = "http://" + serverName + "/" + dbkey + "/user_info.asp?manageTracing=1&action=deleteTrace&dbKey=" + dbkey;
			}
			var httpResponse = JsHTTPGet(strURL)
			if (httpResponse == '1'){
				alert('File deleted');		
			}
			else{
				alert('Error deleting trace file\r'+ httpResponse);
			}
		}	 
	}
	
	function GetRadioSelection(elm){
		for (i=0; i< elm.length; i++){
			if (elm[i].checked == true) return elm[i].value;
		}	
	}
	
	function StartTrace(){
		
		var level= document.getElementById("traceLevel").value;		
		var scope = GetRadioSelection(document.getElementsByName("traceScope"));		
		var bContinue;
		bContinue = confirm('Are you sure you want to start tracing for ' + dbkey + ' with ' + scope + ' scope and level ' + level + '?');

		if (bContinue){ 
			if(res_HTTP == 'https'){
			var strURL = "https://" + serverName + "/" + dbkey + "/user_info.asp?manageTracing=1&action=StartTracing&level=" + level + "&scope=" + scope;
			}
			else{
			var strURL = "http://" + serverName + "/" + dbkey + "/user_info.asp?manageTracing=1&action=StartTracing&level=" + level + "&scope=" + scope;
			}
			var httpResponse = JsHTTPGet(strURL);
			if (httpResponse == '1'){
				DisableStatusAndLevel(true)
				alert('Trace: On\rScope: ' + scope + '\rLevel: '+ level);		
			}
			else{
				alert('Error while starting trace\r'+ httpResponse);
			}
		}
		else{
			document.getElementsByName("traceStatus")[0].checked = true
		}	 
	}
	
	
	function StopTrace(){
		 
		var bContinue;
		bcontinue = confirm('Are you sure you want to stop tracing for ' + dbkey + '?');
		if (bcontinue){ 
			if(res_HTTP == 'https'){
			var strURL = "https://" + serverName + "/" + dbkey + "/user_info.asp?manageTracing=1&action=StopTracing";
			}
			else{
			var strURL = "http://" + serverName + "/" + dbkey + "/user_info.asp?manageTracing=1&action=StopTracing";
			}
			var httpResponse = JsHTTPGet(strURL)
			if (httpResponse == '1'){
				DisableStatusAndLevel(false)
				alert('Trace: Off\r');		
			}
			else{
				alert('Error while stoping trace\r'+ httpResponse);
			}
		}
		else{
			traceStatus[1].checked = true
		}	 	 
	}
	/////////////////////////////////////////////////////////////////////
	//	GetHTTP Content using xmlhttp
	//	
	function JsHTTPGet(strURL){
		if (window.XMLHttpRequest) {
                   var xhttp = new XMLHttpRequest();
                } else {                    
                    xhttp = new ActiveXObject("Microsoft.XMLHTTP");
                }
                xhttp.onreadystatechange = function () {
                    if (this.readyState == 4 && this.status == 200) {
                        return xhttp.responseText;
                    }
                };
                xhttp.open("GET", strURL, false);
                xhttp.send();
				return  xhttp.onreadystatechange();
	}
	
	function DisableStatusAndLevel(bool){
		document.getElementsByName("traceScope")[0].disabled = bool;
		document.getElementsByName("traceScope")[1].disabled = bool;
		document.getElementById("traceLevel").disabled = bool;	
	}

	function SetPageTimer(b){
		var bContinue;
		var state
		(b ? state = "on" : state = "off") 
		bcontinue = confirm('Are you sure you want to turn the page timer ' + state + ' for ' + dbkey + '?');
		if (bcontinue){
			var scope = GetRadioSelection(document.getElementsByName("traceScope"));
			if(res_HTTP == 'https'){
			var strURL = "https://" + serverName + "/" + dbkey + "/user_info.asp?manageTracing=1&action=SetPageTimer&state=" + state + "&scope=" + scope;
			}
			else{
			var strURL = "http://" + serverName + "/" + dbkey + "/user_info.asp?manageTracing=1&action=SetPageTimer&state=" + state + "&scope=" + scope;
			}
			var httpResponse = JsHTTPGet(strURL)
			if (httpResponse == '1'){
				alert('Timer: ' + state + '\r');		
			}
			else{
				alert('Error while setting page timer\r'+ httpResponse);
			}
		}
		else{
			pageTimer.checked = (!b==1);
		}		
	
	}
window.onload = function(){window.focus()}
</SCRIPT>



<table border="0" width="100%" cellpadding="0" style="border-collapse: collapse"><tr><td align="right">
<%
CS_SEC_UserName = UCase(Request.Cookies("CS_SEC_UserName"))
If Len(CS_SEC_UserName) > 0 Then Response.write "Current login: " & CS_SEC_UserName & "&nbsp;&nbsp;"
dim fs,fo,tfile
set fs=Server.CreateObject("Scripting.FileSystemObject")
if fs.FileExists("C:\inetpub\wwwroot\ChemOffice\"+dbkey+"\logfiles\"+dbkey+"_trace.html") then
response.write("")
else 
Set fo=fs.GetFolder("C:\inetpub\wwwroot\ChemOffice\"+dbkey+"\logfiles")
Set tfile=fo.CreateTextFile(dbkey+"_trace.html",false)
tfile.Close
end if
set tfile=nothing
set fo=nothing
set fs=nothing
%>	
</td></tr></table><br>

<input type="hidden" name="action" value="changeStatus">
<table>
	<tr>
		<th>
			Application
		</th>
		<th>
			Trace Level
		</th>
		<th>
			Trace Scope
		</th>
		<th>
			Trace Status
		</th>
		<th>
			View Trace
		</th>
		<th>
			Delete Trace
		</th>
	</tr>
	<tr>
		<td>
			<%=dbkey%>
		</td>
		<td align="center">
			<input type="text" id="traceLevel" name="traceLevel" value="20" size="3" xonchange="StartTrace();">			
		</td>
		<td>
			<input checked type="radio" name="traceScope" value="Session">Session
			<input type="radio" name="traceScope" value="Application">Application			
		</td>
		<td>
			<input checked type="radio" name="traceStatus" value="0" onclick="StopTrace()">off
			<input type="radio" name="traceStatus" value="1" onclick="StartTrace()">on
		</td>	
		<td>
			<a target="_new" href="/<%=dbkey%>/logfiles/<%=dbkey%>_trace.html">view</a>
		</td>
		<td>
			<a href="#" onclick="DeleteTrace('<%=dbkey%>'); return false;">delete</a>
		</td>
	</tr>
</table>
<BR><BR><input type="checkbox" id="pageTimer" name="pageTimer" value="1" onclick="SetPageTimer(this.checked);">			
Enable page timer for <%=dbkey%>
<center>
	<input type="button" value="Back" onClick="history.back()" id=button1 name=button1>
	<input type="button" value="Close" onClick="window.close()" id=button1 name=button1>
</center>
<SCRIPT LANGUAGE=javascript>
	var dbkey = "<%=dbkey%>";
	var scope = "<%=scope%>"
	var level = "<%=level%>"
	var pTimer = "<%=pTimer%>"
	
		
	
	document.getElementsByName("traceStatus")[0].checked = true
	document.getElementsByName("traceScope")[1].checked = true
	document.getElementById("traceLevel").value = 20
	if (level > 0){
		document.getElementById("traceLevel").value = level
		document.getElementsByName("traceStatus")[1].checked = true
		if (scope == "session") document.getElementsByName("traceScope")[0].checked = true;
		DisableStatusAndLevel(true);
	}
	var pt = document.getElementById("pageTimer");
	if ((pTimer == "on") && (!pt.checked)) pt.checked = true; 
	if ((pTimer == "off") && (pt.checked)) pt.checked = false;
</SCRIPT>

<!-- #include virtual="/cs_security/footer.asp" -->
