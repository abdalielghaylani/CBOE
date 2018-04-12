-- inv_orders_ad0.trg
-- Generated on 24-JAN-05
-- Script to create AFTER-DELETE audit trigger for the INV_ORDERS table.

create or replace trigger TRG_AUDIT_INV_ORDERS_AD0
  after delete
  on inv_orders
  for each row

declare
  raid number(10);
  deleted_data varchar2(4000);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_ORDERS', :old.rid, 'D');

  deleted_data :=
  to_char(:old.order_id) || '|' ||
  to_char(:old.delivery_location_id_fk) || '|' ||
  :old.ship_to_name || '|' ||
  to_char(:old.order_status_id_fk) || '|' ||
  :old.shipping_conditions || '|' ||
  to_char(:old.date_created) || '|' ||
  to_char(:old.date_shipped) || '|' ||
  to_char(:old.date_received) || '|' ||
  :old.cancel_reason || '|' ||
  to_char(:old.rid) || '|' ||
  :old.creator || '|' ||
  to_char(:old.timestamp);

insert into audit_delete
(raid, row_data) values (raid, deleted_data);
end;
/