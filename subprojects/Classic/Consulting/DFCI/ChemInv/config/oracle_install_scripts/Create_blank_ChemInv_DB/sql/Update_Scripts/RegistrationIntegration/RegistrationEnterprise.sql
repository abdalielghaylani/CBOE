-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Update file for integrating Inventory Enterprise with Registration Enterprise
--######################################################### 


spool ON
spool Logs\LOG_RegistrationIntegration.txt

--' Intialize variables
@Parameters.sql
@Prompts.sql

--' grant select on the underlying reg tables to cheminvdb2
Connect &&regSchemaName/&&regSchemaPass@&&serverName
GRANT SELECT ON reg_numbers TO &&schemaName;
GRANT SELECT ON structures TO &&schemaName;
GRANT SELECT ON alt_ids TO &&schemaName;
GRANT SELECT on compound_molecule TO &&schemaName;
grant SELECT on notebooks TO &&schemaName;
grant SELECT on notebooks TO inv_browser;

--' create views in the Inventory schema
Connect &&schemaName/&&schemaPass@&&serverName

CREATE OR REPLACE VIEW INV_VW_REG_STRUCTURES AS
Select 
       base64_cdx AS BASE64_CDX
       , reg_id AS REGID
       , '' as LASTUPDATEDDATE
FROM structures, reg_numbers
WHERE structures.cpd_internal_id = reg_numbers.cpd_internal_id;


COMMENT ON COLUMN INV_VW_REG_STRUCTURES.BASE64_CDX IS 'Reg Structure::on';
COMMENT ON COLUMN INV_VW_REG_STRUCTURES.REGID IS 'Reg ID ::off';
COMMENT ON COLUMN INV_VW_REG_STRUCTURES.LASTUPDATEDDATE IS 'Last Updated Date::off'; 


CREATE OR REPLACE VIEW INV_VW_REG_BATCHES AS
SELECT 
       reg_numbers.reg_number AS RegNumber
       , batches.batch_Number AS BatchNumber
       , reg_numbers.reg_id as RegID
       , ( SELECT identifier 
           FROM alt_ids 
           WHERE reg_numbers.Reg_id = alt_ids.Reg_internal_id 
           AND alt_ids.identifier_type = 1
           ) AS RegCas
       , reg_numbers.reg_number||'-'||batches.batch_Number AS RegBatchID
       , batches.notebook_page AS RegPage
       , batches.purity AS RegPurity
       , batches.notebook_text AS RegNoteBook
       , batches.notebook_internal_id AS RegNoteBookId
       , cs_security.people.user_id AS RegScientist
       , batches.amount||' '||batches.amount_units AS RegAmount
       , batches.amount_units AS RegAmountUnits
       , ( SELECT alt_ids.identifier 
           FROM alt_ids 
           WHERE alt_ids.identifier_type = 0 
           AND reg_internal_id = reg_numbers.reg_id 
           AND ROWNUM = 1
           ) AS RegName
       , reg_numbers.sequence_number as RegSequence
       , cm.mol_id
       , cm.sequence_internal_id
       , cm.compound_type
       , cm.root_number
       , cm.mf_text
       , cm.mw_text
       , cm.root_reg_date
       , structure_comments_txt
       , cm.product_type
       , cm.chiral
       , cm.clogp
       , cm.h_bond_donors
       , cm.h_bond_acceptors
       , batches.salt_name as RegSaltName
       , batches.solvate_name as RegSolvateName
FROM reg_numbers
     , batches 
     , cs_security.people
     , regdb.compound_molecule cm
WHERE reg_numbers.reg_id = batches.reg_internal_id
AND batches.scientist_id = cs_security.people.person_id(+)
And reg_numbers.reg_id = cm.cpd_database_counter;

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


CREATE OR REPLACE VIEW INV_VW_REG_ALTIDS AS
SELECT 
       reg_internal_id AS regid
       , identifier
FROM Alt_ids
Where identifier_type=2;


-- GRANTS NECESSARY FOR REG INTEGRATION
SET verify off

connect &&regSchemaName/&&regSchemaPass@&&serverName;
GRANT SELECT ON ALT_IDS TO CHEMINVDB2 WITH GRANT OPTION;
GRANT SELECT ON REG_NUMBERS TO CHEMINVDB2 WITH GRANT OPTION;
GRANT SELECT ON BATCHES TO CHEMINVDB2 WITH GRANT OPTION;
GRANT SELECT ON STRUCTURES TO CHEMINVDB2 WITH GRANT OPTION;
GRANT SELECT ON COMPOUND_MOLECULE TO CHEMINVDB2 WITH GRANT OPTION;
grant SELECT on notebooks TO CHEMINVDB2 WITH GRANT OPTION;
grant SELECT on notebooks TO cs_security WITH GRANT OPTION;

connect &&securitySchemaName/&&securitySchemaPass@&&serverName;
GRANT SELECT ON PEOPLE TO CHEMINVDB2 WITH GRANT OPTION;
--INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', 'regdb', 'notebooks');


@PLSQL\RecompilePLSQL.sql

/
prompt 
spool off

exit
