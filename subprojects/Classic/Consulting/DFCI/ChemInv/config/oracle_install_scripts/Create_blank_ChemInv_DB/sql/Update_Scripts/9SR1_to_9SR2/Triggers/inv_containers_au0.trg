-- inv_containers_au0.trg
-- Script to create AFTER-UPDATE audit trigger for the less frequently
-- changed INV_CONTAINERS fields.
CREATE OR REPLACE TRIGGER TRG_AUDIT_INV_CONTAINERS_AU0
AFTER UPDATE OF
		CONTAINER_ID,
    COMPOUND_ID_FK, 
    PARENT_CONTAINER_ID_FK, 
    REG_ID_FK, 
    FAMILY, 
    BATCH_NUMBER_FK, 
    CONTAINER_NAME, 
    CONTAINER_DESCRIPTION,
    QTY_MAX, 
    QTY_INITIAL,
    QTY_REMAINING, 
    QTY_MINSTOCK, 
    QTY_MAXSTOCK, 
    WELL_NUMBER, 
    WELL_ROW,
    WELL_COLUMN,
    DATE_EXPIRES, 
    DATE_CREATED, 
    CONTAINER_TYPE_ID_FK,
    PURITY, 
    SOLVENT_ID_FK,
    CONCENTRATION,
    UNIT_OF_MEAS_ID_FK,
    UNIT_OF_WGHT_ID_FK, 
    UNIT_OF_CONC_ID_FK,
    GRADE,
    WEIGHT, 
    UNIT_OF_PURITY_ID_FK, 
    TARE_WEIGHT, 
    OWNER_ID_FK, 
    CONTAINER_COMMENTS, 
    STORAGE_CONDITIONS,
    HANDLING_PROCEDURES, 
    ORDERED_BY_ID_FK, 
    DATE_ORDERED, 
    DATE_RECEIVED, 
    DATE_CERTIFIED, 
    DATE_APPROVED,
    LOT_NUM, 
    RECEIVED_BY_ID_FK, 
    FINAL_WGHT, 
    NET_WGHT, 
    QTY_AVAILABLE, 
    QTY_RESERVED,
    PHYSICAL_STATE_ID_FK,
    SUPPLIER_ID_FK, 
    SUPPLIER_CATNUM, 
    DATE_PRODUCED,
    CONTAINER_COST, 
    UNIT_OF_COST_ID_FK, 
    DEF_LOCATION_ID_FK, 
	  BARCODE, 
    PO_NUMBER, 
    REQ_NUMBER, 
    DENSITY, 
    UNIT_OF_DENSITY_ID_FK, 
    PO_LINE_NUMBER, 
    FIELD_1,
    FIELD_2, 
    FIELD_3, 
    FIELD_4, 
    FIELD_5, 
    FIELD_6,
    FIELD_7, 
    FIELD_8, 
    FIELD_9, 
    FIELD_10, 
    DATE_1, 
    DATE_2, 
    DATE_3, 
    DATE_4,
    DATE_5, 
    RID
ON INV_CONTAINERS FOR EACH ROW
DECLARE
  raid number(10);
BEGIN
  SELECT seq_audit.NEXTVAL INTO raid FROM dual;

  audit_trail.record_transaction
    (raid, 'INV_CONTAINERS', :old.rid, 'U');

  audit_trail.check_val(raid, 'CONTAINER_ID', :new.CONTAINER_ID, :old.CONTAINER_ID);
  audit_trail.check_val(raid, 'COMPOUND_ID_FK', :new.COMPOUND_ID_FK, :old.COMPOUND_ID_FK);
  audit_trail.check_val(raid, 'PARENT_CONTAINER_ID_FK', :new.PARENT_CONTAINER_ID_FK, :old.PARENT_CONTAINER_ID_FK);
  audit_trail.check_val(raid, 'REG_ID_FK', :new.REG_ID_FK, :old.REG_ID_FK);
  audit_trail.check_val(raid, 'BATCH_NUMBER_FK', :new.BATCH_NUMBER_FK, :old.BATCH_NUMBER_FK);
  audit_trail.check_val(raid, 'FAMILY', :new.FAMILY, :old.FAMILY);
  audit_trail.check_val(raid, 'CONTAINER_NAME', :new.CONTAINER_NAME, :old.CONTAINER_NAME);
  audit_trail.check_val(raid, 'CONTAINER_DESCRIPTION', :new.CONTAINER_DESCRIPTION, :old.CONTAINER_DESCRIPTION);
  audit_trail.check_val(raid, 'QTY_MAX', :new.QTY_MAX, :old.QTY_MAX);
  audit_trail.check_val(raid, 'QTY_INITIAL', :new.QTY_INITIAL, :old.QTY_INITIAL);
  audit_trail.check_val(raid, 'QTY_REMAINING', :new.QTY_REMAINING, :old.QTY_REMAINING);
  audit_trail.check_val(raid, 'QTY_MINSTOCK', :new.QTY_MINSTOCK, :old.QTY_MINSTOCK);
  audit_trail.check_val(raid, 'QTY_MAXSTOCK', :new.QTY_MAXSTOCK, :old.QTY_MAXSTOCK);
  audit_trail.check_val(raid, 'WELL_NUMBER', :new.WELL_NUMBER, :old.WELL_NUMBER);
  audit_trail.check_val(raid, 'WELL_ROW', :new.WELL_ROW, :old.WELL_ROW);
  audit_trail.check_val(raid, 'WELL_COLUMN', :new.WELL_COLUMN, :old.WELL_COLUMN);
  audit_trail.check_val(raid, 'DATE_EXPIRES', :new.DATE_EXPIRES, :old.DATE_EXPIRES);
  audit_trail.check_val(raid, 'DATE_CREATED', :new.DATE_CREATED, :old.DATE_CREATED);
  audit_trail.check_val(raid, 'CONTAINER_TYPE_ID_FK', :new.CONTAINER_TYPE_ID_FK, :old.CONTAINER_TYPE_ID_FK);
  audit_trail.check_val(raid, 'PURITY', :new.PURITY, :old.PURITY);
  audit_trail.check_val(raid, 'SOLVENT_ID_FK', :new.SOLVENT_ID_FK, :old.SOLVENT_ID_FK);
  audit_trail.check_val(raid, 'CONCENTRATION', :new.CONCENTRATION, :old.CONCENTRATION);
  audit_trail.check_val(raid, 'UNIT_OF_MEAS_ID_FK', :new.UNIT_OF_MEAS_ID_FK, :old.UNIT_OF_MEAS_ID_FK);
  audit_trail.check_val(raid, 'UNIT_OF_WGHT_ID_FK', :new.UNIT_OF_WGHT_ID_FK, :old.UNIT_OF_WGHT_ID_FK);
  audit_trail.check_val(raid, 'UNIT_OF_CONC_ID_FK', :new.UNIT_OF_CONC_ID_FK, :old.UNIT_OF_CONC_ID_FK);
  audit_trail.check_val(raid, 'GRADE', :new.GRADE, :old.GRADE);
  audit_trail.check_val(raid, 'WEIGHT', :new.WEIGHT, :old.WEIGHT);
  audit_trail.check_val(raid, 'UNIT_OF_PURITY_ID_FK', :new.UNIT_OF_PURITY_ID_FK, :old.UNIT_OF_PURITY_ID_FK);
  audit_trail.check_val(raid, 'TARE_WEIGHT', :new.TARE_WEIGHT, :old.TARE_WEIGHT);
  audit_trail.check_val(raid, 'OWNER_ID_FK', :new.OWNER_ID_FK, :old.OWNER_ID_FK);
  audit_trail.check_val(raid, 'CONTAINER_COMMENTS', :new.CONTAINER_COMMENTS, :old.CONTAINER_COMMENTS);
  audit_trail.check_val(raid, 'STORAGE_CONDITIONS', :new.STORAGE_CONDITIONS, :old.STORAGE_CONDITIONS);
  audit_trail.check_val(raid, 'HANDLING_PROCEDURES', :new.HANDLING_PROCEDURES, :old.HANDLING_PROCEDURES);
  audit_trail.check_val(raid, 'ORDERED_BY_ID_FK', :new.ORDERED_BY_ID_FK, :old.ORDERED_BY_ID_FK);
  audit_trail.check_val(raid, 'DATE_ORDERED', :new.DATE_ORDERED, :old.DATE_ORDERED);
  audit_trail.check_val(raid, 'DATE_RECEIVED', :new.DATE_RECEIVED, :old.DATE_RECEIVED);
  audit_trail.check_val(raid, 'DATE_CERTIFIED', :new.DATE_CERTIFIED, :old.DATE_CERTIFIED);
  audit_trail.check_val(raid, 'DATE_APPROVED', :new.DATE_APPROVED, :old.DATE_APPROVED);
  audit_trail.check_val(raid, 'LOT_NUM', :new.LOT_NUM, :old.LOT_NUM);
  audit_trail.check_val(raid, 'RECEIVED_BY_ID_FK', :new.RECEIVED_BY_ID_FK, :old.RECEIVED_BY_ID_FK);
  audit_trail.check_val(raid, 'FINAL_WGHT', :new.FINAL_WGHT, :old.FINAL_WGHT);
  audit_trail.check_val(raid, 'NET_WGHT', :new.NET_WGHT, :old.NET_WGHT);
  audit_trail.check_val(raid, 'QTY_AVAILABLE', :new.QTY_AVAILABLE, :old.QTY_AVAILABLE);
  audit_trail.check_val(raid, 'QTY_RESERVED', :new.QTY_RESERVED, :old.QTY_RESERVED);
  audit_trail.check_val(raid, 'PHYSICAL_STATE_ID_FK', :new.PHYSICAL_STATE_ID_FK, :old.PHYSICAL_STATE_ID_FK);
  audit_trail.check_val(raid, 'SUPPLIER_ID_FK', :new.SUPPLIER_ID_FK, :old.SUPPLIER_ID_FK);
  audit_trail.check_val(raid, 'SUPPLIER_CATNUM', :new.SUPPLIER_CATNUM, :old.SUPPLIER_CATNUM);
  audit_trail.check_val(raid, 'DATE_PRODUCED', :new.DATE_PRODUCED, :old.DATE_PRODUCED);
  audit_trail.check_val(raid, 'CONTAINER_COST', :new.CONTAINER_COST, :old.CONTAINER_COST);
  audit_trail.check_val(raid, 'UNIT_OF_COST_ID_FK', :new.UNIT_OF_COST_ID_FK, :old.UNIT_OF_COST_ID_FK);
  audit_trail.check_val(raid, 'DEF_LOCATION_ID_FK', :new.DEF_LOCATION_ID_FK, :old.DEF_LOCATION_ID_FK);
  audit_trail.check_val(raid, 'BARCODE', :new.BARCODE, :old.BARCODE);
  audit_trail.check_val(raid, 'PO_NUMBER', :new.PO_NUMBER, :old.PO_NUMBER);
  audit_trail.check_val(raid, 'REQ_NUMBER', :new.REQ_NUMBER, :old.REQ_NUMBER);
  audit_trail.check_val(raid, 'DENSITY', :new.DENSITY, :old.DENSITY);
  audit_trail.check_val(raid, 'UNIT_OF_DENSITY_ID_FK', :new.UNIT_OF_DENSITY_ID_FK, :old.UNIT_OF_DENSITY_ID_FK);
  audit_trail.check_val(raid, 'PO_LINE_NUMBER', :new.PO_LINE_NUMBER, :old.PO_LINE_NUMBER);
  audit_trail.check_val(raid, 'FIELD_1', :new.FIELD_1, :old.FIELD_1);
  audit_trail.check_val(raid, 'FIELD_2', :new.FIELD_2, :old.FIELD_2);
  audit_trail.check_val(raid, 'FIELD_3', :new.FIELD_3, :old.FIELD_3);
  audit_trail.check_val(raid, 'FIELD_4', :new.FIELD_4, :old.FIELD_4);
  audit_trail.check_val(raid, 'FIELD_5', :new.FIELD_5, :old.FIELD_5);
  audit_trail.check_val(raid, 'FIELD_6', :new.FIELD_6, :old.FIELD_6);
  audit_trail.check_val(raid, 'FIELD_7', :new.FIELD_7, :old.FIELD_7);
  audit_trail.check_val(raid, 'FIELD_8', :new.FIELD_8, :old.FIELD_8);
  audit_trail.check_val(raid, 'FIELD_9', :new.FIELD_9, :old.FIELD_9);
  audit_trail.check_val(raid, 'FIELD_10', :new.FIELD_10, :old.FIELD_10);
  audit_trail.check_val(raid, 'DATE_1', :new.DATE_1, :old.DATE_1);
  audit_trail.check_val(raid, 'DATE_2', :new.DATE_2, :old.DATE_2);
  audit_trail.check_val(raid, 'DATE_3', :new.DATE_3, :old.DATE_3);
  audit_trail.check_val(raid, 'DATE_4', :new.DATE_4, :old.DATE_4);
  audit_trail.check_val(raid, 'DATE_5', :new.DATE_5, :old.DATE_5);
  audit_trail.check_val(raid, 'RID', :new.RID, :old.RID);
  --audit_trail.check_val(raid, 'CREATOR', :new.CREATOR, :old.CREATOR);
  --audit_trail.check_val(raid, 'TIMESTAMP', :new.TIMESTAMP, :old.TIMESTAMP);

END;


/
