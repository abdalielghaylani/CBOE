// Choose the appropriate stylesheet

if (navigator.appName == "Netscape") {
	
	version = getBrowserVersion();
	if(version>=10)
	{
	
	NS = false;
	
	if ((useStyleSheet == 'biosar')||(useStyleSheet == ''))
	{
		useStyleSheet =  '/biosar_browser/source/biosar_ie.css';
	}
	else
	{
		useStyleSheet =  '/biosar_browser/config/styles/' + useStyleSheet;
	}
	document.write('<LINK REL=STYLESHEET HREF="' + useStyleSheet + '" TYPE="text/css">');
	
	}
	else
	{
	NS = false;
	NS = true;
	
	document.write('<LINK REL=STYLESHEET HREF="/biosar_browser/source/biosar_ns.css" TYPE="text/css">');
	}
	
}
else{
	
	NS = false;
	//document.write('<LINK REL=STYLESHEET HREF="/biosar_browser/source/biosar_ie.css" TYPE="text/css">');
	if ((useStyleSheet == 'biosar')||(useStyleSheet == '')){
		useStyleSheet =  '/biosar_browser/source/biosar_ie.css';
	}
	else
	{
		useStyleSheet =  '/biosar_browser/config/styles/' + useStyleSheet;
	}
	document.write('<LINK REL=STYLESHEET HREF="' + useStyleSheet + '" TYPE="text/css">');


}

function getBrowserVersion() {
	
		var str = navigator.appVersion;
		var i = str.indexOf("MSIE");
		if (i >= 0) {
			str = str.substr(i + 4);
			return parseFloat(str);
		}
		else {
			//IE 10+
			var isAtLeastIE10 = !!navigator.userAgent.match(/Trident/);
			
			if (isAtLeastIE10) {
				return 10;
			}
		
			return 0;
		}
	
}



