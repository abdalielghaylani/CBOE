<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.Master" AutoEventWireup="true"
    Codebehind="EditFormsXml.aspx.cs" Inherits="RegistrationWebApp.Forms.RegistrationAdmin.ContentArea.EditFormsXml"
    Title="Untitled Page" ValidateRequest="false" %>

<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics2.WebUI.WebHtmlEditor.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebHtmlEditor" TagPrefix="ighedit" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea"
    TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <div class="Content">
        <div class="TabsContent">
            <ul class="UlTabsHeader" style="width:950px">
                <li id="LiSearchTemporary" class="LiTabsHeader" runat="server">
                    <div class="LeftCornerTabsHeader">
                    </div>
                    <asp:LinkButton ID="LinkButtonSearchTemporary" runat="server" CssClass="ATabsHeaderEditXml" />
                    <div class="RigthCornerTabsHeader">
                    </div>
                </li>
                <li id="LiSearchPermanent" class="LiTabsHeader" runat="server">
                    <div class="LeftCornerTabsHeader">
                    </div>
                    <asp:LinkButton ID="LinkButtonSearchPermanent" runat="server" CssClass="ATabsHeaderEditXml" />
                    <div class="RigthCornerTabsHeader">
                    </div>
                </li>
                <li id="LiSendToRegitration" class="LiTabsHeader" runat="server">
                    <div class="LeftCornerTabsHeader">
                    </div>
                    <asp:LinkButton ID="LinkButtonSendToRegitration" runat="server" CssClass="ATabsHeaderEditXml" />
                    <div class="RigthCornerTabsHeader">
                    </div>
                </li>
                <li id="LiELNSearchTempForm" class="LiTabsHeader" runat="server">
                    <div class="LeftCornerTabsHeader">
                    </div>
                    <asp:LinkButton ID="LinkButtonELNSearchTemp" runat="server" CssClass="ATabsHeaderEditXml" />
                    <div class="RigthCornerTabsHeader">
                    </div>
                </li>
                <li id="LiELNSearchPermForm" class="LiTabsHeader" runat="server">
                    <div class="LeftCornerTabsHeader">
                    </div>
                    <asp:LinkButton ID="LinkButtonELNSearchPerm" runat="server" CssClass="ATabsHeaderEditXml" />
                    <div class="RigthCornerTabsHeader">
                    </div>
                </li>
            </ul>
            <ul class="UlTabsHeader" style="width:950px">
                <li id="LiDeleteLog" class="LiTabsHeader" runat="server">
                    <div class="LeftCornerTabsHeader">
                    </div>
                    <asp:LinkButton ID="LinkButtonDeleteLog" runat="server" CssClass="ATabsHeaderEditXml" />
                    <div class="RigthCornerTabsHeader">
                    </div>
                </li>
                <li id="LiSubmitMixture" class="LiTabsHeader" runat="server">
                    <div class="LeftCornerTabsHeader">
                    </div>
                    <asp:LinkButton ID="LinkButtonSubmitMixture" runat="server" CssClass="ATabsHeaderEditXml" />
                    <div class="RigthCornerTabsHeader">
                    </div>
                </li>
                <li id="LiRegisterMixture" class="LiTabsHeader" runat="server">
                    <div class="LeftCornerTabsHeader">
                    </div>
                    <asp:LinkButton ID="LinkButtonReviewRegister" runat="server" CssClass="ATabsHeaderEditXml" />
                    <div class="RigthCornerTabsHeader">
                    </div>
                </li>
                <li id="LiViewMixture" class="LiTabsHeader" runat="server">
                    <div class="LeftCornerTabsHeader">
                    </div>
                    <asp:LinkButton ID="LinkButtonViewMixture" runat="server" CssClass="ATabsHeaderEditXml" />
                    <div class="RigthCornerTabsHeader">
                    </div>
                </li>                
            </ul>
            <ul class="UlTabsHeader" style="width:950px">
                <li id="LiRegistryDuplicatesForm" class="LiTabsHeader" runat="server">
                    <div class="LeftCornerTabsHeader">
                    </div>
                    <asp:LinkButton ID="LinkButtonRegistryDuplicates" runat="server" CssClass="ATabsHeaderEditXml" />
                    <div class="RigthCornerTabsHeader">
                    </div>
                </li>    
                <li id="LiComponentDuplicates" class="LiTabsHeader" runat="server">
                    <div class="LeftCornerTabsHeader">
                    </div>
                    <asp:LinkButton ID="LinkButtonComponentDuplicates" runat="server" CssClass="ATabsHeaderEditXml" />
                    <div class="RigthCornerTabsHeader">
                    </div>
                </li>          
                <li id="LiDataLoader" class="LiTabsHeader" runat="server">
                    <div class="LeftCornerTabsHeader">
                    </div>
                    <asp:LinkButton ID="LinkButtonDataLoader" runat="server" CssClass="ATabsHeaderEditXml" />
                    <div class="RigthCornerTabsHeader">
                    </div>
                </li>
            </ul>
            <ul class="UlTabsHeader" style="width:950px">
                <li id="LiSearchComponentsToAdd" class="LiTabsHeader" runat="server">
                    <div class="LeftCornerTabsHeader">
                    </div>
                    <asp:LinkButton ID="LinkButtonSearchComponentsToAdd" runat="server" CssClass="ATabsHeaderEditXml" />
                    <div class="RigthCornerTabsHeader">
                    </div>
                </li>
                <li id="LiSearchComponentsToAddRR" class="LiTabsHeader" runat="server">
                    <div class="LeftCornerTabsHeader">
                    </div>
                    <asp:LinkButton ID="LinkButtonSearchComponentsToAddRR" runat="server" CssClass="ATabsHeaderEditXml" />
                    <div class="RigthCornerTabsHeader">
                    </div>
                </li>
            </ul>
            <table class="MessageTableRegAdmin">
                <tr>
                    <td colspan="4" align="center">
                        <uc2:MessagesArea ID="MessagesAreaUserControl" runat="server" Visible="false" />
                    </td>
                </tr>
            </table>
            <asp:LinkButton ID="LinkButtonGoToMain" runat="server" CssClass="Goto2" OnClick="LinkButtonGoToMain_Click">Back</asp:LinkButton>         
            <div class="PTabsContent">
                <div style="text-align: right; margin-right: 20px; margin-top: 20px; margin-bottom: 20px">
                    <asp:Button ID="SaveButton" runat="server" Text="Edit Xml" SkinID="ButtonBigRegAdmin" />
                    <div id="LinkButtonCancelDiv" runat="server" visible="false" style="display: inline">
                        <asp:Button ID="ButtonCancel" runat="server" Text="Cancel" OnClick="ButtonCancel_Click"
                            SkinID="ButtonBigRegAdmin" />
                    </div>
                </div>
                <div style="margin-right: 500px">
                    <asp:Button ID="CopyToClipBoardButton" runat="server" OnClientClick="copyToClipboard();"
                        CssClass="Goto2" Visible="false" Text="Copy to Clipboard" />
                    <asp:Button ID="PasteFromClipBoardButton" runat="server" OnClientClick="pasteFromClipboard();"
                        CssClass="Goto2" Visible="false" Text="Paste from Clipboard" />
                </div>
                <div class="Clear">
                </div>
            </div>
            <div class="MultilineDisplayXml" id="XmlControlDiv" runat="server">
                <asp:Xml ID="XmlControl" runat="server" Visible="true" EnableViewState="false"></asp:Xml>
            </div>
            <textarea id="XmlTextBox" runat="server" class="MultilineInputXmlEdit" visible="false"></textarea>
            <div class="TabsFooter">
                <div class="DivFooter">
                </div>
            </div>
        </div>
    </div>
</asp:Content>
