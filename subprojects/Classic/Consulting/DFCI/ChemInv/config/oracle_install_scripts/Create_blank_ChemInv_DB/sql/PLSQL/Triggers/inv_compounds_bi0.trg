rem inv_compounds_bi0.trg
-- Audit Trigger Before Insert on inv_compounds
create or replace trigger TRG_AUDIT_INV_COMPOUNDS_BI0
  before insert
  on inv_compounds
  for each row
  when (new.rid is null or new.rid = 0)

begin
  select trunc(seq_rid.nextval)
  into :new.rid
  from dual;
end;
/

