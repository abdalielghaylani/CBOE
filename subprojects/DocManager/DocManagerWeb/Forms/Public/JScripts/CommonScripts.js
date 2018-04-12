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
function ConfirmDeleteDocument()
{
    //if(typeof(Page_ClientValidate) == 'function') Page_ClientValidate();debugger;
    //if(Page_IsValid)
        return window.confirm('Are you sure you want to delete the document?');
    //else
    //    return false;
}
