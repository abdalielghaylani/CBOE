--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 

Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Providing additional grants to &&schemaName...'
prompt '#########################################################'
Connect &&regSchemaName/&&regSchemaPass@&&serverName
grant execute on &&REGSchemaName..refresh_User_regnum_count to &&schemaName;

grant select on &&REGSchemaName..vw_structure to &&schemaName;
grant select on &&REGSchemaName..VW_COMPOUND_IDENTIFIER to &&schemaName WITH GRANT OPTION ;
grant select on &&REGSchemaName..VW_COMPOUND to &&schemaName;
grant select on &&REGSchemaName..VW_MIXTURE_COMPONENT to &&schemaName;
grant select on &&REGSchemaName..VW_IDENTIFIERTYPE to &&schemaName;
grant select on &&REGSchemaName..VW_MIXTURE to &&schemaName;
grant select on &&REGSchemaName..VW_BATCHCOMPONENT to &&schemaName;
grant select on &&REGSchemaName..VW_BATCHCOMPONENTFRAGMENT to &&schemaName;
grant select on &&REGSchemaName..VW_COMPOUND_FRAGMENT to &&schemaName;
grant select on &&REGSchemaName..VW_FRAGMENT to &&schemaName;


Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Updating Functions, Procedures and Packages...'
prompt '#########################################################'
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_CreateLocation.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_getRegAltID.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_GETSALTNAME.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_GETSOLVATENAME.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_getRegComponentIdentifier.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_GETREGNAME.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_UpdateLocation.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Links_Body.sql"

prompt '#########################################################'
prompt 'Providing additional grants to &&schemaName...'
prompt '#########################################################'
connect &&InstallUser/&&sysPass@&&serverName
grant alter any materialized view to &&schemaName;


prompt 
-- changed refresh fast on commit to refresh fast on demand
declare
  no_primary_key exception;
  PRAGMA EXCEPTION_INIT (no_primary_key, -12014);
  no_primary_key1 exception;
  PRAGMA EXCEPTION_INIT (no_primary_key1, -12024);
  no_primary_key2 exception;
  PRAGMA EXCEPTION_INIT (no_primary_key2, -12016);
cnt number;
begin
select count(1) into cnt from dba_objects o where object_name = 'INV_VW_COMPOUNDS' 
and owner = '&&schemaName' and object_type = 'MATERIALIZED VIEW';
if cnt >0 then 
begin
/* Oracle does not make it easy to reliably determine if a materialized view is 
created with the PRIMARY KEY clause or not.  So we're going to 
assume it is created with PRIMARY KEY, but if that fails, we'll try again without it. */
  execute immediate 'ALTER MATERIALIZED VIEW &&schemaName..INV_VW_COMPOUNDS REFRESH FAST ON DEMAND START WITH sysdate NEXT sysdate + ((1/24/3600)*120) WITH PRIMARY KEY';
exception
  when others
 -- no_primary_key or no_primary_key1 or no_primary_key2 
  then
    execute immediate 'ALTER MATERIALIZED VIEW &&schemaName..INV_VW_COMPOUNDS REFRESH FAST ON DEMAND START WITH sysdate NEXT sysdate + ((1/24/3600)*120)';
end;
end if;
end;
/

-- changed refresh fast on commit to refresh fast on demand
declare
  no_primary_key exception;
  PRAGMA EXCEPTION_INIT (no_primary_key, -12014);
  no_primary_key1 exception;
  PRAGMA EXCEPTION_INIT (no_primary_key1, -12024);
  no_primary_key2 exception;
  PRAGMA EXCEPTION_INIT (no_primary_key2, -12016);
cnt number;
begin
select count(1) into cnt from dba_objects o where object_name = 'INV_VW_REG_BATCHES' 
and owner = '&&schemaName' and object_type = 'MATERIALIZED VIEW';
if cnt >0 then 
begin
/* Oracle does not make it easy to reliably determine if a materialized view is 
created with the PRIMARY KEY clause or not.  So we're going to 
assume it is created with PRIMARY KEY, but if that fails, we'll try again without it. */
  execute immediate 'ALTER MATERIALIZED VIEW &&schemaName..INV_VW_REG_BATCHES 
 REFRESH FAST ON DEMAND START WITH sysdate NEXT sysdate + ((1/24/3600)*120)
 WITH PRIMARY KEY';
exception
  when others
  --no_primary_key or no_primary_key1 or no_primary_key2 
  then
    execute immediate 'ALTER MATERIALIZED VIEW &&schemaName..INV_VW_REG_BATCHES 
 REFRESH FAST ON DEMAND START WITH sysdate NEXT sysdate + ((1/24/3600)*120)';
end;
end if;
end;
/

UPDATE &&schemaName..Globals
	SET Value = '&&CurrentPatch' 
	WHERE UPPER(ID) = 'VERSION_SCHEMA';
UPDATE &&schemaName..Globals
	SET Value = '&&CurrentPatch' 
	WHERE UPPER(ID) = 'VERSION_APP';
COMMIT;

prompt **** Patch &&CurrentPatch Applied ****

COL setNextPatch NEW_VALUE setNextPatch NOPRINT
SELECT	CASE
		WHEN  '&&toVersion'='&&CurrentPatch'
		THEN  'Patches\stop.sql'
		ELSE  '"Patches\Patch &&nextPatch\patch.sql"'
	END	AS setNextPatch 
FROM	DUAL;
 
prompt ****&&setNextPatch ***






