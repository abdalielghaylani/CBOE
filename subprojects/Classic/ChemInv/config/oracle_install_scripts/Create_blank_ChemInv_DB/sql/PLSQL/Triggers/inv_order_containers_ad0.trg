-- inv_order_containers_ad0.trg
-- Generated on 25-JAN-05
-- Script to create AFTER-DELETE audit trigger for the INV_ORDER_CONTAINERS table.

create or replace trigger TRG_AUDIT_INV_ORD_CNTNRS_AD0
  after delete
  on inv_order_containers
  for each row

declare
  raid number(10);
  deleted_data varchar2(4000);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_ORDER_CONTAINERS', :old.rid, 'D');

  deleted_data :=
	to_char(:old.order_id_fk) || '|' ||
	to_char(:old.container_id_fk) || '|' ||
  to_char(:old.rid) || '|' ||
  :old.creator || '|' ||
  to_char(:old.timestamp);

insert into audit_delete
(raid, row_data) values (raid, deleted_data);
end;

/
