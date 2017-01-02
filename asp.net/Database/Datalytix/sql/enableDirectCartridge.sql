-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.
-- This script enable Direct Cartridge for Datalytix

spool log_enableDirectCartridge.txt

@@parameters.sql

-- Ask user for Oracle service name.
ACCEPT serverName CHAR DEFAULT '' PROMPT 'Enter the target Oracle service name:'

Connect &&globalSchemaName/&&globalSchemaPass@&&serverName
prompt

SET ECHO OFF
SET verify off
SET SERVEROUTPUT ON

-- Ask user for Direct Cartridge schema name.
ACCEPT direct_cartridge_owner CHAR DEFAULT 'C$DIRECT90' PROMPT 'Enter the Accelrys Data Direct schema name (C$DIRECT90):'

VARIABLE script_name VARCHAR2(50)
COLUMN :script_name NEW_VALUE script_file NOPRINT; 

-- Check if Direct Cartridge was installed
DECLARE
  FlagSchema Number;
BEGIN
    SELECT count(1) 
        INTO FlagSchema
        FROM DBA_USERS
        WHERE UPPER(USERNAME)=UPPER('&direct_cartridge_owner');
    IF FlagSchema>0 THEN
        :script_name := 'doEnableDirectCartridge.sql';
    ELSE
        :script_name := 'skipEnableCartridge.sql';		
    END IF;
END;
/
SELECT :script_name FROM DUAL;
@@&script_file


--
spool off
exit