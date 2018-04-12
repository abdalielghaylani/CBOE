rem docmgr_external_links_bi0.trg
-- Audit Trigger Before Insert on DOCMGR_EXTERNAL_LINKS
create or replace trigger TRG_AUDIT_DMEL_BI0
  before insert
  on docmgr_external_links
  for each row
  when (new.rid is null or new.rid = 0)

begin
  select trunc(seq_rid.nextval)
  into :new.rid
  from dual;
end;
/

