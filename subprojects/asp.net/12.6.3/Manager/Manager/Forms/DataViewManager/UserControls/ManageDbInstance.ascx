<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ManageDbInstance.ascx.cs" Inherits="Manager.Forms.DataViewManager.UserControls.ManageDbInstance" %>
<style type="text/css">
.FormLabel
{
    text-align:left;
    font-size:1em;
    font-weight:bold;
}
.FormField
{
    text-align:left;
    font-size:1em;
}
</style>
<script language="javascript" type="text/javascript">
    function ChangeImage(imageId, imageURL)
    {
        document.getElementById(imageId).src = imageURL;
    }

    function SetProgressLayerVisibility() 
    {
        Page_ClientValidate();
        if (Page_IsValid) 
        {
            document.getElementById('UpdateProgressDiv').style.display = 'block';
        }
        else 
        {
            document.getElementById('UpdateProgressDiv').style.display = 'none';
        }
    }
</script>
<br />
<asp:Panel Width="400px" runat="server" >
    <asp:Table runat="server" CssClass="FormLabelField" >
        <asp:TableRow>
            <asp:TableCell ColumnSpan="2">
              <asp:CustomValidator runat="server" ID="validator" Width="275px" Display="Static"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow Height="20 px">
            <asp:TableCell Width="100 px" CssClass="FormLabel">
                <asp:Label runat="server" Text="Data source name:" SkinID="Text" />
            </asp:TableCell>
            <asp:TableCell CssClass="FormField">
                <asp:TextBox Width="175 px" runat="server" ID="txtInstanceName" SkinID="TextBox" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtInstanceName" Display="Dynamic" ToolTip="Data source name cannot be empty." ErrorMessage="Data source name cannot be empty." Text="*" />
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" Display="Dynamic" ControlToValidate="txtInstanceName" 
                ToolTip="Data source name can be alpha-numeric and the maximum length is of 50 characters. Allowed special characters are: = _ () ! , % - and space." 
                ErrorMessage="Data source name can be alpha-numeric and the maximum length is of 50 characters. Allowed special characters are: = _ () ! , % - and space." 
                ValidationExpression="^[a-zA-Z0-9_=()!, %-]{1,50}$" Text="*" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow Height="20 px">
            <asp:TableCell CssClass="FormLabel">
                <asp:Label runat="server" Text="Data source type:" SkinID="Text" />
            </asp:TableCell>
            <asp:TableCell CssClass="FormField">
                <asp:DropDownList Width="175 px" runat="server" ID="ddlInstanceType" DataTextField="Name" DataValueField="Value" />               
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow Height="20 px">
            <asp:TableCell CssClass="FormLabel">
                <asp:Label runat="server" Text="Driver type:" SkinID="Text" />
            </asp:TableCell>
            <asp:TableCell CssClass="FormField">
                <asp:DropDownList Width="175 px" runat="server" ID="dllDriverType" DataTextField="Name" DataValueField="Value" />               
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow Height="20 px"> 
            <asp:TableCell CssClass="FormLabel">
                <asp:Label runat="server" Text="Host:" SkinID="Text" />
            </asp:TableCell>
            <asp:TableCell CssClass="FormField">
                <asp:TextBox Width="175 px" runat="server" ID="txtHostname" SkinID="TextBox" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtHostname" ToolTip="Host cannot be empty." ErrorMessage="Host cannot be empty." Text="*" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow Height="20 px">
            <asp:TableCell CssClass="FormLabel">
                <asp:Label runat="server" Text="Port:" SkinID="Text" />
            </asp:TableCell>
            <asp:TableCell CssClass="FormField">
                <asp:TextBox Width="175 px" runat="server" ID="txtPort" SkinID="TextBox" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPort" Display="Dynamic" ToolTip="Port cannot be empty." ErrorMessage="Port cannot be empty." Text="*" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="txtPort" Display="Dynamic" ErrorMessage="Port must be numeric." ToolTip="Port must be numeric." ValidationExpression="^[0-9]*$" Text="*" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow Height="20 px">
            <asp:TableCell CssClass="FormLabel">
                <asp:Label runat="server" Text="SID:" SkinID="Text" />
            </asp:TableCell>
            <asp:TableCell CssClass="FormField">
                <asp:TextBox Width="175 px" runat="server" ID="txtSid" SkinID="TextBox" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtSid" ToolTip="SID cannot be empty." ErrorMessage="SID cannot be empty." Text="*" /></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow Height="20 px">
            <asp:TableCell ColumnSpan="2">
                <div id="UpdateProgressDiv" style="z-index: 340; display: none;">
                    <img id="ProgressImage" alt="Processing" src="../../../App_Themes/Common/Images/searching.gif"
                        style="position: absolute; top: 72px; left: 140px; z-index: 340;" />
                </div>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <asp:Table runat="server">
    <asp:TableRow>
        <asp:TableCell  Width="275 px" HorizontalAlign="Center">
            <COEManager:ImageButton runat="server" CausesValidation="true" TypeOfButton="Save" ButtonMode="ImgAndTxt" OnClientClick="SetProgressLayerVisibility();" OnButtonClicked="BtnSave_ButtonClicked"/>&nbsp;&nbsp;
            <COEManager:ImageButton runat="server" CausesValidation="false" TypeOfButton="Cancel" ButtonMode="ImgAndTxt" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" OnButtonClicked="BtnCancel_ButtonClicked" />
        </asp:TableCell>
    </asp:TableRow>
    </asp:Table>
</asp:Panel>
