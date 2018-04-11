--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

--####################################
-- Page Control Settings required XMLs
--####################################

-- Updating Registration Configuration
DECLARE
	V_FOUND NUMBER(1) := 0;
BEGIN
	SELECT COUNT(1) INTO V_FOUND FROM &&securitySchemaName..COECONFIGURATION WHERE LOWER(DESCRIPTION)='registration';
	
	IF V_FOUND > 0 THEN
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','MISC','AppPageTitle','CambridgeSoft Registration Enterprise', 'Defines application page tile.', 'TEXT',NULL, 'False');


	END IF;
End;
/