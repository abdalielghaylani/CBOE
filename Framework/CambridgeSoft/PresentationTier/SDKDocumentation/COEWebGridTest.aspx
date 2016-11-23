<%@ Page Language="C#" EnableEventValidation="true" AutoEventWireup="true" CodeFile="COEWebGridTest.aspx.cs" Inherits="wg2" EnableSessionState="True" %>

<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.COEWebGrid"
    TagPrefix="COECntrl" %>




<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<title>COEWebGrid Sample Application</title>
 <script type="text/javascript" >




    </script>

</head>
<body>
    <form id="form1" runat="server">
        &nbsp;<asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="BindExternally through Dataset" />
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Bind through XML input files" />&nbsp;<br />
        &nbsp;
        <COECntrl:COEWebGrid ID="COEWebGrid1" runat="server" DataViewXml='<?xml version="1.0" encoding="utf-8" ?>&#13;&#10;<COEDataView xmlns="COE.COEDataView" basetable="3" database="CHEMINVDB2" dataviewid="1">&#13;&#10;&#9;<!-- The list of tables -->&#13;&#10;&#9;<tables>&#13;&#10;&#9;&#9;<!-- Aliased table -->&#13;&#10;&#9;&#9;<table id="1" database="CHEMINVDB2" alias="c" name="INV_CONTAINERS"  primaryKey="1">&#13;&#10;&#9;&#9;&#9;<fields id="1" name="CONTAINER_ID" dataType="INTEGER"/>&#13;&#10;&#9;&#9;&#9;<fields id="2" name="BARCODE" dataType="TEXT"/>&#13;&#10;&#9;&#9;&#9;<fields id="3" name="QTY_INITIAL" dataType="INTEGER"/>&#13;&#10;&#9;&#9;&#9;<fields id="4" name="LOCATION_ID_FK" dataType="REAL"/>&#13;&#10;&#9;&#9;&#9;<fields id="5" name="COMPOUND_ID_FK" dataType="INTEGER"/>&#13;&#10;&#9;&#9;&#9;<fields id="6" name="SUPPLIER_ID_FK" dataType="INTEGER"/>&#13;&#10;&#9;&#9;&#9;<fields id="8" name="DATE_EXPIRES" dataType="DATE"/>&#13;&#10;&#9;&#9;&#9;<!-- The following is a lookup -->&#13;&#10;&#9;&#9;&#9;<fields id="9" name="UNIT_OF_MEAS_ID_FK" dataType="INTEGER" lookupFieldId="37" lookupDisplayFieldId="39" alias="UnitOfMeas" />&#13;&#10;&#9;&#9;</table>&#13;&#10;&#9;&#9;<!-- Not aliased table -->&#13;&#10;&#9;&#9;<table id="2" database="CHEMINVDB2" name="INV_LOCATIONS" primaryKey="10">&#13;&#10;&#9;&#9;&#9;<fields id="10" name=" LOCATION_ID" dataType="INTEGER"/>&#13;&#10;&#9;&#9;&#9;<fields id="11" name="LOCATION_BARCODE" dataType="TEXT"/>&#13;&#10;&#9;&#9;&#9;<fields id="12" name="LOCATION_NAME" dataType="TEXT"/>&#13;&#10;&#9;&#9;&#9;<fields id="13" name="LOCATION_DESCRIPTION" dataType="TEXT"/>&#13;&#10;&#9;&#9;</table>&#13;&#10;&#9;&#9;<table id="3" database="CHEMINVDB2" name="INV_COMPOUNDS" primaryKey="14">&#13;&#10;&#9;&#9;&#9;<fields id="14" name="COMPOUND_ID" dataType="INTEGER"/>&#13;&#10;&#9;&#9;&#9;<fields id="15" name="SUBSTANCE_NAME" dataType="TEXT"/>&#13;&#10;&#9;&#9;&#9;<fields id="16" name="BASE64_CDX" dataType="TEXT" indexType="CS_CARTRIDGE"/>&#13;&#10;&#9;&#9;&#9;<fields id="17" name="CAS" dataType="TEXT"/>&#13;&#10;&#9;&#9;&#9;<fields id="28" name="TIMESTAMP" dataType="DATE"/>&#13;&#10;&#9;&#9;</table>&#13;&#10;&#9;&#9;<table id="4" database="CHEMINVDB2" name="INV_SUPPLIERS" primaryKey="18">&#13;&#10;&#9;&#9;&#9;<fields id="18" name="SUPPLIER_ID" dataType="INTEGER"/>&#13;&#10;&#9;&#9;</table>&#13;&#10;&#9;&#9;<table id="5" database="CHEMINVDB2" name="INV_REQUESTS" primaryKey="19">&#13;&#10;&#9;&#9;&#9;<fields id="19" name="CONTAINER_ID_FK" dataType="INTEGER"/>&#13;&#10;&#9;&#9;</table>&#13;&#10;&#9;&#9;<table id="6" database="CHEMINVDB2" name="INV_RESERVATIONS" alias="r" primaryKey="20">&#13;&#10;&#9;&#9;&#9;<fields id="20" name="RESERVATION_ID" dataType="INTEGER"/>&#13;&#10;&#9;&#9;&#9;<fields id="21" name="DATE_RESERVED" dataType="DATE"/>&#13;&#10;&#9;&#9;&#9;<fields id="22" name="QTY_RESERVED" dataType="INTEGER"/>&#13;&#10;&#9;&#9;&#9;<fields id="23" name="USER_ID_FK" dataType="TEXT"/>&#13;&#10;&#9;&#9;&#9;<fields id="24" name="CONTAINER_ID_FK" dataType="INTEGER"/>&#13;&#10;&#9;&#9;</table>&#13;&#10;&#9;&#9;<table id="7" database="CHEMINVDB2" name="INV_SYNONYMS" alias="s"  primaryKey="25">&#13;&#10;&#9;&#9;&#9;<fields id="25" name="SYNONYM_ID" dataType="INTEGER"/>&#13;&#10;&#9;&#9;&#9;<fields id="26" name="COMPOUND_ID_FK" dataType="INTEGER"/>&#13;&#10;&#9;&#9;&#9;<fields id="27" name="SUBSTANCE_NAME" dataType="TEXT"/>&#13;&#10;&#9;&#9;</table>&#13;&#10;&#9;&#9;<table id="8" database="CHEMINVDB2" name="INV_UNITS" primaryKey="37">&#13;&#10;&#9;&#9;&#9;<fields id="37" name="UNIT_ID" dataType="INTEGER" />&#13;&#10;&#9;&#9;&#9;<fields id="38" name="CONVERSION_RATIO" dataType="INTEGER" />&#13;&#10;&#9;&#9;&#9;<fields id="39" name="UNIT_ABREVIATION" dataType="TEXT" />&#13;&#10;&#9;&#9;</table>&#13;&#10;&#9;</tables>&#13;&#10;&#9;<!-- The following is the list of relations -->&#13;&#10;&#9;<relationships>&#13;&#10;&#9;&#9;<relationship child="2" &#13;&#10;&#9;&#9;&#9;&#9;  parent="1"  &#13;&#10;&#9;&#9;&#9;&#9;  childkey="10" &#13;&#10;&#9;&#9;&#9;&#9;  parentkey="4"/>&#13;&#10;&#9;&#9;<!-- Relation between inv_compounds and inv_containers -->&#13;&#10;&#9;&#9;<relationship child="3" &#13;&#10;&#9;&#9;&#9;&#9;  parent="1"  &#13;&#10;&#9;&#9;&#9;&#9;  childkey="14" &#13;&#10;&#9;&#9;&#9;&#9;  parentkey="5"/>&#13;&#10;&#9;&#9;<relationship child="4" &#13;&#10;&#9;&#9;&#9;&#9;  parent="1"  &#13;&#10;&#9;&#9;&#9;&#9;  childkey="18" &#13;&#10;&#9;&#9;&#9;&#9;  parentkey="6"/>&#13;&#10;&#9;&#9;<relationship child="5" &#13;&#10;&#9;&#9;&#9;&#9;  parent="1"  &#13;&#10;&#9;&#9;&#9;&#9;  childkey="19" &#13;&#10;&#9;&#9;&#9;&#9;  parentkey="1"/>&#13;&#10;&#13;&#10;&#9;&#9;<relationship child="6" &#13;&#10;&#9;&#9;&#9;&#9;  parent="1"  &#13;&#10;&#9;&#9;&#9;&#9;  childkey="24" &#13;&#10;&#9;&#9;&#9;&#9;  parentkey="1"&#13;&#10;&#9;&#9;&#9;&#9;  jointype="OUTER" />&#13;&#10;&#9;&#9;<relationship child="7" &#13;&#10;&#9;&#9;&#9;&#9;  parent="3"  &#13;&#10;&#9;&#9;&#9;&#9;  childkey="26" &#13;&#10;&#9;&#9;&#9;&#9;  parentkey="14"&#13;&#10;&#9;&#9;&#9;&#9;  jointype="OUTER" />&#13;&#10;&#9;</relationships>&#13;&#10;</COEDataView>&#13;&#10;'
            Height="702px" PagingInfoXml='<?xml version="1.0" encoding="utf-8" ?>&#13;&#10;<PagingInfo xmlns="COE.PagingInfo">&#13;&#10;  <PagingInfoID>0</PagingInfoID>&#13;&#10;  <HitListID>0</HitListID>&#13;&#10;  <!-- Allowed modes are: NONE, TRANSIENT and PERSISTENT -->&#13;&#10;  <KeepAlive>NONE</KeepAlive>&#13;&#10;  <RecordCount>10</RecordCount>&#13;&#10;  <Start>0</Start>&#13;&#10;  <End>0</End>&#13;&#10;</PagingInfo>'
            ResultCriteriaXml="<?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?>&#13;&#10;<resultsCriteria xmlns=&quot;COE.ResultsCriteria&quot;>&#13;&#10;&#9;<!-- Starts the tables definition -->&#13;&#10;&#9;<tables>&#13;&#10;&#9;&#9;<!-- A table (inv_compounds) -->&#13;&#10;&#9;&#9;<table id=&quot;3&quot;>&#13;&#10;&#9;&#9;&#9;<!-- The compound_id -->&#13;&#10;&#9;&#9;&#9;<field fieldId=&quot;14&quot;/>&#13;&#10;&#9;&#9;&#9;<field fieldId=&quot;15&quot;/>&#13;&#10;&#9;&#9;&#9;<field fieldId=&quot;16&quot;/>&#13;&#10;&#9;&#9;&#9;<!-- If CAS is null, display IS NULL, otherwise display the CAS -->&#13;&#10;&#9;&#9;&#9;<switch inputType=&quot;INTEGER&quot; alias=&quot;CAS&quot;>&#13;&#10;&#9;&#9;&#9;&#9;<clause>&#13;&#10;&#9;&#9;&#9;&#9;&#9;<field fieldId =&quot;17&quot;/>&#13;&#10;&#9;&#9;&#9;&#9;</clause>&#13;&#10;&#9;&#9;&#9;&#9;<condition value=&quot;null&quot;>&#13;&#10;&#9;&#9;&#9;&#9;&#9;<literal>'IS NULL'</literal>&#13;&#10;&#9;&#9;&#9;&#9;</condition>&#13;&#10;&#9;&#9;&#9;&#9;<condition default=&quot;true&quot;>&#13;&#10;&#9;&#9;&#9;&#9;&#9;<field fieldId=&quot;17&quot;/>&#13;&#10;&#9;&#9;&#9;&#9;</condition>&#13;&#10;&#9;&#9;&#9;</switch>&#13;&#10;&#9;&#9;&#9;<field fieldId=&quot;28&quot;/>&#13;&#10;&#9;&#9;&#9;<formula fieldId=&quot;16&quot; alias=&quot;Formula&quot;/>&#13;&#10;&#9;&#9;&#9;<MolWeight fieldId=&quot;16&quot; alias=&quot;MolWeight&quot;/>&#13;&#10;&#9;&#9;&#9;<concatenation alias=&quot;concat&quot;>&#13;&#10;&#9;&#9;&#9;&#9;&#13;&#10;&#9;&#9;&#9;&#9;<literal>'* '</literal>&#13;&#10;&#9;&#9;&#9;&#9;<field fieldId=&quot;14&quot;/>&#13;&#10;&#9;&#9;&#9;</concatenation>&#13;&#10;&#9;&#9;</table>&#13;&#10;&#9;&#9;<table id=&quot;1&quot;>&#13;&#10;&#9;&#9;&#9;<!-- This is a lookup, see dataview definition -->&#13;&#10;&#9;&#9;&#9;<field fieldId=&quot;9&quot;/>&#13;&#10;&#9;&#9;&#9;<switch inputType=&quot;INTEGER&quot; alias=&quot;switch&quot;>&#13;&#10;&#9;&#9;&#9;&#9;<clause>&#13;&#10;&#9;&#9;&#9;&#9;&#9;<!-- Depending on compound_id_fk -->&#13;&#10;&#9;&#9;&#9;&#9;&#9;<field fieldId =&quot;5&quot;/>&#13;&#10;&#9;&#9;&#9;&#9;</clause>&#13;&#10;&#9;&#9;&#9;&#9;<condition value=&quot;2020&quot;>&#13;&#10;&#9;&#9;&#9;&#9;&#9;<!-- if 2020 display the barcode -->&#13;&#10;&#9;&#9;&#9;&#9;&#9;<field fieldId =&quot;2&quot;/>&#13;&#10;&#9;&#9;&#9;&#9;</condition>&#13;&#10;&#9;&#9;&#9;&#9;<condition value=&quot;2021&quot;>&#13;&#10;&#9;&#9;&#9;&#9;&#9;<literal>'Not Assigned'</literal>&#13;&#10;&#9;&#9;&#9;&#9;</condition>&#13;&#10;&#9;&#9;&#9;&#9;<condition default=&quot;true&quot;>&#13;&#10;&#9;&#9;&#9;&#9;&#9;<field fieldId=&quot;3&quot;/>&#13;&#10;&#9;&#9;&#9;&#9;</condition>&#13;&#10;&#9;&#9;&#9;</switch>&#13;&#10;&#9;&#9;</table>&#13;&#10;&#9;</tables>&#13;&#10;</resultsCriteria>&#13;&#10;&#13;&#10;"
            SearchCriteriaXml='<?xml version="1.0" encoding="utf-8" ?>&#13;&#10;<searchCriteria xmlns="COE.SearchCriteria">&#13;&#10;  <searchCriteriaItem id="1"  &#13;&#10;&#9;&#9;&#9;&#9;&#9; fieldid="14" &#13;&#10;&#9;&#9;&#9;&#9;&#9; modifier="" &#13;&#10;&#9;&#9;&#9;&#9;&#9; tableid="3">&#13;&#10;&#9;&#9;<numericalCriteria operator="GTE" trim="BOTH">100</numericalCriteria>&#13;&#10;&#9;</searchCriteriaItem>  &#13;&#10;  <searchCriteriaID>1</searchCriteriaID>&#13;&#10;</searchCriteria>'
            Width="549px">
            <Bands>
                <igtbl:UltraGridBand>
                    <addnewrow view="NotSet" visible="NotSet"></addnewrow>
                </igtbl:UltraGridBand>
            </Bands>
            <DisplayLayout BorderCollapseDefault="Separate" RowHeightDefault="20px" Version="4.00">
                <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                </FooterStyleDefault>
                <RowStyleDefault BackColor="White" BorderColor="Gray" BorderStyle="Solid" BorderWidth="1px">
                    <BorderDetails ColorLeft="White" ColorTop="White" />
                    <Padding Left="3px" />
                </RowStyleDefault>
                <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid">
                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                </HeaderStyleDefault>
                <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                </EditCellStyleDefault>
                <FrameStyle BorderStyle="Solid" BorderWidth="1px" Font-Names="Verdana" Font-Size="8pt"
                    Height="702px" Width="549px">
                </FrameStyle>
                <Pager>
                    <Style BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
</Style>
                </Pager>
                <AddNewBox>
                    <Style BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
</Style>
                </AddNewBox>
            </DisplayLayout>
        </COECntrl:COEWebGrid>
    
    
    </form>
    
</body>

</html>