/////////////////////////////////////////////////////////////////////////////////////////////
//
// This is a Javascript library to write multi-browser pages comprising CS ChemDraw Plugin/ActiveX.
//
//
// You will use the following three functions in your web pages:
//   cd_insertObjectStr()
//   cd_insertObject()
//   cd_includeWrapperFile()
//  
// To support other browsers outside IE and Netscape, you should change the following function:
//   cd_figureOutUsing()
//
//
//
// Usually there is no need for you to change any other variables or functions.
//
//
// All Rights Reserved.
//
// ***PLEASE DON'T FORGET CHANGE THE VERSION NUMBER BELOW WHEN CHANGING THIS FILE***
// (version 1.047cows) Sep 21, 2005
// (version 11.0.1coe) Mar 04, 2010
/////////////////////////////////////////////////////////////////////////////////////////////



// ------------------------------------- GLOBAL DATA -------------------------------------------
// Global data. VERY IMPORTANT: never never change these.
var CD_CONTROL150CLSID	= "clsid:F974516A-2072-4af1-B063-489FAA6D4177";
//14.0: Using same GUIDs as 13.0
var CD_CONTROL140CONSTCLSID	= "clsid:CDF38697-B099-4355-92AE-54AF31800E49";
var CD_CONTROL140CLSID	= "clsid:8B0D4252-4E82-463a-84ED-E8A796C4808A";
var CD_CONTROL130CLSID = "clsid:8B0D4252-4E82-463a-84ED-E8A796C4808A";
var CD_CONTROL130CONSTCLSID = "clsid:CDF38697-B099-4355-92AE-54AF31800E49";
var CD_CONTROL120CLSID	= "clsid:4A6F3C59-D184-49FA-9189-AF42BEDFE5E4";
var CD_CONTROL110CLSID	= "clsid:45C31980-E065-49A1-A3D7-E69CD40DAF66";
var CD_CONTROL110CONSTCLSID	= "clsid:CDF38697-B099-4355-92AE-54AF31800E49";
var CD_CONTROL100CLSID	= "clsid:7EF697A4-D9F3-4303-9161-BBBEA1C30097";
var CD_CONTROL90CLSID	= "clsid:60257C74-D60B-41D6-9296-A08BD51F15B5";
var CD_CONTROL80CLSID	= "clsid:51A649C4-3E3D-4557-9BD8-B14C0AD44B0C";
var CD_CONTROL70CLSID	= "clsid:AF2D2DC1-75E4-4123-BC0B-A57BD5C5C5D2";
var CD_CONTROL60CLSID	= "clsid:FA549D21-6F54-11D2-B61B-00C04F736BDF";

//ONNEWVERSION: This version number in the name should be updated when updating version
var CD_CONTROL_CLSID	= CD_CONTROL110CONSTCLSID;


// These three files should be placed in the same folder as the three .js files.

var CD_PLUGIN_JAR	= "camsoft.jar";
var CD_PLUGIN_CAB	= "camsoft.cab";
var CD_PLUGIN_CAB2	= "camsoft2.cab";


// MOST IMPORTANT!!! To indicate which Plugin/ActiveX to use
// 1 - Control/ActiveX;  2 - old Plugin;  3 - new Plugin.

var cd_currentUsing = 0;
var js_canUseTry = false;

// Default threshold can be overridden by declaring it previously in page
if (!cd_plugin_threshold) var cd_plugin_threshold = 5.0;

// !DGB! 12/01
// Declare global array to hold the names of cd_objects in the page
var cd_objectArray = new Array();

// SYAN 2/23/2004
// Default cdax alert version defined if not defined.
if (!alert_cdax_version) var alert_cdax_version = '8.0.3';

// ------------------------------------- TODO AREA -------------------------------------------
// You may change this section when configuring for your website.


// These two variables define the URL for downloading the Plugin/ActiveX control. You may change
// it to your own download address if you choose.

if (!CD_AUTODOWNLOAD_PLUGIN) {
	var CD_AUTODOWNLOAD_PLUGIN  = "http://accounts.cambridgesoft.com/login.cfm?serviceid=11&fp=true";
}
var CD_AUTODOWNLOAD_ACTIVEX = CD_AUTODOWNLOAD_PLUGIN;


/////////////////////////////////////////////////////////////////////////////////////////////
// This function is very important; I is run before anything else, to figure out which
// Plugin/ActiveX control should be used.
// If you would like to configure this to recognize other types of browsers (by default, only
// MS Internet Explorer and Netscape are recognized) you may add to this function.

function cd_figureOutUsing() {
	// ChemDraw Plugin isn't availabe on IE, MAC
	if (cd_IsMacWithIE()) {
		cd_currentUsing = 0;
		return;
	}


	// Only 1, 2, 3 are used. Other codes make no sense.
	// 1 - Control/ActiveX;  2 - old Plugin;  3 - new Plugin.
	
	var version = cd_getBrowserVersion();
	
	// CURRENT SETTING:
	//    ActiveX Control (1) - IE 5.5 or higher versions
	//    old Plugin      (2) - IE 5.0 or lower versions, Netscape 4.x or lower versions
	//    new Plugin      (3) - Netscape 6.0 or higher versions
	if (cd_testBrowserType("Microsoft Internet Explorer")) {
		if (version < cd_plugin_threshold)
			cd_currentUsing = 2;
		else
			cd_currentUsing = 1;
		if (version >= 5.5)
			js_canUseTry = true;
	}
	else if (cd_testBrowserType("Netscape")) {
		if (version < 5.0)
			cd_currentUsing = 2;
		else if (version >= 5.0)
			cd_currentUsing = 3;
		if (version >= 5.0)
			js_canUseTry = true;
	}


	// TODO: add code to support other browsers beside IE and Netscape
	// else if (...)
	//		cd_currentUsing = 1 or 2 or 3;


	// Unknown browser type.
	else
		cd_currentUsing = 0;
}




// -------------------------------- FUNCTIONS USED IN WEB PAGES --------------------------------------
// The following three functions will be used in web pages


/////////////////////////////////////////////////////////////////////////////////////////////
// This function is used to insert a browser-specific Plugin/ActiveX Control object using
// a string to specify parameters.
// Parameter - tagStr - should be like following sample:
// cd_insertObjectStr("<EMBED src='HTML/blank.cdx' align='baseline' border= '0' width='267' height='128' type= 'chemical/x-cdx' name= 'myCDX'>");

//SYAN modified on 2/6/2006 to enable ISIS Draw Editing
function cd_insertObjectStr(tagStr) {
	var paraArray = {"type" : "", "width" : "", "height" : "", "name" : "", "src" : "", "viewonly" : "", "shrinktofit" : "", "dataurl" : "", "dontcache" : "", "isisdraw" : ""};
	cd_parsePara(tagStr, paraArray);
	cd_insertObject(paraArray["type"], paraArray["width"], paraArray["height"], paraArray["name"],
				 paraArray["src"], paraArray["viewonly"], paraArray["shrinktofit"], paraArray["dataurl"], paraArray["dontcache"], paraArray["isisdraw"]);
}
//End of SYAN modification

/////////////////////////////////////////////////////////////////////////////////////////////
// This function is used to insert a browser-specific Plugin/ActiveX Control object using
// specific parameters.
// The first 3 parameters [mimeType, objWidth, objHeight] are required, and the rest are optional.

//SYAN modified on 2/6/2006 to enable ISIS Draw Editing
function cd_insertObject(mimeType, objWidth, objHeight, objName, srcFile, viewOnly, shrinkToFit, dataURL, dontcache, ISISDraw) {
	if (cd_currentUsing >= 1 && cd_currentUsing <= 3)
		//!DGB! 12/01 Add a call to cd_AddToObjectArray
		cd_AddToObjectArray(objName);
		document.write( cd_getSpecificObjectTag(mimeType, objWidth, objHeight, objName, srcFile, viewOnly, shrinkToFit, dataURL, dontcache, ISISDraw) );
}
//End of SYAN modification


/////////////////////////////////////////////////////////////////////////////////////////////
// Use this function to insert a Plugin/ActiveX Control wrapper file.

function cd_includeWrapperFile(basePath, checkInstall) {
	if (basePath == null)
		basePath = "";

	if (checkInstall == null)
		checkInstall = true;

	if (basePath.length > 0) {
		var lastChar = basePath.charAt(basePath.length - 1);
		if (!(lastChar == "\\" || lastChar == "/"))
			basePath += "\\";
		
		// all these files should be place in the same folder as the three js files.
		CD_PLUGIN_JAR	= basePath + "camsoft.jar";
		CD_PLUGIN_CAB	= basePath + "camsoft.cab";
		CD_PLUGIN_CAB2	= basePath + "camsoft2.cab";
	}


	if (cd_currentUsing >= 1 && cd_currentUsing <=3) {
		var wrapperfile = "<script language=JavaScript src=\"";
	
		if (cd_currentUsing == 2 || cd_currentUsing == 3)
			// Plugin uses cdlib_ns.js wrapper file
			wrapperfile += basePath + "cdlib_ns.js";
		else if (cd_currentUsing ==  1)
			// ActiveX Control uses cdlib_ie.js wrapper file
			wrapperfile += basePath + "cdlib_ie.js";
			
		wrapperfile += "\"></script>";

		document.write(wrapperfile);
	}


	// auto-download Plugin/ActiveX
	// If you don't like the auto-download feature, remove the following 4 lines
	if (checkInstall) {
		if (cd_currentUsing == 2 || cd_currentUsing == 3) {
			if (cd_isCDPluginInstalled() == false)
				cd_installNetPlugin();
		}
		else if (cd_currentUsing == 1) {
			if (cd_isCDActiveXInstalled() == false)
				cd_installNetActiveX();
			else if (CD_CONTROL_CLSID == CD_CONTROL60CLSID) {
				if (confirm("You are using the 6.0 ActiveX Control.  We strongly recommend that you" +
					" upgrade to 9.0, or the page may not be correctly displayed.\nDo you want to install it now?"))
					window.open(CD_AUTODOWNLOAD_ACTIVEX);
			}
			// SYAN added 12/04/2003 to detect ActiveX 8.0.3 and alert
			else if (CD_CONTROL_CLSID == CD_CONTROL80CLSID) { // 8.0
				// Create a temporary object just so we can get minor version.
				var obj8 = new ActiveXObject("ChemDrawControl8.ChemDrawCtl")
				var version = obj8.version;
				setCDJSCookie('installedPlugin', '8', 1);
				//SYAN added 2/2/2004 to not detect 8.0.3 Net because we do not distribute upgrade for Net control.
				//so there is no point to alert users.
				//if ((version.indexOf("8.0.3") >= 0) && (version.indexOf("Net") < 0)) {
				//SYAN modified 2/18/2004 to take the version from ini settings.
				if ((version.indexOf(alert_cdax_version) >= 0) && (version.indexOf("Net") < 0)) {
					if (confirm("You are using " + alert_cdax_version + " ActiveX.  We strongly recommend you to upgrade to 8.0.6 and higher, " +
						"or the page may not be correctly displayed.\nDo you want to install it now?")) {
						window.open(CD_AUTODOWNLOAD_ACTIVEX);
					}
				}
			}
		}
		
	}
}




// ------------------------------------- INTERNAL FUNCTIONS DEFINATION -------------------------------------------
// You may never change following codes.


/////////////////////////////////////////////////////////////////////////////////////////////
// At first, run figureOutUsing() to initilize *currentUsing*.

cd_figureOutUsing();


/////////////////////////////////////////////////////////////////////////////////////////////
// !DGB! 12/01 This function appends an element to the cd_objectsArray. 
// The array contains the names of all cd objects in the page
	
function cd_AddToObjectArray(objName) {
	cd_objectArray[cd_objectArray.length] = objName;
}


/////////////////////////////////////////////////////////////////////////////////////////////
// According to browser type and version, choose its corresponding ChemDraw Plugin/ActiveX tag.
// The first 3 parameters [mimeType, objWidth, objHeight] is required, and the last 5 is optional.

var cd_pluginID = 1000;
//SYAN modified on 2/1/2006 to implement ISIS draw editing
function cd_getSpecificObjectTag(mimeType, objWidth, objHeight, objName, srcFile, viewOnly, shrinkToFit, dataURL, dontcache, ISISDraw) {
	mimeType = "chemical/x-cdx";
	var buf = "";
	
	if (dataURL != null) {
		//!DGB! 12/01 add a conditional call to unescape(dataURL)
		//LJB 1/13/2004 add support for other escaped mimetypes
		if ((dataURL.indexOf("%3Bbase64%2C") > 0)||(dataURL.indexOf("%3Achemical")>0)||(dataURL.indexOf("%3CCDXML")>0))
		
			dataURL = unescape(dataURL);
			
	}

	//SYAN added on 2/1/2006 to implement ISIS draw editing
	if (ISISDraw == "True" && viewOnly == "False") {
		//objName = "StructureCtl"
		buf = "<SCRIPT LANGUAGE=\"JavaScript\" FOR=\"" + objName + "\" EVENT=\"DblClick\">";
		buf += "\nvar EditStructure = new ActiveXObject(\"CSISISDraw9.ISISDrawEdit\");";
		buf += "\nvar obj = document.all('" + objName + "');";
		buf += "\nvar cdxml1 = obj.Objects.data(\"cdxml\");";
		buf += "\nvar cdxml = EditStructure.ISISEdit(cdxml1);";
		buf += "\nobj.Objects.Clear();";
		buf += "\nobj.SourceURL = \"data:text/xml,\"" + "+ cdxml;";
		buf += "\n" + "</" + "SCRIPT>";
		viewOnly = "True";
	}
	//End of SYAN modificaiton

	if (cd_currentUsing == 1) {	// ActiveX Control
	
	    if(objHeight.indexOf("px") < 0)
            objHeight += "px";
        if(objWidth.indexOf("px") < 0)
            objWidth += "px";
		
		buf +=	"<OBJECT classid=\"" + CD_CONTROL_CLSID + "\" " +
				"style=\"HEIGHT: " + objHeight + "; WIDTH: " + objWidth + "\"";
				
		if (objName != null && objName != "")
			buf += " name=\"" + objName + "\"";
			
		buf += ">\n";

		if (srcFile != null && srcFile != "")			
			buf += "<param NAME=\"SourceURL\" VALUE=\"" + srcFile + "\">\n";

		if (dataURL != null && dataURL != "")
			buf += "<param NAME=\"DataURL\" VALUE=\"" + dataURL + "\">\n";
		
		if (viewOnly != null && viewOnly != "")
			buf += "<param NAME=\"ViewOnly\" VALUE=\"" + viewOnly + "\">\n";

		if (shrinkToFit != null && shrinkToFit != "")
			buf += "<param NAME=\"ShrinkToFit\" VALUE=\"" + shrinkToFit + "\">\n";
		
		if (dontcache != null && dontcache != "")
			buf += "<param NAME=\"DontCache\" VALUE=\"" + dontcache + "\">\n";

		buf += "<param NAME=\"ShowToolsWhenVisible\" VALUE=\"1\">\n";

		buf += "</OBJECT>\n";
	}
	else if (cd_currentUsing == 2 || cd_currentUsing == 3) { // Plugin

		var pluginID = ++cd_pluginID;

		if (objName == null)
			objName == "";

		if (srcFile == null)
			srcFile == "";
					
		buf +=	"<EMBED ";
		if(srcFile.length > 0)
		    buf += "src=\"" + srcFile + "\" ";
		buf +=  "width=\"" + objWidth + "\"" +
				" height=\"" + objHeight + "\"" +
				" type=\"" + mimeType + "\"";

		if (cd_currentUsing == 3) {
			// In netscape 6, we get data directly from the plugin, not the applet
			if (objName != null && objName != "")
				buf += " name=\"" + objName + "\"";
		}

		if (cd_currentUsing == 2) 
			buf += " id=\"" + pluginID + "\"";
			
		if (dataURL != null && dataURL != "")
			buf += " dataurl=\"" + dataURL + "\"";
		
		if (viewOnly != null && viewOnly != "")
			buf += " viewonly=\"" + viewOnly + "\"";

		if (shrinkToFit != null && shrinkToFit != "")
			buf += " shrinktofit=\"" + shrinkToFit + "\"";
			
		if (dontcache != null && dontcache != "")
			buf += " dontcache=\"" + dontcache + "\"";

		buf += " showtoolswhenvisible=\"1\"";

		buf += ">\n";

		if (cd_currentUsing == 2) {
			// old Plugin needs CDPHelper
			
			buf +=	"<APPLET ID=\"" + objName +  "\" NAME=\"" + objName + "\" CODE=\"camsoft.cdp.CDPHelperAppSimple\" WIDTH=0 HEIGHT=0 ARCHIVE=\"" + CD_PLUGIN_JAR + "\">" +
				"<PARAM NAME=ID VALUE=\"" + pluginID + "\">" +
				"<PARAM NAME=cabbase value=\"" + CD_PLUGIN_CAB + "\"></APPLET>\n";
		}
	}
	else
	{
		buf = "<P><font color=red>\"ALERT: The ChemDraw Plugin is not available for Internet Explorer on the Macintosh!\"</font></P>";
	}
	
	//alert(buf);
	return buf;	
}


/////////////////////////////////////////////////////////////////////////////////////////////
// This function to return the reference of ChemDraw Plugin/ActiveX by its name.

function cd_getSpecificObject(nm) {
	var r = null;

	if (cd_currentUsing == 1) // ActiveX Control
		r = document.all(nm);
	else if (cd_currentUsing == 2) // old Plugin + CDPHelper
		r = document.applets[nm];
	else if (cd_currentUsing == 3) // new Plugin (XPCOM, scriptable old Plugin)
		r = document.embeds[nm];

	if (r == null)
		alert("ERROR: You have the wrong name [" + nm + "] to refer to the Plugin/ActiveX !!!");

	return r;
}


/////////////////////////////////////////////////////////////////////////////////////////////
// To get Browser's version.

function cd_getBrowserVersion() {
	if (cd_testBrowserType("Microsoft Internet Explorer")) {
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
	else
		return parseFloat(navigator.appVersion);
}


/////////////////////////////////////////////////////////////////////////////////////////////
// To test Browser's type.

function cd_testBrowserType(brwType) {
	if (brwType === "Microsoft Internet Explorer") {
		var i = navigator.appName.indexOf(brwType);
		if (i != -1) {
			return true;
		}
		//IE 10 & 11+
		var isAtLeastIE10 = !!navigator.userAgent.match(/Trident/);
		
		if (isAtLeastIE10) {
			return true;
		}

		return false;
	}
	return (navigator.appName.indexOf(brwType) != -1);
}


/////////////////////////////////////////////////////////////////////////////////////////////
// To test if IE runs on MAC.

function cd_IsMacWithIE() {
	return cd_testBrowserType("Microsoft Internet Explorer") && (navigator.platform.indexOf("Mac") != -1 || navigator.platform.indexOf("MAC") != -1);
}


/////////////////////////////////////////////////////////////////////////////////////////////
// To test whether Plugin is installed on locall machine.

function cd_isCDPluginInstalled() {
	if (cd_testBrowserType("Microsoft Internet Explorer")) {
		var str =
		"<div style='left:0;top:0;zIndex:1;position:absolute'><applet code='camsoft.cdp.CDPHelperAppSimple2' width=0 height=0 name='test_plugin'><param name=ID value=99999><param NAME=cabbase value='" + CD_PLUGIN_CAB2 + "'></applet></div>" +
		"<SCRIPT LANGUAGE=javascript>" +
		"	var testpluginonlyonce = false;" +
		"	function document_onmouseover() {" +
		"		if (!testpluginonlyonce) {" +
		"			testpluginonlyonce = true;" +
		"			var pluginstalled = false;" +
		"			pluginstalled = document.applets[\"test_plugin\"].isLoaded();" +
		"			if (!pluginstalled) {" +
		"				CD_PLUGIN_JAR = \"\";" +
		"				CD_PLUGIN_CAB = \"\";" +
		"				cd_installNetPlugin();" +
		"			}" +
		"		}" +
		"	}" +
		"</" + "SCRIPT>" +
		"<SCRIPT LANGUAGE=javascript FOR=document EVENT=onmouseover>document_onmouseover()</" + "SCRIPT>";
		
		document.write(str);
		
		return true;
	}
	
	for (var i = 0; i < navigator.plugins.length; ++i) {
		if (navigator.plugins[i].name.indexOf("ChemDraw") != -1)
			return true;
	}
	
	return false;
}


/////////////////////////////////////////////////////////////////////////////////////////////
// To  install NET plugin on local machine.

function cd_installNetPlugin() {
	var doNotInstallCDPlugin;
	doNotInstallCDPlugin = getCookie('doNotInstallCDPlugin');
	
	if (doNotInstallCDPlugin != 'true') {
		if (confirm("You currently use " + navigator.appName + " " + cd_getBrowserVersion() + ".\n" +
			"This page will use CS ChemDraw Plugin, but it isn't installed on your computer.\n" +
			"Do you want to install it now?")) {
			
			window.open(CD_AUTODOWNLOAD_PLUGIN);
		}
		else {
			setCDJSCookie('doNotInstallCDPlugin', 'true', 1);
			CD_PLUGIN_JAR = "";
			CD_PLUGIN_CAB = "";
		}
	}
}


/////////////////////////////////////////////////////////////////////////////////////////////
// To test whether ActiveX is installed on local machine.

function cd_isCDActiveXInstalled() {
	// Note: try ... catch ... statement isn't available in JavaScript 1.4 (IE 4 uses js 1.4).
	// That means that try/catch code can't even exist if we're using an earlier version of JavaScript
	//  so we have to wrap it as a string.  If we're using a sufficiently-recent browser (that has
	//  a version of JavaScript 1.4 or later), we'll eval the string, which will end up doing a try/catch
	//  For older browsers, we'll just do things the old way, and suffer through any performance penalties.
	
	var retval = true;

	if (js_canUseTry) {
	    var str = "";
		str = str + "try\n";
		str = str + "{\n";
		str = str + " // Try 14.0\n";
		str = str + " var obj14 = new ActiveXObject(\"ChemDrawControl14.ChemDrawCtl\");\n";
		str = str + " CD_CONTROL_CLSID = CD_CONTROL140CLSID; obj14='';\n";
		str = str + "} catch(e14)\n";
		str = str + "{\n";
		str = str + " try\n";
		str = str + " {\n";
		str = str + "  // Try 14.0Constant\n";
		str = str + "  var obj14c = new ActiveXObject(\"ChemDrawControlConst11.ChemDrawCtl.14.0\");\n";
		str = str + "  CD_CONTROL_CLSID = CD_CONTROL140CONSTCLSID; obj14c='';\n";
		str = str + " } catch(e14c)\n";
		str = str + " {\n";
	    str = str + "	try\n";
	    str = str + "	{\n";
	    str = str + "		// Try 13.0Constant\n";
	    str = str + "		var obj13c = new ActiveXObject(\"ChemDrawControlConst11.ChemDrawCtl.13.0\");\n";
	    str = str + "		CD_CONTROL_CLSID = CD_CONTROL130CONSTCLSID;\n";
	    str = str + "	} catch(e13c)\n";
	    str = str + "   {\n";
		str = str + "    try\n";
		str = str + "    {\n";
		str = str + "     // Try 11.0Constant\n";
		str = str + "	  var obj11c = new ActiveXObject(\"ChemDrawControlConst11.ChemDrawCtl\");\n";
		str = str + "	  CD_CONTROL_CLSID = CD_CONTROL110CONSTCLSID;\n";
		str = str + "    } catch(e11c)\n";
		str = str + "    {\n";
		str = str + "	  try\n";
		str = str + "	  {\n";
		str = str + "	   // Try 13.0\n";
		str = str + "	   var obj13 = new ActiveXObject(\"ChemDrawControl13.ChemDrawCtl\");\n";
		str = str + "	   CD_CONTROL_CLSID = CD_CONTROL130CLSID;\n";
		str = str + "     } catch(e13)\n";
		str = str + "	  {\n";
		str = str + "	   try\n";
		str = str + "	   {\n";
		str = str + "	    // Try 12.0\n";
		str = str + "		var obj12 = new ActiveXObject(\"ChemDrawControl12.ChemDrawCtl\");\n";
		str = str + "		CD_CONTROL_CLSID = CD_CONTROL120CLSID;\n";
		str = str + "	   } catch(e12)\n";	
		str = str + "	   {\n";
		str = str + "	    try\n";
		str = str + "	    {\n";
		str = str + "		 // Try 11.0\n";
		str = str + "		 var obj11 = new ActiveXObject(\"ChemDrawControl11.ChemDrawCtl\");\n";
		str = str + "		 CD_CONTROL_CLSID = CD_CONTROL110CLSID;\n";
		str = str + "	    } catch(e11)\n";
		str = str + "   	{\n";
		str = str + "   		try\n";
		str = str + "   		{\n";
		str = str + "   			// Try 10.0\n";
		str = str + "   			var obj10 = new ActiveXObject(\"ChemDrawControl10.ChemDrawCtl\");\n";
		str = str + "   			CD_CONTROL_CLSID = CD_CONTROL100CLSID;\n";
		str = str + "   		} catch(e10)\n";
		str = str + "   		{\n";
		str = str + "   			try\n";
		str = str + "   			{\n";
		str = str + "   				// Try 9.0\n";
		str = str + "   				var obj9 = new ActiveXObject(\"ChemDrawControl9.ChemDrawCtl\");\n";
		str = str + "   				CD_CONTROL_CLSID = CD_CONTROL90CLSID;\n";
		str = str + "   			} catch(e9)\n";
		str = str + "   			{\n";
		str = str + "   				try\n";
		str = str + "   				{\n";
		str = str + "   					// Try 8.0\n";
		str = str + "   					var obj8 = new ActiveXObject(\"ChemDrawControl8.ChemDrawCtl\");\n";
		str = str + "   					CD_CONTROL_CLSID = CD_CONTROL80CLSID;\n";
		str = str + "   				} catch(e8)\n";
		str = str + "   				{\n";
		str = str + "   					try\n";
		str = str + "   					{\n";
		str = str + "   						// try 7.0\n";
		str = str + "   						// Something is wrong in 7.0 installers, which causes \"ChemDrawControl7.ChemDrawCtl\" cannot be used.\n";
		str = str + "   						var obj7 = new ActiveXObject(\"ChemDrawControl7.ChemDrawCtl.7.0\");\n";
		str = str + "   						CD_CONTROL_CLSID = CD_CONTROL70CLSID\n";
		str = str + "   					} catch(e7)\n";
		str = str + "   					{\n";
		str = str + "   						try\n";
		str = str + "   						{\n";
		str = str + "   							// try 6.0\n";
		str = str + "   							var obj6 = new ActiveXObject(\"ChemDrawLib.ChemDrawCtl6.0\");\n";
		str = str + "   							CD_CONTROL_CLSID = CD_CONTROL60CLSID\n";
		str = str + "   						} catch(e6)\n";
		str = str + "   						{\n";
		str = str + "   							// No version installed\n";
		str = str + "   							retval = false;\n";
		str = str + "   						}\n";
		str = str + "   					}\n";
		str = str + "   				}";
		str = str + "   			}";
		str = str + "   		}";
		str = str + "   	}";
		str = str + "	  }";
		str = str + "	 }";
		str = str + "   }";
		str = str + "  }";
		str = str + " }";
		str = str + "}";

		eval(str);
	}
	else {
	    document.write("<OBJECT NAME=\"test_140\" WIDTH=0 HEIGHT=0 CLASSID=\"" + CD_CONTROL140CLSID + "\"><param NAME=ViewOnly VALUE=true></OBJECT>");
	    if (document.all("test_140").Selection != null)
	        CD_CONTROL_CLSID = CD_CONTROL140CLSID;
	    else {
	        document.write("<OBJECT NAME=\"test_140c\" WIDTH=0 HEIGHT=0 CLASSID=\"" + CD_CONTROL140CONSTCLSID + "\"><param NAME=ViewOnly VALUE=true></OBJECT>");
	        if (document.all("test_140c").Selection != null)
				CD_CONTROL_CLSID = CD_CONTROL140CONSTCLSID;
			else {                	        
				document.write("<OBJECT NAME=\"test_130\" WIDTH=0 HEIGHT=0 CLASSID=\"" + CD_CONTROL130CLSID + "\"><param NAME=ViewOnly VALUE=true></OBJECT>");
				if (document.all("test_130").Selection != null)
					CD_CONTROL_CLSID = CD_CONTROL130CLSID;
				else {
					document.write("<OBJECT NAME=\"test_130c\" WIDTH=0 HEIGHT=0 CLASSID=\"" + CD_CONTROL130CONSTCLSID + "\"><param NAME=ViewOnly VALUE=true></OBJECT>");
				   if (document.all("test_130c").Selection != null)
					   CD_CONTROL_CLSID = CD_CONTROL130CONSTCLSID;
				   else {                	        
						if (document.all("test_120").Selection != null)
							CD_CONTROL_CLSID = CD_CONTROL120CLSID;
						else {
							document.write("<OBJECT NAME=\"test_110\" WIDTH=0 HEIGHT=0 CLASSID=\"" + CD_CONTROL110CLSID + "\"><param NAME=ViewOnly VALUE=true></OBJECT>");
							if (document.all("test_110").Selection != null)
								CD_CONTROL_CLSID = CD_CONTROL110CLSID;
							else {
								document.write("<OBJECT NAME=\"test_110c\" WIDTH=0 HEIGHT=0 CLASSID=\"" + CD_CONTROL110CONSTCLSID + "\"><param NAME=ViewOnly VALUE=true></OBJECT>");
								if (document.all("test_110c").Selection != null)
									CD_CONTROL_CLSID = CD_CONTROL110CONSTCLSID;
								else {
									document.write("<OBJECT NAME=\"test_100\" WIDTH=0 HEIGHT=0 CLASSID=\"" + CD_CONTROL100CLSID + "\"><param NAME=ViewOnly VALUE=true></OBJECT>");
									if (document.all("test_100").Selection != null)
										CD_CONTROL_CLSID = CD_CONTROL100CLSID;
									else {
										document.write("<OBJECT NAME=\"test_90\" WIDTH=0 HEIGHT=0 CLASSID=\"" + CD_CONTROL90CLSID + "\"><param NAME=ViewOnly VALUE=true></OBJECT>");
										if (document.all("test_90").Selection != null)
											CD_CONTROL_CLSID = CD_CONTROL90CLSID;
										else {
											document.write("<OBJECT NAME=\"test_80\" WIDTH=0 HEIGHT=0 CLASSID=\"" + CD_CONTROL80CLSID + "\"><param NAME=ViewOnly VALUE=true></OBJECT>");
											if (document.all("test_80").Selection != null)
												CD_CONTROL_CLSID = CD_CONTROL80CLSID;
											else {
												document.write("<OBJECT NAME=\"test_70\" WIDTH=0 HEIGHT=0 CLASSID=\"" + CD_CONTROL70CLSID + "\"><param NAME=ViewOnly VALUE=true></OBJECT>");
												if (document.all("test_70").Selection != null)
													CD_CONTROL_CLSID = CD_CONTROL70CLSID;
												else {
													document.write("<OBJECT NAME=\"test_60\" WIDTH=0 HEIGHT=0 CLASSID=\"" + CD_CONTROL60CLSID + "\"><param NAME=ViewOnly VALUE=true></OBJECT>");
													if (document.all("test_60").Selection != null)
														CD_CONTROL_CLSID = CD_CONTROL60CLSID;
													else
														retval = false;
												} 	                                
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
	return retval;
}


/////////////////////////////////////////////////////////////////////////////////////////////
// To  install NET plugin on locall machine.

function cd_installNetActiveX() {
	var doNotInstallCDAX;
	doNotInstallCDAX = getCookie('doNotInstallCDAX');
	
	if (doNotInstallCDAX != 'true') {
		if (confirm("You currently use " + navigator.appName + " " + cd_getBrowserVersion() + ".\n" +
			"This page will use CS ChemDraw ActiveX control, but it isn't installed on your computer.\n" +
			"Do you want to install it now?")) {
			window.open(CD_AUTODOWNLOAD_ACTIVEX);
		}
		else {
			setCDJSCookie('doNotInstallCDAX', 'true', 1);
			CD_PLUGIN_JAR = "";
			CD_PLUGIN_CAB = "";
		}
	}
}


/////////////////////////////////////////////////////////////////////////////////////////////
// This function to parse all useful parameter from <EMBED> string. Return values is
// stored an array.
// <embed width="200" HEIGHT="200" type="chemical/x-cdx" src="mols/blank.cdx" dataurl="mols/toluene.mol" viewonly="TRUE">

function cd_parsePara(str, paraArray) {

	for (var p in paraArray)
		paraArray[p] = cd_getTagValue(p, str);
}


/////////////////////////////////////////////////////////////////////////////////////////////
// This function return the tag value from <EMBED> string.

function cd_getTagValue(tag, str) {
	var r = "";
	var pos = str.toLowerCase().indexOf(tag, 0);
	var taglen = tag.length;
	
	// make sure tag is a whole word
	while (pos >= 0 && !(pos == 0 && (str.charAt(taglen) == " " || str.charAt(taglen) == "=") ||
		pos > 0 && str.charAt(pos - 1) == " " && (str.charAt(pos + taglen) == " " || str.charAt(pos + taglen) == "=")) ) {
		pos += taglen;
		pos = str.toLowerCase().indexOf(tag, pos);
	}

	if (pos >= 0) {		
		// skip the space chars following tag
		pos += taglen;
		while (str.charAt(pos) == " ")
			pos++;
		
		// following char must be '='
		if (str.charAt(pos) == "=") {
			pos++;
			
			// skip the space chars following '='
			while (str.charAt(pos) == " ")
				pos++;
			
			var p2 = pos;
			if (str.charAt(pos) == "\"") {
				pos++;
				p2 = str.indexOf("\"", pos);
			}
			else if (str.charAt(pos) == "\'") {
				pos++;
				p2 = str.indexOf("\'", pos);
			}
			else {
				p2 = str.indexOf(" ", pos);
			}
			
			if (p2 == -1)
				p2 = str.length;
			else if (pos > p2)
				p2 = str.length - 1;

			r = str.substring(pos, p2);
		}
	}
	
	return r;
}

function setCDJSCookie(name,value,make_expire){
	
	if (make_expire == 1){
		document.cookie = name + "="  + escape(value); 
	}else
	{
		var nextyear = new Date()
		nextyear.setFullYear(nextyear.getFullYear() +1);
		document.cookie = name  + "="  + escape(value) + "; expires=" + nextyear.toGMTString() + "; path=/" + appkey;
	}

	
}
