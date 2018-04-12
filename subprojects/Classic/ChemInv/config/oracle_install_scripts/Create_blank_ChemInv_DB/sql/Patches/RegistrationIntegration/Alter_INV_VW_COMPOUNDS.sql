PROMPT Starting Alter_INV_VW_COMPOUNDS.sql
Connect &&InstallUser/&&sysPass@&&serverName

GRANT ALL ON &&regSchemaName..REG_NUMBERS TO &&schemaName;
GRANT ALL ON &&regSchemaName..MIXTURES TO &&schemaName;
GRANT SELECT on &&regSchemaName..VW_BATCH to &&schemaName with grant option;
GRANT SELECT on &&regSchemaName..BATCHES to &&schemaName;

--Create materialized view logs (for inv_vw_compounds)
DROP MATERIALIZED VIEW LOG ON &&regSchemaName..REG_NUMBERS;

CREATE MATERIALIZED VIEW LOG ON &&regSchemaName..REG_NUMBERS
TABLESPACE T_&&regSchemaName._TABL
WITH ROWID;

DROP MATERIALIZED VIEW LOG ON &&regSchemaName..MIXTURES;

CREATE MATERIALIZED VIEW LOG ON &&regSchemaName..MIXTURES
TABLESPACE T_&&regSchemaName._TABL
WITH ROWID; 

DROP MATERIALIZED VIEW LOG ON &&regSchemaName..BATCHES;

CREATE MATERIALIZED VIEW LOG ON &&regSchemaName..BATCHES
TABLESPACE T_&&regSchemaName._TABL
WITH ROWID; 

grant all on &&regSchemaName..MLOG$_MIXTURES to &&schemaName.;
grant all on &&regSchemaName..MLOG$_REG_NUMBERS to &&schemaName.;
grant all on &&regSchemaName..MLOG$_BATCHES to &&schemaName.;

--Drop the original view created by Inventory
DROP VIEW &&schemaName..INV_VW_COMPOUNDS;

-- Materialized view section
DROP INDEX  &&schemaName..VW_MX FORCE;
DROP MATERIALIZED VIEW &&schemaName..INV_VW_COMPOUNDS;

--CREATE MATERIALIZED VIEW LOG ON &&schemaName..INV_COMPOUNDS
--TABLESPACE &&tableSpaceName.
--WITH ROWID; 

--&&schemaName already has the privileges, but when a "materialized view" is created for other owner is necessary grant this privileges directly rather than through a "ROLE".   
GRANT CREATE TABLE TO &&schemaName; 
GRANT ON COMMIT REFRESH  to &&schemaName;

CREATE MATERIALIZED VIEW &&schemaName..INV_VW_COMPOUNDS
STORAGE (INITIAL 1M NEXT 1M PCTINCREASE 0)
LOB (BASE64_CDX) STORE AS
(
  DISABLE STORAGE IN ROW NOCACHE PCTVERSION 10
  TABLESPACE &&lobsTableSpaceName.
  STORAGE (INITIAL &&lobB64cdx. NEXT &&lobB64cdx.)
)
PCTFREE 0
PCTUSED 99
NOLOGGING
TABLESPACE &&tableSpaceName.
BUILD IMMEDIATE
REFRESH FAST ON COMMIT
WITH ROWID
ENABLE QUERY REWRITE 
as
select 1 as marker,
       &&SchemaName..inv_compounds.COMPOUND_ID,
       &&SchemaName..inv_compounds.MOL_ID,
       &&SchemaName..inv_compounds.CAS,
       &&SchemaName..inv_compounds.ACX_ID,
       &&SchemaName..GETREGNAME(reg_id_fk) as substance_name,
       &&regSchemaName..MIXTURES.STRUCTUREAGGREGATION as BASE64_CDX,
       &&SchemaName..inv_compounds.MOLECULAR_WEIGHT,
       &&SchemaName..inv_compounds.DENSITY,
       &&SchemaName..inv_compounds.CLOGP,
       &&SchemaName..inv_compounds.ROTATABLE_BONDS,
       &&SchemaName..inv_compounds.TOT_POL_SURF_AREA,
       &&SchemaName..inv_compounds.HBOND_ACCEPTORS,
       &&SchemaName..inv_compounds.HBOND_DONORS,
       &&SchemaName..inv_compounds.ALT_ID_1,
       &&SchemaName..inv_compounds.ALT_ID_2,
       &&SchemaName..inv_compounds.ALT_ID_3,
       &&SchemaName..inv_compounds.ALT_ID_4,
       &&SchemaName..inv_compounds.ALT_ID_5,
       &&SchemaName..inv_compounds.CONFLICTING_FIELDS,
       &&SchemaName..inv_compounds.REG_ID_FK,
       &&SchemaName..inv_compounds.BATCH_NUMBER_FK,
       &&regSchemaName..REG_NUMBERS.REG_NUMBER AS REG_NUMBER,
       &&regSchemaName..BATCHES.FULLREGNUMBER as REG_BATCH_ID,
       &&regSchemaName..REG_NUMBERS.REG_ID as CPD_INTERNAL_ID,
       &&SchemaName..inv_compounds.rowid ic_rowid,
       &&regSchemaName..REG_NUMBERS.rowid rn_rowid,
       &&regSchemaName..MIXTURES.rowid s_rowid,
       &&regSchemaName..BATCHES.rowid b_rowid
  from &&SchemaName..INV_COMPOUNDS,
       &&regSchemaName..REG_NUMBERS,
       &&regSchemaName..MIXTURES,
       &&regSchemaName..BATCHES
 where &&regSchemaName..REG_NUMBERS.REG_ID = &&SchemaName..INV_COMPOUNDS.REG_ID_FK
   and &&regSchemaName..MIXTURES.REGID = &&SchemaName..INV_COMPOUNDS.REG_ID_FK
   and &&SchemaName..INV_COMPOUNDS.REG_ID_FK =
       &&regSchemaName..BATCHES.REG_INTERNAL_ID
   and &&SchemaName..inv_compounds.BATCH_NUMBER_FK =
       &&regSchemaName..BATCHES.BATCH_NUMBER
   and reg_id_fk is not null
UNION ALL
select 2 as marker,
       &&SchemaName..inv_compounds.COMPOUND_ID,
       &&SchemaName..inv_compounds.MOL_ID,
       &&SchemaName..inv_compounds.CAS,
       &&SchemaName..inv_compounds.ACX_ID,
       &&SchemaName..inv_compounds.substance_name as substance_name,
       &&SchemaName..inv_compounds.BASE64_CDX as BASE64_CDX,
       &&SchemaName..inv_compounds.MOLECULAR_WEIGHT,
       &&SchemaName..inv_compounds.DENSITY,
       &&SchemaName..inv_compounds.CLOGP,
       &&SchemaName..inv_compounds.ROTATABLE_BONDS,
       &&SchemaName..inv_compounds.TOT_POL_SURF_AREA,
       &&SchemaName..inv_compounds.HBOND_ACCEPTORS,
       &&SchemaName..inv_compounds.HBOND_DONORS,
       &&SchemaName..inv_compounds.ALT_ID_1,
       &&SchemaName..inv_compounds.ALT_ID_2,
       &&SchemaName..inv_compounds.ALT_ID_3,
       &&SchemaName..inv_compounds.ALT_ID_4,
       &&SchemaName..inv_compounds.ALT_ID_5,
       &&SchemaName..inv_compounds.CONFLICTING_FIELDS,
       &&SchemaName..inv_compounds.REG_ID_FK,
       &&SchemaName..inv_compounds.BATCH_NUMBER_FK,
       null AS REG_NUMBER,
       null as REG_BATCH_ID,
       0 as CPD_INTERNAL_ID,
       &&SchemaName..inv_compounds.rowid ic_rowid,
       null rn_rowid,
       null s_rowid,
       null b_rowid
  from &&SchemaName..INV_COMPOUNDS
 where  reg_id_fk is null;

CREATE INDEX &&schemaName..INV_VW_COMPOUNDS_CPD_IDX ON &&schemaName..INV_VW_COMPOUNDS(COMPOUND_ID ASC) TABLESPACE &&indexTableSpaceName ONLINE;
CREATE INDEX &&schemaName..INV_VW_COMPOUNDS_CAS_IDX ON &&schemaName..INV_VW_COMPOUNDS(CAS ASC) TABLESPACE &&indexTableSpaceName ONLINE;
CREATE INDEX &&schemaName..INV_VW_COMPOUNDS_ACXID_IDX ON &&schemaName..INV_VW_COMPOUNDS(ACX_ID ASC) TABLESPACE &&indexTableSpaceName ONLINE;
CREATE INDEX &&schemaName..INV_VW_COMPOUNDS_ic_rowid_IDX ON &&schemaName..INV_VW_COMPOUNDS(ic_rowid ASC) TABLESPACE &&indexTableSpaceName ONLINE; 
CREATE INDEX &&schemaName..INV_VW_COMPOUNDS_rn_rowid_IDX ON &&schemaName..INV_VW_COMPOUNDS(rn_rowid ASC) TABLESPACE &&indexTableSpaceName ONLINE; 
CREATE INDEX &&schemaName..INV_VW_COMPOUNDS_s_rowid_IDX ON &&schemaName..INV_VW_COMPOUNDS(s_rowid ASC) TABLESPACE &&indexTableSpaceName ONLINE; 
CREATE INDEX &&schemaName..INV_VW_COMPOUNDS_b_rowid_IDX ON &&schemaName..INV_VW_COMPOUNDS(b_rowid ASC) TABLESPACE &&indexTableSpaceName ONLINE;
CREATE INDEX &&schemaName..VW_MX ON &&schemaName..INV_VW_COMPOUNDS(base64_cdx) indexType is cscartridge.moleculeindextype PARAMETERS('&&cscartIndexOptions');

GRANT SELECT ON &&schemaName..INV_VW_COMPOUNDS TO &&securitySchemaName WITH GRANT OPTION;
