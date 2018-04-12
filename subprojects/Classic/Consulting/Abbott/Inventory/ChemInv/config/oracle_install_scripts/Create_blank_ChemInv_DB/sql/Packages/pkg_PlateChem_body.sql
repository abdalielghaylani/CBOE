CREATE OR REPLACE PACKAGE BODY "PLATECHEM"
AS

 --GetChemDataCopy
	FUNCTION GetChemDataCopy(pChemData ChemData)
	RETURN ChemData
	IS
		vChemDataCopy ChemData;
	BEGIN
        vChemDataCopy.CompoundID := pChemData.CompoundID;
        vChemDataCopy.RegID := pChemData.RegID;
        vChemDataCopy.BatchNumber := pChemData.BatchNumber;
        vChemDataCopy.CompoundState := pChemData.CompoundState;
        vChemDataCopy.QtyRemaining := pChemData.QtyRemaining;
        vChemDataCopy.QtyRemainingUnitID := pChemData.QtyRemainingUnitID;
        vChemDataCopy.QtyUnitTypeID := pChemData.QtyUnitTypeID;
		    vChemDataCopy.Concentration := pChemData.Concentration;
		    vChemDataCopy.ConcentrationUnitID := pChemData.ConcentrationUnitID;
        vChemDataCopy.MolarAmount := pChemData.MolarAmount;
        vChemDataCopy.MolarConc := pChemData.MolarConc;
        vChemDataCopy.SolventVolume := pChemData.SolventVolume;
        vChemDataCopy.SolventVolumeUnitID := pChemData.SolventVolumeUnitID;
        vChemDataCopy.AvgMW := pChemData.AvgMW;
        vChemDataCopy.AvgDensity := pChemData.AvgDensity;
        RETURN vChemDataCopy;
	END;

--GetAverageMW
  FUNCTION GetAverageMW(pWellID inv_wells.well_id%TYPE)
  RETURN number
  IS
    vCount integer := 0;
    vTotal number := 0;
    vAverage number := null;
  BEGIN

    FOR vWC_rec IN (SELECT * FROM inv_well_compounds WHERE well_id_fk = pWellID)
    LOOP
			vCount := vCount + 1;
      vTotal := vTotal + ChemCalcs.GetMW(vWC_rec.compound_id_fk, vWC_rec.reg_id_fk, vWC_rec.batch_number_fk);
    END LOOP;

    IF vCount > 0 THEN
    	vAverage := vTotal/vCount;
    END IF;
		RETURN vAverage;

  END GetAverageMW;

--GetAverageDensity
  FUNCTION GetAverageDensity(pWellID inv_wells.well_id%TYPE)
  RETURN number
  IS
    vCount integer := 0;
    vTotal number := 0;
    vAverage number := null;
  BEGIN

    FOR vWC_rec IN (SELECT * FROM inv_well_compounds WHERE well_id_fk = pWellID)
    LOOP
			vCount := vCount + 1;
      vTotal := vTotal + ChemCalcs.GetDensity(vWC_rec.compound_id_fk, vWC_rec.reg_id_fk, vWC_rec.batch_number_fk);
    END LOOP;

    IF vCount > 0 THEN
    	vAverage := vTotal/vCount;
    END IF;
		RETURN vAverage;

  END GetAverageDensity;

--GetWellCompoundState
	FUNCTION GetWellCompoundState(
		pSolventVolume IN inv_wells.solvent_volume%TYPE,
		pQtyRemainingUnitID IN inv_units.unit_id%TYPE)
	RETURN number
	IS
	vReturnValue number := -1;
	vUnitTypeID inv_unit_types.unit_type_id%TYPE;
	invalid_well exception;
	BEGIN
		vUnitTypeID := chemcalcs.GetUnitType(pQtyRemainingUnitID);
		IF vUnitTypeID = 0 OR vUnitTypeID IS NULL THEN
			vReturnValue := platechem.cEmpty;
		ELSE
			IF pSolventVolume = 0 OR pSolventVolume IS NULL THEN
				IF vUnitTypeID = 1 THEN
					vReturnValue := platechem.cWet;
				ELSIF vUnitTypeID = 2 THEN
					vReturnValue := platechem.cDry;
				END IF;
			ELSE
				IF vUnitTypeID = 1 THEN
					vReturnValue := platechem.cSolvatedWet;
				ELSIF vUnitTypeID = 2 THEN
					vReturnValue := platechem.cSolvatedDry;
				END IF;
			END IF;
   	END IF;

		IF vReturnValue < 0 THEN
			RAISE invalid_well;
		ELSE
			RETURN vReturnValue;
		END IF;

		EXCEPTION
			WHEN invalid_well THEN
				RETURN -1;
	END;

--GetQtyFromSolutionVolume
    FUNCTION GetQtyFromSolutionVolume(
    	pWellData ChemData)
    RETURN inv_wells.qty_remaining%TYPE
    IS
	vMW inv_compounds.molecular_weight%TYPE;
	vMolarAmount inv_wells.molar_amount%TYPE := -1;
	vQtyTemp inv_wells.qty_remaining%TYPE;
	vQty inv_wells.qty_remaining%TYPE;
  vDensity inv_compounds.density%TYPE;

	invalid_compound exception;
	BEGIN
		--vMW := ChemCalcs.GetMW(pWellData.CompoundID, pWellData.RegID, pWellData.BatchNumber);
    vMW := pWellData.AvgMW;
		IF vMW = -1 THEN
			RAISE invalid_compound;
		END IF;

		IF pWellData.QtyUnitTypeID = constants.cMassID THEN
			vMolarAmount :=	ChemCalcs.CalcMolarAmountFromMolarConc(pWellData.SolventVolume, pWellData.SolventVolumeUnitID, pWellData.MolarConc);
			vQtyTemp := vMolarAmount * vMW;
			vQty := ChemCalcs.Convert(vQtyTemp, constants.cGramID, pWellData.QtyRemainingUnitID);
		ELSIF pWellData.QtyUnitTypeID = constants.cVolumeID THEN
			--solvated well, wet compound
      --vDensity := ChemCalcs.GetDensity(pWellData.CompoundID, pWellData.RegID, pWellData.BatchNumber);
      vDensity := pWellData.AvgDensity;
			vMolarAmount :=	ChemCalcs.CalcMolarAmountFromMolarConc(pWellData.SolventVolume, pWellData.SolventVolumeUnitID, pWellData.MolarConc);
			vQtyTemp := vMolarAmount * vMW / vDensity;
			vQty := ChemCalcs.Convert(vQtyTemp, constants.cLiterID, pWellData.QtyRemainingUnitID);
		END IF;


		RETURN vQty;
		EXCEPTION
			WHEN invalid_compound THEN
				RETURN -2;
			WHEN OTHERS THEN
				RETURN -5;

    END GetQtyFromSolutionVolume;

--GetSolventVolumeTaken
	-- Given a solution volume, compound, and compound quantity, calculates the solvent volume
	Function GetSolVol_SolutionVolume(
		pWellData ChemData)
    RETURN inv_wells.solvent_volume%TYPE
    IS
		vSolventVolume inv_wells.solvent_volume%TYPE;
		vCompoundVolume inv_wells.qty_remaining%TYPE;
		Invalid_Unit exception;
 	BEGIN
 		IF pWellData.CompoundState = cSolvatedDry OR pWellData.CompoundState = cSolvatedWet THEN
	 		IF pWellData.QtyUnitTypeID = constants.cVolumeID THEN
		 		--vMW := ChemCalcs.GetMW(pWellData.CompoundID, pWellData.RegID, pWellData.BatchNumber);
	 			--vDensity := ChemCalcs.GetDensity(pWellData.CompoundID, pWellData.RegID, pWellData.BatchNumber);
				vCompoundVolume := ChemCalcs.Convert(pWellData.QtyRemaining, pWellData.QtyRemainingUnitID, constants.cLiterID);
	 			vSolventVolume := ChemCalcs.Convert(pWellData.SolventVolume, pWellData.SolventVolumeUnitID, constants.cLiterID);
 				vSolventVolume := vSolventVolume - vCompoundVolume;
	 			vSolventVolume := ChemCalcs.Convert(vSolventVolume, constants.cLiterID, pWellData.SolventVolumeUnitID);
 			ELSIF pWellData.QtyUnitTypeID = constants.cMassID THEN
 				vSolventVolume := pWellData.SolventVolume;
	 		ELSE
 		    	RAISE Invalid_Unit;
 	 		END IF;
 	 	ELSE
			-- if the plate is not solvated then no solvent has been removed
			vSolventVolume := 0;
 	 	END IF;

		RETURN vSolventVolume;

		EXCEPTION
			WHEN Invalid_Unit THEN
				RETURN -1;
			WHEN OTHERS THEN
				RETURN -100;

    END GetSolVol_SolutionVolume;

--GetSolVol_Qty
	-- Given a compound, compound quantity, and molar concentartion calculates the amount of solvent
	FUNCTION GetSolVol_Qty(
		pWellData ChemData)
	RETURN inv_wells.solvent_volume%TYPE
	IS
		vCompoundVolume inv_wells.qty_remaining%TYPE;
		vSolventVolume inv_wells.solvent_volume%TYPE;
    	vMW inv_compounds.molecular_weight%TYPE;
    	vMolarAmount inv_wells.molar_amount%TYPE;

		Invalid_Unit exception;
	BEGIN
 		IF pWellData.CompoundState = cSolvatedWet THEN
			vCompoundVolume := ChemCalcs.Convert(pWellData.QtyRemaining, pWellData.QtyRemainingUnitID, constants.cLiterID);
 			vSolventVolume := ChemCalcs.Convert(pWellData.SolventVolume, pWellData.SolventVolumeUnitID, constants.cLiterID);
			vSolventVolume := vSolventVolume - vCompoundVolume;
		ELSIF pWellData.CompoundState = cSolvatedDry THEN
			vMW := ChemCalcs.GetMW(pWellData.CompoundID, pWellData.RegID, pWellData.BatchNumber);
			vMolarAmount := ChemCalcs.CalcMolarAmountFromWeight(vMW, pWellData.QtyRemaining, pWellData.QtyRemainingUnitID);
			vSolventVolume := vMolarAmount / pWellData.MolarConc;
 	 	ELSE
			-- if the plate is not solvated then no solvent has been removed
			vSolventVolume := 0;
 	 	END IF;

		vSolventVolume := ChemCalcs.Convert(vSolventVolume, constants.cLiterID, pWellData.SolventVolumeUnitID);
		RETURN vSolventVolume;

		EXCEPTION
			WHEN Invalid_Unit THEN
				RETURN -1;
			WHEN OTHERS THEN
				RETURN -100;
	END GetSolVol_Qty;

--GetWellMolarAmount
	FUNCTION GetWellMolarAmount(
    	pQtyRemaining inv_wells.qty_remaining%TYPE,
    	pQtyRemainingUnitID inv_wells.qty_unit_fk%TYPE,
    	pCompoundID inv_compounds.compound_id%TYPE,
    	pRegID inv_well_compounds.reg_id_fk%TYPE,
    	pBatchNumber inv_well_compounds.batch_number_fk%TYPE)
 	RETURN inv_wells.molar_amount%TYPE
 	IS
		vUnitTypeID inv_unit_types.unit_type_id%TYPE;
		vMW number;
		vDensity inv_compounds.density%TYPE;
		vMolarAmount inv_wells.molar_amount%TYPE;
		invalidUnit exception;
 	BEGIN
 		vUnitTypeID := ChemCalcs.GetUnitType(pQtyRemainingUnitID);
   IF pCompoundID IS NULL AND pREGID IS NULL THEN
      vMolarAmount := null;
   ELSE
 		vMW := ChemCalcs.GetMW(pCompoundID, pRegID, pBatchNumber);
     IF vMW < 0 THEN vMW := 0; END IF;
 		IF vUnitTypeID = constants.cVolumeID THEN
	 		vDensity := ChemCalcs.GetDensity(pCompoundID, pRegID, pBatchNumber);
 			vMolarAmount := ChemCalcs.CalcMolarAmountFromVolume(vMW, pQtyRemaining, pQtyRemainingUnitID, vDensity);
 		ELSIF vUnitTypeID = constants.cMassID THEN
 			vMolarAmount := ChemCalcs.CalcMolarAmountFromWeight(vMW, pQtyRemaining, pQtyRemainingUnitID);
 		ELSE
 	    	RAISE invalidUnit;
 	 	END IF;
  END IF;
		RETURN vMolarAmount;

		EXCEPTION
			WHEN invalidUnit THEN
				RETURN -1;
			WHEN OTHERS THEN
				RETURN-100;
 	END GetWellMolarAmount;

--GetWellMolarAmount
	FUNCTION GetWellMolarAmount(
    	pQtyRemaining inv_wells.qty_remaining%TYPE,
    	pQtyRemainingUnitID inv_wells.qty_unit_fk%TYPE,
			pWellID inv_wells.well_id%TYPE)
 	RETURN inv_wells.molar_amount%TYPE
 	IS
		vUnitTypeID inv_unit_types.unit_type_id%TYPE;
		vMW number;
		vDensity inv_compounds.density%TYPE;
		vMolarAmount inv_wells.molar_amount%TYPE;
    vCount integer;
		invalidUnit exception;
 	BEGIN
 		vUnitTypeID := ChemCalcs.GetUnitType(pQtyRemainingUnitID);
    SELECT count(*) INTO vCount FROM inv_well_compounds WHERE well_id_fk = pWellID;
		IF vCount > 0 THEN
   		vMW := PlateChem.GetAverageMW(pWellID);
      IF vMW < 0 THEN vMW := 0; END IF;
   		IF vUnitTypeID = constants.cVolumeID THEN
  	 		vDensity := PlateChem.GetAverageDensity(pWellID);
   			vMolarAmount := ChemCalcs.CalcMolarAmountFromVolume(vMW, pQtyRemaining, pQtyRemainingUnitID, vDensity);
   		ELSIF vUnitTypeID = constants.cMassID THEN
   			vMolarAmount := ChemCalcs.CalcMolarAmountFromWeight(vMW, pQtyRemaining, pQtyRemainingUnitID);
   		ELSE
   	    	RAISE invalidUnit;
   	 	END IF;
    ELSE
      vMolarAmount := null;
    END IF;
		RETURN vMolarAmount;

		EXCEPTION
			WHEN invalidUnit THEN
				RETURN -1;
			WHEN OTHERS THEN
				RETURN-100;
 	END GetWellMolarAmount;

--GetWellMolarAmount
	FUNCTION GetWellMolarAmount(
			pWellID inv_wells.well_id%TYPE)
 	RETURN inv_wells.molar_amount%TYPE
 	IS
    vQtyRemaining inv_wells.qty_remaining%TYPE;
    vQtyRemainingUnitID inv_wells.qty_unit_fk%TYPE;
		vUnitTypeID inv_unit_types.unit_type_id%TYPE;
		vMW number;
		vDensity inv_compounds.density%TYPE;
		vMolarAmount inv_wells.molar_amount%TYPE;
    vCount integer;
		invalidUnit exception;
 	BEGIN
  	SELECT qty_remaining, qty_unit_fk INTO vQtyRemaining, vQtyRemainingUnitID FROM inv_wells WHERE well_id = pWellID;
 		vUnitTypeID := ChemCalcs.GetUnitType(vQtyRemainingUnitID);
    SELECT count(*) INTO vCount FROM inv_well_compounds WHERE well_id_fk = pWellID;
		IF vCount > 0 THEN
   		vMW := PlateChem.GetAverageMW(pWellID);
      IF vMW < 0 THEN vMW := 0; END IF;
   		IF vUnitTypeID = constants.cVolumeID THEN
  	 		vDensity := PlateChem.GetAverageDensity(pWellID);
   			vMolarAmount := ChemCalcs.CalcMolarAmountFromVolume(vMW, vQtyRemaining, vQtyRemainingUnitID, vDensity);
   		ELSIF vUnitTypeID = constants.cMassID THEN
   			vMolarAmount := ChemCalcs.CalcMolarAmountFromWeight(vMW, vQtyRemaining, vQtyRemainingUnitID);
   		ELSE
   	    	RAISE invalidUnit;
   	 	END IF;
    ELSE
      vMolarAmount := null;
    END IF;
		RETURN vMolarAmount;

		EXCEPTION
			WHEN invalidUnit THEN
				RETURN -1;
			WHEN OTHERS THEN
				RETURN-100;
 	END GetWellMolarAmount;


--GetNormalizeUnit
	FUNCTION GetNormalizeUnit(pUnitType inv_unit_types.unit_type_id%TYPE)
	RETURN inv_units.unit_id%TYPE
	IS
		vConvertToUnitID inv_units.unit_id%TYPE;
	BEGIN
		IF pUnitType = constants.cVolumeID THEN
			vConvertToUnitID := constants.cLiterID;
		ELSIF pUnitType = constants.cMassID THEN
			vConvertToUnitID := constants.cGramID;
		END IF;

		RETURN vConvertToUnitID;

	END GetNormalizeUnit;


--Normalize
   	FUNCTION Normalize(
		pWellData ChemData)
	RETURN ChemData
	IS
		vConvertToUnitID inv_units.unit_id%TYPE;
		vNewQtyRemaining inv_wells.qty_remaining%TYPE;
		vNewSolventVolume inv_wells.solvent_volume%TYPE;
		vSourceWellNormalized ChemData;
	BEGIN
		vSourceWellNormalized := GetChemDataCopy(pWellData);

		-- convert qty to grams or liters
		vConvertToUnitID := GetNormalizeUnit(pWellData.QtyUnitTypeID);
		vNewQtyRemaining := chemcalcs.Convert(pWellData.QtyRemaining, pWellData.QtyRemainingUnitID, vConvertToUnitID);
		vSourceWellNormalized.QtyRemaining := vNewQtyRemaining;
		vSourceWellNormalized.QtyRemainingUnitID := vConvertToUnitID;

		-- convert solvent volume to liters
		IF pWellData.CompoundState = platechem.cSolvatedDry OR pWellData.CompoundState = cSolvatedWet THEN
			vNewSolventVolume := chemcalcs.Convert(pWellData.SolventVolume, pWellData.SolventVolumeUnitID, constants.cLiterID);
			vSourceWellNormalized.SolventVolume := vNewSolventVolume;
			vSourceWellNormalized.SolventVolumeUnitID := constants.cLiterID;
		END IF;

    	RETURN vSourceWellNormalized;


	END Normalize;

--DilutePlateToConc
	FUNCTION DilutePlatesToConc(
		pPlateIDList varchar2,
		pSolventIDList varchar2,
		pTargetConcList varchar2,
		pTargetConcUnitIDList varchar2)
	RETURN varchar2
	IS
	vPlateIDList_t STRINGUTILS.t_char;
	vSolventIDList_t STRINGUTILS.t_char;
	vTargetConcList_t STRINGUTILS.t_char;
	vTargetConcUnitIDList_t STRINGUTILS.t_char;

	vSolventVolumeAddedList varchar2(2000) := '';
	vSolventVolumeUnitIDAddedList varchar2(2000) := '';
	vMolarAmount inv_plates.molar_amount%TYPE;
	vCurrSolventVolume inv_plates.solvent_volume%TYPE;
	vCurrSolventVolumeUnitID inv_plates.solvent_volume_unit_id_fk%TYPE;

	vOut varchar2(10);
	BEGIN
		vPlateIDList_t := STRINGUTILS.split(pPlateIDList, ',');
		vSolventIDList_t := STRINGUTILS.split(pSolventIDList, ',');
		vTargetConcList_t := STRINGUTILS.split(pTargetConcList, ',');
		vTargetConcUnitIDList_t := STRINGUTILS.split(pTargetConcUnitIDList, ',');

		--update plate chem values
		SetAggregatedPlateData(pPlateIDList);

		FOR i in vPlateIDList_t.First..vPlateIDList_t.Last
		Loop
			SELECT molar_amount,
				solvent_volume,
				solvent_volume_unit_id_fk
			INTO vMolarAmount,vCurrSolventVolume,vCurrSolventVolumeUnitID
			FROM inv_plates
			WHERE plate_id = vPlateIDList_t(i);
	   		--calc solvent neeeded
/*
	   		insert into inv_debug values('vMolarAmount',vMolarAmount, null);
	   		insert into inv_debug values(vTargetConcList_t(i),null,null);
	   		insert into inv_debug values(vTargetConcUnitIDList_t(i),null,null);
	   		insert into inv_debug values('vCurrSolventVolume',vCurrSolventVolume,null);
	   		insert into inv_debug values('vCurrSolventVolumeUnitID',vCurrSolventVolumeUnitID,null);
	   		insert into inv_debug values('test',ChemCalcs.GetAddedSolventVolume(vMolarAmount, to_number(vTargetConcList_t(i)), to_number(vTargetConcUnitIDList_t(i)), constants.cMicroLiterID, vCurrSolventVolume, vCurrSolventVolumeUnitID),null);
*/
	   		vSolventVolumeAddedList := vSolventVolumeAddedList || to_char(ChemCalcs.GetAddedSolventVolume(vMolarAmount,	to_number(vTargetConcList_t(i)), to_number(vTargetConcUnitIDList_t(i)), constants.cMicroLiterID, vCurrSolventVolume, vCurrSolventVolumeUnitID), '9.9999999EEEE') || ',';
	   		vSolventVolumeUnitIDAddedList := vSolventVolumeUnitIDAddedList || to_char(constants.cMicroLiterID) || ',';
		End Loop;
		vSolventVolumeAddedList := rtrim(vSolventVolumeAddedList,',');
		vSolventVolumeUnitIDAddedList := rtrim(vSolventVolumeUnitIDAddedList,',');

		vOut := SolvatePlates(pPlateIDList,pSolventIDList,vSolventVolumeAddedList,vSolventVolumeUnitIDAddedList,pTargetConcList,pTargetConcUnitIDList);
		--vOut := vSolventVolumeAddedList;

		--update plate chem values
		SetAggregatedPlateData(pPlateIDList);

		RETURN vOut;


	END;

	PROCEDURE SetAggregatedPlateData(pPlateIDList varchar2)
	IS
	vPlateIDList_t STRINGUTILS.t_char;
    vAvgQtyRemaining inv_plates.qty_remaining%TYPE;
    vAvgQtyRemainingUnit inv_plates.qty_unit_fk%TYPE;
    vAvgMolarAmount inv_plates.molar_amount%TYPE;
    vAvgMolarConc inv_plates.molar_conc%TYPE;
    vSolvent inv_plates.solvent_id_fk%TYPE;
    vAvgSolventVolume inv_plates.solvent_volume%TYPE;
    vAvgSolventVolumeUnit inv_plates.solvent_volume_unit_id_fk%TYPE;
    vAvgConcentration inv_plates.concentration%TYPE;
    vAvgConcentrationUnit inv_plates.conc_unit_fk%TYPE;
    vCount integer;
    vIsFirst boolean;
    vTotal number;
    vTotalWells integer;
    vCurrUnit inv_units.unit_id%TYPE;
    vCurrHighWellCount integer;
	BEGIN
		vPlateIDList_t := STRINGUTILS.split(pPlateIDList, ',');
		FOR i in vPlateIDList_t.First..vPlateIDList_t.Last
		LOOP
		    --determine avg quantity remaining
			SELECT count(distinct qty_unit_fk) INTO vCount FROM inv_wells WHERE qty_remaining is not null AND plate_id_fk = vPlateIDList_t(i);
		    IF vCount = 1 THEN
				SELECT avg(qty_remaining) INTO vAvgQtyRemaining FROM inv_wells WHERE qty_remaining is not null AND qty_unit_fk is not null AND plate_id_fk = vPlateIDList_t(i);
				SELECT distinct qty_unit_fk INTO vAvgQtyRemainingUnit FROM inv_wells WHERE qty_remaining is not null AND qty_unit_fk is not null AND plate_id_fk = vPlateIDList_t(i) GROUP BY qty_unit_fk;
			ELSE
				vIsFirst := true;
				vTotal := 0;
				vTotalWells := 0;
				vCurrHighWellCount := 0;
				FOR vQty_rec in (SELECT avg(qty_remaining) AS avgQty, qty_unit_fk, count(well_id) AS wellCount FROM inv_wells WHERE qty_remaining is not null AND plate_id_fk = vPlateIDList_t(i) GROUP BY qty_unit_fk)
				LOOP
					--use the first qty unit as the display unit
					IF vIsFirst THEN
						vCurrUnit := vQty_rec.qty_unit_fk;
						vAvgQtyRemainingUnit := vQty_rec.qty_unit_fk;
						vTotal := vQty_rec.avgQty;
						vCurrHighWellCount := vQty_rec.wellCount;
						vIsFirst := false;
					ELSE
						IF vQty_rec.wellCount > vCurrHighWellCount THEN
							vCurrUnit := vQty_rec.qty_unit_fk;
							vCurrHighWellCount := vQty_rec.wellCount;
						END IF;
						vTotal := vTotal + (chemcalcs.convert(vQty_rec.avgQty,vQty_rec.qty_unit_fk,vAvgQtyRemainingUnit) * vQty_rec.wellCount);
					END IF;
					vTotalWells := vTotalWells + vQty_rec.wellCount;
				END LOOP;
				IF vTotalWells > 0 THEN
					IF vCurrUnit <> vAvgQtyRemainingUnit THEN
						vTotal := chemcalcs.convert(vTotal, vAvgQtyRemainingUnit, vCurrUnit);
						vAvgQtyRemainingUnit := vCurrUnit;
					END IF;
					vAvgQtyRemaining := vTotal/vTotalWells;
				END IF;
				--insert into inv_debug values ('total',vAvgQtyRemaining,null);
			END IF;

			--determine avg molar amount and molar concentration
			--SELECT avg(molar_amount), avg(molar_conc) INTO vAvgMolarAmount, vAvgMolarConc FROM inv_wells where (compound_id_fk is not null OR reg_id_fk is not null) and plate_id_fk = vPlateIDList_t(i);
			SELECT avg(molar_amount), avg(molar_conc) INTO vAvgMolarAmount, vAvgMolarConc FROM inv_wells where molar_amount is not null and plate_id_fk = vPlateIDList_t(i);
			--determine solvent
		    --SELECT count(distinct solvent_id_fk) INTO vCount FROM inv_wells WHERE (compound_id_fk is not null OR reg_id_fk is not null) AND plate_id_fk = vPlateIDList_t(i);
		    SELECT count(distinct solvent_id_fk) INTO vCount FROM inv_wells WHERE solvent_id_fk is not null AND plate_id_fk = vPlateIDList_t(i);
		    IF vCount = 1 THEN
				--SELECT distinct solvent_id_fk INTO vSolvent FROM inv_wells where (compound_id_fk is not null OR reg_id_fk is not null) and plate_id_fk = vPlateIDList_t(i);
				SELECT distinct solvent_id_fk INTO vSolvent FROM inv_wells where solvent_id_fk is not null and plate_id_fk = vPlateIDList_t(i);
			ELSE
				--if multiple solvents then don't set one
				vSolvent := null;
			END IF;

			--determine avg solvent volume
		    --SELECT count(distinct solvent_volume_unit_id_fk) INTO vCount FROM inv_wells WHERE (compound_id_fk is not null OR reg_id_fk is not null) AND plate_id_fk = vPlateIDList_t(i);
		    SELECT count(distinct solvent_volume_unit_id_fk) INTO vCount FROM inv_wells WHERE solvent_volume is not null AND plate_id_fk = vPlateIDList_t(i);
		    IF vCount = 1 THEN
				--SELECT avg(solvent_volume) INTO vAvgSolventVolume FROM inv_wells where (compound_id_fk is not null OR reg_id_fk is not null) and plate_id_fk = vPlateIDList_t(i);
				--SELECT distinct solvent_volume_unit_id_fk INTO vAvgSolventVolumeUnit FROM inv_wells where (compound_id_fk is not null OR reg_id_fk is not null) and plate_id_fk = vPlateIDList_t(i) GROUP BY solvent_volume_unit_id_fk;
				SELECT avg(solvent_volume) INTO vAvgSolventVolume FROM inv_wells where solvent_volume is not null and plate_id_fk = vPlateIDList_t(i);
				SELECT distinct solvent_volume_unit_id_fk INTO vAvgSolventVolumeUnit FROM inv_wells where solvent_volume is not null AND solvent_volume_unit_id_fk is not null and plate_id_fk = vPlateIDList_t(i) GROUP BY solvent_volume_unit_id_fk;
			ELSE
				vIsFirst := true;
				vTotal := 0;
				vTotalWells := 0;
				vCurrHighWellCount := 0;
				--FOR vSolventVolume_rec in (SELECT avg(solvent_volume) AS avgSolventVolume, solvent_volume_unit_id_fk, count(well_id) AS wellCount FROM inv_wells WHERE (compound_id_fk is not null or reg_id_fk is not null) AND plate_id_fk = vPlateIDList_t(i) GROUP BY solvent_volume_unit_id_fk)
				FOR vSolventVolume_rec in (SELECT avg(solvent_volume) AS avgSolventVolume, solvent_volume_unit_id_fk, count(well_id) AS wellCount FROM inv_wells WHERE solvent_volume is not null AND plate_id_fk = vPlateIDList_t(i) GROUP BY solvent_volume_unit_id_fk)
				LOOP
					--use the first qty unit as the display unit
					IF vIsFirst THEN
						vCurrUnit := vSolventVolume_rec.solvent_volume_unit_id_fk;
						vAvgSolventVolumeUnit := vSolventVolume_rec.solvent_volume_unit_id_fk;
						vTotal := vSolventVolume_rec.avgSolventVolume;
						vCurrHighWellCount := vSolventVolume_rec.wellCount;
						vIsFirst := false;
					ELSE
						IF vSolventVolume_rec.wellCount > vCurrHighWellCount THEN
							vCurrUnit := vSolventVolume_rec.solvent_volume_unit_id_fk;
							vCurrHighWellCount := vSolventVolume_rec.wellCount;
						END IF;
						vTotal := vTotal + (chemcalcs.convert(vSolventVolume_rec.avgSolventVolume,vSolventVolume_rec.solvent_volume_unit_id_fk,vAvgSolventVolumeUnit) * vSolventVolume_rec.wellCount);
					END IF;
					vTotalWells := vTotalWells + vSolventVolume_rec.wellCount;
				END LOOP;
				IF vTotalWells > 0 THEN
					IF vCurrUnit <> vAvgSolventVolumeUnit THEN
						vTotal := chemcalcs.convert(vTotal, vAvgSolventVolumeUnit, vCurrUnit);
						vAvgSolventVolumeUnit := vCurrUnit;
					END IF;
					vAvgSolventVolume := vTotal/vTotalWells;
				END IF;
				--insert into inv_debug values ('total',vAvgSolventVolume,null);
			END IF;

			--determine avg concentration
		    --SELECT count(distinct conc_unit_fk) INTO vCount FROM inv_wells WHERE (compound_id_fk is not null OR reg_id_fk is not null) AND plate_id_fk = vPlateIDList_t(i);
		    SELECT count(distinct conc_unit_fk) INTO vCount FROM inv_wells WHERE concentration is not null AND plate_id_fk = vPlateIDList_t(i);
		    IF vCount = 1 THEN
				--SELECT avg(concentration) INTO vAvgConcentration FROM inv_wells where (compound_id_fk is not null OR reg_id_fk is not null) and plate_id_fk = vPlateIDList_t(i);
				--SELECT distinct conc_unit_fk INTO vAvgConcentrationUnit FROM inv_wells where (compound_id_fk is not null OR reg_id_fk is not null) and plate_id_fk = vPlateIDList_t(i) GROUP BY conc_unit_fk;
				SELECT avg(concentration) INTO vAvgConcentration FROM inv_wells where concentration is not null and plate_id_fk = vPlateIDList_t(i);
				SELECT distinct conc_unit_fk INTO vAvgConcentrationUnit FROM inv_wells where concentration is not null and plate_id_fk = vPlateIDList_t(i) GROUP BY conc_unit_fk;
			ELSE
				vIsFirst := true;
				vTotal := 0;
				vTotalWells := 0;
				vCurrHighWellCount := 0;
				--FOR vConcentration_rec in (SELECT avg(concentration) AS avgConcentration, conc_unit_fk, count(well_id) AS wellCount FROM inv_wells WHERE (compound_id_fk is not null or reg_id_fk is not null) AND plate_id_fk = vPlateIDList_t(i) GROUP BY conc_unit_fk)
				FOR vConcentration_rec in (SELECT avg(concentration) AS avgConcentration, conc_unit_fk, count(well_id) AS wellCount FROM inv_wells WHERE concentration is not null AND plate_id_fk = vPlateIDList_t(i) GROUP BY conc_unit_fk)
				LOOP
					--use the first qty unit as the display unit
					IF vIsFirst THEN
						vCurrUnit := vConcentration_rec.conc_unit_fk;
						vAvgConcentrationUnit := vConcentration_rec.conc_unit_fk;
						vTotal := vConcentration_rec.avgConcentration;
						vCurrHighWellCount := vConcentration_rec.wellCount;
						vIsFirst := false;
					ELSE
						IF vConcentration_rec.wellCount > vCurrHighWellCount THEN
							vCurrUnit := vConcentration_rec.conc_unit_fk;
							vCurrHighWellCount := vConcentration_rec.wellCount;
						END IF;
						vTotal := vTotal + (chemcalcs.convert(vConcentration_rec.avgConcentration,vConcentration_rec.conc_unit_fk,vAvgConcentrationUnit) * vConcentration_rec.wellCount);
					END IF;
					vTotalWells := vTotalWells + vConcentration_rec.wellCount;
				END LOOP;
				IF vTotalWells > 0 THEN
					IF vCurrUnit <> vAvgConcentrationUnit THEN
						vTotal := chemcalcs.convert(vTotal, vAvgConcentrationUnit, vCurrUnit);
						vAvgConcentrationUnit := vCurrUnit;
					END IF;
					vAvgConcentration := vTotal/vTotalWells;
				END IF;
				--insert into inv_debug values ('total',vAvgConcentration,null);
			END IF;

    		UPDATE inv_plates SET
    			qty_remaining = vAvgQtyRemaining,
    			qty_unit_fk = vAvgQtyRemainingUnit,
    			molar_amount = vAvgMolarAmount,
    			molar_conc = vAvgMolarConc,
    			solvent_id_fk = vSolvent,
    			solvent_volume = vAvgSolventVolume,
    			solvent_volume_unit_id_fk = vAvgSolventVolumeUnit,
    			concentration = vAvgConcentration,
    			conc_unit_fk = vAvgConcentrationUnit
    		 WHERE
    		 	plate_id = vPlateIDList_t(i) ;
		    /*
    		UPDATE inv_plates SET
    			(qty_remaining, molar_amount, molar_conc, solvent_volume, concentration) = (select avg(qty_remaining), avg(molar_amount), avg(molar_conc), avg(chemcalcs.convert(solvent_volume,solvent_volume_unit_id_fk,4)), avg(concentration) from inv_wells where plate_id_fk = vPlateIDList_t(i) and compound_id_fk is not null),
    			(solvent_id_fk, qty_unit_fk, conc_unit_fk) = (select solvent_id_fk, qty_unit_fk, conc_unit_fk from inv_wells where plate_id_fk = vPlateIDList_t(i) and compound_id_fk is not null group by solvent_id_fk, qty_unit_fk, conc_unit_fk),
				solvent_volume_unit_id_fk = 4
    		 WHERE
    		 	plate_id = vPlateIDList_t(i) ;
    		 */
    	END LOOP;
	END;

    FUNCTION ChangeWellQty(pSrcWell inv_wells.well_id%TYPE, pChangeQty inv_wells.qty_remaining%TYPE, pChangeUnit inv_wells.qty_unit_fk%TYPE)
	RETURN number
	IS
		vSrcQty inv_wells.qty_remaining%TYPE;
		vSrcUnit inv_wells.qty_unit_fk%TYPE;
		vSrcUnitType inv_unit_types.unit_type_id%TYPE;
		vSrcSolventQty inv_wells.solvent_volume%TYPE;
		vSrcSolventUnit inv_wells.solvent_volume_unit_id_fk%TYPE;
		vChangeUnitType inv_unit_types.unit_type_id%TYPE;
		vConvertToUnitID inv_units.unit_id%TYPE;
		vNSrcQty inv_wells.qty_remaining%TYPE;
		vNSrcSolventQty inv_wells.solvent_volume%TYPE;
		vNChangeQty inv_wells.qty_remaining%TYPE;
		vNewQty inv_wells.qty_remaining%TYPE;
		vNewSrcSolventQty inv_wells.solvent_volume%TYPE;

		incompatible_unit_types exception;
	BEGIN
		--get source qty
		SELECT qty_remaining, qty_unit_fk INTO vSrcQty, vSrcUnit FROM inv_wells WHERE well_id = pSrcWell;
		--make sure qty types are compatible
		vSrcUnitType := chemcalcs.GetUnitType(vSrcUnit);
		vChangeUnitType := chemcalcs.GetUnitType(pChangeUnit);
		IF vSrcUnitType <> vChangeUnitType THEN
			RAISE incompatible_unit_types;
		END IF;

		vConvertToUnitID := GetNormalizeUnit(vSrcUnitType);
		--normalize source qty
		vNSrcQty := chemcalcs.Convert(vSrcQty, vSrcUnit, vConvertToUnitID);
		--normalize change qty
		vNChangeQty := chemcalcs.Convert(pChangeQty, pChangeUnit, vConvertToUnitID);
		--do math
		vNewQty := vNSrcQty + vNChangeQty;
		--update source qty with new value in change qty units
		UPDATE inv_wells SET qty_remaining = chemcalcs.Convert(vNewQty, vConvertToUnitID, pChangeUnit), qty_unit_fk = pChangeUnit WHERE well_id = pSrcWell;


		RETURN 1;


		--exceptions
		EXCEPTION
			WHEN incompatible_unit_types THEN
				RETURN -100;
			WHEN OTHERS THEN
				RETURN -1;

	END ChangeWellQty;

 -- returns A(U1)-B(U2) in units of U1
 FUNCTION QuantitySubtraction(
          pMinuend number,
          pMinuendUnit inv_units.unit_id%TYPE,
          pSubtrahend number,
          pSubtrahendUnit inv_units.unit_id%TYPE)
 RETURN number
 IS
   --N:Normalized, M:Minuend, S:Subtrahend
	  vMUnitType inv_unit_types.unit_type_id%TYPE;
   vSUnitType inv_unit_types.unit_type_id%TYPE;
   vSameUnit boolean := FALSE;
   vNUnit inv_units.unit_id%TYPE;
   vNMinuend number;
   vNSubtrahend number;
   vNDifference number;
   vDifference number;
  	incompatible_unit_types exception;
   invalid_minuend exception;
   invalid_subtrahend exception;
   invalid_conversion exception;
 BEGIN
      --validate input
      IF pSubtrahend IS NOT NULL AND pSubtrahendUnit IS NOT NULL THEN
        IF pMinuend IS NULL THEN
           RAISE invalid_minuend;
        END IF;
      ELSE
        RAISE invalid_subtrahend;
      END IF;
      --make unit types are compatible
      vMUnitType := chemcalcs.GetUnitType(pMinuendUnit);
      vSUnitType := chemcalcs.GetUnitType(pSubtrahendUnit);
      IF vMUnitType <> vSUnitType THEN
         RAISE incompatible_unit_types;
      END IF;
      --check for same units
      IF pMinuendUnit = pSubtrahendUnit THEN vSameUnit := TRUE; END IF;
      IF vSameUnit THEN
         vDifference := pMinuend - pSubtrahend;
      ELSE
         --if not then normalize
        	vNUnit := GetNormalizeUnit(vMUnitType);
         vNMinuend := chemcalcs.Convert(pMinuend, pMinuendUnit, vNUnit);
         IF vNMinuend = -1 OR vNMinuend = -2 THEN RAISE invalid_conversion; END IF;
         vNSubtrahend := chemcalcs.Convert(pSubtrahend, pSubtrahendUnit, vNUnit);
         IF vNSubtrahend = -1 OR vNSubtrahend = -2 THEN RAISE invalid_conversion; END IF;
         vNDifference := vNMinuend - vNSubtrahend;
         --convert back
         vDifference := chemcalcs.Convert(vNDifference, vNUnit, pMinuendUnit);
      END IF;
      RETURN vDifference;
 		   EXCEPTION
			   WHEN incompatible_unit_types THEN
				       RETURN -133;
      WHEN invalid_minuend THEN
           RETURN pMinuend;
      WHEN invalid_subtrahend THEN
           RETURN pMinuend;
      WHEN invalid_conversion THEN
           RETURN pMinuend;
			   WHEN OTHERS THEN
				       RETURN -1;

 END QuantitySubtraction;
 FUNCTION DecrementWellQuantities(
          pSWellID inv_wells.well_id%TYPE,
          pTWellID inv_wells.well_id%TYPE,
          pTQtyInitial inv_wells.qty_initial%TYPE,
          pTQtyRemaining inv_wells.qty_remaining%TYPE,
          pTQtyUnit inv_wells.qty_unit_fk%TYPE,
          pTSolventVolume inv_wells.solvent_volume%TYPE,
          pTSolventVolumeUnit inv_wells.solvent_volume_unit_id_fk%TYPE,
          pTWeight inv_wells.weight%TYPE,
          pTWeightUnit inv_wells.weight_unit_fk%TYPE,
          pSourceAction varchar2 := 'replace')
  RETURN varchar2
  IS
    --S:Source, T:Target, N:Normalized
    vSQtyRemaining inv_wells.qty_remaining%TYPE;
    vSQtyUnit inv_wells.qty_unit_fk%TYPE;
    vSSolventVolume inv_wells.solvent_volume%TYPE;
    vSSolventVolumeUnit inv_wells.solvent_volume_unit_id_fk%TYPE;
    vSWeight inv_wells.weight%TYPE;
    vSWeightUnit inv_wells.weight_unit_fk%TYPE;
    vSQtyRemaining_new inv_wells.qty_remaining%TYPE;
    vSSolventVolume_new inv_wells.solvent_volume%TYPE;
    vSWeight_new inv_wells.weight%TYPE;
    vTQtyRemaining_new inv_wells.qty_remaining%TYPE;
    vTQtyRemaining_add inv_wells.qty_remaining%TYPE;
    vTSolventVolume_new inv_wells.solvent_volume%TYPE;
    vTSolventVolume_add inv_wells.solvent_volume%TYPE;
    vTWeight_new inv_wells.weight%TYPE;
    vTWeight_add inv_wells.weight%TYPE;
    vInsufficientQuantity boolean := false;
    vInsufficientSolvent boolean := false;
    vInsufficientWeight boolean := false;
    vSQL varchar2(500);
    vNumClauses number := 0;
  BEGIN
       SELECT qty_remaining, qty_unit_fk, solvent_volume, solvent_volume_unit_id_fk, weight, weight_unit_fk
              INTO vSQtyRemaining, vSQtyUnit, vSSolventVolume, vSSolventVolumeUnit, vSWeight, vSWeightUnit
              FROM inv_wells WHERE well_id = pSWellID;
         --qty
         IF pTQtyRemaining IS NOT NULL AND pTQtyRemaining > 0 AND pTQtyUnit IS NOT NULL THEN
            IF vSQtyRemaining IS NULL THEN
               vInsufficientQuantity := TRUE;
               vSQtyRemaining_new := vSQtyRemaining;
	             vTQtyRemaining_add := 0;
            ELSE
               vSQtyRemaining_new := QuantitySubtraction(vSQtyRemaining, vSQtyUnit, pTQtyRemaining, pTQtyUnit);
            vTQtyRemaining_add := pTQtyRemaining;
            END IF;
         ELSE
             vSQtyRemaining_new := vSQtyRemaining;
             vTQtyRemaining_add := 0;
         END IF;
         --solvent_volume
         IF pTSolventVolume IS NOT NULL AND pTSolventVolume > 0 AND pTSolventVolumeUnit IS NOT NULL THEN
            IF vSSolventVolume IS NULL THEN
               vInsufficientSolvent := TRUE;
               vSSolventVolume_new := vSSolventVolume;
	             vTSolventVolume_add := 0;
            ELSE
               vSSolventVolume_new := QuantitySubtraction(vSSolventVolume, vSSolventVolumeUnit, pTSolventVolume, pTSolventVolumeUnit);
            vTSolventVolume_add := pTSolventVolume;
            END IF;
         ELSE
             vSSolventVolume_new := vSSolventVolume;
             vTSolventVolume_add :=0;
         END IF;
         --weight
         IF pTWeight IS NOT NULL AND pTWeight > 0 AND pTWeightUnit IS NOT NULL THEN
            IF vSWeight IS NULL THEN
               vInsufficientWeight := TRUE;
               vSWeight_new := vSSolventVolume;
               vTWeight_add := 0;
            ELSE
               vSWeight_new := QuantitySubtraction(vSWeight, vSWeightUnit, pTWeight, pTWeightUnit);
            vTWeight_add := pTWeight;
            END IF;
         ELSE
             vSWeight_new := vSWeight;
             vTWeight_add := 0;
         END IF;
         --logic for negative value
         IF vSQtyRemaining_new < 0 THEN
            vInsufficientQuantity := TRUE;
            vSQtyRemaining_new := 0;
         END IF;
         IF vSSolventVolume_new < 0 THEN
            vInsufficientSolvent := TRUE;
            vSSolventVolume_new := 0;
         END IF;
         IF vSWeight_new < 0 THEN
            vInsufficientWeight := TRUE;
            vSWeight_new := 0;
         END IF;
         --update source well
         UPDATE inv_wells SET
                qty_remaining = vSQtyRemaining_new,
                solvent_volume = vSSolventVolume_new,
                weight = vSWeight_new
         WHERE well_id = pSWellID;
         --update target well, if neccessary
         IF lower(pSourceAction) = 'add' then

            UPDATE inv_wells SET
                   qty_remaining = qty_remaining + vTQtyRemaining_add,
                   solvent_volume = solvent_volume + vTSolventVolume_add,
                   weight = weight + vTWeight_add
            WHERE well_id = pTWellID;

         ELSE
         IF vInsufficientQuantity OR vInsufficientSolvent OR vInsufficientWeight THEN
            vSQL := 'UPDATE inv_wells SET ';
            IF vInsufficientQuantity THEN
               vTQtyRemaining_new := chemcalcs.Convert(vSQtyRemaining, vSQtyUnit, pTQtyUnit);
               IF vTQtyRemaining_new < 0 THEN vTQtyRemaining_new := 0; END IF;
               vSQL := vSQL || ' qty_remaining = ' || vTQtyRemaining_new;
               vNumClauses := vNumClauses + 1;
            END IF;
            IF vInsufficientSolvent THEN
               vTSolventVolume_new := chemcalcs.Convert(vSSolventVolume, vSSolventVolumeUnit, pTSolventVolumeUnit);
               IF vTSolventVolume_new < 0 THEN vTSolventVolume_new := 0; END IF;
               IF vNumClauses > 0 THEN vSQL := vSQL || ', '; END IF;
               vSQL := vSQL || ' solvent_volume = ' || vTSolventVolume_new;
               vNumClauses := vNumClauses + 1;
            END IF;
            IF vInsufficientWeight THEN
               vTWeight_new := chemcalcs.Convert(vSWeight, vSWeightUnit, pTWeightUnit);
               IF vTWeight_new < 0 THEN vTWeight_new := 0; END IF;
               IF vNumClauses > 0 THEN vSQL := vSQL || ', '; END IF;
               vSQL := vSQL || ' weight = ' || vTWeight_new;
               vNumClauses := vNumClauses + 1;
            END IF;
            vSQL := vSQL || ' WHERE well_id = ' || pTWellID;
            EXECUTE IMMEDIATE vSQL;
           END IF;
         END IF;
         IF vInsufficientQuantity OR vInsufficientSolvent OR vInsufficientWeight THEN
            RETURN '-132';
         ELSE
            RETURN '1';
         END IF;
         --RETURN vSQtyRemaining_new || ':' || vSSolventVolume_new;
    		--exceptions
    		/*
      EXCEPTION
    			WHEN OTHERS THEN
    				RETURN '-1';
      */
  END DecrementWellQuantities;
  FUNCTION DecrementParentQuantities(pChildPlateID inv_plates.plate_id%TYPE)
  RETURN inv_plates.plate_id%TYPE
  IS
    CURSOR vWellParent_cur IS
           SELECT wp.*
                  FROM inv_well_parent wp, inv_wells
                  WHERE well_id = child_well_id_fk
                        AND plate_id_fk = pChildPlateID;
    CURSOR vPlateParent_cur IS
           SELECT pp.*
                  FROM inv_plate_parent pp
                  WHERE child_plate_id_fk = pChildPlateID;
  --C:Child, P:Parent
  vCQtyInitial inv_wells.qty_initial%TYPE;
  vCQtyRemaining inv_wells.qty_remaining%TYPE;
  vCQtyUnit inv_wells.qty_unit_fk%TYPE;
  vCSolventVolume inv_wells.solvent_volume%TYPE;
  vCSolventVolumeUnit inv_wells.solvent_volume_unit_id_fk%TYPE;
  vCWeight inv_wells.weight%TYPE;
  vCWeightUnit inv_wells.weight_unit_fk%TYPE;
  vCConcentration inv_wells.concentration%TYPE;
  vCConcUnit inv_wells.concentration%TYPE;
  vCurrTWellID inv_wells.well_id%TYPE := 0;
  vDecrementStatus varchar2(200);
  --Source Action determines whether target quantities are calc'd by either replacing with the Source quantity or whether the Source quantity is added to the target.  Neceassary for mixture wells.
  vSourceAction varchar2(50);
  BEGIN
       FOR vWellParent_rec IN vWellParent_cur LOOP
         --get current well qty_initial, qty_unit, concentration, conc_unit, solvent_volume, solvent_volume_unit, weight, weight_unit
         IF vCurrTWellID <> vWellParent_rec.Child_Well_ID_FK THEN
            SELECT qty_initial, qty_remaining, qty_unit_fk, solvent_volume, solvent_volume_unit_id_fk, weight, weight_unit_fk, concentration, conc_unit_fk
              INTO vCQtyInitial, vCQtyRemaining, vCQtyUnit, vCSolventVolume, vCSolventVolumeUnit, vCWeight, vCWeightUnit, vCConcentration, vCConcUnit
              FROM inv_wells WHERE well_id = vWellParent_rec.Child_Well_ID_FK;
            vCurrTWellID := vWellParent_rec.Child_Well_ID_FK;
            vSourceAction := 'replace';
         ELSE
             vSourceAction := 'add';
         END IF;
         vDecrementStatus := DecrementWellQuantities(vWellParent_rec.Parent_Well_ID_FK, vWellParent_rec.Child_Well_ID_FK, vCQtyInitial, vCQtyRemaining, vCQtyUnit, vCSolventVolume, vCSolventVolumeUnit, vCWeight, vCWeightUnit, vSourceAction);
      	END LOOP;
       --set parent plate aggregated values
       FOR vPlateParent_rec IN vPlateParent_cur
       LOOP
           SetAggregatedPlateData(vPlateParent_rec.Parent_Plate_ID_FK);
       END LOOP;
       --set child plate aggregated values
       SetAggregatedPlateData(pChildPlateID);
       RETURN pChildPlateID;
  END DecrementParentQuantities;
  FUNCTION DecrementParentQuantities(pChildPlateIDs varchar2)
  RETURN varchar2
  IS
    vPlateIDList_t STRINGUTILS.t_char;
    vPlateID inv_plates.plate_id%TYPE;
  BEGIN
  	vPlateIDList_t := STRINGUTILS.split(pChildPlateIDs, ',');
		 FOR i in vPlateIDList_t.First..vPlateIDList_t.Last
		 LOOP
       vPlateID := DecrementParentQuantities(to_number(vPlateIDList_t(i)));
   END LOOP;
   RETURN pChildPlateIDs;
  END;

END PLATECHEM;
/
show errors;
