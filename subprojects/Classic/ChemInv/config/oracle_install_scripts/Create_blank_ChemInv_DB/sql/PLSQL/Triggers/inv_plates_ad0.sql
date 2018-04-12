CREATE OR REPLACE TRIGGER TRG_AUDIT_INV_PLATES_AD0
  after delete
  on inv_plates
  for each row
declare
  raid number(10);
  deleted_data varchar2(4000);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_PLATES', :old.rid, 'D');

  deleted_data :=
  to_char(:old.PLATE_ID) || '|' ||
  to_char(:old.LOCATION_ID_FK) || '|' ||
  to_char(:old.CONTAINER_ID_FK) || '|' ||
  to_char(:old.PLATE_TYPE_ID_FK) || '|' ||
  to_char(:old.PLATE_FORMAT_ID_FK) || '|' ||
  :old.PLATE_BARCODE || '|' ||
  :old.PLATE_NAME|| '|' ||
  to_char(:old.STATUS_ID_FK) || '|' ||
  :old.GROUP_NAME || '|' ||
  to_char(:old.LIBRARY_ID_FK) || '|' ||
  to_char(:old.FT_CYCLES) || '|' ||
  to_char(:old.WEIGHT) || '|' ||
  to_char(:old.WEIGHT_UNIT_FK) || '|' ||
  to_char(:old.QTY_INITIAL) || '|' ||
  to_char(:old.QTY_INITIAL) || '|' ||
  to_char(:old.QTY_UNIT_FK) || '|' ||
  :old.SOLVENT || '|' ||
  to_char(:old.CONCENTRATION) || '|' ||
  to_char(:old.CONCENTRATION) || '|' ||
  to_char(:old.DATE_CREATED) || '|' ||
  :old.SUPPLIER_BARCODE || '|' ||
  :old.SUPPLIER_SHIPMENT_CODE || '|' ||
  to_char(:old.SUPPLIER_SHIPMENT_NUMBER) || '|' ||
  to_char(:old.SUPPLIER_SHIPMENT_DATE) || '|' ||
  to_char(:old.SOLVENT_ID_FK) || '|' ||
  to_char(:old.SOLVENT_VOLUME) || '|' ||
  to_char(:old.SOLVENT_VOLUME_INITIAL) || '|' ||
  to_char(:old.SOLVENT_VOLUME_UNIT_ID_FK) || '|' ||
  to_char(:old.MOLAR_AMOUNT) || '|' ||
  to_char(:old.MOLAR_UNIT_FK) || '|' ||
  to_char(:old.MOLAR_CONC) || '|' ||
  to_char(:old.WELL_CAPACITY) || '|' ||
  to_char(:old.WELL_CAPACITY_UNIT_ID_FK) || '|' ||
  to_char(:old.AMOUNTS_DIFFER) || '|' ||
  :old.SUPPLIER || '|' ||
  to_char(:old.PLATE_EXISTS) || '|' ||
  to_char(:old.IS_PLATE_MAP) || '|' ||
  to_char(:old.IS_PLATE_MAP) || '|' ||
  to_char(:old.PURITY) || '|' ||
  to_char(:old.PURITY_UNIT_FK) || '|' ||
  :old.field_1 || '|' ||
  :old.field_2 || '|' ||
  :old.field_3 || '|' ||
  :old.field_4 || '|' ||
  :old.field_5 || '|' ||
  to_char(:old.date_1) || '|' ||
  to_char(:old.date_2);

insert into audit_delete
(raid, row_data) values (raid, deleted_data);
end;
/


