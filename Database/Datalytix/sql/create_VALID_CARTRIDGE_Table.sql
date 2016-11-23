-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.
-- This script create COEDB.VALID_CARTRIDGE table for Datalytix

SET SERVEROUTPUT ON

prompt Create COEDB.VALID_CARTRIDGE table

declare
v_sql LONG;
begin

v_sql:='CREATE TABLE "COEDB"."VALID_CARTRIDGE"
  (
    CARTRIDGE_ID     NUMBER(8,0) NOT NULL,
    CARTRIDGE_NAME   VARCHAR2(50),
    CARTRIDGE_SCHEMA VARCHAR2(50),
	CONSTRAINT PK_VALID_CARTRIDGE PRIMARY KEY (CARTRIDGE_ID) USING INDEX TABLESPACE &&indexTableSpaceName
  )';
execute immediate v_sql;

EXCEPTION
    WHEN OTHERS THEN
      IF SQLCODE = -955 THEN
        DBMS_OUTPUT.PUT_LINE('The table already exists.');
		NULL; -- suppresses ORA-00955 exception
      ELSE
         RAISE;
      END IF;
END;
/
