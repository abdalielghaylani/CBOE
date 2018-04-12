-- docmgr_external_links_ad0.trg
-- Audit Trigger After Delete on docmgr_external_links

create or replace trigger TRG_AUDIT_DMEL_AD0
  after delete
  on docmgr_external_links
  for each row

declare
  raid number(10);
  deleted_data varchar2(4000);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'docmgr_external_links', :old.rid, 'D');

  deleted_data :=
  to_char(:old.RID) || '|' ||
  (:old.APPNAME) || '|' ||
  (:old.LINKTYPE) || '|' ||
  (:old.LINKID) || '|' ||
  to_char(:old.DOCID) || '|' ||
  (:old.LINKFIELDNAME) || '|' ||
  (:old.SUBMITTER) || '|' ||
  to_char(:old.DATE_SUBMITTED);

insert into audit_delete
(raid, row_data) values (raid, deleted_data);
end;
/
