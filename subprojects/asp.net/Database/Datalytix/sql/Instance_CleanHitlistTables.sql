-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.

spool ON
spool log_cleanHitlistTables.txt

--#########################################################
--CLEAN HITLIST TABLES
--#########################################################

PROMPT Starting Instance_CleanHitlistTables.sql...

@@Instance_parameters.sql
@@Instance_prompts.sql

-- Connect and grant privillege
CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA
GRANT CREATE TABLE TO &&globalSchemaName;

CONNECT &&globalSchemaName/&&globalSchemaPass@&&serverName

ACCEPT recordsNumTokeep NUMBER DEFAULT 10000 PROMPT 'Enter the record number to keep (10000):'

SET serveroutput on

DECLARE
   p NUMBER;
   r NUMBER;
   i NUMBER;
   t NUMBER;   
BEGIN	
	-- Check if Oracle Partioning feature is enabled
	SELECT COUNT(1) INTO p FROM v$option where UPPER(parameter) = 'PARTITIONING' and UPPER(value) = 'TRUE';
	
	-- Partition is disabled
	IF p < 1 THEN
	
		SELECT COUNT(*) INTO r FROM TEMPCHITLISTID;
		
		-- To many rows, need to thrink
		IF r > &&recordsNumTokeep THEN
		
			-- It temp table is already existed, drop it
			SELECT COUNT(1) INTO i FROM all_tables WHERE UPPER(owner)=UPPER('&&globalSchemaName') AND UPPER(table_name) = 'TEMP1';			
			IF i > 0 THEN
					EXECUTE IMMEDIATE 'DROP TABLE ' || UPPER ('&&globalSchemaName') || '.TEMP1';
					DBMS_OUTPUT.put_line('The existed TEMP1 table is dropped');
			END IF;
			
			-- Create new temp table
			EXECUTE IMMEDIATE 'CREATE TABLE TEMP1(
				CHILDHITLISTID NUMBER(9,0) NOT NULL ENABLE PRIMARY KEY, 
				HITLISTID      NUMBER(9,0) NOT NULL ENABLE, 
				TABLEID        NUMBER(9,0) NOT NULL ENABLE, 
				DATESTAMP      DATE DEFAULT SYSDATE NOT NULL ENABLE)';
			EXECUTE IMMEDIATE 'CREATE INDEX ' || UPPER ('&&globalSchemaName') || '.IDX_TEMP1_HITLISTID ON '  || UPPER ('&&globalSchemaName') || '.TEMP1 (HITLISTID ASC)  TABLESPACE ' || UPPER('&&indexTableSpaceName');
			EXECUTE IMMEDIATE 'CREATE INDEX ' || UPPER ('&&globalSchemaName') || '.IDX_TEMP1_TABLEID ON '    || UPPER ('&&globalSchemaName') || '.TEMP1 (TABLEID ASC)  TABLESPACE '   || UPPER('&&indexTableSpaceName');
			EXECUTE IMMEDIATE 'CREATE INDEX ' || UPPER ('&&globalSchemaName') || '.IDX_TEMP1_DATESTAMP ON '  || UPPER ('&&globalSchemaName') || '.TEMP1 (DATESTAMP ASC)  TABLESPACE ' || UPPER('&&indexTableSpaceName');
			DBMS_OUTPUT.put_line('TEMP1 table is created');
			
			-- COPY the latest 10000 records
			EXECUTE IMMEDIATE 'INSERT INTO ' || UPPER ('&&globalSchemaName') || '.TEMP1 SELECT * FROM (SELECT * FROM '|| UPPER ('&&globalSchemaName') || '.TEMPCHITLISTID ORDER BY HITLISTID DESC) WHERE ROWNUM <= ' || &&recordsNumTokeep;
			DBMS_OUTPUT.put_line(sql%rowcount || ' rows are copied from TEMPCHITLISTID to TEMP1');
			
			-- It temp table is already existed, drop it
			SELECT COUNT(1) INTO t FROM all_tables where UPPER(owner)=UPPER('&&globalSchemaName') and UPPER(table_name) = 'TEMP2';
			IF t > 0 THEN
				EXECUTE IMMEDIATE 'DROP TABLE ' || UPPER('&&globalSchemaName') || '.TEMP2';
				DBMS_OUTPUT.put_line('The existed TEMP2 table is dropped');
			END IF;
			
			-- Create new temp table
			EXECUTE IMMEDIATE 'CREATE TABLE ' || UPPER('&&globalSchemaName') || '.TEMP2(
				CHILDHITLISTID  NUMBER(9,0) NOT NULL ENABLE,
				ID          NUMBER(9,0),
				VAR_ID      VARCHAR2(255 BYTE),
				ROW_ID      ROWID,
				SORTORDER   NUMBER(9,0))';
			EXECUTE IMMEDIATE 'CREATE INDEX ' || UPPER ('&&globalSchemaName') || '.IDX_TEMP2_CHITLISTID ON '  || UPPER ('&&globalSchemaName') || '.TEMP2 (CHILDHITLISTID ASC)  TABLESPACE ' || UPPER('&&indexTableSpaceName');
			EXECUTE IMMEDIATE 'CREATE INDEX ' || UPPER ('&&globalSchemaName') || '.IDX_TEMP2_ID ON '    || UPPER ('&&globalSchemaName') || '.TEMP2 (ID ASC)  TABLESPACE '   || UPPER('&&indexTableSpaceName');
			EXECUTE IMMEDIATE 'CREATE INDEX ' || UPPER ('&&globalSchemaName') || '.IDX_TEMP2_VAR_ID ON '  || UPPER ('&&globalSchemaName') || '.TEMP2 (VAR_ID ASC)  TABLESPACE ' || UPPER('&&indexTableSpaceName');
			EXECUTE IMMEDIATE 'CREATE INDEX ' || UPPER ('&&globalSchemaName') || '.IDX_TEMP2_ROW_ID ON '  || UPPER ('&&globalSchemaName') || '.TEMP2 (ROW_ID ASC)  TABLESPACE ' || UPPER('&&indexTableSpaceName');
			EXECUTE IMMEDIATE 'CREATE INDEX ' || UPPER ('&&globalSchemaName') || '.IDX_TEMP2_SORTORDER ON '  || UPPER ('&&globalSchemaName') || '.TEMP2 (SORTORDER ASC)  TABLESPACE ' || UPPER('&&indexTableSpaceName');
			DBMS_OUTPUT.put_line('TEMP2 table is created');
			
			-- copy the latest hit records into temp table			
			EXECUTE IMMEDIATE 'INSERT INTO ' || UPPER ('&&globalSchemaName') || '.TEMP2 SELECT * FROM '|| UPPER ('&&globalSchemaName') || '.TEMPCHITLIST WHERE CHILDHITLISTID >= (SELECT MIN(CHILDHITLISTID) FROM TEMP1)';
			DBMS_OUTPUT.put_line(sql%rowcount || ' rows are copied from TEMPCHITLIST to TEMP2');
			
			-- remove old hitlist tables
			EXECUTE IMMEDIATE 'DROP TABLE ' || UPPER ('&&globalSchemaName') || '.TEMPCHITLISTID';
			DBMS_OUTPUT.put_line('TEMPCHITLISTID table is dropped');
			EXECUTE IMMEDIATE 'DROP TABLE ' || UPPER ('&&globalSchemaName') || '.TEMPCHITLIST';
			DBMS_OUTPUT.put_line('TEMPCHITLISTID table is TEMPCHITLIST');
			
			-- rename new hitlist tables
			EXECUTE IMMEDIATE 'ALTER TABLE ' || UPPER ('&&globalSchemaName') || '.TEMP1 RENAME TO TEMPCHITLISTID';
			DBMS_OUTPUT.put_line('Table TEMP1 is renamed to TEMPCHITLISTID');
			EXECUTE IMMEDIATE 'ALTER TABLE ' || UPPER ('&&globalSchemaName') || '.TEMP2 RENAME TO TEMPCHITLIST';
			DBMS_OUTPUT.put_line('Table TEMP2 is renamed to TEMPCHITLIST');
			
			-- rename index			
			EXECUTE IMMEDIATE 'ALTER INDEX IDX_TEMP1_HITLISTID RENAME TO IDX_TEMPCHITLISTID_HITLISTID';
			EXECUTE IMMEDIATE 'ALTER INDEX IDX_TEMP1_TABLEID RENAME TO IDX_TEMPCHITLISTID_TABLEID';
			EXECUTE IMMEDIATE 'ALTER INDEX IDX_TEMP1_DATESTAMP RENAME TO IDX_TEMPCHITLISTID_DATESTAMP';			
			EXECUTE IMMEDIATE 'ALTER INDEX IDX_TEMP2_CHITLISTID RENAME TO IDX_TEMPCHITLIST_CHITLISTID';
			EXECUTE IMMEDIATE 'ALTER INDEX IDX_TEMP2_ID RENAME TO IDX_TEMPCHITLIST_ID';
			EXECUTE IMMEDIATE 'ALTER INDEX IDX_TEMP2_VAR_ID RENAME TO IDX_TEMPCHITLIST_VAR_ID';
			EXECUTE IMMEDIATE 'ALTER INDEX IDX_TEMP2_ROW_ID RENAME TO IDX_TEMPCHITLIST_ROW_ID';
			EXECUTE IMMEDIATE 'ALTER INDEX IDX_TEMP2_SORTORDER RENAME TO IDX_TEMPCHITLIST_SORTORDER';
			DBMS_OUTPUT.put_line('All related Indexes on TEMP table are renamed');
			
			DBMS_OUTPUT.put_line('Hitlist history is cleaned. Hitlist for the latest' || &&recordsNumTokeep || ' queries is kept');

		ELSE
			DBMS_OUTPUT.put_line('TEMPCHITLISTID table only has '|| r ||' rows, it is less than the rows to be kept ('|| &&recordsNumTokeep ||')');
			DBMS_OUTPUT.put_line('No hitlist history is deleted');
		END IF;

	ELSE
		DBMS_OUTPUT.put_line('Partition feature is enabled, the hitlist tables should be cleanup by schedule task automaticlly.');
	END IF;
	
END;
/

prompt logged session to: log_cleanHitlistTables.txt
spool off

-- Connect and revoke privillege
CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA
REVOKE CREATE TABLE FROM &&globalSchemaName;

EXIT
