--  spool commands to grant old table grants on new table
-- Used later during CLOB update script

set heading off
set feedback off
set termout off

spool sql\tmp.sql

SELECT 'grant '||s.privilege||' on '||s.table_name||' to '||s.grantee||DECODE(s.grantable,'YES', ' with grant option')||';'
FROM user_tab_privs s WHERE s.table_name IN ('STRUCTURES','TEMPORARY_STRUCTURES');

spool off

set termout on
