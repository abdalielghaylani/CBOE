--Copyright 1999-2008 CambridgeSoft Corporation. All rights reserved

spool ON
spool sql\log_testdataviews.txt

@parameters.sql
@promptsForTestDataViews.sql

CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA






@chemacxDB_DV.sql
@cheminvDB2_DV.sql
@regDB_DV.sql
@sampleDB_DV.sql




prompt logged session to: sql/test dataViews\log_testdataviews_ora.txt
spool off

exit
