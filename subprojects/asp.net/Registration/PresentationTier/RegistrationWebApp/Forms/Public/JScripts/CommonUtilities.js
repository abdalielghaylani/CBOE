    /* CSBR # - 117333
     * Changed By - Soorya Anwar
     * Date - 25-Jun-2010
     * Purpose - To add a new function that opens the specified url in a new window with parameterized window attributes.
     */
function OpenNewWindow(attribs,url)
{      
      var regWindowName = 'REGISTRATION';
      var windowname = gup('windowname');
      if(windowname == '') windowname = regWindowName;     
     //End of Change - 15-Jul-2010
      if (attribs == '' || attribs == null) attribs = "toolbar=0,location=0,directories=0,status=1,menubar=0,scrollbars,resizable,height=740,width=1070px,top=0,left=100";      
      if (url != '' || url != null) 
      {    
        var newWin = window.open(url,windowname,attribs);
        if(newWin == null || typeof(newWin) == "undefined") //Check if window was really open. If not, display a PopUp Blocker message.
        {
            window.alert('Pop-up blocker detected!\n\nTo use CambridgeSoft Registration Enterprise, your web browser must allow pop-up windows.\n For information about allowing pop-up windows, see the instructions for your pop-up blocking software.');
            history.back();
        }
        else
        {
            window.open('','_self');       
        }         
    }
}

/* Changed by -Soorya Anwar
Date of Change - 15-Jul-2010
Purpose - Added function to enable a more generic form of version support      
*/          
//function isVersionSupported(versionSupported)
//{
//  var agent = navigator.userAgent.toLowerCase();
//  var strAgents = agent.split(";");
//  var ieVersionUsed = null;
//  
//  for(i=0;i < strAgents.length; i++)
//  {
//    if(strAgents[i].indexOf("msie") > 0)
//    {
//        ieVersionUsed = strAgents[i];
//    }
//  }
//  var supportedIEVersion = versionSupported.split("|");
//  for(j=0; j < supportedIEVersion.length; j++)
//  { 
//    if (ieVersionUsed.indexOf(supportedIEVersion[j]) != -1)
//        return true;
//  }
//  return false;
//}


// JScript File
function msover(image1,ref1) {document.all[image1].src = ref1;}
function msout(image1,ref1) {document.all[image1].src = ref1;}
function changeImage(imageId,imageURL){document.getElementById(imageId).src = imageURL;}		
function MM_preloadImages() { 
  var d=document; if(d.images){ if(!d.MM_p) d.MM_p=new Array();
    var i,j=d.MM_p.length,a=MM_preloadImages.arguments; for(i=0; i<a.length; i++)
    if (a[i].indexOf("#")!=0){ d.MM_p[j]=new Image; d.MM_p[j++].src=a[i];}}
}
// JScript File
function getElementById(controlID, oDoc) {
    if( document.getElementById ) {
	    return document.getElementById(controlID); 
	}
	
	if( document.all ) {
	    return document.all[controlID]; 
	}

    if( !oDoc ) { oDoc = document; }

    if( document.layers ) {
        if( oDoc.layers[controlID] ) { 
            return oDoc.layers[controlID]; 
        } 
        else {
            //repeatedly run through all child layers
                for( var x = 0, y; !y && x < oDoc.layers.length; x++ ) {
				    //on success, return that layer, else return nothing
                    y = getRefToDiv(controlID,oDoc.layers[x].document); 
                }
                return y; 
        } 
    }
	return false;
} 
	
function Analyze(pluginName, formulaTextBoxName, molweightTextBoxName)
{
    formulaTextBox = getElementById(formulaTextBoxName)
    if(formulaTextBox)
        formulaTextBox.value = cd_getFormula(pluginName, 0);
		
    molWeightTextBox = getElementById(molweightTextBoxName)
    if(molWeightTextBox)
        molWeightTextBox.value = cd_getMolWeight(pluginName, 0);
}

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
function WindowOnload(f) {
    var prev=window.onload;
    window.onload=function(){ if(prev)prev(); f(); }
  }
function CheckToOpenNewWindow( versionSupported )
{
      var regWindowName = 'REGISTRATION';
      var objectId = gup('SubmittedObjectId');
      var closewithdone = gup('closewithdone');
      var windowname = gup('windowname');
      if(windowname == '') windowname = regWindowName;
      //Read browser version (now supports IE6up )
      var agt = navigator.userAgent.toLowerCase();
      var is_ie6 = (agt.indexOf("msie 6.") != -1);
      var is_ie7 = (agt.indexOf("msie 7.") != -1);
      var attribs = "toolbar=0,location=0,directories=0,status=1,menubar=0,scrollbars,resizable,height=740,width=1070px,top=0,left=100";            

      var isSupported_ie6 = (versionSupported.indexOf("6") != -1);
      var isSupported_ie7 = (versionSupported.indexOf("7") != -1);
      var browserSupported = false;
      if (is_ie6) browserSupported = isSupported_ie6;
      if (is_ie7) browserSupported = isSupported_ie7;
      
      if (browserSupported) //If the current browser version is supported by our App, we try to close and re open a new clean window.
      { 
          if(window.name.toUpperCase()!= windowname) //Different window context
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

function ConfirmCancel()
{
    return window.confirm('Are you sure you want to cancel? You will lose all the changes.');
}
function ConfirmCancelAndHistoryBack()
{
    if(window.confirm('Are you sure you want to cancel? You will lose all the changes.'))
    {
        history.back(-1);
        return true;
    }
    else
        return false;
}
function ConfirmAllItemsDeletion(value1,value2)
{
    if(parseInt(value1) == parseInt(value2))
        return window.confirm('Are you sure you want to delete all selected item?');
    else
        return true;
}

function ConfirmSelectedItemsDeletion(alertMessage,selectItemsCounter,noitemsSelectMessage){
    if (parseInt(document.getElementById(selectItemsCounter).value) > 0)
    {
        return window.confirm(alertMessage);
    }
    else
    {
        alert(noitemsSelectMessage);
        return false;
    }
}

function igtbl_chkBoxChangeCustomized(evnt,gn)
{
	if(igtbl_dontHandleChkBoxChange||
	 (ig_csom.IsIE && evnt.propertyName!="checked")||
	 (!ig_csom.IsIE && evnt.type!="change")
	 )
		return false;
	var se=igtbl_srcElement(evnt);
	if(!se)return false;
	var c=se.parentNode;
	while(c && !(c.tagName=="TD" && c.id!=""))
		c=c.parentNode;
	if(!c) return;
	var s=se;
	var cell=igtbl_getCellById(c.id);
	if(!cell) return;
	var column=cell.Column;
	var gs=igtbl_getGridById(gn);
	gs.event=evnt;
	var oldValue=!s.checked;
	if(gs._exitEditCancel || !cell.isEditable() || igtbl_fireEvent(gn,gs.Events.BeforeCellUpdate,"(\""+gn+"\",\""+c.id+"\",\""+s.checked+"\")"))
	{
		igtbl_dontHandleChkBoxChange=true;
		s.checked=oldValue;
		igtbl_dontHandleChkBoxChange=false;
		return true;
	}
	cell.Row._dataChanged|=2;
	if(typeof(cell._oldValue)=="undefined")
		cell._oldValue=oldValue;
	igtbl_saveChangedCell(gs,cell,s.checked.toString());
	cell.Value=cell.Column.getValueFromString(s.checked);
	if(!c.getAttribute("oldValue"))
		c.setAttribute("oldValue",s.checked);
	c.setAttribute("chkBoxState",s.checked.toString());
	var cca=igtbl_getCellClickAction(gn,column.Band.Index);
	if(cca==1 || cca==3)
		igtbl_setActiveCell(gn,c);
	else if(cca==2)
		igtbl_setActiveRow(gn,c.parentNode);
		
	if(cell.Node)
	{
		cell.setNodeValue(!s.checked?"False":"True");
		var cdata=cell.Node.firstChild;
		if(s.checked)
			cdata.text=cdata.text.replace("type='checkbox'","type='checkbox' checked");
		else
			cdata.text=cdata.text.replace(" checked","");
		gs.invokeXmlHttpRequest(gs.eReqType.UpdateCell,cell,s.checked);
	}
	else if(ig_csom.IsNetscape6)
		gs.invokeXmlHttpRequest(gs.eReqType.UpdateCell,cell,s.checked);
	igtbl_fireEvent(gn,gs.Events.AfterCellUpdate,"(\""+gn+"\",\""+c.id+"\",\""+s.checked+"\")");
	if(gs.LoadOnDemand==3)
		gs.NeedPostBack=false;
	if(gs.NeedPostBack)
		igtbl_doPostBack(gn);
	return false;
}
function SelectAllItems(gridName, columnKey, counterId)
{
    var myGrid = igtbl_getGridById(gridName);
    for (i = 0; i < myGrid.Rows.length; i++)
    {
        myGrid.Rows.getRow(i).getCellFromKey(columnKey).setValue(1);
    }
    document.getElementById(counterId).value = parseInt(myGrid.Rows.length);   
}


function getElementById(controlID, oDoc) {
    if( document.getElementById ) {
	    return document.getElementById(controlID); 
	}
	
	if( document.all ) {
	    return document.all[controlID]; 
	}

    if( !oDoc ) { oDoc = document; }

    if( document.layers ) {
        if( oDoc.layers[controlID] ) { 
            return oDoc.layers[controlID]; 
        } 
        else {
            //repeatedly run through all child layers
                for( var x = 0, y; !y && x < oDoc.layers.length; x++ ) {
				    //on success, return that layer, else return nothing
                    y = getRefToDiv(controlID,oDoc.layers[x].document); 
                }
                return y; 
        } 
    }
	return false;
} 
function RemoveComponent(actionField)
{
    if(window.confirm('Are you sure you want to remove the current component?'))
        document.getElementById(actionField).value = 'REMOVE';
     else
        return false;
}

function ConfirmMovingBatch()
{
    if(typeof(Page_ClientValidate) == 'function') Page_ClientValidate();
    if(Page_IsValid)
        return window.confirm('Are you sure you want to move the batch?');
    else
        return false;
}
function ConfirmDeletingBatch()
{
if(typeof(Page_ClientValidate) == 'function') Page_ClientValidate();
    if(Page_IsValid)
        return window.confirm('Are you sure you want to delete the batch?');
    else
        return false;
}
function ConfirmDeleteRegistry()
{
    //if(typeof(Page_ClientValidate) == 'function') Page_ClientValidate();debugger;
    //if(Page_IsValid)
        return window.confirm('Are you sure you want to delete the registry?');
    //else
    //    return false;
}
function SetSQL(txtBoxId, selection)
{
   if(document.getElementById(txtBoxId) != null)
        document.getElementById(txtBoxId).value = 'SELECT ID as key,PICKLISTVALUE as value FROM REGDB.PICKLIST WHERE PICKLISTDOMAINID =' + selection;  
   YAHOO.COERegistration.PickListDomainPanel.hide();  
}
function ShowPanel(anchorCtrlId, innerCtrls, title) 
{
    if(!document.getElementById(anchorCtrlId).disabled)
    {
    YAHOO.namespace('COERegistration');
    YAHOO.COERegistration.PickListDomainPanel = new YAHOO.widget.Panel('pickListPanel',{width:150,visible:true,draggable:true,close:true,modal:false,constraintoviewport:true,zIndex:100,context:[anchorCtrlId,'br','br']});
    var header = '<span class="PickListDivTitle">' + title + '</span>';
    YAHOO.COERegistration.PickListDomainPanel.setHeader(header); 
    YAHOO.COERegistration.PickListDomainPanel.setBody(innerCtrls);
    YAHOO.COERegistration.PickListDomainPanel.render(document.body);
    }
}
function CheckPageValidatorForMenus() {
    if (typeof (Page_ClientValidate) == 'function') {
        Page_ClientValidate();
        if (!Page_IsValid)
            return false;
    }
    return true;
}
function CheckPageValidator(treeId,nodeId)
{
    var Node = igtree_getNodeById(nodeId);
    var Tree = igtree_getTreeById(treeId);
    if(typeof(Page_ClientValidate) == 'function')
    { 
        Page_ClientValidate();
        if(!Page_IsValid)
        {
            Tree.NeedPostBack = false;
            Tree.CancelPostBack = true;
            event.cancelBubble = true;
            return false;
        } 
    }       
}
function IsAValidCas(input)
{
    var retVal = false;
    if (input.length == 0)
        retVal = true;
    else {
        var newCasRegExp = new RegExp("(^[0-9]{1,7}-[0-9][0-9]-[0-9]$)");
        //var regExpCASRN = new RegExp("(^%?[0-9]{1,11}%?$|^%?[0-9]{1,6}[-][0-9]{1,2}%$|^%?[0-9]{1,6}-[0-9]{1,2}-*[0-9*]$)");
        //var regExpCASRN1 = new RegExp("(^[-][0-9]+$|^[-][0-9]*[-][0-9]+$|^[-][0-9]*[-]+$|^[0-9]{1,11}$)");
        //var regExpACX = new RegExp("(^[Xx][0-9]{3,7}$|^[Xx][0-9]{3,7}[%*]?$|^[Xx][0-9]{3,7}-[0-9]{1}$)","i"); 
        //if(regExpCASRN.test(input) && !regExpCASRN1.test(input))
        //if(newCasRegExp.test(input))
        retVal = newCasRegExp.test(input);
        //CheckSum implementation.
        if (retVal) {
            freeCAS = input.replace('-', '').replace('-', '');
            CASsum = 0;
            i = freeCAS.length - 1;
            while (i >= 1) {
                CASsum = CASsum + i * (freeCAS.substring(freeCAS.length - i - 1, freeCAS.length - i));
                i--;
            }
            if ((CASsum % 10) == ((freeCAS.substring(freeCAS.length - 1, freeCAS.length)) % 10))
                retVal = true;
            else
                retVal = false;
        }
    }
    return retVal;
}                                
function ShowMessagePopUp(panleId) 
{
    if(document.getElementById(panleId)!=null)
    {
    YAHOO.namespace('COERegistration');
    YAHOO.COERegistration.MessagePopupPanel = new YAHOO.widget.Panel(panleId,{width:450,height:90,visible:true,iframe:true,fixedcenter:true,draggable:true,close:true,modal:false,constraintoviewport:true,zIndex:15000});
    YAHOO.COERegistration.MessagePopupPanel.render(document.body);
    }
}


