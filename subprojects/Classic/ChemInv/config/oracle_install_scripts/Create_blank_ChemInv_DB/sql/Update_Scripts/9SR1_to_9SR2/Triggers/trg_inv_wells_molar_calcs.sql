CREATE OR REPLACE TRIGGER "TRG_INV_WELLS_MOLAR_CALCS" BEFORE
    INSERT OR
    UPDATE OF "QTY_REMAINING", "QTY_UNIT_FK", "SOLVENT_VOLUME"
    ON "CHEMINVDB2"."INV_WELLS"
    FOR EACH ROW 
    
when (NEW.qty_remaining is not NULL OR (NEW.solvent_volume IS NOT NULL))
DECLARE
vMolarAmount inv_wells.molar_amount%TYPE;
vWellCompoundState NUMBER;
BEGIN
	/*
		This trigger calculates solution_volume, if necessary, and molar amount, molar conc always..
	*/

	--* Set the solution volume
  --* if solution volume changed don't calculate anything b/c the user is setting the value
  IF :NEW.solution_volume = :OLD.solution_volume OR (:NEW.solution_volume IS NULL AND :OLD.solution_volume IS NULL) THEN
		--* set solution_volume equal to solvent_volume
    :NEW.solution_volume := :NEW.solvent_volume;
   	IF :NEW.solution_volume IS NULL THEN :NEW.solution_volume := 0; END IF;
  	--* if qty is liquid and there is solvent then change solution volume 
  	IF :NEW.qty_remaining > 0 AND :NEW.qty_remaining IS not NULL AND CHEMCALCS.GetUnitType(:NEW.qty_unit_fk) = 1 AND :NEW.solution_volume > 0 THEN
      	:NEW.solution_volume := :NEW.solution_volume + CHEMCALCS.Convert((:NEW.qty_remaining), :NEW.qty_unit_fk, :NEW.solvent_volume_unit_id_fk);
    END IF;
  END IF;

  --* Calculate the molar amount
  vMolarAmount := PlateChem.GetWellMolarAmount(:NEW.qty_remaining,:NEW.qty_unit_fk,:NEW.well_id);
  IF vMolarAmount < 0 THEN vMolarAmount := 0; END IF;
  :NEW.molar_amount := vMolarAmount;
  
  --* Calculate the molar concentration
  IF :NEW.solution_volume > 0 THEN
  	:NEW.molar_conc := vMolarAmount / CHEMCALCS.convert(:NEW.solution_volume, :NEW.solvent_volume_unit_id_fk, CONSTANTS.cLiterID);
  ELSIF :OLD.molar_conc > 0 AND (:NEW.solution_volume = 0 OR :NEW.solution_volume is null) THEN
    :NEW.molar_conc := 0;
  END IF;

END;
/
