<%@ Control Language="C#" AutoEventWireup="true" Inherits="Lookup" Codebehind="Lookup.ascx.cs" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<asp:HiddenField runat="server" ID="SelectedNode_DataViewBOHidden" />
<table class="NewRelationShips" border="1">
    <tr>
        <td colspan="2" align="center">
            <asp:Label runat="server" ID="LookupTitle" SkinID="Title"></asp:Label>
            <br />
            <asp:Label runat="server" ID="SelectedFieldNameLabel" SkinID="Text"></asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label runat="server" ID="InvalidFieldsLabel"></asp:Label>
        </td>
    </tr>
    <tr>
        <td valign="top">
            <ignav:UltraWebTree ID="DataViewBOUltraWebTree" runat="server" SkinID="CurrentDVBOUltraWebTree" DataKeyOnClient="true" SingleBranchExpand="true">
                <ClientSideEvents NodeChecked="DataViewBOUltraWebTree_NodeCheckedClientSide" />
            </ignav:UltraWebTree>
        </td>
        <td valign="top" align="left">
            <table>
                <tr>
                    <td valign="top"><asp:Label runat="server" ID="LookupFieldTitleLabel" SkinID="Title"></asp:Label></td>
                    <td valign="top"> <asp:RadioButtonList runat="server" ID="LookupDisplayFieldRadioList" SkinID="FieldRadioItems"></asp:RadioButtonList></td>
                </tr>
                <tr>
                    <td valign="top"><asp:Label runat="server" ID="LookupSortOrderTitleLabel" SkinID="Title"></asp:Label></td>
                    <td valign="top"><asp:DropDownList runat="server" ID="SortingDropDownList" ></asp:DropDownList></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td colspan="2" align="center">
            <COEManager:ImageButton ID="CancelImageButton" runat="server" TypeOfButton="Cancel" ButtonMode="ImgAndTxt" CausesValidation="false" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" />
            <COEManager:ImageButton ID="ClearImageButton" runat="server" TypeOfButton="Clear" ButtonMode="ImgAndTxt" CausesValidation="false" OnClientClick="ClearCurrentSelections();" />
            <COEManager:ImageButton ID="OKImageButton" runat="server" TypeOfButton="Submit" ButtonMode="ImgAndTxt" CausesValidation="true" />
            <COEManager:ImageButton ID="DeleteImageButton" runat="server" TypeOfButton="Delete" ButtonMode="ImgAndTxt" CausesValidation="false" />
        </td>
    </tr>
</table>