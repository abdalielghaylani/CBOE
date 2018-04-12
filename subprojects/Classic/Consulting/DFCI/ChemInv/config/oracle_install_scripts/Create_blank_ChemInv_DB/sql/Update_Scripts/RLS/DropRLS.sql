--Copyright 1999-2005 CambridgeSoft Corporation. All rights reserved
set echo off

@parameters.sql
@prompts.sql

set feedback on
spool Logs/LOG_DropRLS.txt


Connect &&schemaName/&&schemaPass@&&serverName

--' drop policies 
BEGIN
  DBMS_RLS.DROP_POLICY( 'CHEMINVDB2','INV_LOCATIONS','LOCATION_VPD_P1');
  DBMS_RLS.DROP_POLICY( 'CHEMINVDB2','INV_CONTAINERS','LOCATION_VPD_P1');
  DBMS_RLS.DROP_POLICY( 'CHEMINVDB2','INV_PLATES','LOCATION_VPD_P1');
END; 
/

Connect &&schemaName/&&schemaPass@&&serverName
--update Globals table
UPDATE globals SET VALUE='0' WHERE ID = 'RLS_ENABLED';
/


spool off
exit
