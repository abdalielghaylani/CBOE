cd_setCDPVersion();

function ResetValidators()
{
    if (typeof(Page_Validators) == "undefined")
        return true;
    var i,elm;
    for (i = 0; i < Page_Validators.length; i++) 
    {
        Page_Validators[i].isvalid = true;
        for(var j = 0, elm; elm = Page_Validators[i].parentElement.children.item(j++);)
		{
          if(elm != null && elm.id != '' && elm.nodeName.toUpperCase() == "SPAN" && elm.id.indexOf("errorMessage") != -1)
          {
             hideErrorDiv(elm.id);
             break;
          }
		}
        ValidatorUpdateDisplay(Page_Validators[i]);
    }
}

function coe_clearForm() {
    if (typeof(cd_objectArray)!='undefined' && typeof(cd_objectArray)!='unknown' && cd_objectArray) {
        for(i = 0; i < cd_objectArray.length; i++) {
            cd_clear(cd_objectArray[i]);
        }
    }
    
    clearForm(document.forms[0].id);
    ResetValidators();
    return false;
}

function clearForm(formId) 
{
    var form, elements, i, elm, tempHTML, strOuterHTML; 

  form = document.getElementById 
    ? document.getElementById(formId) 
    : document.forms[formId]; 

	if (document.getElementsByTagName)
	{
		elements = form.getElementsByTagName('input');
		for( i=0, elm; elm=elements.item(i++); )
		{
			if (elm.getAttribute('type') == "text" && elm.readOnly == false)
			    elm.value = '';
			if (elm.getAttribute('type') == "file" && elm.readOnly == false) 
            {
                if (elm.value != "") 
                {
			        tempHTML = elm.outerHTML.toString();
			        var valueAtrrib = tempHTML.search("value=");
			        var dot = tempHTML.indexOf(".", valueAtrrib + 7);
			        var space = tempHTML.indexOf(" ", dot + 1);
			        var filePath = tempHTML.substring(valueAtrrib, space + 1);
			        var strOuterHTML = tempHTML.replace(filePath, "");
			        elm.outerHTML = strOuterHTML;
			    }

			}
			if (elm.getAttribute('type') == "checkbox" && elm.disabled == false)
				elm.checked = false;
		}
		elements = form.getElementsByTagName('select');
		for( i=0, elm; elm=elements.item(i++); )
		{
			if (elm.disabled == false)
				elm.options.selectedIndex=0;
		}
		
		elements = form.getElementsByTagName('textarea');
		for( i=0, elm; elm=elements.item(i++); )
		{
			elm.value='';
		}
	}
	else
	{
		elements = form.elements;
		for( i=0, elm; elm=elements[i++]; )
		{
			if (elm.type == "text" || elm.type == "textarea" && elm.readOnly == false)
			{
				elm.value = '';
			}			
			else if (elm.type == "select" && elm.disabled == false)
			{
			    elm.options.selectedIndex = 0;
			}
		}
	}
}

function ClearOutputValue(objIndex) 
{
    if (!isNaN(objIndex))
    {
        if (typeof(document.getElementById(cd_objectArray[parseInt(objIndex)] + 'Output'))!='undefined' && typeof(document.getElementById(cd_objectArray[parseInt(objIndex)] + 'Output'))!='unknown' && document.getElementById(cd_objectArray[parseInt(objIndex)] + 'Output'))
            document.getElementById(cd_objectArray[parseInt(objIndex)] + 'Output').value = 'null';
    }
}

/////////////////////////////////////////////////////////////////////////////////////////////
// Use this function load the proper version of the Plugin/ActiveX Control.
function cd_setCDPVersion() {
	basePath = "";

	// auto-download Plugin/ActiveX
	// If you don't like the auto-download feature, remove the following 4 lines
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



function getCookie(name) {
    var start = document.cookie.indexOf( name + "=" );
    var len = start + name.length + 1;
    if ( ( !start ) && ( name != document.cookie.substring( 0, name.length ) ) ) {
        return null;
    }
    if ( start == -1 )
        return null;
    var end = document.cookie.indexOf( ';', len );
    if ( end == -1 )
        end = document.cookie.length;
    
    return unescape( document.cookie.substring( len, end ) );
}

