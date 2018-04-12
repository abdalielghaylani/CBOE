CREATE OR REPLACE TRIGGER "TRG_INV_WELL_CMPDS_MOLAR_CALCS" AFTER
    INSERT OR
    UPDATE OF "BATCH_NUMBER_FK", "COMPOUND_ID_FK", "REG_ID_FK"
    ON "INV_WELL_COMPOUNDS"
    FOR EACH ROW
when (NEW.compound_id_fk is not null or NEW.reg_id_fk is not null)
DECLARE
lMolarAmount inv_wells.molar_amount%TYPE;
lMolarConc inv_wells.molar_conc%TYPE;
lSolutionVolume inv_wells.solution_volume%TYPE;
lSolventVolumeUnitID inv_wells.solvent_volume_unit_id_fk%TYPE;
BEGIN
	/*
		This trigger calculates the molar amount, molar conc when the well compound changes.
	*/

  --* Calculate the molar amount
  lMolarAmount := PlateChem.GetWellMolarAmount(:NEW.well_id_fk);
  IF lMolarAmount < 0 THEN lMolarAmount := 0; END IF;

	--* Get the well solution volume  
  SELECT solution_volume, solvent_volume_unit_id_fk INTO lSolutionVolume, lSolventVolumeUnitID FROM inv_wells WHERE well_id = :NEW.well_id_fk;
  
  --* Calculate the molar concentration
  IF lSolutionVolume > 0 THEN
  	lMolarConc := lMolarAmount / CHEMCALCS.convert(lSolutionVolume, lSolventVolumeUnitID, CONSTANTS.cLiterID);
  ELSIF lSolutionVolume = 0 OR lSolutionVolume is NULL THEN
    lMolarConc := null;
  END IF;
  
  --* Update the well
	UPDATE inv_wells set molar_amount = lMolarAmount, molar_conc = lMolarConc WHERE well_id = :NEW.well_id_fk;

END;
/
