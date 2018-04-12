-- Create procedure/function GETCOMPOUNDIDFROMNAME.
CREATE OR REPLACE  FUNCTION "&&SchemaName"."GETCOMPOUNDIDFROMNAME"   (
    pSubstanceName in Inv_Compounds.Substance_Name%Type)
return integer
IS
CompoundID integer;
BEGIN
  Select Compound_ID INTO CompoundID FROM Inv_Compounds WHERE Substance_Name = pSubstanceName;
  RETURN CompoundID;

exception
when no_data_found then
  RETURN 0;
END GetCompoundIDFromName;
/
show errors;
