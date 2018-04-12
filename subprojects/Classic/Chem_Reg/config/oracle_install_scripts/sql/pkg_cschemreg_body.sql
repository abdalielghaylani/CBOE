create or replace PACKAGE BODY "CSCHEMREG"
IS
Procedure GetPermregFromTempId(
--TempId in Batches.TEMP_ID%Type,
BatchN OUT Batches.BATCH_NUMBER%Type,
RegN OUT reg_numbers.reg_number%type)
IS
Begin
Select b.Batch_Number, r.Reg_number into BatchN, RegN from Batches b , reg_numbers r where r.reg_id = b.reg_internal_id;-- and b.Temp_Id= TempId;
EXCEPTION
  WHEN NO_DATA_FOUND THEN
  dbms_output.put_line('Sorry there is No Data associated with the Temp Id you have given. Please search again using a different Temp Id!');
  raise;
End GetPermregFromTempId;

Function strreplace(str varchar2)
  return varchar2
AS
  str_temp varchar2(4000);

BEGIN
--   RETURN REPLACE (TRANSLATE (str, '~`!@#$%^&*()_+={}[]|\:;''",.<>?', '~'), '~','''');
RETURN REPLACE (str, '''', '''''');
END strreplace;

Procedure TranslateIds(
paramValue in out varchar2,
paramName in varchar2
)
IS
BatchesSql varchar (2000);
Begin
IF paramName='SCIENTIST_ID' THEN
Select PERSON_ID into paramValue from CS_Security.People where USER_ID=paramValue;
End IF;
IF paramName='PROJECT_ID' THEN
Select PROJECT_INTERNAL_ID into paramValue from PROJECTS where PROJECT_NAME=paramValue;
End IF;
IF paramName='BATCH_PROJECT_ID' THEN
Select Batch_Project_Id into paramValue from Batch_Projects where Project_Name=paramValue;
End IF;
IF paramName='COMPOUND_TYPE' THEN
Select COMPOUND_TYPE into paramValue from COMPOUND_TYPE where Description=paramValue;
End IF;
IF paramName='NOTEBOOK_INTERNAL_ID' THEN
Select NOTEBOOK_NUMBER into paramValue from NOTEBOOKS where NOTEBOOK_NAME=paramValue;
End IF;
END TranslateIds;
Procedure UpdateBatch(
pBatchNumber in Batches.BATCH_NUMBER%Type,
pREG_NUMBER in Reg_Numbers.REG_NUMBER%Type,
pSCIENTIST_ID in CS_SECURITY.PEOPLE.USER_ID%Type default null,
nSCIENTIST_ID in Batches.SCIENTIST_ID%Type default null,
pBP in Batches.BP%Type default null,
pMP in Batches.MP%Type default null,
pH1NMR in Batches.H1NMR%Type default null,
pC13NMR in Batches.C13NMR%Type default null,
pMS in Batches.MS%Type default null,
pIR in Batches.IR%Type default null,
pGC in Batches.GC%Type default null,
pPHYSICAL_FORM in Batches.PHYSICAL_FORM%Type default null,
pCOLOR in Batches.COLOR%Type default null,
pFLASHPOINT in Batches.FLASHPOINT%Type default null,
pHPLC in Batches.HPLC%Type default null,
pOPTICAL_ROTATION in Batches.OPTICAL_ROTATION%Type default null,
pREFRACTIVE_INDEX in Batches.REFRACTIVE_INDEX%Type default null,
pSALT_CODE in Salts.SALT_CODE%Type default null,
pSALT_NAME in Batches.SALT_NAME%Type default null,
pSALT_EQUIVALENTS in Batches.SALT_EQUIVALENTS%Type default null,
pSOLVATE_NAME in Batches.SOLVATE_NAME%Type default null,
pSOLVATE_EQUIVALENTS in Batches.SOLVATE_EQUIVALENTS%Type default null,
pSOURCE in Batches.Source%Type default null,
pVENDOR_NAME in Batches.VENDOR_NAME%Type default null,
pVENDOR_ID in Batches.VENDOR_ID%Type default null,
pPERCENT_ACTIVE in Batches.PERCENT_ACTIVE%Type default null,
pAMOUNT_UNITS in Batches.AMOUNT_UNITS%Type default null,
pPURITY in BATCHES.PURITY%Type default null,
pLC_UV_MS in Batches.LC_UV_MS%Type default null,
pCHN_COMBUSTION in Batches.CHN_COMBUSTION%Type default null,
pUV_SPECTRUM in Batches.UV_SPECTRUM%Type default null,
pAPPEARANCE in Batches.APPEARANCE%Type default null,
pLOGD in Batches.LOGD%Type default null,
pSOLUBILITY in Batches.SOLUBILITY%Type default null,
pLAST_MOD_PERSON_ID in Batches.LAST_MOD_PERSON_ID%Type default null,
pAMOUNT in Batches.AMOUNT%Type default null,
pFIELD_1 in Batches.FIELD_1%Type default null,
pFIELD_2 in Batches.FIELD_2%Type default null,
pFIELD_3 in Batches.FIELD_3%Type default null,
pFIELD_4 in Batches.FIELD_4%Type default null,
pFIELD_5 in Batches.FIELD_5%Type default null,
pFIELD_6 in Batches.FIELD_6%Type default null,
pFIELD_7 in Batches.FIELD_7%Type default null,
pFIELD_8 in Batches.FIELD_8%Type default null,
pFIELD_9 in Batches.FIELD_9%Type default null,
pFIELD_10 in Batches.FIELD_10%Type default null,
pLOAD_ID in Batches.LOAD_ID%Type default null,
--pDATETIME_STAMP in Batches.DATETIME_STAMP%Type default null,
pINT_BATCH_FIELD_1 in Batches.INT_BATCH_FIELD_1%Type default null,
pINT_BATCH_FIELD_2 in Batches.INT_BATCH_FIELD_2%Type default null,
pINT_BATCH_FIELD_3 in Batches.INT_BATCH_FIELD_3%Type default null,
pINT_BATCH_FIELD_4 in Batches.INT_BATCH_FIELD_4%Type default null,
pINT_BATCH_FIELD_5 in Batches.INT_BATCH_FIELD_5%Type default null,
pINT_BATCH_FIELD_6 in Batches.INT_BATCH_FIELD_6%Type default null,
pREAL_BATCH_FIELD_1 in Batches.REAL_BATCH_FIELD_1%Type default null,
pREAL_BATCH_FIELD_2 in Batches.REAL_BATCH_FIELD_2%Type default null,
pREAL_BATCH_FIELD_3 in Batches.REAL_BATCH_FIELD_3%Type default null,
pREAL_BATCH_FIELD_4 in Batches.REAL_BATCH_FIELD_4%Type default null,
pREAL_BATCH_FIELD_5 in Batches.REAL_BATCH_FIELD_5%Type default null,
pREAL_BATCH_FIELD_6 in Batches.REAL_BATCH_FIELD_6%Type default null,
--pDATE_BATCH_FIELD_1 in Batches.DATE_BATCH_FIELD_1%Type default null,
pDATE_BATCH_FIELD_1 in varchar2 default null,
pDATE_BATCH_FIELD_2 in varchar2 default null,
pDATE_BATCH_FIELD_3 in varchar2 default null,
pDATE_BATCH_FIELD_4 in varchar2 default null,
pDATE_BATCH_FIELD_5 in varchar2 default null,
pDATE_BATCH_FIELD_6 in varchar2 default null,
pREAL_CMPD_FIELD_1 in Compound_Molecule.REAL_CMPD_FIELD_1%Type default null,
pREAL_CMPD_FIELD_2 in Compound_Molecule.REAL_CMPD_FIELD_2%Type default null,
pREAL_CMPD_FIELD_3 in Compound_Molecule.REAL_CMPD_FIELD_3%Type default null,
pREAL_CMPD_FIELD_4 in Compound_Molecule.REAL_CMPD_FIELD_4%Type default null,
pPRODUCT_TYPE in COMPOUND_MOLECULE.PRODUCT_TYPE%Type default null,
pCHIRAL in COMPOUND_MOLECULE.CHIRAL%Type default null,
pCLOGP in COMPOUND_MOLECULE.CLOGP%Type default null,
pH_BOND_DONORS in COMPOUND_MOLECULE.H_BOND_DONORS%Type default null,
pH_BOND_ACCEPTORS in COMPOUND_MOLECULE.H_BOND_ACCEPTORS%Type default null,
pMW_TEXT in COMPOUND_MOLECULE.MW_TEXT%Type default null,
pMF_TEXT in COMPOUND_MOLECULE.MF_TEXT%Type default null,
pCOMPOUND_TYPE in COMPOUND_TYPE.DESCRIPTION%Type default null,
nCOMPOUND_TYPE in COMPOUND_MOLECULE.COMPOUND_TYPE%Type default null,
pSTRUCTURE_COMMENTS_TXT in COMPOUND_MOLECULE.STRUCTURE_COMMENTS_TXT%Type default null,
pTXT_CMPD_FIELD_1 in COMPOUND_MOLECULE.TXT_CMPD_FIELD_1%Type default null,
pTXT_CMPD_FIELD_2 in COMPOUND_MOLECULE.TXT_CMPD_FIELD_2%Type default null,
pTXT_CMPD_FIELD_3 in COMPOUND_MOLECULE.TXT_CMPD_FIELD_3%Type default null,
pTXT_CMPD_FIELD_4 in COMPOUND_MOLECULE.TXT_CMPD_FIELD_4%Type default null,
pINT_CMPD_FIELD_1 in COMPOUND_MOLECULE.INT_CMPD_FIELD_1%Type default null,
pINT_CMPD_FIELD_2 in COMPOUND_MOLECULE.INT_CMPD_FIELD_2%Type default null,
pINT_CMPD_FIELD_3 in COMPOUND_MOLECULE.INT_CMPD_FIELD_3%Type default null,
pINT_CMPD_FIELD_4 in COMPOUND_MOLECULE.INT_CMPD_FIELD_4%Type default null,
pDATE_CMPD_FIELD_1 in varchar2 default null,
pDATE_CMPD_FIELD_2 in varchar2 default null,
pDATE_CMPD_FIELD_3 in varchar2 default null,
pDATE_CMPD_FIELD_4 in varchar2 default null,
pAPPROVED in REG_APPROVED.APPROVED%Type default null,
pSOLVATE_ID in Solvates.SOLVATE_ID%Type default null,
pCompound_PROJECT_ID in PROJECTS.PROJECT_NAME%Type default null,
nCompound_PROJECT_ID in TEMPORARY_STRUCTURES.PROJECT_ID%Type default null,
pAPPROVED_TIME in REG_APPROVED.DATETIME_STAMP%Type default null,
pAPPROVED_DATE in REG_APPROVED.DATETIME_STAMP%Type default null,
pFORMULA_WEIGHT in BATCHES.FORMULA_WEIGHT%Type default null,
pBATCH_PROJECT_ID in Batch_Projects.PROJECT_NAME%Type default null,
nBATCH_PROJECT_ID in Batches.BATCH_PROJECT_ID%Type default null,
fCHEMICAL_NAME in TEMPORARY_STRUCTURES.CHEMICAL_NAME%Type default null,
fCHEM_NAME_AUTOGEN in TEMPORARY_STRUCTURES.CHEM_NAME_AUTOGEN%Type default null,
fSYNONYM_R in TEMPORARY_STRUCTURES.SYNONYM_R%Type default null,
fCAS_NUMBER in TEMPORARY_STRUCTURES.CAS_NUMBER%Type default null,
fRNO_NUMBER in TEMPORARY_STRUCTURES.RNO_NUMBER%Type default null,
fFEMA_GRAS_NUMBER in TEMPORARY_STRUCTURES.FEMA_GRAS_NUMBER%Type default null,
fGROUP_CODE in TEMPORARY_STRUCTURES.GROUP_CODE%Type default null,
pNOTEBOOK_PAGE in BATCHES.NOTEBOOK_PAGE%Type default null,
pNOTEBOOK_TEXT in BATCHES.NOTEBOOK_TEXT%Type default null,
pNOTEBOOK_INTERNAL_ID in NOTEBOOKS.NOTEBOOK_NAME%Type default null,
nNOTEBOOK_INTERNAL_ID in BATCHES.NOTEBOOK_INTERNAL_ID%Type default null,
pProducer in BATCHES.PRODUCER%Type default null,
pLit_Ref in BATCHES.LIT_REF%Type default null,
pBatch_Comment in BATCHES.BATCH_COMMENT%Type default null,
pDateFormat in varchar2,
pSTORAGE_REQ_AND_WARNINGS in BATCHES.STORAGE_REQ_AND_WARNINGS%Type default null,
pPreparation in BATCHES.PREPARATION%Type default null,
pBATCH_INTERNAL_ID out BATCHES.BATCH_INTERNAL_ID%Type
)
IS
pLAST_MOD_DATE Date := SYSDATE;
BatchesSql varchar (2000);
ComMolSql varchar (2000);
ComProjSql varchar (500);
SaltsSql varchar(200);
SolvateSql varchar(200);
BatchProjSql varchar(200);
ComTypeSql varchar(200);
RegAppSql varchar(1000);
AltIdsSql varchar(1000);
RegSql varchar(1000);
sSaltName Batches.SALT_NAME%Type;
sSALT_MW Batches.SALT_MW%Type;
sSaltCode Salts.SALT_CODE%Type;
sSolvateId Solvates.SOLVATE_ID%Type;
sSolvateMW Solvates.SOLVATE_MW%Type;
sSolvateName Solvates.SOLVATE_NAME%Type;
bBatch_Project_Id Batches.BATCH_PROJECT_ID%Type;
cCOMPOUND_TYPE Compound_Molecule.COMPOUND_TYPE%Type;
vRegInternalId Batches.REG_INTERNAL_ID%Type;
vCPDINTERNALID Batches.CPD_INTERNAL_ID%Type;
vRegN Reg_Numbers.REG_NUMBER%Type;
solvate_rec Solvates%ROWTYPE;
solvate_rec_Name Solvates%ROWTYPE;
salts_rec Salts%ROWTYPE;
salts_rec_Name Salts%ROWTYPE;
Batches_Rec Batch_Projects%ROWTYPE;
Compound_Rec Compound_Type%ROWTYPE;
Out_Rec BATCHES%ROWTYPE;
pMolWeight Compound_Molecule.MW2%Type default null;
NOTEBOOK_Rec NOTEBOOKS%ROWTYPE;
Batches_NBSql varchar(1000);
OutSQL varchar(1000);
Alt_rec ALT_IDS%ROWTYPE;
dateSql varchar(500);
BatchesScientistSql varchar(500);
scientist_rec CS_SECURITY.PEOPLE%ROWTYPE;
projSql varchar2(1000);
comProj PROJECTS%ROWTYPE;
OutComSql varchar2(1000);
pSaltMw Compound_Molecule.MW2%Type;
Out_ComRec Compound_Molecule%ROWTYPE;
pSalIntId BATCHES.SALT_INTERNAL_ID%Type;
pSolIntId BATCHES.SOLVATE_ID%Type;
pSalEquivalents BATCHES.SALT_EQUIVALENTS%Type;
pSolEquivalents BATCHES.SOLVATE_EQUIVALENTS%Type;
UBatSql varchar2(1000);
SaltsFlag number default null;
SolvatesFlag number default null;
Begin
pBATCH_INTERNAL_ID := 0;
BatchesSql:= 'Update BATCHES SET ';
IF nSCIENTIST_ID IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'SCIENTIST_ID = '''||STRREPLACE(nSCIENTIST_ID)||''', ';
END IF;
IF pSCIENTIST_ID IS NOT NULL THEN
BatchesScientistSql := 'Select * from CS_Security.People where USER_ID=:pSCIENTIST_ID';
Execute Immediate BatchesScientistSql into scientist_rec using pSCIENTIST_ID;
BatchesSql := BatchesSql  ||  'SCIENTIST_ID = '''||STRREPLACE(scientist_rec.Person_ID)||''', ';
END IF;
IF pBP IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'BP = '''||STRREPLACE(pBP)||''', ';
END IF;
IF pMP IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'MP = '''||STRREPLACE(pMP)||''', ';
END IF;
IF pH1NMR IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'H1NMR = '''||STRREPLACE(pH1NMR)||''', ';
END IF;
IF pC13NMR IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'C13NMR = '''||STRREPLACE(pC13NMR)||''', ';
END IF;
IF pMS IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'MS = '''||STRREPLACE(pMS)||''', ';
END IF;
IF pIR IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'IR = '''||STRREPLACE(pIR)||''', ';
END IF;
IF pGC IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'GC = '''||STRREPLACE(pGC)||''', ';
END IF;
IF pPHYSICAL_FORM IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'PHYSICAL_FORM = '''||STRREPLACE(pPHYSICAL_FORM)||''', ';
END IF;
IF pCOLOR IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'COLOR = '''||STRREPLACE(pCOLOR)||''', ';
END IF;
IF pFLASHPOINT IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'FLASHPOINT = '''||STRREPLACE(pFLASHPOINT)||''', ';
END IF;
IF pHPLC IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'HPLC = '''||STRREPLACE(pHPLC)||''', ';
END IF;
IF pOPTICAL_ROTATION IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'OPTICAL_ROTATION = '''||STRREPLACE(pOPTICAL_ROTATION)||''', ';
END IF;
IF pREFRACTIVE_INDEX IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'REFRACTIVE_INDEX = '''||STRREPLACE(pREFRACTIVE_INDEX)||''', ';
END IF;
IF pSALT_CODE IS NOT NULL THEN
SaltsSql := 'SELECT * from Salts Where Salt_Code = :pSALT_CODE';
Execute Immediate SaltsSql into salts_rec using pSALT_CODE;
BatchesSql := BatchesSql  ||  'SALT_INTERNAL_ID = '''||STRREPLACE(salts_rec.SALT_CODE)||''', ';
BatchesSql := BatchesSql  ||  'SALT_NAME = '''||STRREPLACE(salts_rec.Salt_NAME)||''', ';
BatchesSql := BatchesSql  ||  'SALT_MW = '''||STRREPLACE(salts_rec.SALT_MW)||''', ';
SaltsFlag := 1;
END IF;
IF pSALT_NAME IS NOT NULL THEN
SaltsSql := 'SELECT * from Salts Where Salt_Name = :pSALT_NAME';
Execute Immediate SaltsSql into salts_rec_Name using pSALT_NAME;
BatchesSql := BatchesSql  ||  'SALT_INTERNAL_ID = '''||STRREPLACE(salts_rec_Name.SALT_CODE)||''', ';
BatchesSql := BatchesSql  ||  'SALT_NAME = '''||STRREPLACE(salts_rec_Name.Salt_NAME)||''', ';
BatchesSql := BatchesSql  ||  'SALT_MW = '''||STRREPLACE(salts_rec_Name.SALT_MW)||''', ';
SaltsFlag := 1;
END IF;
IF pSALT_EQUIVALENTS IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'SALT_EQUIVALENTS = '''||STRREPLACE(pSALT_EQUIVALENTS)||''', ';
SaltsFlag := 1;
END IF;
IF pSOLVATE_NAME IS NOT NULL THEN
SolvateSql := 'SELECT * from Solvates Where Solvates.SOLVATE_NAME=:pSOLVATE_NAME';
Execute Immediate SolvateSql into solvate_rec_Name using pSOLVATE_NAME;
BatchesSql := BatchesSql  ||  'SOLVATE_NAME = '''||STRREPLACE(solvate_rec_Name.SOLVATE_NAME)||''', ';
BatchesSql := BatchesSql  ||  'SOLVATE_MW = '''||STRREPLACE(solvate_rec_Name.SOLVATE_MW)||''', ';
BatchesSql := BatchesSql  ||  'SOLVATE_ID = '''||STRREPLACE(solvate_rec_Name.SOLVATE_ID)||''', ';
SaltsFlag := 1;
END IF;
IF pSOLVATE_ID IS NOT NULL THEN
SolvateSql := 'SELECT * from Solvates Where Solvates.SOLVATE_ID=:pSOLVATE_ID';
Execute Immediate SolvateSql into solvate_rec using pSolvate_Id;
BatchesSql := BatchesSql  ||  'SOLVATE_NAME = '''||STRREPLACE(solvate_rec.SOLVATE_NAME)||''', ';
BatchesSql := BatchesSql  ||  'SOLVATE_MW = '''||STRREPLACE(solvate_rec.SOLVATE_MW)||''', ';
BatchesSql := BatchesSql  ||  'SOLVATE_ID = '''||STRREPLACE(solvate_rec.SOLVATE_ID)||''', ';
SaltsFlag := 1;
END IF;
IF pSOLVATE_EQUIVALENTS IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'SOLVATE_EQUIVALENTS = '''||STRREPLACE(pSOLVATE_EQUIVALENTS)||''', ';
SaltsFlag := 1;
END IF;
IF pSource IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'Source = '''||STRREPLACE(pSource)||''', ';
END IF;
IF pVENDOR_NAME IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'VENDOR_NAME = '''||STRREPLACE(pVENDOR_NAME)||''', ';
END IF;
IF pVENDOR_ID IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'VENDOR_ID = '''||STRREPLACE(pVENDOR_ID)||''', ';
END IF;
IF pPERCENT_ACTIVE IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'PERCENT_ACTIVE = '''||STRREPLACE(pPERCENT_ACTIVE)||''', ';
END IF;
IF pAMOUNT_UNITS IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'AMOUNT_UNITS = '''||STRREPLACE(pAMOUNT_UNITS)||''', ';
END IF;
IF pPURITY IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'PURITY = '''||STRREPLACE(pPURITY)||''', ';
END IF;
IF pLC_UV_MS IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'LC_UV_MS = '''||STRREPLACE(pLC_UV_MS)||''', ';
END IF;
IF pCHN_COMBUSTION IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'CHN_COMBUSTION = '''||STRREPLACE(pCHN_COMBUSTION)||''', ';
END IF;
IF pUV_SPECTRUM IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'UV_SPECTRUM = '''||STRREPLACE(pUV_SPECTRUM)||''', ';
END IF;
IF pAPPEARANCE IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'APPEARANCE = '''||STRREPLACE(pAPPEARANCE)||''', ';
END IF;
IF pLOGD IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'LOGD = '''||STRREPLACE(pLOGD)||''', ';
END IF;
IF pSOLUBILITY IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'SOLUBILITY = '''||STRREPLACE(pSOLUBILITY)||''', ';
END IF;
IF pLAST_MOD_PERSON_ID IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'LAST_MOD_PERSON_ID = '''||STRREPLACE(pLAST_MOD_PERSON_ID)||''', ';
END IF;
IF pAMOUNT IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'AMOUNT = '''||STRREPLACE(pAMOUNT)||''', ';
END IF;
IF pFIELD_1 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'FIELD_1 = '''||STRREPLACE(pFIELD_1)||''', ';
END IF;
IF pFIELD_2 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'FIELD_2 = '''||STRREPLACE(pFIELD_2)||''', ';
END IF;
IF pFIELD_3 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'FIELD_3 = '''||STRREPLACE(pFIELD_3)||''', ';
END IF;
IF pFIELD_4 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'FIELD_4 = '''||STRREPLACE(pFIELD_4)||''', ';
END IF;
IF pFIELD_5 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'FIELD_5 = '''||STRREPLACE(pFIELD_5)||''', ';
END IF;
IF pFIELD_6 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'FIELD_6 = '''||STRREPLACE(pFIELD_6)||''', ';
END IF;
IF pFIELD_7 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'FIELD_7 = '''||STRREPLACE(pFIELD_7)||''', ';
END IF;
IF pFIELD_8 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'FIELD_8 = '''||STRREPLACE(pFIELD_8)||''', ';
END IF;
IF pFIELD_9 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'FIELD_9 = '''||STRREPLACE(pFIELD_9)||''', ';
END IF;
IF pFIELD_10 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'FIELD_10 = '''||STRREPLACE(pFIELD_10)||''', ';
END IF;
IF pLOAD_ID IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'LOAD_ID = '''||STRREPLACE(pLOAD_ID)||''', ';
END IF;
IF pINT_BATCH_FIELD_1 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'INT_BATCH_FIELD_1 = '''||STRREPLACE(pINT_BATCH_FIELD_1)||''', ';
END IF;
IF pINT_BATCH_FIELD_2 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'INT_BATCH_FIELD_2 = '''||STRREPLACE(pINT_BATCH_FIELD_2)||''', ';
END IF;
IF pINT_BATCH_FIELD_3 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'INT_BATCH_FIELD_3 = '''||STRREPLACE(pINT_BATCH_FIELD_3)||''', ';
END IF;
IF pINT_BATCH_FIELD_4 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'INT_BATCH_FIELD_4 = '''||STRREPLACE(pINT_BATCH_FIELD_4)||''', ';
END IF;
IF pINT_BATCH_FIELD_5 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'INT_BATCH_FIELD_5 = '''||STRREPLACE(pINT_BATCH_FIELD_5)||''', ';
END IF;
IF pINT_BATCH_FIELD_6 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'INT_BATCH_FIELD_6 = '''||STRREPLACE(pINT_BATCH_FIELD_6)||''', ';
END IF;
IF pREAL_BATCH_FIELD_1 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'REAL_BATCH_FIELD_1 = '''||STRREPLACE(pREAL_BATCH_FIELD_1)||''', ';
END IF;
IF pREAL_BATCH_FIELD_2 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'REAL_BATCH_FIELD_2 = '''||STRREPLACE(pREAL_BATCH_FIELD_2)||''', ';
END IF;
IF pREAL_BATCH_FIELD_3 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'REAL_BATCH_FIELD_3 = '''||STRREPLACE(pREAL_BATCH_FIELD_3)||''', ';
END IF;
IF pREAL_BATCH_FIELD_4 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'REAL_BATCH_FIELD_4 = '''||STRREPLACE(pREAL_BATCH_FIELD_4)||''', ';
END IF;
IF pREAL_BATCH_FIELD_5 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'REAL_BATCH_FIELD_5 = '''||STRREPLACE(pREAL_BATCH_FIELD_5)||''', ';
END IF;
IF pREAL_BATCH_FIELD_6 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'REAL_BATCH_FIELD_6 = '''||STRREPLACE(pREAL_BATCH_FIELD_6)||''', ';
END IF;
IF pDATE_BATCH_FIELD_1 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'DATE_BATCH_FIELD_1 = '||pDATE_BATCH_FIELD_1||', ';
END IF;
IF pDATE_BATCH_FIELD_2 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'DATE_BATCH_FIELD_2 = '||pDATE_BATCH_FIELD_2||', ';
END IF;
IF pDATE_BATCH_FIELD_3 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'DATE_BATCH_FIELD_3 = '||pDATE_BATCH_FIELD_3||', ';
END IF;
IF pDATE_BATCH_FIELD_4 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'DATE_BATCH_FIELD_4 = '||pDATE_BATCH_FIELD_4||', ';
END IF;
IF pDATE_BATCH_FIELD_5 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'DATE_BATCH_FIELD_5 = '||pDATE_BATCH_FIELD_5||', ';
END IF;
IF pDATE_BATCH_FIELD_6 IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'DATE_BATCH_FIELD_6 = '||pDATE_BATCH_FIELD_6||', ';
END IF;
IF nBatch_Project_Id IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'BATCH_PROJECT_ID = '''||STRREPLACE(nBatch_Project_Id)||''', ';
END IF;
IF pBatch_Project_Id IS NOT NULL THEN
BatchProjSql := 'Select * from Batch_Projects where Project_Name=:pBatch_Project_Id';
Execute Immediate BatchProjSql into Batches_Rec using pBatch_Project_Id;
BatchesSql := BatchesSql  ||  'BATCH_PROJECT_ID = '''||STRREPLACE(Batches_Rec.Batch_Project_Id)||''', ';
END IF;
IF pNOTEBOOK_PAGE IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'NOTEBOOK_PAGE = '''||STRREPLACE(pNOTEBOOK_PAGE)||''', ';
END IF;
IF pNOTEBOOK_TEXT IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'NOTEBOOK_TEXT = '''||STRREPLACE(pNOTEBOOK_TEXT)||''', ';
END IF;
IF pFORMULA_WEIGHT IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'FORMULA_WEIGHT = '''||STRREPLACE(pFORMULA_WEIGHT)||''', ';
END IF;
IF pNOTEBOOK_INTERNAL_ID IS NOT NULL THEN
Batches_NBSql := 'Select * from NOTEBOOKS where NOTEBOOK_NAME=:pNOTEBOOK_INTERNAL_ID';
Execute Immediate Batches_NBSql into NOTEBOOK_Rec using pNOTEBOOK_INTERNAL_ID;
BatchesSql := BatchesSql  ||  'NOTEBOOK_INTERNAL_ID = '''||STRREPLACE(NOTEBOOK_Rec.NOTEBOOK_NUMBER)||''', ';
END IF;
IF nNOTEBOOK_INTERNAL_ID IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'NOTEBOOK_INTERNAL_ID = '''||STRREPLACE(nNOTEBOOK_INTERNAL_ID)||''', ';
END IF;
IF pProducer IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'Producer = '''||STRREPLACE(pProducer)||''', ';
END IF;
IF pLit_Ref IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'Lit_Ref = '''||STRREPLACE(pLit_Ref)||''', ';
END IF;
IF pBatch_Comment IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'Batch_Comment = '''||STRREPLACE(pBatch_Comment)||''', ';
END IF;
IF pSTORAGE_REQ_AND_WARNINGS IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'STORAGE_REQ_AND_WARNINGS = '''||STRREPLACE(pSTORAGE_REQ_AND_WARNINGS)||''', ';
END IF;
IF pPreparation IS NOT NULL THEN
BatchesSql := BatchesSql  ||  'Preparation = '''||STRREPLACE(pPreparation)||''', ';
END IF;
BatchesSql := BatchesSql || 'LAST_MOD_DATE = ''' ||pLAST_MOD_DATE||''', ';
BatchesSql := BatchesSql || 'Batch_Number = ''' ||pBatchNumber||'''';
BatchesSql := BatchesSql || ' Where Batches.Batch_Number=''' ||pBatchNumber||''' AND Batches.Reg_Internal_Id=(Select REG_NUMBERS.REG_ID from REG_NUMBERS WHERE REG_NUMBERS.REG_NUMBER=:1) RETURNING CPD_INTERNAL_ID into :2';
--Update batches set field_1=BatchesSql where Batches.BATCH_INTERNAL_ID=139;

Execute Immediate BatchesSql using pREG_NUMBER, out vCPDINTERNALID;
OutSql := 'SELECT * from BATCHES Where CPD_INTERNAL_ID = :vCPDINTERNALID AND Batches.Batch_Number=''' ||pBatchNumber||''' AND Batches.Reg_Internal_Id=(Select REG_NUMBERS.REG_ID from REG_NUMBERS WHERE REG_NUMBERS.REG_NUMBER=:pREG_NUMBER)';

Execute Immediate OUTSql into Out_Rec using vCPDINTERNALID, pREG_NUMBER;
--If vCPDINTERNALID IS NOT NULL THEN
pBATCH_INTERNAL_ID := Out_Rec.BATCH_INTERNAL_ID;
pSalIntId := Out_Rec.SALT_INTERNAL_ID;
pSolIntId := Out_Rec.SOLVATE_ID;
pSalEquivalents := Out_Rec.SALT_EQUIVALENTS;
pSolEquivalents := Out_Rec.SOLVATE_EQUIVALENTS;

--END IF;
RegAppSql := 'Update REG_APPROVED SET ';
IF pAPPROVED IS NOT NULL THEN
RegAppSql := RegAppSql  ||  'APPROVED = '''||STRREPLACE(pAPPROVED)||''',';
END IF;
IF pAPPROVED_TIME IS NOT NULL THEN
RegAppSql := RegAppSql  ||  'DATETIME_STAMP = '''||STRREPLACE(pAPPROVED_TIME)||''', ';
END IF;
IF pAPPROVED_DATE IS NOT NULL THEN
RegAppSql := RegAppSql  ||  'DATETIME_STAMP = '''||STRREPLACE(pAPPROVED_DATE)||''', ';
END IF;
RegAppSql := RegAppSql  ||  'BATCH_INTERNAL_ID = '''||pBATCH_INTERNAL_ID||'''';
RegAppSql := RegAppSql || ' Where REG_APPROVED.REG_INTERNAL_ID= :vCPDINTERNALID';
Execute immediate RegAppSql using vCPDINTERNALID;

ComMolSql:= 'Update COMPOUND_MOLECULE SET ';
IF pREAL_CMPD_FIELD_1 IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'REAL_CMPD_FIELD_1 = '''||STRREPLACE(pREAL_CMPD_FIELD_1)||''',';
END IF;
IF pREAL_CMPD_FIELD_2 IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'REAL_CMPD_FIELD_2 = '''||STRREPLACE(pREAL_CMPD_FIELD_2)||''',';
END IF;
IF pREAL_CMPD_FIELD_3 IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'REAL_CMPD_FIELD_3 = '''||STRREPLACE(pREAL_CMPD_FIELD_3)||''',';
END IF;
IF pREAL_CMPD_FIELD_4 IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'REAL_CMPD_FIELD_4 = '''||STRREPLACE(pREAL_CMPD_FIELD_4)||''',';
END IF;
IF pPRODUCT_TYPE IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'PRODUCT_TYPE = '''||STRREPLACE(pPRODUCT_TYPE)||''',';
END IF;
IF pCHIRAL IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'CHIRAL = '''||STRREPLACE(pCHIRAL)||''',';
END IF;
IF pCLOGP IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'CLOGP = '''||STRREPLACE(pCLOGP)||''',';
END IF;
IF pH_BOND_DONORS IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'H_BOND_DONORS = '''||STRREPLACE(pH_BOND_DONORS)||''',';
END IF;
IF pH_BOND_ACCEPTORS IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'H_BOND_ACCEPTORS = '''||STRREPLACE(pH_BOND_ACCEPTORS)||''',';
END IF;
IF pMW_TEXT IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'MW_TEXT = '''||STRREPLACE(pMW_TEXT)||''',';
END IF;
IF pMF_TEXT IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'MF_TEXT = '''||STRREPLACE(pMF_TEXT)||''',';
END IF;
IF nCOMPOUND_TYPE IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'COMPOUND_TYPE = '''||STRREPLACE(nCOMPOUND_TYPE)||''',';
END IF;
IF pCOMPOUND_TYPE IS NOT NULL THEN
ComTypeSql := 'Select * from COMPOUND_TYPE where Description=:pCOMPOUND_TYPE';
Execute immediate ComTypeSql into Compound_Rec using pCOMPOUND_TYPE;
ComMolSql := ComMolSql  ||  'COMPOUND_TYPE = '''||Compound_Rec.COMPOUND_TYPE||''',';
END IF;
IF pSTRUCTURE_COMMENTS_TXT IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'STRUCTURE_COMMENTS_TXT = '''||STRREPLACE(pSTRUCTURE_COMMENTS_TXT)||''',';
END IF;
IF pTXT_CMPD_FIELD_1 IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'TXT_CMPD_FIELD_1 = '''||STRREPLACE(pTXT_CMPD_FIELD_1)||''',';
END IF;
IF pTXT_CMPD_FIELD_2 IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'TXT_CMPD_FIELD_2 = '''||STRREPLACE(pTXT_CMPD_FIELD_2)||''',';
END IF;
IF pTXT_CMPD_FIELD_3 IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'TXT_CMPD_FIELD_3 = '''||STRREPLACE(pTXT_CMPD_FIELD_3)||''',';
END IF;
IF pTXT_CMPD_FIELD_4 IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'TXT_CMPD_FIELD_4 = '''||STRREPLACE(pTXT_CMPD_FIELD_4)||''',';
END IF;
IF pINT_CMPD_FIELD_1 IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'INT_CMPD_FIELD_1 = '''||STRREPLACE(pINT_CMPD_FIELD_1)||''',';
END IF;
IF pINT_CMPD_FIELD_2 IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'INT_CMPD_FIELD_2 = '''||STRREPLACE(pINT_CMPD_FIELD_2)||''',';
END IF;
IF pINT_CMPD_FIELD_3 IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'INT_CMPD_FIELD_3 = '''||STRREPLACE(pINT_CMPD_FIELD_3)||''',';
END IF;
IF pINT_CMPD_FIELD_4 IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'INT_CMPD_FIELD_4 = '''||STRREPLACE(pINT_CMPD_FIELD_4)||''',';
END IF;
IF pDATE_CMPD_FIELD_1 IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'DATE_CMPD_FIELD_1 = '||pDATE_CMPD_FIELD_1||',';
END IF;
IF pDATE_CMPD_FIELD_2 IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'DATE_CMPD_FIELD_2 = '||pDATE_CMPD_FIELD_2||',';
END IF;
IF pDATE_CMPD_FIELD_3 IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'DATE_CMPD_FIELD_3 = '||pDATE_CMPD_FIELD_3||',';
END IF;
IF pDATE_CMPD_FIELD_4 IS NOT NULL THEN
ComMolSql := ComMolSql  ||  'DATE_CMPD_FIELD_4 = '||pDATE_CMPD_FIELD_4||',';
END IF;
ComMolSql := ComMolSql  ||  'Cpd_DataBase_Counter = '''||vCPDINTERNALID||'''';
ComMolSql := ComMolSql || 'Where Compound_Molecule.Cpd_DataBase_Counter=:1 RETURNING MW2 into :2';
Execute Immediate ComMolSql using vCPDINTERNALID, out pMolWeight;

If SaltsFlag !=0 THEN
SaltsSql := 'SELECT * from Salts Where Salt_Code = :pSALT_CODE';
Execute Immediate SaltsSql into salts_rec using pSalIntId;
SolvateSql := 'SELECT * from Solvates Where Solvates.SOLVATE_ID=:pSOLVATE_ID';
Execute Immediate SolvateSql into solvate_rec using pSolIntId;

If pMolWeight Is NUll Then
pMolWeight := 0;
End If;

If salts_rec.SALT_MW Is NUll Then
salts_rec.SALT_MW := 0;
End If;

If pSalEquivalents Is NUll Then
pSalEquivalents := 0;
End If;

If solvate_rec.SOLVATE_MW Is NUll Then
salts_rec.SALT_MW := 0;
End If;

If pSolEquivalents Is NUll Then
pSolEquivalents := 0;
End If;

SolvatesFlag := pMolWeight + (salts_rec.SALT_MW * pSalEquivalents) + ((solvate_rec.SOLVATE_MW * pSolEquivalents));
UBatSql := 'Update Batches Set Formula_Weight = ''' ||SolvatesFlag || '''Where CPD_INTERNAL_ID = :vCPDINTERNALID AND Batches.Batch_Number=''' ||pBatchNumber||''' AND Batches.Reg_Internal_Id=(Select REG_NUMBERS.REG_ID from REG_NUMBERS WHERE REG_NUMBERS.REG_NUMBER=:pREG_NUMBER)';
Execute Immediate UBatSql using vCPDINTERNALID, pREG_NUMBER;
END IF;

IF nCompound_PROJECT_ID IS NOT NULL THEN
ComProjSql := 'Update COMPOUND_PROJECT SET PROJECT_INTERNAL_ID = '''||nCompound_PROJECT_ID||''' Where COMPOUND_PROJECT.CPD_INTERNAL_ID=:vCPDINTERNALID';
Execute Immediate ComProjSql using vCPDINTERNALID;
END IF;
IF pCompound_PROJECT_ID IS NOT NULL THEN
projSql := 'Select * from PROJECTS where PROJECT_NAME=:pCompound_PROJECT_ID';
Execute immediate projSql into comProj using pCompound_PROJECT_ID;
ComProjSql := 'Update COMPOUND_PROJECT SET PROJECT_INTERNAL_ID = '''||comProj.PROJECT_INTERNAL_ID||''' Where COMPOUND_PROJECT.CPD_INTERNAL_ID=:vCPDINTERNALID';
Execute Immediate ComProjSql using vCPDINTERNALID;
END IF;

IF fCHEMICAL_NAME IS NOT NULL THEN
AltIdsSql:= 'Insert into alt_ids (REG_INTERNAL_ID, IDENTIFIER, IDENTIFIER_TYPE, DATETIME_STAMP) Values ( '''||vCPDINTERNALID||''', :fCHEMICAL_NAME ,'''||0||''', '''||pLAST_MOD_DATE||''')';
Execute Immediate AltIdsSql using fCHEMICAL_NAME;
--Update Batches SET FIELD_10= AltIdsSql where REG_INTERNAL_ID='2';
END IF;
IF fCHEM_NAME_AUTOGEN IS NOT NULL THEN
AltIdsSql:= 'Insert into alt_ids (REG_INTERNAL_ID, IDENTIFIER, IDENTIFIER_TYPE, DATETIME_STAMP)
Values ( '''||vCPDINTERNALID||''', :fCHEM_NAME_AUTOGEN ,'''||6||''', '''||pLAST_MOD_DATE||''')';
Execute Immediate AltIdsSql using fCHEM_NAME_AUTOGEN;
END IF;
IF fSYNONYM_R IS NOT NULL THEN
AltIdsSql:=' Insert into alt_ids (REG_INTERNAL_ID, IDENTIFIER, IDENTIFIER_TYPE, DATETIME_STAMP)
Values ( '''||vCPDINTERNALID||''', :fSYNONYM_R ,'''||2||''', '''||pLAST_MOD_DATE||''')';
Execute Immediate AltIdsSql using fSYNONYM_R;
END IF;
IF fCAS_NUMBER IS NOT NULL THEN
AltIdsSql:= 'Insert into alt_ids (REG_INTERNAL_ID, IDENTIFIER, IDENTIFIER_TYPE, DATETIME_STAMP)
Values ( '''||vCPDINTERNALID||''', :fCAS_NUMBER ,'''||2||''', '''||pLAST_MOD_DATE||''')';
Execute Immediate AltIdsSql using fCAS_NUMBER;
END IF;
IF fRNO_NUMBER IS NOT NULL THEN
AltIdsSql:=' Insert into alt_ids (REG_INTERNAL_ID, IDENTIFIER, IDENTIFIER_TYPE, DATETIME_STAMP)
Values ( '''||vCPDINTERNALID||''', :fRNO_NUMBER ,'''||1||''', '''||pLAST_MOD_DATE||''')';
Execute Immediate AltIdsSql using fRNO_NUMBER;
END IF;
IF fFEMA_GRAS_NUMBER IS NOT NULL THEN
AltIdsSql:= 'Insert into alt_ids (REG_INTERNAL_ID, IDENTIFIER, IDENTIFIER_TYPE, DATETIME_STAMP)
Values ( '''||vCPDINTERNALID||''', :fFEMA_GRAS_NUMBER ,'''||4||''', '''||pLAST_MOD_DATE||''')';
Execute Immediate AltIdsSql using fFEMA_GRAS_NUMBER;
END IF;
IF fGROUP_CODE IS NOT NULL THEN
AltIdsSql:= 'Insert into alt_ids (REG_INTERNAL_ID, IDENTIFIER, IDENTIFIER_TYPE, DATETIME_STAMP)
Values ( '''||vCPDINTERNALID||''', :fGROUP_CODE ,'''||5||''', '''||pLAST_MOD_DATE||''')';
Execute Immediate AltIdsSql using fGROUP_CODE;
END IF;
End UpdateBatch;
Procedure CreateBatch(
pReg_Internal_Id in Varchar,
batchN out Batches.BATCH_NUMBER%Type
)
IS
regSql varchar(200);
batchesSql varchar(50);
reg_rec_name Reg_Numbers%ROWTYPE;
reg_int_id Batches.REG_INTERNAL_ID%Type;
bNumber Batches.BATCH_NUMBER%Type;
Begin
regSql := 'SELECT * from Reg_Numbers Where Reg_Numbers.Reg_Number=:pReg_Internal_Id';
--SELECT Reg_Numbers.REG_ID into regId from Reg_Numbers Where Reg_Numbers.Reg_Number=pReg_Internal_Id;
Execute Immediate regSql into reg_rec_Name using pReg_Internal_Id;
Select max(Batches.BATCH_NUMBER) into bNumber from Batches where Batches.REG_INTERNAL_ID = reg_rec_Name.REG_ID;
If bNumber is not null then 
Insert Into Batches(Batches.REG_INTERNAL_ID, Batches.BATCH_NUMBER) Values(reg_rec_Name.REG_ID, bNumber + 1);
batchN := bNumber+1;
else
Insert Into Batches(Batches.REG_INTERNAL_ID, Batches.BATCH_NUMBER) Values(reg_rec_Name.REG_ID, 1);
batchN := 1;
end if;
End CreateBatch;
/*Procedure CreateBatch(
pBatch_Internal_Id in Batches.BATCH_INTERNAL_ID%TYPE,
pGiv_Batch_Id in BATCHES.GIV_BATCH_ID%TYPE default null,
pBatch_Number in Batches.BATCH_NUMBER%TYPE default null,
pMol_Id in BATCHES.MOL_ID%TYPE default null,
pCpd_Internal_Id in BATCHES.CPD_INTERNAL_ID%TYPE default null,
pProducer in BATCHES.PRODUCER%TYPE default null,
pScientist_Id in BATCHES.SCIENTIST_ID%TYPE default null,
pReg_Internal_Id in BATCHES.REG_INTERNAL_ID%TYPE default null,
pLast_Mod_Person_Id in BATCHES.LAST_MOD_PERSON_ID%TYPE default null,
pEntry_Person_Id in BATCHES.ENTRY_PERSON_ID%TYPE default null,
pBatch_Reg_Person_Id in BATCHES.BATCH_REG_PERSON_ID%TYPE default null,
pBatch_Reg_Date in BATCHES.BATCH_REG_DATE%TYPE default null,
pLast_Mod_Date in BATCHES.LAST_MOD_DATE%TYPE default null,
pBatch_Comment in BATCHES.BATCH_COMMENT%TYPE default null,
pNotebook_Page in Batches.NOTEBOOK_PAGE%TYPE default null,
pNotebook_Text in BATCHES.NOTEBOOK_TEXT%TYPE default null,
pNotebook_Internal_Id in BATCHES.NOTEBOOK_INTERNAL_ID%TYPE default null,
pSalt_Internal_Id in BATCHES.SALT_INTERNAL_ID%TYPE default null,
pSalt_Name in BATCHES.SALT_NAME%TYPE default null,
pSalt_Mw in BATCHES.SALT_MW%TYPE default null,
pSalt_Equivalents in BATCHES.SALT_EQUIVALENTS%TYPE default null,
pSolvate_Id in BATCHES.SOLVATE_ID%TYPE default null,
pSolvate_Name in BATCHES.SOLVATE_NAME%TYPE default null,
pSolvate_Mw in BATCHES.SOLVATE_MW%TYPE default null,
pSolvate_Equivalents in BATCHES.SOLVATE_EQUIVALENTS%TYPE default null,
pFormula_Weight in BATCHES.FORMULA_WEIGHT%TYPE default null,
pBatch_Formula in BATCHES.BATCH_FORMULA%TYPE default null,
pBatch_Project_Id in BATCHES.BATCH_PROJECT_ID%TYPE default null,
pSource in BATCHES.SOURCE%TYPE default null,
pVendor_Name in BATCHES.VENDOR_NAME%TYPE default null,
pVendor_Id in BATCHES.VENDOR_ID%TYPE default null,
pPercent_Active in BATCHES.PERCENT_ACTIVE%TYPE default null,
pAmount_Units in BATCHES.AMOUNT_UNITS%TYPE default null,
pPurity in BATCHES.PURITY%TYPE default null,
pLC_UV_MS in BATCHES.LC_UV_MS%TYPE default null,
pChn_Combustion in BATCHES.CHN_COMBUSTION%TYPE default null,
pUv_Spectrum in BATCHES.UV_SPECTRUM%TYPE default null,
pAppearance in BATCHES.APPEARANCE%TYPE default null,
pH1NMR in BATCHES.H1NMR%TYPE default null,
pC13NMR in BATCHES.C13NMR%TYPE default null,
pLogD in BATCHES.LOGD%TYPE default null,
pSolubility in BATCHES.SOLUBILITY%TYPE default null,
pMS in BATCHES.MS%TYPE default null,
pIR in BATCHES.IR%TYPE default null,
pGC in BATCHES.GC%TYPE default null,
pBP in BATCHES.BP%TYPE default null,
pMP in BATCHES.MP%TYPE default null,
pFlashPoint in BATCHES.FLASHPOINT%TYPE default null,
pPhysical_Form in BATCHES.PHYSICAL_FORM%TYPE default null,
pColor in BATCHES.COLOR%TYPE default null,
pHPLC in BATCHES.HPLC%TYPE default null,
pOptical_Rotation in BATCHES.OPTICAL_ROTATION%TYPE default null,
pRefractive_Index in BATCHES.REFRACTIVE_INDEX%TYPE default null,
pPreparation in BATCHES.PREPARATION%TYPE default null,
pStorage_Req_And_Warnings in BATCHES.STORAGE_REQ_AND_WARNINGS%TYPE default null,
pLit_Ref in BATCHES.LIT_REF%TYPE default null,
pCreation_Date in BATCHES.CREATION_DATE%TYPE default null,
pAmount in BATCHES.AMOUNT%TYPE default null,
pField_1 in BATCHES.FIELD_1%TYPE default null,
pField_2 in BATCHES.FIELD_2%TYPE default null,
pField_3 in BATCHES.FIELD_3%TYPE default null,
pField_4 in BATCHES.FIELD_4%TYPE default null,
pField_5 in BATCHES.FIELD_5%TYPE default null,
pField_6 in BATCHES.FIELD_6%TYPE default null,
pField_7 in BATCHES.FIELD_7%TYPE default null,
pField_8 in BATCHES.FIELD_8%TYPE default null,
pField_9 in BATCHES.FIELD_9%TYPE default null,
pField_10 in BATCHES.FIELD_10%TYPE default null,
pLoad_Id in BATCHES.LOAD_ID%TYPE default null,
pDateTime_Stamp in BATCHES.DATETIME_STAMP%TYPE default null,
pInt_Batch_Field_1 in BATCHES.INT_BATCH_FIELD_1%TYPE default null,
pInt_Batch_Field_2 in BATCHES.INT_BATCH_FIELD_2%TYPE default null,
pInt_Batch_Field_3 in BATCHES.INT_BATCH_FIELD_3%TYPE default null,
pInt_Batch_Field_4 in BATCHES.INT_BATCH_FIELD_4%TYPE default null,
pInt_Batch_Field_5 in BATCHES.INT_BATCH_FIELD_5%TYPE default null,
pInt_Batch_Field_6 in BATCHES.INT_BATCH_FIELD_6%TYPE default null,
pReal_Batch_Field_1 in BATCHES.REAL_BATCH_FIELD_1%TYPE default null,
pReal_Batch_Field_2 in BATCHES.REAL_BATCH_FIELD_2%TYPE default null,
pReal_Batch_Field_3 in BATCHES.REAL_BATCH_FIELD_3%TYPE default null,
pReal_Batch_Field_4 in BATCHES.REAL_BATCH_FIELD_4%TYPE default null,
pReal_Batch_Field_5 in BATCHES.REAL_BATCH_FIELD_5%TYPE default null,
pReal_Batch_Field_6 in BATCHES.REAL_BATCH_FIELD_6%TYPE default null,
pDate_Batch_Field_1 in BATCHES.DATE_BATCH_FIELD_1%TYPE default null,
pDate_Batch_Field_2 in BATCHES.DATE_BATCH_FIELD_2%TYPE default null,
pDate_Batch_Field_3 in BATCHES.DATE_BATCH_FIELD_3%TYPE default null,
pDate_Batch_Field_4 in BATCHES.DATE_BATCH_FIELD_4%TYPE default null,
pDate_Batch_Field_5 in BATCHES.DATE_BATCH_FIELD_5%TYPE default null,
pDate_Batch_Field_6 in BATCHES.DATE_BATCH_FIELD_6%TYPE default null,
pTemp_Id in BATCHES.TEMP_ID%Type default null
)
IS
batchesSql varchar(2000);
BEGIN

batchesSql:= 'Insert Into BATCHES (GIV_BATCH_ID,
BATCH_NUMBER,
MOL_ID,
CPD_INTERNAL_ID,
PRODUCER,
SCIENTIST_ID,
REG_INTERNAL_ID,
LAST_MOD_PERSON_ID,
ENTRY_PERSON_ID,
BATCH_REG_PERSON_ID,
BATCH_REG_DATE,
LAST_MOD_DATE,
BATCH_COMMENT,
NOTEBOOK_PAGE,
NOTEBOOK_TEXT,
NOTEBOOK_INTERNAL_ID,
SALT_INTERNAL_ID,
SALT_NAME,
SALT_MW,
SALT_EQUIVALENTS,
SOLVATE_ID,
SOLVATE_NAME,
SOLVATE_MW,
SOLVATE_EQUIVALENTS,
FORMULA_WEIGHT,
BATCH_FORMULA,
BATCH_PROJECT_ID,
SOURCE,
VENDOR_NAME,
VENDOR_ID,
PERCENT_ACTIVE,
AMOUNT_UNITS,
PURITY,
LC_UV_MS,
CHN_COMBUSTION,
UV_SPECTRUM,
APPEARANCE,
H1NMR,
C13NMR,
LOGD,
SOLUBILITY,
MS,
IR,
GC,
BP,
MP,
FLASHPOINT,
PHYSICAL_FORM,
COLOR,
HPLC,
OPTICAL_ROTATION,
REFRACTIVE_INDEX,
PREPARATION,
STORAGE_REQ_AND_WARNINGS,
LIT_REF,
CREATION_DATE,
AMOUNT,
FIELD_1,
FIELD_2,
FIELD_3,
FIELD_4,
FIELD_5,
FIELD_6,
FIELD_7,
FIELD_8,
FIELD_9,
FIELD_10,
LOAD_ID,
DATETIME_STAMP,
INT_BATCH_FIELD_1,
INT_BATCH_FIELD_2,
INT_BATCH_FIELD_3,
INT_BATCH_FIELD_4,
INT_BATCH_FIELD_5,
INT_BATCH_FIELD_6,
REAL_BATCH_FIELD_1,
REAL_BATCH_FIELD_2,
REAL_BATCH_FIELD_3,
REAL_BATCH_FIELD_4,
REAL_BATCH_FIELD_5,
REAL_BATCH_FIELD_6,
DATE_BATCH_FIELD_1,
DATE_BATCH_FIELD_2,
DATE_BATCH_FIELD_3,
DATE_BATCH_FIELD_4,
DATE_BATCH_FIELD_5,
DATE_BATCH_FIELD_6,
TEMP_ID) VALUES(';

batchesSql := batchesSql ||''''||STRREPLACE(pGiv_Batch_Id)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pBatch_Number)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pMol_Id)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pCpd_Internal_Id)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pProducer)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pScientist_Id)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pReg_Internal_Id)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pLast_Mod_Person_Id)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pEntry_Person_Id)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pBatch_Reg_Person_Id)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pBatch_Reg_Date)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pLast_Mod_Date)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pBatch_Comment)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pNotebook_Page)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pNotebook_Text)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pNotebook_Internal_Id)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pSalt_Internal_Id)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pSalt_Name)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pSalt_Mw)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pSalt_Equivalents)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pSolvate_Id)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pSolvate_Name)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pSolvate_Mw)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pSolvate_Equivalents)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pFormula_Weight)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pBatch_Formula)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pBatch_Project_Id)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pSource)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pVendor_Name)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pVendor_Id)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pPercent_Active)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pAmount_Units)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pPurity)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pLC_UV_MS)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pChn_Combustion)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pUv_Spectrum)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pAppearance)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pH1NMR)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pC13NMR)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pLogD)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pSolubility)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pMS)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pIR)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pGC)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pBP)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pMP)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pFlashPoint)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pPhysical_Form)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pColor)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pHPLC)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pOptical_Rotation)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pRefractive_Index)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pPreparation)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pStorage_Req_And_Warnings)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pLit_Ref)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pCreation_Date)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pAmount)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pField_1)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pField_2)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pField_3)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pField_4)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pField_5)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pField_6)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pField_7)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pField_8)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pField_9)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pField_10)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pLoad_Id)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pDateTime_Stamp)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pInt_Batch_Field_1)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pInt_Batch_Field_2)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pInt_Batch_Field_3)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pInt_Batch_Field_4)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pInt_Batch_Field_5)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pInt_Batch_Field_6)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pReal_Batch_Field_1)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pReal_Batch_Field_2)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pReal_Batch_Field_3)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pReal_Batch_Field_4)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pReal_Batch_Field_5)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pReal_Batch_Field_6)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pDate_Batch_Field_1)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pDate_Batch_Field_2)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pDate_Batch_Field_3)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pDate_Batch_Field_4)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pDate_Batch_Field_5)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pDate_Batch_Field_6)||''',';
batchesSql := batchesSql ||''''||STRREPLACE(pTemp_Id)||''')';



--Execute Immediate batchesSql;
Update batches set field_1=batchesSql where Batches.BATCH_INTERNAL_ID=139;
END CreateBatch;*/
END;
/