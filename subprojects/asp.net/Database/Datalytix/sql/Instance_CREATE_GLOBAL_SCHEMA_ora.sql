-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.

--Creation script for Instance_CREATE_GLOBAL_SCHEMA_ora.sql

--#########################################################
--CREATE Tables
--#########################################################

CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA

DECLARE
   n NUMBER;
   isOracleENTEdition NUMBER := 0;
BEGIN

	select count(*) into isOracleENTEdition from v$version where lower(banner) like '%enterprise%';
   
	SELECT COUNT(1) INTO n FROM dba_tables where owner=UPPER('&&globalSchemaName') and table_name = Upper('TEMPCHITLISTID');
	
	IF n < 1 THEN
		IF isOracleENTEdition = 1 THEN
			EXECUTE IMMEDIATE 'CREATE TABLE ' || UPPER ('&&globalSchemaName') || '.TEMPCHITLISTID(
				CHILDHITLISTID NUMBER(9,0) NOT NULL ENABLE, 
				HITLISTID      NUMBER(9,0) NOT NULL ENABLE, 
				TABLEID        NUMBER(9,0) NOT NULL ENABLE, 
				DATESTAMP      DATE DEFAULT SYSDATE NOT NULL ENABLE,
				CONSTRAINT TEMPCHITLISTID_PK PRIMARY KEY (CHILDHITLISTID) USING INDEX TABLESPACE ' || UPPER('&&indexTableSpaceName') || ')			
				PARTITION BY RANGE (CHILDHITLISTID) (PARTITION TEMPCHITLISTIDMAXVALUE VALUES LESS THAN(MAXVALUE))';
		ELSE
			EXECUTE IMMEDIATE 'CREATE TABLE ' || UPPER ('&&globalSchemaName') || '.TEMPCHITLISTID(
				CHILDHITLISTID NUMBER(9,0) NOT NULL ENABLE, 
				HITLISTID      NUMBER(9,0) NOT NULL ENABLE, 
				TABLEID        NUMBER(9,0) NOT NULL ENABLE, 
				DATESTAMP      DATE DEFAULT SYSDATE NOT NULL ENABLE,
				CONSTRAINT TEMPCHITLISTID_PK PRIMARY KEY (CHILDHITLISTID) USING INDEX TABLESPACE ' || UPPER('&&indexTableSpaceName') || ')';			
		END IF;
		EXECUTE IMMEDIATE 'CREATE INDEX ' || UPPER ('&&globalSchemaName') || '.IDX_TEMPCHITLISTID_HITLISTID ON '  || UPPER ('&&globalSchemaName') || '.TEMPCHITLISTID (HITLISTID ASC)  TABLESPACE ' || UPPER('&&indexTableSpaceName');
		EXECUTE IMMEDIATE 'CREATE INDEX ' || UPPER ('&&globalSchemaName') || '.IDX_TEMPCHITLISTID_TABLEID ON '    || UPPER ('&&globalSchemaName') || '.TEMPCHITLISTID (TABLEID ASC)  TABLESPACE '   || UPPER('&&indexTableSpaceName');
		EXECUTE IMMEDIATE 'CREATE INDEX ' || UPPER ('&&globalSchemaName') || '.IDX_TEMPCHITLISTID_DATESTAMP ON '  || UPPER ('&&globalSchemaName') || '.TEMPCHITLISTID (DATESTAMP ASC)  TABLESPACE ' || UPPER('&&indexTableSpaceName');
	END IF;

	SELECT COUNT(1) INTO n FROM dba_tables where owner=UPPER('&&globalSchemaName') and table_name = Upper('TEMPCHITLIST');
	
	IF n < 1 THEN
		IF isOracleENTEdition = 1 THEN
			EXECUTE IMMEDIATE 'CREATE TABLE ' || UPPER ('&&globalSchemaName') || '.TEMPCHITLIST(
				CHILDHITLISTID  NUMBER(9,0) NOT NULL ENABLE,
				ID          NUMBER(9,0),
				VAR_ID      VARCHAR2(255 BYTE),
				ROW_ID      ROWID,
				SORTORDER   NUMBER(9,0))
				PARTITION BY RANGE (CHILDHITLISTID) (PARTITION TEMPCHITLISTMAXVALUE VALUES LESS THAN(MAXVALUE))';
		ELSE
			EXECUTE IMMEDIATE 'CREATE TABLE ' || UPPER ('&&globalSchemaName') || '.TEMPCHITLIST(
				CHILDHITLISTID  NUMBER(9,0) NOT NULL ENABLE,
				ID          NUMBER(9,0),
				VAR_ID      VARCHAR2(255 BYTE),
				ROW_ID      ROWID,
				SORTORDER   NUMBER(9,0))';
		END IF;
		EXECUTE IMMEDIATE 'CREATE INDEX ' || UPPER ('&&globalSchemaName') || '.IDX_TEMPCHITLIST_CHITLISTID ON '  || UPPER ('&&globalSchemaName') || '.TEMPCHITLIST (CHILDHITLISTID ASC)  TABLESPACE ' || UPPER('&&indexTableSpaceName');
		EXECUTE IMMEDIATE 'CREATE INDEX ' || UPPER ('&&globalSchemaName') || '.IDX_TEMPCHITLIST_ID ON '    || UPPER ('&&globalSchemaName') || '.TEMPCHITLIST (ID ASC)  TABLESPACE '   || UPPER('&&indexTableSpaceName');
		EXECUTE IMMEDIATE 'CREATE INDEX ' || UPPER ('&&globalSchemaName') || '.IDX_TEMPCHITLIST_VAR_ID ON '  || UPPER ('&&globalSchemaName') || '.TEMPCHITLIST (VAR_ID ASC)  TABLESPACE ' || UPPER('&&indexTableSpaceName');
		EXECUTE IMMEDIATE 'CREATE INDEX ' || UPPER ('&&globalSchemaName') || '.IDX_TEMPCHITLIST_ROW_ID ON '  || UPPER ('&&globalSchemaName') || '.TEMPCHITLIST (ROW_ID ASC)  TABLESPACE ' || UPPER('&&indexTableSpaceName');
		EXECUTE IMMEDIATE 'CREATE INDEX ' || UPPER ('&&globalSchemaName') || '.IDX_TEMPCHITLIST_SORTORDER ON '  || UPPER ('&&globalSchemaName') || '.TEMPCHITLIST (SORTORDER ASC)  TABLESPACE ' || UPPER('&&indexTableSpaceName');
	END IF;

--########################
--SEQUENCES
--########################

	SELECT COUNT (1) INTO n
    FROM dba_sequences
	WHERE UPPER (sequence_name) = 'COECHILDHITLISTID_SEQ'
	AND UPPER (sequence_owner) = UPPER('&&globalSchemaName');
	
	IF n < 1 THEN
		EXECUTE IMMEDIATE 'CREATE SEQUENCE ' || UPPER ('&&globalSchemaName') || '.COECHILDHITLISTID_SEQ MINVALUE 1 MAXVALUE 1000000000000000000000000000 INCREMENT BY 1 START WITH 1 CACHE 20 NOORDER NOCYCLE';
	END IF;

COMMIT;
   
END;
/