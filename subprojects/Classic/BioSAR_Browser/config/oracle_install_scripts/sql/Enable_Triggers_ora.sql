spool Enable_triggers.log
SET verify off
@@parameters.sql

Connect &&schemaName/&&schemaPass@&&serverName

ALTER TRIGGER BIOSARDB.DB_COLUMN_TRIG 
    ENABLE;
    
ALTER TRIGGER BIOSARDB.DB_TABLE_TRIG 
    ENABLE;
    
--RESET DB_COLUMN SEQUENCE
DECLARE
  maxID INTEGER;
  currID INTEGER;
  i number;
  j number;
BEGIN
  SELECT BIOSARDB.DB_COLUMN_SEQ.nextval into currid from dual;
  SELECT MAX(COLUMN_ID) INTO maxID FROM BIOSARDB.DB_COLUMN;
  for i in currid..maxid loop
    select BIOSARDB.DB_COLUMN_SEQ.nextval into j from dual;
  end loop;
END;
/

--RESET DB_TABLE SEQUENCE
DECLARE
  maxID INTEGER;
  currID INTEGER;
  i number;
  j number;
BEGIN
  SELECT BIOSARDB.DB_TABLE_SEQ.nextval into currid from dual;
  SELECT MAX(TABLE_ID) INTO maxID FROM BIOSARDB.DB_TABLE;
  for i in currid..maxid loop
    select BIOSARDB.DB_TABLE_SEQ.nextval into j from dual;
  end loop;
END;
/

--ENABLE CONSTRAINTS
ALTER TABLE "BIOSARDB"."DB_COLUMN" 
    ENABLE 
    CONSTRAINT FK_COLUMN_TABLE 
    ENABLE 
    CONSTRAINT FK_COL_DISPL_ID 
    ENABLE
    CONSTRAINT FK_COL_ID;

spool off
exit
