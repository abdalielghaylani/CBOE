<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim RS

CompoundId = Request("CompoundID")

if CompoundId = "" Then CompoundID = 0
'GetInvConnection() 
Set Conn = Server.CreateObject("ADODB.Connection")
Conn.Open "FILE NAME=c:\xx.udl;User ID=chemacxdb;Password=oracle"
sql = "SELECT base64_cdx, MOL_ID, csNum FROM substance where csnum = " & compoundid 
'sql =  "SELECT base64_cdx, MOL_ID, Compound_ID FROM inv_compounds WHERE Compound_ID =" & CompoundID
Set RS = Conn.Execute(sql)
If NOT (RS.EOF AND RS.BOF) then
MolID = RS("MOL_ID")
base64str = RS("base64_cdx")
Else
MolID = 0
base64str = ""
End if
RS.close
Set RS = nothing
Set Conn = Nothing
%>
<HTML>
<HEAD>
<META NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<script language="JavaScript" src= "/cfserverasp/source/chemdraw.js"></script>
<script>cd_includeWrapperFile("/cfserverasp/source/")</script>
</HEAD>
<BODY>

<%=ShowInlineStructure("X", 200, 300, 1, base64str)%>
<BR>
<form id=form1 name=form1>
Compound ID:&nbsp;<INPUT type=text name="compoundid" value="<%=CompoundID%>"><BR>
MOL ID:&nbsp;<INPUT type=text name="molid" value="<%=MolID%>" disabled><BR>

<Input type=submit value="Refresh">
</form>
</BODY>
</HTML>
