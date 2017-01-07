<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfigSectionsUC.ascx.cs" Inherits="RegistrationWebApp.Forms.RegistrationAdmin.UserControls.ConfigSectionsUC" %>
 
<asp:Repeater runat="server" ID="AppConfigSectionRepeater" >
    <ItemTemplate>
        <asp:LinkButton ID="SectionLinkButton" runat="server" CommandName="TabHeader"></asp:LinkButton>&nbsp;&nbsp;&nbsp;
    </ItemTemplate>
</asp:Repeater>
<br /><br />
&nbsp;&nbsp;
<asp:Label runat="server" ID="SectionTitleLabel" SkinID="PageTitleLabelSmall"></asp:Label>
<asp:Label runat="server" ID="SectionDescriptionLabel"></asp:Label>
<br /><br />
<table border="1">        
    <asp:Repeater runat="server" ID="SettingsRepeater">
        <HeaderTemplate>
        </HeaderTemplate>
        <ItemTemplate>
            <tr style="height:50px">  
               <td>
                    <asp:LinkButton ID="HelpLink" runat="server" CssClass="HelpButton2"><asp:Label runat="server" ID="ConfigSettingDescription"></asp:Label></asp:LinkButton>
                </td> 
                <td>
                    <asp:Label SkinID="ConfigSettingTitle" ID="ConfigSettingTitle" runat="server" Width="250px"></asp:Label>
                </td>
                <td>    
                    <asp:TextBox ID="ConfigSettingTextBox" runat="server" Width="350px"></asp:TextBox>
                </td>             
            </tr>
        </ItemTemplate>
    </asp:Repeater>
 </table>
 <br />