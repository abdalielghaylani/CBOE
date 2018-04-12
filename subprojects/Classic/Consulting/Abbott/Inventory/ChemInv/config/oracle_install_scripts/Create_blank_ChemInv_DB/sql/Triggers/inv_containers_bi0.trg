rem inv_containers_bi0.trg
-- Audit Trigger Before Insert on inv_containers
create or replace trigger TRG_AUDIT_INV_CONTAINERS_BI0
  before insert
  on inv_containers
  for each row
  when (new.rid is null or new.rid = 0)

begin
  select trunc(seq_rid.nextval)
  into :new.rid
  from dual;
end;
/

