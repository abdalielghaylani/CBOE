<%@ LANGUAGE=VBScript %>
<% Option Explicit %>
<% Response.Expires = 0 %>

<% 

Const L_LOOKIN_TEXT = "Look in:"
Const L_NAME_TEXT = "Name"
Const L_SIZE_TEXT = "Size"
Const L_TYPE_TEXT = "Type"
Const L_LASTMODIFIED_TEXT = "Modified"
Const L_SLASH_TEXT = "\"	
Const FIXEDDISK = 2

Dim path, FileSystem, drives, drive, primarydrive

path = Request.Cookies("HTMLA")("LASTPATH")

If path = "" Then
	Set FileSystem=CreateObject("Scripting.FileSystemObject")
	Set drives = FileSystem.Drives

	For Each drive in drives
		primarydrive = drive	
		
		'exit after the first FIXEDDISK if there is one...
		if drive.DriveType = FIXEDDISK then
			Exit For			
		end if
		
	Next
	
	primarydrive = primarydrive & L_SLASH_TEXT

	Response.Cookies("HTMLA")("LASTPATH")=primarydrive
	path = primarydrive
End If
%>

<HTML>
<HEAD>
	<TITLE></TITLE>
	
	<SCRIPT LANGUAGE="JavaScript">
	
	function writeTblOpen(){
			document.write("<TABLE BORDER = 1 CELLPADDING = 1 CELLSPACING = 0  BORDERCOLOR='#CCCCCC'  BORDERCOLORDARK='#CCCCCC' BORDERCOLORLIGHT='#CCCCCC'><TR>");			
	}
	
	function writeTblClose(){
		document.write("</TABLE>");
	}
	
	function writeTblHead(cellwidth,sortcol,sorttype,stringwidth,thestring)
		{
		var i;	
		var quote = '"'
		
		if (isNaN(stringwidth))
		{
			stringwidth = 20;
		}
				
		var writestr = "<TD WIDTH=" + cellwidth + " BORDER = 1  BORDERCOLOR='#CCCCCC' BORDERCOLORDARK='#FFFFFF' BORDERCOLORLIGHT='#000000'><FONT SIZE=1 FACE='HELV'>";
		if (sortcol != "")
		{
			writestr += "<A HREF = 'javascript:sortList("+quote+sortcol+quote+","+quote+sorttype+quote+");'>";
			stringwidth = stringwidth - thestring.length;
			for (i=stringwidth;i>0;i--)
				{
				thestring += "&nbsp;"
				}
			writestr += thestring;		
			writestr += "</A></TD>";
		}
		else
		{
			stringwidth = stringwidth - thestring.length;
			for (i=stringwidth;i>0;i--)
				{
				thestring += "&nbsp;"
				}
			writestr += thestring;		
			writestr += "</TD>";		
		}
	

		document.write(writestr);		
		return;
		}
	</SCRIPT>
</HEAD>

<BODY BGCOLOR="#CCCCCC" LINK="#000000" VLINK="#000000" ALINK="#000000" TOPMARGIN = 5 LEFTMARGIN = 5 onLoad="loadList();">
<FORM NAME="userform" onSubmit="return false;">

<TABLE>
<TR><TD WIDTH = 40><FONT FACE="Helv" SIZE = 1>
	<%= L_LOOKIN_TEXT %>
	</TD>
	<TD>
	<INPUT TYPE="text" NAME="currentPath" VALUE="<%= path %>" SIZE = 50 OnBlur="changeDir(this.value);">
	</TD>
	<TD>
		<A HREF="javascript:upDir();"><IMG SRC="updir.GIF" WIDTH=23 HEIGHT=22 BORDER=0></A>
	</TD>
	</TR>
</TABLE>

<SCRIPT LANGUAGE="JavaScript">
	writeTblOpen();
	writeTblHead(176, "fname", "str", 50,"<%= L_NAME_TEXT %>");	
	writeTblHead(75, "fsize","num", 15,"<%= L_SIZE_TEXT %>");
	writeTblHead(75, "ftype","str", 20,"<%= L_TYPE_TEXT %>");
	writeTblHead(160, "","str", 40, "<%= L_LASTMODIFIED_TEXT %>");	
	writeTblClose();
</SCRIPT>

</FORM>

<SCRIPT LANGUAGE="JavaScript">
	function loadList()
		{
		parent.hlist.location.href = "JSBrwSet.asp?btype=" + top.opener.JSBrowser.browsertype;		
		}
	function redrawList()
		{
		parent.list.location.href = "JSBrwLs.asp";		
		}
		

	function listFuncs()
		{
		this.loadList = loadList;
		this.sortList = sortList;
		this.SetFilter = SetFilter;
		this.changeDir = changeDir;
		this.setPath = setPath;
		this.selIndex = 0;
		this.sortby = "fname";
		this.sortAsc = true;
		this.filterType = "";
		}


	function upDir()
		{
		lastpath = document.userform.currentPath.value;
		uppath = lastpath;
		while (lastpath.indexOf("/") > -1)
			{
			lastpath = lastpath.substring(0,lastpath.indexOf("/")) + "\\" + lastpath.substring(lastpath.indexOf("/")+1,lastpath.length);
			} 		
		if (lastpath.lastIndexOf("\\") == lastpath.length-1)
			{
			lastpath = lastpath.substring(0,lastpath.length-1);
			}
		lastwhack = lastpath.lastIndexOf("\\");
		if (lastwhack > 0)
			{
			uppath = lastpath.substring(0,lastwhack+1);
			}	
		document.userform.currentPath.value = uppath;
		if (lastpath.lastIndexOf(":") == lastpath.length-1)
			{
			lastpath += "\\";
			}
		changeDir(uppath);
		}
	
	function changeDir(newpath)
		{
		newpath.toUpperCase();
		var thispath = "JSBrwSet.asp?btype=" + top.opener.JSBrowser.browsertype + "&path=" + escape(newpath);
		parent.hlist.location.href = thispath;
		return false;
		}
	
	function setPath()
		{		
		top.opener.JSBrowser.currentFile = parent.filter.document.userform.currentFile.value;
		top.opener.JSBrowser.currentPath = document.userform.currentPath.value;		
		top.opener.JSBrowser.BrowserObjSetPath();
		top.location.href = "JSBrwCl.asp";		
		}
	
	function SetFilter(selFilter)
		{
		listFunc.filterType = selFilter.options[selFilter.selectedIndex].value;
		loadList();
		}


	function numOrder(a,b)
		{
		return a[listFunc.sortby]-b[listFunc.sortby];
		}

	function sortList(sortby,sorttype)
		{
		if (sortby != listFunc.sortby)
			{
			listFunc.sortby = sortby;
			listFunc.sortAsc = true;
			}
		else
			{
			listFunc.sortAsc = !listFunc.sortAsc;
			}

		listItem = cachedList[0];
		var num = parseFloat(listItem[sortby]);

		if (sorttype == "str")
			{
			cachedList.sort(sortOrder);
			}
		else
			{ 
			cachedList.sort(numOrder);
			}
			
		if (!listFunc.sortAsc)
			{
			cachedList.reverse();
			}
		redrawList();
		}

	function sortOrder(a,b)
		{
		
		astr = a["isFolder"] + a[listFunc.sortby]
		bstr = b["isFolder"] + b[listFunc.sortby]
		
		
		if (astr.toLowerCase() < bstr.toLowerCase())
			{
			return -1;
			}
		else
			{
			if (astr.toLowerCase() > bstr.toLowerCase())
				{
				return 1;
				}
			else
				{
				return 0;
				}
			}
		}

		
	function crop(thestring,size)
		{
		if (top.opener.JSBrowser.sysfontsize == 1)
			{
				size = size-5;
			}
		if (thestring.length > size)
			{
			thestring = thestring.substring(0,size) + "...";
			}
		return thestring;
		}


	function fullname(fname,fext)
		{
		if (fext == "")
			{
			return fname;		
			}
		else
			{			
			return (fname + "." + fext);
			}
		}
		
	function formatsize(iStr)
		{
			iStr = parseInt(iStr);
			if (!isNaN(iStr))
			{
				iStr = Math.round(iStr/1024);
			}
			else
			{
				iStr = 0
			}
			return iStr;
		}
	
	function listObj(fpath, fname,fext, fsize, ftype, lastupdated,isFolder)
		{
		this.path = fpath;
		if (ftype == "File Folder")
			{
			this.icon = "dir.gif"
			}
		else
			{
			this.icon = "cfw.gif"
			}
		this.isFolder = isFolder;
		this.fname = fname;	
		this.fext = fext;
		this.displayname = crop(fullname(fname,fext),30);	
		this.fsize = fsize;
		this.displaysize = formatsize(fsize);
		this.ftype = crop(ftype,12);
		this.nodetype = ftype
		this.lastupdated = lastupdated;
		this.displaydate = crop(lastupdated,20);
		this.deleted = false;
		this.updated = false;
		this.newitem = false;
		}
	
	cachedList = new Array();	
	listFunc = new listFuncs();
	
</SCRIPT>

</BODY>
</HTML>
