rem docmgr_documents_bi0.trg
-- Audit Trigger Before Insert on DOCMGR_DOCUMENTS
create or replace trigger TRG_AUDIT_DMDOC_BI0
  before insert
  on docmgr_documents
  for each row
  when (new.rid is null or new.rid = 0)

begin
  select trunc(seq_rid.nextval)
  into :new.rid
  from dual;
end;
/

