rem inv_requests_bi0.trg
rem Generated on 31-AUG-02
rem Script to create BI audit trigger for the INV_REQUESTS table.

create or replace trigger TRG_AUDIT_INV_REQUESTS_BI0
  before insert
  on inv_requests
  for each row
  when (new.rid is null or new.rid = 0)

begin
  select trunc(seq_rid.nextval)
  into :new.rid
  from dual;
end;
/

