
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
								vChemDataCopy.SolventID := pChemData.SolventID;
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
    vCompoundID number :=null;
  BEGIN

    FOR vWC_rec IN (SELECT ic.compound_id , ic.reg_id_fk, ic.batch_number_fk FROM inv_compounds ic, inv_well_compounds wc WHERE ic.compound_id= wc.compound_id_fk AND wc.well_id_fk = pwellid) 
    LOOP
		  vCount := vCount + 1;
		  IF vWC_rec.reg_id_fk IS NULL THEN 
			vCompoundID := vWC_rec.compound_id;
		  ELSE
			vCompoundID := NULL;
		  END IF;
	 	  vTotal := vTotal + ChemCalcs.GetMW(vCompoundID, vWC_rec.reg_id_fk, vWC_rec.batch_number_fk);
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
		pSolutionVolume IN inv_wells.solution_volume%TYPE,
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
			IF pSolutionVolume = 0 OR pSolutionVolume IS NULL THEN
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
    vMW := pWellData.AvgMW;
		IF vMW = -1 THEN
			RAISE invalid_compound;
		END IF;

		IF pWellData.QtyUnitTypeID = constants.cMassID THEN
			vMolarAmount :=	ChemCalcs.CalcMolarAmountFromMolarConc(pWellData.SolutionVolume, pWellData.SolventVolumeUnitID, pWellData.MolarConc);
      --* if you can convert moles to the qty unit then do that
      vQty:= ChemCalcs.Convert(vMolarAmount, constants.cMoleID, pWellData.QtyRemainingUnitID);
      IF vQty < 0 THEN
	      --* if not try from grams
				vQtyTemp := vMolarAmount * vMW;
				vQty := ChemCalcs.Convert(vQtyTemp, constants.cGramID, pWellData.QtyRemainingUnitID);
      END IF;
		ELSIF pWellData.QtyUnitTypeID = constants.cVolumeID THEN
			--solvated well, wet compound
      vDensity := pWellData.AvgDensity;
			vMolarAmount :=	ChemCalcs.CalcMolarAmountFromMolarConc(pWellData.SolutionVolume, pWellData.SolventVolumeUnitID, pWellData.MolarConc);
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
				vCompoundVolume := ChemCalcs.Convert(pWellData.QtyRemaining, pWellData.QtyRemainingUnitID, constants.cLiterID);
	 			vSolventVolume := ChemCalcs.Convert(pWellData.SolutionVolume, pWellData.SolventVolumeUnitID, constants.cLiterID);
 				vSolventVolume := vSolventVolume - vCompoundVolume;
	 			vSolventVolume := ChemCalcs.Convert(vSolventVolume, constants.cLiterID, pWellData.SolventVolumeUnitID);
 			ELSIF pWellData.QtyUnitTypeID = constants.cMassID THEN
 				vSolventVolume := pWellData.SolutionVolume;
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
								--' use Avg MW from WellData
								vMW := pWellData.AvgMW;
								--vMW := ChemCalcs.GetMW(pWellData.CompoundID, pWellData.RegID, pWellData.BatchNumber);
      --' catch MW calc error
      IF vMW = -1 THEN
      	vMW := 0;
     	END IF;
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
    vAvgQtyRemaining number(18,8); 
    vAvgQtyRemainingUnit inv_plates.qty_unit_fk%TYPE;
    vAvgMolarAmount inv_plates.molar_amount%TYPE;
    vAvgMolarConc inv_plates.molar_conc%TYPE;
    vSolvent inv_plates.solvent_id_fk%TYPE;
    vAvgSolventVolume number(18,8); 
    vAvgSolventVolumeUnit inv_plates.solvent_volume_unit_id_fk%TYPE;
    vAvgSolutionVolume number(18,8); 
    vAvgConcentration number(18,8); 
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
			END IF;

			--determine avg solution volume
	    SELECT count(distinct solvent_volume_unit_id_fk) INTO vCount FROM inv_wells WHERE solution_volume is not null AND plate_id_fk = vPlateIDList_t(i);
	    IF vCount = 1 THEN
				SELECT avg(solution_volume) INTO vAvgSolutionVolume FROM inv_wells where solution_volume is not null and plate_id_fk = vPlateIDList_t(i);
				SELECT distinct solvent_volume_unit_id_fk INTO vAvgSolventVolumeUnit FROM inv_wells where solution_volume is not null AND solvent_volume_unit_id_fk is not null and plate_id_fk = vPlateIDList_t(i) GROUP BY solvent_volume_unit_id_fk;
			ELSE
				vIsFirst := true;
				vTotal := 0;
				vTotalWells := 0;
				vCurrHighWellCount := 0;
				FOR vSolutionVolume_rec in (SELECT avg(solution_volume) AS avgSolutionVolume, solvent_volume_unit_id_fk, count(well_id) AS wellCount FROM inv_wells WHERE solution_volume is not null AND plate_id_fk = vPlateIDList_t(i) GROUP BY solvent_volume_unit_id_fk)
				LOOP
					--use the first qty unit as the display unit
					IF vIsFirst THEN
						vCurrUnit := vSolutionVolume_rec.solvent_volume_unit_id_fk;
						vAvgSolventVolumeUnit := vSolutionVolume_rec.solvent_volume_unit_id_fk;
						vTotal := vSolutionVolume_rec.avgSolutionVolume;
						vCurrHighWellCount := vSolutionVolume_rec.wellCount;
						vIsFirst := false;
					ELSE
						IF vSolutionVolume_rec.wellCount > vCurrHighWellCount THEN
							vCurrUnit := vSolutionVolume_rec.solvent_volume_unit_id_fk;
							vCurrHighWellCount := vSolutionVolume_rec.wellCount;
						END IF;
						vTotal := vTotal + (chemcalcs.convert(vSolutionVolume_rec.avgSolutionVolume,vSolutionVolume_rec.solvent_volume_unit_id_fk,vAvgSolventVolumeUnit) * vSolutionVolume_rec.wellCount);
					END IF;
					vTotalWells := vTotalWells + vSolutionVolume_rec.wellCount;
				END LOOP;
				IF vTotalWells > 0 THEN
					IF vCurrUnit <> vAvgSolventVolumeUnit THEN
						vTotal := chemcalcs.convert(vTotal, vAvgSolventVolumeUnit, vCurrUnit);
						vAvgSolventVolumeUnit := vCurrUnit;
					END IF;
					vAvgSolutionVolume := vTotal/vTotalWells;
				END IF;
			END IF;

			--determine avg concentration
	    --SELECT count(distinct conc_unit_fk) INTO vCount FROM inv_wells WHERE (compound_id_fk is not null OR reg_id_fk is not null) AND plate_id_fk = vPlateIDList_t(i);
			--' determine if any rows have a null unit value
	    SELECT count(distinct conc_unit_fk) INTO vCount FROM inv_wells WHERE concentration is not null AND conc_unit_fk IS NOT NULL AND plate_id_fk = vPlateIDList_t(i);
	    IF vCount = 1 THEN
				--SELECT avg(concentration) INTO vAvgConcentration FROM inv_wells where (compound_id_fk is not null OR reg_id_fk is not null) and plate_id_fk = vPlateIDList_t(i);
				--SELECT distinct conc_unit_fk INTO vAvgConcentrationUnit FROM inv_wells where (compound_id_fk is not null OR reg_id_fk is not null) and plate_id_fk = vPlateIDList_t(i) GROUP BY conc_unit_fk;
				SELECT avg(concentration) INTO vAvgConcentration FROM inv_wells where concentration is not null AND conc_unit_fk IS NOT NULL and plate_id_fk = vPlateIDList_t(i);
				SELECT distinct conc_unit_fk INTO vAvgConcentrationUnit FROM inv_wells where concentration is not null AND conc_unit_fk IS NOT NULL and plate_id_fk = vPlateIDList_t(i) GROUP BY conc_unit_fk;
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
			END IF;

   		UPDATE inv_plates SET
  			qty_remaining = vAvgQtyRemaining,
   			qty_unit_fk = vAvgQtyRemainingUnit,
   			molar_amount = vAvgMolarAmount,
   			molar_conc = vAvgMolarConc,
   			solvent_id_fk = vSolvent,
   			solvent_volume = vAvgSolventVolume,
         solution_volume = vAvgSolutionVolume,
   			solvent_volume_unit_id_fk = vAvgSolventVolumeUnit,
   			concentration = vAvgConcentration,
   			conc_unit_fk = vAvgConcentrationUnit
   		 WHERE
   		 	plate_id = vPlateIDList_t(i) ;
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
          pTSolutionVolume inv_wells.solution_volume%TYPE,
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
    vSSolutionVolume inv_wells.solution_volume%TYPE;
    vSSolventVolumeUnit inv_wells.solvent_volume_unit_id_fk%TYPE;
    vSWeight inv_wells.weight%TYPE;
    vSWeightUnit inv_wells.weight_unit_fk%TYPE;
    vSQtyRemaining_new inv_wells.qty_remaining%TYPE;
    vSSolventVolume_new inv_wells.solvent_volume%TYPE;
    vSSolutionVolume_new inv_wells.solution_volume%TYPE;
    vSWeight_new inv_wells.weight%TYPE;
    vTQtyRemaining_new inv_wells.qty_remaining%TYPE;
    vTQtyRemaining_add inv_wells.qty_remaining%TYPE;
    vTSolventVolume_new inv_wells.solvent_volume%TYPE;
    vTSolventVolume_add inv_wells.solvent_volume%TYPE;
    vTSolutionVolume_new inv_wells.solution_volume%TYPE;
    vTSolutionVolume_add inv_wells.solution_volume%TYPE;
    vTWeight_new inv_wells.weight%TYPE;
    vTWeight_add inv_wells.weight%TYPE;
    vInsufficientQuantity boolean := false;
    vInsufficientSolvent boolean := false;
    vInsufficientSolution BOOLEAN := FALSE;
    vInsufficientWeight boolean := false;
    vSQL varchar2(500);
  BEGIN
       SELECT qty_remaining, qty_unit_fk, solvent_volume, solution_volume, solvent_volume_unit_id_fk, weight, weight_unit_fk
              INTO vSQtyRemaining, vSQtyUnit, vSSolventVolume, vSSolutionVolume, vSSolventVolumeUnit, vSWeight, vSWeightUnit
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
         --' solution_volume
         IF pTSolutionVolume IS NOT NULL AND pTSolutionVolume > 0 AND pTSolventVolumeUnit IS NOT NULL THEN
            IF vSSolutionVolume IS NULL THEN
               vInsufficientSolution := TRUE;
               vSSolutionVolume_new := vSSolutionVolume;
	             vTSolutionVolume_add := 0;
            ELSE
               vSSolutionVolume_new := QuantitySubtraction(vSSolutionVolume, vSSolventVolumeUnit, pTSolutionVolume, pTSolventVolumeUnit);
            	 vTSolutionVolume_add := pTSolutionVolume;
            END IF;
         ELSE
             vSSolutionVolume_new := vSSolutionVolume;
             vTSolutionVolume_add :=0;
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
         IF vSSolutionVolume_new < 0 THEN
            vInsufficientSolution := TRUE;
            vSSolutionVolume_new := 0;
         END IF;
         IF vSWeight_new < 0 THEN
            vInsufficientWeight := TRUE;
            vSWeight_new := 0;
         END IF;
         --update source well
         UPDATE inv_wells SET
                qty_remaining = vSQtyRemaining_new,
                solvent_volume = vSSolventVolume_new,
                solution_volume = vSSolutionVolume_new,
                weight = vSWeight_new
         WHERE well_id = pSWellID;

         --INSERT INTO inv_debug VALUES (vSQtyRemaining_new || ',' ||vSSolventVolume_new || ',' || vSSolutionVolume_new);
         --update target well, if neccessary
         IF lower(pSourceAction) = 'add' then

            UPDATE inv_wells SET
                   qty_remaining = qty_remaining + vTQtyRemaining_add,
                   solvent_volume = solvent_volume + vTSolventVolume_add,
                   solution_volume = solution_volume + vTSolutionVolume_add,
                   weight = weight + vTWeight_add
            WHERE well_id = pTWellID;

         ELSE
         IF vInsufficientQuantity OR vInsufficientSolvent OR vInsufficientSolution OR vInsufficientWeight THEN
            IF vInsufficientQuantity THEN
               vSQL := 'UPDATE inv_wells SET ';    
               vTQtyRemaining_new := chemcalcs.Convert(vSQtyRemaining, vSQtyUnit, pTQtyUnit);
               IF vTQtyRemaining_new < 0 THEN vTQtyRemaining_new := 0; END IF;
               vSQL := vSQL || ' qty_remaining = :1';
               vSQL := vSQL || ' WHERE well_id = :2';
              EXECUTE IMMEDIATE vSQL USING vTQtyRemaining_new, pTWellID;
            END IF;
            IF vInsufficientSolvent THEN
               vSQL := 'UPDATE inv_wells SET ';    
               vTSolventVolume_new := chemcalcs.Convert(vSSolventVolume, vSSolventVolumeUnit, pTSolventVolumeUnit);
               IF vTSolventVolume_new < 0 THEN vTSolventVolume_new := 0; END IF;
               vSQL := vSQL || ' solvent_volume = :1';
               vSQL := vSQL || ' WHERE well_id = :2';
              EXECUTE IMMEDIATE vSQL USING vTSolventVolume_new, pTWellID;
            END IF;
            IF vInsufficientSolution THEN
               vSQL := 'UPDATE inv_wells SET ';    
               vTSolutionVolume_new := chemcalcs.Convert(vSSolutionVolume, vSSolventVolumeUnit, pTSolventVolumeUnit);
               IF vTSolutionVolume_new < 0 THEN vTSolutionVolume_new := 0; END IF;
               vSQL := vSQL || ' solution_volume = :1';
               vSQL := vSQL || ' WHERE well_id = :2';
              EXECUTE IMMEDIATE vSQL USING vTSolutionVolume_new, pTWellID;
            END IF;
            IF vInsufficientWeight THEN
               vSQL := 'UPDATE inv_wells SET ';    
               vTWeight_new := chemcalcs.Convert(vSWeight, vSWeightUnit, pTWeightUnit);
               IF vTWeight_new < 0 THEN vTWeight_new := 0; END IF;
               vSQL := vSQL || ' weight = :1';
               vSQL := vSQL || ' WHERE well_id = :2';
              EXECUTE IMMEDIATE vSQL USING vTWeight_new, pTWellID;
            END IF;            
           END IF;
         END IF;
         IF vInsufficientQuantity OR vInsufficientSolvent OR vInsufficientSolution OR vInsufficientWeight THEN
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
  vCSolutionVolume inv_wells.solvent_volume%TYPE;
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
            SELECT qty_initial, qty_remaining, qty_unit_fk, solvent_volume, solution_volume, solvent_volume_unit_id_fk, weight, weight_unit_fk, concentration, conc_unit_fk
              INTO vCQtyInitial, vCQtyRemaining, vCQtyUnit, vCSolventVolume, vCSolutionVolume, vCSolventVolumeUnit, vCWeight, vCWeightUnit, vCConcentration, vCConcUnit
              FROM inv_wells WHERE well_id = vWellParent_rec.Child_Well_ID_FK;
            vCurrTWellID := vWellParent_rec.Child_Well_ID_FK;
            vSourceAction := 'replace';
         ELSE
             vSourceAction := 'add';
         END IF;
         vDecrementStatus := DecrementWellQuantities(vWellParent_rec.Parent_Well_ID_FK, vWellParent_rec.Child_Well_ID_FK, vCQtyInitial, vCQtyRemaining, vCQtyUnit, vCSolventVolume, vCSolutionVolume, vCSolventVolumeUnit, vCWeight, vCWeightUnit, vSourceAction);
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

	FUNCTION GetSolutionVolume(
  	p_oldSolutionVol inv_wells.solution_volume%TYPE,
    p_newSolutionVol inv_wells.solution_volume%TYPE,
    p_newSolventVol inv_wells.solvent_volume%TYPE,
    p_solventVolUnit inv_wells.solvent_volume_unit_id_fk%TYPE,
    p_newQty inv_wells.qty_remaining%TYPE,
    p_newQtyUnit inv_wells.qty_unit_fk%TYPE,
    p_concentration inv_wells.concentration%TYPE)
	RETURN inv_wells.solution_volume%TYPE
  IS
	  l_solutionVolume inv_wells.solution_volume%TYPE := 0;
  BEGIN
    IF p_newSolutionVol = p_oldSolutionVol OR (p_newSolutionVol IS NULL AND p_oldSolutionVol IS NULL) THEN
  		--* set solution_volume equal to solvent_volume
      l_solutionVolume := p_newSolventVol;
     	IF l_solutionVolume IS NULL THEN l_solutionVolume := 0; END IF;
    	--* if qty is liquid and there is solvent or there is a concentration then change solution volume
    	IF p_newQty > 0 AND p_newQty IS not NULL AND CHEMCALCS.GetUnitType(p_newQtyUnit) = 1 AND (l_solutionVolume > 0 OR p_concentration > 0) THEN
        	l_solutionVolume := l_solutionVolume + CHEMCALCS.Convert((p_newQty), p_newQtyUnit, p_solventVolUnit);
      END IF;
		ELSE
    	l_solutionVolume := p_newSolutionVol;      
    END IF;  
    
    RETURN l_solutionVolume;
  END GetSolutionVolume;
      

END PLATECHEM;
/
show errors;
