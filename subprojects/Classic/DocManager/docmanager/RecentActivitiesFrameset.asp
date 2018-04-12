<%
submissionTime = Request("submissionTime")
sortBy = Request("sortBy")
days = Request("days")

%>	
<HTML>
<HEAD>
<TITLE>Document submission</TITLE>
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
</HEAD>

<frameset rows="325,*" border="0">
	<frame name="upper" src="RecentActivitiesUpper.asp?submissionTime=<%=submissionTime%>&sortBy=<%=sortBy%>&days=<%=days%>">
	<frame name="lower" src="RecentActivitiesLower.asp?submissionTime=<%=submissionTime%>&sortBy=<%=sortBy%>&days=<%=days%>">
</frameset>
</HTML>
