--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

PROMPT Starting Migration data script from CS_Security to CoeDB

ALTER TRIGGER &&schemaName..SITES_TRIG DISABLE;
ALTER TRIGGER &&schemaName..PEOPLE_TRIG DISABLE;
ALTER TRIGGER &&schemaName..SECURITY_ROLES_TRIG DISABLE;
ALTER TRIGGER &&schemaName..PRIVILEGE_TABLES_TRIG DISABLE;

DELETE &&schemaName..sites;
DELETE &&schemaName..security_roles;
DELETE &&schemaName..privilege_tables;
DELETE &&schemaName..people;
DELETE &&schemaName..object_privileges;
DELETE &&schemaName..cs_security_privileges;
DELETE &&schemaName..audit_row;
DELETE &&schemaName..audit_delete;
DELETE &&schemaName..audit_column;

 insert into &&schemaName..audit_column (RAID, CAID, COLUMN_NAME, OLD_VALUE, NEW_VALUE) 
(select RAID, CAID, COLUMN_NAME, OLD_VALUE, NEW_VALUE
   from &&securitySchemaName..audit_column);

 insert into &&schemaName..audit_delete (RAID, ROW_DATA)
(select RAID, ROW_DATA
   from &&securitySchemaName..audit_delete);

 insert into &&schemaName..audit_row (RAID, TABLE_NAME, RID, ACTION, TIMESTAMP, USER_NAME)
(select RAID, TABLE_NAME, RID, ACTION, TIMESTAMP, USER_NAME
   from &&securitySchemaName..audit_row);

 insert into &&schemaName..cs_security_privileges (ROLE_INTERNAL_ID, CSS_LOGIN, CSS_CREATE_USER, CSS_EDIT_USER, CSS_DELETE_USER, CSS_CHANGE_PASSWORD, CSS_CREATE_ROLE, CSS_EDIT_ROLE, CSS_DELETE_ROLE, CSS_CREATE_WORKGRP, CSS_EDIT_WORKGRP, CSS_DELETE_WORKGRP, RID, CREATOR, TIMESTAMP)
(select ROLE_INTERNAL_ID, CSS_LOGIN, CSS_CREATE_USER, CSS_EDIT_USER, CSS_DELETE_USER, CSS_CHANGE_PASSWORD, CSS_CREATE_ROLE, CSS_EDIT_ROLE, CSS_DELETE_ROLE, CSS_CREATE_WORKGRP, CSS_EDIT_WORKGRP, CSS_DELETE_WORKGRP, RID, CREATOR, TIMESTAMP
   from &&securitySchemaName..cs_security_privileges);

 insert into &&schemaName..object_privileges (PRIVILEGE_NAME, PRIVILEGE, "SCHEMA", OBJECT_NAME)
(select distinct PRIVILEGE_NAME, PRIVILEGE, "SCHEMA", OBJECT_NAME
   from &&securitySchemaName..object_privileges);

 insert into &&schemaName..sites (SITE_ID, SITE_CODE, SITE_NAME, ACTIVE)
(select SITE_ID, SITE_CODE, SITE_NAME, ACTIVE
   from &&securitySchemaName..sites);

 insert into &&schemaName..people (PERSON_ID, USER_CODE, USER_ID, SUPERVISOR_INTERNAL_ID, TITLE, FIRST_NAME, MIDDLE_NAME, LAST_NAME, SITE_ID, DEPARTMENT, INT_ADDRESS, TELEPHONE, EMAIL, ACTIVE, RID, CREATOR, TIMESTAMP)
(select PERSON_ID, USER_CODE, USER_ID, SUPERVISOR_INTERNAL_ID, TITLE, FIRST_NAME, MIDDLE_NAME, LAST_NAME, NVL((SELECT SITE_ID FROM Sites  S WHERE P.SITE_ID=S.SITE_ID ), 1), DEPARTMENT, INT_ADDRESS, TELEPHONE, EMAIL, ACTIVE, RID, CREATOR, TIMESTAMP
   from &&securitySchemaName..people P);

 insert into &&schemaName..privilege_tables (PRIVILEGE_TABLE_ID, PRIVILEGE_TABLE_NAME, APP_NAME, APP_URL, TABLE_SPACE)
(select PRIVILEGE_TABLE_ID, PRIVILEGE_TABLE_NAME, APP_NAME, APP_URL, TABLE_SPACE
   from &&securitySchemaName..privilege_tables);

 insert into &&schemaName..security_roles (role_id, privilege_table_int_id, role_name, rid, creator, TIMESTAMP)
   (SELECT role_id, privilege_table_int_id, role_name, rid, creator, TIMESTAMP
      FROM &&securitySchemaName..security_roles sr);




DROP SEQUENCE SITES_SEQ;
DROP SEQUENCE PEOPLE_SEQ;
DROP SEQUENCE SECURITY_ROLES_SEQ;
DROP SEQUENCE PRIVILEGE_TABLES_seq;

DECLARE
  MaxID VARCHAR2(10);
BEGIN
  SELECT NVL(MAX(Site_ID),0)+1 INTO MaxID FROM &&schemaName..Sites;
  EXECUTE IMMEDIATE 'CREATE SEQUENCE Sites_Seq INCREMENT BY 1 START WITH '||MaxID;
END;
/
DECLARE
  MaxID VARCHAR2(10);
BEGIN
  SELECT NVL(MAX(Person_ID),0)+1 INTO MaxID FROM &&schemaName..People;
  EXECUTE IMMEDIATE 'CREATE SEQUENCE PEOPLE_SEQ INCREMENT BY 1 START WITH '||MaxID;
END;
/
DECLARE
  MaxID VARCHAR2(10);
BEGIN
  SELECT NVL(MAX(Role_ID),0)+1 INTO MaxID FROM &&schemaName..Security_Roles;
  EXECUTE IMMEDIATE 'CREATE SEQUENCE SECURITY_ROLES_SEQ INCREMENT BY 1 START WITH '||MaxID;
END;
/
DECLARE
  MaxID VARCHAR2(10);
BEGIN
  SELECT NVL(MAX(Privilege_Table_ID),0)+1 INTO MaxID FROM &&schemaName..Privilege_Tables;
  EXECUTE IMMEDIATE 'CREATE SEQUENCE PRIVILEGE_TABLES_seq INCREMENT BY 1 START WITH '||MaxID;
END;
/

ALTER TRIGGER &&schemaName..SITES_TRIG ENABLE;
ALTER TRIGGER &&schemaName..PEOPLE_TRIG ENABLE;
ALTER TRIGGER &&schemaName..SECURITY_ROLES_TRIG ENABLE;
ALTER TRIGGER &&schemaName..PRIVILEGE_TABLES_TRIG ENABLE;


-- Create AS Select to copy application privilege tables from cs_security to COEDB.
DECLARE
   CURSOR cTables IS
      SELECT privilege_table_name tableName
        FROM &&securitySchemaName..privilege_tables
       WHERE NOT privilege_table_name = 'CS_SECURITY_PRIVILEGES';
BEGIN
   FOR rTables IN cTables LOOP
     BEGIN    
      EXECUTE IMMEDIATE    'CREATE TABLE &&schemaName..'
                        || rTables.tableName
                        || ' AS (Select * from &&securitySchemaName..'
                        || rTables.tableName
                        || ')';
						
	  EXECUTE IMMEDIATE '
							alter table &&schemaName..' || rTables.tableName || ' add constraint PK_' || rTables.tableName || ' primary key(role_internal_id)
						';
						  					
     EXCEPTION
        WHEN OTHERS THEN NULL;
     END;
   END LOOP;
END;
/


--Updating CoeIdentifier of the Roles
DECLARE 

    CURSOR C_Tables IS
        SELECT Privilege_Table_Name, 
              DECODE(UPPER(Privilege_Table_Name),
                    'CS_SECURITY_PRIVILEGES'   ,'COE',
                    'CHEMINV_PRIVILEGES'       ,'INVENTORY',
                    'BIOSAR_BROWSER_PRIVILEGES','BIOSARBROWSER',
                    'DOCMANAGER_PRIVILEGES'    ,'DOCMANAGER',
                    'DRUGDEG_PRIVILEGES'       ,'DRUGDEG',
                    'D3_PRIVILEGES'            ,'D3',
                    'BIOASSAY_PRIVILEGES'      ,'BIOASSAYHTS',
                    'CHEMACX_PRIVILEGES'       ,'CHEMACX',
                    'COE_SECMANAGER_PRIVILEGES','COEMANAGER_SEC',
                    'COE_DVMANAGER_PRIVILEGES' ,'COEMANAGER_DV',
                    'CHEM_REG_PRIVILEGES'      ,'REGISTRATION',
                    'CBV_PRIVILEGES'           ,'CHEMBIOVIZ CLIENT') COEIdentifier
        FROM &&schemaName..Privilege_Tables;

    LExist NUMBER;            

BEGIN
    FOR R_Tables IN C_Tables LOOP
     
        SELECT Count(1)
            INTO LExist 
            FROM DBA_Tables 
            WHERE  Table_Name=R_Tables.Privilege_Table_Name AND UPPER(Owner)=UPPER('&&schemaName');
            
        IF LExist>0 THEN

            EXECUTE IMMEDIATE 
            'UPDATE Security_Roles SET 
                COEIdentifier='''||R_Tables.COEIdentifier||'''
                WHERE Role_ID IN (SELECT Role_Internal_ID FROM &&schemaName..'||R_Tables.Privilege_Table_Name||')';

        END IF;

    END LOOP;
END;    
/

Commit;

UPDATE &&schemaName..People
 SET site_id =1
 WHERE Person_ID IN (SELECT P.Person_ID
       FROM CoeDB.People P, CoeDB.Sites S
       WHERE P.Site_ID = S.Site_ID(+)
       AND S.Site_ID IS Null);

Commit;


--#########################################################
--REGISTRATION 
--#########################################################

DECLARE
    LExist NUMBER;    
BEGIN
	-- If Registration privileges table exists...
    SELECT Count(1) INTO LExist FROM USER_TABLES WHERE Table_Name='CHEM_REG_PRIVILEGES';
    IF LExist=1 THEN
		-- Add New application privileges
        EXECUTE IMMEDIATE '
							ALTER TABLE CHEM_REG_PRIVILEGES ADD(
							REGISTER_DIRECT NUMBER(1,0),
							CONFIG_REG NUMBER(1,0),
							ADD_COMPONENT NUMBER(1,0),
							ADD_PICKLIST_TABLE NUMBER(1,0),
							EDIT_PICKLIST_TABLE NUMBER(1,0),
							DELETE_PICKLIST_TABLE NUMBER(1,0),
							ADD_IDENTIFIER_TYPE_TABLE NUMBER(1,0),
							EDIT_IDENTIFIER_TYPE_TABLE NUMBER(1,0),
							DELETE_IDENTIFIER_TYPE_TABLE NUMBER(1,0))
						  ';


		-- Denny all roles the new privileges
        EXECUTE IMMEDIATE '
							UPDATE CHEM_REG_PRIVILEGES SET
							REGISTER_DIRECT=0,
							CONFIG_REG=0,
							ADD_COMPONENT=0,
							ADD_PICKLIST_TABLE=0,
							EDIT_PICKLIST_TABLE=0,
							DELETE_PICKLIST_TABLE=0,
							ADD_IDENTIFIER_TYPE_TABLE=0,
							EDIT_IDENTIFIER_TYPE_TABLE=0,
							DELETE_IDENTIFIER_TYPE_TABLE=0
						  ';

		-- Allow supervising_chemical_admin the new privileges
        EXECUTE IMMEDIATE '
							UPDATE CHEM_REG_PRIVILEGES SET
							REGISTER_DIRECT=1,
							CONFIG_REG=1,
							ADD_COMPONENT=1,
							ADD_PICKLIST_TABLE=1,
							EDIT_PICKLIST_TABLE=1,
							DELETE_PICKLIST_TABLE=1,
							ADD_IDENTIFIER_TYPE_TABLE=1,
							EDIT_IDENTIFIER_TYPE_TABLE=1,
							DELETE_IDENTIFIER_TYPE_TABLE=1
							WHERE role_internal_id = (Select role_id from security_roles where upper(role_name) = ''SUPERVISING_CHEMICAL_ADMIN'')
						  ';

		-- Allow chemical_administrator the REGISTER_DIRECT privilege
        EXECUTE IMMEDIATE '
							UPDATE CHEM_REG_PRIVILEGES SET REGISTER_DIRECT	=1 WHERE role_internal_id =
								(Select role_id from security_roles where upper(role_name) = ''CHEMICAL_ADMINISTRATOR'')
						  ';
						  
		
    END IF;
END;
/