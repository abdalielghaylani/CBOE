 /*
  * Infragistics WebNavigator CSOM Script: ig_webmenux.js
  * Version 6.3.20063.53
  * Copyright(c) 2001-2006 Infragistics, Inc. All Rights Reserved.
  */


var ig_menuPopup = null;
var ig_oActiveMenu = null;
var ig_oCurrentSubMenu;
var igmenu_initialized = false;

if(typeof(igmenu_array) != "object")
	var igmenu_array=[];
if(typeof(igmenu_itemArray) != "object")
	var igmenu_itemArray=[];
if(typeof(igmenu_submenuArray) != "object")
	var igmenu_submenuArray = [];
if(typeof(igmenu_panelArray) != "object")
	var igmenu_panelArray = [];

var iframeCount = 0; 
var igmenu_dropDowns;

// Warning: Private functions for internal component usage only
// The functions in this section are not intended for general use and are not supported
// or documented.
function igmenu_podmouseover(evnt) {
	if(evnt == null)
		evnt = window.event;
	var eItem = ig_fromEvent(evnt);
	var oMenu = igmenu_getMenuByItemId(eItem.id);
	var oItem = igmenu_getItemById(eItem.id);
			
	if(oItem.isSeparator()) {
		return;
	}
	if(!oItem.isEnabled())
		return;
	
	oItem.hover();
	ig_inMenu = true;	


}
function igmenu_podmouseout(evnt) {
	var eItem = ig_fromEvent(evnt);
	var e = ig.getSourceElement(evnt);
	if(ig_isMouseOutSourceAChild(evnt,eItem))
		return;
	var oItem = igmenu_getItemById(eItem.id);
	var oMenu = oItem.WebMenu;
	
	if(oItem.isSeparator()) {
		return;
	}
	if(!oItem.isEnabled())
		return;
	if(!oItem.getSelected())
		oItem.unhover(true);
	ig_inMenu = false;	
	
		
}
function igmenu_podmousedown(evnt) {
   	ig_inMenu = true;	
	ig_startClick = true;
}
function igmenu_podmouseup(evnt) {
	if(ig_startClick) {
	    var eItem = ig_fromEvent(evnt);
	    var e = ig.getSourceElement(evnt);
	    if(ig_isMouseOutSourceAChild(evnt,eItem))
		    return;
	    var oItem = igmenu_getItemById(eItem.id);
	    var oMenu = oItem.WebMenu;
    	
        if(oMenu._uninitialized == true) {
            oMenu._uninitialized = false;
	        if(!oMenu.viewport) {
			    oMenu.viewport = new ig_viewport();
			    oMenu.viewport.createViewport(oMenu._frameElement, ViewportOrientationEnum.Horizontal);
			    oMenu.viewport.transferPositionToDiv(oMenu._frameElement);
	        }
    	}
		if(!oItem.isEnabled())
			return;
	    var oChildSubMenu = oItem.getChildSubMenu();
	    var oOwnerSubMenu = oItem.getOwnerSubMenu();
	    if(oOwnerSubMenu.element.style.width == "")
			oOwnerSubMenu.element.style.width = oOwnerSubMenu.element.offsetWidth.toString() + "px";
	    if(oChildSubMenu != null) {
		    oChildSubMenu.drillDownDisplay(oOwnerSubMenu, oItem);
		    return;
	    }
	}
    igmenu_mouseup(evnt);
}

function igmenu_podheadermousedown(evnt) {
}
function igmenu_podheadermouseup(evnt) {
	var e = ig.getSourceElement(evnt);
	var parent = e.parentNode;
	var eSubMenu;
	while(parent) {
		if(parent.getAttribute("submenu")) {
			eSubMenu = parent;
			break;
		}
		parent = parent.parentNode;
	}
	var oSubMenu = igmenu_getSubMenuById(null, eSubMenu.id);
	oSubMenu.drillDownUnDisplay();
	
}

function igmenu_submenumouseover(evnt) {
	clearTimeout(igmenu_clearMenuTimerId);
	clearCurrentMenu = false;
}
function igmenu_submenumouseout(evnt) {
	var e = ig.getSourceElement(evnt);
	while(!e.id)
		e = e.parentNode;
	var oMenu = igmenu_getMenuByItemId(e.id);

	clearTimeout(igmenu_clearMenuTimerId);
	clearCurrentMenu = true;
	if(oMenu)
		igmenu_clearMenuTimerId = setTimeout('clearTimerExpired()', oMenu.ExpandEffects.RemovalDelay);
	else
		alert("oMenu == null");
}

// private - implements mouseover event handling for the menu
function igmenu_mouseover(evnt) {
	if(evnt == null)
		evnt = window.event;
	var eItem = ig_fromEvent(evnt);
	ig_inMenu = true;	
	var oMenu = igmenu_getMenuByItemId(eItem.id);
	var oItem = igmenu_getItemById(eItem.id);
	if(oItem._hovered) {
		clearTimeout(igmenu_clearMenuTimerId);
		return;
	}
	if(ig_oActiveMenu && ig_oActiveMenu != oMenu) {
		ig_oActiveMenu.hideSubMenus();
		ig_oActiveMenu = oMenu;
	}
	clearCurrentMenu = false;
			
	if(oItem.isSeparator()) {
		return;
	}
	if(!oItem.getEnabled()) {
		if(oItem.isTopLevelItem()) 
			oMenu.hideSubMenus();
		else {
			var oOwnerSubMenu = oItem.getOwnerSubMenu();
			oOwnerSubMenu.dismissDescendantSubMenus();
		}
		return;
	}
	oItem.hover();
	
	if(oMenu.isHorizontal() && 
		oItem.isTopLevelItem() &&
		(oMenu.isRichClient() || oMenu.isXPClient()) && 
		!oMenu.TopHoverStarted)
			return;

	var oChildSubMenu = oItem.getChildSubMenu();
	var oOwnerSubMenu = oItem.getOwnerSubMenu();
	clearTimeout(igmenu_displayMenuTimerId);
	if(oChildSubMenu != null) {
		if(oMenu.isHorizontal() && oItem.isTopLevelItem()) {
			oChildSubMenu.show(oItem);
		}
		else {
			oItem.__displaySubMenu(oChildSubMenu);
		}
	}
	else  
		oOwnerSubMenu.dismissDescendantSubMenus();
}

var igmenu_displayMenuTimerId;
var igmenu_clearMenuTimerId;

// private - implements mouseout event handling
function igmenu_mouseout(evnt) {
	ig_inMenu = false;	
	var eItem = ig_fromEvent(evnt);
	
	var e = ig.getSourceElement(evnt);
	if(ig_isMouseOutSourceAChild(evnt,eItem))
		return;
	var oItem = igmenu_getItemById(eItem.id);
	var oMenu = oItem.WebMenu;
	
	if(oItem.isSeparator()) {
		clearCurrentMenu = true;
		if(ig_csom.IsIE) {
			clearTimeout(igmenu_clearMenuTimerId);
			igmenu_clearMenuTimerId = setTimeout('clearTimerExpired()', oMenu.ExpandEffects.RemovalDelay);
		}
		return;
	}
	if(!oItem.isEnabled())
		return;
	if(!oItem.getSelected())
		oItem.unhover(true);
		
	clearTimeout(igmenu_displayMenuTimerId);
	clearCurrentMenu = true;
	clearTimeout(igmenu_clearMenuTimerId);
	igmenu_clearMenuTimerId = setTimeout('clearTimerExpired()', oMenu.ExpandEffects.RemovalDelay);
	ig_cancelEvent(evnt);
}

// private - initializes the menu object on the client
function igmenu_initMenu(menuId) {
   igmenu_itemArray=[];
   if(!igmenu_initialized) {
	  igmenu_initialize();
	  igmenu_initialized = true;
   }

   ig_oActiveMenu = null;
   var oMenu = igmenu_array[menuId];
	if(oMenu)
	{
		oMenu.cancelAnimations();
		for(var id in igmenu_submenuArray)
		{
			var o = igmenu_submenuArray[id];
			if(o && o.WebMenu && o.WebMenu.MenuName == menuId)
				igmenu_submenuArray[id] = null;
		}
		oMenu.base = oMenu.Events = null;
		ig_dispose(oMenu);
	}
   
   var menuElement = igmenu_getElementById(menuId+"_MainM");
   oMenu = igmenu_Create(menuElement,eval("igmenu_"+menuId+"_Menu"));
	
   igmenu_array[menuId] = oMenu;
   oMenu.fireEvent(oMenu.Events.InitializeMenu,"(\""+menuId+"\");");
   
   if(!ig_csom.IsIE  || ig_csom.IsMac)
		oMenu.HideDropDowns = false;
		
   if(!ig_csom.IsIE55Plus && oMenu.HideDropDowns==true && igmenu_dropDowns==null) {
		igmenu_dropDowns = document.all.tags("SELECT");
   }
   
   oMenu.Element.tabIndex = 0;
   oMenu.hasFrame = false;
   var frameElement = menuElement;
   if(frameElement.parentNode.getAttribute('menuframe') == "1") {
	  frameElement = frameElement.parentNode;
	  oMenu.hasFrame = true;
   }
   if(oMenu.isDrillDown()) {
       if(frameElement.offsetWidth == 0 && frameElement.offsetHeight == 0) {
            oMenu._uninitialized = true;
            oMenu._frameElement = frameElement;
       }
       else 
	   if(!oMenu.viewport) {
			oMenu.viewport = new ig_viewport();
			oMenu.viewport.createViewport(frameElement, ViewportOrientationEnum.Horizontal);
			oMenu.viewport.transferPositionToDiv(frameElement);
	   }
   }
   if(ig_csom.IsIE) {
      if(!oMenu.Element.isDisabled)
		igmenu_initHandlers(oMenu, oMenu.getItems());
   }
   else
		igmenu_initHandlers(oMenu, oMenu.getItems());
	if(!oMenu.Element._old)
	{
		ig_shared.addEventListener(oMenu.Element, "mouseover", igmenu_submenumouseover, true);
		ig_shared.addEventListener(oMenu.Element, "mouseout", igmenu_submenumouseout, true);
		ig_shared.addEventListener(oMenu.Element, "focus", igmenu_keyboard.onfocus, true);
	}
	oMenu.Element._old = true;
   oMenu.MenuName = menuId;		
      
   oMenu.MenuLoaded=true;
   return oMenu;
}

// private - constructor for the menu object
function igmenu_Create(menuElement, props) {
    if(!ig_WebControl.prototype.isPrototypeOf(ig_WebMenu.prototype))
    {
        ig_WebMenu.prototype = new ig_WebControl();
        ig_WebMenu.prototype.constructor = ig_WebMenu;
        ig_WebMenu.prototype.base=ig_WebControl.prototype;
		
		ig_WebMenu.prototype.init = function(menuElement, menuProps) 
		{
			this.MenuId = menuElement.id;
			this.MenuElement = menuElement;
			this.Id = menuElement.id;
			this.Element = menuElement;
			this.UniqueId = menuProps[0];
			this.MenuTarget=menuProps[1];
			this.WebMenuStyle=menuProps[2];
			this.HoverClass=menuProps[4];
			this.TopSelectedClass=menuProps[5];
			this.ExpandEffects = new igmenu_expandEffects(menuProps[8], menuProps[9], menuProps[10], menuProps[11], menuProps[12], menuProps[13], menuProps[14]);
			this.CheckedImage=menuProps[15];
			this.UncheckedImage=menuProps[16];
			this.DisabledClass=menuProps[17];
			this.DefaultItemClass=menuProps[18];
			this.ScrollImageTop=menuProps[19];
			this.ScrollImageTopDisabled=menuProps[20];
			this.ScrollImageBottom=menuProps[21];
			this.ScrollImageBottomDisabled=menuProps[22];
			this.LeftHandDisplay=menuProps[23];
			this.CurrentLeftHandDisplay=this.LeftHandDisplay
			this.HideDropDowns=menuProps[24];
			this.TargetUrl=menuProps[25];
			this.TargetFrame=menuProps[26];
	
			if(menuProps.length > 26) {
				this.MergeStyles = menuProps[27];
			}
			else {
				this.MergeStyles = false;
			}
			
			if(menuProps.length > 27) {
				this.HeaderClass = menuProps[28];
			}
			
			var uniqueId = this.getClientUniqueId();
			this.Events = new igmenu_events(eval("igmenu_"+uniqueId+"_Events"));
			this.MenuLoaded = false;
			this.NeedPostBack = false;
			this.CancelPostBack = false;
			this.TopHoverStarted = false;
			
		}
		ig_WebMenu.prototype.isHorizontal = function () {
			return (this.MenuTarget == 1);
		}
		ig_WebMenu.prototype.isVertical = function () {
			return (this.MenuTarget == 2);
		}
		ig_WebMenu.prototype.isPopup = function () {
			return (this.MenuTarget == 3);
		}
		ig_WebMenu.prototype.isWebStandard = function () {
			return (this.WebMenuStyle == 1);
		}
		ig_WebMenu.prototype.isRichClient = function () {
			return (this.WebMenuStyle == 2);
		}
		ig_WebMenu.prototype.isXPClient = function () {
			return (this.WebMenuStyle == 3);
		}
		ig_WebMenu.prototype.isDrillDown = function () {
			return (this.WebMenuStyle == 4);
		}
		
		ig_WebMenu.prototype.getItems = function ()	{
			var itemAr = new Array();
			var rows, cells, index = 0;
			if(this.isPopup()) {
			    if(ig_csom.IsIE)
				    rows = this.Element.firstChild.firstChild.rows;
				else
				    rows = this.Element.childNodes[1].firstChild.rows;
			}
			else
				rows = this.Element.rows;
			for(var r = 0; r < rows.length; r++) {	
				cells = rows[r].cells;
				for(var i = 0; i < cells.length; i++) 
					itemAr[index++] = igmenu_getItemById(cells[i].firstChild.id);
				}
			return itemAr;
		}

		// private
		ig_WebMenu.prototype.getClientUniqueId = function() {
			var u = this.UniqueId.replace(/:/gi, "");
			while(u.indexOf("$") != -1)
				u = u.replace("$",""); // CLR 2.0
	        
			u = u.replace(/_/gi, "");
			u = u.replace(/\-/gi,"");
			if(u.indexOf("{") != -1) {
    			u = "IG" + u;
				u = u.replace(/\{/gi, "");
				u = u.replace(/\}/gi, "");
			}
			return u;
		}
		
		// private - Fires an event to client-side script and then to the server if necessary
		ig_WebMenu.prototype.fireEvent = function (eventObj,eventString)
		{
			var result=false;
			if(eventObj[0]!="")
				result=eval(eventObj[0]+eventString);
			if(this.MenuLoaded && result!=true && eventObj[1]==1 && !this.CancelPostBack)
				this.NeedPostBack=true;
			this.CancelPostBack=false;
			return result;
		}

		ig_WebMenu.prototype.getFirstEnabledItem = function()
		{			
			var oItem = null;
			var oItems = this.getItems();
			for (var iCount=0; oItems && iCount < oItems.length; iCount++)
			{
				oItem = oItems[iCount];
				if (oItem.getEnabled()) 
					return oItem;
			}
			return null;		
		}
		
		ig_WebMenu.prototype.initEventHandlers = function(oSubMenu, oItem) {
			if(!oSubMenu._events) {
				if(!oItem && this.isPopup())
					return;
				else  {
					var oItems = oItem.getItems();
					igmenu_initHandlers(oItem.WebMenu, oItems);
					oSubMenu._events = true;
					if(oSubMenu.element._old)
						return;
					oSubMenu.element._old = true;
					ig_shared.addEventListener(oSubMenu.element, "mouseover", igmenu_submenumouseover, true);
					ig_shared.addEventListener(oSubMenu.element, "mouseout", igmenu_submenumouseout, true);
				}
			}
		}

		// private - clears all submenus from display
		ig_WebMenu.prototype.hideSubMenus = function() {
			this.CurrentLeftHandDisplay=this.LeftHandDisplay;
			this.igCurrentChild = null;

			if(this._oRootSubMenu)
				this._oRootSubMenu.dismiss();
			var oSubMenuMain = igmenu_getSubMenuById(this, this.Element.id);
			oHoveredItem = oSubMenuMain.getHoveredItem();
			if(oHoveredItem)
				oHoveredItem.unhover();
		}
		
		// private - cancels all animations in progress
		ig_WebMenu.prototype.cancelAnimations = function() {
			if(this._oRootSubMenu)
				this._oRootSubMenu.cancelAnimation();
		}

		// private - Hides the menu if it is displayed as a popup
		ig_WebMenu.prototype.dismiss = function () {
			ig_inMenu = false;
			igmenu_pageMouseDown();
		}

		// private - returns the tabIndex of the webMenu
		ig_WebMenu.prototype.getTabIndex = function(){
			return this.Element.tabIndex;
		}		
		
		// private - Update internal buffer for items that are checked on or off
		ig_WebMenu.prototype.updateItemCheck = function(itemId, bChecked){ 
			var formControl = igmenu_getElementById(this.UniqueId);
			if(formControl == null)
				return;
			var menuState = formControl.value;
			var newValue;
			var oldValue;
			if(bChecked){
				oldValue = "0";	newValue = "1";
			}
			else{
				oldValue = "1";	newValue = "0";
			}
			var oldString = itemId + ":Chck=" + oldValue + "<%;";
			var newString = itemId + ":Chck=" + newValue + "<%;";
			if(menuState.search(oldString) >= 0)
				menuState = menuState.replace(oldString, newString);
			else {
			oldString = itemId + ":Chck=" + newValue + "<%;";
			if(menuState.search(oldString) >= 0){
				menuState = menuState.replace(oldString, newString);
			}
			else
				menuState += newString;
			}
			formControl.value = menuState; 
		}
	
		
		ig_WebMenu.prototype.initSubMenuRoot = function (oSubMenu) {
			this.hideSubMenus();
			if(!oSubMenu)
				return;
			ig_oCurrentSubMenu = oSubMenu;
			ig_oActiveMenu = this;
			this._oRootSubMenu = oSubMenu;
			this._oActiveSubMenu = oSubMenu;
			if(!ig_csom.IsIE55Plus && this.HideDropDowns) {
				igmenu_hideDropDowns(true);
			}
		}
	}
    return new ig_WebMenu(menuElement, props);
}

function ig_WebMenu(menuElement, props)
{
    if(arguments.length != 0)
        this.init(menuElement, props);
}

// private - event initialization for menu object
function igmenu_events(events)
{
	this.InitializeMenu=events[0];
	this.ItemCheck=events[1];
	this.ItemClick=events[2];
	this.SubMenuDisplay=events[3];
	this.ItemHover=events[4];
}

// private - event initialization for menu object
function igmenu_expandEffects(a, b, c, d, e, f, g)
{
	this.Duration=a;
	this.Opacity=b;
	this.Type=c;
	this.ShadowColor=d;
	this.ShadowWidth=e;
	this.Delay=f;
	this.RemovalDelay=g;
}

// private - obtains the row element associated with the event
function ig_fromEvent(evnt) { 
	var item;
	if(ig_csom.IsNetscape6)  {
		item = evnt.target;
	}
	else
		item = evnt.srcElement;
	return ig_fromItem(item);
}

// private 
function ig_fromItem(eItem) {
	while(eItem.tagName != "TABLE") {
		if(eItem == null)
			return null;
		eItem = eItem.parentNode;
	}
	return eItem;
}

// private
function igmenu_getSubMenu(eItem) {
	submenu = false;
	while(!submenu)  {
		if(eItem.getAttribute("submenu") == '1')
			submenu = true;
		else
			eItem = eItem.parentNode;
	}
	return eItem;
}

// private
function igmenu_getRightPos(e) {
    var x = e.offsetRight;
    var tmpE = e.offsetParent;
    while (tmpE != null) {
        x += tmpE.offsetRight;
        tmpE = tmpE.offsetParent;
    }
    return x;
}
// private
function igmenu_getLeftPos(element) {
    var x = 0;
    var parent = element;
    while (parent != null) {
        x += parent.offsetLeft;
        parent = parent.offsetParent;
    }
    return x;
}
// private
function igmenu_getTopPos(element) {
    var y = 0;
    var parent = element;
    while(parent != null) {
		y += parent.offsetTop;
        parent = parent.offsetParent;
	}
    return y;
}

var clearCurrentMenu = true;
// private - Clears submenus at timer expiration
function clearTimerExpired() {
	if(clearCurrentMenu && ig_oActiveMenu != null) {
		ig_oActiveMenu.hideSubMenus();
		clearTimeout(igmenu_displayMenuTimerId);
		igmenu_hideDropDowns(false);
	}
}

// private - Handles the mouse down event
function igmenu_mousedown(evnt) {
	if(evnt == null)
		evnt = window.event;
	var eItem = ig_fromEvent(evnt);
	if(eItem != null) {
		ig_inMenu = true;
		var igDisabled = eItem.getAttribute("igDisabled");
		if(igDisabled != null && igDisabled.length > 0) {
			return;
		}
	}
	else
		return;
	var oMenu = igmenu_getMenuByItemId(eItem.id);
	var oItem = igmenu_getItemById(eItem.id);
	if(oMenu == null || oMenu.MenuLoaded == false)
		return;

	if(oMenu.isHorizontal() && (oMenu.isRichClient() || oMenu.isXPClient()) && oItem.isTopLevelItem()){
		var childId = oItem.getChildSubMenuId();
		if(childId != null && childId.length > 0) {
			var currentChildId = igmenu_getSubMenu(eItem).igCurrentChildId;
			//if(childId != null && childId.length > 0 && childId == currentChildId) {
			//	oMenu.hideSubMenus();
			//	ig_startClick=false;
			//	oMenu.TopHoverStarted = false;
			//	oItem.hover();
			//	return;
			//}
			var oldClass = eItem.igClass;
			eItem.setAttribute("igPrevClass", oldClass);
			clearTimeout(igmenu_displayMenuTimerId);
			oMenu.TopHoverStarted = true;
			oItem._hovered = false;
			oItem.hover();
			var oSubMenu = oItem.getChildSubMenu();
			oSubMenu.show(oItem);
			igmenu_getSubMenu(eItem).igCurrentChildId = childId;
			igmenu_getSubMenu(eItem).igCurrentItem = item.id;
			return;
		}
	}
	if(oItem.getChildSubMenuId() == null)
		oMenu.TopHoverStarted=false;
	ig_startClick = true;	
	ig_cancelEvent(evnt);
}

var	ig_startClick = false;

// private - Handles the mouse up event
function igmenu_mouseup(evnt, eItemElement) {
	var eItem = null; 
	if(evnt == null)
		evnt = window.event;
	if (!eItemElement)
		eItem = ig_fromEvent(evnt);
	else
		eItem = ig_fromItem(eItemElement);
			
	if(eItem==null){return;}
	
	var oMenu = igmenu_getMenuByItemId(eItem.id);
	var oItem = igmenu_getItemById(eItem.id);
	if(oMenu==null) {
		return;
	}
	var topLevel = oItem.isTopLevelItem();
	var oSubMenu = oItem.getChildSubMenu();
	var igUrl = oItem.getTargetUrl();
	
	if(ig_csom.IsIE)
		evnt.cancelBubble = true;
	if(!oItem.isEnabled()) 
		return;
		
	if(oItem.isSeparator()) {
		clearCurrentMenu = false;
		return;
	}
	
	if(oSubMenu) {
		if(igUrl == null || igUrl.length == 0) {
			oMenu.fireEvent(oMenu.Events.ItemClick,"(\""+ oMenu.MenuName +"\",\""+eItem.id+"\")");
			if(oMenu.NeedPostBack && (oMenu.isWebStandard() || ((oMenu.isRichClient() || oMenu.isXPClient()) && !topLevel)))	
				__doPostBack(oMenu.UniqueId,eItem.id+":MenuClick");
			return;
		}
	}
		
	if(ig_startClick == true)
	{
		if(!oMenu.isDrillDown())
		    oMenu.hideSubMenus();
		var checked=eItem.getAttribute("igChk");
		var checkbox=eItem.getAttribute("igChkBx");
		if(ig.notEmpty(checkbox)) 
		{
			var bCheck=(checked != null) && (checked == '0');
			var postCommand="";
			if(oMenu.fireEvent(oMenu.Events.ItemCheck, "(\"" + oMenu.MenuName + "\",\"" + eItem.id + "\"," + bCheck+")"))
				return;
			
			var checkElement = oItem.getCheckBoxElement();
			if(checked!=null && checked=="1") {
				bCheck=false;
				postCommand=":Uncheck";
				if(checkElement.tagName == "IMG")
					checkElement.src = oMenu.UncheckedImage;
				else
				if(checkElement.tagName == "SPAN")
					checkElement.innerHTML = "";
				eItem.setAttribute("igChk", "0");
			}
			else {
				if(checkElement.tagName == "IMG")
					checkElement.src = oMenu.CheckedImage;
				else
				if(checkElement.tagName == "SPAN") {
				    if(ig_csom.IsIE)
					    checkElement.innerHTML = "a";
					else
					    checkElement.innerHTML = "*";
				}
				bCheck=true;
				postCommand=":Check";
				eItem.setAttribute("igChk", "1");
			}
			
			if(oMenu.NeedPostBack)	{
				__doPostBack(oMenu.UniqueId,eItem.id+postCommand);
			}
			oMenu.updateItemCheck(eItem.id, bCheck);
			ig_startClick=false;
			if(ig_menuPopup != null) {
				var oSubMenu = igmenu_getSubMenuById(oMenu, ig_menuPopup.id);
				oSubMenu.dismiss();
				ig_menuPopup = null;
			}
			var igFrame = eItem.getAttribute("igFrame");
			if(igUrl!=null) {
				ig.navigateUrl(igUrl,igFrame);
			}
			oItem.hover();
			return;
		}
		if(oMenu.fireEvent(oMenu.Events.ItemClick, "(\"" + oMenu.MenuName + "\",\"" + eItem.id + "\")"))
			return;
			
		if(!oMenu.isDrillDown())
    		igmenu_mouseout(evnt);
		
		igmenu_getSubMenu(eItem).igCurrentChild = null;
		igmenu_hideDropDowns(false);
		if(ig_menuPopup != null) {
			var oSubMenu = igmenu_getSubMenuById(oMenu, ig_menuPopup.id);
			oSubMenu.dismiss();
			ig_menuPopup = null;
		}
		ig_startClick = false;
		var igFrame = eItem.getAttribute("igFrame");
		if(igUrl!=null) {
			ig.navigateUrl(igUrl, igFrame);
			return;
		}
		if(oMenu.NeedPostBack)	{
			__doPostBack(oMenu.UniqueId, eItem.id + ":MenuClick");
			return;
		}
	}
}

var ig_inMenu = false;
var ig_MouseX;
var ig_MouseY;
// private - Handles the global mouse down event
function igmenu_pageMouseDown(evnt) {
	if(evnt != null) {
		ig_MouseX = evnt.clientX;
		ig_MouseY = evnt.clientY;
	}
	if(ig_inMenu == true) 
		return;		
	ig_startClick = false;
	ig_inMenu = false;		
	if(ig_oActiveMenu != null) {
		ig_oActiveMenu.TopHoverStarted = false;
		ig_oActiveMenu.hideSubMenus();
	}
	igmenu_hideDropDowns(false);
	if(ig_menuPopup != null) {
		var oSubMenu = igmenu_getSubMenuById(ig_oActiveMenu, ig_menuPopup.id);
		oSubMenu.dismiss();
		ig_menuPopup = null;
	}
}

// private - Handles mouse selection for the menu
function igmenu_selectStart(evnt) {
	if(evnt == null)
	   evnt = window.event;
	ig_cancelEvent(evnt);
	return false;	
}

// private - Displays a popup menu in the appropriate position
function igmenu_showMenu(name, evnt, x, y) {
	if(ig_menuPopup != null) 
	{
		if(ig_oActiveMenu)
		{
			ig_oActiveMenu.hideSubMenus();
			var oSubMenu = igmenu_getSubMenuById(ig_oActiveMenu, ig_menuPopup.id);
			oSubMenu.dismiss();
			//ig_oActiveMenu.hideSubMenu(ig_menuPopup);
		}
		ig_menuPopup = null;
		igmenu_hideDropDowns(false);
	}
	var eItem = igmenu_getElementById(name + "_MainM");
	if(eItem == null) {
		alert("Popup menu name is not valid")
		return;
	}
	var oMenu = igmenu_getMenuById(name);
	if(oMenu == null)
		return;
	var oSubMenu = igmenu_getSubMenuById(oMenu, name + "_MainM");
	if(evnt == null) {
		if(ig_csom.IsIE)
			evnt = window.event;
		else
			evnt = window.Event;
	}
	if(eItem != null) {
		if(x && y) {
			eItem.parentNode.style.left = x;
			eItem.parentNode.style.top = y;	
		}
		else {
			if(ig_csom.IsIE) {
				
				y = evnt.y - 2 + document.body.scrollTop + document.body.parentNode.scrollTop;	
				x = evnt.x - 2 + document.body.scrollLeft + document.body.parentNode.scrollLeft;
			}
			else {
				y = ig_MouseY - 2 + document.body.scrollTop + document.body.parentNode.scrollTop;	
				x = ig_MouseX - 2 + document.body.scrollLeft + document.body.parentNode.scrollLeft;
			}
			var src = ig_csom.getSourceElement(evnt);
			if(src) 
			{
				var parent = src;
				var tmpX = x;
				var tmpY = y;
				var sets = false;
				while (parent != null && parent.tagName != "BODY")
				{
					if(parent.style.position=="relative") 
					{
						sets = true;
						tmpX = 0;
						tmpY = 0;
					}
					tmpX += parent.offsetLeft;
					tmpY += parent.offsetTop;
					
					parent = parent.offsetParent;
				}
				if(sets) {
					x += tmpX;
					y += tmpY;
				}
			}
			var parent = eItem;
			while (parent != null) {
				if(parent.style.position=="absolute" ) {
					if(parent.offsetParent == null)
						break;
					x -= parent.offsetParent.offsetLeft;
					y -= parent.offsetParent.offsetTop;
					break;
				}
				parent = parent.offsetParent;
			}

			eItem.parentNode.style.top  = y.toString() + "px";	
			eItem.parentNode.style.left = x.toString() + "px";
		}
		ig_menuPopup = eItem;

		if(!ig_csom.IsIE55Plus && oMenu.HideDropDowns) 
			igmenu_hideDropDowns(true);
		
		oSubMenu.___display(null, true);
		oSubMenu._visible = true;
		ig_menuPopup.menuObject = oMenu;
		ig_oActiveMenu = oMenu;			
	}
}

// private - Initializes an Item object with properties and method references
function ig_CreateMenuItem(eItem)
{
	if(!ig_WebUIElement.prototype.isPrototypeOf(ig_MenuItem.prototype))
    {
        ig_MenuItem.prototype = new ig_WebUIElement();
    
		ig_SubMenu.prototype.getOwnerItem = function() {
			return(this._ownerItem);
		}

		ig_MenuItem.prototype.getElement = function () {
			return this.element;
		}
		
		ig_MenuItem.prototype.getMenuId = function () {
			var menuName = this.element.id;
			var strArray = menuName.split("_");
			menuName = strArray[0];
			return menuName;
		}
		
		ig_MenuItem.prototype.__displaySubMenu = function(oChildSubMenu) {
			clearTimeout(igmenu_clearMenuTimerId);
			var callback = igmenu_createCallback(this.__display, this, oChildSubMenu);
			igmenu_displayMenuTimerId = setTimeout(callback, this.WebMenu.ExpandEffects.Delay);
		}
	    
		ig_MenuItem.prototype.__display = function(oChildSubMenu) {
			oChildSubMenu.show(this);
		}

		ig_MenuItem.prototype._getTextElement = function ()
		{
			var i = 1;
			if(ig_csom.IsIE)
				i = 0;
			if(this.WebMenu.isHorizontal() && this.isTopLevelItem())
				return this.element.rows[0].cells[1].childNodes[0];
			else
				return this.element.rows[0].cells[1].childNodes[i];
		}
		
		ig_MenuItem.prototype.getText = function ()
		{
			var element = this._getTextElement();
			var text = element.innerHTML;
			text = text.replace("<U>", "");
			text = text.replace("</U>", "");
			text = text.replace("<u>", "");
			text = text.replace("</u>", "");
			text = text.replace(/&amp;/g, "&");
			return text;
		}

		ig_MenuItem.prototype.setText = function(text) {
			var element = this._getTextElement();
			element.innerHTML = text;
		}	
		
		ig_MenuItem.prototype.isSeparator = function() {
			var igSeparator = this.element.getAttribute("igSep");
			return (igSeparator != null && igSeparator.length > 0) ? true : false;
		}
		
		ig_MenuItem.prototype.isTopLevelItem = function() {
			var igTop = this.element.getAttribute("igTop");
			return (igTop != null && igTop.length > 0) ? true : false;
		}
		
		ig_MenuItem.prototype.isEnabled = function() {
			return this.getEnabled();
		}

		ig_MenuItem.prototype.getSelected = function() {
			return this._selected;
		}
		ig_MenuItem.prototype.setSelected = function(bSelected) {
			this._selected = bSelected;
		}
		
		ig_MenuItem.prototype.getOwnerSubMenu = function() {
			var eSubMenu = this.element;
			while(true)  {
				if(eSubMenu.getAttribute("submenu") == '1') {
					return igmenu_getSubMenuById(this.WebMenu, eSubMenu.id);
				}
				else
					eSubMenu = eSubMenu.parentNode;
			}
			return null;
		}

		ig_MenuItem.prototype.getChildSubMenuId = function() {
			var submenuId = this.element.getAttribute("igChildId");
			if(ig.isEmpty(submenuId)) 
				return null;
			else 
				return submenuId;
		}

		ig_MenuItem.prototype.getChildSubMenu = function() {
			var submenuId = this.getChildSubMenuId();
			if(submenuId != null) {
				var oSub = igmenu_submenuArray[submenuId];
				if(oSub)
					return oSub;	
				var eSub = igmenu_getElementById(submenuId);
				if(!eSub)
					return null;
				oSub = ig_CreateSubMenu(this.WebMenu, eSub);
				igmenu_submenuArray[submenuId] = oSub;
				oSub.setOwnerItem(this);
				return oSub;
			}
			return null;
		}

		ig_MenuItem.prototype.getTag = function() {
			var igTag = this.element.getAttribute("igTag");
			return (igTag != null && igTag.length > 0) ? igTag : null;
		}
		ig_MenuItem.prototype.setTag = function(text) {
			this.element.setAttribute("igTag", text);
		}
		ig_MenuItem.prototype.getHoverClass = function() {
			return this.element.getAttribute("igHov")
		}
		ig_MenuItem.prototype.setHoverClass = function(hoverClass) {
			this.element.setAttribute("igHov", hoverClass)
		}
		ig_MenuItem.prototype.getEnabled = function() {
			if(this.element != null) {
				return(this.element.getAttribute("igDisabled") ? false : true);
			}
		}
		
		ig_MenuItem.prototype.setEnabled = function(enabled) {
			var oMenu = this.WebMenu;
			if(enabled == true) {
				if(this.isEnabled())
					return;
				this.element.removeAttribute("igDisabled");
				var oldClass=this.element.getAttribute("oldEnabledClass");
				if(oldClass == null && oMenu.DefaultItemClass != null)
					this.element.className = oMenu.DefaultItemClass;
				else				
					this.element.className = oldClass;
				this.element.removeAttribute("oldEnabledClass");
			}
			else {
				if(!this.isEnabled())
					return;
				var disabledClass = oMenu.DisabledClass;
				this.element.setAttribute("igDisabled", "1");			
				if(this.element.className != oMenu.HoverClass && 
					this.element.className != this.element.getAttribute("igHov"))
				{
					var oldClass = this.element.className;	
					this.element.setAttribute("oldEnabledClass", oldClass);
				}
				this.element.className = disabledClass;
			}
		}
		
		ig_MenuItem.prototype.getTargetFrame = function() {
			var frame = this.element.getAttribute("igFrame");
			if(ig_csom.notEmpty(frame))
				return frame;
			else
			if(ig_csom.notEmpty(this.WebMenu.TargetFrame)) {
				return this.WebMenu.TargetFrame;
			}
			else
				return null;
		}
		ig_MenuItem.prototype.setTargetFrame = function(frame) {
			this.element.setAttribute("igFrame", frame)
		}
		ig_MenuItem.prototype.getTargetUrl = function() {
			var url = this.element.getAttribute("igUrl");
			if(ig_csom.notEmpty(url))
				return url;
			else
			if(ig_csom.notEmpty(this.WebMenu.TargetUrl)) {
				return this.WebMenu.TargetUrl;
			}
			else
				return null;
		}
		ig_MenuItem.prototype.setTargetUrl = function(url) {
			this.element.setAttribute("igUrl", url)
		}
		ig_MenuItem.prototype.getNextSibling = function() {
			var e = this.element.parentNode;
			var attr = e.getAttribute("igitem");
			while(attr == null || attr.length == 0) {
				e = e.parentNode;
				attr = e.getAttribute("igitem");
			}
			
			e = e.nextSibling;
			while(e && e.tagName != "TR" && e.tagName != "TD")
				e = e.nextSibling;
			var item = null;
			if(e) {	
				if(e.style.display == "none")
					return;
				if(e.tagName == "TR")
					item = igmenu_getItemById(e.firstChild.firstChild.id);
				if(e.tagName == "TD")
					item = igmenu_getItemById(e.firstChild.id);
			}
			return item;
		}
		ig_MenuItem.prototype.getPrevSibling = function() {
			var e = this.element.parentNode;
			var attr = e.getAttribute("igitem");
			while(attr == null || attr.length == 0) {
				e = e.parentNode;
				attr = e.getAttribute("igitem");
			}
			e = e.previousSibling;
			while(e && e.tagName != "TR" && e.tagName != "TD")
				e = e.previousSibling;
			var item = null;
			if(e) {	
				if(e.tagName == "TR")
					item = igmenu_getItemById(e.firstChild.firstChild.id);
				if(e.tagName == "TD")
					item = igmenu_getItemById(e.firstChild.id);
			}
			return item;
		}

		ig_MenuItem.prototype.getFirstChild = function() {
			var item=null;
			item=igmenu_getItemById(this.element.id+"_1");
			if(item==null) {
				if(this.getChildSubMenuId()) {
					var index = 2;
					while(item==null && index<100) {
						item=igmenu_getItemById(this.element.id+"_"+index);
						index++
					}
				}
			}
			return item;
		}
		ig_MenuItem.prototype.getParent = function() {
			var item = null;
			if(this.getLevel() == 0)
				return null;
			var itemName = this.element.id.split("_")
			if(itemName.length > 1)
			{
				var parentName = this.element.id.substr(0,this.element.id.length-itemName[itemName.length-1].length-1);
				item = igmenu_getItemById(parentName);
			}
			return item;
		}
		ig_MenuItem.prototype.getItems = function() {
			var itemAr = new Array();
			var itemCount = 0;
			var item = this.getFirstChild();
			while(item)	{
				itemAr[itemCount++] = item;
				item = item.getNextSibling();
			}
			return itemAr;
		}
		ig_MenuItem.prototype.getChecked = function() {
			var item = this.element;
			var checked = item.getAttribute("igChk");
			var checkbox = item.getAttribute("igChkBx");
			if(checkbox != null && checkbox.length>0) 
				if(checked != null && checked.length>0 && checked == '1') 
					return true;
			return false;			
		}
		ig_MenuItem.prototype.setChecked = function(bChecked) {
			var ms = igmenu_getMenuByItemId(this.element.id);
			var item = this.element;
			var checkbox = item.getAttribute("igChkBx");
			if(checkbox == null || checkbox.length == 0) 
				return;

			var topItem = item.getAttribute("igTop");
			var checkElement = this.getCheckBoxElement();

			if(!bChecked) {
				if(checkElement.tagName == "IMG")
					checkElement.src = ms.UncheckedImage;
				else
				if(checkElement.tagName == "SPAN")
					checkElement.innerHTML = "";
				item.setAttribute("igChk", "0");
			}
			else {
				if(checkElement.tagName == "IMG")
					checkElement.src = ms.CheckedImage;
				else
				if(checkElement.tagName == "SPAN")
				{
					if(ig_csom.IsIE)
						checkElement.innerHTML = "a";
					else
						checkElement.innerHTML = "*";
				}
				item.setAttribute("igChk", "1");
			}
			ms.updateItemCheck(this.element.id,bChecked);
		}
		
		ig_MenuItem.prototype.getCheckBoxElement = function() {
			if(this.WebMenu.isHorizontal() && this.isTopLevelItem())
				return this.element.rows[0].cells[0].firstChild;
			else
				return this.element.rows[0].cells[0].firstChild;
		}
		
		ig_MenuItem.prototype.focus = function() {
			this.element.hideFocus = true;
			try {	
				this.element.focus();
			}
			catch (e) {
				//debugger;
				//alert( "focus exception");
			}
			
		}
		
		ig_MenuItem.prototype.getAccessKey = function () {
			if(this.element.tagName == "TR"){
				var i = 0;
				var td;
				if(ig_csom.IsIE)
					td = this.element.childNodes[0];
				else
					td = this.element.childNodes[1];
				while(td.childNodes[i] != null) {
					if(td.childNodes[i].tagName == "DIV") {
						return  td.accessKey;
					}
					i++;
				}
				return "";
			}	
			else
				return this.element.accessKey;
		}

		ig_MenuItem.prototype.activate = function() {
		    var igFrame = this.element.getAttribute("igFrame");
		    var igUrl= this.element.getAttribute("igUrl");

		    if(igUrl!=null) {
			    ig.navigateUrl(igUrl, igFrame);
			    return;
		    }
			ig_startClick=true;
			igmenu_mouseup(null, this.element); 
		}
		
		ig_MenuItem.prototype.getLevel = function () {
			var itemName=this.element.id.split("_")
			if(itemName.length>1)
			{
				return itemName.length - 2;			
			}
		}
		ig_MenuItem.prototype.getIndex = function () {
			var index=0;
			var itemName=this.element.id.split("_");
			if(itemName.length>1)
			{
				index = parseInt(itemName[itemName.length-1]);
				return index-1;
			}
		}
			// private - displays menu item using the hover styles
		ig_MenuItem.prototype.hover = function()
		{
			if(this._hovered) {
				this.focus();
				return;
			}
			this._hovered = true;
			
			var oOwnerSubMenu = this.getOwnerSubMenu();
			var oHoverItem = oOwnerSubMenu.getHoveredItem();
			if(oHoverItem && oHoverItem != this)
				oHoverItem.unhover();
			oOwnerSubMenu.setHoveredItem(this);
				

			if(!this.isEnabled()) 
				return;
			var oMenu = this.WebMenu;
			var hoverClass = this.element.getAttribute("igHov");
			clearCurrentMenu = false;
			if(oMenu.fireEvent(oMenu.Events.ItemHover,"(\""+ oMenu.MenuName + "\",\""+ this.element.id + "\", true)"))
				return;
			if(hoverClass == null || hoverClass.length == 0) {
					hoverClass = oMenu.HoverClass;
			}

			this.__offsetLeft = null;
			if(ig_csom.IsIE && this.getLevel() > 0) {
				this.__offsetLeft = this.element.cells[2].offsetLeft;
			}

			var mergeStyles = oMenu.MergeStyles;
			if(this.isTopLevelItem() && (oMenu.isHorizontal() && (oMenu.isRichClient() || oMenu.isXPClient()) && 
				oMenu.TopHoverStarted) && oMenu.TopSelectedClass.length > 0) {
					hoverClass = oMenu.TopSelectedClass;
					var topHover = this.element.getAttribute("igHov");
					if(this.element.className != "TopHover" && this.element.className != hoverClass)
						this.element.igClass = this.element.className;
					if(!ig.isEmpty(hoverClass)) {
						if(mergeStyles) {
							this.element.className += " " + hoverClass;
						}
						else
							this.element.className = hoverClass;
					}
			}
			else {
				if(!ig.isEmpty(this.element.className)) {
					if(this.element.className.indexOf(hoverClass) != -1)
						return;
					this.element.igClass = this.element.className;
				}
				if(!ig.isEmpty(hoverClass)) {
					if(mergeStyles)
						this.element.className += " " + hoverClass;
					else
						this.element.className = hoverClass;
				}
			}
			
			this.__offsetLeft = null;
			if(this.__offsetLeft != null) {
				var e = this.element.cells[2];
				if(e.firstChild && (e.offsetLeft != this.__offsetLeft)) {
					e.firstChild.style.marginLeft = 1;
				}
			}

			this.focus();
			
			var hoverimage = this.element.getAttribute("ighovimage");
			if(!ig.isEmpty(hoverimage)) {
				var imgElem = this.getImageElement(eItem);
				if(imgElem != null) {
					this.element.setAttribute("igoldhovimage", imgElem.src);
					imgElem.src = hoverimage;
				}
			}
		}
		// private - displays the item using non-hover styles
		ig_MenuItem.prototype.unhover = function (bFireEvent) {
			var oMenu = this.WebMenu;
			if(bFireEvent && oMenu.fireEvent(oMenu.Events.ItemHover,"(\""+oMenu.MenuName+"\",\""+this.element.id+"\", false)"))
				return;
		
			if(!this._hovered)
				return;

			if(!this.isEnabled()) 
				return;
				
			this._hovered = false;

			var prevClass = this.element.getAttribute("igPrevClass");
			if(prevClass == null) {
				if(this.element.igClass != null)
					this.element.className = this.element.igClass;
				else
					this.element.className = "";
			}
			else {
				this.element.className = prevClass;
			}

			if(this.__offsetLeft != null) {
				var e = this.element.cells[2];
				if(e.firstChild)
					e.firstChild.style.marginLeft = 0;
			}
			
			var hoverimage = this.element.getAttribute("igoldhovimage");
			if(hoverimage != null && hoverimage.length > 0) {
				var imgElem = this.getImageElement(this.element);
				if(imgElem != null) {
					imgElem.src = hoverimage;
				}
			}
		}

		// private - obtain the element containing the item image tag
		ig_MenuItem.prototype.getImageElement = function () {
			var e = null;
			if(this.isTopLevelItem() && this.MenuTarget == 1)
				e = this.element.childNodes[0];
			else
				e = this.element.rows[0].cells[0].firstChild;
			if(e==null || e.tagName!="IMG")
				return null;
			return e;
		}
	}
    return new ig_MenuItem(eItem);
}   
   
function ig_MenuItem(eItem)
{
	this.element=eItem;
	this.Element=eItem;
	this.Id = eItem.id;
	this.WebMenu = igmenu_getMenuByItemId(this.Id);
	this._selected = false;
}        

function igmenu_initHandlers(oMenu, oItems) {
	for(var i = 0; i < oItems.length; i++) {
		var oItem = oItems[i];
		var eItem = oItem.Element;
		if(eItem._old)
			continue;
		eItem._old = true;
		ig_shared.addEventListener( eItem, "selectstart", igmenu_selectStart, true);
		if(oMenu.isDrillDown()) {
			ig_shared.addEventListener( eItem, "mouseover", igmenu_podmouseover, true);
			ig_shared.addEventListener( eItem, "mouseout", igmenu_podmouseout, true);
			ig_shared.addEventListener( eItem, "mousedown", igmenu_podmousedown, true);
			ig_shared.addEventListener( eItem, "mouseup", igmenu_podmouseup, true);
			ig_shared.addEventListener( eItem, "keydown", igmenu_keyboard.itemKeydown, true);
			ig_shared.addEventListener( eItem, "focus", igmenu_keyboard.onitemfocus, true);
		}
		else {
			ig_shared.addEventListener( eItem, "mouseover", igmenu_mouseover, true);
			ig_shared.addEventListener( eItem, "mouseout", igmenu_mouseout, true);
			ig_shared.addEventListener( eItem, "mousedown", igmenu_mousedown, true);
			ig_shared.addEventListener( eItem, "mouseup", igmenu_mouseup, true);
			ig_shared.addEventListener( eItem, "keydown", igmenu_keyboard.itemKeydown, true);
			ig_shared.addEventListener( eItem, "focus", igmenu_keyboard.onitemfocus, true);
		}
	}
}

function igmenu_ExecuteItem()
{
	ig_startClick=true;
	igmenu_mouseup(); //evt, oItemElement);
}

function igmenu_pageUnload(){
	if(ig_csom.IsIE55Plus)
	{
		ig_delete(igmenu_panelArray);
		for(var id in igmenu_array)
			igmenu_array[id].base = null;
		ig_delete(igmenu_array);
		ig_delete(igmenu_itemArray);
		ig_delete(igmenu_submenuArray);
	}
}

var igmenu_keyboard = new menu_Keyboard();
function menu_Keyboard() {
	var _this = this;
	this.onfocus = function(evnt) {
		var eSubMenu = ig.getSourceElement(evnt);
		var oMenu = igmenu_getMenuByItemId(eSubMenu.id);	
		var oSubMenu = igmenu_getSubMenuById(oMenu, eSubMenu.id);	
		var oItem = _this.getFirstKbItem(oSubMenu);
		if(oItem) {
			_this.showActive(oItem);
			_this.showAndActivateSubMenu(oItem);
		}		
	}

	this.onitemfocus = function(evnt) {
	    if(evnt.altKey == false)
	        return;
		var eItem = ig_fromEvent(evnt);
		var oItem = igmenu_getItemById(eItem.id);
		var oMenu = oItem.WebMenu;
		var oSubMenu = oItem.getChildSubMenu();
		if(oItem && oItem.getLevel() == 0 && oMenu.MenuTarget == 1) {
			var oChildItem = oItem.getFirstChild();		
			if (oChildItem)				
				_this.showAndActivateSubMenu(oItem);
			else
				oItem.activate();
		}		
	}

	this.itemKeydown = function(evnt) {
		if(evnt.keyCode == 18)
			return;
		var eItem = ig_fromEvent(evnt);
		var oItem = igmenu_getItemById(eItem.id);
		var oMenu = oItem.WebMenu;
		var oSubMenu = oItem.getChildSubMenu();
		var oOwnerSubMenu = oItem.getOwnerSubMenu();
		
		if ((evnt.keyCode >= 48 && evnt.keyCode <= 57)	|| // 0 - 9
				(evnt.keyCode >= 65 && evnt.keyCode <= 122)) // A - Z a - z
			return(_this.processAccessKey(oMenu, oItem, evnt.keyCode));
		
		var iLevel = oItem.getLevel();
		var oNextItem;
		if(oMenu.isDrillDown()) 
		{
			switch(evnt.keyCode) {
				case 32 : // space
					var oChildItem = oItem.getFirstChild();		
					if (oChildItem)				
						_this.showAndActivateSubMenu(oItem);
					else
						oItem.activate();
				break;
				case 9 : // tab
					return;		
					break;	
				case 37 : // left arrow
				case 27 : // escape
					oNextItem = oItem.getParent();
					if(oNextItem) {
						oItem.unhover();
						if (oOwnerSubMenu) {
							oldOnEnd = oMenu.viewport.animate.onEnd;
       						oMenu.viewport.animate.onEnd = function() {
       							oNextItem.focus();
       							oMenu.viewport.animate.onEnd = oldOnEnd;
       						}
							oOwnerSubMenu.drillDownUnDisplay();
						}
					}
					else {
						oItem.unhover();
					}
					break;
				case 38 : // up arrow
					oNextItem = _this.getNextKbItem(oItem, -1);
					_this.showActive(oNextItem);  
					break;
				case 39 : // right arrow
					var oChildItem = oItem.getFirstChild();		
					if (oChildItem)				
						_this.showAndActivateSubMenu(oItem);
					break;
				case 40 : // down arrow			
					oNextItem = _this.getNextKbItem(oItem, 1);
					_this.showActive(oNextItem);
				break;	
			}
		}
		else 
		{
			var isHorizontal = oMenu.isHorizontal();
			var bTopHorizontal = isHorizontal && iLevel == 0;
					
			switch(evnt.keyCode) {
				case 32 : // space
					var oChildItem = oItem.getFirstChild();		
					if (oChildItem)				
						_this.showAndActivateSubMenu(oItem);
					else
						oItem.activate();
				break;
				case 9 : // tab
					oMenu.hideSubMenus();		
				break;	
				case 37 : // left arrow
					if (isHorizontal && (iLevel == 0 || iLevel == 1)) {	
						_this.showNextTopKbItem(oItem, -1);
						return;
					}
					// fall thru
				case 27 : // escape
						oNextItem = oItem.getParent();
						if(oNextItem) {
							if (oOwnerSubMenu) {
								oOwnerSubMenu.dismiss();
							}
							_this.showActive(oNextItem); 
						}
						else {
							oItem.unhover();
						}
				break;
				case 38 : // up arrow
					if (bTopHorizontal)
					{
						oSubMenu = oItem.getChildSubMenu();
						if(oSubMenu) 
							oSubMenu.show(oItem);												
					}
					else {
						oNextItem = _this.getNextKbItem(oItem, -1);
						_this.showActive(oNextItem);  
					}
					break;
				case 39 : // right arrow
					if (bTopHorizontal) {
						_this.showNextTopKbItem(oItem, 1)
					}
					else {
						var oChildItem = oItem.getFirstChild();		
						if (oChildItem)				
							_this.showAndActivateSubMenu(oItem);
						else
						if(isHorizontal) {
							_this.showNextTopKbItem(oItem, 1);
						}
					}
				break;
				case 40 : // down arrow			
					if (bTopHorizontal) {										
						if (!oItem.getFirstChild())
							_this.showActive(oItem);
						else {				
							_this.showAndActivateSubMenu(oItem);
						}
					}
					else {
						oNextItem = _this.getNextKbItem(oItem, 1);
						_this.showActive(oNextItem);
					}
				break;	
			}
		}	
		ig_cancelEvent();		
	}

	this.showActive = function(oItem)	{
		if(oItem) {
			var oOwner = oItem.getOwnerSubMenu();
			oOwner.dismissDescendantSubMenus();
			oItem.hover();
		}
	}		

	this.showNextTopKbItem = function(oItem, iDirection) {
		while(oItem.getLevel() > 0)
			oItem = oItem.getParent();
		var oNextItem = _this.getNextKbItem(oItem, iDirection);
		if(oNextItem) {
			//var useAnimation = constUseAnimation;
			//constUseAnimation = false;
			var oChildItem = oNextItem.getFirstChild();		
			if (oChildItem)				
				_this.showAndActivateSubMenu(oNextItem);
			else
				_this.showActive(oNextItem);
			//constUseAnimation = useAnimation;
		} 
	}
	
	this.showAndActivateSubMenu = function(oItem) {
		var oSubMenu = oItem.getChildSubMenu();
		var oOwner = oItem.getOwnerSubMenu();
		if(!oSubMenu)
			return;
			
		if(oItem.WebMenu.isDrillDown()) {
			oldOnEnd = oItem.WebMenu.viewport.animate.onEnd;
			var oChild = _this.getFirstKbChild(oItem);
			if(oChild) {
       			oItem.WebMenu.viewport.animate.onEnd = function() {
       				oChild.hover();
       				oItem.WebMenu.viewport.animate.onEnd = oldOnEnd;
       			}
       		}
		    oSubMenu.drillDownDisplay(oOwner, oItem);
		}
		else {
			oOwner.dismissDescendantSubMenus(oSubMenu);
			oItem.WebMenu.TopHoverStarted = true;
			oSubMenu.show(oItem);	
			oItem.WebMenu.TopHoverStarted = false;	
			var oChild = _this.getFirstKbChild(oItem);
			if(oChild) {
				oChild.hover();	
			}						
			
		}
	}
	
	this.getFirstKbChild = function(oItem) {
		var firstChild = oItem.getFirstChild();
		if(!firstChild.getEnabled() || firstChild.isSeparator())
			firstChild = _this.getNextKbItem(firstChild);
		return firstChild;
	}
	
	this.getFirstKbItem = function(oSubMenu) {
		var items = oSubMenu.getItems();
		for(var i = 0; i < items.length; i++) {
			var item = items[i];
			if(item.getEnabled() || !item.isSeparator())
				return item;
		}
		return null;
	}

	this.getNextKbItem = function(oItem, bForward) {
		var oNextItem = oItem;
		if(!bForward)
			bForward = 1;
		do {
			if(bForward == 1) {
				oNextItem = oNextItem.getNextSibling();
				if(!oNextItem) {
					if (oItem.getLevel()==0)
						oNextItem = oItem.WebMenu.getItems();
					else
						oNextItem = oItem.getParent().getItems();
					if (oNextItem) 
						oNextItem = oNextItem[0];																								
				}
			}
			else {
				oNextItem = oNextItem.getPrevSibling();
				if(!oNextItem) {
					if (oItem.getLevel()==0)
						oNextItem = oItem.WebMenu.getItems();
					else
						oNextItem = oItem.getParent().getItems();						
					if (oNextItem) 
						oNextItem = oNextItem[oNextItem.length-1];
				}
			}
			if(oNextItem == oItem)
				return null;
		}
		while(oNextItem && (!oNextItem.getEnabled() || oNextItem.isSeparator()));
		return oNextItem;
	}
	
	this.accessKeyMatch = function(oItem, keyCode) {
		var accessKey = oItem.getAccessKey();
		if (ig.notEmpty(accessKey)) {
			if (accessKey.toLocaleLowerCase().charCodeAt(0) == keyCode || 
				accessKey.toLocaleUpperCase().charCodeAt(0) == keyCode)
				if(oItem.getEnabled() && !oItem.isSeparator()) 
					return true;
		}
		return false;
	}
	
	this.processAccessKey = function(oMenu, oItem, keyCode)	{	
		var accessItem  = this.processChildAccess(oMenu, oItem, keyCode);
		if(accessItem == null) {
			accessItem = this.processParentAccess(oMenu, oItem, keyCode);
		}
		if(accessItem != null) {
			var oOwnerSubMenu = accessItem.getOwnerSubMenu();
			var oChildSubMenu = accessItem.getChildSubMenu();
			if(oChildSubMenu) {
				_this.showAndActivateSubMenu(accessItem);
			}
			else {
				accessItem.activate();	
				oOwnerSubMenu.dismissDescendantSubMenus();
			}
		}	
		
	}
		
	this.processChildAccess = function(oMenu, oItem, keyCode) {	
		var oItems = oItem.getItems();
		var oChild = null;
		var accessKey;
		for (var i = 0 ; i < oItems.length ; i++) {
			oChild = oItems[i];
			if(this.accessKeyMatch(oChild, keyCode)) 
				return oChild;
		}
		return null;
	}
	
	this.processParentAccess = function(oMenu, oParentItem, keyCode) {
		if(oParentItem == null)
			return null;
		var	oNextItem = oParentItem.getNextSibling();	
		while(true) {
			if(oNextItem == null) {
				var oItems;
				if (oParentItem.getLevel() == 0) 
					oItems = oMenu.getItems();
				else
					oItems = oParentItem.getParent().getItems();
				oNextItem = oItems[0];	
			}
			if(_this.accessKeyMatch(oNextItem, keyCode)) 
				return oNextItem;
			if(oParentItem == oNextItem)
				return this.processParentAccess(oMenu, oParentItem.getParent(), keyCode);
			oNextItem = oNextItem.getNextSibling();	
		}
	}
}


// private - constructor for the menu scroll object
function igmenu_scroller(subMenuElement) {
	this.subMenu = subMenuElement;
	this.fullHeight = this.subMenu.offsetHeight;
	this.active = false;
	this.oMenu = igmenu_getMenuByItemId(this.subMenu.id);
	
	// private - implements the showing and hiding of submenus
	this.addScroll = function() {
		this.scrollDiv = this.subMenu.firstChild
		this.table = this.scrollDiv.firstChild;
		this.topDiv = window.document.createElement("TABLE");
		this.topDiv.topDiv = true;
		this.bottomDiv = window.document.createElement("TABLE");
		this.bottomDiv.bottomDiv = true;
		
		var tbody = window.document.createElement("TBODY");
		var tr = window.document.createElement("TR");
		var td = window.document.createElement("TD");
		this.topDiv.appendChild(tbody);
		tbody.appendChild(tr);
		tr.appendChild(td);
		td.align = 'center';
		this.topDiv.style.width = this.subMenu.offsetWidth;
		
		tbody = window.document.createElement("TBODY");
		tr = window.document.createElement("TR");
		td = window.document.createElement("TD");
		this.bottomDiv.appendChild(tbody);
		tbody.appendChild(tr);
		tr.appendChild(td);
		td.align = 'center';
		this.bottomDiv.style.width = this.subMenu.offsetWidth;
		
		// add image elements to top and bottom divs
		var img = window.document.createElement("IMG");
		img.src = this.oMenu.ScrollImageTopDisabled;
		this.topDiv.cells[0].appendChild(img);
		img = window.document.createElement("IMG");
		img.src = this.oMenu.ScrollImageBottom;
		this.bottomDiv.cells[0].appendChild(img);
		
		// top and bottom divs to the document object
		this.subMenu.insertBefore(this.topDiv, this.subMenu.firstChild);
		this.subMenu.appendChild(this.bottomDiv);
		
		if(!ig_csom.isEmpty(this.oMenu.DefaultItemClass)) {
			this.topDiv.className = this.oMenu.DefaultItemClass;
			this.bottomDiv.className = this.oMenu.DefaultItemClass;
		}
		// Add mouse event listeners to top and bottom divs
		if(this.topDiv._old)
			return;
		this.topDiv._old = true;
		ig_csom.addEventListener(this.topDiv, "mouseover", igmenu_onScrollMouseOver, true);
		ig_csom.addEventListener(this.bottomDiv, "mouseover", igmenu_onScrollMouseOver, true);
		ig_csom.addEventListener(this.topDiv, "mouseout", igmenu_onScrollMouseOut, true);
		ig_csom.addEventListener(this.bottomDiv, "mouseout", igmenu_onScrollMouseOut, true);
	}
	this.setScrollHeight = function(scrollHeight) {
		if(scrollHeight >= 0)
			this.scrollDiv.style.height=scrollHeight;
	}
	this.getScrollHeight = function() {
		return this.scrollDiv.style.height;
	}
	this.show = function() {
		if(this.topDiv.cells[0].innerHTML.indexOf(this.oMenu.ScrollImageTopDisabled) == -1)
			this.topDiv.cells[0].innerHTML = "<img src='" + this.oMenu.ScrollImageTopDisabled + "'>"
		if(this.bottomDiv.cells[0].innerHTML.indexOf(this.oMenu.ScrollImageBottom) == -1)
			this.bottomDiv.cells[0].innerHTML = "<img src='" + this.oMenu.ScrollImageBottom + "'>"

		this.scrollDiv.scrollTop = 0;
		this.scrollDiv.style.overflow="hidden";
		this.topDiv.style.display="";
		this.topDiv.style.visibility="visible";
		this.bottomDiv.style.display="";
		this.bottomDiv.style.visibility="visible";
		this.active = true;
	}
	this.hide = function() {
		this.topDiv.style.display="none";
		this.topDiv.style.visibility="hidden";
		this.bottomDiv.style.display="none";
		this.bottomDiv.style.visibility="hidden";
		this.subMenu.style.height = this.fullHeight;
		this.scrollDiv.style.overflow="";
		this.active = false;
	}	
	this.addScroll();
}

// private - handles mouse over for scrollable submenus
var igmenu_scrollTimerId;
var igmenu_oScrollMenu;
function igmenu_onScrollMouseOver(evnt) {
	var e = ig_fromEvent(evnt);
		
	ig_inMenu = true;	
	clearTimeout(igmenu_clearMenuTimerId);
	clearCurrentMenu = false;
	var oMenu = igmenu_getMenuByItemId(e.parentNode.id);
	igmenu_oScrollMenu = igmenu_getSubMenuById(oMenu, e.parentNode.id);
	
	
	var hoverClass = oMenu.HoverClass;
	if(oMenu.MergeStyles)
		e.className += " " + hoverClass;
	else
		e.className = hoverClass;

	if(e.topDiv) {
		igmenu_oScrollMenu.scrollInc = -6;
	}
	else
	if(e.bottomDiv) {
		igmenu_oScrollMenu.scrollInc = 6;
	}
	clearInterval(igmenu_scrollTimerId);
	igmenu_scrollTimerId = setInterval(igmenu_onMenuScroll, 30);
}

// private - handles mouse out for scrollable submenus
function igmenu_onScrollMouseOut(evnt) {
	var e = ig_fromEvent(evnt);

	var oMenu = igmenu_getMenuByItemId(e.parentNode.id);
	var itemClass = oMenu.DefaultItemClass;
	e.className = itemClass;

	clearInterval(igmenu_scrollTimerId);
	igmenu_oScrollMenu = null;
	clearCurrentMenu = true;
	clearTimeout(igmenu_clearMenuTimerId);
	igmenu_clearMenuTimerId = setTimeout('clearTimerExpired()', oMenu.ExpandEffects.RemovalDelay);
}

// private - handles scrolling for scrollable submenus
function igmenu_onMenuScroll() {
	if(igmenu_oScrollMenu != null) {
		var scrollDiv = igmenu_oScrollMenu.scroller.scrollDiv;
		
		// save the current scrollTop position
		var oldValue = scrollDiv.scrollTop;
		
		// increment the scrollTop property of the scrollDiv
		scrollDiv.scrollTop += igmenu_oScrollMenu.scrollInc;

		// get references to the top and bottom divs and the menu object
		var topDiv = igmenu_oScrollMenu.scroller.topDiv;
		var bottomDiv = igmenu_oScrollMenu.scroller.bottomDiv;
		var oMenu = igmenu_oScrollMenu.scroller.oMenu;

		// If at the top, display disabled up arrow
		if(scrollDiv.scrollTop == 0) {
			if(topDiv.innerHTML.indexOf(oMenu.ScrollImageTopDisabled) == -1) {
				topDiv.cells[0].innerHTML = "<img src='" + oMenu.ScrollImageTopDisabled + "'>"
				clearInterval(igmenu_scrollTimerId);
			}
		}
		else {
			if(topDiv.cells[0].innerHTML.indexOf(oMenu.ScrollImageTop) == -1)
				topDiv.cells[0].innerHTML = "<img src='" + oMenu.ScrollImageTop + "'>"
		}
	
		// If at the bottom, display disabled down arrow
		if(oldValue == scrollDiv.scrollTop && oldValue > 0) {
			if(bottomDiv.cells[0].innerHTML.indexOf(oMenu.ScrollImageBottomDisabled) == -1) {
				bottomDiv.cells[0].innerHTML = "<img src='" + oMenu.ScrollImageBottomDisabled + "'>"
				clearInterval(igmenu_scrollTimerId);
			}
		}
		else {
			if(bottomDiv.cells[0].innerHTML.indexOf(oMenu.ScrollImageBottom) == -1)
				bottomDiv.cells[0].innerHTML = "<img src='" + oMenu.ScrollImageBottom + "'>"
		}
		
	}
}


if(typeof(ig_csom)!="undefined" && ig_csom.IsIE)
	ig_csom.addEventListener(window, "unload", igmenu_pageUnload, true);

// public - Obtains a menu Item object using its id
function igmenu_getItemById(itemId) {
	var oItem = igmenu_itemArray[itemId];
	if(oItem)
		return oItem;	
	var item=igmenu_getElementById(itemId);
	if(!item)
		return null;
	oItem = new ig_CreateMenuItem(item);
	igmenu_itemArray[itemId] = oItem;
	return oItem;
}

// public - Obtains a menu object using its id
function igmenu_getMenuById(menuId) {
	return igmenu_array[menuId];
}

// public - returns the menu object for the Item Id
function igmenu_getMenuByItemId(itemId) {
	var mn = igmenu_getMenuNameByItemId(itemId);  
	return igmenu_array[mn];
}

// public - returns the submenu object for the SubMenu Id
function igmenu_getSubMenuById(oMenu, submenuId) {
	var oSub = igmenu_submenuArray[submenuId];
	if(oSub)
		return oSub;	
	var eSub=igmenu_getElementById(submenuId);
	if(!eSub)
		return null;
	oSub = ig_CreateSubMenu(oMenu, eSub);
	igmenu_submenuArray[submenuId] = oSub;
	return oSub;
}

// public - returns the Menu Name (mn) from an itemId
function igmenu_getMenuNameByItemId(itemId) {
   var menuName = itemId;
   var strArray = menuName.split("_");
   menuName = strArray[0];
   return menuName;
}

// public - Retrieves an element by its tag name in a browser independant way
function igmenu_getElementById(id) {
	return ig_csom.getElementById(id);
}

// private - 1.0 compatibility function for hiding select boxes
var igmenu_displayMenu = null;
function igmenu_callDisplayMenu(bShow, id) {
	if(igmenu_displayMenu != null)
		igmenu_displayMenu(bShow, id);
}

// private - hides all dropdown select controls for the document.
var ig_hidden=false;
function igmenu_hideDropDowns(bHide) { 
	 if(igmenu_dropDowns == null)
		return;
     if(bHide){
		if(ig_hidden)
			return;
		ig_hidden = true;
         for (i=0; i<igmenu_dropDowns.length;i++)
                 igmenu_dropDowns[i].style.visibility='hidden';
     }
     else {
         for (i=0; i<igmenu_dropDowns.length;i++){
                 igmenu_dropDowns[i].style.visibility='visible';
         }
         ig_hidden = false;
     }
}

// private - Performed on page initialization
function igmenu_initialize() {
	ig_csom.addEventListener(document, "mousedown", igmenu_pageMouseDown, true);
}

function ig_CreateSubMenu(oMenu, eSub)
{
	if(!ig_WebUIElement.prototype.isPrototypeOf(ig_SubMenu.prototype))
    {
        ig_SubMenu.prototype = new ig_WebUIElement();
    
		ig_SubMenu.prototype.getOwnerItem = function() {
			return(this._ownerItem);
		}
		
		ig_SubMenu.prototype.setOwnerItem = function(oItem) {
			this._ownerItem = oItem;
		}
		
		ig_SubMenu.prototype.getHoveredItem = function() {
			return(this._hoveredItem);
		}
		ig_SubMenu.prototype.setHoveredItem = function(oItem) {
			this._hoveredItem = oItem;
		}

		ig_SubMenu.prototype.getSelectedItem = function() {
			return(this._selectedItem);
		}
		ig_SubMenu.prototype.setSelectedItem = function(oItem) {
			this._selectedItem = oItem;
		}
		
		ig_SubMenu.prototype.getParentSubMenu = function() {
			if(this._ownerItem)
				return this._ownerItem.getOwnerSubMenu();
			return null;
		}
		
		ig_SubMenu.prototype.getItems = function() {
			if(!this.getParentSubMenu())
				return this.WebMenu.getItems();
			var itemAr = new Array();
			var id = this.element.id;
			id = id.substring(0, id.length-1);
			id += "_";
			var index = 1;
			while(true) {
				var e = document.getElementById(id + index.toString());
				if(e)
					itemAr[index-1] = igmenu_getItemById(id + index.toString());
				else
					break;
				index++;
			}
			return itemAr;
		}

		ig_SubMenu.prototype.getCurrentDisplayedSubMenu = function() {
			return this._currentDisplayedSubMenu
		}
		
		ig_SubMenu.prototype.show = function(oParentItem) {
			if(this._visible)
				return;
			oMenu = this.WebMenu;	
			this._ownerItem = oParentItem;
			
			if(oMenu.isHorizontal() && oParentItem.isTopLevelItem()) {
				oMenu.initSubMenuRoot(this);
				this._visible = true;
				this.___display(oParentItem, false);
				oParentItem.hover();
			}
			else
			if (oParentItem.getLevel() == 0) {
				this.oParentSubMenu = igmenu_getSubMenu(oParentItem.element);
				oMenu.initSubMenuRoot(this);
				oParentItem.hover();
				this._visible = true;
				this.___display(oParentItem, true);
			}
			else {
				this.oParentSubMenu = igmenu_getSubMenu(oParentItem.element);
				oParentItem.getOwnerSubMenu().dismissDescendantSubMenus();
				this._visible = true;
				this.___display(oParentItem, true);
			}	
			oParentItem.getOwnerSubMenu()._currentDisplayedSubMenu = this;										
			oParentItem.setSelected(true);										
		} // show
		
		ig_SubMenu.prototype.cancelAnimation = function() {
			if(this.animate)
				 this.animate.cancelAnimation();
			this.eContainer.style.display = "none";
			if(this._currentDisplayedSubMenu != null) 
				this._currentDisplayedSubMenu.cancelAnimation();
		}
		
		ig_SubMenu.prototype.dismiss = function() {
			if(!this._visible)
				return;
			this._visible = false;	
			// 1.0 unhide select elements
			igmenu_callDisplayMenu(false, this.id);
			if(this._currentDisplayedSubMenu) {
				this._currentDisplayedSubMenu.dismiss();
			}
			this._currentDisplayedSubMenu = null;
			
			if(this._selectedItem)
				this._selectedItem.unhover(false);
			this._selectedItem = null;
			
			if(this._hoveredItem)
				this._hoveredItem.unhover(false);
			this._hoveredItem = null;

			if(this.WebMenu.fireEvent(this.WebMenu.Events.SubMenuDisplay,"(\""+ this.WebMenu.MenuName +"\",\""+this.id+"\", false)"))
				return;
				
			if(this.WebMenu.ExpandEffects.Type != 'Slide' || this._popup == true) {
				this.element.style.display = "none";
				this.element.style.visibility = "hidden";
			}
			else {	
   				this.animate = new ig_SlideRevealAnimation();
				this.animate.setElement(this.element);
				this.animate.setContainer(this.element.parentNode);
				this.animate.setRate(AnimationRateEnum.Decelerate);
				this.animate.startPos = 0;
       			if(this._vertical) {
					this.animate.setDirection(AnimationDirectionEnum.Left);
					this.animate.finishPos = -this.element.scrollWidth;
       			}
       			else {
					this.animate.setDirection(AnimationDirectionEnum.Up);
					this.animate.finishPos = -this.element.scrollHeight;
       			}
       			this.animate.onEnd = function() {
       				this.getContainer().style.display = "none";
       			}
				this.animate.play();	
			}
			if((ig_csom.IsIE && !ig_csom.IsMac) && this.transPanel!=null) {
				this.transPanel.hide();
			}	
		} // dismiss
		
		ig_SubMenu.prototype.dismissDescendantSubMenus = function(oPreserve) {
			if(oPreserve && this._currentDisplayedSubMenu == oPreserve)
				return;
			if(this._currentDisplayedSubMenu !=null) {
				this._currentDisplayedSubMenu.dismiss();
			}
		}
		
		ig_SubMenu.prototype.getHeaderText = function() {
			if(this.eHeader) 
				return this.eHeader.rows[0].cells[0].innerHTML;
			return null;
		}
		
		ig_SubMenu.prototype.setHeaderText = function(text) {
			if(this.eHeader) 
				this.eHeader.rows[0].cells[0].innerHTML = text;
		}
		
		ig_SubMenu.prototype.addSlideHeader = function() {
			var eSubMenu = this.element;
			this.eHeader = window.document.createElement("TABLE");
			var tbody = window.document.createElement("TBODY");
			var tr = window.document.createElement("TR");
			var td = window.document.createElement("TD");
			this.eHeader.appendChild(tbody);
			tbody.appendChild(tr);
			tr.appendChild(td);
			td.align = 'center';
			if(!this.eHeader._old)
  		    ig_shared.addEventListener(this.eHeader, "mouseup", igmenu_podheadermouseup, true);
			this.eHeader._old = true;

			td.style.width = "100%";
            if(this.WebMenu.HeaderClass != null && this.WebMenu.HeaderClass.length > 0)
                this.eHeader.className = this.WebMenu.DefaultItemClass + " " + this.WebMenu.HeaderClass;
            else {
    			this.eHeader.style.fontWeight = 800;
			    this.eHeader.style.backgroundColor = "E0E0E0";
			    this.eHeader.style.color = "000000";
			    this.eHeader.style.borderStyle = "outset";
			    this.eHeader.style.borderWidth = 1;
            }

			this.eHeader.style.width = "100%";
			if(this.eHeader.style.height == "")
				this.eHeader.style.height = "22px";
			eSubMenu.insertBefore(this.eHeader, eSubMenu.firstChild);
		}
	} 
	
	ig_SubMenu.prototype.ensureScroller = function(scrollElement, fixedHeight) {
		if(ig_csom.IsIE  && !ig_csom.IsMac) {
			var scrollHeight;
			if(!this.scroller)
				scrollHeight = scrollElement.scrollHeight;
			else
				scrollHeight = this.scroller.table.scrollHeight;
			if(scrollHeight > fixedHeight) { // - 4) {
				if(!this.scroller) {
	 				this.scroller = new igmenu_scroller(this.element);
				}
				this.scroller.show();
				
				var divsHeight = this.scroller.topDiv.offsetHeight + this.scroller.bottomDiv.offsetHeight + 8;
				this.scroller.setScrollHeight(fixedHeight - divsHeight);
				//menuHeight = eContainer.offsetHeight;
			}
			else {
				if(this.scroller) {
					this.scroller.hide();
				}
			}			
		}
	}
	
	ig_SubMenu.prototype.drillDownUnDisplay = function() {
		var oParentSubMenu = this._ownerItem.getOwnerSubMenu();
		
		var scrollElement;
		if(this.WebMenu.hasFrame && oParentSubMenu.element.id.indexOf("_MainM") != -1)
			scrollElement = oParentSubMenu.element.parentNode;
		else
			scrollElement = oParentSubMenu.element;
		
		this.WebMenu.viewport.scroll(this.element, scrollElement, AnimationDirectionEnum.Left, AnimationRateEnum.Linear);
	}
	
	ig_SubMenu.prototype.endScroll = function() {
		
	}
	
	// private - implements the showing and hiding of submenus
	ig_SubMenu.prototype.drillDownDisplay = function(oParentSubMenu, oParentItem) {
		this.WebMenu.initEventHandlers(this, oParentItem);
		var items = this.getItems();
		
		var clientWidth = this.WebMenu.viewport.div.clientWidth;
		var oItem;
		if(items.length > 0) {
			var oItem = items[0];
			oItem.element.style.width = clientWidth;
		}
		
		this.element.style.display = "";
		
		var scrollElement;
		if(this.WebMenu.hasFrame && oParentSubMenu.element.id.indexOf("_MainM") != -1)
			scrollElement = oParentSubMenu.element.parentNode;
		else
			scrollElement = oParentSubMenu.element;
		this.WebMenu.viewport.scroll(scrollElement, this.element, AnimationDirectionEnum.Right, AnimationRateEnum.Linear);
		
		var diff = this.element.parentNode.offsetWidth - clientWidth;
		if(oItem)
			oItem.element.style.width = clientWidth - diff;
		this.element.style.width = clientWidth;
		
		this.ensureScroller(this.element, this.WebMenu.viewport.div.offsetHeight-30);
		if(!this.hasHeader) {
			this.addSlideHeader();
			this.hasHeader = true;
			this.setHeaderText(oParentItem.getText());
		}
		
	}
	
	// private - implements the showing and hiding of submenus
	ig_SubMenu.prototype.___display = function(oParent, vertical) {
		var oMenu = this.WebMenu;
		var eParent = null;
		var eSubMenu = this.element;
		var eContainer = this.element.parentNode;
		var effects = oMenu.ExpandEffects;
		if(oParent) {
			eParent = oParent.element;
			oMenu.initEventHandlers(this, oParent);
		}
		
		
		if(ig_csom.IsIE55Plus && oMenu.HideDropDowns) {	
			if(this.transPanel == null) {	
				this.transPanel = ig_csom.createTransparentPanel();
				igmenu_panelArray[iframeCount] = this.transPanel;
				iframeCount++;
			}
		}
		
		// Call 1.0 function to hide select elements
		igmenu_callDisplayMenu(true, this.id);
		
		if(oMenu.fireEvent(oMenu.Events.SubMenuDisplay,"(\"" + oMenu.MenuName + "\",\"" + this.id + "\", true)"))
			return;

		if(ig_csom.IsMac || (eContainer.style.filter == null)) {
			eContainer.style.display = "";
			eContainer.style.visibility = 'visible';
		}
		else
		if(effects.Type != 'NotSet' && effects.Type != 'Slide') {
			eContainer.style.filter = "progid:DXImageTransform.Microsoft." + effects.Type + 
									"(duration=" + (effects.Duration / 1000) +");"
			if(effects.ShadowWidth > 0)
				eContainer.style.filter += " progid:DXImageTransform.Microsoft.Shadow(Direction=135, Strength=" +
									effects.ShadowWidth+",color=" + effects.ShadowColor + ");"
			if(effects.Opacity < 100)
				eContainer.style.filter += " progid:DXImageTransform.Microsoft.Alpha(Opacity=" + effects.Opacity +");"
			try{
			if(eContainer.filters[0] != null)
	        	eContainer.filters[0].apply();
	       	}catch(ex){}
			eContainer.style.display="";
			eContainer.style.visibility='visible'

			try{
			if(eContainer.filters[0] != null)
				eContainer.filters[0].play();
	       	}catch(ex){}
		}
		else {
			eContainer.runtimeStyle.filter = "";
			try{
			if(effects.ShadowWidth > 0)
				eContainer.runtimeStyle.filter = "progid:DXImageTransform.Microsoft.Shadow(Direction=135, Strength=" +
									effects.ShadowWidth + ",color=" + effects.ShadowColor + ");"
			if(opacity < 100)
				eContainer.runtimeStyle.filter += " progid:DXImageTransform.Microsoft.Alpha(Opacity=" + effects.Opacity + ");"
	       	}catch(ex){}
			eContainer.style.visibility='visible';
			eContainer.style.display="";
		}

		// set submenu position	
		var pageWidth	= document.body.clientWidth;		
		var menuWidth	= (ig_csom.IsNetscape6) ? eContainer.offsetWidth : eContainer.clientWidth;		
		var pageHeight	= document.body.clientHeight;
		if(document.documentElement)
		{
			if(document.documentElement.clientHeight != 0 )
				pageHeight = Math.max(pageHeight, document.documentElement.clientHeight);
			if(document.documentElement.clientWidth != 0 )
				pageWidth = Math.max(pageWidth, document.documentElement.clientWidth);
		}
		
		if(ig_csom.IsSafari && window.innerHeight)
			pageHeight = window.innerHeight;
			
		var menuHeight	= eContainer.offsetHeight;
		
		var scrollTop	= document.body.scrollTop + document.body.parentNode.scrollTop;
		var scrollLeft	= document.body.scrollLeft + document.body.parentNode.scrollLeft;
		var menuX		= 0;
		var menuY		= 0;
		
		var useAnimation = (effects.Type == 'Slide') ? true : false;
		
		if(oParent == null) { // popup menu
			menuX = eContainer.offsetLeft;	
			menuY = eContainer.offsetTop;
			this._popup = true;	
		}
		else 
		if(vertical) { // display next to vertical menu
			var eSubParent = this._ownerItem.getOwnerSubMenu().element;
			menuY = igmenu_getTopPos(eParent);	
			//menuY = eSubParent.parentNode.offsetTop;	
			if(ig_csom.IsSafari) {
				menuY += eParent.childNodes[0].offsetTop;
			}
			if(this.oParentSubMenu.scroller != null && this.oParentSubMenu.scroller.active) {
				menuY = menuY - this.oParentSubMenu.scroller.scrollDiv.scrollTop;
			}
			
			if(oMenu.CurrentLeftHandDisplay == false){
				menuX = igmenu_getLeftPos(eSubParent)+ eSubParent.offsetWidth;
				//if(document.documentElement == null || document.documentElement.clientHeight == 0)
				//	menuX -= 4; 
				
				if(eParent.offsetWidth == 0 && ig_csom.IsSafari)
				{
					var temp = eParent.offsetParent;
					var width = 0;
					while(temp != null && width == 0) {
						width += temp.offsetWidth;
						temp = temp.offsetParent;
					}
					menuX += width;
				}
				
			}
			else
				menuX = igmenu_getLeftPos(eParent) - menuWidth; 
					
			var switched = oMenu.CurrentLeftHandDisplay != oMenu.LeftHandDisplay; 
			// Check which way to align the menu
			if(oMenu.CurrentLeftHandDisplay == false && !switched) { // align right
				if((menuX + menuWidth) > pageWidth + scrollLeft) {
						oMenu.CurrentLeftHandDisplay = true; // change to left
					menuX = igmenu_getLeftPos(eParent) - menuWidth; 
				}
			}
			else
			if(oMenu.CurrentLeftHandDisplay == true && !switched) { // aligned left
				if((menuX < 0)) { 
					oMenu.CurrentLeftHandDisplay = false; // change to right
					menuX = igmenu_getLeftPos(eParent) + eParent.offsetWidth; 
				}
			}
		}
		else { // display under horizontal menu
			var eSubParent = this._ownerItem.getOwnerSubMenu().element;
			menuX = igmenu_getLeftPos(eParent); 
			menuY = igmenu_getTopPos(eSubParent) + eSubParent.offsetHeight;
		}

		if(ig_csom.IsIE  && !ig_csom.IsMac) {
			var scrollHeight;
			if(!this.scroller)
				scrollHeight = eSubMenu.firstChild.firstChild.scrollHeight;
			else
				scrollHeight = this.scroller.table.scrollHeight;
			if(scrollHeight > pageHeight - 4) {
				if(!this.scroller) {
	 				this.scroller = new igmenu_scroller(eSubMenu);
				}
				this.scroller.show();
				
				useAnimation = false;
				var divsHeight = this.scroller.topDiv.offsetHeight + this.scroller.bottomDiv.offsetHeight + 8;
				this.scroller.setScrollHeight(pageHeight - divsHeight);
				menuHeight = eContainer.offsetHeight;
			}
			else {
				if(this.scroller) {
					this.scroller.hide();
				}
			}			
		}
		
		if(menuX + menuWidth > pageWidth + scrollLeft)
			menuX = pageWidth - menuWidth + scrollLeft - 8;
		if(menuX < scrollLeft)
			menuX = scrollLeft;
		
		if(menuY + menuHeight > pageHeight + scrollTop)
			menuY = pageHeight - menuHeight + scrollTop - 8;
		if(menuY < scrollTop)
			menuY = scrollTop;
		
		eContainer.style.top = menuY.toString() + "px";
		eContainer.style.left = menuX.toString() +"px";

		this.eContainer = eContainer;

		if(ig_csom.IsIE55Plus && this.transPanel != null) {	
			this.transPanel.setPosition(eContainer.offsetTop, eContainer.offsetLeft, eContainer.offsetWidth, eContainer.offsetHeight);
			this.transPanel.show();
		} 
		
       	if(useAnimation) {
   			this.animate = new ig_SlideRevealAnimation();
			this.animate.setElement(eSubMenu);
			this.animate.setContainer(eSubMenu.parentNode);
			this.animate.setRate(AnimationRateEnum.Decelerate);
			this.animate.finishPos = 0;
       		if(vertical) {
       	   		this._vertical = true;
				this.animate.setDirection(AnimationDirectionEnum.Right);
				eSubMenu.style.visibility = "";
				eSubMenu.style.display = "";
				this.animate.startPos = -eSubMenu.scrollWidth ;
       		}
       		else {
				this.animate.setDirection(AnimationDirectionEnum.Down);
				this.animate.startPos = -eSubMenu.scrollHeight;
       		}
			this.animate.play();
		}
		else {
			eSubMenu.style.display = "";
			eSubMenu.style.visibility = "visible";
		}		
	}
    return new ig_SubMenu(oMenu, eSub);
}   
   
function ig_SubMenu(oMenu, eSub)
{
    this.init(eSub);
	this.WebMenu = oMenu;
	this._currentDisplayedSubMenu = null;
	this._hoveredItem = null;
	this._selectedItem = null;
	this._ownerItem = null;
	this._visible = false;
}        

function ig_WebUIElement(element)
{
	if(arguments.length > 0){
		this.init(id, element);
	}
}

ig_WebUIElement.prototype.init=function(element){
	this.id = element.id;
	this.element = element;
	ig_all[this.id] = this;
}

ig_WebUIElement.prototype.constructor = ig_WebUIElement;
ig_WebUIElement.prototype.getElement = function(){
	return this.element;
}

hideAnimationComplete = function(element, params) {
	params.tr.style.visibility = "hidden";
	params.tr.style.display = "none";
}

igmenu_createCallback = function(method, context, param1, param2, param3 ) {
	
	return function() {
		method.apply(context, [param1, param2, param3]);
	}
}
