CREATE OR REPLACE TRIGGER "TRG_INV_PLATES_SOLUTION_CALC" BEFORE
    INSERT OR
    UPDATE OF "QTY_REMAINING", "QTY_UNIT_FK", "SOLVENT_VOLUME", "SOLVENT_VOLUME_UNIT_ID_FK"
    ON "CHEMINVDB2"."INV_PLATES"
    FOR EACH ROW

when (NEW.qty_remaining is not NULL OR (NEW.solvent_volume IS NOT NULL))
DECLARE
BEGIN
	/*
       This trigger calculates solution_volume, if necessary
     */

	--* Set the solution volume
	:NEW.solution_volume := platechem.GetSolutionVolume(:OLD.solution_volume,
											  :NEW.solution_volume,
											  :NEW.solvent_volume,
											  :NEW.solvent_volume_unit_id_fk,
											  :NEW.qty_remaining,
											  :NEW.qty_unit_fk,
											  :NEW.concentration);

	/*
	--* if solution volume changed don't calculate anything b/c the user is setting the value
	IF (:NEW.solution_volume = :OLD.solution_volume AND
	   :NEW.solvent_volume <> :OLD.solvent_volume) OR
	   (:NEW.solution_volume IS NULL AND :OLD.solution_volume IS NULL) THEN
		--* set solution_volume equal to solvent_volume
		:NEW.solution_volume := :NEW.solvent_volume;
		IF :NEW.solution_volume IS NULL THEN
			:NEW.solution_volume := 0;
		END IF;
		--* if qty is liquid and there is solvent then change solution volume
		IF :NEW.qty_remaining > 0 AND :NEW.qty_remaining IS NOT NULL AND
		   CHEMCALCS.GetUnitType(:NEW.qty_unit_fk) = 1 AND
		   :NEW.solvent_volume > 0 THEN
			:NEW.solution_volume := :NEW.solution_volume +
							    CHEMCALCS.Convert((:NEW.qty_remaining),
											  :NEW.qty_unit_fk,
											  :NEW.solvent_volume_unit_id_fk);
		END IF;
	END IF;
	*/
END;
/
