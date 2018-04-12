-- docmgr_documents_ad0.trg
-- Audit Trigger After Delete on docmgr_documents

create or replace trigger TRG_AUDIT_DMDOC_AD0
  after delete
  on docmgr_documents
  for each row

declare
  raid number(10);
  deleted_data varchar2(4000);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'docmgr_documents', :old.rid, 'D');

  deleted_data :=
  to_char(:old.RID) || '|' ||
  (:old.DOCID) || '|' ||
  (:old.DOCLOCATION) || '|' ||
  (:old.DOCNAME) || '|' ||
  (:old.DOCSIZE) || '|' ||
  (:old.TITLE) || '|' ||
  (:old.AUTHOR) || '|' ||
  (:old.SUBMITTER) || '|' ||
  (:old.SUBMITTER_COMMENTS) || '|' ||
  (:old.DATE_SUBMITTED);

insert into audit_delete
(raid, row_data, doc_blob) values (raid, deleted_data, :old.DOC);
end;
/
