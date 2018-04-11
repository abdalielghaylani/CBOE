<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="false" CodeBehind="Logoff.aspx.cs" Inherits="Logoff" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Log Off </title>
    <script language="javascript" type="text/javascript">
        window.onload = doLogOff;
        function doLogOff()
        {
            if(typeof(window.opener) != 'undefined')
             {
                if (window.opener.top.opener != null)
                {
                    //reload grand parent window
                    window.opener.top.opener.location.reload();
                    
                    //parent window should be closed
                    window.opener.top.close();
                    
                    //close popup window
                    if (window.name != "")
                    {
                        self.opener = this;
                        self.close();
                     }
                    
                 }
             
                else
                {
                    //there was no granparent
                    if(window.opener.location != null)
                    {
                        //reload parent which will go to home page
                        window.opener.location.reload();
                        
                        //close popup window
                        if (window.name != "")
                        {
                            self.opener = this;
                            self.close();
                         }
                    }
                }
                
                
            }
            else if (typeof(top.opener) != 'undefined')
           		{
                if(top.opener.location != null)
                {
                    top.opener.location.reload();
                    if (top.name != "")
                    {
                        top.opener = this;
                        top.close();
                     }
                }
            }
            else
            {
            		
                window.location = "forms/public/contentarea/login.aspx";
            }
        }
     </script>
</head>
<body >
   
</body>
</html>

