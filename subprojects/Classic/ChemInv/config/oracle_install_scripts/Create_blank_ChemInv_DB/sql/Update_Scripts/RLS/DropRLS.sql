--Copyright 1999-2005 CambridgeSoft Corporation. All rights reserved
-- COE11  15-Sep-2009  kfd

set echo off

@parameters.sql
@prompts.sql

set feedback on
spool Logs/LOG_DropRLS.txt


Connect &&InstallUser/&&sysPass@&&serverName
--' drop policies 
BEGIN
  DBMS_RLS.DROP_POLICY( '&&schemaName','INV_LOCATIONS','LOCATION_VPD_P1');
  DBMS_RLS.DROP_POLICY( '&&schemaName','INV_CONTAINERS','LOCATION_VPD_P1');
  DBMS_RLS.DROP_POLICY( '&&schemaName','INV_PLATES','LOCATION_VPD_P1');
END; 
/

Connect &&schemaName/&&schemaPass@&&serverName

--update Globals table
UPDATE globals SET VALUE='0' WHERE ID = 'RLS_ENABLED';
commit;


spool off
exit
