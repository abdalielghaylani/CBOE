<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ConfigurationSectionsUC.ascx.cs"
    Inherits="RegistrationWebApp.Forms.RegistrationAdmin.UserControls.ConfigurationSectionsUC" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebGrid.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebTab.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebTab" TagPrefix="igtab" %>
<igtab:UltraWebTab ID="AppSettingsUltraWebTab" runat="server" SkinID="AppSettingsUltraWebTabSkin">     
    <Tabs>
        <igtab:Tab Key="CBVTabItem">
            <ContentTemplate>
                <igtbl:UltraWebGrid ID="CBVUltraWebGrid" runat="server" SkinID="AppSettingsUltraWebGridSkin">
                </igtbl:UltraWebGrid>
            </ContentTemplate>
        </igtab:Tab>
        <igtab:Tab Key="DOCMGRTabItem">
            <ContentTemplate>
                <igtbl:UltraWebGrid ID="DOCMGRUltraWebGrid" runat="server" SkinID="AppSettingsUltraWebGridSkin">
                </igtbl:UltraWebGrid>
            </ContentTemplate>
        </igtab:Tab>
        <igtab:Tab Key="INVENTORYTabItem">
            <ContentTemplate>
                <igtbl:UltraWebGrid ID="INVENTORYUltraWebGrid" runat="server" SkinID="AppSettingsUltraWebGridSkin">
                </igtbl:UltraWebGrid>
            </ContentTemplate>
        </igtab:Tab>
        <igtab:Tab Key="MISCTabItem">
            <ContentTemplate>
                <igtbl:UltraWebGrid ID="MISCUltraWebGrid" runat="server" SkinID="AppSettingsUltraWebGridSkin">
                </igtbl:UltraWebGrid>
            </ContentTemplate>
        </igtab:Tab>
        <igtab:Tab Key="REGADMINTabItem">
            <ContentTemplate>
                <igtbl:UltraWebGrid ID="REGADMINUltraWebGrid" runat="server" SkinID="AppSettingsUltraWebGridSkin">
                </igtbl:UltraWebGrid>
            </ContentTemplate>
        </igtab:Tab>
        <igtab:Tab Key="ERRORCONTROLTabItem">
            <ContentTemplate>
                <igtbl:UltraWebGrid ID="ERRORCONTROLUltraWebGrid" runat="server" SkinID="AppSettingsUltraWebGridSkin">
                </igtbl:UltraWebGrid>
            </ContentTemplate>
        </igtab:Tab>
    </Tabs>
</igtab:UltraWebTab>
