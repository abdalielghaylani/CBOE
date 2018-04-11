--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

--####################################
-- Updating Registration Configuration
--####################################

DECLARE
	V_FOUND NUMBER(1) := 0;
BEGIN
	SELECT COUNT(1) INTO V_FOUND FROM &&securitySchemaName..COECONFIGURATION WHERE LOWER(DESCRIPTION)='registration';
	
	IF V_FOUND > 0 THEN	  &&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','FragmentSortField','Description', 'Set the value to sort fragments in ascending order', 'PICKLIST','Description|Code', 'False');
	END IF;
END;
/
-- Update Page Title
DECLARE
	V_FOUND NUMBER(1) := 0;
BEGIN
	SELECT COUNT(1) INTO V_FOUND FROM &&securitySchemaName..COECONFIGURATION WHERE LOWER(DESCRIPTION)='registration';
	
	IF V_FOUND > 0 THEN
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','MISC','AppPageTitle','CambridgeSoft Registration Enterprise 12.3.2', 'Defines application page tile.', 'TEXT',NULL, 'False');
	END IF;
END;
/
