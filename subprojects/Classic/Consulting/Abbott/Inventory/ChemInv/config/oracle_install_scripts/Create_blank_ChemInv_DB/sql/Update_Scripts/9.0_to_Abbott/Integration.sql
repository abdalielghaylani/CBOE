-- VIEWS NECESSARY FOR REG INTEGRATION
-- ------------------------------------

CREATE OR REPLACE VIEW INV_VW_REG_STRUCTURES AS
Select 
       base64_cdx AS RegBase64
       , reg_id AS RegID
FROM structures, reg_numbers
WHERE structures.cpd_internal_id = reg_numbers.cpd_internal_id;


COMMENT ON COLUMN inv_vw_reg_structures.regbase64 IS 'Reg Structure::on';
COMMENT ON COLUMN inv_vw_reg_structures.regid IS 'Reg ID ::off';


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
       , batches.notebook_page AS Page
       , batches.purity AS RegPurity
       , batches.notebook_text AS NoteBook
       , cs_security.people.user_id AS RegScientist
       , batches.amount||' '||batches.amount_units AS RegAmount
       , batches.amount_units AS RegAmountUnits
       , ( SELECT alt_ids.identifier 
           FROM alt_ids 
           WHERE alt_ids.identifier_type = 0 
           AND reg_internal_id = reg_numbers.reg_id 
           AND ROWNUM = 1
           ) AS RegName
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
FROM reg_numbers
     , batches 
     , cs_security.people
     , regdb.compound_molecule cm
WHERE reg_numbers.reg_id = batches.reg_internal_id
AND batches.scientist_id = cs_security.people.person_id(+)
And reg_numbers.reg_id = cm.cpd_database_counter;

COMMENT ON COLUMN inv_vw_reg_batches.regnumber IS 'Reg Number::on';
COMMENT ON COLUMN inv_vw_reg_batches.regid IS 'Reg ID::on';
COMMENT ON COLUMN inv_vw_reg_batches.regcas IS 'Reg CAS::on';
COMMENT ON COLUMN inv_vw_reg_batches.regname IS 'Reg Name::on';
COMMENT ON COLUMN inv_vw_reg_batches.batchnumber IS 'RegBatch Number::on';
COMMENT ON COLUMN inv_vw_reg_batches.regbatchid IS 'Reg Batch ID::on';
COMMENT ON COLUMN inv_vw_reg_batches.page IS 'Reg Page::on';
COMMENT ON COLUMN inv_vw_reg_batches.regpurity IS 'Reg Purity::on';
COMMENT ON COLUMN inv_vw_reg_batches.regscientist IS 'Reg Scientist::on';
COMMENT ON COLUMN inv_vw_reg_batches.regamount IS 'Reg Amount::on';
COMMENT ON COLUMN inv_vw_reg_batches.notebook IS 'Reg Notebook::on';


CREATE OR REPLACE VIEW INV_VW_REG_ALTIDS AS
SELECT 
       reg_internal_id AS regid
       , identifier
FROM Alt_ids
Where identifier_type=2;



-- GRANTS NECESSARY FOR REG INTEGRATION
-- ------------------------------------

SET verify off

connect &&regSchemaName/&&regSchemaPass@&&serverName;
GRANT SELECT ON ALT_IDS TO CHEMINVDB2 WITH GRANT OPTION;
GRANT SELECT ON REG_NUMBERS TO CHEMINVDB2 WITH GRANT OPTION;
GRANT SELECT ON BATCHES TO CHEMINVDB2 WITH GRANT OPTION;
GRANT SELECT ON STRUCTURES TO CHEMINVDB2 WITH GRANT OPTION;
GRANT SELECT ON COMPOUND_MOLECULE TO CHEMINVDB2 WITH GRANT OPTION;

connect &&securitySchemaName/&&securitySchemaPass@&&serverName;
GRANT SELECT ON PEOPLE TO CHEMINVDB2 WITH GRANT OPTION;

Connect &&schemaName/&&schemaPass@&&serverName
GRANT SELECT ON INV_VW_REG_STRUCTURES TO INV_ADMIN;
GRANT SELECT ON INV_VW_REG_BATCHES TO INV_ADMIN;
GRANT SELECT ON INV_VW_REG_ALTIDS TO INV_ADMIN;

GRANT SELECT ON INV_VW_REG_STRUCTURES TO INV_BROWSER;
GRANT SELECT ON INV_VW_REG_BATCHES TO INV_BROWSER;
GRANT SELECT ON INV_VW_REG_ALTIDS TO INV_BROWSER;



-- SECURITY NECESSARY FOR REG INTEGRATION
-- --------------------------------------

SET verify off
connect &&securitySchemaName/&&securitySchemaPass@&&serverName;

INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_VW_REG_STRUCTURES');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_VW_REG_ALTIDS');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_VW_REG_BATCHES');


Connect &&schemaName/&&schemaPass@&&serverName
GRANT SELECT ON "&&SchemaName".INV_CONTAINER_BATCHES TO CS_SECURITY WITH GRANT OPTION;
GRANT SELECT ON "&&SchemaName".INV_PROJECT TO CS_SECURITY WITH GRANT OPTION;
GRANT SELECT ON "&&SchemaName".INV_UNIT_CONVERSION_FORMULA TO CS_SECURITY WITH GRANT OPTION;
GRANT SELECT ON "&&SchemaName".INV_VW_REG_STRUCTURES to CS_SECURITY WITH GRANT OPTION;
GRANT SELECT ON "&&SchemaName".INV_VW_REG_BATCHES to CS_SECURITY WITH GRANT OPTION;
GRANT SELECT ON "&&SchemaName".INV_VW_REG_ALTIDS to CS_SECURITY WITH GRANT OPTION;
GRANT EXECUTE ON "&&SchemaName".UPDATECONTAINERBATCHES to CS_SECURITY WITH GRANT OPTION;



-- SYNONYMS NECESSARY FOR REG INTEGRATION
-- --------------------------------------

Connect &&InstallUser/&&sysPass@&&serverName

DECLARE
	PROCEDURE createSynonym(synName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_synonyms where Upper(synonym_name) = synName;
			if n = 0 then
				execute immediate 'CREATE PUBLIC SYNONYM ' || synName || ' FOR &&schemaName..' || synName;
			end if;
		END createSynonym;
BEGIN

	createSynonym('INV_VW_REG_BATCHES');
	createSynonym('INV_VW_REG_STRUCTURES');
	createSynonym('INV_VW_REG_ALTIDS');
     
END;
/                                           

