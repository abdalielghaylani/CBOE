CREATE OR REPLACE PACKAGE "CHEMCALCS"
AS
	FUNCTION Convert(
		pAmount number,
		pFromUnitID inv_units.unit_id%TYPE,
		pToUnitID inv_units.unit_id%TYPE)
	RETURN number;

    FUNCTION GetMW(
    	pCompoundID inv_well_compounds.compound_id_fk%TYPE,
    	pRegID inv_well_compounds.reg_id_fk%TYPE,
    	pBatchNumber inv_well_compounds.batch_number_fk%TYPE)
    RETURN number;

	FUNCTION CalcMolarAmountFromMolarConc(
		pSolventVolume IN inv_wells.solvent_volume%TYPE,
		pSolventVolumeUnitID IN inv_wells.solvent_volume_unit_id_fk%TYPE,
		pMolarConc IN inv_wells.molar_conc%TYPE)
	RETURN number;

	FUNCTION CalcMolarAmountFromWeight(
		pMW IN inv_compounds.molecular_weight%TYPE,
		pQtyRemaining IN inv_wells.qty_remaining%TYPE,
		pQtyRemainingUnitID IN inv_wells.qty_remaining%TYPE)
	RETURN number;

	FUNCTION CalcMolarAmountFromVolume(
		pMW IN inv_compounds.molecular_weight%TYPE,
		pQtyRemaining IN inv_wells.qty_remaining%TYPE,
		pQtyRemainingUnitID IN inv_wells.qty_remaining%TYPE,
		pDensity IN inv_compounds.density%TYPE)
	RETURN number;

	FUNCTION CalcWeightFromMolarAmount(
		pMW IN inv_compounds.molecular_weight%TYPE,
		pMolarAmount IN inv_wells.molar_amount%TYPE,
		pWeightUnitID IN inv_units.unit_id%TYPE)
	RETURN inv_wells.molar_amount%TYPE;

    FUNCTION GetDensity(
    	pCompoundID inv_well_compounds.compound_id_fk%TYPE,
    	pRegID inv_well_compounds.reg_id_fk%TYPE,
    	pBatchNumber inv_well_compounds.batch_number_fk%TYPE)
    RETURN number;

	FUNCTION GetMolarConc(
		pMolarAmount inv_plates.molar_amount%TYPE,
		pSolventVolume1 inv_plates.solvent_volume%TYPE,
		pSolventVolumeUnitID1 inv_units.unit_id%TYPE,
		pSolventVolume2 inv_plates.solvent_volume%TYPE,
		pSolventVolumeUnitID2 inv_units.unit_id%TYPE)
	RETURN number;

	FUNCTION GetAddedSolventVolume(
		pMolarAmount inv_plates.molar_amount%TYPE,
		pConcentration inv_plates.molar_conc%TYPE,
		pConcentrationUnitID inv_units.unit_id%TYPE,
		pVolumeUnitID inv_units.unit_id%TYPE,
		pCurrSolventVolume inv_plates.solvent_volume%TYPE,
		pCurrSolventVolumeUnitID inv_units.unit_id%TYPE)
	RETURN number;

	FUNCTION GetUnitType(
		pUnitID inv_units.unit_id%TYPE)
	RETURN inv_unit_types.unit_type_id%TYPE;

END CHEMCALCS;
/
show errors;