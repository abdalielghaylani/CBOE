--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

--#####################################
-- Updating Registration Configuration
--#####################################

BEGIN 

	-- Update the 'application name' configuration value
	UPDATE &&securitySchemaName..coeconfiguration c
    	SET c.configurationxml = (
      		SELECT  XmlType(replace( cr.configurationxml.GetClobVal(), '11.0.4', '12.1' ))
		FROM &&securitySchemaName..coeconfiguration cr
      		WHERE cr.Description = c.Description )
    	WHERE c.Description = 'Registration';

    COMMIT;
END;
/
