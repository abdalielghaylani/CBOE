-- cs_security_privileges_ad0.trg
-- Generated on 28-JAN-05
-- Script to create AFTER-DELETE audit trigger for the CS_SECURITY_PRIVILEGES table.

create or replace trigger TRG_AUDIT_CS_SEC_PRIV_AD0
  after delete
  on cs_security_privileges
  for each row

declare
  raid number(10);
  deleted_data varchar2(4000);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'CS_SECURITY_PRIVILEGES', :old.rid, 'D');

  deleted_data :=
	to_char(:old.role_internal_id) || '|' ||
	to_char(:old.css_login) || '|' ||
	to_char(:old.css_create_user) || '|' ||
	to_char(:old.css_edit_user) || '|' ||
	to_char(:old.css_delete_user) || '|' ||
	to_char(:old.css_change_password) || '|' ||
	to_char(:old.css_create_role) || '|' ||
	to_char(:old.css_edit_role) || '|' ||
	to_char(:old.css_delete_role) || '|' ||
	to_char(:old.css_create_workgrp) || '|' ||
	to_char(:old.css_edit_workgrp) || '|' ||
	to_char(:old.css_delete_workgrp) || '|' ||
    to_char(:old.rid) || '|' ||
    :old.creator || '|' ||
    to_char(:old.timestamp);

insert into audit_delete
(raid, row_data) values (raid, deleted_data);
end;

/
