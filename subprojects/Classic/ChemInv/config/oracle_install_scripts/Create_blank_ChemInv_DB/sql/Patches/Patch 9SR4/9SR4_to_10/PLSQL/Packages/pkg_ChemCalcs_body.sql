CREATE OR REPLACE PACKAGE BODY "CHEMCALCS"
AS

--Convert Units
	Function Convert(
		pAmount number,
		pFromUnitID inv_units.unit_id%TYPE,
		pToUnitID inv_units.unit_id%TYPE)
	RETURN number
	IS
	vConversionFactor inv_unit_conversion.conversion_factor%TYPE;
	CURSOR vConversionFactor_cur IS
		SELECT to_unit_id_fk, conversion_factor FROM inv_unit_conversion WHERE from_unit_id_fk = pFromUnitID;
	vCount number;
	Invalid_Conversion exception;
	BEGIN
		--see if no conversion is needed
		IF pAmount = 0 OR pAmount IS NULL THEN
			RETURN 0;
		ELSIF pFromUnitID = pToUnitID THEN
			RETURN pAmount;
		ELSE

			--try to find direct conversion factor
			SELECT count(*) INTO vCount FROM inv_unit_conversion WHERE to_unit_id_fk = pToUnitID AND from_unit_id_fk = pFromUnitID;
			IF vCount > 0 THEN
				SELECT conversion_factor INTO vConversionFactor	FROM inv_unit_conversion WHERE from_unit_id_fk = pFromUnitID AND to_unit_id_fk = pToUnitID;
			END IF;
			--if none than try to find a common conversion and build the conversion factor
			IF vConversionFactor IS NULL THEN
				FOR vConversionFactor_rec in vConversionFactor_cur
				LOOP
					SELECT count(*) INTO vCount FROM inv_unit_conversion WHERE to_unit_id_fk = vConversionFactor_rec.to_unit_id_fk AND from_unit_id_fk = pToUnitID;
					IF vCount > 0 THEN
						SELECT vConversionFactor_rec.conversion_factor/conversion_factor INTO vConversionFactor FROM inv_unit_conversion WHERE from_unit_id_fk = pToUnitID and to_unit_id_fk = vConversionFactor_rec.to_unit_id_fk;
						RETURN vConversionFactor * pAmount;
					END IF;
				END LOOP;
			END IF;

			IF vConversionFactor IS NULL THEN
				RAISE Invalid_Conversion;
			END IF;

			RETURN vConversionFactor * pAmount;
		END IF;
		EXCEPTION
			WHEN Invalid_Conversion THEN
				RETURN -2;
			WHEN OTHERS THEN
				RETURN -1;

	END Convert;

--GetMolarAmount: Dry/Wet Plate
--	pDensity must be in g/L
	FUNCTION CalcMolarAmountFromVolume(pMW IN inv_compounds.molecular_weight%TYPE,
		pQtyRemaining IN inv_wells.qty_remaining%TYPE,
		pQtyRemainingUnitID IN inv_wells.qty_remaining%TYPE,
		pDensity IN inv_compounds.density%TYPE)
	RETURN number
	IS
	vMolarAmount inv_wells.molar_amount%TYPE;
	vConversionFactor inv_unit_conversion.conversion_factor%TYPE;
	vConvertToUnitID inv_units.unit_id%TYPE;
	BEGIN
		vConvertToUnitID := 2;
		SELECT conversion_factor INTO vConversionFactor
			FROM inv_unit_conversion
			WHERE from_unit_id_fk = pQtyRemainingUnitID
				AND to_unit_id_fk = vConvertToUnitID;
		vMolarAmount := (pQtyRemaining*vConversionFactor*pDensity)/pMW;

		RETURN vMolarAmount;
	END;

	FUNCTION CalcMolarAmountFromWeight(
		pMW IN inv_compounds.molecular_weight%TYPE,
		pQtyRemaining IN inv_wells.qty_remaining%TYPE,
		pQtyRemainingUnitID IN inv_wells.qty_remaining%TYPE)
	RETURN number
	IS
	vMolarAmount inv_wells.molar_amount%TYPE;
	vCount NUMBER := 0;
  vConversionFactor inv_unit_conversion.conversion_factor%TYPE;
	BEGIN
  	-- try to convert to moles
		SELECT count(conversion_factor) INTO vCount FROM inv_unit_conversion WHERE from_unit_id_fk = pQtyRemainingUnitID AND to_unit_id_fk = constants.cMoleID;  	
    IF vCount > 0 THEN
			SELECT conversion_factor INTO vConversionFactor	FROM inv_unit_conversion WHERE from_unit_id_fk = pQtyRemainingUnitID AND to_unit_id_fk = constants.cMoleID;
    	vMolarAmount := pQtyRemaining * vConversionFactor;
    ELSE
								IF pMW = 0  THEN
											vMolarAmount := 0;
								ELSE
      -- try to convert to g
  		SELECT conversion_factor INTO vConversionFactor	FROM inv_unit_conversion WHERE from_unit_id_fk = pQtyRemainingUnitID AND to_unit_id_fk = constants.cGramID;
			--!need to do something with sigfigs here
			vMolarAmount := (pQtyRemaining*vConversionFactor)/pMW;
							END IF;
		END IF;


		RETURN vMolarAmount;
	END;


--GetMolarAmount: Solvated Plate given Quantity Remaining
	FUNCTION CalcMolarAmount(pMW IN inv_compounds.molecular_weight%TYPE,
		pSolventVolume IN inv_wells.solvent_volume%TYPE,
		pSolventVolumeUnitID IN inv_wells.solvent_volume_unit_id_fk%TYPE,
		pQtyRemaining IN inv_wells.qty_remaining%TYPE,
		pQtyRemainingUnitID IN inv_wells.qty_remaining%TYPE,
		pWellType IN number)
	RETURN number
	IS
	v_MolarAmount inv_wells.molar_amount%TYPE;
	BEGIN
		RETURN 1;
	END;

--GetMolarAmount: Solvated Plate given Molar Concentration
	FUNCTION CalcMolarAmountFromMolarConc(
		pSolventVolume IN inv_wells.solvent_volume%TYPE,
		pSolventVolumeUnitID IN inv_wells.solvent_volume_unit_id_fk%TYPE,
		pMolarConc IN inv_wells.molar_conc%TYPE)
	RETURN number
	IS
	vMolarAmount inv_wells.molar_amount%TYPE;
	vConversionFactor inv_unit_conversion.conversion_factor%TYPE;
	vConvertToUnitID inv_units.unit_id%TYPE;
	vSolventVolumeInLiters inv_wells.solvent_volume%TYPE;
	BEGIN
		vConvertToUnitID := 2;
		SELECT conversion_factor INTO vConversionFactor
			FROM inv_unit_conversion
			WHERE from_unit_id_fk = pSolventVolumeUnitID
				AND to_unit_id_fk = vConvertToUnitID;

		vSolventVolumeInLiters := pSolventVolume * vConversionFactor;
		vMolarAmount := pMolarConc * vSolventVolumeInLiters;
		RETURN vMolarAmount;

		EXCEPTION
			WHEN OTHERS THEN
				RETURN -1;
	END;

--GetMW
    FUNCTION GetMW(
    	pCompoundID inv_well_compounds.compound_id_fk%TYPE,
    	pRegID inv_well_compounds.reg_id_fk%TYPE,
    	pBatchNumber inv_well_compounds.batch_number_fk%TYPE)
    RETURN number
    IS
   	vMW number := -1;
	invalid_compound exception;
	vIndexName varchar2(200);
	vTableName varchar2(200);
  vCount integer;
   	BEGIN

   		IF pCompoundID is not null THEN
   			--assumes cheminvdb2 is the schema name
   			--SELECT INDEX_NAME INTO vIndexName FROM ALL_INDEXES WHERE TABLE_OWNER = 'CHEMINVDB2' AND TABLE_NAME = 'INV_COMPOUNDS' AND ITYP_OWNER = 'CSCARTRIDGE' AND ITYP_NAME = 'MOLECULEINDEXTYPE';
				--SELECT CSCartridge.Aux.MWTabName('CHEMINVDB2',vIndexName) INTO vTableName FROM DUAL;
				--SELECT Cscartridge.FastIndexAccess.RowIDtoMolWeight(c.rowid,vTableName) INTO vMW FROM inv_compounds c WHERE compound_id = pCompoundID;
        SELECT Cscartridge.MolWeight(base64_cdx) INTO vMW FROM inv_compounds c WHERE compound_id = pCompoundID;
   		ELSIF pRegID is not null THEN
   			--assumes regdb is the schema name
        BEGIN			
     		--SELECT INDEX_NAME INTO vIndexName FROM ALL_INDEXES WHERE TABLE_OWNER = 'REGDB' AND TABLE_NAME = 'STRUCTURES' AND ITYP_OWNER = 'CSCARTRIDGE' AND ITYP_NAME = 'MOLECULEINDEXTYPE';
  			--SELECT CSCartridge.Aux.MWTabName('REGDB',vIndexName) INTO vTableName FROM DUAL;
  			--EXECUTE IMMEDIATE 'SELECT Cscartridge.FastIndexAccess.RowIDtoMolWeight(s.rowid,vTableName) INTO vMW FROM regdb.reg_numbers r, regdb.structures s WHERE r.cpd_internal_id = s.cpd_internal_id AND reg_id = pRegID';
			--EXECUTE IMMEDIATE 'SELECT Cscartridge.MolWeight(base64_CDX) FROM regdb.reg_numbers r, regdb.structures s WHERE r.cpd_internal_id = s.cpd_internal_id AND reg_id = ' || pRegID  INTO vMW;
			EXECUTE IMMEDIATE 'SELECT r.Formula_Weight FROM regdb.batches r WHERE r.reg_internal_id = :1 AND r.Batch_Number = :2 ' 
			INTO vMW
			USING pRegID, pBatchNumber;
		EXCEPTION WHEN OTHERS THEN
			vMW := 0;			
   		END;
		END IF;

  		IF vMW < 0 THEN
 			RAISE invalid_compound;
  		ELSE
  			RETURN vMW;
  		END IF;

  		EXCEPTION
  			WHEN invalid_compound THEN
  				RETURN -1;

   	END;

--GetDensity
    FUNCTION GetDensity(pCompoundID inv_well_compounds.compound_id_fk%TYPE,
    	pRegID inv_well_compounds.reg_id_fk%TYPE,
    	pBatchNumber inv_well_compounds.batch_number_fk%TYPE)
    RETURN number
    IS
   	vDensity number := 1;
	invalid_density exception;
   	BEGIN

   		IF pCompoundID is not null THEN
   			SELECT density INTO vDensity
   				FROM inv_compounds
   				WHERE compound_id = pCompoundID;
   		ELSIF pRegID is not null THEN
   			--need to add this when I know the name of the index for a chemreg system
   			vDensity := 1;
   		END IF;

  		IF vDensity < 0 THEN
 			RAISE invalid_density;
  		ELSE
  			RETURN vDensity;
  		END IF;

  		EXCEPTION
  			WHEN invalid_density THEN
  				RETURN -1;

   	END;

	FUNCTION CalcWeightFromMolarAmount(
		pMW IN inv_compounds.molecular_weight%TYPE,
		pMolarAmount IN inv_wells.molar_amount%TYPE,
		pWeightUnitID IN inv_units.unit_id%TYPE)
	RETURN inv_wells.molar_amount%TYPE
	IS
	vConvertToUnitID inv_units.unit_id%TYPE;
	vConversionFactor inv_unit_conversion.conversion_factor%TYPE;
	vWeight number;
	BEGIN

		vConvertToUnitID := constants.cGramID;
		SELECT conversion_factor INTO vConversionFactor
			FROM inv_unit_conversion
			WHERE from_unit_id_fk = pWeightUnitID
				AND to_unit_id_fk = vConvertToUnitID;
		--RETURN vConversionFactor;
		--!need to do something with sigfigs here
		vWeight := round((pMolarAmount*pMW/vConversionFactor),15);

		RETURN vWeight;

	END CalcWeightFromMolarAmount;


--GetMolarConc
	FUNCTION GetMolarConc(
		pMolarAmount inv_plates.molar_amount%TYPE,
		pSolventVolume1 inv_plates.solvent_volume%TYPE,
		pSolventVolumeUnitID1 inv_units.unit_id%TYPE,
		pSolventVolume2 inv_plates.solvent_volume%TYPE,
		pSolventVolumeUnitID2 inv_units.unit_id%TYPE)
	RETURN number
	IS
	vSolventLiterVolume1 number;
	vSolventLiterVolume2 number;
	vSolventLiterVolumeTotal number;
	vMolarConc number;

	Invalid_Units exception;
	BEGIN
		--convert solvent volume to liters
		vSolventLiterVolume1 := Convert(pSolventVolume1, pSolventVolumeUnitID1, Constants.cLiterID);
		IF vSolventLiterVolume1 = -1 THEN RAISE Invalid_Units; END IF;
		IF pSolventVolume2 is not null THEN
			vSolventLiterVolume2 := Convert(pSolventVolume2, pSolventVolumeUnitID2, Constants.cLiterID);
			vSolventLiterVolumeTotal := vSolventLiterVolume1 + vSolventLiterVolume2;
		ELSE
			vSolventLiterVolumeTotal := vSolventLiterVolume1;
		END IF;
		--get concentration in Molar
		vMolarConc := pMolarAmount/vSolventLiterVolumeTotal;
		IF vMolarConc = -1 THEN RAISE Invalid_Units; END IF;

		RETURN vMolarConc;

		EXCEPTION
			WHEN Invalid_Units THEN
				RETURN -1;

	END GetMolarConc;

--GetAddedSolventVolume
	FUNCTION GetAddedSolventVolume(
		pMolarAmount inv_plates.molar_amount%TYPE,
		pConcentration inv_plates.molar_conc%TYPE,
		pConcentrationUnitID inv_units.unit_id%TYPE,
		pVolumeUnitID inv_units.unit_id%TYPE,
		pCurrSolventVolume inv_plates.solvent_volume%TYPE,
		pCurrSolventVolumeUnitID inv_units.unit_id%TYPE)
	RETURN number
	IS
	vNumMoles number;
	vConcentrationMolar number;
	vTotalVolume inv_plates.solvent_volume%TYPE;
	vAddedVolume inv_plates.solvent_volume%TYPE;
  vCurrLiterSolventVolume inv_plates.solvent_volume%TYPE;
  vIsNegative BOOLEAN := FALSE;
	Invalid_Units exception;
	BEGIN
		--convert concentration to molar
		vConcentrationMolar := Convert(pConcentration, pConcentrationUnitID, Constants.cMolarID);
		IF vConcentrationMolar < 0 THEN RAISE Invalid_Units; END IF;
		--get volume in liters
		vTotalVolume := pMolarAmount/vConcentrationMolar;
		vCurrLiterSolventVolume := Convert(pCurrSolventVolume,pCurrSolventVolumeUnitID,constants.cLiterID);
		vAddedVolume := vTotalVolume - vCurrLiterSolventVolume;

    --make volume positive before the conversion
    IF vAddedVolume < 0 THEN
    	vIsNegative := TRUE;
      vAddedVolume := vAddedVolume * -1;
    END IF;
		--convert liters to desired volume units
		vAddedVolume := Convert(vAddedVolume, Constants.cLiterID, pVolumeUnitID);
		IF vAddedVolume < 0 THEN RAISE Invalid_Units; END IF;
		IF vIsNegative THEN
    	vAddedVolume := vAddedVolume * -1;
    END IF;
		RETURN vAddedVolume;

		EXCEPTION
			WHEN Invalid_Units THEN
				RETURN -1;

	END;


--GetUnitType
	FUNCTION GetUnitType(
		pUnitID inv_units.unit_id%TYPE)
	RETURN inv_unit_types.unit_type_ID%TYPE
	IS
		vUnitTypeID inv_unit_types.unit_type_id%TYPE;
	BEGIN
  	IF pUnitID IS NULL THEN
    	vUnitTypeID := NULL;
    ELSE
  		SELECT unit_type_id INTO vUnitTypeID
  			FROM inv_unit_types, inv_units
  			WHERE unit_id = pUnitID
  				AND unit_type_id_fk = unit_type_id;
    END IF;
		RETURN vUnitTypeID;
	END GetUnitType;

/*
	FUNCTION GetUnitType(
		pUnitID inv_units.unit_id%TYPE)
	RETURN inv_unit_types.unit_type_name%TYPE
	IS
		vUnitTypeName inv_unit_types.unit_type_name%TYPE;
	BEGIN
		SELECT trim(lower(unit_type_name)) INTO vUnitTypeName
			FROM inv_unit_types, inv_units
			WHERE unit_id = pUnitID
				AND unit_type_id_fk = unit_type_id;
		RETURN vUnitTypeName;
	END GetUnitType;
*/
END CHEMCALCS;
/
show errors;