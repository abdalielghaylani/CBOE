CREATE OR REPLACE TRIGGER TRG_AUDIT_INV_PROTOCOL_BI0
  before insert
  on inv_PROTOCOL
  for each row
  WHEN ( new.rid is null or new.rid = 0 ) 
begin
  select trunc(seq_rid.nextval)
  into :new.rid
  from dual;
end;
/