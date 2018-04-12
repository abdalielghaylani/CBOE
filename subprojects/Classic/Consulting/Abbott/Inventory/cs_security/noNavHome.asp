<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils.js"-->
<SCRIPT LANGUAGE=javascript>
	
	var dw;
	var bypassUnloadEvent = false
	window.onload = function(){openNoNav()}
	window.onunload = function(){onUnload()}
	window.onbeforeunload = function(){onBeforeUnload(event)}
	
	
	//Never load the novav into the nonavdialog!
	if (window.name == "noNavDialog"){
		bypassUnloadEvent = true;
		window.location.href = "/cs_security/home.asp?nonav=1";
	}
	
	
	
	// Opens the home page in a dialog box with no navigation
	function openNoNav(){
		dw = OpenDialog("home.asp?noNav=1", "noNavDialog",4);
	}
	
	// Closes the previously opened dialog when this windo is closed
	function onUnload(){
		dw.close();
	}

	// warns about navigating away or closing this page
	function onBeforeUnload(ev){
		// While loggin off from the noNav dialog, login.asp page loads,
		// and the frame buster in it will reload it in the opener.
		// This causes this event to fire.  Since this event puts out an alert
		// which interrupts the frame buster, the noNav dialog page hangs.  To
		// avoid this condition we do not alert if we know the dialog window
		// is loading the login.asp page.  This is detected using the isLoginPage
		// global variable set in login.asp
		var isLogin
		
		// We protect against the case when the dialog window has been closed
		try{ 
			isLogin = dw.isLoginPage;		
		}
		catch (err){
			isLogin = false			
		}
		
		// We warn the user unless we know that we are going to the login page
		if ((!isLogin) && (!bypassUnloadEvent)) ev.returnValue = 'Navigating away from the this page does *NOT* end your session!\nYou must click the log off button or close your browser to prevent possible unathorized access to the system.';			
		
		
	}
	
	// Return to the application in the noNav dialog 
	function returnToApplication(){
		try{
			dw.focus();
		}
		catch(e){
			openNoNav()
		}
	}
	
	// Wipe all cs_security cookies
	// This functions are not being used because they cannot be called from onBeforeUnload event
	// They were written in hopes that we would be able to delete cookies before abandoning the page
	// but writing to the document is ignored onBeforeUnload.
	function ClientKillSession(){
		var aspids = getCookie("COWSASPIDS")
		alert(aspids);
		
		var tmparr = aspids.split(",");
		for (var i=0; i < tmparr.length; i++){
			ClearCookie("ASPSESSIONID" + tmparr[i]);
		}
		ClearCookie("CS_SEC_UserName");
		ClearCookie("CS_SEC_UserID"); 		
	}
	
	function ClearCookie(cookieName){
		var lastyear = new Date();
		lastyear.setFullYear(lastyear.getFullYear() -1);
		document.cookie = cookieName + "=;path=/;expires=" +  lastyear.toGMTString();
	}
</SCRIPT>
</HEAD>
<!-- #include file="variables.asp" -->
<!-- #include file="functions.asp" -->
<%
PAGE_URL = "CambridgeSoft ChemOffice Enterprise"
PAGE_COLOR = color_blue
PAGE_APP = "header_" & color_blue & "_coent.gif"
TOP_NAV = ""
%>

<!-- #include file="header.asp" -->

<br>
<%
Response.write renderBoxBegin ("User Login","")
CS_SEC_UserName = UCase(Request.Cookies("CS_SEC_UserName"))

If Len(CS_SEC_UserName) > 0 Then 
	Response.write "&nbsp;&nbsp;You are logged in as: <b>" & CS_SEC_UserName & "</b>"
End if

Response.Write "<BR><BR>"
Response.Write "<div align=""center""><input type=""button"" value=""Return to Application""  onclick=""returnToApplication()"">&nbsp;<input type=""button"" value=""Log off"" onclick=""Javascript:bypassUnloadEvent = true;window.location='/cs_security/login.asp?ClearCookies=true';"" >"	  
Response.write renderBoxEnd()

%>
<SCRIPT LANGUAGE=javascript>
var firstCheckMinutes = <%=Application("CookieExpiresMinutes")%>;
var nextCheckMinutes = firstCheckMinutes/10;

//The first time wait 10 minutes before checking
window.setTimeout("timeoutCheck();", firstCheckMinutes * 60 * 1000);

function timeoutCheck() {
	var now = new Date 
	var utc = Date.parse(now)
    //Read the cookie timeout timestamp
	var csTimeStamp = getCookie("cstimestamp") 
	
	//alert("cookie= " + csTimeStamp + " now= " + utc + "<BR>")
    if (utc > csTimeStamp) {
		window.focus()
		alert('Your session has timed out.  You will need to login to return to the application.')
        bypassUnloadEvent = true;
        window.location = "/cs_security/login.asp?clearcookies=1"
    }
    else {
	  // Continue checking every minute	
      window.setTimeout("timeoutCheck();", nextCheckMinutes * 60 * 1000);
    }
}


//Returns a string containing value of specified cookie,
//   or null if cookie does not exist.
function getCookie(name)
{
    var dc = document.cookie;
    var prefix = name + "=";
    var begin = dc.indexOf("; " + prefix);
    if (begin == -1)
    {
        begin = dc.indexOf(prefix);
        if (begin != 0) return null;
    }
    else
    {
        begin += 2;
    }
    var end = document.cookie.indexOf(";", begin);
    if (end == -1)
    {
        end = dc.length;
    }
    return unescape(dc.substring(begin + prefix.length, end));
}

</SCRIPT>
<!-- #include file="footer.asp" -->

