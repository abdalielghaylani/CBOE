--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved
prompt 
prompt 
prompt *** WARNING: The Patch &&currentPatch was applied but the CS_Security schema wasn't modified. ***
prompt 
prompt

BEGIN
raise_application_error(-20000, 'Patching interrrupted by the user.');

END;
/
exit 

