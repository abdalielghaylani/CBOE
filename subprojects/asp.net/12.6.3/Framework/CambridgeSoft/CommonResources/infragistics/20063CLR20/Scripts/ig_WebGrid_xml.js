 /*
  * Infragistics WebGrid CSOM Script: ig_WebGrid_xml.js
  * Version 6.3.20063.53
  * Copyright(c) 2001-2006 Infragistics, Inc. All Rights Reserved.
  */

// ig_WebGrid_xml.js
// Infragistics UltraWebGrid Script 
// Copyright (c) 2001-2006 Infragistics, Inc. All Rights Reserved.
function igtbl_onReadyStateChange()
{
	var gn;
	var g=this._control;
	gn=g.Id;
	var xmlHttp=this._xmlHttpRequest;
	if(xmlHttp.readyState==4)
	{
		g.responseText=xmlHttp.responseText;
		g.ReqType=this.ReqType;
		var r=this.RowToQuery;
		g.RowToQuery=r;
		if (g.XmlResponseObject)
		{
			var a = g.XmlResponseObject;
			g.XmlResponseObject = null;
			igtbl_dispose(a);
		}
		var xmlRespObj = new Object();
		g.XmlResponseObject=xmlRespObj;
		xmlRespObj.ResponseStatus=g.eError.Ok;
		xmlRespObj.ReqType=g.ReqType;
		xmlRespObj.Tag=null;
		xmlRespObj.XmlResp=null;
		xmlRespObj.Cancel=false;
		if(g.responseText=="")
			xmlRespObj.ResponseStatus=g.eError.LoadFailed;
		else if(ig_csom.IsIE)
		{
			var start=g.responseText.indexOf("<xml");
			var end=g.responseText.indexOf("</xml>")+6;
			g.XmlResp.loadXML(g.responseText.substr(start,end-start));
			var node=g.XmlResp.selectSingleNode("xml/UltraWebGrid/XmlHTTPResponse");
			if(node)
			{
				xmlRespObj.StatusMessage = unescape(node.selectSingleNode("StatusMessage").text);
				xmlRespObj.Tag = unescape(node.selectSingleNode("Tag").text);
				xmlRespObj.XmlResp=g.XmlResp;
				if(node.getAttribute("ResponseStatus")!=0)
					xmlRespObj.ResponseStatus=g.eError.LoadFailed;
				xmlRespObj.Cancel=(node.selectSingleNode("Cancel").text=="true");	
				var srlNode=node.selectSingleNode("ServerRowsLength");
				if(srlNode)
				{
					xmlRespObj.ServerRowsLength=igtbl_parseInt(srlNode.text);
					if(g.ReqType==g.eReqType.MoreRows
						|| g.ReqType==g.eReqType.Filter
					)
						g.RowsServerLength=xmlRespObj.ServerRowsLength;
				}
			}
			else
			{
				xmlRespObj.StatusMessage=g.responseText;
				xmlRespObj.ResponseStatus=g.eError.LoadFailed;
				var de=g.getDivElement();
				de.removeAttribute("oldST");
				de.removeAttribute("noOnScroll");
			}
		}
		else
		{	
		
		
		
			 
			var parsedString="var "+g.Id+"_XmlHttpResponseObj=";	
			var start = g.responseText.indexOf(parsedString);
			var jsonObj="";
			if (start>0)
			{
				var startCurlyBrace = 0;				
				start=start+parsedString.length;
				do
				{
					if (g.responseText[start]=="{")
						startCurlyBrace++;
					if (g.responseText[start]=="}")
						startCurlyBrace--;
					jsonObj+=g.responseText[start];
					start++;			
				}while(startCurlyBrace>0 && g.responseText[start]!=null );
				var xmlHttpRspObj=eval( "("+jsonObj+");" );
								
				xmlRespObj.StatusMessage = unescape(xmlHttpRspObj.StatusMessage);
				xmlRespObj.Tag = unescape(xmlHttpRspObj.Tag);
				xmlRespObj.XmlResp=g.XmlResp;
				if(xmlHttpRspObj.ResponseStatus!=0 && xmlHttpRspObj.ResponseStatus!="Success")
					xmlRespObj.ResponseStatus=g.eError.LoadFailed;
				xmlRespObj.Cancel=xmlHttpRspObj.Cancel=="true";	
				var srlNode=xmlHttpRspObj.ServerRowsLength;
				if(srlNode!==null && typeof(srlNode)!="undefined")
				{
					xmlRespObj.ServerRowsLength=igtbl_parseInt(srlNode);
					if(g.ReqType==g.eReqType.MoreRows)
						g.RowsServerLength=xmlRespObj.ServerRowsLength;
				}
				xmlHttpRspObj=null;
			}
			
			parsedString="var "+g.Id+"_FilterListObject=";
			start = g.responseText.indexOf(parsedString);
			if (start>0)
			{
				jsonObj="";
				var startCurlyBrace = 0;				
				start=start+parsedString.length;
				do
				{
					if (g.responseText[start]=="{")
						startCurlyBrace++;
					if (g.responseText[start]=="}")
						startCurlyBrace--;
					jsonObj+=g.responseText[start];
					start++;			
				}while(startCurlyBrace>0 && g.responseText[start]!=null );
				var xmlHttpRspObj=eval( "("+jsonObj+");" );
				xmlRespObj._filterInformation = xmlHttpRspObj;
			}	
			
		}
		if(g.fireEvent(g.Events.XmlHTTPResponse,[g.Id,r?r.Element.id:"",g.XmlResponseObject]) || xmlRespObj.ResponseStatus==g.eError.LoadFailed)
		{
			if(g.Events.XmlHTTPResponse[1]==1)
				g.NeedPostBack=false;
			g.ReadyState=g.eReadyState.Ready;
			g.Error=g.eError.LoadFailed;
			if(g.ReqType==g.eReqType.UpdateRow)
				g.RowToQuery._generateUpdateRowSemaphore(true);
			if(g._servingXmlHttp)
				igtbl_dispose(g._servingXmlHttp);
			g._hidePI();
			this.RowToQuery=null;
			return;
		}
		if(g.Events.XmlHTTPResponse[1]==1)
			g.NeedPostBack=false;
		switch(g.ReqType)
		{
			case g.eReqType.ChildRows:
				igtbl_requestChildRowsComplete(gn);
				break;
			case g.eReqType.MoreRows:
				igtbl_requestMoreRowsComplete(gn);
				break;
			case g.eReqType.Refresh:
			case g.eReqType.Sort:
				igtbl_requestSortComplete(gn);
				igtbl_updateFooters(gn);
				break;
			case g.eReqType.UpdateRow:
				igtbl_requestUpdateRowComplete(gn);
				break;
			case g.eReqType.Page:
				igtbl_requestPageComplete(gn);
				igtbl_updateFooters(gn);
				break;
			case g.eReqType.Scroll:
				igtbl_requestScrollComplete(gn);
				break;
			case g.eReqType.FilterDropDownFill:				
				igtbl_requestFilterDropDownFillComplete(gn);
				break;
			case g.eReqType.Filter:
				igtbl_requestFilterComplete(gn);
				igtbl_updateFooters(gn);
				break;
			default:
				igtbl_requestComplete(gn
					,xmlHttp
				);
				break;
		}
		g.ReadyState=g.eReadyState.Ready;
		g.fireEvent(g.Events.AfterXmlHttpResponseProcessed,[g.Id]);
		if(g.ReqType!=g.eReqType.None)
		{
			g.RowToQuery=null;
			this.RowToQuery=null;
		}
		g.ReqType=g.eReqType.None;
		g.Error=g.eError.Ok;
		if(g._servingXmlHttp)
			igtbl_dispose(g._servingXmlHttp);
		g._hidePI();
		this.RowToQuery=null;
	}
}
function igtbl_requestChildRowsComplete(gn)
{
	var g=igtbl_getGridById(gn);
	var r=g.RowToQuery;
	if(!ig_csom.IsIE)
	{
		
		g._innerObj.innerHTML=g.responseText.substring(0);
		var rows=g._innerObj.getElementsByTagName("tr");
		var i=0,row=rows[i];		
				
		if (r.DataKey)
			
			while(row && (unescape(row.getAttribute(igtbl_litPrefix+"datakey"))!=r.DataKey || row.id.length<r.Element.id.length))
				row=rows[++i];
		else	
			while(row && row.id!=r.Element.id)
				row=rows[++i];
								
		if(row && row.nextSibling && row.nextSibling.getAttribute("hiddenRow"))
		{
			
			if(r.Element.nextSibling)
				r.Element.parentNode.insertBefore(row.nextSibling,r.Element.nextSibling);
			else
				r.Element.parentNode.appendChild(row.nextSibling);
			r.HiddenElement=r.Element.nextSibling;			
			
			var childRows=igtbl_getChildRows(gn,r.Element);
			r.ChildRowsCount=igtbl_rowsCount(childRows);
			r.VisChildRowsCount=igtbl_visRowsCount(childRows);
			
			r.Rows=new igtbl_Rows(null,r.Band.Grid.Bands[r.Band.Index+1],r);
			r.FirstChildRow=r.Rows.getRow(0);
		}
	}
	else
	{
		var rowsNode=g.XmlResp.selectSingleNode("form");
		if(!rowsNode)
			rowsNode=g.XmlResp;
		
		var selStr="xml/UltraWebGrid/Rs";
		for(var i=0;i<=r.Band.Index;i++)
		{
			var cband=g.Bands[i];
			if(cband.SortedColumns && cband.SortedColumns.length>0)
				for(var j=0;j<cband.SortedColumns.length;j++)
				{
					var col=igtbl_getColumnById(cband.SortedColumns[j]);
					if(col.IsGroupBy)
						selStr+="/Group/Rs"
					else
						break;
				}
			selStr+="/R/Rs"
		}
		rowsNode=rowsNode.selectSingleNode(selStr);
			
		if(rowsNode!=null)
		{
			r.Node.appendChild(rowsNode);			
			if(!r.Rows)
				r.Rows=new igtbl_Rows(r.Node.selectSingleNode(
						"Rs"
						),r.Band.Grid.Bands[r.Band.Index+1],r);
			else
			{
				
				r.Rows.Node=rowsNode;
				r.Rows.SelectedNodes=rowsNode.selectNodes(
					"R"
					);
				if(r.Rows.SelectedNodes.length==0)
					r.Rows.SelectedNodes=rowsNode.selectNodes("Group");
				r.Rows.length=r.Rows.SelectedNodes.length;
			}
			
			var rowIslandFilters=null;
			if (rowsNode)rowIslandFilters=eval(rowsNode.getAttribute("columnFilters"));
			if (rowIslandFilters)
			{	
				_igtbl_processServerPassedColumnFilters(rowIslandFilters,g);
			}
			r.prerenderChildRows();
			r.Rows.render();
		}
	}
	r._setExpandedComplete(true);
}

function igtbl_onScrollXml(evnt,gn)
{
	var g=igtbl_getGridById(gn);
	g.event=evnt;
	var de=g.getDivElement();
	if(g.noMoreRows)
		return;
	if(de && de.scrollHeight==de.scrollTop+de.clientHeight && g.RowsRange>0
		&& (g.XmlLoadOnDemandType==0
		|| g.XmlLoadOnDemandType==4
		)
		|| (g.XmlLoadOnDemandType==1 && de.parentNode.childNodes[1].scrollHeight<=de.scrollTop+de.clientHeight + 50)
	)
	{
		if(g.RowsServerLength>g.Rows.length)
		{
			
			if(g.ReadyState==0)
			g.invokeXmlHttpRequest(g.eReqType.MoreRows);
			return igtbl_cancelEvent(evnt);
		}
	}
	if(g.XmlLoadOnDemandType==2)
	{
		if(g._vScrTimer)
			window.clearTimeout(g._vScrTimer);
		
		if(!g.fireEvent(g.Events.XmlVirtualScroll,[g.Id,Math.floor(de.scrollTop/g.getDefaultRowHeight())]))
			g._vScrTimer=window.setTimeout("igbtl_vScrollGrid('"+gn+"')",g.VirtualScrollDelay);
	}
}

function igbtl_vScrollGrid(gn)
{
	var g=igtbl_getGridById(gn);
	delete g._vScrTimer;
	g.invokeXmlHttpRequest(g.eReqType.Scroll);
}

function igtbl_requestMoreRowsComplete(gn)
{
	var g=igtbl_getGridById(gn);
	if(ig_csom.IsIE)
	{
		var node=g.XmlResp.selectSingleNode("form");
		if(!node)
			node=g.XmlResp;
		node=node.selectSingleNode("xml/UltraWebGrid/Rs");
		g.Node.appendChild(node);
		var strTransform=g.Rows.applyXslToNode(node,g.Rows.SelectedNodes.length);
		if(strTransform)
		{
			g._innerObj.innerHTML="<table>"+strTransform+"</table>";
    		g.Node.removeChild(node);
			var nodes=node.selectNodes("R");
			if(nodes.length==0)
				nodes=node.selectNodes("Group");
			g.Rows.length+=nodes.length;
			g.RowsRetrieved+=nodes.length;
			for(var i=0;i<nodes.length;i++)
			{
				g.Rows.Node.appendChild(nodes[i]);
				g.Rows.Element.appendChild(g._innerObj.firstChild.rows[0]);
			}
			g.Rows.SelectedNodes=g.Rows.Node.selectNodes("R");
			if(g.Rows.SelectedNodes.length==0)
				g.Rows.SelectedNodes=g.Rows.Node.selectNodes("Group");
			g.alignDivs(0,true);
		}
	}
	else
	{
		g._innerObj.innerHTML=g.responseText.substring(0);
		var rows=g._innerObj.getElementsByTagName("tr");
		var i=0,row=rows[i];
		
		
		
		while(row && (!row.id || row.id.lastIndexOf("_mr")==(row.id.length-3) || row.id.lastIndexOf("_gbr")==(row.id.length-4) ) )
			row=rows[++i];
					
		var length=0;
		var pr=g.Rows.getRow(0).Element.parentNode;
		while(row)
		{
			length++;
			var ns=row.nextSibling;
			pr.appendChild(row);
			row=ns;
		}
		if(length>=0)
		{
			g.Rows.length+=length;
			g.RowsRetrieved+=length;
			g.alignDivs(0,true);
		}
	}
	g.Rows.setLastRowId(g.Rows.getRow(g.Rows.length-1).Id);
	if (g.XmlLoadOnDemandType==3)	
		window.setTimeout("_igtbl_getMoreRows('"+g.Id+"');",100);
	if(g.XmlLoadOnDemandType==4)
	{
		g._removeChange("RowsRetrieved",g);
		g._recordChange("RowsRetrieved",g,g.RowsRetrieved);
	}

	g.cancelNoOnScrollTimeout=window.setTimeout("igtbl_cancelNoOnScroll('"+g.Id+"')",100);
}

function igtbl_isArLess(a1,a2)
{
	if(a1.length<a2.length)
		return true;
	if(a1.length>a2.length)
		return false;
	for(var i=0;i<a1.length;i++)
	{
		if(a1[i]<a2[i])
			return true;
		if(a1[i]>a2[i])
			return false;
	}
	return false;
}

function igtbl_sortRowIdsByClctn(rc)
{
	var ar=new Array(),i=0;
	for(var rowId in rc)
	{
		var row=igtbl_getRowById(rowId);
		if(row)
			ar[i++]=row.getLevel();
		else
			ar[i++]=rowId.split('_').slice(1);
	}
	for(var i=0;i<ar.length;i++)
		for(var j=0;j<ar[i].length;j++)
			ar[i][j]=parseInt(ar[i][j],10);
	var sorted=false;
	while(!sorted)
	{
		sorted=true;
		for(var i=0;i<ar.length-1;i++)
			if(igtbl_isArLess(ar[i],ar[i+1]))
			{
				var a=ar[i];
				ar[i]=ar[i+1];
				ar[i+1]=a;
				sorted=false;
			}
	}
	return ar;
}

function _igtbl_PagerRedraw(g)
{
	var node=g.XmlResp.selectSingleNode("form");
	if(!node)
		node=g.XmlResp;
    node=node.selectSingleNode("xml/UltraWebGrid/Pager");
	
    var pager = igtbl_getDocumentElement(g.UniqueID+"_pager");
    if(pager)
    {
	    if(!pager.length)
	    {
		    var oldElem = pager;
		    pager = new Array(1);
		    pager[0] = oldElem;
	    }
	    for (var i=0;i<pager.length;i++)
		    pager[i].innerHTML = unescape(node.getAttribute("Labels"));
	    
	    g.CurrentPageIndex=igtbl_parseInt(node.getAttribute("CurrentPageIndex"));
	    
	    
	    g.PageCount=igtbl_parseInt(node.getAttribute("PageCount"));
	 }
}

function igtbl_requestPageComplete(gn)
{
	var g=igtbl_getGridById(gn);
	if(ig_csom.IsIE)
	{
		var node=g.XmlResp.selectSingleNode("form");
		if(!node)
			node=g.XmlResp;		
		if (node)
		{
			g.clearSelectionAll();
			g.setActiveCell(null);
			g.setActiveRow(null);
			igtbl_requestSortComplete(gn);
			g._recordChange("PageChanged",g,g._pageToGo);
		}
	}
	else if(ig_csom.IsNetscape6)
	{
		g._innerObj.innerHTML=g.responseText.substring(0);
		
		var rows=igtbl_getChildElementById(g._innerObj,gn+"_div").firstChild.tBodies[0].childNodes;
		var rowsLength=rows.length;
		while(g.Rows.Element.childNodes.length>0)
			g.Rows.Element.removeChild(g.Rows.Element.childNodes[0]);
		while(rows.length>0)
			g.Rows.Element.appendChild(rows[0]);

		var arIndex=-1,acColumn=null,acrIndex=-1,aRows=null;
		g.clearSelectionAll();
		g.setActiveRow(null);
		g.setActiveCell(null);
		g.Rows.dispose();
		g.Rows.length=rowsLength;
		g.RowsRetrieved=g.Rows.length;
		if(g._scrElem)
		{
			igtbl_scrollTop(g._scrElem,0);
			if (ig_csom.IsFireFox && g.UseFixedHeaders) 
			{
			    g.alignDivs(g._scrElem.scrollLeft, true); 
			}
			else
			{
			    g.alignDivs();
			}
		}
		else
			igtbl_scrollTop(g.DivElement,0);
		var pager = igtbl_getDocumentElement(g.UniqueID+"_pager");
		g._recordChange("PageChanged",g,g._pageToGo);
		if(pager)
		{
			if(!pager.length)
				pager=[pager];
			var pagerNew=[];
			var tblMain=g._innerObj.childNodes[1];
			while(tblMain && tblMain.id!=g.Id+"_main")
			{
			    var mainTable = igtbl_getChildElementById(tblMain, g.Id+"_main")
			    if(!mainTable)
			    {
			        mainTable = igtbl_getChildElementById(tblMain, gn+"_main")
			    }
			    if(mainTable)
			    {
				    tblMain=mainTable;
				    break;
				}    
				else
				    tblMain=tblMain.nextSibling;
			}
			if(tblMain)
			{
				for(var i=0;i<tblMain.rows.length;i++)
					if(tblMain.rows[i].firstChild.id==g.UniqueID+"_pager")
						pagerNew[pagerNew.length]=tblMain.rows[i].firstChild;
				if(pagerNew.length==pager.length)
					for(var i=0;i<pager.length;i++)
						pager[i].innerHTML = pagerNew[i].innerHTML;
			}
			g.CurrentPageIndex=g._pageToGo;
			delete g._pageToGo;
		}
	}
}
function igtbl_requestScrollComplete(gn)
{
	var g=igtbl_getGridById(gn);
	igtbl_refillXmlGrid(gn);
	var de=g.getDivElement();
	de.removeAttribute("oldST");
	de.removeAttribute("noOnScroll");
}

function igtbl_refillXmlGrid(gn)
{
	var g=igtbl_getGridById(gn);
	if(ig_csom.IsIE)
	{
		var node=g.XmlResp.selectSingleNode("form");
		if(!node)
			node=g.XmlResp;
		if (node)
		{
			g.clearSelectionAll();
			g.setActiveCell(null);
			g.setActiveRow(null);
			igtbl_requestSortComplete(gn);
		}
	}
	else if(ig_csom.IsNetscape6)
	{
		g._innerObj.innerHTML=g.responseText.substring(0);
		var rows=g._innerObj.getElementsByTagName("tbody")[1].childNodes;
		var rowsLength=rows.length;
		while(g.Rows.Element.childNodes.length>0)
			g.Rows.Element.removeChild(g.Rows.Element.childNodes[0]);
		while(rows.length>0)
			g.Rows.Element.appendChild(rows[0]);

		var arIndex=-1,acColumn=null,acrIndex=-1,aRows=null;
		g.clearSelectionAll();
		g.setActiveRow(null);
		g.setActiveCell(null);
		g.Rows.dispose();
		g.Rows.length=rowsLength;
		g.RowsRetrieved=g.Rows.length;
	}
}


function _igtbl_replaceRowIsland(g,node,rows)
{
	rows.Node.parentNode.replaceChild(node,rows.Node);
	rows.Node=node;
	rows.SelectedNodes=node.selectNodes("R");
	// AK 3/9/2006 BR10794: Cannot click on Expansion indicator when XML Paging and GroupingBy a column initially on server side. 
	if(rows.SelectedNodes.length==0)
		rows.SelectedNodes=node.selectNodes("Group");
	var arIndex=-1,acColumn=null,acrIndex=-1,aRows=null;
	if(g.oActiveRow && g.oActiveRow.OwnerCollection==rows)
		arIndex=g.oActiveRow.getIndex();
	if(g.oActiveRow && g.oActiveRow.Band.Index>=rows.Band.Index)
		g.setActiveRow(null);
	if(g.oActiveCell && g.oActiveCell.Row.OwnerCollection==rows)
	{
		acColumn=g.oActiveCell.Column;
		acrIndex=g.oActiveCell.Row.getIndex();
	}
	if(g.oActiveCell && g.oActiveCell.Band.Index>=rows.Band.Index)
		g.setActiveCell(null);
	rows.dispose();
	rows.length=rows.SelectedNodes.length;
	rows.render();
	if(arIndex!=-1)
	{
		
		var r = rows.getRow(arIndex);
		if(r)r.activate();
	}	
	if(acColumn)
	{
		if(acrIndex==-1)
		{
			if(rows.AddNewRow)
				rows.AddNewRow.getCellByColumn(acColumn).activate();
		}
		else if(acrIndex<rows.length)
			rows.getRow(acrIndex).getCellByColumn(acColumn).activate();
	}
	g.RowsRetrieved=rows.length;
	if(rows.Band.Index==0&& g.ReqType!=g.eReqType.Scroll)
	{
		if(g._scrElem)
		{
			igtbl_scrollTop(g._scrElem,0);
			g.alignDivs();
		}
		else
			igtbl_scrollTop(g.DivElement,0);
	}
}

function igtbl_requestSortComplete(gn)
{
	var g=igtbl_getGridById(gn);	
	if(ig_csom.IsIE)
	{	
		var node=g.XmlResp.selectSingleNode("form");
		if(!node)
			node=g.XmlResp;

        _igtbl_PagerRedraw(g);
		node=node.selectSingleNode("xml/UltraWebGrid/Rs");
		if(!node) return;
		var rows=g.Rows;
		if(g.RowToQuery && g.RowToQuery.Rows)
		{
			rows=g.RowToQuery.Rows;
			for(var i=0;i<rows.Band.Index;i++)
				node=node.selectSingleNode("R/Rs")
		}
		if(!node) return;
		_igtbl_replaceRowIsland(g,node,rows);
		
		g.alignDivs(0,true);
	}
	else if(ig_csom.IsNetscape6)
	{
		igtbl_refillXmlGrid(gn);
		if(g._scrElem)
		{
			igtbl_scrollTop(g._scrElem,0);
			g.alignDivs();
		}
		else
			igtbl_scrollTop(g.DivElement,0);
	}
	if(g.XmlLoadOnDemandType==4)
		g._removeChange("RowsRetrieved",g);
}

function igtbl_requestUpdateRowComplete(gn)
{
	var g=igtbl_getGridById(gn);
	var r=g.RowToQuery;
	if(ig_csom.IsIE)
	{
		var node=g.XmlResp.selectSingleNode("form");
		if(!node)node=g.XmlResp;
		node=node.selectSingleNode("xml/UltraWebGrid/XmlHTTPResponse");
		if (node)
		{
			var cellsNode=node.selectSingleNode("R/Cs");
			if(cellsNode)
				for(var i=0;i<cellsNode.childNodes.length;i++)
				{
					var cell=r.getCellFromKey(unescape(cellsNode.childNodes[i].getAttribute(igtbl_litPrefix+"key")));
					if(cell)
					{
						var value=igtbl_getNodeValue(cellsNode.childNodes[i]);
						var oldValue=cell.getNodeValue();
						if(typeof(cell._oldValue)!="undefined")
						{
							delete cell._oldValue;
							g._removeChange("ChangedCells",cell);
						}
						if(value!=oldValue)
						{
							cell.setValue(cell.Column.getValueFromString(value),false);
							g._removeChange("ChangedCells",cell);
						}
					}
				}
		}
	}
	g.fireEvent(g.Events.AfterRowUpdate,[g.Id,r.Element.id]);
	if(g.Events.AfterRowUpdate[1]==1)
		g.NeedPostBack=false;
}
function igtbl_requestFilterDropDownFillComplete(gn)
{
	var g = igtbl_getGridById(gn);
	if(ig_csom.IsIE)
	{
		var node=g.XmlResp.selectSingleNode("form");
		if(!node)node=g.XmlResp;
		node=node.selectSingleNode("xml/UltraWebGrid/FilterCollection");
		if (node)
		{
			var colId = node.getAttribute("ColumnId");
			var parentRowId = node.getAttribute("ParentRowId");
			var column = igtbl_getColumnById(colId);
			var workingList = new Array();
			var childNodeCount = node.childNodes.length;
			for(var i = 0; i < childNodeCount ; i++)
			{
			
				workingList.push([unescape(node.childNodes[i].getAttribute("cond")),unescape(node.childNodes[i].getAttribute("text"))])
			}
			
			var filterPanel=null;
			if ((column.Band.Index==0 || column.RowFilterMode==1) && !column.Band.IsGrouped)
			{
				filterPanel=g.Bands[column.Band.Index]._filterPanels[colId];
			}
			else 
			{
				var row = igtbl_getRowById(parentRowId);
				if(row.GroupByRow)
				{
					parentRowId=parentRowId.replace( "_gr","_t");
				}
				else
				{
					parentRowId=parentRowId.replace( "_r","_t");
				}
				filterPanel=g.Bands[column.Band.Index]._filterPanels[parentRowId][colId];
			}
			filterPanel._afterFilterFilled(g,column,workingList);
		}
	}
	else 
	{
		var xmlResp = g.XmlResponseObject;		
		if(xmlResp)		
		{	
			var filterInfo = xmlResp._filterInformation;
			if(filterInfo)
			{
				var colId =	filterInfo.ColumnId;
				var parentRowId = filterInfo.ParentRowId;
				var column = igtbl_getColumnById(colId);
				var workingList = new Array();
				var childNodeCount = filterInfo.FilterList.length;
				for(var i = 0; i < childNodeCount ; i++)
				{
			    
					workingList.push([unescape(filterInfo.FilterList[i][0]),unescape(filterInfo.FilterList[i][1])])
				}
				
				var filterPanel=null;
				if ((column.Band.Index==0 || column.RowFilterMode==1) && !column.Band.IsGrouped)
				{
					filterPanel=g.Bands[column.Band.Index]._filterPanels[colId];
				}
				else 
				{
					var row = igtbl_getRowById(parentRowId);
					if(row.GroupByRow)
					{
						parentRowId=parentRowId.replace( "_gr","_t");
					}
					else
					{
						parentRowId=parentRowId.replace( "_r","_t");
					}
					filterPanel=g.Bands[column.Band.Index]._filterPanels[parentRowId][colId];
				}
				filterPanel._afterFilterFilled(g,column,workingList);				
			}
			xmlResp._filterInformation=null;
		}
	}
}
function igtbl_requestComplete(gn
	,xmlHttp
)
{
	var g=igtbl_getGridById(gn);
	g.ReqType=g.eReqType.None;
	if(xmlHttp.readyState==4)
		g.ReadyState=g.eReadyState.Ready;
}
function igtbl_requestFilterComplete(gn)
{
	var g=igtbl_getGridById(gn);
	if(ig_csom.IsIE)
	{
		var node=g.XmlResp.selectSingleNode("form");
		if(!node)
			node=g.XmlResp;
        _igtbl_PagerRedraw(g);
        var filterRowIslandInfo=node.selectSingleNode("xml/UltraWebGrid/FilterRowIsland");
        node=node.selectSingleNode("xml/UltraWebGrid/Rs");
		if(!node) return;
		
		var rows=null;
		if (filterRowIslandInfo)
		{
			var row=igtbl_getRowById(filterRowIslandInfo.getAttribute("ParentRowId"));
			var bandIndex=row.Band.Index;
			
			if(!row.GroupByRow)
			{
				while (node && (node.getAttribute("bandNo")-1<=bandIndex))
				{
					node=node.selectSingleNode("R/Rs");
				}
			}
			
			else
			{
				while (node && (node.getAttribute("bandNo")-1<=bandIndex))
				{
					node=node.selectSingleNode("Group/Rs");
				}
			}
				
			if(!node)return;
			rows = row.Rows;
		}
		else
		{
			rows=g.Rows;
		}
		
		_igtbl_replaceRowIsland(g,node,rows);
		var colFilters = node.getAttribute("columnFilters");
		if (colFilters)
		{
			colFilters = eval(colFilters);
			if (colFilters)
			{	
				_igtbl_processServerPassedColumnFilters(colFilters,g);
				_igtbl_setFilterIndicators(colFilters,rows);
			}
		}
	}
	else
	{
		igtbl_requestSortComplete(gn);
	}
}
function igtbl_updateFooters(gn)
{
	var g=igtbl_getGridById(gn);
	var rows=g.Rows;
	if(g.RowToQuery && g.RowToQuery.Rows) rows=g.RowToQuery.Rows;
	if(ig_csom.IsIE)
	{
		var node=rows.Node.lastChild;
		if(!node || node.nodeName!="Footers") return;
		var band=rows.Band;
		for(var i=0;i<band.Columns.length;i++)
		{
			var footerNode=node.childNodes[i];
			if(footerNode)
			{
				var caption=footerNode.getAttribute("caption");
				if(caption)
					rows.setFooterText(band.Columns[i].Key,caption);
			}
			else
				break;
		}
	}
	else if(ig_csom.IsNetscape6 && g._innerObj)
	{
		var footer=g._innerObj.getElementsByTagName("tfoot")[0];
		if(footer)
		{
			var footElem=rows.nextSibling;
			if(!rows.ParentRow && g.StatFooter)
				footElem=g.StatFooter.Element;
			footElem.innerHTML=footer.innerHTML;
		}
	}
}
