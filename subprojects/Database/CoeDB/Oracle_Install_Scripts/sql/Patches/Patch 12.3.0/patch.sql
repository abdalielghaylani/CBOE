--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"sql\Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 
Connect &&schemaName/&&schemaPass@&&serverName

--#########################################################
--TABLES
--######################################################### 

-- Make COEDATAVIEW COLUMN OF TYPE XMLTYPE
declare
doc varchar2(32767);
begin
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
	</xs:complexType>
	<xs:simpleType name="AbstractTypes">
		<xs:restriction base="xs:string">
			<xs:enumeration value="INTEGER" />
			<xs:enumeration value="REAL" />
			<xs:enumeration value="TEXT" />
			<xs:enumeration value="DATE" />
			<xs:enumeration value="BOOLEAN" />
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
</xs:schema>';

dbms_xmlschema.registerSchema('http://cambridgesoft.com/coedataview.xsd', doc,
                              true, true, false, true);
COMMIT;
end;

/

--change to xmltype
ALTER TABLE COEDATAVIEW ADD COEDATAVIEW_XMLTYPE  XMLTYPE
  XMLTYPE COEDATAVIEW_XMLTYPE STORE AS OBJECT RELATIONAL XMLSCHEMA "http://cambridgesoft.com/coedataview.xsd"
  ELEMENT "COEDataView";

UPDATE COEDATAVIEW SET COEDATAVIEW_XMLTYPE = XMLTYPE(COEDATAVIEW);

ALTER TABLE COEDATAVIEW RENAME COLUMN  COEDATAVIEW TO COEDATAVIEW_OLD;
ALTER TABLE COEDATAVIEW RENAME COLUMN  COEDATAVIEW_XMLTYPE TO COEDATAVIEW;
ALTER TABLE COEDATAVIEW DROP COLUMN COEDATAVIEW_OLD;

--#####################################################################
--Export template tables
--#####################################################################

@"sql\Patches\Patch &&currentPatch\sql\ExportTemplates.sql"

--#########################################################
--SEQUENCES
--#########################################################

--#########################################################
--TRIGGERS
--#########################################################

--#########################################################
--INDEXES
--#########################################################

--#########################################################
--CONSTRAINTS
--#########################################################

--#########################################################
--VIEWS
--#########################################################

--#########################################################
--PROCEDURES AND FUNCTIONS
--#########################################################

create or replace FUNCTION createCOERole (
   pRoleName            IN   security_roles.role_name%TYPE,
   pPrivTableName       IN   privilege_tables.privilege_table_name%TYPE,
   pIsAlreadyInOracle   IN   INTEGER,
   pPrivValueList       IN   VARCHAR2,
   pCOEIdentifier       IN   security_roles.coeidentifier%TYPE)
   RETURN VARCHAR2
AS
   privTableId   INTEGER;
   roleId        INTEGER;
BEGIN

   IF pIsAlreadyInOracle = 0 THEN
      SELECT privilege_table_id
        INTO privTableId
        FROM privilege_tables
       WHERE UPPER (privilege_table_name) = UPPER (pPrivTableName);

      INSERT INTO security_roles (privilege_table_int_id, role_name, coeidentifier)
           VALUES (privTableId, UPPER (pRoleName), UPPER (pCOEIdentifier))
        RETURNING role_id
             INTO roleId;

      EXECUTE IMMEDIATE 'INSERT INTO ' || pPrivTableName || ' VALUES ( ' || roleId || ', ' || pPrivValueList || ')';
      EXECUTE IMMEDIATE 'CREATE ROLE ' || pRoleName || ' NOT IDENTIFIED';
	    EXECUTE IMMEDIATE 'GRANT EXECUTE ON COEDB.LOGIN TO ' || pRoleName;
      --EXECUTE IMMEDIATE 'GRANT CSS_USER TO ' || pRoleName;
      EXECUTE IMMEDIATE 'REVOKE ' || pRoleName || ' FROM COEDB';
      GrantPrivsToRole2(pRoleName); --CSBR 127261 : To fix the issue related to new role creation and assigning the role to a new user.
   ELSE
      INSERT INTO security_roles (privilege_table_int_id, role_name, coeidentifier)
           VALUES (NULL, UPPER (pRoleName), UPPER (pCOEIdentifier))
        RETURNING role_id
             INTO roleId;
   END IF;

   RETURN '1';
END createCOERole;
/

-- To ensure WhereCluaseIN can be used
GRANT EXECUTE ON MYTABLETYPE TO CSS_ADMIN;
GRANT EXECUTE ON MYTABLETYPE TO CSS_USER;

-- To ensure database change notification can be used by the caching mechanism
GRANT CHANGE NOTIFICATION TO COEDB;

--#########################################################
--PACKAGES
--#########################################################

@"sql\Patches\Patch &&currentPatch\Packages\pkg_ConfigurationManager.sql"

--#########################################################
--DATA
--#########################################################

--COMMIT;


--#####################################################################
-- COEManager PageControlSettings
--#####################################################################

--@"sql\Patches\Patch &&currentPatch\sql\xxxxx"


UPDATE &&schemaName..CoeGlobals
	SET Value = '&&currentPatch' 
	WHERE UPPER(ID) = 'SCHEMAVERSION';

UPDATE &&schemaName..CoeGlobals
	SET Value = '&&versionApp' 
	WHERE UPPER(ID) = 'VERSION_APP';
COMMIT;

prompt **** Patch &&currentPatch Applied ****

COL setNextPatch NEW_VALUE setNextPatch NOPRINT
SELECT	CASE
		WHEN  '&&toVersion'='&&currentPatch'
		THEN  'sql\Patches\stop.sql'
		ELSE  '"sql\Patches\Patch &&nextPatch\patch.sql"'
	END	AS setNextPatch 
FROM	DUAL;

@&&setNextPatch 









