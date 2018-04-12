-- inv_containers_au0.trg
-- Script to create AFTER-UPDATE audit trigger for the less frequently
-- changed INV_CONTAINERS fields.

CREATE OR REPLACE TRIGGER TRG_AUDIT_INV_CONTAINERS_AU0 
AFTER UPDATE OF 
    BARCODE, BATCH_NUMBER_FK, COMPOUND_ID_FK, CONCENTRATION,
    CONTAINER_COMMENTS, CONTAINER_COST, CONTAINER_DESCRIPTION,
    CONTAINER_ID, CONTAINER_NAME, CONTAINER_TYPE_ID_FK,
    DATE_1, DATE_2, DATE_3, DATE_4,
    DATE_5, DATE_CREATED, DATE_EXPIRES, DATE_ORDERED, DATE_PRODUCED,
    DATE_RECEIVED, DEF_LOCATION_ID_FK, DENSITY, FIELD_1,
    FIELD_10, FIELD_2, FIELD_3, FIELD_4, FIELD_5, FIELD_6,
    FIELD_7, FIELD_8, FIELD_9, FINAL_WGHT, GRADE, 
    LOT_NUM, NET_WGHT, ORDERED_BY_ID_FK, OWNER_ID_FK, PHYSICAL_STATE_ID_FK,
    PO_LINE_NUMBER, PO_NUMBER, PURITY, QTY_AVAILABLE, QTY_INITIAL,
    QTY_MAX, QTY_MAXSTOCK, QTY_MINSTOCK, QTY_REMAINING, QTY_RESERVED,
    RECEIVED_BY_ID_FK, REG_ID_FK, REQ_NUMBER, RID, SOLVENT_ID_FK,
    SUPPLIER_CATNUM, SUPPLIER_ID_FK, TARE_WEIGHT, UNIT_OF_CONC_ID_FK,
    UNIT_OF_COST_ID_FK, UNIT_OF_DENSITY_ID_FK, UNIT_OF_MEAS_ID_FK,
    UNIT_OF_PURITY_ID_FK, UNIT_OF_WGHT_ID_FK, WEIGHT, WELL_COLUMN,
    WELL_NUMBER, WELL_ROW, 
    PARENT_CONTAINER_ID_FK, FAMILY, STORAGE_CONDITIONS, 
    HANDLING_PROCEDURES, DATE_CERTIFIED, DATE_APPROVED
ON INV_CONTAINERS FOR EACH ROW 
DECLARE
  raid number(10);
BEGIN
  SELECT seq_audit.NEXTVAL INTO raid FROM dual;

  audit_trail.record_transaction
    (raid, 'INV_CONTAINERS', :old.rid, 'U');

  IF UPDATING('container_id') THEN
    IF NVL(:old.container_id,0) != NVL(:new.container_id,0) THEN
       audit_trail.column_update
         (raid, 'CONTAINER_ID',
         :old.container_id, :new.container_id);
    END IF;
  END IF;

  IF UPDATING('compound_id_fk') THEN
    IF NVL(:old.compound_id_fk,0) != NVL(:new.compound_id_fk,0) THEN
       audit_trail.column_update
         (raid, 'COMPOUND_ID_FK',
         :old.compound_id_fk, :new.compound_id_fk);
    END IF;
  END IF;

  IF UPDATING('container_name') THEN
    IF :old.container_name != :new.container_name THEN
     audit_trail.column_update
       (raid, 'CONTAINER_NAME',
       :old.container_name, :new.container_name);
    END IF;
  END IF;

  IF UPDATING('container_description') THEN
    IF NVL(:old.container_description,' ') != NVL(:new.container_description,' ') THEN
       audit_trail.column_update
         (raid, 'CONTAINER_DESCRIPTION',
         :old.container_description, :new.container_description);
    END IF;
  END IF;

  IF UPDATING('qty_max') THEN
    IF NVL(:old.qty_max,0) != NVL(:new.qty_max,0) THEN
       audit_trail.column_update
         (raid, 'QTY_MAX',
         :old.qty_max, :new.qty_max);
    END IF;
  END IF;

  IF UPDATING('qty_initial') THEN
    IF :old.qty_initial != :new.qty_initial THEN
       audit_trail.column_update
         (raid, 'QTY_INITIAL',
         :old.qty_initial, :new.qty_initial);
    END IF;
  END IF;

  IF UPDATING('qty_remaining') THEN
    IF :old.qty_remaining != :new.qty_remaining THEN
       audit_trail.column_update
         (raid, 'QTY_REMAINING',
         :old.qty_remaining, :new.qty_remaining);
    END IF;
  END IF;

  IF UPDATING('qty_minstock') THEN
    IF NVL(:old.qty_minstock,0) != NVL(:new.qty_minstock,0) THEN
       audit_trail.column_update
         (raid, 'QTY_MINSTOCK',
         :old.qty_minstock, :new.qty_minstock);
    END IF;
  END IF;

  IF UPDATING('qty_maxstock') THEN
    IF NVL(:old.qty_maxstock,0) != NVL(:new.qty_maxstock,0) THEN
       audit_trail.column_update
         (raid, 'QTY_MAXSTOCK',
         :old.qty_maxstock, :new.qty_maxstock);
    END IF;
  END IF;

  IF UPDATING('well_number') THEN
    IF NVL(:old.well_number,0) != NVL(:new.well_number,0) THEN
       audit_trail.column_update
         (raid, 'WELL_NUMBER',
         :old.well_number, :new.well_number);
    END IF;
  END IF;

  IF UPDATING('well_row') THEN
    IF NVL(:old.well_row,' ') != NVL(:new.well_row,' ') THEN
       audit_trail.column_update
         (raid, 'WELL_ROW',
         :old.well_row, :new.well_row);
    END IF;
  END IF;

  IF UPDATING('well_column') THEN
    IF NVL(:old.well_column,' ') != NVL(:new.well_column,' ') THEN
       audit_trail.column_update
         (raid, 'WELL_COLUMN',
         :old.well_column, :new.well_column);
    END IF;
  END IF;

  IF UPDATING('date_expires') THEN
    IF NVL(:old.date_expires,TO_DATE('1', 'J')) != NVL(:new.date_expires,TO_DATE('1', 'J')) THEN
       audit_trail.column_update
         (raid, 'DATE_EXPIRES',
         :old.date_expires, :new.date_expires);
    END IF;
  END IF;

  IF UPDATING('date_created') THEN
    IF :old.date_created != :new.date_created THEN
       audit_trail.column_update
         (raid, 'DATE_CREATED',
         :old.date_created, :new.date_created);
    END IF;
  END IF;

  IF UPDATING('container_type_id_fk') THEN
    IF :old.container_type_id_fk != :new.container_type_id_fk THEN
       audit_trail.column_update
         (raid, 'CONTAINER_TYPE_ID_FK',
         :old.container_type_id_fk, :new.container_type_id_fk);
    END IF;
  END IF;

  IF UPDATING('purity') THEN
    IF NVL(:old.purity,0) != NVL(:new.purity,0) THEN
       audit_trail.column_update
         (raid, 'PURITY',
         :old.purity, :new.purity);
    END IF;
  END IF;

  IF UPDATING('solvent_ID_FK') THEN
    IF NVL(:old.solvent_ID_FK,0) != NVL(:new.solvent_ID_FK,0) THEN
       audit_trail.column_update
         (raid, 'SOLVENT_ID_FK',
         :old.solvent_ID_FK, :new.solvent_ID_FK);
    END IF;
  END IF;

  IF UPDATING('concentration') THEN
    IF NVL(:old.concentration,0) != NVL(:new.concentration,0) THEN
       audit_trail.column_update
         (raid, 'CONCENTRATION',
         :old.concentration, :new.concentration);
    END IF;
  END IF;

  IF UPDATING('unit_of_meas_id_fk') THEN
    IF NVL(:old.unit_of_meas_id_fk,0) != NVL(:new.unit_of_meas_id_fk,0) THEN
       audit_trail.column_update
         (raid, 'UNIT_OF_MEAS_ID_FK',
         :old.unit_of_meas_id_fk, :new.unit_of_meas_id_fk);
    END IF;
  END IF;

  IF UPDATING('unit_of_wght_id_fk') THEN
    IF NVL(:old.unit_of_wght_id_fk,0) != NVL(:new.unit_of_wght_id_fk,0) THEN
       audit_trail.column_update
         (raid, 'UNIT_OF_WGHT_ID_FK',
         :old.unit_of_wght_id_fk, :new.unit_of_wght_id_fk);
    END IF;
  END IF;

  IF UPDATING('unit_of_conc_id_fk') THEN
    IF NVL(:old.unit_of_conc_id_fk,0) != NVL(:new.unit_of_conc_id_fk,0) THEN
       audit_trail.column_update
         (raid, 'UNIT_OF_CONC_ID_FK',
         :old.unit_of_conc_id_fk, :new.unit_of_conc_id_fk);
    END IF;
  END IF;

  IF UPDATING('grade') THEN
    IF NVL(:old.grade,' ') !=
       NVL(:new.grade,' ') THEN
       audit_trail.column_update
         (raid, 'GRADE',
         :old.grade, :new.grade);
    END IF;
  END IF;

  IF UPDATING('weight') THEN
    IF NVL(:old.weight,0) != NVL(:new.weight,0) THEN
       audit_trail.column_update
         (raid, 'WEIGHT',
         :old.weight, :new.weight);
    END IF;
  END IF;

  IF UPDATING('unit_of_purity_id_fk') THEN
    IF NVL(:old.unit_of_purity_id_fk,0) != NVL(:new.unit_of_purity_id_fk,0) THEN
       audit_trail.column_update
         (raid, 'UNIT_OF_PURITY_ID_FK',
         :old.unit_of_purity_id_fk, :new.unit_of_purity_id_fk);
    END IF;
  END IF;

  IF UPDATING('tare_weight') THEN
    IF NVL(:old.tare_weight,0) != NVL(:new.tare_weight,0) THEN
       audit_trail.column_update
         (raid, 'TARE_WEIGHT',
         :old.tare_weight, :new.tare_weight);
    END IF;
  END IF;

  IF UPDATING('owner_id_fk') THEN
    IF :old.owner_id_fk != :new.owner_id_fk THEN
       audit_trail.column_update
         (raid, 'OWNER_ID_FK',
         :old.owner_id_fk, :new.owner_id_fk);
    END IF;
  END IF;

  IF UPDATING('container_comments') THEN
    IF NVL(:old.container_comments,' ') != NVL(:new.container_comments,' ') THEN
       audit_trail.column_update
         (raid, 'CONTAINER_COMMENTS',
         :old.container_comments, :new.container_comments);
    END IF;
  END IF;

  IF UPDATING('ordered_by_id_fk') THEN
    IF NVL(:old.ordered_by_id_fk,' ') != NVL(:new.ordered_by_id_fk,' ') THEN
       audit_trail.column_update
         (raid, 'ORDERED_BY_ID_FK',
         :old.ordered_by_id_fk, :new.ordered_by_id_fk);
    END IF;
  END IF;

  IF UPDATING('date_ordered') THEN
    IF NVL(:old.date_ordered,TO_DATE('1', 'J')) != NVL(:new.date_ordered,TO_DATE('1', 'J')) THEN
       audit_trail.column_update
         (raid, 'DATE_ORDERED',
         :old.date_ordered, :new.date_ordered);
    END IF;
  END IF;

  IF UPDATING('date_received') THEN
    IF NVL(:old.date_received,TO_DATE('1', 'J')) != NVL(:new.date_received,TO_DATE('1', 'J')) THEN
       audit_trail.column_update
         (raid, 'DATE_RECEIVED',
         :old.date_received, :new.date_received);
    END IF;
  END IF;

  IF UPDATING('lot_num') THEN
    IF NVL(:old.lot_num,' ') != NVL(:new.lot_num,' ') THEN
       audit_trail.column_update
         (raid, 'LOT_NUM',
         :old.lot_num, :new.lot_num);
    END IF;
  END IF;

  IF UPDATING('received_by_id_fk') THEN
    IF NVL(:old.received_by_id_fk,' ') != NVL(:new.received_by_id_fk,' ') THEN
       audit_trail.column_update
         (raid, 'RECEIVED_BY_ID_FK',
         :old.received_by_id_fk, :new.received_by_id_fk);
    END IF;
  END IF;

  IF UPDATING('final_wght') THEN
    IF NVL(:old.final_wght,0) != NVL(:new.final_wght,0) THEN
       audit_trail.column_update
         (raid, 'FINAL_WGHT',
         :old.final_wght, :new.final_wght);
    END IF;
  END IF;

  IF UPDATING('net_wght') THEN
    IF NVL(:old.net_wght,0) != NVL(:new.net_wght,0) THEN
       audit_trail.column_update
         (raid, 'NET_WGHT',
         :old.net_wght, :new.net_wght);
    END IF;
  END IF;

  IF UPDATING('qty_available') THEN
    IF NVL(:old.qty_available,0) != NVL(:new.qty_available,0) THEN
       audit_trail.column_update
         (raid, 'QTY_AVAILABLE',
         :old.qty_available, :new.qty_available);
    END IF;
  END IF;

  IF UPDATING('qty_reserved') THEN
    IF NVL(:old.qty_reserved,0) != NVL(:new.qty_reserved,0) THEN
       audit_trail.column_update
         (raid, 'QTY_RESERVED',
         :old.qty_reserved, :new.qty_reserved);
    END IF;
  END IF;

  IF UPDATING('physical_state_id_fk') THEN
    IF NVL(:old.physical_state_id_fk,0) != NVL(:new.physical_state_id_fk,0) THEN
       audit_trail.column_update
         (raid, 'PHYSICAL_STATE_ID_FK',
         :old.physical_state_id_fk, :new.physical_state_id_fk);
    END IF;
  END IF;

  IF UPDATING('supplier_id_fk') THEN
    IF NVL(:old.supplier_id_fk,0) != NVL(:new.supplier_id_fk,0) THEN
       audit_trail.column_update
         (raid, 'SUPPLIER_ID_FK',
         :old.supplier_id_fk, :new.supplier_id_fk);
    END IF;
  END IF;

  IF UPDATING('supplier_catnum') THEN
    IF NVL(:old.supplier_catnum,' ') != NVL(:new.supplier_catnum,' ') THEN
       audit_trail.column_update
         (raid, 'SUPPLIER_CATNUM',
         :old.supplier_catnum, :new.supplier_catnum);
    END IF;
  END IF;

  IF UPDATING('date_produced') THEN
    IF NVL(:old.date_produced,TO_DATE('1', 'J')) != NVL(:new.date_produced,TO_DATE('1', 'J')) THEN
       audit_trail.column_update
         (raid, 'DATE_PRODUCED',
         :old.date_produced, :new.date_produced);
    END IF;
  END IF;

  IF UPDATING('container_cost') THEN
    IF NVL(:old.container_cost,0) != NVL(:new.container_cost,0) THEN
       audit_trail.column_update
         (raid, 'CONTAINER_COST',
         :old.container_cost, :new.container_cost);
    END IF;
  END IF;

  IF UPDATING('unit_of_cost_id_fk') THEN
    IF NVL(:old.unit_of_cost_id_fk,0) != NVL(:new.unit_of_cost_id_fk,0) THEN
       audit_trail.column_update
         (raid, 'UNIT_OF_COST_ID_FK',
         :old.unit_of_cost_id_fk, :new.unit_of_cost_id_fk);
    END IF;
  END IF;

  IF UPDATING('def_location_id_fk') THEN
    IF :old.def_location_id_fk != :new.def_location_id_fk THEN
       audit_trail.column_update
         (raid, 'DEF_LOCATION_ID_FK',
         :old.def_location_id_fk, :new.def_location_id_fk);
    END IF;
  END IF;

  IF UPDATING('barcode') THEN
    IF :old.barcode != :new.barcode THEN
       audit_trail.column_update
         (raid, 'BARCODE',
         :old.barcode, :new.barcode);
   END IF;
  END IF;

  IF UPDATING('po_number') THEN
    IF NVL(:old.po_number,' ') != NVL(:new.po_number,' ') THEN
       audit_trail.column_update
         (raid, 'PO_NUMBER',
         :old.po_number, :new.po_number);
    END IF;
  END IF;

  IF UPDATING('req_number') THEN
    IF NVL(:old.req_number,' ') != NVL(:new.req_number,' ') THEN
       audit_trail.column_update
         (raid, 'REQ_NUMBER',
         :old.req_number, :new.req_number);
    END IF;
  END IF;

  IF UPDATING('reg_id_fk') THEN
    IF NVL(:old.reg_id_fk,0) != NVL(:new.reg_id_fk,0) THEN
       audit_trail.column_update
         (raid, 'REG_ID_FK',
         :old.reg_id_fk, :new.reg_id_fk);
    END IF;
  END IF;

  IF UPDATING('batch_number_fk') THEN
    IF NVL(:old.batch_number_fk,0) != NVL(:new.batch_number_fk,0) THEN
       audit_trail.column_update
         (raid, 'BATCH_NUMBER_FK',
         :old.batch_number_fk, :new.batch_number_fk);
    END IF;
  END IF;

  IF UPDATING('rid') THEN
    IF :old.rid != :new.rid THEN
       audit_trail.column_update
         (raid, 'RID',
         :old.rid, :new.rid);
    END IF;
  END IF;

  IF UPDATING('density') THEN
    IF NVL(:old.density,0) != NVL(:new.density,0) THEN
       audit_trail.column_update
         (raid, 'DENSITY',
         :old.density, :new.density);
    END IF;
  END IF;
  
  IF UPDATING('unit_of_density_id_fk') THEN
    IF NVL(:old.unit_of_density_id_fk,0) !=
NVL(:new.unit_of_density_id_fk,0) THEN
       audit_trail.column_update
         (raid, 'UNIT_OF_DENSITY_ID_FK',
         :old.unit_of_density_id_fk, :new.unit_of_density_id_fk);
    END IF;
  END IF;

  IF UPDATING('po_line_number') THEN
    IF NVL(:old.po_line_number,0) != NVL(:new.po_line_number,0) THEN
       audit_trail.column_update
         (raid, 'PO_LINE_NUMBER',
         :old.po_line_number, :new.po_line_number);
    END IF;
  END IF;

  IF UPDATING('field_1') THEN
    IF NVL(:old.field_1,' ') != NVL(:new.field_1,' ') THEN
       audit_trail.column_update
         (raid, 'FIELD_1',
         :old.field_1, :new.field_1);
    END IF;
  END IF;

  IF UPDATING('field_2') THEN
    IF NVL(:old.field_2,' ') != NVL(:new.field_2,' ') THEN
       audit_trail.column_update
         (raid, 'FIELD_2',
         :old.field_2, :new.field_2);
    END IF;
  END IF;

  IF UPDATING('field_3') THEN
    IF NVL(:old.field_3,' ') != NVL(:new.field_3,' ') THEN
       audit_trail.column_update
         (raid, 'FIELD_3',
         :old.field_3, :new.field_3);
    END IF;
  END IF;

  IF UPDATING('field_4') THEN
    IF NVL(:old.field_4,' ') != NVL(:new.field_4,' ') THEN
       audit_trail.column_update
         (raid, 'FIELD_4',
         :old.field_4, :new.field_4);
    END IF;
  END IF;

  IF UPDATING('field_5') THEN
    IF NVL(:old.field_5,' ') != NVL(:new.field_5,' ') THEN
       audit_trail.column_update
         (raid, 'FIELD_5',
         :old.field_5, :new.field_5);
    END IF;
  END IF;

  IF UPDATING('field_6') THEN
    IF NVL(:old.field_6,' ') != NVL(:new.field_6,' ') THEN
       audit_trail.column_update
         (raid, 'FIELD_6',
         :old.field_6, :new.field_6);
    END IF;
  END IF;

  IF UPDATING('field_7') THEN
    IF NVL(:old.field_7,' ') != NVL(:new.field_7,' ') THEN
       audit_trail.column_update
         (raid, 'FIELD_7',
         :old.field_7, :new.field_7);
    END IF;
  END IF;

  IF UPDATING('field_8') THEN
    IF NVL(:old.field_8,' ') != NVL(:new.field_8,' ') THEN
       audit_trail.column_update
         (raid, 'FIELD_8',
         :old.field_8, :new.field_8);
    END IF;
  END IF;

  IF UPDATING('field_9') THEN
    IF NVL(:old.field_9,' ') != NVL(:new.field_9,' ') THEN
       audit_trail.column_update
         (raid, 'FIELD_9',
         :old.field_9, :new.field_9);
    END IF;
  END IF;

  IF UPDATING('field_10') THEN
    IF NVL(:old.field_10,' ') != NVL(:new.field_10,' ') THEN
       audit_trail.column_update
         (raid, 'FIELD_1',
         :old.field_10, :new.field_10);
    END IF;
  END IF;

  IF UPDATING('date_1') THEN
    IF NVL(:old.date_1,TO_DATE('1', 'J')) != NVL(:new.date_1,TO_DATE('1',
'J')) THEN
       audit_trail.column_update
         (raid, 'DATE_1',
         :old.DATE_1, :new.DATE_1);
    END IF;
  END IF;

  IF UPDATING('date_2') THEN
    IF NVL(:old.date_2,TO_DATE('1', 'J')) != NVL(:new.date_2,TO_DATE('1',
'J')) THEN
       audit_trail.column_update
         (raid, 'DATE_2',
         :old.DATE_2, :new.DATE_2);
    END IF;
  END IF;

  IF UPDATING('date_3') THEN
    IF NVL(:old.date_3,TO_DATE('1', 'J')) != NVL(:new.date_3,TO_DATE('1',
'J')) THEN
       audit_trail.column_update
         (raid, 'DATE_3',
         :old.DATE_3, :new.DATE_3);
    END IF;
  END IF;

  IF UPDATING('date_4') THEN
    IF NVL(:old.date_4,TO_DATE('1', 'J')) != NVL(:new.date_4,TO_DATE('1',
'J')) THEN
       audit_trail.column_update
         (raid, 'DATE_4',
         :old.DATE_4, :new.DATE_4);
    END IF;
  END IF;

  IF UPDATING('date_5') THEN
    IF NVL(:old.date_5,TO_DATE('1', 'J')) != NVL(:new.date_5,TO_DATE('1',
'J')) THEN
       audit_trail.column_update
         (raid, 'DATE_5',
         :old.DATE_5, :new.DATE_5);
    END IF;
  END IF;

  IF UPDATING('parent_container_id_fk') THEN
    IF NVL(:old.parent_container_id_fk,0) != NVL(:new.parent_container_id_fk,0) THEN
       audit_trail.column_update
         (raid, 'PARENT_CONTAINER_ID_FK',
         :old.parent_container_id_fk, :new.parent_container_id_fk);
    END IF;
  END IF;

  IF UPDATING('family') THEN
    IF NVL(:old.family,0) != NVL(:new.family,0) THEN
       audit_trail.column_update
         (raid, 'FAMILY',
         :old.family, :new.family);
    END IF;
  END IF;

  IF UPDATING('storage_conditions') THEN
    IF NVL(:old.storage_conditions,' ') != NVL(:new.storage_conditions,' ') THEN
       audit_trail.column_update
         (raid, 'STORAGE_CONDITIONS',
         :old.storage_conditions, :new.storage_conditions);
    END IF;
  END IF;

  IF UPDATING('handling_procedures') THEN
    IF NVL(:old.handling_procedures,' ') != NVL(:new.handling_procedures,' ') THEN
       audit_trail.column_update
         (raid, 'HANDLING_PROCEDURES',
         :old.handling_procedures, :new.handling_procedures);
    END IF;
  END IF;

  IF UPDATING('date_certified') THEN
    IF NVL(:old.date_certified,TO_DATE('1', 'J')) != NVL(:new.date_certified,TO_DATE('1', 'J')) THEN
       audit_trail.column_update
         (raid, 'DATE_CERTIFIED',
         :old.date_certified, :new.date_certified);
    END IF;
  END IF;

  IF UPDATING('date_approved') THEN
    IF NVL(:old.date_approved,TO_DATE('1', 'J')) != NVL(:new.date_approved,TO_DATE('1', 'J')) THEN
       audit_trail.column_update
         (raid, 'DATE_APPROVED',
         :old.date_approved, :new.date_approved);
    END IF;
  END IF;

END;
/
