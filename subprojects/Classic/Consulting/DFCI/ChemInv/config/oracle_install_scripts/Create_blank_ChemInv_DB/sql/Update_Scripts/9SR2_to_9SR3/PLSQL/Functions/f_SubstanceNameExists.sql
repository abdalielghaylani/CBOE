-- Create procedure/function SUBSTANCENAMEEXISTS.
CREATE OR REPLACE  FUNCTION "&&SchemaName"."SUBSTANCENAMEEXISTS"    (
    pSubstanceName in Inv_Compounds.Substance_Name%Type)
return integer
IS
rowCount integer;
BEGIN
  Select Count(Compound_ID) INTO rowCount FROM Inv_Compounds WHERE Substance_Name = pSubstanceName;
  if rowCount = 0 then
    RETURN 0;
  Else
    RETURN 1;
  End if;
END SubstanceNameExists;
/
show errors;
