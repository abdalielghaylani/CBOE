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