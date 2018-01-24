--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
prompt 
prompt "Altering &&schemaName for supporting oracle12c"...
prompt 

SET SERVEROUT ON

ALTER USER &&schemaName QUOTA UNLIMITED ON &&tableSpaceName;
ALTER USER &&schemaName QUOTA UNLIMITED ON &&indexTableSpaceName;
ALTER USER &&schemaName QUOTA UNLIMITED ON &&lobsTableSpaceName;
