-- css_people_ad0.trg
-- Generated on 28-JAN-05
-- Script to create AFTER-DELETE audit trigger for the PEOPLE table.

create or replace trigger TRG_AUDIT_CSS_PEOPLE_AD0
  after delete
  on people
  for each row

declare
  raid number(10);
  deleted_data varchar2(4000);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'PEOPLE', :old.rid, 'D');

  deleted_data :=
	to_char(:old.person_id) || '|' ||
	:old.user_code || '|' ||
	:old.user_id || '|' ||
	to_char(:old.supervisor_internal_id) || '|' ||
	:old.title || '|' ||
	:old.first_name || '|' ||
	:old.middle_name || '|' ||
	:old.last_name || '|' ||
	to_char(:old.site_id) || '|' ||
	:old.department || '|' ||
	:old.int_address || '|' ||
	:old.telephone || '|' ||
	:old.email || '|' ||
	to_char(:old.active) || '|' ||
    to_char(:old.rid) || '|' ||
    :old.creator || '|' ||
    to_char(:old.timestamp);

insert into audit_delete
(raid, row_data) values (raid, deleted_data);
end;

/
