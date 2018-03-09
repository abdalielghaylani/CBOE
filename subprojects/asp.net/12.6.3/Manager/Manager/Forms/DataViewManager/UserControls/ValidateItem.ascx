<%@ Control Language="C#" AutoEventWireup="true" Inherits="ValidateItem" Codebehind="ValidateItem.ascx.cs" %>
<div class="ValidationItem">
    <!--<asp:Image runat="server" ID="ValidationImage" />-->
    <COEManager:ImageButton ID="ValidationImageButton" runat="server" TypeOfButton="Valid" ButtonMode="ImgAndTxt" OnClientClick="return false;" ButtonCssClass="ValidButton" ImageCssClass="ValidButton" />
    <!--<asp:Label runat="server" ID="ValidationTitleLabel" SkinID="Title"></asp:Label>-->
</div>
<div id="ErrorMessagesDiv">
    <asp:Repeater runat="server" ID="ErrorMessagesRepeater">
        <ItemTemplate>
            <div>
                <asp:Image runat="server" ID="ErrorImage" />
                <asp:Label runat="server" ID="ErrorMessageLabel" SkinID="Text"></asp:Label>
                <COEManager:ImageButton ID="ResolveImageButton" OnButtonClicked="ValidationImageButton_ButtonClicked" runat="server" TypeOfButton="Resolve" ButtonMode="ImgAndTxt" />
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
