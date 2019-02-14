--Copyright 1998-2018 CambridgeSoft Corporation. All rights reserved

@"Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 

Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Alter tables in &&schemaName'
prompt '#########################################################'

alter table &&schemaName..INV_LOCATION_TYPES modify(LOCATION_TYPE_ID NUMBER(9,0));
alter table &&schemaName..INV_COMPOUNDS modify(LOCATION_TYPE_ID_FK NUMBER(9,0));
alter table &&schemaName..INV_CONTAINERS modify(LOCATION_TYPE_ID_FK NUMBER(9,0));

prompt '#########################################################'
prompt 'Providing additional grants to &&schemaName...'
prompt '#########################################################'


prompt '#########################################################'
prompt 'Updating Functions, Procedures and Packages...'
prompt '#########################################################'


prompt '#########################################################'
prompt 'Providing additional grants to &&schemaName...'
prompt '#########################################################'


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
@&&setNextPatch 






