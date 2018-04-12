// Select style sheet based on browser
if (navigator.appName == "Netscape"){
      document.write('<LINK REL=STYLESHEET HREF="help_ns.css" TYPE="text/css">');
      }
   else{
      document.write('<LINK REL=STYLESHEET HREF="help_ie.css" TYPE="text/css">');
   }