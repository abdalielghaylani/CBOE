-- css_security_roles_ad0.trg
-- Generated on 28-JAN-05
-- Script to create AFTER-DELETE audit trigger for the SECURITY_ROLES table.

create or replace trigger TRG_AUDIT_CSS_SEC_ROLES_AD0
  after delete
  on security_roles
  for each row

declare
  raid number(10);
  deleted_data varchar2(4000);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'SECURITY_ROLES', :old.rid, 'D');

  deleted_data :=
	to_char(:old.role_id) || '|' ||
	to_char(:old.privilege_table_int_id) || '|' ||
	to_char(:old.role_name) || '|' ||
    to_char(:old.rid) || '|' ||
    :old.creator || '|' ||
    to_char(:old.timestamp);

insert into audit_delete
(raid, row_data) values (raid, deleted_data);
end;

/
