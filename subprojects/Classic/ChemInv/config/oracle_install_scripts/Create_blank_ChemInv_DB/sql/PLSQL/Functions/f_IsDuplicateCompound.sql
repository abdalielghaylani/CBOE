CREATE OR REPLACE  FUNCTION "&&SchemaName"."ISDUPLICATECOMPOUND"  (
    pCompoundID IN inv_compounds.compound_id%type)
return integer
IS
CompoundID integer;
BEGIN
  SELECT compound_id INTO CompoundID from inv_compounds
  WHERE compound_id = pCompoundID
  AND conflicting_fields IS NOT NULL;

  RETURN 1;
exception
when no_data_found then
  RETURN 0;
END IsDuplicateCompound;
/
show errors;
