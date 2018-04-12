--#########################################################
--CREATE VIEW FOR BIOASSAY
--#########################################################

Connect &&InstallUser/&&sysPass@&&serverName

GRANT CREATE ANY INDEX TO &&schemaName;
GRANT CREATE ANY SNAPSHOT TO &&schemaName;
GRANT CREATE ANY VIEW TO &&schemaName;

connect &&schemaName/&&schemaPass@&&serverName

drop view vw_reg_batches;

create materialized view vw_reg_batches
	storage (initial 1k next 100k pctincrease 0)
	pctfree 0
	pctused 99
	nologging
	tablespace &&tableSpaceName.
	build immediate
	refresh complete
	with primary key
	start with trunc(sysdate) + 23.2/24
	next trunc(sysdate) + 1 + 23.2/24
as
 SELECT
	reg_numbers.reg_id,
	reg_numbers.mol_id,
	reg_numbers.registry_date,
	reg_numbers.cpd_internal_id,
	reg_numbers.reg_number,
	reg_numbers.root_number,
	reg_numbers.sequence_number,
	reg_numbers.sequence_internal_id,
	reg_numbers.last_batch_number,
	reg_numbers.registrar_person_id,
	reg_numbers.load_id,
	reg_numbers.datetime_stamp,
	batches.batch_internal_id,
	batches.batch_number,
	reg_numbers.reg_number || '-' || batches.batch_number as FULL_REG_NUMBER
FROM
  reg_Numbers,
  batches
WHERE
  reg_numbers.reg_id = batches.reg_internal_id;
  

--INDEX
CREATE INDEX "INDEX_VW_REGDB_BATCHES"
    ON "VW_REG_BATCHES"  ("FULL_REG_NUMBER")
    TABLESPACE "T_REGDB_INDEX";

CREATE INDEX "INDEX_BATCH_INTERNAL_ID"
    ON "VW_REG_BATCHES"  ("BATCH_INTERNAL_ID")
    TABLESPACE "T_REGDB_INDEX";

--GRANTS
grant select, insert, update, delete on vw_reg_batches to supervising_chemical_admin;
grant select, insert, update, delete on vw_reg_batches to chemical_administrator;
grant select on vw_reg_batches to supervising_scientist;
grant select on vw_reg_batches to submitter;
grant select on vw_reg_batches to browser;


--ALTER SEQUENCE TABLE

ALTER TABLE SEQUENCE ADD
(REG_DELIMITER VARCHAR2(1) DEFAULT '/' NULL);

ALTER TABLE SEQUENCE ADD
(PREFIX_DELIMITER VARCHAR2(1) DEFAULT '-' NULL);

ALTER TABLE SEQUENCE ADD
(ROOT_NUMBER_LENGTH NUMBER(3, 0) DEFAULT 6 NULL);

ALTER TABLE SEQUENCE ADD
(DUP_CHECK_LOCAL NUMBER(1, 0) DEFAULT 0 NULL);


UPDATE SEQUENCE SET REG_DELIMITER='/', PREFIX_DELIMITER='-', ROOT_NUMBER_LENGTH=6, DUP_CHECK_LOCAL=0
WHERE SEQUENCE_ID = 1;