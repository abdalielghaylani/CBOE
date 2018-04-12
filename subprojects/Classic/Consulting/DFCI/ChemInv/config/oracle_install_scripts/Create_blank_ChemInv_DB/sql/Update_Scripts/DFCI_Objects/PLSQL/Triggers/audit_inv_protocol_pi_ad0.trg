Create or replace TRIGGER TRG_AUDIT_INV_PROTOCOL_PI_AD0
  after delete
  on inv_protocol_pi
  for each row
declare
  raid number(10);
  deleted_data varchar2(4000);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_PROTOCOL_PI', :old.rid, 'D');

  deleted_data :=
  to_char(:old.protocol_pi_id) || '|' ||
  to_char(:old.protocol_id_fk) || '|' ||
   to_char(:old.pi) || '|' ||
 to_char(:old.start_date) || '|' ||
  to_char(:old.end_date) || '|' ;

insert into audit_delete
(raid, row_data) values (raid, deleted_data);
end;

/