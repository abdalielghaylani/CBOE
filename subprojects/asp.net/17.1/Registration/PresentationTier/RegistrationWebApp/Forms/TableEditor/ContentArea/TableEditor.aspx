<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Forms/Master/Registration.Master"
    Codebehind="TableEditor.aspx.cs" Inherits="RegistrationWebApp.Forms.TableEditor.ContentArea.TableEditor"
    ValidateRequest="false" %>

<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.COETableManager"
    TagPrefix="COECntrl" %>
<asp:Content ID="CompoundDetails" ContentPlaceHolderID="ContentPlaceHolder" runat="Server">
    <div class="Content">
        <ul class="UlTabsHeader">
            <li class="LiFirstTabsHeader"></li>
            <li class="SelectedTabsHeader">
                <div class="LeftCornerTabsHeader">
                </div>
                <a href="#" class="ATabsHeader">Table Editor</a>
                <div class="RigthCornerTabsHeader">
                </div>
            </li>
            <li class="LiLastTabsHeader"></li>
        </ul>
        <div class="Clear">
        </div>
        <div class="TabsContent">
            <div style="margin-bottom: 20px;">
                <asp:LinkButton ID="LinkButtonGoHome" runat="server" CssClass="Goto2" OnClick="LinkButtonGoHome_Click"
                    CausesValidation="false" Style="margin-right: 40px; margin-bottom: 20px; margin-top: 20px">Back</asp:LinkButton>
            </div>
            <div class="Clear">
            </div>
            <div style="text-align:center; width:950px;">
                <COECntrl:COETableManager ID="COETableManager1" runat="server" PageSize="10"></COECntrl:COETableManager>
            </div>
            <div class="Clear">
            </div>
            <div class="TabsFooter">
                <div class="DivFooter">
                </div>
            </div>
        </div>
    </div>
</asp:Content>
