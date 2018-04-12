prompt 'creating audit tables...'


-- unique record IDs for each row in audited tables. 
CREATE SEQUENCE "SEQ_RID"
  START WITH 1000        
  INCREMENT BY 1      
  ORDER            
  NOCYCLE            
  CACHE 100       
  MINVALUE 1      
  NOMAXVALUE;


-- Unique id for audit row table
CREATE SEQUENCE "SEQ_AUDIT" 
  START WITH 1000       
  INCREMENT BY 1     
  ORDER              
  NOCYCLE            
  CACHE 100          
  MINVALUE 1       
  NOMAXVALUE;         


-- AUDIT_ROW - one row per transaction.
CREATE TABLE audit_row (
   raid        NUMBER(10)   NOT NULL,         /* Row audit ID */
   table_name  VARCHAR2(30) NOT NULL,        
   rid         NUMBER(10)   NOT NULL,         /* Record ID (from table) */
   action      VARCHAR2(1)  NOT NULL,         /* Action (I, U, D) */
   timestamp   DATE DEFAULT SYSDATE NOT NULL, 
   user_name   VARCHAR2(30) DEFAULT RTRIM(USER) NOT NULL
)
   TABLESPACE &&auditTableSpaceName
   PCTFREE 0 
   PCTUSED 90 
   STORAGE (INITIAL 100k 
               NEXT 100k
         MAXEXTENTS UNLIMITED 
        PCTINCREASE 0);


-- AUDIT_COLUMN - one row per changed column.
CREATE TABLE audit_column (
   raid        NUMBER(10)   NOT NULL,   /* Row audit ID */
   caid        NUMBER(10)   NOT NULL,   /* Column audit ID */
   column_name VARCHAR2(30) NOT NULL  , /* Column name */ 
   old_value   VARCHAR2(4000),
   new_value   VARCHAR2(4000)
)
TABLESPACE &&docTableSpaceName
   PCTFREE 0 
   PCTUSED 90 
   STORAGE (INITIAL 100k 
               NEXT 100k
         MAXEXTENTS UNLIMITED 
        PCTINCREASE 0);

-- AUDIT_DELETE - one row per deleted row.
CREATE TABLE audit_delete (
   raid      NUMBER(10)     NOT NULL,    
   row_data  VARCHAR2(4000) NOT NULL,
   doc_blob	 BLOB default EMPTY_BLOB()
)  
TABLESPACE &&docTableSpaceName
   PCTFREE 0 
   PCTUSED 90 
   STORAGE (INITIAL 100k 
               NEXT 100k
         MAXEXTENTS UNLIMITED 
        PCTINCREASE 0);

--SYAN added to fix CSBR-75714
Connect &&InstallUser/&&sysPass@&&serverName
GRANT CREATE ANY VIEW TO &&schemaName;
Connect &&schemaName/&&schemaPass@&&serverName
--End of SYAN modification

-- Create Aggregate Audit view
CREATE OR REPLACE VIEW "DOCMGR_VW_AUDIT_AGGREGATE" (
    "RID","RAID","TABLE_NAME","ACTION","USER_NAME","TIMESTAMP") AS 
    	SELECT 	
		rid,
		0 raid,
		'DOCMGR_EXTERNAL_LINKS' table_name,
		'I' action,
		submitter user_name,
		DATE_SUBMITTED
	FROM DOCMGR_EXTERNAL_LINKS
	UNION
	SELECT 
		rid,
		0 raid,	
		'DOCMGR_DOCUMENTS' table_name,
		'I' action,
		submitter user_name,
		DATE_SUBMITTED
	FROM DOCMGR_DOCUMENTS
UNION
	SELECT 
		rid,
		raid as raid,
		table_name,
		action,
		user_name,
		timestamp
		FROM audit_row r WITH READ ONLY;
/

-- Audit Trail Package
@@pkg_Audit_trail_def.sql;
@@pkg_Audit_trail_Body.sql;

@@docmgr_external_links_bi0.trg;
@@docmgr_external_links_au0.trg;
@@docmgr_external_links_ad0.trg;

@@docmgr_documents_bi0.trg;
@@docmgr_documents_au0.trg;
@@docmgr_documents_ad0.trg;

