-- Copyright Cambridgesoft Corp 2001-2009 all rights reserved
-- COE11  9-Jun-2009  kfd

--#########################################################
-- Update file for integrating Inventory Enterprise with Registration Enterprise
--######################################################### 


spool ON
spool Logs\LOG_RegistrationIntegration.txt

--' Intialize variables
@Parameters.sql
@Patches\Prompts.sql

Connect &&schemaName/&&schemaPass@&&serverName
GRANT SELECT ON INV_UNITS to &&regSchemaName with grant option;
GRANT EXECUTE ON RACKS to inv_browser;


-- Special privileges for Inventory schema
Connect &&InstallUser/&&sysPass@&&serverName
GRANT SELECT ON &&securitySchemaName..PEOPLE to &&schemaName;

-- Need access to underlying tables to create fast refresh materialized view
-- Note:  ALL privilege may be excessive
--JHS This is done again in the patch
--GRANT ALL ON &&regSchemaName..REG_NUMBERS TO &&schemaName;
--GRANT ALL ON &&regSchemaName..MIXTURES TO &&schemaName;

--These materialized view logs get recreated later.  Not worth creating them now.
-- Create materialized view logs (for inv_vw_compounds)
--CREATE MATERIALIZED VIEW LOG ON &&regSchemaName..REG_NUMBERS
--TABLESPACE T_&&regSchemaName._TABL
--WITH ROWID;


--These materialized view logs get recreated later.  Not worth creating them now.
--CREATE MATERIALIZED VIEW LOG ON &&regSchemaName..MIXTURES
--TABLESPACE T_&&regSchemaName._TABL
--WITH ROWID;


--These grants are done again later
--grant all on &&regSchemaName..MLOG$_MIXTURES to &&schemaName.;
--grant all on &&regSchemaName..MLOG$_REG_NUMBERS to &&schemaName.;

-- Materialized view section
--DROP VIEW &&schemaName..INV_VW_COMPOUNDS;

--THis one is not done later
DROP MATERIALIZED VIEW LOG ON &&schemaName..INV_COMPOUNDS;
CREATE MATERIALIZED VIEW LOG ON &&schemaName..INV_COMPOUNDS
TABLESPACE &&tableSpaceName.
WITH ROWID;

--&&schemaName already has the privileges, but when a "materialized view" is created for other owner is necessary grant this privileges directly rather than through a "ROLE".   
GRANT CREATE TABLE TO &&schemaName; 

/* This is done again later.  It is super wasteful to do it twice
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
decode(&&schemaName..inv_compounds.reg_id_fk,null,&&schemaName..inv_compounds.BASE64_CDX,&&regSchemaName..MIXTURES.STRUCTUREAGGREGATION) as BASE64_CDX,
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
&&regSchemaName..REG_NUMBERS.REG_NUMBER AS REG_NUMBER,
-- ToDo: this delimiter really should not be hard-coded to a hyphen; use the proper REGDB.Sequence value
&&regSchemaName..REG_NUMBERS.REG_NUMBER || '-' || &&schemaName..inv_compounds.BATCH_NUMBER_FK as REG_BATCH_ID,
decode(&&schemaName..inv_compounds.CPD_INTERNAL_ID_FK,null,0,&&regSchemaName..REG_NUMBERS.REG_ID) as CPD_INTERNAL_ID,
&&schemaName..inv_compounds.rowid ic_rowid,
&&regSchemaName..REG_NUMBERS.rowid rn_rowid,
&&regSchemaName..MIXTURES.rowid s_rowid
from &&schemaName..INV_COMPOUNDS, &&regSchemaName..REG_NUMBERS, &&regSchemaName..MIXTURES
where &&regSchemaName..REG_NUMBERS.REG_ID(+) = &&schemaName..INV_COMPOUNDS.REG_ID_FK
and &&regSchemaName..MIXTURES.REGID(+) = &&schemaName..INV_COMPOUNDS.REG_ID_FK;


CREATE INDEX &&schemaName..INV_VW_COMPOUNDS_CPD_IDX ON &&schemaName..INV_VW_COMPOUNDS(COMPOUND_ID ASC) TABLESPACE &&indexTableSpaceName ONLINE;
CREATE INDEX &&schemaName..INV_VW_COMPOUNDS_CAS_IDX ON &&schemaName..INV_VW_COMPOUNDS(CAS ASC) TABLESPACE &&indexTableSpaceName ONLINE;
CREATE INDEX &&schemaName..INV_VW_COMPOUNDS_ACXID_IDX ON &&schemaName..INV_VW_COMPOUNDS(ACX_ID ASC) TABLESPACE &&indexTableSpaceName ONLINE;
CREATE INDEX &&schemaName..INV_VW_COMPOUNDS_SN_IDX ON &&schemaName..INV_VW_COMPOUNDS(SUBSTANCE_NAME ASC) TABLESPACE &&indexTableSpaceName ONLINE;
CREATE INDEX &&schemaName..VW_MX ON &&schemaName..INV_VW_COMPOUNDS(base64_cdx) indexType is cscartridge.moleculeindextype PARAMETERS('&&cscartIndexOptions');

GRANT SELECT ON &&schemaName..INV_VW_COMPOUNDS TO &&securitySchemaName WITH GRANT OPTION;
*/

Connect &&regSchemaName/&&regSchemaPass@&&serverName

GRANT SELECT on VW_BATCH to &&schemaName with grant option;
GRANT SELECT on VW_REGISTRYNUMBER to &&schemaName with grant option;
GRANT SELECT on VW_COMPOUND_IDENTIFIER to &&schemaName with grant option;
GRANT SELECT on VW_MIXTURE to &&schemaName with grant option;
GRANT SELECT on VW_PICKLIST to &&schemaName with grant option;
GRANT SELECT on notebooks to &&schemaName with grant option;

-- Overwrite the units view in registration with inventory units
Create or replace force view VW_UNIT AS
SELECT UNIT_ID AS ID, UNIT_ABREVIATION AS UNIT, ACTIVE, SORTORDER
FROM &&schemaName..INV_UNITS
WHERE UNIT_TYPE_ID_FK IN (1,2,4) ORDER BY UNIT_ABREVIATION ASC;

GRANT SELECT on VW_Unit to &&schemaName with grant option;

-- May need to uncomment determining what NOTEBOOKS access is for
-- Should probably be changed to VW_NOTEBOOK
--grant SELECT on notebooks TO &&schemaName WITH GRANT OPTION;
--grant SELECT on notebooks TO &&securitySchemaName WITH GRANT OPTION;
--GRANT select on notebooks TO inv_browser;

-- -------------------------------------------
--' create views in the Inventory schema
Connect &&schemaName/&&schemaPass@&&serverName

CREATE OR REPLACE VIEW INV_VW_REG_STRUCTURES AS
Select 
       structureaggregation AS BASE64_CDX
       , regid AS REGID
       , to_char(modified,'dd-mon-yyyy') as LASTUPDATEDDATE
FROM &&regSchemaName..vw_mixture;

COMMENT ON COLUMN INV_VW_REG_STRUCTURES.BASE64_CDX IS 'Reg Structure::on';
COMMENT ON COLUMN INV_VW_REG_STRUCTURES.REGID IS 'Reg ID ::off';
COMMENT ON COLUMN INV_VW_REG_STRUCTURES.LASTUPDATEDDATE IS 'Last Updated Date::off'; 





-- Note on type codes for vw_compound_identifier
--  1 Chemical_Name
--  2 CAS_Number
--  3 Synonym_r
--  4 Alias
--  5 Chem_Name_Autogen
/* Don't create this any more it is done in the patch
CREATE OR REPLACE VIEW INV_VW_REG_BATCHES AS
SELECT
       rn.regnumber AS RegNumber
       , b.batchNumber AS BatchNumber
       , rn.regid as RegID
       , ( SELECT ci.value
           FROM REGDB.vw_compound_identifier ci
           WHERE rn.Regid = ci.Regid
           AND ci.type = 2
           AND ROWNUM = 1
           ) AS RegCas
       , rn.regnumber||'-'||b.batchNumber AS RegBatchID
       , '' as RegPage
       , b.PURITY as RegPurity
       , b.NOTEBOOK_TEXT as RegNoteBook
       , '' as RegNoteBookId
       , p.user_id AS RegScientist
       , b.amount as RegAmount
       , (SELECT id
       		FROM REGDB.VW_Unit
       		WHERE ID = b.amount_units)as RegAmountUnits
       , ( SELECT ci.value
           FROM REGDB.vw_compound_identifier ci
           WHERE ci.type = 1
           AND regid = rn.regid
           AND ROWNUM = 1
           ) AS RegName
       , rn.sequencenumber as RegSequence
       , '' as mol_id
       , '' as sequence_internal_id
       , '' as compound_type
       , '' as root_number
       , '' as mf_text
       , '' as mw_text
       , '' as root_reg_date
       , '' as structure_comments_txt
       , '' as product_type
       , '' as chiral
       , '' as clogp
       , '' as h_bond_donors
       , '' as h_bond_acceptors
       , '' as RegSaltName
       , '' as RegSolvateName
       , (SELECT ci.id
           FROM REGDB.vw_compound_identifier ci
           WHERE ci.type in (3,4,5)
           AND ci.id is not null
           AND ci.regid = rn.regid
           AND ci.type =
           (SELECT MIN(ci2.type)
            FROM REGDB.vw_compound_identifier ci2
            WHERE ci2.type in (3,4,5)
            AND ci2.id is not null
            AND ci2.regid = rn.regid)
           AND ROWNUM = 1
           ) AS RegAltId
FROM REGDB.vw_registrynumber rn
     , REGDB.vw_batch b
     , COEDB.people p
     , REGDB.VW_MIXTURE cm
WHERE rn.regid = b.regid
AND b.personregistered = COEDB.p.person_id(+)
And rn.regid = cm.regid;

COMMENT ON COLUMN INV_VW_REG_BATCHES.REGNUMBER IS 'Reg Number::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGID IS 'Root Number::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGCAS IS 'CAS::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGNAME IS 'Reagent Name::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.BATCHNUMBER IS 'Batch Number::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGBATCHID IS 'Reg Batch ID::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGPAGE IS 'Notebook Page::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGPURITY IS 'Purity::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGSCIENTIST IS 'Scientist::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGAMOUNT IS 'Amount::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGNOTEBOOK IS 'Notebook::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.RegSaltName IS 'Salt Name::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.RegSolvateName IS 'Solvate Name::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGSEQUENCE IS 'Reg Sequence::off'; 
*/

CREATE OR REPLACE VIEW INV_VW_REG_ALTIDS AS
SELECT 
       regid AS regid
       , id as identifier
FROM &&regSchemaName..vw_compound_identifier
Where type=2;


-- Look for Reg RLS and add the policy, if needed
DEFINE RegRLSScript = Add_Reg_RLS_Policy
col rls_col for a30 new_value RegRLSScript

@Update_Scripts\RegistrationIntegration\DetectRegRLS.sql;
@Update_Scripts\RegistrationIntegration\&RegRLSScript..sql;

-- End materialized view section

@Update_Scripts\RegistrationIntegration\f_CreateRegCompound.sql;
@Update_Scripts\RegistrationIntegration\f_UpdateRegCompound.sql;

SET verify off

-- -------------------------------------------
connect &&securitySchemaName/&&securitySchemaPass@&&serverName;

--This next line appears to be obsolete.  It was commented out prior to COE11
--INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', 'regdb', 'notebooks');

GRANT SELECT ON PEOPLE TO &&schemaName WITH GRANT OPTION;
@Update_Scripts\RegistrationIntegration\GrantPrivs.sql;


-- Applying the regintegration patch
@@"Patches\Parameters.sql"
@@"Patches\RegistrationIntegration\patch.sql"

-- -------------------------------------------
Connect &&schemaName/&&schemaPass@&&serverName
@PLSQL\RecompilePLSQL.sql

prompt RegistrationIntegration script ending 
spool off

exit
