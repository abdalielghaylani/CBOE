CREATE OR REPLACE TRIGGER "TRG_INV_WELL_CMPDS_MOLAR_CALCS" AFTER
    INSERT OR
    UPDATE OF "BATCH_NUMBER_FK", "COMPOUND_ID_FK", "REG_ID_FK"
    ON "CHEMINVDB2"."INV_WELL_COMPOUNDS"
    FOR EACH ROW 
when (NEW.compound_id_fk is not null or NEW.reg_id_fk is not null)
DECLARE
vMolarAmount inv_wells.molar_amount%TYPE;
vMolarConc inv_wells.molar_conc%TYPE;
vQtyRemaining inv_wells.qty_remaining%TYPE;
vQtyUnit inv_wells.qty_unit_fk%TYPE;
vSolventVolume inv_wells.solvent_volume%TYPE;
vSolventVolumeUnitID inv_wells.solvent_volume_unit_id_fk%TYPE;
vWellCompoundState NUMBER;
BEGIN
  vMolarAmount := PlateChem.GetWellMolarAmount(:NEW.well_id_fk);
  IF vMolarAmount < 0 THEN vMolarAmount := 0; END IF;
  SELECT solvent_volume, solvent_volume_unit_id_fk, qty_remaining, qty_unit_fk INTO vSolventVolume, vSolventVolumeUnitID, vQtyRemaining, vQtyUnit FROM inv_wells WHERE well_id = :NEW.well_id_fk;
  IF vSolventVolume > 0 THEN
	  vWellCompoundState := platechem.GetWellCompoundState(vSolventVolume, vQtyUnit);
    IF vWellCompoundState = platechem.cSolvatedDry THEN
	    vMolarConc := ChemCalcs.GetMolarConc(vMolarAmount, vSolventVolume, vSolventVolumeUnitID, null,null);
    ELSIF vWellCompoundState = platechem.cSolvatedWet THEN
	    vMolarConc := ChemCalcs.GetMolarConc(vMolarAmount, vSolventVolume, vSolventVolumeUnitID, vQtyRemaining, vQtyUnit);
    END IF;
  ELSIF (vSolventVolume = 0 OR vSolventVolume is null) THEN
    vMolarConc := null;
  END IF;
	UPDATE inv_wells set molar_amount = vMolarAmount, molar_conc = vMolarConc WHERE well_id = :NEW.well_id_fk;

END;
/
