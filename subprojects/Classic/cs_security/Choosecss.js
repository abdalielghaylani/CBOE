// Choose the appropriate stylesheet
if (navigator.appName == "Netscape"){
	NS = true;
	document.write('<LINK REL=STYLESHEET HREF="/cs_security/cs_security_ns.css" TYPE="text/css">');
}
else{
	NS = false;
	document.write('<LINK REL=STYLESHEET HREF="/cs_security/cs_security_ie.css" TYPE="text/css">');
}
