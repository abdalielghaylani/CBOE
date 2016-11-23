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
 function GetClientId(serverId) 
 {
   if (typeof(ClientIDList) !="undefined" && typeof(ServerIDList) !="undefined")  
    {
      for(iListCount=0; iListCount<ServerIDList.length; iListCount++)
       {
         if ( ServerIDList[iListCount].indexOf(serverId)==0 ) 
           {
             return ClientIDList[iListCount];
            }
        }
     }
   }
 
  function ResetDropDownFields(fieldList){
           
          var fieldListArray = fieldList.split(',');
          for (var i=0;i<fieldListArray.length;i++)
          {
                var requiredFieldClientId=GetClientId(fieldListArray[i]);
                var checkDateField =(requiredFieldClientId.indexOf('_input')>0?requiredFieldClientId.replace("_input",""):'');
                if (checkDateField!='' && document.getElementById(checkDateField)!=null && document.getElementById(checkDateField).previousSibling !=null&&document.getElementById(checkDateField).previousSibling.previousSibling !=null)
                     document.getElementById(checkDateField).previousSibling.previousSibling.className ='FELabel';
                if (document.getElementById(requiredFieldClientId).previousSibling!=null && document.getElementById(requiredFieldClientId).type!="checkbox")
                    document.getElementById(requiredFieldClientId).previousSibling.className ='FELabel';
                else if (document.getElementById(requiredFieldClientId).type=="checkbox" && document.getElementById(requiredFieldClientId).nextSibling !=null )
                      document.getElementById(requiredFieldClientId).nextSibling.className ='FELabel';
          }
  }
  
  function DropDown_CheckRequiredFields(source,arguments,selectedIndex,RequiredField,ErrorMessage)
  {
    
    var errorMessageDisplay=(typeof(source.errormessage)=="undefined"?'':source.errormessage);
    if (source.controltovalidate!=null)
       objDropdown =document.getElementById(source.controltovalidate);
    var requiredFieldClientId=GetClientId(RequiredField);
    if (objDropdown.selectedIndex==selectedIndex && requiredFieldClientId!=null && document.getElementById(requiredFieldClientId)!=null)
    {
      var checkDateField =(requiredFieldClientId.indexOf('_input')>0?requiredFieldClientId.replace("_input",""):'');
      if (checkDateField!='' && document.getElementById(checkDateField)!=null && document.getElementById(checkDateField).previousSibling !=null&&document.getElementById(checkDateField).previousSibling.previousSibling !=null)
         document.getElementById(checkDateField).previousSibling.previousSibling.className ='FELabelRequired';
      if (document.getElementById(requiredFieldClientId).previousSibling !=null && document.getElementById(requiredFieldClientId).type!="checkbox")
            document.getElementById(requiredFieldClientId).previousSibling.className ='FELabelRequired';
      else if (document.getElementById(requiredFieldClientId).type=="checkbox" && document.getElementById(requiredFieldClientId).nextSibling !=null)
           document.getElementById(requiredFieldClientId).nextSibling.className ='FELabelRequired';
            //document.getElementById(requiredFieldClientId).focus();
      if (GetControlValue(requiredFieldClientId)=='')
       {
         errorMessageDisplay =errorMessageDisplay+ErrorMessage+'<br>';
         source.errormessage=errorMessageDisplay;
         return false;
        }
      }
//     else if (requiredFieldClientId!=null && document.getElementById(requiredFieldClientId)!=null)
//      {
//       // if (document.getElementById(requiredFieldClientId).previousSibling !=null)
//          document.getElementById(requiredFieldClientId).previousSibling.className ='FELabel';
//      }
     return true;
   }
 
  function CheckBox_EnableFields(objid,FieldList,RequiredFields)
   {
     if (objid !='')
      {
        var obj=document.getElementById(objid);
        var arFields =FieldList.split(',');
        for (iloop=0;iloop<arFields.length;iloop++)
         { 
           var requiredFieldClientId=GetClientId(arFields[iloop]);
          
           if (requiredFieldClientId!=null && document.getElementById(requiredFieldClientId)!=null && obj!=null )
            {
              if (obj.checked)
              {
                 document.getElementById(requiredFieldClientId).setAttribute('readOnly',false);
                 document.getElementById(requiredFieldClientId).removeAttribute('readOnly');
                 document.getElementById(requiredFieldClientId).className='FETextBox';
//                 if (document.getElementById(requiredFieldClientId).readOnly !='undefined')
//                    document.getElementById(requiredFieldClientId).readOnly=false;
               }
              else 
              {
                 document.getElementById(requiredFieldClientId).setAttribute('readOnly','readOnly');
                 document.getElementById(requiredFieldClientId).className='FETextBoxDisable';
                 document.getElementById(requiredFieldClientId).value='';
//                if (document.getElementById(requiredFieldClientId).readOnly !='undefined')
//                    document.getElementById(requiredFieldClientId).readOnly=true;
               }
            }
          }
         var arReqFields =RequiredFields.split(',');
         for (iloop1=0;iloop1<arFields.length;iloop1++)
         { 
           var requiredFieldCurrent=GetClientId(arReqFields[iloop1]);
           if (requiredFieldCurrent!=null && document.getElementById(requiredFieldCurrent)!=null && document.getElementById(requiredFieldCurrent).previousSibling !=null && obj!=null)
            {
              if (obj!=null && obj.checked)
              {
                 document.getElementById(requiredFieldCurrent).previousSibling.className='FELabelRequired';
               }
              else 
               {
                 document.getElementById(requiredFieldCurrent).previousSibling.className='FELabel';
               }
            }
         }
      }
   }
   
   
   
   function CheckBox_ShowFields(objid,FieldList)
   {
     if (objid !='')
      {
        var obj=document.getElementById(objid);
        var arFields =FieldList.split(',');
        for (iloop=0;iloop<arFields.length;iloop++)
         { 
           var requiredFieldClientId=GetClientId(arFields[iloop]);
          
           if (requiredFieldClientId!=null && document.getElementById(requiredFieldClientId)!=null && obj!=null)
            {
              if (obj!=null && obj.checked)
              {
                 document.getElementById(requiredFieldClientId).setAttribute('visible',true);
//                 if (document.getElementById(requiredFieldClientId).readOnly !='undefined')
//                    document.getElementById(requiredFieldClientId).readOnly=false;
               }
              else 
              {
                document.getElementById(requiredFieldClientId).setAttribute('visible','false');
                document.getElementById(requiredFieldClientId).value='';
//                if (document.getElementById(requiredFieldClientId).readOnly !='undefined')
//                    document.getElementById(requiredFieldClientId).readOnly=true;
               }
            }
         }
      } 
   }
    
   function CheckBox_CheckRequiredFields(source,arguments,CheckBoxId,RequiredField,ErrorMessage)
      {
        var objCheckBox;
        var errorMessageDisplay=(typeof(source.errormessage)=="undefined"?'':source.errormessage);
        RequiredField =GetClientId(RequiredField);
        if (source.controltovalidate!=null){
            var FieldClientId=GetClientId(CheckBoxId);
            objCheckBox =document.getElementById(FieldClientId);
           }
        if (objCheckBox!=null && objCheckBox.checked && document.getElementById(RequiredField)!=null)
        {
          var checkDateField =(RequiredField.indexOf('_input')>0?RequiredField.replace("_input",""):'');
          if (checkDateField!='' && document.getElementById(checkDateField)!=null && document.getElementById(checkDateField).previousSibling !=null&&document.getElementById(checkDateField).previousSibling.previousSibling !=null)
             document.getElementById(checkDateField).previousSibling.previousSibling.className ='FELabelRequired';
          if (document.getElementById(RequiredField).previousSibling !=null && document.getElementById(RequiredField).type!="checkbox" )
              document.getElementById(RequiredField).previousSibling.className ='FELabelRequired';
          else if (document.getElementById(RequiredField).type=="checkbox" && document.getElementById(RequiredField).nextSibling !=null)
              document.getElementById(requiredFieldClientId).nextSibling.className ='FELabelRequired';
          if (GetControlValue(RequiredField)=='')
             {
                errorMessageDisplay =errorMessageDisplay+ErrorMessage+'<br>';
                 source.errormessage=errorMessageDisplay;
                return false;
             }
          }
         else if (document.getElementById(RequiredField)!=null)
          {
            if (document.getElementById(RequiredField).previousSibling !=null)
                document.getElementById(RequiredField).previousSibling.style.color='';
          }
          return true;
     }
   
   function ResetRequiredFields(curcontrolId,referenceControlId,value,resetControlList)
    {
      var referencedControl=GetClientId(referenceControlId);
      if (document.getElementById(curcontrolId) !=null  &&  document.getElementById(referencedControl)!=null  && GetControlValue(referencedControl) == value)
         {
           var classname ='FELabelRequired';
           if (GetControlValue(curcontrolId)!='')
              classname ='FELabel';
           var arControlList=resetControlList.split(',');
           for (iCount=0;iCount<arControlList.length;iCount++)
             {
                var resetcontrol=GetClientId(arControlList[iCount]);
                var checkDateField =(resetcontrol.indexOf('_input')>0?resetcontrol.replace("_input",""):'');
                if (checkDateField!='' && document.getElementById(checkDateField)!=null && document.getElementById(checkDateField).previousSibling !=null&&document.getElementById(checkDateField).previousSibling.previousSibling !=null)
                     document.getElementById(checkDateField).previousSibling.previousSibling.className =classname;
                if (document.getElementById(resetcontrol) !=null && document.getElementById(resetcontrol).previousSibling !=null && document.getElementById(resetcontrol).type!="checkbox")
                {
                    document.getElementById(resetcontrol).previousSibling.className =classname;
                }
                else if (document.getElementById(resetcontrol).type=="checkbox" && document.getElementById(resetcontrol).nextSibling !=null)
                      document.getElementById(resetcontrol).nextSibling.className =classname;
             }
          }
      }
    
     function GetControlValue(controlId)
     {
         var returnVal=new String();
         if (document.getElementById(controlId).type=="checkbox")
         {
            if (document.getElementById(controlId).checked)
                returnVal='true';
            else
              returnVal='';
          }
         else if (document.getElementById(controlId).type=="select-one")
             returnVal =(document.getElementById(controlId).selectedIndex==0?'':document.getElementById(controlId).selectedIndex);
         else
         {
            returnVal=document.getElementById(controlId).value; 
            returnVal=trim(returnVal);
            
            if (returnVal.length==0)
            {
            returnVal = ''; 
            }     
          }  
         return returnVal;
      }
      function LTrim( value )
       {
	      var re = /\s*((\S+\s*)*)/;
	      return value.replace(re, "$1");
	   }
      function RTrim( value )
       {
	      var re = /((\s*\S+)*)\s*/;
	      return value.replace(re, "$1");
	   }
     function trim( value )
      {	
	    return LTrim(RTrim(value));
      }
      
      function setApprover(approverfieldname){
        
        var requiredFieldClientId=GetClientId(approverfieldname);
         if (document.getElementById(requiredFieldClientId) !=null){
         
         //get logged in user id and set value to this field
         }
       
         
    }