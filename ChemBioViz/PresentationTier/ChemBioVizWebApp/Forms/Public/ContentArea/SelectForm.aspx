<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectForm.aspx.cs" Inherits="Forms_ContentArea_SelectForm" 
EnableViewState="true" MasterPageFile="~/Forms/Master/MasterPage.master" Async="true"%>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebToolbar.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebToolbar" TagPrefix="igtbar" %>    
<%@ Register Src="../UserControls/ErrorArea.ascx" TagName="ErrorArea" TagPrefix="ChemBioViz" %>
<%@ Register Src="../UserControls/ConfirmationArea.ascx" TagName="ConfirmationArea" TagPrefix="ChemBioViz" %>
<%@ MasterType VirtualPath="~/Forms/Master/MasterPage.master" %>
<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server" >
    <table class="PagesContentTable">
        <tr>
            <td align="center">
                <ChemBioViz:ConfirmationArea ID="ConfirmationAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <ChemBioViz:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false"/>
            </td>
        </tr>
       <tr>
        <td>
            <table>
                <tr>
                    <td>
						<asp:PlaceHolder ID="FormListPH" runat="server"></asp:PlaceHolder>	
                    </td>
                </tr>
            </table>
        </td>
       </tr>
    </table>

	<asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" AllowSorting="false" CellPadding="2">
		
		<Columns> 
			<asp:TemplateField HeaderText="Name" SortExpression="Name">
			  <ItemTemplate>
				<asp:HyperLink ID="hypName" runat="server" Text='<%# (Eval("Name")) %>' NavigateUrl='<%# "~/Forms/Search/ContentArea/ChemBioVizSearch.aspx?AllowFullScan=true&FormGroupId=" + (Eval("ID")) %>'></asp:HyperLink>
			  </ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Description" SortExpression="Description">
			  <ItemTemplate>
				<asp:Label ID="lblDescription" runat="server" Text='<%# (Eval("Description")) %>'></asp:Label>
			  </ItemTemplate>     
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Database" SortExpression="Database">
			  <ItemTemplate>
				<asp:Label ID="lblDatabase" runat="server" Text='<%# (Eval("DatabaseName")) %>'></asp:Label>
			  </ItemTemplate>     
			</asp:TemplateField>			
			<asp:TemplateField HeaderText="Public" SortExpression="Public">
			  <ItemTemplate>
				<asp:CheckBox ID="chkisPublic" runat="server" Checked='<%# (Eval("isPublic")) %>' Enabled="false" />
			  </ItemTemplate>     
			</asp:TemplateField>			
		</Columns>
	
		<HeaderStyle BackColor="#0099ff"  Font-Bold="true" forecolor="white" Font-Names="Verdana" Font-Size="10pt" BorderColor="#0074B9" />
		<RowStyle BackColor="white" Font-Names="Verdana" Font-Size="10pt" BorderWidth="1px" BorderColor="#0074B9" />
		<AlternatingRowStyle BackColor="#D7D9D7" Font-Names="Verdana" Font-Size="10pt" BorderWidth="1px" BorderColor="#0074B9"  />
		
	</asp:GridView>
	
	
</asp:Content>

