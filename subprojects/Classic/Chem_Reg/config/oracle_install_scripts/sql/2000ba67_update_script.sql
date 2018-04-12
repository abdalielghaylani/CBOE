--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

--run this script only is you need to update a schema created sind the chem_reg 67 release. All this changes
--are in the Create_ChemREg_ORA.sql script for post a67 installers.
--add csdo hit list tables


Connect &&securitySchemaName/&&securitySchemaPass@&&serverName;

GRANT UPDATE ON CS_SECURITY.PEOPLE TO SUPERVISING_SCIENTIST;

ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("EDIT_SOLVATES_TABLE" NUMBER(1,0));
ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("ADD_SOLVATES_TABLE" NUMBER(1,0));
ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("DELETE_SOLVATES_TABLE" NUMBER(1,0));
ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("EDIT_UTILIZATIONS_TABLE" NUMBER(1,0));
ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("ADD_UTILIZATIONS_TABLE" NUMBER(1,0));
ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("DELETE_UTILIZATIONS_TABLE" NUMBER(1,0));
ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("EDIT_BATCH_PROJECTS_TABLE" NUMBER(1,0));
ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("ADD_BATCH_PROJECTS_TABLE" NUMBER(1,0));
ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("DELETE_BATCH_PROJECTS_TABLE" NUMBER(1,0));
ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("ADD_NOTEBOOKS_TABLE" NUMBER(1,0));
ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("DELETE_NOTEBOOKS_TABLE" NUMBER(1,0));
ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("ADD_SALT_TABLE" NUMBER(1,0));
ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("DELETE_SALT_TABLE" NUMBER(1,0));
ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("ADD_SEQUENCES_TABLE" NUMBER(1,0));
ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("DELETE_SEQUENCES_TABLE" NUMBER(1,0));
ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("DELETE_BATCH_REG" NUMBER(1,0));
ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("ADD_SITES_TABLE" NUMBER(1,0));
ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("DELETE_SITES_TABLES" NUMBER(1,0));
ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("DELETE_EVAL_DATA" NUMBER(1,0));
ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("ADD_ANALYTICS_TABLES" NUMBER(1,0));
ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("DELETE_ANALYTICS_TABLES" NUMBER(1,0));
ALTER TABLE "CS_SECURITY"."CHEM_REG_PRIVILEGES" ADD ("EDIT_ANALYTICS_TABLES" NUMBER(1,0));

--update Browser
UPDATE chem_reg_privileges SET EDIT_SOLVATES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');
UPDATE chem_reg_privileges SET ADD_SOLVATES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');
UPDATE chem_reg_privileges SET DELETE_SOLVATES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');
UPDATE chem_reg_privileges SET EDIT_UTILIZATIONS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');
UPDATE chem_reg_privileges SET ADD_UTILIZATIONS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');
UPDATE chem_reg_privileges SET DELETE_UTILIZATIONS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');
UPDATE chem_reg_privileges SET EDIT_BATCH_PROJECTS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');
UPDATE chem_reg_privileges SET ADD_BATCH_PROJECTS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');
UPDATE chem_reg_privileges SET DELETE_BATCH_PROJECTS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');
UPDATE chem_reg_privileges SET ADD_NOTEBOOKS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');
UPDATE chem_reg_privileges SET DELETE_NOTEBOOKS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');
UPDATE chem_reg_privileges SET ADD_SALT_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');
UPDATE chem_reg_privileges SET DELETE_SALT_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');
UPDATE chem_reg_privileges SET ADD_SEQUENCES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');
UPDATE chem_reg_privileges SET DELETE_SEQUENCES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');
UPDATE chem_reg_privileges SET DELETE_BATCH_REG='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');

UPDATE chem_reg_privileges SET ADD_SITES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');
UPDATE chem_reg_privileges SET DELETE_SITES_TABLES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');
UPDATE chem_reg_privileges SET DELETE_EVAL_DATA='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');
UPDATE chem_reg_privileges SET ADD_ANALYTICS_TABLES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');
UPDATE chem_reg_privileges SET DELETE_ANALYTICS_TABLES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');
UPDATE chem_reg_privileges SET EDIT_ANALYTICS_TABLES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');

--update submitter
UPDATE chem_reg_privileges SET EDIT_SOLVATES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');
UPDATE chem_reg_privileges SET ADD_SOLVATES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');
UPDATE chem_reg_privileges SET DELETE_SOLVATES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');
UPDATE chem_reg_privileges SET EDIT_UTILIZATIONS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');
UPDATE chem_reg_privileges SET ADD_UTILIZATIONS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');
UPDATE chem_reg_privileges SET DELETE_UTILIZATIONS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');
UPDATE chem_reg_privileges SET EDIT_BATCH_PROJECTS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');
UPDATE chem_reg_privileges SET ADD_BATCH_PROJECTS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');
UPDATE chem_reg_privileges SET DELETE_BATCH_PROJECTS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');
UPDATE chem_reg_privileges SET ADD_NOTEBOOKS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');
UPDATE chem_reg_privileges SET DELETE_NOTEBOOKS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');
UPDATE chem_reg_privileges SET ADD_SALT_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');
UPDATE chem_reg_privileges SET DELETE_SALT_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');
UPDATE chem_reg_privileges SET ADD_SEQUENCES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name)= 'SUBMITTER');
UPDATE chem_reg_privileges SET DELETE_SEQUENCES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');
UPDATE chem_reg_privileges SET DELETE_BATCH_REG='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');
UPDATE chem_reg_privileges SET ADD_SITES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');
UPDATE chem_reg_privileges SET DELETE_SITES_TABLES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');
UPDATE chem_reg_privileges SET DELETE_EVAL_DATA='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');
UPDATE chem_reg_privileges SET ADD_ANALYTICS_TABLES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');
UPDATE chem_reg_privileges SET DELETE_ANALYTICS_TABLES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');
UPDATE chem_reg_privileges SET EDIT_ANALYTICS_TABLES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');



--update supervising scientist
UPDATE chem_reg_privileges SET EDIT_SOLVATES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET ADD_SOLVATES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET DELETE_SOLVATES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET EDIT_UTILIZATIONS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET ADD_UTILIZATIONS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET DELETE_UTILIZATIONS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET EDIT_BATCH_PROJECTS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET ADD_BATCH_PROJECTS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET DELETE_BATCH_PROJECTS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET ADD_NOTEBOOKS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name)= 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET DELETE_NOTEBOOKS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET ADD_SALT_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET DELETE_SALT_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name)= 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET ADD_SEQUENCES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET DELETE_SEQUENCES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET DELETE_BATCH_REG='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET ADD_SITES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET DELETE_SITES_TABLES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET DELETE_EVAL_DATA='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET ADD_ANALYTICS_TABLES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET DELETE_ANALYTICS_TABLES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET EDIT_ANALYTICS_TABLES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');

--update chemical_administrator
UPDATE chem_reg_privileges SET EDIT_SOLVATES_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET ADD_SOLVATES_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET DELETE_SOLVATES_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET EDIT_UTILIZATIONS_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET ADD_UTILIZATIONS_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET DELETE_UTILIZATIONS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET EDIT_BATCH_PROJECTS_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET ADD_BATCH_PROJECTS_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET DELETE_BATCH_PROJECTS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET ADD_NOTEBOOKS_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET DELETE_NOTEBOOKS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET ADD_SALT_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET DELETE_SALT_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET ADD_SEQUENCES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET DELETE_SEQUENCES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET DELETE_BATCH_REG='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET ADD_SITES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET DELETE_SITES_TABLES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET DELETE_EVAL_DATA='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET ADD_ANALYTICS_TABLES='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET DELETE_ANALYTICS_TABLES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET EDIT_ANALYTICS_TABLES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');


--update supervising_chemical_admin
UPDATE chem_reg_privileges SET EDIT_SOLVATES_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET ADD_SOLVATES_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET DELETE_SOLVATES_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET EDIT_UTILIZATIONS_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET ADD_UTILIZATIONS_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET DELETE_UTILIZATIONS_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET EDIT_BATCH_PROJECTS_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) ='SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET ADD_BATCH_PROJECTS_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET DELETE_BATCH_PROJECTS_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET ADD_NOTEBOOKS_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET DELETE_NOTEBOOKS_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET ADD_SALT_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name)= 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET DELETE_SALT_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET ADD_SEQUENCES_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name)= 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET DELETE_SEQUENCES_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name)= 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET DELETE_BATCH_REG='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET ADD_SITES_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET DELETE_SITES_TABLES='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET DELETE_EVAL_DATA='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET ADD_ANALYTICS_TABLES='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET DELETE_ANALYTICS_TABLES='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET EDIT_ANALYTICS_TABLES='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');

--update perfume chemixt
UPDATE chem_reg_privileges SET EDIT_SOLVATES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET ADD_SOLVATES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET DELETE_SOLVATES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET EDIT_UTILIZATIONS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET ADD_UTILIZATIONS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET DELETE_UTILIZATIONS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET EDIT_BATCH_PROJECTS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET ADD_BATCH_PROJECTS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET DELETE_BATCH_PROJECTS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET ADD_NOTEBOOKS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET DELETE_NOTEBOOKS_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET ADD_SALT_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET DELETE_SALT_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET ADD_SEQUENCES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name)= 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET DELETE_SEQUENCES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name)= 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET DELETE_BATCH_REG='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name)= 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET ADD_SITES_TABLE='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET DELETE_SITES_TABLES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET DELETE_EVAL_DATA='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET ADD_ANALYTICS_TABLES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET DELETE_ANALYTICS_TABLES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET EDIT_ANALYTICS_TABLES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'PERFUME_CHEMIST');


Connect &&schemaName/&&schemaPass@&&serverName

ALTER TABLE "REGDB"."PARAMETERTYPE" 
    MODIFY("PARAMETER_TYPE_UNITS" VARCHAR2(100))

ALTER TABLE "REGDB"."RESULTTYPE" 
    MODIFY("RESULT_TYPE_UNITS" VARCHAR2(100))

--ADD SOLVATE TABLE, SEQUENCE, TRIGGER AND STARTING DATA

CREATE TABLE  SOLVATES (
	SOLVATE_ID NUMBER(8,0) not null,
	SOLVATE_NAME VARCHAR2(50) null,
	SOLVATE_MW NUMBER(10,5) null,
	SOLVATE_MF VARCHAR2(50) null,
	ACTIVE NUMBER(1,0) null, constraint PK_SOLVATE_ID primary key (SOLVATE_ID));

CREATE SEQUENCE SEQ_SOLVATES INCREMENT BY 1 START WITH 1;  

-- Create table level triggers for table SOLVATES.
create or replace trigger TRG_SOLVATES
BEFORE INSERT ON SOLVATES
FOR EACH ROW

BEGIN
   SELECT SEQ_SOLVATES.NEXTVAL INTO :NEW.SOLVATE_ID FROM DUAL;
END;
/

--SOLVATES Starting Data

INSERT INTO SOLVATES(SOLVATE_ID,SOLVATE_NAME,ACTIVE)values('1','no_solvate','1');


--ADD TABLES TO STORE UTILIZATIONS

--DROP TABLE BATCH_PROJECTS CASCADE CONSTRAINTS;
CREATE TABLE  BATCH_PROJECTS (
	BATCH_PROJECT_ID NUMBER(8,0) not null,
	PROJECT_NAME VARCHAR2(250) null,
	OWNER_ID NUMBER(8,0) null,
	ACTIVE NUMBER(1,0) null, constraint PK_BATCH_PROJECT_ID primary key (BATCH_PROJECT_ID)) ;



--DROP TABLE BATCH_PROJ_UTILIZATIONS CASCADE CONSTRAINTS;
CREATE TABLE  BATCH_PROJ_UTILIZATIONS(
	BATCH_PROJ_UTIL_ID NUMBER(8,0) not null,
	UTILIZATION_ID NUMBER(8,0) null,
	BATCH_PROJECT_ID NUMBER(8,0) null,
	UTILIZATION_BOOLEAN NUMBER(1,0) null, constraint PK_BATCH_PROJ_UTIL_ID primary key (BATCH_PROJ_UTIL_ID));

--DROP TABLE UTILIZATIONS CASCADE CONSTRAINTS;
CREATE TABLE  UTILIZATIONS(
	UTILIZATION_ID NUMBER(8,0) not null,
	UTILIZATION_TEXT VARCHAR2(250) null, constraint PK_UTILIZATION_ID2 primary key (UTILIZATION_ID)) ;

--DROP TABLE CMPD_MOL_UTILIZATIONS CASCADE CONSTRAINTS;
CREATE TABLE  CMPD_MOL_UTILIZATIONS (
	CMPD_MOL_UTIL_ID NUMBER(8,0) not null,
	CPD_INTERNAL_ID NUMBER(8,0) null,
	UTILIZATION_ID NUMBER(8,0) null,
	LOAD_ID VARCHAR2(50) null,
	UTILIZATION_BOOLEAN NUMBER(1,0) null, constraint PK_CMPD_MOL_UTIL_ID primary key (CMPD_MOL_UTIL_ID)) ;


--DROP SEQUENCE SEQ_BATCH_PROJECTS;
CREATE SEQUENCE SEQ_BATCH_PROJECTS INCREMENT BY 1 START WITH 1;  

-- Create table level triggers for table BATCH_PROJECTS .
create or replace trigger TRG_BATCH_PROJECTS 
BEFORE INSERT ON BATCH_PROJECTS 
FOR EACH ROW

BEGIN
SELECT SEQ_BATCH_PROJECTS.NEXTVAL INTO :NEW.BATCH_PROJECT_ID FROM DUAL;
END;
/

--DROP SEQUENCE SEQ_UTILIZATIONS;
CREATE SEQUENCE SEQ_UTILIZATIONS INCREMENT BY 1 START WITH 1;  

-- Create table level triggers for table UTILIZATIONS.
create or replace trigger TRG_UTILIZATIONS
BEFORE INSERT ON UTILIZATIONS
FOR EACH ROW

BEGIN
SELECT SEQ_UTILIZATIONS.NEXTVAL INTO :NEW.UTILIZATION_ID FROM DUAL;
END;
/

--DROP SEQUENCE SEQ_BATCH_PROJ_UTIL;
CREATE SEQUENCE SEQ_BATCH_PROJ_UTIL INCREMENT BY 1 START WITH 1;  

-- Create table level triggers for table BATCH_PROJ_UTILIZATIONS.
create or replace trigger TRG_BATCH_PROJ_UTILIZATIONS
BEFORE INSERT ON BATCH_PROJ_UTILIZATIONS
FOR EACH ROW

BEGIN
SELECT SEQ_BATCH_PROJ_UTIL.NEXTVAL INTO :NEW.BATCH_PROJ_UTIL_ID FROM DUAL;
END;
/


--DROP SEQUENCE SEQ_CMPD_MOL_UTIL;
CREATE SEQUENCE SEQ_CMPD_MOL_UTIL INCREMENT BY 1 START WITH 1;  

-- Create table level triggers for table CMPD_MOL_UTILIZATIONS.
create or replace trigger TRG_CMPD_MOL_UTILIZATIONS 
BEFORE INSERT ON CMPD_MOL_UTILIZATIONS 
FOR EACH ROW

BEGIN
SELECT SEQ_CMPD_MOL_UTIL.NEXTVAL INTO :NEW.CMPD_MOL_UTIL_ID FROM DUAL;
END;
/
--ADD STARTING DATA

--BATCH_PROJECTS

INSERT INTO BATCH_PROJECTS(BATCH_PROJECT_ID,PROJECT_NAME,ACTIVE)values('1','unspecified','1');

--IDENTIFIERS

INSERT INTO IDENTIFIERS(ID,IDENTIFIER_TYPE,IDENTIFIER_DESCRIPTOR)values('7','6','Chem_Name_Autogen');
INSERT INTO IDENTIFIERS(ID,IDENTIFIER_TYPE,IDENTIFIER_DESCRIPTOR)values('8','7','Collaborator_ID');

--ALTER TABLES AND ADD NEW FIELDS

ALTER TABLE "REGDB"."COMPOUND_MOLECULE" ADD ("MW2" NUMBER(10,5) null);

ALTER TABLE "REGDB"."COMPOUND_MOLECULE" ADD ("FORMULA2" VARCHAR2(500) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("MW2" NUMBER(10,5) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("FORMULA2" VARCHAR2(500) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("SALT_EQUIVALENTS" NUMBER(10,5) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("SOLVATE_EQUIVALENTS" NUMBER(10,5) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("FORMULA_WEIGHT" NUMBER(10,5) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("SOLVATE_ID" NUMBER(8,0) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("BATCH_FORMULA" VARCHAR2(250) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("BATCH_PROJECT_ID" NUMBER(8,0) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("DUPLICATE" VARCHAR2(500) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("CHEM_NAME_AUTOGEN" VARCHAR2(500) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("HPLC" VARCHAR2(250) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("APPEARANCE" VARCHAR2(250) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("MW" NUMBER null);

ALTER TABLE "REGDB"."BATCHES" ADD ("SALT_EQUIVALENTS" NUMBER(10,5) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("SOLVATE_EQUIVALENTS" NUMBER(10,5) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("FORMULA_WEIGHT" NUMBER(10,5) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("SOLVATE_ID" NUMBER(8,0) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("SOLVATE_ID" NUMBER(8,0) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("BATCH_FORMULA" VARCHAR2(250) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("BATCH_PROJECT_ID" NUMBER(8,0) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("HPLC" VARCHAR2(250) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("APPEARANCE" VARCHAR2(250) null);

--Add LOAD_IDS to all tables
ALTER TABLE "REGDB"."TEST_SAMPLES" ADD ("LOAD_ID"  VARCHAR2(50)  null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("LOAD_ID"  VARCHAR2(50)  null);
ALTER TABLE "REGDB"."STRUCTURES" ADD ("LOAD_ID"  VARCHAR2(50)  null);
ALTER TABLE "REGDB"."MOLFILES" ADD ("LOAD_ID"  VARCHAR2(50)  null);
ALTER TABLE "REGDB"."STRUCTURE_MIXTURE" ADD ("LOAD_ID"  VARCHAR2(50)  null);
ALTER TABLE "REGDB"."SPECTRA" ADD ("LOAD_ID"  VARCHAR2(50)  null);
ALTER TABLE "REGDB"."REG_NUMBERS" ADD ("LOAD_ID"  VARCHAR2(50)  null);
ALTER TABLE "REGDB"."COMPOUND_MOLECULE" ADD ("LAST_MOD_PERSON_ID" NUMBER(8,0) null);
ALTER TABLE "REGDB"."COMPOUND_MOLECULE" ADD ("LAST_MOD_DATE" DATE null);
ALTER TABLE "REGDB"."COMPOUND_MOLECULE" ADD ("MW" NUMBER null);



ALTER TABLE "REGDB"."MIXTURES" ADD ("LOAD_ID"  VARCHAR2(50)  null);
ALTER TABLE "REGDB"."DUPLICATES" ADD ("LOAD_ID"  VARCHAR2(50)  null);
ALTER TABLE "REGDB"."COMPOUND_SALT" ADD ("LOAD_ID"  VARCHAR2(50)  null);
ALTER TABLE "REGDB"."COMPOUND_PROJECT" ADD ("LOAD_ID"  VARCHAR2(50)  null);
ALTER TABLE "REGDB"."COMPOUND_MOLECULE" ADD ("LOAD_ID"  VARCHAR2(50)  null);
ALTER TABLE "REGDB"."BATCHES" ADD ("LOAD_ID"  VARCHAR2(50)  null);
ALTER TABLE "REGDB"."ALT_IDS" ADD ("LOAD_ID"  VARCHAR2(50)  null);

ALTER TABLE "REGDB"."COMPOUND_MOLECULE" ADD ("PRODUCT_TYPE" VARCHAR2(250) null);
ALTER TABLE "REGDB"."COMPOUND_MOLECULE" ADD ("CHIRAL" VARCHAR2 (250) null);
ALTER TABLE "REGDB"."COMPOUND_MOLECULE" ADD ("CLOGP" NUMBER (10,5) null);
ALTER TABLE "REGDB"."COMPOUND_MOLECULE" ADD ("H_BOND_DONORS" NUMBER (8,0) null);
ALTER TABLE "REGDB"."COMPOUND_MOLECULE" ADD ("H_BOND_ACCEPTORS" NUMBER (8,0) null);
ALTER TABLE "REGDB"."COMPOUND_MOLECULE" ADD ("STRUCTURE_COMMENTS_TXT" VARCHAR2(250) null);


ALTER TABLE "REGDB"."BATCHES" ADD ("LOAD_ID"  VARCHAR2(50) );
ALTER TABLE "REGDB"."BATCHES" ADD ("SALT_NAME" VARCHAR2(250) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("SALT_MW" NUMBER(10,5) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("SOLVATE_NAME" VARCHAR2(250) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("SOLVATE_MW" NUMBER(10,5) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("SOURCE" VARCHAR2(250) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("VENDOR_NAME" VARCHAR2(250) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("VENDOR_ID" VARCHAR2(250) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("PERCENT_ACTIVE" NUMBER(10,5) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("AMOUNT_UNITS" VARCHAR(250) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("PURITY" VARCHAR(250) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("LC_UV_MS" VARCHAR(250) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("CHN_COMBUSTION" VARCHAR(250) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("UV_SPECTRUM" VARCHAR(250) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("LOGD" NUMBER (10,5) null);
ALTER TABLE "REGDB"."BATCHES" ADD ("SOLUBILITY" VARCHAR2(250) null);

ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("SALT_NAME" VARCHAR2(250) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("SALT_MW" NUMBER(10,5) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("SOLVATE_NAME" VARCHAR2(250) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("SOLVATE_MW" NUMBER(10,5) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("SOURCE" VARCHAR2(250) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("VENDOR_NAME" VARCHAR2(250) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("VENDOR_ID" VARCHAR2(250) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("PERCENT_ACTIVE" NUMBER(10,5) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("AMOUNT_UNITS" VARCHAR(20) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("PURITY" VARCHAR(250) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("LC_UV_MS" VARCHAR(250) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("CHN_COMBUSTION" VARCHAR(250) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("UV_SPECTRUM" VARCHAR(250) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("LOGD" NUMBER (10,5)  null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("SOLUBILITY" VARCHAR2(250) null);

ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("COLLABORATOR_ID" VARCHAR2(15) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("PRODUCT_TYPE" VARCHAR2(250) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("CHIRAL" VARCHAR2 (250) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("CLOGP" NUMBER (10,5) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("H_BOND_DONORS" NUMBER (8,0) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("H_BOND_ACCEPTORS" NUMBER (8,0) null);
ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES" ADD ("STRUCTURE_COMMENTS_TXT" VARCHAR2(250) null);

ALTER TABLE REGDB.TEMPORARY_STRUCTURES ADD (FIELD_1 VARCHAR2(2000));
ALTER TABLE REGDB.TEMPORARY_STRUCTURES ADD (FIELD_2 VARCHAR2(2000));
ALTER TABLE REGDB.TEMPORARY_STRUCTURES ADD (FIELD_3 VARCHAR2(2000));
ALTER TABLE REGDB.TEMPORARY_STRUCTURES ADD (FIELD_4 VARCHAR2(2000));
ALTER TABLE REGDB.TEMPORARY_STRUCTURES ADD (FIELD_5 VARCHAR2(2000));
ALTER TABLE REGDB.TEMPORARY_STRUCTURES ADD (FIELD_6 VARCHAR2(2000));
ALTER TABLE REGDB.TEMPORARY_STRUCTURES ADD (FIELD_7 VARCHAR2(2000));
ALTER TABLE REGDB.TEMPORARY_STRUCTURES ADD (FIELD_8 VARCHAR2(2000));
ALTER TABLE REGDB.TEMPORARY_STRUCTURES ADD (FIELD_9 VARCHAR2(2000));
ALTER TABLE REGDB.TEMPORARY_STRUCTURES ADD (FIELD_10 VARCHAR2(2000));

ALTER TABLE REGDB.BATCHES ADD (FIELD_1 VARCHAR2(2000));
ALTER TABLE REGDB.BATCHES ADD (FIELD_2 VARCHAR2(2000));
ALTER TABLE REGDB.BATCHES ADD (FIELD_3 VARCHAR2(2000));
ALTER TABLE REGDB.BATCHES ADD (FIELD_4 VARCHAR2(2000));
ALTER TABLE REGDB.BATCHES ADD (FIELD_5 VARCHAR2(2000));
ALTER TABLE REGDB.BATCHES ADD (FIELD_6 VARCHAR2(2000));
ALTER TABLE REGDB.BATCHES ADD (FIELD_7 VARCHAR2(2000));
ALTER TABLE REGDB.BATCHES ADD (FIELD_8 VARCHAR2(2000));
ALTER TABLE REGDB.BATCHES ADD (FIELD_9 VARCHAR2(2000));
ALTER TABLE REGDB.BATCHES ADD (FIELD_10 VARCHAR2(2000));

-- ADD INDEXES

create index INDEX_REGNUM_ROOT_NUMBER on REG_NUMBERS (
	ROOT_NUMBER ASC)

--DROP INDEX INDEX_STRUCTURES_MOLID;
create index INDEX_STRUCTURES_MOLID on STRUCTURES(
	MOL_ID ASC);


create index INDEX_CMPDMOL_ROOT_NUM on COMPOUND_MOLECULE (
	ROOT_NUMBER ASC)

create index INDEX_TEMPSTRUC_CHEM_NAME on TEMPORARY_STRUCTURES (
	CHEMICAL_NAME ASC)


-- ADD PRIMARY KEY TO STUCTURES TABLE FOR INDEXING PURPOSES
create index INDEX_TEMPSTRUC_CHEM_NAME on TEMPORARY_STRUCTURES (
	CHEMICAL_NAME ASC)




--ADD PERMISSIONS TO DEFAULT ROLES
                 
GRANT SELECT ON SOLVATES TO BROWSER;
GRANT SELECT ON BATCH_PROJECTS TO BROWSER;
GRANT SELECT ON BATCH_PROJ_UTILIZATIONS TO BROWSER;
GRANT SELECT ON UTILIZATIONS TO BROWSER;
GRANT SELECT ON CMPD_MOL_UTILIZATIONS TO BROWSER;


GRANT SELECT ON SOLVATES TO SUBMITTER;
GRANT SELECT ON BATCH_PROJECTS TO SUBMITTER;
GRANT SELECT ON BATCH_PROJ_UTILIZATIONS TO SUBMITTER;
GRANT SELECT ON UTILIZATIONS TO SUBMITTER;
GRANT SELECT, INSERT ON CMPD_MOL_UTILIZATIONS TO SUBMITTER;


GRANT SELECT ON SOLVATES TO SUPERVISING_SCIENTIST;
GRANT SELECT ON BATCH_PROJECTS TO SUPERVISING_SCIENTIST;
GRANT SELECT ON BATCH_PROJ_UTILIZATIONS TO SUPERVISING_SCIENTIST;
GRANT SELECT ON UTILIZATIONS TO SUPERVISING_SCIENTIST;
GRANT SELECT, INSERT, UPDATE  ON CMPD_MOL_UTILIZATIONS TO SUPERVISING_SCIENTIST;


GRANT SELECT,INSERT,DELETE,UPDATE ON SOLVATES TO CHEMICAL_ADMINISTRATOR;
GRANT SELECT,INSERT,UPDATE ON BATCH_PROJECTS TO CHEMICAL_ADMINISTRATOR;
GRANT SELECT,INSERT,UPDATE ON BATCH_PROJ_UTILIZATIONS TO CHEMICAL_ADMINISTRATOR;
GRANT SELECT,INSERT,UPDATE ON UTILIZATIONS TO CHEMICAL_ADMINISTRATOR;
GRANT SELECT,INSERT,UPDATE ON CMPD_MOL_UTILIZATIONS TO CHEMICAL_ADMINISTRATOR;

GRANT SELECT,INSERT,DELETE,UPDATE ON SOLVATES TO SUPERVISING_CHEMICAL_ADMIN;
GRANT SELECT,INSERT,UPDATE,DELETE ON BATCH_PROJECTS TO SUPERVISING_CHEMICAL_ADMIN;
GRANT SELECT,INSERT,UPDATE,DELETE ON BATCH_PROJ_UTILIZATIONS TO SUPERVISING_CHEMICAL_ADMIN;
GRANT SELECT,INSERT,UPDATE,DELETE ON UTILIZATIONS TO SUPERVISING_CHEMICAL_ADMIN;
GRANT SELECT,INSERT,UPDATE,DELETE ON CMPD_MOL_UTILIZATIONS TO SUPERVISING_CHEMICAL_ADMIN;

GRANT SELECT ON SOLVATES TO PERFUME_CHEMIST;
GRANT SELECT,INSERT,UPDATE ON BATCH_PROJECTS TO PERFUME_CHEMIST;
GRANT SELECT ON BATCH_PROJ_UTILIZATIONS TO PERFUME_CHEMIST;
GRANT SELECT ON UTILIZATIONS TO PERFUME_CHEMIST;
GRANT SELECT ON CMPD_MOL_UTILIZATIONS TO PERFUME_CHEMIST;


Connect &&InstallUser/&&sysPass@&&serverName

DROP  PUBLIC SYNONYM "SOLVATES";
CREATE PUBLIC SYNONYM "SOLVATES" FOR "REGDB"."SOLVATES";


DROP  PUBLIC SYNONYM "BATCH_PROJECTS";
CREATE PUBLIC SYNONYM "BATCH_PROJECTS" FOR "REGDB"."BATCH_PROJECTS";

DROP  PUBLIC SYNONYM "BATCH_PROJ_UTILIZATIONS";
CREATE PUBLIC SYNONYM "BATCH_PROJ_UTILIZATIONS" FOR "REGDB"."BATCH_PROJ_UTILIZATIONS";

DROP  PUBLIC SYNONYM "UTILIZATIONS";
CREATE PUBLIC SYNONYM "UTILIZATIONS" FOR "REGDB"."UTILIZATIONS";

DROP  PUBLIC SYNONYM "CMPD_MOL_UTILIZATIONS";
CREATE PUBLIC SYNONYM "CMPD_MOL_UTILIZATIONS" FOR "REGDB"."CMPD_MOL_UTILIZATIONS";

commit;
