<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCompound.ascx.cs" Inherits="PerkinElmer.COE.Registration.Server.Forms.Public.UserControls.SearchCompound" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<input type="hidden" runat="server" id="CompoundIDHidden" />    
<table id="CompundInfoTable" runat="server" style="display:block">
    <tr>
        <td>
            <ignav:UltraWebTree ID="UltraWebTreeControl" runat="server" ImageDirectory="">
                <SelectedNodeStyle ForeColor="Black" BackColor="#CCCCCC"></SelectedNodeStyle>
                <NodePaddings Bottom="2px" Left="2px" Top="2px" Right="2px"></NodePaddings>
                <Padding Top="6px"></Padding>
                <Levels>
                    <ignav:Level Index="0" LevelCheckBoxes="False"></ignav:Level>
                    <ignav:Level Index="1"></ignav:Level>
                </Levels>   
            </ignav:UltraWebTree>
        </td>
    </tr>
</table>
<table id="SearchTitleTextTable">
    <tr>
        <td colspan="2" align="left">
            <img id="DisplaySearchTableImage"  alt="" src="" width="16" height="16" style="cursor:pointer" runat="server"/>
            <asp:Label runat="server" ID="SearchTitleLabel" CssClass="UCTitlesText"></asp:Label>
        </td>
    </tr>
</table>
<table id="SearchControlsTable" runat="server" style="display:block">
    <tr>
        <td align="right">
            <asp:Label runat="server" ID="RegistrationNumberLabel"></asp:Label>
        </td>
        <td>
            <asp:TextBox runat="server" ID="RegistrationNumberTextBox"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td align="right">
            <asp:Label runat="server" ID="SecuenceNumberLabel"></asp:Label>
        </td>
        <td>
            <asp:TextBox runat="server" ID="SecuenceNumberTextBox"></asp:TextBox>
        </td>
    </tr>
     <tr>
        <td align="right">
            <asp:Label runat="server" ID="IDLabel"></asp:Label>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IDTextBox"></asp:TextBox>
        </td>
    </tr>
     <tr>
        <td>&nbsp;</td>
        <td align="left">
             <asp:Button ID="SearchButton" runat="server" CommandName="Search" OnClick="SearchButton_Click"/>
        </td>
    </tr>
</table>
<table id="CompoundActionsTable">
    <tr>
        <td colspan="2">&nbsp;</td>
    </tr>
    <tr>
        <td>
            <asp:Button ID="UpdateButton" runat="server" CommandName="Update" OnClick="UpdateButton_Click"/>
        </td>
        <td align="left">
            <asp:Button ID="AddBatchButton" runat="server" CommandName="AddBatch" OnClick="AddBatchButton_Click"/>
        </td>
    </tr>  
</table>
