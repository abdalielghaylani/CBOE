create or replace function
&&schemaName..getUserIDFromPersonID(p_personID in integer)
 return varchar2 DETERMINISTIC
is
  Result varchar2(1000);
begin

  select user_id into Result
    from &&securitySchemaName..People p
    where p.person_id = p_personID;

  return(Result);
end getUserIDFromPersonID;
/