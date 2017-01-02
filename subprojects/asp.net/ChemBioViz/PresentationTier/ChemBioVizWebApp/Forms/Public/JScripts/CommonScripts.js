// JScript File
//Method to check if the opened window is a clean one. We also detect popUp blockers (We really need popUp blockers disabled)
function CheckToOpenNewWindow( versionSupported )
{
      var regWindowName = 'ChemBioViz';
      var objectId = gup('SubmittedObjectId');
      var closewithdone = gup('closewithdone');
      var windowname = gup('windowname');
      if(windowname == '') windowname = regWindowName;
      //Read browser version (now supports IE6 & up )
      var agt = navigator.userAgent.toLowerCase();
      var is_ie6 = (agt.indexOf("msie 6.") != -1);
      var is_ie7 = (agt.indexOf("msie 7.") != -1);
      var attribs = "toolbar=0,location=0,directories=0,status=1,menubar=0,scrollbars,resizable,height=700,width=1000px,top=0,left=100";
      var isSupported_ie6 = (versionSupported.indexOf("6") != -1);
      var isSupported_ie7 = (versionSupported.indexOf("7") != -1);
      var noneSupported = (versionSupported.indexOf("-1") != -1);
      var browserSupported = false;
      if (is_ie6) browserSupported = isSupported_ie6;
      if (is_ie7) browserSupported = isSupported_ie7;
      
      if (browserSupported && !noneSupported) //If the current browser version is supported by our App, we try to close and re open a new clean window.
      { 
          if(window.name != windowname) //Different window context
          {
            var newWin = window.open(window.location.href,windowname,attribs);
            if(newWin == null || typeof(newWin) == "undefined") //Check if window was really open. If not, display a PopUp Blocker message.
            {
                window.alert('Pop-up blocker detected!\n\nTo use CambridgeSoft Registration Enterprise, your web browser must allow pop-up windows.\n For information about allowing pop-up windows, see the instructions for your pop-up blocking software.');
                history.back();
            }
	        else
            {
               if (is_ie6)
		        {
			        var oMe = window.self;
			        oMe.opener = window.self;
			        oMe.close();
		        }
		        else
		        {
			        window.open('','_self');
			        window.close();
		        }
            }
         }  
     }         
}

//Save preious onload associated methods. Change for YUI util call
function WindowOnload(f) {
    var prev=window.onload;
    window.onload=function(){ if(prev)prev(); f(); }
  }
//Method to read get vars from the url.
function gup( name )
{  
    name = name.replace(/[\[]/,"\\\[").replace(/[\]]/,"\\\]");  
    var regexS = "[\\?&]"+name+"=([^&#]*)";  
    var regex = new RegExp( regexS );  
    var results = regex.exec( window.location.href );  
    if( results == null )    
        return "";  
    else    
        return results[1];
}
//Method to change the image source.
function ChangeImage(imageId,imageURL)
{
    document.getElementById(imageId).src = imageURL;
}
function ConfirmCancel()
{
    return window.confirm('Are you sure you want to cancel? You will lose all the changes.');
}
function ConfirmRelationShipDeletion(text)
{
    return window.confirm(text);
}
function HideChemDraws() {
    if (typeof(cd_objectArray)!='undefined' && typeof(cd_objectArray)!='unknown' && cd_objectArray) {
        for(i = 0; i < cd_objectArray.length; i++) {
            cd_getSpecificObject(cd_objectArray[i]).style.visibility = 'hidden';
        }
    }
}

function HideChemDrawsCheckingPage() {
    if(IsPageValid()){
        if (typeof(cd_objectArray)!='undefined' && typeof(cd_objectArray)!='unknown' && cd_objectArray) {
            for(i = 0; i < cd_objectArray.length; i++) {
                cd_getSpecificObject(cd_objectArray[i]).style.visibility = 'hidden';
            }
        }
    }
}

function ShowChemDraws() {
    if (typeof(cd_objectArray)!='undefined' && typeof(cd_objectArray)!='unknown' && cd_objectArray) {
        for(i = 0; i < cd_objectArray.length; i++) {
            cd_getSpecificObject(cd_objectArray[i]).style.visibility = 'visible';
        }
    }
}

function UpdateImage(e)
{
    if(document.images && e.srcElement)
    {
        if( (typeof(e.srcElement.parentElement.attributes.searchingImgID) != 'undefined' && typeof(e.srcElement.parentElement.attributes.searchingImgSrc) != 'undefined') || 
            (typeof(e.srcElement.attributes.searchingImgID) != 'undefined' && typeof(e.srcElement.attributes.searchingImgSrc) != 'undefined') )
        {
	        var imgID = typeof(e.srcElement.parentElement.attributes.searchingImgID) == 'undefined' ? e.srcElement.attributes.searchingImgID.value : e.srcElement.parentElement.attributes.searchingImgID.value;
            var imgSrc = typeof(e.srcElement.parentElement.attributes.searchingImgSrc) == 'undefined' ? e.srcElement.attributes.searchingImgSrc.value : e.srcElement.parentElement.attributes.searchingImgSrc.value;
	        setTimeout("document.images['"+imgID+"'].src='"+imgSrc+"';",50);
        }
    }  
}
function HideChemBioFinderMenu(e)
{
    if(document.getElementById('menu-h')) 
            document.getElementById('menu-h').style.visibility = 'hidden';
}

function IsValidRegistryID(registryID)
{
    var retVal = false;
    if(registryID.length == 0)
        retVal = true;
    else
    {
        var regExpCASRN = new RegExp("(^%?[0-9]{1,11}%?$|^%?[0-9]{1,6}[-][0-9]{1,2}%$|^%?[0-9]{1,6}-[0-9]{1,2}-*[0-9*]$)");
        var regExpCASRN1 = new RegExp("(^[-][0-9]+$|^[-][0-9]*[-][0-9]+$|^[-][0-9]*[-]+$|^[0-9]{1,11}$)");
        var regExpACX = new RegExp("(^[Xx][0-9]{3,7}$|^[Xx][0-9]{3,7}[%*]?$|^[Xx][0-9]{3,7}-[0-9]{1}$)","i");  
        if(regExpCASRN.test(registryID) && !regExpCASRN1.test(registryID))
            retVal = regExpCASRN.test(registryID);
        else if (regExpACX.test(registryID))
            retVal = regExpACX.test(registryID);
    }
    return retVal;
}

function CheckMWType(molweight)
{
    var retVal = false;
    var dOperPattern = new RegExp("^[<>]{1}[=][0-9]+$|^[<>]{1}[=][0-9]{1,5}[.][0-9]+$|^[<>]{1}[=][.][0-9]+$");
	var sOperPattern = new RegExp("^[<>]{1}[0-9]+$|^[<>]{1}[0-9]{1,5}[.][0-9]+$|^[<>]{1}[.][0-9]+$");
    var singlePattern = new RegExp("^[0-9]+$|^[.][0-9]+$|^[0-9]*[.][0-9]+$");
	var rangePattern = new RegExp("^[0-9]{1,5}[-][0-9]+$|^[0-9]{1,5}[-][0-9]{1,5}[.][0-9]+$|^[0-9]{1,5}[.][0-9]*[-][0-9]+$|^[0-9]{1,5}[.][0-9]*[-][0-9]{1,5}[.][0-9]+$|^[.][0-9]*[-][0-9]*[.][0-9]+$|^[0-9]{1,5}[.][0-9]*[-][.][0-9]+$|^[.][0-9]*[-][0-9]+$|^[0-9]{1,5}[-][.][0-9]+$");
    if(dOperPattern.exec(molweight) != null || sOperPattern.exec(molweight) != null || singlePattern.exec(molweight) != null || rangePattern.exec(molweight) != null)
        retVal = true;
    return retVal;
}

function CheckMWValue(molweight)
{
    var retVal = false;
    if(molweight.indexOf(">") > -1)
    {
        var minWt;
        if(molweight.indexOf(">=") > -1)
            minWt =  molweight.substr(molweight.indexOf(">=") + 2);
        else
            minWt =  molweight.substr(molweight.indexOf(">") + 1);
        retVal = CheckMWRange(minWt);
    }
    else if (molweight.indexOf("<") > -1)
    {
        var wt;
        if(molweight.indexOf("<=") > -1)
            wt =  molweight.substr(molweight.indexOf("<=") + 2);
        else
            wt =  molweight.substr(molweight.indexOf("<") + 1);
        retVal = CheckMWRange(wt);
    }
    else if (molweight.indexOf("-") > -1)
    {
        var minWt = molweight.substr(0, molweight.indexOf("-"));
        var maxWt = molweight.substr(molweight.indexOf("-") + 1);
        if(CheckMWRange(minWt) && CheckMWRange(maxWt) && CheckBothValueRange(minWt,maxWt))
            retVal = true;
    }
    else if (molweight.indexOf(".") > -1)
    {
        retVal = CheckMWRange(molweight);
    }
    else
        retVal = true;
    return retVal;
}

function CheckMWRange(wt)
{
    var retVal = true;
    //Something to do
    return retVal;
}
function CheckBothValueRange(minWt,maxWt)
{
    var retVal = true;
    //Validate max is greated than min
    return retVal;
}

function IsValidMolWeight(molweight)
{
    var retVal = false;
    if(molweight.length == 0)
        retVal = true;
    else
    {
        molweight = molweight.replace(/\s/g, "");
        var firstValue = molweight.substr(0,1);
        //Check to remove = character
        if(firstValue == "=")
            molweight = molweight.substr(1);
        var checkValueExp = new RegExp("^[<>]?=?[0-9.-]+$");
        if(checkValueExp.exec(molweight) != null)
        {
            if(CheckMWType(molweight))
                retVal = CheckMWValue(molweight);
        }
    }
    return retVal;
}

function IsValidPositiveInteger(value)
{
    value = value.trim();
    
    var retVal = false;
    var regExpPositiveInteger = new RegExp("(^%?>?<?=? ?[0-9]{1,9}%?$|^[0-9]{1,9}[-][0-9]{1,9}$)");
    
    if(value.length == 0)
        retVal = true;
    else
    {
        retVal = (regExpPositiveInteger.exec(value) != null)
    }

    return retVal;
}

function IsValidNameSynonym(name)
{
    var retVal = true;
    var input = name;
    return retVal;
}

function IsAValidRegNumber(reg_num, prefix, prefix_delimiter, rootnumber_length, type)
{
    var retVal = false;
    reg_num = reg_num.trim();
    if(reg_num.length == 0)
        retVal = true;
    else
    {
        //var checkValueExp = new RegExp("^[<>']+$");
        //if(checkValueExp.exec(reg_num) != null)
        //{
            //CheckPrefix
            var enteredPrefix =  reg_num.substr(0,prefix.length);
            if(prefix.toUpperCase() === enteredPrefix.toUpperCase())
            {
                var enteredDelimiter = reg_num.substr(prefix.length,1);
                if (enteredDelimiter === prefix_delimiter)
                {
                    var enteredNum = reg_num.substr(prefix.length + 1,reg_num.length - prefix.length - 1);
                    if(enteredNum.length == rootnumber_length && CheckRegNumType(enteredNum,type))
                       retVal = true;
                }
            }
        //}
    }
    return retVal;
}

function CheckRegNumType(regNum, type)
{
    var retVal = false;
    if(type.toUpperCase()==='NUMERIC')
    {
        if(!isNaN(regNum))
            retVal = true;
    }
    return retVal;
}

function IsValidFormula(formula)
{
    var input = formula;
    formula = formula.trim();
    if(formula.length > 0)
    {
        //Check for exact Formula
        if(formula.indexOf("=") == 0)
            formula = formula.substr(1);
        //Check for valid characters   
        var regValidFormula = new RegExp("^[A-Za-z0-9-+*()]");
        //Checks for valid First characters
        var regCheckFirstCharacter = new RegExp("^[A-Za-z(]");
        var strFirstValue = formula.substr(0,1);
        if(regCheckFirstCharacter.exec(strFirstValue) != null)
        {
            if(regValidFormula.exec(formula))
            {
                //Checks if any value of strNotPattern is present in the entered value,it is not valid for Chemical Formula Search but for Name search.
                var regCheckName = new RegExp("(cyclo|thyl|uck|azo|ou|ut|fu|it|iso|ie|ae|ben|romo|min|ene|flour|fluor|emx|floro|iri|iacin|ine|cti|lact|otepa)","i");
                var regCheckNameCase = new RegExp("(oL|Lo|lo|LO)");
                if(!regCheckName.test(formula) && !regCheckNameCase.test(formula))
                {
                    var chrPattern = "(Ala)|(Asp)|(Asn)|(Arg)|(Cys)|(Gly)|(Glu)|(His)|(Ile)|(Lys)|(Leu)|(Met)|(Phe)|(Pro)|(Ser)|(Thr)|(Trp)|(Tyr)|(Val)|(Uub)|(Uut)|(Uuq)|(Uup)|(Uuh)|(Uus)|(Uuo)|(H)|(D)|(B)|(C)|(N)|(O)|(F)|(P)|(S)|(K)|(V)|(Y)|(I)|(W)|(U)|(T)|(He)|(Li)|(Be)|(Ne)|(Na)|(Mg)|(Al)|(Si)|(Cl)|(Ar)|(Ca)|(Sc)|(Ti)|(Cr)|(Mn)|(Fe)|(Co)|(Ni)|(Cu)|(Zn)|(Ga)|(Ge)|(As)|(Se)|(Br)|(Kr)|(Rb)|(Sr)|(Zr)|(Nb)|(Mo)|(Tc)|(Ru)|(Rh)|(Pd)|(Ag)|(Cd)|(In)|(Sn)|(Sb)|(Te)|(Xe)|(Cs)|(Ba)|(Lu)|(Hf)|(Ta)|(Re)|(Os)|(Ir)|(Pt)|(Au)|(Hg)|(Tl)|(Pb)|(Bi)|(Po)|(At)|(Rn)|(Fr)|(Ra)|(Lr)|(Rf)|(Db)|(Sg)|(Bh)|(Hs)|(Mt)|(Ds)|(Rg)|(La)|(Ce)|(Pr)|(Nd)|(Pm)|(Sm)|(Eu)|(Gd)|(Tb)|(Dy)|(Ho)|(Er)|(Tm)|(Yb)|(Ac)|(Th)|(Pa)|(Np)|(Pu)|(Am)|(Cm)|(Bk)|(Cf)|(Es)|(Fm)|(Md)|(No)";
				    var stoiPattern = "[0-9]{1,5}|[0-9]{1,5}[-][0-9]{1,5}";
				    var chargePattern = "[+][0-9]|[-][0-9]|[*][*][0-9]?|[*][0-9]";
				    var objCheck4 = "^(" + chrPattern + ")$";
				    var regCheck = new RegExp("(" + objCheck4 + ")","i");
				    var strInput = formula;
				    var theElem;
				    var theChar;
				    var inccount = 0;
				    var countLen = strInput.length;
				    var strSetFirst = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
				    var strSetSecond = "abcdefghijklmnopqrstuvwxyz";
				    countLen = countLen - 1;
				    do
				    {
					    theElem = "";
					    theChar = strInput.substr(inccount, 1);
					    if ((theChar == "+") | (theChar == "*") | (theChar == "-") | (theChar == "(") | (theChar == ")") | !isNaN(theChar))
					    {
					    input = input + theChar;
					    theElem = "MATCH";
					    countLen = countLen - 1;
					    inccount = inccount + 1;
					    }
					    else if ((strSetFirst.indexOf(theChar) != -1) || (strSetSecond.indexOf(theChar) != -1))
					    {
						    if (countLen >= 2)
						    {
							    theElem = strInput.substr(inccount, 1).toUpperCase() + strInput.substr(inccount + 1, 2).toLowerCase();
							    if (regCheck.exec(theElem) != null)
							    {
								    input = input + theElem;
								    theElem = "MATCH";
								    countLen = countLen - 3;
								    inccount = inccount + 3;
							    }
						    }
						    if ((countLen >= 1) && (theElem != "MATCH"))
						    {
							    theElem = strInput.substr(inccount, 1).toUpperCase() + strInput.substr(inccount + 1, 1).toLowerCase();
							    if (regCheck.exec(theElem) != null)
							    {
								    input = input + theElem;
								    theElem = "MATCH";
								    countLen = countLen - 2;
								    inccount = inccount + 2;
							    }
						    }
						    if (theElem != "MATCH")
						    {
							    theElem = strInput.substr(inccount, 1).toUpperCase();
							    if (regCheck.exec(theElem) != null)
							    {
								    input = input + theElem;
								    theElem = "MATCH";
								    countLen = countLen - 1;
								    inccount = inccount + 1;
							    }
						    }
						    if (theElem != "MATCH")
						    {
							    break;
						    }
					    }
					    else
					    {
						    break;
					    }
				    }
				    while (!(countLen < 0));
				    if (theElem != "MATCH")
				    {
					    return false;
				    }
				    else
				    {
					    return true;
				    }
			    }
			    else
			    {
				    return false;
			    }
		    }
		    else
		    {
			    return false;
		    }
	    }
	    else
	    {
	    return false;
	    }
	}
	else return true;					
}

function ShowProgressControlAndHideChemDraws(e, otherFunction)
{
    if(IsPageValid())
    {
        HideChemDraws();
        if(typeof(otherFunction) == 'function')
            otherFunction(e);
        ShowProgressControl(e);
    }
}

function ShowProgressControl(e)
{
    YAHOO.chembioviz.searchingPanel.show();
}

function IsPageValid()
{
    if (typeof(Page_ClientValidate) == 'function')
        Page_ClientValidate();

    return typeof(Page_IsValid) != 'undefined' ? Page_IsValid : true;
}

function ValidateInputText()
{
    return IsValidMolWeight() && IsValidFormula() && IsValidNameSynonym() && IsValidRegistryID();
}

