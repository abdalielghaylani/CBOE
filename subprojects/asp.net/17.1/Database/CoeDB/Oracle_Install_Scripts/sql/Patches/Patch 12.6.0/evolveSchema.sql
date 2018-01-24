-- ### Updating Dataview Schema ###

ALTER TABLE COEDATABASE ADD COEDATAVIEW_CLOB CLOB;
ALTER TABLE COEDATAVIEW ADD COEDATAVIEW_CLOB CLOB;

UPDATE COEDATAVIEW D SET D.COEDATAVIEW_CLOB = D.COEDATAVIEW.GETCLOBVAL();
COMMIT;

UPDATE COEDATABASE E SET E.COEDATAVIEW_CLOB = E.COEDATAVIEW.GETCLOBVAL();
COMMIT;

ALTER TABLE COEDATAVIEW DROP COLUMN COEDATAVIEW;
ALTER TABLE COEDATABASE DROP COLUMN COEDATAVIEW;

BEGIN
  DBMS_XMLSCHEMA.deleteSchema(SCHEMAURL => 'http://cambridgesoft.com/coedataview.xsd', DELETE_OPTION => DBMS_XMLSCHEMA.DELETE_CASCADE_FORCE);
END;
/
COMMIT;
DECLARE
	doc varchar2(32767);
BEGIN
	doc:='<?xml version="1.0" encoding="utf-8"?>
<xs:schema xmlns:tns="COE.COEDataView" elementFormDefault="qualified"
targetNamespace="COE.COEDataView"
xmlns:xs="http://www.w3.org/2001/XMLSchema">
	<xs:element name="COEDataView" nillable="true" type="tns:COEDataView" />
	<xs:complexType name="COEDataView">
		<xs:sequence>
			<xs:element minOccurs="0" maxOccurs="1" name="tables" type="tns:ArrayOfCOEDataViewTable" />
			<xs:element minOccurs="0" maxOccurs="1" name="relationships" type="tns:ArrayOfRelationship" />
		</xs:sequence>
		<xs:attribute name="basetable" type="xs:int" use="required" />
		<xs:attribute name="database" type="xs:string" />
		<xs:attribute name="dataviewid" type="xs:int" use="required" />
		<xs:attribute name="name" type="xs:string" />
		<xs:attribute name="application" type="xs:string" />
		<xs:attribute name="description" type="xs:string" />
		<xs:attribute name="dataviewHandling" type="tns:DataViewHandlingOptions" />
	</xs:complexType>
	<xs:complexType name="ArrayOfCOEDataViewTable">
		<xs:sequence>
			<xs:element minOccurs="0" maxOccurs="unbounded" name="table" nillable="true" type="tns:COEDataView.Table" />
		</xs:sequence>
	</xs:complexType>
	<xs:complexType name="COEDataView.Table">
		<xs:sequence>
			<xs:element minOccurs="0" maxOccurs="unbounded" name="fields" type="tns:fields" />
			<xs:element name="tags" minOccurs="0" maxOccurs="1" type="tns:tag" />
		</xs:sequence>
		<xs:attribute name="id" type="xs:int" use="required" />
		<xs:attribute name="name" type="xs:string" />
		<xs:attribute name="alias" type="xs:string" />
		<xs:attribute name="database" type="xs:string" />
		<xs:attribute name="primaryKey" type="xs:string" />
		<xs:attribute default="false" name="isView" type="xs:boolean" />
	</xs:complexType>
	<xs:complexType name="fields">
		<xs:attribute name="id" type="xs:int" use="required" />
		<xs:attribute name="name" type="xs:string" />
		<xs:attribute name="dataType" type="tns:AbstractTypes" use="required" />
		<xs:attribute default="-1" name="lookupFieldId" type="xs:int" />
		<xs:attribute default="-1" name="lookupDisplayFieldId" type="xs:int" />
		<xs:attribute default="" name="alias" type="xs:string" />
		<xs:attribute name="indexType" type="tns:IndexTypes" />
		<xs:attribute name="mimeType" type="tns:MimeTypes" />
		<xs:attribute default="true" name="visible" type="xs:boolean" />
		<xs:attribute default="false" name="isDefault" type="xs:boolean" />
		<xs:attribute default="ASCENDING" name="lookupSortOrder" type="tns:SortDirection" />
		<xs:attribute default="-1" name="sortOrder" type="xs:int" />
		<xs:attribute default="false" name="isUniqueKey" type="xs:boolean" />
		<xs:attribute default="false" name="isDefaultQuery" type="xs:boolean" />
		<xs:attribute default="false" name="isIndexed" type="xs:boolean" />
		<xs:attribute name="indexname" type="xs:string" />
	</xs:complexType>
	<xs:simpleType name="AbstractTypes">
		<xs:restriction base="xs:string">
			<xs:enumeration value="INTEGER" />
			<xs:enumeration value="REAL" />
			<xs:enumeration value="TEXT" />
			<xs:enumeration value="DATE" />
			<xs:enumeration value="BOOLEAN" />
			<xs:enumeration value="BLOB" />
			<xs:enumeration value="CLOB" />
	</xs:restriction>
	</xs:simpleType>
	<xs:simpleType name="IndexTypes">
		<xs:restriction base="xs:string">
			<xs:enumeration value="NONE" />
			<xs:enumeration value="UNKNOWN" />
			<xs:enumeration value="CS_CARTRIDGE" />
			<xs:enumeration value="FULL_TEXT" />
		</xs:restriction>
	</xs:simpleType>
	<xs:simpleType name="MimeTypes">
		<xs:restriction base="xs:string">
			<xs:enumeration value="NONE" />
			<xs:enumeration value="UNKNOWN" />
			<xs:enumeration value="IMAGE_JPEG" />
			<xs:enumeration value="IMAGE_GIF" />
			<xs:enumeration value="IMAGE_PNG" />
			<xs:enumeration value="IMAGE_X_WMF" />
			<xs:enumeration value="CHEMICAL_X_MDLMOLFILE" />
			<xs:enumeration value="CHEMICAL_X_CDX" />
			<xs:enumeration value="CHEMICAL_X_SMILES" />
			<xs:enumeration value="TEXT_XML" />
			<xs:enumeration value="TEXT_HTML" />
			<xs:enumeration value="TEXT_PLAIN" />
			<xs:enumeration value="TEXT_RAW" />
			<xs:enumeration value="APPLICATION_MS_EXCEL" />
			<xs:enumeration value="APPLICATION_MS_MSWORD" />
			<xs:enumeration value="APPLICATION_PDF" />
		</xs:restriction>
	</xs:simpleType>
	<xs:simpleType name="SortDirection">
		<xs:restriction base="xs:string">
			<xs:enumeration value="ASCENDING" />
			<xs:enumeration value="DESCENDING" />
		</xs:restriction>
	</xs:simpleType>
	<xs:complexType name="ArrayOfRelationship">
		<xs:sequence>
			<xs:element minOccurs="0" maxOccurs="unbounded" name="relationship" nillable="true" type="tns:relationship" />
		</xs:sequence>
	</xs:complexType>
	<xs:complexType name="relationship">
		<xs:attribute name="parentkey" type="xs:int" use="required" />
		<xs:attribute name="childkey" type="xs:int" use="required" />
		<xs:attribute name="parent" type="xs:int" use="required" />
		<xs:attribute name="child" type="xs:int" use="required" />
		<xs:attribute name="jointype" type="tns:JoinTypes" use="required" />
	</xs:complexType>
	<xs:simpleType name="JoinTypes">
		<xs:restriction base="xs:string">
			<xs:enumeration value="OUTER" />
			<xs:enumeration value="INNER" />
		</xs:restriction>
	</xs:simpleType>
	<xs:simpleType name="DataViewHandlingOptions">
		<xs:restriction base="xs:string">
			<xs:enumeration value="USE_CLIENT_DATAVIEW" />
			<xs:enumeration value="USE_SERVER_DATAVIEW" />
			<xs:enumeration value="MERGE_CLIENT_AND_SERVER_DATAVIEW" />
		</xs:restriction>
	</xs:simpleType>
	<xs:complexType name="tag">
		<xs:sequence>
			<xs:element name="tag" type="xs:string" maxOccurs="unbounded" nillable="true" />
		</xs:sequence>
	</xs:complexType>
</xs:schema>';

	dbms_xmlschema.registerSchema('http://cambridgesoft.com/coedataview.xsd', doc,
                              true, true, false, true);
END;
/
COMMIT;
ALTER TABLE COEDATAVIEW ADD COEDATAVIEW  XMLTYPE
	XMLTYPE COEDATAVIEW STORE AS OBJECT RELATIONAL XMLSCHEMA "http://cambridgesoft.com/coedataview.xsd"
	ELEMENT "COEDataView";

ALTER TABLE COEDATABASE ADD COEDATAVIEW  XMLTYPE
	XMLTYPE COEDATAVIEW STORE AS OBJECT RELATIONAL XMLSCHEMA "http://cambridgesoft.com/coedataview.xsd"
	ELEMENT "COEDataView";

UPDATE COEDATAVIEW SET COEDATAVIEW = XMLTYPE(COEDATAVIEW_CLOB);
COMMIT;

UPDATE COEDATABASE SET COEDATAVIEW = XMLTYPE(COEDATAVIEW_CLOB);
COMMIT;

ALTER TABLE COEDATAVIEW DROP COLUMN COEDATAVIEW_CLOB;
ALTER TABLE COEDATABASE DROP COLUMN COEDATAVIEW_CLOB;


