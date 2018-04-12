CREATE OR REPLACE TRIGGER TRG_AUDIT_INV_WELLS_AU0
AFTER UPDATE
ON INV_WELLS FOR EACH ROW
DECLARE
  raid number(10);
BEGIN
  SELECT seq_audit.NEXTVAL INTO raid FROM dual;

  audit_trail.record_transaction
    (raid, 'WELLS', :old.rid, 'U');

  audit_trail.check_val(raid, 'WELL_ID', :new.WELL_ID, :old.WELL_ID);
  audit_trail.check_val(raid, 'WELL_FORMAT_ID_FK', :new.WELL_FORMAT_ID_FK, :old.WELL_FORMAT_ID_FK);
  audit_trail.check_val(raid, 'PLATE_ID_FK', :new.PLATE_ID_FK, :old.PLATE_ID_FK);
  audit_trail.check_val(raid, 'PLATE_FORMAT_ID_FK', :new.PLATE_FORMAT_ID_FK, :old.PLATE_FORMAT_ID_FK);
  audit_trail.check_val(raid, 'GRID_POSITION_ID_FK', :new.GRID_POSITION_ID_FK, :old.GRID_POSITION_ID_FK);
  audit_trail.check_val(raid, 'WEIGHT', :new.WEIGHT, :old.WEIGHT);
  audit_trail.check_val(raid, 'WEIGHT_UNIT_FK', :new.WEIGHT_UNIT_FK, :old.WEIGHT_UNIT_FK);
  audit_trail.check_val(raid, 'QTY_INITIAL', :new.QTY_INITIAL, :old.QTY_INITIAL);
  audit_trail.check_val(raid, 'QTY_REMAINING', :new.QTY_REMAINING, :old.QTY_REMAINING);
  audit_trail.check_val(raid, 'QTY_UNIT_FK', :new.QTY_UNIT_FK, :old.QTY_UNIT_FK);
  audit_trail.check_val(raid, 'SOLVENT_ID_FK', :new.SOLVENT_ID_FK, :old.SOLVENT_ID_FK);
  audit_trail.check_val(raid, 'SOLVENT_VOLUME', :new.SOLVENT_VOLUME, :old.SOLVENT_VOLUME);
  audit_trail.check_val(raid, 'SOLVENT_VOLUME_INITIAL', :new.SOLVENT_VOLUME_INITIAL, :old.SOLVENT_VOLUME_INITIAL);
  audit_trail.check_val(raid, 'SOLVENT_VOLUME_UNIT_ID_FK', :new.SOLVENT_VOLUME_UNIT_ID_FK, :old.SOLVENT_VOLUME_UNIT_ID_FK);
  audit_trail.check_val(raid, 'SOLUTION_VOLUME', :new.SOLUTION_VOLUME, :old.SOLUTION_VOLUME);
  audit_trail.check_val(raid, 'CONCENTRATION', :new.CONCENTRATION, :old.CONCENTRATION);
  audit_trail.check_val(raid, 'CONC_UNIT_FK', :new.CONC_UNIT_FK, :old.CONC_UNIT_FK);
  audit_trail.check_val(raid, 'CONCENTRATION', :new.CONCENTRATION, :old.CONCENTRATION);
  audit_trail.check_val(raid, 'MOLAR_AMOUNT', :new.MOLAR_AMOUNT, :old.MOLAR_AMOUNT);
  audit_trail.check_val(raid, 'MOLAR_UNIT_FK', :new.MOLAR_UNIT_FK, :old.MOLAR_UNIT_FK);
  audit_trail.check_val(raid, 'MOLAR_CONC', :new.MOLAR_CONC, :old.MOLAR_CONC);
  audit_trail.check_val(raid, 'FIELD_1', :new.FIELD_1, :old.FIELD_1);
  audit_trail.check_val(raid, 'FIELD_2', :new.FIELD_2, :old.FIELD_2);
  audit_trail.check_val(raid, 'FIELD_3', :new.FIELD_3, :old.FIELD_3);
  audit_trail.check_val(raid, 'FIELD_4', :new.FIELD_4, :old.FIELD_4);
  audit_trail.check_val(raid, 'FIELD_5', :new.FIELD_5, :old.FIELD_5);
  audit_trail.check_val(raid, 'DATE_1', :new.DATE_1, :old.DATE_1);
  audit_trail.check_val(raid, 'DATE_2', :new.DATE_2, :old.DATE_2);
  audit_trail.check_val(raid, 'RID', :new.RID, :old.RID);
  --audit_trail.check_val(raid, 'CREATOR', :new.CREATOR, :old.CREATOR);
  --audit_trail.check_val(raid, 'TIMESTAMP', :new.TIMESTAMP, :old.TIMESTAMP);

END;
/
