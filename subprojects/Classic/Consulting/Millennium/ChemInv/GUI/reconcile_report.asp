<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->

<%
Dim ReportType
Dim LocationPath
Dim CurrentUserID
Dim Title

ReportType = Request("ReportType")
LocationPath = Request("LocationPath")
CurrentUserID = Session("UserName" & "cheminv")

if( ReportType = "NewContainers" ) then
    aBarcodes = split(Session("NewContainerBarcodeList"),",")
    Title = "Unknown container report"
elseif( ReportType = "AwolContainers" ) then
    aBarcodes = split(Session("awolContainerBarcodeList"),",")
    Title = "Misplaced container report"
end if

%>

<html>
<head>
<meta NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript">
    function window.onbeforeprint()
    {
        var PrintBtnDiv = document.getElementById("PrintBtnDiv");
        if (PrintBtnDiv)
        {
            PrintBtnDiv.style.display = "none";
        }
    }
    function window.onafterprint()
    {
        var PrintBtnDiv = document.getElementById("PrintBtnDiv");
        if (PrintBtnDiv)
        {
            PrintBtnDiv.style.display = "inline";
        }
    }
</script>

</head>
<body>
<br />
<p>
<center>
<span class="GUIFeedback"><% = Title %></span>
</center>
</p>
<table cellpadding="2" border="0" cellspacing="2" width="100%">
<tr>
    <th align=left width="20">Location:</th>
    <td align=left><% = LocationPath %></td>
</tr>
<tr>
    <th align=left width="20">User:</th>
    <td align=left><% = CurrentUserID %></td>
</tr>
<tr>
    <th align=left width="20">Date:</th>
    <td align=left><% = now()%></td>
</tr>
<tr>
    <th align=left valign=top width="20">Barcodes:</th>
    <td>
<%
    for i = 0 to ubound(aBarcodes)
        Response.write aBarcodes(i) & "<br />" & vblf
    next
%>    
    </td>
</tr>
<tr>
    <td colspan="2" align=right>
    <div id="PrintBtnDiv">
    <a href="#html" onclick="javascript:window.print();return(false);"><img src="/ChemInv/graphics/sq_btn/print_btn.gif" border="0"/></a>
    <a href="#html" onclick="javascript:window.close();return(false);"><img src="/ChemInv/graphics/sq_btn/close_dialog_btn.gif" border="0"/></a>
    </div>
    </td>
</tr>
</table>
</center>
</body>
</html>

<script language="javascript">
window.print();
</script>
