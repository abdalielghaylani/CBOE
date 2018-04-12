CREATE or REPLACE PACKAGE BODY "COMPOUNDS"
IS
   FUNCTION CREATESYNONYM (pCompoundID IN inv_Synonyms.Compound_ID_FK%Type, pSubstanceName IN inv_Synonyms.Substance_Name%Type)
	   RETURN inv_Synonyms.Synonym_ID%Type AS
	   newSynonymID inv_Synonyms.Synonym_ID%Type;
	 BEGIN
	   INSERT INTO   inv_Synonyms
	                 (Compound_ID_FK, Substance_Name)
	   VALUES        (pCompoundID, pSubstanceName)
	   RETURNING Synonym_ID INTO newSynonymID;
	 RETURN newSynonymID;
	 END CREATESYNONYM;

	 FUNCTION UPDATESYNONYM (pSynonymID IN inv_Synonyms.Synonym_ID%Type, pSubstanceName IN inv_Synonyms.Substance_Name%Type)
	   RETURN inv_Synonyms.Synonym_ID%Type AS
	 BEGIN
	   UPDATE inv_Synonyms
	   SET   Substance_Name = pSubstanceName
	   WHERE Synonym_ID = pSynonymID;
	 RETURN pSynonymID;
	 END UPDATESYNONYM;

	 FUNCTION DELETESYNONYM (pSynonymID IN inv_Synonyms.Synonym_ID%Type)
	   RETURN inv_Synonyms.Synonym_ID%Type AS
	 BEGIN
	   DELETE FROM inv_Synonyms
	   WHERE Synonym_ID = pSynonymID;
	 RETURN pSynonymID;
	 END DELETESYNONYM;

   PROCEDURE GETSYNONYMS (pCompoundID IN  inv_Synonyms.Compound_ID_FK%Type,
                          O_CompoundName OUT inv_Compounds.Substance_Name%Type,
                          O_RS OUT CURSOR_TYPE) AS
   BEGIN
     SELECT Substance_Name into O_CompoundName FROM inv_Compounds WHERE Compound_ID = pCompoundID;
     OPEN O_RS FOR
     SELECT  Synonym_ID, Compound_ID_FK, Substance_Name
     FROM inv_Synonyms
	   WHERE
	     inv_Synonyms.Compound_ID_FK = pCompoundID
	   ORDER BY Substance_Name;
   END GETSYNONYMS;

	Function InsertTempQuery(
		pStructure inv_compounds.base64_cdx%type)
		Return CLOB AS
		vStructure CLOB;
		vCount number;
	BEGIN
		SELECT count(*) INTO vCount FROM cscartridge.tempqueries WHERE id = 0;
		IF vCount = 0 THEN
			INSERT INTO cscartridge.tempqueries VALUES (0,null,pStructure);
		END IF;
		SELECT query INTO vStructure FROM cscartridge.tempqueries WHERE id = 0;
		RETURN vStructure;
	END InsertTempQuery;


   FUNCTION DELETESUBSTANCE (pCompoundID in Inv_compounds.compound_ID%Type)
	RETURN integer
	IS
	containers_refer_to_substance exception;
	pragma exception_init (containers_refer_to_substance, -2292);
	BEGIN
	 DELETE FROM inv_compounds WHERE compound_id = pCompoundID;
	RETURN 1;

	exception
	  WHEN containers_refer_to_substance then
	    RETURN -129;
	END DeleteSubstance;

	Function GetInvCompoundClob (pCompoundID IN inv_compounds.compound_id%type) Return CLOB AS

	    b64Clob CLOB;
	    vSQL varchar2(2000);
	BEGIN
	    vSQL := 'SELECT base64_cdx FROM inv_compounds WHERE compound_id = :cid';
		Execute Immediate vSQL INTO b64Clob USING pCompoundID;
		RETURN b64Clob;
	End GetInvCompoundClob;

	Procedure BuildConflictTypeSQL(
		pSQL in out NOCOPY varchar2,
		pFieldName in varchar2,
		pConflictTypeName in varchar2,
		pFieldValue in varchar2) AS
		vFieldValue_Escaped varchar2(255);

	BEGIN
		if pFieldValue is NOT Null then
		    if length(pSQL) > 0 then
		    	pSQL := pSQL || ' UNION ';
		    end if;
		    vFieldValue_Escaped := replace(pFieldValue,'''', '''''');
			pSQL :=  pSQL || 'select compound_id, ''' || pConflictTypeName || ''' AS conflictType FROM INV_COMPOUNDS WHERE upper(' || pFieldName || ') =  upper(''' || vFieldValue_Escaped || ''') AND rownum < 2';
		end if;
	end BuildConflictTypeSQL;

	Function GetConflictingCompoundIDs(
		pCompoundID in inv_compounds.compound_id%type,
	 	O_RS in CURSOR_TYPE,
	 	oDupStr out inv_compounds.compound_id%type,
	 	oDupSN out inv_compounds.compound_id%type,
	 	oDupCAS out inv_compounds.compound_id%type,
	 	oDupACX out inv_compounds.compound_id%type,
	 	oDupAltID_1 out inv_compounds.compound_id%type,
	 	oDupAltID_2 out inv_compounds.compound_id%type,
	 	oDupAltID_3 out inv_compounds.compound_id%type,
	 	oDupAltID_4 out inv_compounds.compound_id%type,
	 	oDupAltID_5 out inv_compounds.compound_id%type,
	 	oExistingCompoundID out inv_compounds.compound_id%type,
	 	oConflictingFields out inv_compounds.conflicting_fields%type) RETURN integer AS
		vCurrID inv_compounds.compound_id%type;
	    vConflictType varchar2(50);
	    vIsEditConflict boolean:=false;
	    vIsStructureConflict boolean:=false;
	    vIsSubstanceNameConflict boolean:=false;
	    vIsCASConflict boolean:=false;
	    vIsACXIDConflict boolean:=false;
	    vIsAltID1Conflict boolean:=false;
	    vIsAltID2Conflict boolean:=false;
	    vIsAltID3Conflict boolean:=false;
	    vIsAltID4Conflict boolean:=false;
	    vIsAltID5Conflict boolean:=false;
	    vConflictExists boolean:=false;
	BEGIN
		LOOP
			FETCH O_RS INTO vCurrID, vConflictType;
		    Exit when O_RS%NOTFOUND;
			if (pCompoundID is not Null) AND (vCurrID <> pCompoundID) then vIsEditConflict := true; end if;
			if (pCompoundID is null) OR (vIsEditConflict) then
			    if vConflictType = 'structure' then
			    	oDupStr := vCurrID;
			    	vIsStructureConflict := true;
				    vConflictExists:=true;
			    end if;
			    if vConflictType = 'name' then
			    	oDupSN := vCurrID;
			    	vIsSubstanceNameConflict := true;
				    vConflictExists:=true;
			    end if;
			    if vConflictType = 'cas' then
			    	oDupCAS := vCurrID;
			    	vIsCasConflict := true;
				    vConflictExists:=true;
			    end if;
			    if vConflictType = 'acx' then
			    	oDupACX := vCurrID;
			    	vIsACXIDConflict := true;
				    vConflictExists:=true;
			    end if;
			    if vConflictType = 'alt_id_1' then
			    	oDupAltID_1 := vCurrID;
			    	vIsAltID1Conflict := true;
				    vConflictExists:=true;
			    end if;
			    if vConflictType = 'alt_id_2' then
			    	oDupAltID_2 := vCurrID;
			    	vIsAltID2Conflict := true;
				    vConflictExists:=true;
			    end if;
			    if vConflictType = 'alt_id_3' then
			    	oDupAltID_3 := vCurrID;
			    	vIsAltID3Conflict := true;
				    vConflictExists:=true;
			    end if;
			    if vConflictType = 'alt_id_4' then
			    	oDupAltID_4 := vCurrID;
			    	vIsAltID4Conflict := true;
				    vConflictExists:=true;
			    end if;
			    if vConflictType = 'alt_id_5' then
			    	oDupAltID_5 := vCurrID;
			    	vIsAltID5Conflict := true;
				    vConflictExists:=true;
			    end if;
			end if;
		END LOOP;
		if vConflictExists then
	    	-- Build Conflicting Fields string
		    if vIsStructureConflict then
	    		oConflictingFields := oConflictingFields || 'structure|';
		    else
		    	oConflictingFields := oConflictingFields || '|';
	    	end if;
		    if vIsSubstanceNameConflict then
		    	oConflictingFields := oConflictingFields || 'substance_name|';
		    else
		    	oConflictingFields := oConflictingFields || '|';
			end if;
		    if vIsCASConflict then
		    	oConflictingFields := oConflictingFields || 'cas|';
		    else
		    	oConflictingFields := oConflictingFields || '|';
		    end if;
		    if vIsACXIDConflict then
		    	oConflictingFields := oConflictingFields || 'acx_id|';
		    else
		    	oConflictingFields := oConflictingFields || '|';
		    end if;
		    if vIsAltID1Conflict then
		    	oConflictingFields := oConflictingFields || 'alt_id_1|';
		    else
		    	oConflictingFields := oConflictingFields || '|';
		    end if;
		    if vIsAltID2Conflict then
		    	oConflictingFields := oConflictingFields || 'alt_id_2|';
		    else
		    	oConflictingFields := oConflictingFields || '|';
		    end if;
		    if vIsAltID3Conflict then
		    	oConflictingFields := oConflictingFields || 'alt_id_3|';
		    else
		    	oConflictingFields := oConflictingFields || '|';
		    end if;
		    if vIsAltID4Conflict then
		    	oConflictingFields := oConflictingFields || 'alt_id_4|';
		    else
		    	oConflictingFields := oConflictingFields || '|';
		    end if;
		    if vIsAltID5Conflict then
		    	oConflictingFields := oConflictingFields || 'alt_id_5';
		    end if;
		end if;

		if pCompoundID is not Null then -- This is an Edit request
		   if vIsEditConflict then -- Editing causes a conflict
		   		oExistingCompoundID := 0;
		   else -- Editing does not cause a conflict
				oExistingCompoundID := pCompoundID;
		   end if;
		else -- This is an insert request
			if oDupACX > 0 then
				oExistingCompoundID := oDupACX;
			Elsif oDupStr = oDupCAS then
			   oExistingCompoundID := oDupStr;
			Elsif oDupStr = oDupSN then
				oExistingCompoundID := oDupStr;
			Elsif oDupSN = oDupCAS then
			   oExistingCompoundID := oDupCAS;
			Else
				oExistingCompoundID := 0;
			End if;
		End if;
		RETURN O_RS%ROWCOUNT ;
	End GetConflictingCompoundIDs;

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
                        pLocationType in inv_compounds.LOCATION_TYPE_ID_FK%type:=NULL) Return inv_compounds.compound_id%type AS

		vCompoundID inv_compounds.compound_id%type;
	BEGIN
		UPDATE inv_compounds SET
			substance_name = pSubstanceName,
			BASE64_CDX = pStructure,
			CAS = pCAS,
			ACX_ID = pACXID,
      DENSITY = pDensity,
      CLOGP = pcLogP,
      ROTATABLE_BONDS = pRotatableBonds,
      TOT_POL_SURF_AREA = pTotPolSurfArea,
      HBOND_ACCEPTORS = pHBondAcceptors,
      HBOND_DONORS = pHBondDonors,
			ALT_ID_1 = pAltID_1,
			ALT_ID_2 = pAltID_2,
			ALT_ID_3 = pAltID_3,
			ALT_ID_4 = pAltID_4,
			ALT_ID_5 = pAltID_5,
			CONFLICTING_FIELDS = pConflictingFields,
                        LOCATION_TYPE_ID_FK=pLocationType
		WHERE compound_id = pCompoundID;
		RETURN vCompoundID;
	END UpdateInvCompound;


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
		)
		AS
	 	O_RS CURSOR_TYPE;
		vConflictingFields inv_compounds.conflicting_fields%type;
	    vSQL varchar2(4000):='';
	    vConflictingCompoundCount integer;
		vMolWeight integer;
	    vExistingCompoundID inv_compounds.compound_id%type;
	    vStructure CLOB;
	    vRegisterIfConflicts boolean;
            vLocationType number;
	BEGIN

		if pRegisterIfConflicts = 'true' then
			vRegisterIfConflicts := true;
		else
			vRegisterIfConflicts := false;
		end if;
                
                if pLocationType=0 then
                    vLocationType:=NULL;
                else
                    vLocationType:=pLocationType;
                end if;
                

		vStructure := InsertTempQuery(pStructure);
		--INSERT INTO cscartridge.tempqueries VALUES (0,null,pStructure);
		--SELECT query INTO vStructure FROM cscartridge.tempqueries WHERE id = 0;
		IF vStructure is NOT null THEN
			vSQL := 'select compound_id, ''structure'' AS conflictType from INV_COMPOUNDS where cscartridge.MoleculeContains(base64_cdx, ''SELECT query FROM cscartridge.tempqueries WHERE id = 0'','''',''FULL=YES,IDENTITY=YES'')=1 AND rownum < 2 ';
		END IF;
		BuildConflictTypeSQL(vSQL, 'substance_name', 'name', pSubstanceName);
		BuildConflictTypeSQL(vSQL, 'cas', 'cas', pCAS);
		BuildConflictTypeSQL(vSQL, 'acx_id', 'acx', pACXID);
		if InStr(lower(pUniqueAltIDList),'alt_id_1') > 0 then BuildConflictTypeSQL(vSQL, 'alt_id_1',  'alt_id_1', pAltID_1); end if;
		if InStr(lower(pUniqueAltIDList),'alt_id_2') > 0 then BuildConflictTypeSQL(vSQL, 'alt_id_2',  'alt_id_2', pAltID_2); end if;
		if InStr(lower(pUniqueAltIDList),'alt_id_3') > 0 then BuildConflictTypeSQL(vSQL, 'alt_id_3',  'alt_id_3', pAltID_3); end if;
		if InStr(lower(pUniqueAltIDList),'alt_id_4') > 0 then BuildConflictTypeSQL(vSQL, 'alt_id_4',  'alt_id_4', pAltID_4); end if;
		if InStr(lower(pUniqueAltIDList),'alt_id_5') > 0 then BuildConflictTypeSQL(vSQL, 'alt_id_5',  'alt_id_5', pAltID_5); end if;

		select cscartridge.molweight(pStructure) into vMolWeight from dual;

		if vMolWeight = 0 then
			vConflictingCompoundCount := 0;
		else
			--OPEN O_RS FOR vSQL USING pStructure;
			OPEN O_RS FOR vSQL;
	   		vConflictingCompoundCount := GetConflictingCompoundIDs(pCompoundID, O_RS, oDupStr, oDupSN, oDupCAS, oDupACX, oDupAltID_1, oDupAltID_2, oDupAltID_3, oDupAltID_4, oDupAltID_5, vExistingCompoundID, vConflictingFields);
			CLOSE O_RS;
		end if;

		if vConflictingCompoundCount > 0 then --Conflicts found
		--	if vExistingCompoundID > 0 and not vRegisterIfConflicts then  -- Use existing compound
			if vExistingCompoundID > 0  then  -- Use existing compound
		   		if vExistingCompoundID = pCompoundID then --This is and edit request
		   			oCompoundID := UpdateInvCompound(pSubstanceName, vStructure, pCAS, pACXID, pDensity, pcLogP, pRotatableBonds, pTotPolSurfArea, pHBondAcceptors, pHBondDonors, pAltID_1, pAltID_2, pAltID_3, pAltID_4, pAltID_5, pCompoundID, vConflictingFields,vLocationType);
		   		else
		   			oCompoundID := vExistingCompoundID;
		   		end if;
		   		oIsExistingCompound := 1;
		   		oIsDuplicateCompound := 0;
	   	else
	   		if vRegisterIfConflicts then -- Create a duplicate compound
		  		if pCompoundID is not Null then  -- This is an edit request which turns the edited compound into a duplicate
		  		  oCompoundID := UpdateInvCompound(pSubstanceName, vStructure, pCAS, pACXID, pDensity, pcLogP, pRotatableBonds, pTotPolSurfArea, pHBondAcceptors, pHBondDonors, pAltID_1, pAltID_2, pAltID_3, pAltID_4, pAltID_5, pCompoundID, vConflictingFields,vLocationType);
		  		else
		  			oCompoundID := InsertInvCompound(pSubstanceName, vStructure, pCAS, pACXID, pDensity, pcLogP, pRotatableBonds, pTotPolSurfArea, pHBondAcceptors, pHBondDonors, pAltID_1, pAltID_2, pAltID_3, pAltID_4, pAltID_5, vConflictingFields,vLocationType);
					end if;
					oIsExistingCompound := 0;
					oIsDuplicateCompound := 1;
				else  -- No action taken
			    oCompoundID := 0;
			    oIsExistingCompound := 0;
		  			oIsDuplicateCompound := 1;
				end if;
			end if;
		Else -- No conflict found, Create/Edit compound
			if pCompoundID is not Null then -- This is an edit request
				oCompoundID := UpdateInvCompound(pSubstanceName, vStructure, pCAS, pACXID, pDensity, pcLogP, pRotatableBonds, pTotPolSurfArea, pHBondAcceptors, pHBondDonors, pAltID_1, pAltID_2, pAltID_3, pAltID_4, pAltID_5, pCompoundID, vConflictingFields,vLocationType);
			else
				oCompoundID := InsertInvCompound(pSubstanceName, vStructure, pCAS, pACXID, pDensity, pcLogP, pRotatableBonds, pTotPolSurfArea, pHBondAcceptors, pHBondDonors, pAltID_1, pAltID_2, pAltID_3, pAltID_4, pAltID_5, vConflictingFields,vLocationType);
		 	end if;
		 	oIsExistingCompound := 0;
			oIsDuplicateCompound := 0;
		End if;

		-- if no existing

	END RegisterInvCompound;


	Function GetCompoundIDFromMolecule(
		pQuery in inv_compounds.base64_cdx%type) Return inv_compounds.compound_id%type AS

		my_cur CURSOR_TYPE;
		vSQL varchar(2000);
		vCompoundID integer;
	BEGIN
	   	vSQL := 'SELECT compound_id FROM inv_compounds WHERE cscartridge.MoleculeContains(base64_cdx, ''SELECT query FROM cscartridge.tempqueries WHERE id = 0'','''', ''FULL=YES,IDENTITY=YES'')=1';
		OPEN my_cur FOR vSQL;

		FETCH my_cur INTO vCompoundID;
		if my_cur%NOTFOUND then
		  --dbms_output.put_line('No matching molecule found');
		  RETURN 0;
		else
		  --dbms_output.put_line('A matching was found');
		  RETURN vCompoundID;
		end if;
		CLOSE my_cur;
	EXCEPTION
		when others then
			if my_cur%ISOPEN then
				close my_cur;
			end if;
	END GetCompoundIDFromMolecule;

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
                        pLocationType in inv_compounds.LOCATION_TYPE_ID_FK%type:=NULL) Return inv_compounds.compound_id%type AS

	    vCompoundID inv_compounds.compound_id%type;
	    vStructure CLOB;
	BEGIN

		vStructure := InsertTempQuery(pStructure);
		INSERT INTO inv_compounds (
			substance_name,
			base64_CDX,
			CAS,
			ACX_ID,
      DENSITY,
      CLOGP,
      ROTATABLE_BONDS,
      TOT_POL_SURF_AREA,
      HBOND_ACCEPTORS,
      HBOND_DONORS,
			ALT_ID_1,
			ALT_ID_2,
			ALT_ID_3,
			ALT_ID_4,
			ALT_ID_5,
			CONFLICTING_FIELDS,
                        LOCATION_TYPE_ID_FK)
		VALUES (
			pSubstanceName,
			vStructure,
			pCAS,
			pACXID,
      pDensity,
      pcLogP,
      pRotatableBonds,
      pTotPolSurfArea,
      pHBondAcceptors,
      pHBondDonors,
			pAltID_1,
			pAltID_2,
			pAltID_3,
			pAltID_4,
			pAltID_5,
			pConflictingFields,
                        pLocationType)
		RETURNING compound_id INTO vCompoundID;

		RETURN vCompoundID;
	END InsertInvCompound;


END COMPOUNDS;

/
show errors;