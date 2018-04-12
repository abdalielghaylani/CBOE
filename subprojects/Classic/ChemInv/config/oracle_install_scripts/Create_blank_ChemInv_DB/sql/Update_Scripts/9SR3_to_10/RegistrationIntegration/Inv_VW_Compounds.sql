Connect &&regSchemaName/&&regSchemaPass@&&serverName
GRANT ALL ON reg_numbers TO &&schemaName;
GRANT ALL ON structures TO &&schemaName;
GRANT ALL ON compound_project TO &&schemaName;

-- Create materialized view logs
CREATE MATERIALIZED VIEW LOG ON REG_NUMBERS
TABLESPACE T_&&regSchemaName._TABL
WITH ROWID;

CREATE MATERIALIZED VIEW LOG ON STRUCTURES
TABLESPACE T_&&regSchemaName._TABL
WITH ROWID;

grant all on &&regSchemaName..MLOG$_REG_NUMBERS to &&schemaName.;
grant all on &&regSchemaName..MLOG$_STRUCTURES to &&schemaName.;

Connect &&schemaName/&&schemaPass@&&serverName
CREATE MATERIALIZED VIEW LOG ON INV_COMPOUNDS
TABLESPACE &&tableSpaceName.
WITH ROWID;

DROP VIEW &&schemaName..INV_VW_COMPOUNDS;

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
REFRESH FAST
-- Refresh every 15 minutes
START WITH SYSDATE
NEXT SYSDATE + (0.25)/24
WITH ROWID
AS select
&&schemaName..inv_compounds.COMPOUND_ID, 
&&schemaName..inv_compounds.MOL_ID,
&&schemaName..inv_compounds.CAS,
&&schemaName..inv_compounds.ACX_ID,
&&schemaName..inv_compounds.substance_name,
decode(&&schemaName..inv_compounds.reg_id_fk,null,&&schemaName..inv_compounds.BASE64_CDX,&&regSchemaName..STRUCTURES.BASE64_CDX) as BASE64_CDX,
&&schemaName..inv_compounds.MOLECULAR_WEIGHT,
&&schemaName..inv_compounds.DENSITY, 
&&schemaName..inv_compounds.CLOGP, 
&&schemaName..inv_compounds.ROTATABLE_BONDS,
&&schemaName..inv_compounds.TOT_POL_SURF_AREA, 
&&schemaName..inv_compounds.HBOND_ACCEPTORS, 
&&schemaName..inv_compounds.HBOND_DONORS,
&&schemaName..inv_compounds.ALT_ID_1, 
&&schemaName..inv_compounds.ALT_ID_2, 
&&schemaName..inv_compounds.ALT_ID_3, 
&&schemaName..inv_compounds.ALT_ID_4, 
&&schemaName..inv_compounds.ALT_ID_5,
&&schemaName..inv_compounds.CONFLICTING_FIELDS, 
&&schemaName..inv_compounds.REG_ID_FK, 
&&schemaName..inv_compounds.BATCH_NUMBER_FK,
&&regSchemaName..REG_NUMBERS.REG_NUMBER,
-- ToDo: this delimiter really should not be hard-coded to a hyphen; use the proper REGDB.Sequence value
&&regSchemaName..REG_NUMBERS.REG_NUMBER || '-' || &&schemaName..inv_compounds.BATCH_NUMBER_FK as REG_BATCH_ID,
decode(&&schemaName..inv_compounds.CPD_INTERNAL_ID_FK,null,0,&&regSchemaName..REG_NUMBERS.CPD_INTERNAL_ID) as CPD_INTERNAL_ID,
&&schemaName..inv_compounds.rowid ic_rowid,
&&regSchemaName..REG_NUMBERS.rowid rn_rowid,
&&regSchemaName..STRUCTURES.rowid s_rowid
from &&schemaName..INV_COMPOUNDS, &&regSchemaName..REG_NUMBERS, &&regSchemaName..STRUCTURES
where &&regSchemaName..REG_NUMBERS.reg_id(+) = &&schemaName..INV_COMPOUNDS.reg_id_fk
and &&regSchemaName..STRUCTURES.cpd_internal_id(+) = &&schemaName..INV_COMPOUNDS.cpd_internal_id_fk;

CREATE INDEX INV_VW_COMPOUNDS_CPD_IDX ON INV_VW_COMPOUNDS(COMPOUND_ID ASC) TABLESPACE &&indexTableSpaceName ONLINE;
CREATE INDEX INV_VW_COMPOUNDS_CAS_IDX ON INV_VW_COMPOUNDS(CAS ASC) TABLESPACE &&indexTableSpaceName ONLINE;
CREATE INDEX INV_VW_COMPOUNDS_ACXID_IDX ON INV_VW_COMPOUNDS(ACX_ID ASC) TABLESPACE &&indexTableSpaceName ONLINE;
CREATE INDEX INV_VW_COMPOUNDS_SN_IDX ON INV_VW_COMPOUNDS(SUBSTANCE_NAME ASC) TABLESPACE &&indexTableSpaceName ONLINE;
CREATE INDEX VW_MX ON INV_VW_COMPOUNDS(base64_cdx) indexType is cscartridge.moleculeindextype PARAMETERS('&&cscartIndexOptions');

