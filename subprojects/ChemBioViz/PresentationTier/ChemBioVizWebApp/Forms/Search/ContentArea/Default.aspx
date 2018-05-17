<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.ChemDraw"
    TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Loading</title>
</head>
<body>
    <script language="javascript" src="/cfserverasp/source/chemdraw.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        cd_setIsCDPPluginCookie();
        
        var queryString = location.search;
        
        var url = "ChemBioVizSearch.aspx";
        
        if(location.search.length > 0)
            url += location.search;
            
        window.location.href =  url;
    </script>
</body>
</html>
