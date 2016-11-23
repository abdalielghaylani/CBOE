--Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc.  All rights reserved.

--Creation script for Instance_CreateFunction.sql

--#########################################################
--CREATE DATASOURCE_INITIALIZATION FUNCTION
--#########################################################

CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA

CREATE OR REPLACE
  FUNCTION &&globalSchemaName..DATASOURCE_INITIALIZATION( PWD IN VARCHAR2 )
    RETURN NUMBER
  AS
    x NUMBER:=0;
  BEGIN
    -- Check if the procedure exist
    SELECT COUNT(1) INTO x FROM ALL_PROCEDURES
    WHERE UPPER(OBJECT_NAME) ='JCHEM_CORE_PKG'
      AND UPPER(OBJECT_TYPE)   ='PACKAGE'
      AND UPPER(PROCEDURE_NAME)='USE_PASSWORD';
    
    -- Execute procedure
    IF x > 0 THEN
      BEGIN
        EXECUTE IMMEDIATE 'call jchem_core_pkg.use_password('''||PWD||''')';
      EXCEPTION
        WHEN OTHERS
        THEN RETURN -1;
      END;
    END IF;
    
    RETURN x;
  END DATASOURCE_INITIALIZATION;
/
