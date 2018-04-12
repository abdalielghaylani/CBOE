rem cs_security_privileges_bi0.trg
rem Generated on 28-JAN-05
rem Script to create BI audit trigger for the CS_SECURITY_PRIVILEGES table.

create or replace trigger TRG_AUDIT_CS_SEC_PRIV_BI0
  before insert
  on cs_security_privileges
  for each row
  when (new.rid is null or new.rid = 0)

begin
  select trunc(seq_rid.nextval)
  into :new.rid
  from dual;
end;
/

