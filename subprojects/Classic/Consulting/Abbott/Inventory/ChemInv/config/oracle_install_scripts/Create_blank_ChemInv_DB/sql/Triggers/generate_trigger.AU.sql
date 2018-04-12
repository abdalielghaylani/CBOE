REM Purpose: Audit Trail After-Update Trigger Generator.

REM          This script creates a AFTER-UPDATE trigger script for 
REM          a given table for auditing.
REM Notes:   Does not handle LONG or similar columns at all!
REM          Chr(39) is the code representation of a single quote sign

SET SPACE 0;
SET VERIFY OFF;
SET NUMWIDTH 4;
SET HEADING OFF;
SET LINESIZE 80;
SET PAGESIZE 0;
SET FEEDBACK OFF;
SET RECSEP OFF;
SET LONG 255;
SET ECHO OFF;
SET TRIMSPOOL ON;

PROMPT ----------------------------------------------------------;
PROMPT Audit Trail AFTER-UPDATE Trigger Creation Script Generator;
PROMPT ----------------------------------------------------------;
ACCEPT tabname  char PROMPT 'Table Name:  ';
PROMPT Spool File Name:  &&tabname._au0.trg;
PROMPT ----------------------------------------------------------;
PROMPT Working...;

COLUMN remarks FORMAT a80;
COLUMN col0 FORMAT 999999990 noprint;
COLUMN col1 FORMAT a80;
COLUMN col2 FORMAT a80;
COLUMN col3 FORMAT a10;
COLUMN col4 FORMAT a80;
COLUMN col5 FORMAT a80;
COLUMN col6 FORMAT a80;
COLUMN col7 FORMAT a80;
COLUMN col8 FORMAT a80;

SPOOL &&tabname._au0.trg;

REM --------------------------------------------------------------------
REM This query generates a file header.
REM --------------------------------------------------------------------

SELECT RPAD('-- ' || '&&tabname._au0.trg',80,' ') ||
       RPAD('-- ' || 'Generated on ' || SYSDATE||' by ' || 
       user || '.', 80, ' ') ||
       RPAD('-- ' || 'Script to create AFTER-UPDATE audit trigger ' ||
       'for the ' || UPPER('&&tabname') || ' table.',80,' ') || 
       RPAD(' ',80,' ') remarks
  FROM dual; 

REM --------------------------------------------------------------------
REM These queries generate the trigger text.
REM --------------------------------------------------------------------

SELECT RPAD('create or replace trigger TRG_AUDIT_' || 
       table_name || '_AU0', 80, ' ') ||
       RPAD('  after update of', 80, ' ') col1
  FROM user_tables
 WHERE table_name = UPPER('&&tabname');

SELECT '  ' || LOWER(utc.column_name) || ',' col1
  FROM user_tab_columns utc
 WHERE utc.table_name = UPPER('&&tabname')
   AND utc.data_type in ('CHAR', 'VARCHAR', 'VARCHAR2', 'DATE', 'NUMBER')
   AND utc.column_name not in ('RID', 'CREATOR', 'TIMESTAMP')
 ORDER BY utc.column_id;

SELECT '  ' || LOWER(utc.column_name) col1
  FROM user_tab_columns utc
 WHERE utc.table_name = UPPER('&&tabname')
   AND utc.column_name = 'RID';

SELECT RPAD('  on ' || LOWER(table_name),80,' ') ||
       RPAD('  for each row',80,' ') ||
       RPAD(' ',80,' ') ||
       RPAD('declare',80,' ') ||
       RPAD('  raid number(10);',80,' ') ||
       RPAD(' ',80,' ') ||
       RPAD('begin',80,' ') ||
       RPAD('  select ' || 
       'seq_audit.nextval into raid from dual;',80,' ') ||
       RPAD(' ',80,' ') ||
       RPAD('  audit_trail.record_transaction',80,' ') ||
       RPAD('    (raid, ' || CHR(39) || table_name || CHR(39) ||
       ', :' || 'old.rid, ' || CHR(39) || 'U' || CHR(39) || ');',80,' ') ||
       RPAD(' ',80,' ') col1
  FROM user_tables
 WHERE table_name = upper('&&tabname');

REM --------------------------------------------------------------------
REM This section builds the column comparison section.
REM --------------------------------------------------------------------

SELECT RPAD('  if nvl(:old.' || LOWER(a.column_name) || ',' ||
       DECODE(a.data_type, 'NUMBER','0', 
       'DATE', CHR(39) || '9595-12-31' || CHR(39),
       CHR(39) || ' ' || CHR(39) ) ||
       ') != ',80,' ') col4,
       RPAD('     NVL(:new.'||LOWER(a.column_name)||',' ||
       DECODE(a.data_type, 'NUMBER','0', 
       'DATE', CHR(39) || '9595-12-31' || CHR(39), 
       CHR(39) || ' ' || CHR(39) ) ||
       ') then',80,' ') col5,
       RPAD('     audit_trail.column_update',80,' ') col6,
       RPAD('       (raid, ' || chr(39) || a.column_name || CHR(39) ||
       ',',80,' ') col7,
       RPAD('       :old.' || LOWER(a.column_name) ||
       ', :new.' || lower(a.column_name) || ');',80,' ') col8,
       '  end if;' col3
  FROM user_tab_columns a
 WHERE a.table_name = upper('&&tabname')
   AND a.data_type IN ('CHAR', 'VARCHAR', 'VARCHAR2', 'DATE', 'NUMBER')
   AND a.column_name NOT IN ('CREATOR', 'TIMESTAMP')
 ORDER BY a.column_id;

REM --------------------------------------------------------------------
REM This section finishes up the trigger.
REM --------------------------------------------------------------------

SELECT null
  FROM dual;

SELECT 0 col0,
       'end;' col1
  FROM dual
 UNION
SELECT 1 col0,
       '/' col1
  FROM dual
 ORDER BY 1;

-- Optional:  If you want "exit" at the end of your script.
-- SELECT 0 col0,
--        null col1
--   FROM dual
--  UNION
-- SELECT 1 col0,
--        'exit;' col1
--   FROM dual
--  ORDER BY 1;

SPOOL OFF;

SET SPACE 1;
SET VERIFY ON;
SET NUMWIDTH 10;
SET HEADING ON;
SET PAGESIZE 14;

SET FEEDBACK ON;
