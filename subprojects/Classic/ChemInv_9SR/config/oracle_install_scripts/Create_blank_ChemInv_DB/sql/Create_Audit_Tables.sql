-- Copyright Cambridgesoft Corp 2001-2005 all rights reserved
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
TABLESPACE &&auditTableSpaceName
   PCTFREE 0 
   PCTUSED 90 
   STORAGE (INITIAL 100k 
               NEXT 100k
         MAXEXTENTS UNLIMITED 
        PCTINCREASE 0);

-- AUDIT_DELETE - one row per deleted row.
CREATE TABLE audit_delete (
   raid      NUMBER(10)     NOT NULL,    
   row_data  VARCHAR2(4000) NOT NULL
)  
TABLESPACE &&auditTableSpaceName
   PCTFREE 0 
   PCTUSED 90 
   STORAGE (INITIAL 100k 
               NEXT 100k
         MAXEXTENTS UNLIMITED 
        PCTINCREASE 0);

-- AUDIT_COMPOUND
CREATE TABLE audit_compound (
   raid      NUMBER(10)     NOT NULL,    
   old_base64_cdx  CLOB
)  
TABLESPACE &&auditTableSpaceName
   PCTFREE 0 
   PCTUSED 90 
   STORAGE (INITIAL 1M 
               NEXT 1M
         MAXEXTENTS UNLIMITED 
        PCTINCREASE 0);

-- Create Aggregate Audit view
CREATE OR REPLACE VIEW "INV_VW_AUDIT_AGGREGATE" (
    "RID","RAID","TABLE_NAME","ACTION","USER_NAME","TIMESTAMP") AS 
    	SELECT 	
		rid,
		0 raid,
		'INV_COMPOUNDS' table_name,
		'I' action,
		creator user_name,
		timestamp
	FROM inv_compounds
	UNION
	SELECT 
		rid,
		0 raid,	
		'INV_CONTAINERS' table_name,
		'I' action,
		creator user_name,
		timestamp
	FROM inv_containers
	UNION
	SELECT 
		rid,
		0 raid,
		'INV_URL' table_name,
		'I' action,
		creator user_name,
		timestamp
	FROM inv_url
	UNION
	SELECT 
		rid,
		0 raid,
		'INV_LOCATIONS' table_name,
		'I' action,
		creator user_name,
		timestamp
	FROM inv_locations
	UNION
  SELECT
  	rid,
    0 raid,
    'INV_REQUESTS' table_name,
    'I' action,
    creator user_name,
    TIMESTAMP
  FROM inv_requests
	UNION
  SELECT
  	rid,
    0 raid,
    'INV_SUPPLIERS' table_name,
    'I' action,
    creator user_name,
    TIMESTAMP
  FROM inv_suppliers
	UNION
  SELECT
  	rid,
    0 raid,
    'INV_ORDERS' table_name,
    'I' action,
    creator user_name,
    TIMESTAMP
  FROM inv_orders
	UNION
  SELECT
  	rid,
    0 raid,
    'INV_ORDER_CONTAINERS' table_name,
    'I' action,
    creator user_name,
    TIMESTAMP
  FROM inv_order_containers
	UNION
  SELECT
  	rid,
    0 raid,
    'INV_CONTAINER_CHECKIN_DETAILS' table_name,
    'I' action,
    creator user_name,
    TIMESTAMP
  FROM inv_container_checkin_details
	UNION
  SELECT
  	rid,
    0 raid,
    'INV_SYNONYMS' table_name,
    'I' action,
    creator user_name,
    TIMESTAMP
  FROM inv_synonyms
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
@@packages\pkg_Audit_trail_def.sql;
@@packages\pkg_Audit_trail_Body.sql;

-- Trigger to fill in the RID (Record ID) for the inv_locations table
CREATE OR REPLACE TRIGGER trg_Loc_bi0
  BEFORE INSERT
      ON inv_locations
     FOR EACH ROW
    WHEN (new.rid IS NULL) 
BEGIN
  SELECT TRUNC(seq_rid.nextval) 
    INTO :new.rid
    FROM dual; 
END; 
/

@@triggers\inv_locations_bi0.trg;
@@triggers\inv_locations_au0.trg;
@@triggers\inv_locations_ad0.trg;
@@triggers\inv_containers_bi0.trg;
@@triggers\inv_containers_au0.trg;
@@triggers\inv_containers_au1.trg;
@@triggers\inv_containers_ad0.trg;
@@triggers\inv_compounds_bi0.trg;
@@triggers\inv_compounds_au0.trg;
@@triggers\inv_compounds_ad0.trg;
@@triggers\inv_url_bi0.trg;
@@triggers\inv_url_au0.trg;
@@triggers\inv_url_ad0.trg;
@@triggers\inv_requests_bi0.trg;
@@triggers\inv_requests_au0.trg;
@@triggers\inv_requests_ad0.trg;
@@TRIGGERS\trg_inv_reports_id.SQL;
@@triggers\inv_suppliers_bi0.trg;
@@triggers\inv_suppliers_au0.trg;
@@triggers\inv_suppliers_ad0.trg;
@@triggers\inv_orders_bi0.trg;
@@triggers\inv_orders_au0.trg;
@@triggers\inv_orders_ad0.trg;
@@triggers\inv_order_containers_bi0.trg;
@@triggers\inv_order_containers_au0.trg;
@@triggers\inv_order_containers_ad0.trg;
@@triggers\inv_container_checkin_details_bi0.trg;
@@triggers\inv_container_checkin_details_au0.trg;
@@triggers\inv_container_checkin_details_ad0.trg;
@@triggers\inv_synonyms_bi0.trg
@@triggers\inv_synonyms_au0.trg
@@triggers\inv_synonyms_ad0.trg
