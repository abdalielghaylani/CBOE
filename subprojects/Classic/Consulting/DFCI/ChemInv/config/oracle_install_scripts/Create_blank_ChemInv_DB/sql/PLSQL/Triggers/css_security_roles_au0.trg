-- css_security_roles_au0.trg
-- Generated on 28-JAN-05
-- Script to create AFTER-UPDATE audit trigger for the SECURITY_ROLES table.

create or replace trigger TRG_AUDIT_CSS_SEC_ROLES_AU0
  after update of
  role_id,
  privilege_table_int_id,
  role_name,
  rid
  
  on security_roles
  for each row
  
declare
  raid number(10);

begin

  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'SECURITY_ROLES', :old.rid, 'U');

  if updating('role_id') then
    if :old.role_id != :new.role_id then
      audit_trail.column_update
        (raid, 'ROLE_ID',
        :old.role_id, :new.role_id);
    end if;	
  end if; 
  if updating('privilege_table_int_id') then 
    if :old.privilege_table_int_id != :new.privilege_table_int_id then
      audit_trail.column_update
        (raid, 'PRIVILEGE_TABLE_INT_ID',
        :old.privilege_table_int_id, :new.privilege_table_int_id);
    end if;      
  end if; 
  if updating('role_name') then 
    if :old.role_name != :new.role_name then
      audit_trail.column_update
        (raid, 'ROLE_NAME',
        :old.role_name, :new.role_name);
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
