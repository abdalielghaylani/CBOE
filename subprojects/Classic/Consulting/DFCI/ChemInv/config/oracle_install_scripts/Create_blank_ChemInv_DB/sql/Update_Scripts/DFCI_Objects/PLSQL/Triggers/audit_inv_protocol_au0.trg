CREATE OR REPLACE TRIGGER TRG_AUDIT_INV_PROTOCOL_AU0
  after update of
  protocol_identifier,
  start_date,
  end_date,
  rid
  on inv_protocol
  for each row
declare
  raid number(10);
l_newval VARCHAR2(4000);
l_oldval VARCHAR2(4000);
begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_PROTOCOL', :old.rid, 'U');

  audit_trail.check_val(raid, 'PROTOCOL_IDENTIFIER', :new.PROTOCOL_IDENTIFIER, :old.PROTOCOL_IDENTIFIER);
  audit_trail.check_val(raid, 'PROTOCOL_NAME', :new.PROTOCOL_NAME, :old.PROTOCOL_NAME);
  audit_trail.check_val(raid, 'SPONSOR_ID', :new.SPONSOR_ID, :old.SPONSOR_ID);
  audit_trail.check_val(raid, 'NCI_PROTOCOL_NUM', :new.NCI_PROTOCOL_NUM, :old.NCI_PROTOCOL_NUM);
  audit_trail.check_val(raid, 'START_DATE', :new.START_DATE, :old.START_DATE);
  audit_trail.check_val(raid, 'END_DATE', :new.END_DATE, :old.END_DATE);

  audit_trail.check_val(raid, 'RID', :new.RID, :old.RID);
  --audit_trail.check_val(raid, 'CREATOR', :new.CREATOR, :old.CREATOR);
  --audit_trail.check_val(raid, 'TIMESTAMP', :new.TIMESTAMP, :old.TIMESTAMP);
end;

/