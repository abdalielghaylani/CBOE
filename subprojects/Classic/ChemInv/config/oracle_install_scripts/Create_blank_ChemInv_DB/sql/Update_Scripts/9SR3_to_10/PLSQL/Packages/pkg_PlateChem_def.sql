CREATE OR REPLACE PACKAGE "PLATECHEM"
AS
	cEmpty				CONSTANT INTEGER := 0;
	cDry					CONSTANT INTEGER := 1;
	cWet        	CONSTANT INTEGER := 2;
	cSolvatedDry	CONSTANT INTEGER := 3;
  cSolvatedWet	CONSTANT INTEGER := 4;

	TYPE ChemData IS RECORD (
		CompoundID inv_compounds.compound_id%TYPE,
		RegID inv_well_compounds.reg_id_fk%TYPE,
		BatchNumber inv_well_compounds.batch_number_fk%TYPE,
		CompoundState number,
		QtyRemaining inv_wells.qty_remaining%TYPE,
		QtyRemainingUnitID inv_units.unit_id%TYPE,
		QtyUnitTypeID inv_unit_types.unit_type_id%TYPE,
    Concentration inv_wells.concentration%TYPE,
    ConcentrationUnitID inv_wells.conc_unit_fk%TYPE,
		MolarAmount inv_wells.molar_amount%TYPE,
		MolarConc inv_wells.molar_conc%TYPE,
		SolventID inv_wells.solvent_id_fk%TYPE,
		SolventVolume inv_wells.solvent_volume%TYPE,
    SolutionVolume inv_wells.solution_volume%TYPE,
		SolventVolumeUnitID inv_units.unit_id%TYPE,
    AvgMW inv_compounds.molecular_weight%TYPE,
    AvgDensity inv_compounds.density%TYPE);

	FUNCTION GetChemDataCopy(
		pChemData ChemData)
	RETURN ChemData;

  FUNCTION GetAverageMW(pWellID inv_wells.well_id%TYPE)
  RETURN number;

  FUNCTION GetAverageDensity(pWellID inv_wells.well_id%TYPE)
  RETURN NUMBER;


	FUNCTION GetWellCompoundState(
		pSolutionVolume IN inv_wells.solution_volume%TYPE,
		pQtyRemainingUnitID IN inv_units.unit_id%TYPE)
	RETURN number;

	FUNCTION Normalize(
		pWellData ChemData)
	RETURN ChemData;

	FUNCTION GetWellMolarAmount(
    	pQtyRemaining inv_wells.qty_remaining%TYPE,
    	pQtyRemainingUnitID inv_wells.qty_unit_fk%TYPE,
    	pCompoundID inv_compounds.compound_id%TYPE,
    	pRegID inv_well_compounds.reg_id_fk%TYPE,
    	pBatchNumber inv_well_compounds.batch_number_fk%TYPE)
 	RETURN inv_wells.molar_amount%TYPE;

	FUNCTION GetWellMolarAmount(
    	pQtyRemaining inv_wells.qty_remaining%TYPE,
    	pQtyRemainingUnitID inv_wells.qty_unit_fk%TYPE,
			pWellID inv_wells.well_id%TYPE)
 	RETURN inv_wells.molar_amount%TYPE;

	FUNCTION GetWellMolarAmount(
			pWellID inv_wells.well_id%TYPE)
 	RETURN inv_wells.molar_amount%TYPE;

  FUNCTION GetQtyFromSolutionVolume(
    	pWellData ChemData)
    RETURN inv_wells.qty_remaining%TYPE;

	FUNCTION GetSolVol_SolutionVolume(
		pWellData ChemData)
    RETURN inv_wells.solvent_volume%TYPE;

	FUNCTION GetSolVol_Qty(
		pWellData ChemData)
	RETURN inv_wells.solvent_volume%TYPE;

	--given a target concentration, calculate the amount of solvent needed and solvate a list of plates with a given solvent
	FUNCTION DilutePlatesToConc(
		pPlateIDList varchar2,
		pSolventIDList varchar2,
		pTargetConcList varchar2,
		pTargetConcUnitIDList varchar2)
	RETURN varchar2;

	--sets the plate values for molar_amount, molar_conc, solvent_volume, solvent_id_fk
	PROCEDURE SetAggregatedPlateData(
		pPlateIDList varchar2);

    FUNCTION ChangeWellQty(pSrcWell inv_wells.well_id%TYPE, pChangeQty inv_wells.qty_remaining%TYPE, pChangeUnit inv_wells.qty_unit_fk%TYPE) RETURN number;

 FUNCTION QuantitySubtraction(
          pMinuend number,
          pMinuendUnit inv_units.unit_id%TYPE,
          pSubtrahend number,
          pSubtrahendUnit inv_units.unit_id%TYPE)
 RETURN number;

 FUNCTION DecrementParentQuantities(pChildPlateID inv_plates.plate_id%TYPE) RETURN inv_plates.plate_id%TYPE;

 FUNCTION DecrementParentQuantities(pChildPlateIDs varchar2) RETURN varchar2;

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
  RETURN varchar2;

	FUNCTION GetSolutionVolume(
  	p_oldSolutionVol inv_wells.solution_volume%TYPE,
    p_newSolutionVol inv_wells.solution_volume%TYPE,
    p_newSolventVol inv_wells.solvent_volume%TYPE,
    p_solventVolUnit inv_wells.solvent_volume_unit_id_fk%TYPE,
    p_newQty inv_wells.qty_remaining%TYPE,
    p_newQtyUnit inv_wells.qty_unit_fk%TYPE,
    p_concentration inv_wells.concentration%TYPE)
	RETURN inv_wells.solution_volume%TYPE;
  
END PLATECHEM;
/
show errors;