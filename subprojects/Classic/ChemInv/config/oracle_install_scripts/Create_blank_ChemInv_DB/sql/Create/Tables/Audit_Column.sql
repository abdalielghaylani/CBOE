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