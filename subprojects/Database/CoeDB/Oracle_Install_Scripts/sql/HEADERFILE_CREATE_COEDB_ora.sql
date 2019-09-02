--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

spool ON
spool sql\log_create_coedb_ora.txt

@@parameters.sql
@@prompts.sql

CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA

VAR Begin VARCHAR2(20);
VAR End VARCHAR2(20);
VAR Elapsed VARCHAR2(100);
exec :Begin:=to_char(systimestamp,'HH:MI:SS.FF4');

@@drops.sql
@@drops_&&UpgradeCsSecurity
@@tablespaces.sql

@"sql\Patches\Patch 11.0.1.0\sql\CsSecurity\createTempTableSpace_&&OraVersionNumber"

@@users.sql
@@CREATE_COEDB_ora.sql
--@@synonyms.sql
@@coeuser.sql
--Upgrade or Create the CsSecurity Schema
@"sql\Patches\Patch 11.0.1.0\sql\CsSecurity\HeaderFile_Upgrade_CsSecurity_TO_CoeDB_&&UpgradeCsSecurity"
@@inserts.sql
@@inserts_&&UpgradeCsSecurity 
@@grants.sql
@@proxyGrants.sql
--@@roles_&&UpgradeCsSecurity 
@@roles.sql

@sql\Patches\Parameters.sql
@"sql\Patches\Patch 11.0.1.0\Parameters.sql"
@"sql\Patches\Patch 11.0.2\patch.sql"

--11.0.3
@&&setNextPatch 
--11.0.4
@&&setNextPatch 
--12.1.0
@&&setNextPatch 
--12.1.3
@&&setNextPatch 
--12.3.0
@&&setNextPatch 
--12.4.0
@&&setNextPatch 
--12.5.0
@&&setNextPatch 
--12.5.2
@&&setNextPatch 
--12.5.3
@&&setNextPatch 
--12.6.0
@&&setNextPatch 
--12.6.1
@&&setNextPatch 
--12.6.2
@&&setNextPatch 
--12.6.3
@&&setNextPatch 
--17.1.0
@&&setNextPatch 
--17.1.1
@&&setNextPatch 
--18.1.0
@&&setNextPatch 
--18.1.1
@&&setNextPatch 
--19.1.0
@&&setNextPatch 



SET serveroutput on
BEGIN
	:End:=to_char(systimestamp,'HH:MI:SS.FF4');
	:Elapsed:=to_timestamp(:End,'HH:MI:SS.FF4')-to_timestamp(:Begin,'HH:MI:SS.FF4');

	dbms_output.put_line('Begin: '||:Begin);
	dbms_output.put_line('End: '||:End);
	dbms_output.put_line('Elapsed: '||substr(:Elapsed,instr(:Elapsed,' ')+1,13));
END;
/

prompt logged session to: sql/log_create_coedb_ora.txt
spool off

exit
