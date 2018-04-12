CREATE or REPLACE PACKAGE "COMPOUNDS"
	AS
   TYPE  CURSOR_TYPE IS REF CURSOR;

   FUNCTION CREATESYNONYM
      (pCompoundID IN inv_Synonyms.Compound_ID_FK%Type,
	     pSubstanceName IN inv_Synonyms.Substance_Name%Type)
	     RETURN inv_Synonyms.Synonym_ID%Type;

	 FUNCTION UPDATESYNONYM
      (pSynonymID IN inv_Synonyms.Synonym_ID%Type,
	     pSubstanceName IN inv_Synonyms.Substance_Name%Type)
	     RETURN inv_Synonyms.Synonym_ID%Type;

	 FUNCTION DELETESYNONYM
      (pSynonymID IN inv_Synonyms.Synonym_ID%Type)
	     RETURN inv_Synonyms.Synonym_ID%Type;

	 PROCEDURE GETSYNONYMS
	    (pCompoundID IN  inv_Synonyms.Compound_ID_FK%Type,
	     O_CompoundName OUT inv_Compounds.Substance_Name%Type,
	     O_RS OUT CURSOR_TYPE);
	 FUNCTION DELETESUBSTANCE
	  (pCompoundID in Inv_compounds.compound_ID%Type)
		RETURN integer;

	-- Registers a molucule passed in as a clob
	-- Registers a molucule passed in as a clob
        Function UpdateInvCompound(
			pSubstanceName in inv_compounds.substance_name%type,
			pStructure in inv_compounds.base64_cdx%type,
			pCAS in inv_compounds.cas%type,
			pACXID in inv_compounds.acx_id%type,
			pDensity in inv_compounds.density%TYPE,
    	pcLogP in inv_compounds.clogp%TYPE,
    	pRotatableBonds in inv_compounds.rotatable_bonds%TYPE,
    	pTotPolSurfArea in inv_compounds.tot_pol_surf_area%TYPE,
    	pHBondAcceptors in inv_compounds.hbond_acceptors%TYPE,
    	pHBondDonors in inv_compounds.hbond_donors%TYPE,
			pAltID_1 in inv_compounds.alt_id_1%type,
			pAltID_2 in inv_compounds.alt_id_2%type,
			pAltID_3 in inv_compounds.alt_id_3%type,
			pAltID_4 in inv_compounds.alt_id_4%type,
			pAltID_5 in inv_compounds.alt_id_5%type,
			pCompoundID in inv_compounds.compound_id%type,
			pConflictingFields in inv_compounds.conflicting_fields%type,
                        pLocationType in inv_compounds.LOCATION_TYPE_ID_FK%type:=NULL) Return inv_compounds.compound_id%type;
                        
	Procedure RegisterInvCompound(
		pCompoundID in inv_compounds.compound_id%type,
		pRegisterIfConflicts in varchar2,
		pSubstanceName in inv_compounds.substance_name%type,
		pCAS in inv_compounds.cas%type,
		pACXID in inv_compounds.acx_id%type,
		pDensity in inv_compounds.density%TYPE,
    pcLogP in inv_compounds.clogp%TYPE,
    pRotatableBonds in inv_compounds.rotatable_bonds%TYPE,
    pTotPolSurfArea in inv_compounds.tot_pol_surf_area%TYPE,
    pHBondAcceptors in inv_compounds.hbond_acceptors%TYPE,
    pHBondDonors in inv_compounds.hbond_donors%TYPE,
		pAltID_1 in inv_compounds.alt_id_1%type,
		pAltID_2 in inv_compounds.alt_id_2%type,
		pAltID_3 in inv_compounds.alt_id_3%type,
		pAltID_4 in inv_compounds.alt_id_4%type,
		pAltID_5 in inv_compounds.alt_id_5%type,
		pUniqueAltIDList in varchar2,
		pStructure in inv_compounds.base64_cdx%type,
		oDupStr out inv_compounds.compound_id%type,
	 	oDupSN out inv_compounds.compound_id%type,
	 	oDupCAS out inv_compounds.compound_id%type,
	 	oDupACX out inv_compounds.compound_id%type,
	 	oDupAltID_1 out inv_compounds.compound_id%type,
	 	oDupAltID_2 out inv_compounds.compound_id%type,
	 	oDupAltID_3 out inv_compounds.compound_id%type,
	 	oDupAltID_4 out inv_compounds.compound_id%type,
	 	oDupAltID_5 out inv_compounds.compound_id%type,
	 	oCompoundID out inv_compounds.compound_id%type,
	 	oIsExistingCompound out integer,
	 	oIsDuplicateCompound out integer,
	 	pLocationType in inv_compounds.LOCATION_TYPE_ID_FK%type:=NULL
		) ;

	-- Returns an ID field matching a structure passed in as a clob
	Function GetCompoundIDFromMolecule(
		pQuery in inv_compounds.base64_cdx%type) Return inv_compounds.compound_id%type;

	-- Inserts a molecule passed in as a CLOB and returns the newly created ID
	Function InsertInvCompound(
			pSubstanceName in inv_compounds.substance_name%type,
			pStructure in inv_compounds.base64_cdx%type,
			pCAS in inv_compounds.cas%type,
			pACXID in inv_compounds.acx_id%type,
			pDensity in inv_compounds.density%TYPE,
    	pcLogP in inv_compounds.clogp%TYPE,
    	pRotatableBonds in inv_compounds.rotatable_bonds%TYPE,
   	 	pTotPolSurfArea in inv_compounds.tot_pol_surf_area%TYPE,
    	pHBondAcceptors in inv_compounds.hbond_acceptors%TYPE,
    	pHBondDonors in inv_compounds.hbond_donors%TYPE,
			pAltID_1 in inv_compounds.alt_id_1%type,
			pAltID_2 in inv_compounds.alt_id_2%type,
			pAltID_3 in inv_compounds.alt_id_3%type,
			pAltID_4 in inv_compounds.alt_id_4%type,
			pAltID_5 in inv_compounds.alt_id_5%type,
			pConflictingFields in inv_compounds.conflicting_fields%type,
                        pLocationType in inv_compounds.LOCATION_TYPE_ID_FK%type:=NULL) Return inv_compounds.compound_id%type;

END COMPOUNDS;

/
show errors;