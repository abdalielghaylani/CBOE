REM Purpose: Audit Trail After Delete Trigger Generator.

REM          This script creates a AFTER-DELETE trigger script for 
REM          a given table for auditing.
REM Notes:   Does not handle LONG or similar columns at all!

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
PROMPT Audit Trail AFTER-DELETE Trigger Creation Script Generator;
PROMPT ----------------------------------------------------------;
ACCEPT tabname  char PROMPT 'Table Name:  ';
PROMPT Spool File Name:  &&tabname._ad0.trg;
PROMPT ------------------------------------------------------;
PROMPT Working...;

COLUMN remarks FORMAT a80;
COLUMN col0 FORMAT 999999990 noprint;
COLUMN col1 FORMAT a80;
COLUMN col2 FORMAT a80;
COLUMN col3 FORMAT a80;

SPOOL &&tabname._ad0.trg;

REM --------------------------------------------------------------------
REM This query generates a file header.
REM --------------------------------------------------------------------

SELECT RPAD('-- ' || '&&tabname._ad0.trg',80,' ') ||
       RPAD('-- ' || 'Generated on ' || sysdate || ' by ' || 
       user || '.', 80, ' ') ||
       RPAD('-- ' || 'Script to create AFTER-DELETE audit trigger ' ||
       'for the ' || UPPER('&&tabname') || ' table.',80,' ') || 
       RPAD(' ',80,' ') remarks
  FROM dual; 

REM --------------------------------------------------------------------
REM These queries generate the trigger text.
REM --------------------------------------------------------------------

SELECT RPAD('create or replace trigger TRG_AUDIT_' ||
       table_name || '_AD0',80,' ') ||
       RPAD('  after delete',80,' ') ||
       RPAD('  on ' || lower(table_name),80,' ') ||
       RPAD('  for each row',80,' ') ||
       RPAD(' ',80,' ') ||
       RPAD('declare',80,' ') ||
       RPAD('  raid number(10);',80,' ') ||
       RPAD('  deleted_data varchar2(4000);',80,' ') ||
       RPAD(' ',80,' ') ||
       RPAD('begin',80,' ') ||
       RPAD('  select seq_audit.nextval into raid from dual;',80,' ') ||
       RPAD(' ',80,' ') ||
       RPAD('  audit_trail.record_transaction',80,' ') ||
       RPAD('    (raid, ' || chr(39) || table_name || chr(39) ||
       ', :old.rid, ' || chr(39) || 'D' || chr(39) || ');',80,' ') ||
       RPAD(' ',80,' ') || 
       RPAD('  deleted_data :=',80,' ') col1
  FROM user_tables
 WHERE table_name = upper('&&tabname');

SELECT column_id col0,
       RPAD('  ' ||
       DECODE(a.data_type, 'NUMBER','to_char(',
       'DATE','to_char(', null) || ':old.' ||
       LOWER(a.column_name) ||
       DECODE(a.data_type, 'NUMBER',')',
       'DATE',')', null) || ' ' ||
       CHR(124) || CHR(124) || ' ' ||
       CHR(39) || CHR(124) || CHR(39) || ' ' ||
       CHR(124) || CHR(124), 80, ' ') col1
  FROM user_tab_columns a
 WHERE a.table_name = UPPER('&&tabname')
   AND a.data_type IN ('CHAR', 'VARCHAR', 'VARCHAR2', 'DATE', 'NUMBER')
   AND column_id < 
        (SELECT MAX(a2.column_id)
           FROM   user_tab_columns a2
          WHERE  a2.table_name = UPPER('&&tabname')
            AND    a2.data_type IN ('CHAR', 'VARCHAR',
               'VARCHAR2', 'DATE', 'NUMBER'))
 UNION
SELECT column_id col0,
       RPAD('  ' ||
       DECODE(a.data_type, 'NUMBER','to_char(',
       'DATE','to_char(', null) || ':old.' ||
       LOWER(a.column_name) ||
       DECODE(a.data_type, 'NUMBER',')',
       'DATE',')', null) || ';', 80, ' ') col1
  FROM user_tab_columns a
 WHERE a.table_name = UPPER('&&tabname')
   AND a.data_type IN ('CHAR', 'VARCHAR', 'VARCHAR2', 'DATE', 'NUMBER')
   AND column_id = 
       (SELECT MAX(a2.column_id)
          FROM   user_tab_columns a2
         WHERE  a2.table_name = UPPER('&&tabname')
           AND    a2.data_type IN ('CHAR', 'VARCHAR',
                 'VARCHAR2', 'DATE', 'NUMBER'))
 ORDER BY 1;

REM --------------------------------------------------------------------
REM This section finishes up the trigger.
REM --------------------------------------------------------------------

SELECT null
  FROM dual;

SELECT RPAD('insert into ' || 'audit_delete',80,' ') ||
       RPAD('(raid, row_data) values (raid, deleted_data);',80,' ') col1
  FROM dual;

SELECT 0 col0,
       'end;' col1
  FROM dual
 UNION
SELECT 1 col0,
       '/' col1
  FROM dual
 ORDER BY 1;

-- Optional - if you want 'exit' at the end of your script.
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
