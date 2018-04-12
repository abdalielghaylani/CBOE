CREATE OR REPLACE TRIGGER TRG_AUDIT_INV_PLATES_AU0
AFTER UPDATE
ON INV_PLATES FOR EACH ROW
DECLARE
  raid number(10);
BEGIN
  SELECT seq_audit.NEXTVAL INTO raid FROM dual;

  audit_trail.record_transaction
    (raid, 'PLATES', :old.rid, 'U');

  audit_trail.check_val(raid, 'PLATE_ID', :new.PLATE_ID, :old.PLATE_ID);
  audit_trail.check_val(raid, 'LOCATION_ID_FK', :new.LOCATION_ID_FK, :old.LOCATION_ID_FK);
  audit_trail.check_val(raid, 'CONTAINER_ID_FK', :new.CONTAINER_ID_FK, :old.CONTAINER_ID_FK);
  audit_trail.check_val(raid, 'PLATE_TYPE_ID_FK', :new.PLATE_TYPE_ID_FK, :old.PLATE_TYPE_ID_FK);
  audit_trail.check_val(raid, 'PLATE_FORMAT_ID_FK', :new.PLATE_FORMAT_ID_FK, :old.PLATE_FORMAT_ID_FK);
  audit_trail.check_val(raid, 'PLATE_BARCODE', :new.PLATE_BARCODE, :old.PLATE_BARCODE);
  audit_trail.check_val(raid, 'PLATE_NAME', :new.PLATE_NAME, :old.PLATE_NAME);
  audit_trail.check_val(raid, 'STATUS_ID_FK', :new.STATUS_ID_FK, :old.STATUS_ID_FK);
  audit_trail.check_val(raid, 'GROUP_NAME', :new.GROUP_NAME, :old.GROUP_NAME);
  audit_trail.check_val(raid, 'LIBRARY_ID_FK', :new.LIBRARY_ID_FK, :old.LIBRARY_ID_FK);
  audit_trail.check_val(raid, 'FT_CYCLES', :new.FT_CYCLES, :old.FT_CYCLES);
  audit_trail.check_val(raid, 'WEIGHT', :new.WEIGHT, :old.WEIGHT);
  audit_trail.check_val(raid, 'WEIGHT_UNIT_FK', :new.WEIGHT_UNIT_FK, :old.WEIGHT_UNIT_FK);
  audit_trail.check_val(raid, 'QTY_INITIAL', :new.QTY_INITIAL, :old.QTY_INITIAL);
  audit_trail.check_val(raid, 'QTY_REMAINING', :new.QTY_REMAINING, :old.QTY_REMAINING);
  audit_trail.check_val(raid, 'QTY_UNIT_FK', :new.QTY_UNIT_FK, :old.QTY_UNIT_FK);
  audit_trail.check_val(raid, 'SOLVENT', :new.SOLVENT, :old.SOLVENT);
  audit_trail.check_val(raid, 'CONCENTRATION', :new.CONCENTRATION, :old.CONCENTRATION);
  audit_trail.check_val(raid, 'CONC_UNIT_FK', :new.CONC_UNIT_FK, :old.CONC_UNIT_FK);
  audit_trail.check_val(raid, 'DATE_CREATED', :new.DATE_CREATED, :old.DATE_CREATED);
  audit_trail.check_val(raid, 'SUPPLIER_BARCODE', :new.SUPPLIER_BARCODE, :old.SUPPLIER_BARCODE);
  audit_trail.check_val(raid, 'SUPPLIER_SHIPMENT_CODE', :new.SUPPLIER_SHIPMENT_CODE, :old.SUPPLIER_SHIPMENT_CODE);
  audit_trail.check_val(raid, 'SUPPLIER_SHIPMENT_NUMBER', :new.SUPPLIER_SHIPMENT_NUMBER, :old.SUPPLIER_SHIPMENT_NUMBER);
  audit_trail.check_val(raid, 'SUPPLIER_SHIPMENT_DATE', :new.SUPPLIER_SHIPMENT_DATE, :old.SUPPLIER_SHIPMENT_DATE);
  audit_trail.check_val(raid, 'SOLVENT_ID_FK', :new.SOLVENT_ID_FK, :old.SOLVENT_ID_FK);
  audit_trail.check_val(raid, 'SOLVENT_VOLUME', :new.SOLVENT_VOLUME, :old.SOLVENT_VOLUME);
  audit_trail.check_val(raid, 'SOLVENT_VOLUME_INITIAL', :new.SOLVENT_VOLUME_INITIAL, :old.SOLVENT_VOLUME_INITIAL);
  audit_trail.check_val(raid, 'SOLVENT_VOLUME_UNIT_ID_FK', :new.SOLVENT_VOLUME_UNIT_ID_FK, :old.SOLVENT_VOLUME_UNIT_ID_FK);
  audit_trail.check_val(raid, 'SOLUTION_VOLUME', :new.SOLUTION_VOLUME, :old.SOLUTION_VOLUME);
  audit_trail.check_val(raid, 'MOLAR_AMOUNT', :new.MOLAR_AMOUNT, :old.MOLAR_AMOUNT);
  audit_trail.check_val(raid, 'MOLAR_UNIT_FK', :new.MOLAR_UNIT_FK, :old.MOLAR_UNIT_FK);
  audit_trail.check_val(raid, 'MOLAR_CONC', :new.MOLAR_CONC, :old.MOLAR_CONC);
  audit_trail.check_val(raid, 'WELL_CAPACITY', :new.WELL_CAPACITY, :old.WELL_CAPACITY);
  audit_trail.check_val(raid, 'WELL_CAPACITY_UNIT_ID_FK', :new.WELL_CAPACITY_UNIT_ID_FK, :old.WELL_CAPACITY_UNIT_ID_FK);
  audit_trail.check_val(raid, 'AMOUNTS_DIFFER', :new.AMOUNTS_DIFFER, :old.AMOUNTS_DIFFER);
  audit_trail.check_val(raid, 'SUPPLIER', :new.SUPPLIER, :old.SUPPLIER);
  audit_trail.check_val(raid, 'PLATE_EXISTS', :new.PLATE_EXISTS, :old.PLATE_EXISTS);
  audit_trail.check_val(raid, 'IS_PLATE_MAP', :new.IS_PLATE_MAP, :old.IS_PLATE_MAP);
  audit_trail.check_val(raid, 'PLATE_MAP_ID_FK', :new.PLATE_MAP_ID_FK, :old.PLATE_MAP_ID_FK);
  audit_trail.check_val(raid, 'PURITY', :new.PURITY, :old.PURITY);
  audit_trail.check_val(raid, 'PURITY_UNIT_FK', :new.PURITY_UNIT_FK, :old.PURITY_UNIT_FK);
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
