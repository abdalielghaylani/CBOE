Connect &&InstallUser/&&sysPass@&&serverName
exec dbms_rls.add_policy( 'CHEMINVDB2','INV_VW_COMPOUNDS','INV_VW_COMPOUNDS_POLICY','REGDB','PeopleProject_RLL_Function','select');
Connect &&schemaName/&&schemaPass@&&serverName
