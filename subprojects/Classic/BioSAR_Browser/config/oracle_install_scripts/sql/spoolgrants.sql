--  spool commands to grant old table grants on new table
-- Used later during CLOB update script

set echo off


set heading off
set feedback off

set termout off

spool Transfer_Grants.sql

select 'prompt Transfering grants from cs_security to biosardb...' from dual; 

SELECT 'grant '||s.privilege||' on '||s.table_name||' to '||s.grantee||DECODE(s.grantable,'YES', ' with grant option')||';'
FROM user_tab_privs s WHERE s.table_name IN ('DB_TABLE','DB_COLUMN','DB_FORMGROUP','DB_FORM','DB_FORMGROUP_TABLES','DB_FORM_ITEM','DB_RELATIONSHIP','DB_QUERY','DB_QUERY_ITEM')
/

spool off

set termout on

