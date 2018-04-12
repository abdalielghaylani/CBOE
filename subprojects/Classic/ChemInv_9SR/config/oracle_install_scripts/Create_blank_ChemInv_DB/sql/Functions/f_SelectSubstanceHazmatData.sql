
CREATE OR REPLACE FUNCTION "&&SchemaName"."SELECTSUBSTANCEHAZMATDATA"
  (pCompoundID IN inv_compounds.compound_id%TYPE,
   pEHSGroup1 OUT INV_EHS_CAS_Substance.EHS_Group_1%TYPE,
   pEHSGroup2 OUT INV_EHS_CAS_Substance.EHS_Group_2%TYPE,
   pEHSGroup3 OUT INV_EHS_CAS_Substance.EHS_Group_3%TYPE,
   pHealth OUT INV_EHS_CAS_Substance.Health%TYPE,
   pFlammability OUT INV_EHS_CAS_Substance.Flammability%TYPE,
   pReactivity OUT INV_EHS_CAS_Substance.Reactivity%TYPE,
   pIsSensitizer OUT INV_EHS_CAS_Substance.Is_Sensitizer%TYPE,
   pIsRefrigerated OUT INV_EHS_CAS_Substance.Is_Refrigerated%TYPE,
   pPackingGroup OUT INV_EHS_CAS_Substance.Packing_Group%TYPE,
   pUNNumber OUT INV_EHS_CAS_Substance.UN_Number%TYPE,
   pIsOSHACarcinogen OUT INV_EHS_Substances.is_OSHA_Carcinogen%TYPE,
   pACGIHCarcinogenCategory OUT INV_EHS_Substances.ACGIH_Carcinogen_Category%TYPE,
   pIARCCarcinogen OUT INV_EHS_CAS_Substance.IARC_Carcinogen%TYPE,
   pEUCarcinogen OUT INV_EHS_CAS_Substance.EU_Carcinogen%TYPE,
   pIsDefaultSource OUT NUMBER)
RETURN NUMBER IS
BEGIN
  pIsDefaultSource := 1;	
  SELECT HazMat.EHS_Group_1, 
         HazMat.EHS_Group_2, 
         HazMat.EHS_Group_3, 
         HazMat.Health,
         HazMat.Flammability,
         HazMat.Reactivity,
         HazMat.Is_Sensitizer, 
         HazMat.Is_Refrigerated,
         HazMat.Packing_Group, 
         Hazmat.UN_Number,
         NVL(Managed.is_OSHA_Carcinogen, HazMat.Is_Osha_Carcinogen),
         NVL(Managed.ACGIH_Carcinogen_Category, HazMat.Acgih_Carcinogen_Category),
         HazMat.IARC_Carcinogen, 
         HazMat.EU_Carcinogen 
  INTO   pEHSGroup1, pEHSGroup2, pEHSGroup3,
         pHealth, pFlammability, pReactivity,
         pIsSensitizer, pIsRefrigerated, 
         pPackingGroup, pUNNumber,
         pIsOSHACarcinogen, pACGIHCarcinogenCategory,
         pIARCCarcinogen, pEUCarcinogen
  FROM   inv_compounds, INV_EHS_CAS_Substance HazMat, 
         inv_EHS_Substances Managed
  WHERE  
         inv_compounds.compound_id = pCompoundID AND
         HazMat.CAS_Internal (+) = TRANSLATE(UPPER(inv_compounds.CAS), 
                                             Constants.cCASTranslation1, Constants.cCASTranslation2) AND
         Managed.CAS_Internal (+) = TRANSLATE(UPPER(inv_compounds.CAS), 
                                              Constants.cCASTranslation1, Constants.cCASTranslation2);

  -- Return whether any data was found.
  IF (pEHSGroup1 IS NULL AND pEHSGroup2 IS NULL AND pEHSGroup3 IS NULL AND
      pHealth IS NULL AND pFlammability IS NULL AND pReactivity IS NULL AND
      pIsSensitizer IS NULL AND pIsRefrigerated IS NULL AND
      pPackingGroup IS NULL AND pUNNumber IS NULL AND
      pIsOSHACarcinogen IS NULL AND pACGIHCarcinogenCategory IS NULL) THEN
     RETURN 0;
  ELSE
     RETURN 1;
  END IF;
END "SELECTSUBSTANCEHAZMATDATA";
/
show errors;
