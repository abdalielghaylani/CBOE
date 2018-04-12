
<script language="Javascript">


//Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved





///////////////////////////////////////////////////////////////////////////////
// Opens up a dialog box
// Type 1 is standard dialog used for funcitons like Move Container, ChangeQty etc
// Type 2 is the larger dialog used for Create/Edit Container and Substance selector
// Type 3 is the location browser dialog used from the Browse link
// Type 4 is a full screen browser dialog used for running the whole up in noNav
// The size and positions of the popups has been optimized to look Ok even at 800 X 600 resolution
function OpenDialog(url, name, type)
{
	WindowDef_1 = "height=530, width= 530px, top=50, left=0";
	WindowDef_2 = "height=580, width= 760px, top=0, left=0";
	WindowDef_3 = "height=450, width= 300px, top=50, left=540";		
	WindowDef_4 = ""; // let the browser open with default size
	
	var WindowDef = eval("WindowDef_" + type);
	if (type != 4) {   
		var attribs = "toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars,resizable," + WindowDef;
	}
	else{
		var attribs = "toolbar=0,location=0,directories=0,status=1,menubar=0,scrollbars,resizable";
	}
	var DialogWindow = window.open(url,name,attribs);
	return DialogWindow;
}

function IsValidOracleObjectName(fName, fValue){
		var errmsg="";
		if ((fValue.length <3) || (fValue.length > 30)){
				errmsg += errmsg + "- " + fName + " must be between 3 and 30 characters.\r";
				bWriteError = true;
			}
			var illegalChars = /\W/;
			// allow only letters, numbers, and underscores
			if (illegalChars.test(fValue)) {
				errmsg += "- " + fName + " is restricted to letters, numbers, and underscores.\n";
			}
			var illegalChars = /[0-9]/
			if (illegalChars.test(fValue.substring(0,1))) {
				errmsg += "- " + fName + " cannot begin with a number.\n";
			}
			return errmsg;
	}
	

function IsValidPassword(fValue){
		var errmsg="";
		var fName = "Password"
		var re
		
		var pwdMinLength = <%=Application("pwdMinLength")%>;
		var pwdMaxLength = <%=Application("pwdMaxLength")%>;
		var pwdIllegalChars = "<%=Application("pwdIllegalChars")%>"
		var pwdIllegalFirstChar = "<%=Application("pwdIllegalFirstChar")%>";
		var pwdMustHaveChars = "<%=Application("pwdMustHaveChars")%>";
		var pwdIllegalWordList = "<%=Application("pwdIllegalWordList")%>";
		var pwdCannotMatchUserName = <%=Application("pwdCannotMatchUserName")%>;
		
		if ((pwdCannotMatchUserName) && fValue.toLowerCase() == document.user_manager.UserName.value.toLowerCase()){
			errmsg += errmsg + "- " + fName + " cannot be the same as user name.\r";
			bWriteError = true;
		} 
		
		//Password length
		if ((fValue.length < pwdMinLength) || (fValue.length > pwdMaxLength)){
			errmsg += errmsg + "- " + fName + " must be between " + pwdMinLength + " and "+ pwdMaxLength + " characters.\r";
			bWriteError = true;
		}
		
		//Illegal characters
		re = /<%=Application("pwdIllegalChars")%>/
		if ((re.test(fValue))&& (pwdIllegalChars !="NULL")) {
			errmsg += "- " + fName + " uses an illegal character.\n";
			bWriteError = true;
		}
		
		//Illegal first character
		re = /<%=Application("pwdIllegalFirstChar")%>/
		if ((re.test(fValue.substring(0,1)))&& (pwdIllegalFirstChar !="NULL")) {
			errmsg += "- " + fName + " cannot begin with illegal character " + pwdIllegalFirstChar +".\n";
			bWriteError = true;
		}
		
		//Mustp-have characters
		re = /<%=Application("pwdMustHaveChars")%>/
		if ((!re.test(fValue))&& (pwdMustHaveChars !="NULL")) {
			errmsg += "- " + fName + " must contain at least one of the following characters " + pwdMustHaveChars +".\n";
			bWriteError = true;
		}
		
		//IllegalWords
		tempArray = pwdIllegalWordList.split(",")
		for (var i=0;i<tempArray.length;i++){
			if (tempArray[i].toLowerCase() == fValue.toLowerCase()) {
				errmsg += "- " + tempArray[i] + " is not allowed as a password.\n";
				bWriteError = true;
			}
		}	
		return errmsg;
	}
</script>		