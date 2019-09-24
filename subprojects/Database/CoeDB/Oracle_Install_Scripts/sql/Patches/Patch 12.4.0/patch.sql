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
@"sql\Patches\Patch 12.4.0\evolveSchema.sql"

--change to xmltype
ALTER TABLE COEDATABASE ADD COEDATAVIEW_XMLTYPE  XMLTYPE
  XMLTYPE COEDATAVIEW_XMLTYPE STORE AS OBJECT RELATIONAL XMLSCHEMA "http://cambridgesoft.com/coedataview.xsd"
  ELEMENT "COEDataView";

UPDATE COEDATABASE SET COEDATAVIEW_XMLTYPE = XMLTYPE(COEDATAVIEW);

ALTER TABLE COEDATABASE RENAME COLUMN  COEDATAVIEW TO COEDATAVIEW_OLD;
ALTER TABLE COEDATABASE RENAME COLUMN  COEDATAVIEW_XMLTYPE TO COEDATAVIEW;
ALTER TABLE COEDATABASE DROP COLUMN COEDATAVIEW_OLD;

CREATE GLOBAL TEMPORARY TABLE COECURRENTPAGE(BASETABLEPRIMARYKEY NUMBER(9) PRIMARY KEY, SORTORDER FLOAT(126)) ON COMMIT DELETE ROWS;
GRANT SELECT, INSERT, UPDATE ON COECURRENTPAGE TO CSS_USER;

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

--List of all tags in all tables
create or replace view DVTableTags As
select id DVID, t.TableID, tag.tag
	from &&schemaName..coedataview x,
	     XMLTable(XMLNAMESPACES('COE.COEDataView' as "DV"),
        	      '/DV:COEDataView/DV:tables/DV:table'
	               PASSING x.coedataview
	               COLUMNS
	               TableID NUMBER(9,0) PATH '@id',
	               tagXML  XMLTYPE  PATH 'DV:tags') t ,
	      XMLTable(XMLNAMESPACES('COE.COEDataView' as "DV"),
	              '/DV:tags/DV:tag'
	               PASSING t.tagXML
	               COLUMNS
	               Tag VARCHAR(4000) PATH 'text()') tag  
	where rownum<1000000;

------------------------------ Master Dataview -------------------------------
-- Get the list of all tables from the MasterDataview
------------------------------------------------------------------------------
create or replace view MDVTables As
Select id DVID, t.TableID, t.TableName, t.TableAlias, t.TableDB, t.TablePK, t.IsView
from &&schemaName..coedataview x,
     XMLTable(XMLNAMESPACES('COE.COEDataView' as "DV"),
              '/DV:COEDataView/DV:tables/DV:table'
               PASSING x.coedataview
               COLUMNS
               TableID NUMBER(9,0) PATH '@id',
               TableName VARCHAR2(100) PATH '@name',
               TableAlias VARCHAR2(100) PATH '@alias',
               TableDB VARCHAR2(100) PATH '@database',
               TablePK VARCHAR2(100) PATH '@primaryKey',
               IsView VARCHAR2(10) PATH '@isView') t
where x.id = 0;

------------------------- Published Dataview Schemas -------------------------
-- Get the list of all tables from the MasterDataview
------------------------------------------------------------------------------
create or replace view PublishedDVTables As
Select id DVID, t.TableID, t.TableName, t.TableAlias, t.TableDB, t.TablePK, t.IsView
from &&schemaName..coedatabase x,
     XMLTable(XMLNAMESPACES('COE.COEDataView' as "DV"),
              '/DV:COEDataView/DV:tables/DV:table'
               PASSING x.coedataview
               COLUMNS
               TableID NUMBER(9,0) PATH '@id',
               TableName VARCHAR2(100) PATH '@name',
               TableAlias VARCHAR2(100) PATH '@alias',
               TableDB VARCHAR2(100) PATH '@database',
               TablePK VARCHAR2(100) PATH '@primaryKey',
               IsView VARCHAR2(10) PATH '@isView') t;

---------- Get the list of all fields across all dataviews (including the parent table info so it can be used for joins) ----------
create or replace Force view PublishedDVFields 
("DVID","TableID","TableName","TableAlias","TablePK","fieldXML","FieldID","FieldName","FieldAlias","FieldIndex","FieldMime","FieldLookupID","FieldLookupDisplay")
As
Select id DVID, t.TableID, t.TableName, t.TableAlias, f.FieldID, f.FieldName, f.FieldAlias, f.FieldIndex, f.FieldMime, DECODE(t.TablePK, f.FieldID, 1,0) IsPrimaryKey, f.FieldLookupID, f.FieldLookupDisplay
from &&schemaName..coedatabase x,
     XMLTable(XMLNAMESPACES('COE.COEDataView' as "DV"),
              '/DV:COEDataView/DV:tables/DV:table'
               PASSING x.coedataview
               COLUMNS
               TableID NUMBER(9,0) PATH '@id',
               TableName VARCHAR2(100) PATH '@name',
               TableAlias VARCHAR2(100) PATH '@alias',
               TablePK NUMBER(9,0) PATH '@primaryKey',
               fieldXML  XMLTYPE  PATH '/DV:table') t,
            XMLTable(XMLNAMESPACES('COE.COEDataView' as "DV"),
              '/DV:table/DV:fields'
               PASSING t.fieldXML
               COLUMNS
               FieldID NUMBER(9,0) PATH '@id',
               FieldName VARCHAR2(100) PATH '@name',
               FieldAlias VARCHAR2(100) PATH '@alias',
               FieldIndex VARCHAR2(100) PATH '@indexType',
               FieldMime VARCHAR2(100) PATH '@mimeType',
               FieldLookupID NUMBER(9,0) PATH '@lookupFieldId',
               FieldLookupDisplay NUMBER(9,0) PATH '@lookupDisplayFieldId') f;
---------- Get the list of relationships and the tables/field info ----------
Create or Replace Force View PublishedDVRelationships 
("DVID","ParentTableID","ParentKeyID","ChildTableID","ChildKeyID","JoinType",
"TableID_PT","TableName_PT","TableAlias_PT","TablePK_PT","fieldXML_PT",
"TableID","TableName","TableAlias","TablePK","fieldXML","FieldID","FieldName","FieldAlias",
"FieldID_FC","FieldName_FC","FieldAlias_FC")
As
Select id DVID, r.ParentTableID, r.ParentKeyID, pt.TableName_PT ParentTable, 
pt.TableAlias_PT ParentAlias, fp.FieldName ParentFieldName,
		fp.FieldAlias ParentFieldAlias, r.ChildTableID, r.ChildKeyID,
    ct.TableName ChildTable, ct.TableAlias ChildAlias, 
    fc.FieldName_FC ChildFieldName, fc.FieldAlias_FC ChildFieldAlias, r.JoinType
from &&schemaName..coedatabase x,
     XMLTable(XMLNAMESPACES('COE.COEDataView' as "DV"),
              '/DV:COEDataView/DV:relationships/DV:relationship'
               PASSING x.coedataview
               COLUMNS
               ParentTableID NUMBER(9,0) PATH '@parent',
               ParentKeyID VARCHAR2(100) PATH '@parentkey',
               ChildTableID VARCHAR2(100) PATH '@child',
               ChildKeyID VARCHAR2(100) PATH '@childkey',
               JoinType VARCHAR2(100) PATH '@jointype') r,
            XMLTable(XMLNAMESPACES('COE.COEDataView' as "DV"),
              '/DV:COEDataView/DV:tables/DV:table'
               PASSING x.coedataview
               COLUMNS
               TableID_PT NUMBER(9,0) PATH '@id',
               TableName_PT VARCHAR2(100) PATH '@name',
               TableAlias_PT VARCHAR2(100) PATH '@alias',
               TablePK_PT NUMBER(9,0) PATH '@primaryKey',
               fieldXML_PT  XMLTYPE  PATH '/DV:table') pt,
             XMLTable(XMLNAMESPACES('COE.COEDataView' as "DV"),
              '/DV:COEDataView/DV:tables/DV:table'
               PASSING x.coedataview
               COLUMNS
               TableID NUMBER(9,0) PATH '@id',
               TableName VARCHAR2(100) PATH '@name',
               TableAlias VARCHAR2(100) PATH '@alias',
               TablePK NUMBER(9,0) PATH '@primaryKey',
               fieldXML  XMLTYPE  PATH '/DV:table') ct,
            XMLTable(XMLNAMESPACES('COE.COEDataView' as "DV"),
              '/DV:table/DV:fields'
               PASSING pt.fieldXML
               COLUMNS
               FieldID NUMBER(9,0) PATH '@id',
               FieldName VARCHAR2(100) PATH '@name',
               FieldAlias VARCHAR2(100) PATH '@alias') fp,
			XMLTable(XMLNAMESPACES('COE.COEDataView' as "DV"),
              '/DV:table/DV:fields'
               PASSING ct.fieldXML
               COLUMNS
               FieldID_FC NUMBER(9,0) PATH '@id',
               FieldName_FC VARCHAR2(100) PATH '@name',
               FieldAlias_FC VARCHAR2(100) PATH '@alias') fc
WHERE r.ParentTableID = pt.TableID AND r.ChildTableID=ct.TableID AND r.ParentKeyID = fp.FieldID AND r.ChildKeyID = fc.FieldID;

--#########################################################
--PROCEDURES AND FUNCTIONS
--#########################################################

--#########################################################
--PACKAGES
--#########################################################

@"sql\Patches\Patch &&currentPatch\Packages\package coedb.biosar_utils.sql"
@"sql\Patches\Patch &&currentPatch\Packages\packagebody coedb.biosar_utils.sql"

--#########################################################
--DATA
--#########################################################

--COMMIT;

--#####################################################################
-- COEManager PageControlSettings
--#####################################################################


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











