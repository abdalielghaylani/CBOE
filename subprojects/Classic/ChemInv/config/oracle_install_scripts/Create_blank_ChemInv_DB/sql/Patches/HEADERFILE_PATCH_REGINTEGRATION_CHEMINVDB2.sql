--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved


spool Patches\log_Patch_REGINTEGRATION.txt

	@Parameters.sql 
	@Patches\Prompts.sql

Connect &&schemaName/&&schemaPass@&&serverName

	VAR Begin VARCHAR2(20);
	VAR End VARCHAR2(20);
	VAR Elapsed VARCHAR2(100);
	exec :Begin:=to_char(systimestamp,'HH:MI:SS.FF4');

	@"Patches\RegistrationIntegration\patch.sql"

	/
	

spool off

exit


	