<%@ Control Language="C#" AutoEventWireup="true" Inherits="Relationships" Codebehind="Relationships.ascx.cs" %>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebNavigator.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<asp:HiddenField runat="server" ID="SelectedNode_CurrentDVBOHidden" />
<div runat="server" id="InvalidRelationshipsText" visible="false">
    <asp:Label runat="server" ID="InvalidRelationshipsTextLabel" SkinID="InvalidRelationShipText"></asp:Label>
</div>

<table class="NewRelationShips" >
    <tr>
        <td>
            <asp:Label runat="server" ID="CurrentDVBOLabel" SkinID="Title"></asp:Label>
        </td>
        <td>
            <asp:Label runat="server" ID="JoinLabel" SkinID="Title"></asp:Label>
        </td>
    </tr>
    <tr>
        <td valign="top" style="width:400px">
            <ignav:UltraWebTree ID="CurrentDVBOUltraWebTree" runat="server" SkinID="CurrentDVBOUltraWebTree" DataKeyOnClient="true" SingleBranchExpand="true">
                <ClientSideEvents NodeChecked="CurrentDVBOUltraWebTree_NodeChecked" />
            </ignav:UltraWebTree>
        </td>
        <td valign="top" align="center">
            <asp:DropDownList runat="server" ID="JoinTypesDropDown" AutoPostBack="true"></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td align="center" style="width: 300px;" valign="top">
           <asp:Label ID="SelectedFromTableLabel" runat="server" SkinID="RelationShipText" ></asp:Label>
           <asp:Label ID="SelectedFromFieldLabel" runat="server"  SkinID="RelationShipText"></asp:Label>
        </td>
        <td align="center" valign="top">
            <asp:Label ID="SelectionJoinTypeLabel" runat="server" SkinID="RelationShipText"></asp:Label>
        </td>
        <td valign="top">
            <asp:Label ID="SelectedToTableLabel" runat="server" SkinID="RelationShipText"></asp:Label>
            <asp:Label ID="SelectedToFieldLabel" runat="server" SkinID="RelationShipText"></asp:Label>
        </td>
    </tr>
    <tr>
        <td colspan="3" align="center">
            <COEManager:ImageButton ID="CancelImageButton" runat="server" TypeOfButton="Cancel" ButtonMode="ImgAndTxt" CausesValidation="false" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png"/>
            <COEManager:ImageButton ID="OKImageButton" runat="server" TypeOfButton="Submit" ButtonMode="ImgAndTxt" CausesValidation="true" />
            <COEManager:ImageButton ID="DeleteImageButton" runat="server" TypeOfButton="Cancel" ButtonMode="ImgAndTxt" CausesValidation="false" ImageURL="../../../App_Themes/Common/Images/Delete.png" HoverImageURL="../../../App_Themes/Common/Images/Delete.png"/>
        </td>
    </tr>
</table>