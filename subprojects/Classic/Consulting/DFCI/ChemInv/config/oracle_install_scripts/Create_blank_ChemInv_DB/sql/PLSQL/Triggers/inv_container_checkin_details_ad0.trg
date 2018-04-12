-- inv_container_checkin_details_ad0.trg
-- Generated on 26-JAN-05
-- Script to create AFTER-DELETE audit trigger for the INV_CONTAINER_CHECKIN_DETAILS table.

create or replace trigger TRG_AUDIT_INV_CTNR_CHK_DTL_AD0
  after delete
  on inv_container_checkin_details
  for each row

declare
  raid number(10);
  deleted_data varchar2(4000);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_CONTAINER_CHECKIN_DETAILS', :old.rid, 'D');

  deleted_data :=
  to_char(:old.checkin_details_id) || '|' ||
  to_char(:old.container_id_fk) || '|' ||
  :old.user_id_fk || '|' ||
  :old.field_1 || '|' ||
  :old.field_2 || '|' ||
  :old.field_3 || '|' ||
  :old.field_4 || '|' ||
  :old.field_5 || '|' ||
  :old.field_6 || '|' ||
  :old.field_7 || '|' ||
  :old.field_8 || '|' ||
  :old.field_9 || '|' ||
  :old.field_10 || '|' ||
  to_char(:old.date_1) || '|' ||
  to_char(:old.date_2) || '|' ||
  to_char(:old.date_3) || '|' ||
  to_char(:old.rid) || '|' ||
  :old.creator || '|' ||
  to_char(:old.timestamp);

insert into audit_delete
(raid, row_data) values (raid, deleted_data);
end;
/
