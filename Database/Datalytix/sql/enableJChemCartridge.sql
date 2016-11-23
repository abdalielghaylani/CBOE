-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.
-- This script enable JChem Cartridge for Datalytix

spool log_enableJChemCartridge.txt

@@parameters.sql

-- Ask user for Oracle service name.
ACCEPT serverName CHAR DEFAULT '' PROMPT 'Enter the target Oracle service name:'

Connect &&securitySchemaName/&&securitySchemaPass@&&serverName
prompt

SET ECHO OFF
SET verify off
SET SERVEROUTPUT ON

-- Ask user for JChem Cartridge schema name.
ACCEPT jchem_cartridge_owner CHAR DEFAULT 'JCHEM' PROMPT 'Enter the ChemAxon JChem Cartridge schema name (JCHEM):'

VARIABLE script_name VARCHAR2(50)
COLUMN :script_name NEW_VALUE script_file NOPRINT; 

-- Check if JChem Cartridge was installed
DECLARE
  FlagSchema Number;
BEGIN
    SELECT count(1) 
        INTO FlagSchema
        FROM DBA_USERS
        WHERE UPPER(USERNAME)=UPPER('&jchem_cartridge_owner');
    IF FlagSchema>0 THEN
		:script_name := 'doEnableJChemCartridge.sql';
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