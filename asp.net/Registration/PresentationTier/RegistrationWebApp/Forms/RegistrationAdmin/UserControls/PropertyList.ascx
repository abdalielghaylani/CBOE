<%@ Control Language="C#" AutoEventWireup="true" Codebehind="PropertyList.ascx.cs"
    Inherits="CambridgeSoft.COE.RegistrationAdminWebApp.Forms.RegistrationAdmin.UserControls.PropertyList" %>
<%@ Register Assembly="Csla" Namespace="Csla.Web" TagPrefix="cc1" %>
<%@ Register Src="ValidationRules.ascx" TagName="ValidationRules" TagPrefix="uc1" %>
<div style="margin-top: 10px">
    <asp:Panel ID="PropertyListPanel" runat="server" Width="700px" SkinID="ValidationSummaryPanel"
        HorizontalAlign="Center">
        <div style="margin-top: 20px">
            <asp:Panel ID="EnterNewPropertyPanel" runat="server" Width="650px">
                <div>
                    <div style="text-align: center; margin-bottom: 10px; margin-top: 20px">
                        <div style="display: inline">
                            <asp:Label ID="LblName" runat="server" Width="100px" SkinID="Title"></asp:Label>&nbsp;</div>
                        <div style="display: inline">
                            <asp:TextBox ID="TxtName" runat="server" Width="180px"></asp:TextBox></div>
                    </div>
                    <div style="text-align: center; margin-bottom: 10px">
                        <div style="display: inline">
                            <asp:Label ID="LblType" runat="server" Width="100px" SkinID="Title"></asp:Label>&nbsp;</div>
                        <div style="display: inline">
                            <asp:DropDownList ID="DdlType" runat="server" Width="180px">
                                <asp:ListItem>Text</asp:ListItem>
                                <asp:ListItem>Number</asp:ListItem>
                                <asp:ListItem>Date</asp:ListItem>
                            </asp:DropDownList></div>
                    </div>
                    <div style="text-align: center">
                        <div style="display: inline">
                            <asp:Label ID="LblPrecision" runat="server" Width="100px" SkinID="Title"></asp:Label>&nbsp;</div>
                        <div style="display: inline">
                            <asp:TextBox ID="TxtPresicion" runat="server" Width="180px"></asp:TextBox></div>
                    </div>
                </div>
                <div style="margin-top: 20px; text-align: center; margin-bottom: 20px">
                    <div style="display: inline; text-align: justify">
                        <asp:Button ID="BtnCancel" runat="server" OnClick="BtnCancel_Click" Width="150px" />
                    </div>
                    <div style="display: inline; text-align: justify; margin-left: 10px">
                        <asp:Button ID="BtnAddProperty" runat="server" OnClick="BtnAddProperty_Click" Width="150px" />
                    </div>
                </div>
            </asp:Panel>
        </div>
        <div style="margin-top: 20px; margin-bottom: 20px">
            <asp:Panel ID="Delete_Property_Panel_Text" runat="server" Width="650px">
                <div style="text-align:left; margin-top: 20px">
                    <asp:CheckBoxList  ID="Cbl_Properties" runat="server" Width="500px">
                    </asp:CheckBoxList></div>
                <div style="text-align: center; margin-top: 20px; margin-bottom: 20px">
                    <div style="display: inline">
                        <asp:Button ID="BtnCancelProp" runat="server" Width="150px" />
                    </div>
                    <div style="display: inline; margin-left: 10px">
                        <asp:Button ID="BtnDeleteProp" runat="server" Width="150px" />
                    </div>
                </div>
            </asp:Panel>
        </div>
        <div style="text-align: center; margin-bottom: 20px">
           <a href="ValidationRules.ascx" runat="server">Validations Rules</a>
        </div>
    </asp:Panel>
</div>
