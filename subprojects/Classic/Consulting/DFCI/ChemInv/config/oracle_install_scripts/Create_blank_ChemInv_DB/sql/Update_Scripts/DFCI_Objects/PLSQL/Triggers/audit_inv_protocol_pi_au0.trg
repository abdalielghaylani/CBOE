Create or Replace TRIGGER TRG_AUDIT_INV_PROTOCOL_PI_AU0
  after update of
  protocol_id_FK,
  PI,
  start_date,
  end_date,
  rid
  on inv_protocol_pi
  for each row
declare
  raid number(10);
l_newval VARCHAR2(4000);
l_oldval VARCHAR2(4000);
begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_PROTOCOL_PI', :old.rid, 'U');
  audit_trail.check_val(raid, 'PROTOCOL_ID_FK', :new.PROTOCOL_ID_FK, :old.PROTOCOL_ID_FK);
  audit_trail.check_val(raid, 'PI', :new.PI, :old.PI);
  audit_trail.check_val(raid, 'START_DATE', :new.START_DATE, :old.START_DATE);
  audit_trail.check_val(raid, 'END_DATE', :new.END_DATE, :old.END_DATE);

  audit_trail.check_val(raid, 'RID', :new.RID, :old.RID);
  --audit_trail.check_val(raid, 'CREATOR', :new.CREATOR, :old.CREATOR);
  --audit_trail.check_val(raid, 'TIMESTAMP', :new.TIMESTAMP, :old.TIMESTAMP);
end;

/
