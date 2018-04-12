CREATE OR REPLACE TRIGGER TRG_AUDIT_INV_WELLS_AD0
  after delete
  on inv_wells
  for each row
declare
  raid number(10);
  deleted_data varchar2(4000);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_WELLS', :old.rid, 'D');

  deleted_data :=
  to_char(:old.WELL_ID) || '|' ||
  to_char(:old.WELL_FORMAT_ID_FK) || '|' ||
  to_char(:old.PLATE_ID_FK) || '|' ||
  to_char(:old.PLATE_FORMAT_ID_FK) || '|' ||
  to_char(:old.GRID_POSITION_ID_FK) || '|' ||
  to_char(:old.WEIGHT) || '|' ||
  to_char(:old.WEIGHT_UNIT_FK) || '|' ||
  to_char(:old.QTY_INITIAL) || '|' ||
  to_char(:old.QTY_REMAINING) || '|' ||
  to_char(:old.QTY_UNIT_FK) || '|' ||
  to_char(:old.SOLVENT_ID_FK) || '|' ||
  to_char(:old.SOLVENT_VOLUME) || '|' ||
  to_char(:old.SOLVENT_VOLUME_INITIAL) || '|' ||
  to_char(:old.SOLVENT_VOLUME_UNIT_ID_FK) || '|' ||
  to_char(:old.SOLUTION_VOLUME) || '|' ||
  to_char(:old.CONCENTRATION) || '|' ||
  to_char(:old.CONC_UNIT_FK) || '|' ||
  to_char(:old.MOLAR_AMOUNT) || '|' ||
  to_char(:old.MOLAR_UNIT_FK) || '|' ||
  to_char(:old.MOLAR_CONC) || '|' ||
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


