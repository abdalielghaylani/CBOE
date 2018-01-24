--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

--#####################################
-- Updating DataView Manager Configuration 
-- Add Apply_Indexing configuration to index fields for default query field 
--Showing dropdown list for Apply_Indexing in DVManger and PageControlsManager in MISC setting.
--#####################################

BEGIN
	&&schemaName..ConfigurationManager.CreateOrUpdateParameter('Manager','DVManager','Apply_Indexing','DISABLE','Enable or disable the offer to index fields for default query field', 'PICKLIST', 'ENABLE|DISABLE', NULL);
	COMMIT;
	&&schemaName..ConfigurationManager.CreateOrUpdateParameter('Manager','MISC','PageControlsManager','DISABLE','', 'PICKLIST', 'ENABLE|DISABLE', NULL);
	COMMIT;
END;
/






