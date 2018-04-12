-- Purpose: Audit trail before-insert trigger generator.



--          This script creates a BEFORE-INSERT trigger script for 
--          a given table for auditing.

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

PROMPT -----------------------------------------------------------;
PROMPT Audit Trail BEFORE-INSERT Trigger Creation Script Generator;
PROMPT -----------------------------------------------------------;
-- accept tabowner char PROMPT 'Table Owner:  ';
ACCEPT tabname  char PROMPT 'Table Name:  ';
-- accept filename char PROMPT 'Spool to <filename>:  ';
PROMPT Spool File Name:  &&tabname._bi0.trg;
PROMPT ------------------------------------------------------;
PROMPT Working...;

COLUMN remarks FORMAT a80;
COLUMN col0 FORMAT 999999990 NOPRINT;
COLUMN col1 FORMAT a80;

DEFINE spoolfile = &&tabname._bi0.trg

SPOOL &&spoolfile;

REM --------------------------------------------------------------------
REM This query generates a file header.
REM --------------------------------------------------------------------

SELECT RPAD('rem ' || '&&spoolfile',80,' ') ||
       RPAD('rem ' || 'Generated on ' || sysdate || ' by ' || 
       user || '.',80,' ') ||
       RPAD('rem ' || 'Script to create BI audit trigger for the ' ||
       UPPER('&&tabname') || ' table.',80,' ') || RPAD(' ',80,' ') remarks
FROM dual; 

REM --------------------------------------------------------------------
REM This query generates the trigger text.
REM --------------------------------------------------------------------

SELECT RPAD('create or replace trigger TRG_AUDIT_' || table_name || '_BI0',80,' ') ||
       RPAD('  before insert',80,' ') ||
       RPAD('  on ' || lower(table_name),80,' ') ||
       RPAD('  for each row',80,' ') ||
       RPAD('  when (new.rid is null or new.rid = 0)',80,' ') ||
       RPAD(' ',80,' ') ||
       RPAD('begin',80,' ') ||
       RPAD('  select trunc(' || 'seq_rid.nextval)',80,' ') ||
       RPAD('  into :new.rid',80,' ') ||
       RPAD('  from dual;',80,' ') ||
       RPAD('end;',80,' ') ||
         '/' col1
 FROM user_tables
WHERE table_name = UPPER('&&tabname');

SELECT 0 col0,
       null col1
  FROM dual
 UNION
SELECT 1 col0,
       'exit;' col1
  FROM dual
 ORDER BY 1;
          
SPOOL OFF;

SET SPACE 1;
SET VERIFY ON;
SET NUMWIDTH 10;
SET HEADING ON;
SET PAGESIZE 14;



SET FEEDBACK ON;

