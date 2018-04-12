--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 

--#######################################################################
-- Adding permission in COEDB
--####################################################################### 

prompt *******************************
Connect &&securitySchemaName/&&securitySchemaPass@&&serverName
-- Updating OBJECT_PRIVILEGES table
@"Patches\Patch &&currentpatch\Data\OBJECT_PRIVILEGES.sql"
@"Patches\Patch &&currentpatch\Data\inv_report_type.sql"
@"Patches\Patch &&currentpatch\Data\inv_reports.sql"

prompt *******************************
Connect &&schemaName/&&schemaPass@&&serverName


-- Create objects
prompt '#########################################################'
prompt 'Create Tables...'
prompt '#########################################################'
@"Patches\Patch &&currentpatch\Create\Inv_Batch_Type.sql"

GRANT SELECT ON &&SchemaName..INV_BATCH_TYPE TO INV_BROWSER;
GRANT SELECT ON &&SchemaName..INV_BATCH_TYPE TO INV_CHEMIST;
GRANT SELECT ON &&SchemaName..INV_BATCH_TYPE TO INV_RECEIVING;
GRANT SELECT ON &&SchemaName..INV_BATCH_TYPE TO INV_FINANCE;
GRANT SELECT ON &&SchemaName..INV_BATCH_TYPE TO INV_REGISTRAR;
GRANT SELECT ON &&SchemaName..INV_BATCH_TYPE TO INV_ADMIN;


-- Updating other objects
prompt '#########################################################'
prompt 'Alter Tables...'
prompt '#########################################################'
@"Patches\Patch &&currentpatch\Alter\Alter_Inv_Containers.sql"
@"Patches\Patch &&currentpatch\Alter\Inv_Container_Batch_Fields.sql"
@"Patches\Patch &&currentpatch\Alter\Inv_Container_Batchs.sql"
@"Patches\Patch &&currentpatch\Alter\Alter_Inv_requests.sql"
@"Patches\Patch &&currentpatch\Alter\Inv_VW_Reg_Batches.sql"


prompt '#########################################################'
prompt 'Updating Functions and Packages...'
prompt '#########################################################'

@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_GUIUtils_def.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_GUIUtils_Body.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Batch_def.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Batch_body.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Requests_def.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Requests_body.sql"
@"Patches\Patch &&currentpatch\PLSQL\Procedures\proc_UpdateContainerBatches.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_UpdateAllContainerFields.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_UpdateContainerQty.sql"

GRANT EXECUTE ON &&SchemaName..UPDATECONTAINERQTY TO INV_BROWSER;
GRANT EXECUTE ON &&SchemaName..UPDATECONTAINERQTY TO INV_CHEMIST;
GRANT EXECUTE ON &&SchemaName..UPDATECONTAINERQTY TO INV_RECEIVING;
GRANT EXECUTE ON &&SchemaName..UPDATECONTAINERQTY TO INV_FINANCE;
GRANT EXECUTE ON &&SchemaName..UPDATECONTAINERQTY TO INV_REGISTRAR;
GRANT EXECUTE ON &&SchemaName..UPDATECONTAINERQTY TO INV_ADMIN;

prompt '#########################################################'
prompt 'Inserting Table Data...'
prompt '#########################################################'
@"Patches\Patch &&currentpatch\Data\INV_BATCH_TYPE.sql"
@"Patches\Patch &&currentpatch\Data\Update_Inv_Container_Batch_Fields.sql"

@"Patches\Patch &&currentpatch\PLSQL\RecompilePLSQL.sql"

prompt 
set define on

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






