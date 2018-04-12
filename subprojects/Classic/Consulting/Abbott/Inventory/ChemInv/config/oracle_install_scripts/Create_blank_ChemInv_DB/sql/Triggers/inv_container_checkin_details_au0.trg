-- inv_container_checkin_details_au0.trg
-- Generated on 26-JAN-05
-- Script to create AFTER-UPDATE audit trigger for the INV_CONTAINER_CHECKIN_DETAILS table.

create or replace trigger TRG_AUDIT_INV_CTNR_CHK_DTL_AU0
  after update of
  checkin_details_id,
  container_id_fk,
  user_id_fk,
  field_1,
  field_2,
  field_3,
  field_4,
  field_5,
  field_6,
  field_7,
  field_8,
  field_9,
  field_10,
  date_1,
  date_2,
  date_3,
  rid

  on inv_container_checkin_details
  for each row
  
declare
  raid number(10);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_CONTAINER_CHECKIN_DETAILS', :old.rid, 'U');
 if updating('checkin_details_id') then
  if :old.checkin_details_id != :new.checkin_details_id then
     audit_trail.column_update
       (raid, 'CHECKIN_DETAILS_ID',
       :old.checkin_details_id, :new.checkin_details_id);
  end if;	
 end if; 
 if updating('container_id_fk') then 
   if :old.container_id_fk != :new.container_id_fk then
     audit_trail.column_update
       (raid, 'CONTAINER_ID_FK',
       :old.container_id_fk, :new.container_id_fk);
  end if;      
 end if; 
 if updating('user_id_fk') then 
   if :old.user_id_fk != :new.user_id_fk then
     audit_trail.column_update
       (raid, 'USER_ID_FK',
       :old.user_id_fk, :new.user_id_fk);
  end if;
 end if; 
 if updating('field_1') then 
   if :old.field_1 != :new.field_1 then
     audit_trail.column_update
       (raid, 'FIELD_1',
       :old.field_1, :new.field_1);
  end if;
 end if; 
 if updating('field_2') then 
   if :old.field_2 != :new.field_2 then
     audit_trail.column_update
       (raid, 'FIELD_2',
       :old.field_2, :new.field_2);
  end if;
 end if; 
 if updating('field_3') then 
   if :old.field_3 != :new.field_3 then
     audit_trail.column_update
       (raid, 'FIELD_3',
       :old.field_3, :new.field_3);
  end if;
 end if; 
 if updating('field_4') then 
   if :old.field_4 != :new.field_4 then
     audit_trail.column_update
       (raid, 'FIELD_4',
       :old.field_4, :new.field_4);
  end if;
 end if; 
 if updating('field_5') then 
   if :old.field_5 != :new.field_5 then
     audit_trail.column_update
       (raid, 'FIELD_5',
       :old.field_5, :new.field_5);
  end if;
 end if; 
 if updating('field_6') then 
   if :old.field_6 != :new.field_6 then
     audit_trail.column_update
       (raid, 'FIELD_6',
       :old.field_6, :new.field_6);
  end if;
 end if; 
 if updating('field_7') then 
   if :old.field_7 != :new.field_7 then
     audit_trail.column_update
       (raid, 'FIELD_7',
       :old.field_7, :new.field_7);
  end if;
 end if; 
 if updating('field_8') then 
   if :old.field_8 != :new.field_8 then
     audit_trail.column_update
       (raid, 'FIELD_8',
       :old.field_8, :new.field_8);
  end if;
 end if; 
 if updating('field_9') then 
   if :old.field_9 != :new.field_9 then
     audit_trail.column_update
       (raid, 'FIELD_9',
       :old.field_9, :new.field_9);
  end if;
 end if; 
 if updating('field_10') then 
   if :old.field_10 != :new.field_10 then
     audit_trail.column_update
       (raid, 'FIELD_10',
       :old.field_10, :new.field_10);
  end if;
 end if; 
 if updating('date_1') then 
  if nvl(:old.date_1,TO_DATE('1', 'J')) !=
     NVL(:new.date_1,TO_DATE('1', 'J')) then
     audit_trail.column_update
       (raid, 'DATE_1',
       :old.date_1, :new.date_1);
  end if;
 end if; 
 if updating('date_2') then 
  if nvl(:old.date_2,TO_DATE('1', 'J')) !=
     NVL(:new.date_2,TO_DATE('1', 'J')) then
     audit_trail.column_update
       (raid, 'DATE_2',
       :old.date_2, :new.date_2);
  end if;
 end if; 
 if updating('date_3') then 
  if nvl(:old.date_3,TO_DATE('1', 'J')) !=
     NVL(:new.date_3,TO_DATE('1', 'J')) then
     audit_trail.column_update
       (raid, 'DATE_3',
       :old.date_3, :new.date_3);
  end if;
 end if; 
 if updating('rid') then 
   if :old.rid != :new.rid then
     audit_trail.column_update
       (raid, 'RID',
       :old.rid, :new.rid);
  end if;     
 end if;
end;

/
