SET verify off


Connect &&schemaName/&&schemaPass@&&serverName

ALTER TRIGGER BIOSARDB.DB_COLUMN_TRIG 
    DISABLE;
    
ALTER TRIGGER BIOSARDB.DB_TABLE_TRIG 
    DISABLE;

--disable constraints on db_column
ALTER TABLE BIOSARDB.DB_COLUMN 
    DISABLE 
    CONSTRAINT FK_COLUMN_TABLE 
    DISABLE 
    CONSTRAINT FK_COL_DISPL_ID 
    DISABLE 
    CONSTRAINT FK_COL_ID;
    
commit;