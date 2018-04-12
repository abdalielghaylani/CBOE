CREATE OR REPLACE TRIGGER "TRG_MOLAR_CALCS" BEFORE
    INSERT OR
    UPDATE OF "MOLAR_AMOUNT", "MOLAR_CONC", "QTY_REMAINING", "QTY_UNIT_FK", "SOLVENT_VOLUME", "SOLVENT_VOLUME_UNIT_ID_FK"
    ON "INV_WELLS"
    FOR EACH ROW 
when (NEW.qty_remaining is not null)
DECLARE
vMolarAmount inv_wells.molar_amount%TYPE;
vWellCompoundState NUMBER;
BEGIN
  vMolarAmount := PlateChem.GetWellMolarAmount(:NEW.qty_remaining,:NEW.qty_unit_fk,:NEW.well_id);
  IF vMolarAmount < 0 THEN vMolarAmount := 0; END IF;
  :NEW.molar_amount := vMolarAmount;
  IF :NEW.solvent_volume > 0 THEN
	  vWellCompoundState := platechem.GetWellCompoundState(:NEW.solvent_volume, :NEW.qty_unit_fk);
    IF vWellCompoundState = platechem.cSolvatedDry THEN
	    :NEW.molar_conc := ChemCalcs.GetMolarConc(vMolarAmount, :NEW.solvent_volume, :NEW.solvent_volume_unit_id_fk, null,null);
    ELSIF vWellCompoundState = platechem.cSolvatedWet THEN
	    :NEW.molar_conc := ChemCalcs.GetMolarConc(vMolarAmount, :NEW.solvent_volume, :NEW.solvent_volume_unit_id_fk, :NEW.qty_remaining, :NEW.qty_unit_fk);
    END IF;
  ELSIF :OLD.solvent_volume > 0 AND (:NEW.solvent_volume = 0 OR :NEW.solvent_volume is null) THEN
    :NEW.molar_conc := null;
  END IF;

END;

/
