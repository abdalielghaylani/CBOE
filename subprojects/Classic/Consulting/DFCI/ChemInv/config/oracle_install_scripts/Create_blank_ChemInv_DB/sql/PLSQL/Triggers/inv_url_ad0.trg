-- inv_url_ad0.trg
-- After delte on inv_url 
create or replace trigger TRG_AUDIT_INV_URL_AD0
  after delete
  on inv_url
  for each row

declare
  raid number(10);
  deleted_data varchar2(4000);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_URL', :old.rid, 'D');

  deleted_data :=
  to_char(:old.url_id) || '|' ||
  :old.fk_value || '|' ||
  :old.table_name || '|' ||
  :old.fk_name || '|' ||
  :old.url || '|' ||
  :old.link_txt || '|' ||
  :old.image_src || '|' ||
  to_char(:old.sort_order) || '|' ||
  :old.url_type || '|' ||
  to_char(:old.rid) || '|' ||
  :old.creator || '|' ||
  to_char(:old.timestamp);

insert into audit_delete
(raid, row_data) values (raid, deleted_data);
end;
/

