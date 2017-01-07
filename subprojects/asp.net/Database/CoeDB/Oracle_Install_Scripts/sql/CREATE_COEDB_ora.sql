--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

--Creation script for coedb/Oracle

--#########################################################
--CREATE Logs OBJECTS
--#########################################################

Connect &&schemaName/&&schemaPass@&&serverName


CREATE TABLE Log
(
  ID            NUMBER(8),
  LogDate       DATE        DEFAULT SYSDATE,
  LogUser       VARCHAR(30) DEFAULT USER,
  PC            VARCHAR(30) DEFAULT USERENV('TERMINAL'),
  LogProcedure  CLOB,
  LogComment    CLOB
);

ALTER TABLE Log ADD CONSTRAINT LogPK PRIMARY KEY (ID) USING INDEX TABLESPACE &&indexTableSpaceName;

CREATE SEQUENCE LogSeq
  START WITH 1
  INCREMENT BY 1
  MINVALUE 1
  CACHE 2
  NOCYCLE
  NOORDER;

@@sql\triggers\coedb_log.trg;


--#########################################################
--CREATE Partition Managment Parameter OBJECTS
--#########################################################


CREATE TABLE COEPARTITIONMANAGMENT 
( 
  TableName                        VARCHAR2(30 BYTE) NOT NULL, 
  Owner                            VARCHAR2(30 BYTE) NOT NULL, 
  CountHitListIDByPartition        INTEGER, 
  LiveDays                         INTEGER, 
  LiveWeeks                        INTEGER, 
  LiveMonths                      INTEGER, 
  HoursBetweenUpdate               INTEGER, 
  CONSTRAINT PK_COEPARTITIONMANAGMENT PRIMARY KEY (TableName,Owner)
);


INSERT INTO COEPARTITIONMANAGMENT(TableName,Owner,CountHitListIDByPartition,LiveDays,LiveWeeks,LiveMonths,HoursBetweenUpdate)
VALUES('COETEMPHITLISTID','COEDB',100000,0,0,6,12);

COMMIT;

@Sql\Packages\pkg_PartitionManagment.sql


--#########################################################
--CREATE TABLES
--#########################################################

CREATE TABLE COEDATAVIEW
(
  ID            NUMBER(9)                      NOT NULL,
  NAME          VARCHAR2(255 BYTE),
  DESCRIPTION   VARCHAR2(255 BYTE),
  USER_ID       VARCHAR2(30 BYTE),
  IS_PUBLIC     VARCHAR2(1 BYTE)                NOT NULL,
  FORMGROUP     NUMBER(9),
  DATE_CREATED  DATE                            NOT NULL,
  COEDATAVIEW   CLOB                            NOT NULL,
  DATABASE      VARCHAR2(30 BYTE)               NOT NULL
);

CREATE TABLE COEFORMTYPE
(
  FORMTYPEID    NUMBER(3), 
  FORMTYPE      VARCHAR2(255)
);

CREATE TABLE COEFORM
(
  ID            NUMBER(9)                      NOT NULL,
  NAME          VARCHAR2(255 BYTE),
  DESCRIPTION   VARCHAR2(255 BYTE),
  USER_ID       VARCHAR2(30 BYTE),
  IS_PUBLIC     VARCHAR2(1 BYTE)                NOT NULL,
  FORMGROUP     NUMBER(9),
  DATE_CREATED  DATE                            NOT NULL,
  COEFORM       CLOB                            NOT NULL,
  DATABASE      VARCHAR2(30 BYTE)               NOT NULL,
  FORMTYPEID    NUMBER(3)                       DEFAULT 1,
  APPLICATION   VARCHAR2(255)
);

CREATE TABLE COEGENERICOBJECT
(
  ID                NUMBER(10)                  NOT NULL,
  NAME              VARCHAR2(255 BYTE),
  DESCRIPTION       VARCHAR2(255 BYTE),
  USER_ID           VARCHAR2(30 BYTE),
  IS_PUBLIC         CHAR(1 BYTE)                NOT NULL,
  FORMGROUP         NUMBER(10),
  DATE_CREATED      DATE                        NOT NULL,
  COEGENERICOBJECT  CLOB                        NOT NULL,
  DATABASE          VARCHAR2(30 BYTE)           NOT NULL
);

CREATE TABLE COEGLOBALS
(
  ID	VARCHAR2(100 BYTE)                      NOT NULL,
  VALUE	VARCHAR2(500 BYTE)                      NOT NULL
);


CREATE TABLE COETEMPIDS
(
  INTVAL NUMBER(12)            NOT NULL,
  FLOATVAL NUMBER              NOT NULL,
  TEXTVAL VARCHAR2(2000 BYTE)
);


CREATE TABLE COEOBJECTTYPE
(
  ID               NUMBER(10)                   NOT NULL,
  NAME             VARCHAR2(255 BYTE)           NOT NULL
);


CREATE TABLE COEPRINCIPALTYPE
(
  ID               NUMBER(10)                   NOT NULL,
  NAME             VARCHAR2(255 BYTE)           NOT NULL
);


CREATE TABLE COEPERMISSIONS
(
  OBJECTID	        NUMBER(10)                   NOT NULL,
  OBJECTYPEID       NUMBER(4)                    NOT NULL,
  PRINCIPALID       NUMBER(10)                   NOT NULL,
  PRINCIPALTYPEID   NUMBER(4)                    NOT NULL
);


CREATE TABLE COESESSION
(
  ID               NUMBER(4)                   NOT NULL,
  USER_ID          VARCHAR2(30 BYTE)           NOT NULL,
  STARTTIME        DATE                        NOT NULL,
  ENDTIME          DATE
);


CREATE TABLE COEDATABASE
(
  ID                NUMBER(10)                  NOT NULL,
  NAME              VARCHAR2(255 BYTE),  
  COEDATAVIEW       CLOB                        NOT NULL,
  DATE_CREATED      DATE                        NOT NULL
);


CREATE TABLE COECONFIGURATION 
( 
  Description    VARCHAR2(255) NOT NULL, 
  ClassName        VARCHAR2(255) NOT NULL, 
  ConfigurationXML  XMLTYPE NOT NULL,
  CONSTRAINT PK_COECONFIGURATION PRIMARY KEY (Description) 
);

--create generic schema privileges table

CREATE TABLE  COE_PRIVILEGES (
	ROLE_INTERNAL_ID NUMBER(8,0) not null,
	CSS_LOGIN  NUMBER(1,0) null,
	CAN_BROWSE NUMBER(1,0) null,
	CAN_INSERT NUMBER(1,0) null,
	CAN_UPDATE NUMBER(1,0) null,
	CAN_DELETE NUMBER(1,0) null,
	constraint PK_COE_PRIVS 
		primary key (ROLE_INTERNAL_ID) USING INDEX TABLESPACE &&indexTableSpaceName
	) 
;

--create dataview manager privileges table
CREATE TABLE  COE_DVMANAGER_PRIVILEGES (
	ROLE_INTERNAL_ID NUMBER(8,0) not null,	
	CSS_LOGIN  NUMBER(1,0) null,
	CAN_BROWSE NUMBER(1,0) null,
	CAN_INSERT NUMBER(1,0) null,
	CAN_UPDATE NUMBER(1,0) null,
	CAN_DELETE NUMBER(1,0) null,
	constraint PK_COE_DV_PRIVS 
		primary key (ROLE_INTERNAL_ID) USING INDEX TABLESPACE &&indexTableSpaceName
	) 
;

--create dataview manager privileges table
CREATE TABLE  COE_SECMANAGER_PRIVILEGES (
	ROLE_INTERNAL_ID NUMBER(8,0) not null,	
	CSS_LOGIN  NUMBER(1,0) null,
	CAN_BROWSE NUMBER(1,0) null,
	CAN_INSERT NUMBER(1,0) null,
	CAN_UPDATE NUMBER(1,0) null,
	CAN_DELETE NUMBER(1,0) null,
	constraint PK_COE_SEC_PRIVS 
		primary key (ROLE_INTERNAL_ID) USING INDEX TABLESPACE &&indexTableSpaceName) 
;

	

--CS-Security
CREATE TABLE people
(
   person_id                NUMBER(8,0)                         NOT NULL,
   user_code                VARCHAR2(50)                        NULL,
   user_id                  VARCHAR2(50)                        NULL,
   supervisor_internal_id   NUMBER(8,0)                         NULL,
   title                    VARCHAR2(50)                        NULL,
   first_name               VARCHAR2(50)                        NULL,
   middle_name              VARCHAR2(50)                        NULL,
   last_name                VARCHAR2(50)                        NULL,
   site_id                  NUMBER(8,0)                         NULL,
   department               VARCHAR2(50)                        NULL,
   int_address              VARCHAR2(50)                        NULL,
   telephone                VARCHAR2(50)                        NULL,
   email                    VARCHAR2(50)                        NULL,
   active                   NUMBER(1,0)                         NULL,
   "RID"                    NUMBER(10)                          NOT NULL,
   "CREATOR"                VARCHAR2(30)    DEFAULT RTRIM(USER) NOT NULL,
   "TIMESTAMP"              DATE            DEFAULT SYSDATE     NOT NULL,
   CONSTRAINT people_pk
      PRIMARY KEY (person_id) USING INDEX TABLESPACE &&indexTableSpaceName,
   CONSTRAINT people_user_code_uk
      UNIQUE (user_code) USING INDEX TABLESPACE &&indexTableSpaceName,
   CONSTRAINT people_user_id_uk
      UNIQUE (USER_ID) USING INDEX TABLESPACE &&indexTableSpaceName
);

CREATE TABLE sites
(
   site_id      NUMBER(8,0)     NOT NULL,
   site_code    VARCHAR2(50)    NULL,
   site_name    VARCHAR2(250)   NULL,
   active       NUMBER(1,0)     NULL,
   CONSTRAINT sites_pk PRIMARY KEY (site_id) USING INDEX TABLESPACE &&indexTableSpaceName,
   CONSTRAINT Sites_Site_Code_UK UNIQUE (site_code) USING INDEX TABLESPACE &&indexTableSpaceName
);

CREATE TABLE privilege_tables
(
   privilege_table_id   NUMBER(8,0)     NOT NULL,
   privilege_table_name VARCHAR2(50)    NOT NULL,
   app_name             VARCHAR2(50)    NOT NULL,
   app_url              VARCHAR2(250)   NULL,
   table_space          VARCHAR2(100)   NOT NULL,
   CONSTRAINT privilege_table_pk
      PRIMARY KEY (privilege_table_id) USING INDEX TABLESPACE &&indexTableSpaceName,
   CONSTRAINT app_name_u
      UNIQUE(app_name)
);

CREATE TABLE security_roles
(
   role_id                  NUMBER(8,0)                         NOT NULL,
   privilege_table_int_id   NUMBER(8,0)                         NULL,
   role_name                VARCHAR2(120)                       NULL,
   "RID"                    NUMBER(10)                          NOT NULL,
   "CREATOR"                VARCHAR2(30)    DEFAULT RTRIM(USER) NOT NULL,
   "TIMESTAMP"              DATE            DEFAULT SYSDATE     NOT NULL,
   COEIDENTIFIER 			VARCHAR2(100),
   CONSTRAINT security_roles_pk
      PRIMARY KEY (role_id) USING INDEX TABLESPACE &&indexTableSpaceName,
   CONSTRAINT securityroles_privilegetables
      FOREIGN KEY (privilege_table_int_id)
      REFERENCES privilege_tables (privilege_table_id)
);

CREATE TABLE COEPAGECONTROL
(
  ID                INTEGER,
  APPLICATION       VARCHAR2(20 BYTE),
  TYPE              VARCHAR2(20 BYTE),
  CONFIGURATIONXML  CLOB
);


--########################
--SEQUENCES and TRIGGERS
--########################

CREATE SEQUENCE COEDATAVIEW_SEQ       INCREMENT BY 1 START WITH 100000 MAXVALUE 1.0E27 MINVALUE 1 NOCYCLE CACHE 20 NOORDER;
CREATE SEQUENCE COEFORM_SEQ           INCREMENT BY 1 START WITH 1 MAXVALUE 1.0E27 MINVALUE 1 NOCYCLE CACHE 20 NOORDER;
CREATE SEQUENCE COEGLOBALS_SEQ        INCREMENT BY 1 START WITH 1 MAXVALUE 1.0E27 MINVALUE 1 NOCYCLE CACHE 20 NOORDER;
CREATE SEQUENCE COEHITLISTID_SEQ      INCREMENT BY 1 START WITH 1 MAXVALUE 1.0E27 MINVALUE 1 NOCYCLE CACHE 20 NOORDER;
CREATE SEQUENCE COESEARCHCRITERIA_SEQ INCREMENT BY 1 START WITH 1 MAXVALUE 1.0E27 MINVALUE 1 NOCYCLE CACHE 20 NOORDER;
CREATE SEQUENCE COEGENERICOBJECT_SEQ  INCREMENT BY 1 START WITH 1 MAXVALUE 1.0E27 MINVALUE 1 NOCYCLE CACHE 20 NOORDER;
CREATE SEQUENCE COESESSION_SEQ        INCREMENT BY 1 START WITH 1 MAXVALUE 1.0E27 MINVALUE 1 NOCYCLE CACHE 20 NOORDER;
CREATE SEQUENCE COEDATABASE_SEQ       INCREMENT BY 1 START WITH 1 MAXVALUE 1.0E27 MINVALUE 1 NOCYCLE CACHE 20 NOORDER;
CREATE SEQUENCE COEPAGECONTROL_SEQ    INCREMENT BY 1 START WITH 1 MAXVALUE 1.0E27 MINVALUE 1 NOCYCLE CACHE 20 NOORDER;


--CS-Security
CREATE SEQUENCE SITES_SEQ 			  INCREMENT BY 1 START WITH 1;
CREATE SEQUENCE PEOPLE_SEQ 			  INCREMENT BY 1 START WITH 1;
CREATE SEQUENCE SECURITY_ROLES_SEQ 	  INCREMENT BY 1 START WITH 1;
CREATE SEQUENCE PRIVILEGE_TABLES_SEQ  INCREMENT BY 1 START WITH 1;

@@sql\triggers\coedb_sites.trg;
@@sql\triggers\coedb_people.trg;
@@sql\triggers\coedb_security_roles.trg;
@@sql\triggers\coedb_privilege_tables.trg;
@@sql\triggers\coedb_coepagecontrol.trg;

--#########################################################
--INDEXES
--#########################################################

CREATE INDEX INDEX_COEDATAVIEWA_USER_ID 
	ON COEDATAVIEW (USER_ID ASC)  TABLESPACE &&indexTableSpaceName;

CREATE INDEX INDEX_COEDATAVIEW_IS_PUBLIC
	ON COEDATAVIEW (IS_PUBLIC ASC)  TABLESPACE &&indexTableSpaceName;

CREATE INDEX INDEX_COEDATAVIEW_FORMGROUP
	ON COEDATAVIEW (FORMGROUP ASC)  TABLESPACE &&indexTableSpaceName;

CREATE INDEX INDEX_COEDATAVIEW_DATE_CREATED
	ON COEDATAVIEW (DATE_CREATED ASC)  TABLESPACE &&indexTableSpaceName;

CREATE INDEX INDEX_COEDATAVIEW_DATABASE
	ON COEDATAVIEW (DATABASE ASC)  TABLESPACE &&indexTableSpaceName;

CREATE INDEX INDEX_COEFORM_USER_ID
	ON COEFORM (USER_ID ASC)  TABLESPACE &&indexTableSpaceName;

CREATE INDEX INDEX_COEFORM_IS_PUBLIC
	ON COEFORM (IS_PUBLIC ASC)  TABLESPACE &&indexTableSpaceName;

CREATE INDEX INDEX_COEFORM_FORMGROUP
	ON COEFORM (FORMGROUP ASC)  TABLESPACE &&indexTableSpaceName;

CREATE INDEX INDEX_COEFORM_DATE_CREATED
	ON COEFORM (DATE_CREATED ASC)  TABLESPACE &&indexTableSpaceName;

CREATE INDEX INDEX_COEFORM_DATABASE
	ON COEFORM (DATABASE ASC)  TABLESPACE &&indexTableSpaceName;

CREATE INDEX INDEX_COEGENEROBJ_USER_ID
	ON COEGENERICOBJECT (USER_ID ASC)  TABLESPACE &&indexTableSpaceName;

CREATE INDEX INDEX_COEGENEROBJ_IS_PUBLIC
	ON COEGENERICOBJECT (IS_PUBLIC ASC)  TABLESPACE &&indexTableSpaceName;

CREATE INDEX INDEX_COEGENEROBJ_FORMGROUP
	ON COEGENERICOBJECT (FORMGROUP ASC)  TABLESPACE &&indexTableSpaceName;

CREATE INDEX INDEX_COEGENEROBJ_DATE_CREATED
	ON COEGENERICOBJECT (DATE_CREATED ASC)  TABLESPACE &&indexTableSpaceName;

CREATE INDEX INDEX_COEGENEROBJ_DATABASE
	ON COEGENERICOBJECT (DATABASE ASC)  TABLESPACE &&indexTableSpaceName;

CREATE INDEX INDEX_COEPERMISSIONS_OBJTYPEID
	ON COEPERMISSIONS(OBJECTID ASC)  TABLESPACE &&indexTableSpaceName;

CREATE INDEX INDEX_COEPERMISSIONS_PRINTYPID
	ON COEPERMISSIONS(PRINCIPALID ASC)  TABLESPACE &&indexTableSpaceName;

CREATE INDEX INDEX_COEOBJECTTYPE_NAME
	ON COEOBJECTTYPE(NAME ASC)  TABLESPACE &&indexTableSpaceName;

CREATE INDEX INDEX_COEPRINCIPALTYPE_NAME
	ON COEPRINCIPALTYPE(NAME ASC)  TABLESPACE &&indexTableSpaceName;

--CS-Security
CREATE INDEX privilege_table_int_id
   ON security_roles (privilege_table_int_id ASC) TABLESPACE &&indexTableSpaceName;

CREATE INDEX last_name
	ON people (last_name ASC) TABLESPACE &&indexTableSpaceName;

CREATE INDEX supervisor_internal_id
   ON people (supervisor_internal_id ASC) TABLESPACE &&indexTableSpaceName;


--#########################################################
--CONSTRAINTS
--#########################################################

ALTER TABLE COEDATAVIEW ADD (
 CONSTRAINT PK_COEDATAVIEW PRIMARY KEY (ID) USING INDEX TABLESPACE &&indexTableSpaceName);

ALTER TABLE COEFORM ADD (
 CONSTRAINT PK_CHEMINVDB2_COEFORM PRIMARY KEY (ID) USING INDEX TABLESPACE &&indexTableSpaceName);

ALTER TABLE COEGENERICOBJECT ADD (
 CONSTRAINT PK_CHEMINVDB2_COEGENERICOBJE PRIMARY KEY (ID) USING INDEX TABLESPACE &&indexTableSpaceName);

ALTER TABLE COEGLOBALS ADD (
 CONSTRAINT PK_CHEMINVDB2_CCOEGLOBALS PRIMARY KEY (ID) USING INDEX TABLESPACE &&indexTableSpaceName);

ALTER TABLE COEPRINCIPALTYPE ADD (
 CONSTRAINT PK_COEPRINCIPALTYPE PRIMARY KEY (ID) USING INDEX TABLESPACE &&indexTableSpaceName);

ALTER TABLE COESESSION ADD (
 CONSTRAINT PK_COESESSION PRIMARY KEY (ID) USING INDEX TABLESPACE &&indexTableSpaceName);

ALTER TABLE COEOBJECTTYPE ADD (
 CONSTRAINT PK_COEOBJECTTYPE PRIMARY KEY (ID) USING INDEX TABLESPACE &&indexTableSpaceName);

ALTER TABLE COEPERMISSIONS ADD
 CONSTRAINT FK_COEPERMISSIONS_COEOBJECTTYP FOREIGN KEY(OBJECTYPEID) REFERENCES COEOBJECTTYPE(ID);

ALTER TABLE COEPERMISSIONS ADD
 CONSTRAINT FK_COEPERMISSIONS_COEPRINCIPTA FOREIGN KEY(PRINCIPALTYPEID) REFERENCES COEPRINCIPALTYPE(ID);

ALTER TABLE PEOPLE ADD
 CONSTRAINT FK_PEOPLE_SITES FOREIGN KEY(SITE_ID) REFERENCES Sites(SITE_ID);

ALTER TABLE COEFORMTYPE ADD (
 CONSTRAINT PK_COEFORMTYPE PRIMARY KEY (FormTypeID) USING INDEX TABLESPACE &&indexTableSpaceName);

ALTER TABLE COEFORM ADD
 CONSTRAINT FK_COEFORM_COEFORMTYPE FOREIGN KEY(FormTypeID) REFERENCES COEFORMTYPE(FormTypeID);


--#########################################################
--CREATE OBJECT TYPES
--#########################################################
CREATE OR REPLACE TYPE MYTABLETYPE IS TABLE OF VARCHAR2(1024)
/
CREATE OR REPLACE TYPE obj_privilege AS OBJECT(priv_scope VARCHAR2(100), priv_name VARCHAR2(100), priv_value NUMBER)
/
CREATE OR REPLACE TYPE obj_privilege_table IS TABLE OF obj_privilege
/


--#########################################################
--Create Audit Tables
--#########################################################

@@"sql\Patches\Patch 11.0.1.0\sql\CsSecurity\CREATE_Audit_Tables.sql"


--#########################################################
--CREATE PROCEDURES AND FUNCTIONS
--######################################################### 

@Sql\Procedures.sql
SHO ERR
@Sql\Functions.sql
SHO ERR


--#########################################################
--CREATE SERVICE TABLES
--Create Search table, HitList table, and Partitioning HitList table
--#########################################################

EXEC CreateServiceTables(AServiceName=>'ALL', ASchemaName=>'&&schemaName', AIndexTableSpaceName=>'&&indexTableSpaceName');


--#########################################################
--CREATE PACKAGES
--#########################################################

@Sql\packages\PKG_CoeDbLibrary.SQL
@Sql\packages\pkg_ConfigurationManager.sql
SHO ERR
PROMPT
