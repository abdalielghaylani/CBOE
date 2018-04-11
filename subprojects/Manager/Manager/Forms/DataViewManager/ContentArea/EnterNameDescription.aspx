<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_DataViewManager_ContentArea_EnterNameDescription" Codebehind="EnterNameDescription.aspx.cs" MasterPageFile="~/Forms/Master/DataViewManager.Master" %>
<%@ MasterType VirtualPath="~/Forms/Master/DataViewManager.master" %>
<%@ Register Src="~/Forms/DataViewManager/UserControls/NameDescription.ascx" TagName="NameDescription"
    TagPrefix="uc1" %>
<%@ Register Src="~/Forms/DataViewManager/UserControls/SelectBaseTable.ascx" TagName="SelectBaseTable"
    TagPrefix="uc1" %>


    
<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server" >

<%--<script language="javascript" type="text/javascript">

    function Showmessage() {
        alert('Message');
        var a = document.getElementById("ConfirmationMessageTable");
        //alert(document.getElementById("<% =WarningControl.ClientID %>"));
        alert(a);
       
    }

</script>--%>

<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" ScriptMode="Release" EnablePageMethods="true"/>
    <div class="PagesContentTable">
        <div>
            <COEManager:ConfirmationArea ID="ConfirmationAreaUserControl" runat="server" Visible="false" />
        </div>
        <div>
            <COEManager:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false"/>
        </div>
        <div>
            <asp:Label runat="server" ID="SelectedDataViewLabel" />
        </div>
        <table style="width:100%;" class="NameDescUCTable">
            <tr style="vertical-align:top;">
                <td class="NameDescUCTable">
                    <div>
                        <uc1:NameDescription ID="NameDescriptionUserControl" runat="server" />
                    </div>
                </td>
                <td class="NameDescUCTable">
                    <div>      
                    <%--Jira ID: CBOE-1161 : Veritcal scroll bar --%>
                    <COEManager:ConfirmationArea ID="WarningControl" runat="server" Visible="false" />                     
                    <uc1:SelectBaseTable ID="SelectBaseTableUserControl" runat="server" />                    
                    </div>
                </td>
            </tr>
        </table>
        <div style="margin: 5px;text-align:center;clear:both;">
            <COEManager:ImageButton ID="CancelImageButton" runat="server" TypeOfButton="Cancel" ButtonMode="ImgAndTxt" CausesValidation="false" OnClientClick="return ConfirmCancel();" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" />
            <COEManager:ImageButton ID="NextImageButton" runat="server" TypeOfButton="Next" ButtonMode="ImgAndTxt" />
        </div>
    </div>
</asp:Content>