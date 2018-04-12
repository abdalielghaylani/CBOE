create or replace function 
&&schemaName..getPickListValue(p_picklistID in integer)
 return varchar2 DETERMINISTIC
is
  Result varchar2(1000);
begin
  
  select picklistvalue into Result
    from &&regSchemaName..Picklist l 
    where l.id = p_picklistID;
 
  return(Result);
end getPickListValue;
/