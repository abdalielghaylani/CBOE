-- inv_requests_ad0.trg
-- Generated on 31-AUG-02
-- Script to create AFTER-DELETE audit trigger for the INV_REQUESTS table.

create or replace trigger TRG_AUDIT_INV_REQUESTS_AD0
  after delete
  on inv_requests
  for each row

declare
  raid number(10);
  deleted_data varchar2(4000);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_REQUESTS', :old.rid, 'D');

  deleted_data :=
  to_char(:old.request_id) || '|' ||
  to_char(:old.container_id_fk) || '|' ||
  :old.user_id_fk || '|' ||
  to_char(:old.date_required) || '|' ||
  to_char(:old.date_delivered) || '|' ||
  :old.delivered_by_id_fk || '|' ||
  to_char(:old.qty_required) || '|' ||
  to_char(:old.delivery_location_id_fk) || '|' ||
  :old.request_comments || '|' ||
  to_char(:old.request_type_id_fk) || '|' ||
  to_char(:old.request_status_id_fk) || '|' ||
  to_char(:old.container_type_id_fk) || '|' ||
  to_char(:old.number_containers) || '|' ||
	:old.quantity_list || '|' ||
	:old.ship_to_name || '|' ||
	:old.decline_reason || '|' ||
  :old.expense_center || '|' ||
  to_char(:old.rid) || '|' ||
  :old.creator || '|' ||
  to_char(:old.timestamp);

insert into audit_delete
(raid, row_data) values (raid, deleted_data);
end;
/
