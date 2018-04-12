<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim RS
Dim objXmlHttp
Dim httpResponse
Dim StatusCode
Dim URL
Dim LastPercent
Dim StartTime

'URL = "http://localhost/cheminv/cheminv/cheminv_action.asp?dbname=cheminv&formgroup=base_form_group&dataaction=get_structure&Table=inv_compounds&Field=Structure&DisplayType=rawbase64cdx&StrucID=" 
URL = "http://localhost/chemacx/chemacx/chemacx_action.asp?dbname=chemacx&formgroup=base_form_group&dataaction=get_structure&Table=substance&Field=Structure&DisplayType=rawbase64cdx&StrucID=" 
%>
<HTML>
<HEAD>
<META NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
</HEAD>
<BODY>
<% 
Server.ScriptTimeout = 60 * 60 * 6 * 100
if Request("doit")= "" then%>
	<p>Click to update the inv_compounds table with structures from the ChemInv.mst file
	<FORM>
		<INPUT TYPE="hidden" name="doit" value="true">
		<INPUT type="submit" value="Start">
	</FORM>
<%ELSE%>
<H2>Updating Inventory Compounds Table...</H2>
<TABLE WIDTH="100" CELLPADDING=0 CELLSPACING=0 BORDER=0>
<TR>
	<TD align=right nowrap>
		Total rows :&nbsp;
	</TD>
	<TD colspan=20>
		<span id="totalrows"><span>
	</TD>
</TR>
<TR>
	<TD align=right nowrap>
		Rows remaining:&nbsp;
	</TD>
	<TD colspan=20>
		<span id="rowsRemaining"><span>
	</TD>
</TR>
<TR>
	<TD align=right nowrap>
		Rows completed:&nbsp;
	</TD>
	<TD colspan=20>
		<span id="rowsCompleted"><span>
	</TD>
</TR>
<TR>
	<TD align=right>
		Progress:&nbsp;
	</TD>
<% For X = 1 to 20 %>
	<TD>
	<IMG SRC="/xx/grey.gif" NAME="Block<%=X%>" WIDTH=10 HEIGHT=15 ALT="Progress Bar"><IMG SRC="/xx/White.gif" WIDTH="2" height="15">
	</TD>
<%Next%>
</TR>
<tr>
	<TD align=right nowrap>
		Time elapsed:&nbsp;
	</TD>
	<TD colspan=20>
		<span id="timeElapsed"><span>
	</TD>
<tr>
<tr>
	<TD align=right nowrap>
		Rate (rows/sec):&nbsp;
	</TD>
	<TD colspan=20>
		<span id="rate"><span>
	</TD>
<tr>
</TABLE>
<P>
<P>&nbsp;</P>
<%
StartTime = Timer
'GetInvConnection()
Set Conn = Server.CreateObject("ADODB.Connection")
Conn.Open "FILE NAME=c:\xx.udl;User ID=chemacxdb;Password=oracle"
Set RS = Server.CreateObject("ADODB.Recordset")
'sql =  "SELECT compound_id, MOL_ID, base64_cdx FROM inv_compounds"
sql = "SELECT csNum, Mol_id, base64_cdx FROM substance WHERE mol_id > 0 AND base64_cdx is null"
RS.Open sql, Conn, adOpenKeyset,  adLockOptimistic, adCmdText 
RecordCount  = RS.RecordCount
rowsRemaining = RecordCount
rowsCompleted = 0
Call Progressbar(0)
response.flush
response.write String(125," ")
response.flush
LastPercent = 0

IF NOT (RS.EOF AND RS.BOF) then	
		
	' This is the server safe version from MSXML3.
	Set objXmlHttp = Server.CreateObject("Msxml2.ServerXMLHTTP")
	i = 0
	While NOT RS.EOF
		'theURL = URL & RS("compound_id")
		theURL = URL & RS("csnum")
		'Response.Write theurl & "<BR>"
		objXmlHttp.open "GET", theURL, False
		objXmlHttp.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
		objXmlHttp.setRequestHeader "User-Agent", "cheminv"
		objXmlHttp.send pData		
		
		StatusCode = objXmlHttp.status
		'Response.Write objXmlHttp.responseText & "<BR>" & RS("csnum") &"<BR><HR>"
		If StatusCode = "200" then
			RS("base64_cdx") = objXmlHttp.responseText
		Else
			'Response.Write "Failed to get base64cdx for compound_ID= " & RS("compound_id") & "<BR>"
			Response.Write "Failed to get base64cdx for compound_ID= " & RS("csnum") & "<BR>"
		End If
		i= i + 1
		rowsRemaining = rowsRemaining - 1
		Progress = Int((i/RecordCount* 100)/5) 
		
		if (i MOD 500) then Progressbar(Progress)
		RS.MoveNext
	Wend
	Set objXmlHttp = Nothing
ELSE
	Response.Write "There are no rows in the inv_compounds table"
	Response.End
End if
Response.Write "Done"

Sub Progressbar(PercentDone)
		if NOT Response.IsClientConnected then Response.end 
		If LastPercent <> PercentDone then
			TimeElapsed = Timer - StartTime	
			RowsCompleted = RecordCount - rowsremaining 
			Rate = Round(RowsCompleted/TimeElapsed, 2)
			Block = "Block" & PercentDone
			response.write "<SCRIPT>document." & Block & ".src = '/xx/blue.gif';document.all.rowsRemaining.innerHTML='" & rowsRemaining & "' ;document.all.timeElapsed.innerHTML='" & SecondsToTimer(TimeElapsed) & "' ;document.all.rate.innerHTML='" & Rate & "' ;document.all.totalrows.innerHTML='" & RecordCount & "'  ;document.all.rowsCompleted.innerHTML='" & Rowscompleted & "'</SCRIPT>"
			response.flush
			LastPercent = PercentDone	
		End if
End sub

Function SecondsToTimer(seconds)
	Hours = Int(seconds/3600)
	seconds = seconds - hours * 3600
	If hours < 10 then hours = "0" & hours 
	Minutes = Int(seconds/60)
	If Minutes < 10 then Minutes = "0" & Minutes
	seconds = Int(seconds - minutes * 60)
	If seconds < 10 then seconds = "0" & seconds
	SecondsToTimer = Hours & ":" & Minutes & ":" & seconds
End function
End if
%>
</BODY>
</HTML>
