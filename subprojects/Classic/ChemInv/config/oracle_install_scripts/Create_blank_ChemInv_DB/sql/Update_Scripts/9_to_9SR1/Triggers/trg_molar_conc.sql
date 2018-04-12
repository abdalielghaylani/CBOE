CREATE OR REPLACE TRIGGER "TRG_MOLAR_CONC" BEFORE 
    INSERT OR 
    UPDATE OF "MOLAR_AMOUNT", "MOLAR_CONC", "SOLVENT_VOLUME", 
    "SOLVENT_VOLUME_UNIT_ID_FK" 
    ON "INV_WELLS" 
    FOR EACH ROW WHEN (New.Solvent_volume is not null) BEGIN
  IF :NEW.solvent_volume > 0 THEN
    :NEW.molar_conc := ChemCalcs.GetMolarConc(:OLD.molar_amount, :NEW.solvent_volume, :NEW.solvent_volume_unit_id_fk, null,null);
  ELSIF :OLD.solvent_volume > 0 AND (:NEW.solvent_volume = 0 OR :NEW.solvent_volume is null) THEN
    :NEW.molar_conc := null;
  END IF;
END;         
/    
