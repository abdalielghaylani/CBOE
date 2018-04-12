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

  audit_trail.check_val(raid, 'CHECKIN_DETAILS_ID', :new.CHECKIN_DETAILS_ID, :old.CHECKIN_DETAILS_ID);
  audit_trail.check_val(raid, 'CONTAINER_ID_FK', :new.CONTAINER_ID_FK, :old.CONTAINER_ID_FK);
  audit_trail.check_val(raid, 'USER_ID_FK', :new.USER_ID_FK, :old.USER_ID_FK);
  audit_trail.check_val(raid, 'FIELD_1', :new.FIELD_1, :old.FIELD_1);
  audit_trail.check_val(raid, 'FIELD_2', :new.FIELD_2, :old.FIELD_2);
  audit_trail.check_val(raid, 'FIELD_3', :new.FIELD_3, :old.FIELD_3);
  audit_trail.check_val(raid, 'FIELD_4', :new.FIELD_4, :old.FIELD_4);
  audit_trail.check_val(raid, 'FIELD_5', :new.FIELD_5, :old.FIELD_5);
  audit_trail.check_val(raid, 'FIELD_6', :new.FIELD_6, :old.FIELD_6);
  audit_trail.check_val(raid, 'FIELD_7', :new.FIELD_7, :old.FIELD_7);
  audit_trail.check_val(raid, 'FIELD_8', :new.FIELD_8, :old.FIELD_8);
  audit_trail.check_val(raid, 'FIELD_9', :new.FIELD_9, :old.FIELD_9);
  audit_trail.check_val(raid, 'FIELD_10', :new.FIELD_10, :old.FIELD_10);
  audit_trail.check_val(raid, 'DATE_1', :new.DATE_1, :old.DATE_1);
  audit_trail.check_val(raid, 'DATE_2', :new.DATE_2, :old.DATE_2);
  audit_trail.check_val(raid, 'DATE_3', :new.DATE_3, :old.DATE_3);
  audit_trail.check_val(raid, 'RID', :new.RID, :old.RID);
  --audit_trail.check_val(raid, 'CREATOR', :new.CREATOR, :old.CREATOR);
  --audit_trail.check_val(raid, 'TIMESTAMP', :new.TIMESTAMP, :old.TIMESTAMP);

end;

/
