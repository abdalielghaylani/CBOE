--#########################################################
--CREATE VIEW FOR BIOASSAY
--#########################################################
Connect &&InstallUser/&&sysPass@&&serverName

GRANT CREATE ANY INDEX TO &&schemaName;
GRANT CREATE ANY SNAPSHOT TO &&schemaName;
GRANT CREATE ANY VIEW TO &&schemaName;

connect &&schemaName/&&schemaPass@&&serverName

CREATE OR REPLACE VIEW VW_REG_BATCHES ( REG_ID, 
MOL_ID, REGISTRY_DATE, CPD_INTERNAL_ID, REG_NUMBER, 
ROOT_NUMBER, SEQUENCE_NUMBER, SEQUENCE_INTERNAL_ID, LAST_BATCH_NUMBER, 
REGISTRAR_PERSON_ID, LOAD_ID, DATETIME_STAMP, BATCH_INTERNAL_ID, 
BATCH_NUMBER, FULL_REG_NUMBER ) 
AS SELECT  
  reg_id, 
  reg_numbers.mol_id, 
  registry_date, 
  reg_numbers.cpd_internal_id, 
  reg_number, 
  root_number, 
  sequence_number, 
  sequence_internal_id, 
  last_batch_number, 
  registrar_person_id, 
  reg_numbers.load_id, 
  reg_numbers.datetime_stamp, 
  batch_internal_id, 
  batch_number, 
  reg_number || '-' || batch_number 
FROM  
  reg_Numbers, 
  batches 
WHERE  
  reg_numbers.reg_id = batches.reg_internal_id;

--GRANT SELECT ON VW_REG_BATCHES TO BROWSER;
