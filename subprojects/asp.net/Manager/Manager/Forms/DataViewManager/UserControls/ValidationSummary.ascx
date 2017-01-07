<%@ Control Language="C#" AutoEventWireup="True" Inherits="ValidationSummaryUC" Codebehind="ValidationSummary.ascx.cs" %>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Src="ValidateItem.ascx" TagName="ValidateItem" TagPrefix="uc2" %>

<table cellpadding="0" cellspacing="0">
    <!-- Name and Description -->
    <tr>
        <td style="width: 741px;padding-top:15px;" align="center">
            <igmisc:WebPanel ID="NameDescriptionWebPanel" runat="server" Width="400px" Font-Size="14px" Font-Bold="true" ForeColor="#000099">
                <Header TextAlignment="Center"></Header>
                <Template>
                    <table style="text-align:left;margin-top:15px;width:100%;">
                        <tr align="left">
                            <td style="padding-right:5px;width:100px;"><asp:Label ID="NameLabel" runat="server" SkinID="Title" /></td>
                            <td style="width:200px;"><COEManager:TextBoxWithPopUp ID="NameTextBoxWithPopUp" runat="server" /></td>
                        </tr>
                        <tr align="left">
                            <td style="padding-right:5px;width:100px;"><asp:Label runat="server" ID="DescriptionLabel" SkinID="Title"></asp:Label></td>
                            <td style="width:200px;"><COEManager:TextBoxWithPopUp ID="DescriptionTextBoxWithPopUp" runat="server" /></td>
                        </tr>
                    </table>
                </Template>
            </igmisc:WebPanel>
        </td>
    </tr>
    <tr>
        <td align="right"><uc2:ValidateItem ID="NameAndDescriptionValidateItem" runat="server" /></td>
    </tr>
    <!-- Base Table section -->
    <tr>
        <td style="width: 741px" align="center">
            <div style="width:400px;">
                <table style="text-align:left;margin-top:15px;width:100%;">
                    <tr align="left">
                        <td style="padding-right:5px;width:100px;"><asp:Label runat="server" ID="BaseTableTitleLabel" SkinID="Title"></asp:Label></td>
                        <td style="width:200px;"><COEManager:TextBoxWithPopUp ID="BaseTableTextBoxWithPopUp" runat="server" /></td>
                    </tr>
                </table>
            </div>
        </td>
    </tr>
     <tr>
        <td align="right">
            <uc2:ValidateItem ID="BaseTableValidateItem" runat="server" />
        </td>
    </tr>
    <!-- Tables and Fields section -->
     <tr>
        <td style="width: 741px;padding-top:15px;">
            <div style="padding-top:5px;height:50px;border-width:1px; color:Navy; font-size:9pt; font-weight:bold; font-family:Verdana; border-style:solid; background-color:#EBF3FB; position:relative;">
                <asp:Label ID="TablesAndFieldsInfoLabel" runat="server" />
            </div>
        </td>
    </tr>
    <tr>
        <td align="right" style="width: 741px">
            <uc2:ValidateItem ID="TablesFieldValidateItem" runat="server" />
        </td>
    </tr>
    <tr>
        <td style="width: 741px;padding-top:15px;">
            <div style="padding-top:5px;height:50px;border-width:1px; color:Navy; font-size:9pt; font-weight:bold; font-family:Verdana; border-style:solid; background-color:#EBF3FB; position:relative;">
                <asp:Label ID="RelationshipsInfoLabel" runat="server" />
            </div>
        </td>
    </tr>
     <tr>
        <td align="right" style="width: 741px">
            <uc2:ValidateItem ID="RelationshipsValidateItem" runat="server" />
        </td>
    </tr>
    <!-- Security section -->
    <tr>
         <td>
         <table style="margin-top:15px;">
            <tr>
                <td><asp:Label runat="server" ID="SecurityTitleLabel" SkinID="Title"></asp:Label></td>
            </tr>
         </table>
         </td>
    </tr>
     <tr>
        <td style="width: 741px">
            <div style="padding-top:15px;">
               <asp:Repeater runat="server" ID="UsersRepeater">
                    <HeaderTemplate>
                        <asp:Label runat="server" ID="UsersTitleLabel" SkinID="Title"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="UserLabel" SkinID="Text"></asp:Label><br />
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div style="padding-top:15px;">
                <asp:Repeater runat="server" ID="RolesRepeater" >
                    <HeaderTemplate>
                        <asp:Label runat="server" ID="RolesTitleLabel" SkinID="Title"></asp:Label><br />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="RolesLabel" SkinID="Text"></asp:Label>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </td>
      </tr>
    <tr>
        <td align="right" style="width: 741px">
            <uc2:ValidateItem ID="SecurityValidateItem" runat="server" />
            <br />
        </td>
    </tr>
</table>
