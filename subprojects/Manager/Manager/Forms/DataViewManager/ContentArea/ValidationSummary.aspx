<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_DataViewManger_ContentArea_ValidationSummary" Codebehind="ValidationSummary.aspx.cs" MasterPageFile="~/Forms/Master/DataViewManager.Master" %>
<%@ MasterType VirtualPath="~/Forms/Master/DataViewManager.master" %>
<%@ Register Src="../UserControls/ValidationSummary.ascx" TagName="ValidationSummary"
    TagPrefix="uc1" %>
<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server" >
<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" ScriptMode="Release"/>
<iframe id="ParentDiv" class="BackgroundHidden" frameborder="0" scrolling="no"></iframe>
    <script language="javascript" type="text/javascript">
    function SetProgressLayerVisibility(visible)
    {
        if(visible)
        {
            document.getElementById('UpdateProgressDiv').style.display = 'block';
            document.getElementById('ParentDiv').className = 'BackgroundVisible';
        }
        else
        {
            document.getElementById('UpdateProgressDiv').style.display = 'none';
            document.getElementById('ParentDiv').className ='BackgroundHidden';
        }
    }
    </script>
    <table width="100%" class="PagesContentTable">
     <tr>
            <td align="center">
                <COEManager:ImageButton ID="CancelImageButton1" runat="server" TypeOfButton="Cancel" ButtonMode="ImgAndTxt" OnClientClick="if(ConfirmCancel()) { SetProgressLayerVisibility(true); return true; } else return false;" CausesValidation="false" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" />
                <COEManager:ImageButton ID="SubmitImageButton1" runat="server" TypeOfButton="Submit" ButtonMode="ImgAndTxt" OnClientClick="SetProgressLayerVisibility(true); return true;" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <COEManager:ConfirmationArea ID="ConfirmationAreaUserControl" runat="server" Visible="false" />
            </td> 
        </tr>
        <tr>
            <td align="center">
                <asp:UpdatePanel ID="ErrorAreaUpdatePanel" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <COEManager:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td align="center">
                <uc1:ValidationSummary id="ValidationSummaryUserControl" runat="server"></uc1:ValidationSummary>
            </td>
        </tr>
        <tr>
            <td align="center">
                <COEManager:ImageButton ID="CancelImageButton" runat="server" TypeOfButton="Cancel" ButtonMode="ImgAndTxt" OnClientClick="if(ConfirmCancel()) { SetProgressLayerVisibility(true); return true; } else return false;" CausesValidation="false" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" />
                <COEManager:ImageButton ID="SubmitImageButton" runat="server" TypeOfButton="Submit" ButtonMode="ImgAndTxt" OnClientClick="SetProgressLayerVisibility(true); return true;" />
            </td>
            <td>
                <div id="UpdateProgressDiv" style="z-index:340;display:none;">
                    <img id="ProgressImage" alt="Processing" src="../../../App_Themes/Common/Images/searching.gif" style="position:absolute;top:72px;left:762px;z-index:340;"/>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>


