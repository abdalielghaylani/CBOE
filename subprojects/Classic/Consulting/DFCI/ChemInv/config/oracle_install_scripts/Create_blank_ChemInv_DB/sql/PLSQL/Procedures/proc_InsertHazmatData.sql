CREATE OR REPLACE PROCEDURE "&&SchemaName"."INSERTHAZMATDATA"
  (pContainerID IN inv_containers.container_id%TYPE,
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
  INSERT INTO INV_EHS_CatNum_Substance 
    (EHS_CatNum_Substance_ID, 
     supplier_id_fk, supplier_catnum,
     substance_Name, CAS,
     EHS_Group_1, EHS_Group_2, EHS_Group_3,
     Health, Flammability, Reactivity,
     Is_Sensitizer, Is_Refrigerated,
     packing_Group, UN_Number,
     is_OSHA_Carcinogen, ACGIH_Carcinogen_Category,
     IARC_Carcinogen, EU_Carcinogen)
  SELECT SEQ_INV_EHS_CatNum_Substance.NEXTVAL, 
         inv_containers.supplier_id_fk, inv_containers.supplier_catnum,
         inv_compounds.substance_name, inv_compounds.CAS,
         pEHSGroup1, pEHSGroup2, pEHSGroup3,
         pHealth, pFlammability, pReactivity,
         pIsSensitizer, pIsRefrigerated,
         pPackingGroup, pUNNumber,
         pIsOSHACarcinogen, pACGIHCarcinogenCategory,
         pIARCCarcinogen, pEUCarcinogen
  FROM   inv_containers, inv_compounds
  WHERE  inv_containers.container_id = pContainerID AND
         inv_compounds.compound_id = inv_containers.compound_id_fk;
END "INSERTHAZMATDATA";
/
show errors;
