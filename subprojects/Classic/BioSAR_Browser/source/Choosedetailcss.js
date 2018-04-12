// Choose the appropriate stylesheet
if (navigator.appName == "Netscape"){
	NS = true;
	document.write('<LINK REL=STYLESHEET HREF="/biosar_browser/source/biosar_ns_detail.css" TYPE="text/css">');
}
else{
	NS = false;
	if ((useStyleSheet == 'biosar')||(useStyleSheet == '')){
		useStyleSheet =  '/biosar_browser/source/biosar_ie_detail.css';
	}
	else
	{
		useStyleSheet =  '/biosar_browser/config/styles/' + useStyleSheet;
	}
	document.write('<LINK REL=STYLESHEET HREF="' + useStyleSheet + '" TYPE="text/css">');
}
