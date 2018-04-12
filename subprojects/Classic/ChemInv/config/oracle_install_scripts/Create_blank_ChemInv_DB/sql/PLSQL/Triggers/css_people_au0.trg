-- css_people_au0.trg
-- Generated on 28-JAN-05
-- Script to create AFTER-UPDATE audit trigger for the PEOPLE table.

create or replace trigger TRG_AUDIT_CSS_PEOPLE_AU0
  after update of
  person_id,
  user_code,
  user_id,
  supervisor_internal_id,
  title,
  first_name,
  middle_name,
  last_name,
  site_id,
  department,
  int_address,
  telephone,
  email,
  active,
  rid
  
  on people
  for each row
  
declare
  raid number(10);

begin

  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'PEOPLE', :old.rid, 'U');

  if updating('person_id') then
    if :old.person_id != :new.person_id then
      audit_trail.column_update
        (raid, 'PERSON_ID',
        :old.person_id, :new.person_id);
    end if;	
  end if; 
  if updating('user_code') then 
    if :old.user_code != :new.user_code then
      audit_trail.column_update
        (raid, 'USER_CODE',
        :old.user_code, :new.user_code);
    end if;      
  end if; 
  if updating('user_id') then 
    if :old.user_id != :new.user_id then
      audit_trail.column_update
        (raid, 'USER_ID',
        :old.user_id, :new.user_id);
    end if;
  end if; 
  if updating('supervisor_internal_id') then 
    if :old.supervisor_internal_id != :new.supervisor_internal_id then
      audit_trail.column_update
        (raid, 'SUPERVIOR_INTERNAL_ID',
        :old.supervisor_internal_id, :new.supervisor_internal_id);
    end if;
  end if; 
  if updating('title') then 
    if :old.title != :new.title then
      audit_trail.column_update
        (raid, 'TITLE',
        :old.title, :new.title);
    end if;
  end if; 
  if updating('first_name') then 
    if :old.first_name != :new.first_name then
      audit_trail.column_update
        (raid, 'FIRST_NAME',
        :old.first_name, :new.first_name);
    end if;
  end if;
  if updating('middle_name') then 
    if :old.middle_name != :new.middle_name then
      audit_trail.column_update
        (raid, 'MIDDLE_NAME',
        :old.middle_name, :new.middle_name);
    end if;
  end if;
  if updating('last_name') then 
    if :old.last_name != :new.last_name then
      audit_trail.column_update
        (raid, 'LAST_NAME',
        :old.last_name, :new.last_name);
    end if;
  end if;
  if updating('site_id') then 
    if :old.site_id != :new.site_id then
      audit_trail.column_update
        (raid, 'SITE_ID',
        :old.site_id, :new.site_id);
    end if;
  end if;
  if updating('department') then 
    if :old.department != :new.department then
      audit_trail.column_update
        (raid, 'DEPARTMENT',
        :old.department, :new.department);
    end if;
  end if;
  if updating('int_address') then 
    if :old.int_address != :new.int_address then
      audit_trail.column_update
        (raid, 'INT_ADDRESS',
        :old.int_address, :new.int_address);
    end if;
  end if;
  if updating('telephone') then 
    if :old.telephone != :new.telephone then
      audit_trail.column_update
        (raid, 'TELEPHONE',
        :old.telephone, :new.telephone);
    end if;
  end if;
  if updating('email') then 
    if :old.email != :new.email then
      audit_trail.column_update
        (raid, 'EMAIL',
        :old.email, :new.email);
    end if;
  end if;
  if updating('active') then 
    if :old.active != :new.active then
      audit_trail.column_update
        (raid, 'ACTIVE',
        :old.active, :new.active);
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
