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
