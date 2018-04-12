CREATE OR REPLACE FUNCTION "SELECTHAZMATDATA"
  (pContainerID IN inv_containers.container_id%TYPE,
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
CURSOR O_RS IS 
SELECT NVL(CatNum1.EHS_Group_1, NVL(CatNum2.EHS_Group_1, HazMat.EHS_Group_1)),
         NVL(CatNum1.EHS_Group_2, NVL(CatNum2.EHS_Group_2, HazMat.EHS_Group_2)),
         NVL(CatNum1.EHS_Group_3, NVL(CatNum2.EHS_Group_3, HazMat.EHS_Group_3)),
         NVL(CatNum1.Health, NVL(CatNum2.Health, HazMat.Health)),
         NVL(CatNum1.Flammability, NVL(CatNum2.Flammability, HazMat.Flammability)),
         NVL(CatNum1.Reactivity, NVL(CatNum2.Reactivity, HazMat.Reactivity)),
         NVL(CatNum1.Is_Sensitizer, NVL(CatNum2.Is_Sensitizer, HazMat.Is_Sensitizer)),
         NVL(CatNum1.Is_Refrigerated, NVL(CatNum2.Is_Refrigerated, HazMat.Is_Refrigerated)),
         NVL(CatNum1.Packing_Group, NVL(CatNum2.Packing_Group, HazMat.Packing_Group)),
         NVL(CatNum1.UN_Number, NVL(CatNum2.UN_Number, Hazmat.UN_Number)),
         NVL(CatNum1.is_OSHA_Carcinogen, NVL(CatNum2.is_OSHA_Carcinogen, NVL(Managed.is_OSHA_Carcinogen,HazMat.Is_Osha_Carcinogen))),
         NVL(CatNum1.ACGIH_Carcinogen_Category, NVL(CatNum2.ACGIH_Carcinogen_Category, NVL(Managed.ACGIH_Carcinogen_Category,HazMat.ACGIH_Carcinogen_Category))),
         NVL(CatNum1.IARC_Carcinogen, NVL(CatNum2.IARC_Carcinogen, HazMat.IARC_Carcinogen)),
         NVL(CatNum1.EU_Carcinogen, NVL(CatNum2.EU_Carcinogen, HazMat.EU_Carcinogen)),
         DECODE(CatNum1.supplier_catnum_internal, NULL, DECODE(CatNum2.CAS_Internal, NULL, 2, 1), 0) as IsDefaultSource
 FROM   inv_compounds, INV_EHS_CAS_Substance HazMat,
         inv_EHS_Substances Managed, inv_containers,
         inv_EHS_CatNum_Substance CatNum1, inv_EHS_CatNum_Substance CatNum2
 WHERE  inv_containers.container_id = pContainerID AND
         inv_compounds.compound_id (+) = inv_containers.compound_id_fk AND
         HazMat.CAS_Internal (+) = TRANSLATE(UPPER(inv_compounds.CAS),
                                             Constants.cCASTranslation1, Constants.cCASTranslation2) AND
         Managed.CAS_Internal (+) = TRANSLATE(UPPER(inv_compounds.CAS),
                                              Constants.cCASTranslation1, Constants.cCASTranslation2) AND
         CatNum1.supplier_id_fk (+) = inv_containers.supplier_id_fk AND
         CatNum1.supplier_catnum_internal (+) = TRANSLATE(inv_containers.supplier_catnum,
                                                         Constants.cCatNumTranslation1, Constants.cCatNumTranslation2) AND
         CatNum2.CAS_Internal (+) = TRANSLATE(UPPER(inv_compounds.CAS),
                                              Constants.cCASTranslation1, Constants.cCASTranslation2)
  Order by IsDefaultSource;

BEGIN
  OPEN O_RS;
  IF O_RS%NOTFOUND THEN
	RETURN 0;
  ELSE
  -- we need to use the first row of the selection.
   FETCH O_RS INTO pEHSGroup1, pEHSGroup2, pEHSGroup3,
         pHealth, pFlammability, pReactivity,
         pIsSensitizer, pIsRefrigerated,
         pPackingGroup, pUNNumber,
         pIsOSHACarcinogen, pACGIHCarcinogenCategory,
         pIARCCarcinogen, pEUCarcinogen, pIsDefaultSource;
  END IF; 

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
END "SELECTHAZMATDATA";
/
show errors;