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

  if :old.url_id != :new.url_id then
     audit_trail.column_update
       (raid, 'URL_ID',
       :old.url_id, :new.url_id);
  end if;
  if :old.fk_value != :new.fk_value then
     audit_trail.column_update
       (raid, 'FK_VALUE',
       :old.fk_value, :new.fk_value);
  end if;
  if :old.table_name != :new.table_name then
     audit_trail.column_update
       (raid, 'TABLE_NAME',
       :old.table_name, :new.table_name);
  end if;
  if :old.fk_name != :new.fk_name then
     audit_trail.column_update
       (raid, 'FK_NAME',
       :old.fk_name, :new.fk_name);
  end if;
  if :old.url != :new.url then
     audit_trail.column_update
       (raid, 'URL',
       :old.url, :new.url);
  end if;
  if :old.link_txt != :new.link_txt then
     audit_trail.column_update
       (raid, 'LINK_TXT',
       :old.link_txt, :new.link_txt);
  end if;
  if nvl(:old.image_src,' ') !=
     NVL(:new.image_src,' ') then
     audit_trail.column_update
       (raid, 'IMAGE_SRC',
       :old.image_src, :new.image_src);
  end if;
  if nvl(:old.sort_order,0) !=
     NVL(:new.sort_order,0) then
     audit_trail.column_update
       (raid, 'SORT_ORDER',
       :old.sort_order, :new.sort_order);
  end if;
  if nvl(:old.url_type,' ') !=
     NVL(:new.url_type,' ') then
     audit_trail.column_update
       (raid, 'URL_TYPE',
       :old.url_type, :new.url_type);
  end if;
  if :old.rid !=:new.rid then
     audit_trail.column_update
       (raid, 'RID',
       :old.rid, :new.rid);
  end if;
end;
/

