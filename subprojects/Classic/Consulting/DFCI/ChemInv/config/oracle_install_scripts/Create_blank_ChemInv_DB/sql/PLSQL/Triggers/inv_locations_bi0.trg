-- Audit Trigger Before Insert on inv_locations
create or replace trigger TRG_AUDIT_INV_LOCATIONS_BI0
  before insert
  on inv_locations
  for each row
  when (new.rid is null or new.rid = 0)

begin
  select trunc(seq_rid.nextval)
  into :new.rid
  from dual;
end;
/


