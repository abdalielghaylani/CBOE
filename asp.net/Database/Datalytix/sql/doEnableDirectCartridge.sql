-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.

prompt
prompt Enable Accelrys Data Direct in Datalytix
prompt

-- Grant permission for global user to access Direct cartridge.
EXECUTE "&direct_cartridge_owner".MDLAUXOP.SETUP;

Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

-- Grant permission for primary COEDB to access Direct cartridge.
EXECUTE "&direct_cartridge_owner".MDLAUXOP.SETUP;

@@create_VALID_CARTRIDGE_Table.sql
@@insert_cartridge_type.sql 2 'Accelrys Data Direct' &direct_cartridge_owner
