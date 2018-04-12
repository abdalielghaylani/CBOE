-- cs_security_privileges_au0.trg
-- Generated on 28-JAN-05
-- Script to create AFTER-UPDATE audit trigger for the CS_SECURITY_PRIVILEGES table.

create or replace trigger TRG_AUDIT_CS_SEC_PRIV_AU0
  after update of
  role_internal_id,
  css_login,
  css_create_user,
  css_edit_user,
  css_delete_user,
  css_change_password,
  css_create_role,
  css_edit_role,
  css_delete_role,
  css_create_workgrp,
  css_edit_workgrp,
  css_delete_workgrp,
  rid
  
  on cs_security_privileges
  for each row
  
declare
  raid number(10);

begin

  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'CS_SECURITY_PRIVILEGES', :old.rid, 'U');

  if updating('role_internal_id') then
    if :old.role_internal_id != :new.role_internal_id then
      audit_trail.column_update
        (raid, 'ROLE_INTERNAL_ID',
        :old.role_internal_id, :new.role_internal_id);
    end if;	
  end if; 
  if updating('css_create_user') then 
    if :old.css_create_user != :new.css_create_user then
      audit_trail.column_update
        (raid, 'CSS_CREATE_USER',
        :old.css_create_user, :new.css_create_user);
    end if;      
  end if; 
  if updating('css_edit_user') then 
    if :old.css_edit_user != :new.css_edit_user then
      audit_trail.column_update
        (raid, 'CSS_EDIT_USER',
        :old.css_edit_user, :new.css_edit_user);
    end if;
  end if; 
  if updating('css_delete_user') then 
    if :old.css_delete_user != :new.css_delete_user then
      audit_trail.column_update
        (raid, 'CSS_DELETE_USER',
        :old.css_delete_user, :new.css_delete_user);
    end if;
  end if; 
  if updating('css_change_password') then 
    if :old.css_change_password != :new.css_change_password then
      audit_trail.column_update
        (raid, 'CSS_CHANGE_PASSWORD',
        :old.css_change_password, :new.css_change_password);
    end if;
  end if; 
  if updating('css_create_role') then 
    if :old.css_create_role != :new.css_create_role then
      audit_trail.column_update
        (raid, 'CSS_CREATE_ROLE',
        :old.css_create_role, :new.css_create_role);
    end if;
  end if;
  if updating('css_edit_role') then 
    if :old.css_edit_role != :new.css_edit_role then
      audit_trail.column_update
        (raid, 'CSS_EDIT_ROLE',
        :old.css_edit_role, :new.css_edit_role);
    end if;
  end if;
  if updating('css_delete_role') then 
    if :old.css_delete_role != :new.css_delete_role then
      audit_trail.column_update
        (raid, 'CSS_DELETE_ROLE',
        :old.css_delete_role, :new.css_delete_role);
    end if;
  end if;
  if updating('css_create_workgrp') then 
    if :old.css_create_workgrp != :new.css_create_workgrp then
      audit_trail.column_update
        (raid, 'CSS_CREATE_WORKGRP',
        :old.css_create_workgrp, :new.css_create_workgrp);
    end if;
  end if;
  if updating('css_edit_workgrp') then 
    if :old.css_edit_workgrp != :new.css_edit_workgrp then
      audit_trail.column_update
        (raid, 'CSS_EDIT_WORKGRP',
        :old.css_edit_workgrp, :new.css_edit_workgrp);
    end if;
  end if;
  if updating('css_delete_workgrp') then 
    if :old.css_delete_workgrp != :new.css_delete_workgrp then
      audit_trail.column_update
        (raid, 'CSS_DELETE_WORKGRP',
        :old.css_delete_workgrp, :new.css_delete_workgrp);
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
