
Connect &&schemaName/&&schemaPass@&&serverName
@@truncate_tables.sql

host imp &&InstallUser/&&sysPass@&&serverName file=&&dmpFile full=Y ignore=y log=import_of_data.log


@@cartindex.sql


prompt gathering statistics for D3...
 
Connect &&InstallUser/&&sysPass@&&serverName
begin
	dbms_stats.gather_schema_stats(ownname=> 'CSCARTRIDGE' , estimate_percent=> 50 , cascade=> TRUE );
end;
/

prompt gathering statistics for CSCARTRIDGE...

begin
	dbms_stats.gather_schema_stats(ownname=> 'D3DATA' , estimate_percent=> 50 , cascade=> TRUE );
end;
/