<%@ LANGUAGE=VBScript %>
<% Option Explicit %>
<% Response.Expires = 0 %>

<%	

	Const L_PATHNOTFOUND_TEXT = "The path was not found."
	Const L_SLASH_TEXT = "\"	

	Const TDIR = 0
	Const TFILE = 1
	
	Const FIXEDDISK = 2

	
	Dim i, newid,path, f, sc, fc, fl, FileSystem,btype,drive, drives
	Dim primarydrive
	
	bType = CInt(Request.Querystring("btype"))
	
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
	
	newid = 0
	
	If Request.QueryString("path") <> "" Then
		path = Request.QueryString("path")	
		if FileSystem.FolderExists(path) then
			Response.Cookies("HTMLA")("LASTPATH")=path			
		end if
	Else
		path = Request.Cookies("HTMLA")("LASTPATH")
	End If 

	If path = "" Then
		Response.Cookies("HTMLA")("LASTPATH")=primarydrive
		path = primarydrive
	End If	
%>

<HTML>
<HEAD>

<SCRIPT LANGUAGE="JavaScript">

	<% if FileSystem.FolderExists(path) then %>
		top.main.head.cachedList = new Array();
		cachedList = top.main.head.cachedList;		
	<%	
	     Set f=FileSystem.GetFolder(path)
		 Set sc = f.SubFolders
		 For Each i In sc
		 	if i.Attributes AND 2 then 					
			else
			 %>
			 cachedList[<%= newid %>]= new top.main.head.listObj("<%= Replace(i,"\","\\") %>","<%= i.name %>","","","<%= "File Folder" %>","<%= i.DateLastModified %>",true);
			 <%
			 newid = newid +1	 
			end if
		 Next
		 if bType = TFILE then 
			 Set fc = f.Files
			 For Each fl in fc
		 		if fl.Attributes AND 2  then
				else 
					'DGB On some systems with CFW 10 the file type is registered as CFW Document instead of ChemFinder Document
					'Extended to logic to account for possible file type difference.
					if (Instr(UCase(fl.Type), "CHEM")>0) OR (Instr(UCase(fl.Type), "CFW")>0) OR (Instr(UCase(fl.Type), "FOLDER")>0)then%>
					 cachedList[<%= newid %>]= new top.main.head.listObj("<%= Replace(i,"\","\\") %>","<%= fl.Name %>","","<%= fl.size %>","<%= fl.Type %>","<%= fl.DateLastModified %>",false);
					 <%
					 newid = newid +1
					end if			 
				 end if
			
			 Next
		 end if
	%>		 
		top.main.head.listFunc.selIndex=0;
		top.main.list.location.href ="JSBrwLs.asp";
	<% else %>
		alert(top.main.head.document.userform.currentPath.value+"\r\r<%= L_PATHNOTFOUND_TEXT %>");
		top.main.head.document.userform.currentPath.value = "<%= Replace(Request.Cookies("HTMLA")("LASTPATH"),"\","\\") %>";		
	<% end if %>
</SCRIPT>

</HEAD>
<BODY>
</BODY>
</HTML>