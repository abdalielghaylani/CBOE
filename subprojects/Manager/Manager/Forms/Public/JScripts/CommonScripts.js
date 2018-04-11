// JScript File
//Method to check if the opened window is a clean one. We also detect popUp blockers (We really need popUp blockers disables)
function CheckToOpenNewWindow()
{
      var regWindowName = 'COEManager';
      var objectId = gup('SubmittedObjectId');
      var closewithdone = gup('closewithdone');
      var windowname = gup('windowname');
      if(windowname == '') windowname = regWindowName;
      //Read browser version (now supports IE6up )
      var agt = navigator.userAgent.toLowerCase();
      var is_ie6 = (agt.indexOf("msie 6.") != -1);
      var is_ie7 = (agt.indexOf("msie 7.") != -1);
      var attribs = "toolbar=0,location=0,directories=0,status=1,menubar=0,scrollbars,resizable,height=700,width=1100px,top=0,left=100";
      if(window.name != windowname) //Different window context
      {
        var newWin = window.open(window.location.href,windowname,attribs);
        if(newWin == null || typeof(newWin) == "undefined") //Check if window was really open. If not display a PopUp Blocker message.
        {
            window.alert('Pop-up blocker detected!\n\nTo use CambridgeSoft COEManager Enterprise, your web browser must allow pop-up windows.\n For information about allowing pop-up windows, see the instructions for your pop-up blocking software.');
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
function UpdateImage(e)
{
  if(document.images)
  {
    if(typeof(e.attributes.searchingImgID) != 'undefined' && typeof(e.attributes.searchingImgSrc) != 'undefined')
    	setTimeout("document.images['"+e.attributes.searchingImgID.value+"'].src='"+e.attributes.searchingImgSrc.value+"';",50);
  }  
}

function ShowProgressControl(e)
{
//debugger;
if(/*IsPageValid() && */document.getElementById(e.id))
{
    /*Page_IsValid = false;*/
    YAHOO.chembioviz.searchingPanel.show();
}
else 
{
    /*Page_IsValid = false;*/
    YAHOO.util.Event.stopEvent(e);
    }
}
function IsPageValid()
{   if (typeof(Page_ClientValidate) == 'function') Page_ClientValidate();
    return typeof(Page_IsValid) != 'undefined' ? Page_IsValid : true;}
