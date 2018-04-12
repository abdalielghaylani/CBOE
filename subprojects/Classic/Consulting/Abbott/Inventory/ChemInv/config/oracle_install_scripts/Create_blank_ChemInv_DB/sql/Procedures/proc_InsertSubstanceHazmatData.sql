CREATE OR REPLACE PROCEDURE "&&SchemaName"."INSERTSUBSTANCEHAZMATDATA"
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
   pIsOSHACarcinogen IN INV_EHS_CAS_Substance.is_OSHA_Carcinogen%TYPE,
   pACGIHCarcinogenCategory IN INV_EHS_CAS_Substance.ACGIH_Carcinogen_Category%TYPE,
   pIARCCarcinogen IN INV_EHS_CAS_Substance.IARC_Carcinogen%TYPE,
   pEUCarcinogen IN INV_EHS_CAS_Substance.EU_Carcinogen%TYPE) IS
BEGIN
  INSERT INTO INV_EHS_CAS_Substance 
    (EHS_Cas_Substance_ID, 
     substance_Name, CAS,
     EHS_Group_1, EHS_Group_2, EHS_Group_3,
     Health, Flammability, Reactivity,
     Is_Sensitizer, Is_Refrigerated,
     packing_Group, UN_Number,
     IARC_Carcinogen, EU_Carcinogen,
     Is_OSHA_Carcinogen, ACGIH_Carcinogen_Category)
  SELECT SEQ_INV_EHS_CAS_Substance.NEXTVAL, 
         inv_compounds.substance_name, inv_compounds.CAS,
         pEHSGroup1, pEHSGroup2, pEHSGroup3,
         pHealth, pFlammability, pReactivity,
         pIsSensitizer, pIsRefrigerated,
         pPackingGroup, pUNNumber,
         pIARCCarcinogen, pEUCarcinogen,
         pIsOshaCarcinogen, pACGIHCarcinogenCategory
  FROM   inv_compounds
  WHERE  inv_compounds.compound_id = pCompoundID;
END "INSERTSUBSTANCEHAZMATDATA";
/
show errors;
