
prompt 
prompt "Providing additional grants to &&schemaName for supporting oracle12c"...
prompt 

--RLS
declare
X number;
begin
select count(1) into X from v$version where banner like '%Oracle Database%' and banner like '% 11%';
if X = 0 then
	execute immediate 'GRANT INHERIT PRIVILEGES ON USER &&InstallUser TO &&schemaName';
end if;
end;
/
grant execute on wm_concat to &&schemaName;
