// Choose the appropriate stylesheet
if (navigator.appName == "Netscape"){
	NS = true;
	document.write('<LINK REL=STYLESHEET HREF="chemreg_ns.css" TYPE="text/css">');
}
else{
	NS = false;
	document.write('<LINK REL=STYLESHEET HREF="chemreg_ie.css" TYPE="text/css">');
}
