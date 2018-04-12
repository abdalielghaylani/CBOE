set serveroutput on
set feedback off
set verify off
set embedded on
set heading off
spool tmp.sql

prompt create or replace trigger aud#&1
prompt after update on &1
prompt for each row
prompt begin

select '  audit_trail.check_val(raid, ''' || column_name ||
          ''', ' || ':new.' || column_name || ', :old.' || 
             column_name || ');'
from user_tab_columns where table_name = upper('&1') ORDER BY column_id
/
prompt end;;
prompt /

spool off
set feedback on
set embedded off
set heading on
set verify on