CREATE OR REPLACE  FUNCTION "&&SchemaName"."GETCOMPOUNDID"(
    pFieldName in varchar2,
     pFieldValue in varchar2)
return inv_compounds.compound_id%Type
IS
CompoundID integer;
my_sql varchar2(2000);
BEGIN
  
  my_sql := 'SELECT Compound_ID FROM Inv_Compounds 
             WHERE conflicting_fields is null AND Upper(' || pfieldName || ') = :value';
  dbms_output.put_line(my_sql);
  
  EXECUTE IMMEDIATE
    my_sql   
  INTO 
    CompoundID
  USING 
    Upper(pFieldValue)
  ;  
  RETURN CompoundID;
exception
when no_data_found then
  RETURN 0;
END GetCompoundID;
/
show errors;
