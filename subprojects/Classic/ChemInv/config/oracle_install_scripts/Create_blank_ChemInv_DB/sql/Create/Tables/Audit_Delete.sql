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