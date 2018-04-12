rem inv_synonyms_bi0.trg

create or replace trigger TRG_AUDIT_INV_SYNONYMS_BI0
  before insert
  on inv_synonyms
  for each row
  when (new.rid is null or new.rid = 0)

begin
  select trunc(seq_rid.nextval)
  into :new.rid
  from dual;
end;
/

