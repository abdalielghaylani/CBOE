-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.
-- This script insert a cartridge type row in COEDB.VALID_CARTRIDGE table

SET SERVEROUTPUT ON

prompt Insert new cartridge

declare
v_sql LONG;
begin

v_sql:='INSERT INTO "COEDB"."VALID_CARTRIDGE"
  (CARTRIDGE_ID, CARTRIDGE_NAME, CARTRIDGE_SCHEMA)
  VALUES
  (&1, ''&2'', ''&3'')';
execute immediate v_sql;

EXCEPTION
    WHEN OTHERS THEN
      IF SQLCODE = -1 THEN
        DBMS_OUTPUT.PUT_LINE('The Cartridge is already enabled.');
		NULL; -- suppresses ORA-00001: unique constraint exception
      ELSE
         RAISE;
      END IF;
END;
/
