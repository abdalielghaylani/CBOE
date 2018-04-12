-- inv_suppliers_ad0.trg
-- Generated on 24-JAN-05
-- Script to create AFTER-DELETE audit trigger for the INV_SUPPLIERS table.

create or replace trigger TRG_AUDIT_INV_SUPPLIERS_AD0
  after delete
  on inv_suppliers
  for each row

declare
  raid number(10);
  deleted_data varchar2(4000);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_SUPPLIERS', :old.rid, 'D');

  deleted_data :=
	to_char(:old.supplier_id) || '|' ||
	:old.supplier_name || '|' ||
	:old.supplier_short_name || '|' ||
	:old.supplier_code || '|' ||
	:old.supplier_facility_name || '|' ||
	to_char(:old.supplier_address_id_fk) || '|' ||
	to_char(:old.is_official_supplier) || '|' ||
  to_char(:old.rid) || '|' ||
  :old.creator || '|' ||
  to_char(:old.timestamp);

insert into audit_delete
(raid, row_data) values (raid, deleted_data);
end;

/
