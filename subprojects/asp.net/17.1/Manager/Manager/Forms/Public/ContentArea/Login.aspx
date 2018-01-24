
<%@ Page Language="C#" EnableTheming="false" AutoEventWireup="true" Inherits="Forms_Public_ContentArea_Login" Codebehind="Login.aspx.cs" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Forms/Master/MasterPage.master" %>
<%@ Register Src="../UserControls/Login.ascx" TagName="Login" TagPrefix="uc1" %>
<asp:Content ID="Framebuster" ContentPlaceHolderID="FramebusterContent" Runat="Server"    >
<script language="javascript">
	if (opener){opener.top.location.href = window.location.href; opener.focus(); window.close()}
	if(parent.location.href != window.location.href) parent.location.href = window.location.href;
</script>
</asp:Content>
<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server"    >

    <table class="PagesContentTable" style="text-align:center;">
        <tr>
            <td align="center">
                <COEManager:ConfirmationArea ID="ConfirmationAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="center">
               <COEManager:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false"/>
            </td>
        </tr>
       <tr>
        <td>
            <center>
<table>
                <tr>
                    <td align="center">
                        <uc1:Login ID="Login1" runat="server" />
                    </td>
                </tr>
            </table></center>

        </td>
       </tr>
    </table>
</asp:Content>

