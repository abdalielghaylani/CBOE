CREATE OR REPLACE PROCEDURE "&&SchemaName"."UPDATESUBSTANCEHAZMATDATA"
  (pCompoundID IN inv_compounds.compound_id%TYPE,
   pEHSGroup1 IN INV_EHS_CAS_Substance.EHS_Group_1%TYPE,
   pEHSGroup2 IN INV_EHS_CAS_Substance.EHS_Group_2%TYPE,
   pEHSGroup3 IN INV_EHS_CAS_Substance.EHS_Group_3%TYPE,
   pHealth IN INV_EHS_CAS_Substance.Health%TYPE,
   pFlammability IN INV_EHS_CAS_Substance.Flammability%TYPE,
   pReactivity IN INV_EHS_CAS_Substance.Reactivity%TYPE,
   pIsSensitizer IN INV_EHS_CAS_Substance.Is_Sensitizer%TYPE,
   pIsRefrigerated IN INV_EHS_CAS_Substance.Is_Refrigerated%TYPE,
   pPackingGroup IN INV_EHS_CAS_Substance.Packing_Group%TYPE,
   pUNNumber IN INV_EHS_CAS_Substance.UN_Number%TYPE,
   pIsOSHACarcinogen IN INV_EHS_Substances.is_OSHA_Carcinogen%TYPE,
   pACGIHCarcinogenCategory IN INV_EHS_Substances.ACGIH_Carcinogen_Category%TYPE,
   pIARCCarcinogen IN INV_EHS_CAS_Substance.IARC_Carcinogen%TYPE,
   pEUCarcinogen IN INV_EHS_CAS_Substance.EU_Carcinogen%TYPE) IS
BEGIN
  UPDATE INV_EHS_CAS_Substance 
  SET EHS_Group_1 = pEHSGroup1, 
      EHS_Group_2 = pEHSGroup2, 
      EHS_Group_3 = pEHSGroup3,
      Health = pHealth, 
      Flammability = pFlammability, 
      Reactivity = pReactivity,
      Is_Sensitizer = pIsSensitizer, 
      Is_Refrigerated = pIsRefrigerated,
      packing_Group = pPackingGroup, 
      UN_Number = pUNNumber,
      is_OSHA_Carcinogen = pIsOSHACarcinogen, 
      --ACGIH_Carcinogen_Category = pACGIHCarcinogenCategory,
      IARC_Carcinogen = pIARCCarcinogen, 
      EU_Carcinogen = pEUCarcinogen
   WHERE (cas) =
         (SELECT cas
          FROM   inv_compounds
          WHERE  compound_id = pCompoundID);
END "UPDATESUBSTANCEHAZMATDATA";
/
show errors;

