--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"sql\Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&currentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 


--#########################################################
--TABLES
--######################################################### 

ALTER TABLE &&schemaName..LOG_BULKREGISTRATION MODIFY Reg_Number VARCHAR2(50);

--#########################################################
--SEQUENCES
--#########################################################

DECLARE
  LNextID NUMBER(8);
BEGIN
    SELECT NVL(MAX(ID),0)+1 INTO LNextID FROM &&schemaName..HISTORY;
    EXECUTE IMMEDIATE 'DROP SEQUENCE &&schemaName..SEQ_HISTORY';
    EXECUTE IMMEDIATE 'CREATE SEQUENCE &&schemaName..SEQ_HISTORY START WITH '||LNextID||' CACHE 2';
END;
/


--#########################################################
--TRIGGERS
--#########################################################

--#########################################################
--INDEXES
--#########################################################

--#########################################################
--CONSTRAINTS
--#########################################################

--#########################################################
--VIEWS
--#########################################################

CREATE OR REPLACE VIEW &&schemaName..VW_MIXTURE_BATCH AS
        SELECT 	M.MIXTUREID, R.REGNUMBER, B.*, M.STRUCTUREAGGREGATION
        FROM 	&&schemaName..VW_MIXTURE M, &&schemaName..VW_REGISTRYNUMBER R, &&schemaName..VW_BATCH B
        WHERE 	M.REGID = R.REGID AND R.REGID = B.REGID;

--#########################################################
--PACKAGES
--#########################################################

set define off
@"sql\Patches\Patch 11.0.2\Packages\pkg_ConfigurationCompoundRegistry_body.sql"
@"sql\Patches\Patch 11.0.2\Packages\pkg_CompoundRegistry_def.sql"
@"sql\Patches\Patch 11.0.2\Packages\pkg_CompoundRegistry_body.sql"
@"sql\Patches\Patch 11.0.2\Packages\pkg_RegistrationRLS_body.sql"

set define on

--#########################################################
--DATA
--#########################################################

-- DGB CSBR-129585 Make all projects start up with Type "ALL"
update &&schemaName..vw_project set type = 'A';
COMMIT;

@"sql\Patches\Patch 11.0.2\UpdateConfigurations.sql"

-- JED: 03-MAY-2010
-- Patch for CSBR 124837 - change numeric config default values to NULL
declare
  v_csbr_number varchar2(500) := 'Inf: the user adds multiple compounds with no structure or unknown structure, an error appears and the substacne cannot be sent to registry.';
  v_name varchar2(50) := 'CSBR-124837';
  v_found number := 0;
  v_hasData number(1) := 0;
begin

  --bypass re-execution if the log entry for this patch has already been established
  select count(*) into v_found
  from &&schemaName..history h
  where h.name = v_name;

  select count(1) into v_hasData from &&schemaName..coeobjectconfig where objecttypeid = 1;
  
  if (v_found = 0 and v_hasData > 0) then
    
    update &&schemaName..coeobjectconfig c
    set c.xml = updateXml(
      xmltype(c.xml)
      , '//Calculations/Calculation[PropertyName="FORMULA_WEIGHT"]/DefaultText', null
      , '//Calculations/Calculation[PropertyName="BATCH_FORMULA"]/DefaultText', null
    ).getClobVal()
    where c.objecttypeid = 1;

    insert into &&schemaName..history
      (id, name, description, datecreated)
    values
      (&&schemaName..seq_history.nextval, v_name, v_csbr_number, sysdate);
  end if;

end;
/

/*
VERIFICATION: Run this before and after the script to verify manually
Values will be NULL (empty strings) after commit.
(1)
select
  extractValue(
    xmltype(c.xml), '//Calculations/Calculation[PropertyName="FORMULA_WEIGHT"]/DefaultText'
  ) as DefaultText_FormulaWeight,
  extractValue(
    xmltype(c.xml), '//Calculations/Calculation[PropertyName="BATCH_FORMULA"]/DefaultText'
  ) as DefaultText_BatchFormula
from &&schemaName..coeobjectconfig c where c.objecttypeid = 1;
*/


-- JED: 28-JULY-2010
-- For CSBR 128529 - update COE form 4015 to match form 4010

DECLARE
   v_found number := 0;
BEGIN
  --bypass execution if either COEFormGroups 4010 or 4015 is absent
  SELECT count(*) into v_found
  FROM &&securitySchemaName..coeform f
  WHERE f.id in (4010, 4015);

  IF (v_found = 2) THEN
    UPDATE &&securitySchemaName..coeform f
    SET f.coeform = (
      SELECT replace( f4010.coeform, ' id="4010" ', ' id="4015" ' )
      FROM &&securitySchemaName..coeform f4010
      WHERE f4010.id = 4010
    )
    WHERE f.id = 4015;
  END IF;
  
  SELECT count(1) into v_found
  FROM &&securitySchemaName..CoeDataView
  WHERE id =4007;
  if (v_found > 0) then
	--Add the attribute "dataviewid" in rows of CoeDB.CoeDataView where this not exists.
	UPDATE &&securitySchemaName..CoeDataView 
    SET CoeDataView=SUBSTR(CoeDataView,1,INSTR(CoeDataView,'>',INSTR(CoeDataView,'<COEDataView '))-1)||' dataviewid="'||4007||'"'||SUBSTR(CoeDataView,INSTR(CoeDataView,'>',INSTR(CoeDataView,'<COEDataView ')+1),LENGTH(CoeDataView)) 
    WHERE ID=4007 AND INSTR(CoeDataView,'dataviewid')=0;  
  end if;
 
  SELECT count(1) into v_found
  FROM &&securitySchemaName..CoeDataView
  WHERE id =4008;
  if (v_found > 0) then
	--Add the attribute "dataviewid" in rows of CoeDB.CoeDataView where this not exists.
	UPDATE &&securitySchemaName..CoeDataView 
    SET CoeDataView=SUBSTR(CoeDataView,1,INSTR(CoeDataView,'>',INSTR(CoeDataView,'<COEDataView '))-1)||' dataviewid="'||4008||'"'||SUBSTR(CoeDataView,INSTR(CoeDataView,'>',INSTR(CoeDataView,'<COEDataView ')+1),LENGTH(CoeDataView)) 
    WHERE ID=4008 AND INSTR(CoeDataView,'dataviewid')=0; 
  end if;
END;
/

COMMIT;

-- JHS: 29-JULY-2010
-- For CSBR 128765 - An error appears while searching for Project plus any other batch information
-- Replace the outer joins with inner joins, the searching doesn't warrant the outer join usage

DECLARE
   v_found number(1) := 0;
BEGIN
  SELECT count(1) into v_found
  FROM &&securitySchemaName..coedataview dv
  WHERE dv.id = 4003;

  IF (v_found > 0) THEN
    UPDATE &&securitySchemaName..coedataview dv
    SET dv.coedataview = (
      SELECT replace( dv4003.coedataview, 'jointype="OUTER"', 'jointype="INNER"' )
      FROM &&securitySchemaName..coedataview dv4003
      WHERE dv4003.id = 4003
    )
    WHERE dv.id = 4003;
  END IF;
END;
/

COMMIT;

DECLARE
   v_found number(1) := 0;
BEGIN
  SELECT count(1) into v_found
  FROM &&securitySchemaName..coedataview dv
  WHERE dv.id = 4006;

  IF (v_found > 0) THEN
    UPDATE &&securitySchemaName..coedataview dv
    SET dv.coedataview = (
      SELECT replace( dv4006.coedataview, 'jointype="OUTER"', 'jointype="INNER"' )
      FROM &&securitySchemaName..coedataview dv4006
      WHERE dv4006.id = 4006
    )
    WHERE dv.id = 4006;
  END IF;
END;
/

COMMIT;


--#########################################################

UPDATE &&schemaName..Globals
	SET Value = '&&currentPatch' 
	WHERE UPPER(ID) = 'VERSION_SCHEMA';

UPDATE &&schemaName..Globals
	SET Value = '&&versionApp' 
	WHERE UPPER(ID) = 'VERSION_APP';
COMMIT;

prompt **** Patch &&currentPatch Applied ****

COL setNextPatch NEW_VALUE setNextPatch NOPRINT
SELECT	CASE
		WHEN  '&&toVersion'='&&currentPatch'
		THEN  'sql\Patches\stop.sql'
		ELSE  '"sql\Patches\Patch &&nextPatch\patch.sql"'
	END	AS setNextPatch 
FROM	DUAL;

@&&setNextPatch 






