
Connect &&SchemaName/Oracle@&&serverName




CREATE TABLE USER_SETTINGS(
	SETTING_NAME VARCHAR2(255) NOT NULL,
	USER_ID VARCHAR2(30) NOT NULL,
	SETTING_VALUE VARCHAR2(4000) NULL, 
	LAST_UPDATE DATE, 
	CONSTRAINT PK_USER_SETTINGS
	PRIMARY KEY(SETTING_NAME,USER_ID));
				
	create sequence USER_ID_SEQ increment by 1 start with 1;				
			
	GRANT insert, select, update, delete ON USER_SETTINGS to PUBLIC;

CREATE TABLE DB_QUERY (
	QUERY_ID NUMBER(38) not null,
	SESSION_ID NUMBER (38),
	QUERY_NAME VARCHAR2(250),
	DESCRIPTION VARCHAR2(250),
	IS_PUBLIC VARCHAR2(1) not null,
	USER_ID VARCHAR2(250),
	FORMGROUP VARCHAR2(250),
	DBNAME VARCHAR2(250),
	APPNAME VARCHAR2(250),
	NUMBER_HITS NUMBER (38),
	REC_DATE DATE, 
	constraint PK_QUERY_ID primary key (QUERY_ID)
	USING INDEX PCTFREE 0
	TABLESPACE &&indexTableSpaceName
)
TABLESPACE &&tableSpaceName
STORAGE (PCTINCREASE 0);

GRANT insert, select, update, delete ON DB_QUERY to PUBLIC;



CREATE TABLE DB_QUERY_ITEM  (
  	QUERY_ID NUMBER(38) not null,
	FIELD_NAME VARCHAR2(250),
	FIELD_VALUE VARCHAR2(250),
	BASE64_CDX CLOB,
	STRUC_SEARCH_TYPE VARCHAR2(250),
	FIELD_TYPE VARCHAR2(250),
	DATA_TYPE VARCHAR2(250),
    	CONSTRAINT QUERY_ID_FK FOREIGN KEY(QUERY_ID) 
    	REFERENCES BIOSARDB.DB_QUERY(QUERY_ID) 
    	ON DELETE CASCADE, 
    	CONSTRAINT PK_QUERY_ITEM PRIMARY KEY(QUERY_ID, FIELD_NAME) 
    	USING INDEX PCTFREE 0
	TABLESPACE &&indexTableSpaceName)
TABLESPACE &&tableSpaceName
STORAGE (PCTINCREASE 0);

GRANT insert, select, update, delete ON DB_QUERY_ITEM  to PUBLIC;

create sequence DB_QUERY_SEQ increment by 1 start with 1;

GRANT insert, select, update, delete ON DB_QUERY to &&securitySchemaName with grant option;
GRANT insert, select, update, delete ON DB_QUERY_ITEM to &&securitySchemaName with grant option;
GRANT insert, select, update, delete ON USER_SETTINGS to &&securitySchemaName with grant option;






-- Disable the biosar triggers for data transfer and grant all select.

Connect &&InstallUser/&&sysPass@&&serverName;
grant select any table to biosardb;

Connect &&SchemaName/Oracle@&&serverName;

ALTER TRIGGER BIOSARDB.DB_COLUMN_TRIG 
    DISABLE;
    
ALTER TRIGGER BIOSARDB.DB_TABLE_TRIG 
    DISABLE;

ALTER TRIGGER BIOSARDB.DB_FORMGROUP_TRIG 
    DISABLE;

ALTER TRIGGER BIOSARDB.DB_FORM_TRIG 
    DISABLE;

ALTER TRIGGER BIOSARDB.DB_QUERY_TRIG 
    DISABLE;

ALTER TRIGGER BIOSARDB.DB_FORM_ITEM_TRIP 
    DISABLE;




Connect &&InstallUser/&&sysPass@&&serverName

prompt '**********transferring data from cs_security to biosardb ...'
--DB_TABLE
INSERT INTO BIOSARDB.DB_TABLE ( TABLE_ID, OWNER, 
TABLE_NAME, TABLE_SHORT_NAME, DISPLAY_NAME, 
BASE_COLUMN_ID, DESCRIPTION, IS_EXPOSED, IS_VIEW ) 
(SELECT TABLE_ID, OWNER, TABLE_NAME, TABLE_SHORT_NAME,
DISPLAY_NAME, BASE_COLUMN_ID, DESCRIPTION, IS_EXPOSED,
IS_VIEW 
FROM &&securitySchemaName..DB_TABLE);

--RESET SEQUENCE
DECLARE
  maxID INTEGER;
  currID INTEGER;
  i number;
  j number;
BEGIN
  SELECT BIOSARDB.DB_TABLE_SEQ.nextval into currid from dual;
  SELECT MAX(TABLE_ID) INTO maxID FROM BIOSARDB.DB_TABLE;
  for i in currid..maxid loop
    select BIOSARDB.DB_TABLE_SEQ.nextval into j from dual;
  end loop;
END;
/

--DB_COLUMN
INSERT INTO BIOSARDB.DB_COLUMN ( COLUMN_ID, TABLE_ID,
    COLUMN_NAME, DISPLAY_NAME, DESCRIPTION, IS_VISIBLE, 
    DATATYPE, LOOKUP_TABLE_ID, LOOKUP_COLUMN_ID, 
    LOOKUP_COLUMN_DISPLAY, MST_FILE_PATH, LENGTH, SCALE, 
    PRECISION, NULLABLE ) 
(SELECT COLUMN_ID, TABLE_ID, COLUMN_NAME, DISPLAY_NAME,
    DESCRIPTION, IS_VISIBLE, DATATYPE, LOOKUP_TABLE_ID, 
    LOOKUP_COLUMN_ID, LOOKUP_COLUMN_DISPLAY, MST_FILE_PATH,
    LENGTH, SCALE, PRECISION, NULLABLE 
FROM &&securitySchemaName..DB_COLUMN); 

--RESET SEQUENCE
DECLARE
  maxID INTEGER;
  currID INTEGER;
  i number;
  j number;
BEGIN
  SELECT BIOSARDB.DB_COLUMN_SEQ.nextval into currid from dual;
  SELECT MAX(COLUMN_ID) INTO maxID FROM BIOSARDB.DB_COLUMN;
  for i in currid..maxid loop
    select BIOSARDB.DB_COLUMN_SEQ.nextval into j from dual;
  end loop;
END;
/

--DB_FORMGROUP
INSERT INTO BIOSARDB.DB_FORMGROUP ( FORMGROUP_ID, 
FORMGROUP_NAME, USER_ID, IS_PUBLIC, DESCRIPTION, 
BASE_TABLE_ID, CREATED_DATE ) 
(SELECT FORMGROUP_ID, FORMGROUP_NAME, USER_ID, 
IS_PUBLIC, DESCRIPTION, BASE_TABLE_ID, CREATED_DATE 
FROM &&securitySchemaName..DB_FORMGROUP);

--RESET SEQUENCE
DECLARE
  maxID INTEGER;
  currID INTEGER;
  i number;
  j number;
BEGIN
  SELECT BIOSARDB.DB_FORMGROUP_SEQ.nextval into currid from dual;
  SELECT MAX(FORMGROUP_ID) INTO maxID FROM BIOSARDB.DB_FORMGROUP;
  for i in currid..maxid loop
    select BIOSARDB.DB_FORMGROUP_SEQ.nextval into j from dual;
  end loop;
END;
/

--DB_FORM
INSERT INTO BIOSARDB.DB_FORM ( FORM_ID, FORM_NAME, 
FORMGROUP_ID, FORMTYPE_ID, URL ) 
(SELECT FORM_ID, FORM_NAME, FORMGROUP_ID, FORMTYPE_ID,
URL 
FROM &&securitySchemaName..DB_FORM);

--RESET SEQUENCE
DECLARE
  maxID INTEGER;
  currID INTEGER;
  i number;
  j number;
BEGIN
  SELECT BIOSARDB.DB_FORM_SEQ.nextval into currid from dual;
  SELECT MAX(FORM_ID) INTO maxID FROM BIOSARDB.DB_FORM;
  for i in currid..maxid loop
    select BIOSARDB.DB_FORM_SEQ.nextval into j from dual;
  end loop;
END;
/

--DB_FORMGROUP_TABLES
INSERT INTO BIOSARDB.DB_FORMGROUP_TABLES ( FORMGROUP_ID,
TABLE_ID, TABLE_ORDER, TABLE_REL_ORDER )
(SELECT FORMGROUP_ID, TABLE_ID, TABLE_ORDER, 
TABLE_REL_ORDER 
FROM &&securitySchemaName..DB_FORMGROUP_TABLES);

--DB_FORM_ITEM
INSERT INTO BIOSARDB.DB_FORM_ITEM ( FORM_ITEM_ID, 
FORM_ID, TABLE_ID, COLUMN_ID, DISP_TYP_ID, 
DISP_OPT_ID, WIDTH, HEIGHT, COLUMN_ORDER ) 
(SELECT FORM_ITEM_ID, FORM_ID, TABLE_ID, COLUMN_ID, 
DISP_TYP_ID, DISP_OPT_ID, WIDTH, HEIGHT, 
COLUMN_ORDER 
FROM &&securitySchemaName..DB_FORM_ITEM);

--USER_SETTINGS
INSERT INTO BIOSARDB.USER_SETTINGS (SETTING_NAME, 
USER_ID, SETTING_VALUE, LAST_UPDATE ) 
(SELECT SETTING_NAME, USER_ID, SETTING_VALUE, LAST_UPDATE
FROM &&securitySchemaName..USER_SETTINGS);



--RESET SEQUENCE
DECLARE
  maxID INTEGER;
  currID INTEGER;
  i number;
  j number;
BEGIN
  SELECT BIOSARDB.DB_FORM_ITEM_SEQ.nextval into currID from dual;
  SELECT MAX(FORM_ITEM_ID) INTO maxID FROM BIOSARDB.DB_FORM_ITEM;
  for i in currid..maxid loop
    select BIOSARDB.DB_FORM_ITEM_SEQ.nextval into j from dual;
  end loop;
END;
/

--DB_RELATIONSHIP
INSERT INTO BIOSARDB.DB_RELATIONSHIP ( COLUMN_ID, 
TABLE_ID, CHILD_COLUMN_ID, CHILD_TABLE_ID ) 
(SELECT COLUMN_ID, TABLE_ID, CHILD_COLUMN_ID, 
CHILD_TABLE_ID 
FROM &&securitySchemaName..DB_RELATIONSHIP);

--DB_SCHEMA
INSERT INTO BIOSARDB.DB_SCHEMA ( OWNER, DISPLAY_NAME,
SCHEMA_PASSWORD ) 
(SELECT OWNER, DISPLAY_NAME, SCHEMA_PASSWORD 
FROM &&securitySchemaName..DB_SCHEMA);



--DB_QUERY
INSERT INTO BIOSARDB.DB_QUERY ( QUERY_ID, SESSION_ID,
QUERY_NAME, DESCRIPTION, IS_PUBLIC, USER_ID, 
FORMGROUP, DBNAME, APPNAME, NUMBER_HITS, REC_DATE )
(SELECT QUERY_ID, SESSION_ID, QUERY_NAME, DESCRIPTION,
IS_PUBLIC, USER_ID, FORMGROUP, DBNAME, APPNAME, 
NUMBER_HITS, REC_DATE 
FROM &&securitySchemaName..DB_QUERY);


--RESET SEQUENCE
DECLARE
  maxID INTEGER;
  currID INTEGER;
  i number;
  j number;
BEGIN
  SELECT BIOSARDB.DB_QUERY_SEQ.nextval into currID from dual;
  SELECT MAX(QUERY_ID) INTO maxID FROM BIOSARDB.DB_QUERY;
  for i in currid..maxid loop
    select BIOSARDB.DB_QUERY_SEQ.nextval into j from dual;
  end loop;
END;
/

--DB_QUERY_ITEM
INSERT INTO BIOSARDB.DB_QUERY_ITEM ( QUERY_ID, 
FIELD_NAME, FIELD_VALUE, 
STRUC_SEARCH_TYPE, FIELD_TYPE, DATA_TYPE, BASE64_CDX) 
(SELECT QUERY_ID, FIELD_NAME, FIELD_VALUE,
STRUC_SEARCH_TYPE, FIELD_TYPE, DATA_TYPE , to_lob(BASE64_CDX) FROM &&securitySchemaName..DB_QUERY_ITEM);



commit;


--Enable the biosar triggers and revoke the all select.

Connect &&InstallUser/&&sysPass@&&serverName

revoke select any table from biosardb;

Connect &&SchemaName/Oracle@&&serverName;

ALTER TRIGGER BIOSARDB.DB_COLUMN_TRIG 
    ENABLE;
    
ALTER TRIGGER BIOSARDB.DB_TABLE_TRIG 
    ENABLE;

ALTER TRIGGER BIOSARDB.DB_FORMGROUP_TRIG 
    ENABLE;

ALTER TRIGGER BIOSARDB.DB_FORM_TRIG 
    ENABLE;

ALTER TRIGGER BIOSARDB.DB_QUERY_TRIG 
    ENABLE;

ALTER TRIGGER BIOSARDB.DB_FORM_ITEM_TRIP 
    ENABLE;


--NOW update db_item field definitions

prompt '**********updating chemreg and cheminv meta data ...'

UPDATE DB_COLUMN  SET DATATYPE='CLOB' WHERE COLUMN_NAME='BASE64_CDX';
commit;
UPDATE DB_COLUMN  SET INDEX_TYPE_ID='2' WHERE COLUMN_NAME='BASE64_CDX' and DATATYPE='CLOB';
UPDATE DB_COLUMN  SET CONTENT_TYPE_ID='5' WHERE COLUMN_NAME='BASE64_CDX' and DATATYPE='CLOB';
commit;

--Set lookups for Structure, compound_molecule, reg_number and batches tables
UPDATE DB_COLUMN SET LOOKUP_TABLE_ID=(select table_id from db_table where table_name ='REGDB.STRUCTURES') where column_name='MOL_ID' and table_id=(select table_id from db_table where table_name = 'REGDB.REG_NUMBERS');
UPDATE DB_COLUMN SET LOOKUP_COLUMN_ID=(select column_id from db_column where column_name='MOL_ID' and table_id =(select table_id from db_table where table_name ='REGDB.STRUCTURES')) where column_name='MOL_ID' and table_id=(select table_id from db_table where table_name = 'REGDB.REG_NUMBERS');
UPDATE DB_COLUMN SET LOOKUP_COLUMN_DISPLAY=(select column_id from db_column where column_name='BASE64_CDX' and table_id =(select table_id from db_table where table_name ='REGDB.STRUCTURES')) where column_name='MOL_ID' and table_id=(select table_id from db_table where table_name = 'REGDB.REG_NUMBERS');
commit;

UPDATE DB_COLUMN SET LOOKUP_TABLE_ID=(select table_id from db_table where table_name ='REGDB.STRUCTURES') where column_name='MOL_ID' and table_id=(select table_id from db_table where table_name = 'REGDB.COMPOUND_MOLECULE');
UPDATE DB_COLUMN SET LOOKUP_COLUMN_ID=(select column_id from db_column where column_name='MOL_ID' and table_id =(select table_id from db_table where table_name ='REGDB.STRUCTURES')) where column_name='MOL_ID' and table_id=(select table_id from db_table where table_name = 'REGDB.COMPOUND_MOLECULE');
UPDATE DB_COLUMN SET LOOKUP_COLUMN_DISPLAY=(select column_id from db_column where column_name='BASE64_CDX' and table_id =(select table_id from db_table where table_name ='REGDB.STRUCTURES')) where column_name='MOL_ID' and table_id=(select table_id from db_table where table_name = 'REGDB.COMPOUND_MOLECULE');
commit;

UPDATE DB_COLUMN SET LOOKUP_TABLE_ID=(select table_id from db_table where table_name ='REGDB.STRUCTURES') where column_name='MOL_ID' and table_id=(select table_id from db_table where table_name = 'REGDB.BATCHES');
UPDATE DB_COLUMN SET LOOKUP_COLUMN_ID=(select column_id from db_column where column_name='MOL_ID' and table_id =(select table_id from db_table where table_name ='REGDB.STRUCTURES')) where column_name='MOL_ID' and table_id=(select table_id from db_table where table_name = 'REGDB.BATCHES');
UPDATE DB_COLUMN SET LOOKUP_COLUMN_DISPLAY=(select column_id from db_column where column_name='BASE64_CDX' and table_id =(select table_id from db_table where table_name ='REGDB.STRUCTURES')) where column_name='MOL_ID' and table_id=(select table_id from db_table where table_name = 'REGDB.BATCHES');
commit;


UPDATE DB_COLUMN SET LOOKUP_TABLE_ID=(select table_id from db_table where table_name ='REGDB.STRUCTURES') where column_name='MOL_ID' and table_id=(select table_id from db_table where table_name = 'REGDB.COMPOUND_SALT');
UPDATE DB_COLUMN SET LOOKUP_COLUMN_ID=(select column_id from db_column where column_name='MOL_ID' and table_id =(select table_id from db_table where table_name ='REGDB.STRUCTURES')) where column_name='MOL_ID' and table_id=(select table_id from db_table where table_name = 'REGDB.COMPOUND_SALT');
UPDATE DB_COLUMN SET LOOKUP_COLUMN_DISPLAY=(select column_id from db_column where column_name='BASE64_CDX' and table_id =(select table_id from db_table where table_name ='REGDB.STRUCTURES')) where column_name='MOL_ID' and table_id=(select table_id from db_table where table_name = 'REGDB.COMPOUND_SALT');
commit;


UPDATE DB_COLUMN SET LOOKUP_TABLE_ID=NULL where column_name='MOL_ID' and table_id=(select table_id from db_table where table_name = 'REGDB.STRUCTURES');
UPDATE DB_COLUMN SET LOOKUP_COLUMN_ID=NULL where column_name='MOL_ID' and table_id=(select table_id from db_table where table_name = 'REGDB.STRUCTURES');
UPDATE DB_COLUMN SET LOOKUP_COLUMN_DISPLAY=NULL where column_name='MOL_ID' and table_id=(select table_id from db_table where table_name = 'REGDB.STRUCTURES');
commit;

--UpdateInv
UPDATE DB_COLUMN SET LOOKUP_TABLE_ID=(select table_id from db_table where table_name ='CHEMINVDB2.INV_COMPOUNDS') where column_name='COMPOUND_ID' and table_id=(select table_id from db_table where table_name = 'CHEMINVDB2.INV_WELLS');
UPDATE DB_COLUMN SET LOOKUP_COLUMN_ID=(select column_id from db_column where column_name='MOL_ID' and table_id =(select table_id from db_table where table_name ='REGDB.STRUCTURES')) where column_name='COMPOUND_ID' and table_id=(select table_id from db_table where table_name = 'CHEMINVDB2.INV_WELLS');
UPDATE DB_COLUMN SET LOOKUP_COLUMN_DISPLAY=(select column_id from db_column where column_name='BASE64_CDX' and table_id =(select table_id from db_table where table_name ='REGDB.STRUCTURES')) where column_name='COMPOUND_ID' and table_id=(select table_id from db_table where table_name = 'CHEMINVDB2.INV_WELLS');
commit;

UPDATE DB_COLUMN SET LOOKUP_TABLE_ID=(select table_id from db_table where table_name ='CHEMINVDB2.INV_COMPOUNDS') where column_name='COMPOUND_ID' and table_id=(select table_id from db_table where table_name = 'CHEMINVDB2.INV_CONTAINERS');
UPDATE DB_COLUMN SET LOOKUP_COLUMN_ID=(select column_id from db_column where column_name='MOL_ID' and table_id =(select table_id from db_table where table_name ='REGDB.STRUCTURES')) where column_name='COMPOUND_ID' and table_id=(select table_id from db_table where table_name = 'CHEMINVDB2.INV_CONTAINERS');
UPDATE DB_COLUMN SET LOOKUP_COLUMN_DISPLAY=(select column_id from db_column where column_name='BASE64_CDX' and table_id =(select table_id from db_table where table_name ='REGDB.STRUCTURES')) where column_name='COMPOUND_ID' and table_id=(select table_id from db_table where table_name = 'CHEMINVDB2.INV_CONTAINERS');
commit;

UPDATE DB_COLUMN SET LOOKUP_TABLE_ID=NULL where column_name='COMPOUND_ID' and table_id=(select table_id from db_table where table_name = 'CHEMINVDB2.INV_COMPOUNDS');
UPDATE DB_COLUMN SET LOOKUP_COLUMN_ID=NULL where column_name='COMPOUND_ID' and table_id=(select table_id from db_table where table_name = 'CHEMINVDB2.INV_COMPOUNDS');
UPDATE DB_COLUMN SET LOOKUP_COLUMN_DISPLAY=NULL where column_name='COMPOUND_ID' and table_id=(select table_id from db_table where table_name = 'CHEMINVDB2.INV_COMPOUNDS');
commit;

UPDATE DB_FORM_ITEM f SET
f.V_COLUMN_ID = decode(f.disp_typ_id,7,'-2',9,'-3',10,'-4')||(select column_id from db_column where table_id = SubStr(f.column_id,3,length(f.column_id)) AND mst_file_path is Not Null),
f.COLUMN_ID = (select column_id from db_column where table_id = SubStr(f.column_id,3,length(f.column_id)) AND mst_file_path is Not Null)
WHERE f.DISP_TYP_ID IN (7,9,10);

commit;


execute biosardb.ValidateAndRepairSchema.UpdateOutOfSyncTableColumns;
execute biosardb.ValidateAndRepairSchema.InsertMissingTableColumns;
commit;


