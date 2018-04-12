create or replace function
           &&schemaName..getRegAltID(p_regid in integer)
 return varchar2 DETERMINISTIC
is
  Result varchar2(1000);
begin

		select id into Result from (
     SELECT ci.id, row_number() over (partition by ci.type order by ci.type) as rn 
		FROM &&REGSchemaName..vw_compound_identifier ci
		WHERE ci.type in (3,4,5)
					AND ci.id is not null
					AND ci.regid = p_regid )
		where rn = 1;

  return(Result);
end;
/
show errors