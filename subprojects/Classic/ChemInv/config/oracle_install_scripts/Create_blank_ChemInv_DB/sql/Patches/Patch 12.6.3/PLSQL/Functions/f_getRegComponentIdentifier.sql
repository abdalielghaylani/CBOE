 CREATE OR REPLACE function
           &&schemaName..getRegComponentIdentifier(p_mixtureid in number, p_identifierName varchar2)
return varchar2 DETERMINISTIC
is
 Result varchar2(2000):=null;
begin

   select substr(wm_concat(ci.Value),1, 2000) into Result
          from &&REGSchemaName..VW_COMPOUND_IDENTIFIER ci,
               &&REGSchemaName..vw_compound c,
               &&REGSchemaName..vw_mixture_component mc,
               &&REGSchemaName..VW_IDENTIFIERTYPE it
           where ci.regid = c.RegID
           and p_mixtureid = mc.mixtureid
           and mc.compoundid = c.compoundid
           and ci.type = it.id
           and it.name = p_identifierName;
          
  if result is null then
     Result :=' ';
  end if;
  return (Result);
end;
/
show errors
