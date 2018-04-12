@@globals.sql
@@fix_orphan_supervisors.sql

--grant docmgr privs on reg
--this will of course throw an error if REG does not exist.
--in that case the error is ok
Connect &&InstallUser/&&sysPass@&&serverName

grant select on REGDB.VW_REGISTRYNUMBER to &&schemaName;