-- inv_url_au0.trg
-- After update on inv_url 
create or replace trigger TRG_AUDIT_INV_URL_AU0
  after update of
  url_id,
  fk_value,
  table_name,
  fk_name,
  url,
  link_txt,
  image_src,
  sort_order,
  url_type,
  rid
  on inv_url
  for each row

declare
  raid number(10);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_URL', :old.rid, 'U');

  audit_trail.check_val(raid, 'URL_ID', :new.URL_ID, :old.URL_ID);
  audit_trail.check_val(raid, 'FK_VALUE', :new.FK_VALUE, :old.FK_VALUE);
  audit_trail.check_val(raid, 'TABLE_NAME', :new.TABLE_NAME, :old.TABLE_NAME);
  audit_trail.check_val(raid, 'FK_NAME', :new.FK_NAME, :old.FK_NAME);
  audit_trail.check_val(raid, 'URL', :new.URL, :old.URL);
  audit_trail.check_val(raid, 'LINK_TXT', :new.LINK_TXT, :old.LINK_TXT);
  audit_trail.check_val(raid, 'IMAGE_SRC', :new.IMAGE_SRC, :old.IMAGE_SRC);
  --audit_trail.check_val(raid, 'IMAGE', :new.IMAGE, :old.IMAGE);
  audit_trail.check_val(raid, 'SORT_ORDER', :new.SORT_ORDER, :old.SORT_ORDER);
  audit_trail.check_val(raid, 'URL_TYPE', :new.URL_TYPE, :old.URL_TYPE);
  audit_trail.check_val(raid, 'RID', :new.RID, :old.RID);
  --audit_trail.check_val(raid, 'CREATOR', :new.CREATOR, :old.CREATOR);
  --audit_trail.check_val(raid, 'TIMESTAMP', :new.TIMESTAMP, :old.TIMESTAMP);  
end;
/

