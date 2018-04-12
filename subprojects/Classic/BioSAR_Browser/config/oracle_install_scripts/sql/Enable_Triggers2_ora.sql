
SET verify off

Connect &&schemaName/&&schemaPass@&&serverName

ALTER TRIGGER "BIOSARDB"."DB_COLUMN_TRIG" 
    ENABLE;
    
ALTER TRIGGER "BIOSARDB"."DB_TABLE_TRIG" 
    ENABLE;
    


