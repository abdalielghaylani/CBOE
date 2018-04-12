-- Copyright Cambridgesoft Corp 2001-2005 all rights reserved
prompt 'Creating Audit Tables...'
prompt '------------------------'


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


-- Audit Trail Package
@@pkg_Audit_trail_Def.sql;
@@pkg_Audit_trail_Body.sql;

-- Audit Trail Triggers
@@sql\triggers\css_people_ad0.trg;
@@sql\triggers\css_people_au0.trg;
@@sql\triggers\css_people_bi0.trg;
@@sql\triggers\css_security_roles_ad0.trg;
@@sql\triggers\css_security_roles_au0.trg;
@@sql\triggers\css_security_roles_bi0.trg;