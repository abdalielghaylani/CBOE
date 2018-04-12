--Table creation script for Document Manager
--Oracle version 
--Copyright Cambridgesoft Corp. 1999-2004 all rights reserved

--#########################################################
--CREATE TABLES
--#########################################################


@@globals.sql

CREATE TABLE DOCMGR_DOCUMENTS (
	RID NUMBER(10) NOT NULL, 
	DOCID NUMBER(8,0) not null,
	DOC BLOB default EMPTY_BLOB(),
	DOCLOCATION VARCHAR2(500) null,
	DOCNAME VARCHAR2(100) null,
	DOCSIZE NUMBER(10, 0) null, 
	DOCTYPE VARCHAR2(10) null,
	TITLE VARCHAR2(1000) null, 
	AUTHOR VARCHAR2(100) null, 
	SUBMITTER VARCHAR2(100) null, 
	SUBMITTER_COMMENTS VARCHAR2(1000) null, 
	DATE_SUBMITTED DATE null,
	REG_RLS_PROJECT_ID NUMBER(8, 0) null,
	constraint CONSTRAINT_DOCUMENTS_DOCID primary key (DOCID) USING INDEX TABLESPACE &&indexTableSpaceName 
	)
	LOB (DOC) STORE AS(
		DISABLE STORAGE IN ROW NOCACHE PCTVERSION 20
		TABLESPACE &&docTableSpaceName
		STORAGE (INITIAL &&lobDoc NEXT &&lobDoc)
	)	
;

CREATE TABLE DOCMGR_STRUCTURES (
	U_ID			NUMBER(8, 0) not null,
	MOL_ID			NUMBER(8, 0) null,
	DOCID			NUMBER(8, 0) not null,
	BASE64_CDX		BLOB default EMPTY_BLOB(),
	constraint CONSTRAINT_STRUCTURES_UID primary key (U_ID) USING INDEX TABLESPACE &&indexTableSpaceName
	)
	LOB (BASE64_CDX) STORE AS(
		DISABLE STORAGE IN ROW NOCACHE PCTVERSION 10
		TABLESPACE &&lobsTableSpaceName
		STORAGE (INITIAL &&lobB64cdx NEXT &&lobB64cdx)
	)	
;
	
--Added for new external links functionality
CREATE TABLE DOCMGR_EXTERNAL_LINKS (
	RID NUMBER(10) NOT NULL, 
	APPNAME VARCHAR2(100) not null, 
	LINKTYPE VARCHAR2(100) not null,
	LINKID VARCHAR2(100) not null,
	DOCID NUMBER(8,0) not null,
	LINKFIELDNAME VARCHAR2(100) not null,
	SUBMITTER VARCHAR2(100) null, 
	DATE_SUBMITTED DATE null
	)
;	

--#########################################################
--CREATE SEQUENCES, INDICES, SYNONYMS
--#########################################################
CREATE SEQUENCE SEQ_DOCMGR_DOCUMENTS INCREMENT BY 1 START WITH 1;
CREATE SEQUENCE SEQ_DOCMGR_STRUCTURES INCREMENT BY 1 START WITH 1;


--#########################################################
--SET INDEXING MEMORY TO A BIGGER NUMBER
--#########################################################
CONNECT ctxsys/&&ctxPass@&&serverName;

grant execute on ctx_ddl to &&schemaName;

BEGIN
	CTX_ADM.SET_PARAMETER('MAX_INDEX_MEMORY', '100M');
END;
/

BEGIN
	CTX_ADM.SET_PARAMETER('DEFAULT_INDEX_MEMORY', '100M');
END;
/




Connect &&schemaName/&&schemaPass@&&serverName


--#########################################################
--CREATE CTX INDEX
--#########################################################

-- DGB create explicit storage for ctx index and point to ctx tablespace
-- storage object for specifying intermedia index parameters
--
begin
ctx_ddl.create_preference('docmgr_ctx_store', 'BASIC_STORAGE');
ctx_ddl.set_attribute('docmgr_ctx_store', 'I_TABLE_CLAUSE', 'tablespace &&ctxTableSpaceName storage (initial 1K)');
ctx_ddl.set_attribute('docmgr_ctx_store', 'K_TABLE_CLAUSE', 'tablespace &&ctxTableSpaceName storage (initial 1K)');
ctx_ddl.set_attribute('docmgr_ctx_store', 'R_TABLE_CLAUSE', 'tablespace &&ctxTableSpaceName storage (initial 1K)');
ctx_ddl.set_attribute('docmgr_ctx_store', 'N_TABLE_CLAUSE', 'tablespace &&ctxTableSpaceName storage (initial 1K)');
ctx_ddl.set_attribute('docmgr_ctx_store', 'I_INDEX_CLAUSE', 'tablespace &&ctxTableSpaceName storage (initial 1K)');
ctx_ddl.set_attribute('docmgr_ctx_store', 'P_TABLE_CLAUSE', 'tablespace &&ctxTableSpaceName storage (initial 1K)');
end;
/

-- SYAN added to not break words by dash
BEGIN
	CTX_DDL.CREATE_PREFERENCE('MY_LEXER', 'BASIC_LEXER');
	CTX_DDL.SET_ATTRIBUTE('MY_LEXER', 'SKIPJOINS', '-');
END;
/

CREATE INDEX DOCMGR.INDEX_DOCMGR_DOCUMENTS ON DOCMGR_DOCUMENTS(DOC)
indextype IS ctxsys.context PARAMETERS ('filter ctxsys.inso_filter storage docmgr_ctx_store lexer my_lexer' );

EXEC CTX_SCHEDULE_DOCMANAGER.STARTUP ('INDEX_DOCMGR_DOCUMENTS', 'SYNC', 1) ;
EXEC CTX_SCHEDULE_DOCMANAGER.STARTUP ('INDEX_DOCMGR_DOCUMENTS', 'OPTIMIZE FAST', 60);


--#########################################################
--CREATE STORED PROCEDURES
--#########################################################


CREATE OR REPLACE PROCEDURE DOCMGR.INSERTDOC(u_id OUT INTEGER, fileLocation IN VARCHAR2, fileName IN VARCHAR2, fileSize IN INTEGER, fileType IN VARCHAR2, title IN VARCHAR2, author IN VARCHAR2, submitter IN VARCHAR2, submitter_comments IN VARCHAR2, projectID IN INTEGER default null) 
AS
BEGIN
	SELECT DOCMGR.SEQ_DOCMGR_DOCUMENTS.NEXTVAL INTO u_id FROM DUAL;
	INSERT INTO DOCMGR.DOCMGR_DOCUMENTS (DOCID, DOC, DOCLOCATION, DOCNAME, DOCSIZE, DOCTYPE, TITLE, AUTHOR, SUBMITTER, SUBMITTER_COMMENTS, DATE_SUBMITTED, REG_RLS_PROJECT_ID) 
	VALUES (u_id, EMPTY_BLOB(), fileLocation, fileName, fileSize, fileType, title, author, submitter, submitter_comments, SYSDATE, projectID);
	COMMIT;
END;
/

CREATE OR REPLACE PROCEDURE BLOB_IN (doc_pId IN INTEGER, 
						amount IN INTEGER, 
						doc_buffer IN LONG RAW) 
IS
	doc_blob BLOB;
BEGIN
	--FETCH THE LOB LOCATOR
	SELECT DOC INTO doc_blob 
	FROM DOCMGR.DOCMGR_DOCUMENTS 
	WHERE DOCID = doc_pId
	FOR UPDATE;
			
	DBMS_LOB.WRITEAPPEND(doc_blob, amount, doc_buffer);
END;
/

CREATE OR REPLACE PROCEDURE INSERT_STRUCTURE (u_id OUT INTEGER,
												doc_pId IN INTEGER,
												molid IN INTEGER) 
IS
BEGIN
	--INSERT A RECORD
	SELECT DOCMGR.SEQ_DOCMGR_STRUCTURES.NEXTVAL INTO u_id FROM DUAL;
	INSERT INTO DOCMGR.DOCMGR_STRUCTURES (U_ID, DOCID, MOL_ID) 
	VALUES (u_id, doc_pId, molid);
	COMMIT;
END;
/


CREATE OR REPLACE PROCEDURE CDX_BLOB_IN (uid IN INTEGER,
						amount IN INTEGER, 
						cdx_buffer IN LONG RAW) 
IS
	cdx_blob BLOB;
BEGIN

	--FETCH THE LOB LOCATOR
	SELECT BASE64_CDX INTO cdx_blob 
	FROM DOCMGR.DOCMGR_STRUCTURES
	WHERE U_ID = uid
	FOR UPDATE;
			
	DBMS_LOB.WRITEAPPEND(cdx_blob, amount, cdx_buffer);
END;
/

CREATE OR REPLACE FUNCTION INSERTDASH(INSTR VARCHAR2, PREFIX VARCHAR2) RETURN VARCHAR2
IS
  OUTSTR VARCHAR2(100);
BEGIN
  OUTSTR := SUBSTR(INSTR, 1, LENGTH(PREFIX)) || '-' || SUBSTR(INSTR, LENGTH(PREFIX) + 1);
  RETURN OUTSTR;
END;
/

-- Parse DR$INDEX_DOCMGR_DOCUMENTS$I to get token-docid association
prompt The following line may throw an error if either 
prompt 1) Chemical Registration is not installed or  
prompt 2) You did not properly set the password for
prompt Registration at the top of parameters.sql.
prompt You are also likley to see some compilation errors
prompt if the step fails.
prompt    

CONNECT &&regName/&&regPass@&&serverName;
GRANT SELECT, DELETE ON REGDB.SEQUENCE TO DOCMGR;

CONNECT &&schemaName/&&schemaPass@&&serverName;

CREATE OR REPLACE PROCEDURE GET_TOKEN_DOCID_LINK
AS
BEGIN
	DELETE FROM DOCMGR_EXTERNAL_LINKS WHERE APPNAME = 'CHEM_REG';
	FOR I IN (SELECT PREFIX FROM REGDB.SEQUENCE) LOOP
		FOR J IN (SELECT TOKEN_TEXT FROM DR$INDEX_DOCMGR_DOCUMENTS$I WHERE TOKEN_TEXT LIKE I.PREFIX || '%') LOOP
			FOR K IN (SELECT DOCID, SUBMITTER, DATE_SUBMITTED FROM DOCMGR_DOCUMENTS WHERE CONTAINS(DOC, J.TOKEN_TEXT, 1) > 0) LOOP
	           	INSERT INTO DOCMGR_EXTERNAL_LINKS (APPNAME, LINKTYPE, LINKID, DOCID, LINKFIELDNAME, SUBMITTER, DATE_SUBMITTED)
	           								VALUES('CHEM_REG', 'CHEMREGREGNUMBER', INSERTDASH(J.TOKEN_TEXT, I.PREFIX), K.DOCID, 'REG_NUMBER', K.SUBMITTER, K.DATE_SUBMITTED);
			END LOOP;
		END LOOP;
	END LOOP;
END;
/


-- Clear token jobs
CONNECT &&schemaName/&&schemaPass@&&serverName;

CREATE OR REPLACE PROCEDURE CLEARTOKENJOBS
IS
BEGIN
	FOR I IN (SELECT JOB FROM USER_JOBS) LOOP
		DBMS_JOB.REMOVE(I.JOB);
	END LOOP;
END;
/
-- Run job immediately and every midnight
CREATE OR REPLACE PROCEDURE SUBMITTOKENJOB
IS
	JOB_ID BINARY_INTEGER;
BEGIN
	--DBMS_JOB.SUBMIT(JOB_ID, 'GET_TOKEN_DOCID_LINK(''' || PREFIX || ''');', SYSDATE, 'SYSDATE + (120/1440)');
	DBMS_JOB.SUBMIT(JOB_ID, 'GET_TOKEN_DOCID_LINK;', TRUNC(SYSDATE, 'DD'), 'TRUNC(SYSDATE+1,''DD'')'); -- run every midnight
	DBMS_JOB.RUN(JOB_ID, TRUE);
	COMMIT;
END;
/

-- Run GET_TOKEN_DOCID_LINK job next second
CREATE OR REPLACE PROCEDURE RUNTOKENJOB
IS
	JOB_ID BINARY_INTEGER;
BEGIN
	DBMS_LOCK.SLEEP(5);
	SELECT JOB INTO JOB_ID FROM USER_JOBS WHERE WHAT='GET_TOKEN_DOCID_LINK;';
	--DBMS_JOB.SUBMIT(JOB_ID, 'GET_TOKEN_DOCID_LINK;', TRUNC(SYSDATE), 'SYSDATE + 1/86400'); -- run next second
	DBMS_JOB.RUN(JOB_ID, TRUE);
	--DBMS_JOB.REMOVE(JOB_ID);
	COMMIT;
END;
/


-- Submit jobs for all the existing prefixes in regdb.sequence

CONNECT &&schemaName/&&schemaPass@&&serverName;
GRANT EXECUTE ON SUBMITTOKENJOB TO REGDB;
GRANT EXECUTE ON CLEARTOKENJOBS TO REGDB;

CONNECT &&schemaName/&&schemaPass@&&serverName;

BEGIN
	DOCMGR.SUBMITTOKENJOB;	
END;
/

-- Create trigger such that regdb.sequence onchange submit jobs for all the prefixes

--CONNECT SYSTEM/SUNNY;
--GRANT CREATE ANY TRIGGER TO DOCMGR;

--CONNECT DOCMGR/ORACLE;

--CREATE OR REPLACE TRIGGER DOCMGR.PREFIX_ONCHANGE
--AFTER DELETE OR INSERT OR UPDATE ON REGDB.SEQUENCE
--BEGIN
--	DOCMGR.CLEARTOKENJOBS();
--	FOR I IN (SELECT PREFIX FROM REGDB.SEQUENCE) LOOP
--		DOCMGR.SUBMITTOKENJOB(I.PREFIX);
--    END LOOP;
--END;
--/

COMMIT;
SHOW ERRORS;