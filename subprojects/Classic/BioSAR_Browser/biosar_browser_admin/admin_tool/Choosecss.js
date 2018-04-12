// Choose the appropriate stylesheet
if (navigator.appName == "Netscape"){
	NS = true;
	document.write('<LINK REL=STYLESHEET HREF="/biosar_browser/biosar_browser_admin/admin_tool/biosar_ns.css" TYPE="text/css">');
}
else{
	NS = false;
	document.write('<LINK REL=STYLESHEET HREF="/biosar_browser/biosar_browser_admin/admin_tool/biosar_ie.css" TYPE="text/css">');
}
