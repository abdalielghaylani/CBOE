-- This statement will assign 'Blank' to the variable RegRLSScript if the system
-- has Reg RLS.  This is determined by looking for the existence of the PEOPLEPROJECT_RLL_FUNCTION
-- function.  The dummy_table is just there so we can outer join something to dba_objects and
-- thus always get a single record back.
Connect &&InstallUser/&&sysPass@&&serverName

select decode(dba_objects.object_name,null,'Blank','&RegRLSScript.') as rls_col
from (select upper('&&regSchemaName.') as owner from dual) dummy_table
left outer join dba_objects 
on dba_objects.owner = dummy_table.owner
and dba_objects.object_type = 'FUNCTION'
and dba_objects.object_name = 'PEOPLEPROJECT_RLL_FUNCTION';

Connect &&schemaName/&&schemaPass@&&serverName