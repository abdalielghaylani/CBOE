create or replace function 
&&schemaName..getRegComponentIdentifier(p_regid in integer, p_identifierName varchar2) 
return varchar2 DETERMINISTIC
is 
 Result varchar2(2000):=null;
begin
  
   select substr(wm_concat(ci.Value),1, 2000) into Result
          from &&regSchemaName..VW_COMPOUND_IDENTIFIER ci, 
               &&regSchemaName..vw_compound c, 
               &&regSchemaName..vw_mixture m,
               &&regSchemaName..vw_mixture_component mc, 
               &&regSchemaName..VW_IDENTIFIERTYPE it 
           where ci.regid = c.RegID
           and m.mixtureid = mc.mixtureid
           and mc.compoundid = c.compoundid
           and ci.type = it.id
           and it.name = p_identifierName
           and m.regid = p_regid;
  if result is null then
     return(' ');
  else    
     return (Result);
  end if;      
end ;
/
